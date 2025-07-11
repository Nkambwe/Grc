namespace Grc.ui.App.MvcHelpers {

    /// <summary>
    /// Script reference metadata
    /// </summary>
    public class ScriptReferenceMeta {
        /// <summary>
        /// Source
        /// </summary>
        public string Src { get; set; }

        /// <summary>
        /// A value indicating whether to exclude the script from bundling
        /// </summary>
        public bool ExcludeFromBundle { get; set; }

        /// <summary>
        /// A value indicating whether the source is local
        /// </summary>
        public bool IsLocal { get; set; }
    }

}
