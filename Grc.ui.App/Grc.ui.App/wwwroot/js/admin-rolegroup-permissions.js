let roleGroupPermissionsTable;

function initGroupTable() {
    roleGroupPermissionsTable = new Tabulator("#roleGroupPermissionsTable", {
        ajaxURL: "/admin/support/role-groups-permissions-all",
        paginationMode: "remote",
        filterMode: "remote",
        sortMode: "remote",
        pagination: true,
        paginationSize: 10,
        paginationSizeSelector: [10, 20, 35, 40, 50],
        paginationCounter: "rows",
        layout: "fitColumns",
        responsiveLayout: "hide",
        dataTree: true,                    
        dataTreeStartExpanded: false,      
        dataTreeChildField: "permissionSets",       
        dataTreeCollapseElement: "<i class='mdi mdi-chevron-right'></i>",
        dataTreeExpandElement: "<i class='mdi mdi-chevron-down'></i>",

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
                        ["groupName", "groupDescription"].includes(f.field)
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
            alert("Failed to load role groups. Please try again.");
        },

        columns: [
            {
                title: "GROUP NAME / PERMISSION SET",
                field: "groupName",
                minWidth: 250,
                formatter: function (cell) {
                    const data = cell.getRow().getData();

                    // Parent Row (Role Group)
                    if (data.permissionSets) {
                        return `<span class="clickable-title" onclick="editGroupPermission(${data.id})">
                                    ${data.groupName}
                                </span>`;
                    }

                    // Child Row (Permission Set)
                    return `<span style="margin-left:10px;">🔹 ${data.setName || "(Unnamed Set)"} </span>`;
                }
            },
            {
                title: "DESCRIPTION",
                field: "groupDescription",
                minWidth: 250,
                formatter: function (cell) {
                    const data = cell.getRow().getData();
                    return data.permissionSets ? (data.groupDescription || "") : (data.setDescription || "");
                }
            },
            {
                title: "CATEGORY / STATUS",
                field: "groupCategory",
                minWidth: 180,
                formatter: function (cell) {
                    const data = cell.getRow().getData();

                    // Permission set (child)
                    if (!data.permissionSets) {
                        const isDeleted = !!data.isDeleted;
                        const color = isDeleted ? "#ED1C24" : "#08A11C";
                        const text = isDeleted ? "Deactivated" : "Active";
                        return `<span style="color:${color}; font-weight:bold;">${text}</span>`;
                    }

                    // Role group (parent)
                    return data.groupCategory || "";
                }
            },
            {
                title: "CREATED ON",
                field: "createdOn",
                hozAlign: "center",
                headerHozAlign: "center",
                width: 180,
                formatter: function (cell) {
                    const value = cell.getValue() || cell.getRow().getData().createdOn;
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
                hozAlign: "center",
                headerHozAlign: "center",
                width: 200,
                headerSort: false,
                formatter: function (cell) {
                    const data = cell.getRow().getData();

                    // For parent rows (role groups)
                    if (data.permissionSets) {
                        return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteGroup(${data.id})">
                                    <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                                    <span>DELETE GROUP</span>
                                </button>`;
                    }

                    // For child rows (permission sets)
                    return `<button class="grc-table-btn grc-btn-edit grc-edit-action" onclick="editPermissionSet(${data.id})">
                                <span><i class="mdi mdi-pencil" aria-hidden="true"></i></span>
                                <span>EDIT SET</span>
                            </button>`;
                }
            }
        ]
    });

    //..search role group
    initGroupPermissionsSearch();
}

function findGroupPermission(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/role-group-permissions-retrieve/${encodeURIComponent(id)}`,
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

function editGroupPermission(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving role group...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    console.log("ID >> " + id);
    findGroupPermission(id)
        .then(record => {
            Swal.close();
            if (record) {
                openGroupEditor('Edit Role Group', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Role group not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load role group details.' });
        });
}

function addPermissionSet() {
    openGroupEditor('New Role Group', {
        id: 0,
        groupName: '',
        groupDescription: '',
        setDescription: '',
        groupScope: '',
        groupCategory: '',
        department: '',
        isDeleted: false,
        isVerified: false,
        isApproved: false,
        permissionSets: []
    }, false);

}

function openGroupEditor(title, group, isEdit) {
    // Populate form fields
    $("#groupId").val(group?.id || "");
    $("#isEdit").val(isEdit);
    $("#groupName").val(group?.groupName || "");
    $("#groupDescription").val(group?.groupDescription || "");
    $('#isDeleted').prop('checked', group?.isDeleted || false);

    // Show temporary message
    $("#roleGroupListContainer").html("<div class='text-muted p-2'>Loading permission sets...</div>");

    if (isEdit) {
        $.ajax({
            url: `/admin/support/role-group-permissions-retrieve/${group.id}`,
            type: "POST",
            contentType: "application/json",
            success: function (res) {
                if (!res?.success) {
                    $("#roleGroupListContainer").html("<div class='text-danger'>Failed to load role group details.</div>");
                    return;
                }

                const permissionSets = Array.isArray(res.data.permissionSets)
                    ? res.data.permissionSets
                    : [];

                if (permissionSets.length === 0) {
                    $("#roleGroupListContainer").html("<div class='text-muted p-2'>No permission sets found for this role group.</div>");
                    return;
                }

                // Build list
                let html = "<div class='role-group-list'>";
                permissionSets.forEach(s => {
                    const checked = s.isAssigned ? "checked" : "";
                    html += `
                        <div class="form-check mb-1">
                            <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${s.id}" value="${s.id}" ${checked}>
                            <label class="form-check-label" for="perm-${s.id}">
                                ${s.setDescription || s.setName}
                            </label>
                        </div>`;
                });
                html += "</div>";

                $("#roleGroupListContainer").html(html);
            },
            error: function (xhr, status, error) {
                console.error("Error loading permission sets:", error);
                $("#roleGroupListContainer").html("<div class='text-danger'>Failed to load permission sets.</div>");
            }
        });
    } else {
        // Create mode: load all permission sets
        $.get("/admin/support/permission-sets-all", function (res) {
            const permissionSets = Array.isArray(res?.data) ? res.data : [];

            let html = "<div class='role-group-list'>";
            permissionSets.forEach(s => {
                html += `
                    <div class="form-check mb-1">
                        <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${s.id}" value="${s.id}">
                        <label class="form-check-label" for="perm-${s.id}">
                            ${s.setDescription || s.setName}
                        </label>
                    </div>`;
            });
            html += "</div>";
            $("#roleGroupListContainer").html(html);
        }).fail(() => {
            $("#roleGroupListContainer").html("<div class='text-danger'>Failed to load permission sets.</div>");
        });
    }

    // Show overlay panel
    $('#panelTitle').text(title);
    $('.overlay').addClass('active');
    $('#setPanel').addClass('active');
}

function deleteGroup(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Role Group",
        text: "Are you sure you want to delete this role group?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/admin/support/role-groups-delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getGroupPermissionAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Role Group deleted successfully.");
                    if (roleGroupPermissionsTable) {
                        roleGroupPermissionsTable.replaceData();
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

function saveGroupPermissions(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();

    let recordData = {
        id: parseInt($('#groupId').val()) || 0,
        groupName: $('#groupName').val()?.trim(),
        groupDescription: $('#groupDescription').val()?.trim(),
        groupScope:"",
        groupCategory: "",
        department: "",
        departmentId: 0,
        attachedTo: "",
        isDeleted: $('#isDeleted').is(':checked') ? true : false,
        isVerified:true,
        isApproved: true,
        roles:[],
        permissionSets: $(".perm-checkbox:checked").map((_, el) => parseInt(el.value, 10)).get()
    };

    // --- validate required fields ---
    let errors = [];

    if (!recordData.groupName)
        errors.push("Role Group name is required.");

    if (!recordData.groupDescription)
        errors.push("Role Group description is required.");

    if (recordData.permissionSets.length == 0)
        errors.push("Please select set permission sets.");

    // --- stop if validation fails ---
    if (errors.length > 0) {

        highlightPermissionField("#groupName", !recordData.groupName);
        highlightPermissionField("#groupDescription", !recordData.groupDescription);

        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    persistGroupPermissions(isEdit, recordData);
}

function persistGroupPermissions(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/support/role-group-permissions-modify"
        : "/admin/support/role-group-permissions-create";

    Swal.fire({
        title: isEdit ? "Updating role group..." : "Saving role group...",
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
            'X-CSRF-TOKEN': getGroupPermissionAntiForgeryToken()
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

            if (roleGroupPermissionsTable) {
                if (isEdit && res.data) {
                    roleGroupPermissionsTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    roleGroupPermissionsTable.addRow(res.data, true);
                } else {
                    roleGroupPermissionsTable.replaceData();
                }
            }

            closeGroupPermissionPanel();
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

function closeGroupPermissionPanel() {
    console.log('Button clicked');
    $('.overlay').removeClass('active');
    $('#setPanel').removeClass('active');
}

function initGroupPermissionsSearch() {
    const searchInput = $('#roleGroupSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val().trim();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                roleGroupPermissionsTable.setFilter([
                    { field: "groupName", type: "like", value: searchTerm },
                    { field: "groupDescription", type: "like", value: searchTerm }
                ]);
            } else {
                roleGroupPermissionsTable.clearFilter();
            }
        }, 300);
    });
}

//..get antiforegery token from meta tag
function getGroupPermissionAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightPermissionField(selector, hasError, message) {
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

    initGroupTable();

    $('.admin-home').on('click', function () {
        window.location.href = '/admin/support/system-permissionSets-groups';
    });

    //..new role group
    $(".action-btn-new-role-group").on("click", function () {
        console.log(`Clicked new set`);
        addPermissionSet();
    });

    $('#groupForm').on('submit', function (e) {
        e.preventDefault();
    });

});


