using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Bussiness
{
    public interface IEmail
    {

        Task SendEmailAsync(string email, string subject, string message);
    }
}
