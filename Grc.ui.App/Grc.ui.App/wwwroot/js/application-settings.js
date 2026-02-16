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
        const activeTab = getActiveTab();
        if (!activeTab) return;

        const saveBar = activeTab.querySelector(".settings-header-actions");
        if (!saveBar) return;

        saveBar.classList.add("show");
    });
});


//..save button handler
document.addEventListener("click", function (e) {
    if (!e.target.classList.contains("save-settings-btn")) return;

    const tabId = getActiveTab().id;

    if (tabId === "useraccount") saveUserAccountSettings();
    if (tabId === "security") savePasswordSettings();
});

function buildUserAccountSettings() {
    return {
        canVerifySame: $('#canVerifySame').is(':checked'), 
        canApproveSame: $('#canApproveSame').is(':checked'),
    };
}

function saveUserAccountSettings() {
    saveSettings("/admin/support/configuration/user-accounts", buildUserAccountSettings());
}

function buildPasswordSettings() {
    return {
        expirePassword: $('#expirePassword').is(':checked'), 
        exipryDays: Number($('#exipryDays').val()) || 0,
        minimumLength: Number($('#passwordLength').val()) || 0,
        allowMaualReset: $('#allowManualReset').is(':checked'),
        allowPwsReuse: $('#allowPasswordReuse').is(':checked'),
        includeUpper: $('#includeUpper').is(':checked'),
        includeLower: $('#includeLower').is(':checked'),
        includeSpecial: $('#includeSpecial').is(':checked'),
        includeNumerics: $('#includeNumerics').is(':checked'),
    };
}

function savePasswordSettings() {
    saveSettings("/admin/support/configuration/pwd-policy", buildPasswordSettings());
}

function saveSettings(url, record) {
    console.log(record);
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