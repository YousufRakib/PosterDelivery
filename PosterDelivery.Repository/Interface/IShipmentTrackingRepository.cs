using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Repository.Interface {
    public interface IShipmentTrackingRepository 
    {
        Task<IList<ShipmentTracking>> ShipmentTrackingData(string shipmentStatus, string deliveryDate);
        Task<IList<ShipmentTracking>> GetProgressStoresForNewBuyers();
        Task<IList<ShipmentTracking>> DateWisePendingStore(string shipmentStatus, string deliveryDate);
        Task<IList<ShipmentTracking>> GetAssignDriverList();
        Task<IList<ShipmentTracking>> DateWiseProgressStore(string shipmentStatus, string deliveryDate);
        Task<IList<ShipmentTracking>> ShipmentTrackingDataForDriver(string shipmentStatus, string deliveryDate);
        Task<IList<Registration>> GetDriverList();
        Task<bool> UpdateDeliveryVisitDate(int driverCustomerTrackId, int ScannedStore);
        Task<ShipmentTracking> GetShipmentTracking(int driverCustomerTrackId);
        Task<string?> AssignDriver(DriverCustomerTrack driverCustomerTrack);
        Task<int?> SaveAssignDriver(DriverCustomerTrack driverCustomerTrack);
        Task<int?> SaveDriverCustomerTrackForNewBuyer(int UserDriverId, int CustomerId);
        Task<int?> CaptureBuyerFirstVisit(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel);
        Task<int?> FinishScan(int customerId, int DriverCustomerTrackId);
        Task<IList<CaptureStoreInvoiceInputModel>> GetInvoiceDataForEdit(int CustomerId, int DriverCustomerTrackId);
    }
}
