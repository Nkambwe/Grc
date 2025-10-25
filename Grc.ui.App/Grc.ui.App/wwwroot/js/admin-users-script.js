
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

function viewRecord(userId) {
    alert("View record for user ID: " + userId);
}

$(document).ready(function () {
    initUserTable();

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });

    $(".action-btn-new-user").on("click", function () {
        alert("new user button clicked")
    });

    $(".action-btn-edit-user").on("click", function () {
        alert("Edit user button clicked")
    });

    $(".action-btn-delete-user").on("click", function () {
        alert("Delete user button clicked")
    });

    $(".action-btn-approve-user").on("click", function () {
        alert("Approve user button clicked")
    });

    $(".action-btn-lock-account").on("click", function () {
        alert("Lock account button clicked")
    });

    $(".action-btn-password-user").on("click", function () {
        alert("User password button clicked")
    });

});
