using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;

namespace Opain.Jarvis.Presentacion.Web.Areas.Administracion.Controllers
{
    [Area("Administracion")]
    [Authorize(Roles = "ADMINISTRADOR")]
    public class HorariosAerolineasController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        public HorariosAerolineasController(IConfiguration cfg, IServicioApi api)
        {
            configuration = cfg;
            servicioApi = api;
        }

        public async Task<IActionResult> Principal()
        {
            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaPrincipal").Value;
            IList<HorarioAerolineaOtd> respuesta = await servicioApi.GetAsync<IList<HorarioAerolineaOtd>>(rutaRelativa);
            var hoy = DateTime.Now.ToShortDateString();
            //return PartialView(respuesta.Where(x => x.Fecha.ToShortDateString().Equals(hoy)));
            return PartialView(respuesta);
        }

        public async Task<IActionResult> Insertar()
        {
            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);

            ViewData["Aerolineas"] = respuesta.Select(n => new SelectListItem
            {
                Value = n.Id.ToString(),
                Text = n.Nombre.ToString()
            }).ToList();

            return PartialView();
        }


        public async Task<IActionResult> Cargar()
        {
            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);

            ViewData["Aerolineas"] = respuesta.Select(n => new SelectListItem
            {
                Value = n.Id.ToString(),
                Text = n.Nombre.ToString()
            }).ToList();

            return PartialView();
        }

       
        [HttpPost]
        public async Task<IActionResult> CargarVuelos(IFormFile archivo, int idAreolinea)
        {
            bool cargueDirecto = bool.Parse(configuration.GetSection("Cofiguracion:CargueDirecto").Value);
        
            try
            {
                if (cargueDirecto)
                {
                    if (archivo.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
                    {
                        return RedirectToAction("Principal", "AdmonGeneral",  new {  mensaje = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos." });
                    }

                    IList<TripulantesOTD> tripulatesOtd = new List<TripulantesOTD>();

                    StreamReader lineas = CargarArchivos.LeerArchivo(archivo);
                    string linea;
                    while ((linea = lineas.ReadLine()) != null)
                    {
                        var campos = linea.Split(",");
                        tripulatesOtd.Add(new TripulantesOTD()
                        {
                            NomTripulante = campos[0],
                            LicTripulante = campos[1],
                            FunTripulante = campos[2],
                            CodAreolinea = idAreolinea
                        });
                    }

                  
                    try
                    {
                        string rutaRelativa = configuration.GetSection("URIs:TripulantesCargar").Value;
                        await servicioApi.PostAsync<bool>(rutaRelativa, tripulatesOtd);

                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.StackTrace.ToString());
                        return RedirectToAction("Principal", "AdmonGeneral", new { mensaje = "Archivo de tripulantes No se pudo cargar" });
                    }

                }
                else
                {
                    string carpeta = "TRIPULANTES";
                    var fecha = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    var nombreArchivo = string.Format("TRIP{0}-{1}.txt", "xxx", fecha);
                    var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                    await CargarArchivos.Cargar(configuration, archivo, nombreArchivo, carpeta, token);
                }

                return RedirectToAction("Principal", "AdmonGeneral", new {  mensaje = "Archivo de tripulante cargado exitosamente." });
            }

            catch (Exception Ex)
            {
                Console.Write(Ex.StackTrace.ToString());
                // Envia por correo indicando el error
                string filefirma = "https://" + Request.Host + "/images/firma.png";                

            }

            return RedirectToAction("Principal", "AdmonGeneral", new { mensaje = "Archivo de tripulantes No se pudo cargar" });
        }
       

        [HttpPost]
        public async Task<IActionResult> Insertar(HorarioAerolineaOtd horarioAerolineaOtd)
        {
            try
            {
                horarioAerolineaOtd.Fecha = DateTime.Now;
                string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaInsertar").Value;
                bool respuesta = await servicioApi.PostAsync<bool>(rutaRelativa, horarioAerolineaOtd);

                return Json(respuesta);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        public async Task<IActionResult> Eliminar(int Id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:HorarioAerolineaObtener").Value, Id);
            HorarioAerolineaOtd respuesta = await servicioApi.GetAsync<HorarioAerolineaOtd>(rutaRelativa);
            return PartialView(respuesta);
        }
        [HttpPost]
        public async Task<IActionResult> Eliminar(HorarioAerolineaOtd horarioAerolineaOtd)
        {
            try
            {
                string rutaRelativa = string.Format(configuration.GetSection("URIs:HorarioAerolineaEliminar").Value, horarioAerolineaOtd.Id);
                _ = await servicioApi.PostAsync<bool>(rutaRelativa, horarioAerolineaOtd);

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        public async Task<IActionResult> Editar(int Id)
        {
            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);

            string rutaRelativa2 = string.Format(configuration.GetSection("URIs:HorarioAerolineaObtener").Value, Id);
            HorarioAerolineaOtd respuesta2 = await servicioApi.GetAsync<HorarioAerolineaOtd>(rutaRelativa2);

            IList<AerolineaOtd> respuestaunica = respuesta.Where(p => p.Id == respuesta2.IdAerolinea).ToList();
            ViewData["Aerolineas"] = respuestaunica.Select(n => new SelectListItem
            {
                Value = n.Id.ToString(),
                Text = n.Nombre.ToString()
            }).ToList();

            DateTime today = DateTime.Now;

            ViewBag.FchUpdate = today;

            return PartialView(respuesta2);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(HorarioAerolineaOtd horarioAerolineaOtd)
        {
            try
            {
                string rutaRelativa2 = string.Format(configuration.GetSection("URIs:HorarioAerolineaObtener").Value, horarioAerolineaOtd.Id);
                HorarioAerolineaOtd respuesta2 = await servicioApi.GetAsync<HorarioAerolineaOtd>(rutaRelativa2);
                horarioAerolineaOtd.Fecha = DateTime.Now;
                horarioAerolineaOtd.IdAerolinea = respuesta2.IdAerolinea;
                string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaActualizar").Value;
                _ = await servicioApi.PostAsync<bool>(rutaRelativa, horarioAerolineaOtd);

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
    }
}