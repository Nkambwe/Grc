$(document).ready(function () {

    var table = $('#tblActivity').DataTable({
        //..disable search functionality
        searching: false,
        
        //..show pagination
        paging: true,
        
        //..remove entries per page dropdown
        lengthChange: false,
        
        //..remove display info showing x to y of z entries
        info: false,
        
        //..allow ordering/sorting
        ordering: true,
        
        //..make it responsive
        responsive: true,

        processing: true,

        //..using server paging
        serverSide: true,
        ajax: {
            url: '/support/activities/allActivities',
            type: 'POST',
            contentType: "application/json",
            data: function (d) {
                return JSON.stringify({
                    pageIndex: (d.start / d.length) + 1,
                    pageSize: d.length,
                    includeDeleted: false,
                    action: "LOAD_ACTIVITIES"
                });
            },
            dataSrc: function (json) {
                console.log('Ajax response:', json); 
                return json.data || [];
            },
            error: function (xhr, status, error) {
                console.error('Error during Ajax request:', error);
            }
        },

        columns: [
            { 
                data: null, 
                render: function (d, t, r) {
                    return `<a href="/Admin/Support/ActivityRecord/${r.userId}">${r.userFirstName} ${r.userLastName}</a>`;
                }
            }, 
            { 
                data: "userEmail", 
                render: function (d, t, r) {
                    return `<a href="mailto:${r.userEmail}">${r.userEmail}</a>`;
                }
            },
            { data: "entityName" },
            { data: "period" },
            { data: "comment" }
        ],

        
        //..custom language settings
        language: {
            emptyTable: "No activity data available"
        }
    });
    
    //..activate link clicks only when row is selected
    $('#tblActivity tbody').on('click', 'a', function (e) {
        var $link = $(this);
        var $row = $link.closest('tr');
        
        //..prevent events from propergating to the row handler
        e.stopPropagation();

        //..row is not selected
        if (!$row.hasClass('row-selected')) {
            e.preventDefault();
            return false;
        }
        
        //..log selected row
        console.log('Link clicked:', $link.text(), 'Row selected:', $row.hasClass('row-selected'));
    });

    //..select table row
    $('#tblActivity tbody').on('click', 'tr', function (e) {
        //..don't select row if clicking on a link in a selected row
        if ($(e.target).is('a') && $(this).hasClass('row-selected')) {
            return;
        }

        var $row = $(this);

        if ($row.hasClass('row-selected')) {
            $row.removeClass('row-selected');
            //..disable links in this row
            $row.find('a').removeClass('active').attr('tabindex', '-1');
        } else {
            //..remove selection from all other rows
            table.$('tr.row-selected').removeClass('row-selected');
            table.$('tr a').removeClass('active').attr('tabindex', '-1');
            
            //..select current row
            $row.addClass('row-selected');
            //..enable links in selected row
            $row.find('a').addClass('active').removeAttr('tabindex');
        }
    });

    //..prevent text selection on double click
    $('#tblActivity').on('selectstart', function() {
        return false;
    });
});