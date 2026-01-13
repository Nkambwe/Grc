let selectedFrequency = null;
let selectedReturn = null;

function loadFrequencyTree() {
    const request = {
        activityTypeId: 0,
        searchTerm: "",
        pageIndex: 0,
        pageSize: 50,
        sortBy: "",
        sortDirection: "ASC"
    };

    $.ajax({
        url: "/grc/returns/compliance-returns/frequency-returns",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(request), 
        success: function (res) {
            //..destroy previous tree if exists
            if ($('#frequencyTree').jstree(true)) {
                $('#frequencyTree').jstree("destroy");
            }

            // Initialize jsTree
            $('#frequencyTree').jstree({
                core: {
                    data: res,
                    multiple: false,
                    themes: { dots: false, icons: false }
                },
                plugins: ["types"],
                types: {
                    frequency: { icon: "jstree-folder" },
                    report: { icon: "jstree-file" }
                }
            }).on("select_node.jstree", function (e, data) {
                const tree = $(this).jstree(true);
                const node = data.node;

                //..expand node if it has children
                if (node.children.length > 0) {
                    tree.toggle_node(node);
                }

                //..frequency logic
                if (node.type === "frequency") {
                    selectedFrequency = parseInt(node.id.replace("C_", ""));
                    selectedReturn = null;

                    $("#returnView").removeClass("d-none");
                    $("#submissionView").addClass("d-none");
                    $("#frequencyBreadcrumb").html(`<li class="breadcrumb-item active">${node.text}</li>`);

                    reportsTable.setData();
                    showFrequencyBreadcrubs(node.text);
                    loadReturns(selectedFrequency);
                }

                //..feport logic
                if (node.type === "report") {

                    selectedReturn = parseInt(node.id.replace("L_", ""));
                    selectedFrequency = parseInt(node.parent.replace("C_", ""));
                    showReturnView(node.text);
                    loadSubmissions(selectedReturn);
                }
            });
        },
        error: function (xhr, status, error) {
            console.error("Error loading tree:", error);
            console.error("Response:", xhr.responseText);
        }

    });

}


let reportsTable = new Tabulator("#returnsTable", {
    ajaxURL: "/grc/returns/compliance-returns/returns-list",
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
                activityTypeId: selectedFrequency,
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
        Swal.fire("Failed to load returns list. Please try again.");
    },
    layout: "fitColumns",
    responsiveLayout: "hide",
    columns: [
        {
            title: "RETURN/REPORT TITLE",
            field: "reportName",
            widthGrow: 4,
            minWidth: 280,
            headerSort: true,
            formatter: function (cell) {
                return `<span class="clickable-title" onclick="viewReturn(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
            }
        },
        { title: "ARTICLE/SECTION", field: "article", widthGrow: 3, minWidth: 250 },
        { title: "AUTHORITY", field: "authority", widthGrow: 1, minWidth: 150 },
        { title: "DEPARTMENT", field: "department", widthGrow: 1, minWidth: 200 },
        {
            title: "VIEW SUBMISSIONS",
            formatter: () => `<button class="btn btn-sm btn-link">SUBMISSIONS</button>`,
            cellClick: function (e, cell) {
                let report = cell.getRow().getData();
                selectedReturn = report.id;
                showReturnView(report.reportName);
                loadSubmissions(report.id);
            }
        }
    ]
});


let submissionsTable = new Tabulator("#submissionsTable", {
    ajaxURL: "/grc/returns/compliance-returns/return-submissions",
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
                activityTypeId: selectedReturn,
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
        {
            title: "Report Title",
            field: "reportTitle",
            widthGrow: 4,
            minWidth: 280,
            formatter: function (cell) {
                const id = cell.getRow().getData().id;
                return `<span class="clickable-title" onclick="viewSubmission(${id})">${cell.getValue()}</span>`;
            }
        },
        {
            title: "PERIOD(FROM)",
            field: "periodStart",
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
            title: "PERIOD(TO)",
            field: "periodEnd",
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
            title: "STATUS",
            field: "status",
            formatter: function (cell) {
                const cellEl = cell.getElement();

                let rowData = cell.getRow().getData();
                let text = rowData.status;
                let bg = "#FF2413";
                let clr = "#FFFFFF";
                if (text === "OPEN") {
                    bg = "#FF8503";
                } else if (text === "CLOSED") {
                    bg = "#09B831";
                } else {
                    bg = "#FF2413";
                }
                cellEl.style.backgroundColor = bg;
                cellEl.style.color = clr;
                cellEl.style.fontWeight = "bold";
                cellEl.style.textAlign = "center";

                return text;
            },
            hozAlign: "center",
            headerHozAlign: "center",
            maxWidth: 250
        },
        {
            title: "SUBMITTED ON",
            field: "submittedOn",
            widthGrow: 2,
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
            title: "SUBMITTED BY",
            field: "department",
            widthGrow: 2,
            minWidth: 150
        },
        {
            title: "BREACH RISK",
            field: "risk",
            widthGrow: 4,
            minWidth: 280
        }

    ]

});

$('#frequencyTree').on("select_node.jstree", function (e, data) {

    let node = data.node;
    const tree = $('#frequencyTree').jstree(true);
    if (data.node.children.length > 0) {
        tree.toggle_node(data.node);
    }

    console.log("Node:", node);
    console.log("Node type:", node.type);
    console.log("Original:", node.original);

    if (node.type === "frequency") {
        selectedFrequency = parseInt(node.id.replace("C_", ""));
        selectedReturn = null;

        console.log(`1.Selected Frequency ${selectedFrequency}`);

        // Show category view
        $("#returnView").removeClass("d-none");
        $("#submissionView").addClass("d-none");

        $("#frequencyBreadcrumb").html(`<li class="breadcrumb-item active">${node.text}</li>`);

        reportsTable.setData();

        showFrequencyBreadcrubs(node.text);
        loadReturns(selectedFrequency);
    }

    if (node.type === "report") {
        selectedReturn = parseInt(node.id.replace("L_", ""));
        selectedFrequency = parseInt(node.parent.replace("C_", ""));

        console.log(`1.Selected Frequency ${selectedFrequency}`);
        console.log(`1.Selected Return ${selectedReturn}`);
        showReturnView(node.text);
        loadSubmissions(selectedLaw);
    }
});

function loadReturns(frequencyId) {
    selectedFrequency = frequencyId;
    $("#returnView").removeClass("d-none");
    $("#submissionView").addClass("d-none");
    reportsTable.setData();
}

function showReturnView(title) {
    $("#frequencyBreadcrumb").append(`<li class="breadcrumb-item active">${title}</li>`);
}

function loadSubmissions(lawId) {
    selectedReturn = lawId;
    $("#returnView").addClass("d-none");
    $("#submissionView").removeClass("d-none");
    submissionsTable.setData();
}

function showFrequencyBreadcrubs(frequencyName) {
    $("#frequencyBreadcrumb").html(`<li class="breadcrumb-item active">${frequencyName}</li>`);
}

function viewReturn(id) {
    alert(`Selected ID ${id}`);
}

$('#btnAddReturn').on('click', function () {
    alert(`Add Report to ${selectedFrequency}`);
});

function viewSubmission(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving Return/Report...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    findReturnSubmission(id)
        .then(record => {
            Swal.close();
            if (record) {
                try {
                    openSubmissionPanel(record);
                } catch (err) {
                    console.error("openSubmissionPanel failed:", err);
                    throw err; 
                }
            } else {
                Swal.fire({
                    title: 'NOT FOUND',
                    text: 'Return/Report not found',
                    confirmButtonText: 'OK'
                });
            }
        })
        .catch(error => {
            Swal.close();
            Swal.fire({
                title: 'Error',
                text: 'Failed to load Return/Report details. Please try again.',
                confirmButtonText: 'OK'
            });
        });
}

function findReturnSubmission(id) {

    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/returns/compliance-returns/submissions/retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {

                console.log(`Response >> `, response);
                if (response.success && response.data) {
                    resolve(response.data);
                } else {
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                console.log(`Error >> `, error);
                reject(error);
            }
        });
    });
}

function closeSubmissionPane() {
    $('#returnInnerOverlay').removeClass('active');
    $('#returnInnerPanel').removeClass('active');
}

function openSubmissionPanel(record) {

    $('#submissionId').val(record.id);
    $('#title').val(record.title || '');
    $('#period').val(record.period || '');
    $('#ownerId').val(record.period || '0');
    $('#status').val(record.status || 'UNKNOWN').trigger('change');
    $('#isBreached').prop('checked', record.isBreached);
    $('#submissionBreach').val(record.isBreached ? 'YES' : 'NO');
    $('#riskAttached').val(record.riskAttached || '');
    $('#comments').val(record.comments || '');
    $('#reason').val(record.reason || '');
    $('#file').val(record.file || '');
    $('#department').val(record.department || '');
    $('#submittedBy').val(record.submittedBy || '');

    if (record.isBreached) {
        $('#breachBox').show();
        $('#riskAttached').addClass('breach-marker');
        $('#period').addClass('breach-marker');
    }

    //..load dialog window
    closeSubmissionPane();
    $('#returnInnerOverlay').addClass('active');
    $('#returnInnerPanel').addClass('active');
    $('body').css('overflow', 'hidden');
}

function updateSubmission(e) {
    e.preventDefault();

    //..build record payload from form
    let recordData = {
        id: Number($('#submissionId').val()) || 0,
        ownerId: Number($('#ownerId').val()) || 0,
        submissionBreach: $('#submissionBreach').val(),
        isBreached: $('#isBreached').is(':checked') ? true : false,
        comments: $('#comments').val(),
        reason: $('#reason').val(),
        file: $('#file').val(),
        status: $('#status').val() || 'UNKNOWN',
        submittedBy: $('#submittedBy').val(),
        submittedOn: $('#submittedOn').val()
    };

    console.log(recordData);

    //..validate required fields
    let errors = [];
    if (recordData.submissionBreach === 'YES') {
        if (!recordData.reason)
            errors.push("Reason for breach MUST be provided.");
    }

    if (!recordData.status || recordData.status === "UNKNOWN")
        errors.push("You must select the report status");

    if (!recordData.comments)
        errors.push("Provide a note for the submission");

    if (!recordData.submittedBy)
        errors.push("Provide name for person submitting the report");

    if (errors.length > 0) {
        highlightReturnSubmissionField("#submittedBy", !recordData.submittedBy);
        if (recordData.submissionBreach === 'YES') {
            if (!recordData.reason)
                highlightReturnSubmissionField("#reason", !recordData.reason);
        }

        highlightReturnSubmissionField("#comments", !recordData.comments);
        Swal.fire({
            title: "Submission Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveReturnSubmissionRecord(recordData);
}

function saveReturnSubmissionRecord(payload) {
    let url = "/grc/returns/compliance-returns/submissions/update";
    Swal.fire({
        title: "Updating submission...",
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
            'X-CSRF-TOKEN': getReturnSubmissionToken()
        },
        success: function (res) {
            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }

            Swal.fire(res.message || "Return/Report updated successfully")
                .then(() => {
                    closeSubmissionPane();
                    window.location.reload();
                });
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

            Swal.fire("Report/Return Submission Update", errorMessage);
        }
    });
}

let flatpickrInstances = {};

function initReturnSubmissionDate() {

    flatpickrInstances["submittedOn"] = flatpickr("#submittedOn", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

//..get antiforegery token from meta tag
function getReturnSubmissionToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function highlightReturnSubmissionField(selector, hasError, message) {
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
    loadFrequencyTree();
    initReturnSubmissionDate();
    //..hide breach box
    $('#breachBox').hide();

    $('#status').select2({
        width: '100%',
        dropdownParent: $('#returnInnerPanel')
    });

    $('.action-btn-complianceHome').on('click', function () {
        window.location.href = '/grc/compliance';
    });


    $('#innerSubForm').on('submit', function (e) {
        e.preventDefault();
    });


});

