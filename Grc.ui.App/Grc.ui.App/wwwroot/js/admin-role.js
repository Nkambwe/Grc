
let roleTable;
function initRoleTable() {

    roleGroupTable = new Tabulator("#adminRolesTable", {
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
                formatter: (cell) => `<span class="clickable-title" onclick="editRole(${cell.getRow().getData().id})">${cell.getValue()}</span>`
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
                title: "ROLE GROUP",
                field: "groupName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewGroup(${cell.getRow().getData().groupId})">${cell.getValue()}</span>`
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
            {
                title: "APPROVED",
                field: "isApproved",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let value = rowData.isVerified;
                    let color = value === true ? "#08A11C" : "#FFAB26";
                    let text = value === true ? "Approved" : "Pending";
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
            {
                title: "VERIFIED",
                field: "isVerified",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let value = rowData.isVerified;
                    let color = value === true ? "#08A11C" : "#FFAB26";
                    let text = value === true ? "Verified" : "Pending";
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
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteRole(${rowData.id})">
                        <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                        <span>DELETE</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });

    // Search roles
    initRoleSearch();
}

function initGroupListSelect2() {
    $(".js-role-groups").select2({
        width: '100%',
        placeholder: 'Select Group...',
        allowClear: true,
        dropdownParent: $('#dpRoleGroups').parent() 
    });
}

function findRole(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/roles-retrieve/${encodeURIComponent(id)}`,
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

function createRole() {
    openRoleEditor('New Role', {
        id: 0,
        roleName: '',
        roleDescription: '',
        groupId: 0,
        isDeleted: false,
        isVerified: true,
        isApproved: true,
    }, false);
}

function saveRole(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();

    let recordData = {
        id: parseInt($('#roleId').val()) || 0,
        roleName: $('#roleName').val()?.trim(),
        roleDescription: $('#roleDescription').val()?.trim(),
        groupId: parseInt($('#dpRoleGroups').val()) || 0,
        isDeleted: $('#isRoleDeleted').is(':checked') ? true : false,
        isVerified: $('#isVerified').is(':checked') ? true : false,
        isApproved: $('#isApproved').is(':checked') ? true : false
    };

    // --- validate required fields ---
    let errors = [];

    //..role name validation
    if (!recordData.roleName) {
        errors.push("Role name is required.");
    } else if (/[^a-zA-Z0-9\s]/.test(recordData.roleName)) {
        errors.push("Role name cannot contain special characters.");
    }

    //..role description validation
    if (!recordData.roleDescription) {
        errors.push("Role description is required.");
    } else if (/[^a-zA-Z0-9\s]/.test(recordData.roleDescription)) {
        errors.push("Role description cannot contain special characters.");
    }

    //..role group validation
    if (recordData.groupId == 0) {
        errors.push("Role Group is required.");
    }

    // --- stop if validation fails ---
    if (errors.length > 0) {

        // Use correct selectors and display errors
        highlightRoleErrorField("#roleName", !recordData.roleName || /[^a-zA-Z0-9\s]/.test(recordData.roleName), "Role name cannot contain special characters.");
        highlightRoleErrorField("#roleDescription", !recordData.roleDescription || /[^a-zA-Z0-9\s]/.test(recordData.roleDescription), "Role description cannot contain special characters.");
        highlightRoleErrorField("#dpRoleGroups", recordData.groupId == 0, "Role Group is required.");

        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    console.log("Valid Record:", recordData);
    persistRole(isEdit, recordData);
}

function openRoleEditor(title, role, isEdit) {
    // Populate form fields
    $("#roleId").val(role?.id || "");
    $("#isEdit").val(isEdit);
    $("#roleName").val(role?.roleName || "");
    $("#roleDescription").val(role?.roleDescription || "");
    $("#dpRoleGroups").val(role?.groupId || 0).trigger('change');
    //$("#dpRoleGroups").val(role?.groupId || 0).trigger('change.select2');
    $('#isRoleDeleted').prop('checked', role?.isDeleted || false);
    $('#isVerified').prop('checked', role?.isVerified || true);
    $('#isApproved').prop('checked', role?.isApproved || true);

    if (isEdit) {
        console.log("Edit Window");
        $('#edit-pane').show();
    } else {
        console.log("New Window");
        $('#parentInfo').hide();
    }

    // Show overlay panel
    $('#rolePanelTitle').text(title);
    $('.role-overlay').addClass('active');
    $('#rolePanel').addClass('active');
}

function editRole(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving role...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findRole(id)
        .then(record => {
            Swal.close();
            if (record) {
                openRoleEditor('Edit Role', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Role not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load role details.' });
        });
}

function viewGroup(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving role group...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findRoleGroup(id)
        .then(record => {
            Swal.close();
            if (record) {
                openRoleGroupEditor('Edit Role Group', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Role group not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load role group details.' });
        });
}

function findRoleGroup(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/role-groups-retrieve/${encodeURIComponent(id)}`,
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

function openRoleGroupEditor(title, group, isEdit) {
    $("#groupId").val(group?.id || "");
    $('#isEdit').val(isEdit);
    $("#groupName").val(group?.groupName || "");
    $("#groupDescription").val(group?.groupDescription || "");
    $("#groupScope").val(group?.groupScope || "");
    $("#groupCategory").val(group?.groupCategory || "");
    $("#groupType").val(group?.groupType || "");
    $("#department").val(group?.department || "");
    $('#isRoleDeleted').prop('checked', group?.isDeleted || false);

    $("#groupListContainer").html("<div class='text-muted p-2'>Loading roles...</div>");

    if (isEdit) {
        $.ajax({
            url: `/admin/support/role-groups-retrieve/${group.id}`,
            type: "POST",
            contentType: "application/json",
            success: function (res) {
                const roles = Array.isArray(res?.data?.roles)
                    ? res.data.roles
                    : [];

                let html = "<div class='role-group-list'>";
                roles.forEach(p => {
                    const checked = p.isAssigned ? "checked" : "";
                    html += `
                        <div class="form-check">
                            <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${p.id}" value="${p.id}" ${checked}>
                            <label class="form-check-label" for="perm-${p.id}">
                                ${p.roleDescription || p.roleName}
                            </label>
                        </div>`;
                });
                html += "</div>";
                $("#groupListContainer").html(html);

            },
            error: function () {
                $("#groupListContainer").html("<div class='text-danger'>Failed to load roles.</div>");
            }
        });
    }
    else {
        console.log("load all roles");
        // --- Creating new set: load all roles (unchecked) ---
        $.get("/admin/support/roles-all", function (res) {
            const roles = Array.isArray(res?.data) ? res.data : [];

            let html = "<div class='role-group-list'>";
            roles.forEach(p => {
                const checked = p.isAssigned ? "checked" : "";
                html += `
                <div class="form-check">
                    <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${p.id}" value="${p.id}" ${checked}>
                    <label class="form-check-label" for="perm-${p.id}">
                        ${p.roleDescription || p.roleName}
                    </label>
                </div>`;
            });
            html += "</div>";
            $("#groupListContainer").html(html);

        }).fail(() => {
            $("#groupListContainer").html("<div class='text-danger'>Failed to load roles.</div>");
        });
    }

    $('#panelTitle').text(title);
    $('.overlay').addClass('active');
    $('#setPanel').addClass('active');
}

function persistRole(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/support/roles-modify"
        : "/admin/support/roles-create";

    Swal.fire({
        title: isEdit ? "Updating system role..." : "Saving system role...",
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
            'X-CSRF-TOKEN': getRoleAntiForgeryToken()
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

            if (roleTable) {
                if (isEdit && res.data) {
                    roleTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    roleTable.addRow(res.data, true);
                } else {
                    roleTable.replaceData();
                }
            }

            closeGroupPanel();
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

function deleteRole(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete System Role",
        text: "Are you sure you want to delete this system role?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/admin/support/roles-delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getRoleAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "System Role successfully.");
                    if (roleGroupTable) {
                        roleGroupTable.replaceData();
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

function highlightRoleErrorField(selector, hasError, message) {
    const $field = $(selector);
    const $formGroup = $field.closest('.form-group');
    // Remove existing error
    $field.removeClass('is-invalid');
    $formGroup.find('.field-error').remove();

    if (hasError) {
        $field.addClass('is-invalid');
        if (message) {
            $formGroup.append(`<div class="field-error text-danger small mt-1 text-end">${message}</div>`);
        }
    }
}

function initRoleSearch() {
    const searchInput = $('#roleSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val().trim();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                roleTable.setFilter([
                    { field: "roleName", type: "like", value: searchTerm },
                    { field: "roleDescription", type: "like", value: searchTerm },
                    { field: "groupName", type: "like", value: searchTerm }
                ]);
            } else {
                roleTable.clearFilter();
            }
        }, 300);
    });
}

function closeRolePanel() {
    $('.role-overlay').removeClass('active');
    $('#rolePanel').removeClass('active');
}

function closeGroupPanel() {
    $('.groupOverlay').removeClass('active');
    $('#groupPanel').removeClass('active');
}

function getRoleAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

$(document).ready(function () {
    initRoleTable();
    initGroupListSelect2();

    toastr.options = {
        closeButton: true,
        progressBar: true,
        positionClass: "toast-top-right",
        timeOut: "3000"
    };

    $(".action-btn-support-home").on("click", function () {
        window.location.href = '/admin/support';
    });

    $(".action-btn-new-role").on("click", function () {
        createRole();
    });

    $('#roleForm').on('submit', function (e) {
        e.preventDefault();
    });
});
