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
                        ["processName", "hodStatus", "complianceStatus", "bopStatus", "creditStatus", "treasuryStatus", "fintechStatus"].includes(f.field)
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
                formatter: (cell) => `<span class="clickable-title">${cell.getValue()}</span>`,
                cellClick: function (e, cell) {
                    e.stopPropagation();
                    viewTATRecord(cell.getRow().getData().id);
                }
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
    const searchInput = $('#tatSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val().trim();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processTatTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "hodStatus", type: "like", value: searchTerm },
                        { field: "complianceStatus", type: "like", value: searchTerm },
                        { field: "bopStatus", type: "like", value: searchTerm },
                        { field: "creditStatus", type: "like", value: searchTerm },
                        { field: "treasuryStatus", type: "like", value: searchTerm },
                        { field: "fintechStatus", type: "like", value: searchTerm }
                    ]
                ]);
                processTatTable.setPage(1, true);
            } else {
                processTatTable.clearFilter();
            }
        }, 300);
    });
}

function viewTATRecord(id) {

    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving process TAT record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findTAT(id)
        .then(record => {
            Swal.close();
            if (record) {
                openTatDialog(record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Process TAT record not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load process TAT details.' });
        });
   
}

function findTAT(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/tat/retrieve/${encodeURIComponent(id)}`,
            type: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            success: function (res) {
                if (res && res.success) {
                    resolve(res.data);
                } else {
                    resolve(null);
                }
            },
            error: function () {
                reject();
            }
        });
    });
}

function closeProcessPanel() {
    $('.process-overlay').removeClass('active');
    $('#collapsePanel').removeClass('active');
}


function openTatDialog(tat) {
    console.log("Process name >> " + tat?.processName);
    $("#processName").text(tat?.processName || "");
    $("#processStatus").text(tat?.processStatus || "");
    $("#processRequest").text(tat?.requestDate || "");
    $("#hodStatus").text(tat?.hodStatus  || "");
    $("#hodApprovalDate").text(tat?.hodEnddate || "");
    $("#hodComment").text(tat?.hodComment || "");
    $("#riskStatus").text(tat?.riskStatus || "");
    $("#riskApprovalDate").text(tat?.riskEnddate || "");
    $("#riskComment").text(tat?.riskComment || "");
    $("#compStatus").text(tat?.complianceStatus || "");
    $("#compApprovalDate").text(tat?.complianceEnddate || "");
    $("#compComment").text(tat?.complianceComment || "");
    $("#bropStatus").text(tat?.bropStatus || "");
    $("#bropApprovalDate").text(tat?.bropEnddate || "");
    $("#bropComment").text(tat?.bropComment || "");
    $("#treasStatus").text(tat?.treasuryStatus || "");
    $("#treasApprovalDate").text(tat?.treasuryEnddate || "");
    $("#treasuryComment").text(tat?.treasuryComment || "");
    $("#fintechStatus").text(tat?.fintechStatus || "");
    $("#finApprovalDate").text(tat?.fintechEnddate || "");
    $("#finComment").text(tat?.treasuryComment || "");

    // Show overlay panel
    $('.process-overlay').addClass('active');
    $('#collapsePanel').addClass('active');
}

//..toggle section collapse/expand
function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');

    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}

$(document).ready(function () {

    initProcessTatListTable();

    $('#btnTatExport').on('click', function () {

        console.log("Exporting TAT Report");
        $.ajax({
            url: '/operations/workflow/processes/tat/report',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(processTatTable.getData()),
            xhrFields: { responseType: 'blob' },
            success: function (blob) {
                let link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = "PROCESSTAT.xlsx";
                link.click();
            },
            error: function () {
                toastr.error("Export failed. Please try again.");
            }
        });
    });

});