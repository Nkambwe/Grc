using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {

    public class ErrorFactory : IErrorFactory {
        public Task<NotFoundModel> PrepareNotFoundModelAsync()
            => Task.FromResult(new NotFoundModel());
    }
}
