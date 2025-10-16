$(document).ready(function () {

    //..route to home
    $('.action-btn-opsHome').on('click', function () {
        console.log("Home Button clicked");
        try {
            window.location.href = '/operations/dashboard';
        } catch (error) {
            console.error('Navigation failed:', error);
            showToast(error, type = 'error');
        }
    });

    $('.action-btn-totalProposed').on('click', function () {
        console.log("Proposed Button clicked");
    });

    $('.action-btn-totalUnchanged').on('click', function () {
        console.log("Unchanged Button clicked");
    });

    $('.action-btn-totalDueReview').on('click', function () {
        console.log("Button clicked");
    });

    $('.action-btn-totalDorman').on('click', function () {
        console.log("Dormant Button clicked");
    });

    $('.action-btn-totalCancelled').on('click', function () {
        console.log("Cancelled Button clicked");
    });

    $('.action-btn-totalCompleted').on('click', function () {
        console.log("Complete Button clicked");
    });

});