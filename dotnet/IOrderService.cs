using Sabio.Models;
using Sabio.Models.Domain.NewFolder;
using Sabio.Models.Requests.Orders;
using System.Collections.Generic;

namespace Sabio.Services.Interfaces
{
    public interface IOrderService
    {
        int Add(OrderAddRequest model, int userId);

        public Paged<Order> GetByUserIdPaginated(int id, int pageIndex, int pageSize);
    }
}