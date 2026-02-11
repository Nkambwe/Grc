
let deletedTable;

function initDeletedTable() {
    deletedTable = new Tabulator("#deletedUsersTable", {
        ajaxURL: "/admin/support/users/deleted-accounts",
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
                title: "FULL NAME",
                field: "displayName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "USERNAME",
                field: "userName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true
            },
            { title: "PF NUMBER", field: "pfNumber", minWidth: 200 },
            {
                title: "EMAIL ADDRESS",
                field: "emailAddress",
                minWidth: 500,
                widthGrow: 4,
                headerSort: true
            },
            { title: "DEPARTMENT", field: "departmentName", minWidth: 300 },
            { title: "ROLE", field: "roleName", minWidth: 300 },
            {
                title: "RESTORE",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="restoreAccount(${rowData.id})">
                        <span><i class="mdi mdi-account-reactivate-outline" aria-hidden="true"></i></span>
                        <span>RESTORE</span>
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

function openDeletePane(title, record) {

    $('#fullName').val(record?.fullName || '');
    $('#pfNumber').val(record?.pfNumber || '');
    $('#emailAddress').val(record?.emailAddress || '');
    $('#departmentName').val(record?.departmentName || '');
    $('#username').val(record?.userName || '');
    $('#displayName').val(record?.displayName || '');
    $('#phoneNumber').val(record?.phoneNumber || '');
    $('#roleName').val(record?.roleName || '');

    $('#userTitle').text(title);
    $('#deleteOverLay').addClass('active');
    $('#deleteContainer').addClass('active');
}

function viewRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving user...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findDeletedAccount(id)
        .then(record => {
            Swal.close();
            if (record) {
                openDeletePane('User Account Summery', record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'User not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load user details.' });
        });
}

function closeDeletePanel() {
      $('#deleteOverLay').removeClass('active');
      $('#deleteContainer').removeClass('active');
}

function findDeletedAccount(id) {
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

function restoreAccount(id) {
    if (!id && id !== 0) {
        Swal.fire({
            title: "Restore user account",
            text: "User ID is required",
            showCancelButton: false,
            okButtonText: "Ok"
        })
        return;
    }

    Swal.fire({
        title: "Restore User Account",
        text: "Are you sure you want to restore this user account?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Restore",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/admin/support/users/restore-user/${encodeURIComponent(id)}`,
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getDeleteToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "User account restored successfully.");
                    deletedTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Restore failed.");
                }
            },
            error: function () {
                toastr.error("Request failed.");
            }
        });
    });
}

$(document).ready(function () {
    initDeletedTable();

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });
    
    $(".action-btn-new-user").on("click", function () {
        window.location.href = '/admin/support/system-users';
    });
    
    $('#restoreForm').on('submit', function (e) {
        e.preventDefault();
    });
});

function getDeleteToken() {
    return $('meta[name="csrf-token"]').attr('content');
}