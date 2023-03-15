using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PosterDelivery.Services;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Utility;
using PosterDelivery.Utility.EntityModel;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using PosterDelivery.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace PosterDelivery.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
		private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, IConfiguration configuration, ILogger<UserController> logger)
        {
            this._userService = userService;
            this._configuration = configuration;
            this._logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] Login login)
        {
            try
            {
                var userInfo = await _userService.Login(login);
                if (userInfo.Count() > 0)
                {
                    HttpContext.Session.SetString("UserId", userInfo.FirstOrDefault().UserId.ToString());
                    HttpContext.Session.SetString("UserName", userInfo.FirstOrDefault().UserName.ToString());
                    HttpContext.Session.SetString("Email", userInfo.FirstOrDefault().EmailId.ToString());
                    var userRoles = "";
                    for (var item = 0; item < userInfo.Count; item++)
                    {
                        if (item == 0)
                        {
                            userRoles = userInfo[item].RoleName;
                        }
                        else
                        {
                            userRoles = userRoles + "," + userInfo[item].RoleName;
                        }
                    }
                    if (!userRoles.IsNullOrEmpty())
                    {
                        HttpContext.Session.SetString("Roles", userRoles);
                    }
                    var claims = new List<Claim>() {
                        new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userInfo.FirstOrDefault().UserId)),
                        new Claim("UserName", userInfo.FirstOrDefault().UserName),
                        new Claim(ClaimTypes.Role, userRoles),
                        new Claim(ClaimTypes.Email, userInfo.FirstOrDefault().EmailId.ToString())
                    };

                    //Initialize a new instance of the ClaimsIdentity with the claims and authentication scheme    
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //Initialize a new instance of the ClaimsPrincipal with ClaimsIdentity    
                    var principal = new ClaimsPrincipal(identity);
                    //SignInAsync is a Extension method for Sign in a principal for the specified scheme.    
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties());

                    // record last login time
                    await _userService.SaveLastLogin(userInfo.FirstOrDefault().UserId, DateTime.Now);

                    _logger.LogError("User Logged In.");
                    return Json(new Confirmation { msg = "Login Successfully!!", output = "Success", returnvalue = userRoles });
                }
                else
                {
                    return Json(new Confirmation { msg = "User not exist!!", output = "NotFound", returnvalue = login });
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, "Exception Caught");
                throw;
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public  IActionResult UserList()
        {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUserList()
        {
            try
            {
                var users = await _userService.GetUsers("AdminOrManager");
                if (users == null || users.ToList().Count == 0)
                {
                    users = new List<Registration>();
                }
                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = users.ToList() });
            }
            catch (Exception ex)
            {
				_logger.LogError(ex.Message, "Exception Caught");
				List<Registration> customers = new List<Registration>();
                return Json(customers);
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public IActionResult DriverList()
        {
            return View();
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetDriverList()
        {
            try
            {
                var users = await _userService.GetUsers("Driver");
                if (users == null || users.ToList().Count == 0)
                {
                    users = new List<Registration>();
                }
                return Json(new Confirmation { msg = "Data Found!!", output = "Success", returnvalue = users.ToList() });
            }
            catch (Exception ex)
            {
				_logger.LogError(ex.Message, "Exception Caught");
				List<Registration> customers = new List<Registration>();
                return Json(customers);
            }
        }

        public async Task<IActionResult> Profile() {
            // get current user
            ClaimsPrincipal currentUser = this.User;
            var currentUserID = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userService.GetUserById(currentUserID);

            //_userService.GetUserRole

            return View(user);
        }

        //[CustomAuth(Roles = "Admin,Manager")]
        //[HttpGet]
        //public async Task<IActionResult> AddUser() {
        //    IList<Role> roles = new List<Role>();
        //    Registration registration = new Registration();
        //    try {
        //        registration.UserId = 0;
        //        roles = await _userService.GetUserRole();
        //    } catch (Exception ex) {
        //        _logger.LogError(ex.Message, "Exception Caught");
        //    }
        //    ViewBag.Roles = roles;
        //    return View(registration);
        //}

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<ActionResult> AddUser([FromBody] Registration registration)
        {
            try
            {
                var userInfo = await _userService.SaveUserInformation(registration);

                if (userInfo == "1")
                {
                    return Json(new Confirmation { msg = "Data save successfully!!", output = "Success", returnvalue = registration });
                }
                else if (userInfo == "2")
                {
                    return Json(new Confirmation { msg = "This user already exist!!", output = "ExistingUser", returnvalue = registration });
                }
                else
                {
                    return Json(new Confirmation { msg = "Data didn't save successfully!!", output = "DataTypeIssue", returnvalue = registration });
                }
            }
            catch (Exception ex)
            {
				_logger.LogError(ex.Message, "Exception Caught");
				return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = registration });
            }
        }

        [CustomAuth(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddUser(int UserId) {
            Registration user = new Registration();
            ViewBag.UserId = UserId;

            try {
                ViewBag.Roles = await _userService.GetUserRole();
                if (UserId != 0) {
                    user = await _userService.GetUserById(UserId);
                }
            } catch (Exception ex) {
                _logger.LogError(ex.Message, "Exception Caught");
            }
            return View(user);
        }

        [CustomAuth(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int UserId) {
            try 
            {
                if (UserId > 0)
                {
                    var userInfo = await _userService.DeleteUser(UserId);

                    if (userInfo == "1") 
                    {
                        return Json(new Confirmation { msg = "Data InActivated successfully!!", output = "Success", returnvalue = UserId });
                    } else {
                        return Json(new Confirmation { msg = "Data didn't InActivated successfully!!", output = "DataTypeIssue", returnvalue = UserId });
                    }
                }
                else 
                {
                    return Json(new Confirmation { msg = "Data didn't InActivated successfully!!", output = "DataTypeIssue", returnvalue = UserId });
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, "Exception Caught");
                return Json(new Confirmation { msg = "Something went wrong!!", output = "Exception", returnvalue = UserId });
            }
        }
    }
}
