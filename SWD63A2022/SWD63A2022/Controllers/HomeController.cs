using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Diagnostics.AspNetCore3;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SWD63A2022.Models;

namespace SWD63A2022.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IExceptionLogger _googleExceptionLogger;
        public HomeController(ILogger<HomeController> logger, [FromServices] IExceptionLogger exceptionLogger 
 )
        {
            _logger = logger;
            _googleExceptionLogger = exceptionLogger;
        }

        public IActionResult Index( )
        {
            _logger.LogInformation("Accessed index page");
            try
            {
                //3.
                throw new Exception("Test exception");

                //4. before you run to test, make sure you enable error reporting api, by accessing the page
                //  on console.cloud.google.com
            }
            catch (Exception ex)
            {
                _googleExceptionLogger.Log(ex);
            }
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult  Login()
        {
            //when this method is actually accessed it means that the authentication has passed
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
