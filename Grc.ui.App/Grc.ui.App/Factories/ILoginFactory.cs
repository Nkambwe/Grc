using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {

    public interface ILoginFactory {
         Task<LoginModel> PrepareLoginModelAsync();
         Task<UsernameValidationModel> PrepareUsernameValidationModelAsync(string username, string ipAddress);
    }

}
