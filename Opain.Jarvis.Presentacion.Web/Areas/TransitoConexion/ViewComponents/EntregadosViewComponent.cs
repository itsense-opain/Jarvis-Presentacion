using Microsoft.AspNetCore.Mvc;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.TransitoConexion.ViewComponents
{
    public class EntregadosViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<PasajeroTransitoOtd> transitos, List<AerolineaOtd> lstAerolineas)
        {
            ClaimsPrincipal user = User as ClaimsPrincipal;
            DateTime fechaLimite = DateTime.Now.AddDays(1);

            string aerolinea = "";

            try
            {
                aerolinea = user.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;
            }
            catch (Exception)
            {
                aerolinea = "";
            }

            List<PasajeroTransitoOtd> entregados = transitos.Where(x => x.Firmado == 0).Where(x => x.FechaHoraCargue < fechaLimite).ToList();
            int idAerolinea = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);

            // Traigo la abreviatura de la aerolina
            string Abreviatura = "";
            if (idAerolinea == 0)
            {
                entregados = entregados.ToList();
            }
            else
            {
                Abreviatura = lstAerolineas.Where(p => p.Id == idAerolinea).FirstOrDefault().Sigla;
                if (!string.IsNullOrEmpty(aerolinea))
                {
                    
                    entregados = entregados.Where(x => x.AerolineaLlegada.Equals(Abreviatura.ToUpper().Trim())).ToList();
                }
            }


            return View(entregados);
        }
    }
}
