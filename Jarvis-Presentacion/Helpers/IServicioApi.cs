using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Helpers
{
    public interface IServicioApi
    {
        Task<T> GetAsync<T>(string servicio);
        Task<T> PostAsync<T>(string servicio, object parametros);
        Task<T> DeleteAsync<T>(string servicio);
        Task<T> PutAsync<T>(string servicio, object parametros);

        Task<T> AddToken<T>(string token);
    }
}
