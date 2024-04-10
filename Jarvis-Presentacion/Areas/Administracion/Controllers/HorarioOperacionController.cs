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
    public class HorarioOperacionController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        public HorarioOperacionController(IConfiguration cfg, IServicioApi api)
        {
            configuration = cfg;
            servicioApi = api;
        }

        public async Task<IActionResult> Principal()
        {
            string rutaRelativa = configuration.GetSection("URIs:HorarioOperacionPrincipal").Value;
            IList<HorarioOperacionOtd> respuesta = await servicioApi.GetAsync<IList<HorarioOperacionOtd>>(rutaRelativa);
            return PartialView(respuesta);
        }

        public IActionResult Insertar()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Insertar(HorarioOperacionOtd horarioOperacionOtd)
        {
            try
            {
                string rutaRelativa = configuration.GetSection("URIs:HorarioOperacionInsertar").Value;
                _ = await servicioApi.PostAsync<bool>(rutaRelativa, horarioOperacionOtd);

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
    }
}