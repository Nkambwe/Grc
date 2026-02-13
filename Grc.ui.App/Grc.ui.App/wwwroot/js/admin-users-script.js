
let userTable;
function initUserTable() {
    userTable = new Tabulator("#adminUsersTable", {
        ajaxURL: "/admin/support/users-all",
        paginationMode: "remote",
        filterMode: "remote",
        sortMode: "remote",
        pagination: true,
        paginationSize: 10,
        paginationSizeSelector: [10, 20, 35, 40, 50],
        paginationCounter: "rows",
        placeholder: "No records found",
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
                        ["displayName", "userName", "emailAddress", "roleName", "roleGroup", "pfNumber"].includes(f.field)
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
                        if (xhr.status === 401) {
                            window.location = "/login/userlogin";
                        }

                        if (xhr.status === 403) {
                            Swal.fire({
                                title: "Access Denied!",
                                text: "You do not have permission to access this resource."
                            });

                            //..return empty dataset
                            resolve({
                                data: [],
                                last_page: 1,
                                total_records: 0
                            });

                            return;
                        }

                        reject(error);
                        return;
                    }
                });
            });
        },
        ajaxResponse: function (url, params, response) {
            if(response?.status === 403 || response?.hasPermission === false){

                this.clearData();
                this.setPlaceholder("You do not have permission to view these records.");

                return {
                    data: [],
                    last_page: 1
                };
            }

            return response;
        },
        ajaxError: function (error) {
            console.error("Tabulator AJAX Error:", error);
             Swal.fire({
                title: "System Error!",
                text: "Failed to load system roles. Please try again."
            });
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "", field: "startTab",
                maxWidth: 50,
                headerSort: false,
                headerFilter: "input",
                frozen: true,
                formatter: () => `<span class="record-tab"></span>`
            },
            {
                title: "USERNAME",
                field: "userName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                headerFilter: "input",
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "FULL NAME",
                field: "displayName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                headerFilter: "input",
                frozen: true
            },
            {
                title: "EMAIL ADDRESS",
                field: "emailAddress",
                minWidth: 500,
                widthGrow: 4,
                headerSort: true,
                headerFilter: "input",
                frozen: true
            },
            { title: "ROLE", field: "roleName", minWidth: 300, headerFilter: "input" },
            { title: "DEPARTMENT", field: "department", minWidth: 300, headerFilter: "input" },
            { title: "PF NUMBER", field: "pfNumber", minWidth: 200, headerFilter: "input" },
            {
                title: "PASSWORD",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="passwordReset(${rowData.id})">
                        <span><i class="mdi mdi-account-key-outline" aria-hidden="true"></i></span>
                        <span>RESET</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "LOCK",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="lockUser(${rowData.id})">
                        <span><i class="mdi mdi-account-lock-outline" aria-hidden="true"></i></span>
                        <span>LOCK</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "DELETE",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteUser(${rowData.id})">
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

}

function addUser() {
    openUserPane("Add new user", {
        id: 0,
        solId:0,
        roleId:0,
        roleGroupId:0,
        departmentId:0,
        isVerify:  false,
        isApprove:  false,
        firstName: '',
        lastName: '',
        middleName: '',
        userName: '',
        emailAddress: '',
        displayName: '',
        phoneNumber: '',
        pfNumber: '',
        unitCode: '',
        isActive:  false,
    }, false);
}

function openUserPane(title, record, isEdit) {
    $('#isEdit').val(isEdit);
    $('#recordId').val(record?.id || '');
    $('#isVerify').val(record?.isVerified || false);
    $('#isApprove').val(record?.isApprove || false);
    $('#firstName').val(record?.firstName || '');
    $('#lastName').val(record?.lastName || '');
    $('#middleName').val(record?.middleName || '');
    $('#userName').val(record?.userName || '');
    $('#emailAddress').val(record?.emailAddress || '');
    $('#displayName').val(record?.displayName || '');
    $('#phoneNumber').val(record?.phoneNumber || '');
    $('#pfNumber').val(record?.pfNumber || '');
    $('#isActive').prop('checked', record?.isActive || false);
    $('#solId').val(record?.solId || 'Unknown').trigger('change');

    if (isEdit) {
        $('#roleId').val(record.roleId).trigger('change');
        $('#departmentId').val(record.departmentId).trigger('change');
        $('#roleGroupId').val(record.roleGroup || '0').trigger('change');
        $('#unitCode').val(record.unitCode || '0').trigger('change');
    } else {
        $('#roleId').val('0');
        $('#departmentId').val('0');
        $('#roleGroupId').val('0').empty();
        $('#unitCode').val('0').empty();
    }

    $('#userTitle').text(title);
    $('#adminUserOverLay').addClass('active');
    $('#newUserContainer').addClass('active');
}

function ajaxJson(url, type = "GET", data = null) {
    return new Promise((resolve) => {
        $.ajax({
            url: url,
            type: type,
            data: data,
            dataType: "json",
            success: function (response) {
                //..application-level errors
                if (response.success) {
                    resolve({ data: response.data });
                } else {
                    resolve({ error: response.message || "Unknown error occurred" });
                }
            },
            error: function (xhr) {
                // Handle auth issues
                if (xhr.status === 401) {
                    window.location = "/login/userlogin";
                    return;
                }
                if (xhr.status === 403) {
                    resolve({ error: "You do not have permission to perform this action." });
                    return;
                }

                // Network or server errors
                resolve({ error: "Unexpected server error occurred." });
            }
        });
    });
}

function viewRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving user...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    ajaxJson(`/admin/support/users-retrieve/${id}`)
        .then(result => {
            Swal.close();

            if (result.error) {
                Swal.fire("Error", result.error, "warning");
                return;
            }

            if (!result.data) {
                Swal.fire("NOT FOUND", "User not found", "info");
                return;
            }

            //..open slide-out pane
            openUserPane('Edit User', result.data, true);
        });
}

function findUser(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/users-retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    resolve(response.data); 
                } else {
                    //..pass the message
                    resolve({ errorMessage: response.message || "User not found" });
                }
            },
            error: function (xhr) {
                //..handle 401/403 properly
                if (xhr.status === 401) {
                    window.location = "/login/userlogin";
                    return;
                }
                if (xhr.status === 403) {
                    resolve({ errorMessage: "You do not have permission to view this user." });
                    return;
                }
                resolve({ errorMessage: "Unexpected server error occurred." });
            }
        });
    });
}

function saveUser(e) {
    e.preventDefault();
    let id = Number($('#recordId').val()) || 0;
    let roleId = Number($('#roleId').val()) || 0;
    let departmentId = Number($('#departmentId').val()) || 0;
    let roleGroupId = Number($('#roleGroupId').val()) || 0;

    let solval = $('#solId').val();

    let isEdit = $('#isEdit').val() || false;
    let isVerify = $('#isVerify').val() || true;
    let isApprove = $('#isApprove').val() || true;

    //..build record payload from form
    let recordData = {
        id: id,
        isVerify: isVerify,
        isApprove: isApprove,
        solId: solval,
        firstName: $('#firstName').val()?.trim(),
        middleName: $('#middleName').val()?.trim(),
        lastName: $('#lastName').val()?.trim(),
        userName: $('#userName').val()?.trim(),
        emailAddress: $('#emailAddress').val()?.trim(),
        pfNumber: $('#pfNumber').val()?.trim(),
        phoneNumber: $('#phoneNumber').val()?.trim(),
        displayName: $('#displayName').val()?.trim(),
        isActive: $('#isActive').is(':checked') ? true : false,
        roleId: roleId,
        departmentId: departmentId,
        roleGroupId: roleGroupId,
        unitCode: $('#unitCode').val()?.trim()
    };

    //..validate required fields
    let errors = [];
    if (!recordData.firstName)
        errors.push("First Name field is required.");

    if (!recordData.lastName)
        errors.push("Last Name field is required.");

    if (!recordData.userName)
        errors.push("Username field is required.");

    if (!recordData.emailAddress)
        errors.push("Email address field is required.");
        
    if (!recordData.pfNumber)
        errors.push("PF Number field is required.");

    if (!recordData.solId || recordData.solId === 'Unknown')
        errors.push("Branch Field is required.");
        
    if (recordData.roleId == 0)
        errors.push("Role Field is required.");
        
    if (recordData.roleGroupId == 0)
        errors.push("Role group Field is required.");

    if (recordData.departmentId == 0)
        errors.push("Department Field is required.");

    console.log(recordData);

    if (errors.length > 0) {
        errorMakerField("#firstName", !recordData.firstName);
        errorMakerField("#lastName", !recordData.lastName);
        errorMakerField("#userName", !recordData.userName);
        errorMakerField("#emailAddress", !recordData.emailAddress);
        errorMakerField("#pfNumber", !recordData.pfNumber);
        errorMakerField("#authorityId", recordData.solId === 0);
        errorMakerField("#ownerId", recordData.roleId === 0);
        errorMakerField("#authorityId", recordData.departmentId === 0);
        errorMakerField("#ownerId", recordData.roleGroupId === 0);
        Swal.fire({
            title: "User data Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveUserRecord(isEdit, recordData);
}

function saveUserRecord(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/support/users-modify"
        : "/admin/support/users-create";

    Swal.fire({
        title: isEdit ? "Updating user record..." : "Saving user record...",
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
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getPostToken()
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

            Swal.fire(res.message || "User record saved successfully")
                .then(() => {
                    //..close panel
                    closeUserPanel();
                    window.location.reload();
                });
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

function deleteUser(id) {
    console.log("My ID >> " + id)

    let l_id = Number(!id );
    if (l_id !== 0) {
        Swal.fire({
            title: "Delete User",
            text: "User ID is required",
            showCancelButton: false,
            okButtonText: "Ok"
        })
        return;
    }

    Swal.fire({
        title: "Delete User",
        text: "Are you sure you want to delete user account?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/admin/support/users-delete/${encodeURIComponent(id)}`,
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getPostToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "User account has been deleted successfully");
                    userTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Failed to delete user account");
                }
            },
            error: function () {
                toastr.error("Request failed.");
            }
        });
    });
}

function exportUsers() {
    $.ajax({
        url: '/admin/support/users/export-list',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(userTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Active_users.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
}

function approveUsers() {
    window.location.href = '/admin/support/users-unapproved';
}

function lockUser(id) {
    if (!id && id !== 0) {
        Swal.fire({
            title: "Lock User Account",
            text: "User ID is required",
            showCancelButton: false,
            okButtonText: "Ok"
        })
        return;
    }

    Swal.fire({
        title: "Lock User Account",
        text: "Are you sure you want to lock user account?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Lock",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/admin/support/users/lock-user/${encodeURIComponent(id)}`,
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getPostToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "User account has been locked successfully");
                    userTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Failed to lock user account");
                }
            },
            error: function () {
                toastr.error("Request failed.");
            }
        });
    });
}

function passwordReset(id) {
    if (!id && id !== 0) {
        Swal.fire({
            title: "Password Reset",
            text: "User ID is required",
            showCancelButton: false,
            okButtonText: "Ok"
        })
        return;
    }

    Swal.fire({
        title: "Password Reset",
        text: "Are you sure you want to reset user password?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Reset",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/admin/support/users/password-reset/${encodeURIComponent(id)}`,
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getPostToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Password reset. Mail will be set to user with new password.");
                    userTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Password reset failed.");
                }
            },
            error: function () {
                toastr.error("Request failed.");
            }
        });
    });
}

function lockUsers() {
    window.location.href = '/admin/support/users-locked';
}

function closeUserPanel() {
      $('#adminUserOverLay').removeClass('active');
      $('#newUserContainer').removeClass('active');
}

function errorMakerField(selector, hasError, message) {
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

function getPostToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

$(document).ready(function () {
    initUserTable();

    $('#userForm').on('submit', function (e) {
        e.preventDefault();
    });

     $('#roleId ,#roleGroupId, #departmentId, #unitCode, #solId, #unitCode').select2({
        width: '100%',
        dropdownParent: $('#newUserContainer'),
        placeholder: 'Select an option',
        allowClear: true
    });

    $('#departmentId').on('change', function () {
        const id = $(this).val();

        // Reset unit code
        $('#unitCode').empty().trigger('change');

        if (!id) return;
       
        $.ajax({
            url: `/admin/support/department-units/mini-list/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {

                    const $units = $('#unitCode');
                    $units.empty();
                    $units.append(new Option('Select unit...', 'Unknown', false, false));

                    response.data.forEach(item => {
                        $units.append(
                            new Option(item.unitName, item.code, false, false)
                        );
                    });
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
            }
        });
    });

    $(document).on('change', '#roleId', function () {
        const id = $(this).val();

        $('#roleGroupId').empty().trigger('change');

        if (!id || id === "0") return;

        $.ajax({
            url: `/admin/support/role-groups/mini-list/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {

                    const $roleGroup = $('#roleGroupId');
                    $roleGroup.empty();
                    $roleGroup.append(new Option('Select group...', '0', false, false));

                    response.data.forEach(item => {
                        $roleGroup.append(
                            new Option(item.groupName, item.id, false, false)
                        );
                    });
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
            }
        });
    });

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });

    $(".action-btn-new-user").on("click", function () {
        addUser();
    });
    $(document).on('input', '#phoneNumber', function () {
        this.value = this.value.replace(/\D/g, '').slice(0, 12);
    });

    $(document).on('input', '#pfNumber', function () {
        this.value = this.value.replace(/\D/g, '');
    });

    $(document).on('input', '#emailAddress', function () {
        this.value = this.value.replace(/[^a-zA-Z0-9@.]/g, '');
    });

    $(document).on('input', '#userName', function () {
        this.value = this.value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
    });

    $(document).on('input', '#firstName, #middleName, #lastName', function () {
        this.value = this.value.replace(/[^a-zA-Z]/g, '');
    });

    $(document).on('input', '#firstName', function () {
        $('#displayName').val(this.value);
    });

    $(".action-btn-excel-export").on("click", function () {
        exportUsers();
    });

    $(".action-btn-approve-user").on("click", function () {
        approveUsers();
    });

    $(".action-btn-lock-account").on("click", function () {
        lockUsers();
    });

});
