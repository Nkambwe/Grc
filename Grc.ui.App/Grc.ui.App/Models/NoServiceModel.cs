using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;

namespace Grc.ui.App.Models {
    public class NoServiceModel {
        public string StatusCode { get; } = $"{(int)GrcStatusCodes.SERVICEUNVAILABLE}";
        public string ErrorName { get; } = GrcStatusCodes.SERVICEUNVAILABLE.GetDescription();
    }
}
