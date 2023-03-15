using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Repository.Interface {
    public interface IHomeRepository 
    {
        Task<OrderDeliveryCount> GetOrderDeliveryCount();
        Task<IList<Customer>> GetCustomers(string deliveryType, string isFromDashboard);
    }
}
