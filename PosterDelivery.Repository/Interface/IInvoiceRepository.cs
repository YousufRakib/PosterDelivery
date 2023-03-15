using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Repository.Interface
{
    public interface IInvoiceRepository
    {
        Task<InvoiceModel> GetInvoice(int id);
        Task<IList<InvoiceModel>> GetInvoiceByCust(int customerId);
        Task<int> UploadInvoiceRepository(string invoiceDate, int customerId, double invoiceAmount,
                                          string InvoiceSerialNo, string fileName, string filePath, int userId);
        Task<int?> CaptureStoreInvoice(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel);
        Task<int?> UpdateStoreInvoice(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel);
    }
}
