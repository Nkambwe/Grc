$(document).ready(function () {
    initObligationTable();
});

const data = [
    {
        id: 1,
        act: "Financial Institutions Act, 2004 AS Amended by the Financial Institution Act 2016",
        isParent: true,
        _children: [
            {
                id: 11,
                summary: "Shareholding in a Financial Institution",
                description: "No individual or body corporate controlled by one individual shall own or acquire more than 49% of shares of a financial institution",
                coverage: 100,
                covered: true,
                issues: 0,
                locked: false
            },
            {
                id: 12,
                summary: "Capital Adequacy Requirements",
                description: "Financial institutions must maintain a minimum capital adequacy ratio of 12% of risk-weighted assets",
                coverage: 100,
                covered: true,
                issues: 0,
                locked: true
            },
            {
                id: 13,
                summary: "Liquidity Management",
                description: "Maintain liquid assets at minimum 20% of total deposit liabilities",
                coverage: 55,
                covered: false,
                issues: 1,
                locked: false
            }
        ]
    },
    {
        id: 2,
        act: "Anti-Money Laundering Act, 2013",
        isParent: true,
        _children: [
            {
                id: 21,
                summary: "Customer Due Diligence",
                description: "Conduct enhanced due diligence for all high-risk customers and politically exposed persons",
                coverage: 90,
                covered: true,
                issues: 0,
                locked: true
            },
            {
                id: 22,
                summary: "Suspicious Transaction Reporting",
                description: "Report all suspicious transactions to FIU within 24 hours of detection",
                coverage: 100,
                covered: true,
                issues: 0,
                locked: true
            }
        ]
    }
];

let obligationTable;
function initObligationTable() {
    obligationTable = new Tabulator("#obligations-table", {
        data: data,
        dataTree: true,
        dataTreeStartExpanded: false,
        layout: "fitColumns",
        responsiveLayout: "collapse",
        columns: [
            {
                title: "Summary",
                field: "summary",
                widthGrow: 2,
                formatter: function (cell, formatterParams) {
                    const data = cell.getData();
                    if (data.isParent) {
                        return `<div class="parent-row">${data.act}</div>`;
                    }
                    return cell.getValue();
                }
            },
            {
                title: "Description of Obligation",
                field: "description",
                widthGrow: 3,
                formatter: function (cell, formatterParams) {
                    const data = cell.getData();
                    if (data.isParent) {
                        return "";
                    }
                    return cell.getValue();
                }
            },
            {
                title: "Coverage",
                field: "coverage",
                widthGrow: 1,
                hozAlign: "center",
                formatter: function (cell, formatterParams) {
                    const value = cell.getValue();
                    const data = cell.getData();
                    if (data.isParent) {
                        if (data._children && data._children.length > 0) {
                            const total = data._children.reduce((sum, child) => sum + (child.coverage || 0), 0);
                            const avg = Math.round(total / data._children.length);
                            return `<strong>${avg}%</strong>`;
                        }
                    }
                    return value ? `${value}%` : "—";
                }
            },
            {
                title: "Covered",
                field: "covered",
                widthGrow: 1,
                hozAlign: "center",
                formatter: function (cell, formatterParams) {
                    const value = cell.getValue();
                    if (value === true) {
                        return `<i class="mdi mdi-check-circle covered-icon covered-yes"></i>`;
                    } else if (value === false) {
                        return `<i class="mdi mdi-close-circle covered-icon covered-no"></i>`;
                    }
                    return "—";
                }
            },
            {
                title: "Issues",
                field: "issues",
                widthGrow: 1,
                hozAlign: "center",
                formatter: function (cell, formatterParams) {
                    const value = cell.getValue();
                    const data = cell.getData();
                    if (data.isParent && data._children) {
                        // Sum up all issues from children
                        const totalIssues = data._children.reduce((sum, child) => sum + (child.issues || 0), 0);
                        if (totalIssues > 0) {
                            return `<span class="issues-count"><i class="mdi mdi-alert-circle" style="color: #e74c3c;"></i> ${totalIssues}</span>`;
                        }
                        return `<span class="issues-count" style="color: #27ae60;">0</span>`;
                    }
                    if (value > 0) {
                        return `<span class="issues-count"><i class="mdi mdi-alert-circle" style="color: #e74c3c;"></i> ${value}</span>`;
                    }
                    return `<span style="color: #27ae60;">0</span>`;
                }
            },
            {
                title: "Locked",
                field: "locked",
                widthGrow: 1,
                hozAlign: "center",
                formatter: function (cell, formatterParams) {
                    const value = cell.getValue();
                    if (value === true) {
                        return `<span class="locked-badge"><i class="mdi mdi-lock locked-icon"></i> Locked</span>`;
                    }
                    return "—";
                }
            }
        ],
        rowFormatter: function (row) {
            const data = row.getData();
            if (data.isParent) {
                row.getElement().classList.add("parent-row-style");
            }
        }
    });
}

$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

$('.action-btn-oblig-register').on('click', function () {
    window.location.href = '/grc/register/register-list';
});

