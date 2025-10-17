// category color mapping
const cardColors = {
    "Account Services": { bg: "#B759E3", class: `${cardColor}` },
    "Cash": { bg: "#858585", class: `${cardColor}` },
    "Channels": { bg: "#3E8DF6", class: `${cardColor}` },
    "Customer Experience": { bg: "#EC007A", class: `${cardColor}` },
    "Reconciliation": { bg: "#FFA000", class: `${cardColor}` },
    "Records Management": { bg: "#CF5C22", class: `${cardColor}` },
    "Payment": { bg: "#00BA83", class: `${cardColor}` },
    "E-Wallet": { bg: "#4F4F4F", class: `${cardColor}` }
};

//..generate unit cards
function generateUnitCards(record) {

    if (!record) {
        return;
    }

    const unitCardsContainer = document.getElementById('unitCards');
    unitCardsContainer.innerHTML = '';

    //..create cards
    Object.keys(record.CategoryProcesses).forEach(unit => {
        const total = record.CategoryProcesses[unit];
        const card = document.createElement('div');
        card.className = `stats-category-card  ${cardColors[unit].class}`;
        card.innerHTML = `
            <div class="stats-card-title">${unit}</div>
            <div class="stats-card-value">${total}</div>
        `;
        unitCardsContainer.appendChild(card);
    });
}

//..generate piechart
const piechartConfig = {
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

let unitChart = null;
function renderPiechart(record) {
    if (!record) return;

    //..destroy old instance if it exists
    if (unitChart) {
        unitChart.destroy();
    }

    const units = record.CategoryProcesses;
    const unitLabels = Object.keys(units);
    const unitData = Object.values(units);
    const unitColors = unitLabels.map((label, i) => {
        return cardColors[label] ? cardColors[label].bg : `hsl(${i * 45}, 70%, 60%)`;
    });

    unitChart = new Chart(document.getElementById("unitPieChart"), {
        type: 'pie',
        data: {
            labels: unitLabels,
            datasets: [{
                data: unitData,
                backgroundColor: unitColors
            }]
        },
        options: piechartConfig
    });
}

function viewProcesses(unit) {
    //..TODO: implement view processes logic
    console.log("Unit Description >>" + unit);
}

document.addEventListener('DOMContentLoaded', function () {
    generateUnitCards(dashboardRecord)
    renderPiechart(dashboardRecord);

    // Table of processes
    const tableBody = document.querySelector('#processTable tbody');
    tableBody.innerHTML = '';
    Object.entries(dashboardRecord.CategoryProcesses).forEach(([label, value]) => {
        const tr = document.createElement('tr');
        let disabled = value === 0 ? "disabled" : "";
        tr.innerHTML = `<td>${label}</td>
                        <td>${value}</td>
                        <td>
                            <button class="btn btn-category-button ${disabled}" ${disabled} onclick="viewProcesses('${label}')">
                                <span style="display:inline-block; width:15px; height:15px; border-radius:50px; background-color:${cardColors[label].bg};"></span>
                                <span style="display:inline-block; margin-left:10px;"><i class="mdi mdi-eye"></i></span>
                            </button>
                        </td>`;
        tableBody.appendChild(tr);
    });
});

document.querySelectorAll('.action-btn-home').forEach(button => {
    button.addEventListener('click', function () {
        try {
            window.location.href = '/grc/compliance';
        } catch (error) {
            console.error('Navigation failed:', error);
            showToast(error, 'error'); // assuming showToast(message, type)
        }
    });
});

