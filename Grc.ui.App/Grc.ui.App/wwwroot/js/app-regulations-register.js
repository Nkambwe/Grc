let selectedCategory = null;
let selectedLaw = null;
let flatpickr1Instances = {};

//..check permissions
window.hasPermission = function (permissionName) {
    return window.userPermissions.some(p => p.toLowerCase() === permissionName.toLowerCase());
};

//..check if user has ANY of the permissions
window.hasAnyPermission = function (...permissionNames) {
    return permissionNames.some(p => window.hasPermission(p));
};

//..check if user has ALL of the permissions
window.hasAllPermissions = function (...permissionNames) {
    return permissionNames.every(p => window.hasPermission(p));
};

function loadRegulatoryTree() {
    const request = {
        activityTypeId: 0,
        searchTerm: "",
        pageIndex: 0,
        pageSize: 50,
        sortBy: "",
        sortDirection: "ASC"
    };
    $.ajax({
        url: "/grc/compliance/support/categories-all",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(request),
        success: function (res) {

            //..destroy previous tree if exists (optional but safe)
            if ($('#regulatoryTree').jstree(true)) {
                $('#regulatoryTree').jstree("destroy");
            }

            // Initialize jsTree
            $('#regulatoryTree').jstree({
                core: {
                    data: function (node, cb) {
                        //..sanitize the data before passing to jsTree
                        const sanitizedData = res.map(item => ({
                            ...item,
                            text: escapeHtml(item.text), // Escape the text
                            //..if there are other fields that might contain malicious content
                            id: escapeHtml(item.id),
                        }));
                        cb(sanitizedData);
                    },
                    multiple: false,
                    themes: { dots: false, icons: false }
                },
                plugins: ["types"],
                types: {
                    category: { icon: "jstree-folder" },
                    law: { icon: "jstree-file" }
                }
            }).on("select_node.jstree", function (e, data) {
                const tree = $(this).jstree(true);
                const node = data.node;

                //..expand node if it has children
                if (node.children.length > 0) {
                    tree.toggle_node(node);
                }

                if (hasPermission("CANVIEWSTATUTE")) {
                    // Category logic
                    if (node.type === "category") {

                        selectedCategory = parseInt(node.id.replace("C_", ""));
                        selectedLaw = null;

                        $("#categoryView").removeClass("d-none");
                        $("#lawView").addClass("d-none");

                        $("#regulatoryBreadcrumb").html(
                            `<li class="breadcrumb-item active">${node.text}</li>`
                        );

                        lawsTable.setData();
                        showBreadcrubs(node.text);
                        loadLaws(selectedCategory);
                    }

                    // Law logic
                    if (node.type === "law") {

                        selectedLaw = parseInt(node.id.replace("L_", ""));
                        selectedCategory = parseInt(node.parent.replace("C_", ""));
                        showLawView(node.text);
                        loadActs(selectedLaw);
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error("Error loading tree:", error);
            console.error("Response:", xhr.responseText);
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
}

//..helper function to escape HTML
function escapeHtml(unsafe) {
    if (!unsafe) return unsafe;
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;")
        .replace(/\//g, "&#47;");
}

function sanitizeInput(input) {
    if (!input) return input;
    //..remove any script tags, SQL injection attempts, etc.
    return input
        .replace(/<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi, '')
        .replace(/onerror\s*=/gi, 'data-blocked-onerror=')
        .replace(/onload\s*=/gi, 'data-blocked-onload=')
        .replace(/javascript:/gi, 'blocked:')
        //..remove SQL comment attempts
        .replace(/--/g, '')

        //..remove SQL statement terminators
        .replace(/;/g, '');
}

let lawsTable = new Tabulator("#lawsTable", {
    ajaxURL: "/grc/compliance/register/laws-list",
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
                activityTypeId: selectedCategory,
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
        Swal.fire("Failed to load regulatory list. Please try again.");
    },
    layout: "fitColumns",
    responsiveLayout: "hide",
    columns: [
        {
            title: "LAW/REGULATORY NAME",
            field: "lawName",
            widthGrow: 4,
            minWidth: 280,
            headerSort: true,
            formatter: function (cell) {
                //..if user has permission to view/edit
                if (hasPermission("CANVIEWSTATUTE")) {
                    return `<span class="clickable-title" onclick="viewLaw(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                } else {
                    return `<span >${cell.getValue()}</span>`
                }
            }
        },
        { title: "REF CODE", field: "lawCode", width: 200 },
        { title: "STATUS", field: "status", width: 200 },
        {
            title: "VIEW ACT/SECTION",
            formatter: function (cell) {
                //..if user has permission to view/edit
                if (hasPermission("CANVIEWSTATUTE")) {
                    return `<button class="btn btn-sm btn-link">ACTS/SECTIONS</button>`;
                } else {
                    return `<button class="btn btn-sm btn-link disabled" disabled>ACTS/SECTIONS</button>`;
                }
            },
            cellClick: function (e, cell) {
                let law = cell.getRow().getData();
                selectedLaw = law.id;
                showLawView(law.lawName);
                loadActs(law.id);
            }
        }
    ]
});

let actsTable = new Tabulator("#actsTable", {
    ajaxURL: "/grc/compliance/register/acts-all",
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
                activityTypeId: selectedLaw,
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
        Swal.fire("Failed to load regulatory acts. Please try again.");
    },
    layout: "fitColumns",
    responsiveLayout: "hide",
    columns: [
        {
            title: "Section",
            field: "sectionNumber",
            headerFilter: "input",
            width: 120,
            minWidth: 120,
            headerSort: true,
            formatter: function (cell) {
                console.log(cell.getValue());
                const id = cell.getRow().getData().id;
                //..if user has permission to view/edit
                if (hasPermission("CANUPDATESTATUTE")) {
                    return `<span class="clickable-title" onclick="viewSection(${id})">
                                ${cell.getValue()}
                            </span>`;
                } else {
                    return `<span >${cell.getValue()}</span>`
                }
            }
        },
        {
            title: "Title",
            field: "title",
            headerFilter: "input",
            widthGrow: 4,
            minWidth: 280
        },
        {
            title: "Coverage",
            field: "coverage",
            hozAlign: "center",
            width: 200,
            formatter: progressFormatter
        },
        {
            title: "Covered",
            field: "isCovered",
            hozAlign: "center",
            width: 200,
            formatter: coverageIconFormatter
        },
        {
            title: "Assurance",
            field: "assurance",
            hozAlign: "center",
            width: 200,
            formatter: percentFormatter
        },
        {
            title: "Mandatory",
            field: "isMandatory",
            hozAlign: "center",
            width: 200,
            formatter: function (cell) {
                return cell.getValue()
                    ? "<span class='text-success fw-bold'>YES</span>"
                    : "<span class='text-danger fw-bold'>NO</span>";
            }
        }

    ]

});

$('#regulatoryTree').on("select_node.jstree", function (e, data) {

    let node = data.node;
    const tree = $('#regulatoryTree').jstree(true);
    if (data.node.children.length > 0) {
        tree.toggle_node(data.node);
    }

    if (node.type === "category") {
        selectedCategory = parseInt(node.id.replace("C_", ""));
        selectedLaw = null;

        // Show category view
        $("#categoryView").removeClass("d-none");
        $("#lawView").addClass("d-none");

        $("#regulatoryBreadcrumb").html(
            `<li class="breadcrumb-item active">${node.text}</li>`
        );

        lawsTable.setData();

        showBreadcrubs(node.text);
        loadLaws(selectedCategory);
    }

    if (node.type === "law") {
        selectedLaw = parseInt(node.id.replace("L_", ""));
        selectedCategory = parseInt(node.parent.replace("C_", ""));

        showLawView(node.text);
        loadActs(selectedLaw);
    }
});

//..show list of category laws
function loadLaws(categoryId) {
    selectedCategory = categoryId;

    $("#categoryView").removeClass("d-none");
    $("#lawView").addClass("d-none");
    lawsTable.setData();
}

//..show acts/sections for the law
function loadActs(lawId) {
    selectedLaw = lawId;
    $("#lawView").removeClass("d-none");
    $("#categoryView").addClass("d-none");
    console.log("Load acts");
    actsTable.setData();
}

//..show breadcrubs for category view
function showBreadcrubs(categoryName) {
    $("#regulatoryBreadcrumb").html(`
        <li class="breadcrumb-item active">${categoryName}</li>
    `);
}

function showLawView(lawName) {
    $("#regulatoryBreadcrumb").append(`
        <li class="breadcrumb-item active">${lawName}</li>
    `);
}

$(".action-btn-act-new, #btnAddCategory").on("click", function () {
    addCategory();
});

function addCategory() {
    openCategoryPanel('New Regulatory Category', {
        id: 0,
        categoryName: '',
        comments: '',
        isDeleted: false,
    }, false);
}

function openCategoryPanel(title, record, isEdit) {

    //..initialize form fields
    $('#isCategoryEdit').val(isEdit);
    $('#categoryId').val(record.id);
    $('#categoryName').val(record.categoryName || '');
    $('#comments').val(record.comments || '');

    //..load dialog window
    closeAllPanels();
    $('#categoryTitle').text(title);
    $('#lawOverlay').addClass('active');
    $('#lawCategoryPanel').addClass('active');
    $('body').css('overflow', 'hidden');

}

function saveLawCategory(e) {
    e.preventDefault();

    let isEdit = $('#isEdit').val();
    let categoryName = $('#categoryName').val()?.trim();
    let comments = $('#comments').val().trim();

    let isValid = true;
    if (!categoryName) {
        highlightErrorField('#categoryName', true, 'Category name is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(categoryName)) {
        highlightErrorField('#categoryName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!comments) {
        highlightErrorField('#comments', true, 'Category comment field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(comments)) {
        highlightErrorField('#comments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return;
    }

    //..build record payload from form
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        categoryName: categoryName,
        comments: comments
    };

    //..validate required fields
    let errors = [];
    if (!recordData.categoryName)
        errors.push("Category name is required.");

    if (!recordData.comments)
        errors.push("Category comment is required.");

    if (errors.length > 0) {
        highlightErrorField("#categoryName", !categoryName);
        highlightErrorField("#comments", !recordData.comments);
        Swal.fire({
            title: "Category Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }


    //..call backend
    saveCategory(isEdit, recordData);
}

function saveCategory(isEdit, payload) {
    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/support/category-update"
        : "/grc/compliance/support/category-create";

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getCategoryToken()
        },
        success: function (res) {
            Swal.fire("Success", res.message || "Saved successfully.");
            closeAllPanels();

            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || (isEdit ? "Category updated successfully" : "Category created successfully"));
            closeAllPanels();

            // reload table
            regulatoryCategoryTable.replaceData();
        },
        error: function (xhr, status, error) {
            var errorMessage = error;

            try {
                var response = JSON.parse(xhr.responseText);
                if (response.message) {
                    errorMessage = response.message;
                }
            } catch (e) {
                // If parsing fails, use the default error
                errorMessage = "Unexpected error occurred";
            }

            Swal.fire(isEdit ? "Update Category" : "Save Category", errorMessage);
        }
    });
}

function closeLawCategory() {
    closeAllPanels();
    $('#lawCategoryPanel').removeClass('active');
}

$("#btnAddLaw").on("click", function () {

    if (!selectedCategory) {
        toastr.error(res?.message || "No category selected");
        return;
    }

    addLaw({ categoryId: selectedCategory });
});

function addLaw(category) {
    const categoryId = category.categoryId;
    openLawPanel('New Regulation/Law', {
        id: 0,
        lawCode: '',
        lawName: '',
        comments: '',
        typeId: 0,
        authorityId: 0,
        isActive: false,
        categoryId: categoryId,
    }, false);
}

function viewLaw(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving law/regulation...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findLaw(id)
        .then(record => {
            Swal.close();
            if (record) {
                openLawPanel('Edit law/regulation', record, true);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Law/Regulation not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            console.error('Error loading law/regulation:', error);
            Swal.close();

            Swal.fire({
                title: 'Error',
                text: 'Failed to load law/regulation details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findLaw(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/laws-retrieve/${id}`,
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

function openLawPanel(title, record, isEdit) {

    //..initialize form fields
    $('#isLawEdit').val(isEdit);
    $('#lawId').val(record.id);
    $('#lawReference').val(record.lawCode || '');
    $('#lawName').val(record.lawName || '');
    $('#lawCategoryId').val(record.categoryId || 0);
    $('#typeId').val(record.typeId || 0).trigger('change');
    $('#authorityId').val(record.authorityId || 0).trigger('change');

    //..load dialog window
    closeAllPanels();
    $('#lawTitle').text(title);
    $('#lawOverlay').addClass('active');
    $('#lawRegulationPanel').addClass('active');
    $('body').css('overflow', 'hidden');

}

function saveLaw(e) {
    e.preventDefault();

    let isEdit = $('#isLawEdit').val();
    let categoryId = $('#lawCategoryId').val() || 0;
    let lawName = $('#lawName').val()?.trim();
    let lawCode = $('#lawReference').val().trim();

    let isValid = true;
    if (!lawName) {
        highlightErrorField('#lawName', true, 'Law name is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(lawName)) {
        highlightErrorField('#lawName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!lawCode) {
        highlightErrorField('#lawCode', true, 'Law reference field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(lawCode)) {
        highlightErrorField('#lawCode', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return;
    }
    //..build record payload from form
    let recordData = {
        id: Number($('#lawId').val()) || 0,
        lawCode: lawCode,
        lawName: lawName,
        typeId: Number($('#typeId').val()),
        authorityId: Number($('#authorityId').val()),
        categoryId: Number(categoryId),
        isDeleted: false
    };

    //..validate required fields
    let errors = [];
    if (!recordData.lawCode)
        errors.push("Law/Regulatory code is required.");

    if (!recordData.lawName)
        errors.push("Law/Regulatory name is required.");

    if (recordData.categoryId === 0)
        errors.push("Law/Regulatory category ID is required.");

    if (recordData.authorityId === 0)
        errors.push("Law/Regulatory issueing authority is required.");

    if (recordData.typeId === 0)
        errors.push("Law/Regulatory document type ID is required.");

    if (errors.length > 0) {
        highlightErrorField("#lawReference", !recordData.lawCode);
        highlightErrorField("#lawName", !recordData.lawName);
        Swal.fire({
            title: "Law/Regulation Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveLawRecord(isEdit, recordData);
}

function saveLawRecord(isEdit, payload) {
    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/register/laws-update"
        : "/grc/compliance/register/laws-create";

    Swal.fire({
        title: isEdit ? "Updating law/regulation..." : "Saving law/regulation...",
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
            'X-CSRF-TOKEN': getCategoryToken()
        },
        success: function (res) {

            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || (isEdit ? "Law/Regulation updated successfully" : "Law/Regulation created successfully"));
            closeAllPanels();

            // reload table
            lawsTable.replaceData();
        },
        error: function (xhr, status, error) {
            var errorMessage = error;

            try {
                var response = JSON.parse(xhr.responseText);
                if (response.message) {
                    errorMessage = response.message;
                }
            } catch (e) {
                // If parsing fails, use the default error
                errorMessage = "Unexpected error occurred";
            }

            Swal.fire(isEdit ? "Update Law/Regulation" : "Save Law/Regulation", errorMessage);
        }
    });
}

function closeLaw() {
    closeAllPanels();
    $('#lawRegulationPanel').removeClass('active');
}

$("#btnAddAct").on("click", function () {

    if (!selectedLaw) {
        toastr.error(res?.message || "No regulation or law selected");
        return;
    }

    addAct({ id: selectedLaw });
});

function addAct(law) {
    let lawId = law.id
    openActPanel('New Act/Section', {
        id: 0,
        statutoryId: lawId,
        section: '',
        summery: '',
        obligation: '',
        frequencyId: 0,
        ownerId: 0,
        isMandatory: true,
        coverage: 0,
        isCovered: false,
        exclude: false,
        assurance: 0,
        isDeleted: false,
        comments: ''
    }, false);
}

function viewSection(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving Act/Section...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findSection(id)
        .then(record => {
            Swal.close();
            if (record) {
                openActPanel('Edit Act/Section', record, true);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Act/Section not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Failed to load Act/Section details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findSection(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/acts-retrieve/${id}`,
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

function openActPanel(title, record, isEdit) {
    let coverageValue = record.coverage ?? 0;
    let assuranceValue = record.assurance ?? 0;

    //..initialize form fields
    $('#isActEdit').val(isEdit);
    $('#actId').val(record.id);
    $('#actLawId').val(record.statutoryId || '');
    $('#section').val(record.section || '');
    $('#summery').val(record.summery || '');
    $('#obligation').val(record.obligation || '');
    $('#assurance').val(assuranceValue);
    $('#assuranceValue').text(assuranceValue + '%');
    $('#isMandatory').prop('checked', record.isMandatory);
    $('#coverage').val(coverageValue);
    $('#coverageValue').text(coverageValue + '%');
    $('#actComments').val(record.comments || '');
    $('#actFrequencyId').val(record.frequencyId || 0).trigger('change');
    $('#actResponsibleId').val(record.ownerId || 0).trigger('change');

    //..load dialog window
    closeAllPanels();
    $('#actTitle').text(title);
    $('#lawOverlay').addClass('active');
    $('#actPanel').addClass('active');
    $('body').css('overflow', 'hidden');

}

function normalize2Date(value) {
    if (!value) return null;

    const d = new Date(value);
    return isNaN(d) ? null : d;
}

function saveAct(e) {
    e.preventDefault();
    let isEdit = $('#isActEdit').val();
    let lawId = Number($('#actLawId').val()) || 0;
    let coverage = Number($('#coverage').val()) || 0;
    let assurance = Number($('#assurance').val()) || 0;

    let section = $('#section').val()?.trim();
    let summery = $('#summery').val().trim();
    let obligation = $('#obligation').val()?.trim();
    let actComments = $('#actComments').val().trim();

    let isValid = true;
    if (!section) {
        highlightErrorField('#section', true, 'Law section/act field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9.(),\s]*$/.test(section)) {
        highlightErrorField('#section', true, 'Only letters, numbers, commas, periods, brackets and spaces allowed');
        isValid = false;
    }

    if (!summery) {
        highlightErrorField('#summery', true, 'Law/act summery field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(summery)) {
        highlightErrorField('#summery', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!obligation) {
        highlightErrorField('#obligation', true, 'Law/act obligation field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(obligation)) {
        highlightErrorField('#obligation', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!actComments) {
        highlightErrorField('#actComments', true, 'Law/act comment field is required');
        isValid = false;
    } else if (!/^[a-zA-Z0-9\s,.]*$/.test(actComments)) {
        highlightErrorField('#actComments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!assurance && (!/^[a-zA-Z0-9\s,.]*$/).test(assurance)) {
        highlightErrorField('#assurance', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        isValid = false;
    }

    if (!isValid) {
        //..stop submission
        return;
    }
    //..build record payload from form
    let recordData = {
        id: Number($('#actId').val()) || 0,
        statutoryId: lawId,
        section: section,
        summery: summery,
        obligation: obligation,
        isMandatory: $('#isMandatory').is(':checked') ? true : false,
        frequencyId: Number($('#actFrequencyId').val() || 0),
        ownerId: Number($('#actResponsibleId').val() || 0),
        coverage: coverage,
        isCovered: coverage === 100,
        isDeleted: false,
        exclude: false,
        assurance: assurance,
        comments: actComments,
    };

    //..validate required fields
    let errors = [];

    if (!recordData.section)
        errors.push("Act/Section section number is required.");

    if (!recordData.summery)
        errors.push("Act/Section summery is required.");

    if (!recordData.obligation)
        errors.push("Act/Section obligation is required.");

    if (recordData.statutoryId === 0)
        errors.push("Act/Section Law/Regulation ID is required.");

    if (!recordData.comments)
        errors.push("Act/Section comments is required.");


    if (errors.length > 0) {
        highlightErrorField("#section", !recordData.section);
        highlightErrorField("#summery", !recordData.summery);
        highlightErrorField("#obligation", !recordData.obligation);
        highlightErrorField("#actComments", !recordData.comments);
        Swal.fire({
            title: "Act/Section Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveSectionRecord(isEdit, recordData);
}

function saveSectionRecord(isEdit, payload) {

    let url = isEdit === true || isEdit === "true"
        ? "/grc/compliance/register/acts-update"
        : "/grc/compliance/register/acts-create";

    Swal.fire({
        title: isEdit ? "Updating Act/Section..." : "Saving Act/Section...",
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
            'X-CSRF-TOKEN': getCategoryToken()
        },
        success: function (res) {

            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || (isEdit ? "Act/Section updated successfully" : "Act/Section created successfully"));
            closeAllPanels();

            // reload table
            actsTable.replaceData();
        },
        error: function (xhr, status, error) {
            var errorMessage = error;

            try {
                var response = JSON.parse(xhr.responseText);
                if (response.message) {
                    errorMessage = response.message;
                }
            } catch (e) {
                // If parsing fails, use the default error
                errorMessage = "Unexpected error occurred";
            }

            Swal.fire(isEdit ? "Update Act/Section" : "Save Act/Section", errorMessage);
        }
    });
}

function closeLawAct() {
    closeAllPanels();
    $('#actPanel').removeClass('active');
}

function closeAllPanels() {
    $('.law-side-panel').removeClass('active');
    $('#lawOverlay').removeClass('active');
    $('body').css('overflow', '');
}

//..close panel handlers
$('#lawOverlay').on('click', function () {
    closeAllPanels();
});

$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

//..close panel on Escape key
$(document).on('keydown', function (e) {
    if (e.key === 'Escape') {
        closeAllPanels();
    }
})

//..name input validation
function validateNameInput(event) {
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
        highlightErrorField('#categoryName', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightErrorField('#categoryName', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightErrorField('#categoryName', false);
        return true;
    }

}

function handleNamePaste(event) {
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
        highlightErrorField('#categoryName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightErrorField('#categoryName', false);
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
        highlightErrorField('#comments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightErrorField('#comments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightErrorField('#comments', false);
        return true;
    }

}

function handleCommentPaste(event) {
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
        highlightErrorField('#comments', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightErrorField('#comments', false);
    }
}

//..name input validation
function validateLawInput(event) {
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
        highlightErrorField('#lawReference', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightErrorField('#lawReference', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightErrorField('#lawReference', false);
        return true;
    }

}

function handleLawPaste(event) {
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
        highlightErrorField('#lawName', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightErrorField('#lawName', false);
    }
}

//..comments input validation
function validateLawNameInput(event) {
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
        highlightErrorField('#comments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightErrorField('#comments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightErrorField('#comments', false);
        return true;
    }

}

function handleLawNamePaste(event) {
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
        highlightErrorField('#comments', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightErrorField('#comments', false);
    }
}

//..section input validation
function validateSectionInput(event) {
    var key = event.keyCode || event.which;
    var keyChar = String.fromCharCode(key);

    //..allow backspace, tab, enter, delete, arrows, etc.
    if (key == 8 || key == 9 || key == 13 || key == 46 ||
        key == 37 || key == 39 || (key >= 35 && key <= 40)) {
        return true;
    }

    //..allow alphanumeric (a-z, A-Z, 0-9)
    if (/[^a-zA-Z0-9.,()\s]/.test(keyChar)) {
        // Clear any existing error when user starts typing valid chars
        highlightErrorField('#section', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightErrorField('#section', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightErrorField('#section', false);
        return true;
    }

}

function handleSectionPaste(event) {
    event.preventDefault();

    // Get pasted content
    var pastedText = (event.clipboardData || window.clipboardData).getData('text');

    // Clean the pasted content - remove any disallowed characters
    var cleanedText = pastedText.replace(/[^a-zA-Z0-9.,()\s]/g, '');

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
        highlightErrorField('#section', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightErrorField('#section', false);
    }
}

//..summery input validation
function validateSummeryInput(event) {
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
        highlightErrorField('#summery', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightErrorField('#summery', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightErrorField('#summery', false);
        return true;
    }

}

function handleSummeryPaste(event) {
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
        highlightErrorField('#summery', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightErrorField('#summery', false);
    }
}

//..obligation input validation
function validateObligationInput(event) {
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
        highlightErrorField('#obligation', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightErrorField('#obligation', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightErrorField('#obligation', false);
        return true;
    }

}

function handleObligationPaste(event) {
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
        highlightErrorField('#obligation', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightErrorField('#obligation', false);
    }
}

//..act ocomments input validation
function validateActCommentsInput(event) {
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
        highlightErrorField('#actComments', false);
        return true;
    }

    //..allow commas and periods
    if (keyChar == ',' || keyChar == '.') {
        highlightErrorField('#actComments', false);
        return true;
    }

    //..allow space
    if (keyChar == ' ') {
        highlightErrorField('#actComments', false);
        return true;
    }

}

function handleActCommentPaste(event) {
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
        highlightErrorField('#actComments', true, 'Some characters were removed - only letters, numbers, commas, periods, and spaces allowed');
    } else {
        highlightErrorField('#actComments', false);
    }
}

function init2Dates() {

    flatpickr1Instances["submissionDate"] = flatpickr("#submissionDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

}

$(document).ready(function () {
    loadRegulatoryTree();
    init2Dates();

    //console.log(window.userPermissions);

    $('#typeId, #authorityId, #actFrequencyId').select2({
        width: '100%',
        dropdownParent: $('#lawRegulationPanel')
    });

    $('#actFrequencyId, #actResponsibleId, #interval, #intervalType').select2({
        width: '100%',
        dropdownParent: $('#actPanel')
    });

    $('#categoryForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#lawForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#actForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('#coverage').on('input', function () {
        $('#coverageValue').text(this.value + '%');
    });

    $('#assurance').on('input', function () {
        $('#assuranceValue').text(this.value + '%');
    });

    //..category name validation 
    $('#categoryName').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightErrorField('#categoryName', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#categoryName', true, 'Invalid characters detected');
        } else {
            highlightErrorField('#categoryName', false);
        }
    });

    $('#categoryName').on('focus', function () {
        highlightErrorField('#categoryName', false);
    });

    $('#categoryName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightErrorField('#categoryName', true, 'Category name is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#categoryName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightErrorField('#categoryName', false);
        }
    });

    $('#categoryName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightErrorField('#categoryName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightErrorField('#categoryName', false);
                }, 2000);
            }
        }
    });

    //..comments validation 
    $('#comments').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightErrorField('#comments', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#comments', true, 'Invalid characters detected');
        } else {
            highlightErrorField('#comments', false);
        }
    });

    $('#comments').on('focus', function () {
        highlightErrorField('#comments', false);
    });

    $('#comments').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightErrorField('#comments', true, 'Comments field is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#comments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightErrorField('#comments', false);
        }
    });

    $('#comments').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightErrorField('#comments', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightErrorField('#comments', false);
                }, 2000);
            }
        }
    });

    //..law reference validation 
    $('#lawReference').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightErrorField('#lawReference', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#lawReference', true, 'Invalid characters detected');
        } else {
            highlightErrorField('#lawReference', false);
        }
    });

    $('#lawReference').on('focus', function () {
        highlightErrorField('#lawReference', false);
    });

    $('#lawReference').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightErrorField('#lawReference', true, 'Law reference is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#lawReference', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightErrorField('#lawReference', false);
        }
    });

    $('#lawReference').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightErrorField('#lawReference', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightErrorField('#lawReference', false);
                }, 2000);
            }
        }
    });

    //..law name validation 
    $('#lawName').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightErrorField('#lawName', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#lawName', true, 'Invalid characters detected');
        } else {
            highlightErrorField('#lawName', false);
        }
    });

    $('#lawName').on('focus', function () {
        highlightErrorField('#lawName', false);
    });

    $('#lawName').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightErrorField('#lawName', true, 'Law name field is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#lawName', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightErrorField('#lawName', false);
        }
    });

    $('#lawName').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightErrorField('#lawName', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightErrorField('#lawName', false);
                }, 2000);
            }
        }
    });

    //..section validation 
    $('#section').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightErrorField('#section', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9.()]+$/.test(value)) {
            highlightErrorField('#section', true, 'Invalid characters detected');
        } else {
            highlightErrorField('#section', false);
        }
    });

    $('#section').on('focus', function () {
        highlightErrorField('#section', false);
    });

    $('#section').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightErrorField('#section', true, 'Law section field is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#section', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightErrorField('#section', false);
        }
    });

    $('#section').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightErrorField('#section', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightErrorField('#section', false);
                }, 2000);
            }
        }
    });

    //..summery validation 
    $('#summery').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightErrorField('#summery', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#summery', true, 'Invalid characters detected');
        } else {
            highlightErrorField('#summery', false);
        }
    });

    $('#summery').on('focus', function () {
        highlightErrorField('#summery', false);
    });

    $('#summery').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightErrorField('#summery', true, 'Law summery field is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#summery', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightErrorField('#summery', false);
        }
    });

    $('#summery').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightErrorField('#summery', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightErrorField('#summery', false);
                }, 2000);
            }
        }
    });

    //..obligation validation 
    $('#obligation').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightErrorField('#obligation', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#obligation', true, 'Invalid characters detected');
        } else {
            highlightErrorField('#obligation', false);
        }
    });

    $('#obligation').on('focus', function () {
        highlightErrorField('#obligation', false);
    });

    $('#obligation').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightErrorField('#obligation', true, 'Obligations field is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#obligation', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightErrorField('#obligation', false);
        }
    });

    $('#obligation').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightErrorField('#obligation', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightErrorField('#obligation', false);
                }, 2000);
            }
        }
    });

    //..act comments validation 
    $('#actComments').on('keyup', function () {
        var value = $(this).val();

        // Clear error if field is empty
        if (!value) {
            highlightErrorField('#actComments', false);
            return;
        }

        // Show real-time feedback but don't block typing
        if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#actComments', true, 'Invalid characters detected');
        } else {
            highlightErrorField('#actComments', false);
        }
    });

    $('#actComments').on('focus', function () {
        highlightErrorField('#actComments', false);
    });

    $('#actComments').on('blur', function () {
        var value = $(this).val().trim();

        if (!value) {
            highlightErrorField('#actComments', true, 'Comments field is required');
        } else if (!/^[a-zA-Z0-9\s,.]*$/.test(value)) {
            highlightErrorField('#actComments', true, 'Only letters, numbers, commas, periods, and spaces allowed');
        } else {
            highlightErrorField('#actComments', false);
        }
    });

    $('#actComments').on('blur', function () {
        var value = $(this).val();
        if (value) {
            var cleaned = value.replace(/[^a-zA-Z0-9\s,.]/g, '');
            if (value !== cleaned) {
                $(this).val(cleaned);
                highlightErrorField('#actComments', true, 'Removed invalid characters');

                // Clear error after 2 seconds
                setTimeout(function () {
                    highlightErrorField('#actComments', false);
                }, 2000);
            }
        }
    });

});

//..get antiforegery token from meta tag
function getCategoryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightErrorField(selector, hasError, message) {
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

function coverageIconFormatter(cell) {
    const isCovered = cell.getValue();

    if (isCovered === true) {
        return `<i class="mdi mdi-checkbox-marked-circle text-success"></i>`;
    }

    if (isCovered === false) {
        return `<i class="mdi mdi-close-circle text-danger"></i>`;
    }

    return "-";
}

function progressFormatter(cell) {
    let value = Number(cell.getValue()) || 0;

    let color = "#FF3A1A";
    if (value >= 80) color = "#25C756";
    else if (value >= 50) color = "#FF9A17";

    return `
        <div class="progress" style="height:16px;">
            <div class="progress-bar"
                 role="progressbar"
                 style="width:${value}%; background-color:${color};">
                ${value}%
            </div>
        </div>
    `;
}

function percentFormatter(cell) {
    const value = cell.getValue();
    return value !== null && value !== undefined
        ? `${value}%`
        : "-";
}