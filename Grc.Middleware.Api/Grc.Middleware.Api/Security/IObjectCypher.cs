namespace Grc.Middleware.Api.Security {
    public interface IObjectCypher { 
        /// <summary>
        /// Encrypts specified string properties of an object
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="entity">The entity to encrypt properties on</param>
        /// <param name="propertiesToEncrypt">Array of property names to encrypt</param>
        /// <returns>The entity with encrypted properties</returns>
         T EncryptProperties<T>(T entity, string[] propertiesToEncrypt) where T : class ;

        /// <summary>
        /// Decrypts specified string properties of an object
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="entity">The entity to decrypt properties on</param>
        /// <param name="propertiesToDecrypt">Array of property names to decrypt</param>
        /// <returns>The entity with decrypted properties</returns>
        T DecryptProperties<T>(T entity, string[] propertiesToDecrypt) where T : class;

        /// <summary>
        /// Encrypts specified properties and returns a new object with encrypted values
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="entity">The entity to encrypt properties on</param>
        /// <param name="propertiesToEncrypt">Array of property names to encrypt</param>
        /// <returns>A new entity with encrypted properties</returns>
        T EncryptPropertiesImmutable<T>(T entity, string[] propertiesToEncrypt) where T : class, new();

        /// <summary>
        /// Decrypts specified properties and returns a new object with decrypted values
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="entity">The entity to decrypt properties on</param>
        /// <param name="propertiesToDecrypt">Array of property names to decrypt</param>
        /// <returns>A new entity with decrypted properties</returns>
        T DecryptPropertiesImmutable<T>(T entity, string[] propertiesToDecrypt) where T : class, new();
    }
}
