using PosterDelivery.Repository.Interface;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            this._invoiceRepository = invoiceRepository;
        }

        public Task<InvoiceModel> GetInvoiceById(int Id) {
            return _invoiceRepository.GetInvoice(Id);
        }

        public Task<IList<InvoiceModel>> GetInvoiceService(int customerId)
        {
            return _invoiceRepository.GetInvoiceByCust(customerId);
        }
        public Task<int> UploadInvoiceService(string invoiceDate, int customerId, double invoiceAmount,
                                              string InvoiceSerialNo, string fileName, string filePath, int userId)
        {
			return _invoiceRepository.UploadInvoiceRepository(invoiceDate,customerId, invoiceAmount, InvoiceSerialNo, fileName, filePath, userId);
		}
        public Task<int?> CaptureStoreInvoiceService(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel) {
            return _invoiceRepository.CaptureStoreInvoice(objStoreInvoiceInputModel);
        }
        public Task<int?> UpdateStoreInvoice(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel) {
            return _invoiceRepository.UpdateStoreInvoice(objStoreInvoiceInputModel);
        }
    }
}
