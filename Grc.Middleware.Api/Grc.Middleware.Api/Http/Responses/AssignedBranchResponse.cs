namespace Grc.Middleware.Api.Http.Responses {
    public class AssignedBranchResponse {
        public long BranchId { get; set; }
        public long OrganizationId { get; set; }
        public string SolId { get; set; }
        public string OrganizationName { get; set; }
        public string OrgAlias { get; set; }
        public string BranchName { get; set; }
    }

}
