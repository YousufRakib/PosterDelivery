using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PosterDelivery.Repository.Interface;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;

namespace PosterDelivery.Repository.Repository {
    public class MiscRepository : MiscRepoInterface {
        private readonly IConfiguration? _configuration;
        private readonly string? _connectionString;

        public MiscRepository(IConfiguration configuration) {
            this._configuration = configuration;
            this._connectionString = _configuration.GetConnectionString("PosterDeliveryConnection");
        }
        public async Task<int> UploadUpdateCustomerRepo(IList<CustomerUploadExcel> lstCustomerUploadExcel) {
            int status = 0;
            using (var connection = new SqlConnection(_connectionString)) {
                var dt = new DataTable();
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_CustId, typeof(int));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_AccountName, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_ShippingStreet, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_ShippingCity, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_ShippingState, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_ShippingCode, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_ContactName, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_ContactPhone, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_ConsignmentOrBuyer, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_DeliveryDay, typeof(int));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_IsActive, typeof(int));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_Notes, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_Email, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_AlternateContact, typeof(string));
                dt.Columns.Add(EnumModel.CustomerExcelDataUDT_TotalBoxes, typeof(int));
                foreach (var item in lstCustomerUploadExcel) {
                    dt.Rows.Add(item.CustomerId,item.AccountName,item.ShippingStreet,item.ShippingCity,item.ShippingState,item.ShippingCode,item.ContactName,
                        item.ContactPhone, item.ConsignmentOrBuyer,item.DeliveryDay,item.IsActive,item.IsActive,item.Notes,item.Email,item.AlternateContact,item.TotalBoxes);
                }
                connection.Open();

                status = await connection.ExecuteAsync(EnumModel.ExcelDataUploadStoredProc, new { @customerExcelData = dt.AsTableValuedParameter(EnumModel.CustomerExcelDataUDT) },
                commandType: CommandType.StoredProcedure);
            }
            return status;
        }
    }
}
