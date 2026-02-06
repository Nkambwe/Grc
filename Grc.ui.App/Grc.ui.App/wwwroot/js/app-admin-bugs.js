let bugsTable;

function initBugTable() {
    bugsTable = new Tabulator("#bugsTable", {
        ajaxURL: "/admin/configuration/bug-list",
        ajaxConfig: {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        },
        ajaxContentType: "json",

        pagination: true,
        paginationMode: "remote",
        paginationSize: 10,
        paginationSizeSelector: [10, 20, 35, 50],
        paginationCounter: "rows",

        filterMode: "remote",
        sortMode: "remote",

        paginationDataSent: {
            page: "page",
            size: "size",
            sorters: "sort",
            filters: "filter"
        },
        paginationDataReceived: {
            data: "data",
            last_page: "last_page",
            total_records: "total_records"
        },

        ajaxRequestFunc: (url, config, params) => {
            const requestBody = {
                pageIndex: params.page || 1,
                pageSize: params.size || 10,
                filters: params.filter || [],
                sortBy: params.sort?.[0]?.field || "",
                sortDirection: params.sort?.[0]?.dir === "asc" ? "Ascending" : "Descending"
            };

            return $.ajax({
                url,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(requestBody)
            });
        },

        ajaxResponse: (url, params, response) => ({
            data: response.data || [],
            last_page: response.last_page || 1,
            total_records: response.total_records || 0
        }),

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
                title: "ERROR",
                field: "error",
                minWidth: 250,
                frozen: true,
                formatter: cell =>
                    `<span class="clickable-title"
                        onclick="viewRecord(${cell.getRow().getData().id})">
                        ${cell.getValue()}
                    </span>`
            },
            {
                title: "SEVERITY",
                field: "severity",
                hozAlign: "center",
                headerFilter: "list",
                headerFilterParams: {
                    values: {
                        "": "All",
                        "CRITICAL": "Critical",
                        "HIGH": "High",
                        "MEDIUM": "Medium",
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
                formatter: cell => {
                    if (!cell.getValue()) return "";
                    return new Date(cell.getValue()).toLocaleDateString("en-GB");
                }
            },
            {
                title: "ACTION",
                hozAlign: "center",
                headerSort: false,
                formatter: cell => `
                    <button class="grc-table-btn grc-btn-view"
                        onclick="closeBug(${cell.getRow().getData().id})">
                        <i class="mdi mdi-check-circle-outline"></i> CLOSE
                    </button>`
            }
        ]
    });
}

function closeBug(id) {
    console.log("Close Bug ID >> " + id);
}

                        
$(document).ready(function () {

    initBugTable();

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });
    
});
