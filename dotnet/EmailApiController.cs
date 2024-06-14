using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Sabio.Web.Controllers;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Sabio.Web.Models.Responses;
using System;
using System.Web;
using Sabio.Models.AppSettings;
using Microsoft.Extensions.Options;
using Sabio.Models.Requests.EmailRequests;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Sabio.Models.Requests.Users;
using Sabio.Models.Domain.Users;
using Sabio.Web.Api.Controllers.Users;

namespace Sabio.Web.Api.Controllers
{

    [Route("api/emails")]
    [ApiController]

    public class EmailApiController : BaseApiController
    {

        private IEmailService _service = null;
        private readonly BrevoApi _brevo = null;

        public EmailApiController(IEmailService service, IOptions<BrevoApi> brevo, ILogger<EmailApiController> logger) : base(logger)
        {
            _service = service;
            _brevo = brevo.Value;
        }

        [HttpPost("test")]
        public ActionResult<SuccessResponse> TestEmail()
        {
            int code = 201;
            BaseResponse response = null;
            try
            {
                _service.TestEmail();
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                response = new ErrorResponse(ex.Message);
                code = 500;
            }
            return StatusCode(code, response);
        }

        [HttpPost("contactus")]
        [AllowAnonymous]
        public ActionResult<SuccessResponse> AutoContactEmails(EmailAddRequest model)
        {
            int code = 201;
            BaseResponse response = null;
            try
            {
                _service.AutoEmailReceiver(model);
                _service.AutoEmailAdmin(model);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                response = new ErrorResponse(ex.Message);
                code = 500;
            }
            return StatusCode(code, response);
        }

        [HttpPost("confirmemail")]
        [AllowAnonymous]
        public ActionResult<SuccessResponse> EmailConfirm(UserAddRequest model, string tokenId)
        {

            int code = 200;
            BaseResponse response = null;
            try
            {

                _service.EmailConfirm(model, tokenId);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {

                code = 500;
                response = new ErrorResponse(ex.Message);

            }
            return StatusCode(code, response);

        }

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public ActionResult<SuccessResponse> ResetPassword(User model)
        {
            int code = 200;
            BaseResponse response = null;
            string tokenId = Guid.NewGuid().ToString();
            try 
            {

                bool emailExists = _service.EmailCheck(model, tokenId);

                if (emailExists)
                {
                    _service.AutoEmailPasswordReset(model, tokenId);
                    response = new SuccessResponse();
                }
                else
                {
                    code = 404;
                    response = new ErrorResponse("Error");
                }

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

    }
}
