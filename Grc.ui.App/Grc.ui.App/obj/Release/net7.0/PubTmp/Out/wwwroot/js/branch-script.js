$(document).ready(function () {

    //..branch table
    if ($("#branchTable").length) {
        var departmentsTable = $('#branchTable').DataTable({
            searching: false,
            paging: true,
            lengthChange: false,
            info: false,
            ordering: true,
            responsive: true,
            processing: true,
            serverSide: true,
            ajax: {
                url: '/support/organization/branches-all',
                type: 'POST',
                contentType: "application/json",
                data: function (d) {
                    return JSON.stringify({
                        pageIndex: (d.start / d.length) + 1,
                        pageSize: d.length,
                        includeDeleted: false,
                        action: "LOAD_BRANCHES"
                    });
                },
                dataSrc: function (json) {
                    console.log('Ajax response:', json);
                    return json.data || [];
                },
                error: function (xhr, status, error) {
                    showToast("error", `Failed to load branch data: ${error}`);
                    console.error('Error during Ajax request:', error);
                }
            },
            columns: [
                { data: "branchName" },
                { data: "solId" },
                {
                    data: "isDeleted",
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
                emptyTable: "No branch data available"
            }
        });

        //..set row selection
        setupRowSelection('#branchTable', departmentsTable);
    }
});