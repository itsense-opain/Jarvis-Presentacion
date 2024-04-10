using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;


namespace Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.Controllers
{
    [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
    [Area("CargaInformacion")]
    public class ArchivosController : Controller
    {

        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        private readonly PasajerosController pasajerosController;
        private readonly IEmailSender emailSender;
        private readonly ServicioComboBox servicioComboBox;
        private static string tipoCargue;
        private readonly ILogger<VuelosController> _logger;
        private readonly ILogger<PasajerosController> _loggerPasajeros;

        public ArchivosController(IConfiguration cfg, IServicioApi api, IEmailSender email, 
            ServicioComboBox servicioComboBox1, 
            ILogger<VuelosController> logger)
        {
            configuration = cfg;
            servicioApi = api;
            emailSender = email;
            servicioComboBox = servicioComboBox1;
            pasajerosController = new PasajerosController(cfg, api, emailSender, servicioComboBox, _loggerPasajeros);
            _logger = logger;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpGet]
        public async Task<IActionResult> Cargar(int idOperacion, string tipo)
        {
            int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);
            string rutaRelativa = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            IList<AerolineaOtd> respuestaunica = respuesta.Where(p => p.Id == idAerolinea).ToList();
            if (tipo == "PASAJEROS")
            {
                ViewData["TipoPDFPasa"] = respuestaunica[0].PDFPasajeros;
                tipoCargue = respuestaunica[0].PDFPasajeros;
            }

            if (tipo == "TRANSITOS")
            {
                ViewData["TipoPDFPasa"] = respuestaunica[0].PDFPasajeros;
                tipoCargue = respuestaunica[0].PDFPasajeros;
            }

            if (tipo == "MANIFIESTO" || tipo == "GENDEC")
            {
                ViewData["TipoPDFPasa"] = "4";
                tipoCargue = "4";
            }



            ViewData["operacion"] = idOperacion;
            ViewData["carpeta"] = tipo;
            return PartialView();
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,ADMINISTRADOR")]
        [HttpPost]
        public async Task<IActionResult> Cargar(IFormFile archivo, int idOperacion, string carpeta, string numeroVuelo, string fechaInicio, string fechaFinal, string fechaInicioHistorico, string fechaFinalHistorico, string tipoVuelo, string tipoVueloHistorico)
        {
            string FechaCarpeta = "";
            string NumeroVueloArchivo = "";
            string CarpetaDestino="";
            string NombreArchivo = "";
            string MatriculaVuelo = "";
            OperacionVueloOtd operacion = null;
            string Abreviatura = "";
            string extension = Path.GetExtension(archivo.FileName);
            string msg = "";
            bool resultado;

            var tipo = tipoCargue;

            try
            {
                operacion = await ObtenerOperacion(idOperacion);
                FechaCarpeta = operacion.Fecha.ToString("yyyyMMdd");
                NumeroVueloArchivo = operacion.Vuelo;
                MatriculaVuelo = operacion.Matricula;
                string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                IList<AerolineaOtd> ListAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                Abreviatura = ListAerolineas.Where(p => p.Id == Convert.ToInt32(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim())).FirstOrDefault().Sigla;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Err 1 ArchivosController Cargar :" + ResourceMessage.ErrorSystem);
                return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico});
            }

            if (extension.ToLower().Contains(".pdf"))
            {
                if (archivo.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoPdf").Value))
                {
                    _logger.LogInformation("01 ArchivosController Cargar Tamaño del archvio muy alto:" + archivo.Length.ToString());
                    msg = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos.";
                    return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = msg, fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                }

                CarpetaDestino = String.Format("{0}//{1}",
                        Abreviatura.Trim(),
                        FechaCarpeta);

                NombreArchivo = string.Concat(carpeta + "-",
                        NumeroVueloArchivo,"-", MatriculaVuelo,
                        archivo.FileName.Substring(archivo.FileName.LastIndexOf('.'),
                        archivo.FileName.Length - (archivo.FileName.LastIndexOf('.'))));

                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;

                //try
                //{
                //    resultado = await CargarArchivos.Cargar(configuration, archivo, NombreArchivo, CarpetaDestino, token);
                //}
                //catch(Exception Ex)
                //{
                //    _logger.LogError(Ex, "Err 2 ArchivosController Cargar :" + ResourceMessage.ErrorSystem);
                //    return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema." });
                //}

                //if (resultado)
                //{
                ArchivoOtd archivoOtd = new ArchivoOtd()
                {
                    Nombre = CarpetaDestino + "//" + NombreArchivo,
                    Operacion = idOperacion,
                    Tipo = carpeta,
                    carpeta= CarpetaDestino
                };
                bool IsFileValid = await ValidarArchivo(archivoOtd).ConfigureAwait(false);
                
                if (IsFileValid)
                {
                    if (carpeta.Equals("GENDEC"))
                    {
                        operacion.ConfirmacionGenDec = 1;                        
                    }

                    if (carpeta.Equals("MANIFIESTO"))
                    {
                        operacion.ConfirmacionManifiesto = 1;                        
                    }

                    if (carpeta.Equals("PASAJEROS"))
                    {
                        operacion.ConfirmacionPasajeros = 1;                       
                    }

                    await ActualizarOperacion(operacion).ConfigureAwait(false);

                    string rutaCargar = configuration.GetSection("URIs:CargueInsertar").Value;

                    CargueOtd cargue = new CargueOtd()
                    {
                        Aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value,
                        Fecha = DateTime.Now,
                        TipoArchivo = carpeta
                    };

                    await servicioApi.PostAsync<CargueOtd>(rutaCargar, cargue).ConfigureAwait(false);
                    try
                    {
                        // Guarda el archivo en la ruta
                        bool IsLoadValid = await CargarArchivos.Cargar(configuration, archivo, NombreArchivo, CarpetaDestino, token);
                        if (IsLoadValid)
                            msg = "Archivo " + carpeta + " cargado exitosamente.";
                    }
                    catch (Exception Ex)
                    {
                        //Console.Write(ex.StackTrace.ToString());
                        _logger.LogError(Ex, "Err 3 ArchivosController Cargar :" + ResourceMessage.ErrorSystem);
                        msg = "El archivo tiene errores y no se pudo cargar.";
                    }
                    //}
                }
                
            }
            else if(extension.ToLower().Contains(".txt"))
            {
                if (tipo == "2")
                {
                    await CargaArchivo(archivo, idOperacion);
                }
                msg = "Archivo cargado exitosamente.";
            }
            else
            {
                msg = "El archivo que debe cargar debe ser PDF, debido a la configuracion de la aerolínea.";
            }
            return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = msg, fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
        }

        public async Task<IActionResult> CargaArchivo(IFormFile archivo, int idOperacionVuelo)
        {
            bool cargueDirecto = bool.Parse(configuration.GetSection("Cofiguracion:CargueDirecto").Value);
            OperacionVueloOtd operacion = await ObtenerOperacion(idOperacionVuelo);
            operacion.ConfirmacionPasajeros = 1;
            await ActualizarOperacion(operacion);

            try
            {
                if (cargueDirecto)
                {
                    if (archivo.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
                    {
                        return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos." });
                    }

                    IList<PasajeroOtd> pasajerosOtd = new List<PasajeroOtd>();

                    StreamReader lineas = CargarArchivos.LeerArchivo(archivo);
                    string linea;
                    while ((linea = lineas.ReadLine()) != null)
                    {
                        var campos = linea.Split(",");
                        pasajerosOtd.Add(new PasajeroOtd()
                        {
                            Fecha = new DateTime(int.Parse(campos[0].Substring(6, 4)), int.Parse(campos[0].Substring(3, 2)), int.Parse(campos[0].Substring(0, 2))),
                            NumeroVuelo = campos[1],
                            MatriculaVuelo = campos[2],
                            NombrePasajero = campos[3],
                            Categoria = campos[4].Trim(),
                            Operacion = idOperacionVuelo,
                            realiza_viaje = "1",// por default en masivo va que si viaja
                            motivo_exencion = null // el motivo va vacio por defecto para la exencion
                        });
                    }

                    string rutaRelativa = configuration.GetSection("URIs:PasajerosCargar").Value;
                    await servicioApi.PostAsync<bool>(rutaRelativa, pasajerosOtd);

                    string rutaCargar = configuration.GetSection("URIs:CargueInsertar").Value;

                    CargueOtd cargue = new CargueOtd()
                    {
                        Aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value,
                        Fecha = DateTime.Now,
                        TipoArchivo = "Pasajeros"
                    };

                    await servicioApi.PostAsync<CargueOtd>(rutaCargar, cargue).ConfigureAwait(false);
                    try
                    {
                        string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                        IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                        string Abreviatura = respuesta.Where(p => p.Id == Convert.ToInt32(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim())).FirstOrDefault().Sigla;

                        // Guarda el archivo en la ruta 
                        string carpetarReal = String.Format("{0}//{1}//{2}", Abreviatura.Trim(), operacion.Fecha.Year.ToString() + operacion.Fecha.Month.ToString().PadLeft(2, '0') + operacion.Fecha.Day.ToString().PadLeft(2, '0'), operacion.Vuelo.ToString());
                        var fechaReal = string.Format("{0}{1}{2}{3}{4}{5}", operacion.Fecha.Year, operacion.Fecha.Month, operacion.Fecha.Day, operacion.Fecha.Hour, operacion.Fecha.Minute, operacion.Fecha.Second);
                        var nombreArchivoREal = string.Concat("Pasajeros", operacion.Fecha.Year.ToString(), operacion.Fecha.Month.ToString().PadLeft(2, '0'), operacion.Fecha.Day.ToString(), "_", operacion.Fecha.Hour.ToString(), operacion.Fecha.Minute.ToString(), operacion.Fecha.Second.ToString(), operacion.Fecha.Millisecond.ToString(), archivo.FileName.Substring(archivo.FileName.LastIndexOf('.'), archivo.FileName.Length - (archivo.FileName.LastIndexOf('.'))));
                        //var nombreArchivoREal = archivo.FileName.ToString();
                        var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                        bool resultado = await CargarArchivos.Cargar(configuration, archivo, nombreArchivoREal, carpetarReal, token);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.StackTrace.ToString());

                    }
                }
                else
                {
                    string carpeta = "PASAJEROS";
                    var fecha = string.Format("{0}{1}{2}{3}{4}{5}", operacion.Fecha.Year, operacion.Fecha.Month, operacion.Fecha.Day, operacion.Fecha.Hour, operacion.Fecha.Minute, operacion.Fecha.Second);
                    var nombreArchivo = string.Format("PAX{0}-{1}.txt", operacion.Vuelo, fecha);
                    var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                    await CargarArchivos.Cargar(configuration, archivo, nombreArchivo, carpeta, token);
                }


                return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Archivo de Pasajeros cargado exitosamente." });
            }
            catch (Exception Ex)
            {
                Console.Write(Ex.StackTrace.ToString());
                // Envia por correo indicando el error
                string filefirma = "https://" + Request.Host + "/images/firma.png";

                //string filefirma = Request.Host + "/images/firma.png";
                var emailUser = User.Claims.Where(c => c.Type.Equals("UsuarioEmail")).FirstOrDefault().Value;
            }
            return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Archivo de Pasajeros No se pudo cargar" });
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<OperacionVueloOtd> ObtenerOperacion(int id)
        {
            OperacionVueloOtd respuesta = await servicioApi.GetAsync<OperacionVueloOtd>(string.Format(configuration.GetSection("URIs:ArchivosObtenerOperacion").Value, id)).ConfigureAwait(false);
            return respuesta;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task ActualizarOperacion(OperacionVueloOtd operacionVueloOtd)
        {
            _ = await servicioApi.PostAsync<bool>(configuration.GetSection("URIs:ArchivosActualizarOperacion").Value, operacionVueloOtd).ConfigureAwait(false);
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<bool> ValidarArchivo(ArchivoOtd ArchivoOtd)
        {
            bool respuesta = await servicioApi.PostAsync<bool>(configuration.GetSection("URIs:ArchivosValidarArchivo").Value, ArchivoOtd).ConfigureAwait(false);
            return respuesta;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        [HttpGet]
        public async Task<IActionResult> ObtenerArchivo(string carpeta, string nombreArchivo)
        {
            var urlServicio = configuration.GetSection("Rutas:BaseServicio").Value;

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri(urlServicio);
                cliente.DefaultRequestHeaders.Clear();
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var resp = await cliente.GetAsync(string.Format(configuration.GetSection("URIs:ArchivosObtenerArchivo").Value, carpeta, nombreArchivo));
                resp.EnsureSuccessStatusCode();
                var fileStream = await resp.Content.ReadAsStreamAsync();
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = nombreArchivo,
                    Inline = true
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                var tipo = resp.Content.Headers?.ContentType?.MediaType;

                return File(fileStream, tipo);
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        [HttpGet]
        public async Task<IActionResult> ObtenerArchivoPDF(string idAerolinea, string fecha,string idOperacionVuelo)
        {
            var urlServicio = configuration.GetSection("Rutas:BaseServicio").Value;

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri(urlServicio);
                cliente.DefaultRequestHeaders.Clear();
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                string Abreviatura = respuesta.Where(p => p.Id == Convert.ToInt32(idAerolinea)).FirstOrDefault().Sigla;

                _logger.LogInformation("Paremetros idaerolinea= "+ idAerolinea+ "Paremetros fecha= "+ fecha+ "Paremetros idOperacionVuelo= "+ idOperacionVuelo);

                var resp = await cliente.GetAsync(string.Format(configuration.GetSection("URIs:ArchivosObtenerArchivoPDF").Value, Abreviatura, fecha, idOperacionVuelo));
                resp.EnsureSuccessStatusCode();
                var fileStream = await resp.Content.ReadAsStreamAsync();
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = idOperacionVuelo + ".pdf",
                    //FileName = "SoporteAerolinea.pdf",
                    Inline = true
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                var tipo = resp.Content.Headers?.ContentType?.MediaType;

                return File(fileStream, tipo);
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        [HttpGet]
        public async Task<IActionResult> ObtenerArchivoPath(
            string Carpeta, string Nombre)
        {
            var urlDomain = configuration.GetSection("Rutas:BaseServicio").Value;

            using (var cliente = new HttpClient())
            {
                cliente.BaseAddress = new Uri(urlDomain);
                cliente.DefaultRequestHeaders.Clear();
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                _logger.LogInformation("ObtenerArchivoPath : " + Carpeta + " " + Nombre);

                string urlServicio = string.Format(configuration.GetSection("URIs:ArchivosObtenerArchivo").Value, Carpeta, Nombre);

                var resp = await cliente.GetAsync(urlServicio);
                resp.EnsureSuccessStatusCode();
                var fileStream = await resp.Content.ReadAsStreamAsync();
                System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = Nombre,                    
                    Inline = true
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                Response.Headers.Add("X-Content-Type-Options", "nosniff");

                var tipo = resp.Content.Headers?.ContentType?.MediaType;

                return File(fileStream, tipo);
            }
        }

    }
}