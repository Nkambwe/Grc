let selectedCategory = null;
let selectedAudit = null;

let flatpickrInstances = {};

//..route to home
let auditReportTable;

//..check permissions
window.hasPermission = function (permissionName) {
    return window.userPermissions.some(p => p.toLowerCase() === permissionName.toLowerCase());
};

//..check if user has any of the permissions
window.hasAnyPermission = function (...permissionNames) {
    return permissionNames.some(p => window.hasPermission(p));
};

//..check if user has ALL of the permissions
window.hasAllPermissions = function (...permissionNames) {
    return permissionNames.every(p => window.hasPermission(p));
};

function loadAuditTree() {

    const request = {
        reportId: 0,
        searchTerm: "",
        pageIndex: 0,
        pageSize: 50,
        sortBy: "",
        sortDirection: "ASC"
    };

    $.ajax({
        url: "/grc/compliance/audits/categories-all",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(request),
        success: function (res) {

            //..destroy previous tree if exists (optional but safe)
            if ($('#categoryTree').jstree(true)) {
                $('#categoryTree').jstree("destroy");
            }

            //..initialize jsTree
            $('#categoryTree').jstree({
                core: {
                    data: function (node, cb) {
                        //..sanitize the data
                        const sanitizedData = res.map(item => ({
                            //...item
                            text: escapeAuditHtml(item.text),
                            id: escapeAuditHtml(item.id),
                        }));
                        cb(sanitizedData);
                    },
                    multiple: false,
                    themes: { dots: false, icons: false }
                }, 
                plugins: ["types"],
                types: {
                    category: { icon: "jstree-folder" },
                    audit: { icon: "jstree-file" }
                }
            }).on("select_node.jstree", function (e, data) {
                const tree = $(this).jstree(true);
                const node = data.node;

                //..expand node if it has children
                if (node.children.length > 0) {
                    tree.toggle_node(node);
                }

                //..category logic
                if (hasPermission("CANVIEWCOMPLIANCEAUDITS")) {
                    if (node.type === "category") {
                        selectedCategory = parseInt(node.id.replace("C_", ""));
                        selectedAudit = null;

                        $("#auditView").removeClass("d-none");
                        $("#reportView").addClass("d-none");
                        $("#auditBreadcrumb").html(`<li class="breadcrumb-item active">${node.text}</li>`);

                        auditTable.setData();
                        showAuditBreadcrubs(node.text);
                        loadAudits(selectedCategory);
                    }

                    //..audit logic
                    if (node.type === "audit") {
                        selectedAudit = parseInt(node.id.replace("L_", ""));
                        selectedCategory = parseInt(node.parent.replace("C_", ""));
                        showAuditView(node.text);
                        loadReports(selectedAudit);
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error("Error loading tree:", error);
            console.error("Response:", xhr.responseText);
        }
    });
}

let auditTable = new Tabulator("#auditTable", {
    ajaxURL: "/grc/compliance/audits/audit-list",
    paginationMode: "remote",
    filterMode: "remote",
    sortMode: "remote",
    pagination: true,
    paginationSize: 10,
    paginationSizeSelector: [10, 20, 35, 40, 50],
    paginationCounter: "rows",
    ajaxConfig: {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
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
                reportId: selectedCategory,
                pageIndex: params.page || 1,
                pageSize: params.size || 10,
                searchTerm: "",
                sortBy: "",
                sortDirection: "Ascending"
            };

            //..handle sorting
            if (params.sort && params.sort.length > 0) {
                requestBody.sortBy = params.sort[0].field;
                requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
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
                    //..hide permission alert
                    $('#permissionAlert').hide();

                    if (xhr.status === 401) {
                        window.location = "/login/userlogin";
                    }

                    if (xhr.status === 403) {
                        $('#permissionAlert').show();

                        //..return empty dataset
                        resolve({
                            data: [],
                            last_page: 1,
                            total_records: 0
                        });

                        return;
                    }

                    reject(error);
                    return;
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
        Swal.fire("Failed to load audit list. Please try again.");
    },
    layout: "fitColumns",
    responsiveLayout: "hide",
    columns: [
        {
            title: "AUDIT NAME",
            field: "auditName",
            widthGrow: 2,
            minWidth: 200,
            headerSort: true,
            formatter: function (cell) {
                //..if user has permission to view/edit
                if (hasPermission("CANUPDATECOMPLIANCEAUDITS")) {
                     return `<span class="clickable-title" onclick="viewAudit(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                } else {
                    return `<span >${cell.getValue()}</span>`
                }
            }
        },
        { title: "TYPE", field: "typeName", width: 200 },
        { title: "AUTHORITY", field: "authority", width: 200 },
        { title: "COMMENT", field: "notes", widthGrow: 4, minWidth: 280 },
        {
            title: "VIEW REPORTS",
            formatter: function (cell) {
                //..if user has permission to view/edit
                if (hasPermission("CANVIEWCOMPLIANCEAUDITREPORTS")) {
                     return `<button class="btn btn-sm btn-link">AUDIT REPORTS</button>`;
                } else {
                    return `<button class="btn btn-sm btn-link disabled" disabled>AUDIT REPORTS</button>`
                }
            },
            cellClick: function (e, cell) {
                let audit = cell.getRow().getData();
                selectedAudit = audit.id;
                showAuditView(audit.auditName);
                loadReports(audit.id);
            }
        }
    ]
});

let reportTable = new Tabulator("#reportsTable", {
    ajaxURL: "/grc/compliance/audits/reports-list",
    paginationMode: "remote",
    filterMode: "remote",
    sortMode: "remote",
    pagination: true,
    paginationSize: 10,
    paginationSizeSelector: [10, 20, 35, 40, 50],
    paginationCounter: "rows",
    ajaxConfig: {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
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
                reportId: selectedAudit,
                pageIndex: params.page || 1,
                pageSize: params.size || 10,
                searchTerm: "",
                sortBy: "",
                sortDirection: "Ascending"
            };

            //..handle sorting
            if (params.sort && params.sort.length > 0) {
                requestBody.sortBy = params.sort[0].field;
                requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
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
                     //..hide permission alert
                    $('#permissionAlert').hide();

                    if (xhr.status === 401) {
                        window.location = "/login/userlogin";
                    }

                    if (xhr.status === 403) {
                        $('#permissionAlert').show();

                        //..return empty dataset
                        resolve({
                            data: [],
                            last_page: 1,
                            total_records: 0
                        });

                        return;
                    }

                    reject(error);
                    return;
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
        Swal.fire("Failed to load audit reports. Please try again.");
    },
    layout: "fitColumns",
    responsiveLayout: "hide",
    columns: [
        {
            title: "REF",
            field: "reference",
            width: 280,
            formatter: function (cell) {
                //..if user has permission to view/edit
                if (hasPermission("CANUPDATECOMPLIANCEAUDITREPORTS")) {
                      const id = cell.getRow().getData().id;
                return `<span class="clickable-title"
                         onclick="viewReport(${id})">
                        ${cell.getValue()}
                    </span>`;
                } else {
                    return `<span >${cell.getValue()}</span>`
                }
            }
        },
        {
            title: "AUDIT TYPE",
            field: "auditType",
             width: 200,
        },
        {
            title: "REPORT NAME",
            field: "reportName",
            widthGrow: 4,
            minWidth: 280
        },
        {
            title: "STATUS",
            field: "reportStatus",
            formatter: function (cell) {
                const value = cell.getValue();
                const rowData = cell.getRow().getData();
                const cellEl = cell.getElement();

                let bg = "#C2B70B";
                let clr = "#FFFFFF";

                //..first priority: partial resolution
                if (rowData.resolved !== rowData.exceptionCount) {
                    bg = "#FF9800"; // ORANGE
                }
                else if (value === "CLOSED") {
                    bg = "#28C232"; // GREEN
                }
                else if (value === "DUE") {
                    bg = "#F50C0C"; // RED
                }

                cellEl.style.backgroundColor = bg;
                cellEl.style.color = clr;
                cellEl.style.fontWeight = "bold";
                cellEl.style.textAlign = "center";

                return value;
            }
        },
        {
            title: "REPORT DATE",
            field: "reportDate",
            minWidth: 200,
            formatter: function (cell) {
                const value = cell.getValue();
                if (!value) return "";

                const d = new Date(value);
                const day = String(d.getDate()).padStart(2, "0");
                const month = String(d.getMonth() + 1).padStart(2, "0");
                const year = d.getFullYear();

                return `${day}-${month}-${year}`;
            }
        },
        {
            title: "TOTAL FINDINGS",
            field: "exceptionCount",
            hozAlign: "center",
            width: 200
        },
        {
            title: "RESOLVEDFINDINGS",
            field: "resolved",
            hozAlign: "center",
            width: 200
        }

    ]

});

//..helper function to escape HTML
function escapeAuditHtml(unsafe) {
    if (!unsafe) return unsafe;
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;")
        .replace(/\//g, "&#47;");
}

$('#categoryTree').on("select_node.jstree", function (e, data) {

    let node = data.node;
    const tree = $('#categoryTree').jstree(true);
    if (data.node.children.length > 0) {
        tree.toggle_node(data.node);
    }

    if (node.type === "category") {
        selectedCategory = parseInt(node.id.replace("C_", ""));
        selectedAudit = null;

        // Show category view
        $("#auditView").removeClass("d-none");
        $("#reportView").addClass("d-none");

        const $breadcrumb = $("#auditBreadcrumb");
        $breadcrumb.empty();
        $breadcrumb.append(`<li class="breadcrumb-item active">${node.text}</li>`);

        lawsTable.setData();

        showAuditBreadcrubs(node.text);
        loadAudits(selectedCategory);
    }

    if (node.type === "audit") {
        selectedAudit = parseInt(node.id.replace("L_", ""));
        selectedCategory = parseInt(node.parent.replace("C_", ""));

        loadAudits(node.text);
        loadReports(selectedAudit);
    }
});

//..show breadcrubs for category view
function showAuditBreadcrubs(categoryName) {
    $("#auditBreadcrumb").html(`<li class="breadcrumb-item active">${categoryName}</li>`);
}

function loadAudits(categoryId) {
    selectedCategory = categoryId;

    $("#auditView").removeClass("d-none");
    $("#reportView").addClass("d-none");
    auditTable.setData();
}

function showAuditView(auditName) {
    $("#auditBreadcrumb").append(`<li class="breadcrumb-item active">${auditName}</li>`);
}

//..show for the reports
function loadReports(auditId) {
    selectedAudit = auditId;
    $("#reportView").removeClass("d-none");
    $("#auditView").addClass("d-none");
    reportTable.setData();
}

//...add new type/category
$('.action-btn-audit-report-new, #btnAddCategory').on('click', function () {
    addAuditType("New audit type");
});

function addAuditType(title) {
    //..initialize form fields
    $('#isCategoryEdit').val(false);
    $('#auditTypeId').val(0);
    $('#typeCode').val('');
    $('#typeName').val('');
    $('#description').val('');

    closeAuditPanels();
    $('#auditTypeTitle').text(title);
    $('#auditTypeOverlay').addClass('active');
    $('#auditTypePanel').addClass('active');
    $('body').css('overflow', 'hidden');
}

function saveAuditType(e) {
    e.preventDefault();

    let id = Number($('#auditTypeId').val()) || 0;
    let isEdit = false;
    let typeCode = $('#typeCode').val()?.trim();
    let typeName = $('#typeName').val()?.trim();
    let description = $('#description').val()?.trim();

    let isValid = true;
    if (!typeCode) {
        highlightErrorField('#typeCode', true, 'Type code is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s_-]*$/.test(categoryName)) {
        highlightErrorField('#typeCode', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!typeName) {
        highlightErrorField('#typeName', true, 'Type name field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(typeName)) {
        highlightErrorField('#typeName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!description && !/^[a-zA-Z0-9\s,.]*$/.test(description)) {
        highlightErrorField('#description', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return;
    }

    let recordData = {
        id: id,
        typeCode: typeCode,
        typeName: typeName,
        description: description,
    };

    //..validate required fields
    let errors = [];
    if (!recordData.typeCode)
        errors.push("Type code is required.");

    if (!recordData.typeName)
        errors.push("Type name is required.");

    if (errors.length > 0) {
        highlightAuditField("#typeCode", !recordData.typeCode);
        highlightAuditField("#typeName", !recordData.typeName);
        Swal.fire({
            title: "Category Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveAuditType2Record(isEdit, recordData);
}

function saveAuditType2Record(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/audits/types/type-update"
        : "/grc/compliance/audits/types/type-create";

    Swal.fire({
        title: isEdit ? "Updating audit type..." : "Saving audit type...",
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
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getAuditToken()
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

            Swal.fire(res.message || "Type saved successfully")
                .then(() => {
                    //..close panel
                    closeAuditCategory();
                    window.location.reload();
                });
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

function closeAuditCategory() {
    closeAuditPanels();
    $('#auditTypePanel').removeClass('active');
}

//..add new audit
$('#btnAddAudit').on('click', function () {
    addAudit("New audit", {
        id: 0,
        typeId: 0,
        auditName: '',
        description:'',
        isAuditDeleted: false,
        authorityId:0,
    },false);
});

function addAudit(title, record, isEdit) {
    //..initialize form fields
    $('#isAuditEdit').val(isEdit);
    $('#auditId').val(record.id);
    $('#auditName').val(record.auditName || '');
    $('#Description').val(record.Description || '');
    $('#isAuditDeleted').prop('checked', record.isAuditDeleted);
    $('#authorityId').val(record.authorityId || 0).trigger('change');
    $('#typeId').val(record.typeId || 0).trigger('change');

    //..load dialog window
    closeAudit();
    $('#auditTitle').text(title);
    $('#auditTypeOverlay').addClass('active');
    $('#auditPanel').addClass('active');
    $('body').css('overflow', 'hidden');
}

function viewAudit(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving audit...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findAudit(id)
        .then(record => {
            Swal.close();
            if (record) {
                openAuditPanel('Edit audit', record, true);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Audit not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Failed to load audit details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findAudit(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/audits/audit/audit-retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
            }
        });
    });
}

function openAuditPanel(title, record, isEdit) {
    $('#isAuditEdit').val(isEdit);
    $('#auditId').val(record.auditId);
    $('#auditTypeId').val(record.auditTypeId);
    $('#auditName').val(record.auditName || '');
    $('#authorityId').val(record.authorityId || 0).trigger('change');
    $('#typeId').val(record.typeId || 0).trigger('change');
    $('#isAuditDeleted').prop('checked', record.isAuditDeleted);
    $('#description').val(record.description || '');
    
    //..load dialog window
    closeAuditPanels();
    $('#auditTitle').text(title);
    $('#auditTypeOverlay').addClass('active');
    $('#auditPanel').addClass('active');
    $('body').css('overflow', 'hidden');
}

function saveAudit(e) {
    e.preventDefault();

    let isEdit = $('#isAuditEdit').val();
    let auditName = $('#auditName').val()?.trim();
    let description = $('#description').val()?.trim();

    let isValid = true;
    if (!auditName) {
        highlightErrorField('#auditName', true, 'Audit name field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(auditName)) {
        highlightErrorField('#auditName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!description && !/^[a-zA-Z0-9\s,.]*$/.test(description)) {
        highlightErrorField('#description', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return;
    }

    //..build record payload from form
    let recordData = {
        id: parseInt($('#auditId').val()) || 0,
        authorityId: parseInt($('#authorityId').val()) || 0,
        typeId: parseInt($('#typeId').val()) || 0,
        auditName: auditName,
        isDeleted: $('#isAuditDeleted').is(':checked') ? true : false,
        description: description
    };


    //..validate required fields
    let errors = [];
    if (recordData.authorityId === 0)
        errors.push("Authority is required.");

    if (recordData.typeId === 0)
        errors.push("Audit type is required.");

    if (!recordData.auditName)
        errors.push("Audit name is required.");

    if (errors.length > 0) {
        highlightAuditField("#authorityId", recordData.authorityId === 0);
        highlightAuditField("#typeId", recordData.typeId === 0);
        highlightAuditField("#auditName", !recordData.auditName);
        Swal.fire({
            title: "Audit Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }


    //..call backend
    saveAuditRecord(isEdit, recordData);
}

function saveAuditRecord(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/audits/audit/audit-update"
        : "/grc/compliance/audits/audit/audit-create";

    Swal.fire({
        title: isEdit ? "Updating audit..." : "Saving audit...",
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
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getAuditToken()
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

            Swal.fire(res.message || "Audit saved successfully")
                .then(() => {
                    //..close panel
                    closeAudit();
                    window.location.reload();
                });
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

//..audit name input validation
function validateAuditNameInput(event) {
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
        highlightAuditField('#auditName', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#auditName', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#auditName', false);
        return true;
    }

}

function handleAuditNamePaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

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
        highlightAuditField('#auditName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#auditName', false);
    }
}

//..description input validation
function validateAuditDescrInput(event) {
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
        highlightAuditField('#description', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#description', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#description', false);
        return true;
    }

}

function handleAuditDescrPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

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
        highlightAuditField('#description', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#description', false);
    }
}

//..report name input validation
function validateAuditReferenceInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s_-]/.test(keyChar)) {
        //..clear any existing error when user starts typing valid chars
        highlightAuditField('#reference', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#reference', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#reference', false);
        return true;
    }

}

function handleAuditReferencePaste(event) {
    event.preventDefault();

    //..get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    //..clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    //..move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightAuditField('#reference', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#reference', false);
    }
}

//..report name input validation
function validateReportNameInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s,.]/.test(keyChar)) {
        //..clear any existing error when user starts typing valid chars
        highlightAuditField('#reportName', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#reportName', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#reportName', false);
        return true;
    }

}

function handleReportNamePaste(event) {
    event.preventDefault();

    //..get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    //..clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    // Insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    //..move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    // Show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightAuditField('#reportName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#reportName', false);
    }
}

//..report summery input validation
function validateReportSummeryInput(event) {
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
        highlightAuditField('#summery', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#summery', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#summery', false);
        return true;
    }

}

function handleReportSummeryPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

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
        highlightAuditField('#summery', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#summery', false);
    }
}

//..additional notes input validation
function validateAdditionalNotesInput(event) {
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
        highlightAuditField('#additionalNotes', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#additionalNotes', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#additionalNotes', false);
        return true;
    }

}

function handleAdditionalNotesPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

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
        highlightAuditField('#additionalNotes', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#additionalNotes', false);
    }
}

//..management comments input validation
function validateManagementNotesInput(event) {
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
        highlightAuditField('#managementComments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#managementComments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#managementComments', false);
        return true;
    }

}

function handleManagementNotesPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    //..insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    //..move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    //..show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightAuditField('#managementComments', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#managementComments', false);
    }
}

function closeAudit() {
    closeAuditPanels();
    $('#auditPanel').removeClass('active');
}

//..add new report
$('#btnAddReport').on('click', function () {
    openReportPanel("New audit report", {
        id: 0,
        auditId: selectedAudit,
        reportId: 0,
        reference: '',
        reportName: '',
        summery: '',
        reportDate: null,
        responseDate:null,
        isDeleted: false,
        exceptionCount:0,
        reportStatus: 'UNKNOWN',
        additionalNotes: '',
        managementComments: '',
        findings: [],
        updates:[]
    }, false);
});

function openReportPanel(title, record, isEdit) {
    $('#isReportEdit').val(isEdit);
    $('#selectedAudit').val(record.auditId);
    $('#reportId').val(record.id);
    $('#reference').val(record.reference || '');
    $('#reportName').val(record.reportName || '');
    $('#summery').val(record.summery || '');
    $('#exceptionCount').val(record.exceptionCount || 0);
    $('#isReportDeleted').prop('checked', record.isDeleted);
    $('#authorityId').val(record.authorityId || 0).trigger('change');
    $('#auditId').val(record.auditId || 0).trigger('change');
    $('#Status').val(record.Status || 'UNKNOWN').trigger('change');
    $('#additionalNotes').val(record.additionalNotes || '');
    $('#managementComments').val(record.managementComments || '');

    if (record.reportDate) {
        flatpickrInstances["reportDate"].setDate(record.reportDate, true);
    }

    if (record.responseDate) {
        flatpickrInstances["responseDate"].setDate(record.responseDate, true);
    }

    //..add findings
    loadFindings(record.findings);

    //..add updates
    loadUpdates(record.updates);
   
    //..load dialog window
    closeAuditPanels();
    $('#reportTitle').text(title);
    $('#auditTypeOverlay').addClass('active');
    $('#reportPanel').addClass('active');
    $('body').css('overflow', 'hidden');

    restoreReportActiveTab();
}

function viewReport(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving report...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findReport(id)
        .then(record => {
            Swal.close();
            if (record) {
                console.log(`Report log >> `, updates);
                openReportPanel('Edit report', record, true);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Report not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Failed to load report details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findReport(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/audits/reports/report-retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
            }
        });
    });
}

function saveReport(e) {
    e.preventDefault();

    let isEdit = $('#isReportEdit').val();
    let reference = $('#reference').val()?.trim();
    let reportName = $('#reportName').val()?.trim();
    let summery = $('#summery').val()?.trim();
    let additionalNotes = $('#additionalNotes').val()?.trim();
    let managementComments = $('#managementComments').val()?.trim();


    let isValid = true;
    if (!reference) {
        highlightAuditField('#reference', true, 'Audit reference field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s_-]*$/.test(auditName)) {
        highlightAuditField('#reference', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!reportName) {
        highlightAuditField('#reportName', true, 'Report name field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s.,]*$/.test(auditName)) {
        highlightAuditField('#reportName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!summery) {
        highlightAuditField('#summery', true, 'Report summery field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(summery)) {
        highlightAuditField('#summery', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!additionalNotes && !/^[a-zA-Z0-9\s,.]*$/.test(additionalNotes)) {
        highlightAuditField('#additionalNotes', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!managementComments && !/^[a-zA-Z0-9\s,.]*$/.test(managementComments)) {
        highlightAuditField('#managementComments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return;
    }

    //..payload
    let recordData = {
        id: parseInt($('#reportId').val()) || 0,
        reference: reference,
        reportName: reportName,
        summery: summery,
        reportDate: $('#reportDate').val()?.trim(),
        responseDate: $('#responseDate').val()?.trim(),
        exceptionCount: parseInt($('#exceptionCount').val()) || 0,
        isDeleted: $('#isReportDeleted').is(':checked') ? true : false,
        auditId: Number($('#auditId').val() || 0),
        reportStatus: $('#reportStatus').val() || "UNKNOWN",
        additionalNotes: additionalNotes,
        managementComments: managementComments
    };

    //..validate required fields
    let errors = [];
    if (!recordData.reference)
        errors.push("Reference number is required.");

    if (!recordData.reportName)
        errors.push("Report name is required.");

    if (!recordData.reportDate)
        errors.push("Report date is required.");

    if (!recordData.reportStatus)
        errors.push("Report status is required.");

    if (recordData.auditId === 0)
        errors.push("Audit ID is required.");

    console.log("Audit ID >> " + recordData.auditId);

    if (errors.length > 0) {
        highlightAuditField("#reference", !recordData.reference);
        highlightAuditField("#reportName", !recordData.reportName);
        highlightAuditField("#reportDate", !recordData.reportDate);
        highlightAuditField("#reportStatus", !recordData.reportStatus);
        Swal.fire({
            title: "Report Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }


    //..call backend
    saveAuditReport(isEdit, recordData);
}

function saveAuditReport(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/audits/reports/report-update"
        : "/grc/compliance/audits/reports/report-create";

    Swal.fire({
        title: isEdit ? "Updating audit report..." : "Saving audit report...",
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
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getAuditToken()
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

            Swal.fire(res.message || "Audit report saved successfully")
                .then(() => {
                    //..close panel
                    closeAuditReport();
                    window.location.reload();
                });
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

//..load findings
function loadFindings(findings) {
    const $list = $('.exceptions-list');
    $list.empty();

    if (!Array.isArray(findings) || findings.length === 0) {
        $list.append(`
            <li class="text-muted no-report-items">
                Report has no exceptions/findings reported
            </li>
        `);
        return;
    }

    findings.forEach(finding => {
        const listItem = `
            <li class="audit-card" data-id="${finding.id}">
                <div class="audit-card-header">
                    <span class="audit-title">${finding.finding ?? ''}</span>
                    <span class="audit-status ${getStatusClass(finding.status)}">
                        ${finding.status ?? ''}
                    </span>
                </div>

                <div class="audit-card-body">
                    <p class="audit-action">
                        ${finding.proposedAction ?? ''}
                    </p>

                    <button type="button" class="grc-btn-icon grc-edit-exception"
                            data-id="${finding.id}"
                            title="Edit finding">
                        <i class="mdi mdi-pencil-outline"></i>
                    </button>
                </div>
            </li>
        `;

        $list.append(listItem);
    });

    attachExceptionHandlers();
}

function attachExceptionHandlers() { 
    $('.grc-edit-exception').off('click.editException'); 
    $('.grc-edit-exception').on('click.editException', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        const id = $(this).data('id'); 
        editException(id);
        return false;
    });
}

$(document).on('click', '.grc-edit-exception', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const id = $(this).data('id');
    editException(id);
});

//..status styling helper
function getStatusClass(status) {
    switch ((status || '').toLowerCase()) {
        case 'open': return 'status-open';
        case 'closed': return 'status-closed';
        case 'in progress': return 'status-progress';
        default: return 'status-default';
    }
}

function addUpdate(e) {
    e.preventDefault();

    let reportId = $("#reportId").val();
    openUpdatePanel("Report Update", {
        id: 0,
        reportId: reportId,
        updateNotes: '',
        sendReminders: false,
        reminderMessage: '',
        sendDate: '',
        sendToEmails: '',
        isDeleted: false,
        addedBy: ''

    }, false);
}

//..loading updates
function loadUpdates(updates) {
    const $list = $('.update-list');
    $list.empty();

    if (!Array.isArray(updates) || updates.length === 0) {
        $list.append(`
            <li class="text-muted no-report-items">
                Report has no updates
            </li>
        `);
        return;
    }

    updates.forEach(update => {
        const listItem = `
            <li class="audit-card" data-id="${update.id}">
                <div class="audit-card-header">
                    <span class="audit-title">${update.addedBy ?? ''}</span>
                    <span class="audit-status status-default">${update.addedOn ?? ''}</span>
                </div>

                <div class="audit-card-body">
                    <p class="audit-action">${update.updateNotes ?? ''}</p>
                    <button type="button" class="grc-btn-icon grc-edit-notes" data-id="${update.id}" title="Edit Notes">
                        <i class="mdi mdi-pencil-outline"></i>
                    </button>
                </div>
            </li>
        `;

        $list.append(listItem);
    });

    attachNotesHandlers();
}

function attachNotesHandlers() {
    $('.grc-edit-notes').off('click.editNotes');
    $('.grc-edit-notes').on('click.editNotes', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        const id = $(this).data('id');
        editNotes(id);
        return false;
    });
}

//..audit excptions
function addException(e) {
    e.preventDefault();

    let reportId = $("#reportId").val();
    openExceptionPanel("New Exception", {
        id:0,
        reportId: reportId,
        findings: '',
        recomendations:'',
        proposedAction: '',
        correctiveAction:'',
        isDeleted: false,
        responsibileId: '0',
        executioner: '',
        status: 'UNKNOWN',
        riskLevel:'NONE',
        riskRating:0,
        targetDate: '',
        notes: '',
        
    }, false);
}

//..edit audit exception
function editException(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving exception...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findException(id)
        .then(record => {
            Swal.close();
            if (record) {
                openExceptionPanel('EXCEPTION DETAILS', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Exception record not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load exception details.' });
        });
}

function openExceptionPanel(title, record, isEdit) {
    $('#isExceptionEdit').val(isEdit);
    $('#exceptionId').val(record.id);
    $('#parentId').val(record.reportId);
    $('#findings').val(record.findings || '');
    $('#recomendations').val(record.recomendations || '');
    $('#correctiveAction').val(record.correctiveAction || '');
    $('#proposedAction').val(record.proposedAction || '');
    $('#execDeleted').prop('checked', record.isDeleted);
    $('#executioner').val(record.executioner || '');
    $('#execptionStatus').val(record.status || 'UNKNOWN').trigger('change');
    $('#responsibileId').val(record.responsibileId || 0).trigger('change'); 
    $('#riskLevel').val(record.riskLevel || 'NONE').trigger('change');
    $('#riskRate').val(record.riskRating || 0);
    $('#notes').val(record.notes || '');

    if (record.targetDate) {
        flatpickrInstances["targetDate"].setDate(record.targetDate, true);
    }

    //..load dialog window
    $('#exceptionTitle').text(title);
    $('#outerOverlay').addClass('active');
    $('#exceptionPanel').addClass('active');
    $('body').css('overflow', 'hidden');

}

function openUpdatePanel(title, record, isEdit) {
    $('#isUpdateEdit').val(isEdit);
    $('#updateId').val(record.id);
    $('#updateReportId').val(record.reportId);
    $('#updateNotes').val(record.updateNotes || '');
    $('#upDeleted').prop('checked', record.isDeleted);
    $('#sendReminders').prop('checked', record.sendReminders).trigger('change');
    $('#reminderMessage').val(record.reminderMessage || '');
    $('#sendToEmails').val(record.sendToEmails || '');
    $('#addedBy').val(record.addedBy || '');
    
    if (record.sendDate) {
        flatpickrInstances["sendDate"].setDate(record.sendDate, true);
    }

    //..load dialog window
    $('#updateTitle').text(title);
    $('#outerOverlay').addClass('active');
    $('#updatePanel').addClass('active');
    $('body').css('overflow', 'hidden');
}

function findException(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/audit/exceptions/exception-retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
                return;
            }
        });
    });
}

//..edit audit exception
function editNotes(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving notes...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findUpdate(id)
        .then(record => {
            Swal.close();
            if (record) {
                openUpdatePanel('NOTES DETAILS', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Notes record not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load notes details.' });
        });
}

function findUpdate(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/audits/notes/notes-retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                Swal.fire("Error", error);
                return;
            }
        });
    });
}

function saveException(e) {
    e.preventDefault();

    let isEdit = $('#isExceptionEdit').val();
    let findings = $('#findings').val()?.trim();
    let recomendations = $('#recomendations').val()?.trim();
    let proposedAction = $('#proposedAction').val()?.trim();
    let executioner = $('#executioner').val()?.trim();

    let isValid = true;
    if (!findings) {
        highlightAuditField('#findings', true, 'Notes field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(findings)) {
        highlightAuditField('#findings', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!recomendations) {
        highlightAuditField('#recomendations', true, 'Recommendation field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(recomendations)) {
        highlightAuditField('#recomendations', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!proposedAction && !/^[a-zA-Z0-9\s,.]*$/.test(proposedAction)) {
        highlightAuditField('#proposedAction', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!executioner && !/^[a-zA-Z0-9\s.,]*$/.test(executioner)) {
        highlightAuditField('#executioner', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return;
    }


    //..payload
    let recordData = {
        id: parseInt($('#exceptionId').val()) || 0,
        reportId: parseInt($('#parentId').val()) || 0,
        findings: findings,
        recomendations: $('#recomendations').val() || '',
        proposedAction: $('#proposedAction').val() || '',
        correctiveAction: $('#proposedAction').val() || '',
        isDeleted: $('#execDeleted').is(':checked') ? true : false,
        executioner: $('#executioner').val(),
        status: $('#execptionStatus').val() || "UNKNOWN",
        responsibileId: parseInt($('#responsibileId').val()) || 0,
        riskRate: parseInt($('#riskRate').val()) || 0,
        riskLevel: $('#riskLevel').val() || "NONE",
        targetDate: $('#targetDate').val()?.trim(),
        notes: $('#notes').val() || '',
       
    };

    //..validate required fields
    let errors = [];
    if (!recordData.findings)
        errors.push("Finding field is required.");

    if (!recordData.proposedAction)
        errors.push("Proposed action field is required.");

    if (!recordData.correctiveAction)
        errors.push("Collective action field is required");

    if (!recordData.targetDate)
        errors.push("Target date is required.");

    if (!recordData.status)
        errors.push("Exception status is required.");

    if (recordData.responsibileId === 0)
        errors.push("Department responsible is required.");

    if (errors.length > 0) {
        highlightAuditField("#findings", !recordData.findings);
        highlightAuditField("#proposedAction", !recordData.proposedAction);
        highlightAuditField("#correctiveAction", !recordData.correctiveAction);
        highlightAuditField("#targetDate", !recordData.targetDate);
        highlightAuditField("#execptionStatus", !recordData.status);
        highlightAuditField("#responsibileId", recordData.responsibileId === 0);
        Swal.fire({
            title: "Exceution Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveAuditException(isEdit, recordData);
}

//..description input validation
function validateFindingInput(event) {
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
        highlightAuditField('#findings', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#findings', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#findings', false);
        return true;
    }

}

function handleFindingPaste(event) {
    event.preventDefault();

    //..get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    //..clean the pasted content,remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    //..insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    //..move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    //..show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightAuditField('#findings', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#findings', false);
    }
}

//..description input validation
function validateRecommendationInput(event) {
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
        highlightAuditField('#recomendations', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#recomendations', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#recomendations', false);
        return true;
    }

}

function handleRecommendationPaste(event) {
    event.preventDefault();

    //..get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    //..clean the pasted content,remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    //..insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    //..move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    //..show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightAuditField('#recomendations', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#recomendations', false);
    }
}

//..description input validation
function validateProposedInput(event) {
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
        highlightAuditField('#proposedAction', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#proposedAction', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#proposedAction', false);
        return true;
    }

}

function handleProposedPaste(event) {
    event.preventDefault();

    //..get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    //..clean the pasted content,remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    //..insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    //..move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    //..show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightAuditField('#proposedAction', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#proposedAction', false);
    }
}

//..description input validation
function validateCorrectiveInput(event) {
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
        highlightAuditField('#correctiveAction', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#correctiveAction', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#correctiveAction', false);
        return true;
    }

}

function handleCorrectivePaste(event) {
    event.preventDefault();

    //..get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    //..clean the pasted content,remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    //..insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    //..move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    //..show warning if content was modified (using the field error)
    if (pastedText !== cleanedText) {
        highlightAuditField('#correctiveAction', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#correctiveAction', false);
    }
}

function saveUpdate(e) {
    e.preventDefault();

    let isEdit = $('#isUpdateEdit').val();
    let updateNotes = $('#updateNotes').val()?.trim();
    let reminderMessage = $('#reminderMessage').val()?.trim();
    let addedBy = $('#addedBy').val()?.trim();


    let isValid = true;
    if (!updateNotes) {
        highlightAuditField('#updateNotes', true, 'Notes field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(auditName)) {
        highlightAuditField('#updateNotes', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!reminderMessage && !/^[a-zA-Z0-9\s_-]*$/.test(auditName)) {
        highlightAuditField('#reminderMessage', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!addedBy && !/^[a-zA-Z0-9\s_-]*$/.test(addedBy)) {
        highlightAuditField('#addedBy', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return;
    }

    //..payload
    let recordData = {
        id: parseInt($('#updateId').val()) || 0,
        reportId: parseInt($('#updateReportId').val()) || 0,
        updateNotes:updateNotes,
        sendReminders: $('#sendReminders').is(':checked') ? true : false,
        isDeleted: $('#upDeleted').is(':checked') ? true : false,
        reminderMessage: reminderMessage,
        sendToEmails: $('#sendToEmails').val() || '',
        addedBy: addedBy,
        sendDate: $('#sendDate').val()?.trim()

    };

    //..validate required fields
    let errors = [];
    if (!recordData.updateNotes)
        errors.push("Notes field is required.");

    if (recordData.sendReminders) {
        if (!recordData.reminderMessage)
            errors.push("Message field is required");

        if (!recordData.sendDate)
            errors.push("Send date is required.");

        if (!recordData.sendToEmails)
            errors.push("Email List is required.");
    }

    if (!recordData.addedBy)
        errors.push("Name of person adding the notes is required.");

    if (errors.length > 0) {
        highlightAuditField("#updateNotes", !recordData.updateNotes);

        if (recordData.sendReminders) {
            highlightAuditField("#reminderMessage", !recordData.reminderMessage);
            highlightAuditField("#sendDate", !recordData.sendDate);
            highlightAuditField("#sendToEmails", !recordData.sendToEmails);
        }
        
        highlightAuditField("#addedBy", !recordData.addedBy);

        Swal.fire({
            title: "Notes Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveAuditUpdate(isEdit, recordData);
}

function saveAuditException(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
            ? "/grc/compliance/audit/exceptions/exception-update"
            : "/grc/compliance/audit/exceptions/exception-create";

    Swal.fire({
        title: isEdit ? "Updating audit exception..." : "Saving audit exception...",
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
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getAuditToken()
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

            Swal.fire(res.message || "Audit exception saved successfully")
                .then(() => {
                    //..close panel
                    closeException();
                    window.location.reload();
                });
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

function saveAuditUpdate(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/audits/notes/notes-update"
        : "/grc/compliance/audits/notes/notes-create";

    Swal.fire({
        title: isEdit ? "Updating audit notes..." : "Saving audit notes...",
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
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getAuditToken()
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

            Swal.fire(res.message || "Audit notes saved successfully")
                .then(() => {
                    //..close panel
                    closeUpdate();
                    window.location.reload();
                });
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

//..description input validation
function validateFindingInput(event) {
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
        highlightAuditField('#description', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#description', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#description', false);
        return true;
    }

}

function handleDescrPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

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
        highlightAuditField('#description', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#description', false);
    }
}

//..type name
function validateTypeNameInput(event) {
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
        highlightAuditField('#typeName', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#typeName', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#typeName', false);
        return true;
    }

}

function handleTypeNamePaste(event) {
    event.preventDefault();

    //..set pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    //..clean the pasted content, remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

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
        highlightAuditField('#typeName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#typeName', false);
    }
}

//..type code
function validateTypeCodeInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9\s_-]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightAuditField('#typeCode', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightAuditField('#typeCode', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightAuditField('#typeCode', false);
        return true;
    }

}

function handleTypeCodePaste(event) {
    event.preventDefault();

    //..set pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    //..clean the pasted content, remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9]/g, '');

    //..insert cleaned text at cursor position
    var input = event.target;
    var start = input.selectionStart;
    var end = input.selectionEnd;
    var currentValue = input.value;

    input.value = currentValue.substring(0, start) + cleanedText + currentValue.substring(end);

    //..move cursor to end of inserted text
    input.selectionStart = input.selectionEnd = start + cleanedText.length;

    //..show warning if content was modified, using the field error
    if (pastedText !== cleanedText) {
        highlightAuditField('#typeCode', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightAuditField('#typeCode', false);
    }
}

function closeAuditReport() {
    closeAuditPanels();
    $('#reportPanel').removeClass('active');
}

function closeAuditPanels() {
    $('#auditTypeOverlay').removeClass('active');
    $('body').css('overflow', '');
}

function restoreReportActiveTab() {

    const tab = sessionStorage.getItem('reportTabs');
    if (!tab) return;

    const trigger = document.querySelector(`button[data-bs-target="${tab}"]`);
    if (!trigger) return;

    //..ensure bootstrap is available
    if (typeof bootstrap === 'undefined' || !bootstrap.Tab) return;

    bootstrap.Tab.getOrCreateInstance(trigger).show();
}

function initAuditDate() {

    flatpickrInstances["reportDate"] = flatpickr("#reportDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrInstances["sendDate"] = flatpickr("#sendDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrInstances["responseDate"] = flatpickr("#responseDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrInstances["targetDate"] = flatpickr("#targetDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

function getAuditToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function closeException() {
    $('#outerOverlay').removeClass('active');
    $('#exceptionPanel').removeClass('active');
    $('body').css('overflow', '');
}

function closeUpdate() {
    $('#outerOverlay').removeClass('active');
    $('#updatePanel').removeClass('active');
    $('body').css('overflow', '');
}

function highlightAuditField(selector, hasError, message) {
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
    loadAuditTree();
    initAuditDate();

    $('#typeId, #authorityId').select2({
        width: '100%',
        dropdownParent: $('#auditPanel')
    });

    $('#auditId, #reportStatus').select2({
        width: '100%',
        dropdownParent: $('#reportPanel')
    });

    $('#execptionStatus, #responsibileId, #riskLevel').select2({
        width: '100%',
        dropdownParent: $('#exceptionPanel')
    });

    //..route to home
    $('.action-btn-audit-home').on('click', function () {
        try {
            window.location.href = '/grc/compliance/audit/dashboard';
        } catch (error) {
            console.error('Navigation failed:', error);
            showToast(error, type = 'error');
        }
    });

    $('#categoryForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#auditForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#reportForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#exceptionForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#updateForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#responsibileId').on('change', function () {
        const selectedOption = $(this).find('option:selected');
        const executioner = selectedOption.data('executioner') || '';

        $('#executioner').val(executioner);
    });

    //..hide update message reminder
    const $sendReminders = $('#sendReminders');
    const $notificationBox = $('#notificationBox');

    //..ensure initial state
    $notificationBox.toggle($sendReminders.is(':checked'));

    //..toggle on change
    $sendReminders.on('change', function () {
        if (this.checked) {
            $notificationBox.slideDown(200);
        } else {
            $notificationBox.slideUp(200);
        }
    });

    //..type code validation 
    $('#typeCode').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightAuditField('#typeCode', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s_-]*$/.test(value)) {
            highlightAuditField('#typeCode', true, 'Invalid characters detected');
        } else {
            highlightAuditField('#typeCode', false);
        }
    });

    $('#typeCode').on('focus', function () {
        highlightAuditField('#typeCode', false);
    });

    $('#typeCode').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditField('#typeCode', true, 'Category name is required');
        } else if (!/^[a-zA-Z0-9\s_-]*$/.test(value)) {
            highlightAuditField('#typeCode', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuditField('#typeCode', false);
        }
    });

    $('#typeCode').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s_-]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditField('#typeCode', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditField('#typeCode', false);
                }, 2000);
            }
        }
    });

    //..type name validation 
    $('#typeName').on('keyup', function () {
        var value = $(this).val();

        //..clear error if field is empty
        if (!value) {
            highlightAuditField('#typeName', false);
            return;
        }

        //..show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s_-]*$/.test(value)) {
            highlightAuditField('#typeName', true, 'Invalid characters detected');
        } else {
            highlightAuditField('#typeName', false);
        }
    });

    $('#typeName').on('focus', function () {
        highlightAuditField('#typeName', false);
    });

    $('#typeName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditField('#typeName', true, 'Type name is required');
        } else if (!/^[a-zA-Z0-9\s_-]*$/.test(value)) {
            highlightAuditField('#typeName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuditField('#typeName', false);
        }
    });

    $('#typeName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s_-]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditField('#typeName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditField('#typeName', false);
                }, 2000);
            }
        }
    });

    //..type description
    $('#description').on('keyup', function () {
        var value = $(this).val();

        //..clear error if field is empty
        if (!value) {
            highlightAuditField('#description', false);
            return;
        }

        //..show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s_-]*$/.test(value)) {
            highlightAuditField('#description', true, 'Invalid characters detected');
        } else {
            highlightAuditField('#description', false);
        }
    });

    $('#description').on('focus', function () {
        highlightAuditField('#description', false);
    });

    $('#description').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditField('#description', true, 'Type name is required');
        } else if (!/^[a-zA-Z0-9\s_-]*$/.test(value)) {
            highlightAuditField('#description', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuditField('#description', false);
        }
    });

    $('#description').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s_-]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditField('#description', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditField('#description', false);
                }, 2000);
            }
        }
    });

    $('#reference').on('keyup', function () {
        var value = $(this).val();

        //..clear error if field is empty
        if (!value) {
            highlightAuditField('#reference', false);
            return;
        }

        //..show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuditField('#reference', true, 'Invalid characters detected');
        } else {
            highlightAuditField('#reference', false);
        }
    });

    $('#reference').on('focus', function () {
        highlightAuditField('#reference', false);
    });

    $('#reference').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditField('#reference', true, 'Type name is required');
        } else if (!/^[a-zA-Z0-9\s.,]*$/.test(value)) {
            highlightAuditField('#reference', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuditField('#reference', false);
        }
    });

    $('#reference').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s_-]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditField('#reference', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditField('#reference', false);
                }, 2000);
            }
        }
    });

    $('#reportName').on('keyup', function () {
        var value = $(this).val();

        //..clear error if field is empty
        if (!value) {
            highlightAuditField('#reportName', false);
            return;
        }

        //..show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuditField('#reportName', true, 'Invalid characters detected');
        } else {
            highlightAuditField('#reportName', false);
        }
    });

    $('#reportName').on('focus', function () {
        highlightAuditField('#reportName', false);
    });

    $('#reportName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditField('#reportName', true, 'Report name is required');
        } else if (!/^[a-zA-Z0-9\s.,]*$/.test(value)) {
            highlightAuditField('#reportName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuditField('#reportName', false);
        }
    });

    $('#reportName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s_-]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditField('#reportName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditField('#reportName', false);
                }, 2000);
            }
        }
    });

    $('#summery').on('keyup', function () {
        var value = $(this).val();

        //..clear error if field is empty
        if (!value) {
            highlightAuditField('#summery', false);
            return;
        }

        //..show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightAuditField('#summery', true, 'Invalid characters detected');
        } else {
            highlightAuditField('#summery', false);
        }
    });

    $('#summery').on('focus', function () {
        highlightAuditField('#summery', false);
    });

    $('#summery').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightAuditField('#summery', true, 'Report name is required');
        } else if (!/^[a-zA-Z0-9\s.,]*$/.test(value)) {
            highlightAuditField('#summery', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightAuditField('#summery', false);
        }
    });

    $('#summery').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s_-]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightAuditField('#summery', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightAuditField('#summery', false);
                }, 2000);
            }
        }
    });
});