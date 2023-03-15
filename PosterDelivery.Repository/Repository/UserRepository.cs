using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Repository.Repository {
    public class UserRepository : IUserRepository {
        private readonly IConfiguration? _configuration;
        private readonly string? _connectionString;

        public UserRepository(IConfiguration configuration) {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
        }
        public async Task<IList<AuthorizeModel>> Login(Login login) {
            IList<AuthorizeModel> lstAuthLogin = new List<AuthorizeModel>();
            if (login != null) {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    string query = string.Format(DapperQuery.LoginQuery);
                    var data = await connection.QueryAsync<AuthorizeModel>(query, new { EmailId = login.EmailId, Password = Encrypt(login.Password) });
                    lstAuthLogin = (IList<AuthorizeModel>)data;
                }
            }
            return lstAuthLogin;
        }

        public async Task<bool> SaveLastLogin(int userID, DateTime loginDate) {

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                var rowsAffected  = await connection.ExecuteAsync(DapperQuery.UpdateLastLogin,  new { loginDate, userID });

                return rowsAffected > 0;
            }
        }

        public async Task<string?> SaveUserInformation(Registration registration) {
            string result = "";

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList) {
                    if (ip.AddressFamily == AddressFamily.InterNetwork) {
                        registration.IPAddress = ip.ToString();
                    }
                }
                registration.Password = Encrypt(registration.Password);

                if (registration.UserId > 0) {
                    string query = string.Format(DapperQuery.UpdateUser);
                    await connection.QueryAsync<int>(query, registration);
                    result = "1";
                } else {
                    var existingUser = await connection.QueryAsync<Registration>(DapperQuery.SelectLast1User, new { EmailId = registration.EmailId, UserName = registration.UserName }, commandType: CommandType.Text);
                    if (existingUser.FirstOrDefault() == null) {
                        await connection.QueryAsync<string>("SaveUserInformation", new {
                            FirstName = registration.FirstName,
                            MiddleName = registration.MiddleName,
                            LastName = registration.LastName,
                            EmailId = registration.EmailId,
                            UserName = registration.UserName,
                            Password = registration.Password,
                            IPAddress = registration.IPAddress,
                            CreatedBy = registration.CreatedBy,
                            RoleId = registration.UserRole
                        }, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                        result = "1";
                    } else {
                        result = "2";
                    }
                }
            }

            return result;
        }

        private string Encrypt(string password) {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);
            using (Aes encryptor = Aes.Create()) {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream()) {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write)) {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    password = Convert.ToBase64String(ms.ToArray());
                }
            }
            return password;
        }

        private string Decrypt(string cipherText) {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create()) {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream()) {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write)) {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }

        public async Task<IList<Role>> GetUserRole() {

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetUserRoleQuery);
                var roles = await connection.QueryAsync<Role>(query);

                var result = roles.ToList();
                return result;
            }

        }

        public async Task<IList<Registration>> GetUsers(string userType) {

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                string query = "";

                if (userType == "Driver") {
                    query = string.Format(DapperQuery.GetDrivers);
                } else {
                    query = string.Format(DapperQuery.GetUsers);
                }

                var users = await connection.QueryAsync<Registration>(query);

                var result = users.ToList();
                return result;
            }

        }

        public async Task<string?> DeleteUser(int UserId) {
            string result = string.Empty;

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.DeleteUsers);
                await connection.QueryAsync<int>(query, new { Id = UserId });

                result = "1";
            }

            return result;
        }

        public async Task<Registration> GetUserById(int UserId) 
        {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                string query = string.Format(DapperQuery.GetUsersById);
                var user = await connection.QueryAsync<Registration>(query, new { Id = UserId });
                user.FirstOrDefault().Password = Decrypt(user.FirstOrDefault().Password);
                var result = user.FirstOrDefault();
                return result;
            }
        }
    }
}
