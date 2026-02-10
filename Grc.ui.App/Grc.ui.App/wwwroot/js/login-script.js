$(document).ready(function() {
    const $usernameStage = $('#username-stage');
    const $passwordStage = $('#password-stage');
    const $successStage = $('#success-stage');
    const $usernameForm = $('#username-form');
    const $passwordForm = $('#password-form');
    const $backButton = $('#back-to-username');
    const $expiryStage = $('#password-expiry-stage');
    
    //..username form submission
    $usernameForm.on('submit', async function(e) {
        e.preventDefault();
        
        const username = $(this).find('input[name="Username"]').val().trim();
        
        if (!username) {
            showError('username', 'Username is required');
            return;
        }

        console.log('Username:', username);
        await validateUsername(username);
    });
    
    //..password form submission
    $passwordForm.on('submit', async function(e) {
        e.preventDefault();
        
        const $form = $(this);
        const password = $form.find('input[name="Password"]').val().trim();
        const username = $form.find('input[name="Username"]').val() || $('#validated-username').val();
        
        if (!password) {
            showError('password', 'Password is required');
            return;
        }
        
        //..create FormData manually to avoid DOM element issues
        const formData = new FormData();
        formData.append('Username', username);
        formData.append('Password', password);
        console.log("Username >> ", username);
        console.log("Password >> ", password);

        //..add any other form fields
        $form.find('input, select, textarea').each(function() {
            const $field = $(this);
            const name = $field.attr('name');
            const value = $field.val();
            
            if (name && value && name !== 'Username' && name !== 'Password') {
                formData.append(name, value);
            }
        });
        
        await authenticateUser(username, password);
    });
    
    // Back button
    $backButton.on('click', function() {
        slideToStage('username');
        clearErrors();
    });
    
    //..validate username function
    async function validateUsername(username) {
        const $button = $usernameForm.find('.btn-login');
        setButtonLoading($button, true);
        clearErrors();
        
        try {
            const token = getAntiForgeryToken();
            console.log('Username:', username);
            
            const response = await $.ajax({
                url: '/login/validate-username',
                type: 'POST',
                data: { Username: username },
                dataType: 'json',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'X-CSRF-TOKEN': token
                },
                
            });
            
            if (response.success) {
                //..username valid, move to password stage
                $('#username-display').text(username);
                $('#display-name').text(response.displayName || '');
                $('#validated-username').val(username);
                slideToStage('password');
                
                //..focus password input after animation
                setTimeout(() => {
                    $('#password-input').focus();
                }, 400);
                
            } else {
                showError('username', response.message || 'Username validation failed');
            }
            
        } catch (error) {
            console.error('Username validation error:', error);
            let errorMessage = 'Service unavailable. Please try again later.';
            
            // Handle jQuery AJAX error object
            if (error.responseJSON && error.responseJSON.message) {
                errorMessage = error.responseJSON.message;
            } else if (error.responseText) {
                try {
                    const errorData = JSON.parse(error.responseText);
                    errorMessage = errorData.message || errorMessage;
                } catch (parseError) {
                    //..keep default error message
                    errorMessage = "Ajax error-check it out in JS";
                    console.log('Supposed to show default error message');
                }
            }
            
            showError('username', errorMessage);
        } finally {
            setButtonLoading($button, false);
        }
    }
    
    //..authenticate user function
     async function authenticateUser(username, password) {
        const $button = $passwordForm.find('.btn-login');
        setButtonLoading($button, true);
        clearErrors();
        
        try {
            const token = getAntiForgeryToken();
            console.log('Authenticating user:', username);
            
            //..create login data
            const loginData = {
                Username: username,
                Password: password,
                IsUsernameValidated: true,
                DisplayName:username,
                RememberMe: $('#remember-me').is(':checked')
            };
            console.log('Authentication body:', loginData);
            const response = await $.ajax({
                url: '/login/userlogin',
                method: 'POST',
                headers: {
                    'X-CSRF-TOKEN': token,
                    'Content-Type': 'application/json'
                },
                data: JSON.stringify(loginData),
                dataType: 'json'
            });
            
            console.log('Authentication response:', response);
            if (response.success) {
                console.log('stage here :', response.stage);
                if (response.stage === "password-expired") {
                    slideToStage('password-expired');
                    return;
                }

                slideToStage('success');

                setTimeout(() => {
                    const redirectUrl = response.redirectUrl || '/login/validate-username';
                    window.location.href = redirectUrl;
                }, 1500);
            } else {
                //..handle different types of errors
                let errorMessage = 'Authentication failed. Please try again.';
                
                if (response.error && response.error.message) {
                    errorMessage = response.error.message;
                } else if (response.message) {
                    errorMessage = response.message;
                }
                
                showError('password', errorMessage);
            }
            
        } catch (error) {
            console.error('Authentication error:', error);
            let errorMessage = 'Authentication failed. Please try again.';
            
            //..handle jQuery AJAX error object
            if (error.responseJSON) {
                if (error.responseJSON.error && error.responseJSON.error.message) {
                    errorMessage = error.responseJSON.error.message;
                } else if (error.responseJSON.message) {
                    errorMessage = error.responseJSON.message;
                }
            } else if (error.responseText) {
                try {
                    const errorData = JSON.parse(error.responseText);
                    if (errorData.error && errorData.error.message) {
                        errorMessage = errorData.error.message;
                    } else if (errorData.message) {
                        errorMessage = errorData.message;
                    }
                } catch (parseError) {
                   //..keep default error message
                    errorMessage = "Ajax error-check it out in JS";
                    console.log('Supposed to show default error message');
                }
            }
            
            showError('password', errorMessage);
        } finally {
            setButtonLoading($button, false);
        }
    }

    function slideToStage(stageName) {
        const $stages = $usernameStage
                        .add($passwordStage)
                        .add($expiryStage)
                        .add($successStage);
        
        $stages.removeClass('active slide-out-left');
        let $targetStage;
        switch(stageName) {
            case 'username':
                $targetStage = $usernameStage;
                break;
            case 'password':
                $targetStage = $passwordStage;
                break;
            case 'password-expired':
                $targetStage = $expiryStage;
                break;
            case 'success':
                $targetStage = $successStage;
                break;
        }
        
        if ($targetStage && $targetStage.length) {
            setTimeout(() => {
                $targetStage.addClass('active');
            }, 50);
        }
    }
    
    function showError(stage, message) {
        const $errorElement = $(`#${stage}-error`);
        const $inputElement = $(`#${stage}-input`);
        
        if ($errorElement.length && $inputElement.length) {
            $errorElement.text(message).addClass('show');
            $inputElement.addClass('error');
            
            //..remove error on input (one-time event)
            $inputElement.one('input', function() {
                clearError(stage);
            });
        }
    }
    
    function clearError(stage) {
        const $errorElement = $(`#${stage}-error`);
        const $inputElement = $(`#${stage}-input`);
        
        if ($errorElement.length && $inputElement.length) {
            $errorElement.removeClass('show');
            $inputElement.removeClass('error');
        }
    }
    
    function clearErrors() {
        clearError('username');
        clearError('password');
    }
    
    function setButtonLoading($button, loading) {
        const $btnText = $button.find('.btn-text');
        const $btnSpinner = $button.find('.btn-spinner');
        
        if (loading) {
            $btnText.css('opacity', '0');
            $btnSpinner.show();
            $button.prop('disabled', true);
        } else {
            $btnText.css('opacity', '1');
            $btnSpinner.hide();
            $button.prop('disabled', false);
        }
    }
    
   $('#password-expiry-form').on('submit', async function (e) {
        e.preventDefault();

        clearError('new-password-change');
        clearError('confirm-password-change');

        const oldPwd = $('#oldPassword').val().trim();
        const newPwd = $('#newPassword').val().trim();
        const confirmPwd = $('#confirmPassword').val().trim();
        const username = $('#validated-username').val();

            var record = {
                username: username,
                oldPassword: oldPwd,
                newPassword: newPwd,
                confirmPassword: confirmPwd
            }
        
        if (!oldPwd) {
            showFieldError('oldPassword', 'old-password-change-error', 'Current password is required');
            return;
        }

        if (!newPwd) {
            showFieldError('newPassword', 'new-password-change-error', 'New password is required');
            return;
        }

        if (!confirmPwd) {
            showFieldError('confirmPassword', 'confirm-password-change-error', 'Confirm password is required');
            return;
        }

        if (newPwd !== confirmPwd) {
            showFieldError('confirmPassword', 'confirm-password-change-error', 'Passwords do not match');
            showFieldError('newPassword', 'new-password-change-error', 'Passwords do not match');
            return;
        }

        const $button = $('#password-expiry-form .btn-login');
        setButtonLoading($button, true);

        try {
            console.log("Clicked >> ", record);
            const response = await $.ajax({
                url: '/login/expired-password',
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(record),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'X-CSRF-TOKEN': getAntiForgeryToken()
                }
            });

            if (response.success) {
                slideToStage('success');
                setTimeout(() => {
                    window.location.href = response.redirectUrl || '/grc/compliance';
                }, 1200);
            } else {
                showError('new-password-change', response.message || 'Password change failed');
            }

        } catch (xhr) {

            let errorMessage = "Password not changed. An unexpected error occurred";

            if (xhr.responseJSON?.message)
                errorMessage = xhr.responseJSON.message;

            showError('confirm-password-change', errorMessage);
            showError('new-password-change', errorMessage);

        } finally {
            setButtonLoading($button, false);
        }

    });

   $('#newPassword, #confirmPassword').on('input', function () {
        $('#new-password-change-error').removeClass('show');
        $('#confirm-password-change-error').removeClass('show');

        const newPwd = $('#newPassword').val();
        const confirmPwd = $('#confirmPassword').val();

        if (newPwd && confirmPwd && newPwd !== confirmPwd) {
            showFieldError('confirmPassword', 'confirm-password-change-error', 'Passwords do not match');
        }
    });

    function showFieldError(inputId, errorId, message) {
        const $errorElement = $('#' + errorId);
        const $inputElement = $('#' + inputId);

        if ($errorElement.length && $inputElement.length) {
            $errorElement.text(message).addClass('show');
            $inputElement.addClass('error');

            $inputElement.one('input', function () {
                $errorElement.removeClass('show');
                $inputElement.removeClass('error');
            });
        }
    }

    function getAntiForgeryToken() {
        // Get token from meta tag (with your current typo)
        let token = $('meta[name="csrf-token"]').attr('content');
        
        // Debug logging
        console.log('CSRF Token from meta tag:', token);
        
        return token || '';
    }

    // Focus username input on load
    $('#username-input').focus();
    $('#toggle-password').on('click', function () {
        const $passwordInput = $('#password-input');
        const isPassword = $passwordInput.attr('type') === 'password';

        $passwordInput.attr('type', isPassword ? 'text' : 'password');
        $(this).toggleClass('mdi-eye').toggleClass('mdi-eye-off');
    });

    $('#toggle-new-password').on('click', function () {
        const $passwordInput = $('#newPassword');
        const isPassword = $passwordInput.attr('type') === 'password';

        $passwordInput.attr('type', isPassword ? 'text' : 'password');
        $(this).toggleClass('mdi-eye').toggleClass('mdi-eye-off');
    });
    
    $('#toggle-old-password').on('click', function () {
        const $passwordInput = $('#oldPassword');
        const isPassword = $passwordInput.attr('type') === 'password';

        $passwordInput.attr('type', isPassword ? 'text' : 'password');
        $(this).toggleClass('mdi-eye').toggleClass('mdi-eye-off');
    });

    $('#toggle-confirm-password').on('click', function () {
        const $passwordInput = $('#confirmPassword');
        const isPassword = $passwordInput.attr('type') === 'password';

        $passwordInput.attr('type', isPassword ? 'text' : 'password');
        $(this).toggleClass('mdi-eye').toggleClass('mdi-eye-off');
    });
});