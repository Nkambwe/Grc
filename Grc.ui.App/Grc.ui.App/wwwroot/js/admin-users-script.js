
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
            alert("Failed to load users records. Please try again.");
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
                title: "USERNAME",
                field: "userName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "FULL NAME",
                field: "displayName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true
            },
            {
                title: "EMAIL ADDRESS",
                field: "emailAddress",
                minWidth: 500,
                widthGrow: 4,
                headerSort: true,
                frozen: true
            },
            { title: "ROLE", field: "roleName", minWidth: 300 },
            { title: "ROLE GROUP", field: "roleGroup", minWidth: 300 },
            { title: "PF NUMBER", field: "pfNumber", minWidth: 200 },
            {
                title: "ACTIVE",
                field: "isActive",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let value = rowData.isActive;
                    let color = value === true ? "#08A11C" : "#FF2E80";
                    let text = value === true ? "Active" : "Blocked";
                    console.log("User status >> " + text);
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
                    let color = value !== true? "#FF9704" : "#08A11C";
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
                title: "CREATED ON",
                formatter: function (cell) {
                    const value = cell.getRow().getData().createdOn;
                    if (!value) return "";

                    const date = new Date(value);

                    const day = String(date.getDate()).padStart(2, "0");
                    const month = String(date.getMonth() + 1).padStart(2, "0");
                    const year = date.getFullYear();
                    const formattedDate = `${day}-${month}-${year}`;

                    return `
                            <div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${formattedDate}</span>
                            </div>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });

    // Search init
    initUserSearch();
}

function initUserSearch() {

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
    $('#isVerify').val(record?.isVerify || false);
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
        $('#roleGroupId').val(record.roleGroupId || '0').trigger('change');
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


function viewRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving user...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    console.log("ID >> " + id);
    findUser(id)
        .then(record => {
            Swal.close();
            if (record) {
                openUserPane('Edit User', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'User not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load user details.' });
        });
}

function findUser(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/users-retrieve/${id}`,
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
                return;
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

    console.log("User record >> ", record);
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

function exportUsers() {
    alert("Export Users");
}

function approveUsers() {
    alert("Approve user button clicked")
}

function lockUsers() {
    alert("Lock account button clicked");
}

function managePassword() {
     alert("User password button clicked");
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

    $(".action-btn-password-user").on("click", function () {
        managePassword();
    });

});
