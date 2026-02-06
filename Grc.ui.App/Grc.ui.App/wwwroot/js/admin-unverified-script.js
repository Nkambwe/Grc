
let unverifiedTable;

function initUnverifiedTable() {
    unverifiedTable = new Tabulator("#unverifiedTable", {
        ajaxURL: "/admin/support/users/unverified-list",
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
            {
                title: "VERIFY",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="verifyUser(${rowData.id})">
                        <span><i class="mdi mdi-account-check-outline" aria-hidden="true"></i></span>
                        <span>VERIFY</span>
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

    // Search init
    initUnverifiedSearch();
}

function initUnverifiedSearch() {

}

function openUnverifiedPane(title, record) {
    $('#fullName').val(record?.fullName || '');
    $('#pfNumber').val(record?.pfNumber || '');
    $('#emailAddress').val(record?.emailAddress || '');
    $('#departmentName').val(record?.departmentName || '');
    $('#username').val(record?.userName || '');
    $('#displayName').val(record?.displayName || '');
    $('#phoneNumber').val(record?.phoneNumber || '');
    $('#roleName').val(record?.roleName || '');

    $('#userTitle').text(title);
    $('#verifyContainer').addClass('active');
    $('#verifyOverLay').addClass('active');
}

function viewRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving user...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findUnverifiedAccount(id)
        .then(record => {
            Swal.close();
            if (record) {
                openUnverifiedPane('User Account Summery', record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'User not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load user details.' });
        });
}

function closeVerifyPanel() {
      $('#verifyContainer').removeClass('active');
      $('#verifyOverLay').removeClass('active');
}

function findUnverifiedAccount(id) {
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

function verifyUser(id) {
    if (!id && id !== 0) {
        Swal.fire({
            title: "Verify user",
            text: "User ID is required",
            showCancelButton: false,
            okButtonText: "Ok"
        })
        return;
    }

    Swal.fire({
        title: "Verified User",
        text: "Are you sure you want to verify this user account?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Verify",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/admin/support/users/verify/${encodeURIComponent(id)}`,
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getVerifiedToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "User account verified successfully.");
                    unverifiedTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Lock failed.");
                }
            },
            error: function () {
                toastr.error("Request failed.");
            }
        });
    });
}

$(document).ready(function () {
    initUnverifiedTable();

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });
    
    $(".action-btn-new-user").on("click", function () {
        window.location.href = '/admin/support/system-users';
    });
    
    $('#verifyForm').on('submit', function (e) {
        e.preventDefault();
    });

});

function getVerifiedToken() {
    return $('meta[name="csrf-token"]').attr('content');
}


