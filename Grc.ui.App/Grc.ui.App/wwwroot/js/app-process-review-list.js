let processReviewTable;

function initProcessReviewListTable() {
    processReviewTable = new Tabulator("#processReviewTable", {
        ajaxURL: "/operations/workflow/processes/review-list",
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
                title: "PROCESS NAME",
                field: "processName",
                minWidth: 200,
                widthGrow: 1,
                headerSort: true,
                frozen: true
            },
            { title: "REVIEW COMMENTS", field: "comment", widthGrow: 2, minWidth: 400, frozen: true, headerSort: false },
            { title: "ATTACHED UNIT", field: "unitName", minWidth: 250 },
            { title: "PROCESS MANAGER", field: "assigneeName", minWidth: 400 },
            {
                title: "VIEW",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="viewRecord(${rowData.id})">
                                <span><i class="mdi mdi-eye-arrow-right-outline" aria-hidden="true"></i></span>
                                <span>VIEW</span>
                            </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    //..search init
    initProcessReviewSearch();
}

function findNewRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/approval/new-request/${encodeURIComponent(id)}`,
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

function viewRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving process record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findNewRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openReviewEditor('INREVIEW PROCESS', record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Process record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load process details.' });
        });
}

function openReviewEditor(title, approval) {
    var bopRequired = approval?.requiresBopApproval || false;
    var creditRequired = approval?.requiresCreditApproval || false;
    var treasuryRequired = approval?.requiresTreasuryApproval || false;
    var fintechRequired = approval?.requiresFintechApproval || false;

    var tStr = approval?.processName || "";
   
    if (tStr)
        title = `INREVIEW PROCESS - ${tStr}`;

    const date = new Date(approval?.requestDate);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");

    //..populate form fields
    $("#processId").val(approval?.processId || 0);
    $("#processName").val(tStr);
    $("#processDescription").val(approval?.processDescription || "");
    
   
    $("#assigneeComments").val(approval?.assigneeComments || "");
    $("#ownerName").val(approval?.ownerName || "");
    $("#unitName").val(approval?.unitName || "");
    $("#assigneeName").val(approval?.assigneeName || "");
    $("#comments").val(approval?.comments || "");
    $('#requiresBopApproval').prop('checked', bopRequired);
    $('#requiresCreditApproval').prop('checked', creditRequired);
    $('#requiresTreasuryApproval').prop('checked', treasuryRequired);
    $('#requiresFintechApproval').prop('checked', fintechRequired);
    $("#processType").val(approval?.processType || "");
    $("#fileName").val(approval?.fileName || "");
    $("#fileVersion").val(approval?.fileVersion || "");
    $("#assigneeComments").val(approval?.assigneeComments || "");
    $('#approvalPanelTitle').text(title);
    $('#revOverlay').addClass('active');
    $('#collapsePanel').addClass('active');
}

function saveReviewRecord(e) {
    e.preventDefault();
    let fileName = $('#fileName').val()?.trim();
    let fileVersion = $('#fileVersion').val()?.trim();
    let managerComments = $('#assigneeComments').val()?.trim();

    let isValid = true;
    let errors = [];
    if (!fileName) {
        highlightReviewField('#fileName', true, 'File name field is required');
        isValid = false;
        errors.push("File name field is required");
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(typeName)) {
        highlightReviewField('#fileName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
        errors.push("Only letters, numbers, commas, periods, and spaces allowed");
    }

    if (!fileVersion) {
        highlightReviewField('#fileVersion', true, 'File version field is required');
        isValid = false;
        errors.push("File version field is required");
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(fileVersion)) {
        highlightReviewField('#fileVersion', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
        errors.push("Only letters, numbers, commas, periods, and spaces allowed");
    }

     if (!managerComments && !/^[a-zA-Z0-9\s,.]*$/.test(managerComments)) {
        highlightReviewField('#assigneeComments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
        errors.push("Only letters, numbers, commas, periods, and spaces allowed");
    }
    
    if (!isValid) {
         Swal.fire({
            title: "Review Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        //..stop submission
        return;
    }

    // --- validate required fields ---
    let recordData = {
        id: parseInt($('#id').val()) || 0,
        processId: parseInt($('#processId').val()) || 0,
        fileName: fileName,
        fileVersion:fileVersion,
        managerComments: managerComments,
        status: "REVIEWED"
    };

    saveReview(recordData)
}

function saveReview(record) {
    const url ="/grc/compliance/audits/types/type-create";

    Swal.fire({
        title: "Sending request to HOD for approval...",
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
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getAuditToken()
        },
        success: function (res) {
            Swal.close();
            if (!res.success) {
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            Swal.fire(res.message || "Approval request sent to HOD")
                .then(() => {
                    //..close panel
                    closeAuditCategory();
                    window.location.reload();
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
                title: "Request Failed",
                text: errorMessage
            });
        }
    });
}

function initProcessReviewSearch() {
    const searchInput = $('#reviewSearchbox');
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
                processReviewTable.setPage(1, true);
            } else {
                processReviewTable.clearFilter();
            }
        }, 300);
    });
}

function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');
    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}

function closeReviewPanel() {
    $('#revOverlay').removeClass('active');
    $('#collapsePanel').removeClass('active');
}

//..comments input validation
function validateVersionInput(event) {
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
        highlightReviewField('#comments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightReviewField('#comments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightReviewField('#comments', false);
        return true;
    }

}

function handleVersionPaste(event) {
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
        highlightReviewField('#fileVersion', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightReviewField('#fileVersion', false);
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
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightReviewField('#comments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightReviewField('#comments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightReviewField('#comments', false);
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
        highlightReviewField('#comments', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightReviewField('#comments', false);
    }
}


function highlightReviewField(selector, hasError, message) {
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

$(document).ready(function () {

    initProcessReviewListTable();
    $('#processReviewForm').on('submit', function (e) {
        e.preventDefault();
    });
    
    //..category name validation 
    $('#fileVersion').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightReviewField('#fileVersion', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightReviewField('#fileVersion', true, 'Invalid characters detected');
        } else {
            highlightReviewField('#fileVersion', false);
        }
    });

    $('#fileVersion').on('focus', function () {
        highlightReviewField('#fileVersion', false);
    });

    $('#fileVersion').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightReviewField('#fileVersion', true, 'File version is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightReviewField('#fileVersion', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightReviewField('#fileVersion', false);
        }
    });

    $('#fileVersion').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightReviewField('#fileVersion', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightReviewField('#fileVersion', false);
                }, 2000);
            }
        }
    });
     //..comments validation 
    $('#comments').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightReviewField('#comments', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightReviewField('#comments', true, 'Invalid characters detected');
        } else {
            highlightReviewField('#comments', false);
        }
    });

    $('#comments').on('focus', function () {
        highlightReviewField('#comments', false);
    });

    $('#comments').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightReviewField('#comments', true, 'Comments is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightReviewField('#comments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightReviewField('#comments', false);
        }
    });

    $('#comments').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightReviewField('#comments', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightReviewField('#comments', false);
                }, 2000);
            }
        }
    });
});