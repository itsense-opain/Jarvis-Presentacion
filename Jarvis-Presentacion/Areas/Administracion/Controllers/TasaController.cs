using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;

namespace Opain.Jarvis.Presentacion.Web.Areas.Administracion.Controllers
{
    [Area("Administracion")]
    [Authorize(Roles = "ADMINISTRADOR")]
    public class TasaController : Controller
    {

        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        public TasaController(IConfiguration cfg, IServicioApi api)
        {
            configuration = cfg;
            servicioApi = api;
        }
        public IActionResult Insertar()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Insertar(TasaAeroportuariaOtd tasaOtd)
        {
            try
            {
                tasaOtd.Fecha = DateTime.Now;
                string rutaRelativa = configuration.GetSection("URIs:TasaAeroportuariaInsertar").Value;
                _ = await servicioApi.PostAsync<bool>(rutaRelativa, tasaOtd);

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
    }
}