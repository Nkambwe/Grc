 $(document).ready(function () {
    let isCompExpanded = false;

    $(".status-indicator").on("click", function (e) {
        e.stopPropagation(); 
        const $component = $(this).closest('.grc-page-component'); 
        $component.find('.header-more-list').toggleClass("open");
    });

    $(document).on("click", function (e) {
        if (!$(e.target).closest(".header-more-list").length) {
            $(".header-more-list").removeClass("open");
        }
    });

    $('[data-action="expand"]').on('click', function(e) {
        e.preventDefault();
        const action = $(this).data('action');
        console.log('Action:', action); 
    
        isCompExpanded = !isCompExpanded;         
    
        if (isCompExpanded) {
            //..expand the main content
            $('.component-modal-container').addClass('expanded');
            $('#compExpandIncon').removeClass('mdi-arrow-expand').addClass('mdi-arrow-collapse');
            $(this).attr('title', 'Exit Full Screen');
        } else {
            //..collapse back to normal
            $('.component-modal-container').removeClass('expanded');
            $('#compExpandIncon').removeClass('mdi-arrow-collapse').addClass('mdi-arrow-expand');
            $(this).attr('title', 'Full Screen');
        }
    });

     //..handle action button clicks to open modal
     $('.action-btn-departments').on('click', function (e) {
         e.preventDefault();
         openModal('departments');

         console.log("Departments should open");
         initDepartmentsTable();
     });

     function initDepartmentsTable() {
         if ($.fn.DataTable.isDataTable('#departmentsTable')) {
             $('#departmentsTable').DataTable().clear().destroy();
             $('#departmentsTable').empty();
         }

         var departmentsTable = $('#departmentsTable').DataTable({
             searching: false,
             paging: true,
             lengthChange: false,
             info: false,
             ordering: true,
             responsive: true,
             processing: true,
             serverSide: true,
             ajax: {
                 url: '/support/departments/allDepartments',
                 type: 'POST',
                 contentType: "application/json",
                 data: function (d) {
                     return JSON.stringify({
                         pageIndex: (d.start / d.length) + 1,
                         pageSize: d.length,
                         includeDeleted: false,
                         action: "LOADDEPARTMENTS"
                     });
                 },
                 dataSrc: function (json) {
                     console.log('Ajax response:', json);
                     return json.data || [];
                 },
                 error: function (xhr, status, error) {
                     showToast("error", `Failed to load department data: ${error}`);
                     console.error('Error during Ajax request:', error);
                 }
             },
             columns: [
                 { data: "departmentCode" },
                 { data: "departmentName" },
                 { data: "departmentAlias" },
                 { data: "isDeleted" },
                 { data: "branch" },
                 { data: "creatdOn" }
             ],
             language: {
                 emptyTable: "No department data available"
             }
         });

         return departmentsTable;
     }


    //..handle action button clicks to open modal
     $('.action-btn-units').on('click', function(e) {
        e.preventDefault();
         openModal('DepartmentUnits');

        console.log("Units should be");
        initDepartmentUnitsTable();
    });

    //..close component modal when clicking the X button
   $(document).on('click', '.component-modal-close, .componet-modal-back', function() {
        const $modal = $(this).closest('.component-modal-overlay');
        closeModal($modal.data('popup-id'));
    });
    
    //..close component modal when clicking outside the modal container
    $(document).on('click', '.component-modal-overlay', function(e) {
        if (e.target === this) {
            closeModal($(this).data('popup-id'));
        }
    });
    
    //..close modal with Escape key
    $(document).on('keydown', function(e) {
        if (e.key === 'Escape') {
            const $topModal = $('.component-modal-overlay:visible').last();
            if ($topModal.length) {
                closeModal($topModal.data('popup-id'));
            }
        }
    });

    //..activate link clicks only when row is selected
    $('#departmentUnitsTable tbody').on('click', 'a', function (e) {
        e.stopPropagation();

        const $link = $(this);
        const $row = $link.closest('tr');

        if (!$row.hasClass('row-selected')) {
            e.preventDefault(); 
            return false;
        }

        console.log('Link clicked:', $link.text(), 'Row selected');
    });

    //..prevent text selection on double click
    $('#departmentUnitsTable').on('selectstart', function() {
        return false;
    });

    /*--------------------slideout button events*/

     // initalize select elements
    initializeBranches();
    initializeDepartments();
    
    // Your existing event handlers with Select2 initialization
    $('.action-btn-new').on("click", function (e) {
        e.preventDefault();
        const $component = $(this).closest('.grc-page-component'); 
        resetButtons($component);
        $(this).addClass('active');

        $component.find('.component-slideout').addClass('active');
        $component.find('.component-create-container').show();
        $component.find('.component-edit-container').hide();
        
        // Load select items for the create container
        const $createContainer = $component.find('.component-create-container');
        loadBranchesForContainer($createContainer);
        loadDepartmentsForContainer($createContainer);
    
        // Clear the form
        clearNewUnitForm();
    });

    $('.action-btn-edit').on("click", function (e) {
        e.preventDefault();
        const $component = $(this).closest('.grc-page-component'); 
        resetButtons($component);
        $(this).addClass('active');
        $component.find('.component-slideout').addClass('active');
        $component.find('.component-create-container').hide();
        $component.find('.component-edit-container').show();
        
        // Load select items for the edit container
        const $editContainer = $component.find('.component-edit-container');
        loadBranchesForContainer($editContainer);
        loadDepartmentsForContainer($editContainer, function() {
            populateEditForm(response.data);
        });

        
        // Check if a row is selected
        const $selectedRow = $('#departmentUnitsTable tbody tr.row-selected');
        if ($selectedRow.length === 0) {
            showToast("warning", "Please select a unit to edit");
            return;
        }
    
        // Get unit ID from selected row
        const unitId = $selectedRow.data('unit-id');
        if (unitId) {
            loadUnitForEdit(unitId);
        }

    });

     $('.action-btn-Delete').on("click", function (e) {

     });

     // filter buttons
    $('.filter-btn').on("click", function (e) {
        e.preventDefault();

        // reset only inside this component
        const $component = $(this).closest('.grc-page-component'); 

        resetButtons($component);

        $(this).addClass('active');
        $component.find('.component-tabletools-container').addClass('active');
    });
    
     $('.component-new-close, .component-edit-close').on("click", function (e) {
         e.preventDefault();

        // reset only inside this component
        const $component = $(this).closest('.grc-page-component'); 

        resetButtons($component);
        $component.find('.component-slideout').removeClass('active');

        // Clear forms when closing
        clearNewUnitForm();
        $('#form-unit-edit')[0].reset();
        $('#unitEditId').val('');

     });

     $('.component-filter-close').on("click", function (e) {
          e.preventDefault();

          // reset only inside this component
          const $component = $(this).closest('.grc-page-component'); 

          resetButtons($component);
          $component.find('.component-tabletools-container').removeClass('active');
     });

    //..override DataTables alerts
    $.fn.dataTable.ext.errMode = function ( settings, helpPage, message ) {
        showToast("error", message);
    };

    $(document).on('popup.opened', function (e, popupId) {
        if (popupId === 'DepartmentUnits') {
            initDepartmentUnitsTable();
        }
    });

    $(document).on('popup.closed', function (e, popupId) {
        if (popupId === 'DepartmentUnits') {
            if ($.fn.DataTable.isDataTable('#departmentUnitsTable')) {
                $('#departmentUnitsTable').DataTable().destroy(true);
                $('#departmentUnitsTable').empty();
            }
        }
    });

    //..form submission handler for unit update
    $(document).on('submit', '#form-unit-edit', function(e) {
        e.preventDefault();
    
        const formData = {
            id: $('#unitEditId').val(),
            unitCode: $('#tfEditUnitCode').val(),
            unitName: $('#tfEditUnitName').val(),
            departmentId: $('#dpEditDepartments').val(),
            isDeleted: $('#isDeleted').is(':checked')
        };
    
        //..validate required fields
        if (!formData.unitCode || !formData.unitName || !formData.departmentId) {
            showToast("warning", "Please fill in all required fields");
            return;
        }
    
        $.ajax({
            url: '/support/departments/updateUnit',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(formData),
            success: function(response) {
                if (response.success) {
                    showToast("success", "Unit updated successfully");
                
                    // Close slideout
                    const $component = $('.grc-page-component');
                    $component.find('.component-slideout').removeClass('active');
                    resetButtons($component);
                
                    // Refresh the table
                    $('#departmentUnitsTable').DataTable().ajax.reload();
                
                    // Clear form
                    $('#form-unit-edit')[0].reset();
                    $('#unitEditId').val('');
                } else {
                    showToast("error", response.message || "Failed to update unit");
                }
            },
            error: function(xhr, status, error) {
                showToast("error", `Failed to update unit: ${error}`);
                console.error('Error updating unit:', error);
            }
        });
    });

    // Function to initialize basic Select2 with accessibility fixes
    function initializeBranches() {
        $(".js-branches").each(function() {
            if (!$(this).hasClass('select2-hidden-accessible')) {
                initializeBranchElement($(this));
            }
        });
    }

    function initializeDepartments() {
        $(".js-departments").each(function() {
            if (!$(this).hasClass('select2-hidden-accessible')) {
                initializeDepartmentElement($(this));
            }
        });
     }

     // Function to initialize a single Select2 element with proper accessibility
    function initializeBranchElement($element) {
        const elementId = $element.attr('id');
        const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select branch';
    
        $element.select2({
            width: 'resolve',
            placeholder: 'Select a branch...',
            allowClear: true,
            escapeMarkup: function (markup) {
                return markup;
            },
            language: {
                noResults: function() {
                    return "No branches found";
                }
            }
        });
    
        // Fix accessibility issues after Select2 initialization
        setTimeout(() => {
            fixSelect2Accessibility($element, labelText);
        }, 100);
    }

    function initializeDepartmentElement($element) {
        const elementId = $element.attr('id');
        const labelText = $element.closest('.form-group').find('label').text().trim() || 'Select Department';
    
        $element.select2({
            width: 'resolve',
            placeholder: 'Select a department...',
            allowClear: true,
            escapeMarkup: function (markup) {
                return markup;
            },
            language: {
                noResults: function() {
                    return "No departments found";
                }
            }
        });
    
        // Fix accessibility issues after Select2 initialization
        setTimeout(() => {
            fixSelect2Accessibility($element, labelText);
        }, 100);
    }

    // Function to load branches for a specific container
    function loadBranchesForContainer($container) {
        const $branchSelects = $container.find('.js-branches');
        if ($branchSelects.length > 0) {
            $.ajax({
                url: '/support/organization/getBranches',
                type: 'GET',
                dataType: 'json',
                success: function(data) {
                    $branchSelects.each(function() {
                        const $select = $(this);
                        const currentValue = $select.val();
                    
                        // Destroy existing Select2 if it exists
                        if ($select.hasClass('select2-hidden-accessible')) {
                            $select.select2('destroy');
                        }
                    
                        // Clear existing options
                        $select.empty();
                    
                        // Add placeholder option
                        $select.append('<option value="">Select a branch...</option>');
                    
                        // Add branch options
                        if (data.results && data.results.length > 0) {
                            $.each(data.results, function(index, branch) {
                                $select.append(`<option value="${branch.id}">${branch.text}</option>`);
                            });
                        }
                    
                        // Restore previous value if it exists
                        if (currentValue) {
                            $select.val(currentValue);
                        }
                    
                        // Initialize Select2 with accessibility fixes
                        initializeBranchElement($select);
                    });
                },
                error: function(xhr, status, error) {
                    console.error('Error loading branches:', error);
                
                    // Initialize empty Select2 even on error
                    $branchSelects.each(function() {
                        const $select = $(this);
                        if (!$select.hasClass('select2-hidden-accessible')) {
                            initializeBranchElement($select);
                        }
                    });
                }
            });
        }
    }

    function loadDepartmentsForContainer($container) { 
        const $branchSelects = $container.find('.js-departments');
        if ($branchSelects.length > 0) {
            $.ajax({
                url: '/support/departments/getDepartments',
                type: 'GET',
                dataType: 'json',
                success: function(data) {
                    $branchSelects.each(function() {
                        const $select = $(this);
                        const currentValue = $select.val();
                    
                        // Destroy existing Select2 if it exists
                        if ($select.hasClass('select2-hidden-accessible')) {
                            $select.select2('destroy');
                        }
                    
                        // Clear existing options
                        $select.empty();
                    
                        // Add placeholder option
                        $select.append('<option value="">Select a department...</option>');
                    
                        // Add department options
                        if (data.results && data.results.length > 0) {
                            $.each(data.results, function(index, department) {
                                $select.append(`<option value="${department.id}">${department.text}</option>`);
                            });
                        }
                    
                        // Restore previous value if it exists
                        if (currentValue) {
                            $select.val(currentValue);
                        }
                    
                        // Initialize Select2 with accessibility fixes
                        initializeDepartmentElement($select);
                    });
                },
                error: function(xhr, status, error) {
                    console.error('Error loading departments:', error);
                
                    // Initialize empty Select2 even on error
                    $branchSelects.each(function() {
                        const $select = $(this);
                        if (!$select.hasClass('select2-hidden-accessible')) {
                            initializeDepartmentElement($select);
                        }
                    });
                }
            });
		}

	}

    function resetButtons($component) {
        $component.find('.filterBtn').removeClass('active');
        $component.find('.action-btn-new').removeClass('active');
        $component.find('.action-btn-edit').removeClass('active');
    }

    function initDepartmentUnitsTable() {
        if ($.fn.DataTable.isDataTable('#departmentUnitsTable')) {
            $('#departmentUnitsTable').DataTable().clear().destroy();
            $('#departmentUnitsTable').empty();
        }

        var departmentsTable = $('#departmentUnitsTable').DataTable({
            searching: false,
            paging: true,
            lengthChange: false,
            info: false,
            ordering: true,
            responsive: true,
            processing: true,
            serverSide: true,
            ajax: {
                url: '/support/departments/allUnits',
                type: 'POST',
                contentType: "application/json",
                data: function (d) {
                    return JSON.stringify({
                        pageIndex: (d.start / d.length) + 1,
                        pageSize: d.length,
                        includeDeleted: false,
                        action: "LOADUNITS"
                    });
                },
                dataSrc: function (json) {
                    console.log('Ajax response:', json); 
                    return json.data || [];
                },
                error: function (xhr, status, error) {
                        showToast("error", `Failed to load department units data: ${error}`);
                    console.error('Error during Ajax request:', error);
                }
            },
            columns: [
                { data: "unitCode" },
                { data: "unitName" },
                { data: "department" },
                { 
                    data: "isDeleted",
                    render: function(data, type, row) {
                        const checked = !data ? 'checked' : ''; 
                        return `<input type="checkbox" class="form-check-input unit-active-checkbox" ${checked} disabled>`;
                    }
                },
                { data: "createdOn" }
            ],
            createdRow: function(row, data, dataIndex) {
                $(row).attr('data-unit-id', data.id);
                $(row).on('click', function(e) {
                    if ($(e.target).hasClass('unit-active-checkbox')) {
                        return;
                    }
                
                    var $row = $(this);
                    var unitId = $row.data('unit-id');
                    if ($row.hasClass('row-selected')) {
                        //..seselect row
                        $row.removeClass('row-selected');
                        $row.find('.row-selection-icon').remove();
                    } else {
                        //..remove selection from all other rows
                        $('#departmentUnitsTable tbody tr').removeClass('row-selected');
                        $('#departmentUnitsTable tbody tr .row-selection-icon').remove();
                    
                        //..select current row
                        $row.addClass('row-selected');
                        $row.find('td:first').prepend('<i class="mdi mdi-arrow-right row-selection-icon"></i>');
                    
                        //..load unit data for editing
                        loadUnitForEdit(unitId);
                    }
                });
            },
            language: {
                emptyTable: "No department units data available"
            }
        });

        return departmentsTable;
    }

    function openModal(popupIdOrName) {
        let $popup = $(`#${popupIdOrName}`);
        if ($popup.length === 0) {
            $popup = $(`.component-modal-overlay[data-popup-name="${popupIdOrName}"]`);
        }
        console.log("Dialog >> ", $popup);
        if ($popup.length) {
            $popup.show();
            $(document).trigger('popup.opened', [$popup.attr('id')]);
        }
    }

    function closeModal(popupId) {
        $(`#${popupId}`).hide();
        $(document).trigger('popup.closed', [popupId]);
    }
    
    // Function to fix Select2 accessibility issues
    function fixSelect2Accessibility($originalSelect, labelText) {
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
        $originalSelect.on('select2:open', function() {
            $select2Selection.attr('aria-expanded', 'true');
        
            // Focus the search input when dropdown opens
            setTimeout(() => {
                const $searchInput = $('.select2-search__field');
                if ($searchInput.length) {
                    $searchInput.attr('aria-label', `Search ${labelText}`);
                }
            }, 50);
        });
    
        $originalSelect.on('select2:close', function() {
            $select2Selection.attr('aria-expanded', 'false');
        });
    
        // Remove aria-hidden when element gains focus
        $select2Selection.on('focus', function() {
            $originalSelect.removeAttr('aria-hidden');
            $(this).removeAttr('aria-hidden');
        });
    }

    // Function to load unit data for editing
    function loadUnitForEdit(unitId) {
        if (!unitId) {
            console.error('No unit ID provided');
            return;
        }

        $.ajax({
        url: `/support/departments/getUnit/${unitId}`,
        type: 'GET',
        success: function(response) {
            console.log('Server response:', response);
            
            if (response && response.success && response.data) {
                populateEditForm(response.data);
                const $component = $('#departmentUnitsTable').closest('.grc-page-component');
                triggerEditSlideout($component);
            } else {
                showToast("error", response?.message || "Failed to load unit data");
            }
        },
        error: function(xhr, status, error) {
            console.error('Full error details:', { 
                status: xhr.status, 
                statusText: xhr.statusText, 
                responseText: xhr.responseText,
                error: error 
            });
            
            let errorMessage = "Failed to load unit data";
            
            if (xhr.status === 404) {
                errorMessage = "Unit not found";
            } else if (xhr.status === 500) {
                errorMessage = "Server error occurred";
            }
            
            showToast("error", errorMessage);
        }
    });
    }

    // Function to populate the edit form
    function populateEditForm(unitData) {
        $('#tfEditUnitCode').val(unitData.unitCode || '');
        $('#tfEditUnitName').val(unitData.unitName || '');
        $('#dpEditDepartments').val(unitData.departmentId || '').trigger('change');
        $('#isDeleted').prop('checked', unitData.isDeleted || false);
        let hiddenIdField = $('#unitEditId');
        hiddenIdField.val(unitData.id);
    }

    // Function to trigger edit slideout
    function triggerEditSlideout($component) {
        resetButtons($component);
        $component.find('.action-btn-Edit').addClass('active');
        $component.find('.component-slideout').addClass('active');
        $component.find('.component-create-container').hide();
        $component.find('.component-edit-container').show();
    
        // Load select items for the edit container
        const $editContainer = $component.find('.component-edit-container');
        loadBranchesForContainer($editContainer);
        loadDepartmentsForContainer($editContainer);
    }

});
   
