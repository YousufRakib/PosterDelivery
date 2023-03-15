using PosterDelivery.Repository.Interface;
using PosterDelivery.Repository.Repository;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Services {
    public class ShipmentTrackingService : IShipmentTrackingService 
    {
        private readonly IShipmentTrackingRepository _shipmentTrackingRepository;
        public ShipmentTrackingService(IShipmentTrackingRepository shipmentTrackingRepository) {
            this._shipmentTrackingRepository = shipmentTrackingRepository;
        }

        public async Task<IList<ShipmentTracking>> ShipmentTrackingData(string shipmentStatus, string deliveryDate) {
            return await _shipmentTrackingRepository.ShipmentTrackingData(shipmentStatus, deliveryDate);
        }
        public async Task<IList<ShipmentTracking>> GetProgressStoresForNewBuyers() {
            return await _shipmentTrackingRepository.GetProgressStoresForNewBuyers();
        }

        public async Task<IList<ShipmentTracking>> DateWisePendingStore(string shipmentStatus, string deliveryDate) {
            return await _shipmentTrackingRepository.DateWisePendingStore(shipmentStatus, deliveryDate);
        }

        public async Task<IList<ShipmentTracking>> GetAssignDriverList() {
            return await _shipmentTrackingRepository.GetAssignDriverList();
        }

        public async Task<IList<ShipmentTracking>> DateWiseProgressStore(string shipmentStatus, string deliveryDate) {
            return await _shipmentTrackingRepository.DateWiseProgressStore(shipmentStatus, deliveryDate);
        }

        public async Task<IList<ShipmentTracking>> ShipmentTrackingDataForDriver(string shipmentStatus, string deliveryDate) {
            return await _shipmentTrackingRepository.ShipmentTrackingDataForDriver(shipmentStatus, deliveryDate);
        }

        public async Task<bool> UpdateDeliveryVisitDate(int driverCustomerTrackId, int ScannedStore) {
            return await _shipmentTrackingRepository.UpdateDeliveryVisitDate(driverCustomerTrackId, ScannedStore);
        }

        public Task<ShipmentTracking> GetShipmentTracking(int driverCustomerTrackId) 
        {
            return _shipmentTrackingRepository.GetShipmentTracking(driverCustomerTrackId);
        }

        public async Task<IList<Registration>> GetDriverList() {
            return await _shipmentTrackingRepository.GetDriverList();
        }
        public async Task<string?> AssignDriver(DriverCustomerTrack driverCustomerTrack) {
            return await _shipmentTrackingRepository.AssignDriver(driverCustomerTrack);
        }

        public async Task<int?> SaveAssignDriver(DriverCustomerTrack driverCustomerTrack) {
            return await _shipmentTrackingRepository.SaveAssignDriver(driverCustomerTrack);
        }
        public async Task<int?> SaveDriverCustomerTrackForNewBuyer(int UserDriverId, int CustomerId) {
            return await _shipmentTrackingRepository.SaveDriverCustomerTrackForNewBuyer(UserDriverId, CustomerId);
        }
        public Task<int?> CaptureBuyerFirstVisit(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel) {
            return _shipmentTrackingRepository.CaptureBuyerFirstVisit(objStoreInvoiceInputModel);
        }
        public Task<int?> FinishScan(int customerId, int DriverCustomerTrackId) {
            return _shipmentTrackingRepository.FinishScan(customerId, DriverCustomerTrackId);
        }

        public Task<IList<CaptureStoreInvoiceInputModel>> GetInvoiceDataForEdit(int CustomerId, int DriverCustomerTrackId) {
            return _shipmentTrackingRepository.GetInvoiceDataForEdit(CustomerId, DriverCustomerTrackId);
        }
    }
}
