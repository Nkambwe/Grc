
function savePassword() {
    const oldPwd = $('#oldPassword').val().trim();
    const newPwd = $('#newPassword').val().trim();
    const confirmPwd = $('#confirmPassword').val().trim();
    const username = $('#username').val();
    const userId = $('#userId').val();

    // Clear all previous errors
    clearAllPasswordErrors();

    let hasError = false;

    // Validate old password
    if (!oldPwd) {
        showFieldError('oldPassword', 'old-password-change-error', 'Current password is required');
        hasError = true;
    }

    // Validate new password
    if (!newPwd) {
        showFieldError('newPassword', 'new-password-change-error', 'New password is required');
        hasError = true;
    } else {
        const validation = validatePassword(newPwd, true, oldPwd);
        if (!validation.isValid) {
            showFieldError('newPassword', 'new-password-change-error', validation.errors.join('<br>'));
            hasError = true;
        }
    }

    // Validate confirm password
    if (!confirmPwd) {
        showFieldError('confirmPassword', 'confirm-password-change-error', 'Confirm password is required');
        hasError = true;
    } else if (newPwd !== confirmPwd) {
        showFieldError('confirmPassword', 'confirm-password-change-error', 'Passwords do not match');
        showFieldError('newPassword', 'new-password-change-error', 'Passwords do not match');
        hasError = true;
    }

    if (hasError) {
        return;
    }

    // Mark as validated
    $('#validated-password-change').val('true');

    var record = {
        userId: userId,
        username: username,
        oldPassword: oldPwd,
        newPassword: newPwd,
        confirmPassword: confirmPwd,
        allowReuse: settings.CanReusePasswords
    };

    //..submit via AJAX
    submitPasswordChange(record);
}

function submitPasswordChange(record) {
    var url = "/application/passwords/persist-pwd";

    Swal.fire({
        title: "Password Reset...",
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
            'X-CSRF-TOKEN': getPwdToken()
        },
        success: function (res) {
            Swal.close();
            if (!res.success) {
                Swal.fire(res.message);
                return;
            }

            Swal.fire(res.message || "Your password has been reset")
                .then(() => {
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
                title: "Password reset Failed",
                text: errorMessage
            });
        }
    });
}

//..focus username input on load
$('#username-input').focus();

$('.action-btn-home').on('click', function () {
    window.location.href = '/admin/support';
});

//..helper function to clear all password errors
function clearAllPasswordErrors() {
    clearFieldError('oldPassword', 'old-password-change-error');
    clearFieldError('newPassword', 'new-password-change-error');
    clearFieldError('confirmPassword', 'confirm-password-change-error');
}

function clearFieldError(fieldId, errorId) {
    $('#' + fieldId).removeClass('is-invalid');
    $('#' + errorId).text('').hide();
}

function showFieldError(fieldId, errorId, message) {
    const $field = $('#' + fieldId);
    const $error = $('#' + errorId);

    // Add error styling to input field
    $field.addClass('is-invalid');
    // Show error message
    $error.html(message).show();
}

//..password strength indicator
function updatePasswordStrengthIndicator(strength) {
    const $indicator = $('#password-strength-indicator');
    if (!$indicator.length) return;

    const strengthLabels = ['Very Weak', 'Weak', 'Fair', 'Good', 'Strong'];
    const strengthColors = ['#dc3545', '#fd7e14', '#ffc107', '#28a745', '#198754'];

    $indicator.text(strengthLabels[strength - 1] || 'Very Weak');
    $indicator.css('color', strengthColors[strength - 1] || strengthColors[0]);
}

//..keep your toggle password visibility handlers
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

$('#newPassword').on('input', function () {
    const password = $(this).val();
    const validation = validatePassword(password);

    //..clear previous errors
    clearFieldError('newPassword', 'new-password-change-error');

    if (password && !validation.isValid) {
        //..show first error only during typing, or show all on blur
        showFieldError('newPassword', 'new-password-change-error', validation.errors[0]);
    }

    //..show password strength indicator
    const strength = getPasswordStrength(password);
    updatePasswordStrengthIndicator(strength);
});

//..calidation on blur,show all errors
$('#newPassword').on('blur', function () {
    const password = $(this).val();
    if (password) {
        const validation = validatePassword(password);
        if (!validation.isValid) {
            showFieldError('newPassword', 'new-password-change-error', validation.errors.join('<br>'));
        }
    }
});

//..confirm password validation
$('#confirmPassword').on('input', function () {
    const newPwd = $('#newPassword').val();
    const confirmPwd = $(this).val();

    clearFieldError('confirmPassword', 'confirm-password-change-error');

    if (confirmPwd && newPwd !== confirmPwd) {
        showFieldError('confirmPassword', 'confirm-password-change-error', 'Passwords do not match');
    }
});

//..password validation function
function validatePassword(password, isNewPassword = true, oldPassword = null) {
    const errors = [];

    //..check minimum length
    if (password.length < settings.MinimumLength) {
        errors.push(`Password must be at least ${settings.MinimumLength} characters long`);
    }

    //..check uppercase requirement
    if (settings.IncludeUpperChar && !/[A-Z]/.test(password)) {
        errors.push('Password must contain at least one uppercase letter');
    }

    //..check lowercase requirement
    if (settings.IncludeLowerChar && !/[a-z]/.test(password)) {
        errors.push('Password must contain at least one lowercase letter');
    }

    //..check special character requirement
    if (settings.IncludeSpecialChar && !/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) {
        errors.push('Password must contain at least one special character');
    }

    //..check if password contains user initials
    if (isNewPassword && settings.Initials && typeof password === 'string') {
        if (password.toUpperCase().includes(settings.Initials.toUpperCase())) {
            errors.push('Password should not contain your initials');
        }
    }

    //..check if password contains username
    const username = $('#username').val();
    if (isNewPassword && username && typeof password === 'string') {
        if (password.toUpperCase().includes(username.toUpperCase())) {
            errors.push('Password should not contain your username');
        }
    }

    //..check password reuse,only if CanReusePasswords is true
    if (!settings.CanReusePasswords && oldPassword && password === oldPassword) {
        errors.push('You cannot use the same password as the current. Please enter a new password');
    }

    return {
        isValid: errors.length === 0,
        errors: errors
    };
}

//..show password strength indicator
function getPasswordStrength(password) {
    let strength = 0;
    if (password.length >= settings.MinimumLength) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;
    if (/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) strength++;

    return strength;
}

//..display password requirements
function displayPasswordRequirements() {
    const requirements = [];
    requirements.push(`At least a minimum of ${settings.MinimumLength} characters`);
    if (settings.IncludeUpperChar) requirements.push('At least one uppercase letter');
    if (settings.IncludeLowerChar) requirements.push('At least one lowercase letter');
    if (settings.IncludeSpecialChar) requirements.push('At least one special character');
    if (settings.IncludeNumericChar) requirements.push('At least one numeric character');
    return requirements;
}

$(document).ready(function () {

    const requirements = displayPasswordRequirements();
    const $reqList = $('#password-requirements');
    requirements.forEach(req => {
        $reqList.append(`<li>${req}</li>`);
    });

    $('#btnChange').on('click', savePassword);
});

function getPwdToken() {
    return $('meta[name="csrf-token"]').attr('content');
}
