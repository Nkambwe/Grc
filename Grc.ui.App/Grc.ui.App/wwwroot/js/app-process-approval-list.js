let processApprovalsTable;
function initProcessApprovalListTable() {
    processApprovalsTable = new Tabulator("#processApprovalsTable", {
        ajaxURL: "/operations/workflow/processes/approvals-all",
        paginationMode: "remote",
        filterMode: "remote",
        sortMode: "remote",
        pagination: true,
        paginationSize: 10,
        paginationSizeSelector: [10, 20, 35, 40, 50],
        paginationCounter: "rows",
        ajaxConfig: {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        },
        ajaxContentType: "json",
        paginationDataSent: {
            "page": "page",
            "size": "size",
            "sorters": "sort",
            "filters": "filter"
        },
        paginationDataReceived: {
            "last_page": "last_page",
            "data": "data",
            "total_records": "total_records"
        },
        ajaxRequestFunc: function (url, config, params) {
            return new Promise((resolve, reject) => {
                let requestBody = {
                    pageIndex: params.page || 1,
                    pageSize: params.size || 10,
                    searchTerm: "",
                    sortBy: "",
                    sortDirection: "Ascending"
                };

                // Sorting
                if (params.sort && params.sort.length > 0) {
                    requestBody.sortBy = params.sort[0].field;
                    requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
                }

                // Filtering
                if (params.filter && params.filter.length > 0) {
                    let filter = params.filter.find(f =>
                        ["processName", "hodStatus", "complianceStatus", "BopStatus", "creditStatus", "treasuryStatus", "fintechStatus"].includes(f.field)
                    );
                    if (filter) requestBody.searchTerm = filter.value;
                }

                $.ajax({
                    url: url,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(requestBody),
                    success: function (response) {
                        resolve(response);
                    },
                    error: function (xhr, status, error) {
                        console.error("AJAX Error:", error);
                        reject(error);
                    }
                });
            });
        },
        ajaxResponse: function (url, params, response) {
            return {
                data: response.data || [],
                last_page: response.last_page || 1,
                total_records: response.total_records || 0
            };
        },
        ajaxError: function (error) {
            console.error("Tabulator AJAX Error:", error);
            alert("Failed to load tasks. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "PROCESS NAME",
                field: "processName",
                minWidth: 400,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewApproval(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            {
                title: "HOD STATUS",
                field: "hodStatus",
                formatter: function (cell) {
                    const value = cell.getValue(); 
                    const cellEl = cell.getElement();

                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";     
                    cellEl.style.fontWeight = "bold";   
                    cellEl.style.textAlign = "center";  

                    return value; 
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "RISK STATUS",
                field: "riskStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();

                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "COMPLIANCE",
                field: "complianceStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "NEED BOP",
                formatter: function (cell) {
                    const needBop = cell.getRow().getData().requiresBopApproval;
                    let value = needBop ? "YES" : "NO";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${value}</span>
                            </div>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "BOP STATUS",
                field: "bopStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    } 
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "NEED CREDIT",
                formatter: function (cell) {
                    const needCredit = cell.getRow().getData().requiresCreditApproval;
                    let value = needCredit ? "YES" : "NO";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${value}</span>
                            </div>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "CREDIT",
                field: "creditStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "NEED TREASURY",
                formatter: function (cell) {
                    const needTreasury = cell.getRow().getData().requiresTreasuryApproval;
                    let value = needTreasury ? "YES" : "NO";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${value}</span>
                            </div>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "TREASURY",
                field: "treasuryStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "NEED FINTECH",
                formatter: function (cell) {
                    const needFintech = cell.getRow().getData().requiresFintechApproval;
                    let value = needFintech ? "YES" : "NO";
                    return `<div style="
                                display:flex;
                                align-items:center;
                                justify-content:center;
                                font-weight:bold;">
                                <span>${value}</span>
                            </div>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "FINTECH",
                field: "fintechStatus",
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
                    // Default color
                    let bg = "#1FBBF2";

                    if (value === "APPROVED") {
                        bg = "#20C733";
                    }
                    else if (value === "ONHOLD") {
                        bg = "#D1C25D";
                    }
                    else if (value === "PENDING") {
                        bg = "#FF1E27";
                    }
                    else if (!value) {
                        bg = "#1FBBF2";
                    }
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = "#FFFFFF";
                    cellEl.style.fontWeight = "bold";
                    cellEl.style.textAlign = "center";

                    return value;
                },
                widthGrow: 1,
                hozAlign: "center",
                headerHozAlign: "center",
                minWidth: 150,
                headerSort: false
            },
            {
                title: "HOLD PROCESS",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="viewHold(${rowData.id})">
                        <span><i class="mdi mdi-cog-pause-outline" aria-hidden="true"></i></span>
                        <span>ON HOLD</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "APPROVE PROCESS",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="approvalProcess(${rowData.id})">
                        <span><i class="mdi mdi-cog-transfer-outline" aria-hidden="true"></i></span>
                        <span>APPROVE</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessApprovalsSearch();
}

function initProcessApprovalsSearch() {
    const searchInput = $('#approvalsSearchbox');
    let typingTimer;
    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processNewTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "hodStatus", type: "like", value: searchTerm },
                        { field: "complianceStatus", type: "like", value: searchTerm },
                        { field: "BopStatus", type: "like", value: searchTerm },
                        { field: "creditStatus", type: "like", value: searchTerm },
                        { field: "treasuryStatus", type: "like", value: searchTerm },
                        { field: "fintechStatus", type: "like", value: searchTerm }
                    ]
                ]);
                processReviewTable.setPage(1, true);
            } else {
                processReviewTable.clearFilter();
            }
        }, 300);
    });
}

function findApplyRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/approvals-retrieve/${encodeURIComponent(id)}`,
            type: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            success: function (res) {
                if (res && res.success) {
                    resolve(res.data);
                } else {
                    resolve(null);
                }
            },
            error: function () {
                reject();
            }
        });
    });
}

function approvalProcess(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving approval record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findApplyRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openApprovalEditor('Approve Process', record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Approval record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load approval details.' });
        });
}

function viewApproval(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving process record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findApplyRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openApprovalEditor('Approve Process', record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Approval record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load process details.' });
        });
}

function openApprovalEditor(title, approval) {
    var bopRequired = approval?.requiresBopApproval || false;
    $("#bopRequired").val(bopRequired);
    var creditRequired = approval?.requiresCreditApproval || false;
    $("#creditRequired").val(creditRequired);
    var treasuryRequired = approval?.requiresTreasuryApproval || false;
    $("#treasuryRequired").val(treasuryRequired);
    var fintechRequired = approval?.requiresFintechApproval || false;
    $("#fintechRequired").val(fintechRequired);

    var tStr = approval?.processName || "";
    var hodStatus = approval?.hodStatus || "";
    var riskStatus = approval?.riskStatus || "";
    var compStatus = approval?.complianceStatus || "";
    var bopStatus = approval?.bopStatus || "";
    var creditStatus = approval?.creditStatus || "";
    var treasuryStatus = approval?.treasuryStatus || "";
    var fintechStatus = approval?.fintechStatus || "";

    if (tStr)
        title = tStr;

    console.log("Process Name >> " + tStr);

    const date = new Date(approval?.requestDate);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();
    const formattedDate = `${day}-${month}-${year}`;

    console.log("Request Date >> " + formattedDate);
    // Populate form fields
    $("#approvalId").val(approval?.id || 0);
    $("#processId").val(approval?.processId || 0);
    $("#processName").val(tStr);
    $("#processDescription").val(approval?.processDescription || "");
    $('#isDeleted').prop('checked', approval?.isDeleted || false);
    $("#requestDate").val(formattedDate);
    $("#hodStatus").val(hodStatus).trigger('change.select2');
    $("#hodComment").val(approval?.hodComment || "");
    $("#hodEnd").val(approval?.hodEnd || "");
    $("#riskStatus").val(riskStatus).trigger('change.select2');
    $("#riskComment").val(approval?.riskComment || "");
    $("#riskEnd").val(approval?.riskEnd || "");
    $("#complianceStatus").val(compStatus).trigger('change.select2');
    $("#complianceComment").val(approval?.complianceComment || "");
    $("#complianceEnd").val(approval?.complianceEnd || "");
    $('#requiresBopApproval').prop('checked', bopRequired );
    $("#bopStatus").val(bopStatus).trigger('change.select2');
    $("#bopComment").val(approval?.bopComment || "");
    $("#bopStatusEnd").val(approval?.bopStatusEnd || "");
    $('#requiresCreditApproval').prop('checked', creditRequired);
    $("#creditStatus").val(creditStatus).trigger('change.select2');
    $("#creditComment").val(approval?.creditComment || "");
    $("#creditEnd").val(approval?.creditEnd || "");
    $('#requiresTreasuryApproval').prop('checked', treasuryRequired);
    $("#treasuryStatus").val(treasuryStatus).trigger('change.select2');
    $("#treasuryComment").val(approval?.treasuryComment || "");
    $("#treasuryEnd").val(approval?.treasuryEnd || "");
    $('#requiresFintechApproval').prop('checked', fintechRequired);
    $("#fintechStatus").val(fintechStatus).trigger('change.select2');
    $("#fintechComment").val(approval?.fintechComment || "");
    $("#fintechEnd").val(approval?.fintechEnd || "");

    updateSectionExpansion(bopRequired, creditRequired, treasuryRequired, fintechRequired, hodStatus,
        riskStatus, compStatus, bopStatus, creditStatus, treasuryStatus, fintechStatus);
    $('#approvalPanelTitle').text(title);
    $('.process-overlay').addClass('active');
    $('#collapsePanel').addClass('active');
}

function applyApproval(e) {
    e.preventDefault();

    // Helper function to parse boolean from string
    function parseBool(value) {
        if (typeof value === 'boolean') return value;
        if (typeof value === 'string') {
            return value.toLowerCase() === 'true';
        }
        return false;
    }

    let recordData = {
        id: parseInt($('#approvalId').val()) || 0,
        processId: parseInt($('#processId').val()) || 0,
        processName: $('#processName').val()?.trim() || "",
        hodStatus: $('#hodStatus').val()?.trim() || "",
        hodComment: $('#hodComment').val()?.trim() || "",
        riskStatus: $('#riskStatus').val()?.trim() || "",
        riskComment: $('#riskComment').val()?.trim() || "",
        complianceStatus: $('#complianceStatus').val()?.trim() || "",
        complianceComment: $('#complianceComment').val()?.trim() || "",
        bopRequired: parseBool($("#bopRequired").val()),
        bopStatus: $('#bopStatus').val()?.trim() || "",
        bopComment: $('#bopComment').val()?.trim() || "",
        creditRequired: parseBool($("#creditRequired").val()),
        creditStatus: $('#creditStatus').val()?.trim() || "",
        creditComment: $('#creditComment').val()?.trim() || "",
        treasuryRequired: parseBool($("#treasuryRequired").val()),
        treasuryStatus: $('#treasuryStatus').val()?.trim() || "",
        treasuryComment: $('#treasuryComment').val()?.trim() || "",
        fintechRequired: parseBool($("#fintechRequired").val()),
        fintechStatus: $('#fintechStatus').val()?.trim() || "",
        fintechComment: $('#fintechComment').val()?.trim() || "",
    };

    // Debug - check the data structure
    console.log("Record data before validation:", recordData);

    if (!validateApprovalData(recordData)) {
        //..stop save process
        return;
    }
    //..save approval
    saveApproval(recordData);
}

function saveApproval(data) {
    const url = "/operations/workflow/processes/approval-update";

    Swal.fire({
        title: "Update approval status...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    //..debugging
    console.log("Sending data to server:", JSON.stringify(data));
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getApprovalAntiForgeryToken()
        },
        success: function (res) {
            //..lose loader and show success message
            Swal.close();
            if (!res.success) {
                //..error from the server
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            if (processApprovalsTable) {
                processApprovalsTable.replaceData();
            }

            closeApprovalPanel();
        },
        error: function (xhr) {
            Swal.close();

            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }

            Swal.fire({
                title: "Update Failed",
                text: errorMessage
            });
        }
    });
}

function validateApprovalData(recordData) {
    let errors = [];

    //..check if a field needs validation
    function needsAttention(status) {
        return status === "" || status === "undefined" || status === "UNDEFINED" || status === "PENDING";
    }

    //..validate a section
    function validateSection(sectionName, status, comment, fieldId) {
        // If status is set to APPROVED or REJECTED, comment is required
        if ((status === "APPROVED" || status === "REJECTED") && (!comment || comment.trim() === "")) {
            errors.push(`${sectionName} comment is required when ${status.toLowerCase()}.`);
            highlightApprovalField(fieldId, true);
            return false;
        }
        return true;
    }

    //..clear all highlights first
    highlightApprovalField("#hodComment", false);
    highlightApprovalField("#riskComment", false);
    highlightApprovalField("#complianceComment", false);
    highlightApprovalField("#bopComment", false);
    highlightApprovalField("#creditComment", false);
    highlightApprovalField("#treasuryComment", false);
    highlightApprovalField("#fintechComment", false);

    //..always validate HOD if it's being processed
    console.log("HOD Status   >>> " + recordData.hodStatus);
    console.log("HOD Comments >>> " + recordData.hodComment);
    if (needsAttention(recordData.hodStatus)) {
        //...HOD hasn't been processed yet - no validation needed
    } else {
        validateSection("HOD", recordData.hodStatus, recordData.hodComment, "#hodComment");
    }

    // Validate Risk only if HOD is approved
    console.log("Risk Status   >>> " + recordData.riskStatus);
    console.log("Risk Comments >>> " + recordData.riskComment);
    if (recordData.hodStatus === "APPROVED") {
        if (!needsAttention(recordData.riskStatus)) {
            validateSection("Risk", recordData.riskStatus, recordData.riskComment, "#riskComment");
        }
    }

    // Validate Compliance only if Risk is approved
    if (recordData.hodStatus === "APPROVED" && recordData.riskStatus === "APPROVED") {
        if (!needsAttention(recordData.complianceStatus)) {
            validateSection("Compliance", recordData.complianceStatus, recordData.complianceComment, "#complianceComment");
        }
    }

    // Validate BOP only if required and Compliance is approved
    if (recordData.bopRequired &&
        recordData.hodStatus === "APPROVED" &&
        recordData.riskStatus === "APPROVED" &&
        recordData.complianceStatus === "APPROVED") {
        if (!needsAttention(recordData.bopStatus)) {
            validateSection("Branch Operations", recordData.bopStatus, recordData.bopComment, "#bopComment");
        }
    }

    // Check if BOP is complete or not required
    var bopComplete = !recordData.bopRequired || recordData.bopStatus === "APPROVED";

    // Validate Treasury only if required and all previous are approved
    if (recordData.treasuryRequired &&
        recordData.complianceStatus === "APPROVED" &&
        bopComplete) {
        if (!needsAttention(recordData.treasuryStatus)) {
            validateSection("Treasury", recordData.treasuryStatus, recordData.treasuryComment, "#treasuryComment");
        }
    }

    // Validate Credit only if required and all previous are approved
    var treasuryComplete = !recordData.treasuryRequired || recordData.treasuryStatus === "APPROVED";
    if (creditRequired &&
        recordData.complianceStatus === "APPROVED" &&
        bopComplete &&
        treasuryComplete) {
        if (!needsAttention(recordData.creditStatus)) {
            validateSection("Credit", recordData.creditStatus, recordData.creditComment, "#creditComment");
        }
    }

    // Validate Fintech only if required and all previous are approved
    var creditComplete = !recordData.creditRequired || recordData.creditStatus === "APPROVED";
    if (recordData.fintechRequired &&
        recordData.complianceStatus === "APPROVED" &&
        bopComplete &&
        treasuryComplete &&
        creditComplete) {
        if (!needsAttention(recordData.fintechStatus)) {
            validateSection("Fintech", recordData.fintechStatus, recordData.fintechComment, "#fintechComment");
        }
    }

    //..show errors if any
    if (errors.length > 0) {
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
            icon: "warning"
        });
        return false;
    }

    return true;
}

function getApprovalAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

}

function closeApprovalPanel() {
    $('.process-overlay').removeClass('active');
    $('#collapsePanel').removeClass('active');
}

function viewHold(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving process record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findApplyRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openHoldEditor('Hold Process', record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Hold record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load process details.' });
        });
}

function openHoldEditor(title, approval) {

    console.log("Approval ID >> " + approval?.id);
    console.log("Process ID >> " + approval?.processId);
    $("#holdId").val(approval?.id || 0);
    $("#holdProcessId").val(approval?.processId || 0);
    $("#holdName").val(approval.processName);
    $("#holdDescription").val(approval?.processDescription || "");
    
    $('#holdPanelTitle').text(title);
    $('.hold-overlay').addClass('active');
    $('#holdePanel').addClass('active');
}

function holdProcessReview(e) {
    e.preventDefault();

    let recordData = {
        id: parseInt($('#holdId').val()) || 0,
        processId: parseInt($('#holdProcessId').val()) || 0,
        processName: $('#holdName').val()?.trim() || "",
        processStatus: "OnHold",
        holdReason: $('#holdReason').val()?.trim() || ""
    };

    //..check the data structure
    console.log("Record data before validation:", recordData);

    let errors = [];
    if (!recordData.holdReason)
        errors.push("Reason for holding thie process is required.");

    if (errors.length > 0) {
        highlightApprovalField("#holdReason", !recordData.holdReason);
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..save process hold
    saveProcessHold(recordData);
}

function saveProcessHold(data) {
    const url = "/operations/workflow/processes/approval-hold";

    Swal.fire({
        title: "Hold process approvals...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    //..debugging
    console.log("Sending data to server:", JSON.stringify(data));
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getApprovalAntiForgeryToken()
        },
        success: function (res) {
            //..lose loader and show success message
            Swal.close();
            if (!res.success) {
                //..error from the server
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            if (processApprovalsTable) {
                processApprovalsTable.replaceData();
            }

            closeHoldPanel();
        },
        error: function (xhr) {
            Swal.close();

            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }

            Swal.fire({
                title: "Hold Process Failed",
                text: errorMessage
            });
        }
    });
}

function closeHoldPanel() {
    $('.hold-overlay').removeClass('active');
    $('#holdePanel').removeClass('active');
}

function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');
    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}

function updateSectionExpansion(bopRequired, creditRequired, treasuryRequired, fintechRequired, hodStatus,
    riskStatus, compStatus, bopStatus, creditStatus, treasuryStatus, fintechStatus) {

    
    //..check if status needs attention
    function needsAttention(status) {
        return status === "" || status === "REJECTED" || status === "PENDING";
    }

    //..toggle section
    function setSectionExpanded(sectionId, shouldExpand) {
        $('#' + sectionId + ' .section-content').toggleClass('expanded', shouldExpand);
        $('#' + sectionId + ' .section-toggle').toggleClass('expanded', shouldExpand);
    }

    //...expand HOD Section if needs attention
    setSectionExpanded('hodSection', needsAttention(hodStatus));
    if (hodStatus === "APPROVED") {
        $("#hodStatus").prop("disabled", true);
        $("#hodComment").prop("disabled", true);
    } else {
        $("#hodStatus").prop("disabled", false);
        $("#hodComment").prop("disabled", false);
    }

    //...expand Risk Section if HOD approved but risk needs attention
    setSectionExpanded('riskSection', hodStatus === "APPROVED" && needsAttention(riskStatus));
    if (hodStatus !== "APPROVED" || (hodStatus === "APPROVED" && riskStatus === "APPROVED")) {
        $("#riskStatus").prop("disabled", true);
        $("#riskComment").prop("disabled", true);
    } else {
        $("#riskStatus").prop("disabled", false);
        $("#riskComment").prop("disabled", false);
    }

    //...expand  Compliance Section if risk approved but compliance needs attention
    setSectionExpanded('complianceSection', riskStatus === "APPROVED" && needsAttention(compStatus));
    if (riskStatus !== "APPROVED" || (riskStatus === "APPROVED" && compStatus === "APPROVED")) {
        $("#complianceStatus").prop("disabled", true);
        $("#complianceComment").prop("disabled", true);
    } else {
        $("#complianceStatus").prop("disabled", false);
        $("#complianceComment").prop("disabled", false);
    }

    //...expand  BOP Section if compliance approved, BOP required, and needs attention
    if (bopRequired) {
        setSectionExpanded('bopSection', compStatus === "APPROVED" && needsAttention(bopStatus));
        if (!compStatus) {
            $("#bopStatus").prop("disabled", true);
            $("#bopComment").prop("disabled", true);
        } else {
            $("#bopStatus").prop("disabled", false);
            $("#bopComment").prop("disabled", false);
        }
    } else {
        $("#bopSection").hide();
    }
   
    //..determine which section should expand next after BOP
    var bopApprovedOrNotRequired = !bopRequired || bopStatus === "APPROVED";
    var complianceComplete = compStatus === "APPROVED";

    //..treasury Section
    if (treasuryRequired) {
        setSectionExpanded('treasurySection', complianceComplete && bopApprovedOrNotRequired && needsAttention(treasuryStatus));
        if (!complianceComplete || !bopApprovedOrNotRequired) {
            $("#treasuryStatus").prop("disabled", true);
            $("#treasuryComment").prop("disabled", true);
        } else {
            $("#treasuryStatus").prop("disabled", false);
            $("#treasuryComment").prop("disabled", false);
        }
    } else {
        $("#treasurySection").hide();
    }
    

    // Credit Section
    var treasuryApprovedOrNotRequired = !treasuryRequired || treasuryStatus === "APPROVED";
    if (creditRequired) {
        setSectionExpanded('creditSection', complianceComplete && bopApprovedOrNotRequired && treasuryApprovedOrNotRequired && needsAttention(creditStatus));
        if (!complianceComplete || !bopApprovedOrNotRequired || !treasuryApprovedOrNotRequired) {
            $("#creditStatus").prop("disabled", true);
            $("#creditComment").prop("disabled", true);
        } else {
            $("#creditStatus").prop("disabled", false);
            $("#creditComment").prop("disabled", false);
        }
    } else {
        $("#creditSection").hide();
    }
    

    // Fintech Section
    var creditApprovedOrNotRequired = !creditRequired || creditStatus === "APPROVED";
    if (fintechRequired) {
        setSectionExpanded('fintechSection', complianceComplete && bopApprovedOrNotRequired && treasuryApprovedOrNotRequired && creditApprovedOrNotRequired && needsAttention(fintechStatus));
        if (!complianceComplete || !bopApprovedOrNotRequired || !treasuryApprovedOrNotRequired || !creditApprovedOrNotRequired) {
            $("#fintechStatus").prop("disabled", true);
            $("#fintechComment").prop("disabled", true);
        } else {
            $("#fintechStatus").prop("disabled", false);
            $("#fintechComment").prop("disabled", false);
        }
    } else {
        $("#fintechSection").hide();
    }
   
}

function highlightApprovalField(selector, hasError, message) {
    const $field = $(selector);
    const $formGroup = $field.closest('.form-group, .mb-3, .col-sm-8');

    // Remove existing error
    $field.removeClass('is-invalid');
    $formGroup.find('.field-error').remove();

    if (hasError) {
        $field.addClass('is-invalid');
        if (message) {
            $formGroup.append(`<div class="field-error text-danger small mt-1">${message}</div>`);
        }
    }
}

$(document).ready(function () {

    initProcessApprovalListTable();

    $('#hodStatus, #riskStatus, #complianceStatus, #bopStatus, #creditStatus, #treasuryStatus, #fintechStatus').select2({
        width: '100%',
        dropdownParent: $('#collapsePanel')
    });

    $('#approvalForm').on('submit', function (e) {
        e.preventDefault();
    });

});

