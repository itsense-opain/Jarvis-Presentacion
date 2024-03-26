using Microsoft.AspNetCore.Mvc;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.TransitoConexion.ViewComponents
{
    public class RecibidosViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<PasajeroTransitoOtd> transitos)
        {
            ClaimsPrincipal user = User as ClaimsPrincipal;
            DateTime fechaLimite = DateTime.Now.AddDays(1);

            string aerolinea = "";

            try
            {
                aerolinea = user.Claims.FirstOrDefault(c => c.Type.Equals("SiglaAerolinea")).Value;
            }
            catch (Exception)
            {
                aerolinea = "";
            }

            List<PasajeroTransitoOtd> recibidos = transitos.Where(x => x.FechaHoraCargue < fechaLimite).ToList();

            if (!string.IsNullOrEmpty(aerolinea))
            {
                recibidos = recibidos.Where(x => x.AerolineaSalida.Equals(aerolinea)).ToList();
            }

            return View(recibidos);
        }
    }
}
