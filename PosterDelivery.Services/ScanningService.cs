using PosterDelivery.Repository.Interface;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services {
    public class ScanningService : IScanningService {
        private readonly IScanningRepository _scanningRepository;
        public ScanningService(IScanningRepository scanningRepository) {
            this._scanningRepository = scanningRepository;
        }

        public Task<IList<ScanningInvoiceModel>> GetDeliveryInvoice(IList<PickupScanningModel> lstPickupModel) {
            return _scanningRepository.GetDeliveryInvoice(lstPickupModel);
        }

        public Task<IList<ScanningInvoiceModel>> GetScanItemWithoutPickUp(int customerId, int driverCustomerTrackId, int userId) {
            return _scanningRepository.GetScanItemWithoutPickUp(customerId, driverCustomerTrackId, userId);
        }

        public Task<int?> CaptureDeliveryScan(IList<PickupScanningModel> lstPickupModel) {
            return _scanningRepository.CaptureDeliveryScan(lstPickupModel);
        }
        public Task<CaptureStoreCountModel> GetNoScanningSalesService(int userId, int customerId, int driverCustomerTrackId, int pickupCount, int deliveryCount) {
            return _scanningRepository.GetNoScanningSales(userId, customerId, driverCustomerTrackId, pickupCount, deliveryCount);
        }

        public Task<IList<ScanningInvoiceModel>> SkipPickUpScanService(int customerId, int driverCustomerTrackid) {
            return _scanningRepository.SkipPickUpScanRepo(customerId, driverCustomerTrackid);
        }
    }
}
