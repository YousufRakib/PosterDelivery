using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Utility.EntityModel;
using PosterDelivery.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Transactions;

namespace PosterDelivery.Repository.Repository {
    public class CustomerRepository : ICustomerRepository {
        private readonly IConfiguration? _configuration;
        private readonly string? _connectionString;

        public CustomerRepository(IConfiguration configuration) {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
        }

        public async Task<string?> SaveCustomerInformation(Customer customer) {
            string result = "";

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                string query = "";

                if (customer.CustomerId > 0) {
                    query = string.Format(DapperQuery.UpdateCustomerInformation);
                } else {
                    query = string.Format(DapperQuery.SaveCustomerInformation);
                }

                await connection.QueryAsync<int>(query, customer);

                result = "1";
            }

            return result;
        }

        public async Task<IList<Customer>> GetCustomers() {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetCustomers);
                var customers = await connection.QueryAsync<Customer>(query);

                var result = customers.ToList();
                return result;
            }
        }

        public async Task<IList<Customer>> GetCustomerVisitedHistory(int customerId) {
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    //string query = string.Format(DapperQuery.GetCustomerVisitedHistory);
                    var customers = await connection.QueryAsync<Customer>("GetCustomerHistory", new { CustomerId = customerId },
                        commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    var result = customers.ToList();
                    return result;
                }
            } catch (Exception ex) {
                return null;
            }
        }

        public async Task<IList<InvoiceModel>> GetInvoices() {

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetInvoices);
                var invoices = await connection.QueryAsync<InvoiceModel>(query);

                var result = invoices.ToList();
                return result;
            }

        }

        public async Task<Customer> GetCustomerInfo(int customerId) {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetCustomerInfo);
                var customer = await connection.QueryAsync<Customer>(query, new { Id = customerId });

                var result = customer.FirstOrDefault();
                return result;
            }

        }

        public async Task<string?> UpdateCustomerInformation(Customer customer) {
            string result = "";

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.UpdateCustomerInformation);
                await connection.QueryAsync<int>(query, customer);

                result = "1";
            }

            return result;
        }

        public async Task<string?> DeleteCustomerInfo(int customerId) {
            string result = string.Empty;

            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.DeleteCustomerInformation);
                await connection.QueryAsync<int>(query, new { Id = customerId });

                result = "1";
            }

            return result;
        }

        public async Task<int> ChangeCustomerStatus(int customerId, string status) 
        {
            var result = 0;
            int isActive = 0;

            if(status=="Active") 
            {
                isActive= 0;
            }
            else {
                isActive= 1;
            }
            
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.UpdateCustomerStatus);
                result = await connection.ExecuteAsync(query, new { Id = customerId, IsActive = isActive });
            }

            return result;
        }

        public async Task<int?> CustomerUpload(DataTable customer) {
            try {
                int status;
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    status = await connection.ExecuteAsync(EnumModel.ExcelDataUploadStoredProcedure, new { @customerExcelData = customer.AsTableValuedParameter(EnumModel.CustomerExcelDataUDT) },
                    commandType: CommandType.StoredProcedure);
                }
                return status;
            } catch {
                return null;
            }
        }
    }
}