using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Helpers
{
    public static class CargarArchivos
    {
   
        public static async Task<bool> Cargar(IConfiguration configuration, IFormFile archivo, string nombreArchivo, string carpeta, string token)
        {
            string urlServicio = configuration.GetSection("Rutas:BaseServicio").Value;
            HttpContent fileStreamContent = new StreamContent(archivo.OpenReadStream());
            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "archivo", FileName = nombreArchivo };
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {

                formData.Add(fileStreamContent);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.PostAsync(string.Format("{0}{1}", urlServicio, configuration.GetSection("URIs:CargarArchivosCargar").Value) + carpeta, formData);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
        }

        //public static async Task<bool> CrearArchivo(IConfiguration configuration, string tipo, string texto, string carpeta)
        //{
        //    string urlServicio = configuration.GetSection("Rutas:BaseServicio").Value;

        //    var server = System.Configuration.ConfigurationManager.AppSettings["RutaServicioLocal"].ToString();


        //    string rutaFn = configuration.GetSection("URIs:CrearArchivosCargue").Value;

           
        //    using (var client = new HttpClient())
        //    {
        //        var response = await client.PostAsync(rutaFn, new HttpContent nn );

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;



        //    return false;     
        //}

        public static async Task<bool> CargarArchArray(IConfiguration configuration, string bytes, string IdAerolinea, string token,string idOperacionVuelo)
        {
            string urlServicio = configuration.GetSection("Rutas:BaseServicio").Value;

            string rutaFn = string.Format("{0}{1}", urlServicio, configuration.GetSection("URIs:CargarArchivosCargarSoporteAero").Value);

            var archivo = new ArchivoOtd { base64 = bytes, carpeta = IdAerolinea, Nombre = idOperacionVuelo };

            StringContent params2 = new StringContent(JsonConvert.SerializeObject(archivo), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
              
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.PostAsync(rutaFn + IdAerolinea, params2);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;                
        }

        public static async Task<bool> CargarArchSoportes(IConfiguration configuration, 
            string bytes, string token,
            string Carpeta,
            string NombreArchivo)
        {
            string urlServicio = configuration.GetSection("Rutas:BaseServicio").Value;

            string rutaFn = string.Format("{0}{1}", urlServicio, configuration.GetSection("URIs:CargarArchivosCargarSoporteAero").Value);

            var archivo = new ArchivoOtd
            {
                base64 = bytes,
                carpeta = Carpeta,
                Nombre = NombreArchivo
            };

            StringContent params2 = new StringContent(JsonConvert.SerializeObject(archivo), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.PostAsync(rutaFn + NombreArchivo, params2);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }

        public static StreamReader LeerArchivo(IFormFile archivo)
        {
            var reader = new StreamReader(archivo.OpenReadStream());

            return reader;
        }
    }
}
