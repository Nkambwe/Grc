let processRegisterTable;
let dateList = {};
var uploadedFiles = [];
var fileCounter = 0;

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

                //..sorting
                if (params.sort && params.sort.length > 0) {
                    requestBody.sortBy = params.sort[0].field;
                    requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
                }

                //..filtering
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
                headerFilter: "input",
                formatter: function(cell){
                    return `<span class="clickable-title" onclick="viewProcess(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                }
            },
            {
                title: "PROCESS DESCRIPTION",
                field: "description",
                widthGrow: 1,
                minWidth: 400,
                headerFilter: "input",
                frozen: true,
                headerSort: false
            },
            {
                title: "PROCESS OWNER",
                field: "ownerName",
                headerSort: true,
                headerFilter: "input",
                minWidth: 250
            },
            {
                title: "STATUS",
                field: "processStatus",
                minWidth: 200,
                headerFilter: "list",
                headerFilterParams: {
                    values: {
                        "": "All",
                        "DRAFT": "Draft",
                        "INREVIEW": "Under review",
                        "UPTODATE": "Uptodate",
                        "NEEDREVIEW": "Need Review",
                        "PROPOSED": "Newly Proposed",
                        "OBSOLETE": "No longer in use",
                    }
                },
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
        
                    // Professional color scheme with better contrast
                    let bg = "#f8f9fa";      // Light gray background for default
                    let clr = "#212529";      // Dark gray text for readability
                    let border = "2px solid #dee2e6";          // Optional border accent
        
                    switch(value) {
                        case "UPTODATE":
                            bg = "#d4edda";    // Soft green
                            clr = "#155724";    // Dark green text
                            border = "2px solid #28a745";
                            break;
                        case "PROPOSED":
                            bg = "#fff3cd";    // Soft yellow
                            clr = "#856404";    // Dark yellow/brown text
                            border = "2px solid #ffc107";
                            break;
                        case "INREVIEW":
                            bg = "#cce5ff";    // Soft blue
                            clr = "#004085";    // Dark blue text
                            border = "2px solid #007bff";
                            break;
                        case "DRAFT":
                            bg = "#e2e3e5";    // Soft gray
                            clr = "#383d41";    // Dark gray text
                            border = "2px solid #6c757d";
                            break;
                        case "NEEDREVIEW":
                            bg = "#f8d7da";    // Soft red/pink
                            clr = "#721c24";    // Dark red text
                            border = "2px solid #dc3545";
                            break;
                        case "OBSOLETE":
                            bg = "#343a40";    // Dark gray
                            clr = "#ffffff";    // White text
                            border = "2px solid #1d2124";
                            break;
                        default:
                            bg = "#f8f9fa";    // Light gray
                            clr = "#212529";    // Dark gray
                    }
        
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = clr;
                    cellEl.style.fontWeight = "600";
                    cellEl.style.textAlign = "center";
                    cellEl.style.padding = "4px 8px";
                    cellEl.style.borderRadius = "4px";
                    if (border) cellEl.style.border = border;
        
                    return value;
                }
            },
            {
                title: "REVIEW STATUS",
                field: "workflowStage",
                minWidth: 250,
                headerFilter: "list",
                headerFilterParams: {
                    values: {
                        "": "All",
                        "INPROGRESS": "In Progress",
                        "PENDING": "Pending",
                        "ONHOLD": "On Hold",
                        "COMPLETED": "Completed",
                    }
                },
                formatter: function (cell) {
                    const value = cell.getValue();
                    const cellEl = cell.getElement();
        
                    // Professional color scheme for workflow stages
                    let bg = "#f8f9fa";      // Light gray background for default
                    let clr = "#212529";      // Dark gray text for readability
                    let border = "2px solid #dee2e6";
        
                    switch(value) {
                        case "INPROGRESS":
                            bg = "#cce5ff";    // Soft blue
                            clr = "#004085";    // Dark blue text
                            border = "2px solid #007bff";
                            break;
                        case "PENDING":
                            bg = "#fff3cd";    // Soft yellow
                            clr = "#856404";    // Dark yellow/brown text
                            border = "2px solid #ffc107";
                            break;
                        case "ONHOLD":
                            bg = "#f8d7da";    // Soft red/pink
                            clr = "#721c24";    // Dark red text
                            border = "2px solid #dc3545";
                            break;
                        case "COMPLETED":
                            bg = "#d4edda";    // Soft green
                            clr = "#155724";    // Dark green text
                            border = "2px solid #28a745";
                            break;
                        default:
                            bg = "#f8f9fa";    // Light gray
                            clr = "#212529";    // Dark gray
                    }
        
                    cellEl.style.backgroundColor = bg;
                    cellEl.style.color = clr;
                    cellEl.style.fontWeight = "600";
                    cellEl.style.textAlign = "center";
                    cellEl.style.padding = "4px 8px";
                    cellEl.style.borderRadius = "4px";
                    if (border) cellEl.style.border = border;
        
                    return value;
                }
            },
            {
                title: "VERSION",
                field: "version",
                hozAlign: "left",
                minWidth: 120
            }
        ]
    });
}

function createProcess() {
    openProcessEditor('Create new process', {
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
    var isLocked = process?.isLockProcess;
    var status = process?.processStatus;
    console.log("STATUS >> " + status);

    // Populate form fields
    $("#processId").val(process?.id || "");
    $("#isEdit").val(isEdit);
    $("#processName").val(process?.processName || "");
    $("#processDescription").val(process?.description || "");
    $("#typeId").val(process?.typeId || 0).trigger('change.select2');
    $('#isDeleted').prop('checked', process?.isDeleted || false);
    $("#effectiveDate").val(process?.effectiveDate);
    $("#unitId").val(process?.unitId || 0).trigger('change.select2');
    $("#ownerId").val(process?.ownerId || 0).trigger('change.select2');
    $("#assigneedId").val(process?.assigneedId || 0).trigger('change.select2');
    $("#comments").val(process?.comment || "");

    $('#isLockProcess').prop('checked', isLocked);
     //..hide lock process checkbox if not edit mode
    if (!isEdit) {
        $("#lockProcess").hide(); 
        $("#IsDeletedBox").hide(); 
    } else {
        $("#lockProcess").show();
        $("#IsDeletedBox").show(); 
    }

    //..review tab
    $("#processStatus").val(status).trigger('change.select2');
    $("#onholdReason").val(process?.onholdReason || "");

    // Hide "on-hold reason" if status = 3
    if (status === "ONHOLD") {
        $("#onHoldBox").hide();
    } else {
        $("#onHoldBox").show();
    }

    let hodStatus = process?.hodApprovalStatus;
    $("#hodApprovalOn").val(process?.hodApprovalOn || "");
    $("#hodApprovalStatus").val(process?.hodApprovalStatus || "PENDING").trigger('change.select2');
    $("#hoApprovalComment").val(process?.hoApprovalComment || "");
    
    let riskStatus = process?.riskApprovalStatus;
    $("#riskApprovalOn").val(process?.riskApprovalOn || "");
    $("#riskApprovalStatus").val(process?.riskApprovalStatus || "PENDING").trigger('change.select2');
    $("#riskApprovalComment").val(process?.riskApprovalComment || "");
    if (hodStatus === "APPROVED") {
        $("#riskBox").show();
    } else {
         $("#riskBox").hide();
    }

    let compStatus = process?.complianceApprovalStatus;
    $("#complianceApprovalOn").val(process?.complianceApprovalOn || "");
    $("#complianceApprovalStatus").val(process?.complianceApprovalStatus || "PENDING").trigger('change.select2');
    $("#complianceApprovalComment").val(process?.complianceApprovalComment || "");
    if (riskStatus === "APPROVED") {
        $("#compBox").show();
    } else {
         $("#compBox").hide();
    }

    let bopStatus = process?.branchOpsApprovalStatus;
    let needsBop = process?.needsBranchOperations;
    $('#needsBranchOperations').prop('checked', process?.needsBranchOperations || false);
    $("#branchOpsApprovalOn").val(process?.branchOpsApprovalOn || "");
    $("#branchOpsApprovalStatus").val(process?.branchOpsApprovalStatus || "PENDING").trigger('change.select2');
    $("#branchOpsApprovalComment").val(process?.branchOpsApprovalComment || "");
    if (needsBop && compStatus === "APPROVED") {
        $("#branchBox").show();
    } else {
         $("#branchBox").hide();
    }
   
    let needsCredit = process?.needsCreditReview;
    let creditStatus = process?.creditApprovalStatus;
    $('#needsCreditReview').prop('checked', process?.needsCreditReview || false);
    $("#creditApprovalOn").val(process?.creditApprovalOn || "");
    $("#creditApprovalStatus").val(process?.creditApprovalStatus || "PENDING").trigger('change.select2');
    $("#creditApprovalComment").val(process?.creditApprovalComment || "");
    if (needsCredit && bopStatus === "APPROVED") {
         $("#creditBox").show();
    } else {
         $("#creditBox").hide();
    }

    let needsTreasury = process?.needsTreasuryReview;
    let treasuryStatus = process?.creditApprovalStatus;
    $('#needsTreasuryReview').prop('checked', process?.needsTreasuryReview || false);
    $("#treasuryApprovalOn").val(process?.treasuryApprovalOn || "");
    $("#treasuryApprovalStatus").val(process?.treasuryApprovalStatus || "PENDING").trigger('change.select2');
    $("#treasuryApprovalComment").val(process?.treasuryApprovalComment || "");
    if (needsTreasury && creditStatus === "APPROVED") {
         $("#treasuryBox").show();
    } else {
         $("#treasuryBox").hide();
    }
    
    let needsFintech = process?.needsFintechReview;
    $('#needsFintechReview').prop('checked', process?.needsFintechReview || false);
    $("#fintechApprovalOn").val(process?.fintechApprovalOn || "");
    $("#fintechApprovalStatus").val(process?.fintechApprovalStatus || "PENDING").trigger('change.select2');
    $("#fintechApprovalComment").val(process?.fintechApprovalComment || "");
    if (needsFintech && treasuryStatus === "APPROVED"){
         $("#fintechBox").show();
    } else {
         $("#fintechBox").hide();
    }

    $("#fileName").val(process?.fileName || "");
    $("#fileVersion").val(process?.currentVersion || "");

    //..disable all fields if editing a locked process
    if (isLocked) {
        //..disable form fields but NOT tab buttons
        $("#processForm")
            .find("input, textarea, select")
            .not("#isLockProcess")
            .prop("disabled", true);

        //..allow lock toggle & close buttons
        $('#isLockProcess').prop('disabled', false);
        $(".panel-close, .btn-grc-secondary").prop("disabled", false);
    } else {
        //..ensure fields are enabled when not locked
        $("#processForm :input").prop("disabled", false); 
    }

    //..show overlay panel
    $('#processTitle').text(title);
    $('#processOverlay').addClass('active');
    $('#processPanel').addClass('active');
}

//..name input validation
function validateProcessNameInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }
    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#processName', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#processName', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#processName', false);
        return true;
    }

}

function handleProcessNamePaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#processName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#processName', false);
    }
}

//..description input validation
function validateProcessDescriptionInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#processDescription', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#processDescription', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#processDescription', false);
        return true;
    }

}

function handleProcessDescriptionPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#processDescription', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#processDescription', false);
    }
}

//..comments input validation
function validateCommentInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#comments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#comments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#comments', false);
        return true;
    }

}

function validateOnHoldInput(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#comments', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#comments', false);
    }
}

//..comments input validation
function validateCommentInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#onholdReason', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#onholdReason', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#onholdReason', false);
        return true;
    }

}

function handleOnHoldPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#onholdReason', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#onholdReason', false);
    }
}

//..comments input validation
function validateHodInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#hoApprovalComment', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#hoApprovalComment', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#hoApprovalComment', false);
        return true;
    }

}

function handleHodPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#hoApprovalComment', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#hoApprovalComment', false);
    }
}

//..hod comments input validation
function validateHodInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#hoApprovalComment', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#hoApprovalComment', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#hoApprovalComment', false);
        return true;
    }

}

function handleHodPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#hoApprovalComment', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#hoApprovalComment', false);
    }
}

//..risk comments input validation
function validateApprovalInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#riskApprovalComment', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#riskApprovalComment', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#riskApprovalComment', false);
        return true;
    }

}

function handleApprovalPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#riskApprovalComment', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#riskApprovalComment', false);
    }
}

//..compliance comments input validation
function validateComplianceInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#riskApprovalComment', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#riskApprovalComment', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#riskApprovalComment', false);
        return true;
    }

}

function handleCompliancePaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#riskApprovalComment', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#riskApprovalComment', false);
    }
}

//..branch ops comments input validation
function validateBranchOpsInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#branchOpsApprovalComment', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#branchOpsApprovalComment', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#branchOpsApprovalComment', false);
        return true;
    }

}

function handleBranchOpsPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#branchOpsApprovalComment', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#branchOpsApprovalComment', false);
    }
}

//..credit comments input validation
function validateCreditInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#creditApprovalComment', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#creditApprovalComment', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#creditApprovalComment', false);
        return true;
    }

}

function handleCreditPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#creditApprovalComment', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#creditApprovalComment', false);
    }
}

//..credit comments input validation
function validateTreasuryInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightProcessField('#treasuryApprovalComment', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightProcessField('#treasuryApprovalComment', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightProcessField('#treasuryApprovalComment', false);
        return true;
    }

}

function handleTreasuryPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9\s,.]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    // Move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightProcessField('#treasuryApprovalComment', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightProcessField('#treasuryApprovalComment', false);
    }
}

function initiateReview(data) {
    $("#initiateId").val(data.id || 0);
    $("#initiateName").val(data.processName || "");
    $("#initiateDescription").val(data.description || "");

    //..show overlay panel
    $('.initiate-overlay').addClass('active');
    $('#initiatePanel').addClass('active');
}

function initiateProcessReview(e) {
    if (e) e.preventDefault();
    let recordData = {
        id: parseInt($('#initiateId').val()) || 0,
        processName: $('#initiateName').val()?.trim(),
        processStatus: 'INREVIEW',
        unlockReason: $('#unlockReason').val()?.trim(),
    };

    // --- validate required fields ---
    let errors = [];
    if (!recordData.unlockReason)
        errors.push("You must provide reason for unlocking process.");
    
    if (errors.length > 0) {
        highlightProcessField("#unlockReason", !recordData.unlockReason);
        
        Swal.fire({
            title: "Validate Record",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..save review
    saveInitiateReview(recordData);
}

function saveInitiateReview(payload) {
    const url = "/operations/workflow/processes/registers/initiate-review";

    Swal.fire({
        title: "Initiate Review...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getProcessAntiForgeryToken()
        },
        success: function (res) {
            Swal.close();
            if (!res.success) {
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            if (processRegisterTable) {
                if (isEdit && res.data) {
                    processRegisterTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    processRegisterTable.addRow(res.data, true);
                } else {
                    processRegisterTable.replaceData();
                }
            }

            closeInitiatePanel();
        },
        error: function (xhr) {
            Swal.close();

            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }

            Swal.fire({
                title: "Initiation Failed",
                text: errorMessage
            });
        }
    });
}

function closeInitiatePanel() {
    $('.initiate-overlay').removeClass('active');
    $('#initiatePanel').removeClass('active');
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
    $('#processOverlay').removeClass('active');
    $('#processPanel').removeClass('active');
}

function saveProcessRecord(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();
    let recordData = {
        id: parseInt($('#processId').val()) || 0,
        processName: $('#processName').val()?.trim(),
        description: $('#processDescription').val()?.trim(),
        typeId: parseInt($('#typeId').val()) || 0,
        unitId: parseInt($('#unitId').val()) || 0,
        ownerId: parseInt($('#ownerId').val()) || 0,
        responsibilityId: parseInt($('#assigneedId').val()) || 0,
        processStatus: $('#processStatus').val()?.trim(),
        isDeleted: $('#isDeleted').prop('checked'),
        isLockProcess: $('#isLockProcess').prop('checked'),
        onholdReason: $('#onholdReason').val()?.trim(),
        fileName: $('#fileName').val()?.trim(),
        currentVersion: $('#fileVersion').val()?.trim(),
        comments: $('#comments').val()?.trim(),
        needsBranchReview: $('#needsBranchOperations').prop('checked'),
        needsCreditReview: $('#needsCreditReview').prop('checked'),
        needsTreasuryReview: $('#needsTreasuryReview').prop('checked'),
        needsFintechReview: $('#needsFintechReview').prop('checked')
    };

    console.log(recordData);

    // --- validate required fields ---
    let errors = [];
    if (!recordData.processName)
        errors.push("Process name is required.");
    if (!recordData.description)
        errors.push("Process description is required.");
    if (!recordData.processStatus)
        errors.push("Process status is required.");
    if (recordData.typeId === 0)
        errors.push("Process type is required.");
    if (recordData.unitId === 0)
        errors.push("Department unit is required.");
    if (recordData.ownerId === 0)
        errors.push("Process owner is required.");
    if (recordData.responsibilityId === 0)
        errors.push("Responsible manager is required.");
    if (!recordData.comments)
        errors.push("Comment is required.");

    if (errors.length > 0) {
        highlightProcessField("#processName", !recordData.processName);
        highlightProcessField("#processDescription", !recordData.description);
        highlightProcessField("#comment", !recordData.comments);
        highlightProcessField("#processStatus", !recordData.processStatus);
        highlightProcessField("#typeId", recordData.typeId === 0);
        highlightProcessField("#unitId", recordData.unitId === 0);
        highlightProcessField("#ownerId", recordData.ownerId === 0);
        highlightProcessField("#assigneedId", recordData.responsibilityId === 0);
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    // Get files from uploadedFiles array
    let filesToUpload = uploadedFiles.map(f => f.file);

    // Save process first, then upload files
    saveProcessWithFiles(isEdit, recordData, filesToUpload);
}

function saveProcessWithFiles(isEdit, payload, files) {
    const url = (isEdit === true || isEdit === "true")
        ? "/operations/workflow/processes/registers/retrieve/update"
        : "/operations/workflow/processes/registers/retrieve/create";

    Swal.fire({
        title: isEdit ? "Updating process..." : "Saving process...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getProcessAntiForgeryToken()
        },
        success: function (res) {
            if (!res.success) {
                Swal.close();
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            //..if there are files to upload, upload them now
            if (files && files.length > 0) {
                uploadProcessFiles(res.data.id, files, isEdit, res);
            } else {
                // No files, just finish
                handleProcessSaveSuccess(isEdit, res);
            }
        },
        error: function (xhr) {
            Swal.close();
            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }
            Swal.fire({
                title: isEdit ? "Update Failed" : "Save Failed",
                text: errorMessage
            });
        }
    });
}

function uploadProcessFiles(processId, files, isEdit, processResponse) {
    var formData = new FormData();
    formData.append('processId', processId);

    //..append all files
    $.each(files, function (i, fileData) {
        formData.append('files', fileData.file);
        formData.append('fileNames[' + i + ']', fileData.name);
        formData.append('fileIsCurrent[' + i + ']', fileData.isCurrent);
    });

    Swal.fire({
        title: "Uploading files...",
        text: "Please wait while we upload your files.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: "/operations/workflow/processes/registers/upload-files",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getProcessAntiForgeryToken()
        },
        success: function (fileRes) {
            Swal.close();
            if (!fileRes.success) {
                Swal.fire({
                    title: "File Upload Warning",
                    html: "Process saved but files failed to upload: " + fileRes.message
                });
            }
            handleProcessSaveSuccess(isEdit, processResponse);
        },
        error: function (xhr) {
            Swal.close();
            Swal.fire({
                title: "File Upload Warning",
                text: "Process saved but files failed to upload. Please try uploading them again."
            });
            handleProcessSaveSuccess(isEdit, processResponse);
        }
    });
}

function handleProcessSaveSuccess(isEdit, res) {
    Swal.close();
    if (processRegisterTable) {
        if (isEdit && res.data) {
            processRegisterTable.updateData([res.data]);
        } else if (!isEdit && res.data) {
            processRegisterTable.addRow(res.data, true);
        } else {
            processRegisterTable.replaceData();
        }
    }
    closeProcessPanel();

    Swal.fire({
        title: "Success",
        text: isEdit ? "Process updated successfully!" : "Process created successfully!",
        icon: "success",
        timer: 2000
    });
}

// function to get all files data when saving
window.getUploadedFiles = function () {
    return uploadedFiles.map(function (f) {
        return {
            id: f.id,
            name: f.name,
            isCurrent: f.isCurrent,
            file: f.file
        };
    });
};

function getProcessAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

}

function highlightProcessField(selector, hasError, message) {
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

//..toggle section collapse/expand
function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');

    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}

function initDatePickers() {

    dateList["effectiveDate"] = flatpickr("#effectiveDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

function handleFiles(files) {
    $.each(files, function (i, file) {
        var fileId = 'file_' + fileCounter++;
        var fileItem = {
            id: fileId,
            name: file.name,
            file: file,
            isCurrent: false
        };

        uploadedFiles.push(fileItem);
        addFileToList(fileItem);
    });
}

function addFileToList(fileItem) {
    var listItem = $('<li></li>')
        .addClass('border rounded p-3 mb-2 d-flex align-items-center justify-content-between')
        .attr('data-file-id', fileItem.id);

    var leftSection = $('<div></div>').addClass('d-flex align-items-center');

    var checkbox = $('<div></div>').addClass('form-check me-3').html(
        '<input class="form-check-input file-current-checkbox" type="checkbox" id="' + fileItem.id + '">' +
        '<label class="form-check-label" for="' + fileItem.id + '">' +
        '</label>'
    );

    var fileName = $('<span></span>')
        .addClass('file-name')
        .text(fileItem.name);

    leftSection.append(checkbox).append(fileName);

    var removeBtn = $('<button></button>')
        .addClass('btn btn-sm btn-danger')
        .attr('type', 'button')
        .html('<i class="bi bi-trash"></i> Remove')
        .on('click', function (e) {
            e.stopPropagation();
            removeFile(fileItem.id);
        });

    listItem.append(leftSection).append(removeBtn);
    $('#fileList').append(listItem);
}

function removeFile(fileId) {
    uploadedFiles = uploadedFiles.filter(function (f) {
        return f.id !== fileId;
    });
    $('li[data-file-id="' + fileId + '"]').remove();
    updateFileNameField();
}

function updateFileNameField() {
    var currentFile = uploadedFiles.find(function (f) {
        return f.isCurrent === true;
    });

    if (currentFile) {
        $('#fileName').val(currentFile.name);
    } else {
        $('#fileName').val('');
    }
}

function setProcessReadOnly(isLocked) {

    const $form = $("#processForm");

    //..disable ONLY data-entry fields (not buttons, not tab headers)
    $form.find("input, textarea, select")
        .not("#isLockProcess")
        .not("[type='hidden']")
        .prop("disabled", isLocked);

    // Always allow hidden fields
    $form.find("input[type='hidden']").prop("disabled", false);

    // Flatpickr handling
    if (window.flatpickrInstances) {
        Object.values(flatpickrInstances).forEach(fp => {
            if (!fp) return;
            fp.set("clickOpens", !isLocked);
            fp.input.disabled = isLocked;
        });
    }

    // Explicit switches
    $("#isDeleted, #isAligned").prop("disabled", isLocked);

    // Save button only
    $form.find("button[onclick='saveProcessRecord()']")
        .prop("disabled", isLocked)
        .toggleClass("disabled", isLocked);

}

function lockProcess(id, isLocked) {
    let payload = {
        id: id,
        isLocked: isLocked
    };

    const url = "/operations/workflow/processes/registers/lock-process";
    Swal.fire({
        title: isLocked ? "Locking process record..." : "Unlocking process record...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getPolicyAnti2ForgeryToken()
        },
        success: function (res) {
            Swal.close();
            if (!res.success) {
                //..error from the server
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            if (res && res.data) {
                if (isEdit) {
                    processRegisterTable.updateData([res.data]);
                } else {
                    processRegisterTable.addData([res.data], true);
                }
            }

            Swal.fire({
                title: isLocked ? "Locking process record" : "Unlocking process record",
                text: res.message || "Saved successfully.",
                timer: 2000,
                showConfirmButton: false
            });

            closePolicyPanel();
        },
        error: function (xhr) {
            Swal.close();

            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }

            Swal.fire({
                title: isEdit ? "Update Failed" : "Save Failed",
                text: errorMessage
            });
        }
    });
}

function deleteProcessRecord(id, isDeleted) {
    let payload = {
        id: id,
        isDeleted: isDeleted
    };

    const url = "/operations/workflow/processes/registers/delete-process";
    Swal.fire({
        title: isDeleted ? "Deleting process record..." : "Restoring process record...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getPolicyAnti2ForgeryToken()
        },
        success: function (res) {
            Swal.close();
            if (!res.success) {
                //..error from the server
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            if (res && res.data) {
                if (isEdit) {
                    processRegisterTable.updateData([res.data]);
                } else {
                    processRegisterTable.addData([res.data], true);
                }
            }

            Swal.fire({
                title: isLocked ? "Deleting process" : "Restoring process",
                text: res.message || "Request completed successfully.",
                timer: 2000,
                showConfirmButton: false
            });

            closePolicyPanel();
        },
        error: function (xhr) {
            Swal.close();

            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }

            Swal.fire({
                title: isEdit ? "Update Failed" : "Save Failed",
                text: errorMessage
            });
        }
    });
}

$(document).ready(function () {

    initProcessRegisterTable();
  

    $("#onHoldBox").addClass("d-none");

    $('#typeId, #processStatus, #unitId, #ownerId, #assigneedId, #complianceStatus, #branchManagerStatus, #approvalStatus').select2({
        width: '100%',
        dropdownParent: $('#processPanel')
    });

    $('.action-btn-process-new').on('click', function () {
        createProcess();
    });

    $('.action-btn-excel-export').on('click', function () {
        $.ajax({
            url: '/operations/workflow/processes/registers/retrieve/export-all',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(processRegisterTable.getData()),
            xhrFields: { responseType: 'blob' },
            success: function (blob) {
                let link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = "Operations_processes.xlsx";
                link.click();
            },
            error: function () {
                toastr.error("Export failed. Please try again.");
            }
        });
    });

    $("#processStatus").on("change", function () {
        const selectedValue = $(this).val();
        if (selectedValue === "8") {
            $("#onHoldBox").removeClass("d-none");
        } else {
            $("#onHoldBox").addClass("d-none");
        }
    });

    //..click to browse files
    $('#dropZone').on('click', function (e) {
        if (e.target === this || $(e.target).closest('#dropZone').length) {
            document.getElementById('fileInput').click();
        }
    });

    //..handle file input change
    $('#fileInput').on('change', function (e) {
        handleFiles(e.target.files);
        $(this).val('');
    });

    //..prevent default drag behaviors
    $('#dropZone').on('drag dragstart dragend dragover dragenter dragleave drop', function (e) {
        e.preventDefault();
        e.stopPropagation();
    });

    //..add hover effect
    $('#dropZone').on('dragover dragenter', function () {
        $(this).addClass('border-primary bg-light');
    });

    $('#dropZone').on('dragleave dragend drop', function () {
        $(this).removeClass('border-primary bg-light');
    });

    //..handle drop
    $('#dropZone').on('drop', function (e) {
        var files = e.originalEvent.dataTransfer.files;
        handleFiles(files);
    });

    //..handle checkbox changes
    $(document).on('change', '.file-current-checkbox', function () {
        var fileId = $(this).attr('id');
        var isChecked = $(this).is(':checked');

        // Uncheck all other checkboxes (only one can be current)
        if (isChecked) {
            $('.file-current-checkbox').not(this).prop('checked', false);
            uploadedFiles.forEach(function (f) {
                f.isCurrent = false;
            });
        }

        // Update the current file
        var fileItem = uploadedFiles.find(function (f) {
            return f.id === fileId;
        });
        if (fileItem) {
            fileItem.isCurrent = isChecked;
        }

        // Update the fileName text field
        updateFileNameField();
    });

    //..function to get all files data when saving
    window.getUploadedFiles = function () {
        return uploadedFiles.map(function (f) {
            return {
                id: f.id,
                name: f.name,
                isCurrent: f.isCurrent,
                file: f.file
            };
        });
    };

    $('#processForm').on('submit', function (e) {
        e.preventDefault();
    });

    //..render process readonly
    const $isLocked = $('#isLockProcess');

    //..toggle on change
    $isLocked.on('change', function () {
        const isLocked = this.checked;
        const id = $('#processId').val();

        setProcessReadOnly(isLocked);
        lockProcess(id, isLocked);
    });

     //..render process readonly
    const $isDeleted = $('#isDeleted');

    //..toggle on change
    $isDeleted.on('change', function () {
        const isDeleted = this.checked;
        const id = $('#processId').val();
        deleteProcessRecord(id, isLocked);
    });

    //..category name validation 
    $('#processName').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#processName', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#processName', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#processName', false);
        }
    });

    $('#processName').on('focus', function () {
        highlightProcessField('#processName', false);
    });

    $('#processName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightProcessField('#processName', true, 'Process name is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#processName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#processName', false);
        }
    });

    $('#processName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#processName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#processName', false);
                }, 2000);
            }
        }
    });

    //..process description validation 
    $('#processDescription').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#processDescription', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#processDescription', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#processDescription', false);
        }
    });

    $('#processDescription').on('focus', function () {
        highlightProcessField('#processDescription', false);
    });

    $('#processDescription').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightProcessField('#processDescription', true, 'process description is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#processDescription', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#processDescription', false);
        }
    });

    $('#processDescription').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#processDescription', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#processDescription', false);
                }, 2000);
            }
        }
    });

    //..comments validation 
    $('#comments').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#comments', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#comments', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#comments', false);
        }
    });

    $('#comments').on('focus', function () {
        highlightProcessField('#comments', false);
    });

    $('#comments').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightProcessField('#comments', true, 'Comments is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#comments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#comments', false);
        }
    });

    $('#comments').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#comments', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#comments', false);
                }, 2000);
            }
        }
    });

    //..on hold reason validation 
    $('#onholdReason').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#onholdReason', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#onholdReason', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#onholdReason', false);
        }
    });

    $('#onholdReason').on('focus', function () {
        highlightProcessField('#onholdReason', false);
    });

    $('#onholdReason').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#onholdReason', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#onholdReason', false);
        }
    });

    $('#onholdReason').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#onholdReason', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#onholdReason', false);
                }, 2000);
            }
        }
    });

    //..hod comments validation 
    $('#hoApprovalComment').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#hoApprovalComment', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#hoApprovalComment', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#hoApprovalComment', false);
        }
    });

    $('#hoApprovalComment').on('focus', function () {
        highlightProcessField('#hoApprovalComment', false);
    });

    $('#hoApprovalComment').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#hoApprovalComment', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#hoApprovalComment', false);
        }
    });

    $('#hoApprovalComment').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#hoApprovalComment', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#hoApprovalComment', false);
                }, 2000);
            }
        }
    });
    
    //..hod comments validation 
    $('#riskApprovalComment').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#riskApprovalComment', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#riskApprovalComment', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#riskApprovalComment', false);
        }
    });

    $('#riskApprovalComment').on('focus', function () {
        highlightProcessField('#riskApprovalComment', false);
    });

    $('#riskApprovalComment').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#riskApprovalComment', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#riskApprovalComment', false);
        }
    });

    $('#riskApprovalComment').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#riskApprovalComment', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#riskApprovalComment', false);
                }, 2000);
            }
        }
    });
    
    //..comp comments validation 
    $('#complianceApprovalComment').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#complianceApprovalComment', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#complianceApprovalComment', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#complianceApprovalComment', false);
        }
    });

    $('#complianceApprovalComment').on('focus', function () {
        highlightProcessField('#complianceApprovalComment', false);
    });

    $('#complianceApprovalComment').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#complianceApprovalComment', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#complianceApprovalComment', false);
        }
    });

    $('#complianceApprovalComment').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#complianceApprovalComment', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#complianceApprovalComment', false);
                }, 2000);
            }
        }
    });
    
    //..comp comments validation 
    $('#branchOpsApprovalComment').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#branchOpsApprovalComment', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#branchOpsApprovalComment', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#branchOpsApprovalComment', false);
        }
    });

    $('#branchOpsApprovalComment').on('focus', function () {
        highlightProcessField('#branchOpsApprovalComment', false);
    });

    $('#branchOpsApprovalComment').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#branchOpsApprovalComment', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#branchOpsApprovalComment', false);
        }
    });

    $('#branchOpsApprovalComment').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#branchOpsApprovalComment', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#branchOpsApprovalComment', false);
                }, 2000);
            }
        }
    });
    
    //..credit comments validation 
    $('#creditApprovalComment').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#creditApprovalComment', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#creditApprovalComment', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#creditApprovalComment', false);
        }
    });

    $('#creditApprovalComment').on('focus', function () {
        highlightProcessField('#creditApprovalComment', false);
    });

    $('#creditApprovalComment').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#creditApprovalComment', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#creditApprovalComment', false);
        }
    });

    $('#creditApprovalComment').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#creditApprovalComment', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#creditApprovalComment', false);
                }, 2000);
            }
        }
    });
    
    //..treasury comments validation 
    $('#treasuryApprovalComment').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#treasuryApprovalComment', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#treasuryApprovalComment', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#treasuryApprovalComment', false);
        }
    });

    $('#treasuryApprovalComment').on('focus', function () {
        highlightProcessField('#treasuryApprovalComment', false);
    });

    $('#treasuryApprovalComment').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#treasuryApprovalComment', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#treasuryApprovalComment', false);
        }
    });

    $('#treasuryApprovalComment').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#treasuryApprovalComment', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#treasuryApprovalComment', false);
                }, 2000);
            }
        }
    });
    
    //..fintech comments validation 
    $('#fintechApprovalComment').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightProcessField('#fintechApprovalComment', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#fintechApprovalComment', true, 'Invalid characters detected');
        } else {
            highlightProcessField('#fintechApprovalComment', false);
        }
    });

    $('#fintechApprovalComment').on('focus', function () {
        highlightProcessField('#fintechApprovalComment', false);
    });

    $('#fintechApprovalComment').on('blur', function () {
        var value = $(this).val().trim();

        if (!value && !/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightProcessField('#fintechApprovalComment', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightProcessField('#fintechApprovalComment', false);
        }
    });

    $('#fintechApprovalComment').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightProcessField('#fintechApprovalComment', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightProcessField('#fintechApprovalComment', false);
                }, 2000);
            }
        }
    });

    initDatePickers();

});