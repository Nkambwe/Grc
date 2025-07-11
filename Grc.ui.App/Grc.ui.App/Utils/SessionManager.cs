using System.Text.Json;

namespace Grc.ui.App.Utils {

    /// <summary>
    /// Provides strongly typed session management for the application
    /// </summary>
    public class SessionManager {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManager(IHttpContextAccessor httpContextAccessor)  {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Save a value to the session with the specified key
        /// </summary>
        /// <typeparam name="T">Type of object to add to session</typeparam>
        /// <param name="key">Session Key</param>
        /// <param name="value">Value to save to session</param>
        public void Save<T>(string key, T value){ 

            if (string.IsNullOrEmpty(key)) 
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            var session = _httpContextAccessor.HttpContext?.Session ??
                throw new InvalidOperationException("HttpContext.Session is not available");

            if (value == null){ 
                session.Remove(key);
                return;
            }

            // For primitive types and strings, use the built-in session extensions
            if (typeof(T) == typeof(string)){
                session.SetString(key, value as string);
            } else if (typeof(T) == typeof(int)){
                session.SetInt32(key, (int)(object)value);
            } else if (typeof(T) == typeof(bool)){
                session.SetInt32(key, (bool)(object)value ? 1 : 0);
            } else { 
                // For complex types, serialize to JSON
                var serialized = JsonSerializer.Serialize(value);
                session.SetString(key, serialized);
            }
        }

        /// <summary>
        /// Get a value from the session with the specified key
        /// </summary>
        /// <typeparam name="T">Type of object to add to session</typeparam>
        /// <param name="key">Session Key</param>
        /// <returns>Object of type T saved in session</returns>
        public T Get<T>(string key) { 

            if (string.IsNullOrEmpty(key)) 
                throw new ArgumentException("Session Key cannot is required", nameof(key));

            //..make sure session exists
            var session = _httpContextAccessor.HttpContext?.Session ?? throw new InvalidOperationException("Session Expired");

            //..for primitive types and strings just use the built-in session extensions
            if (typeof(T) == typeof(string)) {
                return (T)(object)session.GetString(key);
            } else if (typeof(T) == typeof(int)) {
                var value = session.GetInt32(key);
	            return value.HasValue ? (T)(object)value.Value : default;
            } else if (typeof(T) == typeof(bool)) {
                var value = session.GetInt32(key);
	            return value.HasValue ? (T)(object)(value.Value == 1) : default;
            } else {

                //..we're dealing with complex object we need to deserialize from JSON
                var value = session.GetString(key);
                if (string.IsNullOrEmpty(value)){ 
                    return default;
                }

                return JsonSerializer.Deserialize<T>(value);
            }

        }

        /// <summary>
        /// Remove a value from the session with the specified key
        /// </summary>
        /// <param name="key">Session Key</param>
        /// <exception cref="ArgumentException">Invalid argument exception</exception>
        /// <exception cref="InvalidOperationException">Invalid Operation exception</exception>
        public void Remove(string key){ 
            
            if (string.IsNullOrEmpty(key)) 
                throw new ArgumentException("Session Key cannot is required", nameof(key));

            var session = _httpContextAccessor.HttpContext?.Session  ?? throw new InvalidOperationException("Session Expired");
            session.Remove(key);
        }

        /// <summary>
        /// Clear all session data
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Clear() { 
            var session = _httpContextAccessor.HttpContext?.Session ?? throw new InvalidOperationException("Session Expired");
            session.Clear();
        }

        /// <summary>
        /// Check if a key exists in the session
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key){ 

            if (string.IsNullOrEmpty(key)) 
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            var session = _httpContextAccessor.HttpContext?.Session ?? throw new InvalidOperationException("Session Expired");

            byte[] value;
            return session.TryGetValue(key, out value);
        }
    }

}
