using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility
{
    public class ResponseMessage
    {
        public string EmailExist = "This user/email already exists!";
        public string UsernameExist = "This Username already exists!";
        public string UnitTypeExist = "This UnitType already exists!";
        public string ProductCategoryExist = "This ProductCategory already exists!";
        public string ProductExist = "This Product already exists!";
        public string CountryNameExist = "This country name already exists!";
        public string RoleExist = "This role already exists!";
        public string TerminatedUser = "This user is terminated from the system!";
        public string WrongRoleInfo = "Invalid Role Attempt!";
        public string RoleNotExist = "UserRole not found!";
        public string UserNotExist = "User not found!";
        public string EmptyErrorDetails = "No error found!";
        public string TryCatchError = "An error occured.To solve the error, Please check Error Log or Contact with your support engineer";
    }
}
