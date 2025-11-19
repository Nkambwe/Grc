let processRegisterTable;
let dateList = {};
var uploadedFiles = [];
var fileCounter = 0;
function initProcessRegisterTable() {
    processRegisterTable = new Tabulator("#processRegisterTable", {
        ajaxURL: "/operations/workflow/processes/register/all",
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
                        ["processName", "description", "typeName", "ownerName", "assigneeName", "unitName"].includes(f.field)
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
            alert("Failed to load processes. Please try again.");
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                title: "PROCESS NAME",
                field: "processName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewProcess(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "PROCESS DESCRIPTION", field: "description", widthGrow: 1, minWidth: 400, frozen: true, headerSort: false },
            { title: "PROCESS OWNER", field: "ownerName", headerSort: true, minWidth: 200 },
            { title: "ATTACHED UNIT", field: "unitName", minWidth: 250 },
            { title: "PROCESS MANAGER", field: "assigneeName", minWidth: 400 },
            {
                title: "ON FILE",
                field: "fileName",
                minWidth: 100,
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<span class="clickable-title" onclick="viewFile(${rowData.fileName})">${rowData.fileName}</span>`
                }
            },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let isLocked = rowData.isLockProcess ?? false;
                    if (isLocked) {
                        return `<button class="grc-table-btn disabled" disabled>
                            <span><i class="mdi mdi-lock-outline" aria-hidden="true"></i></span>
                            <span>LOCKED</span>
                        </button>`;
                    } else {
                        return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteProcess(${rowData.id})">
                            <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                            <span>DELETE</span>
                        </button>`;
                    }
                   
                },
                width: 150,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            }
        ]
    });

    // Search init
    initProcessSearch();
}

function createProcess() {
    openProcessEditor('New Process', {
        // Populate form fields
        id:0,
        isEdit:false,
        processName:'',
        description:'',
        typeId: 0,
        isDeleted: false,

        //..process status
        processStatus:0,
        comment: '',
        onholdReason:'',

        //..file info
        originalOnFile: true,
        fileName:'',
        CurrentVersion: '',

        //..approval info
        approvalStatus:'',
        approvalComment: '',
        effectiveDate:'',

        //..responsibility
        unitId: 0,
        ownerId:0,
        assigneedId:0
    }, false);
}

function findProcess(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/operations/workflow/processes/registers/retrieve/${encodeURIComponent(id)}`,
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

function openProcessEditor(title, process, isEdit) {
    var isLocked = process?.isLockProcess || false;
    var status = process?.processStatus || 0;

    // Populate form fields
    $("#processId").val(process?.id || "");
    $("#isEdit").val(isEdit);
    $("#processName").val(process?.processName || "");
    $("#processDescription").val(process?.description || "");
    $("#typeId").val(process?.typeId || 0).trigger('change.select2');
    $('#isDeleted').prop('checked', process?.isDeleted || false);
    $("#effectiveDate").val(process?.effectiveDate);
    $('#isLockProcess').prop('checked', isLocked);
    $("#onholdReason").val(process?.onholdReason || "");
    $("#processStatus").val(status || 0).trigger('change.select2');
    $("#comment").val(process?.comment || "");
    $("#fileName").val(process?.fileName || "");
    $("#fileVersion").val(process?.CurrentVersion || "");
    $("#unitId").val(process?.unitId || 0).trigger('change.select2');
    $("#ownerId").val(process?.ownerId || 0).trigger('change.select2');
    $("#assigneedId").val(process?.assigneedId || 0).trigger('change.select2');
    $("#hodApprovalOn").val(process?.hodApprovalOn || "");
    $("#hodApprovalStatus").val(process?.hodApprovalStatus || "PENDING").trigger('change.select2');
    $("#hoApprovalComment").val(process?.hoApprovalComment || "");
    $("#riskApprovalOn").val(process?.riskApprovalOn || "");
    $("#riskApprovalStatus").val(process?.riskApprovalStatus || "PENDING").trigger('change.select2');
    $("#riskApprovalComment").val(process?.riskApprovalComment || "");
    $("#complianceApprovalOn").val(process?.complianceApprovalOn || "");
    $("#complianceApprovalStatus").val(process?.complianceApprovalStatus || "PENDING").trigger('change.select2');
    $("#complianceApprovalComment").val(process?.complianceApprovalComment || "");
    $('#needsBranchOperations').prop('checked', process?.needsBranchOperations || false);
    $("#branchOpsApprovalOn").val(process?.branchOpsApprovalOn || "");
    $("#branchOpsApprovalStatus").val(process?.branchOpsApprovalStatus || "PENDING").trigger('change.select2');
    $("#branchOpsApprovalComment").val(process?.branchOpsApprovalComment || "");
    $('#needsCreditReview').prop('checked', process?.needsCreditReview || false);
    $("#creditApprovalOn").val(process?.creditApprovalOn || "");
    $("#creditApprovalStatus").val(process?.creditApprovalStatus || "PENDING").trigger('change.select2');
    $("#creditApprovalComment").val(process?.creditApprovalComment || "");
    $('#needsTreasuryReview').prop('checked', process?.needsTreasuryReview || false);
    $("#treasuryApprovalOn").val(process?.treasuryApprovalOn || "");
    $("#treasuryApprovalStatus").val(process?.treasuryApprovalStatus || "PENDING").trigger('change.select2');
    $("#treasuryApprovalComment").val(process?.treasuryApprovalComment || "");
    $('#needsFintechReview').prop('checked', process?.needsFintechReview || false);
    $("#fintechApprovalOn").val(process?.fintechApprovalOn || "");
    $("#fintechApprovalStatus").val(process?.fintechApprovalStatus || "PENDING").trigger('change.select2');
    $("#fintechApprovalComment").val(process?.fintechApprovalComment || "");

    // Hide "on-hold reason" if status = 3
    if (status === 3) {
        $("#onHoldBox").hide();
    } else {
        $("#onHoldBox").show();
    }

    //..hide lock process checkbox if not edit mode
    if (!isEdit) {
        $("#lockProcess").hide(); 
        $("#IsDeletedBox").hide(); 
    } else {
        $("#lockProcess").show();
        $("#IsDeletedBox").show(); 
    }

    //..disable all fields if editing a locked process
    //if (isEdit && isLocked) {
    //    //..disable all inputs inside form
    //    $("#processForm :input").prop("disabled", true); 

    //    //..allow cancel/close still usable
    //    $("#closeButton, #cancelButton").prop("disabled", false); 
    //} else {
    //    //..ensure fields are enabled when not locked
    //    $("#processForm :input").prop("disabled", false); 
    //}

    // Show overlay panel
    $('#processPanelTitle').text(title);
    $('.process-overlay').addClass('active');
    $('#collapsePanel').addClass('active');
}

function viewProcess(id){
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving process record...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    findProcess(id)
        .then(record => {
            Swal.close();
            if (record) {
                openProcessEditor('Edit Process', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Process record found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load process details.' });
        });
}

function deleteProcess(id) {
    alert('Delete Process >>> ' + id);
}

function viewFile(fileName) {
    alert(`View File >>> ${fileName}`);
}

function initProcessSearch() {
    const searchInput = $('#processesSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                processRegisterTable.setFilter([
                    [
                        { field: "processName", type: "like", value: searchTerm },
                        { field: "description", type: "like", value: searchTerm },
                        { field: "typeName", type: "like", value: searchTerm },
                        { field: "ownerName", type: "like", value: searchTerm },
                        { field: "assigneeName", type: "like", value: searchTerm },
                        { field: "unitName", type: "like", value: searchTerm }
                    ]
                ]);
                processRegisterTable.setPage(1, true);
            } else {
                processRegisterTable.clearFilter();
            }
        }, 300);
    });
}

function closeProcessPanel() {
    $('.process-overlay').removeClass('active');
    $('#collapsePanel').removeClass('active');
}

function saveProcessRecord(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();
    console.log('needsBranchOperations:', $('#needsBranchOperations').is(':checked'));
    console.log('needsCreditReview:', $('#needsCreditReview').is(':checked'));
    console.log('needsTreasuryReview:', $('#needsTreasuryReview').is(':checked'));
    console.log('needsFintechReview:', $('#needsFintechReview').is(':checked'));
    let recordData = {
        id: parseInt($('#processId').val()) || 0,
        processName: $('#processName').val()?.trim(),
        description: $('#processDescription').val()?.trim(),
        typeId: parseInt($('#typeId').val()) || 0,
        unitId: parseInt($('#unitId').val()) || 0,
        ownerId: parseInt($('#ownerId').val()) || 0,
        responsibilityId: parseInt($('#assigneedId').val()) || 0,
        processStatus: $('#processStatus').val()?.trim(),
        isDeleted: $('#isDeleted').is(':checked') ? true : false,
        isLockProcess: $('#isLockProcess').is(':checked') ? true : false,
        onholdReason: $('#onholdReason').val()?.trim(),
        fileName: $('#fileName').val()?.trim(),
        currentVersion: $('#fileVersion').val()?.trim(),
        comments: $('#comment').val()?.trim(),
        needsBranchReview: $('#needsBranchOperations').prop('checked'),
        needsCreditReview: $('#needsCreditReview').prop('checked'),
        needsTreasuryReview: $('#needsTreasuryReview').prop('checked'),
        needsFintechReview: $('#needsFintechReview').prop('checked')
        //needsBranchReview: $('#needsBranchOperations').is(':checked') ? true : false,
        //needsCreditReview: $('#needsCreditReview').is(':checked') ? true : false,
        //needsTreasuryReview: $('#needsTreasuryReview').is(':checked') ? true : false,
        //needsFintechReview: $('#needsFintechReview').is(':checked') ? true : false

    };

    // --- validate required fields ---
    let errors = [];

    if (!recordData.processName)
        errors.push("Process name is required.");

    if (!recordData.description)
        errors.push("Process description is required.");

    if (!recordData.processStatus)
        errors.push("Process status is required.");

    if (typeId === 0)
        errors.push("Process type is required.");

    if (unitId === 0)
        errors.push("Department unit is required.");

    if (ownerId === 0)
        errors.push("Process owner is required.");

    if (assigneedId === 0)
        errors.push("Responsible manager is required.");

    if (!recordData.comments)
        errors.push("Comment is required.");

    //var formData = new FormData(this);
    //var files = getUploadedFiles();

    //$.each(files, function (i, fileData) {
    //    formData.append('files[' + i + '].file', fileData.file);
    //    formData.append('files[' + i + '].name', fileData.name);
    //    formData.append('files[' + i + '].isCurrent', fileData.isCurrent);
    //});

    // --- stop if validation fails ---
    if (errors.length > 0) {
        highlightProcessField("#processName", !recordData.processName);
        highlightProcessField("#processDescription", !recordData.description);
        highlightProcessField("#comment", !recordData.comments);
        highlightProcessField("#processStatus", !recordData.processStatus);
        highlightProcessField("#typeId", !recordData.typeId);
        highlightProcessField("#typeId", !recordData.typeId);
        highlightProcessField("#unitId", !recordData.unitId);
        highlightProcessField("#ownerId", !recordData.ownerId);
        highlightProcessField("#assigneedId", !recordData.assigneedId);

        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    console.log('Record Data:', recordData);
    saveProcess(isEdit, recordData);
}

// Function to get all files data when saving
window.getUploadedFiles = function () {
    return uploadedFiles.map(function (f) {
        return {
            id: f.id,
            name: f.name,
            isCurrent: f.isCurrent,
            file: f.file
        };
    });
};

function saveProcess(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/operations/workflow/processes/registers/retrieve/update"
        : "/operations/workflow/processes/registers/retrieve/create";

    Swal.fire({
        title: isEdit ? "Updating process..." : "Saving process...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    //..debugging
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getProcessAntiForgeryToken()
        },
        success: function (res) {
            //..lose loader and show success message
            Swal.close();
            if (!res.success) {
                //..error from the server
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            if (processRegisterTable) {
                if (isEdit && res.data) {
                    processRegisterTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    processRegisterTable.addRow(res.data, true);
                } else {
                    processRegisterTable.replaceData();
                }
            }

            closeProcessPanel();
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

function getProcessAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

}

function highlightProcessField(selector, hasError, message) {
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

//..toggle section collapse/expand
function toggleSection(header) {
    const content = header.nextElementSibling;
    const toggle = header.querySelector('.section-toggle');

    content.classList.toggle('expanded');
    toggle.classList.toggle('expanded');
}

function initEffectiveDatePickers() {
    dateList["effectiveDate"] = flatpickr("#effectiveDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

function handleFiles(files) {
    $.each(files, function (i, file) {
        var fileId = 'file_' + fileCounter++;
        var fileItem = {
            id: fileId,
            name: file.name,
            file: file,
            isCurrent: false
        };

        uploadedFiles.push(fileItem);
        addFileToList(fileItem);
    });
}

function addFileToList(fileItem) {
    var listItem = $('<li></li>')
        .addClass('border rounded p-3 mb-2 d-flex align-items-center justify-content-between')
        .attr('data-file-id', fileItem.id);

    var leftSection = $('<div></div>').addClass('d-flex align-items-center');

    var checkbox = $('<div></div>').addClass('form-check me-3').html(
        '<input class="form-check-input file-current-checkbox" type="checkbox" id="' + fileItem.id + '">' +
        '<label class="form-check-label" for="' + fileItem.id + '">' +
        '</label>'
    );

    var fileName = $('<span></span>')
        .addClass('file-name')
        .text(fileItem.name);

    leftSection.append(checkbox).append(fileName);

    var removeBtn = $('<button></button>')
        .addClass('btn btn-sm btn-danger')
        .attr('type', 'button')
        .html('<i class="bi bi-trash"></i> Remove')
        .on('click', function (e) {
            e.stopPropagation();
            removeFile(fileItem.id);
        });

    listItem.append(leftSection).append(removeBtn);
    $('#fileList').append(listItem);
}

function removeFile(fileId) {
    uploadedFiles = uploadedFiles.filter(function (f) {
        return f.id !== fileId;
    });
    $('li[data-file-id="' + fileId + '"]').remove();
    updateFileNameField();
}

function updateFileNameField() {
    var currentFile = uploadedFiles.find(function (f) {
        return f.isCurrent === true;
    });

    if (currentFile) {
        $('#fileName').val(currentFile.name);
    } else {
        $('#fileName').val('');
    }
}

$(document).ready(function () {

    initProcessRegisterTable();
    initEffectiveDatePickers();
    $("#onHoldBox").addClass("d-none");

    $('#typeId, #processStatus, #unitId, #ownerId, #assigneedId, #complianceStatus, #branchManagerStatus, #approvalStatus').select2({
        width: '100%',
        dropdownParent: $('#collapsePanel')
    });

    //..initialize Flatpickr
    flatpickr("#effectiveDate", {
        dateFormat: "Y-m-d",
        allowInput: true
    });

    $('.action-btn-process-new').on('click', function () {
        createProcess();
    });

    //$('#btnExportProcess').on('click', function () {
    //    $.ajax({
    //        url: '/grc/compliance/register/policies-export',
    //        type: 'POST',
    //        contentType: 'application/json',
    //        data: JSON.stringify(policyRegisterTable.getData()),
    //        xhrFields: { responseType: 'blob' },
    //        success: function (blob) {
    //            let link = document.createElement('a');
    //            link.href = window.URL.createObjectURL(blob);
    //            link.download = "Policy_Register.xlsx";
    //            link.click();
    //        },
    //        error: function () {
    //            toastr.error("Export failed. Please try again.");
    //        }
    //    });
    //});

    $("#processStatus").on("change", function () {
        const selectedValue = $(this).val();
        if (selectedValue === "8") {
            $("#onHoldBox").removeClass("d-none");
        } else {
            $("#onHoldBox").addClass("d-none");
        }
    });

    // Click to browse files
    $('#dropZone').on('click', function (e) {
        if (e.target === this || $(e.target).closest('#dropZone').length) {
            document.getElementById('fileInput').click();
        }
    });

    // Handle file input change
    $('#fileInput').on('change', function (e) {
        handleFiles(e.target.files);
        $(this).val('');
    });

    // Prevent default drag behaviors
    $('#dropZone').on('drag dragstart dragend dragover dragenter dragleave drop', function (e) {
        e.preventDefault();
        e.stopPropagation();
    });

    // Add hover effect
    $('#dropZone').on('dragover dragenter', function () {
        $(this).addClass('border-primary bg-light');
    });

    $('#dropZone').on('dragleave dragend drop', function () {
        $(this).removeClass('border-primary bg-light');
    });

    // Handle drop
    $('#dropZone').on('drop', function (e) {
        var files = e.originalEvent.dataTransfer.files;
        handleFiles(files);
    });

    // Handle checkbox changes
    $(document).on('change', '.file-current-checkbox', function () {
        var fileId = $(this).attr('id');
        var isChecked = $(this).is(':checked');

        // Uncheck all other checkboxes (only one can be current)
        if (isChecked) {
            $('.file-current-checkbox').not(this).prop('checked', false);
            uploadedFiles.forEach(function (f) {
                f.isCurrent = false;
            });
        }

        // Update the current file
        var fileItem = uploadedFiles.find(function (f) {
            return f.id === fileId;
        });
        if (fileItem) {
            fileItem.isCurrent = isChecked;
        }

        // Update the fileName text field
        updateFileNameField();
    });

    // Function to get all files data when saving
    window.getUploadedFiles = function () {
        return uploadedFiles.map(function (f) {
            return {
                id: f.id,
                name: f.name,
                isCurrent: f.isCurrent,
                file: f.file
            };
        });
    };

    $('#processForm').on('submit', function (e) {
        e.preventDefault();
    });

});