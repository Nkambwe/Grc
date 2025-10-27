
let roleGroupTable;
function initRoleGroupTable() {

    roleGroupTable = new Tabulator("#adminRoleGroupsTable", {
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
                title: "ROLE GROUP",
                field: "groupName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "GROUP DESCRIPTION",
                field: "groupDescription",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true
            },
            {
                title: "DEPARTMENT",
                field: "department",
                minWidth: 500,
                widthGrow: 4,
                headerSort: true,
                frozen: true
            },
            {
                title: "SCOPE",
                field: "groupScope",
                minWidth: 500,
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
            { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });

    // Search role groups
    initRoleGroupSearch();
}

function initRoleGroupSearch() {

}

function viewRecord(userId) {
    alert("View record for role group ID: " + userId);
}

$(document).ready(function () {
    initRoleGroupTable();

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });

    $(".action-btn-new-role").on("click", function () {
        alert("new role group button clicked")
    });

    $(".action-btn-edit-role").on("click", function () {
        alert("Edit role group button clicked")
    });

    $(".action-btn-delete-role").on("click", function () {
        alert("Delete role group button clicked")
    });

});
