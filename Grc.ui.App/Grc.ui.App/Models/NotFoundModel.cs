using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;

namespace Grc.ui.App.Models {
    public class NotFoundModel {
        public string StatusCode { get; } = $"{(int)GrcStatusCodes.NOTFOUND}";
        public string ErrorName { get; } = GrcStatusCodes.NOTFOUND.GetDescription();
    }
}
