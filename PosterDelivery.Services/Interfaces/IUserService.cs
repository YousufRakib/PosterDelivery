using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services.Interfaces
{
    public interface IUserService
    {
        Task<IList<AuthorizeModel>> Login(Login login);
        Task<IList<Role>> GetUserRole();
        Task<string?> SaveUserInformation(Registration registration);
        Task<bool> SaveLastLogin(int userId, DateTime lastLogin);
        Task<IList<Registration>> GetUsers(string userType);
        Task<string?> DeleteUser(int UserId);
        Task<Registration> GetUserById(int UserId);
    }
}
