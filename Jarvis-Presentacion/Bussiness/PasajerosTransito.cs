using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Bussiness
{
    public class PasajerosTransito
    {
        public async Task<IList<PasajeroTransitoOtd>> TransitosEnTramite(System.Security.Claims.ClaimsPrincipal User,
           Microsoft.Extensions.Configuration.IConfiguration config,
           Opain.Jarvis.Presentacion.Web.Helpers.IServicioApi servicioApi,
           TransitoRequest oFiltro)
        {
            string urlAPI = "api/PasajeroTransito/ObtenerDatos";
            oFiltro.OpcionOperacion = "F";
            oFiltro.Categoria = " in ('TTC')";
            IList<PasajeroTransitoOtd> oResult;

            if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
            {
                oFiltro.AerolineaSalida = User.Claims.FirstOrDefault(c => c.Type.Equals("SiglaAerolinea")).Value;
                oFiltro.EstadoProceso = " In (0,1,2,3) ";
                oResult = await servicioApi.PostAsync<IList<PasajeroTransitoOtd>>(urlAPI, oFiltro);
                oFiltro.OpcionOperacion = "EA";
                IList<PasajeroTransitoOtd> oResultHuerfanos = await servicioApi.PostAsync<IList<PasajeroTransitoOtd>>(urlAPI, oFiltro);
                oResult = AddVuelosHuerfanos(oResultHuerfanos, oResult);
            }
            else
            {                
                oFiltro.EstadoProceso = " In (1,2,3) ";
                oResult = await servicioApi.PostAsync<IList<PasajeroTransitoOtd>>(urlAPI, oFiltro);
                oFiltro.OpcionOperacion = "E";
                IList<PasajeroTransitoOtd> oResultHuerfanos = await servicioApi.PostAsync<IList<PasajeroTransitoOtd>>(urlAPI, oFiltro);
                oResult = AddVuelosHuerfanos(oResultHuerfanos, oResult);
            }
            return oResult;
        }

        private async Task<IList<PasajeroTransitoOtd>> TransitosHuerfanos(System.Security.Claims.ClaimsPrincipal User,
           Microsoft.Extensions.Configuration.IConfiguration config,
           Opain.Jarvis.Presentacion.Web.Helpers.IServicioApi servicioApi,
           TransitoRequest oFiltro)
        {
            string urlAPI = "api/PasajeroTransito/ObtenerDatos";
            oFiltro.OpcionOperacion = "E";            
                        
            IList<PasajeroTransitoOtd> oResult = await servicioApi.PostAsync<IList<PasajeroTransitoOtd>>(urlAPI, oFiltro);
            return oResult;
        }
        private IList<PasajeroTransitoOtd> AddVuelosHuerfanos(IList<PasajeroTransitoOtd> oListOrigen,
            IList<PasajeroTransitoOtd> oListDestino)
        {
            foreach (PasajeroTransitoOtd item in oListOrigen)
            {
                oListDestino.Add(item);
            }
            return oListDestino;
        }
    }    
}
