let permissionSetTable;
function initPermissionSetTable() {
    permissionSetTable = new Tabulator("#adminPermissionSetTable", {
        ajaxURL: "/admin/support/permission-sets/list",
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
        ajaxRequestFunc: (url, config, params) => {
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
                        ["setName", "setDescription"].includes(f.field)
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
            alert("Failed to load permission sets. Please try again.");
        },
        layout: "fitColumns",
        dataTree: false, 
        responsiveLayout: "hide",
        columns: [
            {
                title: "SET NAME",
                field: "setDescription",
                minWidth: 250,
                formatter: (cell) => `<span class="clickable-title" onclick="editSetRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "STATUS",
                field: "isDeleted",
                hozAlign: "center",
                headerHozAlign: "center",
                width: 150,
                formatter: (cell) => {
                    const isDeleted = !!cell.getValue();
                    const color = isDeleted ? "#ED1C24" : "#08A11C";
                    const text = isDeleted ? "Deactivated" : "Active";
                    return `
                        <span style="color:${color}; font-weight:bold;">
                            ${text}
                        </span>`;
                }
            },
            {
                title: "CREATED ON",
                field: "createdOn",
                hozAlign: "center",
                headerHozAlign: "center",
                width: 200,
                formatter: (cell) => {
                    const value = cell.getValue();
                    if (!value) return "";
                    const date = new Date(value);
                    const day = String(date.getDate()).padStart(2, "0");
                    const month = String(date.getMonth() + 1).padStart(2, "0");
                    const year = date.getFullYear();
                    return `${day}-${month}-${year}`;
                }
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteSetRecord(${rowData.id})">
                        <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                        <span>DELETE</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    //..search permission sets
    initSetSearch();
}

function findSetRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/permission-sets-retrieve/${encodeURIComponent(id)}`,
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

function editSetRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving permission set...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    console.log("ID >> " + id);
    findSetRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openSetEditor('Edit Permission set', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Permission set not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load permission set details.' });
        });
}

function addPermissionSetRecord() {
    openSetEditor('New Permission Set', {
        id: 0,
        setName: '',
        setDescription: '',
        isdeleted: false,
        permissions: []
    }, false);

}

function openSetEditor(title, set, isEdit) {

    console.log(set);
    $("#setId").val(set?.id || "");
    $('#isEdit').val(isEdit);
    $("#setName").val(set?.setName || "");
    $("#setDescription").val(set?.setDescription || "");
    $('#isDeleted').prop('checked', set?.isDeleted || false);

    $("#permissionSetListContainer").html("<div class='text-muted p-2'>Loading permissions...</div>");

    if (isEdit) {
        $.ajax({
            url: `/admin/support/permission-sets-retrieve/${set.id}`,
            type: "POST",
            contentType: "application/json",
            success: function (res) {
                const permissions = Array.isArray(res?.data?.permissions)
                    ? res.data.permissions
                    : [];

                    let html = "<div class='permissions-list'>";
                    permissions.forEach(p => {
                        const checked = p.isAssigned ? "checked" : "";
                        html += `
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${p.id}" value="${p.id}" ${checked}>
                            <label class="form-check-label" for="perm-${p.id}">
                                ${p.permissionDescription || p.permissionName}
                            </label>
                        </div>`;
                    });
                    html += "</div>";
                    $("#permissionSetListContainer").html(html);

                },
                error: function () {
                    $("#permissionSetListContainer").html("<div class='text-danger'>Failed to load permissions.</div>");
                }
            });
        }
     else {
        // --- Creating new set: load all permissions (unchecked) ---
        $.get("/admin/support/set-permissions-all", function (res) {
            const permissions = Array.isArray(res?.data) ? res.data : [];

            let html = "<div class='permissions-list'>";
            permissions.forEach(p => {
                const checked = p.isAssigned ? "checked" : "";
                html += `
                <div class="form-check">
                    <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${p.id}" value="${p.id}" ${checked}>
                    <label class="form-check-label" for="perm-${p.id}">
                        ${p.permissionDescription || p.permissionName}
                    </label>
                </div>`;
            });
            html += "</div>";
            $("#permissionSetListContainer").html(html);

        }).fail(() => {
            $("#permissionSetListContainer").html("<div class='text-danger'>Failed to load permissions.</div>");
        });
    }

    $('#panelTitle').text(title);
    $('#permsetOverLay').addClass('active');
    $('#permsetPanel').addClass('active');
}

function deleteSetRecord(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Permission Set",
        text: "Are you sure you want to delete this permission set?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/admin/support/permission-sets-delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getSetAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Permission set deleted successfully.");
                    if (permissionSetTable) {
                        permissionSetTable.replaceData();
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

function savePermissionSet(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();

    let recordData = {
        id: parseInt($('#setId').val()) || 0,
        setName: $('#setName').val()?.trim(),
        setDescription: $('#setDescription').val()?.trim(),
        isDeleted: $('#isDeleted').is(':checked') ? true : false,
        permissions: $(".perm-checkbox:checked").map((_, el) => parseInt(el.value, 10)).get()
    };

    // --- validate required fields ---
    let errors = [];

    if (!recordData.setName)
        errors.push("Permission set name is required.");

    if (!recordData.setDescription)
        errors.push("Permission set description is required.");

    if (recordData.permissions.length == 0)
        errors.push("Please select set permissions.");

    // --- stop if validation fails ---
    if (errors.length > 0) {

        highlightField("#setName", !recordData.setName);
        highlightField("#setDescription", !recordData.setDescription);

        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    console.log("Valid Record:", recordData);
    persistPermissionSet(isEdit, recordData);
}

function persistPermissionSet(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/support/permission-sets-modify"
        : "/admin/support/permission-sets-create";

    Swal.fire({
        title: isEdit ? "Updating permission set..." : "Saving permission set...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    //..debugging
    console.log("Sending data to server:", JSON.stringify(payload));
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getSetAntiForgeryToken()
        },
        success: function (res) {
            //..lose loader and show success message
            Swal.close();
            if (!res.success) {
                //..error from the server
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            if (permissionSetTable) {
                if (isEdit && res.data) {
                    permissionSetTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    permissionSetTable.addRow(res.data, true);
                } else {
                    permissionSetTable.replaceData();
                }
            }

            closeSetPanel();
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

function closeSetPanel() {
    $('#permsetOverLay').removeClass('active');
    $('#permsetPanel').removeClass('active');
}

function initSetSearch() {
    const searchInput = $('#permissionSetSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val().trim();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                permissionSetTable.setFilter([
                    { field: "setDescription", type: "like", value: searchTerm }
                ]);
            } else {
                permissionSetTable.clearFilter();
            }
        }, 300);
    });
}

//..get antiforegery token from meta tag
function getSetAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightField(selector, hasError, message) {
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

$(document).ready(function () {

    toastr.options = {
        closeButton: true,
        progressBar: true,
        positionClass: "toast-top-right",
        timeOut: "3000"
    };

    initPermissionSetTable();

    $('.admin-home').on('click', function () {
        window.location.href = '/admin/support/permission/sets';
    });

    //..new permission set
    $(".action-btn-new-set").on("click", function () {
        addPermissionSetRecord();
    });

    $('#setForm').on('submit', function (e) {
        e.preventDefault();
    });

});
