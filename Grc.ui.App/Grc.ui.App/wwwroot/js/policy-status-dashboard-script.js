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
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving document details...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findDocument(id)
        .then(record => {
            Swal.close();
            if (record) {
                OpenDocumentView(record);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Document details not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Failed to load document details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findDocument(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/policies-retrieve/${id}`,
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

function OpenDocumentView(record) {
    $('#documentName').val(record.documentName || '');
    $('#documentTypeName').val(record.documentTypeName || '');
    $('#departmentName').val(record.departmentName || '');
    $('#responsibilityName').val(record.responsibilityName || '');
    $('#lastReview').val(record.lastReview || '');
    $('#nextReview').val(record.nextReview || '');
    $('#frequencyName').val(record.frequencyName || '');
    $('#documentStatus').val(record.documentStatus || '');
    $('#comments').val(record.comments || '');

    $('#polOverlay').addClass('active');
    $('#polPanel').addClass('active');
}

function closePolPanel() {
    $('#polOverlay').removeClass('active');
    $('#polPanel').removeClass('active');
}

document.addEventListener('DOMContentLoaded', function () {
    generateCards(policyData);
    renderPiechart(policyData);


    $('#recordForm').on('submit', function (e) {
        e.preventDefault();
    })

    const policies = policyData.PolicyData.Policies;
    const cardColorList = Object.values(cardColors);

    new Tabulator("#processTable", {
        data: policies,
        layout: "fitColumns",
        pagination: "local",
        paginationSize: 10,
        paginationSizeSelector: [10, 25, 50],
        movableColumns: false,
        responsiveLayout: "hide",

        columns: [
            {
                title: "DOCUMENT TITLE",
                field: "Title",
                headerFilter: "input"
            },
            {
                title: "REVIEW DATE",
                field: "ReviewDate",
                formatter: cell => {
                    const v = cell.getValue();
                    return v
                        ? new Date(v).toLocaleDateString('en-GB', {
                            day: '2-digit',
                            month: 'short',
                            year: 'numeric'
                        })
                        : '';
                }
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
                                onclick="viewPolicy('${report.Id}')">
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
