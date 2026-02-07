let branchesTable;

function initBranchTable() {
    branchesTable = new Tabulator("#branchesTable", {
        ajaxURL: "/admin/support/organization/branches-all",
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

                //..sorting
                if (params.sort && params.sort.length > 0) {
                    requestBody.sortBy = params.sort[0].field;
                    requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
                }

                //..filtering
                if (params.filter && params.filter.length > 0) {
                    let filter = params.filter.find(f =>
                        ["branchName", "solId"].includes(f.field)
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
            alert("Failed to load branches. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                formatter: () => `<span class="record-tab"></span>`,
                width: 40,
                headerSort: false,
                frozen: true
            },
            {
                title: "BRANCH NAME",
                field: "branchName",
                minWidth: 250,
                headerFilter: "input",
                frozen: true,
                formatter: cell =>
                    `<span class="clickable-title" onclick="viewBranchRecord(${cell.getRow().getData().id})">
                         ${cell.getValue()}
                     </span>`
            },
            { title: "SOLID", field: "solId", width: 150, headerFilter: "input"},
            {
                title: "CREATED ON",
                field: "createdOn",
                hozAlign: "center",
                headerHozAlign: "center",
                headerFilter: "input",
                formatter: cell => {
                    if (!cell.getValue()) return "";
                    return new Date(cell.getValue()).toLocaleDateString("en-GB");
                }
            },
            {
                title: "ACTION",
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false,
                formatter: cell => `
                     <div style="display:flex; justify-content:center; align-items:center; height:100%;">
                        <button class="grc-table-btn grc-btn-delete grc-delete-action"
                                onclick="deleteBranch(${cell.getRow().getData().id})">
                            <i class="mdi mdi-trash-can-outline"></i> DELETE
                        </button>
                    </div>`
            },
            { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });
}

function viewBranchRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving branch record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findBranch(id)
        .then(record => {
            Swal.close();
            if (record) {
                openBranchPane('Modify branch record', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Branch not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load branch details.' });
        });
}

function findBranch(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/organization/branches-retrieve/${id}`,
            type: "POST",
            dataType: "json",
            success: function (response) {
                if (response.success === true && response.data) {
                    resolve(response.data);
                } else {
                    reject(response.message || "Branch not found");
                }
            },
            error: function (xhr) {
                reject(xhr.responseJSON?.message || "Server error");
            }
        });
    });
}


function openBranchPane(title, record, isEdit) {
    $('#isEdit').val(isEdit);
    $('#branchId').val(record?.id || 0);
    $('#branchName').val(record?.branchName || '');
    $('#solId').val(record?.solId || '');
    $('#branchTitle').text(title);
    $('#branchOverLay').addClass('active');
    $('#branchContainer').addClass('active');
}

function saveBranch(e) {
    e.preventDefault();
    const id = parseInt($('#branchId').val()) || 0;
    const branchName = $('#branchName').val();
    const solId = $('#solId').val();
    const isEdit = $('#isEdit').val();

    const branchData = {
        id: id,
        branchName: branchName,
        solId: solId
    }

    let errors = [];
    if (!branchData.branchName)
        errors.push("Branch name is required.");

    if (!branchData.solId)
        errors.push("Sol ID field is required");

    if (errors.length > 0) {
        branchErrorField("#branchName", !branchData.branchName);
        branchErrorField("#solId", !branchData.solId);
        Swal.fire({
            title: "Branch data Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    console.log(branchData);
    //..call backend
    saveBranchRecord(isEdit, branchData, isEdit);
}

function saveBranchRecord(isEdit, record, isEdit) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/support/organization/branches/update-branch"
        : "/admin/support/organization/branches/create-branch";

    Swal.fire({
        title: isEdit ? "Updating branch record..." : "Saving branch record...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    console.log("Branch record >> ", record);
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getBranchToken()
        },
        success: function (res) {
            Swal.close();
            if (!res.success) {
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            Swal.fire(res.message || "Branch record saved successfully")
                .then(() => {
                    //..close panel
                    closeBranchPanel();
                    window.location.reload();
                });
        },
        error: function (xhr) {
            Swal.close();

            let errorMessage = "Unexpected error occurred.";
            try {
                let response = JSON.parse(xhr.responseText);
                if (response.message) errorMessage = response.message;
            } catch (e) { }

            Swal.fire({
                title: isEdit ? "Update Failed" : "Save Failed",
                text: errorMessage
            });
        }
    });
}

function deleteBranch(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Branch",
        text: "Are you sure you want to delete this branch?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {

        if (!result.isConfirmed)
            return;

        $.ajax({
            url: `/admin/support/organization/branches/delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getBranchToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Branch deleted successfully.");
                    if (branchesTable) {
                        branchesTable.replaceData();
                    }
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function (xhr, status, error) {
                toastr.error(xhr.responseJSON?.message || "Request failed.");
            }
        });
    });
}

function closeBranchPanel() {
    $('#branchOverLay').removeClass('active');
    $('#branchContainer').removeClass('active');
}

function getBranchToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

$(document).ready(function () {

    initBranchTable();

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });

    $('.action-btn-new').on('click', function () {
        openBranchPane("New Branch",
            {
                id: 0,
                branchName: '',
                solId: ''
            },
            false);
    });

    $('#branchForm').on('submit', function (e) {
        e.preventDefault();
    });

});

function branchErrorField(selector, hasError, message) {
    const $field = $(selector);
    const $formGroup = $field.closest('.form-group, .mb-3, .col-sm-8');

    // Remove existing error
    $field.removeClass('is-invalid');
    $formGroup.find('.field-error').remove();

    if (hasError) {
        $field.addClass('is-invalid');
        if (message) {
            $formGroup.append(`<div class="field-error text-danger small mt-1">${message}</div>`);
        }
    }
}