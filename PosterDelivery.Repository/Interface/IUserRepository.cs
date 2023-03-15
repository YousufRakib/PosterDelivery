using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Repository.Interface
{
    public interface IUserRepository
    {
        Task<IList<AuthorizeModel>> Login(Login login);
        Task<IList<Role>> GetUserRole();
        Task<string?> SaveUserInformation(Registration registration);
        Task<bool> SaveLastLogin(int userID, DateTime loginDate);
        Task<IList<Registration>> GetUsers(string userType);
        Task<string?> DeleteUser(int UserId);
        Task<Registration> GetUserById(int UserId);
    }
}
