 $(document).ready(function () {
    let isCompExpanded = false;

    $(".status-indicator").on("click", function (e) {
        e.stopPropagation(); 
        const $component = $(this).closest('.grc-page-component'); 
        $component.find('.header-more-list').toggleClass("open");
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
    $('.action-btn-Units').on('click', function(e) {
        e.preventDefault();
         openModal('DepartmentUnits');

        initDepartmentUnitsTable();
    });

    //..close component modal when clicking the X button
   $(document).on('click', '.component-modal-close, .componet-modal-back', function() {
        const $modal = $(this).closest('.component-modal-overlay');
        closeModal($modal.data('popup-id'));
    });
    
    //..close component modal when clicking outside the modal container
    $(document).on('click', '.component-modal-overlay', function(e) {
        if (e.target === this) {
            closeModal($(this).data('popup-id'));
        }
    });
    
    //..close modal with Escape key
    $(document).on('keydown', function(e) {
        if (e.key === 'Escape') {
            const $topModal = $('.component-modal-overlay:visible').last();
            if ($topModal.length) {
                closeModal($topModal.data('popup-id'));
            }
        }
    });

    //..activate link clicks only when row is selected
    $('#departmentUnitsTable tbody').on('click', 'a', function (e) {
        e.stopPropagation();

        const $link = $(this);
        const $row = $link.closest('tr');

        if (!$row.hasClass('row-selected')) {
            e.preventDefault(); 
            return false;
        }

        console.log('Link clicked:', $link.text(), 'Row selected');
    });

    //..prevent text selection on double click
    $('#departmentUnitsTable').on('selectstart', function() {
        return false;
    });

    /*--------------------department button events*/

    // filter buttons
     $('.action-btn-New').on("click", function (e) {
          e.preventDefault();

          // reset only inside this component
          const $component = $(this).closest('.grc-page-component'); 

          resetButtons($component);
          $(this).addClass('active');

          $component.find('.component-slideout').addClass('active');
          $component.find('.component-create-container').show();
          $component.find('.component-edit-container').hide();
     });

     $('.action-btn-Edit').on("click", function (e) {
          e.preventDefault();

          // reset only inside this component
          const $component = $(this).closest('.grc-page-component'); 

          resetButtons($component);
          $(this).addClass('active');

          $component.find('.component-slideout').addClass('active');
          $component.find('.component-create-container').hide();
          $component.find('.component-edit-container').show();
     });
     
     $('.action-btn-Delete').on("click", function (e) {

     });

     // filter buttons
    $('.filter-btn').on("click", function (e) {
        e.preventDefault();

        // reset only inside this component
        const $component = $(this).closest('.grc-page-component'); 

        resetButtons($component);

        $(this).addClass('active');
        $component.find('.component-tabletools-container').addClass('active');
    });
    
     $('.component-new-close, .component-edit-close').on("click", function (e) {
         e.preventDefault();

        // reset only inside this component
        const $component = $(this).closest('.grc-page-component'); 

        resetButtons($component);
        $component.find('.component-slideout').removeClass('active');

     });

     $('.component-filter-close').on("click", function (e) {
          e.preventDefault();

          // reset only inside this component
          const $component = $(this).closest('.grc-page-component'); 

          resetButtons($component);
          $component.find('.component-tabletools-container').removeClass('active');
     });

    //..override DataTables alerts
    $.fn.dataTable.ext.errMode = function ( settings, helpPage, message ) {
        showToast("error", message);
    };

    $(document).on('popup.opened', function (e, popupId) {
        if (popupId === 'DepartmentUnits') {
            initDepartmentUnitsTable();
        }
    });

    $(document).on('popup.closed', function (e, popupId) {
        if (popupId === 'DepartmentUnits') {
            if ($.fn.DataTable.isDataTable('#departmentUnitsTable')) {
                $('#departmentUnitsTable').DataTable().destroy(true);
                $('#departmentUnitsTable').empty();
            }
        }
    });

    function resetButtons($component) {
        $component.find('.filterBtn').removeClass('active');
        $component.find('.action-btn-New').removeClass('active');
        $component.find('.action-btn-Edit').removeClass('active');
    }

    function initDepartmentUnitsTable() {
        if ($.fn.DataTable.isDataTable('#departmentUnitsTable')) {
            $('#departmentUnitsTable').DataTable().clear().destroy();
            $('#departmentUnitsTable').empty();
        }

        var departmentsTable = $('#departmentUnitsTable').DataTable({
            searching: false,
            paging: true,
            lengthChange: false,
            info: false,
            ordering: true,
            responsive: true,
            processing: true,
            serverSide: true,
            ajax: {
                url: '/support/departments/allUnits',
                type: 'POST',
                contentType: "application/json",
                data: function (d) {
                    return JSON.stringify({
                        pageIndex: (d.start / d.length) + 1,
                        pageSize: d.length,
                        includeDeleted: false,
                        action: "LOADUNITS"
                    });
                },
                dataSrc: function (json) {
                    console.log('Ajax response:', json); 
                    return json.data || [];
                },
                error: function (xhr, status, error) {
                     showToast("error", `Failed to load department units data: ${error}`);
                    console.error('Error during Ajax request:', error);
                }
            },
            columns: [
                { data: "unitCode" },
                { data: "unitName" },
                { data: "department" },
                { data: "isDeleted" },
                { data: "creatdOn" }
            ],
            language: {
                emptyTable: "No department units data available"
            }
        });

        //..set row selection
        setupRowSelection('#departmentUnitsTable', departmentsTable);
    }

    function openModal(popupIdOrName) {
        let $popup = $(`#${popupIdOrName}`);
        if ($popup.length === 0) {
            $popup = $(`.component-modal-overlay[data-popup-name="${popupIdOrName}"]`);
        }

        if ($popup.length) {
            $popup.show();
            $(document).trigger('popup.opened', [$popup.attr('id')]);
        }
    }

    function closeModal(popupId) {
        $(`#${popupId}`).hide();
        $(document).trigger('popup.closed', [popupId]);
    }

});
