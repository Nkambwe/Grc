let selectedCategory = null;
let selectedLaw = null;

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
                    data: res,
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

                console.log("EVENT FIRED");
                console.log(node);

                // Expand node if it has children
                if (node.children.length > 0) {
                    tree.toggle_node(node);
                }

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
                    showCategoryView(node.text);
                    loadLaws(selectedCategory);
                }

                // Law logic
                if (node.type === "law") {
                    selectedLaw = parseInt(node.id.replace("L_", ""));
                    selectedCategory = parseInt(node.parent.replace("C_", ""));

                    showLawView(node.text);
                    loadActs(selectedLaw);
                }
            });
        },
        error: function (xhr, status, error) {
            console.error("Error loading tree:", error);
            console.error("Response:", xhr.responseText);
        }
    });
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
        Swal.fire("Failed to load regulatory list. Please try again.");
    },
    layout: "fitColumns",
    responsiveLayout: "hide",
    columns: [
        { title: "Law Name", field: "lawName", widthGrow: 4, minWidth: 280 },
        { title: "Code", field: "lawCode", width: 120 },
        { title: "Status", field: "status", width: 200 },
        {
            title: "Actions",
            formatter: () => `<button class="btn btn-sm btn-link">View Sections</button>`,
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
        Swal.fire("Failed to load regulatory acts. Please try again.");
    },
    layout: "fitColumns",
    responsiveLayout: "hide",
    columns: [
        { title: "Section", field: "sectionNumber", width: 120 },
        { title: "Title", field: "title", widthGrow: 4, minWidth: 280 },
        { title: "Mandatory", field: "isMandatory", formatter: "tickCross", width: 300 }
    ]
});


$('#regulatoryTree').on("select_node.jstree", function (e, data) {

    let node = data.node;
    const tree = $('#regulatoryTree').jstree(true);

    console.log(node);
    console.log(node.type);
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

        showCategoryView(node.text);
        loadLaws(selectedCategory);
    }

    if (node.type === "law") {
        selectedLaw = parseInt(node.id.replace("L_", ""));
        selectedCategory = parseInt(node.parent.replace("C_", ""));

        showLawView(node.text);
        loadActs(selectedLaw);
    }
});

function loadLaws(categoryId) {
    selectedCategory = categoryId;

    $("#categoryView").removeClass("d-none");
    $("#lawView").addClass("d-none");
    lawsTable.setData(); 
}

function loadActs(lawId) {
    selectedLaw = lawId;

    $("#lawView").removeClass("d-none");
    $("#categoryView").addClass("d-none");

    actsTable.setData();
}

function showCategoryView(categoryName) {
    $("#regulatoryBreadcrumb").html(`
        <li class="breadcrumb-item active">${categoryName}</li>
    `);
}

function showLawView(lawName) {
    $("#regulatoryBreadcrumb").append(`
        <li class="breadcrumb-item active">${lawName}</li>
    `);
}

$("#btnAddCategory").on("click", function () {
    addLawCategory();
});

function addLawCategory() {
    openCategoryPanel('New Regulatory Category', {
        id: 0,
        categoryName: '',
        comments: '',
        documentTypeId: 0,
    }, false);
}

function editLawCategory() {
    //..open edit panel
}

function openCategoryPanel(title, record, isEdit) {

    //..initialize form fields
    $('#isCategoryEdit').val(isEdit);
    $('#categoryId').val(record.id);
    $('#categoryName').val(record.categoryName || '');
    $('#categoryDescription').val(record.comments || '');

    //..load dialog window
    closeAllPanels();
    $('#categoryTitle').text(title);
    $('#lawOverlay').addClass('active');
    $('#lawCategoryPanel').addClass('active');
    $('body').css('overflow', 'hidden');

}

function saveLawCategory(e) {
    alert("Save Law Category clicked");
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
    console.log(`Category ID ${categoryId}`);

    openLawPanel('New Regulation/Law', {
        id: 0,
        code: '',
        lawName: '',
        comments: '',
        authorityId: 0,
        lawCategoryId: categoryId,
    }, false);
}

function editLaw() {
    //..open edit panel
}

function openLawPanel(title, record, isEdit) {

    //..initialize form fields
    $('#isEdit').val(isEdit);
    $('#lawId').val(record.id);
    $('#lawCategoryId').val(record.lawCategoryId || '');
    $('#lawReference').val(record.code || '');
    $('#lawName').val(record.lawName || '');
    $('#lawDescription').val(record.comments || '');

    //..load dialog window
    closeAllPanels();
    $('#lawTitle').text(title);
    $('#lawOverlay').addClass('active');
    $('#lawRegulationPanel').addClass('active');
    $('body').css('overflow', 'hidden');

}

function saveLaw(e) {
    alert("Save Law Category clicked");
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

    addAct({ id: selectedCategory });
});

function addAct(law) {
    let lawId = law.id
    console.log(`Law ID ${lawId}`);
    openActPanel('New Act/Section', {
        id: 0,
        actLawId: lawId,
        sectionNumber: '',
        section: '',
        obligation: '',
        isMandatory: false,
        Summery: '',
    }, false);
}

function openActPanel(title, record, isEdit) {

    //..initialize form fields
    $('#isActEdit').val(isEdit);
    $('#actId').val(record.id);
    $('#actLawId').val(record.actLawId || '');
    $('#sectionNumber').val(record.sectionNumber || '');
    $('#obligation').val(record.obligation || '');

    //..load dialog window
    closeAllPanels();
    $('#actTitle').text(title);
    $('#lawOverlay').addClass('active');
    $('#actPanel').addClass('active');
    $('body').css('overflow', 'hidden');

}

function saveAct(e) {
    alert("Save act/section clicked");
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

//..close panel on Escape key
$(document).on('keydown', function (e) {
    if (e.key === 'Escape') {
        closeAllPanels();
    }
})

$(document).ready(function () {
    loadRegulatoryTree();
});




