let obligationTable;

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

function getSelectedControlIds() {
    const selectedIds = [];

    document.querySelectorAll("#controlTree > li").forEach(parent => {

        const parentCheck = parent.querySelector(".parent-check");
        const childChecks = parent.querySelectorAll(".child-check");

        // Parent has NO children
        if (childChecks.length === 0) {
            if (parentCheck && parentCheck.checked) {
                selectedIds.push(parentCheck.dataset.id);
            }
            return;
        }

        // Parent checked → take parent ID
        if (parentCheck.checked) {
            selectedIds.push(parentCheck.dataset.id);
            return;
        }

        // Parent not checked → take checked children
        childChecks.forEach(child => {
            if (child.checked) {
                selectedIds.push(child.dataset.id);
            }
        });
    });

    return selectedIds;
}

function updateObligation(e) {
    e.preventDefault();
    let coverage = Number($('#coverage').val()) || 0;
    let assurance = Number($('#assurance').val()) || 0;

    //..collect compliance maps from the rendered controls
    let complianceMaps = [];

    //..loop through each parent control
    $('.parent-control').each(function () {
        const $parentCheckbox = $(this);
        const mapId = $parentCheckbox.data('map-id');
        const isParentChecked = $parentCheckbox.is(':checked');

        //..collect child controls for this parent
        let controlMaps = [];
        $(`.child-control[data-parent-id="${mapId}"]`).each(function () {
            const $childCheckbox = $(this);
            controlMaps.push({
                id: $childCheckbox.data('control-id'),
                parentId: $childCheckbox.data('parent-id'),
                include: $childCheckbox.is(':checked')
            });
        });

        //..add the parent map with its children
        complianceMaps.push({
            id: mapId,
            include: isParentChecked,
            controlMaps: controlMaps
        });
    });

    //..build record payload from form
    let recordData = {
        id: Number($('#obligationId').val()) || 0,
        isMandatory: $('#isMandatory').is(':checked'),
        coverage: coverage,
        isCovered: coverage === 100,
        exclude: $('#exclude').is(':checked'),
        assurance: assurance,
        rationale: $('#rationale').val(),
        complianceMaps: complianceMaps
    };

    //..validate required fields
    let errors = [];
    if (!recordData.rationale)
        errors.push("Compliance reason/rationale is required.");

    if (errors.length > 0) {
        highlightErrorField("#rationale", !recordData.rationale);
        Swal.fire({
            title: "Compliance Map Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    console.log("Sending data:", recordData);
    //..call backend
    updateRecord(recordData);
}

function updateRecord(record) {
    let url = "/grc/register/obligations/create-map";
    Swal.fire({
        title: "Compliance Mapping...",
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
            'X-CSRF-TOKEN': getObligationToken()
        },
        success: function (res) {
            if (!res || res.success !== true) {
                Swal.fire(res?.message || "Operation failed");
                return;
            }
            Swal.fire(res.message || "Compliance map created successfully");
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
                //..if parsing fails, use the default error
                errorMessage = "Unexpected error occurred";
            }
            Swal.fire("Save Compliance map", errorMessage);
        }
    });
}

function closeObligation() {
    $('.obligation-panel-overlay').removeClass('active');
    $('#obligationPanel').removeClass('active');
}

function viewRequirement(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving obligation...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findObligation(id)
        .then(record => {
            Swal.close();
            if (record) {
                openObligationPanel('REQUIREMENT DETAILS', record);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Requirement record not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load requirement details.' });
        });
}

function findObligation(id) {
    console.log(`Retrieve requirement with id >> ${id}`);
    return new Promise((resolve, reject) => {
        $.ajax({
            url:`/grc/register/obligations/request/${id}`,
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

function openObligationPanel(title, record) {

    let coverageValue = record.coverage ?? 0;
    let assuranceValue = record.assurance ?? 0;

    $('#obligationId').val(record.id);
    $('#category').text(record.category || '');
    $('#statute').text(record.statute || '');
    $('#summery').text(`${record.section} ${record.summery}`);
    $('#obligation').text(record.obligation || '');
    $('#isMandatory').prop('checked', record.isMandatory);
    $('#exclude').prop('checked', record.exclude);
    $('#isCovered').prop('checked', record.isCovered);
    $('#assurance').val(assuranceValue);
    $('#assuranceValue').text(`${assuranceValue}%`);
    $('#coverage').val(coverageValue);
    $('#coverageValue').text(`${coverageValue}%`);
    $('#rationale').val(record.complianceReason);

    //..load compliance controls
    renderComplianceControls(record.complianceMaps);

    //..load issues
    renderIssues(record.issues);

    $('#obligationTitle').text(title);
    $('.obligation-panel-overlay').addClass('active');
    $('#obligationPanel').addClass('active');

    restoreActiveTab();
}

function renderComplianceControls(complianceMaps) {
    const $controlTree = $('#controlTree');
    $controlTree.empty(); // Clear existing content

    if (!complianceMaps || complianceMaps.length === 0) {
        $controlTree.append('<li class="list-group-item">No compliance controls mapped</li>');
        return;
    }

    complianceMaps.forEach(map => {
        // Create parent control item
        const parentItem = $(`
            <li class="list-group-item">
                <div class="d-flex justify-content-between align-items-center">
                    <div class="form-check">
                        <input class="form-check-input parent-control" 
                               type="checkbox" 
                               id="map_${map.id}" 
                               data-map-id="${map.id}"
                               ${map.include ? 'checked' : ''}>
                        <label class="form-check-label fw-bold" for="map_${map.id}">
                            ${map.mapControl}
                        </label>
                    </div>
                </div>
            </li>
        `);

        // If there are child controls, create nested list
        if (map.controlMaps && map.controlMaps.length > 0) {
            const childList = $('<ul class="list-group list-group-flush ms-4 mt-2"></ul>');

            map.controlMaps.forEach(control => {
                const childItem = $(`
                    <li class="list-group-item border-0 py-1">
                        <div class="form-check">
                            <input class="form-check-input child-control" 
                                   type="checkbox" 
                                   id="control_${control.id}" 
                                   data-control-id="${control.id}"
                                   data-parent-id="${control.categoryId}"
                                   ${control.include ? 'checked' : ''}>
                            <label class="form-check-label" for="control_${control.id}">
                                ${control.itemName}
                            </label>
                        </div>
                    </li>
                `);
                childList.append(childItem);
            });

            parentItem.append(childList);
        }

        $controlTree.append(parentItem);
    });
}

function addControlToList(map) {
    const $controlTree = $('#controlTree');
    //..create parent control item
    const parentItem = $(`
            <li class="list-group-item">
                <div class="d-flex justify-content-between align-items-center">
                    <div class="form-check">
                        <input class="form-check-input parent-control" 
                               type="checkbox" 
                               id="map_${map.id}" 
                               data-map-id="${map.id}"
                               ${map.include ? 'checked' : ''}>
                        <label class="form-check-label fw-bold" for="map_${map.id}">
                            ${map.mapControl}
                        </label>
                    </div>
                </div>
            </li>
        `);
    //..if there are child controls, create nested list
    if (map.controlMaps && map.controlMaps.length > 0) {
        const childList = $('<ul class="list-group list-group-flush ms-4 mt-2"></ul>');

        map.controlMaps.forEach(control => {
            const childItem = $(`
                    <li class="list-group-item border-0 py-1">
                        <div class="form-check">
                            <input class="form-check-input child-control" 
                                   type="checkbox" 
                                   id="control_${control.id}" 
                                   data-control-id="${control.id}"
                                   data-parent-id="${control.categoryId}"
                                   ${control.include ? 'checked' : ''}>
                            <label class="form-check-label" for="control_${control.id}">
                                ${control.itemName}
                            </label>
                        </div>
                    </li>
                `);
            childList.append(childItem);
        });

        parentItem.append(childList);
    }

    $controlTree.append(parentItem);
}

function renderIssues(issues) {
    const $list = $('.issues-list');
    $list.empty();

    if (issues.length === 0) {
        $list.append('<li class="text-muted no-control-items">No issues reported</li>');
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
    attachEditIssueHandlers();
}

function addIssueToList(issue) {
    console.log("Issues called with:", issue);
    const $list = $('.issues-list');

    if (!issue) {
        return;
    }

    // Remove "No issues reported" placeholder if it exists
    $list.find('.no-control-items').remove();

    const isChecked = true;
    const nameClass = issue.isDeleted ? 'text-decoration-line-through' : '';
    const newItem = `
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

    $list.append(newItem);

    //..attach handler
    attachEditIssueHandlers();
}

function attachEditIssueHandlers() {
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

function addControl(e) {
    e.preventDefault();

    let id = Number($('#obligationId').val()) || 0;
    openNewControlPane("New Control", {
        id: id
    }, false);
}

function openNewControlPane(title, record, isEdit) {
    $('#sectionId').val(record.id);
    $('#controlIsEdit').val(isEdit);

    $('#mapTitle').text(title);
    $('.map-panel-overlay').addClass('active');
    $('#mapPanel').addClass('active');
}

function closeControl() {
    $('.map-panel-overlay').removeClass('active');
    $('#mapPanel').removeClass('active');
}

function addIssue(e) {
    e.preventDefault();
    let articleId = Number($('#obligationId').val()) || 0;
    openIssue("New Issue", {
        id: 0,
        articleId: articleId,
        description:'',
        comments: '',
        isClosed: false,
        isDeleted:false,
    }, false);
}

function openIssue(title,record, isEdit) {
    $('#issueId').val(record.id);
    $('#articleId').val(record.articleId);
    $('#issueDescription').val(record.description || '');
    $('#issueComments').val(record.comments || '');
    $('#issueEdit').val(isEdit);
    $('#isIssueClosed').prop('checked', record.isClosed);
    $('#isIssueDeleted').prop('checked', record.isDeleted);

    //..open panel
    $('#issueTitle').text(title);
    $('.map-panel-overlay').addClass('active');
    $('#issuePanel').addClass('active');
}

function saveIssue(e) {
    e.preventDefault();
    let id = Number($('#issueId').val()) || 0;
    let articleId = Number($('#articleId').val()) || 0;
    let isEdit = $('#issueEdit').val() || false;

    //..build record payload from form
    let recordData = {
        id: id,
        articleId: articleId,
        description: $('#issueDescription').val()?.trim(),
        comments: $('#issueComments').val()?.trim(),
        isClosed: $('#isIssueClosed').prop('checked'),
        isDeleted: $('#isIssueDeleted').prop('checked'),
    };

    //..validate required fields
    let errors = [];
    if (!recordData.description)
        errors.push("Description field is required.");

    if (!recordData.comments)
        errors.push("Notes field is required.");

    if (errors.length > 0) {
        highlightError("#issueDescription", !recordData.description);
        highlightError("#issueComments", !recordData.comments);
        Swal.fire({
            title: "Compliance issue Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveIssueRecord(isEdit, recordData);
}

function saveIssueRecord(isEdit, record){
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/register/issues/update-issue"
        : "/grc/compliance/register/issues/create-issue";

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
            'X-CSRF-TOKEN': getIssueToken()
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
            closeIssue();
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
    editIssue(id);
    return false;
});

function editIssue(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving issue...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findIssue(id)
        .then(record => {
            Swal.close();
            if (record) {
                openIssue('ISSUE DETAILS', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Issue record not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load issue details.' });
        });
}

function findIssue(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/issues/retrieve-issue/${id}`,
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

function closeIssue() {
    $('.map-panel-overlay').removeClass('active');
    $('#issuePanel').removeClass('active');
}

//..capture tab state
$('button[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
    sessionStorage.setItem('obligationActiveTab', e.target.getAttribute('data-bs-target'));
});

function restoreActiveTab() {

    const tab = sessionStorage.getItem('obligationActiveTab');
    if (!tab) return;

    const trigger = document.querySelector(`button[data-bs-target="${tab}"]`);
    if (!trigger) return;

    //..ensure bootstrap is available
    if (typeof bootstrap === 'undefined' || !bootstrap.Tab) return;

    bootstrap.Tab.getOrCreateInstance(trigger).show();
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

function getObligationToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function getSelectedControlItems() {
    const selectedItems = [];
    $('.child-control:checked').each(function () {
        const itemId = $(this).data('control-id');
        selectedItems.push(itemId);
    });
    return selectedItems;
}

function assignControl(event) {
    event.preventDefault();

    //..get the selected child control IDs
    const selectedControlIds = getSelectedControlItems();

    //..validate that at least one control is selected
    if (selectedControlIds.length === 0) {
        Swal.fire({
            title: 'No Controls Selected',
            text: 'Please select at least one control item to assign.',
            icon: 'warning'
        });
        return;
    }

    //..get other form data
    const sectionId = $('#sectionId').val();
    const categoryId = $('.parent-control:checked').data('map-id'); 

    //..build your record object
    const record = {
        sectionId: sectionId,
        categoryId: categoryId,
        controlItemIds: selectedControlIds
    };

    console.log('Submitting record:', record);

    //..call your save function
    saveControlAssignment(record);
}

function saveControlAssignment(record) {
    const url ="/grc/compliance/register/controlitems/assign-control";
    Swal.fire({
        title: "Compliance mapping...",
        text: "Please wait while we map controls.",
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
            'X-CSRF-TOKEN': getIssueToken()
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

            //..add control to list
            addControlToList(record);

            Swal.fire(res.message || "Compliance mapping completed successfully");

            //..close panel
            closeControl();
        },
        error: function (xhr) {
            Swal.close();

            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }

            Swal.fire({
                title: "Compliance mapping failed",
                text: errorMessage
            });
        }
    });

}

function getControl(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/compliance-controls/retrieve-control/${id}`,
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

$(document).ready(function () {
    initObligationable();
    $('#controlId').select2({
        width: '100%',
        dropdownParent: $('#mapPanel')
    });
   
    $('#controlId').on('change', function () {
        const selectedValue = $(this).val();
        if (selectedValue === '0' || !selectedValue) {
            return;
        }
        $('#parentId').val(selectedValue);
        Swal.fire({
            title: 'Loading...',
            text: 'Retrieving control...',
            allowOutsideClick: false,
            allowEscapeKey: false,
            didOpen: () => Swal.showLoading()
        });

        getControl(selectedValue)
            .then(response => {
                Swal.close();

                console.log(`RESP >>> ${response}`);
                console.log(`Response cat ID ${response.categoryId}`);
                if (response && response.categoryId) {  
                    const data = response;
                    const $controlTree = $('.controls-assigned-list'); 
                    $controlTree.empty();

                    //..create parent control item
                    const parentItem = $(`
                        <li class="list-group-item">
                            <div class="d-flex justify-content-between align-items-center">
                                <div class="form-check">
                                    <input class="form-check-input parent-control" 
                                           type="checkbox" 
                                           id="map_${data.categoryId}" 
                                           data-map-id="${data.categoryId}">
                                    <label class="form-check-label fw-bold" for="map_${data.categoryId}">
                                        ${data.categoryName}
                                    </label>
                                </div>
                            </div>
                        </li>
                    `);

                    //..if there are child controls, create nested list
                    if (data.items && data.items.length > 0) {
                        const childList = $('<ul class="list-group list-group-flush ms-4 mt-2"></ul>');
                        data.items.forEach(control => {
                            const deletedClass = control.isItemDeleted ? 'text-decoration-line-through text-muted' : '';
                            const childItem = $(`
                                <li class="list-group-item border-0 py-1">
                                    <div class="form-check">
                                        <input class="form-check-input child-control" 
                                               type="checkbox" 
                                               id="control_${control.itemId}" 
                                               data-control-id="${control.itemId}"
                                               data-parent-id="${data.categoryId}">
                                        <label class="form-check-label ${deletedClass}" for="control_${control.itemId}">
                                            ${control.itemName}
                                        </label>
                                    </div>
                                </li>
                            `);
                            childList.append(childItem);
                        });
                        parentItem.append(childList);
                    }

                    $controlTree.append(parentItem);

                    //..handler for parent checkbox to select/deselect all children
                    attachControlCheckboxHandlers();

                } else {
                    Swal.fire({ title: 'NOT FOUND', text: 'Control record not found' });
                }
            })
            .catch((error) => {
                Swal.close();
                console.error('Error loading control:', error);
                Swal.fire({ title: 'Error', text: 'Failed to load control details.' });
            });
    });

    $('#coverage').on('input', function () {
        $('#coverageValue').text(this.value + '%');
    });

    $('#assurance').on('input', function () {
        $('#assuranceValue').text(this.value + '%');
    });

    $('#issueForm').on('submit', function (e) {
        e.preventDefault();
    })

    $('#controlForm').on('submit', function (e) {
        e.preventDefault();
    })

    $(document).on('change', '.parent-control', function () {
        const $parent = $(this);
        const isChecked = $parent.is(':checked');
        const $parentLi = $parent.closest('li');

        //..update all child checkboxes
        $parentLi.find('.child-control').prop('checked', isChecked);

        console.log(`Parent control ${$parent.data('map-id')} changed to ${isChecked}`);
    });

    $(document).on('change', '.child-control', function () {
        const $child = $(this);
        const parentId = $child.data('parent-id');
        console.log(`Child control ${$child.data('control-id')} changed`);

        //..find the parent checkbox using the parentId
        const $parentCheckbox = $(`#map_${parentId}`);

        //..check if any child controls of this parent are checked
        const $allChildrenOfParent = $(`.child-control[data-parent-id="${parentId}"]`);
        const anyChildChecked = $allChildrenOfParent.filter(':checked').length > 0;
        const allChildrenChecked = $allChildrenOfParent.length === $allChildrenOfParent.filter(':checked').length;

        //..update parent checkbox state
        if (allChildrenChecked && $allChildrenOfParent.length > 0) {
            //..if all children are checked, check the parent
            $parentCheckbox.prop('checked', true);
        } else if (!anyChildChecked) {
            //..if no children are checked, uncheck the parent
            $parentCheckbox.prop('checked', false);
        } else {
            //..if some but not all children are checked, add an indeterminate state
            $parentCheckbox.prop('indeterminate', true);
        }
    });

});

function attachControlCheckboxHandlers() {
    //..checked/unchecked all children when parent clicked
    $('.parent-control').off('change').on('change', function () {
        const isChecked = $(this).prop('checked');
        $(this).closest('li').find('.child-control').prop('checked', isChecked);
    });

    //..update parent if all children are checked
    $('.child-control').off('change').on('change', function () {
        const $parent = $(this).closest('li').closest('li').find('.parent-control');
        const $allChildren = $(this).closest('ul').find('.child-control');
        const allChecked = $allChildren.length === $allChildren.filter(':checked').length;
        $parent.prop('checked', allChecked);
    });
}

function highlightError(selector, hasError, message) {
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

function getIssueToken() {
    return $('meta[name="csrf-token"]').attr('content');
}
