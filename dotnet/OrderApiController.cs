using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.NewFolder;
using Sabio.Models.Requests.Orders;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : BaseApiController
    {

        private IOrderService _orderService = null;
        private IAuthenticationService<int> _authenticationService = null;
        public OrderApiController(IOrderService orderService
            , ILogger<OrderApiController> logger
            , IAuthenticationService<int> authenticationService) : base(logger)
        {
            _orderService = orderService;
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(OrderAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = _authenticationService.GetCurrentUserId();
                int id = _orderService.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int> { Item = id };
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

        [HttpGet("orderhistory")]
        public ActionResult<ItemResponse<Paged<Order>>> Pagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;
            int userId = _authenticationService.GetCurrentUserId();

            try
            {
                Paged<Order> paged = _orderService.GetByUserIdPaginated(userId, pageIndex, pageSize);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records Not Found."));
                }
                else
                {
                    ItemResponse<Paged<Order>> response = new ItemResponse<Paged<Order>> {Item = paged};
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }
    }
}
