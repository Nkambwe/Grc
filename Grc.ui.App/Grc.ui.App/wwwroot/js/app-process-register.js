let processRegisterTable;
function initProcessRegisterTable() {
    processRegisterTable = new Tabulator("#processRegisterTable", {
        ajaxURL: "/operations/processes/register/all",
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
                        ["processName", "description", "typeName", "ownerName", "assigneeName", "unitName"].includes(f.field)
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
            alert("Failed to load processes. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "PROCESS NAME",
                field: "processName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewProcess(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "PROCESS DESCRIPTION", field: "description", widthGrow: 1, minWidth: 400, frozen: true, headerSort: false },
            { title: "PROCESS TYPE", field: "typeName", headerSort: true, minWidth: 200 },
            { title: "PROCESS OWNER", field: "ownerName", headerSort: true, minWidth: 200 },
            { title: "ATTACHED UNIT", field: "unitName", minWidth: 250 },
            { title: "PROCESS MANAGER", field: "assigneeName", minWidth: 400 },
            {
                title: "PROCESS FILE",
                field: "fileName",
                minWidth: 200,
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<span class="clickable-title" onclick="viewFile(${rowData.fileName})">${rowData.fileName}</span>`
                }
            },
            { title: "STATUS", field: "processStatus", minWidth: 120 },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteProcess(${rowData.id})">
                        <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                        <span>DELETE</span>
                    </button>`;
                },
                width: 100,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessSearch();
}

function viewProcess(){

}

function deleteProcess() {

}

function viewFile() {

}

function initProcessSearch() {
    const searchInput = $('#processesSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processRegisterTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "description", type: "like", value: searchTerm },
                        { field: "typeName", type: "like", value: searchTerm },
                        { field: "ownerName", type: "like", value: searchTerm },
                        { field: "assigneeName", type: "like", value: searchTerm },
                        { field: "unitName", type: "like", value: searchTerm }
                    ]
                ]);
                processRegisterTable.setPage(1, true);
            } else {
                processRegisterTable.clearFilter();
            }
        }, 300);
    });
}


$(document).ready(function () {
    initProcessRegisterTable();
});