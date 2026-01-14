const auditColors = {
    "Total": { bg: "#8B5CF6", class1: "stat-card-light", class2: "stat-separator-primary" },
    "< 1 Month": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "1 to 2 Months": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "> 3 Months": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" }
};

const chartColors = {
    "Color1": { bg: "#5C0947", class: "" },
    "Color2": { bg: "#8C0E6C", class: "" },
    "Color3": { bg: "#A61180", class: "" },
    "Color4": { bg: "#C21495", class: "" },
    "Color5": { bg: "#E017AD", class: "" },
    "Color6": { bg: "#F519BD", class: "" },
    "Color7": { bg: "#FFAFF2", class: "" },
};

function generateAuditCards(record) {

    console.log(record);
    if (!record || !record.Statistics) return;

    const auditContainer = document.getElementById('auditCards');
    auditContainer.innerHTML = '';

    let data = record.Statistics.Findings;
    Object.keys(data).forEach(stat => {
        const total = data[stat];
        const colors = auditColors[stat];
        const route = reportRoutes[stat];

        if (!colors || !route) {
            console.warn(`Missing config for report: ${stat}`);
            return;
        }

        const card = document.createElement('div');
        card.className = `stat-card ${colors.class1}`;

        card.innerHTML = `
            <div class="stat-number">
                <span>${total}</span>
                <span>${stat}</span>
            </div>
            <span class="stat-separator ${colors.class2}"></span>
            <a class="stat-link" href="${route}">
                <span><i class="mdi mdi-chevron-right"></i></span>
                <span>See more...</span>
            </a>
        `;

        auditContainer.appendChild(card);
    });
}


//..generate piechart
const chartConfig = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            position: 'left',
            align: 'start',
            labels: {
                boxWidth: 12,
                padding: 20,
                usePointStyle: true,
                pointStyle: 'circle',
                generateLabels(chart) {
                    const labels = Chart.overrides.pie.plugins.legend.labels.generateLabels(chart);
                    labels.forEach(label => {
                        label.textAlign = 'left';
                    });
                    return labels;
                }
            }
        }
    },
    layout: {
        padding: { left: 5, right: 20, top: 20, bottom: 20 }
    }
};

let reportChart = null;

function renderReportChart(record) {
    if (!record) return;

    //..destroy old instance if it exists
    if (reportChart) {
        reportChart.destroy();
    }

    let statuses = record.Statistics.Completions;
    const statusLabels = Object.keys(statuses);
    const statusData = Object.values(statuses);
    const cardColorList = Object.values(chartColors);
    const deptColors = statusLabels.map((_, i) =>
        cardColorList[i % cardColorList.length].bg
    );

    reportChart = new Chart(document.getElementById("unitPieChart"), {
        type: 'pie',
        data: {
            labels: statusLabels,
            datasets: [{
                data: statusData,
                backgroundColor: deptColors
            }]
        },
        options: chartConfig
    });
}

$(document).ready(function () {
    console.log(`Audit Data >> `, auditData);
    generateAuditCards(auditData);
    renderReportChart(auditData);

    /* stack barchart */
    let statuses = auditData.Statistics.BarChart;
    const periods = Object.keys(statuses);

    //..get unique statuses (OPEN, CLOSED, etc.)
    const allStatuses = [
        ...new Set(
            Object.values(statuses)
                .flatMap(periodObj => Object.keys(periodObj))
        )
    ];

    const statusColors = {
        "Open": '#09B831',
        "Closed": '#5C0947',
        "Over Due": '#FF2413'
    };

    //...build datasets
    const datasets = allStatuses.map(status => ({
        label: status,
        data: periods.map(period => statuses[period][status] || 0),
        backgroundColor: statusColors[status] || '#FF2413'
    }));

    new Chart(document.getElementById("reportChart"), {
        type: 'bar',
        data: {
            labels: periods,
            datasets: datasets
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'EXCEPTION STATUS CHART',
                    font: {
                        size: 16,
                        weight: 'bold'
                    },
                    padding: {
                        top: 10,
                        bottom: 20
                    }
                },
                legend: {
                    position: 'top'
                }
            },
            scales: {
                x: { stacked: true },
                y: { stacked: true, beginAtZero: true }
            }
        }
    });
});