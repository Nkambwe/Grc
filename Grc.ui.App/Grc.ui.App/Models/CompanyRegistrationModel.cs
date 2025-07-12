namespace Grc.ui.App.Models {

    public class CompanyRegistrationModel {
        public string CompanyName { get; set; }
        public string Alias { get; set; }
        public string RegistrationNumber { get; set; }
        public string SystemLanguage { get; set; }

        public UserModel SystemAdmin { get; set; }
    }

}
