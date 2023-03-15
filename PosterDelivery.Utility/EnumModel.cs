using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility {
    public class EnumModel {
        public const string PickupScanStoredProcedure = "dbo.CapturePickupScan";
        public const string GetScanItemWithoutPickup = "dbo.SkipPickupScan";
        public const string DeliveryScanStoredProcedure = "dbo.CaptureDeliveryScan";
        public const string ExcelDataUploadStoredProcedure = "dbo.ExcelDataUpload";
        public const string PickupScanUDT = "PickupScanDetailsType";
        public const string DeliveryScanUDT = "DeliveryScanDetailsType";
        public const string CustomerExcelDataUDT = "CustomerExcelData";
        public const string PickupScanUDT_CustId = "customerId";
        public const string PickupScanUDT_ScannedBy = "ScannedBy";
        public const string PickupScanUDT_ProductId = "ProductId";
        public const string PickupScanUDT_Qty = "Quantity";
        public const string PickupScanUDT_DriverCustomerTrack = "driverCustomerTrackId";
        public const string StoreCountStoredProcedure = "dbo.CaptureStoreCount";
        public const string GenerateInvoiceStoredProcedure = "dbo.GenerateInvoice";
        public const string UpdateInvoiceStoredProcedure = "dbo.UpdateInvoice";
        public const string CustomerExcelDataUDT_CustId = "CustomerId";
        public const string CustomerExcelDataUDT_AccountName = "AccountName";
        public const string CustomerExcelDataUDT_ShippingStreet = "ShippingStreet";
        public const string CustomerExcelDataUDT_ShippingCity = "ShippingCity";
        public const string CustomerExcelDataUDT_ShippingState = "ShippingState";
        public const string CustomerExcelDataUDT_ShippingCode = "ShippingCode";
        public const string CustomerExcelDataUDT_ContactName = "ContactName";
        public const string CustomerExcelDataUDT_ContactPhone = "ContactPhone";
        public const string CustomerExcelDataUDT_ConsignmentOrBuyer = "ConsignmentOrBuyer";
        public const string CustomerExcelDataUDT_DeliveryDay = "DeliveryDay";
        public const string CustomerExcelDataUDT_IsActive = "IsActive";
        public const string CustomerExcelDataUDT_Notes = "Notes";
        public const string CustomerExcelDataUDT_Email = "Email";
        public const string CustomerExcelDataUDT_AlternateContact = "AlternateContact";
        public const string CustomerExcelDataUDT_TotalBoxes = "TotalBoxes";
        public const string ExcelDataUploadStoredProc = "dbo.ExcelDataUpload";
        public const string SkipPickupScanStoredProcedure = "dbo.SkipPickupScan";
        public const string CaptureBuyerFirstVisitStoredProcedure = "dbo.CaptureBuyerFirstVisit";
    }
}
