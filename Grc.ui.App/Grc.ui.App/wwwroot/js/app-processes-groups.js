let processGroupTable;

function initProcessGroupListTable() {
    processGroupTable = new Tabulator("#processGroupTable", {
        ajaxURL: "/operations/workflow/processes/groups/all",
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
                        ["groupName", "groupDescription"].includes(f.field)
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
            alert("Failed to load process groups. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "GROUP NAME",
                field: "groupName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewProcessGroup(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "GROUP DESCRIPTION", field: "groupDescription", widthGrow: 1, minWidth: 400, frozen: true, headerSort: false },
            {
                title: "ACTIVE",
                field: "isDeleted",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let color = rowData.aligned !== true ? "#28a745" : "#dc3545";
                    let text = rowData.aligned !== true ? "YES" : "NO";

                    return `<div style="
                            display:flex;
                            align-items:center;
                            justify-content:center;
                            width:100%;
                            height:100%;
                            border-radius:50px;
                            color:${color};
                            font-weight:bold;">
                        ${text}
                         </div>`;
                    },
                hozAlign: "center",
                headerHozAlign: "center",
                maxWidth: 400
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteProcessGroup(${rowData.id})">
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
    initProcessGroupSearch();

};

function initProcessGroupSearch() {
    const searchInput = $('#groupSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processGroupTable.setFilter([
                    [
                        { field: "groupName", type: "like", value: searchTerm },
                        { field: "groupDescription", type: "like", value: searchTerm }
                    ]
                ]);
                processGroupTable.setPage(1, true);
            } else {
                processGroupTable.clearFilter();
            }
        }, 300);
    });

}

function findProcessGroup(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/groups/retrieve/${encodeURIComponent(id)}`,
            type: 'GET',
            headers: { 'X-Requested-With': 'XMLHttpRequest' },
            success: function (res) {
                if (res && res.success) {
                    resolve(res.data);
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

function viewProcessGroup(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving group record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findProcessGroup(id)
        .then(record => {
            Swal.close();
            if (record) {
                openProcessGroupEditor('Edit Process Group', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Group record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load group details.' });
        });
}

function openProcessGroupEditor(title, group, isEdit) {
    $("#groupId").val(group?.id || "");
    $("#isEdit").val(isEdit);
    $("#groupName").val(group?.groupName || "");
    $("#groupDescription").val(group?.groupDescription || "");

    $("#groupListContainer").html("<div class='text-muted p-2'>Loading process groups...</div>");

    if (isEdit) {
        const processes = Array.isArray(group.processes) ? group.processes : [];
        let html = "<div class='role-group-list'>";
        processes.forEach(p => {
            const checked = p.isAssigned ? "checked" : "";
            html += `<div class="form-check">
                            <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${p.id}" value="${p.id}" ${checked}>
                            <label class="form-check-label" for="perm-${p.id}">${p.processName}</label>
                        </div>`;
        });
        html += "</div>";
        $("#groupListContainer").html(html);

        //..show deactivation checkbox
        $("#IsDeletedBox").show(); 
    } else {
        console.log("load all processes");
        $.get("/operations/workflow/processes/groups/processes-min", function (res) {
            const processes = Array.isArray(res?.processes) ? res.processes : [];

            let html = "<div class='role-group-list'>";
            processes.forEach(p => {
                const checked = p.isAssigned ? "checked" : "";
                html += `
                <div class="form-check">
                    <input type="checkbox" class="form-check-input perm-checkbox" id="perm-${p.id}" value="${p.id}" ${checked}>
                    <label class="form-check-label" for="perm-${p.id}">
                        ${p.processName}
                    </label>
                </div>`;
            });
            html += "</div>";
            $("#groupListContainer").html(html);

        }).fail(() => {
            $("#groupListContainer").html("<div class='text-danger'>Failed to load processes.</div>");
        });

        //..hide deactivation checkbox
        $("#IsDeletedBox").hide(); 
    }
    
    //..show overlay panel
    $('#groupPanelTitle').text(title);
    $('.process-overlay').addClass('active');
    $('#collapsePanel').addClass('active');
}

function saveProcessGroupRecord(e) {

    e.preventDefault();

    let isEdit = $('#isEdit').val();
    let recordData = {
        id: parseInt($('#groupId').val()) || 0,
        groupName: $('#groupName').val()?.trim(),
        groupDescription: $('#groupDescription').val()?.trim(),
        isDeleted: $('#isDeleted').prop('checked'),
        processes: $(".perm-checkbox:checked").map((_, el) => parseInt(el.value, 10)).get()
    };

    // --- validate required fields ---
    let errors = [];
    if (!recordData.groupName)
        errors.push("Group name is required.");
    if (!recordData.groupDescription)
        errors.push("Group description is required.");

    if (errors.length > 0) {
        highlightProcessGroupField("#groupName", !recordData.groupName);
        highlightProcessGroupField("#groupDescription", !recordData.groupDescription);
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..save group
    saveGroup(isEdit, recordData);
}

function saveGroup(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/operations/workflow/processes/groups/update"
        : "/operations/workflow/processes/groups/create";

    Swal.fire({
        title: isEdit ? "Updating group..." : "Saving group...",
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
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getProcessGroupAntiForgeryToken()
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

            if (processGroupTable) {
                if (isEdit && res.data) {
                    processGroupTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    processGroupTable.addRow(res.data, true);
                } else {
                    processGroupTable.replaceData();
                }
            }

            closeProcessGroupPanel()();
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

function highlightProcessGroupField(selector, hasError, message) {
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

function createGroup() {
    openProcessGroupEditor('New Process Group', {
        id: 0,
        isEdit: false,
        groupName: '',
        groupDescription: ''
    }, false);
}

function closeProcessGroupPanel() {
    $('.process-overlay').removeClass('active');
    $('#collapsePanel').removeClass('active');
}

function deleteProcessGroup(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Group",
        text: "Are you sure you want to delete this group?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/operations/workflow/processes/groups/delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getProcessGroupAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Process deleted successfully.");
                    if (roleGroupTable) {
                        roleGroupTable.replaceData();
                    }
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function (xhr, status, error) {
                console.error("Delete error:", error);
                console.error("Response:", xhr.responseText);
                toastr.error(xhr.responseJSON?.message || "Request failed.");
            }
        });
    });
}

function getProcessGroupAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

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