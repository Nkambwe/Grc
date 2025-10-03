
/*------------------------------------------ initialize table*/

$(document).ready(function () {
    initDocTypeTable();
    initializeDocTypeStatusSelect2();
});

let regulatoryTypeTable;
function initDocTypeTable() {
    regulatoryTypeTable = new Tabulator("#regulatory-doctypes-table", {
        ajaxURL: "/grc/compliance/settings/document-types-all",
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
                    let typeFilter = params.filter.find(f => f.field === "typeName");
                    if (typeFilter) {
                        requestBody.searchTerm = typeFilter.value;
                    }
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
            alert("Failed to load document types. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "",
                field: "startTab",
                maxWidth: 50,
                headerSort: false,
                formatter: function (cell) {
                    return `<span class="record-tab"></span>`;
                }
            },
            {
                title: "DOCUMENT TYPE",
                field: "typeName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                formatter: function (cell) {
                    return `<span class="clickable-title" onclick="viewDocTypeRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                }
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
                        <button class="grc-delete-action" onclick="deleteDocTypeRecord(${rowData.id})">Delete</button>
                    `;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false,
                cssClass: "action-column"
            },
            {
                title: "",
                field: "endTab",
                maxWidth: 50,
                headerSort: false,
                formatter: function (cell) {
                    return `<span class="record-tab"></span>`;
                }
            },
        ]
    });

    // Initialize search
    initDoctypeSearch();
}

//..route to home
$('.action-btn-complianceHome').on('click', function () {
    console.log("Home Button clicked");
    try {
        window.location.href = '/grc/compliance';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

$('.action-btn-new-type').on('click', function () {
    addDocTypeRootRecord();
});

/*------------------------------------------------ export to excel*/
$('#btnDoctypeExportFiltered').on('click', function () {
    $.ajax({
        url: '/grc/compliance/settings/document-types-export',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryTypeTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Regulatory_Document_types.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-type-export').on('click', function () {
    $.ajax({
        url: '/grc/compliance/settings/document-types-export-full',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryTypeTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "RegulatoryDocumentTypes.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

// Function to initialize basic Select2 with accessibility fixes
function initializeDocTypeStatusSelect2() {
    $(".js-record-doctype").each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            initializeDocTypeSelect2($(this));
        }
    });
}

// Function to initialize a single Select2 element with proper accessibility
function initializeDocTypeSelect2($element) {
    const elementId = $element.attr('id');
    const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select Select status';

    $element.select2({
        width: 'resolve',
        placeholder: 'Select a status...',
        allowClear: true,
        escapeMarkup: function (markup) {
            return markup;
        },
        language: {
            noResults: function () {
                return "No status found";
            }
        }
    });
}

//..add type
function addDocTypeRootRecord() {
    openRegulatoryTypePanel('New document type', {
        id: 0,
        startTab: '',
        category: '',
        status: 'Active',
        addedon: "01-02-2025",
        endTab: '',
    }, false);
}

//..open slide panel
function openRegulatoryTypePanel(title, record, isEdit) {
    $('#recordId').val(record.id);
    $('#typeName').val(record.typeName || '');
    $('#isEdit').val(isEdit);
    $('#dpTypeStatus').val(record.status || 'Active').trigger('change');
    $('#panelTitle').text(title);
    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

/*------------------------------------------------ save/edit new record*/
function saveDocumentTypeRecord() {
    let isEdit = $('#isEdit').val();
    //..build record payload from form
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        category: $('#typeName').val(),
        status: $('#dpTypeStatus').val() || "Active"
    };

    //..call backend
    saveDocumentType(isEdit, recordData);
}

//...edit record in data
function updateRegulatoryTypeRecordInData(data, updatedRecord) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === updatedRecord.id) {
            Object.assign(data[i], updatedRecord);
            return true;
        }
    }
    return false;
}

/*------------------------------------------------ save via controller*/
function saveDocumentType(isEdit, payload) {
    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/settings/document-types-update"
        : "/grc/compliance/settings/document-types-create";

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getDoctypeAntiForgeryToken()
        },
        success: function (res) {
            if (res && res.data) {
                if (isEdit) {
                    //..update existing type
                    regulatoryTypeTable.updateData([res.data]);
                } else {
                    //..add new type
                    regulatoryTypeTable.addData([res.data], true);
                }
            }

            Swal.fire("Process document type", res.message || "Saved successfully.");
            closeDoctypePanel();
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
                console.error("Failed to parse error response:", e);
            }

            Swal.fire(isEdit ? "Update document type" : "Save document type", errorMessage);
            console.error("Save error:", error, xhr.responseText);
        }
    });
}

/*------------------------------------------------ delete Record*/
function deleteDocTypeRecord(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Type",
        text: "Are you sure you want to delete this type?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        const url = `/grc/compliance/settings/document-types-delete/${encodeURIComponent(id)}`;
        $.ajax({
            url: url,
            type: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getDoctypeAntiForgeryToken()
            }, 
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Type deleted successfully.");

                    if (typeof regulatoryTypeTable.replaceData === "function") {
                        regulatoryTypeTable.replaceData();
                    }

                    else if (typeof regulatoryTypeTable.ajax !== "undefined") {
                        regulatoryTypeTable.ajax.reload();
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
                console.error("Delete error:", error, xhr.responseText);
            }
        });
    });
}

function removeRegulationDoctypeRecordFromData(data, id) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === id) {
            data.splice(i, 1);
            return true;
        }
    }
    return false;
}

//..view regulatory type record
function viewDocTypeRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving type...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findDoctypeRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openRegulatoryTypePanel('Edit document type', record, true);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Type not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();

            Swal.fire({
                title: 'Error',
                text: 'Failed to load type details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

//..find document type record from server
function findDoctypeRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/settings/types-retrieve/${id}`,
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

/*------------------------------------------------- Helper Functions*/

//..close panel
function closeDoctypePanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

//...save new record
function addDoctpeRecordToData(data, newRecord) {
    data.push(newRecord);
}

//..function to manually reload table data
function initDoctypeSearch() {
    const searchInput = $('#doctypeSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                regulatoryTypeTable.setFilter("globalSearch", "like", searchTerm);
                regulatoryTypeTable.setPage(1, true);
            } else {
                regulatoryTypeTable.clearFilter();
            }
        }, 300);
    });
}

//..get antiforegery token from meta tag
function getDoctypeAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}
