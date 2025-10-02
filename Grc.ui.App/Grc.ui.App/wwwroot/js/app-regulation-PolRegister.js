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
let regulatoryAuthorityTable;

function initPolicyGuidTable() {
    regulatoryAuthorityTable = new Tabulator("#regulatory-policy-register-table", {
        ajaxURL: "/grc/compliance/settings/policies-all",
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
            { title: "", field: "startTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` },
            {
                title: "POLICY/PROCEDURE NAME",
                field: "documentName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewPolicyRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "DOCUMENT TYPE", field: "documentType", minWidth: 200, widthGrow: 4 },
            { title: "DOCUMENT OWNER", field: "documentOwner", minWidth: 200, widthGrow: 4 },
            { title: "DEPARTMENT", field: "department", minWidth: 200, widthGrow: 4 },
            { title: "STATUS", field: "status", hozAlign: "center", headerHozAlign: "center", maxWidth: 200 },
            { title: "ALIGNED", field: "aligned", hozAlign: "center", headerHozAlign: "center", maxWidth: 200 },
            { title: "LAST REVIEW", field: "lastReview" },
            { title: "NEXT REVIEW", field: "nextReview" },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-delete-action" onclick="deletePolicyRecord(${rowData.id})">Delete</button>`;
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

$('.action-btn-new-policy-doc').on('click', function () {
    addPolicyDocsRootRecord();
});

/*------------------------------------------
    Export to Excel
-------------------------------------------*/
$('#btnPolicyDocsExportFiltered').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/policies-export',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryAuthorityTable.getData()),
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

$('.action-btn-policy-docs-export').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/policies-export-full',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryAuthorityTable.getData()),
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

/*------------------------------------------
    Open / Close Slide Panel
-------------------------------------------*/
function openPolicyDocPanel(title, record, isEdit) {
    $('#isEdit').val(isEdit);
    $('#recordId').val(record.id);
    $('#documentName').val(record.documentName || '');
    $('#dpDocumentType').val(record.documentType).trigger('change');
    $('#dpDocumentStatus').val(record.status).trigger('change');
    $('#dpOwner').val(record.documentOwner).trigger('change');
    $('#departmentName').val(record.department || '');
    $('#comments').val(record.comments || '');
    $('#isAligned').prop('checked', record.aligned === 'YES');

    //..use Flatpickr setDate
    if (record.lastReview) {
        flatpickrInstances["lastRevisionDate"].setDate(record.lastReview, true, "Y-m-d");
    } else {
        flatpickrInstances["lastRevisionDate"].clear();
    }

    if (record.nextReview) {
        flatpickrInstances["nextRevisionDate"].setDate(record.nextReview, true, "Y-m-d");
    } else {
        flatpickrInstances["nextRevisionDate"].clear();
    }

    $('#panelTitle').text(title);
    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

function closePolicyDocumentPanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

/*------------------------------------------
    Save Record
-------------------------------------------*/
function savePolRegisterRecord() {
    let isEdit = $('#isEdit').val();

    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        documentName: $('#documentName').val(),
        documentType: $('#dpDocumentType').val(),
        documentOwner: $('#dpOwner').val(),
        status: $('#dpDocumentStatus').val() || "Active",
        aligned: $('#isAligned').is(':checked') ? "YES" : "NO",
        department: $('#departmentName').val(),

        //..flatpickr value in ISO format
        lastReview: flatpickrInstances["lastRevisionDate"].input.value || null,
        nextReview: flatpickrInstances["nextRevisionDate"].input.value || null,

        comments: $('#comments').val()
    };

    savePolicy(isEdit, recordData);
}

function savePolicy(isEdit, payload) {
    let url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/register/policies-update"
        : "/grc/compliance/settings/policies-create";

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        success: function (res) {
            if (res && res.data) {
                if (isEdit) {
                    regulatoryAuthorityTable.updateData([res.data]);
                } else {
                    regulatoryAuthorityTable.addData([res.data], true);
                }
            }
            Swal.fire("Success", res.message || "Saved successfully.");
            closePolicyDocumentPanel();
        },
        error: function (xhr, status, error) {
            let errorMessage = "Unexpected error occurred";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }
            Swal.fire(isEdit ? "Update Policy/Procedure" : "Save Policy/Procedure", errorMessage);
        }
    });
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
            url: `/app-compliance-register-policies-delete/${encodeURIComponent(id)}`,
            type: 'DELETE',
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Policy/Procedure deleted successfully.");
                    regulatoryAuthorityTable.setPage(1, true);
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
                regulatoryAuthorityTable.setFilter([
                    [
                        { field: "documentName", type: "like", value: searchTerm },
                        { field: "documentType", type: "like", value: searchTerm },
                        { field: "documentOwner", type: "like", value: searchTerm },
                        { field: "department", type: "like", value: searchTerm },
                        { field: "status", type: "like", value: searchTerm },
                        { field: "lastReview", type: "like", value: searchTerm },
                        { field: "nextReview", type: "like", value: searchTerm },
                    ]
                ]);
                regulatoryAuthorityTable.setPage(1, true);
            } else {
                regulatoryAuthorityTable.clearFilter();
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

