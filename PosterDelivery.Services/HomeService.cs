using PosterDelivery.Repository.Interface;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services {
    public class HomeService : IHomeService
    {
        private readonly IHomeRepository _homeRepository;
        public HomeService(IHomeRepository homeRepository) 
        {
            this._homeRepository = homeRepository;
        }

        public async Task<OrderDeliveryCount> GetOrderDeliveryCount() 
        {
            return await _homeRepository.GetOrderDeliveryCount();
        }

        public async Task<IList<Customer>> GetCustomers(string deliveryType, string isFromDashboard) 
        {
            return await _homeRepository.GetCustomers(deliveryType,isFromDashboard);
        }
    }
}
