using System.Text.RegularExpressions;

namespace Grc.Middleware.Api.Sanitizer {
    public static class DataSanitizer {
        public static string SanitizeInput(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            //..remove potential XSS payloads
            input = RemoveScriptTags(input);
            input = RemoveEventHandlers(input);
            input = RemoveJavascriptProtocols(input);
            input = RemoveSqlInjectionPatterns(input);

            //..encode remaining HTML special characters
            input = System.Net.WebUtility.HtmlEncode(input);

            return input;
        }

        public static bool ContainsPathTraversal(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] pathTraversalPatterns = {
            @"\.\./", @"\.\.\\",
            @"\betc/passwd\b", @"\bwindows\\system.ini\b",
            @"\bcmd\.exe\b", @"\bpowershell\b",
            @"\bcat\b.*\bpasswd\b", @"\btype\b.*\bini\b"
        };

            foreach (var pattern in pathTraversalPatterns) {
                if (Regex.IsMatch(input, pattern,
                    RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

        public static bool ContainsXSS(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] xssPatterns = {
            @"<script", @"javascript:", @"onerror\s*=", @"onload\s*=",
            @"onclick\s*=", @"alert\s*\(", @"<img.*onerror", @"<svg.*onload",
            @"&lt;script", @"&lt;img", @"eval\s*\(", @"document\.cookie"
        };

            foreach (var pattern in xssPatterns) {
                if (Regex.IsMatch(input, pattern,
                    RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

        public static bool ContainsSQLInjection(string input) {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] sqlPatterns = {
            @"'\s*OR\s*'", @"'\s*AND\s*'", @"--", @";\s*DROP\s+TABLE",
            @";\s*DELETE\s+FROM", @";\s*UPDATE\s+.*SET", @"UNION\s+SELECT",
            @"SLEEP\s*\(", @"WAITFOR\s+DELAY", @"BENCHMARK\s*\(",
            @"INFORMATION_SCHEMA", @"sys\.tables", @"xp_cmdshell"
        };

            foreach (var pattern in sqlPatterns) {
                if (Regex.IsMatch(input, pattern,
                    RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

        private static string RemoveScriptTags(string input) {
            // Remove <script> tags and their contents
            input = Regex.Replace(input,
                @"<script[^>]*>.*?</script>", "",
                RegexOptions.IgnoreCase |
                RegexOptions.Singleline);

            // Remove standalone script tags
            input = Regex.Replace(input,
                @"<[\/]?script[^>]*>", "",
                RegexOptions.IgnoreCase);

            return input;
        }

        private static string RemoveEventHandlers(string input) {
            string[] eventHandlers = {
            "onerror", "onload", "onclick", "onmouseover", "onmouseout",
            "onfocus", "onblur", "onsubmit", "onreset", "onchange", "onselect",
            "onabort", "onkeydown", "onkeypress", "onkeyup"
        };

            foreach (var handler in eventHandlers) {
                input = Regex.Replace(input,
                    $@"{handler}\s*=\s*[""'][^""']*[""']", "",
                    RegexOptions.IgnoreCase);
            }

            return input;
        }

        private static string RemoveJavascriptProtocols(string input) {
            input = Regex.Replace(input,
                @"javascript\s*:", "blocked:",
                RegexOptions.IgnoreCase);

            input = Regex.Replace(input,
                @"vbscript\s*:", "blocked:",
                RegexOptions.IgnoreCase);

            return input;
        }

        private static string RemoveSqlInjectionPatterns(string input) {
            input = Regex.Replace(input,
                @"([';])|(--)|(\bOR\b.*=)|(\bAND\b.*=)|(\bUNION\b.*\bSELECT\b)", "",
                RegexOptions.IgnoreCase);

            return input;
        }

        // Helper method to sanitize an entire object
        public static T SanitizeObject<T>(T obj) where T : class {
            if (obj == null) return obj;

            var properties = typeof(T).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanWrite);

            foreach (var prop in properties) {
                var value = prop.GetValue(obj) as string;
                if (!string.IsNullOrWhiteSpace(value)) {
                    var sanitizedValue = SanitizeInput(value);
                    prop.SetValue(obj, sanitizedValue);
                }
            }

            return obj;
        }
    }
}
