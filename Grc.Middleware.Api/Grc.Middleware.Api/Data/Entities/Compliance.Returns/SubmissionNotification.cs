namespace Grc.Middleware.Api.Data.Entities.Compliance.Returns
{
    public class SubmissionNotification: BaseEntity {
        public string SentTo { get; set; }
        public string CarbonCopy { get; set; }
        public string BlindCopy { get; set; }
        public string Message { get; set; }
        public long SubmissionId { get; set; }
        public DateTime SentOn { get; set; }
        public virtual ReturnSubmission Submission { get; set; }    
    }
}
