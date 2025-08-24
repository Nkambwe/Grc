using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {

    public class GrcErrorFactory: IGrcErrorFactory {

        public Task<GrcErrorModel> PrepareErrorModelAsync(long companyId, string message, string source, string stacktrace)
            => Task.FromResult(new GrcErrorModel(){ 
                CompanyId = companyId,
                Message = message,
                Source = source,
                StackTrace = stacktrace
            });
    }
}
