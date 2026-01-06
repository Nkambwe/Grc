let circularTable;

$(document).ready(function () {
    initCircularTable();
});

function initCircularTable() {
    circularTable = new Tabulator("#circular-register-table", {
        ajaxURL: "/grc/returns/circular-returns/circular-register",
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
                        ["circularTitle", "authority", "status", "department"].includes(f.field)
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
                field: "circularTitle",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewCircular(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "AUTHORITY", field: "authority", minWidth: 200, frozen: true, headerSort: true },
            { title: "DEPARTMENT", field: "department", minWidth: 200 },
            {
                title: "RECIEVE DATE",
                field: "recievedOn",
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
            {
                title: "STATUS",
                field: "status",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();

                    // Default color
                    let bg = "#C2B70B";
                    let clr = "#FFFFFF";
                    if (value === "CLOSED") {
                        bg = "#28C232";
                    }
                    else if (value === "DUE") {
                        bg = "#F50C0C";
                    } else { //ON-GOING
                        bg = "#C2B70B";
                    }
                    
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = clr;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 200,
                headerSort: true
            },
            {
                title: "CLOSE DATE",
                field: "submissionDate",
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
            {
                title: "VIEW ISSUES",
                field: "hasIssues",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let value = rowData.hasIssues;
                    let hasIssues = value === true ? "" : "disabled";
                    return `<button class="grc-table-btn grc-btn-default grc-task-action ${hasIssues}" ${hasIssues} onclick="viewIssues(${rowData.id})">
                            <span><i class="mdi mdi-link-lock" aria-hidden="true"></i></span>
                            <span>ISSUES</span>
                        </button>`;
                },
                width: 200,
                hozAlign: "left",
                headerHozAlign: "left",
                headerSort: false
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteCircular(${rowData.id})">
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

function viewCircular(id) {
    alert("View circular with ID >> " + id);
}

function viewIssues(id) {
    alert("View Issues for ID >> " + id)
}

function deleteCircular(id) {
    alert("Delete circular with ID >> " + id);
}

