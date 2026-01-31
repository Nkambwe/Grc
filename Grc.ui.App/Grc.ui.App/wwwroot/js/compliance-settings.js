document.querySelectorAll(".tab").forEach(tab => {
    tab.addEventListener("click", () => {
        document.querySelectorAll(".tab").forEach(t => t.classList.remove("active"));
        document.querySelectorAll(".tab-content").forEach(c => c.classList.remove("active"));

        tab.classList.add("active");
        document.getElementById(tab.dataset.tab).classList.add("active");
    });
});

function getActiveTab() {
    return document.querySelector(".tab-content.active");
}

// DIRTY STATE
document.querySelectorAll(".setting-input").forEach(input => {
    input.addEventListener("change", () => {
        const saveBar = getActiveTab().querySelector(".settings-header-actions");
        saveBar.classList.add("show");
    });
});

// SAVE BUTTON HANDLER
document.addEventListener("click", function (e) {
    if (!e.target.classList.contains("save-settings-btn")) return;

    const tabId = getActiveTab().id;

    if (tabId === "general") saveGeneralSettings();
    if (tabId === "policies") savePolicySettings();
});

// BUILD RECORDS
function buildGeneralSettings() {
    return {
        softDeleteRecord: document.getElementById("softDelete").checked,
        includeDeletedRecords: document.getElementById("includeDeleted").checked
    };
}

function buildPolicySettings() {
    return {
        sendNotifications: document.getElementById("policySendNotifications").checked,
        maximumNotifications: parseInt(
            document.getElementById("policyMaxNotifications").value || 0
        )
    };
}

// SAVE FUNCTIONS (AJAX-ready)
function saveGeneralSettings() {
    saveSettings("/grc/compliance/settings/save-general", buildGeneralSettings());
}

function savePolicySettings() {
    saveSettings("/grc/compliance/settings/save-policies", buildPolicySettings());
}

function saveSettings(url, record) {

    Swal.fire({
        title: "Saving settings...",
        allowOutsideClick: false,
        didOpen: () => Swal.showLoading()
    });

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getCircularAntiForgeryToken()
        },
        success: function (res) {
            Swal.close();

            if (!res.success) {
                Swal.fire("Save failed", res.message || "Invalid data");
                return;
            }

            Swal.fire("Settings saved successfully");

            getActiveTab()
                .querySelector(".settings-header-actions")
                .classList.remove("show");
        },
        error: function () {
            Swal.close();
            Swal.fire("Save failed", "Unexpected error occurred");
        }
    });
}