using Grc.ui.App.Models;

namespace Grc.ui.App.Factories {
    public interface IDepartmentFactory { 
         Task<DepartmentListModel> PrepareDepartmentListModelAsync(UserModel currentUser);
         Task<DepartmentModel>  PrepareDepartmentModelAsync(UserModel userModel);
    }
}
