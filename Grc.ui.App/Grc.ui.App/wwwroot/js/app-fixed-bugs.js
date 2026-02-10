let bugsTable;

function initBugTable() {
   bugsTable = new Tabulator("#bugsTable", {
        ajaxURL: "/admin/configuration/bug-list",
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
            alert("Failed to load system errors. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                 formatter: () => `<span class="record-tab"></span>`,
                 width: 40,
                 headerSort: false,
                 frozen: true
             },
             {
                 title: "ERROR MESSAGE",
                 field: "error",
                 minWidth: 250,
                 frozen: true,
                 formatter: cell =>
                     `<span class="clickable-title" onclick="viewRecord(${cell.getRow().getData().id})">
                         ${cell.getValue()}
                     </span>`
             },
             {
                 title: "SEVERITY",
                 field: "severity",
                 hozAlign: "center",
                 headerFilter: "list",
                 headerHozAlign: "center",
                 headerFilterParams: {
                     values: {
                         "": "All",
                         "CRITICAL": "Critical",
                         "HIGH": "High",
                         "MEDIUM":"Medium",
                         "LOW": "Low"
                     }
                 },
                 formatter: cell =>
                     badgeFormatter(cell.getValue(), {
                         CRITICAL: "#dc2626",
                         HIGH: "#ea580c",
                         MEDIUM: "#ca8a04",
                         LOW: "#16a34a"
                 })
             },
             {
                 title: "STATUS",
                 field: "status",
                 hozAlign: "center",
                 headerFilter: "list",
                 headerHozAlign: "center",
                 headerFilterParams: {
                     values: {
                         "": "All",
                         "OPEN": "Open",
                         "PENDING": "In Progress",
                         "CLOSED": "Closed"
                     }
                 },
                 formatter: cell =>
                     badgeFormatter(cell.getValue(), {
                         OPEN: "#ef4444",
                         PENDING: "#f59e0b",
                         CLOSED: "#22c55e"
                     })
             },
             {
                 title: "REPORTED ON",
                 field: "createdOn",
                 hozAlign: "center",
                 headerHozAlign: "center",
                 formatter: cell => {
                     if (!cell.getValue()) return "";
                     return new Date(cell.getValue()).toLocaleDateString("en-GB");
                 }
             },
             {
                 title: "ACTION",
                 hozAlign: "center",
                 headerHozAlign: "center",
                 headerSort: false,
                 formatter: cell => `
                     <div style="display:flex; justify-content:center; align-items:center; height:100%;">
                        <button class="grc-table-btn grc-btn-view"
                                onclick="closeBug(${cell.getRow().getData().id})">
                            <i class="mdi mdi-check-circle-outline"></i> CLOSE
                        </button>
                    </div>`
             },
             { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });
}

function viewRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving error record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findError(id)
        .then(record => {
            Swal.close();
            if (record) {
                openErrorPane('Modify error record', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'System Error not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load system error details.' });
        });
}

function findError(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/configuration/bugs/retrieve/${id}`,
            type: "POST",
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

function openErrorPane(title, record, isEdit) {
    console.log(record);
    $('#isEdit').val(isEdit);
    $('#errorId').val(record?.id || 0);
    $('#error').val(record?.error || '');
    $('#stackTrace').val(record?.stackTrace || '');
    $('#severity').val(record?.severity || 'NONE').trigger('change');
    $('#status').val(record?.severity || 'NONE').trigger('change');
    $('#assignedTo').val(record?.assignedTo || '');
    $('#bugTitle').text(title);
    $('#bugOverLay').addClass('active');
    $('#bugContainer').addClass('active');
}

function closeBugPanel() {
    $('#bugOverLay').removeClass('active');
    $('#bugContainer').removeClass('active');
}

function saveError(e) {
    e.preventDefault();

    let isEdit = $('#isEdit').val() || false;
    const id = parseInt($('#errorId').val()) || 0;
    const errorData = {
        id: id,
        errorMessage: $('#error').val()?.trim(),
        stackTrace: $('#stackTrace').val()?.trim(),
        severity: $('#severity').val()?.trim(),
        assignedTo: $('#assignedTo').val()?.trim(),
        status: $('#status').val()?.trim()
    }

    let errors = [];
    if (!errorData.error)
        errors.push("Error message is required.");

    if (!errorData.stackTrace)
        errors.push("Stack trace field is required");

    if (!errorData.severity || errorData.severity === "NONE")
        errors.push("Error severity is required");

    if (!errorData.status || errorData.status === "NONE")
        errors.push("Error status is required");

    if (errors.length > 0) {
        bugErrorField("#error", !errorData.error);
        bugErrorField("#stackTrace", !errorData.stackTrace);
        bugErrorField("#severity", !errorData.severity || errorData.severity === "NONE");
        bugErrorField("#status", !errorData.status || errorData.status === "NONE");
        Swal.fire({
            title: "Bug data Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveBugRecord(isEdit, errorData);

}

function saveBugRecord(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/configuration/bugs/update-bug"
        : "/admin/configuration/bugs/create-bug";

    Swal.fire({
        title: isEdit ? "Updating issue record..." : "Saving issue record...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    console.log("Bug record >> ", record);
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getBugToken()
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

            Swal.fire(res.message || "Issue record saved successfully")
                .then(() => {
                    //..close panel
                    closeBugPanel();
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

function closeBug(id) {
    closeBugRecord({
        id: id,
        errorMessage: $('#error').val()?.trim(),
        stackTrace: $('#stackTrace').val()?.trim(),
        severity: $('#severity').val()?.trim(),
        assignedTo: $('#assignedTo').val()?.trim(),
        status: 'CLOSED'
    })
}

function closeBugRecord(record) {

    Swal.fire({
        title: "Close Issue",
        text: "Are you sure you want to close this issue?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Close",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {

        //..cancelled
        if (!result.isConfirmed) return;

        Swal.fire({
            title: "Close issue record...",
            text: "Please wait while we process your request.",
            allowOutsideClick: false,
            allowEscapeKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        console.log("Bug record >> ", record);
        $.ajax({
            url: "/admin/configuration/bugs/create-bug",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(record),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getBugToken()
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

                Swal.fire(res.message || "Issue closed successfully")
                    .then(() => {
                        //..close panel
                        closeBugPanel();
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
                    title: "Closure Failed",
                    text: errorMessage
                });
            }
        });
    });
}

function badgeFormatter(value, colorMap) {
    if (!value) return "";

    const color = colorMap[value.toUpperCase()] || "#6b7280";

    return `
        <span style="
            display:inline-flex;
            align-items:center;
            justify-content:center;
            min-width:80px;
            padding:4px 12px;
            border-radius:12px;
            background:${color};
            color:#fff;
            font-weight:600;
            font-size:12px;
            text-transform:capitalize;
            letter-spacing:.3px;
        ">
            ${value}
        </span>
    `;
}

function severityFormatter(cell) {
    return badgeFormatter(cell.getValue(), {
        CRITICAL: "#dc2626",
        HIGH: "#ea580c",
        MEDIUM: "#ca8a04",
        LOW: "#16a34a"
    });
}

function statusFormatter(cell) {
    return badgeFormatter(cell.getValue(), {
        OPEN: "#ef4444",
        PENDING: "#f59e0b",
        CLOSED: "#22c55e"
    });
}

function dateFormatter(cell) {
    const value = cell.getValue();
    if (!value) return "";

    const date = new Date(value);
    return date.toLocaleDateString("en-GB");
}

function errorFormatter(cell) {
    const data = cell.getRow().getData();
    return `
        <span class="clickable-title"
            onclick="viewRecord(${data.id})">
            ${cell.getValue()}
        </span>
    `;
}

function closeFormatter(cell) {
    const data = cell.getRow().getData();
    return `
        <button class="grc-table-btn grc-btn-view"onclick="closeBug(${data.id})">
            <i class="mdi mdi-check-circle-outline"></i> CLOSE
        </button>
    `;
}

function getBugToken() {
    return $('meta[name="csrf-token"]').attr('content');
}
                
$(document).ready(function () {

    initBugTable();
    $('#severity, #status').select2({
        width: '100%',
        dropdownParent: $('#bugPanel')
    });

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });

    $('.action-btn-email').on('click', function () {
        alert("Send Mail")
    });

    $('.action-btn-new').on('click', function () {
        openErrorPane("User Created Error",
            {
                id: 0,
                error: '',
                stackTrace: '',
                assignedTo:'',
                severity: 'NONE',
                status: 'OPEN'
            },
            false);
    });

    $('#bugForm').on('submit', function (e) {
        e.preventDefault();
    });

    $('.action-btn-bug-export').on('click', function () {
        $.ajax({
            url: '/admin/configuration/bug-export',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(userTable.getData()),
            xhrFields: { responseType: 'blob' },
            success: function (blob) {
                let link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = "Active_users.xlsx";
                link.click();
            },
            error: function () {
                toastr.error("Export failed. Please try again.");
            }
        });
    });

});

function bugErrorField(selector, hasError, message) {
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
