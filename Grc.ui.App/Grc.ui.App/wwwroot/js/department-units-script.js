// Handle New button
$('#btnActionNew').on('click', function () {
    $('.component-slideout').show();
    $('.component-create-container').show();
    $('.component-edit-container').hide();
    $('.component-split-table-container').css('margin-left', '300px'); // slide table right
});

// Handle Edit button
$('#btnActionEdit').on('click', function () {
    $('.component-slideout').show();
    $('.component-edit-container').show();
    $('.component-create-container').hide();
    $('.component-split-table-container').css('margin-left', '300px');
});

// Close slideout 
function closeSlideout() {
    $('.component-slideout').hide();
    $('.component-split-table-container').css('margin-left', '0');
}
