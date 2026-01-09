let returnsTable;


$(document).ready(function () {
    initReturnsTable();
    $('#sectionId, #returnTypeId, #departmentId, #frequencyId, #authorityId').select2({
        width: '100%',
        dropdownParent: $('#returnPanel')
    });
});

function initReturnsTable() {
    returnsTable = new Tabulator("#returns-register-table", {
    ajaxURL: "/grc/returns/compliance-returns/paged-register",
    paginationMode: "remote",
    filterMode: "remote",
    sortMode: "remote",
    pagination: true,
    paginationSize: 10,
    paginationSizeSelector: [10, 20, 35, 40, 50],
    paginationCounter: "rows",
    ajaxConfig: {
        method: "POST",
        headers: { "Content-Type": "application/json" }
    },
    ajaxContentType: "json",
    paginationDataSent: {
        "page": "page",
        "size": "size",
        "sorters": "sort",
        "filters": "filter"
    },
    paginationDataReceived: {
        "last_page": "last_page",
        "data": "data",
        "total_records": "total_records"
    },
    ajaxRequestFunc: function (url, config, params) {
        return new Promise((resolve, reject) => {
            let requestBody = {
                pageIndex: params.page || 1,
                pageSize: params.size || 10,
                searchTerm: "",
                sortBy: "",
                sortDirection: "Ascending"
            };

            //..sorting
            if (params.sort && params.sort.length > 0) {
                requestBody.sortBy = params.sort[0].field;
                requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
            }

            //..filtering
            if (params.filter && params.filter.length > 0) {
                let filter = params.filter.find(f =>
                    ["reportName", "owner", "department", "authority", "frequency"].includes(f.field)
                );
                if (filter) requestBody.searchTerm = filter.value;
            }

            $.ajax({
                url: url,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(requestBody),
                success: function (response) {
                    resolve(response);
                },
                error: function (xhr, status, error) {
                    console.error("AJAX Error:", error);
                    reject(error);
                }
            });
        });
    },
    ajaxResponse: function (url, params, response) {
        return {
            data: response.data || [],
            last_page: response.last_page || 1,
            total_records: response.total_records || 0
        };
    },
    ajaxError: function (error) {
        console.error("Tabulator AJAX Error:", error);
        alert("Failed to load returns. Please try again.");
    },
    layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "REPORT/RETURN NAME",
                field: "reportName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewReport(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "AUTHORITY", field: "authority", minWidth: 200, frozen: true, headerSort: true },
            { title: "FREQUENCY", field: "frequency", widthGrow: 1, minWidth: 200, frozen: true, headerSort: true },
            { title: "DEPARTMENT", field: "department", minWidth: 200 },
            { title: "ENABLING LAW/REGULATION/GUIDELINE.", field: "article", minWidth: 200 },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteReturn(${rowData.id})">
                            <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                            <span>DELETE</span>
                        </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    //..initialize search
    initReturnSearch();
}

function initReturnSearch() {
   
}

function closeReturnPane() {
    closePanelReturns();
}

$(".action-btn-return-new").on("click", function () {
    openReturnPanel('New Return/Report', {
        id: 0,
        sectionId: 0,
        returnName: '',
        returnTypeId: 0,
        departmentId: 0,
        frequencyId: 'UNKNOWN',
        authorityId: 0,
        isDeleted: false,
        riskAttached:'',
        comments: ''
    }, false);
});

function openReturnPanel(title, record, isEdit) {

    console.log(record);
    //..initialize form fields
    $('#isEdit').val(isEdit);
    $('#returnId').val(record.id);
    $('#sectionId').val(record.sectionId || 0).trigger('change');
    $('#returnName').val(record.returnName || '');
    $('#returnTypeId').val(record.returnTypeId || 0).trigger('change');
    $('#departmentId').val(record.departmentId || 0).trigger('change');
    $('#frequencyId').val(record.frequencyId || 'UNKNOWN').trigger('change');
    $('#authorityId').val(record.authorityId || 0).trigger('change');
    $('#isDeleted').prop('checked', record.isDeleted);
    $('#riskAttached').val(record.riskAttached || '');
    $('#comments').val(record.comments || '');

    //..load dialog window
    closePanelReturns();
    $('#returnTitle').text(title);
    $('#returnOverlay').addClass('active');
    $('#returnPanel').addClass('active');
    $('body').css('overflow', 'hidden');

}

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

    findReport(id)
        .then(record => {
            Swal.close();
            if (record) {
                openReturnPanel('Edit Return/Report', record, true);
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

function findReport(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/returns/compliance-returns/request/${id}`,
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

function closePanelReturns() {
    $('.obligation-panel-overlay').removeClass('active');
    $('#returnPanel').removeClass('active');
}

function deleteReturn(id) {
    alert("Delete return with ID >> " + id);
}

function saveReturn(e) {
    e.preventDefault();
    let isEdit = $('#isEdit').val();
    
    //..build record payload from form
    let recordData = {
        id: Number($('#returnId').val()) || 0,
        returnTypeId: Number($('#returnTypeId').val() || 0),
        sectionId: Number($('#sectionId').val()) || 0,
        returnName: $('#returnName').val(),
        departmentId: Number($('#departmentId').val() || 0),
        frequencyId: Number($('#frequencyId').val() || 'UNKNOWN'),
        authorityId: Number($('#authorityId').val() || 0),
        comments: $('#comments').val(),
        isDeleted: $('#isDeleted').is(':checked') ? true : false,
        riskAttached: $('#riskAttached').val()
    };

    //..validate required fields
    let errors = [];
    if (!recordData.returnName)
        errors.push("Return/report name is required.");

    if (!recordData.comments)
        errors.push("Return/report comments is required.");

    if (!recordData.riskAttached)
        errors.push("Return/report obligation is required.");

    if (recordData.frequencyId === 'UNKNOWN')
        errors.push("Frquency ID is required.");

    if (recordData.authorityId === 0)
        errors.push("Authority ID is required.");

    if (recordData.departmentId === 0)
        errors.push("Department ID is required.");

    if (errors.length > 0) {
        highlightReturnErrorField("#returnName", !recordData.returnName);
        highlightReturnErrorField("#riskAttached", !recordData.riskAttached);
        highlightReturnErrorField("#actComments", !recordData.comments);
        Swal.fire({
            title: "Return/report Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveReturnRecord(isEdit, recordData);
}

function saveReturnRecord(isEdit, payload) {

    let url = isEdit === true || isEdit === "true"
        ? "/grc/returns/compliance-returns/update-return"
        : "/grc/returns/compliance-returns/create-return";

    Swal.fire({
        title: isEdit ? "Updating Return/Report..." : "Saving Return/Report...",
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
            'X-CSRF-TOKEN': getAntiForgeryReturnToken()
        },
        success: function (res) {

            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || (isEdit ? " Return/Report updated successfully" : " Return/Report created successfully"));
            closePanelReturns();

            // reload table
            returnsTable.replaceData();
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

            Swal.fire(isEdit ? "Update  Return/Report" : "Save  Return/Report", errorMessage);
        }
    });
}

//..route to home
$('.action-btn-complianceHome').on('click', function () {
    try {
        window.location.href = '/grc/compliance';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

$("#btnExportReturn").on("click", function () {
    alert("Export list to excell")
});

$(".action-btn-excel").on("click", function () {
    alert("Export list to excell")
});

//..get antiforegery token from meta tag
function getAntiForgeryReturnToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightReturnErrorField(selector, hasError, message) {
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

