//..initialize on load
$(document).ready(function () {
    initObligationTable();
});

let sampleObligationData = [
    {
        id: 1,
        code: "PRJ-001",
        title: "Main Project Alpha",
        category: "Technical",
        status: "Active",
        owner: "John Smith",
        children: [
            {
                id: 11,
                parentId: 1,
                code: "TSK-001",
                title: "Database Design",
                category: "Technical",
                status: "Completed",
                owner: "Jane Doe"
            },
            {
                id: 12,
                parentId: 1,
                code: "TSK-002",
                title: "API Development",
                category: "Technical",
                status: "Active",
                owner: "Bob Wilson",
                children: [
                    {
                        id: 121,
                        parentId: 12,
                        code: "SUB-001",
                        title: "Authentication Module",
                        category: "Technical",
                        status: "Active",
                        owner: "Alice Brown"
                    }
                ]
            }
        ]
    },
    {
        id: 2,
        code: "PRJ-002",
        title: "Project Beta",
        category: "Administrative",
        status: "Pending",
        owner: "Sarah Johnson",
        children: [
            {
                id: 21,
                parentId: 2,
                code: "TSK-003",
                title: "Requirements Gathering",
                category: "Administrative",
                status: "Active",
                owner: "Mike Davis"
            }
        ]
    }
];

let obligationTable;
let nextObligationId = 1000;

//..table initialization function
function initObligationTable() {
    obligationTable = new Tabulator("#obligations-table", {
        data: sampleObligationData,
        dataTree: true,
        dataTreeStartExpanded: false,
        layout: "fitColumns", 
        responsiveLayout: "hide",
        dataTreeChildField: "children",
        columns: [
            {
                title: "Record Code",
                field: "code",
                minWidth: 120, 
                widthGrow: 1,
                formatter: function (cell) {
                    return `<span class="record-code">${cell.getValue()}</span>`;
                }
            },
            {
                title: "Title",
                field: "title",
                minWidth: 200, 
                widthGrow: 4,
                formatter: function (cell) {
                    return `<span class="clickable-title" onclick="viewRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`;
                }
            },
            {
                title: "Category",
                field: "category",
                minWidth: 120,
                widthGrow: 1
            },
            {
                title: "Status",
                field: "status",
                minWidth: 100,
                widthGrow: 1,
                formatter: function (cell) {
                    let value = cell.getValue();
                    let color = {
                        "Active": "#28a745",
                        "Pending": "#ffc107",
                        "Completed": "#6c757d",
                        "Archived": "#dc3545"
                    }[value] || "#6c757d";
                    return `<span style="color: ${color}; font-weight: 600;">${value}</span>`;
                }
            },
            {
                title: "Owner",
                field: "owner",
                minWidth: 120,
                widthGrow: 2
            },
            {
                title: "Actions",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `
                        <button class="btn" style="padding: 5px 10px; font-size: 12px; margin-right: 5px;" onclick="addChildRecord(${rowData.id})">Add Child</button>
                        <button class="btn" style="padding: 5px 10px; font-size: 12px; background: #dc3545;" onclick="deleteRecord(${rowData.id})">Delete</button>
                    `;
                },
                width: 200, 
                hozAlign: "center",
                headerSort: false
            }
        ]
    });
}

//..view/edit Record
function viewObligationRecord(id) {
    let record = findObligationRecord(sampleData, id);
    if (record) {
        openPanel('Edit Record', record, true);
    }
}

//..add regiter
function addObligationRootRecord() {
    openPanel('Add New Record', {
        id: ++nextObligationId,
        code: '',
        title: '',
        category: 'General',
        status: 'Active',
        owner: ''
    }, false);
}

//..add Child Record
function addObligationChildRecord(parentId) {
    let parent = findRecord(sampleData, parentId);
    openPanel('Add Child Record', {
        id: ++nextObligationId,
        parentId: parentId,
        code: '',
        title: '',
        category: 'General',
        status: 'Active',
        owner: '',
        _parent: parent
    }, false);
}

//..open slide panel
function openObligationPanel(title, record, isEdit) {
    $('#panelTitle').text(title);
    $('#recordId').val(record.id);
    $('#parentId').val(record.parentId || '');
    $('#isEdit').val(isEdit);
    $('#recordCode').val(record.code || '');
    $('#recordTitle').val(record.title || '');
    $('#recordCategory').val(record.category || 'General');
    $('#recordStatus').val(record.status || 'Active');
    $('#recordOwner').val(record.owner || '');
    $('#recordDescription').val(record.description || '');

    if (record.parentId && record._parent) {
        $('#parentInfo').show();
        $('#parentDisplay').val(`${record._parent.code} - ${record._parent.title}`);
    } else {
        $('#parentInfo').hide();
    }

    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

//..close panel
function closeObligationPanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

//..save record (AJAX simulation)
function saveObligationRecord() {
    let isEdit = $('#isEdit').val() === 'true';
    let recordData = {
        id: parseInt($('#recordId').val()),
        code: $('#recordCode').val(),
        title: $('#recordTitle').val(),
        category: $('#recordCategory').val(),
        status: $('#recordStatus').val(),
        owner: $('#recordOwner').val(),
        description: $('#recordDescription').val()
    };

    let parentId = $('#parentId').val();
    if (parentId) {
        recordData.parentId = parseInt(parentId);
    }

    //..save via controller
    saveObligationViaController(recordData, isEdit, function (success) {
        if (success) {
            if (isEdit) {
                updateObligationRecordInData(sampleData, recordData);
            } else {
                addObligationRecordToData(sampleData, recordData);
            }

            obligationTable.replaceData(sampleData);
            closePanel();
            alert('Record saved successfully!');
        }
    });
}

//..save register
function saveObligation(data, isEdit, callback) {
    // This simulates an AJAX call to your ASP.NET Core controller
    // Replace with actual $.ajax call:
    /*
    $.ajax({
        url: isEdit ? '/Records/Update' : '/Records/Create',
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function(response) {
            callback(true);
        },
        error: function(xhr, status, error) {
            alert('Error saving record: ' + error);
            callback(false);
        }
    });
    */

    // Simulated success
    setTimeout(() => callback(true), 100);
}

//..delete Record
function deleteObligationRecord(id) {
    if (confirm('Are you sure you want to delete this record and all its children?')) {
        // Simulate AJAX delete
        /*
        $.ajax({
            url: '/Records/Delete/' + id,
            type: 'DELETE',
            success: function(response) {
                removeRecordFromData(sampleData, id);
                table.replaceData(sampleData);
                alert('Record deleted successfully!');
            }
        });
        */

        removeObligationRecordFromData(sampleData, id);
        obligationTable.replaceData(sampleData);
        alert('Record deleted successfully!');
    }
}

// Helper Functions
function findObligationRecord(data, id) {
    for (let item of data) {
        if (item.id === id) return item;
        if (item._children) {
            let found = findRecord(item._children, id);
            if (found) return found;
        }
    }
    return null;
}

function addObligationRecordToData(data, newRecord) {
    if (newRecord.parentId) {
        let parent = findRecord(data, newRecord.parentId);
        if (parent) {
            if (!parent._children) parent._children = [];
            parent._children.push(newRecord);
        }
    } else {
        data.push(newRecord);
    }
}

function updateObligationRecordInData(data, updatedRecord) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === updatedRecord.id) {
            Object.assign(data[i], updatedRecord);
            return true;
        }
        if (data[i]._children) {
            if (updateObligationRecordInData(data[i]._children, updatedRecord)) {
                return true;
            }
        }
    }
    return false;
}

function removeObligationRecordFromData(data, id) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === id) {
            data.splice(i, 1);
            return true;
        }
        if (data[i]._children) {
            if (removeObligationRecordFromData(data[i]._children, id)) {
                return true;
            }
        }
    }
    return false;
}

//..initialize dropdowns
//function initializeBranches() {
//    $(".js-branches").each(function () {
//        if (!$(this).hasClass('select2-hidden-accessible')) {
//            initializeBranchElement($(this));
//        }
//    });
//}

//function initializeDepartments() {
//    $(".js-departments").each(function () {
//        if (!$(this).hasClass('select2-hidden-accessible')) {
//            initializeDepartmentElement($(this));
//        }
//    });
//}

//function initializeBranchElement($element) {
//    const elementId = $element.attr('id');
//    const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select branch';

//    $element.select2({
//        width: 'resolve',
//        placeholder: 'Select a branch...',
//        allowClear: true,
//        escapeMarkup: function (markup) {
//            return markup;
//        },
//        language: {
//            noResults: function () {
//                return "No branches found";
//            }
//        }
//    });

//    // Fix accessibility issues after Select2 initialization
//    setTimeout(() => {
//        fixSelect2Accessibility($element, labelText);
//    }, 100);
//}

//function initializeDepartmentElement($element) {
//    const elementId = $element.attr('id');
//    const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select Department';

//    $element.select2({
//        width: 'resolve',
//        placeholder: 'Select a department...',
//        allowClear: true,
//        escapeMarkup: function (markup) {
//            return markup;
//        },
//        language: {
//            noResults: function () {
//                return "No departments found";
//            }
//        }
//    });

//    // Fix accessibility issues after Select2 initialization
//    setTimeout(() => {
//        fixSelect2Accessibility($element, labelText);
//    }, 100);
//}

//function loadBranchesForContainer($container) {
//    const $branchSelects = $container.find('.js-branches');
//    if ($branchSelects.length > 0) {
//        $.ajax({
//            url: '/support/organization/getBranches',
//            type: 'GET',
//            dataType: 'json',
//            success: function (data) {
//                $branchSelects.each(function () {
//                    const $select = $(this);
//                    const currentValue = $select.val();

//                    // Destroy existing Select2 if it exists
//                    if ($select.hasClass('select2-hidden-accessible')) {
//                        $select.select2('destroy');
//                    }

//                    // Clear existing options
//                    $select.empty();

//                    // Add placeholder option
//                    $select.append('<option value="">Select a branch...</option>');

//                    // Add branch options
//                    if (data.results && data.results.length > 0) {
//                        $.each(data.results, function (index, branch) {
//                            $select.append(`<option value="${branch.id}">${branch.text}</option>`);
//                        });
//                    }

//                    // Restore previous value if it exists
//                    if (currentValue) {
//                        $select.val(currentValue);
//                    }

//                    // Initialize Select2 with accessibility fixes
//                    initializeBranchElement($select);
//                });
//            },
//            error: function (xhr, status, error) {
//                console.error('Error loading branches:', error);

//                // Initialize empty Select2 even on error
//                $branchSelects.each(function () {
//                    const $select = $(this);
//                    if (!$select.hasClass('select2-hidden-accessible')) {
//                        initializeBranchElement($select);
//                    }
//                });
//            }
//        });
//    }
//}

//function loadDepartmentsForContainer($container) {
//    const $branchSelects = $container.find('.js-departments');
//    if ($branchSelects.length > 0) {
//        $.ajax({
//            url: '/support/departments/getDepartments',
//            type: 'GET',
//            dataType: 'json',
//            success: function (data) {
//                $branchSelects.each(function () {
//                    const $select = $(this);
//                    const currentValue = $select.val();

//                    // Destroy existing Select2 if it exists
//                    if ($select.hasClass('select2-hidden-accessible')) {
//                        $select.select2('destroy');
//                    }

//                    // Clear existing options
//                    $select.empty();

//                    // Add placeholder option
//                    $select.append('<option value="">Select a department...</option>');

//                    // Add department options
//                    if (data.results && data.results.length > 0) {
//                        $.each(data.results, function (index, department) {
//                            $select.append(`<option value="${department.id}">${department.text}</option>`);
//                        });
//                    }

//                    // Restore previous value if it exists
//                    if (currentValue) {
//                        $select.val(currentValue);
//                    }

//                    // Initialize Select2 with accessibility fixes
//                    initializeDepartmentElement($select);
//                });
//            },
//            error: function (xhr, status, error) {
//                console.error('Error loading departments:', error);

//                // Initialize empty Select2 even on error
//                $branchSelects.each(function () {
//                    const $select = $(this);
//                    if (!$select.hasClass('select2-hidden-accessible')) {
//                        initializeDepartmentElement($select);
//                    }
//                });
//            }
//        });
//    }

//}
