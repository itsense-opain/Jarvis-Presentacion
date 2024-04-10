using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Opain.Jarvis.Presentacion.Web.Controllers
{
    public class ContactenosController : Controller
    {
        public IActionResult Contactenos()
        {
            return View();
        }

        public IActionResult ContactenosAdmin()
        {
            return View();
        }
    }
}