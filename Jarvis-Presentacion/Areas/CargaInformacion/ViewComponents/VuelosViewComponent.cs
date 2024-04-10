using Microsoft.AspNetCore.Mvc;
using Opain.Jarvis.Dominio.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.ViewComponents
{
    public class VuelosViewComponent : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync(List<OperacionVueloOtd> listadoVuelos)
        {
            var vuelos = listadoVuelos;

            if (User.IsInRole("ADMINISTRADOR"))
            {
                vuelos = listadoVuelos.Where(x => x.ConfirmacionOperacion == 0 && x.EstadoProceso != "5" && x.EstadoProceso != "6").ToList();
            }
            else
            {

                vuelos = listadoVuelos.Where(x => x.ConfirmacionOperacion == 0).ToList();
            }
                            
            return View(vuelos);
        }

    }
}
