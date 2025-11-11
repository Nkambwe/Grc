let processTagTable;
function initProcessTagListTable() {
    processTagTable = new Tabulator("#processTagTable", {


    });
}

function createTag() {
    // Logic to create a new tag
}   

$(document).ready(function () {

    initProcessTagListTable();

    $('.action-btn-process-tag').on('click', function () {
        createTag();
    });

    $('#processTagForm').on('submit', function (e) {
        e.preventDefault();
    });

});