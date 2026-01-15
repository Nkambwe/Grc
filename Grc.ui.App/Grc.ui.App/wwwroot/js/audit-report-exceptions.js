

//..route to home
$('.action-btn-audit-home').on('click', function () {
    try {
        window.location.href = '/grc/compliance/audit/dashboard';
    } catch (error) {
        console.error('Navigation failed:', error);
        showToast(error, type = 'error');
    }
});

$(document).ready(function () {

    $('#auditForm').on('submit', function (e) {
        e.preventDefault();
    });

});