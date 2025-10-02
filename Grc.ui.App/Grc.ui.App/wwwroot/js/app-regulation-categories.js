
/*------------------------------------------ initialize table*/

$(document).ready(function () {
    initRegulatoryCategoryTable();
    initializeCatStatusSelect2();
});

let regulatoryCategoryTable;
function initRegulatoryCategoryTable() {
    regulatoryCategoryTable = new Tabulator("#category-table", {
        ajaxURL: "/grc/compliance/support/categories-all",
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
                    let categoryFilter = params.filter.find(f => f.field === "category");
                    if (categoryFilter) {
                        requestBody.searchTerm = categoryFilter.value;
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
            alert("Failed to load regulatory categories. Please try again.");
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
                title: "CATEGORY",
                field: "category",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                formatter: function (cell) {
                    return `<span class="clickable-title" onclick="viewRegulatoryCategoryRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                }
            },
            {
                title: "STATUS",
                field: "status",
                hozAlign: "center",
                headerHozAlign: "center",
                maxWidth: 200,
                headerSort: true,
                formatter: function (cell) {
                    let value = cell.getValue();
                    let color = {
                        "Active": "#28a745",
                        "Pending": "#ffc107",
                        "Completed": "#6c757d",
                        "Archived": "#dc3545"
                    }[value] || "#6c757d";
                    return `<span style="color: ${color}; font-weight: 600;">${value}</span>`;
                }
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
                        <button class="grc-delete-action" onclick="deleteRegulatoryCategoryRecord(${rowData.id})">Delete</button>
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
    initRegulatoryCategorySearch();
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

$('.action-btn-new-category').on('click', function () {
    addRegulatoryCategoryRootRecord();
});

/*------------------------------------------------ export to excel*/
$('#btnCategoryExportFiltered').on('click', function () {
    $.ajax({
        url: '/grc/compliance/support/category-export',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryCategoryTable.getData()),
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

$('.action-btn-category-export').on('click', function () {
    $.ajax({
        url: '/grc/compliance/support/category-export-full',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(regulatoryCategoryTable.getData()),
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

/*------------------------------------------------ add new record to pane*/
$('.action-btn-new-category').on('click', function () {
    addRegulatoryCategoryRootRecord();
});

// Function to initialize basic Select2 with accessibility fixes
function initializeCatStatusSelect2() {
    $(".js-record-category").each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            initializeCatSelect2($(this));
        }
    });
}

// Function to initialize a single Select2 element with proper accessibility
function initializeCatSelect2($element) {
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

//..add category
function addRegulatoryCategoryRootRecord() {
    openRegulatoryCategoryPanel('New regulatory category', {
        id:0,
        startTab: '',
        category: '',
        status: 'Active',
        addedon: "01-02-2024",
        endTab: '',
    }, false);
}

//..open slide panel
function openRegulatoryCategoryPanel(title, record, isEdit) {
    $('#recordId').val(record.id);
    $('#categoryName').val(record.category || '');
    $('#isEdit').val(isEdit);
    $('#dpCategoryStatus').val(record.status || 'Active').trigger('change');

    $('#panelTitle').text(title);

    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

/*------------------------------------------------ save/edit new record*/
function saveRegulatoryCategoryRecord() {
    let isEdit = $('#isEdit').val();
    //..build record payload from form
    let recordData = {
        id: parseInt($('#recordId').val()) || 0, 
        category: $('#categoryName').val(),
        status: $('#dpCategoryStatus').val() || "Active"
    };

    //..call backend
    saveRegulatoryCategory(isEdit, recordData);
}

//...edit record in data
function updateRegulatoryCategoryRecordInData(data, updatedRecord) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === updatedRecord.id) {
            Object.assign(data[i], updatedRecord);
            return true;
        }
        if (data[i]._children) {
            if (updateRegulatoryCategoryRecordInData(data[i]._children, updatedRecord)) {
                return true;
            }
        }
    }
    return false;
}

/*------------------------------------------------ save via controller*/
function saveRegulatoryCategory(isEdit, payload) {
    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/support/category-update"
        : "/grc/compliance/support/category-create";

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        success: function (res) {
            if (res && res.data) {
                if (isEdit) {
                    //..update existing category
                    regulatoryCategoryTable.updateData([res.data]);
                } else {
                    //..add new category
                    regulatoryCategoryTable.addData([res.data], true);
                }
            }

            Swal.fire("Success", res.message || "Saved successfully.");
            closeRegulatoryCategoryPanel();
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

            Swal.fire(isEdit ? "Update Category" : "Save Category", errorMessage);
            console.error("Save error:", error, xhr.responseText);
        }
    });
}

/*------------------------------------------------ delete Record*/
function deleteRegulatoryCategoryRecord(id) {
    Swal.fire({
        title: "Delete Category",
        text: "Are you sure you want to delete this category?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/grc/compliance/support/category-delete/' + id,
                type: 'DELETE',
                success: function (res) {
                    Swal.fire("Deleted!", res.message || "Category deleted successfully.");
                    regulatoryCategoryTable.setPage(1, true); 
                },
                error: function (xhr, status, error) {
                    Swal.fire("Error", error);
                    console.error("Delete error:", error, xhr.responseText);
                }
            });
        }
    });
}

function removeRegulationCategoryRecordFromData(data, id) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === id) {
            data.splice(i, 1);
            return true;
        }
    }
    return false;
}

//..view regulatory category record
function viewRegulatoryCategoryRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving category...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findRegulatoryCategoryRecord(id)
        .then(record => {
            Swal.close(); 
            if (record) {
                openRegulatoryCategoryPanel('Edit Regulatory category', record, true);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Category not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            console.error('Error loading category:', error);
            Swal.close(); 

            Swal.fire({
                title: 'Error',
                text: 'Failed to load category details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

//..find regulatory category record from server
function findRegulatoryCategoryRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/support/retrieve-category/${id}`, 
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
function closeRegulatoryCategoryPanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

//...save new record
function addRegulatoryCategoryRecordToData(data, newRecord) {
    data.push(newRecord);
}

/*----------------------------------------------- search functionality*/
function initRegulatoryCategorySearch() {
    let typingTimer;
    $("#categorySearchbox").on("keyup", function () {
        clearTimeout(typingTimer);
        let searchValue = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchValue.length >= 2) {  
                regulatoryCategoryTable.setFilter("globalSearch", "like", searchTerm);
                regulatoryCategoryTable.setPage(1, true);
            } else {
                regulatoryCategoryTable.clearFilter();
            }
        }, 300);
    });
}
