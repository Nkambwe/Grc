let processApprovalsTable;

function initProcessApprovalListTable() {
    processApprovalsTable = new Tabulator("#processApprovalsTable", {

    });

    // Search init
    initProcessApprovalsSearch();
}

function initProcessApprovalsSearch() {

}

$(document).ready(function () {

    initProcessApprovalListTable();

    $('#processReviewForm').on('submit', function (e) {
        e.preventDefault();
    });

});

