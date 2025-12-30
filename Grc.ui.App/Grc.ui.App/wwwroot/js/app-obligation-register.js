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

function updateObligationt(e) {
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

function closeObligationt() {
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

    //..load controls
    renderComplianceControls(record.complianceMaps);

    $('#obligationTitle').text(title);
    $('.obligation-panel-overlay').addClass('active');
    $('#obligationPanel').addClass('active');

    restoreActiveTab();
}

function renderComplianceControls(complianceMaps) {
    console.log("renderComplianceControls called with:", complianceMaps);

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
                                   data-parent-id="${control.parentId}"
                                   ${control.include ? 'checked' : ''}>
                            <label class="form-check-label" for="control_${control.id}">
                                ${control.mapControl}
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

    console.log("Controls rendered successfully");
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

    // 🔹 Ensure Bootstrap is available
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

$(document).ready(function () {
    initObligationable();

    $('#coverage').on('input', function () {
        $('#coverageValue').text(this.value + '%');
    });

    $('#assurance').on('input', function () {
        $('#assuranceValue').text(this.value + '%');
    });

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

