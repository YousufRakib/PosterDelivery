using Microsoft.AspNetCore.Mvc;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility.EntityModel;
using PosterDelivery.Infrastructure;
using PosterDelivery.Utility;
using System.Data;
using ExcelDataReader;
using System.Reflection;
using ClosedXML.Excel;

namespace PosterDelivery.Controllers {
    public class CustomerController : Controller {
        private readonly ICustomerService _customerService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, IConfiguration configuration, ILogger<CustomerController> logger) {
            this._customerService = customerService;
            this._configuration = configuration;
            this._logger = logger;
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public IActionResult CustomerList() {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public IActionResult CustomerUpDown() {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetCustomerList() {
            try {
                var customers = await _customerService.GetCustomers();
                if (customers == null || customers.ToList().Count == 0) {
                    customers = new List<Customer>();
                }
                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customers.ToList() });
            } catch (Exception ex) {
                List<Customer> customers = new List<Customer>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(customers);
            }
        }

        public IActionResult CustomerVisitedHistory(int customerId) {
            ViewBag.CustomerId = customerId;
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetCustomerVisitedHistory(int customerId) {
            try {
                var customers = await _customerService.GetCustomerVisitedHistory(customerId);
                if (customers == null || customers.ToList().Count == 0) {
                    customers = new List<Customer>();
                }
                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customers.ToList() });
            } catch (Exception ex) {
                List<Customer> customers = new List<Customer>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(customers);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddCustomer(int CustomerId) {
            Customer customer = new Customer();
            ViewBag.CustomerId = CustomerId;

            try {
                if (CustomerId != 0) {
                    customer = await _customerService.GetCustomerInfo(CustomerId);
                    if (customer != null && customer.ConsignmentOrBuyer == "B") {
                        customer.ConsignmentOrBuyer = "Buyer";
                    } else if (customer != null && customer.ConsignmentOrBuyer == "C") {
                        customer.ConsignmentOrBuyer = "Consignment";
                    }
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
            }
            return View(customer);
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer) {
            try {
                var customerInfo = await _customerService.SaveCustomerInformation(customer);

                if (customerInfo == "1") {
                    return Json(new Confirmation { msg = "Data save successfully!!", output = "Success", returnvalue = customer });
                } else {
                    return Json(new Confirmation { msg = "Data didn't save successfully!!", output = "DataTypeIssue", returnvalue = customer });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = customer });
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int CustomerId) {
            try {
                if (CustomerId > 0) {
                    var customerInfo = await _customerService.DeleteCustomerInfo(CustomerId);

                    if (customerInfo == "1") {
                        return Json(new Confirmation { msg = "Data InActivated successfully!!", output = "Success", returnvalue = CustomerId });
                    } else {
                        return Json(new Confirmation { msg = "Data didn't InActivated successfully!!", output = "DataTypeIssue", returnvalue = CustomerId });
                    }
                } else {
                    return Json(new Confirmation { msg = "Data didn't InActivated successfully!!", output = "DataTypeIssue", returnvalue = CustomerId });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = CustomerId });
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> ChangeCustomerStatus(int customerId, string status) {
            try {
                var customerInfo = await _customerService.ChangeCustomerStatus(customerId, status);

                if (customerInfo > 0) {
                    return Json(new Confirmation { msg = "Status changed successfully!!", output = "Success", returnvalue = null });
                } else {
                    return Json(new Confirmation { msg = "Status not changed!!", output = "Error", returnvalue = null });
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public IActionResult InvoiceList() {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetInvoiceList() {
            try {
                var invoices = await _customerService.GetInvoices();
                if (invoices == null || invoices.ToList().Count == 0) {
                    invoices = new List<InvoiceModel>();
                }
                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = invoices.ToList() });
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
                List<InvoiceModel> invoices = new List<InvoiceModel>();
                return Json(invoices);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult> CustomerUpload(IFormFile file) {
            try {
                if (file == null) {
                    throw new Exception("No file uploaded");
                }

                if (file.Length == 0) {
                    throw new Exception("Invalid file uploaded");
                }

                string[] allowedExtsnions = new string[] { ".xls", ".xlsx" };

                if (!allowedExtsnions.Contains("." + file.FileName.Split(".")[1])) {
                    throw new Exception("Invalid file uploaded");
                }

                IExcelDataReader reader;
                DataSet ds = new DataSet();
                //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var stream = file.OpenReadStream()) {
                    if (file.FileName.Split(".")[1] == "xls") {
                        reader = ExcelReaderFactory.CreateReader(stream);
                        ds = reader.AsDataSet();
                        reader.Close();
                    } else {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        ds = reader.AsDataSet();
                        reader.Close();
                    }
                }
                DataTable dt = new DataTable();
                string Headers = "CustomerId,AccountName,ShippingStreet,ShippingCity,ShippingState,ShippingCode,ContactName,ContactPhone,ConsignmentOrBuyer,DeliveryDay,Notes,Email,AlternateContact,TotalBoxes";
                foreach (string header in Headers.Split(",")) {
                    if (header == "CustomerId" || header == "TotalBoxes") {
                        dt.Columns.Add(header, typeof(int));
                    } else if (header == "DeliveryDay") {
                        dt.Columns.Add(header, typeof(int));
                        dt.Columns.Add("IsActive", typeof(int));
                    } else {
                        dt.Columns.Add(header);
                    }
                }
                List<Customer> lstCustomer = new List<Customer>();
                if (ds != null || ds.Tables.Count > 0) {
                    for (int i = 1; i < ds.Tables[0].Rows.Count; i++) {
                        Customer customer = new Customer();
                        customer.CustomerId = NullToInt(ds.Tables[0].Rows[i][0]);
                        customer.AccountName = NullToString(ds.Tables[0].Rows[i][1]);
                        customer.ShippingStreet = NullToString(ds.Tables[0].Rows[i][2]);
                        customer.ShippingCity = NullToString(ds.Tables[0].Rows[i][3]);
                        customer.ShippingState =  NullToString(ds.Tables[0].Rows[i][4]);
                        customer.ShippingCode = NullToString(ds.Tables[0].Rows[i][5]);
                        customer.ContactName = NullToString(ds.Tables[0].Rows[i][6]);
                        customer.ContactPhone = NullToString(ds.Tables[0].Rows[i][7]);
                        customer.ConsignmentOrBuyer = NullToString(ds.Tables[0].Rows[i][8]).Contains("C") ? "C" : "B";
                        customer.DeliveryDay = NullToInt(ds.Tables[0].Rows[i][9]);
                        customer.Notes = NullToString(ds.Tables[0].Rows[i][10]);
                        customer.Email = NullToString(ds.Tables[0].Rows[i][11]);
                        customer.AlternateContact = NullToString(ds.Tables[0].Rows[i][12]);
                        customer.TotalBoxes = NullToInt(ds.Tables[0].Rows[i][13]);
                        customer.IsActive = "1";
                        lstCustomer.Add(customer);
                    }
                }
                foreach (var item in lstCustomer) {
                    dt.Rows.Add(item.CustomerId, item.AccountName, item.ShippingStreet,
                        item.ShippingCity, item.ShippingState, item.ShippingCode, item.ContactName,
                        item.ContactPhone, item.ConsignmentOrBuyer, item.DeliveryDay, Convert.ToInt16(item.IsActive),
                        item.Notes, item.Email, item.AlternateContact, item.TotalBoxes);
                }
                var result = await _customerService.CustomerUpload(dt);
                if (result > 0) {
                    return Json(new Confirmation { msg = "Customer data updated successfully!!", output = "Success", returnvalue = result });
                } else {
                    return Json(new Confirmation { msg = "Customer data didn't updated successfully!!", output = "Success", returnvalue = result });
                }
            } catch (Exception ex) {
                return Json(new Confirmation { msg = "Something went wrong!! " + ex.Message.ToString(), output = "Exception", returnvalue = null });
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult> ExportCustomer() {
            try {
                var customers = await _customerService.GetCustomers();
                DataTable dt = ToDataTable(customers);
                string[] selectedColumns = new[] { "CustomerId", "AccountName", "ShippingStreet", "ShippingCity", "ShippingState", "ShippingCode", "ContactName", "ContactPhone", "ConsignmentOrBuyer", "DeliveryDay", "Notes", "Email", "AlternateContact", "TotalBoxes" };
                DataTable dtFinal = new DataView(dt).ToTable(false, selectedColumns);
                using (XLWorkbook wb = new XLWorkbook()) {
                    wb.Worksheets.Add(dtFinal);
                    using (MemoryStream stream = new MemoryStream()) {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CustomersList.xlsx");
                    }
                }
            } catch {
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = null });
            }
        }
        public DataTable ToDataTable<T>(IList<T> items) {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props) {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items) {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++) {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public string NullToString(object value) {
            if (value == DBNull.Value || value == null || value.ToString().Trim() == "null" ||
                value.ToString().Trim() == string.Empty) {
                return "";
            }
            return value.ToString().Trim();
        }
        public int NullToInt(object value) {
            if (value == DBNull.Value || value == null || value.ToString().Trim() == "null" ||
                value.ToString().Trim() == string.Empty) {
                return 0;
            }
            return Convert.ToInt32(value);
        }
    }
}