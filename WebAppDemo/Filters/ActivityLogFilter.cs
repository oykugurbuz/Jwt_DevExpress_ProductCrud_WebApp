using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using WebAppDemo.Models;

namespace WebAppDemo.Filters
{
    public class ActivityLogFilter : IActionFilter
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Stopwatch _stopwatch;
        private ILogger<ActivityLogFilter> _logger;
        private Serilog.ILogger _serilogger;

        public ActivityLogFilter(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<ActivityLogFilter> logger,Serilog.ILogger serilogger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _serilogger = serilogger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
            context.HttpContext.Items["StartTime"] = DateTime.Now;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var httpContext = _httpContextAccessor.HttpContext;
            var userName = httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.Name : "Anonim";
            var pageUrl = httpContext.Request.Path;
            var actionName = context.ActionDescriptor.RouteValues["action"];
            var controllerName = context.ActionDescriptor.RouteValues["controller"];
            var startTime = (DateTime)httpContext.Items["StartTime"];
            var endTime = DateTime.UtcNow;
            var durationMs = (int)_stopwatch.ElapsedMilliseconds;
            var clientIp = httpContext.Connection.RemoteIpAddress?.ToString();
            var browserInfo = httpContext.Request.Headers["User-Agent"].ToString();

            // DB kaydı
            try
            {
                var logactivity = new ActivityLog
                {
                    UserName = userName,
                    PageUrl = pageUrl,
                    ActionName = actionName,
                    ControllerName = controllerName,
                    StartTime = startTime,
                    EndTime = endTime,
                    DurationMs = durationMs,
                    CliendIp = clientIp,
                    BrowserInfo = browserInfo
                };

                _context.ActivityLogs.Add(logactivity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ActivityLog, Db yazılırken hata oluştu.");
            }

            // Elasticsearch / Kibana için structured logging
            _logger.LogInformation(
                "User {Username} executed {Action} on {Controller} from IP {IP} in {Duration}ms",
                userName,
                actionName,
                controllerName,
                clientIp,
                durationMs
            );

            _serilogger.ForContext("UserName", userName);
            _serilogger.ForContext("PageUrl", pageUrl);
            _serilogger.ForContext("ActionName", actionName);
            _serilogger.ForContext("ControllerName", controllerName);
            _serilogger.ForContext("StartTime", startTime);
            _serilogger.ForContext("EndTime", endTime);
            _serilogger.ForContext("DurationMs", durationMs);
            _serilogger.ForContext("ClientIp", clientIp);


        }
        }
    }


