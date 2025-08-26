$(document).ready(function () {

    $('#tblActivity').DataTable({
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
        
        //..custom language settings
        language: {
            emptyTable: "No activity data available"
        }
    });
});