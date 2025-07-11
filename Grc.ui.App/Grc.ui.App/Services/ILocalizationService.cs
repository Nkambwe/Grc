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
    }

}
