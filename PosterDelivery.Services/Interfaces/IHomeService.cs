using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services.Interfaces {
    public interface IHomeService
    {
        Task<OrderDeliveryCount> GetOrderDeliveryCount();
        Task<IList<Customer>> GetCustomers(string deliveryType, string isFromDashboard);
    }
}
