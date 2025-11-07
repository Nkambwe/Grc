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

    $('.action-btn-process-new').on('click', function () {
        alert("New Process Button clicked");
    });

    $('.action-btn-proposed-list').on('click', function () {
        alert("Proposed Button clicked");
    });

    $('.action-btn-unchanged-list').on('click', function () {
        alert("Unchanged Button clicked");
    });

    $('.action-btn-due-review-list').on('click', function () {
        alert("Button clicked");
    });

    $('.action-btn-dorman-list').on('click', function () {
        alert("Dormant Button clicked");
    });

    $('.action-btn-cancelled-list').on('click', function () {
        alert("Cancelled Button clicked");
    });

    $('.action-btn-completed-list').on('click', function () {
        alert("Complete Button clicked");
    });

    $('.action-btn-dormant-list').on('click', function () {
        alert("Dormat Button clicked");
    });

    $('.action-btn-cancelled-list').on('click', function () {
        alert("Complete Button clicked");
    });

    $('.action-btn-process-export').on('click', function () {
        alert("Export Process Button clicked");
    });

});