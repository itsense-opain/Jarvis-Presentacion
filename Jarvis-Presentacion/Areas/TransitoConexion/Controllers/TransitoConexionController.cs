//Todo
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Opain.Jarvis.Dominio.Entidades;
//using Opain.Jarvis.Presentacion.Web.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;


//using Opain.Jarvis.Dominio.Entidades;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using System.IO;
//using System.Net.Http;
//using System.Net;
//using System.IO.Compression;
//using ClosedXML.Excel;
//using Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.Controllers;
//using iTextSharp.text.html.simpleparser;
//using DocumentFormat.OpenXml.Office2010.Excel;

//namespace Opain.Jarvis.Presentacion.Web.Areas.TransitoConexion.Controllers
//{
//    [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
//    [Area("TransitoConexion")]
//    public class TransitoConexionController : Controller
//    {
//        private readonly IConfiguration configuration;
//        private readonly IServicioApi servicioApi;
//        private readonly IEmailSender emailSender;
//        private readonly ILogger<TransitoConexionController> _logger;
//        private static List<PasajeroTransitoOtd> pasajeroTransitoOtd = new List<PasajeroTransitoOtd>();

//        public TransitoConexionController(IConfiguration cfg, IServicioApi api, IEmailSender email, ILogger<TransitoConexionController> logger)
//        {
//            configuration = cfg;
//            servicioApi = api;
//            emailSender = email;
//            _logger = logger;
//        }

//        protected async Task<IList<PasajeroTransitoOtd>> AccionesPrincipal()
//        {
//            TransitoRequest oFiltro = new TransitoRequest();
//            string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
//            IList<AerolineaOtd> lstAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
//            ViewData["Aerolineas"] = lstAerolineas;

//            //string rutaRelativa = configuration.GetSection("URIs:TransitoConexionPrincipal").Value;

//            //List<PasajeroTransitoOtd> pasajeros = new List<PasajeroTransitoOtd>();
//            Opain.Jarvis.Presentacion.Web.Bussiness.PasajerosTransito oList = new Bussiness.PasajerosTransito();            
//            IList<PasajeroTransitoOtd> oListTransitos = await oList.TransitosEnTramite(User, configuration, servicioApi, oFiltro);

//            //IList<PasajeroTransitoOtd> pasajeros = null;
//            //pasajeros = await servicioApi.GetAsync<IList<PasajeroTransitoOtd>>(rutaRelativa).ConfigureAwait(false);

//            //pasajeros = pasajeros
//            //    .Where(x => x.TTC == 1 && x.TTL == 0)
//            //    .ToList();
//            return oListTransitos;
//        }

//        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
//        public async Task<IActionResult> Principal(string Download = "")
//        {
//            ViewBag.Download = "NA";
//            if (Download != "")
//            {
//                ViewBag.Download = Download;
//                ViewBag.extension =  Path.GetExtension(Download);
//            }
//            //// Cargo las aerolineas
//            //string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
//            //IList<AerolineaOtd> lstAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
//            //ViewData["Aerolineas"] = lstAerolineas;

//            //string rutaRelativa = configuration.GetSection("URIs:TransitoConexionPrincipal").Value;

//            IList<PasajeroTransitoOtd> pasajeros = await AccionesPrincipal();
//            //    await servicioApi.GetAsync<IList<PasajeroTransitoOtd>>(rutaRelativa).ConfigureAwait(false);


//            //pasajeros = pasajeros
//            //    .Where(x => x.TTC == 1 && x.TTL == 0)
//            //    .ToList();

//            return View(pasajeros);
//        }
//        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]

//        [HttpGet]
//        public async Task<IActionResult> PrincipalDownload(string Download)
//        {
//            IList<PasajeroTransitoOtd> pasajeros = await AccionesPrincipal();

//            ViewBag.Download = false;
//            return View(pasajeros);
//        }

//        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
//        [HttpPost]
//        public async Task<IActionResult> Firmar(List<string> chkAprobar, List<string> chkRechazar, string pdfyexcel)
//        {
//            /*FileResult*/
//            //MemoryStream archivoDescarga = null;
//            string archivoDescarga = string.Empty;
//            try
//            {
//                int contAprobar = 0;
//                List<TransitoRechazado> transitosRechazados = new List<TransitoRechazado>();
//                List<PasajeroTransitoOtd> listaPasajeroTransitoOtd = new List<PasajeroTransitoOtd>();
//                int contTotal = 0;
//                int contTotalTTL = 0;
//                int contTotalTTC = 0;
//                List<int> idVuelos = new List<int>();


//                if (chkAprobar.Count > 0)
//                {
//                    DateTime fechaHoraFirma = DateTime.Now;
//                    foreach (var item in chkAprobar)
//                    {
//                        contAprobar = contAprobar + 1;
//                        contTotal = contTotal + 1;
//                        string[] datoss = item.Split(',');
//                        string id = datoss[0];
//                        string fecha = datoss[1];
//                        string hora = datoss[2];

//                        string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitoConexionFirmar").Value, id);
//                        PasajeroTransitoOtd respuesta = await servicioApi.GetAsync<PasajeroTransitoOtd>(rutaRelativa).ConfigureAwait(false);
//                        respuesta.Firmado = 1;
//                        respuesta.FechaHoraFirma = fechaHoraFirma;
//                        string rutaRelativaActualizar = configuration.GetSection("URIs:TransitoConexionFirmar2").Value;
//                        await servicioApi.PostAsync<bool>(rutaRelativaActualizar, respuesta);
//                        // Actualizo el estado de vuelo si todos los transitos en conexión de ese vuelo ya fueron firmados
//                        int IdOperacionVuelo = respuesta.Operacion;
//                        AgregarVuelo(idVuelos, respuesta.Operacion);
//                        string rutaRelativa2 = string.Format(configuration.GetSection("URIs:ActualizarVuelosFirmados").Value, IdOperacionVuelo);
//                        await servicioApi.PostAsync<int>(rutaRelativa2, IdOperacionVuelo);
//                        // Actualizo la fecha y hora
//                        PasajeroTransitoOtd transito = await ObtenerTransito(Convert.ToInt32(id));

//                        if (fecha != null)
//                        {
//                            DateTime tempDate = Convert.ToDateTime(fecha);//DateTime.ParseExact(fecha, "M/dd/yyyy", CultureInfo.InvariantCulture);
//                            transito.FechaLlegada = tempDate;
//                        }

//                        if (hora != null)
//                        {
//                            transito.HoraLlegada = hora;
//                        }

//                        string rutaRelativa22 = configuration.GetSection("URIs:TransitosActualizar").Value;
//                        await servicioApi.PostAsync<bool>(rutaRelativa22, transito);


//                        listaPasajeroTransitoOtd.Add(transito);

//                    }
//                    //Generar Reporte pdf O excel
//                    if (listaPasajeroTransitoOtd != null && listaPasajeroTransitoOtd.ToString() != "" && listaPasajeroTransitoOtd.Count >= 1)
//                    {
//                        switch (pdfyexcel)
//                        {
//                            //1
//                            case "pdf":
//                                archivoDescarga = await DescargarTransitoConexionPDF(listaPasajeroTransitoOtd);
//                                break;
//                            //2
//                            case "excel":
//                                archivoDescarga = await DescargarTransitoConexionExcel(listaPasajeroTransitoOtd);
//                                break;

//                            default:
//                                break;
//                        }
//                    }

//                }

//                if (chkRechazar.Count > 0)
//                {
//                    DateTime fechaHoraFirma = DateTime.Now;

//                    foreach (var item in chkRechazar)
//                    {
//                        contTotal = contTotal + 1;


//                        string[] datoss = item.Split(',');
//                        string id = datoss[0];
//                        string fecha = datoss[1];
//                        string hora = datoss[2];

//                        string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitoConexionFirmar").Value, id);
//                        PasajeroTransitoOtd respuesta = await servicioApi.GetAsync<PasajeroTransitoOtd>(rutaRelativa).ConfigureAwait(false);
//                        respuesta.Firmado = 2;
//                        respuesta.FechaHoraFirma = fechaHoraFirma;
//                        string rutaRelativaActualizar = configuration.GetSection("URIs:TransitoConexionFirmar2").Value;
//                        await servicioApi.PostAsync<bool>(rutaRelativaActualizar, respuesta);
//                        // Actualizo el estado de vuelo si todos los tránsitos en conexión de ese vuelo ya fueron firmados
//                        int IdOperacionVuelo = respuesta.Operacion;

//                        if (respuesta.TTC > 0)
//                        {
//                            contTotalTTC = contTotalTTC + 1;
//                        }
//                        if (respuesta.TTL > 0)
//                        {
//                            contTotalTTL = contTotalTTL + 1;
//                        }

//                        AgregarVuelo(idVuelos, respuesta.Operacion);
//                        AgregarCantidadRechazado(respuesta.Operacion, transitosRechazados, contTotalTTC, contTotalTTL);
//                        string rutaRelativa2 = string.Format(configuration.GetSection("URIs:ActualizarVuelosFirmados").Value, IdOperacionVuelo);
//                        await servicioApi.PostAsync<int>(rutaRelativa2, IdOperacionVuelo);
//                        // Actualizo la fecha y hora
//                        PasajeroTransitoOtd transito = await ObtenerTransito(Convert.ToInt32(id));

//                        if (fecha != null)
//                        {
//                            DateTime tempDate = Convert.ToDateTime(fecha);//DateTime.ParseExact(fecha, "M/dd/yyyy", CultureInfo.InvariantCulture);
//                            transito.FechaLlegada = tempDate;
//                        }

//                        if (hora != null)
//                        {
//                            transito.HoraLlegada = hora;
//                        }

//                        string rutaRelativa22 = configuration.GetSection("URIs:TransitosActualizar").Value;
//                        await servicioApi.PostAsync<bool>(rutaRelativa22, transito);
//                    }
//                }

//                // Envio mensaje de pendientes
//                try
//                {
//                    IList<PasajeroTransitoOtd> pasajeros = await AccionesPrincipal();
//                    //string rutaRelativap = configuration.GetSection("URIs:TransitoConexionPrincipal").Value;
//                    //IList<PasajeroTransitoOtd> pasajeros =
//                    //  await servicioApi.GetAsync<IList<PasajeroTransitoOtd>>(rutaRelativap).ConfigureAwait(false);

//                    //pasajeros = pasajeros
//                    //    .Where(x => x.TTC == 1 && x.TTL == 0)
//                    //    .ToList();

//                    OperacionVueloOtd oVuelo;
//                    string urlAPI = "";
//                    foreach (var idVuelo in idVuelos)
//                    {
//                        urlAPI = string.Format(configuration.GetSection("URIs:VuelosObtener").Value, idVuelo);
//                        oVuelo = await servicioApi.GetAsync<OperacionVueloOtd>(urlAPI);

//                        DateTime fechaLimite = DateTime.Now.AddDays(-1);
//                        //List<PasajeroTransitoOtd> recibidos = pasajeros.Where(x => x.FechaHoraCargue > fechaLimite && x.Operacion == idVuelo).ToList();
//                        List<PasajeroTransitoOtd> recibidos = pasajeros.Where(x => x.Operacion == idVuelo).ToList();

//                        int contAproBd = recibidos.Count(x => x.Firmado == 1);
//                        int contPendBd = recibidos.Count(x => x.Firmado == 0);

//                        if (transitosRechazados.Any(x => x.IdVuelo == idVuelo))
//                        {
//                            await this.EnviarValidacionesAutomaticas(oVuelo.Id,
//                                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadRechazos,
//                                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadTTC,
//                                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadTTL);
//                        }

//                        if (recibidos.Count > 0)
//                        {
//                            await this.EnviarCorreosTransitosAprobados(oVuelo.IdAerolinea, recibidos.Count, contAproBd, contPendBd, pasajeros, oVuelo);
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, ResourceMessage.ErrorSystem);
//                }
//                return RedirectToAction("Principal", new { Download = archivoDescarga });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, ResourceMessage.ErrorSystem);
//                throw;
//            }
//        }

//        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
//        [HttpPost]
//        public async Task<IActionResult> FirmarPDFYExcel(FirmasAprobarYRechazarYPdfyExcelOTD _FirmasAprobarYRechazarYPdfyExcelOTD)
//        {
//            /*FileResult*/
//            //MemoryStream archivoDescarga = null;
//            string archivoDescarga = string.Empty;
//            try
//            {
//                int contAprobar = 0;
//                List<TransitoRechazado> transitosRechazados = new List<TransitoRechazado>();
//                List<PasajeroTransitoOtd> listaPasajeroTransitoOtd = new List<PasajeroTransitoOtd>();
//                int contTotal = 0;
//                int contTotalTTL = 0;
//                int contTotalTTC = 0;
//                List<int> idVuelos = new List<int>();


//                if (_FirmasAprobarYRechazarYPdfyExcelOTD.chkAprobar.Count > 0)
//                {
//                    DateTime fechaHoraFirma = DateTime.Now;
//                    foreach (var item in _FirmasAprobarYRechazarYPdfyExcelOTD.chkAprobar)
//                    {
//                        contAprobar = contAprobar + 1;
//                        contTotal = contTotal + 1;
//                        string[] datoss = item.Split(',');
//                        string id = datoss[0];
//                        string fecha = datoss[1];
//                        string hora = datoss[2];

//                        string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitoConexionFirmar").Value, id);
//                        PasajeroTransitoOtd respuesta = await servicioApi.GetAsync<PasajeroTransitoOtd>(rutaRelativa).ConfigureAwait(false);
//                        respuesta.Firmado = 1;
//                        respuesta.FechaHoraFirma = fechaHoraFirma;
//                        string rutaRelativaActualizar = configuration.GetSection("URIs:TransitoConexionFirmar2").Value;
//                        await servicioApi.PostAsync<bool>(rutaRelativaActualizar, respuesta);
//                        // Actualizo el estado de vuelo si todos los transitos en conexión de ese vuelo ya fueron firmados
//                        int IdOperacionVuelo = respuesta.Operacion;
//                        AgregarVuelo(idVuelos, respuesta.Operacion);
//                        string rutaRelativa2 = string.Format(configuration.GetSection("URIs:ActualizarVuelosFirmados").Value, IdOperacionVuelo);
//                        await servicioApi.PostAsync<int>(rutaRelativa2, IdOperacionVuelo);
//                        // Actualizo la fecha y hora
//                        PasajeroTransitoOtd transito = await ObtenerTransito(Convert.ToInt32(id));

//                        if (fecha != null)
//                        {
//                            DateTime tempDate = Convert.ToDateTime(fecha);//DateTime.ParseExact(fecha, "M/dd/yyyy", CultureInfo.InvariantCulture);
//                            transito.FechaLlegada = tempDate;
//                        }

//                        if (hora != null)
//                        {
//                            transito.HoraLlegada = hora;
//                        }

//                        string rutaRelativa22 = configuration.GetSection("URIs:TransitosActualizar").Value;
//                        await servicioApi.PostAsync<bool>(rutaRelativa22, transito);


//                        listaPasajeroTransitoOtd.Add(transito);

//                    }
//                    //Generar Reporte pdf O excel
//                    if (listaPasajeroTransitoOtd != null && listaPasajeroTransitoOtd.ToString() != "" && listaPasajeroTransitoOtd.Count >= 1)
//                    {
//                        switch (_FirmasAprobarYRechazarYPdfyExcelOTD.pdfyexcel)
//                        {
//                            case "pdf":
//                                archivoDescarga = await DescargarTransitoConexionPDF(listaPasajeroTransitoOtd);
//                                break;

//                            case "excel":
//                                archivoDescarga = await DescargarTransitoConexionExcel(listaPasajeroTransitoOtd);
//                                break;

//                            default:
//                                break;
//                        }
//                    }

//                }

//                if (_FirmasAprobarYRechazarYPdfyExcelOTD.chkRechazar.Count > 0)
//                {
//                    DateTime fechaHoraFirma = DateTime.Now;

//                    foreach (var item in _FirmasAprobarYRechazarYPdfyExcelOTD.chkRechazar)
//                    {
//                        contTotal = contTotal + 1;


//                        string[] datoss = item.Split(',');
//                        string id = datoss[0];
//                        string fecha = datoss[1];
//                        string hora = datoss[2];

//                        string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitoConexionFirmar").Value, id);
//                        PasajeroTransitoOtd respuesta = await servicioApi.GetAsync<PasajeroTransitoOtd>(rutaRelativa).ConfigureAwait(false);
//                        respuesta.Firmado = 2;
//                        respuesta.FechaHoraFirma = fechaHoraFirma;
//                        string rutaRelativaActualizar = configuration.GetSection("URIs:TransitoConexionFirmar2").Value;
//                        await servicioApi.PostAsync<bool>(rutaRelativaActualizar, respuesta);
//                        // Actualizo el estado de vuelo si todos los tránsitos en conexión de ese vuelo ya fueron firmados
//                        int IdOperacionVuelo = respuesta.Operacion;

//                        if (respuesta.TTC > 0)
//                        {
//                            contTotalTTC = contTotalTTC + 1;
//                        }
//                        if (respuesta.TTL > 0)
//                        {
//                            contTotalTTL = contTotalTTL + 1;
//                        }

//                        AgregarVuelo(idVuelos, respuesta.Operacion);
//                        AgregarCantidadRechazado(respuesta.Operacion, transitosRechazados, contTotalTTC, contTotalTTL);
//                        string rutaRelativa2 = string.Format(configuration.GetSection("URIs:ActualizarVuelosFirmados").Value, IdOperacionVuelo);
//                        await servicioApi.PostAsync<int>(rutaRelativa2, IdOperacionVuelo);
//                        // Actualizo la fecha y hora
//                        PasajeroTransitoOtd transito = await ObtenerTransito(Convert.ToInt32(id));

//                        if (fecha != null)
//                        {
//                            DateTime tempDate = Convert.ToDateTime(fecha);//DateTime.ParseExact(fecha, "M/dd/yyyy", CultureInfo.InvariantCulture);
//                            transito.FechaLlegada = tempDate;
//                        }

//                        if (hora != null)
//                        {
//                            transito.HoraLlegada = hora;
//                        }

//                        string rutaRelativa22 = configuration.GetSection("URIs:TransitosActualizar").Value;
//                        await servicioApi.PostAsync<bool>(rutaRelativa22, transito);
//                    }
//                }

//                // Envio mensaje de pendientes
//                try
//                {
//                    string rutaRelativap = configuration.GetSection("URIs:TransitoConexionPrincipal").Value;
//                    IList<PasajeroTransitoOtd> pasajeros =
//                      await servicioApi.GetAsync<IList<PasajeroTransitoOtd>>(rutaRelativap).ConfigureAwait(false);

//                    pasajeros = pasajeros
//                        .Where(x => x.TTC == 1 && x.TTL == 0)
//                        .ToList();

//                    foreach (var idVuelo in idVuelos)
//                    {
//                        string rutaRelativa6 = string.Format(configuration.GetSection("URIs:VuelosObtener").Value, idVuelo);
//                        OperacionVueloOtd respuestaVuelo = await servicioApi.GetAsync<OperacionVueloOtd>(rutaRelativa6);

//                        DateTime fechaLimite = DateTime.Now.AddDays(-1);
//                        List<PasajeroTransitoOtd> recibidos = pasajeros.Where(x => x.FechaHoraCargue > fechaLimite && x.Operacion == idVuelo).ToList();

//                        int contAproBd = recibidos.Count(x => x.Firmado == 1);
//                        int contPendBd = recibidos.Count(x => x.Firmado == 0);

//                        if (transitosRechazados.Any(x => x.IdVuelo == idVuelo))
//                        {
//                            await this.EnviarValidacionesAutomaticas(respuestaVuelo.Id,
//                                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadRechazos,
//                                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadTTC,
//                                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadTTL);
//                        }

//                        if (recibidos.Count > 0)
//                        {
//                            await this.EnviarCorreosTransitosAprobados(respuestaVuelo.IdAerolinea, recibidos.Count, contAproBd, contPendBd, pasajeros, respuestaVuelo);
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, ResourceMessage.ErrorSystem);
//                }

//                //return RedirectToAction("Principal", new { id = pdfyexcel });
//                //return RedirectToAction("Index","Download");
//                //return Redirect("/consultas?pdfyexcel=" + pdfyexcel);
//                //return Content("<script>window.open('{url}','_blank')</script>");
//                return RedirectToAction("Principal", new { Download = true });


//                //switch (_FirmasAprobarYRechazarYPdfyExcelOTD.pdfyexcel)
//                //{
//                //    case "pdf":
//                //        return File(archivoDescarga.ToArray(), "application/pdf", "TransitoConexionPDF.pdf");
//                //        break;

//                //    default:
//                //        return File(archivoDescarga.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TransitoConexionExcel.xlsx");
//                //        break;

//                //}

//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, ResourceMessage.ErrorSystem);
//                throw;
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> Firmar22(int id, string Observacion)
//        {
//            string rutaRelativa2 = string.Format(configuration.GetSection("URIs:ActualizarVuelosRechazados").Value, id, Observacion);
//            await servicioApi.PostAsync<int>(rutaRelativa2, id);
//            return Json("Ok");
//        }

//        public async Task<IActionResult> EnviarValidacionesAutomaticas(int id, int contador, int CantidadTTC, int CantidadTTL)

//        {
//            string rutaRelativa2 = string.Format(configuration.GetSection("URIs:VuelosRechazadosGuardarCausa").Value, id, contador, CantidadTTC, CantidadTTL);
//            await servicioApi.PostAsync<int>(rutaRelativa2, id);
//            return Json("Ok");

//        }

//        private async Task SendMail(string _Asunto,string _Para, string _ConCopia, string _Mensaje)
//        {
//            string urlAPI = configuration.GetSection("URIs:EmailSender").Value;
//            Opain.Jarvis.Dominio.Entidades.Notificacion notificacion = new Notificacion
//            {
//                Asunto= _Asunto,
//                Destinos=_Para,
//                Copias=_ConCopia,
//                Cuerpo =_Mensaje
//            };            
            
//            bool result = await servicioApi.PostAsync<bool>(urlAPI, notificacion);
//        }
//        public async Task EnviarCorreosTransitosAprobados(int idAreolinea, 
//            int contTotal, int contAprobar, int contPendientes, 
//            IList<PasajeroTransitoOtd> pasajero,
//            OperacionVueloOtd vuelo)
//        {
//            try
//            {
//                //Notificacion 5
//                // Add la nueva solicitud de Jhon que debe consultar solo los trasito ttc y por aerolinea de llegada enviar los correo correspondiente
//                string urlAPI = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
//                IList<AerolineaOtd> lAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(urlAPI);
//                IList<AerolineaOtd> oAerolinea = lAerolineas.Where(a => a.Id == idAreolinea).ToList();

//                string NombreAerolinea = oAerolinea[0].Nombre;                

//                IEnumerable<UsuarioOtd> usuarios = await servicioApi.GetAsync<IEnumerable<UsuarioOtd>>(configuration.GetSection("URIs:UsuariosObtenerTodos").Value).ConfigureAwait(false);
//                string HostImg = configuration.GetSection("SendGrid:HostImg").Value;
//                string file = HostImg + "/images/firma.png";

//                string Asunto = "Notificación tránsitos en conexión";
//                _logger.LogInformation($"Notificación tránsitos en conexión");
//                string Para = "";
//                string ConCopia = "";
//                if (configuration.GetSection("SendGrid:cc").Value != null)
//                {
//                    ConCopia = configuration.GetSection("SendGrid:cc").Value;
//                }
//                string Mensaje = "Notificación tránsitos en conexión" +
//                    "<p>Buen día estimada aerolínea, " + NombreAerolinea  + "</p>" +                    
//                    "<p>Nos permitimos informarle que el vuelo de salida (" + vuelo.Vuelo + ")  cuenta con : </p>" + 
//                    "</br>* " + contTotal.ToString() + " tránsitos en conexión" +
//                    "</br>* " + contAprobar.ToString() + " aprobado(s) " +
//                    "</br>* " + contPendientes.ToString() + " pendiente(s) " +
//                    "</br><p><b>Nota: Recuerde que plazo máximo para la firma de tránsitos es de 24 horas.</b></p>" +
//                    "</br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <p><img src='" + file + "' width='250' height='70'/></p><p></p> " +
//                    "</br> Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ";

//                foreach (var datosUsuario in usuarios.Where(x => x.UsuarioAerolinea.Any()))
//                {
//                    foreach (var aerolinea in datosUsuario.UsuarioAerolinea)
//                    {
//                        if (idAreolinea == aerolinea.IdAerolinea)
//                        {                          
//                            Para = Para +datosUsuario.Email.Trim() + ";";
//                            _logger.LogInformation($"Notificación 5 tránsitos en conexión aprobados, Email Usuario: {datosUsuario.Email.Trim()} ");
//                        }
//                    }
//                }
//                if (Para.IndexOf(";") > 0)
//                {
//                    Para = Para.Substring(0, Para.Length - 1);
//                }
//                //await SendMail(Asunto, Para, ConCopia, Mensaje);

//                string[] emails = Para.Split(';');

//                foreach (string email in emails)
//                {
//                    try
//                    {
//                        await emailSender.SendEmailAsync(email.Trim(), Asunto, Mensaje);
//                    }
//                    catch (FormatException ex)
//                    {
//                        _logger.LogError(ex, ResourceMessage.ErrorSystem);
//                    }
//                }
                
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, ResourceMessage.ErrorSystem);
//            }
//        }

//        [HttpGet]
//        public async Task<int> PendienteAutorizarAero()
//        {
//            int pendientes = 0;
//            if (!User.IsInRole("ADMINISTRADOR"))
//            {
                
//                string aerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;
//                string siglaAerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("SiglaAerolinea")).Value;
//                string rutaRelativap = string.Format(configuration.GetSection("URIs:TransitoConexionAerolinea").Value, siglaAerolinea);
                

//                IList<PasajeroTransitoOtd> pasajeros =
//                    await servicioApi.GetAsync<IList<PasajeroTransitoOtd>>(rutaRelativap).ConfigureAwait(false);
//                pasajeros = pasajeros
//                    .Where(x => x.TTC == 1 && x.TTL == 0)
//                    .ToList();
//                DateTime fechaLimite = DateTime.Now.AddDays(-1);

//                List<PasajeroTransitoOtd> entregados = pasajeros.Where(x => x.FechaHoraCargue > fechaLimite && x.Firmado == 0).ToList();

//                if (aerolinea != string.Empty)
//                {
//                 if (!string.IsNullOrEmpty(aerolinea))
//                    {
//                        entregados = entregados.Where(x => x.AerolineaLlegada.Equals(siglaAerolinea.ToUpper())).ToList();
//                    }

//                    pendientes = entregados.Count;
//                }
//            }

//            return pendientes;
//        }

//        [HttpGet]
//        public async Task<IActionResult> ObtenerObservacionRechazo(int id)
//        {
//            string rutaRelativa2 = string.Format(configuration.GetSection("URIs:VuelosRechazadosObtener").Value, id);
//            string respuesta = await servicioApi.GetAsync<string>(rutaRelativa2);
//            if (string.IsNullOrEmpty(respuesta))
//            {
//                respuesta = string.Empty;
//            }
//            return Json(respuesta);

//        }

//        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
//        public async Task<PasajeroTransitoOtd> ObtenerTransito(int id)
//        {
//            string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitosObtenerTransito").Value, id);
//            PasajeroTransitoOtd respuesta = await servicioApi.GetAsync<PasajeroTransitoOtd>(rutaRelativa).ConfigureAwait(false);

//            return respuesta;
//        }

//        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
//        public async Task<IActionResult> Actualizar(int id)
//        {
//            PasajeroTransitoOtd transito = await ObtenerTransito(id);

//            return PartialView(transito);

//        }

//        //[Authorize(Roles = "AEROLINEA,SUPERVISOR,ADMINISTRADOR")]
//        [HttpGet]
//        public async Task<IActionResult> ActualizarOnBlur(int id, string fechaLlegada, string horaLlegada)
//        {
//            PasajeroTransitoOtd transito = await ObtenerTransito(id);


//            if (fechaLlegada != null)
//            {

//                fechaLlegada = fechaLlegada.Substring(0, 19);

//                CultureInfo culture = new CultureInfo("en-US");
//                DateTime tempDate = Convert.ToDateTime(fechaLlegada, culture);

//                transito.FechaLlegada = tempDate;
//            }

//            if (horaLlegada != null)
//                transito.HoraLlegada = horaLlegada;

//            string rutaRelativa = configuration.GetSection("URIs:TransitosActualizar").Value;

//            await servicioApi.PostAsync<bool>(rutaRelativa, transito);

//            return RedirectToAction(nameof(Principal));

//        }

//        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
//        [HttpPost]
//        public async Task<IActionResult> Actualizar(PasajeroTransitoOtd pasajeroTransito)
//        {
//            try
//            {
//                string rutaRelativa = configuration.GetSection("URIs:TransitosActualizar").Value;
//                await servicioApi.PostAsync<bool>(rutaRelativa, pasajeroTransito);

//                return RedirectToAction(nameof(Principal));
//            }
//            catch (Exception)
//            {
//                return BadRequest();
//            }
//        }

//        private void AgregarVuelo(List<int> idVuelos, int idVuelo)
//        {
//            if (!idVuelos.Contains(idVuelo))
//            {
//                idVuelos.Add(idVuelo);
//            }
//        }

//        private void AgregarCantidadRechazado(int idVuelo, List<TransitoRechazado> transitosRechazados, int CantidadTTC, int CantidadTTL)
//        {
//            if (!transitosRechazados.Any(x => x.IdVuelo == idVuelo))
//            {
//                transitosRechazados.Add(new TransitoRechazado { IdVuelo = idVuelo, CantidadRechazos = 1, CantidadTTC = CantidadTTC, CantidadTTL = CantidadTTL });
//            }
//            else
//            {
//                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadRechazos += 1;
//                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadTTC = CantidadTTC;
//                transitosRechazados.FirstOrDefault(x => x.IdVuelo == idVuelo).CantidadTTL = CantidadTTL;
//            }
//        }

//        [HttpGet]
//        public async Task<string> DescargarTransitoConexionPDF(List<PasajeroTransitoOtd> _PasajeroTransitoOtd)
//        {
//            _logger.LogInformation($"Inicio descarga PDF consulta");
//            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
//            Document pdfDoc = new Document(PageSize.LEGAL.Rotate(), 5, 5, 5,5);
//            List<PasajeroTransitoOtd> listaPasajeroTransitoOtd = new List<PasajeroTransitoOtd>();

//            try
//            {
//                _logger.LogInformation($"paso 1 descarga PDF Transito Conexion");
//                MemoryStream ms = new MemoryStream();
//                PdfWriter.GetInstance(pdfDoc, ms).CloseStream = false;

//                _logger.LogInformation($"paso 2 descarga PDF Transito Conexion");

//                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
//                pdfDoc.Open();

//                PdfPTable table = new PdfPTable(3);

//                table.WidthPercentage = 100f;
//                table.HorizontalAlignment = 1;
//                table.SpacingBefore = 1f;
//                table.SpacingAfter = 1f;
//                table.DefaultCell.Border = Rectangle.NO_BORDER;

//                iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(ruta + "\\logo-jarvis-informe.png");
//                _logger.LogInformation($"Imagen 1 descarga PDF Transito Conexion");
//                imagen.BorderWidth = 0;
//                imagen.ScalePercent(40f);
//                PdfPCell cellImg = new PdfPCell();
//                cellImg.Border = 0;
//                cellImg.HorizontalAlignment = 0;
//                cellImg.AddElement(imagen);
//                table.AddCell(cellImg);

//                Paragraph parrafo = new Paragraph();
//                parrafo.Alignment = Element.ALIGN_CENTER;
//                parrafo.Font = FontFactory.GetFont("Arial", 14);
//                parrafo.Font.SetStyle(Font.BOLD);
//                parrafo.Font.SetColor(235, 204, 64);
//                parrafo.Add("Tránsitos en Conexión ");
//                parrafo.Add(new Paragraph("\n"));
//                parrafo.Add("Fecha : " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString());
                
//                parrafo.Add(new Paragraph("\n"));
//                parrafo.Add("Hora: " + DateTime.Now.Hour.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0') + ":" + DateTime.Now.Second.ToString());


//                table.AddCell(parrafo);

//                cellImg = new PdfPCell();
//                imagen = iTextSharp.text.Image.GetInstance(ruta + "\\opain-logo-informe.png");
//                _logger.LogInformation($"Imagen 2 descarga PDF Transito Conexion");
//                imagen.BorderWidth = 0;
//                imagen.ScalePercent(45f);
//                cellImg.Border = 1;
//                cellImg.AddElement(imagen);
//                cellImg.HorizontalAlignment = 1;
//                table.AddCell(cellImg);
//                pdfDoc.Add(table);

//                // Insertamos salto de linea
//                Paragraph saltoDeLinea = new Paragraph("");
//                pdfDoc.Add(saltoDeLinea);
//                saltoDeLinea = new Paragraph("");
//                pdfDoc.Add(saltoDeLinea);
//                saltoDeLinea = new Paragraph("");
//                pdfDoc.Add(saltoDeLinea);


//                // Insertamos salto de linea
//                saltoDeLinea = new Paragraph("");
//                pdfDoc.Add(saltoDeLinea);
//                saltoDeLinea = new Paragraph("");
//                pdfDoc.Add(saltoDeLinea);

//                //Table
//                table = new PdfPTable(16);
//                table.WidthPercentage = 100f;
//                //0=Left, 1=Centre, 2=Right
//                table.HorizontalAlignment = 1;
//                table.SpacingBefore = 1f;
//                table.SpacingAfter = 1f;
//                table.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
//                table.TotalWidth = 1000f;
//                table.LockedWidth = true;
                
               
//                table.AddCell(new Paragraph("Aerolínea" + Environment.NewLine + "de" + Environment.NewLine + "Llegada", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Vuelo" + Environment.NewLine + "de" + Environment.NewLine + "Llegada", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Fecha" + Environment.NewLine + "de" + Environment.NewLine + " llegada", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Hora" + Environment.NewLine + "de" + Environment.NewLine + " llegada", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Origen", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Responsable" + Environment.NewLine + "de" + Environment.NewLine + "Aerolínea", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Aerolínea" + Environment.NewLine + "de" + Environment.NewLine + "Salida", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Vuelo Salida", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Fecha Salida", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Hora Salida", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Destino", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Nombre" + Environment.NewLine + "del" + Environment.NewLine + "Pasajero", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Responsable" + Environment.NewLine + "de" + Environment.NewLine + "Aerolínea", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Fecha firma" + Environment.NewLine + "del" + Environment.NewLine + "Tránsito", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                table.AddCell(new Paragraph("Hora firma" + Environment.NewLine + "del" + Environment.NewLine + "Tránsito", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
//                PdfPCell CCantidad = new PdfPCell(new Paragraph("Cantidad", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));
                
//                CCantidad.NoWrap = true;
//                table.AddCell(CCantidad);

//                foreach (PdfPCell celda in table.Rows[0].GetCells())
//                {
//                    celda.BackgroundColor = new iTextSharp.text.BaseColor(235, 204, 64);
//                    celda.HorizontalAlignment = 1;
//                    celda.NoWrap = true;
//                    celda.Colspan = 1;
//                }

//                _logger.LogInformation($"paso 3 descarga PDF Transito Conexion");

//                listaPasajeroTransitoOtd = _PasajeroTransitoOtd;

//                var ClaimNombre = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreUsuario"));
//                int Cantidad = 0;

//                foreach (var item in listaPasajeroTransitoOtd.ToList())
//                {

//                    string rutaRelativa = string.Format(configuration.GetSection("URIs:ConsultarIdCargue").Value, item.IdCargue);
//                    CargueOtd cargue = await servicioApi.GetAsync<CargueOtd>(rutaRelativa).ConfigureAwait(false);

//                    _logger.LogInformation($"Inicio paso 3.1 descarga PDF Transito Conexion");
//                    table.AddCell(item.AerolineaLlegada.ToString());

//                    _logger.LogInformation($"Inicio paso 3.2 descarga PDF Transito Conexion");
//                    table.AddCell(item.NumeroVueloLlegada.ToString());

//                    _logger.LogInformation($"Inicio paso 3.3 descarga PDF Transito Conexion");
//                    table.AddCell(item.FechaLlegada.Day.ToString().PadLeft(2, '0') + "/" + @item.FechaLlegada.Month.ToString().PadLeft(2, '0') + "/" + @item.FechaLlegada.Year.ToString());

//                    _logger.LogInformation($"Inicio paso 3.4 descarga PDF Transito Conexion");
//                    table.AddCell(item.HoraLlegada.ToString());

//                    _logger.LogInformation($"Inicio paso 3.5 descarga PDF Transito Conexion");
//                    table.AddCell(item.Origen.ToString());

//                    _logger.LogInformation($"Inicio paso 3.6 descarga PDF Transito Conexion");

//                    table.AddCell(cargue.NombreCompletoUsuario.ToString());
//                    //table.AddCell((User.Claims.FirstOrDefault(c => c.Type.Equals("NombreUsuario")).Value).ToString());

//                    _logger.LogInformation($"Inicio paso 3.6 descarga PDF Transito Conexion");
//                    table.AddCell((String.IsNullOrEmpty(item.AerolineaSalida) ?"": item.AerolineaSalida).ToString());
                    

//                    _logger.LogInformation($"Inicio paso 3.7 descarga PDF Transito Conexion");
//                    table.AddCell((item.NumeroVueloSalida).ToString());

//                    _logger.LogInformation($"Inicio paso 3.8 descarga PDF Transito Conexion");
//                    table.AddCell(item.FechaSalida.Day.ToString().PadLeft(2, '0') + "/" + item.FechaSalida.Month.ToString().PadLeft(2, '0') + "/" + item.FechaSalida.Year.ToString());
//                    //table.AddCell((item.FechaSalida).ToShortDateString());

//                    _logger.LogInformation($"Inicio paso 3.9 descarga PDF Transito Conexion");
//                    table.AddCell(item.HoraSalida.Substring(0, 5).ToString());

//                    _logger.LogInformation($"Inicio paso 4.0 descarga PDF Transito Conexion");
//                    table.AddCell((item.Destino).ToString());

//                    _logger.LogInformation($"Inicio paso 4.1 descarga PDF Transito Conexion");
//                    table.AddCell((item.NombrePasajero).ToString());

//                    _logger.LogInformation($"Inicio paso 4.2 descarga PDF Transito Conexion");
//                    table.AddCell((String.IsNullOrEmpty(ClaimNombre.Value) ? "" : ClaimNombre.Value).ToString());

//                    //table.AddCell((String.IsNullOrEmpty(item.NombreAerolinea) ? "" : item.NombreAerolinea).ToString());
//                    //table.AddCell("Nombre Aerolinea");

//                    _logger.LogInformation($"Inicio paso 4.3 descarga PDF Transito Conexion");
                    
//                    table.AddCell(item.FechaHoraFirma.Day.ToString().PadLeft(2, '0') + "/" + item.FechaHoraFirma.Month.ToString().PadLeft(2, '0') + "/" + item.FechaHoraFirma.Year.ToString());
//                    //table.AddCell((item.FechaHoraFirma).ToShortDateString());

//                    _logger.LogInformation($"Inicio paso 4.4 descarga PDF Transito Conexion");
//                    table.AddCell((item.FechaHoraFirma).ToString("hh:mm tt", CultureInfo.InvariantCulture));

//                    _logger.LogInformation($"Inicio paso 4.5 descarga PDF Transito Conexion");
//                    table.AddCell(("1").ToString());
//                    Cantidad = Cantidad + 1;
//                }
//                var FontColour = new BaseColor(255, 255, 255);
//                var Calibri8 = FontFactory.GetFont("Calibri", 9, FontColour);
//                for (int Pos = 1; Pos <= 14; Pos++)
//                { 
//                    PdfPCell Celda = new PdfPCell(new Paragraph("", Calibri8));
//                    Celda.Border = 0;
//                    //Celda.NoWrap = true;
//                    table.AddCell(Celda);                    
//                }
//                PdfPCell cell = new PdfPCell(new Paragraph("Total:", Calibri8));
//                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
//                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;

//                table.AddCell(cell);

//                cell = new PdfPCell(new Paragraph(Cantidad.ToString(), Calibri8));
//                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
//                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
//                //cell.NoWrap = true;
//                table.AddCell(cell);

//                _logger.LogInformation($"Sale paso 3 descarga PDF Transito Conexion");
//                //totalizado
//               /* var FontColour = new BaseColor(255, 255, 255);
//                var Calibri8 = FontFactory.GetFont("Calibri", 10, FontColour);
//                PdfPCell cell = new PdfPCell(new Paragraph("Total:", Calibri8));
//                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
//                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
//                cell.Colspan = 4;
                
//                table.AddCell(cell);
//                cell = new PdfPCell(new Paragraph(Cantidad.ToString(), Calibri8));
//                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);*/
                
//                //table.AddCell(cell);

                
//                pdfDoc.Add(table);

//                Paragraph parrafofinal = new Paragraph();
//                parrafofinal.Alignment = Element.ALIGN_CENTER;
//                parrafofinal.Font = FontFactory.GetFont("Arial", 10);
//                parrafofinal.Font.SetStyle(Font.BOLD);
//                parrafofinal.Font.SetColor(0, 0, 0);
//                var ClaimNombreUsuario = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreUsuario"));
//                parrafofinal.Add("Generado por: " + ClaimNombreUsuario.Value);
//                pdfDoc.Add(parrafofinal);

//                //new Guid();

//                _logger.LogInformation($"paso 4 descarga PDF consulta");

//                writer.CloseStream = false;
//                pdfDoc.Close();
//                byte[] bytes = ms.ToArray();

//                string RutaArchivos = configuration.GetSection("Config:RutaArchivosTransitoConexion").Value;
//                _logger.LogInformation($"paso  5 descarga PDF consulta, Ruta " + RutaArchivos);

//                using (MemoryStream stream = new MemoryStream())
//                {
//                    //if (Directory.Exists(RutaArchivos))
//                    //{
//                    //    //Directory.Delete(RutaArchivos, true);
//                    //    _logger.LogInformation($"Elimina ruta: {RutaArchivos} ");
//                    //}

//                    if (!Directory.Exists(RutaArchivos))
//                    {
//                        Directory.CreateDirectory(RutaArchivos);
//                        _logger.LogInformation($"Crea la ruta: {RutaArchivos} ");
//                    }

//                    //if (System.IO.File.Exists(RutaArchivos + "/" + nombreArchivo))
//                    //{
//                    //    System.IO.File.Delete(RutaArchivos + nombreArchivo);
//                    //}
//                    string NombraArchivo = User.Identity.Name.Trim().ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0')+ DateTime.Now.Minute.ToString().PadLeft(2, '0');
//                    using (FileStream fs = new FileStream(RutaArchivos + "/" +  NombraArchivo +".pdf", FileMode.OpenOrCreate))
//                    {
//                        fs.Write(bytes, 0, bytes.Length);
//                        fs.Close();

//                    };
//                    byte[] fileData = null;
//                    using (BinaryReader binaryReader = new BinaryReader(stream))
//                    {
//                        fileData = binaryReader.ReadBytes((int)stream.Length);
//                    }
//                    string BytesEncode = System.Convert.ToBase64String(bytes);
//                    var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
//                    string SiglaAerolinea = User.Claims.Where(c => c.Type.Equals("SiglaAerolinea")).FirstOrDefault().Value;
//                    await Opain.Jarvis.Presentacion.Web.Helpers.CargarArchivos.CargarArchArray(configuration, BytesEncode, SiglaAerolinea.Trim(), token, "TTC-"+NombraArchivo);
//                    //return stream;
//                    return NombraArchivo+".pdf";
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, ResourceMessage.ErrorSystem);
//                _logger.LogInformation($"paso 6 descarga PDF consulta, error " + ex.Message);
//                throw;
//            }

//        }

//        [HttpGet]
//        public async Task<string> DescargarTransitoConexionExcel(List<PasajeroTransitoOtd> _PasajeroTransitoOtd)
//        {
//            try
//            {
//                using (var workbook = new XLWorkbook())
//                {
//                    //Generamos la hoja
//                    var worksheet = workbook.Worksheets.Add("Tránsitos");
//                    //Generamos la cabecera

//                    worksheet.Range("A1:P1").Merge().Value = "";
//                    worksheet.Range("A1:P1").Style.Fill.BackgroundColor = XLColor.White;

//                    worksheet.Range("D3:M3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

//                    worksheet.Range("D3:M3").Merge().Value = "Tránsitos en Conexión";
//                    worksheet.Range("D2:M2").Style.Font.Bold = true;
//                    worksheet.Range("A2:P2").Style.Fill.BackgroundColor = XLColor.White;
//                    worksheet.Range("D2:L2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

//                    worksheet.Range("D4:M4").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
//                    worksheet.Range("A5:P5").Style.Fill.BackgroundColor = XLColor.White;
//                    worksheet.Range("D4:I4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

//                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
//                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("N2")).Scale(0.6);

//                    worksheet.Cell("A6").Value = "Aerolínea de Llegada";
//                    worksheet.Cell("B6").Value = "Vuelo de Llegada";
//                    worksheet.Cell("C6").Value = "Fecha de llegada";
//                    worksheet.Cell("D6").Value = "Hora de llegada ";
//                    worksheet.Cell("E6").Value = "Origen";
//                    worksheet.Cell("F6").Value = "Responsable de la Aerolínea";
//                    worksheet.Cell("G6").Value = "Aerolínea de Salida";
//                    worksheet.Cell("H6").Value = "Vuelo de Salida";
//                    worksheet.Cell("I6").Value = "Fecha de salida";
//                    worksheet.Cell("J6").Value = "Hora de salida";
//                    worksheet.Cell("K6").Value = "Destino";
//                    worksheet.Cell("L6").Value = "Nombre del Pasajero";
//                    worksheet.Cell("M6").Value = "Responsable de la Aerolínea";
//                    worksheet.Cell("N6").Value = "Fecha firma del Tránsito";
//                    worksheet.Cell("O6").Value = "Hora firma del Tránsito";
//                    worksheet.Cell("P6").Value = "Cantidad";

//                    ////-----------Le damos el formato a la cabecera----------------
//                    worksheet.Cell("A6").Style.Font.Bold = true;
//                    worksheet.Cell("A6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("B6").Style.Font.Bold = true;
//                    worksheet.Cell("B6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("C6").Style.Font.Bold = true;
//                    worksheet.Cell("C6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("D6").Style.Font.Bold = true;
//                    worksheet.Cell("D6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("E6").Style.Font.Bold = true;
//                    worksheet.Cell("E6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("F6").Style.Font.Bold = true;
//                    worksheet.Cell("F6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("G6").Style.Font.Bold = true;
//                    worksheet.Cell("G6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("H6").Style.Font.Bold = true;
//                    worksheet.Cell("H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("I6").Style.Font.Bold = true;
//                    worksheet.Cell("I6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("J6").Style.Font.Bold = true;
//                    worksheet.Cell("J6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("K6").Style.Font.Bold = true;
//                    worksheet.Cell("K6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("G6").Style.Font.Bold = true;
//                    worksheet.Cell("G6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("L6").Style.Font.Bold = true;
//                    worksheet.Cell("L6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("M6").Style.Font.Bold = true;
//                    worksheet.Cell("M6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("N6").Style.Font.Bold = true;
//                    worksheet.Cell("N6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("O6").Style.Font.Bold = true;
//                    worksheet.Cell("O6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
//                    worksheet.Cell("P6").Style.Font.Bold = true;
//                    worksheet.Cell("P6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);


//                    //-----------Genero la tabla de datos-----------
//                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    

//                    //Sacamos el usuario de la Aerolinea
//                    //CargueOtd usuario = await servicioApi.GetAsync<CargueOtd>(string.Format(configuration.GetSection("URIs:UsuariosConsultarPorAlias").Value, User.Identity.Name)).ConfigureAwait(false);




                    

//                    List<PasajeroTransitoOtd> listaPasajeroTransitoOtd = new List<PasajeroTransitoOtd>();
//                    //principal.Identity.Name
//                   listaPasajeroTransitoOtd = _PasajeroTransitoOtd;

//                    var ClaimNombre = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreUsuario"));
//                    int Cantidad = 0;
//                    foreach (var datos in listaPasajeroTransitoOtd.ToList())
//                    {
//                        worksheet.Cell(nRow, 1).Value = datos.AerolineaLlegada.ToString();
//                        worksheet.Cell(nRow, 2).Value = datos.NumeroVueloLlegada.ToString();
//                        //worksheet.Cell(nRow, 3).Value = datos.FechaLlegada.Day.ToString().PadLeft(2, '0') + "/" + datos.FechaLlegada.Month.ToString().PadLeft(2, '0') + "/" + datos.FechaLlegada.Year.ToString();
//                        worksheet.Cell(nRow, 3).Value = "'" + datos.FechaLlegada.ToString("dd/MM/yyyy");
//                        //worksheet.Cell(nRow, 3).Value = datos.FechaLlegada.ToString();
//                        worksheet.Cell(nRow, 4).Value = "'"+ datos.HoraLlegada.Substring(0,5).ToString();
//                        worksheet.Cell(nRow, 5).Value = datos.Origen.ToString();

//                        string rutaRelativa = string.Format(configuration.GetSection("URIs:ConsultarIdCargue").Value, datos.IdCargue);
//                        CargueOtd cargue = await servicioApi.GetAsync<CargueOtd>(rutaRelativa).ConfigureAwait(false);

//                        worksheet.Cell(nRow, 6).Value = cargue.NombreCompletoUsuario.ToString();
//                        worksheet.Cell(nRow, 7).Value = String.IsNullOrEmpty(datos.AerolineaSalida) ? "" : datos.AerolineaSalida; // "Aerolinea Salida";
//                        worksheet.Cell(nRow, 8).Value = datos.NumeroVueloSalida.ToString();
//                        //worksheet.Cell(nRow, 9).Value = datos.FechaSalida.Day.ToString().PadLeft(2, '0') + "/" + datos.FechaSalida.Month.ToString().PadLeft(2, '0') + "/" + datos.FechaSalida.Year.ToString();
//                        worksheet.Cell(nRow, 9).Value = "'" + datos.FechaSalida.ToString("dd/MM/yyyy");
//                        //worksheet.Cell(nRow, 9).Value = datos.FechaSalida.ToShortDateString();
//                        worksheet.Cell(nRow, 10).Value = "'" + datos.HoraSalida.Substring(0, 5).ToString();
//                        worksheet.Cell(nRow, 11).Value = datos.Destino.ToString();
//                        worksheet.Cell(nRow, 12).Value = datos.NombrePasajero.ToString();
//                        worksheet.Cell(nRow, 13).Value = ClaimNombre.Value; //usuario.Nombre; 
//                        //worksheet.Cell(nRow, 14).Value = datos.FechaHoraFirma.Day.ToString().PadLeft(2, '0') + "/" + datos.FechaHoraFirma.Month.ToString().PadLeft(2, '0') + "/" + datos.FechaHoraFirma.Year.ToString();
//                        worksheet.Cell(nRow, 14).Value = "'" + datos.FechaHoraFirma.ToString("dd/MM/yyyy");
//                        //worksheet.Cell(nRow, 14).Value = datos.FechaHoraFirma.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
//                        worksheet.Cell(nRow, 15).Value = "'" + datos.FechaHoraFirma.ToString("hh:mm tt", CultureInfo.InvariantCulture);
//                        worksheet.Cell(nRow, 16).Value = "1";    
//                        nRow++;
//                        Cantidad = Cantidad + 1;
//                    }

//                    worksheet.Cell(nRow, 14).Style.Font.Bold = true;
//                    //worksheet.Row(nRow).Style.Fill.BackgroundColor = XLColor.Black;
//                    string celda = "A" + nRow + ":O" + nRow;
//                    var rango = worksheet.Range(celda).Merge();
//                    rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
//                    worksheet.Cell(nRow, 1).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 2).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 3).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 4).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 5).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 6).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 7).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 8).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 9).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 10).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 11).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 12).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 13).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 14).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 15).Style.Fill.BackgroundColor = XLColor.Black;
//                    worksheet.Cell(nRow, 16).Style.Fill.BackgroundColor = XLColor.Black;
                   
//                    worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;

//                    worksheet.Cell(nRow, 16).Style.Font.Bold = true;
//                    //Le damos estilos a los totales sumados
//                    worksheet.Cell(nRow, 11).Style.Font.Bold = true;
//                    worksheet.Cell(nRow, 1).Value = "Totales";

//                    worksheet.Cell(nRow, 16).Value = Cantidad.ToString();
//                    worksheet.Cell(nRow, 1).Style.Font.FontColor = XLColor.White;
//                    //worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;


//                    //worksheet.Range("A" + nRow + ":P" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

//                    //worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;

//                    worksheet.Columns(1, 17).AdjustToContents(); //Ajustamos el ancho de las columnas para que se muestren todos los contenidos
//                    using (MemoryStream stream = new MemoryStream())
//                    {
//                        workbook.SaveAs(stream);//Guardamos el fichero
//                        byte[] byteRetorno = stream.ToArray();
//                        string RutaArchivos = configuration.GetSection("Config:RutaArchivosTransitoConexion").Value;

//                        //if (Directory.Exists(RutaArchivos))
//                        //{
//                        //    Directory.Delete(RutaArchivos, true);
//                        //    _logger.LogInformation($"Elimina ruta: {RutaArchivos} ");
//                        //}

//                        if (!Directory.Exists(RutaArchivos))
//                        {
//                            Directory.CreateDirectory(RutaArchivos);
//                            _logger.LogInformation($"Crea la ruta: {RutaArchivos} ");
//                        }
                        

//                        string NombraArchivo = User.Identity.Name.Trim().ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');

//                        using (FileStream fs = new FileStream(RutaArchivos + "/" + NombraArchivo + ".xlsx", FileMode.OpenOrCreate))
//                        {
//                            fs.Write(byteRetorno, 0, byteRetorno.Length);
//                        };
//                        byte[] fileData = null;
//                        using (BinaryReader binaryReader = new BinaryReader(stream))
//                        {
//                            fileData = binaryReader.ReadBytes((int)stream.Length);
//                        }
//                        string BytesEncode = System.Convert.ToBase64String(byteRetorno);
//                        var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
//                        string SiglaAerolinea = User.Claims.Where(c => c.Type.Equals("SiglaAerolinea")).FirstOrDefault().Value;
//                        await Opain.Jarvis.Presentacion.Web.Helpers.CargarArchivos.CargarArchArray(configuration, BytesEncode, SiglaAerolinea.Trim(), token, "TTC-" + NombraArchivo+ ".xlsx");

//                        return NombraArchivo + ".xlsx";
//                    };
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
        
//    }
//}