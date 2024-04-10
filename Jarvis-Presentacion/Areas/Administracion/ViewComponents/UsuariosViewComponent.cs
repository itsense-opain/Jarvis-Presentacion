using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.Administracion.ViewComponents
{
    public class UsuariosViewComponent : ViewComponent
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;

        public UsuariosViewComponent(IConfiguration cfg, IServicioApi api)
        {
            servicioApi = api;
            configuration = cfg;
        }

        public async Task<IViewComponentResult> InvokeAsync(string aerolinea = "")
        {
            IEnumerable<UsuarioOtd> usuarios = await servicioApi.GetAsync<IEnumerable<UsuarioOtd>>(configuration.GetSection("URIs:UsuariosObtenerTodos").Value).ConfigureAwait(false);

            if (User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
            {
                usuarios = usuarios.Where(x => x.Aerolinea == aerolinea);
            }

            return View(usuarios);
        }

    }
}
