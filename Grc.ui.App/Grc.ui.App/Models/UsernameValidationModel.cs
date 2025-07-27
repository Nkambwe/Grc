namespace Grc.ui.App.Models {
    public class UsernameValidationModel { 
        public string Username { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string[] Encrypt {get; set; } = Array.Empty<string>();
        public string[] Decrypt {get; set; } = new string[] {"firstName"};

    }
}
