let roleGroupTable;
function initGroupTable() {
    roleGroupTable = new Tabulator("#roleGroupTable", {
        ajaxURL: "/admin/support/system-role-groups/list",
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
        layout: "fitColumns",
        dataTree: false,
        responsiveLayout: "hide",
        columns: [
            {
                title: "GROUP NAME",
                field: "groupName",
                minWidth: 250,
                formatter: (cell) => `<span class="clickable-title" onclick="editGroup(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "GROUP DESCRIPTION",
                field: "groupDescription",
                minWidth: 250
            },
            {
                title: "GROUP TYPE",
                field: "groupCategory",
                minWidth: 250
            },
            {
                title: "SCOPE",
                field: "groupScope",
                minWidth: 250
            },
            {
                title: "DEPARTMENT",
                field: "department",
                minWidth: 250
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
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteGroup(${rowData.id})">
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

    //..search role group
    initGroupSearch();
}

function findGroup(id) {
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

function editGroup(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving role group...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    console.log("ID >> " + id);
    findGroup(id)
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

function addGroupRecord() {
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
        roles: []
    }, false);

}

function openGroupEditor(title, group, isEdit) {
    $("#groupId").val(group?.id || "");
    $('#isEdit').val(isEdit);
    $("#groupName").val(group?.groupName || "");
    $("#groupDescription").val(group?.groupDescription || "");
    $("#groupScope").val(group?.groupScope || "");
    $("#groupCategory").val(group?.groupCategory || "");
    $("#groupType").val(group?.groupType || "");
    $("#department").val(group?.department || "");
    $('#isDeleted').prop('checked', group?.isDeleted || false);

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
                'X-CSRF-TOKEN': getGroupAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Role Group deleted successfully.");
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

function saveGroup(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();

    let recordData = {
        id: parseInt($('#id').val()) || 0,
        groupName: $('#groupName').val()?.trim(),
        groupDescription: $('#groupDescription').val()?.trim(),
        setDescription: $('#setDescription').val()?.trim(),
        groupScope: $('#groupScope').val()?.trim(),
        groupCategory: $('#groupCategory').val()?.trim(),
        department: $('#setName').val()?.trim(),
        departmentId: 0,
        attachedTo: "",
        isDeleted: $('#isDeleted').is(':checked') ? true : false,
        isVerified: $('#isVerified').is(':checked') ? true : false,
        isApproved: $('#isApproved').is(':checked') ? true : false,
        permissionSets:[],
        roles: $(".perm-checkbox:checked").map((_, el) => parseInt(el.value, 10)).get()
    };

    // --- validate required fields ---
    let errors = [];

    if (!recordData.setName)
        errors.push("Role Group name is required.");

    if (!recordData.setDescription)
        errors.push("Role Group description is required.");

    if (recordData.permissions.length == 0)
        errors.push("Please select set roles.");

    // --- stop if validation fails ---
    if (errors.length > 0) {

        highlightField("#groupName", !recordData.groupName);
        highlightField("#groupDescription", !recordData.groupDescription);

        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    console.log("Valid Record:", recordData);
    persistGroup(isEdit, recordData);
}

function persistGroup(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/support/role-groups-modify"
        : "/admin/support/role-groups-create";

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
            'X-CSRF-TOKEN': getGroupAntiForgeryToken()
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

            if (roleGroupTable) {
                if (isEdit && res.data) {
                    roleGroupTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    roleGroupTable.addRow(res.data, true);
                } else {
                    roleGroupTable.replaceData();
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

function closeGroupPanel() {
    console.log('Button clicked');
    $('.overlay').removeClass('active');
    $('#setPanel').removeClass('active');
}

function initGroupSearch() {
    const searchInput = $('#roleGroupSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val().trim();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                roleGroupTable.setFilter([
                    { field: "groupName", type: "like", value: searchTerm },
                    { field: "groupDescription", type: "like", value: searchTerm }
                ]);
            } else {
                roleGroupTable.clearFilter();
            }
        }, 300);
    });
}

//..get antiforegery token from meta tag
function getGroupAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
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
        window.location.href = '/admin/support/system-roles-groups';
    });

    //..new role group
    $(".action-btn-new-role-group").on("click", function () {
        console.log(`Clicked new set`);
        addGroupRecord();
    });

    $('#groupForm').on('submit', function (e) {
        e.preventDefault();
    });

});
