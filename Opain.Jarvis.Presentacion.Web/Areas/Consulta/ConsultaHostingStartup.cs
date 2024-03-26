using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Opain.Jarvis.Presentacion.Web.Areas.Consulta.ConsultaHostingStartup))]
namespace Opain.Jarvis.Presentacion.Web.Areas.Consulta
{
    public class ConsultaHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });

        }
    }
}
