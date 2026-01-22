let controlsTable;

function initControlList() {
    controlsTable = new Tabulator("#controls-table", {
        ajaxURL: "/grc/register/compliance-controls/paged-list",
        paginationMode: "remote",
        filterMode: "remote",
        sortMode: "remote",
        dataTree: true,
        dataTreeChildField: "_children",
        dataTreeStartExpanded: false,
        dataTreeCollapseElement: "<span><i class='mdi mdi-chevron-down'></i></span>",
        dataTreeExpandElement: "<span><i class='mdi mdi-chevron-right'></i></span>",
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
                        ["categoryName"].includes(f.field)
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
            const treeData = (response.data || []).map(category => ({
                rowType: "category",
                categoryId: category.categoryId,
                categoryName: category.categoryName,
                isExcluded: category.isExcluded != null ? (category.isExcluded ? "YES" : "NO") : "",
                isDeleted: category.isDeleted != null ? (category.isDeleted ? "NO" : "YES") : "",
                notes: category.notes != null ? category.notes : "",
                _children: (category.controls || []).map(control => ({
                    rowType: "control",
                    itemId: control.itemId,
                    categoryName: control.controlName || "Unnamed Control",
                    isExcluded: control.isExcluded != null ? (control.isExcluded ? "YES" : "NO") : "",
                    isDeleted: control.isDeleted != null ? (control.isDeleted ? "NO" : "YES") : "",
                    notes: control.notes != null ? control.notes : "",
                }))
            }));

            return {
                data: treeData,
                last_page: response.last_page || 1,
                total_records: response.total_records || treeData.length
            };
        },
        ajaxError: function (error) {
            console.error("Tabulator AJAX Error:", error);
            alert("Failed to load compliance controls. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "COMPLIANCE CONTROL",
                field: "categoryName",
                headerFilter: "input",
                minWidth: 250,
                widthGrow: 2,
                formatter: function (cell) {
                    return `<span class="clickable-title">${cell.getValue()}</span>`;
                },
                cellClick: function (e, cell) {
                    const rowData = cell.getRow().getData();
                    console.log(rowData)
                    if (rowData.rowType === "control" && rowData.itemId) {
                        viewItem(rowData.itemId);
                    } else {
                        viewCategory(rowData.categoryId);
                    }
                }
            },
            {
                title: "EXCLUDED",
                field: "isExcluded",
                headerFilter: "input",
                minWidth: 150,
                formatter: yesNoFormatter,
                hozAlign: "left"
            },
            {
                title: "ACTIVE",
                field: "isDeleted",
                minWidth: 150,
                formatter: percentTextFormatter,
                hozAlign: "left"
            },
            {
                title: "NOTES",
                field: "notes",
                headerFilter: "input",
                minWidth: 250,
                widthGrow: 2,
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    const rowData = cell.getRow().getData();
                    if (rowData.rowType === "control") {
                        const id = rowData.itemId || '';
                        const deleted = rowData.isDeleted === "YES" ? true : false;
                        const deleteVal = !deleted ? 'disabled' : '';
                        return `
                        <button class="grc-table-btn grc-btn-delete grc-delete-action ${deleteVal}" 
                                ${deleteVal} 
                                data-id="${id}">
                            <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                            <span>DELETE</span>
                        </button>`;
                    } else {
                        const id = rowData.categoryId || '';
                        return `
                <button class="grc-table-btn grc-view-action grc-add-action" data-id="${id}">
                    <span><i class="mdi mdi-filter-cog-outline" aria-hidden="true"></i></span>
                    <span>NEW ITEM</span>
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

    // Search init
    initControlearch();
}

function addItem(parentId) {
    openItemPanel('NEW ITEM', {
        itemId: 0,
        parentId: parentId,
        itemName: '',
        itemComments: '',
        isItemDeleted: false,
        isItemExcluded: false
    }, false);
}

function viewItem(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving control item...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findItem(id)
        .then(record => {
            Swal.close();
            if (record) {
                openItemPanel('ITEM DETAILS', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Control item record not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load control item details.' });
        });
}

function findItem(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/compliance-controlitems/retrieve-item/${id}`,
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

function openItemPanel(title, item, isEdit) {
    $('#itemId').val(item.itemId);
    $('#parentId').val(item.parentId);
    $('#itemName').val(item.itemName || '');
    $('#itemComments').val(item.itemComments || '');
    $('#isItemEdit').val(isEdit);
    $('#isItemExcluded').prop('checked', item.isItemExcluded);
    $('#isItemDeleted').prop('checked', item.isItemDeleted);

    //..open panel
    $('#itemTitle').text(title);
    $('.control-panel-overlay').addClass('active');
    $('#itemPanel').addClass('active');
}

function saveItem(e) {
    e.preventDefault();

    let isEdit = $('#isItemEdit').val();
    let recordData = {
        id: parseInt($('#itemId').val()) || 0,
        categoryId: parseInt($('#parentId').val()) || 0,
        itemName: $('#itemName').val()?.trim(),
        comments: $('#itemComments').val()?.trim(),
        isDeleted: $('#isItemDeleted').prop('checked'),
        isExcluded: $('#isItemExcluded').prop('checked'),
    };

    let errors = [];
    if (!recordData.itemName)
        errors.push("Item name is required.");
    if (!recordData.comments)
        errors.push("Comment is required.");

    if (errors.length > 0) {
        highlightControlField("#itemName", !recordData.itemName);
        highlightControlField("#itemComments", !recordData.comments);
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..save item to database
    saveItemRecord(isEdit, recordData);
}

function saveItemRecord(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/register/controlitems-update"
        : "/grc/compliance/register/controlitems-create";

    Swal.fire({
        title: isEdit ? "Updating item..." : "Saving item...",
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
            'X-CSRF-TOKEN': getControlToken()
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
            Swal.fire(res.message || (isEdit ? "Item updated successfully" : "Item created successfully"));
            closeItem();

            // reload table
            controlsTable.replaceData();
            
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

function deleteItem(id) {
    if (!id && id !== 0) {
        toastr.error("Item ID not provided");
        return;
    }

    Swal.fire({
        title: "Delete Item",
        text: "Are you sure you want to delete this item?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/grc/compliance/register/controlitems-delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getControlToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Control item deleted successfully.");
                    if (roleGroupTable) {
                        roleGroupTable.replaceData();
                    }
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function (xhr, status, error) {
                toastr.error(xhr.responseJSON?.message || "Request failed.");
            }
        });
    });
}

function closeItem() {
    $('.control-panel-overlay').removeClass('active');
    $('#itemPanel').removeClass('active');
}

function viewCategory(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving control category...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findCategory(id)
        .then(record => {
            Swal.close();
            if (record) {
                openCategoryPanel('CATEGORY DETAILS', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Control category record not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load control category details.' });
        });
}

function findCategory(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/compliance-controls/retrieve-control/${id}`,
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

function openCategoryPanel(title, category, isEdit) {
    $('#categoryId').val(category.categoryId);
    $('#categoryName').val(category.categoryName || '');
    $('#categoryComments').val(category.categoryComments || '');
    $('#isControlEdit').val(isEdit);
    $('#isCategoryExcluded').prop('checked', category.isCategoryExcluded);
    $('#isCategoryDeleted').prop('checked', category.isCategoryDeleted);

    // Populate control items list
    populateControlItems(category.items || []);

    //..open panel
    $('#controlTitle').text(title);
    $('.control-panel-overlay').addClass('active');
    $('#controlPanel').addClass('active');
}

function populateControlItems(items) {
    const $list = $('.control-item-list');
    $list.empty(); // Clear existing items

    if (items.length === 0) {
        $list.append('<li class="text-muted no-control-items">No items available</li>');
        return;
    }

    items.forEach(item => {
        const isChecked = !item.isItemExcluded; 
        const nameClass = item.isItemDeleted ? 'text-decoration-line-through' : '';

        const listItem = `
            <li class="control-item mb-2">
                <div class="form-check">
                    <input class="form-check-input" 
                           type="checkbox" 
                           id="item_${item.itemId}" 
                           data-item-id="${item.itemId}"
                           ${isChecked ? 'checked' : ''}>
                    <label class="form-check-label ${nameClass}" for="item_${item.itemId}">
                        ${item.itemName}
                    </label>
                </div>
            </li>
        `;

        $list.append(listItem);
    });
}

$('.action-btn-control-new').on('click', function () {
    openCategoryPanel('NEW CATEGORY', {
        categoryId: 0,
        categoryName: '',
        categoryComments: '',
        isCategoryDeleted: false,
        isCategoryExcluded: false
    }, false);
});

function saveControl(e) {
    e.preventDefault();

    let isEdit = $('#isControlEdit').val();
    let recordData = {
        id: parseInt($('#categoryId').val()) || 0,
        categoryName: $('#categoryName').val()?.trim(),
        comments: $('#categoryComments').val()?.trim(),
        isDeleted: $('#isCategoryDeleted').prop('checked'),
        isExcluded: $('#isCategoryExcluded').prop('checked'),
    };

    let errors = [];
    if (!recordData.categoryName)
        errors.push("Category name is required.");
    if (!recordData.comments)
        errors.push("Comment is required.");

    if (errors.length > 0) {
        highlightControlField("#categoryName", !recordData.categoryName);
        highlightControlField("#categoryComments", !recordData.comments);
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..save item to database
    saveCategoryRecord(isEdit, recordData);
}

function saveCategoryRecord(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/register/controls-update"
        : "/grc/compliance/register/controls-create";

    Swal.fire({
        title: isEdit ? "Updating category..." : "Saving category...",
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
            'X-CSRF-TOKEN': getControlToken()
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

            Swal.fire(res.message || (isEdit ? "Category updated successfully" : "Category created successfully"));
            closeControl();

            // reload table
            controlsTable.replaceData();
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
function closeControl() {
    $('.control-panel-overlay').removeClass('active');
    $('#controlPanel').removeClass('active');
}

function initControlearch() {
    // Add your search implementation here
}

$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

function percentTextFormatter(cell) {
    const value = cell.getValue();
    if (value === undefined || value === null) return "";
    return value;
}

function yesNoFormatter(cell) {
    const value = cell.getValue();
    if (value === "" || value == null) {
        return "";
    }

    //..show Yes/No
    return value ? "YES" : "NNOo";
}

$(document).on('click', '.grc-delete-action', function () {
    const id = $(this).data('id');
    deleteItem(id);
});

$(document).on('click', '.grc-add-action', function () {
    const id = $(this).data('id');
    addItem(id);
});

$(document).ready(function () {
    initControlList();

    $('#controlForm').on('submit', function (e) {
        e.preventDefault();
    })

    $('#itemForm').on('submit', function (e) {
        e.preventDefault();
    })
});

function highlightControlField(selector, hasError, message) {
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

function getControlToken() {
    return $('meta[name="csrf-token"]').attr('content');
}
