let processGroupTable;
function initProcessGroupListTable() {
    processGroupTable = new Tabulator("#processGroupTable", {


    });
}

function createGroup() {
    // Logic to create a new group
}

$(document).ready(function () {

    initProcessGroupListTable();

    $('.action-btn-process-group').on('click', function () {
        createGroup();
    });

    $('#processGroupForm').on('submit', function (e) {
        e.preventDefault();
    });

});