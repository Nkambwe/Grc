let processTatTable;
function initProcessTagListTable() {
    processTatTable = new Tabulator("#processTatTable", {


    });
}

$(document).ready(function () {

    initProcessTagListTable();

    $('#processTagForm').on('submit', function (e) {
        e.preventDefault();
    });

});