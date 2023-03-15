using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PosterDelivery.Infrastructure;
using PosterDelivery.Models;
using PosterDelivery.Services;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System.Diagnostics;

namespace PosterDelivery.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _appSettings;

        public HomeController(IHomeService homeService, IConfiguration configuration, ILogger<HomeController> logger, IOptions<AppSettings> settings) {
            this._homeService = homeService;
            this._configuration = configuration;
            this._logger = logger;
            this._appSettings = settings.Value;
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public async Task<IActionResult> Index()
        {
            OrderDeliveryCount orderDeliveryCount = new OrderDeliveryCount();
            try 
            {
                orderDeliveryCount = await _homeService.GetOrderDeliveryCount();
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, "Exception Caught");
            }
            return View(orderDeliveryCount);
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetCustomerList(string deliveryType, string isFromDashboard) 
        {
            try 
            {
                var customers = await _homeService.GetCustomers(deliveryType,isFromDashboard);
                if (customers == null || customers.ToList().Count == 0) 
                {
                    customers = new List<Customer>();
                }
                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = customers.ToList() });
            } 
            catch (Exception ex) 
            {
                List<Customer> customers = new List<Customer>();
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(customers);
            }
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        public IActionResult Privacy()
        {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager,Driver")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public IActionResult CustomerOrderDeliveries(string orderType) 
        {
            ViewBag.Title = orderType;
            return View();
        }
    }
}