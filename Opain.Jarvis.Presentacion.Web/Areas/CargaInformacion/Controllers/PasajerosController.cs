using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Opain.Jarvis.Presentacion.Web.Bussiness;

namespace Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.Controllers
 
{
    [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
    [Area("CargaInformacion")]
    public class PasajerosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        private readonly IEmail emailSender;
        private readonly ServicioComboBox servicioComboBox;
        private readonly ILogger<PasajerosController> _logger;

        public PasajerosController(IConfiguration cfg, IServicioApi api, IEmail email, 
            ServicioComboBox servicioComboBox1,
            ILogger<PasajerosController> logger)
        {
            configuration = cfg;
            servicioApi = api;
            emailSender = email;
            servicioComboBox = servicioComboBox1;
            _logger = logger;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpGet]
        public async Task<IActionResult> Cargar(int idOperacion)
        {

            ViewData["operacion"] = idOperacion;
            return PartialView();
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> TraerAeroJDE()
        {
            servicioComboBox.TraerAeroJDE();

            return Json(true);

        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> TraerCiudadesJDE()
        {
            servicioComboBox.TraerCiudadesJDE();

            return Json(true);

        }

        //
        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> UpdVueloValidJDE()
        {
            servicioComboBox.UpdVueloValidJDE();

            return Json(true);

        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> TraerVuelosJDE()
        {
            servicioComboBox.TraerVuelosJDE();

            return Json(true);

        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        public async Task<IActionResult> Cargar(IFormFile archivo, int idOperacionVuelo, string numeroVuelo, string fechaInicio, string fechaFinal, string fechaInicioHistorico, string fechaFinalHistorico, string tipoVuelo, string tipoVueloHistorico)
        {
            bool cargueDirecto = false;
            string FechaCarpeta = "";
            string NumeroVueloArchivo = "";
            string MatriculaVuelo = "HK";
            OperacionVueloOtd operacion = null;
            string rutaRelativa = "";
            string linea;
            try
            {
                cargueDirecto = bool.Parse(configuration.GetSection("Cofiguracion:CargueDirecto").Value);
                operacion = await ObtenerOperacion(idOperacionVuelo);
                operacion.ConfirmacionPasajeros = 1;
                _logger.LogInformation("01 PasajerosControler Cargar Inicia Actualización operación.");
                await ActualizarOperacion(operacion);
                _logger.LogInformation("01 PasajerosControler Cargar Fin Actualización operación.");
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Err 1 PasajerosControler Cargar :" + ResourceMessage.ErrorSystem);
                return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
            }

            if (cargueDirecto)
            {
                try
                {
                    if (archivo.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
                    {
                        _logger.LogInformation("01 PasajerosControler Cargar Tamaño del archvio muy alto:" + archivo.Length.ToString());
                        return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                    }
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, "Err 2 PasajerosControler Cargar :" + ResourceMessage.ErrorSystem);
                    return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                }

                IList<PasajeroOtd> pasajerosOtd = new List<PasajeroOtd>();
                string rutaCargar = configuration.GetSection("URIs:CargueInsertar").Value;
                CargueOtd cargue = new CargueOtd()
                {
                    Aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value,
                    Fecha = DateTime.Now,
                    TipoArchivo = "Pasajeros",
                    Usuario = User.Identity.Name,
                    NombreArchivo = archivo.FileName,
                    //NombreCompletoUsuario = User.Claims.Where(c => c.Type.Equals("FullName")).FirstOrDefault().Value
                };
                cargue = await servicioApi.PostAsync<CargueOtd>(rutaCargar, cargue).ConfigureAwait(false);
                _logger.LogInformation("02 PasajerosControler Cargar Generando IDCarga.");

                if (cargue == null)
                {
                    _logger.LogInformation("01 PasajerosControler Cargar fallo la Generación del IDCarga.");
                    return RedirectToAction("Principal", "Vuelos", new { mensaje = "Se presento una falla al generar el ID del identificador de la carga.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                }

                _logger.LogInformation("02 PasajerosControler Cargar Inicia lectura del archivo.");
                try
                {
                    StreamReader lineas = CargarArchivos.LeerArchivo(archivo);
                    FechaCarpeta = operacion.Fecha.ToString("yyyyMMdd");
                    NumeroVueloArchivo = operacion.Vuelo;
                    MatriculaVuelo = operacion.Matricula;
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
                            motivo_exencion = null, // el motivo va vacio por defecto para la exencion,
                            IdCargue = cargue.Id
                        });
                    }
                    rutaRelativa = configuration.GetSection("URIs:PasajerosCargar").Value;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Err 3 PasajerosControler Cargar : :" + ResourceMessage.ErrorSystem);
                    return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                    //Console.Write(ex.StackTrace.ToString());
                }

                try
                {
                    _logger.LogInformation("03 PasajerosControler Cargar Inicio Envio de DTO a la base de datos.");
                    await servicioApi.PostAsync<bool>(rutaRelativa, pasajerosOtd);
                    _logger.LogInformation("03 PasajerosControler Cargar Fin Envio de DTO a la base de datos.");

                    _logger.LogInformation("04 PasajerosControler Cargar Construcción de ruta de alamcenamiento archivo.");
                    string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                    IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                    string Abreviatura = respuesta.Where(p => p.Id == Convert.ToInt32(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim())).FirstOrDefault().Sigla;

                    // Guarda el archivo en la ruta 
                    string CarpetaDestino = String.Format("{0}//{1}",
                        Abreviatura.Trim(),
                        FechaCarpeta);

                    _logger.LogInformation("04 PasajerosControler Cargar Carpeta destino. " + CarpetaDestino);

                    var NombreArchivo = string.Concat("Pasajeros-",
                        NumeroVueloArchivo, "-", MatriculaVuelo,
                        archivo.FileName.Substring(archivo.FileName.LastIndexOf('.'),
                        archivo.FileName.Length - (archivo.FileName.LastIndexOf('.'))));

                    _logger.LogInformation("04 PasajerosControler Cargar Nombre del archivo destino. " + NombreArchivo);

                    //var nombreArchivoREal = archivo.FileName.ToString();
                    var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;

                    var Usuario = User.Claims.Where(c => c.Type.Equals("NombreUsuario")).FirstOrDefault().Value;
                    //var NombreCompletoUsuario = User.Claims.Where(c => c.Type.Equals("FullName")).FirstOrDefault().Value;
                    //var NombreArchivo = archivo.FileName;
                    bool resultado = await CargarArchivos.Cargar(configuration, archivo, NombreArchivo, CarpetaDestino, token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Err 4 PasajerosControler Cargar :" + ResourceMessage.ErrorSystem);
                    return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                    //Console.Write(ex.StackTrace.ToString());
                }
            }
            else
            {
                _logger.LogInformation("02 PasajerosControler Cargar No directo. ");
                string carpeta = "PASAJEROS";
                var fecha = string.Format("{0}{1}{2}{3}{4}{5}",
                    operacion.Fecha.Year, operacion.Fecha.Month,
                    operacion.Fecha.Day, operacion.Fecha.Hour,
                    operacion.Fecha.Minute, operacion.Fecha.Second);

                var nombreArchivo = string.Format("PAX{0}-{1}.txt",
                    operacion.Vuelo, fecha);

                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;

                var Usuario = User.Claims.Where(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")).FirstOrDefault().Value;
                var NombreCompletoUsuario = User.Claims.Where(c => c.Type.Equals("FullName")).FirstOrDefault().Value;

                _logger.LogInformation("02 PasajerosControler Cargar Nombre del carpeta y archivo destino. " + carpeta + " " + nombreArchivo);

                try
                {
                    await CargarArchivos.Cargar(configuration, archivo, nombreArchivo, carpeta, token);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Err 5 PasajerosControler Cargar ::" + ResourceMessage.ErrorSystem);
                    return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                    //Console.Write(ex.StackTrace.ToString());
                }
            }

            return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Archivo de Pasajeros cargado exitosamente.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<OperacionVueloOtd> ObtenerOperacion(int id)
        {
            var ruta = configuration.GetSection("URIs:PasajerosObtenerOperacion").Value;
            string rutaRelativa = string.Format(ruta, id);
            OperacionVueloOtd respuesta = await servicioApi.GetAsync<OperacionVueloOtd>(rutaRelativa).ConfigureAwait(false);

            return respuesta;

        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task ActualizarOperacion(OperacionVueloOtd operacionVueloOtd)
        {
            string rutaRelativa = configuration.GetSection("URIs:PasajerosActualizarOperacion").Value;
            await servicioApi.PostAsync<bool>(rutaRelativa, operacionVueloOtd).ConfigureAwait(false);
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> Principal(int idOperacion, int acciones)
        {
            if (acciones == 2)
            {
                string rutaRelativalocal = string.Format(configuration.GetSection("URIs:PasajerosPrincipal").Value, idOperacion);
                IList<PasajeroOtd> pasajerosJarvis = await servicioApi.GetAsync<IList<PasajeroOtd>>(rutaRelativalocal).ConfigureAwait(false);
                pasajerosJarvis = pasajerosJarvis.Where(P => P.Categoria.ToUpper() == "EX").ToList();

                string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitosObtenerOperacion").Value, idOperacion);
                OperacionVueloOtd respuestavuelo = await servicioApi.GetAsync<OperacionVueloOtd>(rutaRelativa).ConfigureAwait(false);

                //TODO CONSULTAR EN LA TABLA NUEVA.
                var rutaGetExentos = string.Format(configuration.GetSection("URIs:ObtenerExentos").Value,
                    respuestavuelo.Vuelo, respuestavuelo.Fecha.ToShortDateString());
                var respExentos = await servicioApi.GetAsync<IList<ExentoODT>>(rutaGetExentos).ConfigureAwait(false);

                //var respuesta = await servicioComboBox.TraerExentos(respuestavuelo.Vuelo, respuestavuelo.Fecha);
                IList<PasajeroOtd> pasajeros = new List<PasajeroOtd>();
                foreach (var item in respExentos)
                {
                    PasajeroOtd pasajeroOtd = new PasajeroOtd();
                    pasajeroOtd.Categoria = "EX";
                    pasajeroOtd.Fecha = Convert.ToDateTime(item.Fecha);
                    pasajeroOtd.MatriculaVuelo = item.Matricula.ToString();
                    pasajeroOtd.NombrePasajero = item.Pasajero;
                    //var datospasajero = pasajerosJarvis.Where(p => p.NombrePasajero == item.Pasajero.Trim().ToUpper());
                   // if (datospasajero.Count() > 0)
                   // {
                        pasajeroOtd.Origen = 0;
                        pasajeroOtd.motivo_exencion = string.Empty;  //datospasajero.FirstOrDefault().Id.ToString();
                        pasajeroOtd.realiza_viaje = item.realiza_viaje; //datospasajero.FirstOrDefault().realiza_viaje;
                        //pasajerosJarvis.Remove(datospasajero.FirstOrDefault());
                  //  }
                  
                    pasajeroOtd.NumeroVuelo = item.id_vuelo;
                    pasajeroOtd.Id = Convert.ToInt32(item.Id);

                    pasajeros.Add(pasajeroOtd);
                }
                /*
                foreach (var itemJarvis in pasajerosJarvis)
                {
                    itemJarvis.Origen = 1;
                    pasajeros.Add(itemJarvis);
                }*/

                ViewData["acciones"] = acciones;
                ViewData["operacion"] = idOperacion;
                return PartialView(pasajeros);
            }
            else {
                string rutaRelativalocal = string.Format(configuration.GetSection("URIs:PasajerosPrincipal").Value, idOperacion);
                IList<PasajeroOtd> pasajerosJarvis = await servicioApi.GetAsync<IList<PasajeroOtd>>(rutaRelativalocal).ConfigureAwait(false);
                ViewData["acciones"] = acciones;
                ViewData["operacion"] = idOperacion;
                return PartialView(pasajerosJarvis);
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public IActionResult Insertar(int idOperacion)
        {
            ViewData["IdOperacion"] = idOperacion;
            return PartialView();
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        public async Task<IActionResult> Insertar(PasajeroOtd pasajeroOtd)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string rutaRelativa = configuration.GetSection("URIs:PasajerosInsertar").Value;
                    await servicioApi.PostAsync<bool>(rutaRelativa, pasajeroOtd);

                    OperacionVueloOtd operacion = await ObtenerOperacion(pasajeroOtd.Operacion);
                    operacion.ConfirmacionPasajeros = 1;
                    await ActualizarOperacion(operacion);

                    // Guardo el archivo .txt

                    try
                    {
                        // Guarda el archivo en la ruta 
                        string carpetarReal = String.Format("{0}//{1}//{2}", User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim(), DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0'), operacion.Id.ToString());
                        var fechaReal = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        string Tipo = "PAX";
                        string cadenaPAX = string.Concat(pasajeroOtd.Fecha.ToShortDateString(), ",", pasajeroOtd.NumeroVuelo, ",", pasajeroOtd.MatriculaVuelo, ",", pasajeroOtd.NombrePasajero, ",", pasajeroOtd.Categoria);
                        string rutaFn = string.Format(configuration.GetSection("URIs:CrearArchivosCargue").Value, carpetarReal, cadenaPAX, Tipo);
                        await servicioApi.PostAsync<bool>(rutaFn, "").ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.StackTrace.ToString());
                    }


                    return Json(true);
                }

                return Json(false);

            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task<IActionResult> Actualizar(int id)
        {
            PasajeroOtd pasajero = await ObtenerPasajero(id);

            ViewData["fecha"] = pasajero.Fecha.ToString("yyyy-MM-dd");
            ViewData["vuelo"] = pasajero.NumeroVuelo;
            ViewData["matricula"] = pasajero.MatriculaVuelo;
            ViewData["operacion"] = pasajero.Operacion;
            ViewData["id"] = id;
            ViewData["nombre"] = pasajero.NombrePasajero;

            string pax = "", inf = "", ttl = "", ttc = "", ex = "", trip = "", ot = "";

            switch (pasajero.Categoria)
            {
                case "PAX":
                    pax = "selected";
                    break;
                case "INF":
                    inf = "selected";
                    break;
                case "TTL":
                    ttl = "selected";
                    break;
                case "TTC":
                    ttc = "selected";
                    break;
                case "EX":
                    ex = "selected";
                    break;
                case "TRIP":
                    trip = "selected";
                    break;
                case "OT":
                    ot = "selected";
                    break;
                default:
                    break;
            }

            string categoria = "<option value='PAX' " + pax + ">Pasajero</option>" +
                "<option value='INF'" + inf + ">Infante</option>" +
                "<option value='TTL'" + ttl + ">Tránsito en Línea</option>" +
                "<option value='TTC'" + ttc + ">Tránsito en Conexión</option>" +
                "<option value='EX'" + ex + ">Exento</option>" +
                "<option value='TRIP'" + trip + ">Tripulante</option>" +
                "<option value='OT'" + ot + ">Otros</option>";

            ViewData["categoria"] = categoria;

            string ddlRealizaViaje = "";

            if (pasajero.Categoria.Equals("EX"))
            {
                ViewData["motivo_exencion"] = pasajero.motivo_exencion;

                //if (pasajero.realiza_viaje.Equals("0"))
                //{
                //    ddlRealizaViaje = "<option value='0' selected>No</option>";
                //    ddlRealizaViaje += "<option value='1' >Si</option>";
                //}
                //else if (pasajero.realiza_viaje.Equals("1"))
                //{
                //    ddlRealizaViaje = "<option value='0' >No</option>";
                //    ddlRealizaViaje += "<option value='1' selected>Si</option>";
                //}

                ViewData["realiza_viaje"] = ddlRealizaViaje;
            }
            else
            {
                ViewData["motivo_exencion"] = "";
                ddlRealizaViaje = "<option value='1'>Si</option>";
                ddlRealizaViaje += "<option value='0' >No</option>";

                ViewData["realiza_viaje"] = ddlRealizaViaje;
            }

            return PartialView();

        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task<IActionResult> ActualizarEx(int id)
        {
            PasajeroOtd pasajero = await ObtenerPasajero(id);

            ViewData["fecha"] = pasajero.Fecha.ToString("yyyy-MM-dd");
            ViewData["vuelo"] = pasajero.NumeroVuelo;
            ViewData["matricula"] = pasajero.MatriculaVuelo;
            ViewData["operacion"] = pasajero.Operacion;
            ViewData["id"] = id;
            ViewData["nombre"] = pasajero.NombrePasajero;

            string pax = "", inf = "", ttl = "", ttc = "", ex = "", trip = "", ot = "";

            switch (pasajero.Categoria)
            {
                case "PAX":
                    pax = "selected";
                    break;
                case "INF":
                    inf = "selected";
                    break;
                case "TTL":
                    ttl = "selected";
                    break;
                case "TTC":
                    ttc = "selected";
                    break;
                case "EX":
                    ex = "selected";
                    break;
                case "TRIP":
                    trip = "selected";
                    break;
                case "OT":
                    ot = "selected";
                    break;
                default:
                    break;
            }

            string categoria = "<option value='PAX' " + pax + ">Pasajero</option>" +
                "<option value='INF'" + inf + ">Infante</option>" +
                "<option value='TTL'" + ttl + ">Tránsito en Línea</option>" +
                "<option value='TTC'" + ttc + ">Tránsito en Conexión</option>" +
                "<option value='EX'" + ex + ">Exento</option>" +
                "<option value='TRIP'" + trip + ">Tripulante</option>" +
                "<option value='OT'" + ot + ">Otros</option>";

            ViewData["categoria"] = categoria;

            string ddlRealizaViaje = "";

            if (pasajero.Categoria.Equals("EX"))
            {
                ViewData["motivo_exencion"] = pasajero.motivo_exencion;

                //if (pasajero.realiza_viaje.Equals("0"))
                //{
                //    ddlRealizaViaje = "<option value='0' selected>No</option>";
                //    ddlRealizaViaje += "<option value='1' >Si</option>";
                //}
                //else if (pasajero.realiza_viaje.Equals("1"))
                //{
                //    ddlRealizaViaje = "<option value='0' >No</option>";
                //    ddlRealizaViaje += "<option value='1' selected>Si</option>";
                //}

                ViewData["realiza_viaje"] = ddlRealizaViaje;
            }
            else
            {
                ViewData["motivo_exencion"] = "";
                ddlRealizaViaje = "<option value='1'>Si</option>";
                ddlRealizaViaje += "<option value='0' >No</option>";

                ViewData["realiza_viaje"] = ddlRealizaViaje;
            }

            return PartialView();

        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        public async Task<IActionResult> Actualizar(PasajeroOtd pasajeroOtd)
        {
            try
            {
                string rutaRelativa = configuration.GetSection("URIs:PasajerosActualizar").Value;
                await servicioApi.PostAsync<bool>(rutaRelativa, pasajeroOtd);

                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<PasajeroOtd> ObtenerPasajero(int id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:PasajerosObtenerPasajero").Value, id);
            PasajeroOtd respuesta = await servicioApi.GetAsync<PasajeroOtd>(rutaRelativa).ConfigureAwait(false);

            return respuesta;
        }


        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        public async Task<IActionResult> GuardarViajeExtento(string idExtento, string viaja, string idpasajeroJ)
        {
            try
            {
                // string rutaRelativa = string.Format(configuration.GetSection("URIs:PasajerosObtenerPasajero").Value, idpasajeroJ);
                // PasajeroOtd respuesta = await servicioApi.GetAsync<PasajeroOtd>(rutaRelativa).ConfigureAwait(false);

                PasajeroOtd respuesta = new PasajeroOtd();

                respuesta.motivo_exencion = "";
                respuesta.Id = Convert.ToInt32(idExtento);
                respuesta.realiza_viaje = viaja;

                string rutaRelativa2 = configuration.GetSection("URIs:PasajerosActualizarExtj").Value;
                await servicioApi.PostAsync<bool>(rutaRelativa2, respuesta);
                return Json(true); 
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
    }
}