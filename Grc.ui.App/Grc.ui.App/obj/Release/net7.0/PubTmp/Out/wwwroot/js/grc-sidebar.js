
async function performLogout(event) {
    event.preventDefault();
    // Show loading state
    const logoutLink = event.currentTarget;
    const originalHtml = logoutLink.innerHTML;
    logoutLink.innerHTML = '<span><i class="mdi mdi-loading mdi-spin"></i></span><span>Logging out...</span>';
    
    try {
        const token = getAntiForgeryToken();
        console.log('Performing logout...');
        
        const response = await $.ajax({
            url: window.ADMIN_LOGOUT_URL,
            method: 'POST',
            headers: {
                'X-CSRF-TOKEN': token,
                'Content-Type': 'application/json'
            },
            dataType: 'json'
        });
        
        console.log('Logout response:', response);
        
        if (response.success) {
            console.log('Logout successful, redirecting...');
            
            //..show success message briefly before redirect
            logoutLink.innerHTML = '<span><i class="mdi mdi-check"></i></span><span>Logged out!</span>';
            
            //..redirect after short delay
            setTimeout(() => {
                const redirectUrl = response.redirectUrl || '/Application/Login';
                window.location.href = redirectUrl;
            }, 1000);
            
        } else {
            console.error('Logout failed:', response);
            
            //..show error message
            let errorMessage = 'Logout failed. Please try again.';
            if (response.error && response.error.message) {
                errorMessage = response.error.message;
            } else if (response.message) {
                errorMessage = response.message;
            }
            
            showToast(errorMessage, 'error');
            logoutLink.innerHTML = originalHtml; 
        }
    } catch (error) {
        console.error('Logout error:', error);
        let errorMessage = 'Logout failed. Please try again.';
        
        //..error handling
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
                errorMessage = "Logout error - check console for details";
                console.log('Error parsing logout response');
            }
        }
        
        showToast(errorMessage, 'error');
        logoutLink.innerHTML = originalHtml; 
    }
}

function getAntiForgeryToken() {
    //..get token from meta tag
    let token = $('meta[name="csrf-token"]').attr('content');
        
    // Debug logging
    console.log('CSRF Token from meta tag:', token);
        
    return token || '';
}

//..notifications
function showToast(message, type = 'info') {
    //..create toast element
    const toast = $(`
        <div class="toast-notification toast-${type}">
            <i class="mdi ${type === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'}"></i>
            <span>${message}</span>
        </div>
    `);
    
    //..add to page
    $('body').append(toast);
    
    // Show with animation
    setTimeout(() => toast.addClass('show'), 100);
    
    //..remove after delay
    setTimeout(() => {
        toast.removeClass('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

$(document).ready(function() {
     //..user dropdown toggle
    window.toggleDropdown = function() {
        $('#userDropdown').toggleClass('is-active');
        console.log("User dropdown clicked");
    };

     $('.avator-button').on('click', function(e) {
        e.preventDefault();
        $('#userDropdown').toggleClass('is-active');
        console.log("User avator clicked");
    });
    
    //..user dropdown toggle
    $('#userDropdown .dropdown-trigger button').on('click', function(event) {
        event.stopPropagation();
        toggleDropdown();
    });
    
    //..close dropdown on escape key
    $(document).keydown(function(event) {
        if (event.key === 'Escape') {
            $('#userDropdown').removeClass('is-active');
        }
    });

    // Close dropdown when clicking outside
    $(document).on('click', function(event) {
        const $dropdown = $('#userDropdown');
        const $dropdownTrigger = $dropdown.find('.dropdown-trigger');
        
        if ($dropdown.length && $dropdownTrigger.length && !$dropdown.is(event.target) && $dropdown.has(event.target).length === 0) {
            $dropdown.removeClass('is-active');
        }
    });

});