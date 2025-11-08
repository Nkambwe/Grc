
// category color mapping
const cardColors = {
    "UpToDate": { bg: "#10B981", class: "card-uptodate" },
    "Unclassified": { bg: "#3B82F6", class: "card-proposed" },
    "Cancelled": { bg: "#3E8DF6", class: "card-cancelled" },
    "Unchanged": { bg: "#8B5CF6", class: "card-unchanged" },
    "Need Review": { bg: "#F59E0B", class: "card-due" },
    "Dormant": { bg: "#EF4444", class: "card-dormant" }
};

//..generate status total cards
function generateCategoryCards() {
    const container = document.getElementById('categoryCards');

    //..aggregate totals by status
    const categoryTotals = {};
    chartData.forEach(item => {
        const total = item.Categories["Total"];
        if (categoryTotals[item.Banner]) {
            categoryTotals[item.Banner] += total;
        } else {
            categoryTotals[item.Banner] = total;
        }
    });

    //..create cards
    Object.keys(categoryTotals).forEach(status => {
        const card = document.createElement('div');
        card.className = `stats-category-card ${cardColors[status].class}`;
        card.innerHTML = `
                    <div class="stats-card-title">${status}</div>
                    <div class="stats-card-value">${categoryTotals[status]}</div>
                `;
        container.appendChild(card);
    });
}

//..generate charts for each operation unit
function generateCharts() {
    const container = document.getElementById('categoryChartsContainer');

    //..get all operation units
    const operationUnits = Object.keys(chartData[0].Categories).filter(unit => unit !== "Total");
    operationUnits.forEach(unit => {
        // Create chart card
        const chartCard = document.createElement('div');
        chartCard.className = 'category-chart-card ';
        chartCard.innerHTML = `
                    <div class="chart-card-title">${unit}</div>
                    <div class="chart-category-container">
                        <canvas id="chart-${unit.replace(/\s+/g, '-')}"></canvas>
                    </div>
                `;
        container.appendChild(chartCard);

        // Prepare data for this category
        const chartData = prepareChartData(unit);

        // Create chart
        createHorizontalChart(`chart-${unit.replace(/\s+/g, '-')}`, chartData);
    });
}

//..prepare chart data for a specific operation unit
function prepareChartData(unit) {
    const labels = [];
    const values = [];
    const colors = [];

    //..aggregate data by status
    const statusData = {};
    chartData.forEach(item => {
        const status = item.Banner;
        const value = item.Categories[unit];
        if (statusData[status]) {
            statusData[status] += value;
        } else {
            statusData[status] = value;
        }
    });

    // Convert to arrays
    Object.keys(statusData).forEach(status => {
        labels.push(status);
        values.push(statusData[status]);
        colors.push(cardColors[status].bg);
    });

    return { labels, values, colors };
}

// Create horizontal bar chart
function createHorizontalChart(canvasId, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        return;
    }

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.labels,
            datasets: [{
                label: 'Processes',
                data: data.values,
                backgroundColor: data.colors,
                borderColor: data.colors,
                borderWidth: 1,
                barThickness: 18,
                maxBarThickness: 25
            }]
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                        font: { size: 11 }
                    },
                    grid: {
                        drawBorder: false
                    }
                },
                y: {
                    grid: { display: false },
                    ticks: {
                        font: { size: 11 }
                    }
                }
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    padding: 12,
                    titleFont: { size: 13 },
                    bodyFont: { size: 12 }
                }
            }
        }
    });
}

// Initialize dashboard
document.addEventListener('DOMContentLoaded', function () {
    generateCategoryCards();
    generateCharts();
});