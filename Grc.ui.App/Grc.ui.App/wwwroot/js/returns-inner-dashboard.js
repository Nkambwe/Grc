const cardColors = {
    "Color1": { bg: "#5C0947", class: "" },
    "Color2": { bg: "#8C0E6C", class: "" },
    "Color3": { bg: "#A61180", class: "" },
    "Color4": { bg: "#C21495", class: "" },
    "Color5": { bg: "#E017AD", class: "" },
    "Color6": { bg: "#F519BD", class: "" },
    "Color7": { bg: "#FFAFF2", class: "" },
};

//..route to home
$('.action-btn-returns-home').on('click', function () {
    try {
        window.location.href = '/grc/returns/compliance-returns/returns-dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

$('#processReviewForm').on('submit', function (e) {
    e.preventDefault();
});

function viewReport(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving Return/Report...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findSubmission(id)
        .then(record => {
            Swal.close();
            if (record) {
                openViewPanel(record);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Return/Report not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Failed to load Return/Report details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findSubmission(id) {

    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/returns/compliance-returns/submissions/retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
            }
        });
    });
}

function openViewPanel(record) {

    $('#submissionId').val(record.id);
    $('#title').val(record.title || '');
    $('#period').val(record.period || '');
    $('#ownerId').val(record.period || '0');
    $('#status').val(record.status || 'UNKNOWN').trigger('change');
    $('#isBreached').prop('checked', record.isBreached);
    $('#submissionBreach').val(record.isBreached ? 'YES': 'NO');
    $('#riskAttached').val(record.riskAttached || '');
    $('#submittedOn').val(record.submittedOn || '');
    $('#comments').val(record.comments || '');
    $('#reason').val(record.reason || '');
    $('#file').val(record.file || '');
    $('#department').val(record.department || '');
    $('#submittedBy').val(record.submittedBy || '');

    if (record.isBreached) {
        $('#breachBox').show();
        $('#riskAttached').addClass('breach-marker');
        $('#period').addClass('breach-marker');
    }

    //..load dialog window
    closeInnerPane();
    $('#returnInnerOverlay').addClass('active');
    $('#returnInnerPanel').addClass('active');
    $('body').css('overflow', 'hidden');
}

function updateSubmission(e) {
    e.preventDefault();

    //..build record payload from form
    let recordData = {
        id: Number($('#submissionId').val()) || 0,
        ownerId: Number($('#ownerId').val()) || 0,
        submissionBreach: $('#submissionBreach').val(), 
        submittedOn: $('#submittedOn').val(),
        isBreached: $('#isBreached').is(':checked') ? true : false,
        comments: $('#comments').val(),
        reason: $('#reason').val(),
        file: $('#file').val(),
        status: $('#status').val() || 'UNKNOWN',
        submittedBy: $('#submittedBy').val()
    };

    console.log(recordData);

    //..validate required fields
    let errors = [];
    if (recordData.submissionBreach === 'YES') {
        if (!recordData.reason)
            errors.push("Reason for breach MUST be provided.");
    }

    if (!recordData.status || recordData.status === "UNKNOWN")
        errors.push("You must select the report status");

    if (!recordData.comments)
        errors.push("Provide a note for the submission");

    if (!recordData.submittedBy)
        errors.push("Provide name for person submitting the report");

    if (errors.length > 0) {
        highlightSubmissionField("#submittedBy", !recordData.submittedBy);
        if (recordData.submissionBreach === 'YES') {
            if (!recordData.reason)
                highlightSubmissionField("#reason", !recordData.reason);
        }

        highlightSubmissionField("#comments", !recordData.comments);
        Swal.fire({
            title: "Submission Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveSubmissionRecord(recordData);
}

function saveSubmissionRecord(payload) {
    let url = "/grc/returns/compliance-returns/submissions/update";
    Swal.fire({
        title: "Updating submission...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getSubmissionToken()
        },
        success: function (res) {
            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || "Return/Report updated successfully")
                .then(() => {
                    closeInnerPane();
                    window.location.reload();
                });
        },
        error: function (xhr, status, error) {
            var errorMessage = error;

            try {
                var response = JSON.parse(xhr.responseText);
                if (response.message) {
                    errorMessage = response.message;
                }
            } catch (e) {
                // If parsing fails, use the default error
                errorMessage = "Unexpected error occurred";
            }

            Swal.fire("Report/Return Submission Update", errorMessage);
        }
    });
}

function closeInnerPane() {
    $('#returnInnerOverlay').removeClass('active');
    $('#returnInnerPanel').removeClass('active');
}

let flatpickrInstances = {};

function initReturnInnerSubmissionDate() {

    flatpickrInstances["submittedOn"] = flatpickr("#submittedOn", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

document.addEventListener('DOMContentLoaded', function () {

    initReturnInnerSubmissionDate();

    // hide breach box
    $('#breachBox').hide();

    $('#status').select2({
        width: '100%',
        dropdownParent: $('#returnInnerPanel')
    });

    const returns = returnsData.Returns.Reports;
    const cardColorList = Object.values(cardColors);

    new Tabulator("#returnsTable", {
        data: returns,
        layout: "fitColumns",
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [10, 25, 50],
        responsiveLayout: "hide",
        height: "100%",

        columns: [
            {
                title: "RETURN / REPORT",
                field: "Title",
                headerFilter: "input"
            },
            {
                title: "FOR PERIOD",
                field: "Period",
                headerFilter: "input"
            },
            {
                title: "STATUS",
                field: "Status",
                headerFilter: "select",
                headerFilterParams: {
                    values: {
                        "": "All",
                        "OPEN": "OPEN",
                        "CLOSED": "CLOSED"
                    }
                },
                hozAlign: "center",
                formatter: function (cell) {
                    const status = cell.getValue();
                    const el = cell.getElement();

                    let bg = "#FF2413"; 
                    if (status === "CLOSED") bg = "#09B831";
                    else if (status === "OPEN") bg = "#FF8503";

                    // color the whole cell
                    el.style.backgroundColor = bg;
                    el.style.color = "#FFFFFF";
                    el.style.fontWeight = "600";
                    el.style.textAlign = "center";

                    return status;
                }
            },
            {
                title: "RISK",
                field: "Risk",
                headerFilter: "input"
            },
            {
                title: "DEPARTMENT",
                field: "Department",
                headerFilter: "input"
            },
            {
                title: "VIEW",
                hozAlign: "center",
                formatter: function (cell) {
                    const row = cell.getRow();
                    const data = row.getData();
                    const index = row.getPosition();
                    const color = cardColorList[index % cardColorList.length].bg;

                    return `
                        <button class="btn btn-category-button"
                                onclick="viewReport('${data.Id}')">
                            <span style="
                                display:inline-block;
                                width:15px;
                                height:15px;
                                border-radius:50%;
                                background-color:${color};">
                            </span>
                            <span style="margin-left:10px;">
                                <i class="mdi mdi-eye"></i>
                            </span>
                        </button>
                    `;
                }
            }
        ]
    });
});

//..get antiforegery token from meta tag
function getSubmissionToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightSubmissionField(selector, hasError, message) {
    const $field = $(selector);
    const $formGroup = $field.closest('.form-group, .mb-3, .col-sm-8');

    // Remove existing error
    $field.removeClass('is-invalid');
    $formGroup.find('.field-error').remove();

    if (hasError) {
        $field.addClass('is-invalid');
        if (message) {
            $formGroup.append(`<div class="field-error text-danger small mt-1">${message}</div>`);
        }
    }
}

