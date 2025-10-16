$(document).ready(function () {
    
    //..activity table
    if ($("#tblActivity").length) {
        var activityTable = $('#tblActivity').DataTable({
            searching: false,
            paging: true,
            lengthChange: false,
            info: false,
            ordering: true,
            responsive: true,
            processing: true,
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
                        action: "LOADACTIVITIES"
                    });
                },
                dataSrc: function (json) {
                    console.log('Ajax response:', json); 
                    return json.data || [];
                },
                error: function (xhr, status, error) {
                    showToast("error", `Failed to load activity data: ${error}`);
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
            language: {
                emptyTable: "No activity data available"
            }
        });

        //..set row selection
        setupRowSelection('#tblActivity', activityTable);
    }

    //..activate link clicks only when row is selected
    $('#tblActivity tbody, #departmentsTable tbody').on('click', 'a', function (e) {
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
    $('#tblActivity, #departmentsTable').on('selectstart', function() {
        return false;
    });

    //..override DataTables alerts
    $.fn.dataTable.ext.errMode = function ( settings, helpPage, message ) {
        showToast("error", message);
    };
});

function setupRowSelection(tableSelector, dataTableInstance) {
    $(`${tableSelector} tbody`).on('click', 'tr', function (e) {
        //..don't select row if clicking on a link in a selected row
        if ($(e.target).is('a') && $(this).hasClass('row-selected')) {
            return;
        }

        var $row = $(this);

        if ($row.hasClass('row-selected')) {
            //..unselect
            $row.removeClass('row-selected');
            $row.find('a').removeClass('active').attr('tabindex', '-1');
            $row.find('.row-selection-icon').remove();
        } else {
            //..clear other selections in this table
            dataTableInstance.$('tr.row-selected').removeClass('row-selected');
            dataTableInstance.$('tr a').removeClass('active').attr('tabindex', '-1');
            dataTableInstance.$('tr .row-selection-icon').remove();

            //..select this row
            $row.addClass('row-selected');
            $row.find('a').addClass('active').removeAttr('tabindex');
            $row.find('td:first').prepend('<i class="mdi mdi-arrow-right row-selection-icon"></i>');
        }
    });
}

function showToast(type, message) {
    const toast = $(`
        <div class="toast-notification toast-${type}">
            <i class="mdi ${type === 'success' ? 'mdi-check-circle' : 'mdi-alert-circle'}"></i>
            <span>${message}</span>
        </div>
    `);

    $("body").append(toast);

    // animate in
    setTimeout(() => {
        toast.addClass("show");
    }, 50);

    // animate out & remove
    setTimeout(() => {
        toast.removeClass("show");
        setTimeout(() => toast.remove(), 300);
    }, 4000);
}

