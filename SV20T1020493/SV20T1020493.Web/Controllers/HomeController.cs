using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DataLayers.SQLServer;
using SV20T1020493.Web.Models;
using System.Diagnostics;

namespace SV20T1020493.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
       
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string connectionString = Configuration.ConnectionString;
            var accountEmployeeDAL = new AccountEmployeeDAL(connectionString);

            int employeeID = Convert.ToInt32(User.GetUserData()?.UserId);
            string employeeName = User.GetUserData()?.DisplayName;
             accountEmployeeDAL.SetEmployeeIDInContext(employeeID, employeeName);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
