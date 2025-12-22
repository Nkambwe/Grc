
$(document).ready(function () {
    initRegulatoryCategoryTable();
});

let regulatoryCategoryTable;
function initRegulatoryCategoryTable() {
    regulatoryCategoryTable = new Tabulator("#category-table", {
        ajaxURL: "/grc/compliance/support/paged-categories-all",
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
            Swal.fire("Failed to load regulatory categories. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
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
                title: "COMMENTS",
                field: "comments",
                minWidth: 300,
                widthGrow: 4
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
                        <button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteRegulatoryCategoryRecord(${rowData.id})">
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

$('.action-btn-new-category').on('click', function () {
    addRegulatoryCategoryRootRecord();
});

//..add category
function addRegulatoryCategoryRootRecord() {
    openRegulatoryCategoryPanel('New regulatory category', {
        id:0,
        categoryName: '',
        comments:'',
        isActive: false
    }, false);
}

//..open slide panel
function openRegulatoryCategoryPanel(title, record, isEdit) {
    $('#recordId').val(record.id);
    $('#categoryName').val(record.categoryName || '');
    $('#comments').val(record.comments || '');
    $('#isEdit').val(isEdit);
    $('#isActive').prop('checked', record.isActive);

    //..open panel
    $('#panelTitle').text(title);
    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

function saveRegulatoryCategoryRecord(e) {
    e.preventDefault(); 

    let isEdit = $('#isEdit').val();

    //..build record payload from form
    let recordData = {
        id: parseInt($('#recordId').val()) || 0, 
        categoryName: $('#categoryName').val(),
        comments: $('#comments').val(),
        isActive: $('#isActive').is(':checked') ? true : false
    };

    //..validate required fields
    let errors = [];
    if (!recordData.categoryName)
        errors.push("Category name is required.");

    if (!recordData.comments)
        errors.push("Category comment is required.");

    if (errors.length > 0) {
        highlightCategoryField("#categoryName", !recordData.tagName);
        highlightCategoryField("#comments", !recordData.tagDescription);
        Swal.fire({
            title: "Category Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveRegulatoryCategory(isEdit, recordData);
}

function saveRegulatoryCategory(isEdit, payload) {
    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/support/category-update"
        : "/grc/compliance/support/category-create";

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getCategoryAntiForgeryToken()
        },
        success: function (res) {
            Swal.fire("Success", res.message || "Saved successfully.");
            closeRegulatoryCategoryPanel();

            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || (isEdit ? "Category updated successfully" : "Category created successfully"));
            closeRegulatoryCategoryPanel();

            // reload table
            regulatoryCategoryTable.replaceData();
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

function deleteRegulatoryCategoryRecord(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Category",
        text: "Are you sure you want to delete this category?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        const url = `/grc/compliance/support/category-delete/${encodeURIComponent(id)}`;
        $.ajax({
            url: url,
            type: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getCategoryAntiForgeryToken()
            },
            success: function (res) {

                if (res && res.success) {
                    toastr.success(res.message || "Category deleted successfully.");

                    if (typeof regulatoryCategoryTable.replaceData === "function") {
                        regulatoryCategoryTable.replaceData();
                    }

                    else if (typeof regulatoryCategoryTable.ajax !== "undefined") {
                        regulatoryCategoryTable.ajax.reload();
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
            }
        });
    });
}

//..close panel
function closeRegulatoryCategoryPanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

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

//..get antiforegery token from meta tag
function getCategoryAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightCategoryField(selector, hasError, message) {
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
