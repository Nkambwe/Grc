namespace Grc.Middleware.Api.Data.Entities {

    /// <summary>
    /// Base class for all Micro domain entities
    /// </summary>
    public abstract class BaseEntity : ISoftDelete {
        /// <summary>
        /// Gets or sets the unique Id of the entity in the underlying data store.
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Get or set the record as deleted
        /// </summary>
        public bool IsDeleted { get;set; }

        /// <summary>
        /// Get or Set Creation date
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Get or Set Person who created record
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Get or Set Modification Date
        /// </summary>
        public DateTime? LastModifiedOn {get;set;}

        /// <summary>
        /// Get or Set Person who modified record
        /// </summary>
        public string LastModifiedBy { get;set; }

        /// <summary>
        /// Checks if the current domain entity has an identity.
        /// </summary>
        /// <returns>True if the domain entity has no identity yet, false otherwise.</returns>
        public bool IsNew() => default == 0 || Id.Equals(default);

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        /// The object to compare with the current object. 
        /// </param>
        public override bool Equals(object obj) {

            if (obj is not BaseEntity)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (BaseEntity)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.Id.Equals(Id);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />
        /// </returns>
        public override int GetHashCode() 
            => ToString().GetHashCode();

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="thisEntity">The first instance to compare.</param>
        /// <param name="thatEntity">The second instance to compare.</param>
        public static bool operator ==(BaseEntity thisEntity, BaseEntity thatEntity) 
            => thisEntity?.Equals(thatEntity) ?? Equals(thatEntity, null);

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="thisEntity">The first instance to compare.</param>
        /// <param name="thatEntity">The second instance to compare.</param>
        /// <returns>False when the objects are the same, true otherwise.</returns>
        public static bool operator !=(BaseEntity thisEntity, BaseEntity thatEntity) 
            => !(thisEntity == thatEntity);
    }

}
