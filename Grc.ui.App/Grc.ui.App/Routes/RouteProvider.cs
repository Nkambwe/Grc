namespace Grc.ui.App.Routes {
    public class RouteProvider : IRouteProvider {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder) {
            
            routeBuilder.MapControllerRoute(
                name: "admin-departments",
                pattern: "admin/support/departments",
                defaults: new { area = "Admin", controller = "Support", action = "Departments" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-activeusers",
                pattern: "admin/support/users-active",
                defaults: new { area = "Admin", controller = "Support", action = "ActiveUsers" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-lockedusers",
                pattern: "admin/support/users-locked",
                defaults: new { area = "Admin", controller = "Support", action = "LockedUsers" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-unapprovedUsers",
                pattern: "admin/support/users-unapproved",
                defaults: new { area = "Admin", controller = "Support", action = "UnapprovedUsers" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-unverifiedUser",
                pattern: "admin/support/users-unverified",
                defaults: new { area = "Admin", controller = "Support", action = "UnverifiedUser" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-deletedUsers",
                pattern: "admin/support/users-deactivated",
                defaults: new { area = "Admin", controller = "Support", action = "DeletedUsers" }
            );

            /*----------------------- System activity routes*/
            routeBuilder.MapControllerRoute(
                name: "admin-system-activities-retrieve",
                pattern: "admin/support/system-activities-retrieve/{id:long}",
                defaults: new { area = "Admin", controller = "Support", action = "GetSystemActivity" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-system-activities-list",
                pattern: "admin/support/system-activities/list",
                defaults: new { area = "Admin", controller = "Support", action = "GetPagedSystemActivities" }
            );

            /*----------------------- Users routes*/
            routeBuilder.MapControllerRoute(
                name: "admin-users",
                pattern: "admin/support/system-users",
                defaults: new { area = "Admin", controller = "Support", action = "Users" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-users-list",
                pattern: "admin/support/system-users/list",
                defaults: new { area = "Admin", controller = "Support", action = "GetPagedUsers" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-users-retrieve",
                pattern: "admin/support/users-retrieve/{id:long}",
                defaults: new { area = "Admin", controller = "Support", action = "GetUser" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-users-all",
                pattern: "admin/support/users-all",
                defaults: new { area = "Admin", controller = "Support", action = "GetUsers" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-users-create",
                pattern: "admin/support/users-create",
                defaults: new { area = "Admin", controller = "Support", action = "CreateUser" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-users-modify",
                pattern: "admin/support/users-modify",
                defaults: new { area = "Admin", controller = "Support", action = "ModifyUser" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-users-approve",
                pattern: "admin/support/users-approve",
                defaults: new { area = "Admin", controller = "Support", action = "ApproveUser" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-users-delete",
                pattern: "admin/support/users-delete/{id:long}",
                defaults: new { area = "Admin", controller = "Support", action = "DeleteUser" }
            );

            /*----------------------- Role routes*/
            routeBuilder.MapControllerRoute(
                name: "admin-roles",
                pattern: "admin/support/system-roles",
                defaults: new { area = "Admin", controller = "Support", action = "Roles" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-roles-list",
                pattern: "admin/support/system-roles/list",
                defaults: new { area = "Admin", controller = "Support", action = "GetPagedRoles" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-roles-retrieve",
                pattern: "admin/support/roles-retrieve/{id:long}",
                defaults: new { area = "Admin", controller = "Support", action = "GetRole" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-roles-all",
                pattern: "admin/support/roles-all",
                defaults: new { area = "Admin", controller = "Support", action = "GetRoles" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-roles-create",
                pattern: "admin/support/roles-create",
                defaults: new { area = "Admin", controller = "Support", action = "CreateRole" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-roles-modify",
                pattern: "admin/support/roles-modify",
                defaults: new { area = "Admin", controller = "Support", action = "UpdateRole" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-roles-approve",
                pattern: "admin/support/roles-approve",
                defaults: new { area = "Admin", controller = "Support", action = "ApproveRole" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-roles-delete",
                pattern: "admin/support/roles-delete/{id:long}",
                defaults: new { area = "Admin", controller = "Support", action = "DeleteRole" }
            );

            /*----------------------- Role Permissions routes*/
            routeBuilder.MapControllerRoute(
                name: "admin-role-permissions",
                pattern: "admin/support/system-roles-permissions",
                defaults: new { area = "Admin", controller = "Support", action = "RolePermissions" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-role-permissions-retrieve",
               pattern: "admin/support/role-permissions-retrieve/{id:long}",
               defaults: new { area = "Admin", controller = "Support", action = "GetRoleWithPermissions" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-role-permissionsets-retrieve",
               pattern: "admin/support/role-permission-sets-retrieve/{id:long}",
               defaults: new { area = "Admin", controller = "Support", action = "GetRoleWithPermissionSets" }
           );

            /*----------------------- Role Group routes*/
            routeBuilder.MapControllerRoute(
               name: "admin-role-groups",
               pattern: "admin/support/system-roles-groups",
               defaults: new { area = "Admin", controller = "Support", action = "RoleGroups" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-role-groups-retrieve",
               pattern: "admin/support/role-groups-retrieve/{id:long}",
               defaults: new { area = "Admin", controller = "Support", action = "GetRoleGroupWithRoles" }
            );
            routeBuilder.MapControllerRoute(
              name: "admin-role-group-sets",
              pattern: "admin/support/system-roles-group-sets",
              defaults: new { area = "Admin", controller = "Support", action = "RoleGroupPermissions" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-groups-all",
                pattern: "admin/support/role-groups-all",
                defaults: new { area = "Admin", controller = "Support", action = "GetRoleGroups" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-groups-droplist",
                pattern: "admin/support/role-groups-droplist",
                defaults: new { area = "Admin", controller = "Support", action = "GetRoleGroupLists" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-role-groups-list",
               pattern: "admin/support/system-role-groups/list",
               defaults: new { area = "Admin", controller = "Support", action = "GetPagedRoleGroups" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-role-group-sets-list",
               pattern: "admin/support/system-role-group-sets/list",
               defaults: new { area = "Admin", controller = "Support", action = "GetPagedRoleGroups" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-groups-create",
                pattern: "admin/support/role-groups-create",
                defaults: new { area = "Admin", controller = "Support", action = "CreateRoleGroup" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-groups-modify",
                pattern: "admin/support/role-groups-modify",
                defaults: new { area = "Admin", controller = "Support", action = "UpdateRoleGroup" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-groups-approve",
                pattern: "admin/support/role-groups-approve",
                defaults: new { area = "Admin", controller = "Support", action = "ApproveRoleGroup" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-groups-delete",
                pattern: "admin/support/role-groups-delete/{id:long}",
                defaults: new { area = "Admin", controller = "Support", action = "DeleteRoleGroup" }
            );

            /*----------------------- System Permission routes*/
            routeBuilder.MapControllerRoute(
              name: "admin-support-permissions",
              pattern: "admin/support/set-permissions-all",
              defaults: new { area = "Admin", controller = "Support", action = "GetPermissions" }
            );

            /*----------------------- Group Permission routes*/
            routeBuilder.MapControllerRoute(
              name: "admin-support-permission-role-groups",
              pattern: "admin/support/permission/role-groups",
              defaults: new { area = "Admin", controller = "Support", action = "RoleGroup" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-role-group-permissions-retrieve",
               pattern: "admin/support/role-group-permissions-retrieve/{id:long}",
               defaults: new { area = "Admin", controller = "Support", action = "GetRoleGroupWithPermissions" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-group-sets-permissions-all",
                pattern: "admin/support/role-groups-permissions-all",
                defaults: new { area = "Admin", controller = "Support", action = "GetPagedRoleGroupWithPermissionSets" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-group-permissions-create",
                pattern: "admin/support/role-group-permissions-create",
                defaults: new { area = "Admin", controller = "Support", action = "CreateRoleGroupPermissions" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-role-group-permissions-modify",
                pattern: "admin/support/role-group-permissions-modify",
                defaults: new { area = "Admin", controller = "Support", action = "UpdateRoleGroupPermissions" }
            );

            /*----------------------- Permission Sets routes*/
            routeBuilder.MapControllerRoute(
                 name: "admin-support-permission-sets",
                 pattern: "admin/support/permission/sets",
                 defaults: new { area = "Admin", controller = "Support", action = "PermissionSets" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-support-permission-sets-retrieve",
               pattern: "admin/support/permission-sets-retrieve/{id:long}",
               defaults: new { area = "Admin", controller = "Support", action = "GetPermissionSet" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-permission-sets-all",
                pattern: "admin/support/permission-sets-all",
                defaults: new { area = "Admin", controller = "Support", action = "GetPermissionSets" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-support-permission-sets-list",
               pattern: "admin/support/permission-sets/list",
               defaults: new { area = "Admin", controller = "Support", action = "GetPagedPermissionSets" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-permission-sets-create",
                pattern: "admin/support/permission-sets-create",
                defaults: new { area = "Admin", controller = "Support", action = "CreatePermissionSet" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-permission-sets-modify",
                pattern: "admin/support/permission-sets-modify",
                defaults: new { area = "Admin", controller = "Support", action = "UpdatePermissionSet" }
            );
            routeBuilder.MapControllerRoute(
                name: "admin-support-permission-sets-delete",
                pattern: "admin/support/permission-sets-delete/{id:long}",
                defaults: new { area = "Admin", controller = "Support", action = "DeletePermissionSet" }
            );

            /*----------------------- Role Delegation routes*/
            routeBuilder.MapControllerRoute(
               name: "admin-rolesDelegations",
               pattern: "admin/support/system-roles-delegations",
               defaults: new { area = "Admin", controller = "Support", action = "RoleDelegation" }
            );

            routeBuilder.MapControllerRoute(
               name: "admin-support-branches",
               pattern: "support/organization/branches-all",
               defaults: new { area = "Admin", controller = "Support", action = "GetBranches" }
            );

            routeBuilder.MapControllerRoute(
                name: "admin-settings",
                pattern: "admin/settings",
                defaults: new { area = "Admin", controller = "Settings", action = "Index" }
            );

            /*----------------------- Admin configuration*/
            routeBuilder.MapControllerRoute(
                name: "admin-configuration",
                pattern: "admin/configuration",
                defaults: new { area = "Admin", controller = "Configuration", action = "Index" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-configuration-organization",
               pattern: "admin/configuration/organization",
               defaults: new { area = "Admin", controller = "Configuration", action = "Organization" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-configuration-branches",
               pattern: "admin/configuration/branches",
               defaults: new { area = "Admin", controller = "Configuration", action = "Branches" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-configuration-users",
               pattern: "admin/configuration/users",
               defaults: new { area = "Admin", controller = "Configuration", action = "UserData" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-configuration-users-groups",
               pattern: "admin/configuration/users/groups",
               defaults: new { area = "Admin", controller = "Configuration", action = "UserGroups" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-configuration-cypher",
               pattern: "admin/configuration/cypher",
               defaults: new { area = "Admin", controller = "Configuration", action = "DataEncryptions" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-configuration-bugs",
               pattern: "admin/configuration/bugs",
               defaults: new { area = "Admin", controller = "Configuration", action = "BugReporter" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-configuration-activities",
               pattern: "admin/configuration/activities",
               defaults: new { area = "Admin", controller = "Configuration", action = "SystemActivity" }
            );

            /*----------------------- Admin Support*/
            routeBuilder.MapControllerRoute(
                name: "admin-support",
                pattern: "admin/support",
                defaults: new { area = "Admin", controller = "Support", action = "Index" }
            );
            routeBuilder.MapControllerRoute(
               name: "admin-support-departments",
               pattern: "admin/support/departments",
               defaults: new { area = "Admin", controller = "Support", action = "Departments" }
            );
            routeBuilder.MapControllerRoute(
              name: "admin-support-passwords",
              pattern: "admin/support/passwords",
              defaults: new { area = "Admin", controller = "Support", action = "PasswordPolicy" }
            );
            /*----------------------- Operations dashboard routes*/
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard",
                pattern: "/operations/dashboard",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Index" }
            );

            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-totalprocs",
                pattern: "/operations/dashboard/totalprocs",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "TotalProcesses" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-proposed",
                pattern: "/operations/dashboard/proposed",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Proposed" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-completed",
                pattern: "/operations/dashboard/completed",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Completed" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-dormant",
                pattern: "/operations/dashboard/dormant",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Dormant" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-cancelled",
                pattern: "/operations/dashboard/cancelled",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Cancelled" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-unchanged",
                pattern: "/operations/dashboard/unchanged",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Unchanged" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-due",
                pattern: "/operations/dashboard/due",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Due" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-account-services",
                pattern: "/operations/dashboard/account-services",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "AccountServices" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-reconciliation",
                pattern: "/operations/dashboard/reconciliation",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "RecordsManagement" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-wallets",
                pattern: "/operations/dashboard/wallets",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Wallets" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-cash",
                pattern: "/operations/dashboard/cash",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Cash" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-payments",
                pattern: "/operations/dashboard/payments",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Payments" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-channels",
                pattern: "/operations/dashboard/channels",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Channels" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-customerExperience",
                pattern: "/operations/dashboard/customer-experience",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "CustomerExperience" }
            );

            /*----------------------- Operations processes routes*/
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-register",
               pattern: "/operations/workflow/processes/registers",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "RegisterProcess" }
            );
            routeBuilder.MapControllerRoute(
              name: "app-operations-processes-register-all",
              pattern: "/operations/workflow/processes/register/all",
              defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ProcessRegisterList" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-register-retrieve",
               pattern: "/operations/workflow/processes/registers/retrieve/{id:long}",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "GetProcessRegister" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-register-create",
                pattern: "/operations/workflow/processes/registers/retrieve/create",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "CreateProcessRegister" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-register-update",
                pattern: "/operations/workflow/processes/registers/retrieve/update",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "UpdateProcessRegister" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-files-upload",
                pattern: "/operations/workflow/processes/registers/upload-files",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "UploadProcessFiles" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-register-delete",
                pattern: "/operations/workflow/processes/registers/delete/{id:long}",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "DeleteProcessRegister" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-register-export-register-all",
                pattern: "/operations/workflow/processes/registers/retrieve/export-all",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ExportProcessRegisterAll" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-register-initiate-review",
                pattern: "/operations/workflow/processes/registers/initiate-review",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "InitiateReview" }
            );

            /*----------------------- Operations process groups routes*/
            routeBuilder.MapControllerRoute(
              name: "app-operations-processes-groups",
              pattern: "/operations/workflow/processes/groups",
              defaults: new { area = "Operations", controller = "OperationWorkflow", action = "GroupProcesses" }
            );
            routeBuilder.MapControllerRoute(
              name: "app-operations-processes-groups-all",
              pattern: "/operations/workflow/processes/groups/all",
              defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ProcessGroupList" }
            );
            routeBuilder.MapControllerRoute(
              name: "app-operations-processes-groups-processes-min",
              pattern: "/operations/workflow/processes/groups/processes-min",
              defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ProcessMinProcessesList" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-groups-processes-retrieve",
               pattern: "/operations/workflow/processes/group-processes/retrieve/{id:long}",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "GetGroupProcesses" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-groups-retrieve",
               pattern: "/operations/workflow/processes/groups/retrieve/{id:long}",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "GetProcessGroup" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-groups-create",
                pattern: "/operations/workflow/processes/groups/create",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "CreateProcessGroup" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-groups-update",
                pattern: "/operations/workflow/processes/groups/update",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "UpdateProcessGroup" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-groups-delete",
                pattern: "/operations/workflow/processes/groups/delete/{id:long}",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "DeleteProcessGroup" }
            );

            /*----------------------- Operations process tags routes*/
            routeBuilder.MapControllerRoute(
              name: "app-operations-processes-tags",
              pattern: "/operations/workflow/processes/tags",
              defaults: new { area = "Operations", controller = "OperationWorkflow", action = "TagProcesses" }
            );
            routeBuilder.MapControllerRoute(
              name: "app-operations-processes-tags-all",
              pattern: "/operations/workflow/processes/tags/all",
              defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ProcessTagList" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-tags-retrieve",
               pattern: "/operations/workflow/processes/tags/retrieve/{id:long}",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "GetProcessTag" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-tags-create",
                pattern: "/operations/workflow/processes/tags/create",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "CreateProcessTag" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-tags-update",
                pattern: "/operations/workflow/processes/tags/update",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "UpdateProcessTag" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-tags-delete",
                pattern: "/operations/workflow/processes/tags/delete/{id:long}",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "DeleteProcessTag" }
            );
            /*----------------------- Operations process tat routes*/
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-reports",
               pattern: "/operations/workflow/processes/reports",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "TATReport" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-tat-all",
               pattern: "/operations/workflow/processes/tat/all",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ProcessTATList" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-tat-retrieve",
               pattern: "/operations/workflow/processes/tat/retrieve/{id:long}",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "GetProcessTat" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-tat-report",
               pattern: "/operations/workflow/processes/tat/report",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "GetExportTATReport" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-new",
                pattern: "/operations/workflow/processes/all-new",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "NewProcess" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-new-list",
                pattern: "/operations/workflow/processes/new-list",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ProcessNewList" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-review-list",
                pattern: "/operations/workflow/processes/review-list",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ProcessReviewList" }
            );
            routeBuilder.MapControllerRoute(
                 name: "app-operations-processes-revisions",
                 pattern: "/operations/workflow/processes/revisions",
                 defaults: new { area = "Operations", controller = "OperationWorkflow", action = "Revisions" }
            );

            /*----------------------- Process Approvals routes*/
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-approvals",
                pattern: "/operations/workflow/processes/approvals",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "Approvals" }
            );
            routeBuilder.MapControllerRoute(
                 name: "app-operations-processes-approvals-all",
                 pattern: "/operations/workflow/processes/approvals-all",
                 defaults: new { area = "Operations", controller = "OperationWorkflow", action = "ProcessApprovalList" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-approvals-retrieve",
                pattern: "/operations/workflow/processes/approvals-retrieve/{id:long}",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "GetApproval" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-approval-update",
               pattern: "/operations/workflow/processes/approval-update",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "UpdateApproval" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-operations-processes-approval-hold",
               pattern: "/operations/workflow/processes/approval-hold",
               defaults: new { area = "Operations", controller = "OperationWorkflow", action = "HoldApproval" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-approval-request",
                pattern: "/operations/workflow/processes/approval/request/{id:long}",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "RequestApproval" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-operations-processes-approval-new-request",
                pattern: "/operations/workflow/processes/approval/new-request/{id:long}",
                defaults: new { area = "Operations", controller = "OperationWorkflow", action = "NewProcessApproval" }
            );
            /*----------------------- Compliance dashboard routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-dashboard",
                pattern: "/grc/compliance",
                defaults: new { controller = "Application", action = "Dashboard" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-received",
                pattern: "/grc/regulations/received",
                defaults: new { controller = "Regulations", action = "ReceivedRegulations" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-open",
                pattern: "/grc/regulations/open",
                defaults: new { controller = "Regulations", action = "OpenRegulations" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-closed",
                pattern: "/grc/regulations/closed",
                defaults: new { controller = "Regulations", action = "ClosedRegulations" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-applicable",
                pattern: "/grc/regulations/applicable",
                defaults: new { controller = "Regulations", action = "RegulatoryApplicable" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-gaps",
                pattern: "/grc/regulations/gaps",
                defaults: new { controller = "Regulations", action = "RegulatoryGaps" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-covered",
                pattern: "/grc/regulations/covered",
                defaults: new { controller = "Regulations", action = "RegulatoryCovered" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-issues",
                pattern: "/grc/regulations/issues",
                defaults: new { controller = "Regulations", action = "RegulatoryIssues" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-na",
                pattern: "/grc/regulations/not-applicable",
                defaults: new { controller = "Regulations", action = "RegulatoryNotApplicable" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-returns",
               pattern: "/grc/register/register-returns",
               defaults: new { controller = "Register", action = "RegulationReturns" }
            );

            /*----------------------------------------------Obligation Routes*/
           
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-obligations",
               pattern: "/grc/register/obligations",
               defaults: new { controller = "Register", action = "RegulationObligations" }
            );

            routeBuilder.MapControllerRoute(
              name: "app-compliance-register-obligations-list",
              pattern: "/grc/register/obligations/paged-list",
              defaults: new { controller = "Register", action = "GetObligationList" }
            );

            /*----------------------------------------------Law routes*/
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-law-list",
               pattern: "/grc/register/register-law-list",
               defaults: new { controller = "Register", action = "GetLawList" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-laws-retrieve",
               pattern: "/grc/compliance/register/laws-retrieve/{id:long}",
               defaults: new { controller = "Register", action = "GetLaw" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-laws-create",
                pattern: "/grc/compliance/register/laws-create",
                defaults: new { controller = "Register", action = "CreateLaw" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-laws-update",
                pattern: "/grc/compliance/register/laws-update",
                defaults: new { controller = "Register", action = "UpdateLaw" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-laws-delete",
                pattern: "/grc/compliance/register/laws-delete/{id:long}",
                defaults: new { controller = "Register", action = "DeleteLaw" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-laws-list",
                pattern: "/grc/compliance/register/laws-list",
                defaults: new { controller = "Register", action = "GetRegulatoryLaws" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-laws-all",
                pattern: "/grc/compliance/register/laws-all",
                defaults: new { controller = "Register", action = "GetPagedRegulatoryLaws" }
            );
            /*----------------------------------------------Acts routes*/
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-list",
               pattern: "/grc/register/register-list",
               defaults: new { controller = "Register", action = "RegulationList" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-acts-list",
                pattern: "/grc/compliance/register/acts-list",
                defaults: new { controller = "Register", action = "GetAllRegulatoryActs" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-acts-all",
                pattern: "/grc/compliance/register/acts-all",
                defaults: new { controller = "Register", action = "GetRegulatoryActs" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-acts-retrieve",
               pattern: "/grc/compliance/register/acts-retrieve/{id:long}",
               defaults: new { controller = "Register", action = "GetRegulatoryAct" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-acts-create",
                pattern: "/grc/compliance/register/acts-create",
                defaults: new { controller = "Register", action = "CreateRegulatoryAct" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-acts-update",
                pattern: "/grc/compliance/register/acts-update",
                defaults: new { controller = "Register", action = "UpdateRegulatoryAct" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-acts-delete",
                pattern: "/grc/compliance/register/acts-delete/{id:long}",
                defaults: new { controller = "Register", action = "DeleteRegulatory" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-acts-export",
                pattern: "/grc/compliance/register/acts-export",
                defaults: new { controller = "Register", action = "ExcelExportRegulatory" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-acts-full",
                pattern: "/grc/compliance/register/acts-export-full",
                defaults: new { controller = "Register", action = "ExcelExportAllRegulatory" }
            );

            /*----------------------------------------------regulatory type routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-regulatory-types",
                pattern: "/grc/compliance/settings/regulatory-types",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceRegulatoryTypes" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-types-retrieve",
                pattern: "/grc/compliance/settings/types-retrieve/{id:long}",
                defaults: new { controller = "ComplianceSettings", action = "GetRegulatoryType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-types-all",
                pattern: "/grc/compliance/settings/types-all",
                defaults: new { controller = "ComplianceSettings", action = "AllRegulatoryTypes" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-types-create",
                pattern: "/grc/compliance/settings/types-create",
                defaults: new { controller = "ComplianceSettings", action = "CreateRegulatoryType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-types-update",
                pattern: "/grc/compliance/settings/types-update",
                defaults: new { controller = "ComplianceSettings", action = "UpdateRegulatoryType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-types-delete",
                pattern: "/grc/compliance/settings/types-delete/{id:long}",
                defaults: new { controller = "ComplianceSettings", action = "DeleteRegulatoryType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-types-export",
                pattern: "/grc/compliance/settings/types-export",
                defaults: new { controller = "ComplianceSettings", action = "ExcelExportTypes" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-types-export-full",
                pattern: "/grc/compliance/settings/types-export-full",
                defaults: new { controller = "ComplianceSettings", action = "ExcelExportAllTypes" }
            );

            /*----------------------------------------------regulatory authorities routes*/
           
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-regulatory-authorities",
                pattern: "/grc/compliance/settings/regulatory-authorities",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceAuthorities" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-authorities-retrieve",
                pattern: "/grc/compliance/settings/authorities-retrieve/{id:long}",
                defaults: new { controller = "ComplianceSettings", action = "GetRegulatoryAuthority" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-authorities-all",
                pattern: "/grc/compliance/settings/authorities-all",
                defaults: new { controller = "ComplianceSettings", action = "AllRegulatoryAuthorities" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-authorities-create",
                pattern: "/grc/compliance/settings/authorities-create",
                defaults: new { controller = "ComplianceSettings", action = "CreateRegulatoryAuthority" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-authorities-update",
                pattern: "/grc/compliance/settings/authorities-update",
                defaults: new { controller = "ComplianceSettings", action = "UpdateRegulatoryAuthority" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-authorities-delete",
                pattern: "/grc/compliance/settings/authorities-delete/{id:long}",
                defaults: new { controller = "ComplianceSettings", action = "DeleteRegulatoryAuthority" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-authorities-export",
                pattern: "/grc/compliance/settings/authorities-export",
                defaults: new { controller = "ComplianceSettings", action = "ExcelExportAuthorities" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-authorities-export-full",
                pattern: "/grc/compliance/settings/authorities-export-full",
                defaults: new { controller = "ComplianceSettings", action = "ExcelExportAllAuthorities" }
            );

            /*----------------------------------------------document type routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-document-types",
                pattern: "/grc/compliance/settings/document-types",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceDocumentType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-document-types",
                pattern: "/grc/compliance/settings/document-types-list",
                defaults: new { controller = "ComplianceSettings", action = "GetDocumentTypes" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-settings-document-types-retrieve",
               pattern: "/grc/compliance/settings/document-types-retrieve/{id:long}",
               defaults: new { controller = "ComplianceSettings", action = "GetDocumentType" }
           );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-document-types-all",
                pattern: "/grc/compliance/settings/document-types-all",
                defaults: new { controller = "ComplianceSettings", action = "AllDocumentTypes" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-document-types-create",
                pattern: "/grc/compliance/settings/document-types-create",
                defaults: new { controller = "ComplianceSettings", action = "CreateDocumentType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-document-types-update",
                pattern: "/grc/compliance/settings/document-types-update",
                defaults: new { controller = "ComplianceSettings", action = "UpdateDocumentType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-document-types-delete",
                pattern: "/grc/compliance/settings/document-types-delete/{id:long}",
                defaults: new { controller = "ComplianceSettings", action = "DeleteDocumentType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-document-types-export",
                pattern: "/grc/compliance/settings/document-types-export",
                defaults: new { controller = "ComplianceSettings", action = "ExcelExportDoctypes" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-document-types-full",
                pattern: "/grc/compliance/settings/document-types-export-full",
                defaults: new { controller = "ComplianceSettings", action = "ExcelExportAllDoctypes" }
            );

            /*--------------------------------------------- compliance Policies/Procedures routes*/
            routeBuilder.MapControllerRoute(
              name: "app-compliance-policies-registers",
              pattern: "/grc/register/policies",
              defaults: new { controller = "CompliancePolicy", action = "PoliciesRegisters" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-policies-retrieve",
                pattern: "/grc/compliance/register/policies-retrieve/{id:long}",
                defaults: new { controller = "CompliancePolicy", action = "GetPolicy" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-policies-all",
                pattern: "/grc/compliance/register/policies-all",
                defaults: new { controller = "CompliancePolicy", action = "AllPolicies" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-policies-list",
               pattern: "/grc/compliance/register/policies-list",
               defaults: new { controller = "CompliancePolicy", action = "GetAllPolicies" }
           );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-policies-create",
                pattern: "/grc/compliance/register/policies-create",
                defaults: new { controller = "CompliancePolicy", action = "CreatePolicy" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-policies-update",
                pattern: "/grc/compliance/register/policies-update",
                defaults: new { controller = "CompliancePolicy", action = "UpdatePolicy" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-policies-delete",
                pattern: "/grc/compliance/register/policies-delete/{id:long}",
                defaults: new { controller = "CompliancePolicy", action = "DeletePolicy" }
            ); 
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-policies-lock",
                pattern: "/grc/compliance/register/policies-lock/{id:long}",
                defaults: new { controller = "CompliancePolicy", action = "LockPolicy" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-register-policies-export",
                pattern: "/grc/compliance/register/policies-export",
                defaults: new { controller = "CompliancePolicy", action = "ExcelExportPolicies" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-authorities-export-full",
                pattern: "/grc/compliance/register/policies-export-full",
                defaults: new { controller = "CompliancePolicy", action = "ExcelExportAllPolicies" }
            );

            /*--------------------------------------------- compliance Policies tasks routes*/
            routeBuilder.MapControllerRoute(
              name: "app-compliance-policies-registers-tasks",
              pattern: "/grc/register/policies-tasks",
              defaults: new { controller = "CompliancePolicy", action = "PoliciesTasks" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-policies-registers-tasks-retrieve",
                pattern: "/grc/compliance/register/tasks-retrieve/{id:long}",
                defaults: new { controller = "CompliancePolicy", action = "GetTask" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-policies-registers-tasks-all",
                pattern: "/grc/compliance/register/tasks-all",
                defaults: new { controller = "CompliancePolicy", action = "AllTasks" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-policies-registers-tasks-list",
               pattern: "/grc/compliance/register/tasks-list",
               defaults: new { controller = "CompliancePolicy", action = "GetAllTasks" }
           );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-policies-registers-tasks-create",
                pattern: "/grc/compliance/register/tasks-create",
                defaults: new { controller = "CompliancePolicy", action = "CreateTask" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-policies-registers-tasks-update",
                pattern: "/grc/compliance/register/tasks-update",
                defaults: new { controller = "CompliancePolicy", action = "UpdateTask" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-policies-registers-tasks-delete",
                pattern: "/grc/compliance/register/tasks-delete/{id:long}",
                defaults: new { controller = "CompliancePolicy", action = "DeleteTask" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-policies-registers-tasks-export",
                pattern: "/grc/compliance/register/tasks-export",
                defaults: new { controller = "CompliancePolicy", action = "ExcelExportTasks" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-policies-registers-tasks-export-full",
                pattern: "/grc/compliance/register/tasks-export-full",
                defaults: new { controller = "CompliancePolicy", action = "ExcelExportAlTasks" }
            );

            /*--------------------------------------------- compliance Policies Documents routes*/

            routeBuilder.MapControllerRoute(
              name: "app-compliance-policies-documents",
              pattern: "/grc/register/policies-documents",
              defaults: new { controller = "CompliancePolicy", action = "PoliciesDocuments" }
            );

            /*----------------------------------------------compliance user routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-users",
                pattern: "/grc/compliance/support/users",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceUsers" }
            );

            /*----------------------------------------------compliance delegation routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-delegation",
                pattern: "/grc/compliance/support/delegation",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceDelegation" }
            );

            /*----------------------------------------------compliance department routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-departments",
                pattern: "/grc/compliance/support/departments",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceDepartments" }
            );

            /*----------------------------------------------compliance responsibilities routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-responsibilities",
                pattern: "/grc/compliance/support/responsibilities",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceResponsibilities" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-responsibilities-types",
                pattern: "/grc/compliance/settings/responsibilities-list",
                defaults: new { controller = "ComplianceSettings", action = "GetResponsibilities" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-settings-responsibilities-retrieve",
               pattern: "/grc/compliance/settings/responsibilities-retrieve/{id:long}",
               defaults: new { controller = "ComplianceSettings", action = "GetResponsibility" }
           );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-responsibilities-all",
                pattern: "/grc/compliance/settings/responsibilities-all",
                defaults: new { controller = "ComplianceSettings", action = "AllResponsibilities" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-responsibilities-create",
                pattern: "/grc/compliance/settings/responsibilities-create",
                defaults: new { controller = "ComplianceSettings", action = "CreateResponsibility" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-responsibilities-update",
                pattern: "/grc/compliance/settings/responsibilities-update",
                defaults: new { controller = "ComplianceSettings", action = "UpdateResponsibility" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-responsibilities-delete",
                pattern: "/grc/compliance/settings/responsibilities-delete/{id:long}",
                defaults: new { controller = "ComplianceSettings", action = "DeleteResponsibility" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-responsibilities-export",
                pattern: "/grc/compliance/settings/responsibilities-export",
                defaults: new { controller = "ComplianceSettings", action = "ExcelExportResponsibilities" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-responsibilities-full",
                pattern: "/grc/compliance/settings/responsibilities-export-full",
                defaults: new { controller = "ComplianceSettings", action = "ExcelExportAllResponsibilities" }
            );

            /*----------------------------------------------regulatory categories routes*/
            routeBuilder.MapControllerRoute(
               name: "app-compliance-settings-regulatory-categories",
               pattern: "/grc/compliance/settings/regulatory-categories",
               defaults: new { controller = "ComplianceSettings", action = "ComplianceRegulatoryCategories" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-categories",
                pattern: "/grc/compliance/support/categories",
                defaults: new { controller = "ComplianceSettings", action = "GetRegulatoryCategories" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-categories-all",
                pattern: "/grc/compliance/support/categories-all",
                defaults: new { controller = "ComplianceSettings", action = "AllRegulatoryCategories" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-paged-categories-all",
                pattern: "/grc/compliance/support/paged-categories-all",
                defaults: new { controller = "ComplianceSettings", action = "GetPagedCategories" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-category-create",
                pattern: "/grc/compliance/support/category-create",
                defaults: new { controller = "ComplianceSettings", action = "CreateRegulatoryCategory" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-category-update",
                pattern: "/grc/compliance/support/category-update",
                defaults: new { controller = "ComplianceSettings", action = "UpdateRegulatoryCategory" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-category-retrieve",
                pattern: "/grc/compliance/support/retrieve-category/{id:long}",
                defaults: new { controller = "ComplianceSettings", action = "GetRegulatoryCategory" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-category-delete",
                pattern: "/grc/compliance/support/category-delete/{id:long}",
                defaults: new { controller = "ComplianceSettings", action = "DeleteRegulatoryCategory" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-category-export",
                pattern: "/grc/compliance/support/category-export",
                defaults: new { controller = "ComplianceSettings", action = "ExportToExcel" }
            );

            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-category-export-full",
                pattern: "/grc/compliance/support/category-export-full",
                defaults: new { controller = "ComplianceSettings", action = "ExportAllCategories" }
            );
            /*----------------------- Application login routes*/
            routeBuilder.MapControllerRoute(
                name: "login",
                pattern: "/login/validate-username",
                defaults: new { controller = "Application", action = "ValidateUsername" }
            );

            // login user
            routeBuilder.MapControllerRoute(
                name: "login",
                pattern: "/login/userlogin",
                defaults: new { controller = "Application", action = "Login" }
            );

            // logout user
            routeBuilder.MapControllerRoute(
                name: "logout",
                pattern: "/app/logout",
                defaults: new { controller = "Application", action = "Logout" }
            );

            // No service
            routeBuilder.MapControllerRoute(
                name: "noservice",
                pattern: "/org/noservice/",
                defaults: new { controller = "Application", action = "NoService" }
            );

            // Create organization
            routeBuilder.MapControllerRoute(
                name: "register",
                pattern: "/org/register/",
                defaults: new { controller = "Application", action = "Register" }
            );
            // Create organization
            routeBuilder.MapControllerRoute(
                name: "language-change",
                pattern: "/org/changeLanguage/",
                defaults: new { controller = "Application", action = "ChangeLanguage" }
            );

            // 404
            routeBuilder.MapControllerRoute(
                name: "404",
                pattern: "/Error/Status404/",
                defaults: new { controller = "Error", action = "Status404" }
            );
            
            //..admin areas
            routeBuilder.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller}/{action=Index}/{id?}"
            );

            // User login (default)
            routeBuilder.MapControllerRoute(
                name: "default",
                pattern: "{controller=Application}/{action=Login}/{id?}"
            );

        }
    }
}
