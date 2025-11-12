let processGroupTable;
function initProcessGroupListTable() {
    processGroupTable = new Tabulator("#processGroupTable", {
        ajaxURL: "/operations/workflow/processes/groups/all",
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
            alert("Failed to load process groups. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "GROUP NAME",
                field: "groupName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewProcessGroup(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "GROUP DESCRIPTION", field: "groupDescription", widthGrow: 1, minWidth: 400, frozen: true, headerSort: false },
            {
                title: "ACTIVE",
                field: "isDeleted",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let color = rowData.aligned !== true ? "#28a745" : "#dc3545";
                    let text = rowData.aligned !== true ? "YES" : "NO";

                    return `<div style="
                            display:flex;
                            align-items:center;
                            justify-content:center;
                            width:100%;
                            height:100%;
                            border-radius:50px;
                            color:${color};
                            font-weight:bold;">
                        ${text}
                         </div>`;
                    },
                hozAlign: "center",
                headerHozAlign: "center",
                maxWidth: 400
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteProcessGroup(${rowData.id})">
                    <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                    <span>DELETE</span>
                </button>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessGroupSearch();

};

function initProcessGroupSearch() {


}

function deleteProcessGroup(id) {
    alert("Delete Process Group ID: " + id);
}

function viewProcessGroup(id) {
    alert("View Process Group ID: " + id);
}

function createGroup() {
    alert("Create Process Group");
}

$(document).ready(function () {

    initProcessGroupListTable();

    $('.action-btn-process-group').on('click', function () {
        createGroup();
    });

    $('#processGroupForm').on('submit', function (e) {
        e.preventDefault();
    });

});