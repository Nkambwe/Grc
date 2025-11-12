let processNewTable;

function initProcessNewListTable() {
    processNewTable = new Tabulator("#processNewTable", {

    });

    // Search init
    initProcessNewSearch();
}

function initProcessNewSearch() {

}

$(document).ready(function () {

    initProcessNewListTable();

    $('#processReviewForm').on('submit', function (e) {
        e.preventDefault();
    });

});
