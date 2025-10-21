using Grc.Middleware.Api.Data.Entities.Operations.Processes;

namespace Grc.Middleware.Api.Data.Entities.Support {
    public class ProcessType: BaseEntity {
        public string TypeName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<OperationProcess> Processes { get; set; }
    }

}
