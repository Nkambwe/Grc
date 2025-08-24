using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IGrcErrorFactory { 
        Task<GrcErrorModel> PrepareErrorModelAsync(long companyId, string message, string source, string stacktrace);
        
    }
}
