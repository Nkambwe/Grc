using Grc.Middleware.Api.Data.Entities.Support;

namespace Grc.Middleware.Api.Data.Entities.Compliance.Audits
{
    public class AuditException: BaseEntity {
        /// <summary>
        /// Gets or sets the audit finding associated with the current entity.
        /// </summary>
        public string AuditFinding { get; set; }
        /// <summary>
        /// Get or set observation by auditors
        /// </summary>
        public string ExceptionNoted { get; set; }
        /// <summary>
        /// Get or Set remediation plan to address the exception
        /// </summary>
        public string RemediationPlan { get; set; }
        /// <summary>
        /// Get or Set Collective action to be taken
        /// </summary>
        public string CorrectiveAction { get; set; }
        /// <summary>
        /// Get or Set target date for remediation completion
        /// </summary>
        public DateTime TargetDate { get; set; }
        /// <summary>
        /// Get or Set risk assessment associated with the exception
        /// </summary>
        public string RiskAssessment { get; set; }
        /// <summary>
        /// Get or Set risk rating //CRITICAL, HIGH, MIDIUM, LOW
        /// </summary>
        public float RiskRating { get; set; }
        /// <summary>
        /// Get or Set Person responsible
        /// </summary>
        public string Executioner { get; set; }
        /// <summary>
        /// Get or Set current status of the exception (e.g., Open, In Progress, Closed)
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Get or Set reference to the associated Audit Report
        /// </summary>
        public long AuditReportId { get; set; }
        public long? AuditTaskId { get; set; }
        public virtual AuditReport AuditReport { get; set; }
        public virtual AuditTask AuditTask { get; set; }
        public long ResponsabilityId { get; set; }
        public virtual Responsebility Responsability { get; set; }
    }
}
