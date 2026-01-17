//..route to home
let auditReportTable;

function initAuditTypeTable() {
    auditTypeTable = new Tabulator("#auditExceprionTable", {
        ajaxURL: "/grc/compliance/audit/types",
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
                        ["typeCode", "typeName", "description"].includes(f.field)
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
            alert("Failed to load audit types. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            { title: "TYPE CODE", field: "typeCode", minWidth: 200, frozen: true, headerSort: true },
            {
                title: "TYPE NAME",
                field: "typeName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewAuditType(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "DESCRIPTION", field: "description", minWidth: 200, widthGrow: 3 },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteAuditType(${rowData.id})">
                            <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                            <span>DELETE</span>
                        </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    //..initialize search
    initAuditTypeSearch();
}



$('.action-btn-audit-home').on('click', function () {
    try {
        window.location.href = '/grc/compliance/audit/dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

$(document).ready(function () {

    $('#auditForm').on('submit', function (e) {
        e.preventDefault();
    });

});


function getAuditTypeToken() {
    return $('meta[name="csrf-token"]').attr('content');
}