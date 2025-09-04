 $(document).ready(function () {

    $("#moreActionsBtn").on("click", function (e) {
        e.stopPropagation(); 
        $(".header-more-list").toggleClass("open");
    });

    $(document).on("click", function (e) {
        if (!$(e.target).closest(".header-more-list").length) {
            $(".header-more-list").removeClass("open");
        }
    });

});
