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
    public class AerolineaController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        public AerolineaController(IConfiguration cfg, IServicioApi api)
        {
            configuration = cfg;
            servicioApi = api;
        }

        public async Task<IActionResult> Principal()
        {
            string rutaRelativa = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            return PartialView(respuesta.OrderBy(x => x.Nombre).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(AerolineaOtd aerolineaOtd)
        {
            try
            {
                //if (string.IsNullOrEmpty(aerolineaOtd.PDFPasajeros))
                //{
                //    aerolineaOtd.PDFPasajeros = "False";
                //}
                //else
                //{
                //    aerolineaOtd.PDFPasajeros = "True";
                //}
              
                if (string.IsNullOrEmpty(aerolineaOtd.IdEstado))
                {
                    aerolineaOtd.IdEstado = "False";
                }
                else
                {
                    aerolineaOtd.IdEstado = "True";
                }

                string rutaRelativa = configuration.GetSection("URIs:AerolineaActualizar").Value;
                await servicioApi.PostAsync<IList<AerolineaOtd>>(rutaRelativa, aerolineaOtd);

                return RedirectToAction("Principal", "AdmonGeneral", new {  mensaje = "Aerolínea actualizada exitosamente."});
            }
            catch (Exception)
            {
                return RedirectToAction("Principal", "AdmonGeneral", new { mensaje = "Error al actualizar aerolínea." });
            }
        }
    }
}