using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Opain.Jarvis.Presentacion.Web.Areas.Contactenos.ContactenosHostingStartup))]
namespace Opain.Jarvis.Presentacion.Web.Areas.Contactenos
{
    public class ContactenosHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });

        }
    }
}
