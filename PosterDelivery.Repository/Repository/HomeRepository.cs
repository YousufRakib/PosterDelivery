using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Utility.EntityModel;
using PosterDelivery.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace PosterDelivery.Repository.Repository {
    public class HomeRepository : IHomeRepository
    {
        private readonly IConfiguration? _configuration;
        private readonly string? _connectionString;

        public HomeRepository(IConfiguration configuration) 
        {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
        }

        public async Task<OrderDeliveryCount> GetOrderDeliveryCount() 
        {
            OrderDeliveryCount orderDeliveryCount = new OrderDeliveryCount();
            try 
            {
                using (var connection = new SqlConnection(_connectionString)) 
                {
                    connection.Open();

                    var result = await connection.QueryAsync<OrderDeliveryCount>("OrderDeliveryCount", commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    orderDeliveryCount = result.FirstOrDefault();
                }                return orderDeliveryCount;
            } 
            catch (Exception ex) {
                return null;
            }
        }

        public async Task<IList<Customer>> GetCustomers(string deliveryType, string isFromDashboard) 
        {
            try {
                using (var connection = new SqlConnection(_connectionString)) 
                {
                    connection.Open();

                    var customers = await connection.QueryAsync<Customer>("GetCustomerOrders", new { DeliveryType = deliveryType, IsFromDashboard = isFromDashboard }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    var result = customers.ToList();
                    return result;
                }
            } 
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
