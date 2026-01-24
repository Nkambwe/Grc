//..route to home
let auditReportTable;

function initAuditExceptionTable() {
    auditReportTable = new Tabulator("#auditExceptionTable", {
        ajaxURL: "/grc/compliance/audit/exceptions",
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
                        ["finding", "recomendations", "proposedAction", "correctiveAction", "riskLevel"].includes(f.field)
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
            alert("Failed to load audit exceptions. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            { title: "REF-NO", field: "reference", width: 200 },
            {
                title: "FINDING",
                field: "finding",
                headerFilter: "input",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewAuditException(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "RECOMMENDATION", field: "recomendations", minWidth: 200, widthGrow: 3, headerFilter: "input" },
            { title: "EXCUTIONER", field: "excutioner", minWidth: 200, widthGrow: 3, headerFilter: "input" },
            {
                title: "RISK LEVEL",
                field: "riskLevel",
                headerFilter: "select",
                headerFilterParams: {
                    values: {
                        "NONE": "All",
                        "LOW": "Low",
                        "MEDIUM": "Medium",
                        "HIGH": "High"
                    }
                },
                minWidth: 200,
                widthGrow: 3,
                formatter: function (cell) {
                    const value = (cell.getValue() || "").toUpperCase();
                    const el = cell.getElement();

                    let bg = "#FF2413";
                    let fg = "#fff";
                    switch (value) {
                        case "NONE":
                            bg = "#adb5bd"; 
                            fg = "#000";
                            break;
                        case "LOW":
                            bg = "#198754"; 
                            break;
                        case "MEDIUM":
                            bg = "#fd7e14"; 
                            break;
                        case "HIGH":
                            bg = "#dc3545"; 
                            break;
                    }

                    el.style.backgroundColor = bg;
                    el.style.color = fg;
                    el.style.fontWeight = "600";

                    return value || "-";
                }
            },
            {
                title: "RISK RATING",
                field: "riskRating",
                headerFilter: "input",
                minWidth: 200,
                widthGrow: 3,
                hozAlign: "center",
                formatter: function (cell) {
                    const value = Number(cell.getValue());
                    const el = cell.getElement();

                    let bg = "#198754"; 
                    let fg = "#fff";

                    if (value >= 50) {
                        bg = "#dc3545"; 
                    } else if (value >= 30) {
                        bg = "#fd7e14"; 
                    }

                    el.style.backgroundColor = bg;
                    el.style.color = fg;
                    el.style.fontWeight = "700";

                    return isNaN(value) ? "-" : value;
                }
            }
        ]
    });

    //..initialize search
    initAuditExceptionSearch();
}

function viewAuditException(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving Exception Report...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findView(id)
        .then(record => {
            console.log(`Then >> `, record);
            Swal.close();
            if (record) {
                openViewPanel(record);
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Exception record not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            console.error('Error loading exception record :', error);
            Swal.close();

            Swal.fire({
                title: 'Error',
                text: 'Failed to load exception record  details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findView(id) {
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
            }
        });
    });
}

function openViewPanel(record) {
    $('#reference').val(record.reference || '');
    $('#findings').val(record.findings || '');
    $('#recomendations').val(record.recomendations || '');
    $('#proposedAction').val(record.proposedAction || '');
    $('#correctiveAction').val(record.correctiveAction || '');
    $('#executioner').val(record.executioner || '');
    $('#isDeleted').prop('isDeleted', record.sendReminders).trigger('change');
    $('#execptionStatus').val(record.status || '');
    $('#targetDate').val(record.targetDate || '');
    $('#riskLevel').val(record.riskLevel || '');
    $('#notes').val(record.notes || '');
   
    $('#exceptionViewPanel').addClass('active');
    $('#exceptionViewOverlay').addClass('active');
}

function initAuditExceptionSearch() {

}

function closeViewException() {
    $('#exceptionViewPanel').removeClass('active');
    $('#exceptionViewOverlay').removeClass('active');
    $('body').css('overflow', '');
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
    console.log("DOM Loged");
   initAuditExceptionTable();
    $('#exceptionViewForm').on('submit', function (e) {
        e.preventDefault();
    });
});

function getExcToken() {
    return $('meta[name="csrf-token"]').attr('content');
}