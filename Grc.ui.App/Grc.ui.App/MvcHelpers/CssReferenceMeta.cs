namespace Grc.ui.App.MvcHelpers {

        /// <summary>
        /// CSS reference metadata
        /// </summary>
        public class CssReferenceMeta {
            /// <summary>
            /// Source
            /// </summary>
            public string Src { get; set; }

            /// <summary>
            /// A value indicating whether to exclude the style sheet from bundling
            /// </summary>
            public bool ExcludeFromBundle { get; set; }

            /// <summary>
            /// A value indicating whether the source is local
            /// </summary>
            public bool IsLocal { get; set; }
        }
}
