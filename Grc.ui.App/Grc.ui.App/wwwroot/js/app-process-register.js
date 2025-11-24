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
                title: "REVIEW",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-view grc-view-action" onclick="initiateReview(${rowData.id})">
                        <span><i class="mdi mdi-cog-play-outline" aria-hidden="true"></i></span>
                        <span>INITIATE</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            {
                title: "LOCK FILE",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    let isLocked = rowData.isLockProcess ?? false;
                    if (isLocked) {
                        return `<button class="grc-table-btn disabled" disabled>
                            <span><i class="mdi mdi-lock-outline" aria-hidden="true"></i></span>
                            <span>LOCKED</span>
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
    var isLocked = process?.isLockProcess;
    var status = process?.processStatus;
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
    $("#processStatus").val(status).trigger('change.select2');
    $("#comment").val(process?.comment || "");
    $("#fileName").val(process?.fileName || "");
    $("#fileVersion").val(process?.currentVersion || "");
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
    if (isLocked) {
        //..disable all inputs inside form
        $("#processForm :input").prop("disabled", true); 

        //..allow cancel/close still usable
        $("#closeButton, #cancelButton").prop("disabled", false); 
    } else {
        //..ensure fields are enabled when not locked
        $("#processForm :input").prop("disabled", false); 
    }

    //..show overlay panel
    $('#processPanelTitle').text(title);
    $('.process-overlay').addClass('active');
    $('#collapsePanel').addClass('active');
}

function initiateReview(id) {
    alert("Initiate Review >>> " + id);
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
    let recordData = {
        id: parseInt($('#processId').val()) || 0,
        processName: $('#processName').val()?.trim(),
        description: $('#processDescription').val()?.trim(),
        typeId: parseInt($('#typeId').val()) || 0,
        unitId: parseInt($('#unitId').val()) || 0,
        ownerId: parseInt($('#ownerId').val()) || 0,
        responsibilityId: parseInt($('#assigneedId').val()) || 0,
        processStatus: $('#processStatus').val()?.trim(),
        isDeleted: $('#isDeleted').prop('checked'),
        isLockProcess: $('#isLockProcess').prop('checked'),
        onholdReason: $('#onholdReason').val()?.trim(),
        fileName: $('#fileName').val()?.trim(),
        currentVersion: $('#fileVersion').val()?.trim(),
        comments: $('#comment').val()?.trim(),
        needsBranchReview: $('#needsBranchOperations').prop('checked'),
        needsCreditReview: $('#needsCreditReview').prop('checked'),
        needsTreasuryReview: $('#needsTreasuryReview').prop('checked'),
        needsFintechReview: $('#needsFintechReview').prop('checked')
    };

    // --- validate required fields ---
    let errors = [];
    if (!recordData.processName)
        errors.push("Process name is required.");
    if (!recordData.description)
        errors.push("Process description is required.");
    if (!recordData.processStatus)
        errors.push("Process status is required.");
    if (recordData.typeId === 0)
        errors.push("Process type is required.");
    if (recordData.unitId === 0)
        errors.push("Department unit is required.");
    if (recordData.ownerId === 0)
        errors.push("Process owner is required.");
    if (recordData.responsibilityId === 0)
        errors.push("Responsible manager is required.");
    if (!recordData.comments)
        errors.push("Comment is required.");

    // --- stop if validation fails ---
    if (errors.length > 0) {
        highlightProcessField("#processName", !recordData.processName);
        highlightProcessField("#processDescription", !recordData.description);
        highlightProcessField("#comment", !recordData.comments);
        highlightProcessField("#processStatus", !recordData.processStatus);
        highlightProcessField("#typeId", recordData.typeId === 0);
        highlightProcessField("#unitId", recordData.unitId === 0);
        highlightProcessField("#ownerId", recordData.ownerId === 0);
        highlightProcessField("#assigneedId", recordData.responsibilityId === 0);
        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    // Get files from uploadedFiles array
    let filesToUpload = uploadedFiles.map(f => f.file);

    // Save process first, then upload files
    saveProcessWithFiles(isEdit, recordData, filesToUpload);
}

function saveProcessWithFiles(isEdit, payload, files) {
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
            if (!res.success) {
                Swal.close();
                Swal.fire({
                    title: "Invalid record",
                    html: res.message.replaceAll("; ", "<br>")
                });
                return;
            }

            //..if there are files to upload, upload them now
            if (files && files.length > 0) {
                uploadProcessFiles(res.data.id, files, isEdit, res);
            } else {
                // No files, just finish
                handleProcessSaveSuccess(isEdit, res);
            }
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

function uploadProcessFiles(processId, files, isEdit, processResponse) {
    var formData = new FormData();
    formData.append('processId', processId);

    //..append all files
    $.each(files, function (i, fileData) {
        formData.append('files', fileData.file);
        formData.append('fileNames[' + i + ']', fileData.name);
        formData.append('fileIsCurrent[' + i + ']', fileData.isCurrent);
    });

    Swal.fire({
        title: "Uploading files...",
        text: "Please wait while we upload your files.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: "/operations/workflow/processes/registers/upload-files",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getProcessAntiForgeryToken()
        },
        success: function (fileRes) {
            Swal.close();
            if (!fileRes.success) {
                Swal.fire({
                    title: "File Upload Warning",
                    html: "Process saved but files failed to upload: " + fileRes.message
                });
            }
            handleProcessSaveSuccess(isEdit, processResponse);
        },
        error: function (xhr) {
            Swal.close();
            Swal.fire({
                title: "File Upload Warning",
                text: "Process saved but files failed to upload. Please try uploading them again."
            });
            handleProcessSaveSuccess(isEdit, processResponse);
        }
    });
}

function handleProcessSaveSuccess(isEdit, res) {
    Swal.close();
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

    Swal.fire({
        title: "Success",
        text: isEdit ? "Process updated successfully!" : "Process created successfully!",
        icon: "success",
        timer: 2000
    });
}

// function to get all files data when saving
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

function getProcessAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');

}

function highlightApprovalField(selector, hasError, message) {
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

    $('#btnExportProcess').on('click', function () {
        console.log("Exporting Processes");
        $.ajax({
            url: '/operations/workflow/processes/registers/retrieve/export-all',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(processRegisterTable.getData()),
            xhrFields: { responseType: 'blob' },
            success: function (blob) {
                let link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = "Operations_processes.xlsx";
                link.click();
            },
            error: function () {
                toastr.error("Export failed. Please try again.");
            }
        });
    });

    $("#processStatus").on("change", function () {
        const selectedValue = $(this).val();
        if (selectedValue === "8") {
            $("#onHoldBox").removeClass("d-none");
        } else {
            $("#onHoldBox").addClass("d-none");
        }
    });

    //..click to browse files
    $('#dropZone').on('click', function (e) {
        if (e.target === this || $(e.target).closest('#dropZone').length) {
            document.getElementById('fileInput').click();
        }
    });

    //..handle file input change
    $('#fileInput').on('change', function (e) {
        handleFiles(e.target.files);
        $(this).val('');
    });

    //..prevent default drag behaviors
    $('#dropZone').on('drag dragstart dragend dragover dragenter dragleave drop', function (e) {
        e.preventDefault();
        e.stopPropagation();
    });

    //..add hover effect
    $('#dropZone').on('dragover dragenter', function () {
        $(this).addClass('border-primary bg-light');
    });

    $('#dropZone').on('dragleave dragend drop', function () {
        $(this).removeClass('border-primary bg-light');
    });

    //..handle drop
    $('#dropZone').on('drop', function (e) {
        var files = e.originalEvent.dataTransfer.files;
        handleFiles(files);
    });

    //..handle checkbox changes
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

    //..function to get all files data when saving
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