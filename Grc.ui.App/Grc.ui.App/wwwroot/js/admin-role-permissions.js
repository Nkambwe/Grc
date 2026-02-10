let rolePermissionsTable;
function initRolePermissionTable() {
    rolePermissionsTable = new Tabulator("#rolePermissionsTable", {
        ajaxURL: "/admin/support/system-roles/list",
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
        ajaxRequestFunc: function (url, config, params) {
            return new Promise((resolve, reject) => {
                let requestBody = {
                    pageIndex: params.page || 1,
                    pageSize: params.size || 10,
                    searchTerm: "",
                    sortBy: "",
                    sortDirection: "Ascending"
                };

                //..sorting
                if (params.sort && params.sort.length > 0) {
                    requestBody.sortBy = params.sort[0].field;
                    requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
                }

                //..filtering
                if (params.filter && params.filter.length > 0) {
                    let filter = params.filter.find(f =>
                        ["roleName", "roleDescription", "roleGroup"].includes(f.field)
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
            alert("Failed to load system roles. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "", field: "startTab",
                maxWidth: 50,
                headerSort: false,
                frozen: true,
                frozen: true, formatter: () => `<span class="record-tab"></span>`
            },
            {
                title: "ROLE NAME",
                field: "roleName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewRolePermissions(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "ROLE DESCRIPTION",
                field: "roleDescription",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true
            },
            {
                title: "STATUS",
                field: "isDelete",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let value = rowData.isDeleted;
                    let color = value === true ? "#ED1C24" : "#08A11C";
                    let text = value === true ? "Blocked" : "Active";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                width:100%;
                                height:100%;
                                border-radius:50px;
                                color:${color || "#D6D6D6"};
                                font-weight:bold;">
                                ${text}
                            </div>`;
                },
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 250
            },
            { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });

}

function findRolePermissionRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/role-permissions-retrieve/${encodeURIComponent(id)}`,
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

function openRolePermissionEditor(title, record) {
    $("#setId").val(record?.setId || "");
    $('#isEdit').val(isEdit);
    $("#roleName").val(record?.roleName || "");
    $("#roleDescription").val(record?.roleDescription || "");

    //..render permissions
     renderRolePermissions(record); 

    $('#rolePanelTitle').text(title);
    $('#rolePermOverlay').addClass('active');
    $('#rolePermissionPanel').addClass('active');
}

function renderRolePermissions(role) {
    const container = $('#rolePermissionListContainer');
    container.empty();

    if (!role.sets || role.sets.length === 0) {
        container.html('<div class="text-muted">No permission sets available.</div>');
        return;
    }

    let html = '';
    role.sets.forEach(set => {
        const setPermissions = role.permissions ? role.permissions.filter(p => p.setId === set.id) : [];
        const allChecked = setPermissions.length > 0 && setPermissions.every(p => p.isAssigned);

        html += `
            <div class="permission-set-block">
                <div class="permission-set-header">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <h6 class="mb-0">
                                <i class="bi bi-shield-lock"></i> ${set.setDescription }
                            </h6>
                        </div>
                        ${setPermissions.length > 0 ? `
                        <div class="form-check">
                            <input class="form-check-input select-all-checkbox"
                                   type="checkbox"
                                   data-set-id="${set.id}"
                                   ${allChecked ? 'checked' : ''}>
                            <label class="form-check-label text-white small">Select All</label>
                        </div>
                        ` : ''}
                    </div>
                </div>
                <div class="permission-set-body">
        `;

        if (setPermissions.length === 0) {
            html += `<div class="text-muted small text-center py-3"><em>No permissions in this set.</em></div>`;
        } else {
            setPermissions.forEach(p => {
                html += `
                    <div class="form-check permission-item">
                        <input class="form-check-input permission-checkbox"
                               type="checkbox"
                               id="perm-${p.id}"
                               data-permission-id="${p.id}"
                               data-set-id="${p.setId}"
                               ${p.isAssigned ? 'checked' : ''}>
                        <label class="form-check-label" for="perm-${p.id}">
                            <span class="permission-name">${p.permissionDescription }</span>
                        </label>
                    </div>
                `;
            });
        }

        html += `</div></div>`;
    });

    container.html(html);

    // Add select-all functionality
    $('.select-all-checkbox').on('change', function () {
        const setId = $(this).data('set-id');
        const isChecked = $(this).prop('checked');
        $(`.permission-checkbox[data-set-id="${setId}"]`).prop('checked', isChecked);
    });
}

function closeRolePermPanel() {
    $('#rolePermOverlay').removeClass('active');
    $('#rolePermissionPanel').removeClass('active');
}

function viewRolePermissions(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving role permissions...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findRolePermissionRecord(id)
        .then(record => {

            console.log(record);
            Swal.close();
            if (record) {
                openRolePermissionEditor('Edit Role Permissions', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Role not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load role permission details.' });
        });
}

function saveRolePermissions(event) {
    event.preventDefault();

    const roleId = currentRoleId;
    const permissionIds = $('.permission-checkbox:checked')
        .map(function () { return $(this).data('permission-id'); })
        .get();

    $.ajax({
        url: '/admin/support/role-permissions-save',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            roleId: roleId,
            permissionIds: permissionIds
        }),
        success: function (res) {
            if (res.success) {
                Swal.fire('Saved', 'Permissions updated successfully', 'success');
                closeRolePermPanel();
            } else {
                Swal.fire('Error', res.message || 'Failed to save');
            }
        },
        error: function () {
            Swal.fire('Error', 'Server error');
        }
    });
}

function getRolePermissionsAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

$(document).ready(function () {
    initRolePermissionTable();

    $(".action-btn-support-home").on("click", function () {
        window.location.href = '/admin/support';
    });

    $(".action-btn-new-role").on("click", function () {
        window.location.href = '/admin/support/system-roles';
    });

    $('#rolePermissionForm').on('submit', function (e) {
        e.preventDefault();
    });

});