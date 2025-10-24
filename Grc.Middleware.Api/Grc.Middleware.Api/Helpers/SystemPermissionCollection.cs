namespace Grc.Middleware.Api.Helpers
{
    public class SystemPermissionCollection {

        //..user management
        public List<PermissionRecord> UserManagementPermissions => new() { 
            new()
            {
                Name = "ManageUsers",
                Description = "Can manage system users"
            },
            new()
            {
                Name = "ViewUsers",
                Description = "Can view system users"
            },
            new()
            {
                Name = "CreateUser",
                Description = "Can create system users"
            },
            new()
            {
                Name = "EditUser",
                Description = "Can modify system users"
            },
            new()
            {
                Name = "DeleteUser",
                Description = "Can delete system users"
            }
        };

        //..role management
        public List<PermissionRecord> RoleManagementPermissions => new() {
            new()
            {
                Name = "ManageRoles",
                Description = "Can manage system roles"
            },
            new()
            {
                Name = "ViewRoles",
                Description = "Can view system roles"
            },
            new()
            {
                Name = "CreateRole",
                Description = "Can create system roles"
            },
            new()
            {
                Name = "EditRole",
                Description = "Can modify system roles"
            },
            new()
            {
                Name = "DeleteRole",
                Description = "Can delete system roles"
            },
            new()
            {
                Name = "AssignUserRoles",
                Description = "Can assign system roles"
            },
            new()
            {
                Name = "ManageDepartmentRoles",
                Description = "Can manage department roles"
            },
            new()
            {
                Name = "ViewDepartmentRoles",
                Description = "Can view department roles"
            },
            new()
            {
                Name = "CreateDepartmentRole",
                Description = "Can create department roles"
            },
            new()
            {
                Name = "EditDepartmentRole",
                Description = "Can modify department roles"
            },
            new()
            {
                Name = "DeleteDeleteRole",
                Description = "Can delete department roles"
            },
            new()
            {
                Name = "AssignUserDepartmentRoles",
                Description = "Can assign department system roles"
            },
            new()
            {
                Name = "ManageRoleGroups",
                Description = "Can manage role groups"
            },
            new()
            {
                Name = "ViewRoleGroups",
                Description = "Can view role groups"
            },
            new()
            {
                Name = "CreateRoleGroup",
                Description = "Can create roleGroups"
            },
            new()
            {
                Name = "EditRoleGroup",
                Description = "Can modify role groups"
            },
            new()
            {
                Name = "DeleteRoleGroup",
                Description = "Can delete role groups"
            },
            new()
            {
                Name = "AssignUserRoleGroups",
                Description = "Can assign role groups"
            },
            new()
            {
                Name = "ManageDepartmentRoleGroups",
                Description = "Can manage department role groups"
            },
            new()
            {
                Name = "ViewDepartmentRoleGroups",
                Description = "Can view department role groups"
            },
            new()
            {
                Name = "CreateDepartmentRoleGroup",
                Description = "Can create department role groups"
            },
            new()
            {
                Name = "EditDepartmentRoleGroup",
                Description = "Can modify department role groups"
            },
            new()
            {
                Name = "DeleteDepartmentRoleGroup",
                Description = "Can delete department role groups"
            },
            new()
            {
                Name = "AssignUserDepartmentRoleGroups",
                Description = "Can assign department role groups"
            },
        };

        //..permissions
        public List<PermissionRecord> UserPermissionsManagement => new() {
            new()
            {
                Name = "ManagePermissions",
                Description = "Can manage user permissions"
            },
            new()
            {
                Name = "ViewPermissions",
                Description = "Can view user permissions"
            },
            new()
            {
                Name = "CreatePermissionSet",
                Description = "Can create permission sets"
            },
            new()
            {
                Name = "EditPermissionSet",
                Description = "Can modify permission sets"
            },
            new()
            {
                Name = "DeletePermissionSet",
                Description = "Can delete permission sets"
            },
            new()
            {
                Name = "ManageDepartmentPermissions",
                Description = "Can manage department user permissions"
            },
            new()
            {
                Name = "ViewDepartmentPermissions",
                Description = "Can view department user permissions"
            },
            new()
            {
                Name = "CreateDepartmentPermissionSet",
                Description = "Can create department permission sets"
            },
            new()
            {
                Name = "EditPermissionSet",
                Description = "Can modify permission sets"
            },
            new()
            {
                Name = "DeleteDepartmentPermissionSet",
                Description = "Can delete department permission sets"
            }
        };

        //..delegation management
        public List<PermissionRecord> PermissionDelegationManagement => new() {
            new()
            {
                Name = "DelegatePermissions",
                Description = "Can delegate permissions to users"
            },
            new()
            {
                Name = "ViewDelegatedPermissions",
                Description = "Can view delegated permissions"
            },
            new()
            {
                Name = "RevokeDelegatedPermissions",
                Description = "Can revoke delegated permissions"
            } 

        };

        //..system administration
        public List<PermissionRecord> SystemAdministrationPermissions => new() {
            new()
            {
                Name = "PerformPurge",
                Description = "Can perform system purge. System purge permanently deletes all records marked as deleted"
            },
            new()
            {
                Name = "SuspendUserAccount",
                Description = "Can suspend or lock system account"
            },
            new()
            {
                Name = "ApproveUser",
                Description = "Can approve or verify system user"
            },
            new()
            {
                Name = "SystemAdministration",
                Description = "Can perform system administration role"
            },
            new()
            {
                Name = "ViewSystemLogs",
                Description = "Can view system activity logs"
            },
            new()
            {
                Name = "ManageSystemSettings",
                Description = "Can manage system settings and configurations"
            },
            new()
            {
                Name = "ManageEncryptions",
                Description = "Can manage entity encryption and decryption in the system"
            },
            new()
            {
                Name = "ManagePasswords",
                Description = "Can manage trigger user password reset"
            }
        };

        //..authority management
        public List<PermissionRecord> AuthorityManagementPermissions => new() {
            new()
            {
                Name = "ManageAuthorities",
                Description = "Can manage compliance and operations regulation authorities"
            },
            new()
            {
                Name = "ViewAuthorities",
                Description = "Can view compliance and operations regulation authorities"
            },
            new()
            {
                Name = "CreateAuthority",
                Description = "Can create compliance and operations regulation authorities"
            },
            new()
            {
                Name = "EditAuthority",
                Description = "Can modify compliance and operations regulation authorities"
            },
            new()
            {
                Name = "DeleteAuthority",
                Description = "Can delete compliance and operations regulation authorities"
            }
        };

        //..branch management
        public List<PermissionRecord> BranchManagementPermissions => new() {
            new()
            {
                Name = "ManageBranches",
                Description = "Can manage business branches",
            },
            new()
            {
                Name = "ViewBranches",
                Description = "Can view business branches",
            },
            new()
            {
                Name = "CreateBranches",
                Description = "Can create business branches",
            },
            new()
            {
                Name = "EditBranches",
                Description = "Can modify business branches",
            },
            new()
            {
                Name = "DeleteBranches",
                Description = "Can delete business branches",
            }
        };

        //..department management
        public List<PermissionRecord> DepartmentManagementPermissions => new() {
            new()
            {
                Name = "ManageDepartments",
                Description = "Can manage departments",
            },
            new()
            {
                Name = "ViewDepartments",
                Description = "Can view departments",
            },
            new()
            {
                Name = "CreateDepartments",
                Description = "Can create departments",
            },
            new()
            {
                Name = "EditDepartments",
                Description = "Can modify departments",
            },
            new()
            {
                Name = "DeleteDepartments",
                Description = "Can delete departments",
            }
        };

        //..department unit management
        public List<PermissionRecord> DepartmentUnitManagementPermissions => new() {
            new()
            {
                Name = "ManageDepartmentUnits",
                Description = "Can manage department units",
            },
            new()
            {
                Name = "ViewDepartments",
                Description = "Can view department units",
            },
            new()
            {
                Name = "CreateDepartments",
                Description = "Can create department units",
            },
            new()
            {
                Name = "EditDepartments",
                Description = "Can modify department units",
            },
            new()
            {
                Name = "DeleteDepartments",
                Description = "Can delete department units",
            }
        };

        //..submission frequency management
        public List<PermissionRecord> SubmissionFrequencyManagementPermissions => new() {
            new()
            {
                Name = "ManageSubmissionFrequency",
                Description = "Can manage submission frequencys regulation authorities"
            },
            new()
            {
                Name = "ViewSubmissionFrequency",
                Description = "Can view submission frequency regulation authorities"
            },
            new()
            {
                Name = "CreateSubmissionFrequency",
                Description = "Can create submission frequency regulation authorities"
            },
            new()
            {
                Name = "EditSubmissionFrequency",
                Description = "Can modify submission frequency regulation authorities"
            },
            new()
            {
                Name = "DeleteSubmissionFrequency",
                Description = "Can delete submission frequency regulation authorities"
            }
        };

        //..return type management
        public List<PermissionRecord> ReturnTypeManagementPermissions => new() {
            new()
            {
                Name = "ManageReturnTypes",
                Description = "Can manage compliance return type"
            },
            new()
            {
                Name = "ViewReturnType",
                Description = "Can view compliance return type"
            },
            new()
            {
                Name = "CreateReturnType",
                Description = "Can create compliance return type"
            },
            new()
            {
                Name = "EditReturnType",
                Description = "Can modify compliance return type"
            },
            new()
            {
                Name = "DeleteReturnType",
                Description = "Can delete compliance return type"
            }
        };

        //..responsibility and ownership management
        public List<PermissionRecord> OwnerManagementPermissions => new() {
            new()
            {
                Name = "ManageReturnOwners",
                Description = "Can manage responsibility and return ownership"
            },
            new()
            {
                Name = "ViewReturnOwners",
                Description = "Can view responsibility and return ownership"
            },
            new()
            {
                Name = "CreateReturnOwners",
                Description = "Can create responsibility and return ownership"
            },
            new()
            {
                Name = "EditReturnOwners",
                Description = "Can modify responsibility and return ownership"
            },
            new()
            {
                Name = "DeleteReturnOwners",
                Description = "Can delete responsibility and return ownership"
            }
        };

        //..regulatory types management
        public List<PermissionRecord> RegulatoryTypesManagementPermissions => new() {
            new()
            {
                Name = "ManageRegulatoryTypes",
                Description = "Can manage regulatory types"
            },
            new()
            {
                Name = "ViewRegulatoryTypes",
                Description = "Can view regulatory types"
            },
            new()
            {
                Name = "CreateRegulatoryTypes",
                Description = "Can create regulatory types"
            },
            new()
            {
                Name = "EditRegulatoryTypes",
                Description = "Can modify regulatory types"
            },
            new()
            {
                Name = "DeleteRegulatoryTypes",
                Description = "Can delete regulatory types"
            }
        };

        //..regulatory return management
        public List<PermissionRecord> RegulatoryReturnsManagementPermissions => new() {
            new()
            {
                Name = "ManageRegulatoryReturns",
                Description = "Can manage regulatory returns"
            },
            new()
            {
                Name = "ViewRegulatoryReturns",
                Description = "Can view regulatory returns"
            },
            new()
            {
                Name = "CreateRegulatoryReturns",
                Description = "Can create regulatory returns"
            },
            new()
            {
                Name = "EditRegulatoryReturns",
                Description = "Can modify regulatory returns"
            },
            new()
            {
                Name = "DeleteRegulatoryReturns",
                Description = "Can delete regulatory returns"
            }
        };

        //..regulatory return submissions management
        public List<PermissionRecord> RegulatorySubmissionManagementPermissions => new() {
            new()
            {
                Name = "ManageRegulatoryReturnSubmissions",
                Description = "Can manage regulatory return submission"
            },
            new()
            {
                Name = "ViewRegulatoryReturns",
                Description = "Can view regulatory return submission"
            },
            new()
            {
                Name = "CreateRegulatoryReturns",
                Description = "Can create regulatory return submission"
            },
            new()
            {
                Name = "EditRegulatoryReturns",
                Description = "Can modify regulatory return submission"
            },
            new()
            {
                Name = "DeleteRegulatoryReturns",
                Description = "Can delete regulatory return submission"
            }
        };

        //..regulation and guides submissions management
        public List<PermissionRecord> RegulationAndGuidsManagementPermissions => new() {
            new()
            {
                Name = "ManageRegulationAndGuides",
                Description = "Can manage regulations and guides"
            },
            new()
            {
                Name = "ViewRegulationAndGuides",
                Description = "Can view regulations and guides"
            },
            new()
            {
                Name = "CreateRegulationAndGuides",
                Description = "Can create regulations and guides"
            },
            new()
            {
                Name = "EditRegulationAndGuides",
                Description = "Can modify regulations and guides"
            },
            new()
            {
                Name = "DeleteRegulationAndGuides",
                Description = "Can delete regulations and guides"
            },
            new()
            {
                Name = "CreateStatutoryRegulations",
                Description = "Can create statutory regulations"
            },
            new()
            {
                Name = "EditStatutoryRegulations",
                Description = "Can modify statutory regulations"
            },
            new()
            {
                Name = "DeleteStatutoryRegulations",
                Description = "Can delete statutory regulations"
            }
        };

        //..user compliance audits
        public List<PermissionRecord> ComplianceAuditManagementPermissions => new() {
            new()
            {
                Name = "ManageComplianceAudits",
                Description = "Can manage compliance audits"
            },
            new()
            {
                Name = "ViewComplianceAudits",
                Description = "Can view compliance audits"
            },
            new()
            {
                Name = "CreateComplianceAudits",
                Description = "Can create compliance audits"
            },
            new()
            {
                Name = "EditComplianceAudits",
                Description = "Can modify compliance audits"
            },
            new()
            {
                Name = "DeleteComplianceAudits",
                Description = "Can delete compliance audits"
            }
        };

        //..regulatory categories management
        public List<PermissionRecord> RegulatoryCategoriesManagementPermissions => new() {
            new()
            {
                Name = "ManageRegulatoryCategories",
                Description = "Can manage regulatory Categories",
            },
            new()
            {
                Name = "ViewRegulatoryCategories",
                Description = "Can view regulatory Categories",
            },
            new()
            {
                Name = "CreateRegulatoryCategories",
                Description = "Can create regulatory Categories",
            },
            new()
            {
                Name = "EditRegulatoryCategories",
                Description = "Can modify regulatory Categories",
            },
            new()
            {
                Name = "DeleteRegulatoryCategories",
                Description = "Can delete regulatory Categories",
            }
        };

        //..process types management
        public List<PermissionRecord> ProcessTypeManagementPermissions => new() {
            new()
            {
                Name = "ManageOperationProcessTypes",
                Description = "Can manage Operations processes and service types",
            },
            new()
            {
                Name = "ViewOperationProcessTypes",
                Description = "Can view Operations processes and service types",
            },
            new()
            {
                Name = "CreateOperationProcessTypes",
                Description = "Can create Operations processes and service types",
            },
            new()
            {
                Name = "EditOperationProcessTypes",
                Description = "Can modify Operations processes and service types",
            },
            new()
            {
                Name = "DeleteOperationProcessTypes",
                Description = "Can delete Operations processes and service types",
            }
        };

        //..process tags management
        public List<PermissionRecord> ProcessTagManagementPermissions => new() {
            new()
            {
                Name = "ManageOperationProcessTags",
                Description = "Can manage Operations processes and service tags",
            },
            new()
            {
                Name = "ViewOperationProcessTags",
                Description = "Can view Operations processes and service tags",
            },
            new()
            {
                Name = "CreateOperationProcessTags",
                Description = "Can create Operations processes and service tags",
            },
            new()
            {
                Name = "EditOperationProcessTags",
                Description = "Can modify Operations processes and service tags",
            },
            new()
            {
                Name = "DeleteOperationProcessTags",
                Description = "Can delete Operations processes and service tags",
            }
        };

        //..process groups management
        public List<PermissionRecord> ProcessGroupManagementPermissions => new() {
            new()
            {
                Name = "ManageOperationProcessGroups",
                Description = "Can manage Operations processes and service groups",
            },
            new()
            {
                Name = "ViewOperationProcessGroups",
                Description = "Can view Operations processes and service groups",
            },
            new()
            {
                Name = "CreateOperationProcessGroups",
                Description = "Can create Operations processes and service groups",
            },
            new()
            {
                Name = "EditOperationProcessGroups",
                Description = "Can modify Operations processes and service groups",
            },
            new()
            {
                Name = "DeleteOperationProcessGroups",
                Description = "Can delete Operations processes and service groups",
            }
        };

        //..process tasks management
        public List<PermissionRecord> ProcessTaskManagementPermissions => new() {
            new()
            {
                Name = "ManageOperationProcessTasks",
                Description = "Can manage Operations processes tasks",
            },
            new()
            {
                Name = "ViewOperationProcessTasks",
                Description = "Can view Operations processes tasks",
            },
            new()
            {
                Name = "CreateOperationProcessTasks",
                Description = "Can create Operations processes tasks",
            },
            new()
            {
                Name = "EditOperationProcessTasks",
                Description = "Can modify Operations processes tasks",
            },
            new()
            {
                Name = "DeleteOperationProcessTasks",
                Description = "Can delete Operations process tasks",
            }
        };

        //..process activites management
        public List<PermissionRecord> ProcessActivityManagementPermissions => new() {
            new()
            {
                Name = "ManageOperationProcessActivities",
                Description = "Can manage Operations process activities",
            },
            new()
            {
                Name = "ViewOperationProcessActivities",
                Description = "Can view Operations process activities",
            },
            new()
            {
                Name = "CreateOperationProcessActivities",
                Description = "Can create Operations process activities",
            },
            new()
            {
                Name = "EditOperationProcessActivities",
                Description = "Can modify Operations process activities",
            },
            new()
            {
                Name = "DeleteOperationProcessActivities",
                Description = "Can delete Operations process activities",
            }
        };

        //..process groups management
        public List<PermissionRecord> OperationProcessManagementPermissions => new() {
            new()
            {
                Name = "ManageOperationProcesses",
                Description = "Can manage Operations processes",
            },
            new()
            {
                Name = "ViewOperationProcesses",
                Description = "Can view Operations processes",
            },
            new()
            {
                Name = "CreateOperationProcesses",
                Description = "Can create Operations processes",
            },
            new()
            {
                Name = "EditOperationProcesses",
                Description = "Can modify Operations processes",
            },
            new()
            {
                Name = "DeleteOperationProcesses",
                Description = "Can delete Operations processes",
            }
        };

    }
}
