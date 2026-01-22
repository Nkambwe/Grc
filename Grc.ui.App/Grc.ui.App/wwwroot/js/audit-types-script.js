let auditTypeTable;

function initAuditType2Table() {
    auditTypeTable = new Tabulator("#auditTypetable", {
        ajaxURL: "/grc/compliance/audit/types",
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
                        ["typeCode", "typeName", "description"].includes(f.field)
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
            alert("Failed to load audit types. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            { title: "TYPE CODE", field: "typeCode", minWidth: 200, frozen: true, headerSort: true },
            {
                title: "TYPE NAME",
                field: "typeName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewAuditType(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "DESCRIPTION", field: "description", minWidth: 200, widthGrow: 3 },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteAuditType(${rowData.id})">
                            <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                            <span>DELETE</span>
                        </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    //..initialize search
    initAuditTypeSearch();
}

function initAuditTypeSearch() {

}

$('.action-btn-audit-type-new').on('click', function () {
    addAuditType();
});

function addAuditType() {
    openAuditTypePanel('Add Audit Type', {
        id: 0,
        typeCode: '',
        typeName: '',
        description: '',
        isDeleted: false,
    }, false);
}

function openAuditTypePanel(title, record, isEdit) {
    $('#isTypeEdit').val(isEdit);
    $('#typeId').val(record.id);
    $('#typeCode').val(record.typeCode || '');
    $('#typeName').val(record.typeName || '');
    $('#description').val(record.description || '');
    $('#isDeleted').prop('checked', record.isDeleted);

    //load dialog window
    $('#auditTypeTitle').text(title);
    $('#auditTypeOverlay').addClass('active');
    $('#auditTypePanel').addClass('active');
}

function viewAuditType(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving type...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    console.log("ID >> " + id);
    findTypeRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openAuditTypePanel('Edit Type', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Audit type not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load type details.' });
        });
}

function findTypeRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/audit/types/type-retrieve/${id}`,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.success && response.data) {
                    resolve(response.data);
                } else {
                    resolve(null);
                }
            },
            error: function () {
                reject();
            }
        });
    });
}

function deleteAuditType(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete audit type",
        text: "Are you sure you want to delete this type?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        const url = `/grc/compliance/audits/types/type-delete/${encodeURIComponent(id)}`;
        $.ajax({
            url: url,
            type: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getTypeToken()
            },
            success: function (res) {

                if (res && res.success) {
                    toastr.success(res.message || "Type deleted successfully.");
                    auditTypeTable.replaceData();
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function (xhr, status, error) {
                let msg = "Request failed.";
                try {
                    const json = xhr.responseJSON || JSON.parse(xhr.responseText || "{}");
                    if (json && json.message) msg = json.message;
                } catch (e) { }
                toastr.error(msg);
            }
        });
    });
}

function saveAuditTypePane(e) {
    e.preventDefault();
    let id = Number($('#typeId').val()) || 0;
    let isEdit = $('#isTypeEdit').val() || false;

    //..build record payload from form
    let recordData = {
        id: id,
        typeCode: $('#typeCode').val()?.trim(),
        typeName: $('#typeName').val()?.trim(),
        description: $('#description').val()?.trim(),
        isDeleted: $('#isIssueDeleted').prop('checked'),
    };

    //..validate required fields
    let errors = [];
    if (!recordData.typeCode)
        errors.push("Type code field is required.");

    if (!recordData.typeName)
        errors.push("Type name field is required.");

    if (errors.length > 0) {
        highlightError("#typeCode", !recordData.typeCode);
        highlightError("#typeName", !recordData.typeName);
        Swal.fire({
            title: "Audit type Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveAuditTypeRecord(isEdit, recordData);
}

function saveAuditTypeRecord(isEdit, record) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/audits/types/type-update"
        : "/grc/compliance/audits/types/type-create";

    Swal.fire({
        title: isEdit ? "Updating type..." : "Saving type...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getTypeToken()
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

            Swal.fire(res.message || (isEdit ? "Type updated successfully" : "Type created successfully"));

            // reload table
            auditTypeTable.replaceData();

            //..close panel
            closeAuditTypePane();

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

function getTypeToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function closeAuditTypePane() {
    $('#auditTypeOverlay').removeClass('active');
    $('#auditTypePanel').removeClass('active');
}

$('.action-btn-audit-home').on('click', function () {
    try {
        window.location.href = '/grc/compliance/audit/dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

$(document).ready(function () {
    initAuditType2Table();

    $('#auditTypeForm').on('submit', function (e) {
        e.preventDefault();
    });
});