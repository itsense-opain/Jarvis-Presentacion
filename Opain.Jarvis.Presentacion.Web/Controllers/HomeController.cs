using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Opain.Jarvis.Presentacion.Web.Helpers;
using Opain.Jarvis.Presentacion.Web.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
namespace Opain.Jarvis.Presentacion.Web.Controllers
{
    //public class HomeController : Controller
    //{
    //    private readonly IServicioApi servicioApi;
    //    private readonly ILogger<HomeController> _logger;
    //    public IConfiguration Configuration { get; }
    //    public HomeController(IConfiguration conf, IServicioApi api, ILogger<HomeController> logger)
    //    {
    //        Configuration = conf;
    //        servicioApi = api;
    //        _logger = logger;
    //    }

    //    public IActionResult Index()
    //    {
    //        try
    //        {
    //            _logger.LogInformation("Ingreso al home");

    //            if (HttpContext.User.Claims.Count() == 0)
    //            {
    //                return LocalRedirect("/Identity/Account/Login");
    //            }

    //            return View();
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, ResourceMessage.ErrorSystem);
    //            throw;
    //        }
    //    }

    //    public IActionResult Privacy()
    //    {
    //        return View();
    //    }

    //    public IActionResult Error404()
    //    {
    //        return View();
    //    }

    //    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //    public IActionResult Error()
    //    {
    //        if (HttpContext.User.Claims.Count() == 0)
    //        {
    //            return LocalRedirect("/Identity/Account/Login");
    //        }
    //        else { return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); }


    //    }

    //}


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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
