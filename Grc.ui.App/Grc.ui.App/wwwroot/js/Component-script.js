 $(document).ready(function () {
    let isCompExpanded = false;

    $("#moreActionsBtn").on("click", function (e) {
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
            $('#layoutWrapper').addClass('expanded');
            $('#compExpandIncon').removeClass('mdi-arrow-expand').addClass('mdi-arrow-collapse');
            $(this).attr('title', 'Exit Full Screen');
        } else {
            //..collapse back to normal
            $('#layoutWrapper').removeClass('expanded');
            $('#compExpandIncon').removeClass('mdi-arrow-collapse').addClass('mdi-arrow-expand');
            $(this).attr('title', 'Full Screen');
        }
    });

});
