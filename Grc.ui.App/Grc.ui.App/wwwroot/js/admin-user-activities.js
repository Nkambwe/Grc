let activitiesTable;
function initActivityTable() {
   bugsTable = new Tabulator("#activitiesTable", {
        ajaxURL: "/admin/support/users/actvities/all",
        paginationMode: "remote",
        filterMode: "remote",
        sortMode: "remote",
        pagination: true,
        paginationSize: 10,
        paginationSizeSelector: [10, 20, 35, 40, 50],
        paginationCounter: "rows",
        placeholder: "No records found",
        ajaxConfig: {
            method: "POST",
            headers: { "Content-Type": "application/json" }
        },
        ajaxContentType: "json",
        paginationDataSent: {
            "page": "page",
            "size": "size",
            "sorters": "sort",
            "filters": "filter"
        },
        paginationDataReceived: {
            "last_page": "last_page",
            "data": "data",
            "total_records": "total_records"
        },
        ajaxRequestFunc: function (url, config, params) {
            return new Promise((resolve, reject) => {
                let requestBody = {
                    pageIndex: params.page || 1,
                    pageSize: params.size || 10,
                    searchTerm: "",
                    sortBy: "",
                    sortDirection: "Ascending"
                };

                //..sorting
                if (params.sort && params.sort.length > 0) {
                    requestBody.sortBy = params.sort[0].field;
                    requestBody.sortDirection = params.sort[0].dir === "asc" ? "Ascending" : "Descending";
                }

                //..filtering
                if (params.filter && params.filter.length > 0) {
                    let filter = params.filter.find(f =>
                        ["action", "entityName", "activityType"].includes(f.field)
                    );
                    if (filter) requestBody.searchTerm = filter.value;
                }

                $.ajax({
                    url: url,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(requestBody),
                    success: function (response) {
                        resolve(response);
                    },
                    error: function (xhr, status, error) {
                        if (xhr.status === 401) {
                            window.location = "/login/userlogin";
                        }

                        if (xhr.status === 403) {
                            Swal.fire({
                                title: "Access Denied!",
                                text: "You do not have permission to access this resource."
                            });

                            //..return empty dataset
                            resolve({
                                data: [],
                                last_page: 1,
                                total_records: 0
                            });

                            return;
                        }

                        reject(error);
                        return;
                    }
                });
            });
        },
        ajaxResponse: function (url, params, response) {
            if(response?.status === 403 || response?.hasPermission === false){

                this.clearData();
                this.setPlaceholder("You do not have permission to view these records.");

                return {
                    data: [],
                    last_page: 1
                };
            }

            return response;
        },
        ajaxError: function (error) {
            console.error("Tabulator AJAX Error:", error);
             Swal.fire({
                title: "System Error!",
                text: "Failed to load system roles. Please try again."
            });
        },
        layout: "fitColumns",
        responsiveLayout: "hide",
        columns: [
            {
                 formatter: () => `<span class="record-tab"></span>`,
                 width: 40,
                 headerSort: false,
                 frozen: true
             },
             {
                 title: "USER",
                 field: "accessedBy",
                 headerFilter: "input",
                 width: 200
             },
             {
                 title: "IP ADDRESS",
                 field: "ipAddress",
                 headerFilter: "input",
                 width: 200,
             },
             {
                 title: "ACTIVITY",
                 field: "action",
                 headerFilter: "input",
                 minWidth: 250,
                 widthGrow: 4
             },
             {
                 title: "ENTITY",
                 field: "entityName",
                 minWidth: 200, 
                 headerSort: true, 
                 headerFilter: "input" 
             },
             {
                 title: "ENTITY TYPE",
                 field: "activityType",
                 minWidth: 200, 
                 headerSort: true, 
                 headerFilter: "input"
             },
             {
                 title: "ACCESSED ON",
                 field: "activityDate",
                 headerFilter: "input",
                 minWidth: 200,
                 formatter: function (cell) {
                    const value = cell.getValue();
                    if (!value) return "";
                    const d = new Date(value);
                    const day = String(d.getDate()).padStart(2, "0");
                    const month = String(d.getMonth() + 1).padStart(2, "0");
                    const year = d.getFullYear();

                    return `${day}-${month}-${year}`;
                 }
             },
             { title: "", field: "endTab", maxWidth: 50, headerSort: false, formatter: () => `<span class="record-tab"></span>` }
        ]
    });
}

function exportActivities() {
    $.ajax({
        url: '/admin/support/users/activities/export-list',
        type: 'POST',
        xhrFields: { responseType: 'blob' },
        success: function (blob) {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = "User_Activities.xlsx";
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        },
        error: function () {
            toastr.error("Export failed. Please try again.");
        }
    });
}


$(document).ready(function () {
     initActivityTable();    
     
     $(".action-btn-excel-export").on("click", function () {
        exportActivities();
    });

    $(".action-btn-admin-home").on("click", function () {
        window.location.href = '/admin/support';
    });
});