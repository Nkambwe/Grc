using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {

    public interface ILoginFactory {
         Task<LoginModel> PrepareLoginModelAsync();
    }

}
