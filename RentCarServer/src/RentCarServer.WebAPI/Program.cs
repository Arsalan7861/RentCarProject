using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.RateLimiting;
using RentCarServer.Application;
using RentCarServer.Infrastructure;
using RentCarServer.WebAPI;
using RentCarServer.WebAPI.Controllers;
using RentCarServer.WebAPI.Middlewares;
using RentCarServer.WebAPI.Modules;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimiter(cfr =>
{ // A fixed window limiter divides time into windows of fixed size (here: 1 second), and limits the number of requests allowed within each window.
    cfr.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 100;
        options.QueueLimit = 100;
        options.Window = TimeSpan.FromSeconds(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    cfr.AddFixedWindowLimiter("login-fixed", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    cfr.AddFixedWindowLimiter("forgot-password-fixed", options =>
    {
        options.PermitLimit = 2;
        options.Window = TimeSpan.FromMinutes(5);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    cfr.AddFixedWindowLimiter("reset-password-fixed", options =>
    {
        options.PermitLimit = 3;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    cfr.AddFixedWindowLimiter("check-forgot-password-code-fixed", options =>
    {
        options.PermitLimit = 2;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services // OData (Open Data Protocol) is an open protocol developed by Microsoft that allows you to query and manipulate data using standard HTTP operations (GET, POST, PUT, DELETE), along with powerful URL-based query options.
    .AddControllers()
    .AddOData(opt =>
        opt.Select()
           .Filter()
           .OrderBy()
           .Expand()
           .Count()
           .SetMaxTop(null)
           .AddRouteComponents(
                "odata",
                MainODataController.GetEdmModel()
            )
    );

builder.Services.AddCors();
builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });
builder.Services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();

builder.Services.AddResponseCompression(opt =>
{
    opt.EnableForHttps = true; // Enable response compression for HTTPS requests
});

builder.Services.AddTransient<CheckTokenMiddleware>(); // Middleware to check the validity of the token in the request header

builder.Services.AddHostedService<CheckLoginTokenBackgroundService>(); // Background service to check the validity of login tokens periodically

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection(); // Http -> Https
app.UseCors(x =>
    x.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader()
     .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.UseResponseCompression(); // Response compression, isteklerin daha hızlı yanıtlanmasını sağlar. Örneğin, gzip veya brotli gibi sıkıştırma algoritmalarını kullanır.

app.UseStaticFiles(); // wwwroot klasörü kullanmamızı sağlar.

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(); // Global exception handler, tüm uygulama için geçerli olacak şekilde hata yakalama işlemi yapar.

app.UseMiddleware<CheckTokenMiddleware>(); // Token kontrol middleware'i, isteklerdeki tokenin geçerliliğini kontrol eder.

app.UseRateLimiter(); // Rate limiting middleware, istekleri sınırlamak için kullanılır. Örneğin, belirli bir süre içinde belirli bir sayıda istek yapılmasına izin verir.

app.MapControllers().
    RequireRateLimiting("fixed")
    .RequireAuthorization();

// Minimal API modüllerini ekle
app.MapAuth();
app.MapBranch();
app.MapRole();
app.MapPermission();
app.MapUser();
app.MapCategory();
app.MapProtectionPackage();
app.MapExtra();
app.MapVehicle();
//app.MapSeedData();
app.MapCustomer();
app.MapReservation();
app.MapReservationForm();

app.MapGet("/", () => "Hello").RequireAuthorization();
//await app.CreateFirstUserAsync(); // Uygulama ilk çalıştığında ilk kullanıcıyı oluşturur, bu sayede veritabanında en az bir kullanıcı olur.
await app.CleanRemovedPermissionsFromRoleAsync(); // Uygulama ilk çalıştığında, rollerden kaldırılan izinleri temizler.
await app.RunAsync();
