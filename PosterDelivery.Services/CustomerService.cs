using PosterDelivery.Repository.Interface;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services
{
    public class CustomerService : ICustomerService {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository) {
            this._customerRepository = customerRepository;
        }

        public Task<string?> SaveCustomerInformation(Customer customer) {
            return _customerRepository.SaveCustomerInformation(customer);
        }

        public Task<string?> UpdateCustomerInformation(Customer customer) {
            return _customerRepository.UpdateCustomerInformation(customer);
        }

        public async Task<IList<Customer>> GetCustomers() {
            return await _customerRepository.GetCustomers();
        }

        public async Task<IList<Customer>> GetCustomerVisitedHistory(int customerId) {
            return await _customerRepository.GetCustomerVisitedHistory(customerId);
        }

        public async Task<IList<InvoiceModel>> GetInvoices() {
            return await _customerRepository.GetInvoices();
        }

        public async Task<Customer> GetCustomerInfo(int customerId) {
            return await _customerRepository.GetCustomerInfo(customerId);
        }

        public async Task<string?> DeleteCustomerInfo(int customerId) {
            return await _customerRepository.DeleteCustomerInfo(customerId);
        }

        public async Task<int> ChangeCustomerStatus(int customerId, string status) {
            return await _customerRepository.ChangeCustomerStatus(customerId, status);
        }

        public async Task<int?> CustomerUpload(DataTable customer) {
            return await _customerRepository.CustomerUpload(customer);
        }
    }
}
