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

//..dirty state
document.querySelectorAll(".setting-input").forEach(input => {
    input.addEventListener("change", () => {
        const saveBar = getActiveTab().querySelector(".settings-header-actions");
        saveBar.classList.add("show");
    });
});

//..save button handler
document.addEventListener("click", function (e) {
    if (!e.target.classList.contains("save-settings-btn")) return;

    const tabId = getActiveTab().id;

    if (tabId === "general") saveGeneralSettings();
    if (tabId === "policies") savePolicySettings();
});

function buildGeneralSettings() {
    return {
        softDeleteRecords: $('#softDelete').is(':checked'), 
        includeDeletedRecord: $('#includeDeleted').is(':checked'),
    };
}

function buildPolicySettings() {
    return {
        sendPolicyNotifications: $('#policySendNotifications').is(':checked'), 
        maximumNumberOfNotifications: Number($('#policyMaxNotifications').val()) || 0 
    };
}

function saveGeneralSettings() {
    saveSettings("/grc/compliance/configurations/general-config", buildGeneralSettings());
}

function savePolicySettings() {
    saveSettings("/grc/compliance/configurations/policy-config", buildPolicySettings());
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
            'X-CSRF-TOKEN': getConfigAntiForgeryToken()
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

    function getConfigAntiForgeryToken() {
        return $('meta[name="csrf-token"]').attr('content');
    }

}