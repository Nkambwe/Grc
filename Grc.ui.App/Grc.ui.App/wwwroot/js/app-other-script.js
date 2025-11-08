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

});