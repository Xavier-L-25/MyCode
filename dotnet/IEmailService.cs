using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.EmailRequests;
using Sabio.Models.Requests.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services.Interfaces
{
    public interface IEmailService
    {
        Task TestEmail();

        Task AutoEmailReceiver(EmailAddRequest model);

        Task AutoEmailAdmin(EmailAddRequest model);

        Task EmailConfirm(UserAddRequest model, string tokenId);

        public bool EmailCheck(User model, string tokenId);

        Task AutoEmailPasswordReset(User model, string tokenId);

    }
}
