let processTatTable;

function initProcessTatListTable() {
    processTatTable = new Tabulator("#processTatTable", {
        ajaxURL: "/operations/workflow/processes/tat/all",
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
                        ["processName", "hodStatus", "complianceStatus", "BopStatus", "creditStatus", "treasuryStatus", "fintechStatus"].includes(f.field)
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
            alert("Failed to load tasks. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "PROCESS NAME",
                field: "processName",
                minWidth: 400,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
            },
            {
                title: "PROCESS STATUS",
                field: "processStatus",
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "HOD",
                field: "hodCount",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    let color = "#000000";
                    var bg = "";
                    if (value > 0 && value <= 5) {
                        bg = "#39BA0B";
                        color = "#FFFFFF";
                    }
                    else if (value > 5 && value <= 10) {
                        bg = "#EB7503";
                        color = "#FFFFFF";
                    }
                    else if (value > 10) {
                        bg = "#EB0303";
                        color = "#FFFFFF";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = color;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";
                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "RISK",
                field: "riskCount",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    let color = "#000000";
                    var bg = "";
                    if (value > 0 && value <= 5) {
                        bg = "#39BA0B";
                        color = "#FFFFFF";
                    }
                    else if (value > 5 && value <= 10) {
                        bg = "#EB7503";
                        color = "#FFFFFF";
                    }
                    else if (value > 10) {
                        bg = "#EB0303";
                        color = "#FFFFFF";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = color;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";
                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "COMPLIANCE",
                field: "complianceCount",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    let color = "#000000";
                    var bg = "";
                    if (value > 0 && value <= 5) {
                        bg = "#39BA0B";
                        color = "#FFFFFF";
                    }
                    else if (value > 5 && value <= 10) {
                        bg = "#EB7503";
                        color = "#FFFFFF";
                    }
                    else if (value > 10) {
                        bg = "#EB0303";
                        color = "#FFFFFF";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = color;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";
                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "BOP",
                field: "bopCount",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    let color = "#000000";
                    var bg = "";
                    if (value > 0 && value <= 5) {
                        bg = "#39BA0B";
                        color = "#FFFFFF";
                    }
                    else if (value > 5 && value <= 10) {
                        bg = "#EB7503";
                        color = "#FFFFFF";
                    }
                    else if (value > 10) {
                        bg = "#EB0303";
                        color = "#FFFFFF";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = color;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "TREASURY",
                field: "treasuryCount",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    let color = "#000000";
                    var bg = "";
                    if (value > 0 && value <= 5) {
                        bg = "#39BA0B";
                        color = "#FFFFFF";
                    }
                    else if (value > 5 && value <= 10) {
                        bg = "#EB7503";
                        color = "#FFFFFF";
                    }
                    else if (value > 10) {
                        bg = "#EB0303";
                        color = "#FFFFFF";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = color;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "FINTECH",
                field: "fintechCount",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    let color = "#000000";
                    var bg = "";
                    if (value > 0 && value <= 5) {
                        bg = "#39BA0B";
                        color = "#FFFFFF";
                    }
                    else if (value > 5 && value <= 10) {
                        bg = "#EB7503";
                        color = "#FFFFFF";
                    }
                    else if (value > 10) {
                        bg = "#EB0303";
                        color = "#FFFFFF";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = color;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "CREDIT",
                field: "creditCount",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    let color = "#000000";
                    var bg = "";
                    if (value > 0 && value <= 5) {
                        bg = "#39BA0B";
                        color = "#FFFFFF";
                    }
                    else if (value > 5 && value <= 10) {
                        bg = "#EB7503";
                        color = "#FFFFFF";
                    }
                    else if (value > 10) {
                        bg = "#EB0303";
                        color = "#FFFFFF";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = color;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "TOTAL",
                field: "totalCount",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    var bg = "";
                    let color = "#000000";
                    if (value > 0 && value <= 5) {
                        bg = "#39BA0B";
                        color = "#FFFFFF";
                    }
                    else if (value > 5 && value <= 10) {
                        bg = "#EB7503";
                        color = "#FFFFFF";
                    }
                    else if (value > 10) {
                        bg = "#EB0303";
                        color = "#FFFFFF";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = color;
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessTatSearch();
}

function initProcessTatSearch() {

}

$(document).ready(function () {

    initProcessTatListTable();

    $('#processTagForm').on('submit', function (e) {
        e.preventDefault();
    });

});