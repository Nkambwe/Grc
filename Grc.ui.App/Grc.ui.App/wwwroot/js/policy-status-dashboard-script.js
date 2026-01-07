const cardColors =  {
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

    const cardsContainer = document.getElementById('policyCards');
    cardsContainer.innerHTML = '';

    //..create cards
    let data = record.PolicyData.Statistics;
    Object.keys(data).forEach(policy => {
        const total = data[policy];
        const card = document.createElement('div');
        card.className = `stats-category-card  ${cardClass}`;
        card.innerHTML = `
            <div class="stats-card-value">${total}</div>
            <div class="stats-card-title">${policy}</div>
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

let policyChart = null;

function renderPiechart(record) {
    if (!record) return;

    //..destroy old instance if it exists
    if (policyChart) {
        policyChart.destroy();
    }

    let policies = record.PolicyData.Statistics;
    const deptLabels = Object.keys(policies);
    const deptData = Object.values(policies);
    const cardColorList = Object.values(cardColors);
    const deptColors = deptLabels.map((_, i) =>
        cardColorList[i % cardColorList.length].bg
    );

    policyChart = new Chart(document.getElementById("unitPieChart"), {
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

function viewPolicy(id) {
    alert(`Policy ID >> ${id}`)
}

document.addEventListener('DOMContentLoaded', function () {
    generateCards(policyData);
    renderPiechart(policyData);

    // Table of processes
    const tableBody = document.querySelector('#processTable tbody');
    tableBody.innerHTML = '';

    const policies = policyData.PolicyData.Policies;
    const cardColorList = Object.values(cardColors);
    policies.forEach((policy, index) => {
        const tr = document.createElement('tr');

        const color = cardColorList[index % cardColorList.length].bg;

        tr.innerHTML = `
        <td>${policy.Title}</td>
        <td>${policy.Department ?? 'Unassigned'}</td>
        <td>${new Date(policy.ReviewDate).toLocaleDateString()}</td>
        <td>
            <button class="btn btn-category-button" onclick="viewPolicy('${policy.id}')">
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
