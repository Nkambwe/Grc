
let exceptionsTable = new Tabulator("#exceptionsTable", {
    ajaxURL: "/grc/compliance/audit/exceptions/mini-report-list",
    paginationMode: "remote",
    filterMode: "remote",
    sortMode: "remote",
    pagination: true,
    paginationSize: 10,
    paginationSizeSelector: [10, 20, 35, 40, 50],
    paginationCounter: "rows",
    ajaxConfig: {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
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

            if (params.sort && params.sort.length > 0) {
                requestBody.sortBy = params.sort[0].field;
                requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
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
        Swal.fire("Error", "Failed to load audit exception reports. Please try again.", "error");
    },
    layout: "fitColumns",
    responsiveLayout: "hide",
    layoutColumnsOnNewData: false,
    columns: [
        {
            title: "REF",
            field: "reference",
            width: 200,
            headerSort: true,
            resizable: false, 
            formatter: function (cell) {
                const id = cell.getRow().getData().id;
                console.log(id);
                return `<span class="clickable-title" onclick="viewReport(${id})">${cell.getValue()}</span>`;
            }
        },
        {
            title: "AUDIT REPORT",
            field: "reportName",
            widthGrow: 2,
            minWidth: 200,
            resizable: false, 
            headerSort: true
        },
        {
            title: "REPORT DATE",
            field: "auditedOn",
            minWidth: 200,
            headerSort: true,
            resizable: false, 
            formatter: function (cell) {
                const date = new Date(cell.getValue());
                return date.toLocaleDateString('en-GB', {
                    day: '2-digit',
                    month: 'short',
                    year: 'numeric'
                });
            }
        },
        {
            title: "AUDIT TYPE",
            field: "auditType",
            minWidth: 200,
            headerSort: true,
            resizable: false,
            hozAlign: "center",
            formatter: function (cell) {
                const ref = cell.getRow().getData().reference || "";
                const type = ref.split('/')[0]?.trim() || "N/A";

                const el = cell.getElement();
                el.style.backgroundColor = "#0d6efd";
                el.style.color = "#fff";
                el.style.fontWeight = "600";
                el.style.borderRadius = "4px";

                return type;
            }
        },
        {
            title: "FINDINGS",
            field: "total",
            minWidth: 150,
            hozAlign: "center",
            headerHozAlign: "center",
            resizable: false, 
            headerSort: true,
            formatter: function (cell) {
                return `<span class="fw-bold">${cell.getValue() || 0}</span>`;
            }
        },
        {
            title: "CLOSED",
            field: "closed",
            minWidth: 150,
            hozAlign: "center",
            headerHozAlign: "center",
            resizable: false,
            headerSort: true,
            formatter: function (cell) {
                const el = cell.getElement();
                el.classList.add("closed-cell");
                return cell.getValue() || 0;
            }
        },
        {
            title: "OPEN",
            field: "open",
            minWidth: 150,
            hozAlign: "center",
            headerHozAlign: "center",
            resizable: false,
            headerSort: true,
            formatter: function (cell) {
                const el = cell.getElement();
                el.classList.remove("open-danger", "open-warning");
                const overdue = cell.getRow().getData().overdue || 0;

                if (overdue > 0) {
                    el.classList.add("open-danger");
                } else {
                    el.classList.add("open-warning");
                }

                return cell.getValue() || 0;
            }
        },
        {
            title: "% COMPLETED",
            field: "completed",
            minWidth: 200,
            hozAlign: "center",
            resizable: false, 
            headerSort: true,
            formatter: function (cell) {
                const value = parseFloat(cell.getValue() || 0);
                const color = value >= 80 ? "success" : value >= 50 ? "warning" : "danger";
                return `
                    <div class="progress" style="height: 20px;">
                        <div class="progress-bar bg-${color}" role="progressbar" 
                             style="width: ${value}%" 
                             aria-valuenow="${value}" 
                             aria-valuemin="0" 
                             aria-valuemax="100">
                            ${value.toFixed(1)}%
                        </div>
                    </div>
                `;
            }
        },
        {
            title: "% OUTSTANDING",
            field: "outstanding",
            minWidth: 200,
            hozAlign: "center",
            resizable: false, 
            headerSort: true,
            formatter: function (cell) {
                const value = parseFloat(cell.getValue() || 0);
                const color = value > 50 ? "danger" : value > 20 ? "warning" : "success";
                return `
                    <div class="progress" style="height: 20px;">
                        <div class="progress-bar bg-${color}" role="progressbar" 
                             style="width: ${value}%" 
                             aria-valuenow="${value}" 
                             aria-valuemin="0" 
                             aria-valuemax="100">
                            ${value.toFixed(1)}%
                        </div>
                    </div>
                `;
            }
        }
    ]
});

function viewReport(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving Audit Report...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findAuditReport(id)
        .then(record => {
            console.log(`Then >> `, record);
            Swal.close();
            if (record) {
                openAuditReportPanel(record);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Audit Report not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            console.error('Error loading Audit Report:', error);
            Swal.close();

            Swal.fire({
                title: 'Error',
                text: 'Failed to load Audit Report details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findAuditReport(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/audit/exceptions/mini-report-retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
            }
        });
    });
}

function openAuditReportPanel(record) {
    console.log(`Open >> `, record);
    //..initialize form fields
    $('#reportId').val(record.id);
    $('#reference').val(record.reference || '');
    $('#reportName').val(record.reportName || '');
    $('#auditedOn').val(record.auditedOn || '');
    $('#status').val(record.status || '');
    $('#total').val(record.total || 0);
    $('#open').val(record.open || 0);
    $('#close').val(record.close || 0);
    $('#overdue').val(record.overdue || 0);

    //..add exception list
    addExceptions(record.exceptions);

    //..load dialog window
    closeAuditReport();
    $('#auditPanel').addClass('active');
    $('#auditOverlay').addClass('active');
    $('body').css('overflow', 'hidden');
}

function addExceptions(exceptions) {
    //..load table data
    const tableBody = document.querySelector('#exceptionsListTable tbody');
    tableBody.innerHTML = '';
    exceptions.forEach((report, index) => {
        const tr = document.createElement('tr');

        let statusColor = "#FF2413";
        if (report.status === "CLOSED") {
            statusColor = "#09B831";
        } else if (report.status === "OPEN") {
            statusColor = "#FF8503";
        } else {
            statusColor = "#FF2413";
        }

        let riskColor = "#2DDB09";
        if (report.riskStatement === "HIGH") {
            riskColor = "#FF9333";
        } else if (report.riskStatement === "CRITICAL") {
            riskColor = "#FF2413";
        } else if (report.riskStatement === "MEDIUM") {
            riskColor = "#FFC604";
        } else if (report.riskStatement === "LOW") {
            riskColor = "#F2DF0C";
        } 

        tr.innerHTML = `
            <td>${report.findings}</td>
            <td>${report.proposedAction}</td>
            <td style="
                background-color:${statusColor};
                color:#FFFFFF;
                font-weight:600;
                text-align:center;">
                ${report.status}
            </td>
            <td style="
                background-color:${riskColor};
                color:#FFFFFF;
                font-weight:600;
                text-align:center;">
                ${report.riskStatement}
            </td>
            <td>${report.responsible}</td>
            <td>${report.excutioner}</td>
        `;
        tableBody.appendChild(tr);
    });
}

function closeAuditReport() {
    $('#auditPanel').removeClass('active');
    $('#auditOverlay').removeClass('active');
    $('body').css('overflow', '');
}

//..route to home
$('.action-btn-audit-home').on('click', function () {
    try {
        window.location.href = '/grc/compliance/audit/dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});


$(document).ready(function () {

    $('#auditForm').on('submit', function (e) {
        e.preventDefault();
    });

});