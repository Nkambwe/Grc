let returnsTable;


$(document).ready(function () {
    initReturnsTable();
});

function initReturnsTable() {
    returnsTable = new Tabulator("#returns-register-table", {
    ajaxURL: "/grc/returns/compliance-returns/paged-register",
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
                    ["reportName", "owner", "department", "authority", "frequency"].includes(f.field)
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
        alert("Failed to load returns. Please try again.");
    },
    layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "REPORT/RETURN NAME",
                field: "reportName",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewReport(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "AUTHORITY", field: "authority", minWidth: 200, frozen: true, headerSort: true },
            { title: "FREQUENCY", field: "frequency", widthGrow: 1, minWidth: 200, frozen: true, headerSort: true },
            {
                title: "OWNER/RESPONSIBLE",
                field: "owner",
                widthGrow: 1,
                minWidth: 280,
                headerSort: true
            },
            { title: "DEPARTMENT", field: "department", minWidth: 200 },
            {
                title: "DEADLINE",
                field: "deadlineOn",
                minWidth: 200,
                formatter: function (cell) {
                    const value = cell.getValue();
                    if (!value) return "";

                    const d = new Date(value);
                    const day = String(d.getDate()).padStart(2, "0");
                    const month = String(d.getMonth() + 1).padStart(2, "0");
                    const year = d.getFullYear();

                    return `${day}-${month}-${year}`;
                }
            },
            { title: "ENABLING LAW/REGULATION/GUIDELINE.", field: "article", minWidth: 200 },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteReturn(${rowData.id})">
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

    //..initialize search
    initReturnSearch();
}

function initReturnSearch() {
   
}

function viewReport(id) {
    alert("View report with ID >> " + id);
}

function deleteReturn(id) {
    alert("Delete return with ID >> " + id);
}

