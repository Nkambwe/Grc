﻿using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.System {

    public class SystemUser : BaseEntity  {
        public long RoleId { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentUnit { get; set; }
        public string PersonalFileNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string BranchSolId { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsVerified { get; set; }
        public bool IsActive { get; set; }
        public bool IsLoggedIn { get; set; } //Last_Password_Change
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public virtual Department Department { get; set; }
        public virtual SystemRole Role { get; set; }
        public override bool Equals(object obj) {

            if (obj is not SystemUser)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (SystemUser)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.PersonalFileNumber.Equals(PersonalFileNumber) &&
                   item.FirstName.Equals(FirstName) &&
                   item.LastName.Equals(LastName);
        }

        public override string ToString() => $"{PersonalFileNumber} :: {FirstName} {LastName}";
        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }

}
