using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Security {

    public class ObjectCypher : IObjectCypher {

        private readonly IServiceLogger _logger;
        public ObjectCypher(IServiceLoggerFactory logger) { 
            _logger = logger.CreateLogger();
        }

        public T EncryptProperties<T>(T entity, string[] propertiesToEncrypt) where T : class {
            if (entity == null || propertiesToEncrypt == null || propertiesToEncrypt.Length == 0)
                return entity;
            
            var entityType = entity.GetType();
    
            //..create a dictionary of property names (case-insensitive)
            var properties = entityType.GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite)
                .ToDictionary(p => p.Name.ToLower(), p => p);
            
            foreach (var propertyName in propertiesToEncrypt) {
                var normalizedPropertyName = propertyName.ToLower();
        
                //..check if the property exists in our dictionary
                if (properties.TryGetValue(normalizedPropertyName, out var propertyInfo)) {
                    //..get the plain text value
                    var plainTextValue = propertyInfo.GetValue(entity) as string;
            
                    if (!string.IsNullOrEmpty(plainTextValue)) {
                        try {
                            //..encrypt the value
                            var encryptedValue = HashGenerator.EncryptString(plainTextValue);
                    
                            //..set the encrypted value back to the property
                            propertyInfo.SetValue(entity, encryptedValue);
                        } catch (Exception ex)  {
                            string msg = $"Failed to encrypt property {propertyInfo.Name}: {ex.Message}";
                            _logger.LogActivity(msg, "ERROR");
                            _logger.LogActivity(ex.StackTrace, "STACKTRACE");
                            throw new Exception(msg);
                        }
                    }
                }
            }
        
            return entity;
        }
    
        public T DecryptProperties<T>(T entity, string[] propertiesToDecrypt) where T : class {
            if (entity == null || propertiesToDecrypt == null || propertiesToDecrypt.Length == 0)
                return entity;
            
            var entityType = entity.GetType();
    
            //..create a dictionary of property names (case-insensitive)
            var properties = entityType.GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite)
                .ToDictionary(p => p.Name.ToLower(), p => p);
            
            foreach (var propertyName in propertiesToDecrypt) {
                var normalizedPropertyName = propertyName.ToLower();
        
                //..check if the property exists in our dictionary
                if (properties.TryGetValue(normalizedPropertyName, out var propertyInfo)) {
                    //..get the encrypted value
                    var encryptedValue = propertyInfo.GetValue(entity) as string;
            
                    if (!string.IsNullOrEmpty(encryptedValue)) {
                        try 
                        {
                            //..decrypt the value
                            var decryptedValue = HashGenerator.DecryptString(encryptedValue);
                    
                            //..set the decrypted value back to the property
                            propertyInfo.SetValue(entity, decryptedValue);
                        } catch (Exception ex) {
                            string msg = $"Failed to decrypt property {propertyInfo.Name}: {ex.Message}";
                            _logger.LogActivity(msg, "ERROR");
                            _logger.LogActivity(ex.StackTrace, "STACKTRACE");
                            throw new Exception(msg);
                        }
                    }
                }
            }
        
            return entity;
        }
    
        public T EncryptPropertiesImmutable<T>(T entity, string[] propertiesToEncrypt) where T : class, new() {
            if (entity == null) return null;
        
            //..create a copy of the entity
            var copy = CreateCopy(entity);
        
            //..encrypt the copy and return it
            return EncryptProperties(copy, propertiesToEncrypt);
        }
    
        public T DecryptPropertiesImmutable<T>(T entity, string[] propertiesToDecrypt) where T : class, new() {
            if (entity == null) return null;
        
            //..create a copy of the entity
            var copy = CreateCopy(entity);
        
            //..decrypt the copy and return it
            return DecryptProperties(copy, propertiesToDecrypt);
        }
    
        /// <summary>
        /// Creates a shallow copy of an object
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="source">The source object to copy</param>
        /// <returns>A copy of the source object</returns>
        private T CreateCopy<T>(T source) where T : class, new() {
            var copy = new T();
            var properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.CanWrite);
            
            foreach (var property in properties) {
                try {
                    var value = property.GetValue(source);
                    property.SetValue(copy, value);
                } catch (Exception ex) {
                    _logger.LogActivity($"Failed to copy property {property.Name}: {ex.Message}", "WARNING");
                }
            }
        
            return copy;
        }
    }
}
