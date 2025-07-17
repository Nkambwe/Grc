using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {

    public enum GroupScope {
          [Description("Undefined")]
          UNDEFINED = 0,
          [Description("System")]
          SYSTEM = 1,
          [Description("Department")]
          DEPARTMENTAL = 2
    }
}
