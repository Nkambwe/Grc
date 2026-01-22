namespace Grc.ui.App.Http.Responses {
    public class GrcUserSupportResponse {
        public List<GrcBranchItemResponse> Branches {get;set;}
        public List<GrcRoleItemResponse> Roles {get;set;}
        public List<GrcDepartmentItemResponse> Departments {get;set;}
    }
}
