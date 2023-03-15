using PosterDelivery.Repository.Repository;
using PosterDelivery.Services.Interfaces;
using PosterDelivery.Services;
using PosterDelivery.Repository.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using NLog.Web;
using System.Configuration;
using PosterDelivery.Models;
using PosterDelivery.Middleware;
using PosterDelivery.Repository;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
logger.Debug("init main");

try {

    var builder = WebApplication.CreateBuilder(args);

    #region Add services to the container.

    builder.Services.AddControllersWithViews();
    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    //other classes that need the logger 
    //builder.Services.AddTransient<GenericHelper>();

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(x => x.LoginPath = "/User/Login");

    builder.Services.AddControllers();
    builder.Services.AddHttpClient();
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();



    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddScoped<IInvoiceService, InvoiceService>();
    builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
    builder.Services.AddScoped<ILoggerRepository, LoggerRepository>();
    builder.Services.AddScoped<ILoggerService, LoggerService>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<IShipmentTrackingService, ShipmentTrackingService>();
    builder.Services.AddScoped<IShipmentTrackingRepository, ShipmentTrackingRepository>();
    builder.Services.AddScoped<IHomeService, HomeService>();
    builder.Services.AddScoped<IHomeRepository, HomeRepository>();
    builder.Services.AddScoped<IScanningRepository, ScanningRepository>();
    builder.Services.AddScoped<IScanningService, ScanningService>();

    builder.Services.AddSession(options => {
        int sessionTimeout = int.Parse(builder.Configuration.GetSection("AppSettings:SessionTimeout").Value);
        options.IdleTimeout = TimeSpan.FromSeconds(sessionTimeout);
    });

    #endregion


    var app = builder.Build();

    #region Configure the HTTP request pipeline.
   
    //Middlware to capture exception logs in db
    app.UseMiddleware(typeof(ExeceptionHandlingMiddleware));
    
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment()) {
        app.UseExceptionHandler("/Account/ErrorDisplay");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.Use(async (context, next) => {
        await next();
        if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403 || context.Response.StatusCode == 404 || context.Response.StatusCode == 405 || context.Response.StatusCode == 406 || context.Response.StatusCode == 412 || context.Response.StatusCode == 500 || context.Response.StatusCode == 501 || context.Response.StatusCode == 502) {
            context.Request.Path = "/Account/ErrorDisplay";
            await next();
        }
    });
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseSession();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
 
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=User}/{action=Login}/{id?}");

    app.Run();


    #endregion
} catch (Exception exception) {
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
} finally {
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
