let permissionSetTable;
function initPermissionSetTable() {
    permissionSetTable = new Tabulator("#adminPermissionSetTable", {
        ajaxURL: "/admin/support/permission-sets-all",
        ajaxConfig: "POST",
        ajaxContentType: "json",
        layout: "fitColumns",
        dataTree: false, // no nested tree now
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
        ],
        ajaxRequestFunc: (url, config, params) => {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: url,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({
                        searchTerm: $("#permissionSetSearchbox").val() || ""
                    }),
                    success: function (response) {
                        resolve(response.data || []);
                    },
                    error: function (xhr, status, error) {
                        console.error("Error loading permission sets:", error);
                        reject(error);
                    }
                });
            });
        }
    });

    // Search box event
    $("#permissionSetSearchbox").on("input", function () {
        permissionSetTable.setData();
    });
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
    $("#setId").val(set?.id || "");
    $('#isEdit').val(isEdit);
    $("#setName").val(set?.setName || "");
    $("#setDescription").val(set?.setDescription || "");
    $('#isDeleted').prop('checked', set?.isDeleted || false);

    console.log(`Opening ${isEdit ? "edit" : "new"} permission set editor for ID: ${set?.id || "(new)"}`);

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
    $('.overlay').addClass('active');
    $('#setPanel').addClass('active');
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
            type: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getActAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Permission set deleted successfully.");
                    policyRegisterTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function () {
                toastr.error("Request failed.");
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
        highlightField("#set Description", !recordData.setDescription);

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

            if (res && res.data) {
                if (isEdit) {
                    initPermissionSetTable.updateData([res.data]);
                } else {
                    initPermissionSetTable.addData([res.data], true);
                }
            }

            Swal.fire({
                title: isEdit ? "Updating permission set..." : "Saving permission set...",
                text: res.message || "Saved successfully.",
                timer: 2000,
                showConfirmButton: false
            });

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
    console.log('Button clicked');
    $('.overlay').removeClass('active');
    $('#setPanel').removeClass('active');
}

//..get antiforegery token from meta tag
function getSetAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

$(document).ready(function () {
    initPermissionSetTable();

    $('.action-btn-nome').on('click', function () {
        window.location.href = '/admin/support/permission/sets';
    });

    //..new permission set
    $(".action-btn-new-set").on("click", function () {
        console.log(`Clicked new set`);
        addPermissionSetRecord();
    });

    $('#setForm').on('submit', function (e) {
        e.preventDefault();
    });

});
