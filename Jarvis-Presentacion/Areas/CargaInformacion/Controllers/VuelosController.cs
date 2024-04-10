
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.Controllers
{
    [Area("CargaInformacion")]
    public partial class VuelosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly IEmailSender emailSender;
        private readonly ILogger<VuelosController> _logger;
        private readonly ServicioComboBox servicioComboBox;

        private bool ControlArchivos(System.Security.Claims.ClaimsPrincipal Identity, IList<OperacionVueloOtd> List)
        {
            string SiglaAerolinea = Identity.Claims.Where(c => c.Type.Equals("SiglaAerolinea")).FirstOrDefault().Value.Trim();
            string PrefijoArchivoAerolinea = List[0].Vuelo.Substring(0, 3).Trim();
            if (SiglaAerolinea != PrefijoArchivoAerolinea)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> SubirVuelo(OperacionVueloOtd ParametrosServicio)
        {
            bool retono = false;
            try
            {
                string rutaRelativa = configuration.GetSection("URIs:VuelosCargarAerolinea").Value;
                var RespuestaApiCargaVueloAerolinea = await servicioApi.PostAsync<OperacionVueloOtd>(rutaRelativa, ParametrosServicio);
                if (RespuestaApiCargaVueloAerolinea != null)
                {
                    retono = true;
                }
                else
                {
                    _logger.LogError("Error método SubirVuelo : no se ejecuto exitoramente para el vuelo " + ParametrosServicio.IdCargue,
                        ResourceMessage.ErrorSystem);
                }
            }
            catch (Exception Excepcion)
            {
                _logger.LogError("Error método SubirVuelo : " + Excepcion.Message, ResourceMessage.ErrorSystem);
            }
            return retono;
        }

        private int ObtenerIDAerolinea(System.Security.Claims.ClaimsPrincipal Identity)
        {
            try
            {
                int ID = int.Parse(Identity.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim());
                return ID;
            }
            catch (Exception Exception)
            {
                return 0;
            }
        }

        public VuelosController(IConfiguration cfg, IServicioApi api, IHttpContextAccessor httpContextAccessor, IEmailSender email, ILogger<VuelosController> logger, ServicioComboBox servicioComboBox1)
        {
            configuration = cfg;
            servicioApi = api;
            this.HttpContextAccessor = httpContextAccessor;
            emailSender = email;
            _logger = logger;
            servicioComboBox = servicioComboBox1;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> Principal(string cantRegistro = "", string mensaje = "", string IdOperacionVuelo = "", string retornopdfotxt="", string fechaInicio="", 
            string fechaFinal="", string numeroVuelo="", string fechaInicioHistorico = "", string fechaFinalHistorico = "", string tabActiva = "", string tipoVuelo = "", string tipoVueloHistorico = "")
        {
            OperacionVueloOTDRequest oFiltro = new OperacionVueloOTDRequest();
            ViewData["mensaje"] = mensaje;
            ViewData["IdOperacionVuelo"] = IdOperacionVuelo;
            ViewData["cantRegistros"] = "";
            try
            {
                if (retornopdfotxt != string.Empty && retornopdfotxt != null)
                {
                    ViewData["retornopdfotxt"] = retornopdfotxt;
                    ViewData["filepath"] = configuration.GetSection("Config:RutaArchivosCierreManual").Value;
                }

                
                IList<OperacionVueloOtd> vuelosHistoricos;

                if (string.IsNullOrEmpty(fechaInicio) && string.IsNullOrEmpty(fechaFinal) && string.IsNullOrEmpty(numeroVuelo))
                {
                    var dia = DateTime.Now.Day;
                    int diafinal = 0;
                    if (dia > 15)
                        diafinal = dia - 30 + 17;
                    else
                        diafinal = dia + 2;

                    DateTime fechainicial = DateTime.Now.AddDays(-diafinal);
                    ViewBag.fechaInidefault = fechainicial.Year.ToString() + "-" + fechainicial.Month.ToString().PadLeft(2, '0') + "-" + fechainicial.Day.ToString().PadLeft(2, '0');
                    ViewBag.fechaFindefault = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0');
                    DateTime fechainicialHistorico = DateTime.Now.AddDays(-diafinal);
                    ViewBag.fechaInicioHistoricoDefault = fechainicialHistorico.Year.ToString() + "-" + fechainicialHistorico.Month.ToString().PadLeft(2, '0') + "-" + fechainicialHistorico.Day.ToString().PadLeft(2, '0');
                    ViewBag.fechaFinHistoricoDefault = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0');
                    TempData["fechaInicio"] = ViewBag.fechaInidefault;
                    TempData["fechaFinal"] = ViewBag.fechaFindefault;
                    oFiltro.FechaDesde = ViewBag.fechaInidefault;
                    oFiltro.FechaHasta = ViewBag.fechaFindefault;
                    vuelosHistoricos = await ObtenerTodosAsync(
                        fechainicialHistorico.Day.ToString().PadLeft(2, '0') + "-" + fechainicialHistorico.Month.ToString().PadLeft(2, '0') + "-" + fechainicialHistorico.Year.ToString(),
                        DateTime.Now.Day.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Year.ToString(), ""
                    );
                }
                else
                {
                    DateTime fechainicial = DateTime.ParseExact(fechaInicio, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime fechafin = DateTime.ParseExact(fechaFinal, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    if (string.IsNullOrEmpty(fechaInicioHistorico))
                        fechaInicioHistorico = fechaInicio;
                    if (string.IsNullOrEmpty(fechaFinalHistorico))
                        fechaFinalHistorico = fechaFinal;

                    DateTime fechainicialhistorico = DateTime.ParseExact(fechaInicioHistorico, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime fechafinhistorico = DateTime.ParseExact(fechaFinalHistorico, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    ViewBag.fechaInicioHistoricoDefault = fechainicialhistorico.Year.ToString() + "-" + fechainicialhistorico.Month.ToString().PadLeft(2, '0') + "-" + fechainicialhistorico.Day.ToString().PadLeft(2, '0');
                    ViewBag.fechaFinHistoricoDefault = fechafinhistorico.Year.ToString() + "-" + fechafinhistorico.Month.ToString().PadLeft(2, '0') + "-" + fechafinhistorico.Day.ToString().PadLeft(2, '0');
                    ViewBag.fechaInidefault = fechainicial.Year.ToString() + "-" + fechainicial.Month.ToString().PadLeft(2, '0') + "-" + fechainicial.Day.ToString().PadLeft(2, '0');
                    ViewBag.fechaFindefault = fechafin.Year.ToString() + "-" + fechafin.Month.ToString().PadLeft(2, '0') + "-" + fechafin.Day.ToString().PadLeft(2, '0');
                    ViewBag.numeroVuelo = numeroVuelo;

                    ViewBag.chkVueloNacional = tipoVuelo;
                    ViewBag.chkVueloInternacional = tipoVuelo;
                    ViewBag.chkVueloNacionalHistorico = tipoVueloHistorico;
                    ViewBag.chkVueloInternacionalHistorico = tipoVueloHistorico;

                    TempData["fechaInicio"] = ViewBag.fechaInidefault;
                    TempData["fechaFinal"] = ViewBag.fechaFindefault;
                    TempData["numeroVuelo"] = ViewBag.numeroVuelo;
                    oFiltro.FechaDesde = ViewBag.fechaInidefault;
                    oFiltro.FechaHasta = ViewBag.fechaFindefault;
                    oFiltro.Vuelo = numeroVuelo;
                    oFiltro.Tipo = string.IsNullOrEmpty(tipoVuelo)?"":tipoVuelo;

                    vuelosHistoricos = await ObtenerTodosAsync(
                        fechainicialhistorico.Day.ToString().PadLeft(2, '0') + "-" + fechainicialhistorico.Month.ToString().PadLeft(2, '0') + "-" + fechainicialhistorico.Year.ToString(),
                        fechafinhistorico.Day.ToString().PadLeft(2, '0') + "-" + fechafinhistorico.Month.ToString().PadLeft(2, '0') + "-" + fechafinhistorico.Year.ToString(),
                        tipoVueloHistorico
                    );
                }
                //IList<OperacionVueloOtd> vuelosConsultados = await ObtenerTodosAsync(ViewBag.fechaInidefault, ViewBag.fechaFindefault);
                //var vuelos = vuelosConsultados.Where(p => p.EstadoProceso != "0" && p.EstadoProceso != "2").ToList();

                Opain.Jarvis.Presentacion.Web.Bussiness.OperacionesVuelo oList = new Bussiness.OperacionesVuelo();
                IList<OperacionVueloOtd> oListVuelos = await oList.ObtenerVuelosPendientesProcesar(User, configuration, servicioApi, oFiltro);
                var contadorVuelos = oListVuelos.Where(p => p.EstadoProceso == "1").ToList();
                ViewData["cantRegistros"] = contadorVuelos.Count();

                await ObtenerOOperacionVueloTodos();
                //await ObtenerVuelosPendientesAsync(1);
                ViewBag.VuelosPendientes = oListVuelos;
                string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
                IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);

                if (User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
                {
                    var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;
                    ViewData["Aerolineas"] = respuesta.Where(x => x.Nombre.Equals(aerolinea)).Select(n => new SelectListItem
                    {
                        Value = n.Id.ToString(),
                        Text = n.Nombre.ToString()
                    }).ToList();
                }
                else
                {
                    ViewData["Aerolineas"] = respuesta.Select(n => new SelectListItem
                    {
                        Value = n.Id.ToString(),
                        Text = n.Nombre.ToString()
                    }).ToList();
                }
                ViewBag.tabActiva = (string.IsNullOrEmpty(tabActiva) || tabActiva.Equals("1")) ? "1" : "2";
                //return View(oListVuelos);
                return View("Principal", vuelosHistoricos.Where(p => p.EstadoProceso != "0" && p.EstadoProceso != "2").ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                throw;
            }

        
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> VuelosHistoricoFechas(string fechaInicioHistorico, string fechaFinalHistorico, string numeroVuelo)
        {
            ViewData["cantRegistros"] = "";
            TempData["fechaInicio"] = fechaInicioHistorico;
            TempData["fechaFinal"] = fechaFinalHistorico;

            var vuelos = await ObtenerTodosAsync(fechaInicioHistorico, fechaFinalHistorico, "");
            ViewData["mensaje"] = "";

            await ObtenerVuelosPendientesAsync(1);
            //vuelos = vuelos.Where(x => x.ConfirmacionOperacion == 1).ToList();
            ViewBag.tabActiva = "2";

            if (string.IsNullOrEmpty(fechaInicioHistorico) && string.IsNullOrEmpty(fechaInicioHistorico))
            {
                DateTime fechainicial = DateTime.Now.AddMonths(-1);
                ViewBag.fechaInicioHistoricoDefault = fechainicial.Year.ToString() + "-" + fechainicial.Month.ToString().PadLeft(2, '0') + "-" + fechainicial.Day.ToString().PadLeft(2, '0');
                ViewBag.fechaFinHistoricoDefault = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0');
            }
            else
            {
                DateTime fechainicial = DateTime.ParseExact(fechaInicioHistorico, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                DateTime fechafin = DateTime.ParseExact(fechaFinalHistorico, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                ViewBag.fechaInicioHistoricoDefault = fechainicial.Year.ToString() + "-" + fechainicial.Month.ToString().PadLeft(2, '0') + "-" + fechainicial.Day.ToString().PadLeft(2, '0');
                ViewBag.fechaFinHistoricoDefault = fechafin.Year.ToString() + "-" + fechafin.Month.ToString().PadLeft(2, '0') + "-" + fechafin.Day.ToString().PadLeft(2, '0');
                ViewBag.numeroVuelo = numeroVuelo;
            }
            
            IList<OperacionVueloOtd> vuelosConsultados = await ObtenerTodosAsync(ViewBag.fechaInidefault, ViewBag.fechaFindefault, "");
            var vuelosreales = vuelosConsultados.Where(p => p.EstadoProceso != "0" && p.EstadoProceso != "2").ToList();

            var contadorVuelos = vuelosConsultados.Where(p => p.EstadoProceso == "1").ToList();
            ViewData["cantRegistros"] = contadorVuelos.Count();
            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);

            if (User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
            {
                var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;

                ViewData["Aerolineas"] = respuesta.Where(x => x.Nombre.Equals(aerolinea)).Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Nombre.ToString()
                }).ToList();
            }
            else
            {
                ViewData["Aerolineas"] = respuesta.Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Nombre.ToString()
                }).ToList();
            }

            return View("Principal", vuelos.Where(p => p.EstadoProceso != "0" && p.EstadoProceso != "2").ToList());
            // return ViewComponent("VuelosHistoricos", vuelos);
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> VuelosHistoricos(string fechaInicio, string fechaFinal,string tipoVuelo)
        {
            TempData["fechaInicio"] = fechaInicio;
            TempData["fechaFinal"] = fechaFinal;
            OperacionVueloOTDRequest oFiltro = new OperacionVueloOTDRequest();
            oFiltro.FechaDesde = fechaInicio;
            oFiltro.FechaHasta = fechaFinal;
            oFiltro.Tipo = string.IsNullOrEmpty(tipoVuelo) ? "" : tipoVuelo;

            //var vuelosAll = await ObtenerTodosAsync(fechaInicio, fechaFinal);
            //var vuelos = vuelosAll.Where(p => p.EstadoProceso != "0" && p.EstadoProceso != "2").ToList();
            Opain.Jarvis.Presentacion.Web.Bussiness.OperacionesVuelo oList = new Bussiness.OperacionesVuelo();
            IList<OperacionVueloOtd> oListVuelos = await oList.ObtenerVuelosPendientesProcesar(User, configuration, servicioApi, oFiltro);

            return Json(oListVuelos.ToList());
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IList<OperacionVueloOtd>> ObtenerTodosAsync(string fechaInicio, string fechaFinal, string tipoVueloHistorico)
        {

            TempData["fechaInicio"] = fechaInicio;
            TempData["fechaFinal"] = fechaFinal;

            string bandera = "0";
            string rutaRelativa = "";

            int IDAerolinea = ObtenerIDAerolinea(User);
            rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodosAerolinea").Value, IDAerolinea);
            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            {
                rutaRelativa = rutaRelativa + "&fechaVueloInicio={0}&fechaVueloFinal={1}&bandera={2}&tipoVueloHistorico={3}";
                rutaRelativa = string.Format(rutaRelativa, fechaInicio, fechaFinal, bandera, tipoVueloHistorico);
            }
            if (User.IsInRole("ADMINISTRADOR"))
            {
                bandera = "1";
                rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos").Value, bandera);
                if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
                    rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFinal, bandera);
            }

            IList<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);

            //if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR"))
            //{
            //    string aerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;
            //    respuesta = respuesta.Where(x => x.NombreAerolinea.ToLower().Equals(aerolinea.ToLower())).ToList();
            //}

            return respuesta;
        }

        public async Task<IList<HorarioOperacionOtd>> ObtenerOOperacionVueloTodos()
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:HorarioOperacionPrincipal").Value);
            IList<HorarioOperacionOtd> respuesta = await servicioApi.GetAsync<IList<HorarioOperacionOtd>>(rutaRelativa);
            string diasemana = DateTime.Now.DayOfWeek.ToString().ToUpper();
            string diaformateado = "";
            switch (diasemana)
            {
                case "FRIDAY":
                    diaformateado = "V";
                    break;
                case "THURSDAY":
                    diaformateado = "J";
                    break;
                case "WEDNESDAY":
                    diaformateado = "W";
                    break;
                case "TUESDAY":
                    diaformateado = "M";
                    break;
                case "MONDAY":
                    diaformateado = "L";
                    break;
                case "SUNDAY":
                    diaformateado = "D";
                    break;
                case "SATURDAY":
                    diaformateado = "S";
                    break;
                default:
                    break;
            }
            if (respuesta.Where(P => P.Dia == diaformateado).Count() > 0)
            {
                HorarioOperacionOtd horarioAero = respuesta.Where(P => P.Dia == diaformateado).FirstOrDefault();
                // verifico las horas
                DateTime fechaini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(horarioAero.HoraInicio.Substring(0, 2)), Convert.ToInt32(horarioAero.HoraInicio.Substring(3, 2)), 0);
                DateTime fechafin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(horarioAero.HoraFin.Substring(0, 2)), Convert.ToInt32(horarioAero.HoraFin.Substring(3, 2)), 0);
                // verifico que estè entre los parametros de horas permitidas
                if (DateTime.Now >= fechaini && DateTime.Now <= fechafin)
                {
                    ViewBag.HabilitaCargue = "1";
                }
                else
                {
                    ViewBag.HabilitaCargue = "0";
                }

            }
            else { ViewBag.HabilitaCargue = "1"; }
            return respuesta;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IList<OperacionVueloOtd>> ObtenerVuelosFiltradosAsync(string fechaInicio, string fechaFinal, int aerolinea)
        {
            TempData["fechaInicio"] = fechaInicio;
            TempData["fechaFinal"] = fechaFinal;

            string bandera = "0";

            if (User.IsInRole("ADMINISTRADOR"))
            {
                bandera = "1";
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos").Value, bandera);

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
                rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFinal, bandera);

            IList<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);

            if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR"))
            {
                respuesta = respuesta.Where(x => x.IdAerolinea.Equals(aerolinea) && (x.EstadoProceso.Equals("2") || x.EstadoProceso.Equals("1"))).ToList();
            }

            return respuesta;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        private async Task<IList<OperacionVueloOtd>> ObtenerVuelosFiltradosTransitosFirmadosAsync(string fechaInicio, string fechaFinal, string numeroVuelo, string tipoVuelo, int aerolinea)
        {
            TempData["fechaInicio"] = fechaInicio;
            TempData["fechaFinal"] = fechaFinal;

            OperacionVueloOTDRequest oFiltro = new OperacionVueloOTDRequest();
            DateTime fechainicial = DateTime.ParseExact(fechaInicio, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime fechafin = DateTime.ParseExact(fechaFinal, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            oFiltro.FechaDesde = fechainicial.Year.ToString() + "-" + fechainicial.Month.ToString().PadLeft(2, '0') + "-" + fechainicial.Day.ToString().PadLeft(2, '0');
            oFiltro.FechaHasta = fechafin.Year.ToString() + "-" + fechafin.Month.ToString().PadLeft(2, '0') + "-" + fechafin.Day.ToString().PadLeft(2, '0');
            oFiltro.Vuelo = numeroVuelo;
            oFiltro.Tipo = string.IsNullOrEmpty(tipoVuelo) ? "" : tipoVuelo;
            //string bandera = "LIQ";

            //if (User.IsInRole("ADMINISTRADOR"))
            //{
            //    bandera = "1";
            //}

            //string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos").Value, bandera);
            //string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodosAerolinea").Value, aerolinea.ToString());
            //rutaRelativa = rutaRelativa + "&bandera="+bandera.ToString();

            //if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
            //    rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFinal, bandera);

            //IList<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);

            Opain.Jarvis.Presentacion.Web.Bussiness.OperacionesVuelo oList = new Bussiness.OperacionesVuelo();
            IList<OperacionVueloOtd> oListVuelos = await oList.ObtenerVuelosLiquidar(User, configuration, servicioApi, oFiltro);

            //if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR"))
            //{
            //    respuesta = respuesta.Where(x => x.IdAerolinea.Equals(aerolinea) && (x.EstadoProceso.Equals("2") || x.EstadoProceso.Equals("1"))).ToList();
            //}

            return oListVuelos;
        }


        private async Task<IList<OperacionVueloOtd>> VuelosPendientesConfirmarObtenerTodos()
        {
            string IdAerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value;
            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosPendientesConfirmarObtenerTodos").Value, IdAerolinea);

            IList<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);

            return respuesta;
        }

        public async Task<IList<OperacionVueloOtd>> ObtenerVuelosPendienteConfirmar()
        {
            string IdAerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value;
            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosPendientesConfirmarObtenerTodos").Value, IdAerolinea);

            IList<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);

            return respuesta;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> ObtenerVuelosPendientes(string pagina)
        {
            ViewData["cantRegistros"] = "";
            ViewData["mensaje"] = "";

            var vuelos = await ObtenerTodosAsync("", "", "");
            //var vuelos = await VuelosPendientesConfirmarObtenerTodos();
            ViewData["cantRegistros"] = vuelos.Count();

            await ObtenerVuelosPendientesAsync(int.Parse(pagina));
            ViewBag.tabActiva = "1";
            return View("Principal", vuelos);
        }
        [Authorize(Roles = "OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> ValidacionManual(int id, string mensaje, string FechaInicialfiltro, string FechaFinalFiltro)
        {
            ViewBag.Mensaje = "";
            if (string.IsNullOrEmpty(mensaje))
                ViewData["mensaje"] = mensaje;

            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtener").Value, id);
            OperacionVueloOtd respuesta = await servicioApi.GetAsync<OperacionVueloOtd>(rutaRelativa);

            rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosValidaManual").Value, id);
            RespuestaValidacionManual respuestaValidaManual = await servicioApi.GetAsync<RespuestaValidacionManual>(rutaRelativa);

            // obtengo nombre de archivo pdf
            if (respuesta.PDFPasajeros == 1 || respuesta.PDFPasajeros == 2)
            {
                string rutaArchivos = string.Format(configuration.GetSection("URIs:ArchivosObtenerporIdoperacion").Value, respuesta.Id);
                List<ArchivoOtd> archivo = await servicioApi.GetAsync<List<ArchivoOtd>>(rutaArchivos);
                ArchivoOtd archivops = new ArchivoOtd();
                archivops = archivo.Where(p => p.Tipo == "PASAJEROS").FirstOrDefault();

                if (archivops != null)
                {
                    ViewBag.NombreArchivoPDFPasajero = archivops.Nombre;
                }

            }


            ViewBag.NombreAerolinea = respuesta.NombreAerolinea;
            ViewBag.Tipo = respuesta.Tipo;
            ViewBag.TRIP = respuesta.TRIP;
            ViewBag.Matricula = respuesta.Matricula;
            ViewBag.Vuelo = respuesta.Vuelo;
            ViewBag.Fecha = respuesta.Fecha.ToString("dd/MM/yyyy");
            ViewBag.Hora = respuesta.Hora;
            ViewBag.Destino = respuesta.Destino;

            ViewBag.CantPasajeros = respuestaValidaManual.CantPasajeros;
            ViewBag.CantTransitos = respuestaValidaManual.CantTransitos;
            ViewBag.CantInfantes = respuestaValidaManual.CantInfantes;
            ViewBag.CantTTL = respuestaValidaManual.CantTTL;
            ViewBag.CantTTC = respuestaValidaManual.CantTTC;
            ViewBag.CantEX = respuestaValidaManual.CantEX;
            ViewBag.CantTRIP = respuestaValidaManual.CantTRIP;
            ViewBag.path = string.Format(configuration.GetSection("Rutas:BaseServicio").Value);
            ViewBag.nomGenDec = respuesta.ArchivoGendec;
            ViewBag.nomManifiesto = respuesta.ArchivoManifiesto;
            ViewBag.Id = respuesta.Id;
            ViewBag.PDFPasajeros = respuesta.PDFPasajeros;
            ViewBag.PagoCOP = respuesta.PAGOCOP_LIQ;
            ViewBag.PagoUSD = respuesta.PAGOUSD_LIQ;
            ViewBag.fechainicial = FechaInicialfiltro;
            ViewBag.fechafinal = FechaFinalFiltro;

            string rutaCausales = string.Format(configuration.GetSection("URIs:ObtenerCausalPorTipo").Value, 1);
            /*
            
            List<Causal> sacarCausal5657585966 = new List<Causal>
            {
                new Causal { Codigo = "56", Id = 56 },
                new Causal { Codigo = "57", Id = 57 },
                new Causal { Codigo = "58", Id = 58 },
                new Causal { Codigo = "59", Id = 59 },
                new Causal { Codigo = "66", Id = 66 },

            };
            */
            List<Causal> causalesTipo1 = await servicioApi.GetAsync<List<Causal>>(rutaCausales);

            //ViewBag.causalesTipo1 = bucleCausales(causalesTipo1.Except(sacarCausal5657585966).ToList()).ToString();
            ViewBag.causalesTipo1 = bucleCausales(causalesTipo1, ViewBag.Tipo).ToString();

            string rutaCausales2 = string.Format(configuration.GetSection("URIs:ObtenerCausalPorTipo").Value, 0);
            List<Causal> causalesTipo2 = await servicioApi.GetAsync<List<Causal>>(rutaCausales2);
            ViewBag.causalesTipo2 = bucleCausales(causalesTipo2, ViewBag.Tipo).ToString();

            string rutaCausales3 = string.Format(configuration.GetSection("URIs:ObtenerCausalPorTipo").Value, 1);
            List<Causal> causalesTipo3 = await servicioApi.GetAsync<List<Causal>>(rutaCausales3);
            ViewBag.causalesTipo3 = bucleCausales(causalesTipo3, ViewBag.Tipo).ToString();

            string rutaCausales4 = string.Format(configuration.GetSection("URIs:ObtenerCausalPorTipo").Value, 1);
            List<Causal> causalesTipo4 = await servicioApi.GetAsync<List<Causal>>(rutaCausales4);
            ViewBag.causalesTipo4 = bucleCausales(causalesTipo4, ViewBag.Tipo).ToString();

            string rutaCausales5 = string.Format(configuration.GetSection("URIs:ObtenerCausalPorTipo").Value, 1);
            List<Causal> causalesTipo5 = await servicioApi.GetAsync<List<Causal>>(rutaCausales5);
            ViewBag.causalesTipo5 = bucleCausales(causalesTipo5, ViewBag.Tipo).ToString();

            return View(respuesta);
        }

        /// <summary>
        /// Método de guardar validación manual
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> InsertValidacionManual()
        {
            _logger.LogInformation($"InsertValidacionManual Inicio Validacion Manual");
            var fchInicio = string.Empty;
            var fchFinal = string.Empty;
            try
            {


                Dictionary<string, string> variables = new Dictionary<string, string>();


                foreach (var item in this.HttpContextAccessor.HttpContext.Request.Form)
                {
                    variables.Add(item.Key, item.Value);

                }

                string rutaRelativa = configuration.GetSection("URIs:InsertarValidaManual").Value;

                await servicioApi.PostAsync<bool>(rutaRelativa, variables);

                fchInicio = variables["fechaInicio"];
                fchFinal = variables["fechaFinal"];

                _logger.LogInformation($"InsertValidacionManual, fecha Inicio {fchInicio} ");
                _logger.LogInformation($"InsertValidacionManual, fecha Final: {fchFinal} ");

                // DEbe enviar notificacion al correo de los hallazgos hechos para ese vuelo en especifico
                // Tanto las manuales como las automaticas
                string rutaRelativa4 = string.Format(configuration.GetSection("URIs:VuelosObtener").Value, Convert.ToInt32(variables["idVuelo"]));
                OperacionVueloOtd respuesta = await servicioApi.GetAsync<OperacionVueloOtd>(rutaRelativa4);

                ///comparar con este id
                _logger.LogInformation($"InsertValidacionManual, Id Aerolinea: {Convert.ToInt32(respuesta.IdAerolinea)} ");
                int idAreolinea = Convert.ToInt32(respuesta.IdAerolinea);

                string rutaRelativa2 = string.Format(configuration.GetSection("URIs:ObtenerHallazgosVuelos").Value, Convert.ToInt32(variables["idVuelo"]));
                int cantidad = await servicioApi.GetAsync<int>(rutaRelativa2);
                _logger.LogInformation($"InsertValidacionManual, Hallazgos: " + cantidad.ToString());
                //string file = Request.Host + "/images/firma.png";

               // await servicioComboBox.UpdVueloValidJDE();

                await servicioComboBox.UpdVueloValidJDEVuelo(variables["idVuelo"]);
                _logger.LogInformation($"InsertValidacionManual, Actualizados: ");

                // Se quita envio de correo en validacion manual
                //_logger.LogInformation($"Inicio Envia el correo");
                //await this.EnviarCorreos(idAreolinea, respuesta.Fecha);
                //_logger.LogInformation($"Envia el correo");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
            }

            return RedirectToAction("Principal", new
            {
                fechaInicio = fchInicio,
                fechaFinal = fchFinal

            });
        }


        public StringBuilder bucleCausales(List<Causal> causalesTipo1, string tipo)
        {

            StringBuilder sb = new StringBuilder();

            sb.Append("<option value=''>--SELECCIONE--</option>");

            if (tipo == "INT")
            {
                foreach (var type in causalesTipo1)
                {
                    sb.Append("<option value='" + type.Codigo + "'>" + type.Codigo + " - " + type.Descripcion + "</option>");
                }
            }
            else
            {
                foreach (var type in causalesTipo1)
                {

                    if (type.Codigo != "56" && type.Codigo != "57" && type.Codigo != "58" && type.Codigo != "59" && type.Codigo != "66")
                    {
                        sb.Append("<option value='" + type.Codigo + "'>" + type.Codigo + " - " + type.Descripcion + "</option>");
                    }
                }
            }

            return sb;
        }
        public async Task ObtenerVuelosPendientesAsync(int pagina)
        {
            int IdAerolinea = int.Parse(User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);
            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosPendientes").Value,
                                                IdAerolinea,
                                                0,
                                                0);
            Dominio.Entidades.RespuestaGrilla<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<Dominio.Entidades.RespuestaGrilla<OperacionVueloOtd>>(rutaRelativa);
            ViewBag.Pagina = pagina;
            ViewBag.CantidadPaginas = respuesta.CantidadPaginas;
            ViewBag.CantidadGrilla = respuesta.Resultado.Count;
            ViewBag.CantidadRegistros = respuesta.CantidadRegistros;
            ViewBag.VuelosPendientes = respuesta.Resultado;

        }

        public async Task ObtenerVuelosConfirmadosAsync(int pagina)
        {
            int IdAerolinea = int.Parse(User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);

            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosConfirmados").Value,
                                                IdAerolinea,
                                                pagina,
                                                10);

            Dominio.Entidades.RespuestaGrilla<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<Dominio.Entidades.RespuestaGrilla<OperacionVueloOtd>>(rutaRelativa);

            ViewBag.PaginaConfirmados = pagina;
            ViewBag.CantidadPaginasConfirmados = respuesta.CantidadPaginas;
            ViewBag.CantidadGrillaConfirmados = respuesta.Resultado.Count;
            ViewBag.CantidadRegistrosConfirmados = respuesta.CantidadRegistros;
            ViewBag.VuelosConfirmados = respuesta.Resultado;
        }

        public async Task<IActionResult> CargarArchContingencia(IFormFile archivo, string contRows)
        {

            int cont = 0;
            string mensaje = "";

            if (archivo.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
            {
                mensaje = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos.";
                return RedirectToAction(nameof(Principal), new { contRows = contRows, mensaje = mensaje });
            }

            StreamReader lineas = CargarArchivos.LeerArchivo(archivo);
            string linea;

            IList<OperacionVueloOtd> listadoOtd = new List<OperacionVueloOtd>();

            while ((linea = lineas.ReadLine()) != null)
            {
                cont = cont + 1;

                var campos = linea.Split(",");
                var _Id_Daily = int.Parse(campos[0]);
                var _TipoVuelo = campos[1];
                var _Id_Vuelo = int.Parse(campos[2]);
                var _Matricula = campos[3];
                var _NumVuelo = campos[4];
                string rutaRelativaAero = string.Format(configuration.GetSection("URIs:TraerAerolinea").Value, campos[5]);
                var aerolinea = await servicioApi.GetAsync<AerolineaOtd>(rutaRelativaAero);
                var _IdAerolinea = aerolinea.Id;
                var _Destino = campos[6];
                var _Fecha = new DateTime(int.Parse(campos[7].Substring(6, 4)), int.Parse(campos[7].Substring(3, 2)), int.Parse(campos[7].Substring(0, 2)));
                var _Hora = campos[8].Substring(0, 2) + ":" + campos[8].Substring(2, 2);
                var _Rmk = campos[9];

                //Rule: los registros que no tienen informacion en alguno de los siguientes campos (FLTN,OFCH,ATD,RMK,OPER) no se cargan
                if (_NumVuelo == "" || _Hora == "" || _Fecha == null || _Rmk == "")
                {
                    continue;
                }

                // control sobre aerolinea no valida
                if (_IdAerolinea <= 0)
                {
                    mensaje = "El Registro:(" + (cont) + "), no tiene aerolinea valida, favor parametrizar sigla: '" + campos[5] + "'";
                    return RedirectToAction(nameof(Principal), new { contRows = cont, mensaje = mensaje });
                    //continue;
                }

                //Rule: solo se deben cargar los vuelos que tengan fecha anterior al dia actual en el campo ATD de carga del archivo, no se deben cargar registros sin fecha
                if (_Fecha > DateTime.Now || _Fecha == null)
                {
                    continue;
                }

                //Rule: para los vuelos que tienen valor DEP, CLS,SAL,ONTen el campo RMK se cargan con la fecha del campo ATD y hora del campo OFCH
                /*if (_Rmk == "DEP" || _Rmk == "CLS" || _Rmk == "SAL" || _Rmk == "ONT") {                    
                    _Fecha = new DateTime(int.Parse(campos[7].Substring(6, 4)), int.Parse(campos[7].Substring(3, 2)), int.Parse(campos[8].Substring(0, 2)));
                }*/

                //Rule: Los vuelos que tengan valor DLY, CAN, BOR no se deben cargar en el portal
                if (_Rmk == "DLY" || _Rmk == "CAN" || _Rmk == "BOR")
                {
                    continue;
                }


                listadoOtd.Add(new OperacionVueloOtd
                {
                    Id_Daily = campos[0],
                    Tipo = _TipoVuelo,
                    IdVuelo = _Id_Vuelo,
                    Matricula = _Matricula,
                    Vuelo = _NumVuelo,
                    IdAerolinea = _IdAerolinea,
                    Destino = _Destino,
                    Fecha = _Fecha,
                    Hora = _Hora,
                    EstadoProceso = "0"
                });

            }

            // bucle para iterar el obj listadoOtd
            var cont2 = 0;

            ConsecutivoCargueOtd cargueOtd = new ConsecutivoCargueOtd()
            {
                Usuario = User.Identity.Name,
                Tipo = "ArchivoContingencia",
                FechaHora = DateTime.Now,
                Archivo = archivo.FileName
            };

            var idCargue = await InsertarCargue(cargueOtd);

            foreach (var item in listadoOtd)
            {
                item.IdConsecutivoCargue = idCargue;

                // insert into singular tbl OperacionesVuelos
                string rutaRelativa = configuration.GetSection("URIs:VuelosCargarContingencia").Value;
                var respuesta = await servicioApi.PostAsync<OperacionVueloOtd>(rutaRelativa, item);
                cont2 = cont2 + 1;
            }

            contRows = cont2.ToString();

            if (cont2 == 0)
            {
                mensaje = "No se subieron registros favor validar las reglas de negocio de contingencia";
            }
            else if (cont2 > 0)
            {
                mensaje = string.Format(" Se han cargado {0} vuelo(s) por contingencia exitosamente, su consecutivo de cargue es:'" + idCargue.ToString() + "'", contRows);
            }


            return RedirectToAction(nameof(Principal), new { contRows = contRows, mensaje = mensaje });
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,ADMINISTRADOR")]
        [HttpPost]
        public async Task<IActionResult> Cargar(IFormFile archivo, string modalidad, string cantRegistro, string aerolineasel, string numeroVuelo, string fechaInicios, string fechaFinals, string fechaInicioHistorico, string fechaFinalHistorico, string tipoVuelo, string tipoVueloHistorico)
        {

            string mensaje = string.Format("Se han cargado {0} vuelo(s) exitosamente.", cantRegistro);
            bool EsUnaCargaValida = true;
            string FechaCarpeta = "";
            string NumeroVuelo = "Default";
            string MatriculaVuelo = "HK";

            int dia = DateTime.Now.Day;
            int mes = DateTime.Now.Month;
            int anio = DateTime.Now.Year;
            int EstadoCargue = 0;

            DateTime fechaInicio;
            DateTime fechaFinal;

            if (dia > 1 && dia < 17)
            {
                fechaInicio = new DateTime(anio, mes, 1);
                fechaFinal = new DateTime(anio, mes, dia);
            }
            else
            {
                fechaInicio = new DateTime(anio, mes, 16);
                fechaFinal = new DateTime(anio, mes, dia);
            }

            if (archivo.Length > int.Parse(configuration.GetSection("Cofiguracion:TamanoCsv").Value))
            {
                EstadoCargue = 0;
                mensaje = "No se pudo cargar, el tamaño del archivo excede los 2MB permitidos.";
                cantRegistro = "0";

                return RedirectToAction(nameof(Principal), new { cantRegistro = cantRegistro, mensaje = mensaje, fechaInicio = fechaInicios, fechaFinal = fechaFinals, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
            }

            /*             
            CultureInfo cultureinfo = new CultureInfo("en-gb");
            DateTime tempDateIni = Convert.ToDateTime(fechaInicio.ToString(), cultureinfo);            
            DateTime tempDateFin = Convert.ToDateTime(fechaFinal.ToString(), cultureinfo);
            fechaInicio = tempDateIni;
            fechaFinal = tempDateFin;
            */

            bool cargueDirecto = bool.Parse(configuration.GetSection("Cofiguracion:CargueDirecto").Value);

            if (cargueDirecto)
            {
                try
                {
                    #region Control Archivo
                    IList<OperacionVueloOtd> listadoOtd = new List<OperacionVueloOtd>();
                    ConsecutivoCargueOtd cargueOtd = new ConsecutivoCargueOtd()
                    {
                        Usuario = User.Identity.Name,
                        Tipo = "Archivo",
                        FechaHora = DateTime.Now,
                        Archivo = archivo.FileName
                    };
                    var idCargue = await InsertarCargue(cargueOtd);
                    StreamReader lineas = CargarArchivos.LeerArchivo(archivo);
                    string linea;

                    while ((linea = lineas.ReadLine()) != null)
                    {
                        var campos = linea.Split(",");

                        int.TryParse(campos[6], out int _TotalEmbarcados);
                        int.TryParse(campos[7], out int _INF);
                        int.TryParse(campos[8], out int _TTL);
                        int.TryParse(campos[9], out int _TTC);
                        int.TryParse(campos[10], out int _EX);
                        int.TryParse(campos[11], out int _TRIP);

                        int pax = 0;
                        if (campos[1] == "INT")
                        {
                            pax = _TotalEmbarcados - (_TTL + _TTC + _EX + _TRIP);
                        }
                        else
                        {
                            pax = _TotalEmbarcados - (_INF + _TTL + _TTC + _EX + _TRIP);
                        }

                        int Id_Aerolinea = 0;
                        string Nombre_Aerolinea = "";
                        if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR"))
                        {
                            Id_Aerolinea = int.Parse(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value);
                            Nombre_Aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;
                        }
                        else
                        {
                            Id_Aerolinea = Convert.ToInt32(aerolineasel);
                            Nombre_Aerolinea = "EASY FLY";
                        }
                        FechaCarpeta = campos[0].Substring(6, 4) + campos[0].Substring(3, 2) + campos[0].Substring(0, 2);

                        listadoOtd.Add(new OperacionVueloOtd
                        {
                            Fecha = new DateTime(int.Parse(campos[0].Substring(6, 4)), int.Parse(campos[0].Substring(3, 2)), int.Parse(campos[0].Substring(0, 2))),
                            Tipo = campos[1],
                            Matricula = campos[2],
                            Vuelo = campos[3],
                            Destino = campos[4],
                            Hora = campos[5].Substring(0, 2) + ":" + campos[5].Substring(2, 2),
                            TotalEmbarcados = _TotalEmbarcados,
                            INF = _INF,
                            TTL = _TTL,
                            TTC = _TTC,
                            EX = _EX,
                            TRIP = _TRIP,
                            PagoCOP = int.Parse(campos[12]),
                            PagoUSD = int.Parse(campos[13]),
                            ConfirmacionGenDec = 0,
                            ConfirmacionManifiesto = 0,
                            ConfirmacionOperacion = 0,
                            ConfirmacionPasajeros = 0,
                            ConfirmacionTransitos = 0,
                            PAX = pax,
                            IdAerolinea = Id_Aerolinea,
                            NombreAerolinea = Nombre_Aerolinea,
                            IdConsecutivoCargue = idCargue
                        });
                        NumeroVuelo = campos[3];
                        MatriculaVuelo = campos[2];
                    }

                    if (!ControlArchivos(User, listadoOtd))
                    {
                        return RedirectToAction(nameof(Principal), new { cantRegistro = 0, mensaje = "Lo sentimos esta intentando subir un cierre de otra aerolinea.", fechaInicio = fechaInicios, fechaFinal = fechaFinals, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
                    }
                    #endregion

                    int cantidadInvalidos = listadoOtd.Count(x => x.Fecha < fechaInicio && x.Fecha > fechaFinal);

                    if (cantidadInvalidos == 0)
                    {
                        //string rutaRelativa = configuration.GetSection("URIs:VuelosCargar").Value;
                        //await servicioApi.PostAsync<bool>(rutaRelativa, listadoOtd);
                        // Cuento cuantos hay ya insertados
                        //string rutaRelativa2 = string.Format(configuration.GetSection("URIs:VuelosObtenerTodosAerolinea").Value, listadoOtd[0].IdAerolinea);
                        //IList<OperacionVueloOtd> respuesta2 = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa2);
                        int aerolinea = int.TryParse(aerolineasel, out aerolinea) == false ? 0 : aerolinea;

                        if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR"))
                        {
                            //string aerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;
                            //respuesta2 = respuesta2.Where(x => x.NombreAerolinea.ToLower().Equals(aerolinea.ToLower())).ToList();
                            aerolinea = listadoOtd[0].IdAerolinea;
                        }
                        //else
                        //{
                        //    respuesta2 = respuesta2.Where(x => x.IdAerolinea.Equals(aerolineasel)).ToList();
                        //}

                        string rutaRelativa2 = string.Format(configuration.GetSection("URIs:VuelosObtenerTodosAerolinea").Value, aerolinea);

                        string fechaIniciocargue = string.Empty;
                        string fechaFinalCargue = string.Empty;

                        var diaactual = DateTime.Now.Day;
                        int diainicial = 16;

                        fechaIniciocargue = DateTime.Now.AddDays(-diainicial).ToString("dd-MM-yyyy");
                        fechaFinalCargue = DateTime.Now.AddDays(5).ToString("dd-MM-yyyy");


                       

                        IList<OperacionVueloOtd> lstado;
                        IList<OperacionVueloOtd> VueloInterfaz;
                        //OperacionVueloOtd ParametrosServicio = null;

                        int cantidad = 0;
                        int cantidad2 = 0;
                        int cantidadinv = 0;
                        int Adicionado = 0;
                        foreach (OperacionVueloOtd item in listadoOtd)
                        {

                            string fechaInicial = item.Fecha.AddDays(-1).ToString("dd-MM-yyyy");
                            string finalal = item.Fecha.AddDays(1).ToString("dd-MM-yyyy");

                            if (!string.IsNullOrEmpty(fechaIniciocargue) && !string.IsNullOrEmpty(fechaFinalCargue))
                            {
                                rutaRelativa2 = rutaRelativa2 + "&fechaVueloInicio={0}&fechaVueloFinal={1}&bandera={2}";
                                rutaRelativa2 = string.Format(rutaRelativa2, fechaInicial, finalal, "CARGUE");
                            }


                            IList<OperacionVueloOtd> respuesta2 = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa2);


                            lstado = respuesta2.Where(x => Math.Abs((Convert.ToDateTime(x.Hora).Subtract(Convert.ToDateTime(item.Hora)).Minutes)) <= 60
                            && x.Matricula.Equals(item.Matricula)
                            && x.Vuelo.Equals(item.Vuelo)
                            && x.Fecha.Equals(item.Fecha)
                            && x.IdCargue != 1
                            && (x.EstadoProceso != "-9" && x.EstadoProceso != "-10" && x.EstadoProceso != "-11")).ToList();
                            VueloInterfaz = respuesta2.Where(x => Math.Abs((Convert.ToDateTime(x.Hora).Subtract(Convert.ToDateTime(item.Hora)).Minutes)) <= 60 && x.Matricula.Equals(item.Matricula) && x.Vuelo.Equals(item.Vuelo) && x.Fecha.Equals(item.Fecha) && x.IdCargue == 1).ToList();
                            // lstado = respuesta2.Where(x => x.Hora.Equals(item.Hora) && x.Matricula.Equals(item.Matricula) && x.Vuelo.Equals(item.Vuelo) && x.Fecha.Equals(item.Fecha)).ToList();
                            if (VueloInterfaz == null || VueloInterfaz.Count <= 0)
                            {
                                cantidadinv = 1;
                            }
                            else
                            {
                                if (lstado.Count > 0)
                                {
                                    if (lstado[0].EstadoProceso == "0")
                                    {
                                        EsUnaCargaValida = await SubirVuelo(item);
                                        if (EsUnaCargaValida)
                                        {
                                            Adicionado++;
                                        }
                                    }
                                    else
                                    {
                                        cantidad++;
                                    }
                                }
                                else
                                {
                                    EsUnaCargaValida = await SubirVuelo(item);
                                    if (EsUnaCargaValida)
                                    {
                                        Adicionado++;
                                    }
                                }
                            }


                            if (cantidad > 0 || cantidadinv > 0)
                            {
                                mensaje = string.Format("Se han cargado {0} vuelo(s) exitosamente , {1} vuelos ya existian y {2} vuelos no existen en JDE", Adicionado, cantidad, cantidadinv);
                            }
                        }
                        cantRegistro = Adicionado.ToString();

                        if (Adicionado > 0)
                        {
                            try
                            {
                                if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR"))
                                {
                                    // Saco la abreviacion de aerolinea
                                    string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                                    IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                                    string Abreviatura = respuesta.Where(p => p.Id == Convert.ToInt32(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim())).FirstOrDefault().Sigla;

                                    // Guarda el archivo en la ruta 
                                    //string carpetarReal = String.Format("{0}//{1}", Abreviatura.Trim(),
                                    //    DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0'));
                                    string carpetarReal = String.Format("{0}//{1}", Abreviatura.Trim(),
                                        FechaCarpeta);
                                    //var fechaReal = string.Format("{0}{1}{2}{3}{4}{5}", 
                                    //    DateTime.Now.Year, DateTime.Now.Month, 
                                    //    DateTime.Now.Day, DateTime.Now.Hour, 
                                    //    DateTime.Now.Minute, 
                                    //    DateTime.Now.Second);
                                    //var nombreArchivoREal = string.Concat("CierreVuelo", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString(), "_", DateTime.Now.Hour.ToString(), DateTime.Now.Minute.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Millisecond.ToString(), archivo.FileName.Substring(archivo.FileName.LastIndexOf('.'), archivo.FileName.Length - (archivo.FileName.LastIndexOf('.'))));
                                    string nombreArchivoREal = "";

                                    if (listadoOtd.Count == 1)
                                    {
                                        nombreArchivoREal = string.Concat("CierreVuelo-", NumeroVuelo, "-", MatriculaVuelo,
                                             archivo.FileName.Substring(archivo.FileName.LastIndexOf('.'),
                                            archivo.FileName.Length - (archivo.FileName.LastIndexOf('.'))));
                                    }
                                    else
                                    {
                                        nombreArchivoREal = string.Concat("CierreVuelo-", FechaCarpeta,
                                        DateTime.Now.Year.ToString(),
                                        DateTime.Now.Month.ToString().PadLeft(2, '0'),
                                        DateTime.Now.Day.ToString(), "-",
                                        DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                                        DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                                        DateTime.Now.Second.ToString().PadLeft(2, '0'),
                                        DateTime.Now.Millisecond.ToString(),
                                        archivo.FileName.Substring(archivo.FileName.LastIndexOf('.'),
                                        archivo.FileName.Length - (archivo.FileName.LastIndexOf('.'))));
                                    }


                                    //var nombreArchivoREal = archivo.FileName.ToString();
                                    var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                                    bool resultado = await CargarArchivos.Cargar(configuration, archivo, nombreArchivoREal, carpetarReal, token);
                                    EstadoCargue = 1;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.StackTrace.ToString());
                            }
                        }
                    }
                    else
                    {
                        EstadoCargue = 0;
                        //mensaje += " \n No se pudo cargar el archivo de vuelos else. fechaInicio: " + fechaInicio.ToString()+ " fechaFinal: "+ fechaFinal.ToString()+ " cantidadInvalidos: "+ cantidadInvalidos.ToString();
                        mensaje = " NO se pudo cargar el archivo de vuelos.";
                        cantRegistro = "0";
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.StackTrace.ToString());
                    EstadoCargue = 0;
                    //mensaje += " \n No se pudo cargar el archivo de vuelos catch. fechaInicio: " + fechaInicio.ToString() + " fechaFinal: " + fechaFinal.ToString() + " ex: "+ex.ToString();
                    mensaje = " No se pudo cargar el archivo de vuelos.";
                    cantRegistro = "0";
                }
            }
            else
            {
                string carpeta = "VUELOS";
                string fecha = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                string nombreArchivo = string.Format("{0}{1}.txt", modalidad, fecha);
                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                bool resultado = await CargarArchivos.Cargar(configuration, archivo, nombreArchivo, carpeta, token);
                if (!resultado)
                {
                    EstadoCargue = 0;
                    mensaje = "NO Se pudo cargar el archivo de vuelos.";
                    cantRegistro = "0";
                }
            }
            try
            {
                if (EstadoCargue == 1)
                {
                    string file = "https://" + Request.Host + "/images/firma.png";
                    //string file = Request.Host + "/images/firma.png";
                    var emailUser = User.Claims.Where(c => c.Type.Equals("UsuarioEmail")).FirstOrDefault().Value;
                    /*
                    await emailSender.SendEmailAsync(
                     emailUser,
                        "Cargue de vuelos exitoso",
                      "Buen día estimada aerolínea,</br></br></br>Nos permitimos informales que el cargue de la información reportada por parte de ustedes se realizó exitosamente,</br>la cuál será revisada previamente por el equipo de facturación.</br> Por favor estar muy atentos a los cambios que se puedan presentar.</br></br>Cordialmente,</br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <img src='" + file + "' width='250' height='120'/> </br>Tel. + 57(1) 439 70 70  Ext.2012 </br>Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");
                    */
                }
                else
                {
                    string file = "https://" + Request.Host + "/images/firma.png";
                    //string file = Request.Host + "/images/firma.png";
                    var emailUser = User.Claims.Where(c => c.Type.Equals("UsuarioEmail")).FirstOrDefault().Value;

                    //string rutaFn = string.Format(configuration.GetSection("URIs:EnviarCorreosSMTP").Value, emailUser, "Cargue de vuelos rechazado", "Buen día estimada aerolínea,</br></br></br>Sus vuelos no han sido cargados exitosamente, por los siguientes motivos:</br><b>Error cierre del vuelo.</b>campos en blanco, revisar si corresponde a formato numérico o alfa-numérico.</br></br>Cordialmente,</br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <img src='" + ruta + "' width='250' height='70'/> </br>Tel. + 57(1) 439 70 70  Ext.2012 </br>Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");
                    //await servicioApi.PostAsync<bool>(rutaFn, "").ConfigureAwait(false);

                    //enviarCorreo(emailUser, "Cargue de vuelos rechazado", "Buen día estimada aerolínea,</br></br></br>Sus vuelos no han sido cargados exitosamente, por los siguientes motivos:</br><b>Error cierre del vuelo.</b>campos en blanco, revisar si corresponde a formato numérico o alfa-numérico.</br></br>Cordialmente,</br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <img src='" + ruta + "' width='250' height='70'/> </br>Tel. + 57(1) 439 70 70  Ext.2012 </br>Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");
                    /*  emailSender.SendEmailAsync(
                         emailUser,
                         "Cargue de vuelos rechazado",
                         "Buen día estimada aerolínea,</br></br></br>Sus vuelos no han sido cargados exitosamente, por los siguientes motivos:</br><b>Error cierre del vuelo.</b>campos en blanco, revisar si corresponde a formato numérico o alfa-numérico.</br></br>Cordialmente,</br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <img src='" + file + "' width='250' height='120'/> </br>Tel. + 57(1) 439 70 70  Ext.2012 </br>Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");
                 */
                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction(nameof(Principal), new { cantRegistro = cantRegistro, mensaje = mensaje, fechaInicio = fechaInicios, fechaFinal = fechaFinals, numeroVuelo = numeroVuelo, fechaInicioHistorico = fechaInicioHistorico, fechaFinalHistorico = fechaFinalHistorico, tipoVuelo = tipoVuelo, tipoVueloHistorico = tipoVueloHistorico });
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<IActionResult> Detalle(int? id)
        {
            var vuelo = await Obtener(id);
            return PartialView(vuelo);
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public IActionResult Insertar()
        {
            return PartialView();
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Insertar(OperacionVueloOtd operacionVueloOtd)
        {
            OperacionVueloOTDRequest oFiltro = new OperacionVueloOTDRequest();
            try
            {
                var idAerolinea = int.Parse(User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value);
                var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;

                ConsecutivoCargueOtd cargue = new ConsecutivoCargueOtd()
                {
                    Usuario = User.Identity.Name,
                    Tipo = "Manual",
                    FechaHora = DateTime.Now,
                    Archivo = string.Empty
                };

                var idCargue = await InsertarCargue(cargue);

                operacionVueloOtd.IdConsecutivoCargue = idCargue;
                operacionVueloOtd.IdAerolinea = idAerolinea;
                operacionVueloOtd.NombreAerolinea = aerolinea;


                //string rutaRelativa2 = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos").Value, 0);
                //IList<OperacionVueloOtd> respuesta2 = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa2);
                oFiltro.FechaDesde = operacionVueloOtd.Fecha.ToString("yyyy-MM-dd");
                oFiltro.Matricula = operacionVueloOtd.Matricula;
                Opain.Jarvis.Presentacion.Web.Bussiness.OperacionesVuelo oList = new Bussiness.OperacionesVuelo();
                IList<OperacionVueloOtd> oListVuelos = await oList.ObtenerVuelosParaValidar(User, configuration, servicioApi, oFiltro);

                //if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR"))
                //{
                //    string aerolineaACtual = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;
                //    respuesta2 = respuesta2.Where(x => x.NombreAerolinea.ToLower().Equals(aerolinea.ToLower())).ToList();
                //}

                int cantidadexistente = oListVuelos.Where(x => Math.Abs((Convert.ToDateTime(x.Hora).Subtract(Convert.ToDateTime(operacionVueloOtd.Hora)).Minutes)) <= 60
                && x.Matricula.Equals(operacionVueloOtd.Matricula)
                && x.Vuelo.Equals(operacionVueloOtd.Vuelo)
                && x.Fecha.Equals(operacionVueloOtd.Fecha)).ToList().Count();


                //string rutaRelativa = configuration.GetSection("URIs:VuelosCargarAerolinea").Value;
                //var respuestaAero = await servicioApi.PostAsync<OperacionVueloOtd>(rutaRelativa, operacionVueloOtd);

                //string rutaRelativa = configuration.GetSection("URIs:VuelosInsertar").Value;
                //await servicioApi.PostAsync<bool>(rutaRelativa, operacionVueloOtd);

                var cantRegistro = "1";

                // Guardo el archivo .txt

                try
                {
                    // Guarda el archivo en la ruta 

                    string carpetarReal = String.Format("{0}//{1}", User.Claims.Where(c => c.Type.Equals("IdAerolinea")).FirstOrDefault().Value.Trim(), DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0'));
                    var fechaReal = string.Format("{0}{1}{2}{3}{4}{5}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    string Tipo = "FLY";
                    string cadenaFLY = string.Concat(operacionVueloOtd.Fecha.ToShortDateString(), ",", operacionVueloOtd.Tipo, ",", operacionVueloOtd.Matricula, ",", operacionVueloOtd.Vuelo, ",", operacionVueloOtd.Destino, ",", operacionVueloOtd.Hora, ",", operacionVueloOtd.TotalEmbarcados, ",", operacionVueloOtd.INF, ",", operacionVueloOtd.TTL, ",", operacionVueloOtd.TTC, ",", operacionVueloOtd.EX, ",", operacionVueloOtd.TRIP, ",", operacionVueloOtd.PagoCOP, ",", operacionVueloOtd.PagoUSD);
                    string rutaFn = string.Format(configuration.GetSection("URIs:CrearArchivosCargue").Value, carpetarReal, cadenaFLY, Tipo);
                    await servicioApi.PostAsync<bool>(rutaFn, "").ConfigureAwait(false);

                }
                catch (Exception ex)
                {
                    Console.Write(ex.StackTrace.ToString());
                }
                if (cantidadexistente == 0)
                {
                    return RedirectToAction(nameof(Principal), new { cantRegistro = cantRegistro, mensaje = "El vuelo no existe en JDE." });
                }
                else
                {
                    return RedirectToAction(nameof(Principal), new { cantRegistro = cantRegistro, mensaje = "Vuelo creado exitosamente." });
                }

            }
            catch (Exception)
            {
                return View(operacionVueloOtd);
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task<IActionResult> Actualizar(int? id)
        {
            var vuelo = await Obtener(id);
            return PartialView(vuelo);
        }

        public async Task<IActionResult> Exentos()
        {
            return PartialView("Exentos");
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR,OPAIN,ADMINISTRADOR")]
        public async Task<OperacionVueloOtd> Obtener(int? id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtener").Value, id);
            OperacionVueloOtd respuesta = await servicioApi.GetAsync<OperacionVueloOtd>(rutaRelativa);
            return respuesta;
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Actualizar(OperacionVueloOtd operacionVueloOtd, string pdfyTxt, string numeroVuelo, string fechaInicio, string fechaFinal)
        {
            try
            {
                await ActualizarVuelo(operacionVueloOtd);
                var retornopdfotxt = string.Empty;
                if (pdfyTxt.ToString() == "pdf")
                    retornopdfotxt = await CierreManualPDF(operacionVueloOtd);
                else
                    retornopdfotxt = await CierreManualTXT(operacionVueloOtd);

                return RedirectToAction(nameof(Principal), new { cantRegistro = "0", mensaje = "Vuelo actualizado exitosamente.", retornopdfotxt, fechaInicio = fechaInicio, fechaFinal = fechaFinal, numeroVuelo = numeroVuelo });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task<bool> ActualizarVuelo(OperacionVueloOtd operacionVueloOtd)
        {
            string rutaRelativa = configuration.GetSection("URIs:VuelosActualizarVuelo").Value;
            return await servicioApi.PostAsync<bool>(rutaRelativa, operacionVueloOtd);
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> Eliminar(int? id)
        {
            var vuelo = await Obtener(id);
            return PartialView(vuelo);
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(int id)
        {
            try
            {
                string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosConfirmarEliminar").Value, id);
                _ = await servicioApi.PostAsync<bool>(rutaRelativa, id).ConfigureAwait(false);
                return RedirectToAction(nameof(Principal));

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        [HttpPost]
        public async Task<IActionResult> Confirmar(IEnumerable<Opain.Jarvis.Dominio.Entidades.OperacionVueloOtd> model, string fechaInicio, string fechaFinal, string numeroVuelo, string fechaInicioHistorico, string fechaFinalHistorico, string tipoVuelo, string tipoVueloHistorico)
        {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
            string rutaRelativa = "";
            int contador = 0;
            var vuelosFiltrados = TempData.Peek("vuelosFiltrados");
            IList<PasajeroTransitoOtd> pasajeros;
            var emailUser = User.Claims.Where(c => c.Type.Equals("UsuarioEmail")).FirstOrDefault().Value;
            _logger.LogInformation($"Confirmar Vuelos, email Usuario: {emailUser} ");
            int TotalNovedades = 0;
            int IdAeroli = int.Parse(User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);

            Opain.Jarvis.Presentacion.Web.Bussiness.OperacionesVuelo oOperaciones = new Bussiness.OperacionesVuelo();
            int cantidadinvalidos = 0;
            var fch = string.Format("{0}{1}{2}", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"));
            string RutaArchivos = "";

            IdAerolineaYIdOperacionVueloOtd IdAerolineaYIdOperacionVuelo = new IdAerolineaYIdOperacionVueloOtd();
            try
            {
                RutaArchivos = configuration.GetSection("Config:RutaArchivos").Value;
                var vuelos = await ObtenerVuelosFiltradosTransitosFirmadosAsync(fechaInicio, fechaFinal, numeroVuelo, tipoVuelo, IdAeroli);
                if (!vuelos.Count.Equals(0))
                {
                    foreach (var oItem in vuelos)
                    {
                        var item = await Obtener(oItem.Id);
                        _logger.LogInformation($"Analizando el vuelo : " + item.Vuelo + " Fecha de Vuelo : " + item.Fecha + " ID : " + item.Id);
                        rutaRelativa = string.Format(configuration.GetSection("URIs:TransitosPrincipal").Value, item.Id);
                        pasajeros = await servicioApi.GetAsync<IList<PasajeroTransitoOtd>>(rutaRelativa).ConfigureAwait(false);
                        _logger.LogInformation($"Pasajeros de transito: vuelo : " + item.Vuelo + ": Transitos :" + pasajeros.Count.ToString());
                      //  await EnviarCorreosTransitos(pasajeros);

                        if (item.Tipo == "DOM" && (!item.ConfirmacionPasajeros.Equals(1) || !item.ConfirmacionManifiesto.Equals(1)))
                        {
                            cantidadinvalidos++;
                        }
                        else if (item.Tipo == "INT" && (!item.ConfirmacionPasajeros.Equals(1) || !item.ConfirmacionManifiesto.Equals(1) || !item.ConfirmacionGenDec.Equals(1)))
                        {
                            cantidadinvalidos++;
                        }
                        else
                        {
                            item.EstadoProceso = "4";
                            if (oItem.TTC_LIQ > 0)
                            {
                                item.EstadoProceso = "3";
                            }
                            item.ConfirmacionOperacion = 1;
                            await ActualizarVuelo(item);

                            int idAerolineaSel = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);
                            string rutaRelativaAerolinea = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                            IList<AerolineaOtd> respuestaAerolinea = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAerolinea);
                            IList<AerolineaOtd> respuestaunica = respuestaAerolinea.Where(p => p.Id == idAerolineaSel).ToList();

                            if (respuestaunica.Count() > 0 && (respuestaunica[0].PDFPasajeros == "0" || respuestaunica[0].PDFPasajeros == "2" || respuestaunica[0].PDFPasajeros == "1"))
                            {
                               
                                await ValidarVuelo(item);
                                contador++;
                            }

                            string rutaRelativaAuditoria = string.Format(configuration.GetSection("URIs:Auditoria_Cargue").Value, User.Identity.Name, "SUBIRVUELOS", item.Id);
                            var respuestaAuditoria = await servicioApi.PostAsync<bool>(rutaRelativaAuditoria, "");

                            string rutaRelativa2 = configuration.GetSection("URIs:ObtenerNovedadesxOperacion").Value;
                            IList<NovedadOtd> respuesta2 = await servicioApi.GetAsync<IList<NovedadOtd>>(string.Format(rutaRelativa2, item.Id));

                            if (respuesta2.Count() > 0)
                            {
                                TotalNovedades++;
                            }
                        }
                    }

                    CargueExitosoNotificacionHallazgosOtd _CargueExitosoNotificacionHallazgos = new CargueExitosoNotificacionHallazgosOtd();
                    IdAerolineaYIdOperacionVuelo.IdAerolinea = 0;
                    IdAerolineaYIdOperacionVuelo = await oOperaciones.GenerarPDFVuelosSoporte(User, configuration, servicioApi, vuelos);
                    _CargueExitosoNotificacionHallazgos.CantidadSinCargar = TotalNovedades;
                    _CargueExitosoNotificacionHallazgos.Contador = contador;

                    //// Notificaciones 1,2,3 para todas la aerolineas
                    IEnumerable<UsuarioOtd> usuarios = await servicioApi.GetAsync<IEnumerable<UsuarioOtd>>(configuration.GetSection("URIs:UsuariosObtenerTodos").Value).ConfigureAwait(false);
                    var ListaUsuarios = usuarios.Where(x => x.UsuarioAerolinea.Any() && (x.Perfil.Equals("AEROLINEA") || x.Perfil.Equals("SUPERVISOR")));
                    var ListaEmail = ListaUsuarios.Where(y => y.UsuarioAerolinea.Any(r => r.IdAerolinea.Equals(IdAeroli)));

                    foreach (UsuarioOtd datosUsuario in ListaEmail)
                    {
                        _CargueExitosoNotificacionHallazgos.EmailUser = _CargueExitosoNotificacionHallazgos.EmailUser
                            + datosUsuario.Email.Trim() + ";";
                    }
                    await CargueExitosoNotificacionHallazgosUnoYDos(_CargueExitosoNotificacionHallazgos);

                    if (cantidadinvalidos > 0)
                    {
                        return RedirectToAction(nameof(Principal),
                            new
                            {
                                mensaje = string.Format("Hubo {0} vuelos que no se pudieron confirmar por falta de documentación ",
                                cantidadinvalidos.ToString()),
                                IdAerolinea = IdAerolineaYIdOperacionVuelo.IdAerolinea,
                                fch = fch,
                                RutaArchivos = IdAerolineaYIdOperacionVuelo.UrlFile,
                                IdOperacionVuelo = IdAerolineaYIdOperacionVuelo.IdOperacionVuelo,
                                fechaInicio = fechaInicio,
                                fechaFinal = fechaFinal,
                                fechaInicioHistorico = fechaInicioHistorico,
                                fechaFinalHistorico = fechaFinalHistorico,
                                numeroVuelo = numeroVuelo,
                                tipoVuelo = tipoVuelo,
                                tipoVueloHistorico = tipoVueloHistorico
                            });
                    }
                }
                else
                {
                    return RedirectToAction(nameof(Principal), new
                    {
                        mensaje = "No hay vuelos que se puedan confirmar",
                        fechaInicio = fechaInicio,
                        fechaFinal = fechaFinal,
                        numeroVuelo = numeroVuelo,
                        fechaInicioHistorico = fechaInicioHistorico,
                        fechaFinalHistorico = fechaFinalHistorico,
                    });
                }

                return RedirectToAction(nameof(Principal),
                   new
                   {
                       cantRegistro = contador.ToString(),
                       mensaje = "Vuelos han sido confirmados",
                       IdAerolinea = IdAerolineaYIdOperacionVuelo.IdAerolinea,
                       fch = fch,
                       RutaArchivos = IdAerolineaYIdOperacionVuelo.UrlFile,
                       IdOperacionVuelo = IdAerolineaYIdOperacionVuelo.IdOperacionVuelo,
                       fechaInicio = fechaInicio,
                       fechaFinal = fechaFinal,
                       fechaInicioHistorico = fechaInicioHistorico,
                       fechaFinalHistorico = fechaFinalHistorico,
                       numeroVuelo = numeroVuelo
                                   });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                throw;
            }
        }

        public async Task EnviarCorreosTransitos(IList<PasajeroTransitoOtd> pasajero)
        {
            try
            {
                string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                IList<AerolineaOtd> respuestaAero = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                IList<ContadosCorreos> contadorAreolinea = new List<ContadosCorreos>();
                _logger.LogInformation($"Pasajeros a Notificar : " + pasajero.Count.ToString());

                _logger.LogInformation($"Coleccion...: {Newtonsoft.Json.JsonConvert.SerializeObject(pasajero)} ");

                foreach (var pasa in pasajero.Where(x => x.Firmado.ToString() == "0").ToList())
                {
                    _logger.LogInformation($"Pasajero Analizado : " + pasa.AerolineaLlegada);
                    if (pasa.TTC == 1)
                    {
                        var contadorEmail = new ContadosCorreos();
                        if (respuestaAero.Where(p => p.Sigla.Trim() == pasa.AerolineaLlegada.Trim()).FirstOrDefault() != null)
                        {
                            contadorEmail.IdAerolinea = respuestaAero.Where(p => p.Sigla.Trim() == pasa.AerolineaLlegada.Trim()).FirstOrDefault().Id;
                            contadorEmail.NumeroTransitos = 1;
                            contadorAreolinea.Add(contadorEmail);
                            _logger.LogInformation($"Aerolinea : " + pasa.AerolineaLlegada + " : " + contadorEmail.IdAerolinea.ToString());
                        }
                    }
                }

                IEnumerable<UsuarioOtd> usuarios = await servicioApi.GetAsync<IEnumerable<UsuarioOtd>>(configuration.GetSection("URIs:UsuariosObtenerTodos").Value).ConfigureAwait(false);
                string file = "https://" + Request.Host + "/images/firma.png";

                var results = contadorAreolinea.GroupBy(x => x.IdAerolinea)
                          .Select(x => (IdAerolinea: x.Key, NumeroTransitos: x.Select(p => p.NumeroTransitos).ToList())
                          ).ToList();

                _logger.LogInformation($"Inico notificación tránsitos en conexión inicio foreach");

                string ConCopia = "";
                if (configuration.GetSection("SendGrid:cc").Value != null)
                {
                    ConCopia = configuration.GetSection("SendGrid:cc").Value;
                }
                _logger.LogInformation($"Notificación 4 tránsitos total aerolineas: " + results.Count.ToString());
                foreach (var resultado in results)
                {
                    _logger.LogInformation($"Notificación 4 tránsitos en conexión aprobados");
                    string Para = "";
                    string Mensaje = "<p>Buen día estimada aerolínea,</p></br></br></br>" +
                    "<p>Nos permitimos informales que tiene " + resultado.NumeroTransitos.Count +
                    " tránsitos pendientes por su confirmación u autorización, <p></br><p>" +
                    "Agradecemos su gestión a la mayor brevedad posible.</p></br>  <b><p> " +
                    "Nota: Recordar que plazo máximo para la firma de tránsitos es de 24 horas. </p></b> </br></br>" +
                    "<p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <p>" +
                    "<img src='" + file + "' width='250' height='70'/></p><p></p> </br> " +
                    "Bogotá D.C. - Colombia </br>www.eldorado.aero </br>" +
                    "Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ";

                    var ua = usuarios.Where(u => u.UsuarioAerolinea.Any()).ToList();
                    var e = ua.Where(x => x.UsuarioAerolinea.Where(z => z.IdAerolinea == resultado.IdAerolinea).Any());
                    foreach (UsuarioOtd u in e)
                    {
                        Para = Para + u.Email.Trim() + ";";
                    }
                    if (Para.IndexOf(";") > 0)
                    {
                        Para = Para.Substring(0, Para.Length - 1);
                        await SendMail("Notificación tránsitos en conexión",
                        Para.Trim(),
                        ConCopia, Mensaje);
                    }

                    #region Apagado 
                    /*
                        foreach (var datosUsuario in usuarios.Where(x => x.UsuarioAerolinea.Any()))
                        {
                        
                        foreach (var aerolinea in datosUsuario.UsuarioAerolinea)
                        {
                            if (resultado.IdAerolinea == aerolinea.IdAerolinea)
                            {
                                await emailSender.SendEmailAsync(
                                    datosUsuario.Email.Trim(),
                                    "Notificación tránsitos en conexión",
                                    "<p>Buen día estimada aerolínea,</p></br></br></br><p>Nos permitimos informales que tiene " + resultado.NumeroTransitos.Count + " tránsitos pendientes por su confirmación u autorización, <p></br><p>Agradecemos su gestión a la mayor brevedad posible.</p></br>  <b><p> Nota: Recordar que plazo máximo para la firma de tránsitos es de 24 horas. </p></b> </br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <p><img src='" + file + "' width='250' height='70'/></p><p></p> </br> Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ");
                                    _logger.LogInformation($"Notificación 4 tránsitos en conexión aprobados, Email Usuario: {datosUsuario.Email.Trim()} ");
                                Para = Para + datosUsuario.Email.Trim() + ";";
                            }
                        }
                        if (Para.IndexOf(";") > 0)
                        {
                            Para = Para.Substring(0, Para.Length - 1);
                            await SendMail("Notificación tránsitos en conexión",
                            Para.Trim(),
                            ConCopia, Mensaje);
                        }                        
                        }*/
                    #endregion
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                throw;
            }
        }

        [Authorize(Roles = "AEROLINEA,SUPERVISOR")]
        public async Task<int> InsertarCargue(ConsecutivoCargueOtd cargueOtd)
        {
            string rutaRelativa = configuration.GetSection("URIs:ConsecutivoCargueInsertar").Value;
            return await servicioApi.PostAsync<int>(rutaRelativa, cargueOtd);
        }

        private async Task<string> ValidarVuelo(OperacionVueloOtd vuelo)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosValidar").Value, vuelo.Id);
            string Resultado = await servicioApi.GetAsync<string>(rutaRelativa);

            return Resultado;
        }

        [HttpGet]
        public async Task<string> CierreManualTXT(OperacionVueloOtd _OperacionVueloOtd)
        {
            string RutaArchivos = configuration.GetSection("Config:RutaArchivosCierreManual").Value;
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
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                    await writer.WriteLineAsync(
                          _OperacionVueloOtd.Fecha.ToString("dd/MM/yyyy") + ","
                        + _OperacionVueloOtd.Tipo + ","
                        + _OperacionVueloOtd.Matricula + ","
                        + _OperacionVueloOtd.Vuelo.ToString() + ","
                        + _OperacionVueloOtd.Destino + ","
                        + _OperacionVueloOtd.Hora + ","
                        + _OperacionVueloOtd.TotalEmbarcados + ","
                        + _OperacionVueloOtd.INF + ","
                        + _OperacionVueloOtd.TTL + ","
                        + _OperacionVueloOtd.TTC + ","
                        + _OperacionVueloOtd.EX + ","
                        + _OperacionVueloOtd.TRIP + ","
                        + _OperacionVueloOtd.PagoCOP + ","
                        + _OperacionVueloOtd.PagoUSD);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                _logger.LogInformation($"paso 6 GENERAR TXT , error " + ex.Message);
                throw;
            }
            return NombraArchivo;

        }

        public async Task<string> CierreManualPDF(OperacionVueloOtd _OperacionVueloOtd)
        {

            string filePath = configuration.GetSection("Config:RutaPlantillas").Value;

            string filePathCierreManual = configuration.GetSection("Config:RutaArchivosCierreManual").Value;

            string fileNameExisting = @"PlantillaPDFCierreManual.pdf";

            string NombraArchivo = User.Identity.Name.Trim().ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');

            string fileNameNew = NombraArchivo + ".pdf";

            string fullNewPath = Path.Combine(filePathCierreManual + "\\" + fileNameNew);
            string fullExistingPath = Path.Combine(filePath + "\\" + fileNameExisting);

            using (var existingFileStream = new FileStream(fullExistingPath, FileMode.Open))

            using (var newFileStream = new FileStream(fullNewPath, FileMode.Create))
            {
                // Open existing PDF
                var pdfReader = new PdfReader(existingFileStream);

                // PdfStamper, which will create
                var stamper = new PdfStamper(pdfReader, newFileStream);

                AcroFields fields = stamper.AcroFields;
                fields.SetField("NumeroVuelo", _OperacionVueloOtd.Vuelo);
                fields.SetField("NumeroMatricula", _OperacionVueloOtd.Matricula);
                fields.SetField("Tipo", _OperacionVueloOtd.Tipo);
                fields.SetField("Destino", _OperacionVueloOtd.Destino.ToString());
                fields.SetField("FechaVuelo", _OperacionVueloOtd.Fecha.ToString("dd-MM-yyyy"));
                fields.SetField("HoraVuelo", _OperacionVueloOtd.Hora);
                fields.SetField("TotalEmbarcados", _OperacionVueloOtd.TotalEmbarcados.ToString());
                fields.SetField("Infantes", _OperacionVueloOtd.INF.ToString());
                fields.SetField("TTL", _OperacionVueloOtd.TTL.ToString());
                fields.SetField("TTC", _OperacionVueloOtd.TTC.ToString());
                fields.SetField("TTP", _OperacionVueloOtd.TRIP.ToString());
                fields.SetField("EX", _OperacionVueloOtd.EX.ToString());
                fields.SetField("PagosUSD", _OperacionVueloOtd.PagoUSD.ToString());
                fields.SetField("PagosCOP", _OperacionVueloOtd.PagoCOP.ToString());

                //fields.SetField("Date", DateTime.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-ES")));

                // "Flatten" the form so it wont be editable/usable anymore
                stamper.FormFlattening = true;

                stamper.Close();
                pdfReader.Close();

                return fileNameNew;
            }
        }

        [HttpGet]
        public async Task<string> DescargarCierreManulaPDF(OperacionVueloOtd _OperacionVueloOtd)
        {
            _logger.LogInformation($"Inicio descarga PDF Cierre");
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25);

            try
            {
                _logger.LogInformation($"paso 1 descarga PDF Transito Conexion");
                MemoryStream ms = new MemoryStream();
                PdfWriter.GetInstance(pdfDoc, ms).CloseStream = false;

                _logger.LogInformation($"paso 2 descarga PDF Transito Conexion");

                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                PdfPTable table = new PdfPTable(3);

                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.DefaultCell.Border = Rectangle.NO_BORDER;

                iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(ruta + "\\logo-jarvis-informe.png");
                _logger.LogInformation($"Imagen 1 descarga PDF Cierre");
                imagen.BorderWidth = 0;
                imagen.ScalePercent(40f);
                PdfPCell cellImg = new PdfPCell();
                cellImg.Border = 0;
                cellImg.HorizontalAlignment = 0;
                cellImg.AddElement(imagen);
                table.AddCell(cellImg);

                Paragraph parrafo = new Paragraph();
                parrafo.Alignment = Element.ALIGN_CENTER;
                parrafo.Font = FontFactory.GetFont("Arial", 10);
                parrafo.Font.SetStyle(Font.BOLD);
                parrafo.Font.SetColor(235, 204, 64);
                parrafo.Add("Cierre Manual ");
                parrafo.Add(new Paragraph("\n"));
                parrafo.Add("Fecha : " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString());

                table.AddCell(parrafo);

                cellImg = new PdfPCell();
                imagen = iTextSharp.text.Image.GetInstance(ruta + "\\opain-logo-informe.png");
                _logger.LogInformation($"Imagen 2 descarga PDF Cierre Manual");
                imagen.BorderWidth = 0;
                imagen.ScalePercent(45f);
                cellImg.Border = 1;
                cellImg.AddElement(imagen);
                cellImg.HorizontalAlignment = 1;
                table.AddCell(cellImg);
                pdfDoc.Add(table);

                // Insertamos salto de linea
                Paragraph saltoDeLinea = new Paragraph("");
                pdfDoc.Add(saltoDeLinea);
                saltoDeLinea = new Paragraph("");
                pdfDoc.Add(saltoDeLinea);
                saltoDeLinea = new Paragraph("");
                pdfDoc.Add(saltoDeLinea);

                // Insertamos salto de linea
                saltoDeLinea = new Paragraph("");
                pdfDoc.Add(saltoDeLinea);
                saltoDeLinea = new Paragraph("");
                pdfDoc.Add(saltoDeLinea);

                //Table
                table = new PdfPTable(16);
                table.WidthPercentage = 100;
                //0=Left, 1=Centre, 2=Right
                table.HorizontalAlignment = 1;
                table.SpacingBefore = 1f;
                table.SpacingAfter = 1f;
                table.HorizontalAlignment = Element.ALIGN_JUSTIFIED;



                table.AddCell("Número de Vuelo");
                table.AddCell("Número de Matrícula");
                table.AddCell("Tipo");
                table.AddCell("Destino");
                table.AddCell("Origen");
                table.AddCell("Fecha de vuelo");
                table.AddCell("Hora de Vuelo");
                table.AddCell("Total Embarcados");
                table.AddCell("Infantes");
                table.AddCell("Transitos en línea");
                table.AddCell("Tránsito en Conexión");
                table.AddCell("Exentos");
                table.AddCell("Tripulantes");
                table.AddCell("Pagos en USD");
                table.AddCell("Pagos en COP");

                PdfPCell CCantidad = new PdfPCell(new Paragraph("Cantidad", FontFactory.GetFont("Calibri", 10, Font.BOLD, new BaseColor(0, 0, 0))));

                CCantidad.NoWrap = true;
                table.AddCell(CCantidad);

                foreach (PdfPCell celda in table.Rows[0].GetCells())
                {
                    celda.BackgroundColor = new iTextSharp.text.BaseColor(235, 204, 64);
                    celda.HorizontalAlignment = 1;
                    celda.Colspan = 1;
                }

                var ClaimNombre = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreUsuario"));


                _logger.LogInformation($"Inicio paso 3.1 descarga PDF Vuelo");
                table.AddCell(_OperacionVueloOtd.Vuelo.ToString());

                _logger.LogInformation($"Inicio paso 3.2 descarga PDF Matricula");
                table.AddCell(_OperacionVueloOtd.Matricula.ToString());

                //_logger.LogInformation($"Inicio paso 3.3 descarga PDF Transito Conexion");
                //table.AddCell(item.FechaLlegada.Day.ToString().PadLeft(2, '0') + "/" + @item.FechaLlegada.Month.ToString().PadLeft(2, '0') + "/" + @item.FechaLlegada.Year.ToString());

                _logger.LogInformation($"Inicio paso 3.4 descarga PDF Tipo");
                table.AddCell(_OperacionVueloOtd.Tipo.ToString());

                _logger.LogInformation($"Inicio paso 3.5 descarga PDF Destino");
                table.AddCell(_OperacionVueloOtd.Destino.ToString());

                _logger.LogInformation($"Inicio paso 3.6 descarga PDF Origen");
                //table.AddCell(_OperacionVueloOtd.Origen.ToString());

                _logger.LogInformation($"Inicio paso 3.6 descarga PDF Transito Conexion");
                table.AddCell(_OperacionVueloOtd.Fecha.ToString());


                //_logger.LogInformation($"Inicio paso 3.7 descarga PDF Transito Conexion");
                //table.AddCell((item.NumeroVueloSalida).ToString());

                //_logger.LogInformation($"Inicio paso 3.8 descarga PDF Transito Conexion");
                //table.AddCell((item.FechaSalida).ToString());

                //_logger.LogInformation($"Inicio paso 3.9 descarga PDF Transito Conexion");
                //table.AddCell((item.HoraSalida).ToString());

                //_logger.LogInformation($"Inicio paso 4.0 descarga PDF Transito Conexion");
                //table.AddCell((item.Destino).ToString());

                //_logger.LogInformation($"Inicio paso 4.1 descarga PDF Transito Conexion");
                //table.AddCell((item.NombrePasajero).ToString());

                //_logger.LogInformation($"Inicio paso 4.2 descarga PDF Transito Conexion");
                //table.AddCell((String.IsNullOrEmpty(ClaimNombre.Value) ? "" : ClaimNombre.Value).ToString());

                ////table.AddCell((String.IsNullOrEmpty(item.NombreAerolinea) ? "" : item.NombreAerolinea).ToString());
                ////table.AddCell("Nombre Aerolinea");

                //_logger.LogInformation($"Inicio paso 4.3 descarga PDF Transito Conexion");
                //table.AddCell((item.FechaHoraFirma).ToString());

                //_logger.LogInformation($"Inicio paso 4.4 descarga PDF Transito Conexion");
                //table.AddCell((item.FechaHoraFirma).ToString());

                //_logger.LogInformation($"Inicio paso 4.5 descarga PDF Transito Conexion");
                //table.AddCell(("1").ToString());


                var FontColour = new BaseColor(255, 255, 255);
                var Calibri8 = FontFactory.GetFont("Calibri", 10, FontColour);
                for (int Pos = 1; Pos <= 14; Pos++)
                {
                    PdfPCell Celda = new PdfPCell(new Paragraph("", Calibri8));
                    Celda.Border = 0;
                    table.AddCell(Celda);
                }
                PdfPCell cell = new PdfPCell(new Paragraph("Total:", Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;

                table.AddCell(cell);

                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;

                table.AddCell(cell);

                _logger.LogInformation($"Sale paso 3 descarga PDF Cierre Manual");

                pdfDoc.Add(table);

                Paragraph parrafofinal = new Paragraph();
                parrafofinal.Alignment = Element.ALIGN_CENTER;
                parrafofinal.Font = FontFactory.GetFont("Arial", 10);
                parrafofinal.Font.SetStyle(Font.BOLD);
                parrafofinal.Font.SetColor(0, 0, 0);
                var ClaimNombreUsuario = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreUsuario"));
                parrafofinal.Add("Generado por: " + ClaimNombreUsuario.Value);
                pdfDoc.Add(parrafofinal);

                writer.CloseStream = false;
                pdfDoc.Close();
                byte[] bytes = ms.ToArray();

                string RutaArchivos = configuration.GetSection("Config:RutaArchivosCierreManual").Value;
                _logger.LogInformation($"paso  5 descarga PDF consulta, Ruta " + RutaArchivos);

                using (MemoryStream stream = new MemoryStream())
                {
                    //if (Directory.Exists(RutaArchivos))
                    //{
                    //    //Directory.Delete(RutaArchivos, true);
                    //    _logger.LogInformation($"Elimina ruta: {RutaArchivos} ");
                    //}

                    if (!Directory.Exists(RutaArchivos))
                    {
                        Directory.CreateDirectory(RutaArchivos);
                        _logger.LogInformation($"Crea la ruta: {RutaArchivos} ");
                    }

                    //if (System.IO.File.Exists(RutaArchivos + "/" + nombreArchivo))
                    //{
                    //    System.IO.File.Delete(RutaArchivos + nombreArchivo);
                    //}
                    string NombraArchivo = User.Identity.Name.Trim().ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');
                    using (FileStream fs = new FileStream(RutaArchivos + "/" + NombraArchivo + ".pdf", FileMode.OpenOrCreate))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();

                    };
                    //return stream;
                    return NombraArchivo + ".pdf";
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                _logger.LogInformation($"paso 6 descarga PDF consulta, error " + ex.Message);
                throw;
            }

        }

        public async Task<bool> Suspender(string ID)
        {
            OperacionVueloOtd OTD = new OperacionVueloOtd();
            try
            {
                string rutaRelativa = configuration.GetSection("URIs:SuspenderVuelo").Value + "IDOperacion=" + ID;
                var result = await servicioApi.PostAsync<bool>(rutaRelativa, OTD);
                if (result == false)
                {
                    throw new Exception("Ha superado el límite de suspensiones admitidas.");
                }
            }
            catch (Exception o)
            {
                throw;
            }
            await NotificacionSuspension(int.Parse(ID));
            return true;
        }

        private async Task SendMail(string _Asunto, string _Para, string _ConCopia, string _Mensaje)
        {
            string urlAPI = configuration.GetSection("URIs:EmailSender").Value;
            Opain.Jarvis.Dominio.Entidades.Notificacion notificacion = new Notificacion
            {
                Asunto = _Asunto,
                Destinos = _Para,
                Copias = _ConCopia,
                Cuerpo = _Mensaje
            };

            bool result = await servicioApi.PostAsync<bool>(urlAPI, notificacion);
        }

        //Notificación de vuelo suspendido
        //envía la notificación a la aerolínea sobre la suspensión del vuelo 
        private async Task NotificacionSuspension(int id)
        {
            OperacionVueloOtd operacionVuelo = new OperacionVueloOtd();
            string Para = "";
            string ConCopia = "";
            try
            {
                string urlAPI = string.Format(configuration.GetSection("URIs:VuelosObtener").Value, id);
                operacionVuelo = await servicioApi.GetAsync<OperacionVueloOtd>(urlAPI);
                int idAreolinea = operacionVuelo.IdAerolinea;
                IEnumerable<UsuarioOtd> usuarios = await servicioApi.GetAsync<IEnumerable<UsuarioOtd>>(configuration.GetSection("URIs:UsuariosObtenerTodos").Value).ConfigureAwait(false);

                foreach (var datosUsuario in usuarios.Where(x => x.UsuarioAerolinea.Any()))
                {
                    foreach (var aerolinea in datosUsuario.UsuarioAerolinea)
                    {
                        if (idAreolinea == aerolinea.IdAerolinea)
                        {
                            Para = Para + datosUsuario.Email.Trim() + ";";
                            _logger.LogInformation($"Notificación de vuelo suspendido, Email Usuario: {datosUsuario.Email.Trim()} ");
                        }
                    }
                }
                urlAPI = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
                IList<AerolineaOtd> lAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(urlAPI);
                IList<AerolineaOtd> oAerolinea = lAerolineas.Where(a => a.Id == idAreolinea).ToList();
                string NombreAerolinea = oAerolinea[0].Nombre;

                string HostImg = configuration.GetSection("SendGrid:HostImg").Value;
                string file = HostImg + "/images/firma.png";

                string Asunto = "Notificación de suspensión del vuelo " + operacionVuelo.Vuelo;
                _logger.LogInformation($"Notificación de suspensión del vuelo");

                if (configuration.GetSection("SendGrid:cc").Value != null)
                {
                    ConCopia = configuration.GetSection("SendGrid:cc").Value;
                }
                string Mensaje = "" +
                    "<p>Buen día estimada aerolínea, " + NombreAerolinea + "</p>" +
                    "<p>Nos permitimos informarle que el vuelo de salida (" + operacionVuelo.Vuelo + ") del " + operacionVuelo.Fecha.ToString("dd-MM-yyyy") +
                    ", será suspendido su proceso de liquidación por 24" +
                    " horas para que pueda hacer las correcciones requeridas antes de la facturación. Recuerde que pasado este tiempo si las" +
                    " correcciones no son efectuadas el sistema de JARVIS procederá a realizar la liquidación con la información suministrada inicialmente.</p>" +
                    "</br><p><b>Tenga en cuenta que esta ventana de espera solo estará disponible por 24 horas a partir del envío de este comunicado.</b></p>" +
                    "</br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <p><img src='" + file + "' width='250' height='70'/></p><p></p> " +
                    "</br> Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ";

                if (Para.IndexOf(";") > 0)
                {
                    Para = Para.Substring(0, Para.Length - 1);
                }
                await SendMail(Asunto, Para, ConCopia, Mensaje);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
            }
        }
    }
}