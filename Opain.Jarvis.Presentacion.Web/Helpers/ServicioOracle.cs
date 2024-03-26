using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Helpers
{
    public class ServicioOracle : IServicioApi
    {
        public HttpClient Cliente { get; set; }

        public HttpStatusCode httpStatus { get; set; }
        public object HttpServerUtility { get; }

        public ServicioOracle(IConfiguration configuration)
        {
            var server = configuration.GetSection("Rutas:Oracle").Value;

            Cliente = new HttpClient
            {
                BaseAddress = new Uri(server),
            };

            Cliente.DefaultRequestHeaders.Accept.Clear();
            Cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
        }

        public async Task<T> DeleteAsync<T>(string servicio)
        {
            HttpResponseMessage responseMessage = await Cliente.DeleteAsync(servicio).ConfigureAwait(false);
            this.httpStatus = responseMessage.StatusCode;
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadAsAsync<T>();
            }

            return default(T);
        }

        public async Task<T> GetAsync<T>(string servicio)
        {
            HttpResponseMessage responseMessage = await Cliente.GetAsync(servicio).ConfigureAwait(false);
            this.httpStatus = responseMessage.StatusCode;
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadAsAsync<T>();
            }

            return default(T);
        }

        public async Task<T> PostAsync<T>(string servicio, object parametros)
        {
            StringContent postParameters = new StringContent(JsonConvert.SerializeObject(parametros), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await Cliente.PostAsync(servicio, postParameters).ConfigureAwait(false);
            this.httpStatus = responseMessage.StatusCode;
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadAsAsync<T>();
            }

            return default(T);
        }

        public async Task<T> PutAsync<T>(string servicio, object parametros)
        {
            StringContent postParameters = new StringContent(JsonConvert.SerializeObject(parametros), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await Cliente.PutAsync(servicio, postParameters).ConfigureAwait(false);
            this.httpStatus = responseMessage.StatusCode;
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadAsAsync<T>();
            }

            return default(T);
        }

        public async Task<T> AddToken<T>(string token)
        {
            if (token != "false" && token != "")
            {
                Cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return default(T);
        }
    }
}
