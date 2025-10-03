$(document).ready(function () {

    //..departments table
    if ($("#departmentsTable").length) {
        var departmentsTable = $('#departmentsTable').DataTable({
            searching: false,
            paging: true,
            lengthChange: false,
            info: false,
            ordering: true,
            responsive: true,
            processing: true,
            serverSide: true,
            ajax: {
                url: '/support/departments/allDepartments',
                type: 'POST',
                contentType: "application/json",
                data: function (d) {
                    return JSON.stringify({
                        pageIndex: (d.start / d.length) + 1,
                        pageSize: d.length,
                        includeDeleted: false,
                        action: "LOADDEPARTMENTS"
                    });
                },
                dataSrc: function (json) {
                    console.log('Ajax response:', json);
                    return json.data || [];
                },
                error: function (xhr, status, error) {
                    showToast("error", `Failed to load department data: ${error}`);
                    console.error('Error during Ajax request:', error);
                }
            },
            columns: [
                { data: "departmentCode" },
                { data: "departmentName" },
                { data: "departmentAlias" },
                { data: "isDeleted" },
                { data: "branch" },
                { data: "creatdOn" }
            ],
            language: {
                emptyTable: "No department data available"
            }
        });

        //..set row selection
        setupRowSelection('#departmentsTable', departmentsTable);
    }
});