
function initReportsTable(reports) {
    new Tabulator("#exceptionsTable", {
        data: reports,
        layout: "fitColumns",
        pagination: "local",
        pagination: true,
        paginationSize: 10,
        paginationSizeSelector: [10, 20, 35, 40, 50],
        responsiveLayout: "hide",
        paginationCounter: "rows",
        height: "100%",
        reactiveData: true,
        layoutColumnsOnNewData: false,
        columns: [
            {
                title: "REF",
                field: "reference",
                headerFilter: "input",
                width:200
            },
            {
                title: "Report Name",
                field: "reportName",
                headerFilter: "input",
                widthGrow: 2,
                minWidth: 200,
                resizable: false,
                formatter: function (cell) {
                    const id = cell.getRow().getData().id;
                    return `<span class="clickable-title" onclick="viewReport(${id})">${cell.getValue()}</span>`;
                }
            },
            {
                title: "Audited On",
                field: "auditedOn",
                headerFilter: "input",
                widthGrow: 2,
                minWidth: 200,
                formatter: function (cell) {
                    const value = cell.getValue();
                    return value
                        ? new Date(value).toLocaleDateString()
                        : "";
                }
            },
            { title: "EXC OPEN", field: "open", hozAlign: "left", width: 200 },
            { title: "EXC CLOSED", field: "closed", hozAlign: "left", width: 200 },
            { title: "EXC DUE", field: "overdue", hozAlign: "left", width: 200 },
            {
                title: "STATUS",
                field: "status",
                headerFilter: "list",
                headerFilterParams: {
                    values: [
                        { label: "All", value: "" },
                        { label: "OPEN", value: "Open" },
                        { label: "CLOSED", value: "Closed" },
                        { label: "OVER DUE", value: "Due" }
                    ],
                    clearable: true
                },
                hozAlign: "center",
                formatter: function (cell) {
                    const status = cell.getValue();
                    const el = cell.getElement();

                    let bg = "#FF2413";
                    if (status === "Closed") bg = "#09B831";
                    else if (status === "Open") bg = "#FF8503";

                    el.style.backgroundColor = bg;
                    el.style.color = "#FFFFFF";
                    el.style.fontWeight = "600";
                    el.style.textAlign = "center";

                    return status ?? "—";
                }
            }
        ]
    });
}

function renderAuditStats(statuses) {
    const container = document.getElementById("auditCard");
    container.innerHTML = "";

    const items = [
        { label: "Total", value: statuses.Total },
        { label: "Open", value: statuses.Open, class:"open-warning" },
        { label: "Due", value: statuses.Due, class: "open-danger" },
        { label: "Closed", value: statuses.Close, class: "open-success" }
    ];

    items.forEach(item => {
        const div = document.createElement("div");
        div.className = "audit-stat";
        div.innerHTML = `
            <div class="value ${item.class}">${item.value}</div>
            <div class="label ${item.class}">${item.label}</div>
        `;
        container.appendChild(div);
    });
}

function loadAuditDashboard(stats) {
    renderAuditStats(stats.Statuses);
    initReportsTable(stats.Reports);
}

$('.action-btn-audit-home').on('click', function () {
    try {
        window.location.href = '/grc/compliance/audit/dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
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

    findInnerAuditReport(id)
        .then(record => {
            console.log(`Then >> `, record);
            Swal.close();
            if (record) {
                openAuditInnerReportPanel(record);
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

function findInnerAuditReport(id) {
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

function openAuditInnerReportPanel(record) {
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
    addInnerExceptions(record.exceptions);

    //..load dialog window
    closeExceptioneInnerPanel();
    $('#auditPanel').addClass('active');
    $('#auditOverlay').addClass('active');
    $('body').css('overflow', 'hidden');
}

function addInnerExceptions(exceptions) {
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

function closeExceptioneInnerPanel() {
    $('#auditPanel').removeClass('active');
    $('#auditOverlay').removeClass('active');
    $('body').css('overflow', '');
}


$(document).ready(function () {
    //..load stats
    var stats = auditData.Statistics;
    loadAuditDashboard(stats)

    $('#auditForm').on('submit', function (e) {
        e.preventDefault();
    });

});