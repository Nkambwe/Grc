
$(document).ready(function () {
    initRegulatoryTypeTable();
});

let regulatoryTypeTable;

function initRegulatoryTypeTable() {
    regulatoryTypeTable = new Tabulator("#regulatory-types-table", {
        ajaxURL: "/grc/compliance/settings/types-all",
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
            Swal.fire("Failed to load regulatory types. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "TYPE NAME",
                field: "typeName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                formatter: function (cell) {
                    return `<span class="clickable-title" onclick="viewRegulatoryTypeRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
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
                        <button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteRegulatoryTypeRecord(${rowData.id})">
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
    initRegulatoryTypeSearch();
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
    addRegulatoryType();
});

//..add type
function addRegulatoryType() {
    openRegulatoryTypePanel('New regulatory type', {
        id: 0,
        typeName: '',
        isActive: false
    }, false);
}

$('#btnTypeExportFiltered').on('click', function () {
    $.ajax({
        url: '/grc/compliance/settings/types-export',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryTypeTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Regulatory_Categories.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-type-export').on('click', function () {
    $.ajax({
        url: '/grc/compliance/settings/types-export-full',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryTypeTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "RegulatoryCategories.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-newType').on('click', function () {
    exportToExcel();
});

//..open slide panel
function openDocumentTypePanel(title, record, isEdit) {
    $('#recordId').val(record.id);
    $('#typeName').val(record.typeName || '');
    $('#isEdit').val(isEdit);
    $('#isActive').prop('checked', record.isActive);

    //..open window
    $('#panelTitle').text(title);
    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

function saveRegulatoryTypeRecord(e) {
    e.preventDefault(); 

    let isEdit = $('#isEdit').val();
    //..build record payload from form
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        typeName: $('#typeName').val(),
        isActive: $('#isActive').is(':checked') ? true : false,
    };

    //..call backend
    saveRegulatoryType(isEdit, recordData);
}

function saveRegulatoryType(isEdit, payload) {
    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/settings/types-update"
        : "/grc/compliance/settings/types-create";

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getRegulationTypeAntiForgeryToken()
        },
        success: function (res) {
            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || (isEdit ? "Regulatory type updated successfully" : "Regulatory type created successfully"));
            closeRegulatoryTypePanel();

            //..reload table
            regulatoryTypeTable.replaceData();
        },
        error: function (xhr, status, error) {
            var errorMessage = error;
            try {
                var response = JSON.parse(xhr.responseText);
                if (response.message) {
                    errorMessage = response.message;
                }
            } catch (e) {
                //..if parsing fails, use the default error
                errorMessage = "Unexpected error occurred";
            }

            Swal.fire(isEdit ? "Update Type" : "Save Type", errorMessage);
        }

    });
}

function deleteRegulatoryTypeRecord(id) {
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

        const url = `/grc/compliance/settings/types-delete/${encodeURIComponent(id)}`;
        $.ajax({
            url: url,
            type: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getRegulationTypeAntiForgeryToken()
            },
            success: function (res) {

                if (!res || res.success !== true) {
                    toastr.success(res.message || "Document Type deleted successfully.");
                    return;
                }

                toastr.error(res?.message || "Delete failed.");
               
                //..reload table
                regulatoryTypeTable.replaceData();
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

function removeRegulationTypeRecordFromData(data, id) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === id) {
            data.splice(i, 1);
            return true;
        }
    }
    return false;
}

//..view regulatory type record
function viewRegulatoryTypeRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving type...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findRegulatoryTypeRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openRegulatoryTypePanel('Edit Regulatory type', record, true);
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

//..find regulatory category record from server
function findRegulatoryTypeRecord(id) {
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

//..close panel
function closeRegulatoryTypePanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

//...save new record
function addRegulatoryTypeRecordToData(data, newRecord) {
    data.push(newRecord);
}

//..function to manually reload table data
function initRegulatoryTypeSearch() {
    const searchInput = $('#typeSearchbox');
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
function getRegulationTypeAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

