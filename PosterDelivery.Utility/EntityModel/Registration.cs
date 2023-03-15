using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility.EntityModel
{
    public class Registration
    {
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? EmailId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public string? IPAddress { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? IsActive { get; set; }
        public string? UserRole { get; set; }

        public string? Phone { get; set; }
        public string? Address { get; set; }

        public string LastLoginFriendly {
            get {
                if (LastLoginDateTime.HasValue) {
                    return $"{LastLoginDateTime.Value.ToShortDateString()} {LastLoginDateTime.Value.ToShortTimeString()}";
                } else {
                    return "None";
                }
            }
        }
    }
}
