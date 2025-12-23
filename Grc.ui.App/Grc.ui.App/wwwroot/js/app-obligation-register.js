let obligationTable;

$(document).ready(function () {
    initObligationable();
});

function initObligationable() {
    obligationTable = new Tabulator("#obligations-table", {
        ajaxURL: "/grc/register/obligations/paged-list",
        paginationMode: "remote",
        filterMode: "remote",
        sortMode: "remote",
        dataTree: true,
        dataTreeChildField: "_children",
        dataTreeStartExpanded: false,
        dataTreeCollapseElement: "<span><i class='mdi mdi-chevron-down'></i></span>",
        dataTreeExpandElement: "<span><i class='mdi mdi-chevron-right'></i></span>",
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
                        ["requirement"].includes(f.field)
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
            const treeData = (response.data || []).map(category => ({
                rowType: "category",
                requirement: category.categoryName,
                coverage: "-",
                isCovered: "-",
                assurance: "-",
                issues: "-",
                _children: (category.laws || []).map(law => ({
                    rowType: "law",
                    requirement: law.lawName || "Unnamed Law",
                    coverage: law.coverage != null ? law.coverage + "%" : "",
                    isCovered: law.isCovered != null ? (law.isCovered ? "Yes" : "No") : "",
                    assurance: law.assurance != null ? law.assurance + "%" : "",
                    issues: law.issues != null ? law.issues : "0",
                    _children: (law.sections || []).map(section => ({
                        rowType: "section",
                        sectionId: section.sectionId,
                        requirement: section.requirement || `Section ${section.sectionId}`,
                        coverage: section.coverage + "%",
                        isCovered: section.isCovered ? "Yes" : "No",
                        assurance: section.assurance + "%",
                        issues: section.issues
                    }))
                }))
            }));

            return {
                data: treeData,
                last_page: response.last_page || 1,
                total_records: response.total_records || treeData.length
            };
        },
        ajaxError: function (error) {
            console.error("Tabulator AJAX Error:", error);
            alert("Failed to load obligation requirements. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "REQUIREMENT",
                field: "requirement",
                minWidth: 250,
                widthGrow: 2,
                formatter: function (cell) {
                    const rowData = cell.getRow().getData();

                    if (rowData.rowType === "section") {
                        return `<span class="clickable-title">${cell.getValue()}</span>`;
                    }

                    return cell.getValue();
                },
                cellClick: function (e, cell) {
                    const rowData = cell.getRow().getData();
                    console.log(rowData)
                    if (rowData.rowType === "section" && rowData.sectionId) {
                        viewRequirement(rowData.sectionId);
                    }
                }
            },
            {
                title: "COVERAGE",
                field: "coverage",
                minWidth: 150,
                formatter: percentTextFormatter,
                hozAlign: "left"
            },
            {
                title: "COMPLIANT",
                field: "isCovered",
                minWidth: 150,
                formatter: yesNoFormatter,
                hozAlign: "left"
            },
            {
                title: "ASSURANCE",
                field: "assurance",
                minWidth: 150,
                formatter: percentTextFormatter,
                hozAlign: "left"
            },
            {
                title: "ISSUES",
                field: "issues",
                minWidth: 200
            }
        ]
    });

    // Search init
    initObligationsearch();
}

$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

function initObligationsearch() {
    // Add your search implementation here
}

$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

function viewRequirement(id) {
    alert(`Selcted ID ${id}`);
}

function initObligationsearch() {

}

function percentTextFormatter(cell) {
    const value = cell.getValue();
    if (value === undefined || value === null) return "";
    return value;
}

function yesNoFormatter(cell) {
    const value = cell.getValue();
    if (value === "-" || value === "" || value == null) {
        return "";
    }

    //..show Yes/No
    return value ? "Yes" : "No";
}

