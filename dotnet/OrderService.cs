using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain.NewFolder;
using Sabio.Models.Requests.Orders;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class OrderService : IOrderService
    {
        IDataProvider _data = null;
        public OrderService(IDataProvider data) 
        { 
            _data = data; 
        }

        public int Add(OrderAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[Orders_Insert]";

            _data.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection inputcollection)
            {
                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                inputcollection.Add(idOut);

                inputcollection.AddWithValue("@Name", model.Name);
                inputcollection.AddWithValue("@Price", model.Price);
                inputcollection.AddWithValue("@VenueId", model.VenueId);
                inputcollection.AddWithValue("@CreatedBy", userId);
            },
            returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object objectId = returnCollection["@Id"].Value;
                int.TryParse(objectId.ToString(), out id);
            }
            );
            return id;
        }

        public Paged<Order> GetByUserIdPaginated(int id, int pageIndex, int pageSize)
        {
            Paged<Order> orderList = null;
            List<Order> orders = null;
            int totalCount = 0;
            string procName = "[dbo].[Orders_Select_ByUser]";

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection param)
            {
                param.AddWithValue("@UserId", id);
                param.AddWithValue("@PageIndex", pageIndex);
                param.AddWithValue("@PageSize", pageSize);
            },
            delegate (IDataReader reader, short set)
            {
                Order order = new Order();

                int startingIndex = 0;
                order.Venue = new Models.Domain.Venues.Venue();
                order.CreatedBy = new Models.Domain.BaseUser();
                order.Venue.LocationInfo = new Models.Domain.Locations.BaseLocation();
                order.Venue.VenueType = new Models.Domain.LookUp();
                order.Venue.CreatedBy = new Models.Domain.BaseUser();
                order.Venue.ModifiedBy = new Models.Domain.BaseUser();

                order.Id = reader.GetSafeInt32(startingIndex++);
                order.Name = reader.GetSafeString(startingIndex++);
                order.Price = reader.GetSafeDouble(startingIndex++);
                order.Venue.Id = reader.GetSafeInt32(startingIndex++);
                order.Venue.Name = reader.GetSafeString(startingIndex++);
                order.Venue.Description = reader.GetSafeString(startingIndex++);
                order.Venue.LocationInfo.Id = reader.GetSafeInt32(startingIndex++);
                order.Venue.VenueType.Id = reader.GetSafeInt32(startingIndex++);
                order.Venue.Url = reader.GetSafeString(startingIndex++);
                order.Venue.CreatedBy.Id = reader.GetSafeInt32(startingIndex++);
                order.Venue.ModifiedBy.Id = reader.GetSafeInt32(startingIndex++);
                order.Venue.DateCreated = reader.GetSafeDateTime(startingIndex++);
                order.Venue.DateModifed = reader.GetSafeDateTime(startingIndex++);    
                order.CreatedBy.Id = reader.GetSafeInt32(startingIndex++);
                order.CreatedBy.FirstName = reader.GetSafeString(startingIndex++);
                order.CreatedBy.LastName = reader.GetSafeString(startingIndex++);
                order.CreatedBy.Mi = reader.GetSafeString(startingIndex++);
                order.CreatedBy.AvatarUrl = reader.GetSafeString(startingIndex++);
                order.DateCreated = reader.GetSafeDateTime(startingIndex++);

                totalCount = reader.GetSafeInt32(startingIndex);

                if (orders == null)
                {
                    orders = new List<Order>();
                }

                orders.Add(order);
            }
            );

            if (orders != null)
            {
                orderList = new Paged<Order>(orders, pageIndex, pageSize, totalCount);
            }

            return orderList;
        }
    }
}
