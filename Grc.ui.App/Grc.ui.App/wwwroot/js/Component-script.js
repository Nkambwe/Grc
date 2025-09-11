 $(document).ready(function () {
    let isCompExpanded = false;

    $("#moreActionsBtn1, #moreActionsBtn2").on("click", function (e) {
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
    $('#btnActionUnits1').on('click', function(e) {
        e.preventDefault();
         openModal('DepartmentUnits');

        initDepartmentUnitsTable();
    });

    //..close component modal when clicking the X button
    $('.component-modal-close,.componet-modal-back').on('click', function() {
        closeModal();
    });
    
    //..close component modal when clicking outside the modal container
    $('#componentModal').on('click', function(e) {
        if (e.target === this) {
            closeModal();
        }
    });
    
    //..close modal with Escape key
    $(document).on('keydown', function(e) {
        if (e.key === 'Escape') {
            closeModal();
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
     $('#btnActionNew1').on("click", function (e) {
          resetButtons();
          $(this).addClass('active');

          $('.component-slideout').addClass('active');
          $('.component-create-container').show();
          $('.component-edit-container').hide();
     });

     $('#btnActionEdit1').on("click", function (e) {
          resetButtons();
          $(this).addClass('active');

          $('.component-slideout').addClass('active');
          $('.component-edit-container').show();
          $('.component-create-container').hide();
     });
     
     $('#btnActionDelete1').on("click", function (e) {
     closeSlideout()
     });
     
     $('#filterBtn1').on("click", function (e) {
     
     });
     /*--------------------department unit button events*/
     $('#btnActionNew2').on("click", function (e) {
          resetButtons();
          $(this).addClass('active');

          $('.component-slideout').addClass('active');
          $('.component-create-container').show();
          $('.component-edit-container').hide();
     });
     
     $('#btnActionEdit2').on("click", function (e) {
          resetButtons();
          $(this).addClass('active');

          $('.component-slideout').addClass('active');
          $('.component-edit-container').show();
          $('.component-create-container').hide();
     });
     
     $('#btnActionDelete2').on("click", function (e) {
     
     });
     
     $('#filterBtn2').on("click", function (e) {
          resetButtons();
          $(this).addClass('active');
          $('.component-tabletools-container').addClass('active');
    });
    
     $('.component-new-close, .component-edit-close').on("click", function (e) {
          $('.component-slideout').removeClass('active');
          resetButtons();
     });

     $('.component-filter-close').on("click", function (e) {
         $('.component-tabletools-container').removeClass('active');
         resetButtons();
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

    function resetButtons() {
        $('.action-btn, .icon-btn').removeClass('active');
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

    function openModal(popupId) {
          $('#componentModal').show();
        $(document).trigger('popup.opened', [popupId]);
    }

    function closeModal() {
        $('#componentModal').hide();
        $(document).trigger('popup.closed', [popupId]);
    }

});
