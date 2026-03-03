let processNewTable;

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

function initProcessNewListTable() {
    processNewTable = new Tabulator("#processNewTable", {
        ajaxURL: "/operations/workflow/processes/new-list",
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
                        ["processName", "description", "ownerName", "unitName", "assigneeName"].includes(f.field)
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
                title: "PROCESS NAME",
                field: "processName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                headerSort: true,
                headerFilter: "input",
                formatter: function (cell) {

                    if (hasPermission("EditOperationProcesses")) {
                        return `<span class="clickable-title" onclick="viewProcess(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                    } else {
                        return `<span >${cell.getValue()}</span>`
                    }
                   
                }
            },
            { title: "PROCESS DESCRIPTION", field: "description", widthGrow: 2, minWidth: 400, frozen: true, headerSort: false, headerFilter: "input" },
            { title: "ATTACHED UNIT", field: "unitName", minWidth: 250, headerFilter: "input" },
            { title: "PROCESS MANAGER", field: "assigneeName", minWidth: 400, headerFilter: "input" },
            {
                title: "REQUEST",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();

                    if (hasPermission("ManageOperationProcesses")) {
                        return `<button class="grc-table-btn grc-btn-view grc-view-action"
                                onclick="initiateReview(${rowData.id}, '${rowData.processName.replace(/'/g, "\\'")}')">
                                <span><i class="mdi mdi-eye-arrow-right-outline"></i></span>
                                <span>REQUEST</span>
                            </button>`;
                    } else {
                        return `<button class="grc-table-btn grc-btn-view grc-view-action disabled" disabled>
                                <span><i class="mdi mdi-eye-arrow-right-outline"></i></span>
                                <span>REQUEST</span>
                            </button>`;
                    }
                    
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }, ,
            {
                title: "DELETE",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteProcess(${rowData.id})">
                            <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                            <span>DELETE</span>
                        </button>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessNewSearch();
}

function initProcessNewSearch() {
    const searchInput = $('#newSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processNewTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "description", type: "like", value: searchTerm },
                        { field: "ownerName", type: "like", value: searchTerm },
                        { field: "assigneeName", type: "like", value: searchTerm },
                        { field: "unitName", type: "like", value: searchTerm }
                    ]
                ]);
                processNewTable.setPage(1, true);
            } else {
                processNewTable.clearFilter();
            }
        }, 300);
    });
}

function initiateReview(id, processName) {
    if (!id && id !== 0) {
        toastr.error("Invalid Process ID.");
        return;
    }

    Swal.fire({
        title: "REQUEST FOR REVIEW",
        text: `Send "${processName}" for review?`,
        showCancelButton: true,
        confirmButtonColor: "#5E2A5E",
        confirmButtonText: "Forward For Review",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        let payload = {
            id: id,
            processName: processName,
            ProcessStatus: 'INREVIEW',
            unlockReason:''
        };

        $.ajax({
            url: `/operations/workflow/processes/approval/initiate-review`,
            type: 'POST',
            data: JSON.stringify(payload),
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getAntiForgeryToken()
            },
            success: function (res) {

                if (!res || typeof res !== "object") {
                    Swal.fire("System Error", "Unexpected server response.");
                    return;
                }

                if (res.success) {
                    Swal.fire("REQUEST REVIEW", res.message || "Request submitted.");

                    if (processNewTable?.replaceData) {
                        processNewTable.replaceData();
                    }

                } else {
                    Swal.fire("REQUEST REVIEW", res.message || "Submission Failed.");
                }
            },
            error: function (xhr, status, error) {
                console.error("Submission error:", error);
                let msg = xhr.responseJSON?.message || "Submission failed.";
                Swal.fire("REQUEST APPROVAL", msg);
            }
        });
    });
}

function deleteProcess(id) {
    if (!id && id !== 0) {
        Swal.fire("Error", "Invalid id for delete.", "error");
        return;
    }

    Swal.fire({
        title: "Delete Process",
        text: "Are you sure you want to delete this process?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/operations/workflow/processes/registers/delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getAntiForgeryToken()
            },
            success: function (res) {

                //...avoid crashing if server returns HTML or non-JSON
                if (!res || typeof res !== "object") {
                    console.warn("Unexpected server response:", res);

                    Swal.fire("Deleted Process", "Unexpected server response.");
                    return;
                }

                if (res.success) {

                    Swal.fire({
                        title: "Deleted Process",
                        text: res.message || "Process deleted successfully."
                    });

                    if (processNewTable?.replaceData) {
                        processNewTable.replaceData();
                    }

                } else {
                    Swal.fire("Delete Record", res.message || "Failed to delete process");
                }
            },
            error: function (xhr) {

                let msg = "Failed to delete process";
                if (xhr.responseJSON && typeof xhr.responseJSON.message === "string") {
                    msg = xhr.responseJSON.message;
                }

                Swal.fire("Delete Record", msg);
            }
        });
    });
}

function getAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

}

function findProcessRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/registers/retrieve/${encodeURIComponent(id)}`,
            type: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            success: function (res) {
                if (res && res.success) {
                    resolve(res.data);
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

function openProcessView(process) {
    var status = process?.processStatus;

    $("#processId").val(process?.id || "");
    $("#processName").val(process?.processName || "");
    $("#processDescription").val(process?.description || "");
    $("#typeId").val(process?.typeId || 0).trigger('change.select2');
    $("#effectiveDate").val(process?.effectiveDate);
    $("#unitId").val(process?.unitId || 0).trigger('change.select2');
    $("#ownerId").val(process?.ownerId || 0).trigger('change.select2');
    $("#assigneedId").val(process?.assigneedId || 0).trigger('change.select2');
    $("#comments").val(process?.comment || "");
    $("#processStatus").val(status).trigger('change.select2');

    $('#needsBranchOperations').prop('checked', process?.needsBranchOperations || false);
    $('#needsCreditReview').prop('checked', process?.needsCreditReview || false);
    $('#needsFintechReview').prop('checked', process?.needsFintechReview || false);
    $('#needsTreasuryReview').prop('checked', process?.needsTreasuryReview || false);

    //..show overlay panel
    $('#processOverlay').addClass('active');
    $('#processPanel').addClass('active');
}

function closeProcessPanel() {
    $('#processOverlay').removeClass('active');
    $('#processPanel').removeClass('active');
}

function saveViewEdit(e) {
    if (e) e.preventDefault();
    let recordData = {
        id: parseInt($('#processId').val()) || 0,
        processName: $('#processName').val()?.trim(),
        description: $('#processDescription').val()?.trim(),
        typeId: parseInt($('#typeId').val()) || 0,
        unitId: parseInt($('#unitId').val()) || 0,
        ownerId: parseInt($('#ownerId').val()) || 0,
        processStatus: $('#processStatus').val()?.trim(),
        responsibilityId: parseInt($('#assigneedId').val()) || 0,
        needsBranchReview: $('#needsBranchOperations').prop('checked'),
        needsCreditReview: $('#needsCreditReview').prop('checked'),
        needsTreasuryReview: $('#needsTreasuryReview').prop('checked'),
        needsFintechReview: $('#needsFintechReview').prop('checked'),
        comments: $('#comments').val()?.trim(),
    };
    // --- validate required fields ---
    let errors = [];
    if (!recordData.processName)
        errors.push("Process name is required.");
    if (!recordData.description)
        errors.push("Process description is required.");
    if (!recordData.processStatus)
        errors.push("Process status is required.");
    if (recordData.typeId === 0)
        errors.push("Process type is required.");
    if (recordData.unitId === 0)
        errors.push("Department unit is required.");
    if (recordData.ownerId === 0)
        errors.push("Process owner is required.");
    if (recordData.responsibilityId === 0)
        errors.push("Responsible manager is required.");
    if (!recordData.comments)
        errors.push("Comment is required.");

    if (errors.length > 0) {
        highlightViewField("#processName", !recordData.processName);
        highlightViewField("#processDescription", !recordData.description);
        highlightViewField("#comment", !recordData.comments);
        highlightViewField("#processStatus", !recordData.processStatus);
        highlightViewField("#typeId", recordData.typeId === 0);
        highlightViewField("#unitId", recordData.unitId === 0);
        highlightViewField("#ownerId", recordData.ownerId === 0);
        highlightViewField("#assigneedId", recordData.responsibilityId === 0);
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    saveView(recordData);
}

function saveView(payload) {
    const url = "/operations/workflow/processes/registers/retrieve/update";

    Swal.fire({
        title: "Updating process...",
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
            'X-CSRF-TOKEN': getProcessViewForgeryToken()
        },
        success: function (res) {
            if (!res.success) {
                Swal.close();
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            Swal.close();
            if (processNewTable) {
                processNewTable.updateData([res.data]);
            }
            closeProcessPanel();

            Swal.fire({
                title: "Success",
                text: "Process updated successfully!",
                icon: "success",
                timer: 2000
            });
        },
        error: function (xhr) {
            Swal.close();
            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }
            Swal.fire({
                title: "Update Failed",
                text: errorMessage
            });
        }
    });
}

function viewProcess(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving process record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findProcessRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openProcessView(record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Process record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load process details.' });
        });
}

let dateList2 = {};
function initDate2Pickers() {

    dateList2["effectiveDate"] = flatpickr("#effectiveDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

//..name input validation
function validateProcessNameInput(event) {
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
        highlightViewField('#processName', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightViewField('#processName', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightViewField('#processName', false);
        return true;
    }

}

function handleProcessNamePaste(event) {
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
        highlightViewField('#processName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightViewField('#processName', false);
    }
}

//..comments input validation
function validateDescriptionInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s;,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightViewField('#processDescription', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightViewField('#processDescription', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightViewField('#processDescription', false);
        return true;
    }

}

function handleDescriptionPaste(event) {
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
        highlightViewField('#processDescription', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightViewField('#processDescription', false);
    }
}

//..comments input validation
function validateCommentInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s;,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightViewField('#comments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightViewField('#comments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightViewField('#comments', false);
        return true;
    }

}

function handleCommentPaste(event) {
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
        highlightViewField('#comments', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightViewField('#comments', false);
    }
}

function highlightViewField(selector, hasError, message) {
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

function getProcessViewForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

}

$(document).ready(function () {
    initDate2Pickers();
    initProcessNewListTable();

    $('#processForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#typeId, #processStatus, #unitId, #ownerId, #assigneedId, #complianceStatus, #branchManagerStatus, #approvalStatus').select2({
        width: '100%',
        dropdownParent: $('#processPanel')
    });
    //..category name validation 
    $('#processName').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightViewField('#processName', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightViewField('#processName', true, 'Invalid characters detected');
        } else {
            highlightViewField('#processName', false);
        }
    });

    $('#processName').on('focus', function () {
        highlightViewField('#processName', false);
    });

    $('#processName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightViewField('#processName', true, 'Process name is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightViewField('#processName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightViewField('#processName', false);
        }
    });

    $('#processName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightViewField('#processName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightViewField('#processName', false);
                }, 2000);
            }
        }
    });

    //..process description validation 
    $('#processDescription').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightViewField('#processDescription', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightViewField('#processDescription', true, 'Invalid characters detected');
        } else {
            highlightViewField('#processDescription', false);
        }
    });

    $('#processDescription').on('focus', function () {
        highlightViewField('#processDescription', false);
    });

    $('#processDescription').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightViewField('#processDescription', true, 'process description is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightViewField('#processDescription', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightViewField('#processDescription', false);
        }
    });

    $('#processDescription').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightViewField('#processDescription', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightViewField('#processDescription', false);
                }, 2000);
            }
        }
    });

    //..comments validation 
    $('#comments').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightViewField('#comments', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightViewField('#comments', true, 'Invalid characters detected');
        } else {
            highlightViewField('#comments', false);
        }
    });

    $('#comments').on('focus', function () {
        highlightViewField('#comments', false);
    });

    $('#comments').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightViewField('#comments', true, 'Comments is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightViewField('#comments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightViewField('#comments', false);
        }
    });

    $('#comments').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightViewField('#comments', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightViewField('#comments', false);
                }, 2000);
            }
        }
    });
    
});
