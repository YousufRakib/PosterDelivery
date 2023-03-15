using PosterDelivery.Repository.Interface;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }
        public Task<IList<AuthorizeModel>> Login(Login login)
        {
            return _userRepository.Login(login);
        }

        public async Task<string?> SaveUserInformation(Registration registration)
        {
            return await _userRepository.SaveUserInformation(registration);
        }

        public async Task<bool> SaveLastLogin(int userId, DateTime lastLogin) {
            return await _userRepository.SaveLastLogin(userId, lastLogin);
        }

        public async Task<IList<Role>> GetUserRole()
        {
            return await _userRepository.GetUserRole();
        }

        public async Task<IList<Registration>> GetUsers(string userType)
        {
            return await _userRepository.GetUsers(userType);
        }

        public async Task<string?> DeleteUser(int UserId) 
        {
            return await _userRepository.DeleteUser(UserId);
        }

        public async Task<Registration> GetUserById(int UserId) 
        {
            return await _userRepository.GetUserById(UserId);
        }
    }
}
