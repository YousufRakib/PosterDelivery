using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IList<InvoiceModel>> GetInvoiceService(int customerId);
        Task<InvoiceModel> GetInvoiceById(int Id);
		Task<int> UploadInvoiceService(string invoiceDate, int customerId, double invoiceAmount,
                                       string InvoiceSerialNo, string fileName, string filePath, int userId);
        Task<int?> CaptureStoreInvoiceService(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel);
        Task<int?> UpdateStoreInvoice(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel);

    }
}
