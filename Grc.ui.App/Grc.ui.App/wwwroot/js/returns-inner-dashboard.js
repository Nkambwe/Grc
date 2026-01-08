const cardColors = {
    "Color1": { bg: "#5C0947", class: "" },
    "Color2": { bg: "#8C0E6C", class: "" },
    "Color3": { bg: "#A61180", class: "" },
    "Color4": { bg: "#C21495", class: "" },
    "Color5": { bg: "#E017AD", class: "" },
    "Color6": { bg: "#F519BD", class: "" },
    "Color7": { bg: "#FFAFF2", class: "" },
};

//..route to home
$('.action-btn-returns-home').on('click', function () {
    try {
        window.location.href = '/grc/returns/compliance-returns/returns-dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.querySelector('#returnsTable tbody');
    tableBody.innerHTML = '';
    const returns = retData.Returns.Reports;
    const cardColorList = Object.values(cardColors);
    returns.forEach((report, index) => {
        const tr = document.createElement('tr');

        const color = cardColorList[index % cardColorList.length].bg;

        tr.innerHTML = `
        <td>${report.Title}</td>
        <td>${report.Status}</td>
        <td>${report.Risk}</td>
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