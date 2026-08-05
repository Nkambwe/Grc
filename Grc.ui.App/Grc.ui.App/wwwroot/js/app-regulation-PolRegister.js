
let policyRegisterTable;

let flatpickrInstances = {};

//..check permissions
window.hasPermission = function (permissionName) {
    return window.userPermissions.some(p => p.toLowerCase() === permissionName.toLowerCase());
};

//..check if user has ANY of the permissions
window.hasAnyPermission = function (...permissionNames) {
    return permissionNames.some(p => window.hasPermission(p));
};

//..check if user has ALL of the permissions
window.hasAllPermissions = function (...permissionNames) {
    return permissionNames.every(p => window.hasPermission(p));
};

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
                        //..hide permission alert
                        $('#permissionAlert').hide();

                        if (xhr.status === 401) {
                            window.location = "/login/userlogin";
                        }

                        if (xhr.status === 403) {
                            $('#permissionAlert').show();

                            //..return empty dataset
                            resolve({
                                data: [],
                                last_page: 1,
                                total_records: 0
                            });

                            return;
                        }

                        reject(error);
                        return;
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
            
            //..hide permission alert
            $('#permissionAlert').hide();
        
            //..determine error message
            let errorMessage = "Failed to load data. Please try again.";
        
            if (error.status === 403) {
                //..permission error,show permission alert instead
                $('#permissionAlert').show();
            } else if (error.status === 404) {
                errorMessage = "The requested resource was not found.";
                $('#errorAlertMessage').text(errorMessage);
                $('#errorAlert').show();
            } else if (error.status === 500) {
                errorMessage = "Server error occurred. Please contact support.";
                $('#errorAlertMessage').text(errorMessage);
                $('#errorAlert').show();
            } else if (error.status === 0) {
                errorMessage = "Network error. Please check your connection.";
                $('#errorAlertMessage').text(errorMessage);
                $('#errorAlert').show();
            } else {
                //..generic error - show error alert
                $('#errorAlertMessage').text(errorMessage);
                $('#errorAlert').show();
            }
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "POLICY/PROCEDURE NAME",
                field: "documentName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                headerFilter: "input",
                frozen: true,
                formatter: function (cell) {
                    //..if user has permission to view/edit
                    if (hasPermission("EditRegulationAndGuides")) {
                        return  `<span class="clickable-title" onclick="viewPolicyRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                    } else {
                        return `<span >${cell.getValue()}</span>`
                    }
                }
            },
            {
                title: "DOCUMENT TYPE",
                field: "documentType",
                widthGrow: 1,
                minWidth: 200,
                frozen: true,
                headerFilter: "input",
                headerSort: true
            },
            {
                title: "STATUS",
                field: "documentStatus",
                headerFilter: "select",
                headerFilterParams: {
                    values: {
                        "": "All",
                        "ON-HOLD": "ON HOLD",
                        "PENDING-BOARD": "PENDING BOARD APPROVAL",
                        "DEPT-REVIEW": "PENDING DEP'T APPROVAL",
                        "DUE": "DUE REVIEW",
                    }
                },
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();

                    // Default color
                    let bg = "#DCF5DB";
                    let clr = "#FFFFFF";
                    if (value === "UPTODATE") {
                        bg = "#28C232";
                    }
                    else if (value === "ON-HOLD") {
                        bg = "#C2B70B";
                    }
                    else if (value === "PENDING-BOARD") {
                        bg = "#F5BA0B";
                    }
                    else if (value === "DEPT-REVIEW") {
                        bg = "#F57809";
                    }
                    else if (value === "DUE") {
                        bg = "#F50C0C";
                    }
                    else{
                        bg = "#DCF5DB";
                        clr = "#191C19";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = clr;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 200,
                headerSort: true
            },
            { title: "DOCUMENT OWNER", field: "documentOwner", minWidth: 280, headerFilter: "input" },
            { title: "DEPARTMENT", field: "department", minWidth: 200, headerFilter: "input" },
            {
                title: "LAST REVIEW",
                field: "lastReview",
                minWidth: 200,
                formatter: function (cell) {
                    const value = cell.getValue();
                    if (!value) return "";

                    const d = new Date(value);
                    const day = String(d.getDate()).padStart(2, "0");
                    const month = String(d.getMonth() + 1).padStart(2, "0");
                    const year = d.getFullYear();

                    return `${day}-${month}-${year}`;
                }
            },
            {
                title: "NEXT REVIEW",
                field: "nextReview",
                headerFilter: "input",
                minWidth: 200,
                formatter: function (cell) {
                    const value = cell.getValue();
                    if (!value) return "";

                    const d = new Date(value);
                    const day = String(d.getDate()).padStart(2, "0");
                    const month = String(d.getMonth() + 1).padStart(2, "0");
                    const year = d.getFullYear();

                    return `${day}-${month}-${year}`;
                }
            },
            { title: "APPROVED BY", field: "approvedBy", minWidth: 200, headerFilter: "input", },
            {
                title: "ALIGNED",
                field: "isAligned",
                headerFilter: "input",
                formatter: function (cell) {
                    const cellEl = cell.getElement();

                    let rowData = cell.getRow().getData();
                    let text = rowData.isAligned === true ? "YES" : "NO";

                    let bg = "#DCF5DB";
                    let clr = "#FFFFFF";
                    if (rowData.isAligned === true) {
                        bg = "#28C232";
                    } else {
                        bg = "#F50C0C";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = clr;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return text;
                },
                hozAlign: "center",
                headerHozAlign: "center",
                maxWidth: 200
            },
            {
                title: "LOCK",
                field: "isLocked",
                 formatter: function (cell) {
                  
                   
                     if (hasPermission("CANLOCKPOLICYDOCUMENT")) { 
                        let rowData = cell.getRow().getData();
                        let value = rowData.isLocked;
                        let locked = value === true ? "disabled" : "";
                        return `<button class="grc-table-btn grc-btn-default grc-task-action ${locked}" ${locked} onclick="lockPolicy(${rowData.id})">
                                <span><i class="mdi mdi-link-lock" aria-hidden="true"></i></span>
                                <span>LOCKED</span>
                        </button>`;
                    } else {
                             return `<button class="grc-table-btn grc-btn-default grc-task-action disabled" disabled>
                                 <span><i class="mdi mdi-link-lock" aria-hidden="true"></i></span>
                                 <span>LOCKED</span>
                            </button>`; 
                    }
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

                     if (hasPermission("DeleteRegulationAndGuides")) { 
                         return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deletePolicy(${rowData.id})">
                            <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                            <span>DELETE</span>
                        </button>`;
                    } else {
                             return `<button class="grc-table-btn grc-btn-delete grc-delete-action disabled" disabled>
                                <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                                <span>DELETE</span>
                            </button>`; 
                    }
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initPolicyDocSearch();
}

$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

$('.action-btn-policy-new').on('click', function () {
    addPolicyDocument();
});

$('.action-btn-pol-report-all').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/policies/export/all',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "All_Policies.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-pol-report-summery').on('click', function () {
  $.ajax({
        url: '/grc/compliance/register/policies/export/all-summery',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "All_Policies_Summery.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-pol-report-review').on('click', function () {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/grc/compliance/register/policies/export/reviews');
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.responseType = 'blob';

    xhr.onload = function () {
        if (xhr.status === 200) {
            // Check if it's JSON (error) or Excel file
            var contentType = xhr.getResponseHeader('content-type');

            if (contentType && contentType.includes('application/json')) {
                // It's an error response
                var reader = new FileReader();
                reader.onload = function (e) {
                    try {
                        var response = JSON.parse(e.target.result);
                        toastr.error(response.message || "Export failed. Please try again.");
                    } catch (parseError) {
                        toastr.error("Export failed. Please try again.");
                    }
                };
                reader.readAsText(xhr.response);
            } else {
                // It's the Excel file
                let link = document.createElement('a');
                link.href = window.URL.createObjectURL(xhr.response);
                link.download = "All_Policies_Review.xlsx";
                link.click();
            }
        } else {
            // HTTP error (400, 500, etc.)
            var reader = new FileReader();
            reader.onload = function (e) {
                try {
                    var response = JSON.parse(e.target.result);
                    toastr.error(response.message || "Export failed. Please try again.");
                } catch (parseError) {
                    toastr.error("Export failed. Please try again.");
                }
            };
            reader.readAsText(xhr.response);
        }
    };

    xhr.onerror = function () {
        toastr.error("Export failed. Please try again.");
    };

    xhr.send(JSON.stringify(policyRegisterTable.getData()));
});

$('.action-btn-pol-report-uptodate').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/policies/export/uptodate',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "All_Policies_Uptodate.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-pol-report-due').on('click', function () {
   $.ajax({
        url: '/grc/compliance/register/policies/export/due',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "All_Policies_Due.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-pol-report-board').on('click', function () {
   $.ajax({
        url: '/grc/compliance/register/policies/export/bod',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "BOD_Policies.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-pol-report-board-summery').on('click', function () {
  $.ajax({
        url: '/grc/compliance/register/policies/export/bod-summery',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "BOD_Policies_Summery.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-pol-report-smt').on('click', function () {
   $.ajax({
        url: '/grc/compliance/register/policies/export/smt',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "SMT_Policies.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
   });
});

$('.action-btn-pol-report-smt-summery').on('click', function () {
   $.ajax({
        url: '/grc/compliance/register/policies/export/smt-summery',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyRegisterTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "SMT_Policies_Summery.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
   });
});

function initLastReviewDatePickers() {
    flatpickrInstances["lastReviewDate"] = flatpickr("#lastReviewDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

function initNextReviewDatePickers() {
    flatpickrInstances["nextReviewDate"] = flatpickr("#nextReviewDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

function initApprovalDatePickers() {
    flatpickrInstances["approvalDate"] = flatpickr("#approvalDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

function addPolicyDocument() {
    openPolicyDocPanel('New Policy/Procedure', {
        id: 0,
        documentName: '',
        comments: '',
        documentTypeId: 0,
        ownerId: 0,
        departmentId: 0,
        isDeleted: false,
        lastReviewDate: '',
        nextReviewDate: '',
        frequencyId: 0,
        documentStatus: 'NONE',
        isAligned: false,
        needBoardApproval: false,
        needMcrApproval: false,
        onIntranet:false,
        isLocked: false,
        isApproved: 0,
        approvalDate: '',
        approver: 'NONE',
        interval: '0',
        intervalType: 'NA',
        reminderMessage:''
    }, false);
}

function openPolicyDocPanel(title, record, isEdit) {

    console.log("Document", record);
    $('#isEdit').val(isEdit);
    $('#recordId').val(record.id);
    $('#documentName').val(record.documentName || '');
    $('#comments').val(record.comments || '');
    $('#documentStatus').val(record.documentStatus).trigger('change');
    $('#documentTypeId').val(record.documentTypeId).trigger('change');
    $('#departmentId').val(record.departmentId).trigger('change');
    $('#frequencyId').val(record.frequencyId).trigger('change');
    $('#ownerId').val(record.ownerId).trigger('change');
    $('#isDeleted').prop('checked', record.isDeleted);
    $('#intervalType').val(record.intervalType).trigger('change');
    $('#interval').val(record.interval || '0');
    $('#reminderMessage').val(record.reminderMessage || '');

    //..use setDate
    //const today = new Date();
    //if (record.lastReviewDate) {
    //    flatpickrInstances["lastReviewDate"].setDate(record.lastReviewDate, true, "Y-m-d");
    //} else {
    //    flatpickrInstances["lastReviewDate"].setDate(today, true, "Y-m-d");
    //}

    //if (record.nextReview) {
    //    flatpickrInstances["nextReviewDate"].setDate(record.nextReview, true, "Y-m-d");
    //} else {
    //    flatpickrInstances["nextReviewDate"].setDate(today, true, "Y-m-d");
    //}

    $('#documentStatus').val(record.documentStatus).trigger('change');
    $('#isAligned').prop('checked', record.isAligned);
    $('#isApproved').val(record.isApproved).trigger('change');
    $('#needMcrApproval').val(record.mcrApproval).trigger('change');
    $('#needBoardApproval').val(record.boardApproval).trigger('change');
    $('#onIntranet').val(record.onIntranet).trigger('change');

    //if (record.approvalDate) {
    //    flatpickrInstances["approvalDate"].setDate(record.approvalDate, true, "Y-m-d");
    //} else {
    //    flatpickrInstances["approvalDate"].setDate(today, true, "Y-m-d");
    //}

    $('#approver').val(record.approver).trigger('change');

    //..lock fields is document is locked
    setPolicyPanelReadOnly(record.isLocked === true);

    //..show lock box
    if (isEdit) {
        $('#lockSection').show();
    } else {
        $('#lockSection').hide();
    }

    //load dialog window
    $('#panelTitle').text(title);
    $('#policyOverlay').addClass('active');
    $('#slidePanel').addClass('active');
}

function savePolicyDocument(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();
    let documentName = $('#documentName').val()?.trim();
    let comments = $('#comments').val().trim();
    let interval = $('#interval').val()?.trim();
    let reminderMessage =  $('#reminderMessage').val()?.trim();

    let isValid = true;
    if (!documentName) {
        highlightRegualtionField('#documentName', true, 'Document name is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(documentName)) {
        highlightRegualtionField('#documentName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!comments) {
        highlightRegualtionField('#comments', true, 'Document comment field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(comments)) {
        highlightRegualtionField('#comments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }
    
    if (!interval && !/^[a-zA-Z0-9]*$/.test(interval)) {
        highlightRegualtionField('#interval', true, 'Only letters and numbers allowed');
        isValid = false;
    }
    
    if (!reminderMessage && !/^[a-zA-Z0-9\s,.]*$/.test(reminderMessage)){
        highlightRegualtionField('#reminderMessage', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return; 
    }

    //...gather form values ---
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        documentName: documentName,
        comments: comments,
        documentTypeId: Number($('#documentTypeId').val()),
        departmentId: Number($('#departmentId').val()),
        frequencyId: Number($('#frequencyId').val()),
        responsibilityId: Number($('#ownerId').val()),
        documentStatus: $('#documentStatus').val()?.trim(),
        isDeleted: $('#isDeleted').is(':checked') ? true : false,
        boardApproval: $('#needBoardApproval').is(':checked') ? true : false,
        mcrApproval: $('#needMcrApproval').is(':checked') ? true : false,
        onIntranet: $('#onIntranet').is(':checked') ? true : false,
        lastReview: flatpickrInstances["lastReviewDate"].input.value || null,
        nextReview: flatpickrInstances["nextReviewDate"].input.value || null,
        isAligned: $('#isAligned').is(':checked') ? true : false,
        isApproved: Number($('#isApproved').val()),
        approvalDate: flatpickrInstances["approvalDate"].input.value || null,
        approver: $('#approver').val()?.trim(),
        interval: interval,
        intervalType: $('#intervalType').val()?.trim(),
        reminderMessage: reminderMessage
    };
   
    // --- validate required fields ---
    let errors = [];

    if (!recordData.documentName)
        errors.push("Document name is required.");

    if (!recordData.comments)
        errors.push("Document notes is required.");

    if (!recordData.documentStatus)
        errors.push("Document status is required");

    if (!recordData.documentTypeId || recordData.documentTypeId === 0)
        errors.push("Document type is require");

    if (!recordData.responsibilityId || recordData.responsibilityId === 0)
        errors.push("Designation of person responsible is required.");

    if (!recordData.frequencyId || recordData.frequencyId === 0)
        errors.push("Review Period is required.");

    //..date validation
    if (!recordData.lastReview || recordData.lastReview === null)
        errors.push("Last review date is required.");

    //..date validation
    if (!recordData.nextReview || recordData.nextReview === null)
        errors.push("Next review date is required.");

    // --- stop if validation fails ---
    if (errors.length > 0) {

        highlightRegualtionField("#documentName", !recordData.documentName);
        highlightRegualtionField("#documentStatus", !recordData.documentStatus);
        highlightRegualtionField("#comments", !recordData.comments);
        highlightRegualtionField("#documentTypeId", !recordData.documentTypeId || recordData.documentTypeId === 0);
        highlightRegualtionField("#frequencyId", !recordData.frequencyId || recordData.frequencyId === 0);
        highlightRegualtionField("#ownerId", !recordData.ownerId || recordData.ownerId === 0);
        highlightRegualtionField("#lastReviewDate", !recordData.lastReview || recordData.lastReview === null);
        highlightRegualtionField("#nextReviewDate", !recordData.nextReview || recordData.nextReview === null);

        Swal.fire({
            title: "Document Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    console.log("Valid Record:", recordData);
    savePolicy(isEdit, recordData);
}

function savePolicy(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/register/policies-update"
        : "/grc/compliance/register/policies-create";

    Swal.fire({
        title: isEdit ? "Updating Policy document..." : "Saving Policy document...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    //..debugging
    console.log("Sending data to server:", JSON.stringify(payload));  
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getPolicyAnti2ForgeryToken()
        },
        success: function (res) {
            //..lose loader and show success message
            Swal.close();
            if (!res.success) {
                //..error from the server
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

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

            closePolicyPanel();
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

//..name input validation
function validateDocNameInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }
    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightRegualtionField('#documentName', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightRegualtionField('#documentName', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightRegualtionField('#documentName', false);
        return true;
    }

}

function handleDocNamePaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightRegualtionField('#documentName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightRegualtionField('#documentName', false);
    }
}

//..comments input validation
function validateDocCommentInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightRegualtionField('#comments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightRegualtionField('#comments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightRegualtionField('#comments', false);
        return true;
    }

}

function handleDocCommentPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightRegualtionField('#comments', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightRegualtionField('#comments', false);
    }
}

//..interval input validation
function validateIntervalInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[a-zA-Z0-9]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightRegualtionField('#interval', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightRegualtionField('#interval', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightRegualtionField('#interval', false);
        return true;
    }

}

function handleIntervalPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightRegualtionField('#interval', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightRegualtionField('#interval', false);
    }
}

//..message input validation
function validateReminderInput(event) {
      var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightRegualtionField('#reminderMessage', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightRegualtionField('#reminderMessage', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightRegualtionField('#reminderMessage', false);
        return true;
    }

}

function handleReminderPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightRegualtionField('#reminderMessage', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightRegualtionField('#reminderMessage', false);
    }
}

function highlightRegualtionField(selector, hasError, message) {
    const $field = $(selector);
    const $formGroup = $field.closest('.form-group, .mb-3, .col-sm-8, .field-group');

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

function closePolicyPanel() {
    $('#policyOverlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

function deletePolicy(id) {
    if (!id && id !== 0) {
        Swal.fire({
            title: "Delete Policy",
            text: "Policy document id is required",
            showCancelButton: false,
            okButtonText: "Ok"
        })
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
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getPolicyAnti2ForgeryToken()
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

function lockPolicy(id) {
    if (!id && id !== 0) {
        toastr.error("Policy document ID is required.");
        return;
    }

    Swal.fire({
        title: "Lock Policy",
        text: "Locking document makes it uneditable. Do you want to lock the document?",
        showCancelButton: true,
        confirmButtonColor: "#A10E7B",
        confirmButtonText: "Lock",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/grc/compliance/register/policies-lock/${encodeURIComponent(id)}`,
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getPolicyAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Policy/Procedure locked successfully.");
                    policyRegisterTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Failed to lock document.");
                }
            },
            error: function () {
                toastr.error("Request failed.");
            }
        });
    });
}

function viewPolicyRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving policy document...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    console.log("ID >> " + id);
    findPolicyRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openPolicyDocPanel('Edit Policy document', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Policy document not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load policy document details.' });
        });
}

function findPolicyRecord(id) {
    console.log("Retrieve  document record with id >> " + id);
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
                        { field: "approvedBy", type: "like", value: searchTerm }
                    ]
                ]);
                policyRegisterTable.setPage(1, true);
            } else {
                policyRegisterTable.clearFilter();
            }
        }, 300);
    });
}

function getPolicyAnti2ForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function setPolicyPanelReadOnly(isLocked) {

    const $form = $("#recordForm");

    //..disable all standard inputs
    $form.find("input:not(#isLocked), textarea, select").prop("disabled", isLocked);

    //..allow hidden fields
    $form.find("input[type='hidden']").prop("disabled", false);

    //..flatpickr
    Object.values(flatpickrInstances).forEach(fp => {
        if (!fp) return;
        fp.set("clickOpens", !isLocked);
        fp.input.disabled = isLocked;
    });

    //..disable switches explicitly
    $("#isDeleted, #isAligned").prop("disabled", isLocked);

    //..disable Save button
    $form.find("button[onclick='savePolicyDocument()']")
        .prop("disabled", isLocked)
        .toggleClass("disabled", isLocked);
}

function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');
    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}

function lockPolicyDocument(id, isLocked) {
    let payload = {
        id: id,
        isLocked: isLocked
    };

    const url = "/grc/compliance/register/policies-lock";
    Swal.fire({
        title: isLocked ? "Locking policy document record..." : "Unlocking policy document record...",
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
            'X-CSRF-TOKEN': getPolicyAnti2ForgeryToken()
        },
        success: function (res) {
            Swal.close();
            if (!res.success) {
                //..error from the server
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            if (res && res.data) {
                if (isEdit) {
                    policyRegisterTable.updateData([res.data]);
                } else {
                    policyRegisterTable.addData([res.data], true);
                }
            }

            Swal.fire({
                title: isLocked ? "Locking policy document" : "Unlocking policy document",
                text: res.message || "Saved successfully.",
                timer: 2000,
                showConfirmButton: false
            });

            closePolicyPanel();
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

$(document).ready(function () {
    initPolicyGuidTable();
    initLastReviewDatePickers();
    initNextReviewDatePickers();
    initApprovalDatePickers();

    //console.log(window.userPermissions);

    $('#documentTypeId, #departmentId, #ownerId, #frequencyId ,#documentStatus, #isApproved, #approver, #intervalType').select2({
        width: '100%',
        dropdownParent: $('#slidePanel')
    });

    $('#recordForm').on('submit', function (e) {
        e.preventDefault();
    });

    //..render document readonly
    const $isLocked = $('#isLocked');

    //..initial state
    setPolicyPanelReadOnly($isLocked.prop('checked'));

    //..toggle on change
    $isLocked.on('change', function () {
        const isLocked = this.checked;
        const id = $('#recordId').val();

        setPolicyPanelReadOnly(isLocked);
        lockPolicyDocument(id, isLocked);
    });

    //..category name validation 
    $('#documentName').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightRegualtionField('#documentName', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightRegualtionField('#documentName', true, 'Invalid characters detected');
        } else {
            highlightRegualtionField('#documentName', false);
        }
    });

    $('#documentName').on('focus', function () {
        highlightRegualtionField('#documentName', false);
    });

    $('#documentName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightRegualtionField('#documentName', true, 'Document name is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightRegualtionField('#documentName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightRegualtionField('#documentName', false);
        }
    });

    $('#documentName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightRegualtionField('#documentName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightRegualtionField('#documentName', false);
                }, 2000);
            }
        }
    });

    //..comments validation 
    $('#comments').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightRegualtionField('#comments', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightRegualtionField('#comments', true, 'Invalid characters detected');
        } else {
            highlightRegualtionField('#comments', false);
        }
    });

    $('#comments').on('focus', function () {
        highlightRegualtionField('#comments', false);
    });

    $('#comments').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightRegualtionField('#comments', true, 'Comments is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightRegualtionField('#comments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightRegualtionField('#comments', false);
        }
    });

    $('#comments').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightRegualtionField('#comments', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightRegualtionField('#comments', false);
                }, 2000);
            }
        }
    });
    
    //..interval validation 
    $('#interval').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightRegualtionField('#interval', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightRegualtionField('#interval', true, 'Invalid characters detected');
        } else {
            highlightRegualtionField('#interval', false);
        }
    });

    $('#interval').on('focus', function () {
        highlightRegualtionField('#interval', false);
    });

    $('#interval').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightRegualtionField('#interval', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightRegualtionField('#interval', false);
        }
    });

    $('#interval').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightRegualtionField('#interval', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightRegualtionField('#interval', false);
                }, 2000);
            }
        }
    });

    //..message validation 
    $('#reminderMessage').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightRegualtionField('#reminderMessage', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightRegualtionField('#reminderMessage', true, 'Invalid characters detected');
        } else {
            highlightRegualtionField('#reminderMessage', false);
        }
    });

    $('#reminderMessage').on('focus', function () {
        highlightRegualtionField('#reminderMessage', false);
    });

    $('#reminderMessage').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightRegualtionField('#reminderMessage', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightRegualtionField('#reminderMessage', false);
        }
    });

    $('#reminderMessage').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightRegualtionField('#reminderMessage', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightRegualtionField('#reminderMessage', false);
                }, 2000);
            }
        }
    });
});


