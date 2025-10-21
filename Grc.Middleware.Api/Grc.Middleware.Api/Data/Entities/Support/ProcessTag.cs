using Grc.Middleware.Api.Data.Entities.Operations.Processes;

namespace Grc.Middleware.Api.Data.Entities.Support
{
    public class ProcessTag: BaseEntity {
        public string TagName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ProcessProcessTag> Processes { get; set; }
    }
}
