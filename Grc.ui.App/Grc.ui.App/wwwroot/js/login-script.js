$(document).ready(function() {
    const $usernameStage = $('#username-stage');
    const $passwordStage = $('#password-stage');
    const $successStage = $('#success-stage');
    const $usernameForm = $('#username-form');
    const $passwordForm = $('#password-form');
    const $backButton = $('#back-to-username');
    
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
        console.log("Username >> ", password);

        //..add any other form fields
        $form.find('input, select, textarea').each(function() {
            const $field = $(this);
            const name = $field.attr('name');
            const value = $field.val();
            
            if (name && value && name !== 'Username' && name !== 'Password') {
                formData.append(name, value);
            }
        });
        
        await authenticateUser(formData);
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
                    // Keep default error message
                }
            }
            
            showError('username', errorMessage);
        } finally {
            setButtonLoading($button, false);
        }
    }
    
    //..authenticate user function
    async function authenticateUser(formData) {
        const $button = $passwordForm.find('.btn-login');
        setButtonLoading($button, true);
        clearErrors();
        
        try {
            const token = getAntiForgeryToken();
            console.log('Sending token in headers:', token);
            
            // Add token to FormData if not already present
            if (!formData.has('__RequestVerificationToken')) {
                formData.append('__RequestVerificationToken', token);
            }
            
            const response = await $.ajax({
                url: '/login/userlogin',
                method: 'POST',
                headers: {
                    'X-CSRF-TOKEN': token
                },
                data: formData,
                processData: false,
                contentType: false
            });
            
            if (response.success) {
                //..show success stage
                slideToStage('success');
                
                //..redirect after showing success
                setTimeout(() => {
                    const redirectUrl = response.redirectUrl || '/dashboard/';
                    window.location.href = redirectUrl;
                }, 1500);
                
            } else {
                showError('password', response.message || 'Authentication failed');
            }
            
        } catch (error) {
            console.error('Authentication error:', error);
            let errorMessage = 'Authentication failed. Please try again.';
            
            //..handle jQuery AJAX error object
            if (error.responseJSON && error.responseJSON.message) {
                errorMessage = error.responseJSON.message;
            } else if (error.responseText) {
                try {
                    const errorData = JSON.parse(error.responseText);
                    errorMessage = errorData.message || errorMessage;
                } catch (parseError) {
                    //..keep default error message
                }
            }
            
            showError('password', errorMessage);
        } finally {
            setButtonLoading($button, false);
        }
    }

    function slideToStage(stageName) {
        const $stages = $usernameStage.add($passwordStage).add($successStage);
        
        $stages.removeClass('active slide-out-left');
        
        let $targetStage;
        switch(stageName) {
            case 'username':
                $targetStage = $usernameStage;
                break;
            case 'password':
                $targetStage = $passwordStage;
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
});