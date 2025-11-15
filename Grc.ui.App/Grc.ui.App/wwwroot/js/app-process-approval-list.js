let processApprovalsTable;
function initProcessApprovalListTable() {
    processApprovalsTable = new Tabulator("#processApprovalsTable", {
        ajaxURL: "/operations/workflow/processes/approvals-all",
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
                formatter: (cell) => `<span class="clickable-title" onclick="viewApproval(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "REQUESTED ON",
                formatter: function (cell) {
                    const value = cell.getRow().getData().requestDate;

                    if (!value)
                        return "";

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
                frozen: true, 
                headerSort: true
            },
            {
                title: "HOD STATUS",
                field: "hodStatus",
                formatter: function (cell) {
                    const value = cell.getValue(); 
                    const cellEl = cell.getElement();

                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";     
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
                title: "RISK STATUS",
                field: "riskStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();

                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
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
                title: "COMPLIA.. STATUS",
                field: "complianceStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
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
                title: "NEED BOP",
                formatter: function (cell) {
                    const needBop = cell.getRow().getData().requiresBopApproval;
                    let value = needBop ? "YES" : "NO";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${value}</span>
                            </div>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "BOP STATUS",
                field: "bopStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    } 
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
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
                title: "NEED CREDIT",
                formatter: function (cell) {
                    const needCredit = cell.getRow().getData().requiresCreditApproval;
                    let value = needCredit ? "YES" : "NO";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${value}</span>
                            </div>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "CREDIT STATUS",
                field: "creditStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
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
                title: "NEED TREASURY",
                formatter: function (cell) {
                    const needTreasury = cell.getRow().getData().requiresTreasuryApproval;
                    let value = needTreasury ? "YES" : "NO";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${value}</span>
                            </div>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "TREASURY STATUS",
                field: "treasuryStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
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
                title: "NEED FINTECH",
                formatter: function (cell) {
                    const needFintech = cell.getRow().getData().requiresFintechApproval;
                    let value = needFintech ? "YES" : "NO";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${value}</span>
                            </div>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "FINTECH STATUS",
                field: "fintechStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
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
                title: "VIEW",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="holdApproval(${rowData.processId})">
                        <span><i class="mdi mdi-eye-arrow-right-outline" aria-hidden="true"></i></span>
                        <span>PROGRESS</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessApprovalsSearch();
}

function initProcessApprovalsSearch() {
    const searchInput = $('#approvalsSearchbox');
    let typingTimer;
    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processNewTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "hodStatus", type: "like", value: searchTerm },
                        { field: "complianceStatus", type: "like", value: searchTerm },
                        { field: "BopStatus", type: "like", value: searchTerm },
                        { field: "creditStatus", type: "like", value: searchTerm },
                        { field: "treasuryStatus", type: "like", value: searchTerm },
                        { field: "fintechStatus", type: "like", value: searchTerm }
                    ]
                ]);
                processReviewTable.setPage(1, true);
            } else {
                processReviewTable.clearFilter();
            }
        }, 300);
    });
}

function holdApproval(id) {
    alert("Put this process on hold " + id);
}

function viewApproval(id) {
    alert("View Approval " + id);
}

$(document).ready(function () {

    initProcessApprovalListTable();

    $('#processReviewForm').on('submit', function (e) {
        e.preventDefault();
    });

});

