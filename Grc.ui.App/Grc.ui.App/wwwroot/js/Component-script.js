 $(document).ready(function () {
    let isCompExpanded = false;

    $("#moreActionsBtn1, #moreActionsBtn2").on("click", function (e) {
        e.stopPropagation(); 
        $(".header-more-list").toggleClass("open");
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
    $('#btnActionUnits').on('click', function(e) {
        e.preventDefault();
        openModal();
    });

    //..close component modal when clicking the X button
    $('.component-modal-close,.componet-modal-back').on('click', function() {
        closeModal();
    });
    
    //..close component modal when clicking outside the modal container
    $('#componentModal').on('click', function(e) {
        if (e.target === this) {
            closeModal();
        }
    });
    
    //..close modal with Escape key
    $(document).on('keydown', function(e) {
        if (e.key === 'Escape') {
            closeModal();
        }
    });

    function openModal() {
        $('#componentModal').show();
    }

    function closeModal() {
        $('#componentModal').hide();
        $('#componentodalContent').empty();
    }

});
