
/*------------------------------------------ initialize table*/

$(document).ready(function () {
    initRegulatoryAuthorityTable();

     //..authority alias validation
    $('#authorityAlias').on('keyup', function () {
        var value = $(this).val();
        if (!value) {
            highlightAuthorityField('#authorityAlias', false);
            return;
        }

        if (!/^[a-zA-Z0-9]*$/.test(value)) {
            highlightAuthorityField('#typeCode', true, 'Invalid characters detected');
        } else {
            highlightAuthorityField('#typeCode', false);
        }
    });

    $('#authorityAlias').on('focus', function () {
        highlightAuthorityField('#authorityAlias', false);
    });

    $('#authorityAlias').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuthorityField('#authorityAlias', true, 'Authority alias is required');
        } else if (!/^[a-zA-Z0-9]*$/.test(value)) {
            highlightAuthorityField('#authorityAlias', true, 'Only letters and numbers  allowed');
        } else {
            highlightAuthorityField('#authorityAlias', false);
        }
    });

    $('#authorityAlias').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuthorityField('#authorityAlias', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuthorityField('#authorityAlias', false);
                }, 2000);
            }
        }
    });

    //..authority name validation 
    $('#authorityName').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightAuthorityField('#authorityName', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuthorityField('#authorityName', true, 'Invalid characters detected');
        } else {
            highlightAuthorityField('#authorityName', false);
        }
    });

    $('#authorityName').on('focus', function () {
        highlightAuthorityField('#authorityName', false);
    });

    $('#authorityName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuthorityField('#authorityName', true, 'Authority name is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuthorityField('#authorityName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuthorityField('#authorityName', false);
        }
    });

    $('#authorityName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuthorityField('#authorityName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuthorityField('#authorityName', false);
                }, 2000);
            }
        }
    });
});

let regulatoryAuthorityTable;

function initRegulatoryAuthorityTable() {
    regulatoryAuthorityTable = new Tabulator("#regulatory-authorities-table", {
        ajaxURL: "/grc/compliance/settings/authorities-all",
        paginationMode: "remote",
        filterMode: "remote",
        sortMode: "remote",
        pagination: true,
        paginationSize: 10,
        paginationSizeSelector: [10, 20, 35, 40, 50],
        paginationCounter: "rows",
        ajaxConfig: {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
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

                //..handle sorting
                if (params.sort && params.sort.length > 0) {
                    requestBody.sortBy = params.sort[0].field;
                    requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
                }

                //..handle filtering/search
                if (params.filter && params.filter.length > 0) {
                    let authFilter = params.filter.find(f => f.field === "authorityName" || f.field === "authorityAlias");
                    if (authFilter) {
                        requestBody.searchTerm = authFilter.value;
                    }
                }

                $.ajax({
                    url: url,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(requestBody),
                    success: function (response) {
                        console.log("=== AJAX RESPONSE ===", response);
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
            alert("Failed to load regulatory authorities. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "AUTHORITY NAME",
                field: "authorityName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                formatter: function (cell) {
                    return `<span class="clickable-title" onclick="viewRegulatoryAuthorityRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                }
            },
            {
                title: "SHORT NAME",
                field: "authorityAlias",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true
            },
            {
                title: "STATUS",
                field: "status",
                hozAlign: "center",
                headerHozAlign: "center",
                maxWidth: 200,
                headerSort: true
            },
            {
                title: "DATE ADDED",
                field: "addedon",
                headerSort: true
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `
                        <button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteRegulatoryAuthorityRecord(${rowData.id})">
                         <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                        <span>DELETE</span>
                        </button>
                    `;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false,
                cssClass: "action-column"
            }
        ]
    });

    // Initialize search
    initRegulatoryAuthoritySearch();
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

$('.action-btn-new-authority').on('click', function () {
    addRegulatoryAuthorityRootRecord();
});

$('#btnAuthorityExportFiltered').on('click', function () {
    $.ajax({
        url: '/grc/compliance/settings/authorities-export',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryAuthorityTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Regulatory_Authorities.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-authority-export').on('click', function () {
    $.ajax({
        url: '/grc/compliance/settings/authorities-export-full',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryAuthorityTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "RegulatoryAuthorities.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-new-authority').on('click', function () {
    addRegulatoryAuthorityRootRecord();
});

//..add authority
function addRegulatoryAuthorityRootRecord() {
    openRegulatoryAuthorityPanel('New regulatory authority', {
        id: 0,
        authorityName: '',
        authorityAlias: '',
        isActive: 'Active'
    }, false);
}

//..open slide panel
function openRegulatoryAuthorityPanel(title, record, isEdit) {
    $('#isEdit').val(isEdit);
    $('#recordId').val(record.id);
    $('#authorityName').val(record.authorityName || '');
    $('#authorityAlias').val(record.authorityAlias || '');
    $('#isActive').prop('checked', record.isActive);

    //..open panel
    $('#panelTitle').text(title);
    $('#authOverlay').addClass('active');
    $('#slidePanel').addClass('active');
}

function saveRegulatoryAuthorityRecord(e) {
    e.preventDefault(); 

    let isEdit = $('#isEdit').val();
    let alias = $('#authorityAlias').val().trim();
    let name = $('#authorityName').val().trim();

    let isValid = true;
    if (!alias) {
        highlightAuthorityField('#authorityAlias', true, 'Authority alias is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9]*$/.test(alias)) {
        highlightAuthorityField('#authorityAlias', true, 'Only letters and numbers  allowed');
        isValid = false;
    }

    if (!name) {
        highlightAuthorityField('#authorityName', true, 'Authority name is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(name)) {
        highlightAuthorityField('#authorityName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }
    
    if (!isValid) {
        //..stop submission
        return; 
    }
    
    //..build record payload from form
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        authorityAlias: alias,
        authorityName: name,
        
        isActive: $('#isActive').is(':checked') ? true : false
    };
    //..validate required fields
    let errors = [];
    if (!recordData.authorityAlias)
        errors.push("Authority alias field is required.");

    if (!recordData.authorityName)
        errors.push("Authority name field is required.");

    if (errors.length > 0) {
        highlightAuthorityField("#authorityAlias", !recordData.authorityAlias);
        highlightAuthorityField("#authorityName", !recordData.authorityName);
        Swal.fire({
            title: "Authority Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveRegulatoryAuthority(isEdit, recordData);
}

function saveRegulatoryAuthority(isEdit, payload) {
    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/settings/authorities-update"
        : "/grc/compliance/settings/authorities-create";

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getAuthAntiForgeryToken()
        },
        success: function (res) {
            if(!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || (isEdit ? "Document Type updated successfully" : "Document Type created successfully"));
            closeRegulatoryAuthorityPanel();

            //..reload table
            regulatoryAuthorityTable.replaceData();
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

            Swal.fire(isEdit ? "Update Authority" : "Save Authority", errorMessage);
        }
    });
}

function deleteRegulatoryAuthorityRecord(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Authority",
        text: "Are you sure you want to delete this authority?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        const url = `/grc/compliance/settings/authorities-delete/${encodeURIComponent(id)}`;
        $.ajax({
            url: url,
            type: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getAuthAntiForgeryToken()
            },
            success: function (res) {

                 if (res && res.success) {
                    toastr.success(res.message || "Type deleted successfully.");

                    if (typeof regulatoryAuthorityTable.replaceData === "function") {
                        regulatoryAuthorityTable.replaceData();
                    } else if (typeof regulatoryAuthorityTable.ajax !== "undefined") {
                        regulatoryAuthorityTable.ajax.reload();
                    }
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

//..validate alias
function validateAliasInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    if (/[a-zA-Z0-9]/.test(keyChar)) {
        highlightAuthorityField('#authorityAlias', false);
        return true;
    }

    event.preventDefault();
    highlightAuthorityField('#authorityAlias', true, 'Only letters and numbers');
    return false;
}

function handleAliasPaste(event) {
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
        highlightAuthorityField('#authorityAlias', true, 'Some characters were removed - only letters and numbers allowed');
    } else {
        highlightAuthorityField('#authorityAlias', false);
    }
}

//..validate name
function validateAuthorityName(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    if (/[a-zA-Z0-9]/.test(keyChar)) {
        highlightAuthorityField('#authorityName', false);
        return true;
    }


    if (keyChar == ',' || keyChar == '.') {
        highlightAuthorityField('#authorityName', false);
        return true;
    }

    if (keyChar == ' ') {
        highlightAuthorityField('#authorityName', false);
        return true;
    }

    event.preventDefault();
    highlightAuthorityField('#authorityName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
    return false;
}

function handleAuthorityNamePaste(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    if (/[a-zA-Z0-9]/.test(keyChar)) {
        highlightAuthorityField('#authorityName', false);
        return true;
    }

    if (keyChar == ',' || keyChar == '.') {
        highlightAuthorityField('#authorityName', false);
        return true;
    }

    if (keyChar == ' ') {
        highlightAuthorityField('#authorityName', false);
        return true;
    }

    event.preventDefault();
    highlightAuthorityField('#authorityName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
    return false;
}

//..view regulatory authority record
function viewRegulatoryAuthorityRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving authority...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findRegulatoryAuthorityRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openRegulatoryAuthorityPanel('Edit Regulatory authority', record, true);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Authority not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            console.error('Error loading type:', error);
            Swal.close();

            Swal.fire({
                title: 'Error',
                text: 'Failed to load authority details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

//..find regulatory authority record from server
function findRegulatoryAuthorityRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/settings/authorities-retrieve/${id}`,
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

//..close panel
function closeRegulatoryAuthorityPanel() {
    $('#authOverlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

//...save new record
function addRegulatoryAuthorityRecordToData(data, newRecord) {
    data.push(newRecord);
}

function initRegulatoryAuthoritySearch() {
    const searchInput = $('#authoritySearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                regulatoryAuthorityTable.setFilter([
                    [
                        { field: "authorityName", type: "like", value: searchTerm },
                        { field: "authorityAlias", type: "like", value: searchTerm },
                        { field: "status", type: "like", value: searchTerm },
                        { field: "addedon", type: "like", value: searchTerm }
                    ]
                ]);
                regulatoryAuthorityTable.setPage(1, true);
            } else {
                regulatoryAuthorityTable.clearFilter();
            }
        }, 300);
    });
}

//..get antiforegery token from meta tag
function getAuthAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightAuthorityField(selector, hasError, message) {
    const $field = $(selector);
    const $formGroup = $field.closest('.form-group, .field-group');
    $field.removeClass('is-invalid');
    $formGroup.find('.field-error').remove();

    if (hasError) {
        $field.addClass('is-invalid');
        if (message) {
            $formGroup.append(`<div class="field-error text-danger small mt-1 text-end">${message}</div>`);
        }
    }
}

