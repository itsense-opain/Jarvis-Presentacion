using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Opain.Jarvis.Presentacion.Web.Areas.Administracion.AdministracionHostingStartup))]
namespace Opain.Jarvis.Presentacion.Web.Areas.Administracion
{
    public class AdministracionHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });

        }
    }
}
