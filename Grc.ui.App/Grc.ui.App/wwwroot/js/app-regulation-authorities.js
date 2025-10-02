
/*------------------------------------------ initialize table*/

$(document).ready(function () {
    initRegulatoryAuthorityTable();
    initializeAuthorityStatusSelect2();
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
            console.log("=== PROCESSING RESPONSE ===");
            console.log("Params received:", params);
            console.log("Response last_page:", response.last_page);
            console.log("Response data length:", response.data ? response.data.length : 0);

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
                title: "",
                field: "startTab",
                maxWidth: 50,
                headerSort: false,
                formatter: function (cell) {
                    return `<span class="record-tab"></span>`;
                }
            },
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
                        <button class="grc-delete-action" onclick="deleteRegulatoryAuthorityRecord(${rowData.id})">Delete</button>
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

/*------------------------------------------------ export to excel*/
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

/*------------------------------------------------ add new record to pane*/
$('.action-btn-new-authority').on('click', function () {
    addRegulatoryAuthorityRootRecord();
});

// Function to initialize basic Select2 with accessibility fixes
function initializeAuthorityStatusSelect2() {
    $(".js-record-authority").each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            initializeAuthoritySelect2($(this));
        }
    });
}

// Function to initialize a single Select2 element with proper accessibility
function initializeAuthoritySelect2($element) {
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
function addRegulatoryAuthorityRootRecord() {
    openRegulatoryAuthorityPanel('New regulatory authority', {
        id: 0,
        startTab: '',
        authorityName: '',
        authorityAlias: '',
        status: 'Active',
        addedon: "01-02-2025",
        endTab: '',
    }, false);
}

//..open slide panel
function openRegulatoryAuthorityPanel(title, record, isEdit) {
    $('#isEdit').val(isEdit);
    $('#recordId').val(record.id);
    $('#authorityName').val(record.authorityName || '');
    $('#authorityAlias').val(record.authorityAlias || '');
    $('#dpAuthorityStatus').val(record.status || 'Active').trigger('change');

    $('#panelTitle').text(title);

    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

/*------------------------------------------------ save/edit new record*/
function saveRegulatoryAuthorityRecord() {
    let isEdit = $('#isEdit').val();
    //..build record payload from form
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        authorityName: $('#authorityName').val(),
        authorityAlias: $('#authorityAlias').val(),
        status: $('#dpAuthorityStatus').val() || "Active"
    };

    //..call backend
    saveRegulatoryAuthority(isEdit, recordData);
}

//...edit record in data
function updateRegulatoryAuthorityRecordInData(data, updatedRecord) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === updatedRecord.id) {
            Object.assign(data[i], updatedRecord);
            return true;
        }
    }
    return false;
}

/*------------------------------------------------ save via controller*/
function saveRegulatoryAuthority(isEdit, payload) {

    console.log("IsEdit:", isEdit);
    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/settings/authorities-update"
        : "/grc/compliance/settings/authorities-create";

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        success: function (res) {
            if (res && res.data) {
                if (isEdit) {
                    //..update existing authority
                    regulatoryAuthorityTable.updateData([res.data]);
                } else {
                    //..add new authority
                    regulatoryAuthorityTable.addData([res.data], true);
                }
            }

            Swal.fire("Success", res.message || "Saved successfully.");
            closeRegulatoryAuthorityPanel();
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

            Swal.fire(isEdit ? "Update Authority" : "Save Authority", errorMessage);
            console.error("Save error:", error, xhr.responseText);
        }
    });
}

/*------------------------------------------------ delete Record*/
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

        const url = `app-compliance-settings-authorities-delete/${encodeURIComponent(id)}`;

        $.ajax({
            url: url,
            type: 'DELETE',
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Authorities deleted successfully.");
                    regulatoryAuthorityTable.setPage(1, true);
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

function removeRegulationAuthorityRecordFromData(data, id) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === id) {
            data.splice(i, 1);
            return true;
        }
    }
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
                console.error('AJAX Error:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
            }
        });
    });
}

/*------------------------------------------------- Helper Functions*/

//..close panel
function closeRegulatoryAuthorityPanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

//...save new record
function addRegulatoryAuthorityRecordToData(data, newRecord) {
    data.push(newRecord);
}

/*----------------------------------------------- search functionality*/
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

