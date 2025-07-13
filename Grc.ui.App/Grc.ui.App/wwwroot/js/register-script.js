$(function() {

    $(document).ready(function () {
        var $steps = $('.form-step');
        var $stepNavs = $('.step-nav .step');
        var $prevBtn = $('.btn-prev');
        var $nextBtn = $('.btn-next');
        var $submitBtn = $('.btn-submit');
        var currentStep = 0;
        var $form = $('#registration-form');

        //..initialize
        showStep(currentStep);

        //..next button click handler
        $nextBtn.on('click', function() {
            if (validateStep(currentStep)) {
                currentStep++;
                showStep(currentStep);
            }
        });

        //..previous button click handler
        $prevBtn.on('click', function() {
            currentStep--;
            showStep(currentStep);
        });

        //..password toggle functionality
        $('.password-toggle').on('click', function() {
            var $toggle = $(this);
            var $input = $toggle.parent().find('input');
            var $icon = $toggle.find('i');
        
            if ($input.attr('type') === 'password') {
                $input.attr('type', 'text');
                $icon.removeClass('mdi-eye-off').addClass('mdi-eye');
            } else {
                $input.attr('type', 'password');
                $icon.removeClass('mdi-eye').addClass('mdi-eye-off');
            }
        });

        //..auto-advance Select2 fields
        $('.language-select').on('select2:select', function() {
            if (currentStep === 0 && validateStep(currentStep)) {
                currentStep++;
                showStep(currentStep);
            }
        });

        //..real-time validation event handlers
        setupRealTimeValidation();

        //..form submission with Ajax
        $form.on('submit', function(e) {
            e.preventDefault();
            
            if (validateAllSteps()) {
                submitFormWithAjax();
            }
        });

        function showStep(stepIndex) {
            //..hide all steps and show current one
            $steps.removeClass('active').eq(stepIndex).addClass('active');
        
            //..update navigation indicators
            $stepNavs.removeClass('active completed')
                .each(function(index) {
                    var $nav = $(this);
                    if (index < stepIndex) {
                        $nav.addClass('completed');
                    } else if (index === stepIndex) {
                        $nav.addClass('active');
                    }
                });

            //..update button visibility
            $prevBtn.prop('disabled', stepIndex === 0);
            $nextBtn.toggle(stepIndex !== $steps.length - 1);
            $submitBtn.toggle(stepIndex === $steps.length - 1);
        }

        function validateStep(stepIndex) {
            var isValid = true;
            var $currentFields = $steps.eq(stepIndex).find('[required]');
           
            $currentFields.each(function() {
                var $field = $(this);
                var fieldName = $field.attr('name'); 
                var $validationSpan = $(`[data-valmsg-for='${fieldName}']`);

                if (!validateField($field)) {
                    isValid = false;
                }
            });

            if (!isValid) {
                $('#validation-summary').removeClass('d-none');
                $('html, body').animate({
                    scrollTop: $steps.eq(currentStep).offset().top - 100
                }, 500);
            } else {
                $('#validation-summary').addClass('d-none');
            }

            return isValid;
        }

        $('#System_Language').select2({
            width: '100%',
            height:'35',
            theme: 'default', 
            dropdownCssClass: 'custom-select2-dropdown' 
        });

        function validateAllSteps() {
            var isValid = true;
            
            for (var i = 0; i < $steps.length; i++) {
                if (!validateStep(i)) {
                    isValid = false;
                    currentStep = i;
                    showStep(currentStep);
                    break;
                }
            }
            
            return isValid;
        }

        function validateField($field) {
            var fieldId = $field.attr('id');
            var fieldName = $field.attr('name');
            var value = $field.val().trim();
            var $validationSpan = $(`[data-valmsg-for='${fieldName}']`);
            var isValid = true;
            var errorMessage = '';

            //..remove previous validation classes
            $field.removeClass('is-invalid is-valid');

            //..required field validation
            if ($field.attr('required') && !value) {
                isValid = false;
                errorMessage = 'This field is required.';
            }
            //..field-specific validations
            else if (value) {
                switch (fieldId) {
                    case 'Company_Name':
                        if (!validateAlphabetic(value)) {
                            isValid = false;
                            errorMessage = 'Company name should contain only alphabetic characters and spaces.';
                        }
                        break;
                    
                    case 'User_FirstName':
                    case 'User_MidlleName':
                    case 'User_LastName':
                        if (!validateAlphabetic(value)) {
                            isValid = false;
                            errorMessage = 'Name should contain only alphabetic characters.';
                        }
                        break;
                    
                    case 'User_UserName':
                        if (!validateAlphanumeric(value)) {
                            isValid = false;
                            errorMessage = 'Username should contain only alphanumeric characters.';
                        }
                        break;
                    
                    case 'User_PFNumber':
                        if (!validateNumeric(value)) {
                            isValid = false;
                            errorMessage = 'PF Number should contain only numeric characters.';
                        }
                        break;
                    
                    case 'User_Email':
                        if (!validateEmail(value)) {
                            isValid = false;
                            errorMessage = 'Please enter a valid email address.';
                        }
                        break;
                    
                    case 'SystemPassword':
                        if (!validatePassword(value)) {
                            isValid = false;
                            errorMessage = 'Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.';
                        }
                        break;
                    
                    case 'User_ConfirmPassword':
                        var password = $('#SystemPassword').val();
                        if (value !== password) {
                            isValid = false;
                            errorMessage = 'Passwords do not match.';
                        }
                        break;
                }
            }

            //..apply validation styling and messages
            if (isValid) {
                $field.addClass('is-valid');
                if ($validationSpan.length) {
                    $validationSpan.removeClass('show-error').text('');
                }
            } else {
                $field.addClass('is-invalid');
                if ($validationSpan.length) {
                    $validationSpan.addClass('show-error').text(errorMessage);
                }
            }

            return isValid;
        }

        function setupRealTimeValidation() {
            //..validate on blur (when user leaves the field)
            $form.find('input, select').on('blur', function() {
                validateField($(this));
            });

            //..validate on input for specific fields
            $form.find('input').on('input', function() {
                var $field = $(this);
                var fieldId = $field.attr('id');
                
                //..real-time validation for password confirmation
                if (fieldId === 'User_ConfirmPassword' || fieldId === 'SystemPassword') {
                    setTimeout(function() {
                        validateField($('#User_ConfirmPassword'));
                    }, 100);
                }
            });
        }

        function submitFormWithAjax() {
            //..show loading state
            $submitBtn.prop('disabled', true).text('Processing...');
            
            //..prepare form data
            var formData = $form.serialize();
            
            $.ajax({
                url: $form.attr('action'),
                type: 'POST',
                data: formData,
                dataType: 'json',
                success: function(response) {
                    if (response.success) {
                        //..success - redirect to login
                        window.location.href = response.redirectUrl || '/Application/Login';
                    } else {
                        //..handle server-side validation errors
                        handleServerValidationErrors(response.errors);
                        $submitBtn.prop('disabled', false).text('Submit');
                    }
                },
                error: function(xhr, status, error) {
                    //..handle Ajax errors
                    console.error('Ajax error:', error);
                    alert('An error occurred while processing your request. Please try again.');
                    $submitBtn.prop('disabled', false).text('Submit');
                }
            });
        }

        function handleServerValidationErrors(errors) {
            //..clear previous errors
            $('.field-validation-error').removeClass('show-error');
            $('.form-control').removeClass('is-invalid');
            
            //..display server validation errors
            if (errors) {
                $.each(errors, function(fieldName, errorMessages) {
                    var $field = $(`[name='${fieldName}']`);
                    var $validationSpan = $(`[data-valmsg-for='${fieldName}']`);
                    
                    if ($field.length) {
                        $field.addClass('is-invalid');
                    }
                    
                    if ($validationSpan.length && errorMessages.length > 0) {
                        $validationSpan.addClass('show-error').text(errorMessages[0]);
                    }
                });
            }
            
            //..show validation summary
            $('#validation-summary').removeClass('d-none');
        }
        
        //..validation helper functions
        function validateAlphabetic(value) {
            return /^[a-zA-Z\s]+$/.test(value);
        }

        function validateAlphanumeric(value) {
            return /^[a-zA-Z0-9]+$/.test(value);
        }

        function validateNumeric(value) {
            return /^\d+$/.test(value);
        }

        function validateEmail(value) {
            return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);
        }

        function validatePassword(value) {
            //..at least 8 characters, 1 uppercase, 1 lowercase, 1 number, 1 special character
            return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/.test(value);
        }
    });
   
    $('#System_Language').on('change', function () {
        var selectedLang = $(this).val();
        if (selectedLang && selectedLang !== 'None') {
            window.location.href = '/Application/ChangeLanguage?language=' + selectedLang;
        }
    });

});
          

