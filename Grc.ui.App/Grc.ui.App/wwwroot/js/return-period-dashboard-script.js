const cardColors = {
    "Color1": { bg: "#5C0947", class: "" },
    "Color2": { bg: "#8C0E6C", class: "" },
    "Color3": { bg: "#A61180", class: "" },
    "Color4": { bg: "#C21495", class: "" },
    "Color5": { bg: "#E017AD", class: "" },
    "Color6": { bg: "#F519BD", class: "" },
    "Color7": { bg: "#FFAFF2", class: "" },
};

function generateCards(record) {
    console.log(record);
    if (!record) {
        return;
    }

    const cardsContainer = document.getElementById('returnsCards');
    cardsContainer.innerHTML = '';

    //..create cards
    let data = record.Returns.Statistics;
    Object.keys(data).forEach(report => {
        const total = data[report];
        const card = document.createElement('div');
        card.className = `stats-category-card  ${cardClass}`;
        card.innerHTML = `
            <div class="stats-card-value">${total}</div>
            <div class="stats-card-title">${report}</div>
        `;
        cardsContainer.appendChild(card);
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

let returnsChart = null;
function renderPiechart(record) {
    if (!record) return;

    //..destroy old instance if it exists
    if (returnsChart) {
        returnsChart.destroy();
    }

    let policies = record.Returns.Statistics;
    const deptLabels = Object.keys(policies);
    const deptData = Object.values(policies);
    const cardColorList = Object.values(cardColors);
    const deptColors = deptLabels.map((_, i) =>
        cardColorList[i % cardColorList.length].bg
    );

    returnsChart = new Chart(document.getElementById("unitPieChart"), {
        type: 'pie',
        data: {
            labels: deptLabels,
            datasets: [{
                data: deptData,
                backgroundColor: deptColors
            }]
        },
        options: piechartConfig
    });
}

function viewReport(id) {
    alert(`Report ID >> ${id}`)
}

document.addEventListener('DOMContentLoaded', function () {
    generateCards(returnData);
    renderPiechart(returnData);

    // Table of processes
    const tableBody = document.querySelector('#processTable tbody');
    tableBody.innerHTML = '';

    const returns = returnData.Returns.Reports;
    const cardColorList = Object.values(cardColors);
    returns.forEach((report, index) => {
        const tr = document.createElement('tr');

        const color = cardColorList[index % cardColorList.length].bg;

        tr.innerHTML = `
        <td>${report.Title}</td>
        <td>${report.Department}</td>
        <td>
            <button class="btn btn-category-button" onclick="viewReport('${report.Id}')">
                <span style="
                    display:inline-block;
                    width:15px;
                    height:15px;
                    border-radius:50%;
                    background-color:${color};">
                </span>
                <span style="margin-left:10px;">
                    <i class="mdi mdi-eye"></i>
                </span>
            </button>
        </td>
    `;

        tableBody.appendChild(tr);
    });
});