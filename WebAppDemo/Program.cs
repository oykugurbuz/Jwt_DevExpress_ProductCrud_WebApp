using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native;
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Claims;
using System.Security.Claims;
using System.Text;
using WebAppDemo.Models;
using WebAppDemo.Seed;
using WebAppDemo.Services;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using WebAppDemo.Filters;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

//logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
var _configuration = builder.Configuration;

//serilog config

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext() // Konteksten gelen bilgileri loglara otomatik ekler. 
    .Enrich.WithEnvironmentName() // Ortam adýný (Development, Staging, Production vb.) loglara ekler.
    .Enrich.WithMachineName() // Makine adýný loglara ekler. 
    .Enrich.WithThreadId() // Ýþ parçacýðý kimliðini loglara ekler.
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) //dosya sinki günlük döner Logs/log-2025-10-26.txt
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration["ElasticCloud:Uri"])) // Elasticsearch sunucusunun URI'si
    {
        AutoRegisterTemplate = true, // elasticsearche aktatýrken þema otomatik oluþturulsun
        IndexFormat = "webappdemo-logs-{0:yyyy.MM.dd}", // indeks formatý günlük olarak
        FailureCallback = (logEvent, exception) => // hata durumunda çaðrýlacak geri arama logevent: loglanmaya çalýþýlan olay, exception: oluþan hata
        {
            Console.WriteLine($"{logEvent} de Elasticsearch sink hatasý: {exception?.Message}");
        },
        EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                           EmitEventFailureHandling.WriteToFailureSink |
                           EmitEventFailureHandling.RaiseCallback,
        MinimumLogEventLevel = Serilog.Events.LogEventLevel.Information,
        ModifyConnectionSettings = conn =>
    conn.ApiKeyAuthentication(
        new Elasticsearch.Net.ApiKeyAuthenticationCredentials(
            builder.Configuration["ElasticCloud:ApiKey"]
        )
    )
    })
    .CreateLogger();


builder.Host.UseSerilog();
builder.Services.AddScoped<ActivityLogFilter>();
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
//CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .WithOrigins("http://localhost:5269") // izin verilen originler
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});




////Jwt Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false, // Basitse audience doðrulamasýna gerek yok
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer =  _configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured"))),
            //
            RoleClaimType = ClaimTypes.Role
        };

        // Token'ý cookie'den al
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
            //,
            //OnChallenge = context  =>
            //{
            //    context.HandleResponse();
            //    context.Response.Redirect("/Auth/UnauthorizedView");

            //    return Task.CompletedTask;
            //}
        };
    });

builder.Services.AddAuthorization();
//DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//signalr

builder.Services.AddSignalR();
//devexpress
builder.Services.AddDevExpressControls();
builder.Services.AddSession();
builder.Services.AddScoped<IWebDocumentViewerReportResolver, CustomReportProvider>();
//builder.Services.AddScoped<IReportProvider, CustomReportProvider>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


//builder.Services.ConfigureReportingServices(configurator =>
//{
//    //configurator.UseAsyncEngine();

//});


builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<WebAppDemo.Filters.ActivityLogFilter>(); //tüm actionlara loglama filtresi ekleniyor
});

builder.Services.AddHttpClient();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.SeedPermissions(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseDevExpressControls();

app.UseSerilogRequestLogging(); //otomatik request logging
app.UseSession();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<WebAppDemo.Hubs.UserCountHub>("/userCountHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=auth}/{action=login}/{id?}");
//app.MapControllerRoute(
//    name: "WebDocumentViewer",
//    pattern: "/CustomWebDocumentViewer",
//    defaults: new { controller = "Report", action = "GetReportData" });
app.MapControllerRoute(
    name: "WebDocumentViewer",
    pattern: "CustomWebDocumentViewer/{action=Viewer}",  // action parametresi eklenmeli, DevExpress WebDocumentViewer'ýn ihtiyacý var
    defaults: new { controller = "CustomWebDocumentViewer" });
//app.MapReportingEndpoints();
app.Run();
