using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sabio.Models;
using Sabio.Models.Domain.Users;
using Sabio.Models.Requests.Users;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Core;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sabio.Web.Api.Controllers.Users
{

    [Route("api/users")]
    [ApiController]
    public class UserApiController : BaseApiController
    {

        private IUserService _userService = null;
        private IAuthenticationService<int> _authService = null;
        IOptions<SecurityConfig> _options;

        public UserApiController(IUserService service
            , ILogger<UserApiController> logger
            , IAuthenticationService<int> authService 
            , IOptions<SecurityConfig> options) : base(logger)
        {
            _userService = service;
            _authService = authService;
            _options = options;
        }

        [HttpPost()]
        [AllowAnonymous]
        public ActionResult<ItemResponse<int>> Create(UserAddRequest model)
        {

            ObjectResult result = null;

            try
            {

                int id = _userService.Create(model);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<User>> GetById(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                User user = _userService.GetById(id);

                if (user != null)
                {
                    response = new ItemResponse<User> { Item = user };
                }
                else
                {
                    code = 404;
                    response = new ErrorResponse("User not found");
                }

            }
            catch (Exception ex)
            {

                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());

            }

            return StatusCode(code, response);
        }

        [HttpGet("getall")]
        public ActionResult<ItemResponse<Paged<User>>> GetAllPaged(int pageIndex, int pageSize)
        {

            int code = 200;
            BaseResponse response = null;

            try
            {

                Paged<User> pagedList = _userService.GetAllPaged(pageIndex, pageSize);

                if (pagedList == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<User>> { Item = pagedList };
                }

            }
            catch (Exception ex)
            {

                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(code, response);
        }
        
        [HttpGet("{email}")]
        public ActionResult<ItemResponse<User>> GetByEmail(string email)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {

                User user = _userService.GetByEmail(email);

                if (user == null)
                {
                    code = 404;
                    response = new ErrorResponse("Resource not found");
                }
                else
                {
                    response = new ItemResponse<User>() { Item = user };
                }

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPut("status")]
        public ActionResult UpdateUserStatus(int id, int statusId)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {

                _userService.UpdateUserStatus(id, statusId);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPut("updatepassword")]
        [AllowAnonymous]
        public ActionResult UpdateUserPassword(UserUpdatePasswordRequest model) 
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _userService.UpdateUserPassword(model);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPut("confirmstatus/{id:int}")]
        public ActionResult UserConfirm(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _userService.UpdateIsConfirmed(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPut("confirmuser")]
        [AllowAnonymous]
        public ActionResult ConfirmAccount(string tokenId) 
        {

            int code = 200;
            BaseResponse response = null;

            try
            {
                _userService.ConfirmAccount(tokenId);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                
            }
            return StatusCode(code, response);
        }

        [HttpDelete("usertokens/delete/{token}")]
        public ActionResult DeleteUserToken(string token)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _userService.DeleteUserToken(token);
                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult LogIn(UserLoginRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {

                Task<bool> isSuccessful = _userService.LogInAsync(model);
                if (isSuccessful.Result == true)
                {
                    response = new ItemResponse<object>() { Item = _options };
                }
                else 
                {
                    code = 401;
                    response = new ErrorResponse(isSuccessful.Exception.Message);
                }

            }
            catch (Exception ex) 
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("logout")]
        public async Task<ActionResult<SuccessResponse>> LogoutAsync()
        {
            await _authService.LogOutAsync();
            SuccessResponse response = new SuccessResponse();
            return Ok200(response);
        }

        [HttpGet("current")]
        public ActionResult<ItemResponse<IUserAuthData>> GetCurrent()
        {
            IUserAuthData user = _authService.GetCurrentUser();
            ItemResponse<IUserAuthData> response = new ItemResponse<IUserAuthData>();
            response.Item = user;

            return Ok200(response);
        }

    }
}
