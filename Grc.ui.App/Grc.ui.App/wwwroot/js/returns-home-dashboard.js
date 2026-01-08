const returnColors = {
    "TOTALS": { bg: "#8B5CF6", class1: "stat-card-light", class2: "stat-separator-primary" },
    "NA": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "ONE-OFF": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "ON OCCURRENCE": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "PERIODIC": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "DAILY": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "WEEKLY": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "MONTHLY": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "QUARTERLY": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "ANNUAL": { bg: "#10B981", class: "stat-card-light", class2: "stat-separator-primary" },
    "BIENNIAL": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "BIANNUAL": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "TRIENNIAL": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },

};

function generateInnerCards(record) {

    console.log(record);
    if (!record || !record.Returns) return;

    const returnsContainer = document.getElementById('returnsCards');
    returnsContainer.innerHTML = '';

    let data = record.Returns.Periods;
    Object.keys(data).forEach(period => {
        const total = data[period];
        const colors = returnColors[period];
        const route = reportRoutes[period];

        if (!colors || !route) {
            console.warn(`Missing config for report: ${period}`);
            return;
        }

        const card = document.createElement('div');
        card.className = `stat-card ${colors.class1}`;

        card.innerHTML = `
            <div class="stat-number">
                <span>${total}</span>
                <span>${period}</span>
            </div>
            <span class="stat-separator ${colors.class2}"></span>
            <a class="stat-link" href="${route}">
                <span><i class="mdi mdi-chevron-right"></i></span>
                <span>See more...</span>
            </a>
        `;

        returnsContainer.appendChild(card);
    });
}

document.addEventListener('DOMContentLoaded', function () {
    generateInnerCards(returnsData);

    /* stack barchart */
    let statuses = returnsData.Returns.Statuses;
    const periods = Object.keys(statuses);

    //..get unique statuses (OPEN, CLOSED, etc.)
    const allStatuses = [
        ...new Set(
            Object.values(statuses)
                .flatMap(periodObj => Object.keys(periodObj))
        )
    ];

    const statusColors = {
        OPEN: '#6A1B9A',
        CLOSED: '#F75F0D'
    };

    //...build datasets
    const datasets = allStatuses.map(status => ({
        label: status,
        data: periods.map(period => statuses[period][status] || 0),
        backgroundColor: statusColors[status] || '#9E9E9E'
    }));

    new Chart(document.getElementById("statusChart"), {
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
                    text: 'PERIODIC RETURNS CHART',
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