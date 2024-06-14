using BCrypt;
using Microsoft.AspNetCore.Identity;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Sabio.Services
{
    public class UserService : IUserService
    {
        private IAuthenticationService<int> _authenticationService;
        private IDataProvider _dataProvider;
        private ILookUpService _lookUpService;
        private IEmailService _emailService;
        public UserService(IAuthenticationService<int> authService, IDataProvider dataProvider, ILookUpService lookUpService, IEmailService emailService)
        {
            _authenticationService = authService;
            _dataProvider = dataProvider;
            _lookUpService = lookUpService;
            _emailService = emailService;
        }
        public async Task<bool> LogInAsync(UserLoginRequest model)
        {
            bool isSuccessful = false;
            IUserAuthData response = Get(model.Email, model.Password);
            if (response != null)
            {
                isSuccessful = true;
            }
            Claim fullName = new Claim("Authentication", response.Email);
            await _authenticationService.LogInAsync(response, new Claim[] { fullName });
            return isSuccessful;
        }
        public async Task<bool> LogInTest(string email, string password, int id, string roles = null)
        {
            bool isSuccessful = false;
            IUserAuthData response = new UserBase
            {
                Id = id
                ,
                Email = email
                ,
                Role = roles
                ,
                TenantId = "Acme Corp UId"
            };
            Claim fullName = new Claim("CustomClaim", "Sabio Bootcamp");
            await _authenticationService.LogInAsync(response, new Claim[] { fullName });
            return isSuccessful;
        }
        public int Create(UserAddRequest model)
        {
            int id = 0;
            int tokenType = 1; // hard coded for development
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(model.Password.ToString(), salt);
            string procName = "[dbo].[Users_Insert]";
            _dataProvider.ExecuteNonQuery(procName,
                delegate (SqlParameterCollection col)
                {
                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.AddWithValue("@Email", model.Email);
                    col.AddWithValue("@FirstName", model.FirstName);
                    col.AddWithValue("@LastName", model.LastName);
                    if (model.Mi != null)
                    {
                        col.AddWithValue("Mi", model.Mi);
                    }
                    if (model.AvatarUrl != null)
                    {
                        col.AddWithValue("AvatarUrl", model.AvatarUrl);
                    }
                    col.AddWithValue("@Password", hashedPassword);
                    var isConfirmed = false;
                    col.AddWithValue("IsConfirmed", isConfirmed);
                    col.AddWithValue("@StatusId", model.StatusId);
                    col.AddWithValue("@RoleId", model.RoleId);
                    col.Add(idOut);
                }, delegate (SqlParameterCollection returnCol)
                {
                    object oId = returnCol["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                });
            string createdToken = CreateUserToken(id, tokenType);
            _emailService.EmailConfirm(model, createdToken);
            return id;
        }
        private string CreateUserToken(int userId, int tokenType)
        {
            string tokenId = Guid.NewGuid().ToString();
            string procName = "[dbo].[UserTokens_Insert]";
            _dataProvider.ExecuteNonQuery(procName
                , delegate (SqlParameterCollection col)
                {
                    SqlParameter tokenOut = new SqlParameter("@Token", SqlDbType.VarChar);
                    tokenOut.Direction = ParameterDirection.Output;
                    col.AddWithValue("@UserId", userId);
                    col.AddWithValue("@TokenTypeId", tokenType);
                    col.AddWithValue("@Token", tokenId);
                }, delegate (SqlParameterCollection returnCol)
                {
                    tokenId = returnCol["@Token"].Value.ToString();
                });
            return tokenId;
        }
        public Paged<User> GetAllPaged(int pageIndex, int pageSize)
        {
            Paged<User> pagedList = null;
            List<User> list = null;
            int totalCount = 0;
            string procName = "[dbo].[Users_SelectAll]";
            _dataProvider.ExecuteCmd(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                }, delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    User user = MapSingleUser(reader, ref index);
                    if (list == null)
                    {
                        list = new List<User>();
                    }
                    list.Add(user);
                    if (set == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }
                });
            if (list != null)
            {
                pagedList = new Paged<User>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        public User GetById(int id)
        {
            User user = null;
            string procName = "[dbo].[Users_Select_ById]";
            _dataProvider.ExecuteCmd(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                }, delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    user = MapSingleUser(reader, ref index);
                });
            return user;
        }
        public User GetByEmail(string email)
        {
            User user = null;
            string procName = "[dbo].[Users_SelectByEmail]";
            _dataProvider.ExecuteCmd(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Email", email);
                },
                delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    user = MapSingleUser(reader, ref index);
                });
            return user;
        }
        public void UpdateUserStatus(int id, int statusId)
        {
            string procName = "[dbo].[Users_UpdateStatus]";
            _dataProvider.ExecuteNonQuery(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@StatusId", statusId);
                    col.AddWithValue("@Id", id);
                },
                null);
        }
        public void UpdateUserPassword(UserUpdatePasswordRequest model)
        {
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            string hashedPassword = BCrypt.BCryptHelper.HashPassword(model.Password, salt);

            string procName = "[dbo].[Users_Password_Update]";
            _dataProvider.ExecuteNonQuery(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Token", model.Token);
                    col.AddWithValue("@Password", hashedPassword);
                });
        }
        public void UpdateIsConfirmed(int id)
        {
            string procName = "[dbo].[Users_Confirm]";
            _dataProvider.ExecuteNonQuery(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                }, null);
        }
        public int GetByToken(string token)
        {
            int userId = 0;
            int tokenType = 0;
            string procName = "[dbo].[UserTokens_Select_ByToken]";
            _dataProvider.ExecuteCmd(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Token", token);
                }, delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    userId = reader.GetSafeInt32(index++);
                    tokenType = reader.GetSafeInt32(index++);
                });
            return userId;
        }
        public void ConfirmAccount(string tokenId)
        {
            int id = GetByToken(tokenId);
            if (id != 0)
            {
                int statusId = 1;
                UpdateIsConfirmed(id);
                UpdateUserStatus(id, statusId);
                DeleteUserToken(tokenId);
                return;
            }
            else
            {
                throw new Exception("Please request a new link or contact us.");
            }
        }
        public void DeleteUserToken(string token)
        {
            string procName = "[dbo].[UserTokens_Delete_ByToken]";
            _dataProvider.ExecuteNonQuery(procName,
                delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Token", token);
                }, null);
        }
        private User MapSingleUser(IDataReader reader, ref int index)
        {
            User model = new User();

            model.Id = reader.GetSafeInt32(index++);
            model.Email = reader.GetSafeString(index++);
            model.FirstName = reader.GetSafeString(index++);
            model.LastName = reader.GetSafeString(index++);
            model.Mi = reader.GetSafeString(index++);
            model.AvatarUrl = reader.GetSafeString(index++);
            model.IsConfirmed = reader.GetSafeBool(index++);
            model.Status = reader.GetSafeString(index++);
            model.Role = reader.GetSafeString(index++);
            model.DateCreated = reader.GetSafeDateTime(index++);
            model.DateModified = reader.GetSafeDateTime(index++);
            return model;
        }
        private UserBase GetUserAuthData(string email)
        {
            UserBase userAuthData = null;
            string procName = "[dbo].[Users_Select_AuthData]";
            _dataProvider.ExecuteCmd(procName
                , delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Email", email);
                }, delegate (IDataReader reader, short set)
                {
                    userAuthData = new UserBase();
                    int index = 0;
                    userAuthData.Id = reader.GetSafeInt32(index++);
                    userAuthData.Email = reader.GetSafeString(index++);
                    userAuthData.Role = reader.GetSafeString(index++);
                });
            if (userAuthData != null)
            {
                userAuthData.TenantId = "testTenantId1"; //hard coded for development
            }
            return userAuthData;
        }
        private IUserAuthData Get(string email, string password)
        {
            string passwordFromDb = "";
            UserBase user = null;
            bool isConfirmed = false;
            string procName = "[dbo].[Users_SelectPass_ByEmail]";
            _dataProvider.ExecuteCmd(procName
                , delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Email", email);
                }, delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    passwordFromDb = reader.GetSafeString(index++);
                    isConfirmed = reader.GetSafeBool(index++);
                });
            bool isValidCredentials = BCrypt.BCryptHelper.CheckPassword(password, passwordFromDb);
            if (isValidCredentials)
            {
                if (isConfirmed)
                {
                    user = GetUserAuthData(email);
                }
                else
                {
                    throw new Exception("Email Address not confirmed. Please check email address.");
                }
            }
            else
            {
                throw new Exception("Incorrect password");
            }
            return user;
        }
    }
}
