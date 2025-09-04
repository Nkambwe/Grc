$(document).on("click", ".action-btn[data-action='quick']", function () {
    const componentKey = $(this).closest(".page-component").data("component");
    
    // Load same component into overlay
    $("#overlayContent").load(`/Components/${componentKey}`, function () {
        $("#quickOverlay").removeClass("hidden");
    });
});

// Back button closes overlay
$(document).on("click", ".back-btn", function () {
    $("#quickOverlay").addClass("hidden");
    $("#overlayContent").html("");
});

// Collapse button -> shrink overlay
$(document).on("click", ".collapse-btn", function () {
    $(".overlay-panel").toggleClass("w-3/4 w-1/4");
});
