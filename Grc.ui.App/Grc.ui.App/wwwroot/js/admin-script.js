$(document).ready(function() {
     // User dropdown toggle
    window.toggleDropdown = function() {
        console.log("Clicked");
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
        const isMobile = $(window).width() <= 768;
        if (isMobile) {
            //..on mobile, toggle the 'open' class
            $sidebarContainer.toggleClass('open');
        } else {
            //..on desktop, toggle the 'collapsed' class
            $sidebarContainer.toggleClass('collapsed');
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
    
    // Initialize active states
    function initializeActiveStates() {
        const currentPath = window.location.pathname;
        const $sidebarLinks = $('.sidebar-item a[href]');
        
        $sidebarLinks.each(function() {
            const $link = $(this);
            const href = $link.attr('href');
            
            if (href && currentPath.includes(href) && href !== '#') {
                $link.closest('.sidebar-item').addClass('active');
                
                // If it's in a submenu, open the parent
                const $submenu = $link.closest('.sidebar-submenu');
                if ($submenu.length) {
                    $submenu.addClass('open');
                    const submenuId = $submenu.attr('id');
                    const $parentToggle = $('[data-id="' + submenuId + '"]');
                    if ($parentToggle.length) {
                        $parentToggle.closest('.sidebar-item').addClass('open');
                    }
                }
            }
        });
    }
    
    // Smooth transitions
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
    
    // Handle sidebar item clicks (for better UX)
    $('.sidebar-item a').on('click', function() {
        // Remove active class from all items
        $('.sidebar-item').removeClass('active');
        // Add active class to clicked item
        $(this).closest('.sidebar-item').addClass('active');
    });
    
    // Smooth scrolling for main content
    $('.main-content').css({
        'scroll-behavior': 'smooth'
    });
    
    // Auto-hide mobile sidebar when clicking main content
    $('.main-content').on('click', function() {
        if ($(window).width() <= 768) {
            $('.sidebar-container').removeClass('open');
        }
    });
    
    // Enhanced submenu functionality
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
    
    // Animate number counters (if you have notification counts, etc.)
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
    
    // Add loading states for navigation
    $('.sidebar-item a[href]:not([href="#"])').on('click', function() {
        const $link = $(this);
        const originalText = $link.find('.sidebar-text').text();
        
        // Add loading state (optional)
        $link.addClass('loading');
        
        // Remove loading state after a short delay (in case navigation is quick)
        setTimeout(function() {
            $link.removeClass('loading');
        }, 500);
    });
    
    // Search functionality (if you want to add search in sidebar)
    function initializeSearch() {
        const $searchInput = $('<input>', {
            type: 'text',
            class: 'sidebar-search',
            placeholder: 'Search menu...',
            style: 'display: none; width: calc(100% - 2rem); margin: 0.5rem 1rem; padding: 0.5rem; border: 1px solid #ddd; border-radius: 4px; font-size: 14px;'
        });
        
        // Add search input to sidebar (uncomment if needed)
        // $('.sidebar-geader').after($searchInput);
        
        $searchInput.on('input', function() {
            const searchTerm = $(this).val().toLowerCase();
            
            $('.sidebar-item').each(function() {
                const $item = $(this);
                const text = $item.find('.sidebar-text').text().toLowerCase();
                
                if (text.includes(searchTerm) || searchTerm === '') {
                    $item.show();
                } else {
                    $item.hide();
                }
            });
        });
    }
    
    // Initialize everything
    initializeActiveStates();
    enableSmoothTransitions();
    initializeTooltips();
    animateCounters();
    // initializeSearch(); // Uncomment if you want search functionality
    
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