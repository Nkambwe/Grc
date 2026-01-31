using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IConfigurationFactory {  
        Task<ConfigurationModel> PrepareConfigurationModelAsync(UserModel data);
    }
}
