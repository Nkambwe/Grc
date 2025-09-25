
const doughnutConfig = {
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
                generateLabels: function (chart) {
                    const original = Chart.defaults.plugins.legend.labels.generateLabels;
                    const labels = original.call(this, chart);
                    labels.forEach(label => {
                        label.textAlign = 'left';
                    });
                    return labels;
                }
            }
        }
    },
    layout: {
        padding: {
            left: 5,
            right: 20,
            top: 20,
            bottom: 20
        }
    }
};

//..cpecial config for bar chart
const barConfig = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            position: 'left',
            align: 'start',
            labels: {
                boxWidth: 12,
                padding: 15,
                usePointStyle: true,
                pointStyle: 'circle'
            }
        }
    },
    scales: {
        x: {
            stacked: true
        },
        y: {
            stacked: true,
            beginAtZero: true
        }
    }
};

//..coughnut Chart for categories
new Chart(document.getElementById("categoryChart"), {
    type: 'doughnut',
    data: {
        labels: Object.keys(categories),
        datasets: [{
            data: Object.values(categories),
            backgroundColor: Object.keys(categories).map((_, i) => `hsl(${i * 45},70%,60%)`)
        }]
    },
    options: doughnutConfig
});

//..stacked Bar Chart for status vs categories
const cats = Object.keys(statuses[0].Categories);

//..create consistent colors for bar chart
const barColors = cats.map((_, i) => `hsl(${i * 45},70%,60%)`);

const datasets = cats.map((cat, index) => ({
    label: cat,
    data: statuses.map(s => s.Categories[cat] || 0),
    backgroundColor: barColors[index]
}));

new Chart(document.getElementById("statusChart"), {
    type: 'bar',
    data: {
        labels: statuses.map(s => s.Status),
        datasets
    },
    options: barConfig 
});