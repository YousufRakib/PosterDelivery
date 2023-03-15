using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services.Interfaces {
    public interface IScanningService {
        Task<IList<ScanningInvoiceModel>> GetDeliveryInvoice(IList<PickupScanningModel> lstPickupModel);
        Task<IList<ScanningInvoiceModel>> GetScanItemWithoutPickUp(int customerId, int driverCustomerTrackId, int userId);
        Task<int?> CaptureDeliveryScan(IList<PickupScanningModel> lstPickupModel);
        Task<CaptureStoreCountModel> GetNoScanningSalesService(int userId, int customerId, int driverCustomerTrackId, int pickupCount, int deliveryCount);
        Task<IList<ScanningInvoiceModel>> SkipPickUpScanService(int customerId, int driverCustomerTrackid);
    }
}
