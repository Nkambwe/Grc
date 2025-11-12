let processReviewTable;

function initProcessReviewListTable() {
    processReviewTable = new Tabulator("#processReviewTable", {

    });

    //..search init
    initProcessReviewListTable();
}

function initProcessReviewSearch() {

}

$(document).ready(function () {

    initProcessGroupListTable();

    $('#processReviewForm').on('submit', function (e) {
        e.preventDefault();
    });

});