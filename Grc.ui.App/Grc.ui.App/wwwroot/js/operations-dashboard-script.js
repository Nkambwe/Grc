
/* =======================
   Doughnut Chart Config
======================= */
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
                generateLabels(chart) {
                    // Use Chart.js 4+ override safe method
                    const labels = Chart.overrides.doughnut.plugins.legend.labels.generateLabels(chart);
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

/* =======================
   Stacked Bar Chart Config
======================= */
const barConfig = {
    responsive: true,
    maintainAspectRatio: false,
    indexAxis: 'x',  // vertical bars
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
        x: { stacked: true },
        y: { stacked: true, beginAtZero: true }
    }
};

/* =======================
   Doughnut Chart Creation
======================= */
const categoryLabels = Object.keys(categories); 
const categoryData = Object.values(categories); 
const categoryColors = categoryLabels.map((_, i) => `hsl(${i * 45},70%,60%)`);

new Chart(document.getElementById("categoryChart"), {
    type: 'doughnut',
    data: {
        labels: categoryLabels,
        datasets: [{
            data: categoryData,
            backgroundColor: categoryColors
        }]
    },
    options: doughnutConfig
});

/* =======================
   Stacked Bar Chart Creation
======================= */
const barCategories = Object.keys(statuses[0].Categories).filter(cat => cat !== "Total");

// Generate colors for each category
const barColors = barCategories.map((_, i) => `hsl(${i * 45},70%,60%)`);

// Map datasets: one dataset per category
const multiBarDatasets = barCategories.map((cat, index) => ({
    label: cat,
    data: statuses.map(s => s.Categories[cat] || 0),
    backgroundColor: barColors[index]
}));

// Create grouped bar chart
new Chart(document.getElementById("statusChart"), {
    type: 'bar',
    data: {
        labels: statuses.map(s => s.Status), // x-axis: each status
        datasets: multiBarDatasets
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                position: 'top',
                labels: { boxWidth: 12, padding: 15 }
            }
        },
        scales: {
            x: {
                stacked: false, // important for grouped bars
                title: {
                    display: true,
                    text: 'Status'
                }
            },
            y: {
                stacked: false,
                beginAtZero: true,
                title: {
                    display: true,
                    text: 'Count'
                }
            }
        }
    }
});
