using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using iTextSharp.text.log;
using Microsoft.Extensions.Logging;
using System.Resources;
using System.Text;
using iTextSharp.text.pdf;
using Opain.Jarvis.Presentacion.Web.Bussiness;

namespace Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.Controllers
{
    [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
    [Area("CargaInformacion")]
    public class TransitosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        private readonly IEmail emailSender;
        private readonly ILogger<TransitosController> _logger;
        public TransitosController(IConfiguration cfg, IServicioApi api, IEmail email, ILogger<TransitosController> logger)
        {
            configuration = cfg;
            servicioApi = api;
            emailSender = email;
            _logger = logger;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpGet]
        public IActionResult Cargar(int idOperacion)
        {
            ViewData["operacion"] = idOperacion;
            return PartialView();
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
            string aerolineaSalida = "";
            string linea;
            CargueOtd cargue;

            _logger.LogInformation("01 TransitosController Cargar Inicio. " + idOperacionVuelo.ToString());
            try
            {
                cargueDirecto = bool.Parse(configuration.GetSection("Cofiguracion:CargueDirecto").Value);
                operacion = await ObtenerOperacion(idOperacionVuelo);
                aerolineaSalida = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex, "Err 1 TransitosController Cargar :" + ResourceMessage.ErrorSystem);
                return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
            }

            IList<PasajeroTransitoOtd> pasajerosOtd = new List<PasajeroTransitoOtd>();
            if (cargueDirecto)
            {
                try
                {
                    if (archivo.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
                    {
                        _logger.LogInformation("02 TransitosController Cargar la longitud del archivo supera el limite configurado.");
                        return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                    }
                    string rutaCargar = configuration.GetSection("URIs:CargueInsertar").Value;
                    cargue = new CargueOtd()
                    {
                        Aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value,
                        Fecha = DateTime.Now,
                        TipoArchivo = "Transitos",
                        Usuario = User.Identity.Name,
                        NombreArchivo = archivo.FileName,
                    };
                    cargue = await servicioApi.PostAsync<CargueOtd>(rutaCargar, cargue).ConfigureAwait(false);
                    if (cargue == null)
                    {
                        _logger.LogInformation("02 TransitosController Cargar fallo la Generación del IDCarga.");
                        return RedirectToAction("Principal", "Vuelos", new { mensaje = "Se presento una falla al generar el ID del identificador de la carga.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                    }
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, "Err 2 TransitosController Cargar :" + ResourceMessage.ErrorSystem);
                    return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                }

                StreamReader lineas = CargarArchivos.LeerArchivo(archivo);
                try
                {
                    _logger.LogInformation("03 TransitosController Cargar Creación del DTO. ");
                    FechaCarpeta = operacion.Fecha.ToString("yyyyMMdd");
                    NumeroVueloArchivo = operacion.Vuelo;
                    MatriculaVuelo = operacion.Matricula;
                    while ((linea = lineas.ReadLine()) != null)
                    {
                        var campos = linea.Split(",");                        
                        _logger.LogInformation($"Linea: {linea} IdOperacionVuelo: {idOperacionVuelo}");
                        pasajerosOtd.Add(new PasajeroTransitoOtd()
                        {
                            FechaLlegada = (campos[0] == "" ? new DateTime() : new DateTime(int.Parse(campos[0].Substring(6, 4)), int.Parse(campos[0].Substring(3, 2)), int.Parse(campos[0].Substring(0, 2)))),
                            HoraLlegada = (campos[1] == "" ? "" : campos[1].Substring(0, 2) + ":" + campos[1].Substring(2, 2)),
                            NumeroVueloLlegada = campos[2],
                            Origen = campos[3],
                            FechaSalida = new DateTime(int.Parse(campos[4].Substring(6, 4)), int.Parse(campos[4].Substring(3, 2)), int.Parse(campos[4].Substring(0, 2))),
                            HoraSalida = campos[5].Substring(0, 2) + ":" + campos[5].Substring(2, 2),
                            NumeroVueloSalida = campos[6],
                            Destino = campos[7],
                            NombrePasajero = campos[8],
                            TTC = int.Parse(campos[9]),
                            TTL = int.Parse(campos[10]),
                            Operacion = idOperacionVuelo,
                            //AerolineaLlegada = "NO DEFINIDA",
                            AerolineaLlegada = campos[2].Substring(0, 3),
                            AerolineaSalida = campos[6].ToString().Trim().Substring(0, 3),
                            IdCargue = cargue.Id
                        });
                    }

                    string rutaRelativa = configuration.GetSection("URIs:TransitosCargar").Value;
                    await servicioApi.PostAsync<bool>(rutaRelativa, pasajerosOtd);
                    _logger.LogInformation("03 TransitosController Cargar Salvar DTO. ");
                }
                catch (Exception Ex)
                {
                    _logger.LogError(Ex, "Err 3 TransitosController Cargar :" + ResourceMessage.ErrorSystem);
                    return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Se presento una excepción en el sistema.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                }

                try
                {
                    _logger.LogInformation("03 TransitosController Cargar Enviar archivo a la carpeta. ");

                    string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                    IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                    string Abreviatura = respuesta.Where(p => p.Id == Convert.ToInt32(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim())).FirstOrDefault().Sigla;

                    // Guarda el archivo en la ruta 
                    string CarpetaDestino = String.Format("{0}//{1}",
                        Abreviatura.Trim(),
                        FechaCarpeta);

                    _logger.LogInformation("04 TransitosController Cargar Carpeta destino. " + CarpetaDestino);

                    var NombreArchivo = string.Concat("Transitos-",
                        NumeroVueloArchivo,"-", MatriculaVuelo,
                        archivo.FileName.Substring(archivo.FileName.LastIndexOf('.'),
                        archivo.FileName.Length - (archivo.FileName.LastIndexOf('.'))));

                    _logger.LogInformation("04 TransitosController Cargar Nombre del archivo destino. " + NombreArchivo);

                    var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                    bool resultado = await CargarArchivos.Cargar(configuration, archivo, NombreArchivo, CarpetaDestino, token);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.StackTrace.ToString());
                }
            }
            else
            {
                string carpeta = "TRANSITOS";
                var fecha = string.Format("{0}{1}{2}{3}{4}{5}", operacion.Fecha.Year, operacion.Fecha.Month, operacion.Fecha.Day, operacion.Fecha.Hour, operacion.Fecha.Minute, operacion.Fecha.Second);
                var nombreArchivo = string.Format("TT{0}-{1}.txt", idOperacionVuelo, fecha);
                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                await CargarArchivos.Cargar(configuration, archivo, nombreArchivo, carpeta, token);
            }

            operacion.ConfirmacionTransitos = 1;
            await ActualizarOperacion(operacion);
            // CORREO DE TRANSITO
            // await EnviarCorreosTransitos(pasajerosOtd);

            return RedirectToAction("Principal", "Vuelos", new { cantRegistro = "0", mensaje = "Archivo de tránsitos cargado exitosamente.", fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
        }

        public async Task EnviarCorreosTransitos(IList<PasajeroTransitoOtd> pasajero)
        {
            try
            {
                string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                IList<AerolineaOtd> respuestaAero = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                IList<ContadosCorreos> contadorAreolinea = new List<ContadosCorreos>();

                foreach (var pasa in pasajero.Where(x => x.Firmado == 0).ToList())
                {
                    if (pasa.TTC == 1)
                    {
                        var contadorEmail = new ContadosCorreos();
                        if (respuestaAero.Where(p => p.Sigla.Trim() == pasa.AerolineaLlegada.Trim()).FirstOrDefault() !=null)
                        {
                            contadorEmail.IdAerolinea = respuestaAero.Where(p => p.Sigla.Trim() == pasa.AerolineaLlegada.Trim()).FirstOrDefault().Id;
                            contadorEmail.NumeroTransitos = 1;
                            contadorAreolinea.Add(contadorEmail);
                        }
                    }
                }

                IEnumerable<UsuarioOtd> usuarios = await servicioApi.GetAsync<IEnumerable<UsuarioOtd>>(configuration.GetSection("URIs:UsuariosObtenerTodos").Value).ConfigureAwait(false);
                string file = "https://" + Request.Host + "/images/firma.png";

                var results = contadorAreolinea.GroupBy(x => x.IdAerolinea)
                          .Select(x => (IdAerolinea: x.Key, NumeroTransitos: x.Select(p => p.NumeroTransitos).ToList())
                          ).ToList();

                _logger.LogInformation($"Inico notificación tránsitos en conexión inicio foreach");

                var emailUser = User.Claims.Where(c => c.Type.Equals("UsuarioEmail")).FirstOrDefault().Value;
                foreach (var resultado in results)
                {
                    _logger.LogInformation($"Notificación 4 tránsitos en conexión aprobados");
                    var ListaUsuarios = usuarios.Where(x => x.UsuarioAerolinea.Any() && (x.Perfil.Equals("AEROLINEA") || x.Perfil.Equals("SUPERVISOR")));
                    var ListaEmail = ListaUsuarios
                        .Where(y => y.UsuarioAerolinea.Any(r => r.IdAerolinea.Equals(resultado.IdAerolinea)));
                        
                    foreach(UsuarioOtd datosUsuario in ListaEmail)
                    {
                        await emailSender.SendEmailAsync(
                               datosUsuario.Email.Trim(),
                               "Notificación tránsitos en conexión 4",
                               "<p>Buen día estimada aerolínea,</p></br></br></br><p>Nos permitimos informales que tiene " + resultado.NumeroTransitos.Count + " tránsitos pendientes por su confirmación u autorización, <p></br><p>Agradecemos su gestión a la mayor brevedad posible.</p></br>  <b><p> Nota: Recordar que plazo máximo para la firma de tránsitos es de 24 horas. </p></b> </br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <p><img src='" + file + "' width='250' height='70'/></p><p></p> </br> Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");

                        await emailSender.SendEmailAsync("luis.cantin@componenteserviex.com", "Notificación 4 copia", datosUsuario.Email.Trim());

                        _logger.LogInformation($"Notificación 4 tránsitos en conexión aprobados, Email Usuario: {datosUsuario.Email.Trim()} ");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                throw;
            }
        }


        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<OperacionVueloOtd> ObtenerOperacion(int id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitosObtenerOperacion").Value, id);
            OperacionVueloOtd respuesta = await servicioApi.GetAsync<OperacionVueloOtd>(rutaRelativa).ConfigureAwait(false);

            return respuesta;

        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task ActualizarOperacion(OperacionVueloOtd operacionVueloOtd)
        {
            string rutaRelativa = configuration.GetSection("URIs:TransitosActualizarOperacion").Value;
            await servicioApi.PostAsync<bool>(rutaRelativa, operacionVueloOtd).ConfigureAwait(false);
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> Principal(int idOperacion, int acciones, string retornopdfotxt = "")
        {

            if (retornopdfotxt != string.Empty && retornopdfotxt != null)
            {
                ViewData["retornopdfotxt"] = retornopdfotxt;
                ViewData["filepath"] = configuration.GetSection("Config:RutaArchivosTransitoManual").Value;
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitosPrincipal").Value, idOperacion);

            IList<PasajeroTransitoOtd> pasajeros = await servicioApi.GetAsync<IList<PasajeroTransitoOtd>>(rutaRelativa).ConfigureAwait(false);

            ViewData["acciones"] = acciones;
            ViewData["operacion"] = idOperacion;
            return PartialView(pasajeros);
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task<IActionResult> Insertar(int idOperacion)
        {
            ViewData["IdOperacion"] = idOperacion;
            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            ViewData["Aerolineas"] = respuesta.Select(n => new SelectListItem
            {
                Value = n.Sigla.ToString(),
                Text = n.Nombre.ToString()
            }).ToList();
            ViewBag.fechalleDef = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0'); 
            return PartialView();
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        public async Task<IActionResult> Insertar(PasajeroTransitoOtd pasajeroTransito, string categoria, string AerolineaLlegada)
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                string aerolineaSalida = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;                

                pasajeroTransito.AerolineaSalida = aerolineaSalida;

                if (categoria == "TTL")
                {
                        pasajeroTransito.TTL = 1;
                        pasajeroTransito.TTC = 0;                        
                }
                else
                {
                        pasajeroTransito.TTL = 0;
                        pasajeroTransito.TTC = 1;                        
                }

                pasajeroTransito.AerolineaLlegada = pasajeroTransito.AerolineaLlegada;

                pasajeroTransito.AerolineaSalida = pasajeroTransito.AerolineaSalida;

                OperacionVueloOtd operacion = await ObtenerOperacion(pasajeroTransito.Operacion);

                    operacion.ConfirmacionTransitos = 1;

                    await ActualizarOperacion(operacion);

                    string rutaRelativa = configuration.GetSection("URIs:TransitosInsertar").Value;
                    await servicioApi.PostAsync<bool>(rutaRelativa, pasajeroTransito);

                    // Guardo el archivo .txt

                    try
                    {
                        // Guarda el archivo en la ruta 
                        string carpetarReal = String.Format("{0}//{1}//{2}", User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim(), DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0'), operacion.Id.ToString());
                        var fechaReal = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        string Tipo = "TT";
                        string cadenaTT = string.Concat(pasajeroTransito.FechaLlegada.ToShortDateString(), ",", pasajeroTransito.HoraLlegada, ",", pasajeroTransito.NumeroVueloLlegada, ",", pasajeroTransito.Origen, ",", pasajeroTransito.FechaSalida.ToShortDateString(),",",pasajeroTransito.HoraSalida,",",pasajeroTransito.NumeroVueloSalida,",",pasajeroTransito.Destino,",",pasajeroTransito.NombrePasajero,",",pasajeroTransito.TTC,",",pasajeroTransito.TTL);
                        string rutaFn = string.Format(configuration.GetSection("URIs:CrearArchivosCargue").Value, carpetarReal, cadenaTT, Tipo);
                        await servicioApi.PostAsync<bool>(rutaFn, "").ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                    Console.Write(ex.StackTrace.ToString());
                }

                return Json(true);
                //}

                //return Json(false);
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return Json(false);
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<PasajeroTransitoOtd> ObtenerTransito(int id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitosObtenerTransito").Value, id);
            PasajeroTransitoOtd respuesta = await servicioApi.GetAsync<PasajeroTransitoOtd>(rutaRelativa).ConfigureAwait(false);

            return respuesta;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task<IActionResult> Actualizar(int id)
        {
            
            PasajeroTransitoOtd transito = await ObtenerTransito(id);
            ViewData["operacion"] = transito.Operacion;
            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            ViewData["Aerolineas"] = respuesta.Select(n => new SelectListItem
            {
                Value = n.Sigla.ToString(),
                Text = n.Nombre.ToString()
            }).ToList();
            return PartialView(transito);

        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        public async Task<IActionResult> Actualizar(PasajeroTransitoOtd pasajeroTransito, string categoria, string pdfyTxt)
        {
            try
            {
                string aerolineaSalida = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;
                
                pasajeroTransito.AerolineaSalida = aerolineaSalida;

                if (categoria == "TTL")
                {
                    pasajeroTransito.TTL = 1;
                    pasajeroTransito.TTC = 0;                    
                }
                else
                {
                    pasajeroTransito.TTL = 0;
                    pasajeroTransito.TTC = 1;
                    
                }

                pasajeroTransito.AerolineaLlegada = pasajeroTransito.AerolineaLlegada;

                string rutaRelativa = configuration.GetSection("URIs:TransitosActualizar").Value;
                await servicioApi.PostAsync<bool>(rutaRelativa, pasajeroTransito);



                var retornopdfotxt = string.Empty;
                if (pdfyTxt.ToString() == "pdf")
                    retornopdfotxt = await TransitoManualPDF(pasajeroTransito, categoria);
                else
                {
                    retornopdfotxt = await TransitoManualTXT(pasajeroTransito, categoria);
                }

                string _rutaUrl = string.Empty;
                _rutaUrl = "/Download/TransitoManual?filepath=" + configuration.GetSection("Config:RutaArchivosTransitoManual").Value + "\\" + retornopdfotxt;

                RutaUrlYNombreArchivooOtd _rutaUrlYNombreArchivo = new RutaUrlYNombreArchivooOtd();
                _rutaUrlYNombreArchivo.RutaUrl = _rutaUrl;
                _rutaUrlYNombreArchivo.NombreArchivo = retornopdfotxt;
                return Json(_rutaUrlYNombreArchivo);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }



        [HttpGet]
        public async Task<string> TransitoManualTXT(PasajeroTransitoOtd _pasajeroTransitoOtd, string categoria)
        {
            string RutaArchivos = configuration.GetSection("Config:RutaArchivosTransitoManual").Value;
            // File name  
            string NombraArchivo = User.Identity.Name.Trim().ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');

            try
            {
                NombraArchivo = NombraArchivo + ".txt";
                string fileName = RutaArchivos + "\\" + NombraArchivo;

                FileStream stream = null;
                // Create a FileStream with mode CreateNew  
                stream = new FileStream(fileName, FileMode.OpenOrCreate);

                // Create a StreamWriter from FileStream  
                string Categoria = categoria.ToString() == "TTL" ? "Transito en Linea" : "Transito en Conexion";
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                    await writer.WriteLineAsync(
                        _pasajeroTransitoOtd.FechaLlegada.ToString("dd-MM-yyyy") + "," // Fecha de Vuelo
                        + _pasajeroTransitoOtd.HoraLlegada.ToString() + ","
                        + _pasajeroTransitoOtd.NumeroVueloLlegada.ToString() + ","
                        + _pasajeroTransitoOtd.Origen.ToString() + ","
                        + _pasajeroTransitoOtd.FechaSalida.ToString("dd-MM-yyyy") + ","
                        + _pasajeroTransitoOtd.HoraSalida.ToString() + ","
                        + _pasajeroTransitoOtd.NumeroVueloSalida.ToString() + ","
                        + _pasajeroTransitoOtd.Destino.ToString() + ","
                        + _pasajeroTransitoOtd.NombrePasajero.ToString().Trim() + ","
                        + Categoria.ToString());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                _logger.LogInformation($"paso 6 GENERAR TXT , error " + ex.Message);
                throw;
            }
            return NombraArchivo;

        }

        

        public async Task<string> TransitoManualPDF(PasajeroTransitoOtd _pasajeroTransitoOtd,string categoria)
        {
            
            string filePath = configuration.GetSection("Config:RutaPlantillas").Value;

            string filePathTransitoManual = configuration.GetSection("Config:RutaArchivosTransitoManual").Value;

            string fileNameExisting = @"PlantillaPDFTransitoManual.pdf";

            string NombraArchivo = User.Identity.Name.Trim().ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');

            string fileNameNew = NombraArchivo + ".pdf";

            string fullNewPath = Path.Combine(filePathTransitoManual + "\\" + fileNameNew);
            string fullExistingPath = Path.Combine(filePath + "\\" + fileNameExisting);

            using (var existingFileStream = new FileStream(fullExistingPath, FileMode.Open))

            using (var newFileStream = new FileStream(fullNewPath, FileMode.Create))
            {
                // Open existing PDF
                var pdfReader = new PdfReader(existingFileStream);

                // PdfStamper, which will create
                var stamper = new PdfStamper(pdfReader, newFileStream);

                AcroFields fields = stamper.AcroFields;
                fields.SetField("FechaLlegada", _pasajeroTransitoOtd.FechaLlegada.ToString("dd-MM-yyyy"));
                fields.SetField("HoraLlegada", _pasajeroTransitoOtd.HoraLlegada.ToString());
                fields.SetField("VueloLlegada", _pasajeroTransitoOtd.NumeroVueloLlegada.ToString());
                fields.SetField("Origen", _pasajeroTransitoOtd.Origen.ToString());
                fields.SetField("FechaSalida", _pasajeroTransitoOtd.FechaSalida.ToString("dd-MM-yyyy"));
                fields.SetField("HoraSalida", _pasajeroTransitoOtd.HoraSalida.ToString());
                fields.SetField("VueloSalida", _pasajeroTransitoOtd.NumeroVueloSalida.ToString());
                fields.SetField("Destino", _pasajeroTransitoOtd.Destino.ToString());
                fields.SetField("NombrePasajero", _pasajeroTransitoOtd.NombrePasajero.ToString().Trim());
                fields.SetField("Categoria", categoria .ToString()== "TTL" ? "Transito en Linea" : "Transito en Conexion");

                //fields.SetField("Date", DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-ES")));

                // "Flatten" the form so it wont be editable/usable anymore
                stamper.FormFlattening = true;

                stamper.Close();
                pdfReader.Close();

                return fileNameNew;
            }
        }




    }
}