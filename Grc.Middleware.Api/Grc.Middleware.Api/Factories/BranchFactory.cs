using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Security;

namespace Grc.Middleware.Api.Factories {
    public class BranchFactory {

        public Branch CreateMainBranch(SystemUser user) {
            user.Role = new() {
                RoleName = "Support",
                Description = "Application Support Role",
                Group = new SystemRoleGroup() {
                    GroupName = "Application Support",
                    Description = "This role group provides application support to system users. They have access to much of the system",
                    GroupCategory = "ADMINSUPPORT",
                    Scope = GroupScope.SYSTEM,
                    Type = RoleGroup.SYSTEM,
                    CreatedBy = "1",
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = "1",
                    LastModifiedOn = DateTime.Now
                },
                IsDeleted = false,
                IsApproved = true,
                IsVerified = true,
                CreatedBy = "1",
                CreatedOn = DateTime.Now,
                LastModifiedBy = "1",
                LastModifiedOn = DateTime.Now
            };

            return new(){ 
                BranchName = "Head Office",
                SolId = "MAIN",
                IsDeleted = false,
                CreatedBy = "1",
                CreatedOn = DateTime.Now,
                LastModifiedBy = "1",
                LastModifiedOn = DateTime.Now,
                Departments = new List<Department>() {
                    new() {
                        DepartmentCode = "TECH",
                        DepartmentName = "Business Technology",
                        Alias = "BT",
                        IsDeleted = false,
                        CreatedBy = "1",
                        CreatedOn = DateTime.Now,
                        LastModifiedBy = "1",
                        LastModifiedOn = DateTime.Now,
                        Users = new List<SystemUser>() {
                            user,
                            new() {
                                FirstName = HashGenerator.EncryptString("Grc"),
                                LastName = HashGenerator.EncryptString("System"),
                                Username = "Super",
                                EmailAddress = HashGenerator.EncryptString("grc.system@pearlbank.co.ug"),
                                PasswordHash = HashGenerator.EncryptString(ExtendedHashMapper.HashPassword("super390@pbu")),
                                PFNumber = HashGenerator.EncryptString("00001"),
                                BranchSolId = "MAIN",
                                PhoneNumber = HashGenerator.EncryptString("390400800800"),
                                IsActive = true,
                                IsDeleted = false,
                                IsVerified = true,
                                IsApproved = true,
                                Role = new SystemRole() {
                                    RoleName = "Administrator",
                                    Description = "System Administrator Role",
                                    Group = new SystemRoleGroup() {
                                        GroupName = "System Administrators",
                                        GroupCategory = "ADMINSUPPORT",
                                        Description = "This role group has complete and total access to the entire system",
                                        Scope = GroupScope.SYSTEM,
                                        Type = RoleGroup.SYSTEM,
                                        CreatedBy = "1",
                                        CreatedOn = DateTime.Now,
                                        LastModifiedBy = "1",
                                        LastModifiedOn = DateTime.Now
                                    },
                                    IsDeleted = false,
                                    IsApproved = true,
                                    IsVerified = true,
                                    CreatedBy = "1",
                                    CreatedOn = DateTime.Now,
                                    LastModifiedBy = "1",
                                    LastModifiedOn = DateTime.Now
                                },
                                CreatedBy = "1",
                                CreatedOn = DateTime.Now,
                                LastModifiedBy = "1",
                                LastModifiedOn = DateTime.Now,
                            }
                        }
                    },
                    new() {
                        DepartmentCode = "DIGI",
                        DepartmentName = "Digitization and Innovation",
                        Alias = "DI",
                        IsDeleted = false,
                        CreatedBy = "1",
                        CreatedOn = DateTime.Now,
                        LastModifiedBy = "1",
                        LastModifiedOn = DateTime.Now,
                    },
                    new() {
                        DepartmentCode = "RICO",
                        DepartmentName = "Risk and Compliance",
                        Alias = "CR",
                        IsDeleted = false,
                        CreatedBy = "1",
                        CreatedOn = DateTime.Now,
                        LastModifiedBy = "1",
                        LastModifiedOn = DateTime.Now,
                    }
                }
            };
        }
    }
}
