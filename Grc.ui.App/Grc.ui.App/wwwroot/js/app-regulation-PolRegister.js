/*------------------------------------------
    Initialize on document ready
-------------------------------------------*/
$(document).ready(function () {
    initPolicyGuidTable();
    initializePolicyDocTypeSelect2();
    initializePolicyDocsStatusSelect2();
    initializePolicyDocsOwnerSelect2();
});

/*------------------------------------------
    Tabulator Table
-------------------------------------------*/
let policyRegisterTable;

function initPolicyGuidTable() {
    policyRegisterTable = new Tabulator("#regulatory-policy-register-table", {
        ajaxURL: "/grc/compliance/register/policies-all",
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

                // Sorting
                if (params.sort && params.sort.length > 0) {
                    requestBody.sortBy = params.sort[0].field;
                    requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
                }

                // Filtering
                if (params.filter && params.filter.length > 0) {
                    let filter = params.filter.find(f =>
                        ["documentName", "documentType", "documentOwner", "department", "status"].includes(f.field)
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
            alert("Failed to load policies. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "", field: "startTab",
                maxWidth: 50,
                headerSort: false,
                frozen: true,
                frozen: true, formatter: () => `<span class="record-tab"></span>` },
            {
                title: "POLICY/PROCEDURE NAME",
                field: "documentName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewPolicyRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "DOCUMENT TYPE", field: "documentType", widthGrow: 1, minWidth: 300, frozen: true, headerSort: true },
            {
                title: "ALIGNED",
                field: "aligned",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let color = rowData.aligned === "true" || rowData.aligned === true ? "#28a745" : "#dc3545";
                    let text = rowData.aligned === "true" || rowData.aligned === true ? "YES" : "NO";

                    return `<div style="
                            display:flex;
                            align-items:center;
                            justify-content:center;
                            width:100%;
                            height:100%;
                            border-radius:50px;
                            color:${color};
                            font-weight:bold;
                        ">
                ${text}
            </div>`;
                },
                hozAlign: "center",
                headerHozAlign: "center",
                maxWidth: 200
            },
            {
                title: "IN REVIEW",
                field: "reviewStatus",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let value = rowData.reviewStatus;
                    let color = value === "OVERDUE" ? "#FF3E0A" : (value === "DUE" ? "#FF9704" : "#08A11C");
                    let text = value === "OVERDUE" ? "PASSED DUE" : (value === "DUE" ? "DUE" : "UPTODATE");
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                width:100%;
                                height:100%;
                                border-radius:50px;
                                color:${color || "#D6D6D6"};
                                font-weight:bold;">
                                ${text}
                            </div>`;
                },
                hozAlign: "center",
                headerHozAlign: "center",
                maxWidth: 150
            }
,
            { title: "LAST REVIEW", field: "lastReview", minWidth: 300},
            { title: "NEXT REVIEW", field: "nextReview", minWidth: 300},
            {
                title: "TASK",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-default grc-task-action" onclick="deletePolicyRecord(${rowData.id})">
                        <span><i class="mdi mdi-hand-coin" aria-hidden="true"></i></span>
                            <span>ASSIGN TASK</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "LOCK",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-default grc-task-action" onclick="deletePolicyRecord(${rowData.id})">
                            <span><i class="mdi mdi-link-lock" aria-hidden="true"></i></span>
                            <span>LOCKED</span>
                        </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deletePolicyRecord(${rowData.id})">
                        <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                        <span>DELETE</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });

    // Search init
    initPolicyDocSearch();
}

/*------------------------------------------
    Navigation / Buttons
-------------------------------------------*/
$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

$('.action-btn-policy-new').on('click', function () {
    addPolicyDocsRootRecord();
});

$('.action-btn-policy-task').on('click', function () {
    addPolicyTaskRecord();
});


/*------------------------------------------
    Export to Excel
-------------------------------------------*/
$('#btnPolicyDocsExportFiltered').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/policies-export',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Policy_Register.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-policy-export').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/policies-export-full',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Policy_Register_All.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

/*------------------------------------------
    Select2 Initialization
-------------------------------------------*/
function initializePolicyDocTypeSelect2() {
    $(".js-record-doctype").select2({
        width: 'resolve',
        placeholder: 'Select a type...',
        allowClear: true,
        language: { noResults: () => "No type found" }
    });
}

function initializePolicyDocsStatusSelect2() {
    $(".js-record-docstatus").select2({
        width: 'resolve',
        placeholder: 'Select a status...',
        allowClear: true,
        language: { noResults: () => "No status found" }
    });
}

function initializePolicyDocsOwnerSelect2() {
    $(".js-record-docowner").select2({
        width: 'resolve',
        placeholder: 'Select an owner...',
        allowClear: true,
        language: { noResults: () => "No owner found" }
    });
}

/*------------------------------------------
    Add New Record
-------------------------------------------*/
function addPolicyDocsRootRecord() {
    openPolicyDocPanel('New Policy/Procedure', {
        id: 0,
        documentName: '',
        documentType: '',
        documentOwner: '',
        department: '',
        status: 'Active',
        aligned: 'NO',
        lastReview: "",
        nextReview: "",
        comments: ''
    }, false);
}

function addPolicyTaskRecord() {
    $('#panelTitle').text("Policy/Procedure Tasks");
    $('.task-overlay').addClass('active');
    $('#slideTaskPanel').addClass('active');
}

/*------------------------------------------
    OpenSlide Panel
-------------------------------------------*/
function openPolicyDocPanel(title, record, isEdit) {
    
    $('#isEdit').val(isEdit);
    $('#recordId').val(record.id);
    $('#documentName').val(record.documentName || '');
    $('#dpDocumentStatus').val(record.documentStatus).trigger('change');
    $('#dpDocumentType').val(record.documentType).trigger('change');
    $('#isAligned').prop('checked', (record.aligned || '').toString().toUpperCase() === 'true');
    $('#isDeleted').prop('checked', (record.isDeleted || '').toString().toUpperCase() === 'true');
    $('#isLocked').prop('checked', (record.locked || '').toString().toUpperCase() === 'true');
    $('#dpOwner').val(record.documentOwner).trigger('change');
    $('#comments').val(record.comments || '');

    //..use setDate
    const today = new Date();
    if (record.lastReview) {
        flatpickrInstances["lastRevisionDate"].setDate(record.lastReview, true, "Y-m-d");
    } else {
        flatpickrInstances["lastRevisionDate"].setDate(today, true, "Y-m-d");
    }

    if (record.nextReview) {
        flatpickrInstances["nextRevisionDate"].setDate(record.nextReview, true, "Y-m-d");
    } else {
        flatpickrInstances["nextRevisionDate"].setDate(today, true, "Y-m-d");
    }

    $('#panelTitle').text(title);
    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

/*------------------------------------------
    Save Record
-------------------------------------------*/
function savePolRegisterRecord() {
    let isEdit = $('#isEdit').val();
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        documentName: $('#documentName').val(),
        documentStatus: $('#dpDocumentStatus').val(),
        documentType: $('#dpDocumentType').val(),
        isDeleted: $('#isDeleted').is(':checked') ? "true" : "false",
        aligned: $('#isAligned').is(':checked') ? "true" : "false",
        documentOwner: $('#dpOwner').val(),
        lastReview: flatpickrInstances["lastRevisionDate"].input.value || null,
        nextReview: flatpickrInstances["nextRevisionDate"].input.value || null,
        comments: $('#comments').val()
    };

    console.log("Saving Record:", recordData);
    savePolicy(isEdit, recordData);
}

function savePolicy(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/register/policies-update"
        : "/grc/compliance/register/policies-create";

    Swal.fire({
        title: isEdit ? "Updating Policy..." : "Saving Policy...",
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
            'X-CSRF-TOKEN': getPolicyAntiForgeryToken()
        },
        success: function (res) {
            //..lose loader and show success message
            Swal.close();

            if (res && res.data) {
                if (isEdit) {
                    policyRegisterTable.updateData([res.data]);
                } else {
                    policyRegisterTable.addData([res.data], true);
                }
            }

            Swal.fire({
                title: isEdit ? "Updating Policy..." : "Saving Policy...",
                text: res.message || "Saved successfully.",
                timer: 2000,
                showConfirmButton: false
            });

            closePolicyDocumentPanel();
        },
        error: function (xhr) {
            Swal.close();

            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }

            Swal.fire({
                title: isEdit ? "Update Failed" : "Save Failed",
                text: errorMessage
            });
        }
    });
}

/*------------------------------------------
   Close Slide Panel
-------------------------------------------*/

function closePolicyDocumentPanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}
function closePolicyTaskPanel() {
    $('.task-overlay').removeClass('active');
    $('#slideTaskPanel').removeClass('active');
}

/*------------------------------------------
    Delete Record
-------------------------------------------*/
function deletePolicyRecord(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Policy",
        text: "Are you sure you want to delete this policy/procedure?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/grc/compliance/register/policies-delete/${encodeURIComponent(id)}`,
            type: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getPolicyAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Policy/Procedure deleted successfully.");
                    policyRegisterTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function () {
                toastr.error("Request failed.");
            }
        });
    });
}

/*------------------------------------------
    View Record
-------------------------------------------*/
function viewPolicyRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving policy...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findPolicyRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openPolicyDocPanel('Edit Regulatory Policy/Procedure', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Policy/Procedure not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load policy/procedure details.' });
        });
}

function findPolicyRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/policies-retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                } else {
                    resolve(null);
                }
            },
            error: function () {
                reject();
            }
        });
    });
}

/*------------------------------------------
    Search Functionality
-------------------------------------------*/
function initPolicyDocSearch() {
    const searchInput = $('#policySearchbox'); // fixed ID
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                policyRegisterTable.setFilter([
                    [
                        { field: "documentName", type: "like", value: searchTerm },
                        { field: "documentType", type: "like", value: searchTerm },
                        { field: "documentOwner", type: "like", value: searchTerm },
                        { field: "lastReview", type: "like", value: searchTerm },
                        { field: "nextReview", type: "like", value: searchTerm },
                    ]
                ]);
                policyRegisterTable.setPage(1, true);
            } else {
                policyRegisterTable.clearFilter();
            }
        }, 300);
    });
}

/*------------------------------------------
    Date Pickers with global instances
-------------------------------------------*/
let flatpickrInstances = {};

function initializeDatePickers() {
    flatpickrInstances["lastRevisionDate"] = flatpickr("#lastRevisionDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrInstances["nextRevisionDate"] = flatpickr("#nextRevisionDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

$(document).ready(function () {
    initializeDatePickers();
});

//..get antiforegery token from meta tag
function getPolicyAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

