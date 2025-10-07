$(document).ready(function () {

    //..branch table
    if ($("#usersTable").length) {
        var departmentsTable = $('#usersTable').DataTable({
            searching: false,
            paging: true,
            lengthChange: false,
            info: false,
            ordering: true,
            responsive: true,
            processing: true,
            serverSide: true,
            ajax: {
                url: '/admin/support/users-all',
                type: 'POST',
                contentType: "application/json",
                data: function (d) {
                    return JSON.stringify({
                        pageIndex: (d.start / d.length) + 1,
                        pageSize: d.length,
                        includeDeleted: false,
                        action: "LOAD_USERS"
                    });
                },
                dataSrc: function (json) {
                    console.log('Ajax response:', json);
                    return json.data || [];
                },
                error: function (xhr, status, error) {
                    showToast("error", `Failed to load user data: ${error}`);
                    console.error('Error during Ajax request:', error);
                }
            },
            columns: [
                { data: "displayName" },
                { data: "userName" },
                { data: "emailAddress" },
                { data: "roleName" },
                { data: "roleGroup" },
                { data: "pfNumber" },
                {
                    data: "isVerified",
                    render: function (data, type, row) {
                        return data ? "YES" : "NO";
                    }
                },
                {
                    data: "isActive",
                    render: function (data, type, row) {
                        return data ? "YES" : "NO";
                    }
                },
                {
                    data: "createdOn",
                    render: function (data) {
                        if (!data) return "";
                        const date = new Date(data);
                        return date.toLocaleDateString(); 
                    }
                }
            ],

            language: {
                emptyTable: "No users data available"
            }
        });

        //..set row selection
        setupRowSelection('#usersTable', departmentsTable);
    }
});