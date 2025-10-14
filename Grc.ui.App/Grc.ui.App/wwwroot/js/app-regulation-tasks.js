/*------------------------------------------
    Initialize on document ready
-------------------------------------------*/
$(document).ready(function () {
    initPolicyTaskTable();
    initializeTaskDatePickers();
    initializeTaskOwnerSelect2();
    initializeTaskNewSelect2();

    loadTaskOwners();
    loadTaskDocuments();
});


/*------------------------------------------
    Date Pickers with global instances
-------------------------------------------*/
let flatpickrTaskInstances = {};

function initializeTaskDatePickers() {
    flatpickrTaskInstances["assignedDate"] = flatpickr("#assignedDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrTaskInstances["dueDate"] = flatpickr("#dueDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrTaskInstances["startDate"] = flatpickr("#startDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

/*------------------------------------------
    Tabulator Table
-------------------------------------------*/
let policyTaslTable;

function initPolicyTaskTable() {
    policyTaslTable = new Tabulator("#regulatory-policy-task-table", {
        ajaxURL: "/grc/compliance/register/tasks-all",
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
                        ["documentName", "taskDescription", "assigneeName", "department", "assignDate"].includes(f.field)
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
            alert("Failed to load tasks. Please try again.");
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
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewPolicyRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "TASK DESCRIPTION", field: "taskDescription", widthGrow: 1, minWidth: 300, frozen: true, headerSort: false },
            { title: "ASSIGNED TO", field: "assigneeName", minWidth: 300 },
            { title: "ASSIGNED DATE", field: "assignDate", minWidth: 100 },
            { title: "DUE DATE", field: "dueDate", minWidth: 100 },
            {
                title: "TASK STATUS",
                field: "taskStatus",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let value = rowData.taskStatus;
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
                maxWidth: 200
            },
            { title: "LAST REMINDER", field: "lastReminder", minWidth: 100 },
            { title: "NEXT REMINDER", field: "nextReminder", minWidth: 100 },
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
    initTaskSearch();
}

/*------------------------------------------
    Navigation / Buttons
-------------------------------------------*/
$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

$('.action-btn-policy-task').on('click', function () {
    addPolicyTaskRecord();
})

$('.action-btn-departments-new').on('click', function () {

});

$('.action-btn-responsibility-new').on('click', function () {
  
});

$('.action-btn-policy-task-mail').on('click', function () {
    
});

/*------------------------------------------
    Export to Excel
-------------------------------------------*/
$('#btnPolicyTaskExportFiltered').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/tasks-export',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyTaslTable.getData()),
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

$('.action-btn-policy-task-export').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/tasks-export-full',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyTaslTable.getData()),
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
function initializeTaskOwnerSelect2() {
    $(".js-task-owners").each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            initializeTaskOwnerElement($(this));
        }
    });
}

function initializeTaskOwnerElement($element) {
    const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select document owner';

    $element.select2({
        width: 'resolve',
        placeholder: 'Select a document owner...',
        allowClear: true,
        escapeMarkup: function (markup) {
            return markup;
        },
        language: {
            noResults: function () {
                return "No document owners found";
            }
        }
    });
    setTimeout(() => {
        fixSelect4Accessibility($element, labelText);
    }, 100);
}

function loadTaskOwners() {
    const $ownerSelect = $('#taskForm').find('.js-task-owners');
    $.ajax({
        url: '/grc/compliance/settings/responsibilities-list',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $ownerSelect.each(function () {
                const $select = $(this);
                const currentValue = $select.val();

                // Destroy existing Select2 if it exists
                if ($select.hasClass('select2-hidden-accessible')) {
                    $select.select2('destroy');
                }

                // Clear existing options
                $select.empty();

                // Add placeholder option
                $select.append('<option value="">Select a owner...</option>');

                // Add owners options
                if (data.results && data.results.length > 0) {
                    $.each(data.results, function (index, owner) {
                        $select.append(`<option value="${owner.id}">${owner.ownerName}</option>`);
                    });
                }

                // Restore previous value if it exists
                if (currentValue) {
                    $select.val(currentValue);
                }

                // Initialize Select2 with accessibility fixes
                initializeTaskOwnerElement($select);
            });
        },
        error: function (xhr, status, error) {
            console.error('Error loading owners:', error);

            // Initialize empty Select2 even on error
            $ownerSelect.each(function () {
                const $select = $(this);
                if (!$select.hasClass('select2-hidden-accessible')) {
                    initializeTaskOwnerElement($select);
                }
            });
        }
    });
}

function initializeTaskNewSelect2() {
    $(".js-task-document").each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            initializeTaskElement($(this));
        }
    });
}

function initializeTaskElement($element) {
    const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select documents';

    $element.select2({
        width: 'resolve',
        placeholder: 'Select an document...',
        allowClear: true,
        escapeMarkup: function (markup) {
            return markup;
        },
        language: {
            noResults: function () {
                return "No documents found";
            }
        }
    });

    setTimeout(() => {
        fixSelect4Accessibility($element, labelText);
    }, 100);
}

function loadTaskDocuments() {
    const $docSelect = $('.js-task-document');
    return $.ajax({
        url: '/grc/compliance/register/policies-list',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            $docSelect.each(function () {
                const $select = $(this);
                const currentValue = $select.val();

                if ($select.hasClass('select2-hidden-accessible')) {
                    $select.select2('destroy');
                }

                $select.empty();
                $select.append('<option value="">Select a document...</option>');

                if (data.results?.length > 0) {
                    data.results.forEach(doc => {
                        $select.append(`<option value="${doc.id}">${doc.documentName}</option>`);
                    });
                }

                if (currentValue) $select.val(currentValue);
                initializeTaskElement($select);
            });
        },
        error: function (xhr, status, error) {
            console.error('Error loading documents:', error);
            $docSelect.each(function () {
                const $select = $(this);
                if (!$select.hasClass('select2-hidden-accessible')) {
                    initializeTaskElement($select);
                }
            });
        }
    });
}
/*------------------------------------------
    Add New Record
-------------------------------------------*/
$('#taskForm').on('submit', function (e) {
    //..prevent full postback
    e.preventDefault();
});

function addPolicyTaskRecord() {
    $('#panelTitle').text("Policy/Procedure Tasks");
    $('.task-overlay').addClass('active');
    $('#slideTaskPanel').addClass('active');

    // Load both async requests and wait for both to complete
    $.when(loadTaskDocuments(), loadTaskOwners())
        .done(function () {
            console.log("Both Task Documents and Owners loaded successfully");
        })
        .fail(function () {
            console.error("One or both task lists failed to load.");
        });
}



//function openPolicyDocPanel(title, record, isEdit) {
    
//    $('#isEdit').val(isEdit);
//    $('#recordId').val(record.id);
//    $('#documentName').val(record.documentName || '');
//    $('#dpDocumentStatus').val(record.documentStatus).trigger('change');
//    $('#dpDocumentType').val(record.documentType).trigger('change');
//    $('#isAligned').prop('checked', record.aligned);
//    $('#isDeleted').prop('checked', record.isDeleted);
//    $('#isLocked').prop('checked', record.locked);
//    $('#reviewPeriod').val(record.reviewPeriod || 0);
//    $('#dpOwner').val(record.documentOwner).trigger('change');
//    $('#comments').val(record.comments || ''); 

//    //..use setDate
//    const today = new Date();
//    if (record.lastReview) {
//        flatpickrInstances["lastRevisionDate"].setDate(record.lastReview, true, "Y-m-d");
//    } else {
//        flatpickrInstances["lastRevisionDate"].setDate(today, true, "Y-m-d");
//    }

//    if (record.nextReview) {
//        flatpickrInstances["nextRevisionDate"].setDate(record.nextReview, true, "Y-m-d");
//    } else {
//        flatpickrInstances["nextRevisionDate"].setDate(today, true, "Y-m-d");
//    }

//    $('#panelTitle').text(title);
//    $('.overlay').addClass('active');
//    $('#slidePanel').addClass('active');
//}

/*------------------------------------------
    Save Record
-------------------------------------------*/



//function savePolRegisterRecord(e) {
//    if (e) e.preventDefault();
//    let isEdit = $('#isEdit').val();

//    // --- gather form values ---
//    let recordData = {
//        id: parseInt($('#recordId').val()) || 0,
//        documentName: $('#documentName').val()?.trim(),
//        documentStatus: $('#dpDocumentStatus').val(),
//        documentType: Number($('#dpDocumentType').val()),
//        isDeleted: $('#isDeleted').is(':checked') ? true : false,
//        aligned: $('#isAligned').is(':checked') ? true : false,
//        documentOwner: Number($('#dpOwner').val()) || 0,
//        reviewPeriod: Number($('#reviewPeriod').val()),
//        lastReview: flatpickrInstances["lastRevisionDate"].input.value || null,
//        nextReview: flatpickrInstances["nextRevisionDate"].input.value || null,
//        comments: $('#comments').val()?.trim()
//    };
   
//    // --- validate required fields ---
//    let errors = [];

//    if (!recordData.documentName)
//        errors.push("Document name is required.");

//    if (!recordData.documentStatus)
//        errors.push("Document status is required");

//    if (!recordData.documentType)
//        errors.push("Document type is require");

//    if (!recordData.documentOwner)
//        errors.push("Document owner is required.");

//    if (!recordData.reviewPeriod)
//        errors.push("Review Period is required.");

//    //..date validation
//    if (!recordData.lastReview)
//        errors.push("Last review date is required.");

//    // --- stop if validation fails ---
//    if (errors.length > 0) {

//        highlightField("#documentName", !recordData.documentName);
//        highlightField("#dpDocumentStatus", !recordData.documentStatus);
//        highlightField("#dpDocumentType", !recordData.documentType);
//        highlightField("#dpOwner", !recordData.documentOwner);

//        Swal.fire({
//            title: "Record Validation",
//            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
//        });
//        return;
//    }

//    console.log("Valid Record:", recordData);
//    savePolicy(isEdit, recordData);
//}

//function savePolicy(isEdit, payload) {
//    const url = (isEdit === true || isEdit === "true")
//        ? "/grc/compliance/register/policies-update"
//        : "/grc/compliance/register/policies-create";

//    Swal.fire({
//        title: isEdit ? "Updating Policy..." : "Saving Policy...",
//        text: "Please wait while we process your request.",
//        allowOutsideClick: false,
//        allowEscapeKey: false,
//        didOpen: () => {
//            Swal.showLoading();
//        }
//    });

//    console.log("Sending data to server:", JSON.stringify(payload));  // Debugging
//    $.ajax({
//        url: url,
//        type: "POST",
//        contentType: "application/json",
//        data: JSON.stringify(payload),
//        headers: {
//            'X-Requested-With': 'XMLHttpRequest',
//            'X-CSRF-TOKEN': getPolicyAntiForgeryToken()
//        },
//        success: function (res) {
//            //..lose loader and show success message
//            Swal.close();
//            if (!res.success) {
//                //..error from the server
//                Swal.fire({
//                    title: "Invalid record",
//                    html: res.message.replaceAll("; ", "<br>")
//                });
//                return;
//            }

//            if (res && res.data) {
//                if (isEdit) {
//                    policyRegisterTable.updateData([res.data]);
//                } else {
//                    policyRegisterTable.addData([res.data], true);
//                }
//            }

//            Swal.fire({
//                title: isEdit ? "Updating Policy..." : "Saving Policy...",
//                text: res.message || "Saved successfully.",
//                timer: 2000,
//                showConfirmButton: false
//            });

//            closePolicyDocumentPanel();
//        },
//        error: function (xhr) {
//            Swal.close();

//            let errorMessage = "Unexpected error occurred.";
//            try {
//                let response = JSON.parse(xhr.responseText);
//                if (response.message) errorMessage = response.message;
//            } catch (e) { }

//            Swal.fire({
//                title: isEdit ? "Update Failed" : "Save Failed",
//                text: errorMessage
//            });
//        }
//    });
//}

//function highlightField(selector, isError) {
//    if (isError) $(selector).addClass("input-error");
//    else $(selector).removeClass("input-error");
//}

//function deletePolicyRecord(id) {
//    if (!id && id !== 0) {
//        toastr.error("Invalid id for delete.");
//        return;
//    }

//    Swal.fire({
//        title: "Delete Policy",
//        text: "Are you sure you want to delete this policy/procedure?",
//        showCancelButton: true,
//        confirmButtonColor: "#450354",
//        confirmButtonText: "Delete",
//        cancelButtonColor: "#f41369",
//        cancelButtonText: "Cancel"
//    }).then((result) => {
//        if (!result.isConfirmed) return;

//        $.ajax({
//            url: `/grc/compliance/register/policies-delete/${encodeURIComponent(id)}`,
//            type: 'DELETE',
//            headers: {
//                'X-Requested-With': 'XMLHttpRequest',
//                'X-CSRF-TOKEN': getPolicyAntiForgeryToken()
//            },
//            success: function (res) {
//                if (res && res.success) {
//                    toastr.success(res.message || "Policy/Procedure deleted successfully.");
//                    policyRegisterTable.setPage(1, true);
//                } else {
//                    toastr.error(res?.message || "Delete failed.");
//                }
//            },
//            error: function () {
//                toastr.error("Request failed.");
//            }
//        });
//    });
//}

//function viewPolicyRecord(id) {
//    Swal.fire({
//        title: 'Loading...',
//        text: 'Retrieving policy...',
//        allowOutsideClick: false,
//        allowEscapeKey: false,
//        didOpen: () => Swal.showLoading()
//    });

//    console.log("ID >> " + id);
//    findPolicyRecord(id)
//        .then(record => {
//            Swal.close();
//            if (record) {
//                openPolicyDocPanel('Edit Regulatory Policy/Procedure', record, true);
//            } else {
//                Swal.fire({ title: 'NOT FOUND', text: 'Policy/Procedure not found' });
//            }
//        })
//        .catch(() => {
//            Swal.close();
//            Swal.fire({ title: 'Error', text: 'Failed to load policy/procedure details.' });
//        });
//}

//function findPolicyRecord(id) {
//    console.log("Retriev record for id >> " + id);
//    return new Promise((resolve, reject) => {
//        $.ajax({
//            url: `/grc/compliance/register/policies-retrieve/${id}`,
//            type: "GET",
//            dataType: "json",
//            success: function (response) {
//                if (response.success && response.data) {
//                    resolve(response.data);
//                } else {
//                    resolve(null);
//                }
//            },
//            error: function () {
//                reject();
//            }
//        });
//    });
//}

/*------------------------------------------
    Search Functionality
-------------------------------------------*/
function initTaskSearch() {
    const searchInput = $('#taskSearchbox'); // fixed ID
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                policyRegisterTable.setFilter([
                    [
                        { field: "taskDescrption", type: "like", value: searchTerm },
                        { field: "documentName", type: "like", value: searchTerm },
                        { field: "assignee", type: "like", value: searchTerm }
                    ]
                ]);
                policyRegisterTable.setPage(1, true);
            } else {
                policyRegisterTable.clearFilter();
            }
        }, 300);
    });
}

function fixSelect4Accessibility($originalSelect, labelText) {
    const selectId = $originalSelect.attr('id');
    if (!selectId) return;

    const $select2Container = $originalSelect.next('.select2-container');
    const $select2Selection = $select2Container.find('.select2-selection');
    const $select2Arrow = $select2Container.find('.select2-selection__arrow');

    // Remove problematic aria-hidden from the original select
    $originalSelect.removeAttr('aria-hidden');

    // Add proper ARIA attributes to Select2 elements
    $select2Selection.attr({
        'role': 'combobox',
        'aria-expanded': 'false',
        'aria-haspopup': 'listbox',
        'aria-labelledby': selectId + '-label',
        'aria-describedby': selectId + '-description'
    });

    // Create or update label
    let $label = $(`label[for="${selectId}"]`);
    if ($label.length === 0) {
        $label = $originalSelect.closest('.form-group').find('label').first();
        $label.attr('for', selectId);
    }
    $label.attr('id', selectId + '-label');

    // Add description for screen readers
    if ($(`#${selectId}-description`).length === 0) {
        $('<span>', {
            id: selectId + '-description',
            class: 'sr-only',
            text: 'Use arrow keys to navigate options'
        }).insertAfter($select2Container);
    }

    // Handle Select2 events for accessibility
    $originalSelect.on('select2:open', function () {
        $select2Selection.attr('aria-expanded', 'true');

        // Focus the search input when dropdown opens
        setTimeout(() => {
            const $searchInput = $('.select2-search__field');
            if ($searchInput.length) {
                $searchInput.attr('aria-label', `Search ${labelText}`);
            }
        }, 50);
    });

    $originalSelect.on('select2:close', function () {
        $select2Selection.attr('aria-expanded', 'false');
    });

    // Remove aria-hidden when element gains focus
    $select2Selection.on('focus', function () {
        $originalSelect.removeAttr('aria-hidden');
        $(this).removeAttr('aria-hidden');
    });
}

//..get antiforegery token from meta tag
function getPolicyAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function savePolTaskRecord(e) {

}

function closePolicyTaskPanel() {
    $('.task-overlay').removeClass('active');
    $('#slideTaskPanel').removeClass('active');
}


