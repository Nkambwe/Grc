let bugsTable;

function initBugTable() {
   bugsTable = new Tabulator("#bugsTable", {
        ajaxURL: "/admin/configuration/bug-list",
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
                        ["finding", "recomendations", "proposedAction", "correctiveAction", "riskLevel"].includes(f.field)
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
            alert("Failed to load system errors. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                 formatter: () => `<span class="record-tab"></span>`,
                 width: 40,
                 headerSort: false,
                 frozen: true
             },
             {
                 title: "ERROR MESSAGE",
                 field: "error",
                 minWidth: 250,
                 frozen: true,
                 formatter: cell =>
                     `<span class="clickable-title" onclick="viewRecord(${cell.getRow().getData().id})">
                         ${cell.getValue()}
                     </span>`
             },
             {
                 title: "SEVERITY",
                 field: "severity",
                 hozAlign: "center",
                 headerFilter: "list",
                 headerHozAlign: "center",
                 headerFilterParams: {
                     values: {
                         "": "All",
                         "CRITICAL": "Critical",
                         "HIGH": "High",
                         "MEDIUM":"Medium",
                         "LOW": "Low"
                     }
                 },
                 formatter: cell =>
                     badgeFormatter(cell.getValue(), {
                         CRITICAL: "#dc2626",
                         HIGH: "#ea580c",
                         MEDIUM: "#ca8a04",
                         LOW: "#16a34a"
                 })
             },
             {
                 title: "STATUS",
                 field: "status",
                 hozAlign: "center",
                 headerFilter: "list",
                 headerHozAlign: "center",
                 headerFilterParams: {
                     values: {
                         "": "All",
                         "OPEN": "Open",
                         "PENDING": "In Progress",
                         "CLOSED": "Closed"
                     }
                 },
                 formatter: cell =>
                     badgeFormatter(cell.getValue(), {
                         OPEN: "#ef4444",
                         PENDING: "#f59e0b",
                         CLOSED: "#22c55e"
                     })
             },
             {
                 title: "REPORTED ON",
                 field: "createdOn",
                 hozAlign: "center",
                 headerHozAlign: "center",
                 formatter: cell => {
                     if (!cell.getValue()) return "";
                     return new Date(cell.getValue()).toLocaleDateString("en-GB");
                 }
             },
             {
                 title: "ACTION",
                 hozAlign: "center",
                 headerHozAlign: "center",
                 headerSort: false,
                 formatter: cell => `
                     <button class="grc-table-btn grc-btn-view" onclick="closeBug(${cell.getRow().getData().id})">
                         <i class="mdi mdi-check-circle-outline"></i> CLOSE
                     </button>`
             },
             { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });
}

function closeBug(id) {
    console.log("Close Bug ID >> " + id);
}

function badgeFormatter(value, colorMap) {
    if (!value) return "";

    const color = colorMap[value.toUpperCase()] || "#6b7280";

    return `
        <span style="
            display:inline-flex;
            align-items:center;
            justify-content:center;
            min-width:80px;
            padding:4px 12px;
            border-radius:12px;
            background:${color};
            color:#fff;
            font-weight:600;
            font-size:12px;
            text-transform:capitalize;
            letter-spacing:.3px;
        ">
            ${value}
        </span>
    `;
}

function severityFormatter(cell) {
    return badgeFormatter(cell.getValue(), {
        CRITICAL: "#dc2626",
        HIGH: "#ea580c",
        MEDIUM: "#ca8a04",
        LOW: "#16a34a"
    });
}

function statusFormatter(cell) {
    return badgeFormatter(cell.getValue(), {
        OPEN: "#ef4444",
        PENDING: "#f59e0b",
        CLOSED: "#22c55e"
    });
}

function dateFormatter(cell) {
    const value = cell.getValue();
    if (!value) return "";

    const date = new Date(value);
    return date.toLocaleDateString("en-GB");
}

function errorFormatter(cell) {
    const data = cell.getRow().getData();
    return `
        <span class="clickable-title"
            onclick="viewRecord(${data.id})">
            ${cell.getValue()}
        </span>
    `;
}

function closeFormatter(cell) {
    const data = cell.getRow().getData();
    return `
        <button class="grc-table-btn grc-btn-view"
            onclick="closeBug(${data.id})">
            <i class="mdi mdi-check-circle-outline"></i> CLOSE
        </button>
    `;
}

                        
$(document).ready(function () {

    initBugTable();

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });
    
});
