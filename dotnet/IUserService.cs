using Sabio.Models;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public interface IUserService
    {
        int Create(UserAddRequest model);
        User GetById(int id);
        Paged<User> GetAllPaged(int pageIndex, int pageSize);
        User GetByEmail(string email);
        void UpdateUserStatus(int id, int statusId);
        public void UpdateUserPassword(UserUpdatePasswordRequest model);
        void UpdateIsConfirmed(int id);
        void ConfirmAccount(string token);
        int GetByToken(string token);
        void DeleteUserToken(string token);
        Task<bool> LogInAsync(UserLoginRequest model);
        Task<bool> LogInTest(string email, string password, int id, string roles = null);
    }
}