
// Form submission handler for unit update
$(document).on('submit', '#form-unit-edit', function(e) {
    e.preventDefault();
    
    const formData = {
        id: $('#unitEditId').val(),
        unitCode: $('#tfEditUnitCode').val(),
        unitName: $('#tfEditUnitName').val(),
        departmentId: $('#dpEditDepartments').val(),
        isDeleted: $('#isDeleted').is(':checked')
    };
    
    // Validate required fields
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

                // Form submission handler for new unit save
$(document).on('submit', '#form-unit-new', function(e) {
    e.preventDefault();
    
    const formData = {
        unitCode: $('#tfNewUnitCode').val().trim(),
        unitName: $('#tfNewUnitName').val().trim(),
        departmentId: parseInt($('#dpNewDepartments').val()) || null,
        isDeleted: false, // New units are active by default
        updateUnits: false // Set based on your business logic
    };
    
    // Validate required fields
    if (!formData.unitCode) {
        showToast("warning", "Please enter a unit code");
        $('#tfNewUnitCode').focus();
        return;
    }
    
    if (!formData.unitName) {
        showToast("warning", "Please enter a unit name");
        $('#tfNewUnitName').focus();
        return;
    }
    
    if (!formData.departmentId) {
        showToast("warning", "Please select a department");
        $('#dpNewDepartments').focus();
        return;
    }
    
    // Show loading state
    const $submitBtn = $('#btnUnitNew');
    const originalText = $submitBtn.text();
    $submitBtn.prop('disabled', true).text('Saving...');
    
    $.ajax({
        url: '/support/departments/saveUnit',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function(response) {
            $submitBtn.prop('disabled', false).text(originalText);
            
            if (response && response.success) {
                showToast("success", response.message || "Unit saved successfully");
                
                // Close slideout
                const $component = $('.grc-page-component');
                $component.find('.component-slideout').removeClass('active');
                resetButtons($component);
                
                // Refresh the table to show the new unit
                $('#departmentUnitsTable').DataTable().ajax.reload();
                
                // Clear the form
                clearNewUnitForm();
                
            } else {
                const errorMessage = response?.message || "Failed to save unit";
                showToast("error", errorMessage);
                
                // Handle validation errors if provided
                if (response && response.errors) {
                    handleValidationErrors(response.errors);
                }
            }
        },
        error: function(xhr, status, error) {
            $submitBtn.prop('disabled', false).text(originalText);
            
            let errorMessage = "Failed to save unit";
            
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            } else if (xhr.status === 400) {
                errorMessage = "Invalid data provided. Please check your inputs.";
            } else if (xhr.status === 409) {
                errorMessage = "A unit with this code already exists.";
            } else if (xhr.status === 500) {
                errorMessage = "Server error occurred. Please try again.";
            }
            
            showToast("error", errorMessage);
            console.error('Error saving unit:', {
                status: xhr.status,
                statusText: xhr.statusText,
                responseText: xhr.responseText,
                error: error
            });
        }
    });
});

// Function to clear the new unit form
function clearNewUnitForm() {
    $('#form-unit-new')[0].reset();
    
    // Reset Select2 dropdowns if used
    const $departmentSelect = $('#dpNewDepartments');
    $departmentSelect.val('').trigger('change');
    
    // Clear any validation error states
    $('#form-unit-new .form-control').removeClass('is-invalid');
    $('#form-unit-new .invalid-feedback').remove();
}

// Function to handle validation errors
function handleValidationErrors(errors) {
    // Clear existing validation states
    $('#form-unit-new .form-control').removeClass('is-invalid');
    $('#form-unit-new .invalid-feedback').remove();
    
    // Apply validation errors
    for (const fieldName in errors) {
        const fieldErrors = errors[fieldName];
        let fieldId = '';
        
        // Map field names to form field IDs
        switch (fieldName.toLowerCase()) {
            case 'unitcode':
                fieldId = 'tfNewUnitCode';
                break;
            case 'unitname':
                fieldId = 'tfNewUnitName';
                break;
            case 'departmentid':
                fieldId = 'dpNewDepartments';
                break;
        }
        
        if (fieldId) {
            const $field = $('#' + fieldId);
            $field.addClass('is-invalid');
            
            // Add error message
            const errorMessage = Array.isArray(fieldErrors) ? fieldErrors[0] : fieldErrors;
            $field.after(`<div class="invalid-feedback">${errorMessage}</div>`);
        }
    }
}

