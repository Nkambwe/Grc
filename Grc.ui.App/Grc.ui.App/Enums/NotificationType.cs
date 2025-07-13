using System.Runtime.Serialization;

namespace Grc.ui.App.Enums {

    /// <summary>
    /// Notifcation dialog type
    /// </summary>
    public enum NotificationType {

        [EnumMember(Value = "info")]
        Info = 1,
        [EnumMember(Value = "success")]
        Success = 2,
        [EnumMember(Value = "error")]
        Error = 3,
        [EnumMember(Value = "warning")]
        Warning = 4,
        [EnumMember(Value = "question")]
        Question = 5
    }
}
