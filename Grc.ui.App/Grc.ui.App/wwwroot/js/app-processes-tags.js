let processTagTable;

function initProcessTagListTable() {
    processTagTable = new Tabulator("#processTagTable", {
        ajaxURL: "/operations/workflow/processes/tags/all",
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
                        ["tagName", "tagDescription"].includes(f.field)
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
            alert("Failed to load process tags. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "TAG NAME",
                field: "tagName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewProcessTag(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "TAG DESCRIPTION", field: "tagDescription", widthGrow: 1, minWidth: 400, frozen: true, headerSort: false },
            {
                title: "ACTIVE",
                field: "isDeleted",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let color = rowData.aligned !== true ? "#28a745" : "#dc3545";
                    let text = rowData.aligned !== true ? "YES" : "NO";

                    return `<div style="
                        display:flex;
                        align-items:center;
                        justify-content:center;
                        width:100%;
                        height:100%;
                        border-radius:50px;
                        color:${color};
                        font-weight:bold;">
                    ${text}
                        </div>`;
                },
                hozAlign: "center",
                headerHozAlign: "center",
                maxWidth: 400
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteProcessTag(${rowData.id})">
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
    initProcessTagSearch();
}

function initProcessTagSearch() {
    const searchInput = $('#tagSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processTagTable.setFilter([
                    [
                        { field: "tagName", type: "like", value: searchTerm },
                        { field: "tagDescription", type: "like", value: searchTerm }
                    ]
                ]);
                processTagTable.setPage(1, true);
            } else {
                processTagTable.clearFilter();
            }
        }, 300);
    });
}

function findProcessTag(id) {
    console.log("Tag ID >>> " + id);
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/tags/retrieve/${encodeURIComponent(id)}`,
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

function viewProcessTag(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving tag record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findProcessTag(id)
        .then(record => {
            Swal.close();
            if (record) {
                openProcessTagEditor('Edit Process Tag', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Tag record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load tag details.' });
        });
}

function openProcessTagEditor(title, tag, isEdit) {
    $("#tagId").val(tag?.id || "");
    $("#isEdit").val(isEdit);
    $("#tagName").val(tag?.tagName || "");
    $("#tagDescription").val(tag?.tagDescription || "");
    $("#tagListContainer").html("<div class='text-muted p-2'>Loading process tags...</div>");

    if (isEdit) {
        const processes = Array.isArray(tag.processes) ? tag.processes : [];
        let html = "<div class='role-group-list'>";
        processes.forEach(p => {
            const checked = p.isAssigned ? "checked" : "";
            html += `<div class="form-check">
                            <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${p.id}" value="${p.id}" ${checked}>
                            <label class="form-check-label" for="perm-${p.id}">${p.processName}</label>
                        </div>`;
        });
        html += "</div>";
        $("#tagListContainer").html(html);

        //..show deactivation checkbox
        $("#IsDeletedBox").show();
    } else {
        console.log("load all processes");
        $.get("/operations/workflow/processes/groups/processes-min", function (res) {
            const processes = Array.isArray(res?.processes) ? res.processes : [];

            let html = "<div class='role-group-list'>";
            processes.forEach(p => {
                const checked = p.isAssigned ? "checked" : "";
                html += `
                <div class="form-check">
                    <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${p.id}" value="${p.id}" ${checked}>
                    <label class="form-check-label" for="perm-${p.id}">
                        ${p.processName}
                    </label>
                </div>`;
            });
            html += "</div>";
            $("#tagListContainer").html(html);

        }).fail(() => {
            $("#tagListContainer").html("<div class='text-danger'>Failed to load processes.</div>");
        });

        //..hide deactivation checkbox
        $("#IsDeletedBox").hide();
    }

    //..show overlay panel
    $('#tagPanelTitle').text(title);
    $('#processTagOverlay').addClass('active');
    $('#processTagPanel').addClass('active');
}

function saveProcessTagRecord(e) {

    e.preventDefault();

    let isEdit = $('#isEdit').val();
    let recordData = {
        id: parseInt($('#tagId').val()) || 0,
        tagName: $('#tagName').val()?.trim(),
        tagDescription: $('#tagDescription').val()?.trim(),
        isDeleted: $('#isDeleted').prop('checked'),
        processes: $(".perm-checkbox:checked").map((_, el) => parseInt(el.value, 10)).get()
    };

    // --- validate required fields ---
    let errors = [];
    if (!recordData.tagName)
        errors.push("Tag name is required.");
    if (!recordData.tagDescription)
        errors.push("Tag description is required.");

    if (errors.length > 0) {
        highlightProcessGroupField("#tagName", !recordData.tagName);
        highlightProcessGroupField("#tagDescription", !recordData.tagDescription);
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..save group
    saveTag(isEdit, recordData);
}

function saveTag(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/operations/workflow/processes/tags/update"
        : "/operations/workflow/processes/tags/create";

    Swal.fire({
        title: isEdit ? "Updating tag..." : "Saving tag...",
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
            'X-CSRF-TOKEN': getProcessTagAntiForgeryToken()
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

            if (processTagTable) {
                if (isEdit && res.data) {
                    processTagTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    processTagTable.addRow(res.data, true);
                } else {
                    processTagTable.replaceData();
                }
            }

            closeProcessTagPanel();
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

function createTag() {
    openProcessTagEditor('New Process Tag', {
        id: 0,
        isEdit: false,
        tagName: '',
        tagDescription: ''
    }, false);
}   

function deleteProcessTag(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Tag",
        text: "Are you sure you want to delete this tag?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {

        if (!result.isConfirmed)
            return;

        $.ajax({
            url: `/operations/workflow/processes/tags/delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getProcessTagAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Tag deleted successfully.");
                    if (roleGroupTable) {
                        roleGroupTable.replaceData();
                    }
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function (xhr, status, error) {
                console.error("Delete error:", error);
                console.error("Response:", xhr.responseText);
                toastr.error(xhr.responseJSON?.message || "Request failed.");
            }
        });
    });
}

function closeProcessTagPanel() {
    $('#processTagOverlay').removeClass('active');
    $('#processTagPanel').removeClass('active');
}

//..name input validation
function validateTagNameInput(event) {
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
        highlightProcessTagField('#tagName', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessTagField('#tagName', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessTagField('#tagName', false);
        return true;
    }

}

function handleTagNamePaste(event) {
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
        highlightProcessTagField('#tagName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessTagField('#tagName', false);
    }
}

//..decscription input validation
function validateTagDescrInput(event) {
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
        highlightProcessTagField('#tagDescription', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessTagField('#tagDescription', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessTagField('#tagDescription', false);
        return true;
    }

}

function handleTagDescrPaste(event) {
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
        highlightProcessTagField('#tagDescription', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessTagField('#tagDescription', false);
    }
}

function highlightProcessTagField(selector, hasError, message) {
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

function getProcessTagAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

}

$(document).ready(function () {

    initProcessTagListTable();

    $('.action-btn-process-tag-new').on('click', function () {
        createTag();
    });

    $('#processTagForm').on('submit', function (e) {
        e.preventDefault();
    });

    //..tag name validation 
    $('#tagName').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessTagField('#tagName', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessTagField('#tagName', true, 'Invalid characters detected');
        } else {
            highlightProcessTagField('#tagName', false);
        }
    });

    $('#tagName').on('focus', function () {
        highlightProcessTagField('#tagName', false);
    });

    $('#tagName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightProcessTagField('#tagName', true, 'Tag name is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessTagField('#tagName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessTagField('#tagName', false);
        }
    });

    $('#tagName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessTagField('#tagName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessTagField('#tagName', false);
                }, 2000);
            }
        }
    });

    //..tag description validation 
    $('#tagDescription').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessTagField('#processDescription', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessTagField('#processDescription', true, 'Invalid characters detected');
        } else {
            highlightProcessTagField('#processDescription', false);
        }
    });

    $('#tagDescription').on('focus', function () {
        highlightProcessTagField('#tagDescription', false);
    });

    $('#tagDescription').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightProcessTagField('#tagDescription', true, 'Tag description is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessTagField('#tagDescription', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessTagField('#tagDescription', false);
        }
    });

    $('#tagDescription').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessTagField('#tagDescription', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessTagField('#tagDescription', false);
                }, 2000);
            }
        }
    });
});