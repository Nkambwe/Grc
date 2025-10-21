namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits
{
    public class AuditException: BaseEntity {
        public string Obligation { get; set; }
        public string CorrectiveAction { get; set; }
        /// <summary>
        /// Get or set observation by auditors
        /// </summary>
        public string ExceptionNoted { get; set; }
        /// <summary>
        /// Get or Set remediation plan to address the exception
        /// </summary>
        public string RemediationPlan { get; set; }
        /// <summary>
        /// Get or Set target date for remediation completion
        /// </summary>
        public DateTime TargetDate { get; set; }
        /// <summary>
        /// Get or Set risk assessment associated with the exception
        /// </summary>
        public string RiskAssessment { get; set; }
        /// <summary>
        /// Get or Set risk rating 
        /// </summary>
        public float RiskRating { get; set; }
        /// <summary>
        /// Get or Set current status of the exception (e.g., Open, In Progress, Closed)
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Get or Set date when the exception was last updated
        /// </summary>
        public DateTime? LastUpdated { get; set; }
        /// <summary>
        /// Get or Set reference to the associated Audit Report
        /// </summary>
        public long AuditReportId { get; set; }
        public long AuditTaskId { get; set; }
        public virtual AuditReport AuditReport { get; set; }
        public virtual AuditTask AuditTask { get; set; }
    }
}
