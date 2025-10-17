document.addEventListener('DOMContentLoaded', () => {
    renderDashboard(dashboardRecord);
});

function renderDashboard(record) {
    if (!record) return;

    //..cards for each operation unit
    const unitCardsContainer = document.getElementById('unitCards');
    unitCardsContainer.innerHTML = '';

    Object.keys(record.Categories).forEach(unit => {
        const total = record.Categories[unit];
        const card = document.createElement('div');
        card.className = 'stats-category-card';
        card.innerHTML = `
            <div class="stats-card-title">${unit}</div>
            <div class="stats-card-value">${total}</div>
        `;
        unitCardsContainer.appendChild(card);
    });

    //..pie chart of categories for this record
    const categoryLabels = Object.keys(record.Categories);
    const categoryValues = Object.values(record.Categories);
    const categoryColors = categoryLabels.map((_, i) => `hsl(${i * 45}, 70%, 60%)`);

    new Chart(document.getElementById('unitPieChart'), {
        type: 'doughnut',
        data: {
            labels: categoryLabels,
            datasets: [{
                data: categoryValues,
                backgroundColor: categoryColors
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'left',
                    align: 'start'
                }
            }
        }
    });

    //..table of processes
    const tableBody = document.querySelector('#processTable tbody');
    tableBody.innerHTML = '';

    Object.entries(record.Categories).forEach(([category, value]) => {
        const tr = document.createElement('tr');
        tr.innerHTML = `<td>${category}</td><td>${value}</td>`;
        tableBody.appendChild(tr);
    });
}
