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

    const cardsContainer = document.getElementById('circularCards');
    cardsContainer.innerHTML = '';

    //..create cards
    let data = record.Circulars.Statistics;
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

let circularChart = null;
function renderPiechart(record) {
    if (!record) return;

    //..destroy old instance if it exists
    if (circularChart) {
        circularChart.destroy();
    }

    let policies = record.Circulars.Statistics;
    const deptLabels = Object.keys(policies);
    const deptData = Object.values(policies);
    const cardColorList = Object.values(cardColors);
    const deptColors = deptLabels.map((_, i) =>
        cardColorList[i % cardColorList.length].bg
    );

    circularChart = new Chart(document.getElementById("unitPieChart"), {
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
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving circular details...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findCircular(id)
        .then(record => {
            Swal.close();
            if (record) {
                OpenCircularView(record);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Circular details not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Failed to load circular details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findCircular(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/circulars/retrieve-circular/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                } else {
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
            }
        });
    });
}

function OpenCircularView(record) {
    $('#reference').val(record.reference || '');
    $('#circularTitle').val(record.circularTitle || '');
    $('#circularRequirement').val(record.circularRequirement || '');
    $('#owner').val(record.owner || '');
    $('#authority').val(record.authority || '');
    $('#received').val(record.received || '');
    $('#lastDate').val(record.lastDate || '');
    $('#status').val(record.status || '');
    $('#issueCount').val(record.issueCount || '0');
    $('#breachRisk').val(record.breachRisk || '');
    $('#comments').val(record.comments || '');

    $('#circOverlay').addClass('active');
    $('#circPanel').addClass('active');
}

function closeCircPanel() {
    $('#circOverlay').removeClass('active');
    $('#circPanel').removeClass('active');
}

document.addEventListener('DOMContentLoaded', function () {
    generateCards(circularData);
    renderPiechart(circularData);

    // Table of processes
    const returns = circularData.Circulars.Reports;
    const cardColorList = Object.values(cardColors);

    new Tabulator("#processTable", {
        data: returns,
        layout: "fitColumns",
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [10, 25, 50],
        movableColumns: false,
        responsiveLayout: "hide",

        columns: [
            {
                title: "CIRCULAR TITLE",
                field: "Title",
                headerFilter: "input"
            },
            {
                title: "CIRCULAR STATUS",
                field: "Status",
                headerFilter: "input"
            },
            {
                title: "DEPARTMENT",
                field: "Department",
                headerFilter: "input"
            },
            {
                title: "VIEW",
                hozAlign: "center",
                formatter: function (cell) {
                    const report = cell.getRow().getData();
                    const index = cell.getRow().getPosition();
                    const color = cardColorList[index % cardColorList.length].bg;

                    return `
                        <button class="btn btn-category-button"
                                onclick="viewReport('${report.Id}')">
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
                    `;
                }
            }
        ]
    });

});