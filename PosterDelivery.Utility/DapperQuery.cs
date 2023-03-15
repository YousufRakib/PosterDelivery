using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosterDelivery.Utility
{
    public static class DapperQuery
    {
        public const string LoginQuery = "Select u.UserId, u.UserName, u.EmailId, (Select RoleName From Roles WITH (NOLOCK) where RoleId=r.RoleId) as RoleName from Users u WITH (NOLOCK) LEFT JOIN UserRoles r WITH (NOLOCK) ON u.UserId = r.UserId Where (u.EmailId=@EmailId Or u.UserName=@EmailId) and u.Password=@Password and u.IsActive=1";
        public const string GetUserRoleQuery = "Select * from Roles WITH (NOLOCK) Where IsActive=1";
        public const string SaveUserInformation = "Insert Into Users (FirstName,MiddleName,LastName,EmailId,UserName,Password,IPAddress,CreatedBy,CreatedDate,IsActive) Values (@FirstName,@MiddleName,@LastName,@EmailId,@UserName,@Password,@IPAddress,@CreatedBy, CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ,1)";
        public const string SaveUserRole = "Insert into UserRoles (UserId,RoleId,DateCreated,IsActive) Values(@UserId,@RoleId, CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ,1)";
        public const string SelectLast1User = "Select * from Users WITH (NOLOCK) Where EmailId=@EmailId or UserName=@UserName";
        public const string SaveCustomerInformation = "Insert Into Customers (AccountName,ShippingStreet,ShippingState,ShippingCity,ShippingCode,ContactName,ContactPhone,AlternateContact,ConsignmentOrBuyer,DeliveryDay,TotalBoxes,Notes,Email,CreatedBy,CreatedDate,IsActive) Values (@AccountName,@ShippingStreet,@ShippingState,@ShippingCity,@ShippingCode,@ContactName,@ContactPhone,@AlternateContact,@ConsignmentOrBuyer,@DeliveryDay,@TotalBoxes,@Notes,@Email,@CreatedBy, CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ,1)";
        public const string UpdateCustomerInformation = "Update Customers Set AccountName=@AccountName, ShippingStreet=@ShippingStreet,ShippingState=@ShippingState,ShippingCity=@ShippingCity,ShippingCode=@ShippingCode,ContactName=@ContactName,ContactPhone=@ContactPhone,AlternateContact=@AlternateContact,DeliveryDay=@DeliveryDay,TotalBoxes=@TotalBoxes,Email=@Email,Notes=@Notes,UpdatedBy=@UpdatedBy,UpdatedDate= CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime)  where CustomerId=@CustomerId";
        public const string DeleteCustomerInformation = "Update Customers Set IsActive = 0 Where CustomerId=@Id";
        public const string UpdateCustomerStatus = "Update Customers Set IsActive = @IsActive Where CustomerId=@Id";
        public const string SaveProductInformation = "Insert Into Products (ProductSerial,ProductName,CategoryId,ProductPrice,IsBarcodeGenerated,BarcodeImageName,BarcodeImagePath,ProductImageName,ProductImagePath,CreatedBy,DateCreated,IsActive) Values (@ProductSerial,@ProductName,@CategoryId,@ProductPrice,@IsBarcodeGenerated,@BarcodeImageName,@BarcodeImagePath,@ProductImageName,@ProductImagePath,@CreatedBy, CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ,@IsActive)";
        public const string UpdateProductInformation = "Update Products Set ProductSerial=@ProductSerial, ProductName=@ProductName,CategoryId=@CategoryId,ProductPrice=@ProductPrice,ProductImageName=@ProductImageName,ProductImagePath=@ProductImagePath,IsActive=@IsActive,ModifiedBy=@ModifiedBy,DateModified= CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime)  where ProductId=@ProductId";
        public const string GetAllProductCategory = "Select CategoryId,CategoryName,Case When IsActive=1 Then 'Active' When IsActive='0' Then 'InActive' Else '' End as IsActive from ProductCategory WITH (NOLOCK) Where IsActive = 1";
        public const string GetProductCategory = "Select * from ProductCategory WITH (NOLOCK) Where CategoryId=@CategoryId";
        public const string SaveProductCategoryInformation = "Insert Into ProductCategory (CategoryName,ParentCategoryId,CreatedBy,DateCreated,IsActive) Values (@CategoryName,@ParentCategoryId,@CreatedBy, CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ,1)";
        public const string UpdateProductCategoryInformation = "Update ProductCategory Set CategoryName=@CategoryName, ParentCategoryId=@ParentCategoryId,ModifiedBy=@ModifiedBy,DateModified= CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime)  where CategoryId=@CategoryId";
        //public const string UpdateBoxesNumber = "Update DriverCustomerTrack Set NumberOfBoxes=@NumberOfBoxes where DriverCustomerTrackId=@DriverCustomerTrackId";
        public const string GetUsers = "Select distinct U.UserId, U.FirstName, U.LastName, U.EmailId, U.UserName, Case When U.IsActive=1 Then 'Active' When U.IsActive='0' Then 'InActive' Else '' End as IsActive, (Select STRING_AGG ( ISNULL(R.RoleName,''), ', ') AS UserRole from Roles as R WITH (NOLOCK) Inner Join UserRoles as UR WITH (NOLOCK) on R.RoleId=UR.RoleId where UR.UserId=U.UserId) as UserRole from Users  as U WITH (NOLOCK) Inner Join UserRoles as URM on U.UserId=URM.UserId";        
        public const string UpdateUser = "Update Users Set FirstName=@FirstName,LastName=@LastName,EmailId=@EmailId,UserName=@UserName,Password=@Password,LastLoginDateTime= CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ,IPAddress=@IPAddress,UpdatedDate= CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ,IsActive=@IsActive,MiddleName=@MiddleName Where UserId=@UserId";
        public const string DeleteUsers = "Update Users Set IsActive = 0 Where UserId=@Id";
        public const string GetDrivers = "Select distinct U.UserId, U.FirstName, U.LastName, U.EmailId, U.UserName, Case When U.IsActive=1 Then 'Active' When U.IsActive='0' Then 'InActive' Else '' End as IsActive, (Select STRING_AGG ( ISNULL(R.RoleName,''), ', ') AS UserRole from Roles as R WITH (NOLOCK) Inner Join UserRoles as UR WITH (NOLOCK) on R.RoleId=UR.RoleId where UR.UserId=U.UserId And R.RoleId = 2) as UserRole from Users  as U WITH (NOLOCK) Inner Join UserRoles as URM on U.UserId=URM.UserId where URM.RoleId = 2";
        public const string GetCustomers = "Select C.CustomerId,C.AccountName,C.ShippingStreet,C.ShippingCity,C.ShippingState,C.ShippingCode,C.ContactName,C.ContactPhone, CASE WHEN C.ConsignmentOrBuyer='B' THEN 'Buyer' WHEN C.ConsignmentOrBuyer='C' THEN 'Consignment' Else '' END as ConsignmentOrBuyer, C.DeliveryDay,(Select Case When EXISTS(select top 1 ActualProductsShipped  From dbo.DriverCustomerTrack with(nolock) where CustomerId = C.CustomerId and Iscompleted=1 order by DriverCustomerTrackId desc) Then (select top 1 ActualProductsShipped From dbo.DriverCustomerTrack with(nolock) where CustomerId = C.CustomerId and Iscompleted=1 order by DriverCustomerTrackId desc) Else (Select TotalBoxes From Customers where CustomerId = C.CustomerId And IsActive = 1) End) As TotalBoxes,Case When C.DeliveryDay = 0 Or C.DeliveryDay IS NULL then '' else FORMAT(DATEFROMPARTS(Year( CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ),Month( CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ),CAST(C.DeliveryDay as varchar(50))), 'dd MMM, yyyy') End as DeliveryDate, ISNULL((Select  Top 1 Case When DCT.ActualDate IS NULL then '' else FORMAT(DCT.ActualDate, 'dd MMM, yyyy') End as LastVisitedDateString from DriverCustomerTrack as DCT  where DCT.CustomerId=C.CustomerId and DCT.IsCompleted=1 Order By DCT.ActualDate DESC),'Date ') as LastVisitedDateString, Case When C.IsActive=1 Then 'Active' When C.IsActive=0 Then 'InActive' Else '' End as IsActive, (SUBSTRING(C.Notes, 0, 20)+'..') as Notes, Email, AlternateContact from Customers As C WITH (NOLOCK)";
        public const string GetCustomerVisitedHistory = "Select Top 3 C.CustomerId,C.AccountName,C.ShippingStreet,C.ShippingCity,C.ShippingState,C.ShippingCode,C.ContactName,C.ContactPhone, CASE WHEN C.ConsignmentOrBuyer='B' THEN 'Buyer' WHEN C.ConsignmentOrBuyer='C' THEN 'Consignment' Else '' END as ConsignmentOrBuyer, C.DeliveryDay, Case When C.DeliveryDay = 0 Or C.DeliveryDay IS NULL then '' else FORMAT(DATEFROMPARTS(Year( CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ),Month( CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) ),CAST(C.DeliveryDay as varchar(50))), 'dd MMM, yyyy') End as DeliveryDate,Case When DCT.ActualDate IS NULL then '' else FORMAT(DCT.ActualDate, 'dd MMM, yyyy') End as LastVisitedDateString, Case When C.IsActive=1 Then 'Active' When C.IsActive=0 Then 'InActive' Else '' End as IsActive,C.Notes from DriverCustomerTrack as DCT WITH (NOLOCK) Inner JOIN  Customers As C WITH (NOLOCK) on C.CustomerId = DCT.CustomerId Where DCT.CustomerId = @Id And DCT.IsCompleted=1 Order By DCT.ActualDate DESC";

//MDK 1/23/23 added columns ,IsBarcodeGenerated,BarcodeImageName,BarcodeImagePath **need sprocs here!
        public const string GetProducts = "Select P.ProductId,P.ProductSerial,P.ProductName,PC.CategoryName,P.ProductPrice,FORMAT (P.DateCreated, 'dd MMM, yyyy') as DateCreatedString,Case When P.IsActive=1 Then 'Active' When P.IsActive='0' Then 'InActive' Else '' End as IsActive,IsBarcodeGenerated,BarcodeImageName,BarcodeImagePath from Products as P WITH (NOLOCK) Inner Join ProductCategory as PC WITH (NOLOCK) on P.CategoryId = PC.CategoryId Where P.IsActive = 1 Order By P.ProductId desc";

        public const string GetInvoices = "Select C.AccountName, IH.InvoiceHeaderId, IH.ActualInvoiceAmount, IH.InvoiceSerialNo, FORMAT (IH.InvoiceDate, 'dd MMM, yyyy') as InvoiceDateWithFormat, IH.Tax, IH.Discount, IH.InvoiceFileName, IH.InvoiceFilePath from dbo.InvoiceHeader as IH WITH (NOLOCK) Left Join Customers as C WITH (NOLOCK) On IH.CustomerId=C.CustomerId Where IH.IsActive=1 order by InvoiceHeaderId desc";
        public const string GetDriverList = "Select * from Users U WITH (NOLOCK) JOIN UserRoles UR ON UR.UserId = U.UserId JOIN Roles R ON R.RoleId = UR.RoleId WHERE R.RoleName = 'Driver' AND U.IsActive = 1";
        public const string GetDriverCustomerTrack = "Select DCT.DriverCustomerTrackId, C.AccountName from DriverCustomerTrack as DCT WITH (NOLOCK) INNER JOIN Customers as C WITH (NOLOCK) On DCT.CustomerId=C.CustomerId Where DCT.IsActive = 1 And DCT.DriverCustomerTrackId=@DriverCustomerTrackId";
        public const string GetCustomerInfo = "Select * from Customers WITH (NOLOCK) Where CustomerId=@Id";
        public const string GetProductInfo = "Select * from Products WITH (NOLOCK) Where ProductId=@Id";
        public const string GetProductCategories = "Select * from ProductCategory WITH (NOLOCK) Where IsActive=1";
        public const string GetInvoicesByCustomerQuery = "Select C.AccountName, IH.InvoiceHeaderId, IH.ActualInvoiceAmount, IH.InvoiceSerialNo, FORMAT (IH.InvoiceDate, 'dd MMM, yyyy') as InvoiceDateWithFormat, IH.Tax, IH.Discount, IH.InvoiceFileName, IH.InvoiceFilePath from dbo.InvoiceHeader as IH WITH (NOLOCK) Left Join Customers as C WITH (NOLOCK) On IH.CustomerId=C.CustomerId Where IH.IsActive=1 and IH.CustomerId=@Id order by InvoiceHeaderId desc";
        public const string GetInvoiceById = "Select InvoiceHeaderId, ActualInvoiceAmount, InvoiceDate, Tax, Discount, InvoiceFileName, InvoiceFilePath from dbo.InvoiceHeader WITH (NOLOCK) Where InvoiceHeaderId=@Id";
        public const string InsertInvoiceQuery = "Insert into InvoiceHeader (CustomerId, ActualInvoiceAmount, InvoiceDate, InvoiceSerialNo, InvoiceFileName, InvoiceFilePath, UserId, IsActive) Values (@CustomerId, @InvoiceAmount, @InvoiceDate, @InvoiceSerialNo, @FileName, @FilePath, @UserId, 1)";
        public const string InsertExceptionLogs = "Insert into ExceptionLogs (Message, Type, StackTrace, StatusCode, Source, UserId) Values (@Message,@Type,@StackTrace, @StatusCode,@Source,@UserId)";
        public const string GetProductView = "Select * from vwProducts WITH (NOLOCK)";

        public const string GetUsersById = @"SELECT DISTINCT u.UserId,
                                                   u.FirstName,
                                                   u.LastName,
                                                   u.EmailId,
                                                   u.UserName,
                                                   u.Password,
                                                   u.LastLoginDateTime,
                                                   u.IPAddress,
                                                   u.CreatedDate,
                                                   u.CreatedBy,
                                                   u.UpdatedDate,
                                                   u.UpdatedBy,
                                                   u.IsActive,
                                                   u.MiddleName,
                                                   (
                                                       SELECT STRING_AGG(ISNULL(R.RoleName, ''), ', ') AS UserRole
                                                       FROM Roles AS R WITH (NOLOCK)
                                                       INNER JOIN UserRoles AS UR WITH (NOLOCK) ON R.RoleId = UR.RoleId
                                                       WHERE UR.UserId = u.UserId
                                                   ) AS UserRole
                                            FROM Users u WITH (NOLOCK)
                                            JOIN dbo.UserRoles ur ON ur.UserId = u.UserId
                                            WHERE u.UserId = @Id";

        public const string UpdateLastLogin = "UPDATE dbo.Users SET LastLoginDateTime = @loginDate WHERE UserId = @userID";
        public const string UpdateDeliveryVisitDate = "Update DriverCustomerTrack Set DriverVisitDate= CAST(SYSDATETIMEOFFSET() AT TIME ZONE 'Pacific Standard Time' AS datetime) , ScannedStore=@ScannedStore where DriverCustomerTrackId=@DriverCustomerTrackId";
        public const string GetProgressStoresForNewBuyers = "Select * From Customers C Left Join DriverCustomerTrack DCT ON DCT.CustomerId = C.CustomerId Left Join Users U ON U.UserId = DCT.UserDriverId Left Join CustomerDeliveryHeader CDH ON CDH.CustomerId = DCT.CustomerId AND CDH.DriverCustomerTrackId = CDH.DriverCustomerTrackId AND (IsCompleted = 0 OR IsCompleted = null) Where (ConsignmentOrBuyer = 'B' OR ConsignmentOrBuyer = 'Buyer') AND FirstVisitDate IS NULL AND C.IsActive = 1";
        public const string FinishScan = "Update DriverCustomerTrack Set IsCompleted = 1 Where CustomerId = @CustomerId And DriverCustomerTrackId = @DriverCustomerTrackId";
    }
}
