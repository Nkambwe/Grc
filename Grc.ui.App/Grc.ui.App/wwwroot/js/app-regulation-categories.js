
/*------------------------------------------ initialize table*/

$(document).ready(function () {
    initRegulatoryCategoryTable();
    initializeCatStatusSelect2();
});

let regulatoryCategoryTable;
let nextRegulatoryCategoryId = 1000;
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

$('.action-btn-newCategory').on('click', function () {
    addRegulatoryCategoryRootRecord();
});

/*------------------------------------------------ add new record to pane*/
$('.action-btn-newCategory').on('click', function () {
    exportToExcel();
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

//..add regiter
function addRegulatoryCategoryRootRecord() {
    openRegulatoryCategoryPanel('New regulatory category', {
        id: ++nextRegulatoryCategoryId,
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
    $('#dpCategoryStatus').val(record.status || 'Active');

    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

/*------------------------------------------------ export to excel*/
function exportToExcel() {
    //TODo --export to excel
}

/*------------------------------------------------ save/edit new record*/
function saveRegulatoryCategoryRecord() {
    let isEdit = $('#isEdit').val() === 'true';
    let recordData = {
        id: parseInt($('#recordId').val()),
        startTab: '',
        category: $('#categoryName').val(),
        status: "Active",
        addedon: "01-02-2024",
        endTab: '',

    };

    //..save via controller
    saveRegulatoryCategoryRegister(recordData, isEdit, function (success) {
        if (success) {
            if (isEdit) {
                updateRegulatoryCategoryRecordInData(sampleRegulatoryCatoryData, recordData);
            } else {
                addRegulatoryCategoryRecordToData(sampleRegulatoryCatoryData, recordData);
            }

            regulatoryCategoryTable.replaceData(sampleRegulatoryCatoryData);
            closeRegulatoryCategoryPanel();

            Swal.fire({
                title: "Save Regulatory category",
                text: 'category saved successfully!',
                confirmButtonColor: "#450354",
                confirmButtonText: "OK"
            });
        }
    });
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

/*----------------------------------------------- search functionality*/
function initRegulatoryCategorySearch() {
    const searchInput = $('#categorySearchbox');

    searchInput.on('input', function (e) {
        const searchTerm = $(this).val().toLowerCase();
        regulatoryCategoryTable.setFilter([
            [
                { field: "category", type: "like", value: searchTerm },
                { field: "status", type: "like", value: searchTerm },
                { field: "addedon", type: "like", value: searchTerm }
            ]
        ]);
    });
}

/*------------------------------------------------ save via controller*/
function saveRegulatoryCategoryRegister(data, isEdit, callback) {
    // This simulates an AJAX call to your ASP.NET Core controller
    // Replace with actual $.ajax call:
    /*
    $.ajax({
        url: isEdit ? '/Records/Update' : '/Records/Create',
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function(response) {
            callback(true);
        },
        error: function(xhr, status, error) {
            alert('Error saving record: ' + error);
            callback(false);
        }
    });
    */

    // Simulated success
    setTimeout(() => callback(true), 100);
}

/*------------------------------------------------ delete Record*/
function deleteRegulatoryCategoryRecord(id) {
    // Confirmation dialog
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
            Swal.fire({
                title: 'Deleting...',
                text: 'Please wait while the category is being deleted.',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            // Simulate AJAX delete
            /*
            $.ajax({
                url: '/Records/Delete/' + id,
                type: 'DELETE',
                success: function (resList) {
                        const res = resList[0] || {};
                        const icon = res.status === "false" ? "error" : "success";
                        const title = res.message || (res.status === "false" ? "Error Deleting Voucher" : "Success");
                        const text = res.details || (res.status === "false" ? "Could not delete category." : "Category deleted successfully.");

                        Swal.fire({
                            title: title,
                            text: text,
                            confirmButtonColor: res.status === "false" ? "#f42f3f" : "#450354",
                            confirmButtonText: "OK"
                        });

                        if (res.status !== "false") {
                            $(`button[data-id='${voucher_id}']`).closest('tr').remove();
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("AJAX error:", error);
                        console.log("Server response:", xhr.responseText);
                        Swal.fire({
                            title: "Request Failed",
                            text: "Could not connect to the server. Please try again later.",
                            confirmButtonColor: "#f41369",
                            confirmButtonText: "OK"
                        });
                    }
            });
            */
            removeRegulationCategoryRecordFromData(sampleRegulatoryCatoryData, id);
            regulatoryCategoryTable.replaceData(sampleRegulatoryCatoryData);
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

/*------------------------------------------------- Helper Functions*/ 
function findRegulatoryCategoryRecord(data, id) {
    for (let item of data) {
        if (item.id === id) return item;
        if (item._children) {
            let found = findRegulatoryCategoryRecord(item._children, id);
            if (found) return found;
        }
    }
    return null;
}

//..view/edit Record
function viewRegulatoryCategoryRecord(id) {
    let record = findRegulatoryCategoryRecord(sampleRegulatoryCatoryData, id);
    if (record) {
        openRegulatoryCategoryPanel('Edit Regulatory category', record, true);
    }
}

//..close panel
function closeRegulatoryCategoryPanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

//...save new record
function addRegulatoryCategoryRecordToData(data, newRecord) {
    data.push(newRecord);
}

//..search functionality
function initRegulatoryCategorySearch() {
    $("#category-search-input").on("keyup", function () {
        let searchValue = $(this).val();
        regulatoryCategoryTable.setFilter("category", "like", searchValue);
    });
}

//..function to manually reload table data
function initRegulatoryCategorySearch() {
    let typingTimer;
    $("#categorySearchbox").on("keyup", function () {
        clearTimeout(typingTimer);
        let searchValue = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchValue.length >= 2) {  
                regulatoryCategoryTable.setFilter("category", "like", searchValue);
                regulatoryCategoryTable.setPage(1, true);
            } else {
                regulatoryCategoryTable.clearFilter();
            }
        }, 300);
    });
}




