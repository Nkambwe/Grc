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
$('.action-btn-circular-home').on('click', function () {
    console.log("Home Button clicked");
    try {
        window.location.href = '/grc/returns/circular-returns/circulars-dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

$('#circularForm').on('submit', function (e) {
    e.preventDefault();
})

$('#issueForm').on('submit', function (e) {
    e.preventDefault();
})

function viewCircular(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving Circular...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findCircularSubmission(id)
        .then(record => {
            Swal.close();
            if (record) {
                openCircularPanel(record);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Circular not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Failed to load Circular details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findCircularSubmission(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/returns/circular-returns/submissions/retrieve/${id}`,
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

let flatpickrInstances = {};

function initCircularSubmissionDate() {

    flatpickrInstances["submittedOn"] = flatpickr("#submittedOn", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

function openCircularPanel(record) {
    $('#circularId').val(record.id);
    $('#submissionBreach').val(record.submissionBreach || ''); 
    $('#ownerId').val(record.ownerId || '0');
    $('#circularTitle').val(record.circularTitle || '');
    $('#submittedOn').val(record.submittedOn || '');
    $('#circularRequirement').val(record.circularRequirement || ''); 
    $('#recieveDate').val(record.recievedOn || '');
    $('#deadline').val(record.deadline || '');
    $('#reference').val(record.reference || '');
    $('#circularStatus').val(record.circularStatus || 'UNKNOWN').trigger('change');
    $('#isBreached').prop('checked', record.isBreached);
    $('#breachReason').val(record.breachReason || '');
    $('#breachRisk').val(record.breachRisk || '');
    $('#comments').val(record.comments || '');
    $('#filePath').val(record.filePath || '');
    $('#department').val(record.department || '');
    $('#frequency').val(record.frequency || '');
    $('#submittedBy').val(record.submittedBy || '');

    if (record.isBreached) {
        $('#breachBox').show();
        $('#breachReason').addClass('breach-marker');
    }

    //..load issues
    renderCircularIssues(record.issues);

    //..load dialog window
    closeCircular();
    $('#circularOuterOverlay').addClass('active');
    $('#circularOuterPanel').addClass('active');
    $('body').css('overflow', 'hidden');
}

function renderCircularIssues(issues) {
    const $list = $('.issues-list');
    $list.empty();

    if (issues.length === 0) {
        $list.append('<li class="text-muted no-control-items">No issues reported</li>');
        return;
    }

    issues.forEach(issue => {
        const isChecked = true;
        const nameClass = issue.isDeleted ? 'text-decoration-line-through' : '';

        const listItem = `
            <li class="control-item mb-2">
                <div class="form-check">
                    <input class="form-check-input" 
                           type="checkbox" 
                           id="item_${issue.id}" 
                           data-item-id="${issue.id}"
                           ${isChecked ? 'checked' : ''}>
                    <label class="form-check-label ${nameClass}" for="item_${issue.id}">
                        ${issue.description}
                    </label>
                </div>
                <div class="text-muted small">
                    <button type="button" 
                            class="grc-table-btn grc-view-action grc-edit-issue" 
                            data-id="${issue.id}">
                        <span><i class="mdi mdi-pencil-outline" aria-hidden="true"></i></span>
                    </button>
                </div>
            </li>`;
        $list.append(listItem);
    });

    //..attach handler
    attachEditIssueHandlers();
}

function updateSubmission(e) {
    e.preventDefault();
    //..build record payload from form
    let recordData = {
        circularId: Number($('#circularId').val()) || 0,
        departmentId: Number($('#ownerId').val()) || 0,
        submissionBreach: $('#submissionBreach').val(),
        reference: $('#reference').val(),
        isBreached: $('#isBreached').is(':checked') ? true : false,
        comments: $('#comments').val(),
        breachReason: $('#breachReason').val(),
        filePath: $('#filePath').val(),
        circularStatus: $('#circularStatus').val() || 'UNKNOWN',
        submittedBy: $('#submittedBy').val()
    };

    console.log(recordData);

    //..validate required fields
    let errors = [];
    if (recordData.submissionBreach === 'YES') {
        if (!recordData.breachReason)
            errors.push("Reason for breach MUST be provided.");
    }

    if (!recordData.circularStatus || recordData.circularStatus === "UNKNOWN")
        errors.push("You must select the report status");

    if (!recordData.comments)
        errors.push("Provide a note for the submission");

    if (!recordData.submittedBy)
        errors.push("Provide name for person submitting the report");

    if (errors.length > 0) {
        highlightCircularField("#submittedBy", !recordData.submittedBy);
        if (recordData.submissionBreach === 'YES') {
            if (!recordData.breachReason)
                highlightCircularField("#reason", !recordData.reason);
        }

        highlightCircularField("#comments", !recordData.comments);
        Swal.fire({
            title: "Submission Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveCircularRecord(recordData);
}

function saveCircularRecord(payload) {
    let burl = "/grc/returns/circular-returns/submissions/update";
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
        url: burl,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getCircularToken()
        },
        success: function (res) {
            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || "Circular updated successfully")
                .then(() => {
                    closeCircular();
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

            Swal.fire("Circular Submission Update", errorMessage);
        }
    });
}

function closeCircular() {
    $('#circularOuterOverlay').removeClass('active');
    $('#circularOuterPanel').removeClass('active');
}

document.addEventListener('DOMContentLoaded', function () {

    initCircularSubmissionDate();

    //..hide breach box
    $('#breachBox').hide();

    $('#circularStatus').select2({
        width: '100%',
        dropdownParent: $('#circularOuterPanel')
    });

    $('#issueStatus').select2({
        width: '100%',
        dropdownParent: $('#issuePanel')
    });

    const circulars = circularData.Circulars.Reports;;
    const cardColorList = Object.values(cardColors);
    new Tabulator("#circularTable", {
        data: circulars,
        layout: "fitColumns",
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [10, 25, 50],
        responsiveLayout: "hide",
        height: "100%",

        columns: [
            {
                title: "CIRCULAR TITLE",
                field: "Title",
                headerFilter: "input"
            },
            {
                title: "SUBMISSION DATE",
                field: "RequiredDate",
                hozAlign: "center",
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
                title: "ASSOCIATED RISK",
                field: "BreachRisk",
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
                                onclick="viewCircular('${data.Id}')">
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
function getCircularToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightCircularField(selector, hasError, message) {
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