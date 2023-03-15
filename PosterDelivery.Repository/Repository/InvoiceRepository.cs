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

namespace PosterDelivery.Repository.Repository
{
	public class InvoiceRepository : IInvoiceRepository
	{
		private readonly IConfiguration? _configuration;
		private readonly string? _connectionString;

		public InvoiceRepository(IConfiguration configuration)
		{
			this._configuration = configuration;
			this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
		}

        public async Task<InvoiceModel> GetInvoice(int id) {
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                return await connection.QuerySingleAsync<InvoiceModel>(DapperQuery.GetInvoiceById, new { Id = id });
            }
        }

        /// <summary>
        /// Repository method to get invoices by customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<IList<InvoiceModel>> GetInvoiceByCust(int customerId)
		{
			IList<InvoiceModel> lstInvoiceModel = new List<InvoiceModel>();

			using (var connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = string.Format(DapperQuery.GetInvoicesByCustomerQuery);
				var data = await connection.QueryAsync<InvoiceModel>(query, new { Id = customerId });
				lstInvoiceModel = (IList<InvoiceModel>)data;
			}

			return lstInvoiceModel;
		}

		public async Task<int> UploadInvoiceRepository(string invoiceDate, int customerId, double invoiceAmount, 
                                                       string InvoiceSerialNo, string fileName, string filePath, int userId)
		{
			int resp = 0;

			using (var connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = string.Format(DapperQuery.InsertInvoiceQuery);
				var status = await connection.QueryAsync<int>(query, new { @CustomerId = customerId, @InvoiceAmount = invoiceAmount, 
                    @InvoiceDate = invoiceDate == null ? Convert.ToDateTime("1/1/9999") : Convert.ToDateTime(invoiceDate),
                    @InvoiceSerialNo = InvoiceSerialNo, @FileName = fileName, @FilePath = filePath, @UserId = userId });
				resp = 1;
			}

			return resp;
		}

        public async Task<int?> CaptureStoreInvoice(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel) {
            int status = 0;
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                status = await connection.ExecuteAsync(EnumModel.GenerateInvoiceStoredProcedure, new { @HeaderId = objStoreInvoiceInputModel.HeaderId, @UserId = objStoreInvoiceInputModel.userId, @CustomerId=objStoreInvoiceInputModel.customerId, @InvoiceAmount=objStoreInvoiceInputModel.InvoiceAmount, 
                         @InvoiceSerialNo=objStoreInvoiceInputModel.InvoiceSerialNum,@ActualInvoiceAmount=objStoreInvoiceInputModel.ActualInvoiceAmt,
                         @TotalInvoiceAmount=objStoreInvoiceInputModel.TotalInvoiceAmt, @InvoiceFileName=objStoreInvoiceInputModel.InvoiceFileName,
                         @InvoiceFilePath=objStoreInvoiceInputModel.InvoiceFilePath,@DeliveryCount=objStoreInvoiceInputModel.DeliveryCount,
                         @PickupCount=objStoreInvoiceInputModel.PickupCount,@DriverCustomerTrackId=objStoreInvoiceInputModel.DriverCustomerTrackId,
                         @SoldQuantity= objStoreInvoiceInputModel.SoldQuantity
                },
                commandType: CommandType.StoredProcedure);
            }
            return status;
        }

        public async Task<int?> UpdateStoreInvoice(CaptureStoreInvoiceInputModel objStoreInvoiceInputModel) {
            int status = 0;
            using (var connection = new SqlConnection(_connectionString)) {
                connection.Open();
                status = await connection.ExecuteAsync(EnumModel.UpdateInvoiceStoredProcedure, new {
                    @UserId = objStoreInvoiceInputModel.userId,
                    @CustomerId = objStoreInvoiceInputModel.customerId,
                    @InvoiceAmount = objStoreInvoiceInputModel.InvoiceAmount,
                    @InvoiceSerialNo = objStoreInvoiceInputModel.InvoiceSerialNum,
                    @InvoiceFileName = objStoreInvoiceInputModel.InvoiceFileName,
                    @InvoiceFilePath = objStoreInvoiceInputModel.InvoiceFilePath,
                    @DeliveryCount = objStoreInvoiceInputModel.DeliveryCount,
                    @PickupCount = objStoreInvoiceInputModel.PickupCount,
                    @DriverCustomerTrackId = objStoreInvoiceInputModel.DriverCustomerTrackId,
                    @SoldQuantity = objStoreInvoiceInputModel.SoldQuantity
                },
                commandType: CommandType.StoredProcedure);
            }
            return status;
        }

    }
}
