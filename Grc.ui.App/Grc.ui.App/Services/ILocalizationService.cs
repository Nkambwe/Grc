using Grc.ui.App.Dtos;

namespace Grc.ui.App.Services {

    public interface ILocalizationService { 

        /// <summary>
        /// Get language resource
        /// </summary>
        /// <param name="labelName">Language label name</param>
        /// <returns>Language label value</returns>
        string GetLocalizedLabel(string labelName);

        /// <summary>
        /// Get a list of available languages
        /// </summary>
        /// <returns></returns>
        IList<SystemLanguage> GetAvailableLanguages();

        /// <summary>
        /// Save a language for the System
        /// </summary>
        /// <param name="languageCode">Language code</param>
        void SaveCurrentLanguage(string languageCode);
    }

}
