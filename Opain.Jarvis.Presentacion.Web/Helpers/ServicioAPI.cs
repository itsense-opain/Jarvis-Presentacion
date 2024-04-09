using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Net.Http.Json;

namespace Opain.Jarvis.Presentacion.Web.Helpers
{
    public class ServicioApi : IServicioApi
    {
        public ServicioApi(IConfiguration configuration)
        {
            var server = configuration.GetSection("Rutas:BaseServicio").Value;

            Cliente = new HttpClient
            {
                BaseAddress = new Uri(server),Timeout= TimeSpan.FromSeconds(300)
        };

            Cliente.DefaultRequestHeaders.Accept.Clear();
            Cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public HttpClient Cliente { get; set; }
        public object HttpServerUtility { get; }

        public async Task<T> DeleteAsync<T>(string servicio)
        {
            HttpResponseMessage responseMessage = await Cliente.DeleteAsync(servicio).ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadFromJsonAsync<T>();
            }

            return default(T);
        }


        public async Task<T> GetAsync<T>(string servicio)
        {
            HttpResponseMessage responseMessage = await Cliente.GetAsync(servicio).ConfigureAwait(false);
            //string Json = await responseMessage.Content.ReadAsStringAsync();
            //StringContent postParameters = new StringContent(JsonConvert.SerializeObject(Json), Encoding.UTF8, "application/json");
            //responseMessage.Content = postParameters;
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadFromJsonAsync<T>();
            }

            return default(T);
        }

        public async Task<T> PostAsync<T>(string servicio, object parametros)
        {

            StringContent postParameters = new StringContent(JsonConvert.SerializeObject(parametros), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await Cliente.PostAsync(servicio, postParameters).ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadFromJsonAsync<T>();
            }

            return default(T);
        }

        public async Task<T> PutAsync<T>(string servicio, object parametros)
        {
            StringContent postParameters = new StringContent(JsonConvert.SerializeObject(parametros), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await Cliente.PutAsync(servicio, postParameters).ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadFromJsonAsync<T>();
            }

            return default(T);
        }

        public async Task<T> AddToken<T>(string token)
        {
            if (token != "false" && token != "")
            {                               
                if (!Cliente.DefaultRequestHeaders.Contains("Authorization"))
                {
                    Cliente.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }
                else
                {
                    Cliente.DefaultRequestHeaders.Remove("Authorization");
                    Cliente.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }
            }

            return default(T);
        }
    }
}
