
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

function changePassword(e) {
    e.preventDefault();
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
     // User dropdown toggle
    window.toggleDropdown = function() {
        $('#userDropdown').toggleClass('is-active');
    };

     $('.avator-button').on('click', function(e) {
        e.preventDefault();
        $('#userDropdown').toggleClass('is-active');
    });
    
    //..close dropdown on escape key
    $(document).keydown(function(event) {
        if (event.key === 'Escape') {
            $('#userDropdown').removeClass('is-active');
        }
    });

    //..sidebar toggle functionality
    function toggleSidebar() {
        const $sidebarContainer = $('.sidebar-container');
        const $headerContainer = $('.header-bar');
        const isMobile = $(window).width() <= 768;
        if (isMobile) {
            //..on mobile, toggle the 'open' class
            $sidebarContainer.toggleClass('open');
        } else {
            //..on desktop, toggle the 'collapsed' class
            $sidebarContainer.toggleClass('collapsed');
            $headerContainer.toggleClass('collapsed');
        }
    }
   
    // Submenu toggle functionality
    function toggleSubmenu($element) {
        const menuId = $element.data('id');
        const $submenu = $('#' + menuId);
        const $parentItem = $element.closest('.sidebar-item');
        
        if ($submenu.length) {
            $submenu.toggleClass('open');
            $parentItem.toggleClass('open');
        }
    }
    
    // Close dropdown when clicking outside
    $(document).on('click', function(event) {
        const $dropdown = $('#userDropdown');
        const $dropdownTrigger = $dropdown.find('.dropdown-trigger');
        
        if ($dropdown.length && $dropdownTrigger.length && !$dropdown.is(event.target) && $dropdown.has(event.target).length === 0) {
            $dropdown.removeClass('is-active');
        }
    });
    
    // Handle submenu toggles
    $(document).on('click', '.toggle-submenu', function(event) {
        event.preventDefault();
        toggleSubmenu($(this));
    });
    
    // Handle window resize
    $(window).on('resize', function() {
        const $sidebarContainer = $('.sidebar-container');
        const isMobile = $(window).width() <= 768;
        
        if (!isMobile) {
            // Remove mobile-specific classes when switching to desktop
            $sidebarContainer.removeClass('open');
        } else {
            // Remove desktop-specific classes when switching to mobile
            $sidebarContainer.removeClass('collapsed');
        }
    });
    
    // Sidebar header toggle (the menu button in sidebar header)
    $('.sidebar-geader button').on('click', function() {
        toggleSidebar();
    });
    
    // Mobile menu toggle (hamburger in header)
    $('.mobile-menu-toggle').on('click', function() {
        toggleSidebar();
    });
    
    // User dropdown toggle
    $('#userDropdown .dropdown-trigger button').on('click', function(event) {
        event.stopPropagation();
        toggleDropdown();
    });
    
    //..initialize active states
    function initializeActiveStates() {
        const currentPath = window.location.pathname.toLowerCase();
        const $sidebarLinks = $('.sidebar-item a[href]');

        $sidebarLinks.each(function() {
            const $link = $(this);
            const href = $link.attr('href');

            //..skip empty/placeholder links
            if (!href || href === '#' || href.startsWith("javascript:")) return;

            const linkPath = href.toLowerCase();

            //..only mark exact path match as active
            if (currentPath === linkPath) {
                const $sidebarItem = $link.closest('.sidebar-item');
                $sidebarItem.addClass('active'); 

                //..it's inside a submenu, expand it and highlight parent
                const $submenu = $link.closest('.sidebar-submenu');
                if ($submenu.length) {
                    $submenu.addClass('open');
                    const submenuId = $submenu.attr('id');
                    const $parentToggle = $('[data-id="' + submenuId + '"]');
                    if ($parentToggle.length) {
                        const $parentItem = $parentToggle.closest('.sidebar-item');
                        $parentItem.addClass('open parent-active'); 
                    }
                }
            }
        });
    }

    //..smooth transitions
    function enableSmoothTransitions() {
        $('.sidebar-container').css({
            'transition': 'width 0.3s ease, left 0.3s ease'
        });
    }
    
    // Tooltip functionality for collapsed sidebar
    function initializeTooltips() {
        $('.sidebar-item a').each(function() {
            const $item = $(this);
            const $text = $item.find('.sidebar-text');
            
            if ($text.length) {
                $item.attr('title', $text.text().trim());
            }
        });
    }
    
    // Handle keyboard navigation
    $(document).on('keydown', function(event) {
        // ESC key closes dropdown and mobile sidebar
        if (event.key === 'Escape') {
            const $dropdown = $('#userDropdown');
            const $sidebarContainer = $('.sidebar-container');
            
            if ($dropdown.hasClass('is-active')) {
                $dropdown.removeClass('is-active');
            }
            
            if ($(window).width() <= 768 && $sidebarContainer.hasClass('open')) {
                $sidebarContainer.removeClass('open');
            }
        }
    });
    
    //..handle sidebar item clicks
    $('.sidebar-item a').on('click', function() {
        //..remove active class from all items
        $('.sidebar-item').removeClass('active');
        // Add active class to clicked item
        $(this).closest('.sidebar-item').addClass('active');
    });
    
    //..smooth scrolling for main content
    $('.main-content').css({
        'scroll-behavior': 'smooth'
    });
    
    //..auto-hide mobile sidebar when clicking main content
    $('.main-content').on('click', function() {
        if ($(window).width() <= 768) {
            $('.sidebar-container').removeClass('open');
        }
    });
    
    //..improve submenu performance
    $('.sidebar-item.with-submenu > a').on('click', function(event) {
        if (!$(this).hasClass('toggle-submenu')) {
            const $parentItem = $(this).closest('.sidebar-item');
            const $submenu = $parentItem.find('.sidebar-submenu');
            
            if ($submenu.length) {
                event.preventDefault();
                $submenu.toggleClass('open');
                $parentItem.toggleClass('open');
            }
        }
    });
    
    //..animate number counters
    function animateCounters() {
        $('.notification-count').each(function() {
            const $counter = $(this);
            const finalValue = parseInt($counter.text());
            
            if (!isNaN(finalValue)) {
                $counter.text('0');
                $({ value: 0 }).animate({ value: finalValue }, {
                    duration: 1000,
                    step: function() {
                        $counter.text(Math.ceil(this.value));
                    }
                });
            }
        });
    }
    
    //..add loading states for navigation
    $('.sidebar-item a[href]:not([href="#"])').on('click', function() {
        const $link = $(this);
        const originalText = $link.find('.sidebar-text').text();
        
        //..add loading state
        $link.addClass('loading');
        
        // Remove loading state after a short delay (in case navigation is quick)
        setTimeout(function() {
            $link.removeClass('loading');
        }, 500);
    });
    
    let isExpanded = false;

    $('#expandBtn, #componetExpandBtn').click(function() {
        isExpanded = !isExpanded;         
        if (isExpanded) {
            // Expand the main content
            $('#layoutWrapper').addClass('expanded');
            $('#toggleIcon, #componentExpandIncon').removeClass('mdi-arrow-expand').addClass('mdi-arrow-collapse');
        } else {
            // Collapse back to normal
            $('#layoutWrapper').removeClass('expanded');
            $('#toggleIcon, #componentExpandIncon').removeClass('mdi-arrow-collapse').addClass('mdi-arrow-expand');
        }
    });
    
    // Initialize everything
    initializeActiveStates();
    enableSmoothTransitions();
    initializeTooltips();
    animateCounters();
    
    // Make functions available globally for onclick handlers (if still needed)
    window.toggleSidebar = toggleSidebar;
    window.toggleDropdown = toggleDropdown;
    
    // Optional: Add some nice hover effects
    $('.sidebar-item').hover(
        function() {
            $(this).addClass('hover');
        },
        function() {
            $(this).removeClass('hover');
        }
    );
    
    // Add ripple effect on click (optional modern touch)
    $('.sidebar-item a').on('click', function(e) {
        const $ripple = $('<span class="ripple"></span>');
        const $button = $(this);
        const buttonOffset = $button.offset();
        const xPos = e.pageX - buttonOffset.left;
        const yPos = e.pageY - buttonOffset.top;
        
        $ripple.css({
            position: 'absolute',
            left: xPos + 'px',
            top: yPos + 'px',
            width: '0',
            height: '0',
            borderRadius: '50%',
            background: 'rgba(255, 255, 255, 0.3)',
            transform: 'translate(-50%, -50%)',
            animation: 'ripple 0.6s ease-out',
            pointerEvents: 'none',
            zIndex: '1'
        });
        
        $button.css('position', 'relative').append($ripple);
        
        setTimeout(function() {
            $ripple.remove();
        }, 600);
    });
    
    // Add CSS for ripple animation
    $('<style>')
        .prop('type', 'text/css')
        .html(`
            @keyframes ripple {
                0% {
                    width: 0;
                    height: 0;
                    opacity: 1;
                }
                100% {
                    width: 40px;
                    height: 40px;
                    opacity: 0;
                }
            }
            .sidebar-item a.loading .sidebar-text::after {
                content: '...';
                animation: dots 1.5s infinite;
            }
            @keyframes dots {
                0%, 20% { content: ''; }
                40% { content: '.'; }
                60% { content: '..'; }
                80%, 100% { content: '...'; }
            }
        `)
        .appendTo('head');
});