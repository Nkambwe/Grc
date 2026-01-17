$(document).ready(function () {
    initPolicyGuidTable();
});

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
            alert("Failed to load policy documents. Please try again.");
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
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewPolicyRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "DOCUMENT TYPE", field: "documentType", widthGrow: 1, minWidth: 200, frozen: true, headerSort: true },
            {
                title: "STATUS",
                field: "documentStatus",
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
            { title: "DOCUMENT OWNER", field: "documentOwner", minWidth: 280 },
            { title: "DEPARTMENT", field: "department", minWidth: 200 },
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
            { title: "APPROVED BY", field: "approvedBy", minWidth: 200 },
            {
                title: "ALIGNED",
                field: "isAligned",
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
                    let rowData = cell.getRow().getData();
                    let value = rowData.isLocked;
                    let locked = value === true ? "disabled" : "";
                    return `<button class="grc-table-btn grc-btn-default grc-task-action ${locked}" ${locked} onclick="lockPolicy(${rowData.id})">
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
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deletePolicy(${rowData.id})">
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

    // Search init
    initPolicyDocSearch();
}

$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

$('.action-btn-policy-new').on('click', function () {
    addPolicyDocument();
});

$('#btnExportPolicy').on('click', function () {
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

let flatpickrInstances = {};

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
        lastReviewDate: "",
        nextReviewDate: "",
        frequencyId: 0,
        documentStatus: 'NONE',
        isAligned: false,
        isLocked: false,
        isApproved: 0,
        approvalDate: "",
        approvedBy: "NONE"
    }, false);
}

function openPolicyDocPanel(title, record, isEdit) {
    
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

    //..use setDate
    const today = new Date();
    if (record.lastReviewDate) {
        flatpickrInstances["lastReviewDate"].setDate(record.lastReviewDate, true, "Y-m-d");
    } else {
        flatpickrInstances["lastReviewDate"].setDate(today, true, "Y-m-d");
    }

    if (record.nextReviewDate) {
        flatpickrInstances["nextReviewDate"].setDate(record.nextReviewDate, true, "Y-m-d");
    } else {
        flatpickrInstances["nextReviewDate"].setDate(today, true, "Y-m-d");
    }

    $('#documentStatus').val(record.documentStatus).trigger('change');
    $('#isAligned').prop('checked', record.isAligned);
    $('#isApproved').val(record.isApproved).trigger('change');

    if (record.approvalDate) {
        flatpickrInstances["approvalDate"].setDate(record.approvalDate, true, "Y-m-d");
    } else {
        flatpickrInstances["approvalDate"].setDate(today, true, "Y-m-d");
    }
    $('#approvedBy').val(record.approvedBy).trigger('change');

    //..lock fields is document is locked
    setPolicyPanelReadOnly(record.isLocked === true);

    //load dialog window
    $('#panelTitle').text(title);
    $('#policyOverlay').addClass('active');
    $('#slidePanel').addClass('active');
}

function savePolicyDocument(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();

    // --- gather form values ---
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        documentName: $('#documentName').val()?.trim(),
        comments: $('#comments').val()?.trim(),
        documentTypeId: Number($('#documentTypeId').val()),
        departmentId: Number($('#departmentId').val()),
        frequencyId: Number($('#frequencyId').val()),
        responsibilityId: Number($('#ownerId').val()),
        documentStatus: $('#documentStatus').val()?.trim(),
        isDeleted: $('#isDeleted').is(':checked') ? true : false,
        lastReview: flatpickrInstances["lastReviewDate"].input.value || null,
        nextReview: flatpickrInstances["nextReviewDate"].input.value || null,
        isAligned: $('#isAligned').val(),
        aligned: $('#isAligned').is(':checked') ? true : false,
        isApproved: Number($('#isApproved').val()),
        approvalDate: flatpickrInstances["approvalDate"].input.value || null,
        approvedBy: $('#approvedBy').val()?.trim()
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

        highlightField("#documentName", !recordData.documentName);
        highlightField("#documentStatus", !recordData.documentStatus);
        highlightField("#comments", !recordData.comments);
        highlightField("#documentTypeId", !recordData.documentTypeId || recordData.documentTypeId === 0);
        highlightField("#frequencyId", !recordData.frequencyId || recordData.frequencyId === 0);
        highlightField("#ownerId", !recordData.ownerId || recordData.ownerId === 0);
        highlightField("#lastReviewDate", !recordData.lastReview || recordData.lastReview === null);
        highlightField("#nextReviewDate", !recordData.nextReview || recordData.nextReview === null);

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
            'X-CSRF-TOKEN': getPolicyAntiForgeryToken()
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

function highlightField(selector, isError) {
    if (isError) $(selector).addClass("input-error");
    else $(selector).removeClass("input-error");
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

function getPolicyAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function setPolicyPanelReadOnly(isLocked) {

    const $form = $("#recordForm");

    // Disable all standard inputs
    $form.find("input, textarea, select").prop("disabled", isLocked);

    // Allow hidden fields
    $form.find("input[type='hidden']").prop("disabled", false);

    // Flatpickr (important!)
    Object.values(flatpickrInstances).forEach(fp => {
        if (!fp) return;
        fp.set("clickOpens", !isLocked);
        fp.input.disabled = isLocked;
    });

    // Disable switches explicitly
    $("#isDeleted, #isAligned").prop("disabled", isLocked);

    // Disable Save button
    $form.find("button[onclick='savePolicyDocument()']")
        .prop("disabled", isLocked)
        .toggleClass("disabled", isLocked);

    // Optional: visual lock state
    $("#slidePanel").toggleClass("locked", isLocked);
}

function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');
    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}

$(document).ready(function () {
    initLastReviewDatePickers();
    initNextReviewDatePickers();
    initApprovalDatePickers();

    $('#documentTypeId, #departmentId, #ownerId, #frequencyId ,#documentStatus, #isApproved, #approvedBy').select2({
        width: '100%',
        dropdownParent: $('#slidePanel')
    });

    $('#recordForm').on('submit', function (e) {
        e.preventDefault();
    });

});


