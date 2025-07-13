$(function() {

    $(document).ready(function () {
        var $steps = $('.form-step');
        var $stepNavs = $('.step-nav .step');
        var $prevBtn = $('.btn-prev');
        var $nextBtn = $('.btn-next');
        var $submitBtn = $('.btn-submit');
        var currentStep = 0;
        var $form = $('#registration-form');

        //..initialize
        showStep(currentStep);

        //..next button click handler
        $nextBtn.on('click', function() {
            if (validateStep(currentStep)) {
                currentStep++;
                showStep(currentStep);
            }
        });

        //..previous button click handler
        $prevBtn.on('click', function() {
            currentStep--;
            showStep(currentStep);
        });

        //..password toggle functionality
        $('.password-toggle').on('click', function() {
            var $toggle = $(this);
            var $input = $toggle.parent().find('input');
            var $icon = $toggle.find('i');
        
            if ($input.attr('type') === 'password') {
                $input.attr('type', 'text');
                $icon.removeClass('mdi-eye-off').addClass('mdi-eye');
            } else {
                $input.attr('type', 'password');
                $icon.removeClass('mdi-eye').addClass('mdi-eye-off');
            }
        });

        //..auto-advance Select2 fields
        $('.language-select').on('select2:select', function() {
            if (currentStep === 0 && validateStep(currentStep)) {
                currentStep++;
                showStep(currentStep);
            }
        });

        function showStep(stepIndex) {
            //..hide all steps and show current one
            $steps.removeClass('active').eq(stepIndex).addClass('active');
        
            //..update navigation indicators
            $stepNavs.removeClass('active completed')
                .each(function(index) {
                    var $nav = $(this);
                    if (index < stepIndex) {
                        $nav.addClass('completed');
                    } else if (index === stepIndex) {
                        $nav.addClass('active');
                    }
                });

            //..update button visibility
            $prevBtn.prop('disabled', stepIndex === 0);
            $nextBtn.toggle(stepIndex !== $steps.length - 1);
            $submitBtn.toggle(stepIndex === $steps.length - 1);
        }

        function validateStep(stepIndex) {
            var isValid = true;
            var $currentFields = $steps.eq(stepIndex).find('[required]');
           
        
            $currentFields.each(function() {
                var $field = $(this);
                var fieldName = $field.attr('name'); 
                var $validationSpan = $(`[data-valmsg-for='${fieldName}']`);

                if (!$field.val().trim()) {
                    $field.addClass('is-invalid');
                    isValid = false;

                    //..show validation message if span exists
                    if ($validationSpan.length) {
                        $validationSpan.addClass('show-error');
                    }
                } else {
                    $field.removeClass('is-invalid');

                    //..clear validation message
                    if ($validationSpan.length) {
                         $validationSpan.removeClass('show-error');
                    }
                }
            });

            if (!isValid) {
                $('#validation-summary').removeClass('d-none');
                $('html, body').animate({
                    scrollTop: $steps.eq(currentStep).offset().top - 100
                }, 500);
            } else {
                $('#validation-summary').addClass('d-none');
            }

            return isValid;
        }

        $('#System_Language').select2({
            width: '100%',
            height:'35',
            theme: 'default', 
            dropdownCssClass: 'custom-select2-dropdown' 
        });
    });
   
    $('#System_Language').on('change', function () {
        var selectedLang = $(this).val();
        if (selectedLang && selectedLang !== 'None') {
            window.location.href = '/Application/ChangeLanguage?language=' + selectedLang;
        }
    });

});
          

