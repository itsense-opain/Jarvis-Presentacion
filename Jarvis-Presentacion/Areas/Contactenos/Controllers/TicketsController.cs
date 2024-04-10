using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Opain.Jarvis.Presentacion.Web.Areas.Contactenos.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,OPAIN,AEROLINEA,CARGA")]
    [Area("Contactenos")]
    public class TicketsController : Controller
    {
        private readonly IServicioApi servicioApi;
        private readonly IConfiguration configuration;

        public TicketsController(IServicioApi api, IConfiguration cfg)
        {
            configuration = cfg;
            servicioApi = api;
        }

        [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,OPAIN,AEROLINEA,CARGA")]
        [HttpGet]
        public async Task<IActionResult> Principal(string resultado = "")
        {
            string rutaRelativa = configuration.GetSection("URIs:TicketObtenerTodos").Value;
            IList<TicketOtd> respuesta = await servicioApi.GetAsync<IList<TicketOtd>>(rutaRelativa);

            if (User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("AEROLINEA") || User.IsInRole("CARGA"))
            {
                var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;

                respuesta = respuesta.Where(x => x.NombreAerolinea == aerolinea).ToList();
            }

            ViewData["resultado"] = resultado;

            return View(respuesta);
        }

        [Authorize(Roles = "SUPERVISOR,SUPERVISOR CARGA,AEROLINEA,CARGA")]
        [HttpGet]
        public async Task<IActionResult> Insertar()
        {
            return PartialView();
        }

        //[Authorize(Roles = "SUPERVISOR,SUPERVISOR CARGA,AEROLINEA,CARGA")]
        //[HttpPost]
        //public async Task<IActionResult> Insertar(TicketOtd ticketOtd, IFormFile adjunto)
        //{
        //    string rutaRelativaId = configuration.GetSection("URIs:TicketUltimoId").Value;
        //    var id = await servicioApi.GetAsync<int>(rutaRelativaId);

        //    if (adjunto != null)
        //    {
        //        if (adjunto.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
        //        {
        //            return RedirectToAction("Principal", new { resultado = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos." });
        //        }

        //        var carpeta = "TICKETS";
        //        string extension = Path.GetExtension(adjunto.FileName);
        //        string fecha = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        //        string nombreArchivo = string.Format("{0}{1}-{2}{3}", carpeta, fecha, (id + 1), extension);
        //        var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
        //        bool resultado = await CargarArchivos.Cargar(configuration, adjunto, nombreArchivo, carpeta, token);

        //        if (resultado)
        //        {
        //            ticketOtd.Adjunto = nombreArchivo;
        //        }
        //    }

        //    var aerolinea = int.Parse(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value);
        //    ticketOtd.IdAerolinea = aerolinea;
        //    ticketOtd.FechaCreacion = DateTime.Now;
        //    ticketOtd.Estado = 1;
        //    ticketOtd.Seguimiento = 1;
        //    // obtengo el id de usuario logueado

        //    UsuarioOtd usuario = await servicioApi.GetAsync<UsuarioOtd>(string.Format(configuration.GetSection("URIs:UsuariosConsultarPorAlias").Value, User.Identity.Name)).ConfigureAwait(false);


        //    var userId = usuario.Id;
        //    ticketOtd.IdUsuario = userId;
        //    ticketOtd.NombreUsuario = User.Claims.Where(c => c.Type.Equals("NombreUsuario")).FirstOrDefault().Value;
        //    ticketOtd.NombreAerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;


        //    ticketOtd.NumeroTicket = string.Format("{0}{1}{2}-{3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (id + 1));

        //    string rutaRelativa = configuration.GetSection("URIs:TicketInsertar").Value;
        //    await servicioApi.PostAsync<TicketOtd>(rutaRelativa, ticketOtd);

        //    var mensaje = "Ticket creado número: " + ticketOtd.NumeroTicket;

        //    return RedirectToAction("Principal", new { resultado = mensaje });
        //}



        [Authorize(Roles = "SUPERVISOR,SUPERVISOR CARGA,AEROLINEA,CARGA")]
        [HttpPost]
        public async Task<IActionResult> Insertar(TicketOtd ticketOtd, List<IFormFile> adjunto)
        {
            string rutaRelativaId = configuration.GetSection("URIs:TicketUltimoId").Value;
            var id = await servicioApi.GetAsync<int>(rutaRelativaId);

            // Aquí puedes trabajar con los archivos adjuntados
            foreach (var archivoadjunto in adjunto)
            {
                if (archivoadjunto != null)
                {
                    if (archivoadjunto.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
                    {
                        return RedirectToAction("Principal", new { resultado = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos." });
                    }

                    var carpeta = "TICKETS";
                    string extension = Path.GetExtension(archivoadjunto.FileName);
                    string fecha = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    string nombreArchivo = string.Format("{0}{1}-{2}{3}", carpeta, fecha, (id + 1), extension);
                    var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                    bool resultado = await CargarArchivos.Cargar(configuration, archivoadjunto, nombreArchivo, carpeta, token);
                    //bool resultado = await CargarArchivos.CargarListaArchivos(configuration, archivoadjunto, nombreArchivo, carpeta, token);
                    if (resultado)
                    {
                        ticketOtd.Adjunto += nombreArchivo.TrimEnd() + " "; 
                    }
                }
            }





            var aerolinea = int.Parse(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value);
            ticketOtd.IdAerolinea = aerolinea;
            ticketOtd.FechaCreacion = DateTime.Now;
            ticketOtd.Estado = 1;
            ticketOtd.Seguimiento = 1;
            if (ticketOtd.Adjunto !=null)
            {
                string cadena = ticketOtd.Adjunto.TrimEnd(' ');
                ticketOtd.Adjunto = cadena.ToLower().TrimEnd();

            }
          

            // obtengo el id de usuario logueado

            UsuarioOtd usuario = await servicioApi.GetAsync<UsuarioOtd>(string.Format(configuration.GetSection("URIs:UsuariosConsultarPorAlias").Value, User.Identity.Name)).ConfigureAwait(false);


            var userId = usuario.Id;
            ticketOtd.IdUsuario = userId;
            ticketOtd.NombreUsuario = User.Claims.Where(c => c.Type.Equals("NombreUsuario")).FirstOrDefault().Value;
            ticketOtd.NombreAerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;


            ticketOtd.NumeroTicket = string.Format("{0}{1}{2}-{3}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (id + 1));

            string rutaRelativa = configuration.GetSection("URIs:TicketInsertar").Value;
            await servicioApi.PostAsync<TicketOtd>(rutaRelativa, ticketOtd);

            var mensaje = "Ticket creado número: " + ticketOtd.NumeroTicket;

            return RedirectToAction("Principal", new { resultado = mensaje });
        }




        [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,OPAIN,AEROLINEA,CARGA")]
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:TicketObtener").Value, id);
            var ticket = await servicioApi.GetAsync<TicketOtd>(rutaRelativa);
            return PartialView(ticket);
        }

        [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,OPAIN,AEROLINEA,CARGA")]
        [HttpGet]
        public async Task<IActionResult> DescargarArchivo(string carpeta, string nombreArchivo)
        {
            var urlServicio = configuration.GetSection("Rutas:BaseServicio").Value;

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri(urlServicio);
                cliente.DefaultRequestHeaders.Clear();
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var resp = await cliente.GetAsync(string.Format(configuration.GetSection("URIs:ArchivosObtenerArchivo").Value, carpeta, HttpUtility.UrlEncode(nombreArchivo)));
                resp.EnsureSuccessStatusCode();
                var fileStream = await resp.Content.ReadAsStreamAsync();
                
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = nombreArchivo,
                    Inline = true

                };
                               
                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"" + cd.ToString() + "\"");
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                var tipo = resp.Content.Headers?.ContentType?.MediaType;

                if (tipo == "application/zip")
                {
                    // Establecer el tipo de contenido como "application/zip"
                    return File(fileStream, "application/zip");
                }
                else
                {
                    return File(fileStream, tipo);
                }

                //Response.Headers.Add("Content-Disposition", $"attachment; filename=\"" + nombreArchivo + "\"");

                //return File(fileStream, tipo);
            }
        }

        [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,OPAIN,AEROLINEA,CARGA")]
        [HttpGet]
        public async Task<IActionResult> InsertarMensaje()
        {
            return PartialView();
        }

        [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,OPAIN,AEROLINEA,CARGA")]
        [HttpPost]
        public async Task<IActionResult> InsertarMensaje(RespuestaTicketOtd rticketOtd, IFormFile mensajeAdjunto)
        {

            if (mensajeAdjunto != null)
            {
                if (mensajeAdjunto.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
                {
                    return RedirectToAction("Principal", new { resultado = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos." });
                }

                var carpeta = "TICKETS";
                string extension = Path.GetExtension(mensajeAdjunto.FileName);
                string fecha = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                string nombreArchivo = string.Format("{0}{1}-{2}{3}", carpeta, fecha, rticketOtd.IdTicket, extension);
                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                bool resultado = await CargarArchivos.Cargar(configuration, mensajeAdjunto, nombreArchivo, carpeta,token);

                if (resultado)
                {
                    rticketOtd.Adjunto = nombreArchivo;
                }
            }

            rticketOtd.FechaCreacion = DateTime.Now;
            UsuarioOtd usuario = await servicioApi.GetAsync<UsuarioOtd>(string.Format(configuration.GetSection("URIs:UsuariosConsultarPorAlias").Value, User.Identity.Name)).ConfigureAwait(false);
            
            var userId = usuario.Id;
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            rticketOtd.IdUsuario = userId;
            rticketOtd.NombreUsuario = User.Claims.Where(c => c.Type.Equals("NombreUsuario")).FirstOrDefault().Value;

            string rutaRelativa = configuration.GetSection("URIs:TicketInsertarMensaje").Value;
            await servicioApi.PostAsync<RespuestaTicketOtd>(rutaRelativa, rticketOtd);

            var mensaje = "Mensaje creado exitosamente.";

            return RedirectToAction("Principal", new { resultado = mensaje });
        }

        [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,OPAIN,AEROLINEA,CARGA")]
        [HttpPost]
        public async Task<IActionResult> EstadoTicket(int idTicket, string estadoTicket)
        {
           
            string rutaRelativa = string.Format(configuration.GetSection("URIs:TicketObtener").Value, idTicket);
            var ticket = await servicioApi.GetAsync<TicketOtd>(rutaRelativa);
            var mensaje = "";
            if (estadoTicket == "on")
            {
                ticket.Estado = 1;
                mensaje = "Ticket abierto exitosamente.";
            }
            else
            {
                ticket.Estado = 0;
                mensaje = "Ticket cerrado exitosamente.";
            }

            string rutaRelativaAct = configuration.GetSection("URIs:TicketActualizar").Value;
            await servicioApi.PostAsync<TicketOtd>(rutaRelativaAct, ticket);

            return RedirectToAction("Principal", new { resultado = mensaje });
        }
    }
}