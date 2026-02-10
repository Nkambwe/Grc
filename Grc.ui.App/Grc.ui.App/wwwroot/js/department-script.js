let departmentTable;

function initDepartmentTable() {
    departmentTable = new Tabulator("#depatTable", {
        ajaxURL: "/admin/support/organization/departments-all",
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
                        ["departmentCode", "departmentAlias", "departmentName"].includes(f.field)
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
            alert("Failed to load departments. Please try again.");
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
            { title: "CODE", field: "departmentCode", width: 150, headerFilter: "input" },
            {
                title: "DEPARTMENT NAME",
                field: "departmentName",
                minWidth: 250,
                headerFilter: "input",
                frozen: true,
                formatter: cell =>
                    `<span class="clickable-title" onclick="viewDepartmentRecord(${cell.getRow().getData().id})">
                         ${cell.getValue()}
                     </span>`
            },
            { title: "ALIAS", field: "departmentAlias", width: 150, headerFilter: "input" },
            {
                title: "ACTION",
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false,
                formatter: cell => `
                     <div style="display:flex; justify-content:center; align-items:center; height:100%;">
                        <button class="grc-table-btn grc-btn-delete grc-delete-action"
                                onclick="deleteDepartment(${cell.getRow().getData().id})">
                            <i class="mdi mdi-trash-can-outline"></i> DELETE
                        </button>
                    </div>`
            },
            { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });
}

function viewDepartmentRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving department record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findDepartment(id)
        .then(record => {
            Swal.close();
            if (record) {
                openDepartmentPane('Modify department record', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Department not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load department details.' });
        });
}

function findDepartment(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/organization/departments-retrieve/${id}`,
            type: "POST",
            dataType: "json",
            success: function (response) {
                if (response.success === true && response.data) {
                    resolve(response.data);
                } else {
                    reject(response.message || "Department not found");
                }
            },
            error: function (xhr) {
                reject(xhr.responseJSON?.message || "Server error");
            }
        });
    });
}

function openDepartmentPane(title, record, isEdit) {
    console.log(record);
    $('#isEdit').val(isEdit);
    $('#departmentId').val(record?.id || 0);
    $('#departmentCode').val(record?.code || '');
    $('#departmentAlias').val(record?.alias || '');
    $('#departmentName').val(record?.departmentName || '');
    $('#departmentHead').val(record?.departmentHead || '');
    $('#departmentHeadEmail').val(record?.departmentHeadEmail || '');
    $('#departmentHeadContact').val(record?.departmentHeadContact || '');
    $('#departmentHeadDesignation').val(record?.departmentHeadDesignation || '');

    //..add list of numits
    addDepartmentUnits(record.units);

    $('#deptTitle').text(title);
    $('#deptOverLay').addClass('active');
    $('#deptContainer').addClass('active');
}
function addDepartmentUnits(units) {

    const tableBody = document.querySelector('#unitListTable tbody');
    tableBody.innerHTML = '';

    if (!units || units.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="3" class="text-center text-muted">
                    No units available
                </td>
            </tr>`;
        return;
    }

    units.forEach((unit, index) => {
        const tr = document.createElement('tr');

        tr.innerHTML = `
            <td>${unit.unitCode || ''}</td>
            <td>${unit.unitName || ''}</td>
            <td class="text-center">
                <button class="grc-table-btn grc-btn-delete"
                        onclick="editUnit(this, ${unit.id || 0})">
                    <i class="mdi mdi-trash-can-outline"></i>
                </button>
            </td>
        `;

        tableBody.appendChild(tr);
    });
}

function addNewUnit() {
    const parentId = $("#departmentId").val() || 0;
    openUnitPane("New Department Unit", {
        id:0,
        departmentId:parentId,
        unitCode:'',
        unitName:'',
        unitHead:'',
        departmentHead:'',
        unitContactEmail:'',
        unitContactNumber:'',
        unitHeadDesignation:''
    }, false)
}

function addDepartmentUnits(units) {
    const tableBody = document.querySelector('#unitListTable tbody');
    tableBody.innerHTML = '';

    if (!units || units.length === 0) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="3" class="text-center text-muted py-4">
                    <i class="mdi mdi-information-outline me-1"></i>
                    No units available
                </td>
            </tr>`;
        return;
    }

    units.forEach((unit) => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${unit.unitCode || ''}</td>
            <td>${unit.unitName || ''}</td>
            <td class="unit-action-container">
                <button class="grc-table-btn grc-btn-edit me-2 text-center" 
                        onclick="editUnit(${unit.id || 0})"
                        title="Edit Unit">
                    <i class="mdi mdi-pencil-outline"></i>
                </button>
                <button class="grc-table-btn grc-btn-delete text-center" 
                        onclick="deleteUnit(${unit.id || 0})"
                        title="Delete Unit">
                    <i class="mdi mdi-trash-can-outline"></i>
                </button>
            </td>
        `;
        tableBody.appendChild(tr);
    });
}

// Handler for editing unit
function editUnit(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving department unit record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });
    findUnit(id)
        .then(record => {
            Swal.close();
            if (record) {
                openUnitPane('Modify unit record', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Department unit not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load unit details.' });
        });
}

function findUnit(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/admin/support/organization/departments/unit-retrieve/${id}`,
            type: "POST",
            dataType: "json",
            success: function (response) {
                if (response.success === true && response.data) {
                    resolve(response.data);
                } else {
                    reject(response.message || "Department unit not found");
                }
            },
            error: function (xhr) {
                reject(xhr.responseJSON?.message || "Server error");
            }
        });
    });
}

function openUnitPane(title, record, isEdit) {
    console.log(record);
    $('#isUnitEdit').val(isEdit);
    $('#unitId').val(record?.id || 0);
    $('#parentId').val(record?.departmentId || 0);
    $('#unitCode').val(record?.unitCode || '');
    $('#unitName').val(record?.unitName || '');
    $('#unitHead').val(record?.unitHead || '');
    $('#unitContactEmail').val(record?.unitContactEmail || '');
    $('#unitContactNumber').val(record?.unitContactNumber || '');
    $('#unitHeadDesignation').val(record?.unitHeadDesignation || '');

    $('#unitTitle').text(title);
    $('#unit2OverLay').addClass('active');
    $('#unit2Container').addClass('active');
}

function saveUnit(e) {
    e.preventDefault();

    const isEdit = $('#isUnitEdit').val();
    const id = parseInt($('#unitId').val()) || 0;
    const departmentId = parseInt($('#parentId').val()) || 0;
    const unitCode = $('#unitCode').val();
    const unitName = $('#unitName').val();
    const unitHead = $('#unitHead').val();
    const unitContactEmail = $('#unitContactEmail').val();
    const unitContactNumber =  $('#unitContactNumber').val();
    const unitHeadDesignation =  $('#unitHeadDesignation').val();
    
    const unitData = {
        id: id,
        departmentId:departmentId,
        unitCode: unitCode,
        unitName: unitName,
        unitHead:unitHead,
        unitContactEmail:unitContactEmail,
        unitContactNumber:unitContactNumber,
        unitHeadDesignation:unitHeadDesignation
    }

    let errors = [];
    if (!unitData.unitName)
        errors.push("Unit name is required.");

    if (!unitData.unitCode)
        errors.push("unit code field is required");

    if (!unitData.unitHead)
        errors.push("Unit head name is required.");

    if (!unitData.unitContactEmail)
        errors.push("unit head email field is required");

    if (!unitData.unitContactNumber)
        errors.push("Unit head Phone number field is required");
        
    if (!unitData.unitHeadDesignation)
        errors.push("Unit head designation/title field is required");

    if (errors.length > 0) {
        departmentErrorField("#unitName", !unitData.unitName);
        departmentErrorField("#unitCode", !unitData.unitCode);
        departmentErrorField("#unitHead", !unitData.unitHead);
        departmentErrorField("#unitContactEmail", !unitData.unitContactEmail);
        departmentErrorField("#unitContactNumber", !unitData.unitContactNumber);
        departmentErrorField("#unitHeadDesignation", !unitData.unitHeadDesignation);
        Swal.fire({
            title: "Department unit data Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveUnitRecord(unitData, isEdit);
}

function saveUnitRecord(record, isEdit) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/support/organization/departments/update-unit"
        : "/admin/support/organization/departments/create-unit";

    Swal.fire({
        title: isEdit ? "Updating unit record..." : "Saving unit record...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    console.log("Unit record >> ", record);
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(record),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getDepartmentToken()
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

            Swal.fire(res.message || "Unit record saved successfully")
                .then(() => {
                    //..close panel
                    closeUnitPanel();
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

function closeUnitPanel() {
    $('#unit2OverLay').removeClass('active');
    $('#unit2Container').removeClass('active');
}

function deleteUnit(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Department Unit",
        text: "Are you sure you want to delete this unit?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {

        if (!result.isConfirmed)
            return;

        $.ajax({
            url: `/admin/support/organization/departments/unit-delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getDepartmentToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Unit deleted successfully.");
                    if (departmentTable) {
                        departmentTable.replaceData();
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

function saveDepartment(e) {
    e.preventDefault();
    const id = parseInt($('#departmentId').val()) || 0;
    const departmentName = $('#departmentName').val();
    const code = $('#departmentCode').val();
    const alias = $('#departmentAlias').val();
    const isEdit = $('#isEdit').val();
    const departmentHead =  $('#departmentHead').val();
    const departmentHeadEmail =  $('#departmentHeadEmail').val();
    const departmentHeadContact =  $('#departmentHeadContact').val();
    const departmentHeadDesignation =  $('#departmentHeadDesignation').val();
    
    const deptData = {
        id: id,
        departmentName: departmentName,
        departmentCode: code,
        departmentAlias: alias,
        departmentHead:departmentHead,
        departmentHeadEmail:departmentHeadEmail,
        departmentHeadContact:departmentHeadContact,
        departmentHeadDesignation:departmentHeadDesignation
    }

    let errors = [];
    if (!deptData.departmentName)
        errors.push("Department name is required.");

    if (!deptData.departmentCode)
        errors.push("Department code field is required");

    if (!deptData.departmentAlias)
        errors.push("Department alias field is required");

    if (!deptData.departmentHead)
        errors.push("HOD name is required.");

    if (!deptData.departmentHeadEmail)
        errors.push("HOD email field is required");

    if (!deptData.departmentHeadContact)
        errors.push("HOD Phone number field is required");
        
    if (!deptData.departmentHeadDesignation)
        errors.push("HOD designation/title field is required");

    if (errors.length > 0) {
        departmentErrorField("#departmentName", !deptData.departmentName);
        departmentErrorField("#departmentCode", !deptData.departmentCode);
        departmentErrorField("#departmentAlias", !deptData.departmentAlias);
        departmentErrorField("#departmentHead", !deptData.departmentHead);
        departmentErrorField("#departmentHeadEmail", !deptData.departmentHeadEmail);
        departmentErrorField("#departmentHeadContact", !deptData.departmentHeadContact);
        departmentErrorField("#departmentHeadDesignation", !deptData.departmentHeadDesignation);
        Swal.fire({
            title: "Department data Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    //..call backend
    saveDepartmentRecord(deptData, isEdit);
}

function saveDepartmentRecord(record, isEdit) {
    const url = (isEdit === true || isEdit === "true")
        ? "/admin/support/organization/departments/update-department"
        : "/admin/support/organization/departments/create-department";

    Swal.fire({
        title: isEdit ? "Updating department record..." : "Saving department record...",
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
            'X-CSRF-TOKEN': getDepartmentToken()
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

            Swal.fire(res.message || "Departemnt record saved successfully")
                .then(() => {
                    //..close panel
                    closeDepartmentPanel();
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

function deleteDepartment(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Department",
        text: "Are you sure you want to delete this department?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {

        if (!result.isConfirmed)
            return;

        $.ajax({
            url: `/admin/support/organization/departments/delete/${encodeURIComponent(id)}`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getDepartmentToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Department deleted successfully.");
                    if (departmentTable) {
                        departmentTable.replaceData();
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

function closeDepartmentPanel() {
    $('#deptOverLay').removeClass('active');
    $('#deptContainer').removeClass('active');
}

function getDepartmentToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

$(document).ready(function () {
    initDepartmentTable();
   
    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });

    $('.action-btn-new-dept').on('click', function () {
        openDepartmentPane("New Department",
        {
            id: 0,
            departmentName: '',
            departmentCode: '',
            departmentAlias: '',
            departmentHead:'',
            departmentHeadEmail:'',
            departmentHeadContact:'',
            departmentHeadDesignation:''
        },
        false);
    });

    $('#departmentForm').on('submit', function (e) {
        e.preventDefault();
    });
    
    $('#unitForm').on('submit', function (e) {
        e.preventDefault();
    });

});

function departmentErrorField(selector, hasError, message) {
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