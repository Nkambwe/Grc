
const circularColors = {
    "TOTALS": { bg: "#8B5CF6", class1: "stat-card-light", class2: "stat-separator-primary" },
    "BOU": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "CMA": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "UMRA": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "IRAU": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "FIA": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "PPDA": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "URBRA": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "MoFED": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "URA": { bg: "#10B981", class: "stat-card-light", class2: "stat-separator-primary" },
    "PDPO": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "AG": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "UIB": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },
    "NIRA": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },
    "DPF": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },
    "OTHERS": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },
};

function generateCircularCards(record) {
    if (!record || !record.Circulars) return;

    const circularContainer = document.getElementById('circularCards');
    circularContainer.innerHTML = '';

    let data = record.Circulars.Authorities;
    console.log(data);
    Object.keys(data).forEach(circular => {
        const total = data[circular];
        const colors = circularColors[circular];
        const route = circularRoutes[circular];

        if (!colors || !route) {
            console.warn(`Missing config for circular: ${circular}`);
            return;
        }

        const card = document.createElement('div');
        card.className = `stat-card ${colors.class1}`;

        card.innerHTML = `
            <div class="stat-number">
                <span>${total}</span>
                <span>${circular}</span>
            </div>
            <span class="stat-separator ${colors.class2}"></span>
            <a class="stat-link" href="${route}">
                <span><i class="mdi mdi-chevron-right"></i></span>
                <span>See more...</span>
            </a>
        `;

        circularContainer.appendChild(card);
    });
}

document.addEventListener('DOMContentLoaded', function () {

    console.log(circularData);
    generateCircularCards(circularData);

    /* stack barchart */
    let statuses = circularData.Circulars.Statuses;
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
                    text: 'PERIODIC CIRCULAR CHART',
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