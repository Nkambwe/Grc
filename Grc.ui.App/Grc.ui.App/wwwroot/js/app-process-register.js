let processRegisterTable;
function initProcessRegisterTable() {
    processRegisterTable = new Tabulator("#processRegisterTable", {
        ajaxURL: "/operations/workflow/processes/register/all",
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
                        ["processName", "description", "typeName", "ownerName", "assigneeName", "unitName"].includes(f.field)
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
            alert("Failed to load processes. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "PROCESS NAME",
                field: "processName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewProcess(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "PROCESS DESCRIPTION", field: "description", widthGrow: 1, minWidth: 400, frozen: true, headerSort: false },
            { title: "PROCESS TYPE", field: "typeName", headerSort: true, minWidth: 200 },
            { title: "PROCESS OWNER", field: "ownerName", headerSort: true, minWidth: 200 },
            { title: "ATTACHED UNIT", field: "unitName", minWidth: 250 },
            { title: "PROCESS MANAGER", field: "assigneeName", minWidth: 400 },
            {
                title: "ON FILE",
                field: "fileName",
                minWidth: 100,
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<span class="clickable-title" onclick="viewFile(${rowData.fileName})">${rowData.fileName}</span>`
                }
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteProcess(${rowData.id})">
                        <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                        <span>DELETE</span>
                    </button>`;
                },
                width: 50,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="requestApproval(${rowData.id})">
                        <span><i class="mdi mdi-share-all-outline" aria-hidden="true"></i></span>
                        <span>REAQUEST REVIEW</span>
                    </button>`;
                },
                width: 50,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessSearch();
}

function createProcess() {
    openProcessEditor('New Process', {
        // Populate form fields
        id:0,
        isEdit:false,
        processName:'',
        description:'',
        typeId: 0,
        isDeleted: false,

        //..process status
        processStatus:0,
        comment: '',
        onholdReason:'',

        //..file info
        originalOnFile: true,
        fileName:'',
        CurrentVersion: '',

        //..approval info
        approvalStatus:'',
        approvalComment: '',
        effectiveDate:'',

        //..responsibility
        unitId: 0,
        ownerId:0,
        assigneedId:0
    }, false);
}

function findProcess(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/registers/retrieve/${encodeURIComponent(id)}`,
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

function openProcessEditor(title, process, isEdit) {
    // Populate form fields
    $("#processId").val(process?.id || "");
    $("#isEdit").val(isEdit);
    $("#processName").val(process?.processName || "");
    $("#processDescription").val(process?.description || "");
    $("#typeId").val(process?.typeId || 0).trigger('change.select2');
    $('#isDeleted').prop('checked', process?.isDeleted || false);
    $('#isLockProcess').prop('checked', process?.isLockProcess || false);
    $("#effectiveDate").val(process?.effectiveDate);

    //..process status
    $("#processStatus").val(process?.processStatus || 0).trigger('change.select2');
    $("#comment").val(process?.comment || "");
    $("#onholdReason").val(process?.onholdReason || "");

    $('#originalOnFile').prop('originalOnFile', process?.originalOnFile || true);
    $("#fileName").val(process?.fileName || "");
    $("#fileVersion").val(process?.CurrentVersion || "");

    //..responsibility
    $("#unitId").val(process?.unitId || 0).trigger('change.select2');
    $("#ownerId").val(process?.ownerId || 0).trigger('change.select2');
    $("#assigneedId").val(process?.assigneedId || 0).trigger('change.select2');

    //..approval info
    $("#hodApprovalOn").val(process?.hodApprovalOn || "");
    $("#hodApprovalStatus").val(process?.hodApprovalStatus || "PENDING").trigger('change.select2');
    $("#hoApprovalComment").val(process?.hoApprovalComment || "");
    $("#riskApprovalOn").val(process?.riskApprovalOn || "");
    $("#riskApprovalStatus").val(process?.riskApprovalStatus || "PENDING").trigger('change.select2');
    $("#riskApprovalComment").val(process?.riskApprovalComment || "");
    $("#complianceApprovalOn").val(process?.complianceApprovalOn || "");
    $("#complianceApprovalStatus").val(process?.complianceApprovalStatus || "PENDING").trigger('change.select2');
    $("#complianceApprovalComment").val(process?.complianceApprovalComment || "");
    $('#needsBranchOperations').prop('checked', process?.needsBranchOperations || false);
    $("#branchOpsApprovalOn").val(process?.branchOpsApprovalOn || "");
    $("#branchOpsApprovalStatus").val(process?.branchOpsApprovalStatus || "PENDING").trigger('change.select2');
    $("#branchOpsApprovalComment").val(process?.branchOpsApprovalComment || "");
    $('#needsCreditReview').prop('checked', process?.needsCreditReview || false);
    $("#creditApprovalOn").val(process?.creditApprovalOn || "");
    $("#creditApprovalStatus").val(process?.creditApprovalStatus || "PENDING").trigger('change.select2');
    $("#creditApprovalComment").val(process?.creditApprovalComment || "");
    $('#needsTreasuryReview').prop('checked', process?.needsTreasuryReview || false);
    $("#treasuryApprovalOn").val(process?.treasuryApprovalOn || "");
    $("#treasuryApprovalStatus").val(process?.treasuryApprovalStatus || "PENDING").trigger('change.select2');
    $("#treasuryApprovalComment").val(process?.treasuryApprovalComment || "");
    $('#needsFintechReview').prop('checked', process?.needsFintechReview || false);
    $("#fintechApprovalOn").val(process?.fintechApprovalOn || "");
    $("#fintechApprovalStatus").val(process?.fintechApprovalStatus || "PENDING").trigger('change.select2');
    $("#fintechApprovalComment").val(process?.fintechApprovalComment || "");

    // Show overlay panel
    $('#processPanelTitle').text(title);
    $('.process-overlay').addClass('active');
    $('#collapsePanel').addClass('active');
}

function viewProcess(id){
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving process record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findProcess(id)
        .then(record => {
            Swal.close();
            if (record) {
                openProcessEditor('Edit Process', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Process record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load process details.' });
        });
}

function deleteProcess(id) {
    alert('Delete Process >>> ' + id);
}

function viewFile(fileName) {
    alert(`View File >>> ${fileName}`);
}

function initProcessSearch() {
    const searchInput = $('#processesSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processRegisterTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "description", type: "like", value: searchTerm },
                        { field: "typeName", type: "like", value: searchTerm },
                        { field: "ownerName", type: "like", value: searchTerm },
                        { field: "assigneeName", type: "like", value: searchTerm },
                        { field: "unitName", type: "like", value: searchTerm }
                    ]
                ]);
                processRegisterTable.setPage(1, true);
            } else {
                processRegisterTable.clearFilter();
            }
        }, 300);
    });
}

function closeProcessPanel() {
    $('.process-overlay').removeClass('active');
    $('#collapsePanel').removeClass('active');
}

function saveProcessRecord(e) {

}


//..toggle section collapse/expand
function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');

    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}


$(document).ready(function () {

    initProcessRegisterTable();

    $('#typeId, #processStatus, #unitId, #ownerId, #assigneedId, #complianceStatus, #branchManagerStatus, #approvalStatus').select2({
        width: '100%',
        dropdownParent: $('#collapsePanel')
    });

    //..initialize Flatpickr
    flatpickr("#effectiveDate", {
        dateFormat: "Y-m-d",
        allowInput: true
    });

    $('.action-btn-process-new').on('click', function () {
        createProcess();
    });

    $('.action-btn-proposed-list').on('click', function () {
        alert("Proposed Button clicked");
    });

    $('.action-btn-unchanged-list').on('click', function () {
        alert("Unchanged Button clicked");
    });

    $('.action-btn-due-review-list').on('click', function () {
        alert("Button clicked");
    });

    $('.action-btn-dorman-list').on('click', function () {
        alert("Dormant Button clicked");
    });

    $('.action-btn-cancelled-list').on('click', function () {
        alert("Cancelled Button clicked");
    });

    $('.action-btn-completed-list').on('click', function () {
        alert("Complete Button clicked");
    });

    $('.action-btn-dormant-list').on('click', function () {
        alert("Dormat Button clicked");
    });

    $('.action-btn-cancelled-list').on('click', function () {
        alert("Complete Button clicked");
    });

    $('.action-btn-process-export').on('click', function () {
        alert("Export Process Button clicked");
    });

    $('#processForm').on('submit', function (e) {
        e.preventDefault();
    });

});