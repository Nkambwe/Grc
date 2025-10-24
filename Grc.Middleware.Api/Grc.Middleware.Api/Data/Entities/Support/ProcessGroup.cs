using Grc.Middleware.Api.Data.Entities.Operations.Processes;

namespace Grc.Middleware.Api.Data.Entities.Support
{
    public class ProcessGroup : BaseEntity
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public ICollection<ProcessProcessGroup> Processes { get; set; } = new List<ProcessProcessGroup>();
    }
}
