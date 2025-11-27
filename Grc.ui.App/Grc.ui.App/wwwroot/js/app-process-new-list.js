let processNewTable;

function initProcessNewListTable() {
    processNewTable = new Tabulator("#processNewTable", {
        ajaxURL: "/operations/workflow/processes/new-list",
        paginationMode: "remote",
        filterMode: "remote",
        sortMode: "remote",
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
                        ["processName", "description", "ownerName", "unitName", "assigneeName"].includes(f.field)
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
            return {
                data: response.data || [],
                last_page: response.last_page || 1,
                total_records: response.total_records || 0
            };
        },
        ajaxError: function (error) {
            console.error("Tabulator AJAX Error:", error);
            alert("Failed to load tasks. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            { title: "PROCESS NAME", field: "processName", minWidth: 200, widthGrow: 2, headerSort: true, frozen: true, headerSort: true },
            { title: "PROCESS DESCRIPTION", field: "description", widthGrow: 2, minWidth: 400, frozen: true, headerSort: false },
            { title: "ATTACHED UNIT", field: "unitName", minWidth: 250 },
            { title: "PROCESS MANAGER", field: "assigneeName", minWidth: 400 },
            {
                title: "REQUEST",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="requestApproval(${rowData.id})">
                        <span><i class="mdi mdi-eye-arrow-right-outline" aria-hidden="true"></i></span>
                        <span>REQUEST</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "DELETE",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteProcess(${rowData.id})">
                            <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                            <span>DELETE</span>
                        </button>`;
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessNewSearch();
}

function initProcessNewSearch() {
    const searchInput = $('#newSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processNewTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "description", type: "like", value: searchTerm },
                        { field: "ownerName", type: "like", value: searchTerm },
                        { field: "assigneeName", type: "like", value: searchTerm },
                        { field: "unitName", type: "like", value: searchTerm }
                    ]
                ]);
                processNewTable.setPage(1, true);
            } else {
                processNewTable.clearFilter();
            }
        }, 300);
    });
}

function requestApproval(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid Process ID.");
        return;
    }

    Swal.fire({
        title: "REQUEST APPROVAL",
        text: "Send Process For Approval. Do you want to proceed?",
        showCancelButton: true,
        confirmButtonColor: "#5E2A5E",
        confirmButtonText: "Request Approval",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/operations/workflow/processes/approval/request/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getAntiForgeryToken()
            },
            success: function (res) {

                if (!res || typeof res !== "object") {
                    Swal.fire("System Error", "Unexpected server response.");
                    return;
                }

                if (res.success) {

                    Swal.fire({
                        title: "REQUEST APPROVAL",
                        text: res.message || "Request has been submitted to the Head Of Department Operations for review."
                    });

                    if (processNewTable?.replaceData) {
                        processNewTable.replaceData();
                    }

                } else {
                    Swal.fire("REQUEST APPROVAL", res.message || "Submission Failed.");
                }
            },
            error: function (xhr, status, error) {
                console.error("Submission error:", error);
                console.error("Response:", xhr.responseText);
                let msg = "Submission failed.";
                if (xhr.responseJSON && typeof xhr.responseJSON.message === "string") {
                    msg = xhr.responseJSON.message;
                }
                Swal.fire("REQUEST APPROVAL", msg);
            }
        });
    });
}

function deleteProcess(id) {
    if (!id && id !== 0) {
        Swal.fire("Error", "Invalid id for delete.", "error");
        return;
    }

    Swal.fire({
        title: "Delete Process",
        text: "Are you sure you want to delete this process?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/operations/workflow/processes/registers/delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getAntiForgeryToken()
            },
            success: function (res) {

                //...avoid crashing if server returns HTML or non-JSON
                if (!res || typeof res !== "object") {
                    console.warn("Unexpected server response:", res);

                    Swal.fire("Deleted Process", "Unexpected server response.");
                    return;
                }

                if (res.success) {

                    Swal.fire({
                        title: "Deleted Process",
                        text: res.message || "Process deleted successfully."
                    });

                    if (processNewTable?.replaceData) {
                        processNewTable.replaceData();
                    }

                } else {
                    Swal.fire("Delete Record", res.message || "Failed to delete process");
                }
            },
            error: function (xhr) {

                let msg = "Failed to delete process";
                if (xhr.responseJSON && typeof xhr.responseJSON.message === "string") {
                    msg = xhr.responseJSON.message;
                }

                Swal.fire("Delete Record", msg);
            }
        });
    });
}

function getAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

}

$(document).ready(function () {

    initProcessNewListTable();

    $('#processReviewForm').on('submit', function (e) {
        e.preventDefault();
    });

});
