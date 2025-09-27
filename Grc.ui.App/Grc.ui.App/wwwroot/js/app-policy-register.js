//..initialize on load
$(document).ready(function () {
    initTable();
});

let sampleData = [
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

let table;
let nextId = 1000;

//..table initialization function
function initTable() {
    table = new Tabulator("#policies-table", {
        data: sampleData,
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
function viewRecord(id) {
    let record = findRecord(sampleData, id);
    if (record) {
        openPanel('Edit Record', record, true);
    }
}

//..add regiter
function addRootRecord() {
    openPanel('Add New Record', {
        id: ++nextId,
        code: '',
        title: '',
        category: 'General',
        status: 'Active',
        owner: ''
    }, false);
}

//..add Child Record
function addChildRecord(parentId) {
    let parent = findRecord(sampleData, parentId);
    openPanel('Add Child Record', {
        id: ++nextId,
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
function openPanel(title, record, isEdit) {
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
function closePanel() {
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

//..save record (AJAX simulation)
function saveRecord() {
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

    // Simulate AJAX call to ASP.NET Core controller
    simulateAjaxSave(recordData, isEdit, function (success) {
        if (success) {
            if (isEdit) {
                updateRecordInData(sampleData, recordData);
            } else {
                addRecordToData(sampleData, recordData);
            }

            table.replaceData(sampleData);
            closePanel();
            alert('Record saved successfully!');
        }
    });
}

// Simulate AJAX Save (replace with actual AJAX call)
function simulateAjaxSave(data, isEdit, callback) {
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
function deleteRecord(id) {
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

        removeRecordFromData(sampleData, id);
        table.replaceData(sampleData);
        alert('Record deleted successfully!');
    }
}

// Helper Functions
function findRecord(data, id) {
    for (let item of data) {
        if (item.id === id) return item;
        if (item._children) {
            let found = findRecord(item._children, id);
            if (found) return found;
        }
    }
    return null;
}

function addRecordToData(data, newRecord) {
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

function updateRecordInData(data, updatedRecord) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === updatedRecord.id) {
            Object.assign(data[i], updatedRecord);
            return true;
        }
        if (data[i]._children) {
            if (updateRecordInData(data[i]._children, updatedRecord)) {
                return true;
            }
        }
    }
    return false;
}

function removeRecordFromData(data, id) {
    for (let i = 0; i < data.length; i++) {
        if (data[i].id === id) {
            data.splice(i, 1);
            return true;
        }
        if (data[i]._children) {
            if (removeRecordFromData(data[i]._children, id)) {
                return true;
            }
        }
    }
    return false;
}
