using Microsoft.AspNetCore.Mvc;

namespace DownNotifier.Web.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        public IActionResult Index()
        {
            var logs = _logService.GetLogs();
            return View(logs);
        }
    }

}
