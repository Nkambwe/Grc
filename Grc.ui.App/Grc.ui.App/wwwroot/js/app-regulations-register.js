
$(document).ready(function () {
    initActTable();
    initFrequencySelect2();
    initResponseSelect2();
    initAuthoritySelect2();
});

let actTable;

function initActTable() {
    actTable = new Tabulator("#registers-table", {
        ajaxURL: "/grc/compliance/register/acts-all",
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
                        ["documentName", "taskDescription", "assigneeName", "department", "assignDate"].includes(f.field)
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
            {
                title: "", field: "startTab",
                maxWidth: 50,
                headerSort: false,
                frozen: true,
                frozen: true, formatter: () => `<span class="record-tab"></span>`
            },
            {
                title: "REGULATION",
                field: "regulatoryName",
                minWidth: 200,
                widthGrow: 2,
                headerSort: true,
                frozen: true,
                formatter: (cell) => `<span class="clickable-title" onclick="viewActRecord(${cell.getRow().getData().id})">${cell.getValue()}</span>`
            },
            { title: "REGULATORY AUTHORITY", field: "regulatoryAuthority", widthGrow: 1, minWidth: 300, frozen: true, headerSort: false },
            { title: "FREQUENCY OF REVIEW", field: "reviewFrequency", minWidth: 300 },
            { title: "LAST REVIEW DATE", field: "lastReviewDate", minWidth: 100 },
            { title: "REVIEW RESPONSIBILITY", field: "reviewResponsibility", minWidth: 300 },
            { title: "COMMENTS", field: "comments", minWidth: 400 },
            {
                title: "ACTION",
                formatter: function (cell) {
                    let rowData = cell.getRow().getData();
                    return `<button class="grc-table-btn grc-btn-delete grc-delete-action" onclick="deleteActRecord(${rowData.id})">
                        <span><i class="mdi mdi-delete-circle" aria-hidden="true"></i></span>
                        <span>DELETE</span>
                    </button>`;
                },
                width: 200,
                hozAlign: "center",
                headerHozAlign: "center",
                headerSort: false
            },
            { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });

    // Search init
    initActSearch();
}

/*------------------------------------------
    Navigation / Buttons
-------------------------------------------*/
$('.action-btn-complianceHome').on('click', function () {
    window.location.href = '/grc/compliance';
});

$('.action-btn-act-new').on('click', function () {
    addActRecord();
})

$('#btnActExportFiltered').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/tasks-export',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyTaslTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Policy_Register.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('.action-btn-act-export').on('click', function () {
    $.ajax({
        url: '/grc/compliance/register/tasks-export-full',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(policyTaslTable.getData()),
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            let link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = "Policy_Register_All.xlsx";
            link.click();
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
});

$('#actForm').on('submit', function (e) {
    //..prevent full postback
    e.preventDefault();
});

function addActRecord() {
    openActPanel('New Law/Regulation', {
        id: 0,
        regulatoryName: '',
        authorityId: 0,
        reviewFrequency: '',
        isActive: false,
        reviewResponsibility: '',
        lastReviewDate: "",
        comments: ''
    }, false);
}

let flatpickrInstances = {};

function initReviewDatePickers() {
    flatpickrInstances["lastReviewDate"] = flatpickr("#lastReviewDate", {
        dateFormat: "Y-m-d",
        allowInput: true,
        altInput: true,
        altFormat: "d M Y",
        defaultDate: null
    });
}

function openActPanel(title, record, isEdit) {

    $('#isEdit').val(isEdit);
    $('#recordId').val(record.id);
    $('#regulatoryName').val(record.regulatoryName || '');
    $('#dpAuthority').val(record.authorityId).trigger('change');
    $('#isActive').prop('checked', record.isActive);
    $('#dpFrequency').val(record.reviewFrequency);
    $('#dpResponsibility').val(record.reviewResponsibility).trigger('change');
    $('#actComments').val(record.comments || '');

    $('#panelTitle').text(title);
    $('.overlay').addClass('active');
    $('#slidePanel').addClass('active');
}

function deleteActRecord(id) {
    if (!id && id !== 0) {
        toastr.error("Invalid id for delete.");
        return;
    }

    Swal.fire({
        title: "Delete Law/regulation/Guide",
        text: "Are you sure you want to delete this law/act?",
        showCancelButton: true,
        confirmButtonColor: "#450354",
        confirmButtonText: "Delete",
        cancelButtonColor: "#f41369",
        cancelButtonText: "Cancel"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: `/grc/compliance/register/acts-delete/${encodeURIComponent(id)}`,
            type: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'X-CSRF-TOKEN': getActAntiForgeryToken()
            },
            success: function (res) {
                if (res && res.success) {
                    toastr.success(res.message || "Law/regulation/Guide deleted successfully.");
                    policyRegisterTable.setPage(1, true);
                } else {
                    toastr.error(res?.message || "Delete failed.");
                }
            },
            error: function () {
                toastr.error("Request failed.");
            }
        });
    });
}

function viewActRecord(id) {
    Swal.fire({
        title: 'Loading...',
        text: 'Retrieving Law...',
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => Swal.showLoading()
    });

    console.log("ID >> " + id);
    findActRecord(id)
        .then(record => {
            Swal.close();
            if (record) {
                openActPanel('Edit Regulatory Law/Act', record, true);
            } else {
                Swal.fire({ title: 'NOT FOUND', text: 'Law/Act not found' });
            }
        })
        .catch(() => {
            Swal.close();
            Swal.fire({ title: 'Error', text: 'Failed to load law/act details.' });
        });
}

function findActRecord(id) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/grc/compliance/register/acts-retrieve/${encodeURIComponent(id)}`,
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

function saveActRecord(e) {
    if (e) e.preventDefault();
    let isEdit = $('#isEdit').val();

    // --- gather form values ---
    let recordData = {
        id: parseInt($('#recordId').val()) || 0,
        regulatoryName: $('#regulatoryName').val()?.trim(),
        authorityId: Number($('#dpAuthority').val()),
        reviewResponsibility: Number($('#dpResponsibility')).val(),
        isDeleted: $('#isActive').is(':checked') ? true : false,
        reviewFrequency: Number($('#dpFrequency').val()),
        lastReviewDate: flatpickrInstances["lastReviewDate"].input.value || null,
        comments: $('#actComments').val()?.trim()
    };

    // --- validate required fields ---
    let errors = [];

    if (!recordData.regulatoryName)
        errors.push("Regulatory act name is required.");

    if (!recordData.authorityId || recordData.authorityId === 0)
        errors.push("Authority name is required");

    if (!recordData.numFrequency || recordData.numFrequency === 0)
        errors.push("Review frquency is require");

    if (!recordData.comments)
        errors.push("Comment is required.");

    if (!recordData.reviewResponsibility)
        errors.push("Function/Department responsible is required.");

    //..date validation
    if (!recordData.lastReviewDate)
        errors.push("Last review date is required.");

    // --- stop if validation fails ---
    if (errors.length > 0) {

        highlightField("#regulatoryName", !recordData.regulatoryName);
        highlightField("#dpAuthority", !recordData.authorityId);
        highlightField("#dpResponsibility", !recordData.reviewResponsibility);
        highlightField("#numFrequency", !recordData.numFrequency);
        highlightField("#comments", !recordData.comments);
        highlightField("#lastReviewDate", !recordData.lastReviewDate);

        Swal.fire({
            title: "Record Validation",
            html: `<div style="text-align:left;">${errors.join("<br>")}</div>`,
        });
        return;
    }

    console.log("Valid Record:", recordData);
    saveAct(isEdit, recordData);
}

function saveAct(isEdit, payload) {
    const url = (isEdit === true || isEdit === "true")
        ? "/grc/compliance/register/acts-update"
        : "/grc/compliance/register/acts-create";

    Swal.fire({
        title: isEdit ? "Updating law,regulation..." : "Saving law,regulation...",
        text: "Please wait while we process your request.",
        allowOutsideClick: false,
        allowEscapeKey: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    //..debugging
    console.log("Sending data to server:", JSON.stringify(payload));  
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(payload),
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            'X-CSRF-TOKEN': getActAntiForgeryToken()
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

            if (policyRegisterTable) {
                if (isEdit && res.data) {
                    policyRegisterTable.updateData([res.data]);
                } else if (!isEdit && res.data) {
                    policyRegisterTable.addRow(res.data, true);
                } else {
                    policyRegisterTable.replaceData();
                }
            }

            Swal.fire({
                title: isEdit ? "Updating law,regulation..." : "Saving law,regulation...",
                text: res.message || "Saved successfully.",
                timer: 2000,
                showConfirmButton: false
            });

            closePolicyDocumentPanel();
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

function initActSearch() {
    const searchInput = $('#actSearchbox');
    let typingTimer;

    searchInput.on('input', function () {
        clearTimeout(typingTimer);
        const searchTerm = $(this).val();

        typingTimer = setTimeout(function () {
            if (searchTerm && searchTerm.length >= 2) {
                policyRegisterTable.setFilter([
                    [
                        { field: "regulatoryName", type: "like", value: searchTerm },
                        { field: "regulatoryAuthority", type: "like", value: searchTerm },
                        { field: "reviewResponsibility", type: "like", value: searchTerm }
                    ]
                ]);
                policyRegisterTable.setPage(1, true);
            } else {
                policyRegisterTable.clearFilter();
            }
        }, 300);
    });
}

function initAuthoritySelect2() {
    $(".js-act-authority").each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            initAuthorityElement($(this));
        }
    });
}

function initAuthorityElement($element) {
    const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select authority';

    $element.select2({
        width: 'resolve',
        placeholder: 'Select an authority...',
        allowClear: true,
        escapeMarkup: function (markup) {
            return markup;
        },
        language: {
            noResults: function () {
                return "No authority found";
            }
        }
    });
    setTimeout(() => {
        fixSelect5Accessibility($element, labelText);
    }, 100);
}

function initResponseSelect2() {
    $(".js-act-respons").each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            initResponseElement($(this));
        }
    });
}

function initResponseElement($element) {
    const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select responsibility';

    $element.select2({
        width: 'resolve',
        placeholder: 'Select an responsibility...',
        allowClear: true,
        escapeMarkup: function (markup) {
            return markup;
        },
        language: {
            noResults: function () {
                return "No responsibility found";
            }
        }
    });
    setTimeout(() => {
        fixSelect5Accessibility($element, labelText);
    }, 100);
}

function initFrequencySelect2() {
    $(".js-act-frequency").each(function () {
        if (!$(this).hasClass('select2-hidden-accessible')) {
            initFrequencyElement($(this));
        }
    });
}

function initFrequencyElement($element) {
    const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select frequency';

    $element.select2({
        width: 'resolve',
        placeholder: 'Select an frequency...',
        allowClear: true,
        escapeMarkup: function (markup) {
            return markup;
        },
        language: {
            noResults: function () {
                return "No frequency found";
            }
        }
    });
    setTimeout(() => {
        fixSelect5Accessibility($element, labelText);
    }, 100);
}

function fixSelect5Accessibility($originalSelect, labelText) {
    const selectId = $originalSelect.attr('id');
    if (!selectId) return;

    const $select2Container = $originalSelect.next('.select2-container');
    const $select2Selection = $select2Container.find('.select2-selection');
    const $select2Arrow = $select2Container.find('.select2-selection__arrow');

    // Remove problematic aria-hidden from the original select
    $originalSelect.removeAttr('aria-hidden');

    // Add proper ARIA attributes to Select2 elements
    $select2Selection.attr({
        'role': 'combobox',
        'aria-expanded': 'false',
        'aria-haspopup': 'listbox',
        'aria-labelledby': selectId + '-label',
        'aria-describedby': selectId + '-description'
    });

    // Create or update label
    let $label = $(`label[for="${selectId}"]`);
    if ($label.length === 0) {
        $label = $originalSelect.closest('.form-group').find('label').first();
        $label.attr('for', selectId);
    }
    $label.attr('id', selectId + '-label');

    // Add description for screen readers
    if ($(`#${selectId}-description`).length === 0) {
        $('<span>', {
            id: selectId + '-description',
            class: 'sr-only',
            text: 'Use arrow keys to navigate options'
        }).insertAfter($select2Container);
    }

    // Handle Select2 events for accessibility
    $originalSelect.on('select2:open', function () {
        $select2Selection.attr('aria-expanded', 'true');

        // Focus the search input when dropdown opens
        setTimeout(() => {
            const $searchInput = $('.select2-search__field');
            if ($searchInput.length) {
                $searchInput.attr('aria-label', `Search ${labelText}`);
            }
        }, 50);
    });

    $originalSelect.on('select2:close', function () {
        $select2Selection.attr('aria-expanded', 'false');
    });

    // Remove aria-hidden when element gains focus
    $select2Selection.on('focus', function () {
        $originalSelect.removeAttr('aria-hidden');
        $(this).removeAttr('aria-hidden');
    });
}

//..get antiforegery token from meta tag
function getActAntiForgeryToken() {
    return $('meta[name="csrf-token"]').attr('content');
}

function closeActPanel() {
    console.log('Button clicked');
    $('.overlay').removeClass('active');
    $('#slidePanel').removeClass('active');
}

$(document).ready(function () {
    initReviewDatePickers();
});

