let auditTypeTable;

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

function initAuditType2Table() {
    auditTypeTable = new Tabulator("#auditTypetable", {
        ajaxURL: "/grc/compliance/audit/types",
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
                        ["typeCode", "typeName", "description"].includes(f.field)
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
            Swal.fire("Failed to load audit types. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            { title: "TYPE CODE", field: "typeCode", minWidth: 200, frozen: true, headerSort: true },
            {
                title: "TYPE NAME",
                field: "typeName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: function (cell) {
                    //..if user has permission to view/edit
                    if (hasPermission("CANUPDATECOMPLIANCEAUDITTYPES")) {
                        return `<span class="clickable-title" onclick="viewAuditType(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                    } else {
                        return `<span >${cell.getValue()}</span>`
                    }
                },
                formatter: (cell) => 
            },
            { title: "DESCRIPTION", field: "description", minWidth: 200, widthGrow: 3 },
            {
                title: "ACTION",
                 formatter: function (cell) {
                    if (hasPermission("CANDELETECOMPLIANCEAUDITTYPES")) {
                        let rowData = cell.getRow().getData();
                        return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteAuditType(${rowData.id})">
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
}

$('.action-btn-audit-type-new').on('click', function () {
    addAuditType();
});

function addAuditType() {
    openAuditTypePanel('Add Audit Type', {
        id: 0,
        typeCode: '',
        typeName: '',
        description: '',
        isDeleted: false,
    }, false);
}

function openAuditTypePanel(title, record, isEdit) {
    $('#isTypeEdit').val(isEdit);
    $('#typeId').val(record.id);
    $('#typeCode').val(record.typeCode || '');
    $('#typeName').val(record.typeName || '');
    $('#description').val(record.description || '');
    $('#isDeleted').prop('checked', record.isDeleted);

    //load dialog window
    $('#auditTypeTitle').text(title);
    $('#auditTypeOverlay').addClass('active');
    $('#auditTypePanel').addClass('active');
}

function viewAuditType(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving type...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findTypeRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openAuditTypePanel('Edit Type', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Audit type not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load type details.' });
        });
}

function findTypeRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/audit/types/type-retrieve/${id}`,
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

function deleteAuditType(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete audit type",
        text: "Are you sure you want to delete this type?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        const url = `/grc/compliance/audits/types/type-delete/${encodeURIComponent(id)}`;
        $.ajax({
            url: url,
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getTypeToken()
            },
            success: function (res) {

                if (res && res.success) {
                    toastr.success(res.message || "Type deleted successfully.");
                    auditTypeTable.replaceData();
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function (xhr, status, error) {
                let msg = "Request failed.";
                try {
                    const json = xhr.responseJSON || JSON.parse(xhr.responseText || "{}");
                    if (json && json.message) msg = json.message;
                } catch (e) { }
                toastr.error(msg);
            }
        });
    });
}

function saveAuditTypePane(e) {
    e.preventDefault();
    let id = Number($('#typeId').val()) || 0;
    let isEdit = $('#isTypeEdit').val() || false;
    let typeCode = $('#typeCode').val().trim();
    let typeName = $('#typeName').val().trim();
    let description = $('#description').val().trim();
    
    let isValid = true;
    if (!typeCode) {
        highlightAuditTypeField('#typeCode', true, 'Type code is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9]*$/.test(typeCode)) {
        highlightAuditTypeField('#typeCode', true, 'Only letters and numbers  allowed');
        isValid = false;
    }

    if (!typeName) {
        highlightAuditTypeField('#typeName', true, 'Type name is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(typeName)) {
        highlightAuditTypeField('#typeName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }
    
    if (!description && !/^[a-zA-Z0-9\s,.]*$/.test(description)) {
        highlightAuditTypeField('#description', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }
    
    if (!isValid) {
        // Stop submission
        return; 
    }

    //..build record payload from form
    let recordData = {
        id: id,
        typeCode: typeCode,
        typeName: typeName,
        description: description,
        isDeleted: $('#isIssueDeleted').prop('checked'),
    };

    //..validate required fields
    let errors = [];
    if (!recordData.typeCode)
        errors.push("Type code field is required.");

    if (!recordData.typeName)
        errors.push("Type name field is required.");

    if (errors.length > 0) {
        highlightAuditTypeField("#typeCode", !recordData.typeCode);
        highlightAuditTypeField("#typeName", !recordData.typeName);
        Swal.fire({
            title: "Audit type Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveAuditTypeRecord(isEdit, recordData);
}

function saveAuditTypeRecord(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/audits/types/type-update"
        : "/grc/compliance/audits/types/type-create";

    Swal.fire({
        title: isEdit ? "Updating type..." : "Saving type...",
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
            'X-CSRF-TOKEN': getTypeToken()
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

            Swal.fire(res.message || (isEdit ? "Type updated successfully" : "Type created successfully"));

            // reload table
            auditTypeTable.replaceData();

            //..close panel
            closeAuditTypePane();

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

function getTypeToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function closeAuditTypePane() {
    $('#auditTypeOverlay').removeClass('active');
    $('#auditTypePanel').removeClass('active');
}

//...validate name
function validateNameInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    if (/[a-zA-Z0-9]/.test(keyChar)) {
        highlightAuditTypeField('#typeName', false);
        return true;
    }


    if (keyChar == ',' || keyChar == '.') {
        highlightAuditTypeField('#typeName', false);
        return true;
    }

    if (keyChar == ' ') {
        highlightAuditTypeField('#typeName', false);
        return true;
    }

    event.preventDefault();
    highlightAuditTypeField('#typeName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
    return false;
}

function handleNamePaste(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    if (/[a-zA-Z0-9]/.test(keyChar)) {
        highlightAuditTypeField('#typeName', false);
        return true;
    }

    if (keyChar == ',' || keyChar == '.') {
        highlightAuditTypeField('#typeName', false);
        return true;
    }

    if (keyChar == ' ') {
        highlightAuditTypeField('#typeName', false);
        return true;
    }

    event.preventDefault();
    highlightAuditTypeField('#typeName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
    return false;
}

//..validate code
function validateCodeInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    if (/[a-zA-Z0-9]/.test(keyChar)) {
        highlightAuditTypeField('#typeCode', false);
        return true;
    }

    event.preventDefault();
    highlightAuditTypeField('#typeCode', true, 'Only letters and numbers');
    return false;
}

function handleCodePaste(event) {
    event.preventDefault();
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    if (pastedText !== cleanedText) {
        highlightAuditTypeField('#typeCode', true, 'Some characters were removed - only letters and numbers allowed');
    } else {
        highlightAuditTypeField('#typeCode', false);
    }
}

//..validate description
function validateDescrInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    if (/[a-zA-Z0-9]/.test(keyChar)) {
        highlightAuditTypeField('#typeName', false);
        return true;
    }


    if (keyChar == ',' || keyChar == '.') {
        highlightAuditTypeField('#typeName', false);
        return true;
    }

    if (keyChar == ' ') {
        highlightAuditTypeField('#typeName', false);
        return true;
    }

    event.preventDefault();
    highlightAuditTypeField('#typeName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
    return false;
}

function handleDescrPaste(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    if (/[a-zA-Z0-9]/.test(keyChar)) {
        highlightAuditTypeField('#description', false);
        return true;
    }

    if (keyChar == ',' || keyChar == '.') {
        highlightAuditTypeField('#description', false);
        return true;
    }

    if (keyChar == ' ') {
        highlightAuditTypeField('#description', false);
        return true;
    }

    event.preventDefault();
    highlightAuditTypeField('#description', true, 'Only letters, numbers, commas, periods, and spaces allowed');
    return false;
}


//..handle paste events to clean pasted content
function handlePaste(event) {
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
        highlightType2Field('#typeName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightType2Field('#typeName', false);
    }
}

$('.action-btn-audit-home').on('click', function () {
    try {
        window.location.href = '/grc/compliance/audit/dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

function highlightAuditTypeField(selector, hasError, message) {
    const $field = $(selector);
    const $formGroup = $field.closest('.form-group, .field-group');
    // Remove existing error
    $field.removeClass('is-invalid');
    $formGroup.find('.field-error').remove();

    if (hasError) {
        $field.addClass('is-invalid');
        if (message) {
            $formGroup.append(`<div class="field-error text-danger small mt-1 text-end">${message}</div>`);
        }
    }
}

$(document).ready(function () {
    initAuditType2Table();

    //..type code validation
    $('#typeCode').on('keyup', function () {
        var value = $(this).val();
        if (!value) {
            highlightAuditTypeField('#typeCode', false);
            return;
        }

        if (!/^[a-zA-Z0-9]*$/.test(value)) {
            highlightAuditTypeField('#typeCode', true, 'Invalid characters detected');
        } else {
            highlightAuditTypeField('#typeCode', false);
        }
    });

    $('#typeCode').on('focus', function () {
        highlightAuditTypeField('#typeCode', false);
    });

    $('#typeCode').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditTypeField('#typeCode', true, 'Type code is required');
        } else if (!/^[a-zA-Z0-9]*$/.test(value)) {
            highlightAuditTypeField('#typeCode', true, 'Only letters and numbers  allowed');
        } else {
            highlightAuditTypeField('#typeCode', false);
        }
    });

    $('#typeCode').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditTypeField('#typeCode', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditTypeField('#typeCode', false);
                }, 2000);
            }
        }
    });

    //..type name validation 
    $('#typeName').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightAuditTypeField('#typeName', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuditTypeField('#typeName', true, 'Invalid characters detected');
        } else {
            highlightAuditTypeField('#typeName', false);
        }
    });

    $('#typeName').on('focus', function () {
        highlightAuditTypeField('#typeName', false);
    });

    $('#typeName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditTypeField('#typeName', true, 'Type name is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuditTypeField('#typeName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuditTypeField('#typeName', false);
        }
    });

    $('#typeName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditTypeField('#typeName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditTypeField('#typeName', false);
                }, 2000);
            }
        }
    });
    
    //..type name validation 
    $('#description').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightAuditTypeField('#description', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuditTypeField('#description', true, 'Invalid characters detected');
        } else {
            highlightAuditTypeField('#description', false);
        }
    });

    $('#description').on('focus', function () {
        highlightAuditTypeField('#description', false);
    });

    $('#description').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditTypeField('#description', true, 'Type description is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuditTypeField('#description', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuditTypeField('#description', false);
        }
    });

    $('#description').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditTypeField('#description', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditTypeField('#description', false);
                }, 2000);
            }
        }
    });

    $('#auditTypeForm').on('submit', function (e) {
        e.preventDefault();
    });
});