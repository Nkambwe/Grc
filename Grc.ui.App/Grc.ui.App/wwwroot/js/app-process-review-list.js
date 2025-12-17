let processReviewTable;

function initProcessReviewListTable() {
    processReviewTable = new Tabulator("#processReviewTable", {
        ajaxURL: "/operations/workflow/processes/review-list",
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
                        ["processName", "description", "ownerName", "unitName", "assigneeName"].includes(f.field)
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
                minWidth: 200,
                widthGrow: 1,
                headerSort: true,
                frozen: true
            },
            { title: "REVIEW COMMENTS", field: "comment", widthGrow: 2, minWidth: 400, frozen: true, headerSort: false },
            { title: "ATTACHED UNIT", field: "unitName", minWidth: 250 },
            { title: "PROCESS MANAGER", field: "assigneeName", minWidth: 400 },
            {
                title: "VIEW",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="viewRecord(${rowData.id})">
                                <span><i class="mdi mdi-eye-arrow-right-outline" aria-hidden="true"></i></span>
                                <span>VIEW</span>
                            </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    //..search init
    initProcessReviewSearch();
}

function findNewRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/approval/new-request/${encodeURIComponent(id)}`,
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

function viewRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving process record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findNewRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openNewEditor('Approve Process', record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Approval record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load process details.' });
        });
}

function openNewEditor(title, approval) {
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

    const date = new Date(approval?.requestDate);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();
    const formattedDate = `${day}-${month}-${year}`;

    //..populate form fields
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
    $('#requiresBopApproval').prop('checked', bopRequired);
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

    updateSectionExpansion(bopRequired, creditRequired, treasuryRequired, fintechRequired, hodStatus, riskStatus, compStatus, bopStatus, creditStatus, treasuryStatus, fintechStatus);
    $('#approvalPanelTitle').text(title);
    $('.process-overlay').addClass('active');
    $('#collapsePanel').addClass('active');
}

function updateSectionExpansion(bopRequired, creditRequired, treasuryRequired, fintechRequired, hodStatus, riskStatus, compStatus, bopStatus, creditStatus, treasuryStatus, fintechStatus) {
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

function initProcessReviewSearch() {
    const searchInput = $('#reviewSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processNewTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "description", type: "like", value: searchTerm },
                        { field: "ownerName", type: "like", value: searchTerm },
                        { field: "assigneeName", type: "like", value: searchTerm },
                        { field: "unitName", type: "like", value: searchTerm }
                    ]
                ]);
                processReviewTable.setPage(1, true);
            } else {
                processReviewTable.clearFilter();
            }
        }, 300);
    });
}

function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');
    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}

function closeApprovalPanel() {
    $('.process-overlay').removeClass('active');
    $('#collapsePanel').removeClass('active');
}

$(document).ready(function () {

    initProcessReviewListTable();
    $('#processReviewForm').on('submit', function (e) {
        e.preventDefault();
    });

});