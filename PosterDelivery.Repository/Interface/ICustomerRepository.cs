using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Repository.Interface
{
    public interface ICustomerRepository
    {
        Task<string?> SaveCustomerInformation(Customer customer);
        Task<string?> UpdateCustomerInformation(Customer customer);
        Task<IList<Customer>> GetCustomers();
        Task<IList<InvoiceModel>> GetInvoices();
        Task<Customer> GetCustomerInfo(int customerId);
        Task<string?> DeleteCustomerInfo(int customerId);
        Task<int> ChangeCustomerStatus(int customerId, string status);
        Task<IList<Customer>> GetCustomerVisitedHistory(int customerId);
        Task<int?> CustomerUpload(DataTable customer);
    }
}
