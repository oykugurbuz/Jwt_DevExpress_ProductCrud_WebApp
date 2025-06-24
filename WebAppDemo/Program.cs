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


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

//logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
var _configuration = builder.Configuration;
//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
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
        };
    });

builder.Services.AddAuthorization();
//DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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


builder.Services.AddControllersWithViews();

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


app.UseSession();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();
 

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
