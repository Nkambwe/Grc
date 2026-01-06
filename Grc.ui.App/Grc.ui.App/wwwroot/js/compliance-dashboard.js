const cardColors = {
    "Total": { bg: "#8B5CF6", class1: "stat-card-light", class2: "stat-separator-danger"},
    "Uptodate": { bg: "#10B981", class: "stat-card-light", class2: "stat-separator-primary"},
    "Not Uptodate": { bg: "#9E1D08", class: "stat-card-light", class2: "stat-separator-danger" },
    "Board Review": { bg: "#7CF0AC", class: "stat-card-light", class2: "stat-separator-primary" },
    "Department Review": { bg: "#7CF0AC", class: "stat-card-light", class2: "stat-separator-danger" },
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


document.addEventListener('DOMContentLoaded', function () {
    //let policyCards = dashboardData.Policies;
    //let circularCards = dashboardData.Circulars;
    //console.log(circularCards);
    //let returnCards = dashboardData.Returns;
    //console.log(returnCards);
    //let taskCards = dashboardData.Tasks;
    //console.log(taskCards);
    generatePolicyCards(dashboardData)

});



