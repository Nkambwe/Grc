let circularTable;

function initCircularTable() {
    circularTable = new Tabulator("#circular-register-table", {
        ajaxURL: "/grc/returns/circular-returns/circular-register",
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
                        ["circularTitle", "authority", "status", "department"].includes(f.field)
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
            alert("Failed to load returns. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "REPORT/RETURN NAME",
                field: "circularTitle",
                headerFilter: "input",
                minWidth: 200,
                widthGrow: 4,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewCircular(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "AUTHORITY", field: "authority", minWidth: 200, frozen: true, headerSort: true, headerFilter: "input" },
            { title: "DEPARTMENT", field: "department", minWidth: 200, headerFilter: "input" },
            {
                title: "RECIEVE DATE",
                field: "recievedOn",
                headerFilter: "input",
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
                title: "DEADLINE",
                field: "deadlineOn",
                headerFilter: "input",
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
                headerFilter: "input",
                headerFilterParams: {
                    values: {
                        "": "All",
                        "ON-GOING": "ON GOING",
                        "CLOSED": "CLOSED",
                        "DUE": "DUE UPDATE"
                    }
                },
                hozAlign: "center",
                formatter: function (cell) {
                    const status = cell.getValue();
                    const el = cell.getElement();

                    let bg = "#FF2413";
                    if (status === "CLOSED") bg = "#28C232";
                    else if (status === "ON-GOING") bg = "#C2B70B";
                    else if (status === "DUE") bg = "#F50C0C";

                    // color the whole cell
                    el.style.backgroundColor = bg;
                    el.style.color = "#FFFFFF";
                    el.style.fontWeight = "600";
                    el.style.textAlign = "center";

                    return status;
                }
            },
            {
                title: "CLOSE DATE",
                field: "submissionDate",
                headerFilter: "input",
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
                title: "VIEW ISSUES",
                field: "hasIssues",
                headerFilter: "input",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let value = rowData.hasIssues;
                    let hasIssues = value === true ? "" : "disabled";
                    return `<button class="grc-table-btn grc-btn-default grc-task-action ${hasIssues}" ${hasIssues} onclick="viewIssues(${rowData.id})">
                            <span><i class="mdi mdi-link-lock" aria-hidden="true"></i></span>
                            <span>ISSUES</span>
                        </button>`;
                },
                width: 200,
                hozAlign: "left",
                headerHozAlign: "left",
                headerSort: false
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteCircular(${rowData.id})">
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
    initReturnSearch();
}

function initReturnSearch() {

}

$('.action-btn-return-new').on('click', function () {
    addCircular();
});

$('.action-btn-circular-report-bou').on('click', function () {
    $.ajax({
        url: '/grc/circular/reports/bou',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(circularTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "BOU_Circulars.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-circular-report-dpf').on('click', function () {
    $.ajax({
        url: '/grc/circular/reports/dpf',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(circularTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "DPF_Circulars.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-circular-report-ura').on('click', function () {
    $.ajax({
        url: '/grc/circular/reports/ura',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(circularTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "URA_Circulars.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-circular-report-mofed').on('click', function () {
    $.ajax({
        url: '/grc/circular/reports/mofed',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(circularTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "MoFed_Circulars.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-circular-report-other').on('click', function () {
    $.ajax({
        url: '/grc/circular/reports/other',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(circularTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Other_Authorities_Circulars.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-circular-report-ppd').on('click', function () {
    a$.ajax({
        url: '/grc/circular/reports/ppda',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(circularTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "PPDA_Circulars.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-circular-report-summery').on('click', function () {
    $.ajax({
        url: '/grc/circular/reports/summary',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(circularTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Summary_Circulars.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

function addCircular() {
    openCircular2Panel('Add New Circular', {
        id: 0,
        reference: '',
        circularTitle: '',
        circularRequirement: '',
        recievedOn: '',
        deadline: '',
        requiredSubmissionDate:'',
        isDeleted: false,
        ownerId: 0,
        frequencyId: 0,
        authorityId: 0,
        breachRisk: '',
        sendReminder:false,
        interval: 'NA',
        intervalType:'NA',
        reminder: '',
        requiredSubmissionDay:0,
        comments: '',
        issues:[]
    }, false);
}

function openCircular2Panel(title, record, isEdit) {
    $('#isCircularEdit').val(isEdit);
    $('#circularId').val(record.id);
    $('#reference').val(record.reference || '');
    $('#circularTitle').val(record.circularTitle || '');
    $('#circularRequirement').val(record.circularRequirement || '');
    $('#deadline').val(record.deadline || '');
    $('#isDeleted').prop('checked', record.isDeleted);
    $('#ownerId').val(record.ownerId || '0').trigger('change');
    $('#frequencyId').val(record.frequencyId || '0').trigger('change');
    $('#authorityId').val(record.authorityId || '0').trigger('change');
    $('#breachRisk').val(record.breachRisk || '');
    $('#comments').val(record.comments || '');
    $('#sendReminder').prop('checked', record.sendReminder);
    $('#reminder').val(record.reminder || '');
    $('#submissionDay').val(record.requiredSubmissionDay || 0);
    $('#interval').val(record.interval || 'NA').trigger('change');
    $('#intervalType').val(record.intervalType || 'NA').trigger('change');

    const receivedOn = normalizeDate(record.recievedOn);
    const deadline = normalizeDate(record.deadline);
    const submissionDate = normalizeDate(record.requiredSubmissionDate);

    //..clear first
    flatpickrInstances["recievedOn"]?.clear();
    flatpickrInstances["deadline"]?.clear();
    flatpickrInstances["submissionDate"]?.clear();

    if (receivedOn && flatpickrInstances["recievedOn"]) {
        flatpickrInstances["recievedOn"].setDate(receivedOn, true);
    }

    if (deadline && flatpickrInstances["deadline"]) {
        flatpickrInstances["deadline"].setDate(deadline, true);
    } else {
        record.deadline = "";
    }

    if (submissionDate && flatpickrInstances["submissionDate"]) {
        flatpickrInstances["submissionDate"].setDate(submissionDate, true);
    } else {
        record.requiredSubmissionDate = "";
    }

    //..add issues
    renderCirIssues(record.issues);

    //load dialog window
    $('#paneTitle').text(title);
    $('#circularOverlay').addClass('active');
    $('#circularPanel').addClass('active');
}

function normalizeDate(value) {
    return value && value.trim() !== '' ? value : null;
}

function findCircularRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/circulars/retrieve-circular/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
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

function viewCircular(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving Circular...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    console.log("ID >> " + id);
    findCircularRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openCircular2Panel('Edit Circular', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Circular not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load circular details.' });
        });
}

function saveCircular(e) {
    e.preventDefault();
    let id = Number($('#circularId').val()) || 0;
    let ownerId = Number($('#ownerId').val()) || 0;
    let frequencyId = Number($('#frequencyId').val()) || 0;
    let authorityId = Number($('#authorityId').val()) || 0;
    let isEdit = $('#isCircularEdit').val() || false;
    let submissionDay = Number($('#submissionDay').val()) || 0;

    //..build record payload from form
    let recordData = {
        id: id,
        reference: $('#reference').val()?.trim(),
        circularTitle: $('#circularTitle').val()?.trim(),
        circularRequirement: $('#circularRequirement').val()?.trim(),
        recievedOn: $('#recievedOn').val()?.trim(),
        deadline: $('#deadline').val()?.trim(),
        ownerId: ownerId,
        status:'OPEN',
        frequencyId: frequencyId,
        authorityId: authorityId,
        breachRisk: $('#breachRisk').val()?.trim(),
        isDeleted: $('#isDeleted').is(':checked') ? true : false,
        requiredSubmissionDate: $('#submissionDate').val()?.trim(),
        sendReminder: $('#sendReminder').is(':checked') ? true : false,
        reminder: $('#reminder' || '').val()?.trim(),
        interval: $('#interval').val()?.trim(),
        intervalType: $('#intervalType').val()?.trim(),
        requiredSubmissionDay: submissionDay,
        comments: $('#comments').val()?.trim()
    };

    //..validate required fields
    let errors = [];
    if (!recordData.reference)
        errors.push("Reference field is required.");

    if (!recordData.circularTitle)
        errors.push("Circular Title field is required.");

    if (!recordData.recievedOn)
        errors.push("Receive date field is required.");

    if (!recordData.circularRequirement)
        errors.push("Circular requirement field is required.");

    if (recordData.ownerId == 0)
        errors.push("Owner Field is required.");

    if (recordData.authorityId == 0)
        errors.push("Authority Field is required.");

    if (errors.length > 0) {
        highlightInnerField("#issueDescription", !recordData.reference);
        highlightInnerField("#issueResolution", !recordData.circularTitle);
        highlightInnerField("#circularRequirement", !recordData.circularRequirement);
        highlightInnerField("#circularRequirement", !recordData.circularRequirement);
        highlightInnerField("#authorityId", recordData.authorityId === 0);
        highlightInnerField("#ownerId", recordData.ownerId === 0);
        Swal.fire({
            title: "Circular issue Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveCircular2Record(isEdit, recordData);
}

function saveCircular2Record(isEdit, record) {

    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/circulars/update-circular"
        : "/grc/compliance/circulars/create-circular";

    Swal.fire({
        title: isEdit ? "Updating circular..." : "Saving circular...",
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
            'X-CSRF-TOKEN': getCircularAntiForgeryToken()
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

            Swal.fire(res.message || "Circular saved successfully")
                .then(() => {
                    //..close panel
                    closeCircularPanel();
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

function closeCircularPanel() {
    $('#circularOverlay').removeClass('active');
    $('#circularPanel').removeClass('active');
}

function addIssue(e) {
    e.preventDefault();
    let circularId = Number($('#circularId').val()) || 0;
    openCircularIssue("New Issue", {
        id: 0,
        circularId: circularId,
        issueDescription: '',
        issueResolution: '',
        issueStatus: '',
        issueRecieved: '',
        issueResolved: '',
        issueDeleted: false,
    }, false);
}

function openCircularIssue(title, record, isEdit) {
    $('#issueId').val(record.id);
    $('#issueEdit').val(isEdit);
    $('#parentId').val(record.circularId);
    $('#issueDescription').val(record.issueDescription || '');
    $('#issueResolution').val(record.issueResolution || '');
    $('#issueStatus').val(record.circularStatus || 'UNKNOWN').trigger('change');
    $('#issueDeleted').prop('checked', record.issueDeleted);
    $('#issueRecieved').val(record.issueRecieved || '');
    $('#issueResolved').val(record.issueResolved || '');

    if (record.issueRecieved) {
        flatpickrInstances["issueRecieved"].setDate(record.issueRecieved, true);
    }

    if (record.issueResolved) {
        flatpickrInstances["issueResolved"].setDate(record.issueResolved, true);
    }

    //..open panel
    $('#issueTitle').text(title);
    $('.map-panel-overlay').addClass('active');
    $('#issuePanel').addClass('active');

    restoreCircularActiveTab();
}

function saveFormIssue(e) {
    e.preventDefault();
    let id = Number($('#issueId').val()) || 0;
    let circularId = Number($('#parentId').val()) || 0;
    let isEdit = $('#issueEdit').val() || false;

    //..build record payload from form
    let recordData = {
        id: id,
        circularId: circularId,
        issueDescription: $('#issueDescription').val()?.trim(),
        issueResolution: $('#issueResolution').val()?.trim(),
        issueStatus: $('#issueStatus').val()?.trim(),
        issueRecieved: $('#issueRecieved').val()?.trim(),
        issueResolved: $('#issueResolved').val()?.trim(),
        issueDeleted: $('#isIssueDeleted').is(':checked') ? true : false,
    };

    //..validate required fields
    let errors = [];
    if (!recordData.issueDescription)
        errors.push("Description field is required.");

    if (!recordData.issueRecieved)
        errors.push("Recieve date field is required.");

    if (!recordData.issueResolution)
        errors.push("Resolution field is required.");

    if (recordData.circularId == 0)
        errors.push("Circular ID is required.");

    if (errors.length > 0) {
        highlightInnerField("#issueDescription", !recordData.issueDescription);
        highlightInnerField("#issueResolution", !recordData.issueResolution);
        Swal.fire({
            title: "Circular issue Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveCircularIssue(isEdit, recordData);
}

function saveCircularIssue(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/circular/issues/update-issue"
        : "/grc/compliance/circular/issues/create-issue";

    Swal.fire({
        title: isEdit ? "Updating issue..." : "Saving issue...",
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
            'X-CSRF-TOKEN': getCircularAntiForgeryToken()
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

            //..add issue to list
            addIssueToList(record);

            Swal.fire(res.message || (isEdit ? "Issue updated successfully" : "Issue created successfully"));

            //..close panel
            closeCircularIssue();
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

$(document).on('click', '.grc-edit-issue', function () {
    const id = $(this).data('id');
    editCircularIssue(id);
    return false;
});

function viewCircularIssue(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving issue...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findCircularIssue(id)
        .then(record => {
            Swal.close();
            if (record) {
                openCircularIssue('ISSUE DETAILS', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Issue record not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load issue details.' });
        });
}

function findCircularIssue(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/circular/issues/retrieve-issue/${id}`,
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

function closeCircularIssue() {
    $('.map-panel-overlay').removeClass('active');
    $('#issuePanel').removeClass('active');
}

function restoreCircularActiveTab() {

    const tab = sessionStorage.getItem('obligationActiveTab');
    if (!tab) return;

    const trigger = document.querySelector(`button[data-bs-target="${tab}"]`);
    if (!trigger) return;

    //..ensure bootstrap is available
    if (typeof bootstrap === 'undefined' || !bootstrap.Tab) return;

    bootstrap.Tab.getOrCreateInstance(trigger).show();
}

function deleteCircular(id) {
    alert("Delete circular with ID >> " + id);
}

function getCircularAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function renderCirIssues(issues) {
    const $list = $('.issues-list');
    $list.empty();

    if (issues.length === 0) {
        $list.append('<li class="text-muted no-control-items"><div class="message">No issues reported</div></li>');
        return;
    }

    issues.forEach(issue => {
        const isChecked = true;
        const nameClass = issue.isDeleted ? 'text-decoration-line-through' : '';

        const listItem = `
            <li class="control-item mb-2">
                <div class="form-check">
                    <input class="form-check-input" 
                           type="checkbox" 
                           id="item_${issue.id}" 
                           data-item-id="${issue.id}"
                           ${isChecked ? 'checked' : ''}>
                    <label class="form-check-label ${nameClass}" for="item_${issue.id}">
                        ${issue.description}
                    </label>
                </div>
                <div class="text-muted small">
                    <button type="button" 
                            class="grc-table-btn grc-view-action grc-edit-issue" 
                            data-id="${issue.id}">
                        <span><i class="mdi mdi-pencil-outline" aria-hidden="true"></i></span>
                    </button>
                </div>
            </li>`;
        $list.append(listItem);
    });

    //..attach handler
    attachEditIssue2Handlers();
}

function attachEditIssue2Handlers() {
    //..remove any existing handlers first
    $('.grc-edit-issue').off('click.editIssue');

    //..attach directly to each button
    $('.grc-edit-issue').on('click.editIssue', function (e) {
        e.preventDefault();
        e.stopImmediatePropagation();
        const id = $(this).data('id');
        //..call your edit function
        editIssue(id);

        return false;
    });
}

let flatpickrInstances = {};

function initDates() {

    flatpickrInstances["recievedOn"] = flatpickr("#recievedOn", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrInstances["deadline"] = flatpickr("#deadline", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrInstances["issueRecieved"] = flatpickr("#issueRecieved", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrInstances["issueResolved"] = flatpickr("#issueResolved", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

    flatpickrInstances["submissionDate"] = flatpickr("#submissionDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });

}

function highlightInnerField(selector, hasError, message) {
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
    initCircularTable();
    initDates();

    $('#frequencyId ,#ownerId, #authorityId, #interval, #intervalType').select2({
        width: '100%',
        dropdownParent: $('#circularPanel')
    });

    $('#issueStatus').select2({
        width: '100%',
        dropdownParent: $('#issuePanel')
    });

    $('#circularForm').on('submit', function (e) {
        e.preventDefault();
    });

});

