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
$('.action-btn-circular-home').on('click', function () {
    console.log("Home Button clicked");
    try {
        window.location.href = '/grc/returns/circular-returns/circulars-dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const tableBody = document.querySelector('#circularTable tbody');
    tableBody.innerHTML = '';
   
    const circulars = circularData.Circulars.Reports;;
    const cardColorList = Object.values(cardColors);
    circulars.forEach((report, index) => {
        const tr = document.createElement('tr');

        const color = cardColorList[index % cardColorList.length].bg;

        tr.innerHTML = `
        <td>${report.Title}</td>
        <td>${report.Status}</td>
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