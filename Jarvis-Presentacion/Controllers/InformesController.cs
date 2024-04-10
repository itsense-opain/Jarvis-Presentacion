using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Opain.Jarvis.Presentacion.Web.Controllers
{
    public class InformesController : Controller
    {
        public IActionResult Informes()
        {
            return View();
        }

        public IActionResult EstadisticasIngresos()
        {
            return View();
        }

        public IActionResult EstadisticasCargueArchivos()
        {
            return View();
        }

        public IActionResult EstadisticasHorasIngreso()
        {
            return View();
        }

        public IActionResult EstadisticasCantidadVuelos()
        {
            return View();
        }

        public IActionResult EstadisticasCantidadPasajeros()
        {
            return View();
        }

        public IActionResult EstadisticasCantidadTransitos()
        {
            return View();
        }

        public IActionResult EstadisticasDestinos()
        {
            return View();
        }

        public IActionResult InformesAdmin()
        {
            return View();
        }

        public IActionResult EstadisticasIngresosAdmin()
        {
            return View();
        }

        public IActionResult EstadisticasCargueArchivosAdmin()
        {
            return View();
        }

        public IActionResult EstadisticasHorasIngresoAdmin()
        {
            return View();
        }

        public IActionResult EstadisticasCantidadVuelosAdmin()
        {
            return View();
        }

        public IActionResult EstadisticasCantidadPasajerosAdmin()
        {
            return View();
        }

        public IActionResult EstadisticasCantidadTransitosAdmin()
        {
            return View();
        }

        public IActionResult EstadisticasDestinosAdmin()
        {
            return View();
        }
    }
}