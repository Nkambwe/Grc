using Grc.Middleware.Api.Data.Entities;

namespace Grc.Middleware.Api.Factories {
    public class BranchFactory {

        public Branch CreateMainBranch() 
            => new(){ 
                BranchName = "Head Office",
                SolId = "MAIN",
                IsDeleted = false,
                CreatedBy = "1",
                CreatedOn = DateTime.Now,
                LastModifiedBy = "1",
                LastModifiedOn = DateTime.Now,
                Departments = new List<Department>() {
                    new() {
                        DepartmenCode = "TECH01",
                        DepartmentName = "Business Technology",
                        Alias = "BT",
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
