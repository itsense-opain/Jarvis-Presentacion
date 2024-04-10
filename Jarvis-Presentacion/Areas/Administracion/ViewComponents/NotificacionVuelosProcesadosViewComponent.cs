using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Areas.Administracion.Controllers;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.Administracion.ViewComponents
{
    public class NotificacionVuelosProcesadosViewComponent : ViewComponent
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        private readonly ILogger<AdmonGeneralController> _logger;

        public NotificacionVuelosProcesadosViewComponent(IConfiguration cfg, IServicioApi api, ILogger<AdmonGeneralController> logger)
        {
            servicioApi = api;
            configuration = cfg;
            _logger = logger;
        }

        public async Task<IList<OperacionVueloOtd>> ObtenerTodosAsync(string fechaInicio, 
            string fechaFinal)
        {
            string rutaRelativa = configuration.GetSection("URIs:VuelosObtenerTodos").Value;
            rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFinal, "CONA");

            IList<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);
                        
            return respuesta;
        }

        public async Task<IViewComponentResult> InvokeAsync(string fecha = "")
        {
            var fechaInicial = fecha=="" ? DateTime.Now.AddDays(-15).ToString("yyyy-MM-dd"): fecha;
            var fechaFinal = fecha == "" ? DateTime.Now.ToString("yyyy-MM-dd") : fecha;
            IList<OperacionVueloOtd> vuelos = await ObtenerTodosAsync(fechaInicial, fechaFinal);
            //var vuelosFiltroFechaYEstadoProcesoSeis = vuelos.Where(x => x.Fecha.Equals(fechaFiltro) && x.EstadoProceso == "6" && (x.Id_Daily != null || x.Id_Daily != "0");
            var vuelosFiltroFechaYEstadoProceso = vuelos.Where(
             //x => (x.Fecha >= DateTime.Parse(fechaInicial) && x.Fecha <= DateTime.Parse(fechaFinal))
            //&& x.Fecha.Equals(fechaFiltro.AddDays(-30).ToString())
            //&&
            x=>x.EstadoProceso == "6"
            && x.EnvioNotificacion == "0").ToList();
            
            return View(vuelosFiltroFechaYEstadoProceso);
        }

        //public async Task<PasajeroTransitoOtd> ObtenerTransito(int id)
        //{
        //    string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitosObtenerTransito").Value, id);
        //    PasajeroTransitoOtd respuesta = await servicioApi.GetAsync<PasajeroTransitoOtd>(rutaRelativa).ConfigureAwait(false);

        //    return respuesta;
        //}
    }
}
