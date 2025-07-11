namespace Grc.ui.App.Dtos {

    public class SystemLanguage {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsDefault { get; set; }
        public bool IsRightToLeft { get; set; }

        public List<LanguageResource> Resources { get; protected set; }

        public SystemLanguage() {
            Resources = new List<LanguageResource>();
        }
    }
}
