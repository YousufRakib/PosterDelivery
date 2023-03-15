using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Repository {
    public class ScanningRepository : IScanningRepository {

        private readonly IConfiguration? _configuration;
        private readonly string? _connectionString;
        public ScanningRepository(IConfiguration configuration) {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
        }
        public async Task<IList<ScanningInvoiceModel>> GetDeliveryInvoice(IList<PickupScanningModel> lstPickupModel) {
            IList<ScanningInvoiceModel> lstScanningModel = new List<ScanningInvoiceModel>();
            using (var connection = new SqlConnection(_connectionString)) {
                var dt = new DataTable();
                dt.Columns.Add(EnumModel.PickupScanUDT_CustId, typeof(int));
                dt.Columns.Add(EnumModel.PickupScanUDT_DriverCustomerTrack, typeof(int));
                dt.Columns.Add(EnumModel.PickupScanUDT_ScannedBy, typeof(int));
                dt.Columns.Add(EnumModel.PickupScanUDT_ProductId, typeof(int));
                dt.Columns.Add(EnumModel.PickupScanUDT_Qty, typeof(int));
                foreach (var item in lstPickupModel) {
                    dt.Rows.Add(item.customerId, item.DriverCustomerTrackId, item.ScannedBy,item.ProductId, item.Quantity);
                }
                connection.Open();
                
                var data = await connection.QueryAsync<ScanningInvoiceModel>(EnumModel.PickupScanStoredProcedure, new { @pickupScanTable = dt.AsTableValuedParameter(EnumModel.PickupScanUDT) },
                commandType: CommandType.StoredProcedure);

                lstScanningModel = (IList<ScanningInvoiceModel>)data;

            }
                return lstScanningModel;
        }

        public async Task<IList<ScanningInvoiceModel>> GetScanItemWithoutPickUp(int customerId, int driverCustomerTrackId, int userId) 
        {
            IList<ScanningInvoiceModel> lstScanningModel = new List<ScanningInvoiceModel>();
            
            using (var connection = new SqlConnection(_connectionString)) 
            {
                connection.Open();
                var result = await connection.QueryAsync<ScanningInvoiceModel>(EnumModel.GetScanItemWithoutPickup, new { CustomerId = customerId, DriverCustomerTrackId = driverCustomerTrackId, ScannedBy = userId }, commandType: CommandType.StoredProcedure);
                lstScanningModel = (IList<ScanningInvoiceModel>)result;
            }

            return lstScanningModel;
        }

        public async Task<int?> CaptureDeliveryScan(IList<PickupScanningModel> lstPickupModel) {
            int status = 0;
            IList<ScanningInvoiceModel> lstScanningModel = new List<ScanningInvoiceModel>();
            using (var connection = new SqlConnection(_connectionString)) {
                var dt = new DataTable();
                dt.Columns.Add(EnumModel.PickupScanUDT_CustId, typeof(int));
                dt.Columns.Add(EnumModel.PickupScanUDT_DriverCustomerTrack, typeof(int));
                dt.Columns.Add(EnumModel.PickupScanUDT_ScannedBy, typeof(int));
                dt.Columns.Add(EnumModel.PickupScanUDT_ProductId, typeof(int));
                dt.Columns.Add(EnumModel.PickupScanUDT_Qty, typeof(int));
                foreach (var item in lstPickupModel) {
                    dt.Rows.Add(item.customerId, item.DriverCustomerTrackId, item.ScannedBy, item.ProductId, item.Quantity);
                }
                connection.Open();

                status = await connection.ExecuteAsync(EnumModel.DeliveryScanStoredProcedure, new { @deliveryScanTable = dt.AsTableValuedParameter(EnumModel.DeliveryScanUDT) },
                commandType: CommandType.StoredProcedure);
            }
            return status;
        }

        public async Task<CaptureStoreCountModel> GetNoScanningSales(int userId, int customerId, int driverCustomerTrackId, int pickupCount, int deliveryCount) {
            CaptureStoreCountModel objStoreCountModel = new CaptureStoreCountModel();
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                objStoreCountModel = await connection.QuerySingleAsync<CaptureStoreCountModel>(EnumModel.StoreCountStoredProcedure, new { @UserId = userId, @CustomerId = customerId, @DriverCustomerTrackId = driverCustomerTrackId, @PickupCount=pickupCount, @DeliveryCount=deliveryCount }, commandType: CommandType.StoredProcedure);
            }
            return objStoreCountModel;

        }

        public async Task<IList<ScanningInvoiceModel>> SkipPickUpScanRepo(int customerId, int driverCustomerTrackid) {
            IList<ScanningInvoiceModel> lstScanningModel = new List<ScanningInvoiceModel>();
            using (var connection = new SqlConnection(_connectionString)) {
                
                connection.Open();

                var data = await connection.QueryAsync<ScanningInvoiceModel>(EnumModel.SkipPickupScanStoredProcedure, new { @CustomerId = customerId, @DriverCustomerTrackId=driverCustomerTrackid },
                commandType: CommandType.StoredProcedure);

                lstScanningModel = (IList<ScanningInvoiceModel>)data;

            }
            return lstScanningModel;
        }


    }
}
