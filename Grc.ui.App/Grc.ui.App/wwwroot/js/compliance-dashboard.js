const cardColors =  {
    "Total": { bg: "#8B5CF6", class1: "stat-card-light", class2: "stat-separator-primary"},
    "Uptodate": { bg: "#10B981", class: "stat-card-light", class2: "stat-separator-primary"},
    "Not Uptodate": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "Board Review": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "Department Review": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "On Hold": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "Standard": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },

};

function generatePolicyCards(record) {
    if (!record || !record.Policies) return;

    const policyContainer = document.getElementById('policyCards');
    policyContainer.innerHTML = '';

    Object.keys(record.Policies).forEach(policy => {
        const total = record.Policies[policy];
        const colors = cardColors[policy];
        const route = policyRoutes[policy];
       
        if (!colors || !route) {
            console.warn(`Missing config for policy: ${policy}`);
            return;
        }

        const card = document.createElement('div');
        card.className = `stat-card ${colors.class1}`;

        card.innerHTML = `
            <div class="stat-number">
                <span>${total}</span>
                <span>${policy}</span>
            </div>
            <span class="stat-separator ${colors.class2}"></span>
            <a class="stat-link" href="${route}">
                <span><i class="mdi mdi-chevron-right"></i></span>
                <span>See more...</span>
            </a>
        `;

        policyContainer.appendChild(card);
    });
}

const returnColors =  {
    "TOTALS": { bg: "#8B5CF6", class1: "stat-card-light", class2: "stat-separator-primary" },
    "NA": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "ONE-OFF": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "ON OCCURRENCE": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "PERIODIC": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "DAILY": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "WEEKLY": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "MONTHLY": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "QUARTERLY": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "ANNUAL": { bg: "#10B981", class: "stat-card-light", class2: "stat-separator-primary" },
    "BIENNIAL": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "BIANNUAL": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "TRIENNIAL": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },

};

function generateReturnsCards(record) {
    if (!record || !record.Returns) return;

    const returnsContainer = document.getElementById('returnsCards');
    returnsContainer.innerHTML = '';

    Object.keys(record.Returns).forEach(report => {
        const total = record.Returns[report];
        const colors = returnColors[report];
        const route = reportRoutes[report];

        if (!colors || !route) {
            console.warn(`Missing config for report: ${report}`);
            return;
        }

        const card = document.createElement('div');
        card.className = `stat-card ${colors.class1}`;

        card.innerHTML = `
            <div class="stat-number">
                <span>${total}</span>
                <span>${report}</span>
            </div>
            <span class="stat-separator ${colors.class2}"></span>
            <a class="stat-link" href="${route}">
                <span><i class="mdi mdi-chevron-right"></i></span>
                <span>See more...</span>
            </a>
        `;

        returnsContainer.appendChild(card);
    });
}

const circularColors = {
    "TOTALS": { bg: "#8B5CF6", class1: "stat-card-light", class2: "stat-separator-primary" },
    "BOU": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "CMA": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "UMRA": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "IRAU": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "FIA": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "PPDA": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "URBRA": { bg: "#8B5CF6", class: "stat-card-light", class2: "stat-separator-primary" },
    "MoFED": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "URA": { bg: "#10B981", class: "stat-card-light", class2: "stat-separator-primary" },
    "PDPO": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "AG": { bg: "#F59E0B", class: "stat-card-light", class2: "stat-separator-cancelled" },
    "UIB": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },
    "NIRA": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },
    "DPF": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },
    "OTHERS": { bg: "#3B82F6", class: "stat-card-light", class2: "stat-separator-primary" },
};

function generateCircularCards(record) {
    if (!record || !record.Circulars) return;

    console.log(record.Circulars);
    const circularContainer = document.getElementById('circularCards');
    circularContainer.innerHTML = '';

    Object.keys(record.Circulars).forEach(circular => {
        const total = record.Circulars[circular];
        console.log(circular);
        console.log(total);
        const colors = circularColors[circular];
        const route = circularRoutes[circular];

        if (!colors || !route) {
            console.warn(`Missing config for circular: ${circular}`);
            return;
        }

        const card = document.createElement('div');
        card.className = `stat-card ${colors.class1}`;

        card.innerHTML = `
            <div class="stat-number">
                <span>${total}</span>
                <span>${circular}</span>
            </div>
            <span class="stat-separator ${colors.class2}"></span>
            <a class="stat-link" href="${route}">
                <span><i class="mdi mdi-chevron-right"></i></span>
                <span>See more...</span>
            </a>
        `;

        circularContainer.appendChild(card);
    });
}

document.addEventListener('DOMContentLoaded', function () {
    generatePolicyCards(dashboardData);
    generateReturnsCards(dashboardData);
    generateCircularCards(dashboardData) 

});



