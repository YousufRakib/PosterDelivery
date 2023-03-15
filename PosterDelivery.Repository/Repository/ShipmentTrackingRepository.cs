using Microsoft.Data.SqlClient;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Utility.EntityModel;
using PosterDelivery.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dapper;
using NLog.Filters;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Diagnostics;

namespace PosterDelivery.Repository.Repository {
    public class ShipmentTrackingRepository : IShipmentTrackingRepository {
        private readonly IConfiguration? _configuration;
        private readonly string? _connectionString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShipmentTrackingRepository(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<IList<ShipmentTracking>> ShipmentTrackingData(string shipmentStatus, string deliveryDate) 
        {
            var deliveryMonth = "";
            var deliveryYear = 0;

            if (deliveryDate != null) {
                string[] deliveryMonthAndYear = deliveryDate.Split("-");

                deliveryMonth = deliveryMonthAndYear[0];
                deliveryYear = Convert.ToInt32(deliveryMonthAndYear[1]);
            }

            List<ShipmentTracking> shipmentTrackingList = new List<ShipmentTracking>();
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    var result = await connection.QueryAsync<ShipmentTracking>("OrderDeliveryTracking", new { ShipmentStatus = shipmentStatus, MonthName = deliveryMonth, Year = deliveryYear }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    shipmentTrackingList = result.ToList();
                }

                return shipmentTrackingList;
            } 
            catch (Exception ex) {
                return null;
            }
        }

        public async Task<IList<ShipmentTracking>> GetProgressStoresForNewBuyers() {
            List<ShipmentTracking> shipmentTrackingList = new List<ShipmentTracking>();
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    string query = string.Format(DapperQuery.GetProgressStoresForNewBuyers);
                    var result = await connection.QueryAsync<ShipmentTracking>(query);
                    shipmentTrackingList = result.ToList();
                }
                return shipmentTrackingList;
            } catch {
                return null;
            }
        }

        public async Task<IList<ShipmentTracking>> DateWisePendingStore(string shipmentStatus, string deliveryDate) {
            List<ShipmentTracking> shipmentTrackingList = new List<ShipmentTracking>();
           
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    var result = await connection.QueryAsync<ShipmentTracking>("DateWiseOrderDeliveryTracking", new { ShipmentStatus = shipmentStatus, DeliveryDate = deliveryDate }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    shipmentTrackingList = result.ToList();
                }

                return shipmentTrackingList;
            } 
            catch (Exception ex) 
            {
                return null;
            }
        }

        public async Task<IList<ShipmentTracking>> GetAssignDriverList() 
        {
            List<ShipmentTracking> shipmentTrackingList = new List<ShipmentTracking>();

            try 
            {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    var result = await connection.QueryAsync<ShipmentTracking>("GetCustomerList", commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    shipmentTrackingList = result.ToList();
                }

                return shipmentTrackingList;
            } 
            catch (Exception ex) 
            {
                return null;
            }
        }

        public async Task<IList<ShipmentTracking>> DateWiseProgressStore(string shipmentStatus, string deliveryDate) {
            List<ShipmentTracking> shipmentTrackingList = new List<ShipmentTracking>();

            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    var result = await connection.QueryAsync<ShipmentTracking>("DateWiseOrderDeliveryTracking", new { ShipmentStatus = shipmentStatus, DeliveryDate = deliveryDate }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    shipmentTrackingList = result.ToList();
                }

                return shipmentTrackingList;
            } catch (Exception ex) {
                return null;
            }
        }

        public async Task<IList<ShipmentTracking>> ShipmentTrackingDataForDriver(string shipmentStatus, string deliveryDateString) {

            var deliveryMonth = "";
            var deliveryYear = 0;

            var deliveryDate = "";

            if(deliveryDateString != null) 
            {
                if(shipmentStatus != "Pending") 
                {
                    string[] deliveryMonthAndYear = deliveryDateString.Split("-");

                    deliveryMonth = deliveryMonthAndYear[0];
                    deliveryYear = Convert.ToInt32(deliveryMonthAndYear[1]);
                } 
                else 
                {
                    deliveryDate = deliveryDateString;
                }
            }

            List<ShipmentTracking> shipmentTrackingList = new List<ShipmentTracking>();
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    var result = await connection.QueryAsync<ShipmentTracking>("OrderDeliveryTrackingForDriver", new { ShipmentStatus = shipmentStatus, DriverId = _httpContextAccessor.HttpContext.Session.GetString("UserId"), MonthName = deliveryMonth, Year = deliveryYear , DeliveryDate = deliveryDate }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    shipmentTrackingList = result.ToList();
                }

                return shipmentTrackingList;
            } catch (Exception ex) {
                return null;
            }
        }

        public async Task<IList<Registration>> GetDriverList() {
           
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();

                string query = string.Format(DapperQuery.GetDriverList);
                var roles = await connection.QueryAsync<Registration>(query);

                var result = roles.ToList();
                return result;
            }

        }

        public async Task<ShipmentTracking> GetShipmentTracking(int driverCustomerTrackId) 
        {
            
            using (var connection = new SqlConnection(_connectionString)) 
            {
                connection.Open();

                string query = string.Format(DapperQuery.GetDriverCustomerTrack);
                var roles = await connection.QueryAsync<ShipmentTracking>(query, new { DriverCustomerTrackId = driverCustomerTrackId});

                var result = roles.FirstOrDefault();
                return result;
            }
        }

        public async Task<bool> UpdateDeliveryVisitDate(int driverCustomerTrackId, int ScannedStore) {

            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    string query = string.Format(DapperQuery.UpdateDeliveryVisitDate);
                    await connection.QueryAsync<int>(query, new { DriverCustomerTrackId = driverCustomerTrackId, ScannedStore = ScannedStore });
                    return true;
                }
            } catch (Exception ex) {
                return false;
            }
        }

        public async Task<string?> AssignDriver(DriverCustomerTrack driverCustomerTrack) {
            string result = "";
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    await connection.QueryAsync<string>("AssignDriver", new {
                        UserDriverId = driverCustomerTrack.DriverId,
                        CustomerId = driverCustomerTrack.CustomerId,
                        ActualProductsPicked = driverCustomerTrack.ActualProductsPicked,
                        ActualProductsShipped = driverCustomerTrack.ActualProductsShipped,
                        IsCompleted = driverCustomerTrack.IsCompleted,
                        ReasonForNotCompletion = driverCustomerTrack.ReasonForNotCompletion,
                        ActualDate = driverCustomerTrack.ActualDateString,
                        IsActive = driverCustomerTrack.IsActive,
                        CreatedByUser = driverCustomerTrack.CreatedByUser
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);

                    result = "1";

                }
            } catch {
                throw;
            }
            return result;
        }

        public async Task<int?> SaveAssignDriver(DriverCustomerTrack driverCustomerTrack) {

            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    var spResult = await connection.ExecuteAsync("AssignDriverAndUpdateDeliveryDay", new {
                        UserDriverId = driverCustomerTrack.DriverId,
                        CustomerId = driverCustomerTrack.CustomerId,
                        ActualDate = driverCustomerTrack.ActualDateString,
                        CreatedByUser = driverCustomerTrack.CreatedByUser,
                        //ActualProductsPicked = driverCustomerTrack.ActualProductsPicked,
                        //ActualProductsShipped = driverCustomerTrack.ActualProductsShipped,
                        //IsCompleted = driverCustomerTrack.IsCompleted,
                        //ReasonForNotCompletion = driverCustomerTrack.ReasonForNotCompletion,
                        //IsActive = driverCustomerTrack.IsActive,
                        //DriverCustomerTrackId = driverCustomerTrack.DriverCustomerTrackId,
                        //DeliveryDay = driverCustomerTrack.DeliveryDay
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    
                    return spResult;
                }
            } catch {
                throw;
            }
        }

        public async Task<int?> SaveDriverCustomerTrackForNewBuyer(int UserDriverId, int CustomerId) {
            int result = 0;
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    result = connection.ExecuteScalar<int>("SaveDriverCustomerTrackForNewBuyer", new {
                        UserDriverId = UserDriverId,
                        CustomerId = CustomerId,
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                }
            } catch {
                return 0;
            }
            return result;
        }

        public async Task<int?> CaptureBuyerFirstVisit(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel) {
            int status = 0;
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                status = await connection.ExecuteAsync(EnumModel.CaptureBuyerFirstVisitStoredProcedure, new {
                    @UserId = objStoreInvoiceInputModel.userId,
                    @CustomerId = objStoreInvoiceInputModel.customerId,
                    @DriverCustomerTrackId = objStoreInvoiceInputModel.DriverCustomerTrackId,
                    @InvoiceAmount = objStoreInvoiceInputModel.InvoiceAmount,
                    @InvoiceSerialNo = objStoreInvoiceInputModel.InvoiceSerialNum,
                    @ActualInvoiceAmount = objStoreInvoiceInputModel.ActualInvoiceAmt,
                    @TotalInvoiceAmount = objStoreInvoiceInputModel.TotalInvoiceAmt,
                    @InvoiceFileName = objStoreInvoiceInputModel.InvoiceFileName,
                    @InvoiceFilePath = objStoreInvoiceInputModel.InvoiceFilePath,
                    @DeliveryCount = objStoreInvoiceInputModel.DeliveryCount,
                    @PickupCount = objStoreInvoiceInputModel.PickupCount,
                    @SoldQuantity = objStoreInvoiceInputModel.SoldQuantity
                },
                commandType: CommandType.StoredProcedure);
            }
            return status;
        }

        public async Task<int?> FinishScan(int customerId, int DriverCustomerTrackId) {
            int result = 0;
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();

                    result =  await connection.ExecuteAsync(DapperQuery.FinishScan, new {
                        CustomerId = customerId,
                        DriverCustomerTrackId = DriverCustomerTrackId
                    }, commandType: CommandType.Text, commandTimeout: 0);
                }
            } catch {
                return 0;
            }
            return result;
        }

        public async Task<IList<CaptureStoreInvoiceInputModel>> GetInvoiceDataForEdit(int CustomerId, int DriverCustomerTrackId) {
            List<CaptureStoreInvoiceInputModel> editInvoiceList = new List<CaptureStoreInvoiceInputModel>();
            try {
                using (var connection = new SqlConnection(_connectionString)) {
                    connection.Open();
                    var result = await connection.QueryAsync<CaptureStoreInvoiceInputModel>("GetInvoiceDataForEdit", new {
                        CustomerId = CustomerId,
                        DriverCustomerTrackId = DriverCustomerTrackId,
                    }, commandType: CommandType.StoredProcedure, commandTimeout: 0); ;
                    editInvoiceList = result.ToList();
                }
                return editInvoiceList;
            } catch {
                return null;
            }
        }
    }
}
