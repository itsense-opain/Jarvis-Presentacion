using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes;
using Opain.Jarvis.Presentacion.Web.Areas.Informes.ValidacionInformes;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,TECNOLOGIA,AEROLINEA,CARGA,EXTERNO")]
    [Area("Informes")]
    public class InformesController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        private readonly ServicioOracle ServicioOracle;
        private readonly ILogger<InformesController> _logger;

        public InformesController(IConfiguration cfg, IServicioApi api, ServicioOracle oracle, ILogger<InformesController> logger)
        {
            configuration = cfg;
            servicioApi = api;
            ServicioOracle = oracle;
            _logger = logger;

        }

        #region Antiguo
        [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,TECNOLOGIA,AEROLINEA,CARGA,EXTERNO")]
        public IActionResult Principal()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            return View();
        }

        [Authorize(Roles = "ADMINISTRADOR,TECNOLOGIA,EXTERNO")]
        public async Task<IActionResult> InformeAccesos(string inicio, string fin,int  aerolinea)
        {
            var rutaRelativa = string.Format(configuration.GetSection("URIs:AccesoObtenerTodos").Value, inicio, fin, aerolinea);
            IList<AccesoOtd> respuesta = await servicioApi.GetAsync<IList<AccesoOtd>>(rutaRelativa);

            var lista = respuesta.GroupBy(x => new { x.NombreUsuario, x.Rol, x.Grupo }).Select(x => new { key = x.Key.NombreUsuario, value = x.Count(), grupo = x.Key.Grupo, rol = x.Key.Rol });

            return Json(lista);
        }

        [Authorize(Roles = "ADMINISTRADOR,TECNOLOGIA,EXTERNO")]
        public async Task<IActionResult> InformeCargue(string inicio, string fin)
        {
            var rutaRelativa = string.Format(configuration.GetSection("URIs:CargueObtenerTodos").Value, inicio, fin);
            IList<CargueOtd> respuesta = await servicioApi.GetAsync<IList<CargueOtd>>(rutaRelativa);

            var lista = respuesta.GroupBy(x => new { x.TipoArchivo, x.Aerolinea }).Select(x => new { key = x.Key.TipoArchivo, value = x.Count(), grupo = x.Key.Aerolinea });

            return Json(lista);
        }

        [Authorize(Roles = "ADMINISTRADOR,TECNOLOGIA,EXTERNO")]
        public async Task<IActionResult> InformeHoras(string inicio, string fin, int aerolinea)
        {
            var rutaRelativa = string.Format(configuration.GetSection("URIs:AccesoObtenerTodos").Value, inicio, fin, aerolinea);
            IList<AccesoOtd> respuesta = await servicioApi.GetAsync<IList<AccesoOtd>>(rutaRelativa);

            var lista = respuesta
                .GroupBy(x => new { hora = Convert.ToInt32(x.Hora.Substring(0, 2).Replace(':',' ')) })
                .Select(x => new { key = Convert.ToInt32(int.Parse(x.Key.hora.ToString())), value = x.Count() })
                .OrderBy(x => x.key);


            var listado = lista.Select(x => new { key = x.key.ToString(), value = x.value });
            return Json(listado.ToList());
        }

        [Authorize(Roles = "ADMINISTRADOR,TECNOLOGIA,EXTERNO")]
        public async Task<IActionResult> InformeVuelos(string inicio, string fin)
        {
            var mFechaInicio = inicio.Split("/");
            var mFechaFin = fin.Split("/");
            var fechaInicio = string.Format("{0}-{1}-{2}", mFechaInicio[2], mFechaInicio[1], mFechaInicio[0]);
            var fechaFin = string.Format("{0}-{1}-{2}", mFechaFin[2], mFechaFin[1], mFechaFin[0]);
            var rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFin, "INF");
            IList<OperacionVueloOtd> vList = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);
            IList<OperacionVueloOtd> respuesta = vList.Where(v => v.IdCargue != 1).ToList();

            var lista = respuesta
                .GroupBy(x => new { x.Vuelo, x.Tipo, x.NombreAerolinea })
                .Select(x => new { key = x.Key.Vuelo, tipo = x.Key.Tipo, aerolinea = x.Key.NombreAerolinea, value = x.Count()})
                .OrderBy(x => x.key);

            return Json(lista);
        }

        [Authorize(Roles = "ADMINISTRADOR,TECNOLOGIA,EXTERNO")]
        public async Task<IActionResult> InformePasajeros(string inicio, string fin)
        {
            var rutaRelativa = string.Format(configuration.GetSection("URIs:PasajerosPrincipal").Value, 0);
            IList<PasajeroOtd> respuesta = await servicioApi.GetAsync<IList<PasajeroOtd>>(rutaRelativa);

            DateTime fechaInicio = new DateTime(int.Parse(inicio.Substring(6, 4)), int.Parse(inicio.Substring(3, 2)), int.Parse(inicio.Substring(0, 2)));
            DateTime fechaFin = new DateTime(int.Parse(fin.Substring(6, 4)), int.Parse(fin.Substring(3, 2)), int.Parse(fin.Substring(0, 2)));

            var lista = respuesta
                .Where(x => x.Fecha >= fechaInicio && x.Fecha <= fechaFin)
                .GroupBy(x => new { x.TipoVuelo, x.NombreAerolinea ,x.Categoria})
                //.Select(x => new { key = x.Key.NombreAerolinea, tipo = x.Key.TipoVuelo, value = x.Count(), Pasajeros = x.Key.Categoria, CuentaPasajero =x.Key.Categoria.Count()  })
                .Select(x => new { key = x.Key.Categoria,
                    Tipo = x.Key.TipoVuelo,
                    Aerolinea = x.Key.NombreAerolinea, 
                    //Categoria = x.Key.Categoria, 
                    TotalCategoria = x.Key.Categoria.Count(), 
                    Value = x.Count() })
                .OrderBy(x => x.key);

            return Json(lista);
        }

        [Authorize(Roles = "ADMINISTRADOR,TECNOLOGIA,EXTERNO")]
        public async Task<IActionResult> InformeTransitos(string inicio, string fin)
        {
            var rutaRelativa = string.Format(configuration.GetSection("URIs:TransitosPrincipal").Value, 0);
            IList<PasajeroTransitoOtd> respuesta = await servicioApi.GetAsync<IList<PasajeroTransitoOtd>>(rutaRelativa);

            DateTime fechaInicio = new DateTime(int.Parse(inicio.Substring(6, 4)), int.Parse(inicio.Substring(3, 2)), int.Parse(inicio.Substring(0, 2)));
            DateTime fechaFin = new DateTime(int.Parse(fin.Substring(6, 4)), int.Parse(fin.Substring(3, 2)), int.Parse(fin.Substring(0, 2)));

            var lista = respuesta
                .Where(x => x.FechaSalida >= fechaInicio && x.FechaSalida <= fechaFin)
                .GroupBy(x => new { x.TipoVuelo, x.Tipo, x.NombreAerolinea, x.NumeroVueloSalida})
                .Select(x => new { key = x.Key.NumeroVueloSalida, 
                    Tipo = x.Key.TipoVuelo,
                    Categoria = x.Key.Tipo,
                    Aerolinea = x.Key.NombreAerolinea,
                    Vuelo = x.Key.NumeroVueloSalida,
                    Value = x.Count() })
                .OrderBy(x => x.key);

            return Json(lista);
        }

        [Authorize(Roles = "ADMINISTRADOR,TECNOLOGIA,EXTERNO")]
        public async Task<IActionResult> InformeDestinos(string inicio, string fin)
        {
            var mFechaInicio = inicio.Split("/");
            var mFechaFin = fin.Split("/");
            var fechaInicio = string.Format("{0}-{1}-{2}", mFechaInicio[2], mFechaInicio[1], mFechaInicio[0]);
            var fechaFin = string.Format("{0}-{1}-{2}", mFechaFin[2], mFechaFin[1], mFechaFin[0]);
            var rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFin, "INF");
            IList<OperacionVueloOtd> vList = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);
            IList<OperacionVueloOtd> respuesta = vList.Where(v => v.IdCargue != 1).ToList();

            var lista = respuesta
                .GroupBy(x => new { x.Tipo, x.NombreAerolinea, x.Destino })
                .Select(x => new { key = x.Key.Destino, tipo = x.Key.Tipo, aerolinea = x.Key.NombreAerolinea, value = x.Count(), Destino = x.Key.Destino })
                .OrderBy(x => x.key);

            return Json(lista);
        }
        #endregion

        #region Informes

        #region PuenteAbordaje --- Anexo1
        [HttpGet]
        public async Task<IActionResult> PuenteAbordaje()
        {
            IniciarViewBagFiltroBusqueda();

            ViewBag.aerolinea = 0;
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            return View(new List<Anexo1>());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PuenteAbordaje(string aerolinea, string tipo, string facturaDesde, string facturaHasta, string startDate, string endDate)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }

            ViewBag.aerolinea = aerolinea;
            ViewBag.tipo = tipo;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            if (string.IsNullOrEmpty(tipo) ||
                string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo1>());
            }
            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_PuenteAbordaje").Value,
                                                aerolinea,
                                                startDate,
                                                endDate,
                                                facturaDesde,
                                                facturaHasta,
                                                tipo,
                                                false);
            ViewBag.ConsultaExcel = "PuenteAbordaje" + ";" + ViewBag.aerolinea + ";" + ViewBag.tipo + ";" + ViewBag.facturaDesde + ";" + ViewBag.facturaHasta + ";" + startDate + ";" + endDate + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo1> Anexos1 = await ServicioOracle.GetAsync<List<Anexo1>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo1>());
            }

            if (Anexos1 == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo1>());
            }
            if (Anexos1.Count == 0)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo1>());
            }

            return View(Anexos1);
        }
        #endregion

        #region ParqueoAeronaves --- Anexo2
        [HttpGet]
        public async Task<IActionResult> ParqueoAeronaves()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return  View(new List<Anexo2>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ParqueoAeronaves(string aerolinea, string tipo, string facturaDesde, string facturaHasta, string startDate, string endDate)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.aerolinea = aerolinea;
            ViewBag.tipo = tipo;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            if (string.IsNullOrEmpty(tipo) ||
                string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo2>());
            }

            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }
            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_ParqueoAeronaves").Value,
                                                aerolinea,
                                                startDate,
                                                endDate,
                                                facturaDesde,
                                                facturaHasta,
                                                tipo,
                                                false);
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo2> Anexos2 = await ServicioOracle.GetAsync<List<Anexo2>>(rutaRelativa);
            ViewBag.ConsultaExcel = "ParqueoAeronaves" + ";" + ViewBag.aerolinea + ";" + startDate + ";" + endDate + ";" + ViewBag.facturaDesde + ";" + ViewBag.facturaHasta + ";" + ViewBag.tipo + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo2>());
            }

            if (Anexos2 == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo2>());
            }

            return View(Anexos2);
        }
        #endregion

        #region TasasAeroportuariasCOP --- Anexo3
        [HttpGet]
        public async Task<IActionResult> TasasAeroportuariasCOP()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo3>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TasasAeroportuariasCOP(string aerolinea, string tipo, string facturaDesde, string facturaHasta, string startDate, string endDate)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.aerolinea = aerolinea;
            ViewBag.tipo = tipo;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            if (string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo3>());
            }
            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }
            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_TasasAeroportuariasCOP").Value,
                                                aerolinea,
                                                startDate,
                                                endDate,
                                                facturaDesde,
                                                facturaHasta,
                                                tipo, false);
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo3> Resultado = await ServicioOracle.GetAsync<List<Anexo3>>(rutaRelativa);
            ViewBag.ConsultaExcel = "TasasAeroportuariasCOP" + ";" + ViewBag.aerolinea + ";" + startDate + ";" + endDate + ";" + ViewBag.facturaDesde + ";" + ViewBag.facturaHasta + ";" + ViewBag.tipo + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo3>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo3>());
            }

            return View(Resultado);
        }
        #endregion

        #region TasasAeroportuariasUSD --- Anexo4
        [HttpGet]
        public async Task<IActionResult> TasasAeroportuariasUSD()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo4>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TasasAeroportuariasUSD(string aerolinea, string tipo, string facturaDesde, string facturaHasta, string startDate, string endDate)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);
                aerolinea = idAerolinea.ToString();
            }
       
            ViewBag.aerolinea = aerolinea;
            ViewBag.tipo = tipo;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;


            if (string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo4>());
            }

            double fchIniJuliana = 0;
            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_TasasAeroportuariasUSD").Value,
                                                aerolinea,
                                                startDate,
                                                endDate,
                                                facturaDesde,
                                                facturaHasta,
                                                tipo, false);
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo4> Resultado = await ServicioOracle.GetAsync<List<Anexo4>>(rutaRelativa);
            ViewBag.ConsultaExcel = "TasasAeroportuariasUSD" + ";" + ViewBag.aerolinea + ";" + startDate + ";" + endDate + ";" + ViewBag.facturaDesde + ";" + ViewBag.facturaHasta + ";" + ViewBag.tipo + ";" + ViewBag.startDate + ";" + ViewBag.endDate;

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo4>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo4>());
            }

            return View(Resultado);
        }

        public double ConvertToJulian(DateTime Date)
        {

            double Year = Date.Year;

            double dato = (Year - 1900) * 1000 + Date.DayOfYear;

            return dato;

        }


        #endregion

        #region Mostradores --- Anexo5
        [HttpGet]
        public async Task<IActionResult> Mostradores()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo5>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mostradores(string aerolinea, string startDate, string endDate, string facturaDesde, string facturaHasta)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.aerolinea = aerolinea;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;


            if (string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo5>());
            }
            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }
           

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_Mostradores").Value,
                                                aerolinea,
                                                startDate,
                                                endDate,
                                                facturaDesde,
                                                facturaHasta, false);
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo5> Resultado = await ServicioOracle.GetAsync<List<Anexo5>>(rutaRelativa);
            ViewBag.ConsultaExcel = "MostradoresMostradores" + ";" + ViewBag.aerolinea + ";" + startDate + ";" + endDate + ";" + ViewBag.facturaDesde + ";" + ViewBag.facturaHasta + ";" + ViewBag.startDate + ";" + ViewBag.endDate;


            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo5>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo5>());
            }

            return View(Resultado);
        }
        #endregion

        #region DetallesVuelosInterventoria --- Anexo6
        [HttpGet]
        public async Task<IActionResult> DetallesVuelosInterventoria()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo6>());
        }

        [HttpGet]
        public async Task<IActionResult> DetallesVuelosInterventoriaInfrasa()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo6>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetallesVuelosInterventoria(string startDate, string endDate, string tipoVuelo, string estado, string aerolinea)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.tipoVuelo = tipoVuelo;
            ViewBag.estado = estado;
            ViewBag.aerolinea = aerolinea;

            if (string.IsNullOrEmpty(tipoVuelo) ||
                string.IsNullOrEmpty(estado) ||
                string.IsNullOrEmpty(startDate) ||
                string.IsNullOrEmpty(endDate))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo6>());
            }

            double fchIniJuliana = 0;

            CultureInfo cultureinfo = new CultureInfo("en-gb");

            DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
            if (startDate != "")
            {

                fchIniJuliana = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                startDate = fchIniJuliana.ToString();
            }

            double fchFinJuliana = 0;

            DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

            if (endDate != "")
            {

                fchFinJuliana = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                endDate = fchFinJuliana.ToString();
            }


            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_DetalleVuelosInterventoria").Value,
                                                startDate,
                                                endDate,
                                                tipoVuelo,
                                                estado,
                                                aerolinea, false);
            ViewBag.ConsultaExcel = "DetallesVuelosInterventoria" + ";" + startDate + ";" + endDate + ";" + ViewBag.tipoVuelo + ";" + ViewBag.estado + ";" + ViewBag.aerolinea + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo6> Resultado = await ServicioOracle.GetAsync<List<Anexo6>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo6>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo6>());
            }

            return View(Resultado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetallesVuelosInterventoriaInfrasa(string startDate, string endDate, string tipoVuelo, string estado, string aerolinea)
        {
            if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA"))
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }


            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.tipoVuelo = tipoVuelo;
            ViewBag.estado = estado;
            ViewBag.aerolinea = aerolinea;

            if (string.IsNullOrEmpty(tipoVuelo) ||
                string.IsNullOrEmpty(estado) ||
                string.IsNullOrEmpty(startDate) ||
                string.IsNullOrEmpty(endDate))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo6>());
            }

            double fchIniJuliana = 0;

            CultureInfo cultureinfo = new CultureInfo("en-gb");

            DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);

            if (startDate != "")
            {

                fchIniJuliana = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                startDate = fchIniJuliana.ToString();
            }

            double fchFinJuliana = 0;

            DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

            if (endDate != "")
            {

                fchFinJuliana = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                endDate = fchFinJuliana.ToString();
            }


            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_DetalleVuelosInterventoria").Value,
                                                startDate,
                                                endDate,
                                                tipoVuelo,
                                                estado, aerolinea, false);
            
            ViewBag.ConsultaExcel = "DetallesVuelosInterventoriaInfrasa" + ";" + startDate + ";" + endDate + ";" + tipoVuelo + ";" + estado + ";" + ViewBag.startDate + ";" + ViewBag.endDate + ";" + aerolinea;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo6> Resultado = await ServicioOracle.GetAsync<List<Anexo6>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo6>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo6>());
            }

            return View(Resultado);
        }
        #endregion

        #region ResumenDetalleTasasAeroportuariasFacturadas --- Anexo7B
        [HttpGet]
        public async Task<IActionResult> ResumenDetalleTasasAeroportuariasFacturadas()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo7B>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResumenDetalleTasasAeroportuariasFacturadas(string startDate, string endDate, string tipoVuelo, string aerolinea)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.tipoVuelo = tipoVuelo;
            ViewBag.aerolinea = aerolinea;

            if (string.IsNullOrEmpty(tipoVuelo) ||
                string.IsNullOrEmpty(aerolinea) ||
                string.IsNullOrEmpty(startDate) ||
                string.IsNullOrEmpty(endDate))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo7B>());
            }

            double fchIniJuliana = 0;

            CultureInfo cultureinfo = new CultureInfo("en-gb");

            DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);

            if (startDate != "")
            {

                fchIniJuliana = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                startDate = fchIniJuliana.ToString();
            }

            double fchFinJuliana = 0;

            DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

            if (endDate != "")
            {

                fchFinJuliana = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                endDate = fchFinJuliana.ToString();
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_ResumenDetalleTasasAeroportuariasFacturadas").Value,
                                                startDate,
                                                endDate,
                                                tipoVuelo,
                                                aerolinea,false);
            ViewBag.ConsultaExcel = "ResumenDetalleTasasAeroportuarias" + ";" + startDate + ";" + endDate + ";" + tipoVuelo + ";" + aerolinea + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo7B> Resultado = await ServicioOracle.GetAsync<List<Anexo7B>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo7B>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo7B>());
            }

            return View(Resultado);
        }
        #endregion

        #region UsoCarroBomberos --- Anexo8
        [HttpGet]
        public async Task<IActionResult> UsoCarroBomberos()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo8>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsoCarroBomberos(string startDate, string endDate, string facturaDesde, string facturaHasta, string aerolinea)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.aerolinea = aerolinea;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            if (string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo8>());
            }
            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_UsoCarroBomberos").Value,
                                                startDate,
                                                endDate,
                                                facturaDesde,
                                                facturaHasta,
                                                aerolinea ,false);
            ViewBag.ConsultaExcel = "UsoCarroBomberos" + ";" + startDate + ";" + endDate + ";" + facturaDesde + ";" + facturaHasta + ";" + aerolinea + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo8> Resultado = await ServicioOracle.GetAsync<List<Anexo8>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo8>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo8>());
            }

            if (Resultado.Count == 0)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo8>());
            }

            return View(Resultado);
        }
        #endregion

        #region InfrasasInfantes --- Anexo9
        [HttpGet]
        public async Task<IActionResult> InfrasasInfantes()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo9>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InfrasasInfantes(string startDate, string endDate, string tipoVuelo, string aerolinea, string estado)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.tipoVuelo = tipoVuelo;
            ViewBag.estado = estado;




            if (string.IsNullOrEmpty(tipoVuelo) ||
                string.IsNullOrEmpty(estado) ||
                string.IsNullOrEmpty(startDate) ||
                string.IsNullOrEmpty(endDate))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo9>());
            }

            double fchIniJuliana = 0;

            CultureInfo cultureinfo = new CultureInfo("en-gb");

            DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);

            if (startDate != "")
            {

                fchIniJuliana = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                startDate = fchIniJuliana.ToString();
            }

            double fchFinJuliana = 0;

            DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

            if (endDate != "")
            {

                fchFinJuliana = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                endDate = fchFinJuliana.ToString();
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_InfrasasInfantes").Value,
                                                startDate,
                                                endDate,
                                                tipoVuelo,
                                                aerolinea,
                                                estado);
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo9> Resultado = await ServicioOracle.GetAsync<List<Anexo9>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo9>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo9>());
            }

            return View(Resultado);
        }
        #endregion

        #region ResumenTasasAeroportuariasFacturadas --- Anexo10
        [HttpGet]
        public async Task<IActionResult> ResumenTasasAeroportuariasFacturadas()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo10>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResumenTasasAeroportuariasFacturadas(string startDate, string endDate, string tipoVuelo, string aerolinea)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.aerolinea = aerolinea;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            if (string.IsNullOrEmpty(startDate) ||
                string.IsNullOrEmpty(endDate))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo10>());
            }
            string format = "dd/MM/yyyy";
            DateTime Fechainicial = DateTime.ParseExact(startDate, format, CultureInfo.InvariantCulture);
            DateTime Fechafinal = DateTime.ParseExact(endDate, format, CultureInfo.InvariantCulture);
            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_ResumenTasasAeroportuariasFacturadas").Value,
                                                ConvertToJulian(Convert.ToDateTime(Fechainicial)),
                                                ConvertToJulian(Convert.ToDateTime(Fechafinal)),
                                                tipoVuelo,
                                                aerolinea);
            ViewBag.ConsultaExcel = "ResumenTasasAeroportuariasFacturadas" + ";" + ConvertToJulian(Convert.ToDateTime(Fechainicial)) + ";" + ConvertToJulian(Convert.ToDateTime(Fechafinal)) + ";" + tipoVuelo + ";" + aerolinea + ";" + Fechainicial.ToShortDateString() + ";" + Fechafinal.ToShortDateString();
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo10> Resultado = await ServicioOracle.GetAsync<List<Anexo10>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo10>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo10>());
            }

            return View(Resultado);
        }
        #endregion

        #region TasasAeroportuariasFacturadasporAerolinea --- Anexo11
        [HttpGet]
        public async Task<IActionResult> TasasAeroportuariasFacturadasporAerolinea()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo11>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TasasAeroportuariasFacturadasporAerolinea(string startDate, string endDate, string tipo)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.tipoVuelo = tipo;

            if (string.IsNullOrEmpty(tipo) ||
                string.IsNullOrEmpty(startDate) ||
                string.IsNullOrEmpty(endDate))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo11>());
            }
            string format = "dd/MM/yyyy";
            DateTime Fechainicial = DateTime.ParseExact(startDate, format, CultureInfo.InvariantCulture);
            DateTime Fechafinal = DateTime.ParseExact(endDate, format, CultureInfo.InvariantCulture);
            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_TasasAeroportuariasFacturadasporAerolineas").Value,
                                              ConvertToJulian(Convert.ToDateTime(Fechainicial)),
                                              ConvertToJulian(Convert.ToDateTime(Fechafinal)),
                                              tipo, false);
            ViewBag.ConsultaExcel = "TasasAeroportuariasFacturadasporAerolinea" + ";" + ConvertToJulian(Convert.ToDateTime(Fechainicial)) + ";" + ConvertToJulian(Convert.ToDateTime(Fechafinal)) + ";" + tipo + ";" + Fechainicial + ";" + Fechafinal;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo11> Resultado = await ServicioOracle.GetAsync<List<Anexo11>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo11>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo11>());
            }

            return View(Resultado);
        }
        #endregion

        #region SoportesExencionesTasasAeroportuarias --- Anexo12
        [HttpGet]
        public async Task<IActionResult> SoportesExencionesTasasAeroportuarias()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo12>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoportesExencionesTasasAeroportuarias(string startDate, string endDate, string aerolinea)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.aerolinea = aerolinea;

            if (string.IsNullOrEmpty(startDate) ||
                 string.IsNullOrEmpty(endDate))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo12>());
            }
            string format = "dd/MM/yyyy";
            DateTime Fechainicial = DateTime.ParseExact(startDate, format, CultureInfo.InvariantCulture);
            DateTime Fechafinal = DateTime.ParseExact(endDate, format, CultureInfo.InvariantCulture);

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_SoporteExencionesTasasAeroportuarias").Value,
                                                ConvertToJulian(Convert.ToDateTime(Fechainicial)),
                                              ConvertToJulian(Convert.ToDateTime(Fechafinal)),
                                              aerolinea,false);
            ViewBag.ConsultaExcel = "SoportesExenciones" + ";" + ConvertToJulian(Convert.ToDateTime(Fechainicial)) + ";" + ConvertToJulian(Convert.ToDateTime(Fechafinal)) + ";" + aerolinea + ";" + Fechainicial + ";" + Fechafinal;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo12> Resultado = await ServicioOracle.GetAsync<List<Anexo12>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo12>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo12>());
            }

            return View(Resultado);
        }
        #endregion

        #region FacturacionParqueosAmplicacionLA33 --- Anexo13
        [HttpGet]
        public async Task<IActionResult> FacturacionParqueosAmplicacionLA33()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;
            return View(new List<Anexo13>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FacturacionParqueosAmplicacionLA33(string startDate, string endDate, string facturaDesde, string facturaHasta, string tipo, string aerolinea)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.tipo = tipo;
            ViewBag.aerolinea = aerolinea;
            if (string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo13>());
            }
            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_FacturacionParqueosAmplicacionLA33").Value,
                                                     facturaDesde, facturaHasta, tipo, aerolinea,false,startDate,endDate);
            ViewBag.ConsultaExcel = "FacturacionParqueosAmplicacionLA33" + ";" + facturaDesde + ";" + facturaHasta + ";" + tipo + ";" + aerolinea + ";" + startDate + ";"+ endDate + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo13> Resultado = await ServicioOracle.GetAsync<List<Anexo13>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo13>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo13>());
            }

            return View(Resultado);
        }
        #endregion

        #region FacturacionParqueosAmplicacionLA33100 --- Anexo14
        [HttpGet]
        public async Task<IActionResult> FacturacionParqueosAmplicacionLA33100()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo14>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FacturacionParqueosAmplicacionLA33100(string startDate,string endDate, string facturaDesde, string facturaHasta, string tipo, string aerolinea)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.tipo = tipo;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.aerolinea = aerolinea;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            if (string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo14>());
            }

            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_FacturacionParqueosAmplicacionLA33100").Value,
                                                aerolinea, facturaDesde, facturaHasta, tipo,false,startDate,endDate);
            ViewBag.ConsultaExcel = "FacturacionParqueosAmplicacionLA33100" + ";" + aerolinea + ";" + facturaDesde + ";" + facturaHasta + ";" + tipo + ";" + startDate + ";" + endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo14> Resultado = await ServicioOracle.GetAsync<List<Anexo14>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo14>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo14>());
            }

            return View(Resultado);
        }
        #endregion

        #region FacturacionAmplicacionLA33PuentesAbordaje --- Anexo15
        [HttpGet]
        public async Task<IActionResult> FacturacionAmplicacionLA33PuentesAbordaje()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo15>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FacturacionAmplicacionLA33PuentesAbordaje(string startDate, string endDate, string tipo, string aerolinea, string facturaDesde, string facturaHasta)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;
            ViewBag.tipo = tipo;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.aerolinea = aerolinea;
            if (string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo15>());
            }
            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }
            ViewBag.ConsultaExcel = "FacturacionAmplicacionLA33Puentes" + ";" + ViewBag.aerolinea + ";" + ViewBag.facturaDesde + ";" + ViewBag.facturaHasta + ";" + startDate + ";" + endDate;
            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_FacturacionAmplicacionLA33PuentesAbordaje").Value,
                                                aerolinea, facturaDesde, facturaHasta, startDate,endDate);

            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo15> Resultado = await ServicioOracle.GetAsync<List<Anexo15>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo15>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo15>());
            }

            return View(Resultado);
        }
        #endregion

        #region GPU --- Anexo16
        [HttpGet]
        public async Task<IActionResult> GPU()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo16>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GPU(string startDate, string endDate, string tipo, string aerolinea, string facturaDesde, string facturaHasta)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.tipo = tipo;
            ViewBag.aerolinea = aerolinea;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;

            if (
                string.IsNullOrEmpty(tipo) ||
                string.IsNullOrEmpty(aerolinea) ||
                string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta)
               )
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo16>());
            }
            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_GPU").Value,
                                                startDate,
                                                endDate,
                                                tipo,
                                                aerolinea,
                                                facturaDesde,
                                                facturaHasta,false);
            ViewBag.ConsultaExcel = "GPU" + ";" + startDate + ";" + endDate + ";" + tipo + ";" + aerolinea + ";" + facturaDesde + ";" + facturaHasta + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo16> Resultado = await ServicioOracle.GetAsync<List<Anexo16>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo16>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo16>());
            }

            return View(Resultado);
        }
        #endregion

        #region GPUAerocivil --- Anexo17
        [HttpGet]
        public async Task<IActionResult> GPUAerocivil()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo17>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GPUAerocivil(string startDate, string endDate, string tipo, string aerolinea, string facturaDesde, string facturaHasta)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;

             if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR CARGA") || User.IsInRole("CARGA")  )
            {
                int idAerolinea = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
                aerolinea = idAerolinea.ToString();
            }
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.tipo = tipo;
            ViewBag.aerolinea = aerolinea;
            ViewBag.facturaDesde = facturaDesde;
            ViewBag.facturaHasta = facturaHasta;

            if (
                string.IsNullOrEmpty(tipo) ||
                string.IsNullOrEmpty(aerolinea) ||
                string.IsNullOrEmpty(facturaDesde) ||
                string.IsNullOrEmpty(facturaHasta)
               )
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo17>());
            }

            if (startDate != null && endDate != null)
            {
                double fchIniJulianaA5 = 0;
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fchIniJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fchIniJulianaA5.ToString();
                }

                double fchFinJulianaA5 = 0;

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fchFinJulianaA5 = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fchFinJulianaA5.ToString();
                }
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_GPUAerocivil").Value,
                                                startDate,
                                                endDate,
                                                tipo,
                                                aerolinea,
                                                facturaDesde,
                                                facturaHasta,false);
            ViewBag.ConsultaExcel = "GPUAerocivil" + ";" + startDate + ";" + endDate + ";" + tipo + ";" + aerolinea + ";" + facturaDesde + ";" + facturaHasta  + ";" + ViewBag.startDate + ";" + ViewBag.endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo17> Resultado = await ServicioOracle.GetAsync<List<Anexo17>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo17>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo17>());
            }

            return View(Resultado);
        }
        #endregion

        #region InformeCarnetizacion --- Anexo19
        [HttpGet]
        public async Task<IActionResult> InformeCarnetizacion()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            return View(new List<Anexo19>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InformeCarnetizacion(string startDate, string endDate)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            if (string.IsNullOrEmpty(startDate) ||
                string.IsNullOrEmpty(endDate))
            {
                ViewBag.Error = "Error filtro búsqueda";
                return View(new List<Anexo19>());
            }
            Double fechajulianaCar = 0;
            Double fechajulianaCarf = 0;
            if (startDate != null && endDate != null)
            {
                
                CultureInfo cultureinfo = new CultureInfo("en-gb");
                DateTime tempDateIni = Convert.ToDateTime(startDate, cultureinfo);
                if (startDate != "")
                {

                    fechajulianaCar = ConvertToJulian(Convert.ToDateTime(tempDateIni));

                    startDate = fechajulianaCar.ToString();
                }

           

                DateTime tempDateFin = Convert.ToDateTime(endDate, cultureinfo);

                if (endDate != "")
                {

                    fechajulianaCarf = ConvertToJulian(Convert.ToDateTime(tempDateFin));

                    endDate = fechajulianaCarf.ToString();
                }
            }

            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_InformeCarnetizacion").Value,
                                                startDate,
                                                endDate, false);
            ViewBag.ConsultaExcel = "InformeCarnetizacion" + ";" + ViewBag.startDate + ";" + ViewBag.endDate + ";" + startDate + ";" + endDate;
            var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
            await ServicioOracle.AddToken<string>(token);
            List<Anexo19> Resultado = await ServicioOracle.GetAsync<List<Anexo19>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
            {
                ViewBag.Error = "Error búsqueda";
                return View(new List<Anexo19>());
            }

            if (Resultado == null)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<Anexo19>());
            }

            return View(Resultado);
        }
        #endregion

        #region InformeCobro
        [HttpGet]
        public async Task<IActionResult> InformeCobro()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            string rutaRelativa = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
            IList<AerolineaOtd> listaAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            ViewData["listaAerolinea"] = listaAerolineas.OrderBy(x => x.Nombre).ToList();
            ViewBag.aerolinea = 0;

            return View(new List<InformeCobro>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InformeCobro(string startDate, string endDate, string facturaHasta, string aerolinea)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            string rutaRelativa = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
            IList<AerolineaOtd> listaAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            ViewData["listaAerolinea"] = listaAerolineas.OrderBy(x => x.Nombre).ToList();

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.aerolinea = aerolinea;
            ViewBag.facturaHasta = facturaHasta;
           
            if (facturaHasta == null || facturaHasta == "")
            {
                facturaHasta = "0";
            }
            string AerolineaSel = "";
            if (User.IsInRole("OPAIN") || User.IsInRole("ADMINISTRADOR"))
            {
                AerolineaSel = aerolinea;
            }
            else { AerolineaSel = User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value; }

            string rutaRelativaCobro = string.Format(configuration.GetSection("URIs:Informes_Cobros").Value, AerolineaSel, facturaHasta, startDate, endDate);
            DataTable respuestaCobro = await servicioApi.GetAsync<DataTable>(rutaRelativaCobro);
            ViewBag.ConsultaExcel = "InformeCobro" + ";" + AerolineaSel + ";" + facturaHasta + ";" + startDate + ";" + endDate;
            List<InformeCobro> listaInfoCobro = new List<InformeCobro>();
            if (respuestaCobro.Rows.Count > 1)
            {
                foreach (DataRow itemCobro in respuestaCobro.Rows)
                {
                    try
                    {
                        InformeCobro InfoCobro = new InformeCobro();
                        InfoCobro.lstNovedades = new List<string>();
                        // traigo las novedades
                        string rutaRelativaNoves = configuration.GetSection("URIs:ObtenerNovedadesxOperacion").Value;
                        IList<NovedadOtd> respuesta = await servicioApi.GetAsync<IList<NovedadOtd>>(string.Format(rutaRelativaNoves, itemCobro["Id"].ToString()));

                        foreach (var item in respuesta)
                        {
                            InfoCobro.lstNovedades.Add(item.DescCausal);
                        }

                        InfoCobro.Concepto = "";
                        InfoCobro.DiferenciaTasas = itemCobro["DiferenciaTasas"].ToString();
                        InfoCobro.exentos = Convert.ToInt32(itemCobro["exentos"].ToString());
                        InfoCobro.fechaCargue = Convert.ToDateTime((itemCobro["fechaCargue"].ToString() == string.Empty) ? DateTime.MinValue : itemCobro["fechaCargue"]);
                        InfoCobro.FechaSalida = Convert.ToDateTime(itemCobro["FechaSalida"]);
                        InfoCobro.HoraCargue = itemCobro["HoraCargue"].ToString();
                        InfoCobro.Infantes = Convert.ToInt32(itemCobro["Infantes"].ToString());
                        InfoCobro.Linea = Convert.ToInt32(itemCobro["Linea"].ToString());
                        InfoCobro.Matricula = itemCobro["Matricula"].ToString();
                        InfoCobro.NumeroVuelo = itemCobro["NumeroVuelo"].ToString();
                        InfoCobro.Oaci = itemCobro["Oaci"].ToString();
                        InfoCobro.Pasajeros = Convert.ToInt32(itemCobro["Pasajeros"].ToString());
                        InfoCobro.TasasCobradas = itemCobro["TasasCobradas"].ToString();
                        InfoCobro.TasasReportadas = itemCobro["TasasReportadas"].ToString();
                        InfoCobro.TipoVuelo = itemCobro["TipoVuelo"].ToString();
                        InfoCobro.TransitoConexion = Convert.ToInt32(itemCobro["TransitoConexion"].ToString());
                        InfoCobro.tripulantes = Convert.ToInt32(itemCobro["tripulantes"].ToString());
                        InfoCobro.Usuario = itemCobro["Usuario"].ToString();
                        listaInfoCobro.Add(InfoCobro);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.StackTrace.ToString());
                        throw;
                    }

                }
            }


            if (listaInfoCobro.Count() < 2)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<InformeCobro>());
            }

            return View(listaInfoCobro);
        }
        #endregion
        #endregion

        #region InformeTickets
        [HttpGet]
        public async Task<IActionResult> InformeTickets()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            IniciarViewBagFiltroBusqueda();
            string rutaRelativa = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
            IList<AerolineaOtd> listaAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            ViewData["listaAerolinea"] = listaAerolineas.OrderBy(x => x.Nombre).ToList();
            ViewBag.aerolinea = 0;
            ViewBag.tipoTicket = "0";
            return View(new List<InformeTickets>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InformeTickets(string startDate, string endDate, string aerolinea, string tipoticket)
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            string rutaRelativa = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
            IList<AerolineaOtd> listaAerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            ViewData["listaAerolinea"] = listaAerolineas.OrderBy(x => x.Nombre).ToList();

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            ViewBag.aerolinea = aerolinea;
            ViewBag.tipoTicket = tipoticket;
            ViewBag.ConsultaExcel = "InformeTickets" + ";" + ViewBag.startDate + ";" + ViewBag.endDate + ";" + ViewBag.aerolinea;
            string AerolineaSel = aerolinea;
            if (User.IsInRole("OPAIN") || User.IsInRole("ADMINISTRADOR"))
            {
                AerolineaSel = aerolinea;
            }

            string rutaRelativaCobro = string.Format(configuration.GetSection("URIs:Informes_Tickets").Value, AerolineaSel, startDate, endDate, '0');
            DataTable respuestaticket = await servicioApi.GetAsync<DataTable>(rutaRelativaCobro);
            List<InformeTickets> listaInfotickes = new List<InformeTickets>();
            if (respuestaticket != null)
            {
                if (respuestaticket.Rows.Count >= 1)
                {
                    foreach (DataRow itemCobro in respuestaticket.Rows)
                    {
                        try
                        {
                            InformeTickets Infotickes = new InformeTickets();

                            Infotickes.Aerolinea = itemCobro["Aerolinea"].ToString();
                            Infotickes.Adjunto = itemCobro["Adjunto"].ToString();
                            Infotickes.AdjuntoRespuesta = itemCobro["AdjuntoRespuesta"].ToString();
                            Infotickes.FechaSolicitud = Convert.ToDateTime((itemCobro["FechaSolicitud"].ToString() == string.Empty) ? DateTime.MinValue : itemCobro["FechaSolicitud"]);
                            Infotickes.FechaRespuesta = Convert.ToDateTime((itemCobro["FechaRespuesta"].ToString() == string.Empty) ? DateTime.MinValue : itemCobro["FechaSolicitud"]);
                            Infotickes.Asunto = itemCobro["Asunto"].ToString();
                            Infotickes.Depende = itemCobro["Depende"].ToString();
                            Infotickes.EstadoFinal = itemCobro["EstadoFinal"].ToString();
                            Infotickes.NumeroTicket = itemCobro["NumeroTicket"].ToString();
                            Infotickes.Respuesta = itemCobro["Respuesta"].ToString();
                            Infotickes.TipoTicket = itemCobro["TipoTicket"].ToString();
                            Infotickes.Usuario = itemCobro["Usuario"].ToString();
                            Infotickes.UsuarioRespuesta = itemCobro["UsuarioRespuesta"].ToString();
                            listaInfotickes.Add(Infotickes);
                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.StackTrace.ToString());
                            throw;
                        }

                    }
                }
            }


            if (listaInfotickes.Count() < 1)
            {
                ViewBag.Error = "No hay registros";
                return View(new List<InformeTickets>());
            }

            return View(listaInfotickes);
        }

        #endregion


        private void IniciarViewBagFiltroBusqueda()
        {
            ViewBag.aerolinea = string.Empty;
            ViewBag.tipo = string.Empty;
            ViewBag.facturaDesde = string.Empty;
            ViewBag.facturaHasta = string.Empty;
            ViewBag.startDate = string.Empty;
            ViewBag.endDate = string.Empty;
            ViewBag.tipoVuelo = string.Empty;
            ViewBag.estado = string.Empty;
        }

        #region "Descarga Archivos de aerolinea"
        [HttpGet]
        public async Task<IActionResult> DescargaArchivosAerolinea()
        {
            ViewData["UrlPsePagos"] = configuration.GetSection("Cofiguracion:UrlPsePagos").Value;
            InformeDescarga infdesc = new InformeDescarga();
            infdesc.Aerolinea = string.Empty;
            infdesc.FechaInicial = string.Empty;
            infdesc.FechaFinal = string.Empty;
            IniciarViewBagFiltroBusqueda();

            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            ViewData["Aerolineas"] = respuesta;

            return View(infdesc);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DescargaArchivosAerolinea(IFormCollection collection)
        {
            string format = "dd/MM/yyyy";
            DateTime fechainicial = DateTime.ParseExact(collection["startDate"].ToString(), format, CultureInfo.InvariantCulture);
            DateTime fechafinal = DateTime.ParseExact(collection["endDate"].ToString(), format, CultureInfo.InvariantCulture);
            //DateTime fechainicial = Convert.ToDateTime(collection["startDate"].ToString());
            //DateTime fechafinal = Convert.ToDateTime(collection["endDate"].ToString());
            string Aerolinea = collection["aerolinea"].ToString();


            string rutaRelativaAero = configuration.GetSection("URIs:AerolineaObtenerTodos").Value;
            IList<AerolineaOtd> respuestaaero = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
           
            string Abreviatura = string.Empty;
            string NombreAerolinea = string.Empty;
            string TodasAerolinea = string.Empty;
            int idAerolinea = 0;

            //TodasAerolinea = JsonConvert.SerializeObject(respuestaaero);

            if (Aerolinea.ToString() == "0")
            {
                Abreviatura = "0";
                
            }
            else
            {
                Abreviatura = respuestaaero.Where(p => p.Codigo == Aerolinea).FirstOrDefault().Sigla.TrimEnd();
                idAerolinea = respuestaaero.Where(p => p.Codigo == Aerolinea).FirstOrDefault().Id;
                NombreAerolinea = respuestaaero.Where(p => p.Codigo == Aerolinea).FirstOrDefault().Nombre.Trim();
            }

            // debe traerlo por el servicio
            //string rutaZips = string.Format(configuration.GetSection("URIs:Informes_TraerZips").Value, Aerolinea, fechafinal, fechainicial);
            //var resp = await servicioApi.GetAsync<ActionResult>(rutaZips);
            try
            {
                var urlServicio = configuration.GetSection("Rutas:BaseServicio").Value;
                using (var cliente = new HttpClient())
                {
                    cliente.BaseAddress = new Uri(urlServicio);
                    cliente.DefaultRequestHeaders.Clear();
                    cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                    cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



                    //DateTime.ParseExact(fechainicial, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    //DateTime.ParseExact(fechainicial, "dd/MM/yyyy", CultureInfo.InvariantCulture);


                    //var resp = await cliente.GetAsync(string.Format(configuration.GetSection("URIs:Informes_TraerZips").Value, idAerolinea, NombreAerolinea, TodasAerolinea, 
                    //    DateTime.ParseExact(fechafinal.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture), 
                    //    DateTime.ParseExact(fechainicial.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture)));

                    var resp = await cliente.GetAsync(string.Format(configuration.GetSection("URIs:Informes_TraerZips").Value, 
                        idAerolinea, NombreAerolinea, TodasAerolinea,
                        fechafinal.ToString("yyyy/MM/dd"), 
                        fechainicial.ToString("yyyy/MM/dd")));

                    resp.EnsureSuccessStatusCode();

                    _logger.LogInformation("\n Fecha Inicial= " + fechainicial + "\n Fecha Final= " + fechafinal + "\n Respuesta= " + JsonConvert.SerializeObject(resp) + "\n");

                    var fileStream = await resp.Content.ReadAsStreamAsync();
                    System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = "Datos.zip",
                        Inline = true
                    };
                    Response.Headers.Add("Content-Disposition", cd.ToString());
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    
                    //Response.ContentType = "application/json";
                    //Response.ContentType = "application/zip";
                    Response.ContentType = "application/octet-stream";
                    var tipo = resp.Content.Headers?.ContentType?.MediaType;
                    //_logger.LogInformation("Tipo= " + tipo + "\n   Fecha Inicial= " + fechainicial + "\n     Fecha Final= " + fechafinal+ "\n   Respuesta" + JsonConvert.SerializeObject(resp));
                    if (tipo == "application/octet-stream")
                    {
                        _logger.LogInformation("Tipo= " + tipo + "\n Fecha Inicial= " + fechainicial + "\n Fecha Final= " + fechafinal + "\n Respuesta= " + JsonConvert.SerializeObject(resp)+"\n");
                        return File(fileStream, tipo);
                    }

                    else if(tipo == "application/json")
                    {
                        _logger.LogInformation("Tipo= " + tipo + "\n Fecha Inicial= " + fechainicial + "\n Fecha Final= " + fechafinal + "\n Respuesta= " + JsonConvert.SerializeObject(resp) + "\n"+"\n TOKEN "+ token.ToString());
                        return File(fileStream, tipo);

                    }
                    else
                    {
                        _logger.LogInformation("Tipo= " + tipo + "\n Fecha Inicial= " + fechainicial + "\n Fecha Final= " + fechafinal + "\n Respuesta= " + JsonConvert.SerializeObject(resp) + "\n" + "\n TOKEN " + token.ToString());
                        ViewBag.Error = "No hay archivos generados para las fechas seleccionadas";
                        
                        InformeDescarga infdesc = new InformeDescarga();
                        infdesc.Aerolinea = Aerolinea;
                        infdesc.FechaInicial = collection["startDate"].ToString();
                        infdesc.FechaFinal = collection["endDate"].ToString();
                        string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
                        IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
                        
                        _logger.LogInformation("Tipo= " + tipo + "\n Fecha Inicial= " + fechainicial + "\n Fecha Final= " + fechafinal + "\n Respuesta= " + JsonConvert.SerializeObject(resp) + "\n" + "\n TOKEN " + token.ToString());

                        ViewData["Aerolineas"] = respuesta;
                        return View(infdesc);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                ViewBag.Error = ex.Message;
                InformeDescarga infdesc = new InformeDescarga();
                infdesc.Aerolinea = Aerolinea;
                infdesc.FechaInicial = collection["startDate"].ToString();
                infdesc.FechaFinal = collection["endDate"].ToString();
                string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
                IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);

                ViewData["Aerolineas"] = respuesta;
                return View(infdesc);
            }



            //DateTime fechainicial = Convert.ToDateTime(collection["startDate"].ToString());
            //DateTime fechafinal = Convert.ToDateTime(collection["endDate"].ToString());
            //string Aerolinea = collection["aerolinea"].ToString();
            //string rutaArchivos = configuration.GetSection("Config:RutaArchivos").Value;
            //// Busco los archivos en esa ruta de esa aerolinea y fecha
            //rutaArchivos = rutaArchivos + "//" + Aerolinea.Trim();
            //if (!Directory.Exists(rutaArchivos))
            //{
            //    ViewBag.Error = "No hay archivos generados para las fechas seleccionadas";
            //    InformeDescarga infdesc = new InformeDescarga();
            //    infdesc.Aerolinea = Aerolinea;
            //    infdesc.FechaInicial = collection["startDate"].ToString();
            //    infdesc.FechaFinal = collection["endDate"].ToString();
            //    string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            //    IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);

            //    ViewData["Aerolineas"] = respuesta;
            //    return View(infdesc);
            //}
            //else
            //{
            //    // Busco los archivos de esas fechas saco dias entre esas fechas
            //    TimeSpan difFechas = fechafinal - fechainicial;
            //    int dias = difFechas.Days;
            //    DateTime limiteinicial = fechainicial;
            //    string rutaTemp = configuration.GetSection("Config:RutaArchivos").Value;
            //    rutaTemp = rutaTemp + "//Temp";
            //    if (!Directory.Exists(rutaTemp))
            //    {
            //        Directory.CreateDirectory(rutaTemp);
            //    }
            //    else
            //    {
            //        Directory.Delete(rutaTemp, true);
            //    }
            //    int existe = 0;
            //    for (int i = 0; i < dias; i++)
            //    {
            //        string rutaparcial = rutaArchivos;
            //        string rutaAerolineaFecha = String.Format("{0}{1}{2}", limiteinicial.Year, limiteinicial.Month.ToString().PadLeft(2, '0'), limiteinicial.Day.ToString().PadLeft(2, '0'));
            //        rutaparcial = rutaparcial + "//" + rutaAerolineaFecha;
            //        if (Directory.Exists(rutaparcial))
            //        {
            //            existe = 1;
            //            if (!Directory.Exists(rutaTemp + "//" + rutaAerolineaFecha))
            //            {
            //                Directory.CreateDirectory(rutaTemp + "//" + rutaAerolineaFecha);
            //            }
            //            DirectoryCopy(rutaparcial, rutaTemp + "//" + rutaAerolineaFecha, true);
            //            // Añadase la carpeta a la temporal
            //        }
            //        limiteinicial = limiteinicial.AddDays(1);
            //    }

            //    // La carpeta temporal se zipea y luego se elimina
            //    string startPath = rutaTemp;
            //    string zipPath = rutaArchivos + "DatosCargados.zip";
            //    if (System.IO.File.Exists(zipPath))
            //    {
            //        System.IO.File.Delete(zipPath);
            //    }

            //    if (existe== 0)
            //    {
            //        ViewBag.Error = "No hay archivos generados para las fechas seleccionadas";
            //        InformeDescarga infdesc = new InformeDescarga();
            //        infdesc.Aerolinea = Aerolinea;
            //        infdesc.FechaInicial = collection["startDate"].ToString();
            //        infdesc.FechaFinal = collection["endDate"].ToString();
            //        string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            //        IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);
            //        if (!Directory.Exists(startPath))
            //        {
            //            Directory.Delete(startPath, true);
            //        }
            //        ViewData["Aerolineas"] = respuesta;
            //        return View(infdesc);
            //    }
            //    else
            //    {
            //        ZipFile.CreateFromDirectory(startPath, zipPath);
            //        Directory.Delete(startPath, true);
            //        // Descarga el archivo en el explorador:)
            //        byte[] fileBytes = System.IO.File.ReadAllBytes(zipPath);
            //        string fileName = "DatosCargados.zip";
            //        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            //    }



            //}


        }

        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        #endregion

        #region "Descargar Excel"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportarExcel(string DatosConsulta)
        {
            string pasos = "0";
            try
            {
                
                string[] Datos = DatosConsulta.Split(";");
                RutasRelativas RutasGenericas = new RutasRelativas(configuration);
                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                await ServicioOracle.AddToken<string>(token);
                
                 switch (Datos[0])
                {
                    case "InformeCobro":
                        List<InformeCobro> listaInfoCobro = new List<InformeCobro>();
                        string rutaRelativaCobro = RutasGenericas.GenerarRutaRelativas(DatosConsulta, "InformeCobro");
                        DataTable respuestaCobro = await servicioApi.GetAsync<DataTable>(rutaRelativaCobro);
                        if (respuestaCobro.Rows.Count > 1)
                        {
                            foreach (DataRow itemCobro in respuestaCobro.Rows)
                            {
                                try
                                {
                                    InformeCobro InfoCobro = new InformeCobro();
                                    InfoCobro.lstNovedades = new List<string>();
                                    // traigo las novedades
                                    string rutaRelativaNoves = configuration.GetSection("URIs:ObtenerNovedadesxOperacion").Value;
                                    IList<NovedadOtd> respuesta = await servicioApi.GetAsync<IList<NovedadOtd>>(string.Format(rutaRelativaNoves, itemCobro["Id"].ToString()));

                                    foreach (var item in respuesta)
                                    {
                                        InfoCobro.lstNovedades.Add(item.DescCausal);
                                    }

                                    InfoCobro.Concepto = "";
                                    InfoCobro.DiferenciaTasas = itemCobro["DiferenciaTasas"].ToString();
                                    InfoCobro.exentos = Convert.ToInt32(itemCobro["exentos"].ToString());
                                    InfoCobro.fechaCargue = Convert.ToDateTime((itemCobro["fechaCargue"].ToString() == string.Empty) ? DateTime.MinValue : itemCobro["fechaCargue"]);
                                    InfoCobro.FechaSalida = Convert.ToDateTime(itemCobro["FechaSalida"]);
                                    InfoCobro.HoraCargue = itemCobro["HoraCargue"].ToString();
                                    InfoCobro.Infantes = Convert.ToInt32(itemCobro["Infantes"].ToString());
                                    InfoCobro.Linea = Convert.ToInt32(itemCobro["Linea"].ToString());
                                    InfoCobro.Matricula = itemCobro["Matricula"].ToString();
                                    InfoCobro.NumeroVuelo = itemCobro["NumeroVuelo"].ToString();
                                    InfoCobro.Oaci = itemCobro["Oaci"].ToString();
                                    InfoCobro.Pasajeros = Convert.ToInt32(itemCobro["Pasajeros"].ToString());
                                    InfoCobro.TasasCobradas = itemCobro["TasasCobradas"].ToString();
                                    InfoCobro.TasasReportadas = itemCobro["TasasReportadas"].ToString();
                                    InfoCobro.TipoVuelo = itemCobro["TipoVuelo"].ToString();
                                    InfoCobro.TransitoConexion = Convert.ToInt32(itemCobro["TransitoConexion"].ToString());
                                    InfoCobro.tripulantes = Convert.ToInt32(itemCobro["tripulantes"].ToString());
                                    InfoCobro.Usuario = itemCobro["Usuario"].ToString();
                                    listaInfoCobro.Add(InfoCobro);
                                }
                                catch (Exception ex)
                                {
                                    Console.Write(ex.StackTrace.ToString());
                                    throw;
                                }
                            }
                        }

                        InformeInfoCobro objInformecobro = new InformeInfoCobro();
                        string filtro1cobro = "";
                        string filtro2cobro = "";
                        if (Datos[3] != "")
                        {
                            filtro1cobro = "Fecha Desde: " + Datos[3] + "   Fecha hasta: " + Datos[4];
                        }
                         
                        byte[] AnexoCobro = objInformecobro.ArmarExcel(listaInfoCobro, filtro1cobro, filtro2cobro);
                        pasos = pasos + "3";
                        return File(AnexoCobro.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Informe de cobro - Jarvis.xlsx");

                    case "PuenteAbordaje":
                        pasos = pasos + "1";
                        List<Anexo1> Anexos1 = await ServicioOracle.GetAsync<List<Anexo1>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "PuenteAbordaje"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("PuenteAbordaje", new List<Anexo1>());
                        }
                        InformePuenteAbordaje objInformeAnexo1 = new InformePuenteAbordaje();
                        string filtro1 = "";
                        string filtro2 = "";
                        if (Datos[3] != "")
                        {
                            filtro1= "Factura Desde: " + Datos[3] + "   Factura hasta: " + Datos[4];
                        }
                        if (Datos[5] != "")
                        {
                            filtro2 = "Fecha inicial: " + Datos[7] + "   Fecha final: " + Datos[8];
                        }
                        byte[] Anexo1 = objInformeAnexo1.ArmarExcel(Anexos1, Datos[2], filtro1,filtro2);
                        pasos = pasos + "3";
                        return File(Anexo1.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Puente Abordaje - Jarvis.xlsx");
                    case "ParqueoAeronaves":
                        List<Anexo2> Anexos2 = await ServicioOracle.GetAsync<List<Anexo2>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "ParqueoAeronaves"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            ViewBag.Error = "Error Generar Excel";
                            return View("ParqueoAeronaves", new List<Anexo2>());
                        }
                        InformeParqueAeronaves objInformeAnexo2 = new InformeParqueAeronaves();
                        string filtroA2 = "";
                        string filtroA21 = "";
                        if (Datos[4] != "")
                        {
                            filtroA2 = "Factura Desde: " + Datos[4] + "   Factura hasta: " + Datos[5];
                        }
                        if (Datos[2] != "")
                        {
                            filtroA21 = "Fecha inicial: " + Datos[7] + "   Fecha final: " + Datos[8];
                        }
                        byte[] Anexo2 = objInformeAnexo2.ArmarExcel(Anexos2, Datos[6], filtroA2, filtroA21);
                        return File(Anexo2.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Parqueo Aeronaves - Jarvis.xlsx");
                    case "TasasAeroportuariasCOP":
                        List<Anexo3> Anexos3 = await ServicioOracle.GetAsync<List<Anexo3>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "TasasAeroportuariasCOP"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            ViewBag.Error = "Error Generar Excel";
                            return View("ParqueoAeronaves", new List<Anexo3>());
                        }
                        InformeasTasAeroportuariasCOP objInformeAnexo3 = new InformeasTasAeroportuariasCOP();
                        string filtroA3 = "";
                        string filtroA31 = "";
                        if (Datos[4] != "")
                        {
                            filtroA3 = "Factura Desde: " + Datos[4] + "   Factura hasta: " + Datos[5];
                        }
                        if (Datos[2] != "")
                        {
                            filtroA31 = "Fecha inicial: " + Datos[7] + "   Fecha final: " + Datos[8];
                        }
                        byte[] Anexo3 = objInformeAnexo3.ArmarExcel(Anexos3, Datos[6], filtroA3, filtroA31);
                        return File(Anexo3.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tasas Aeroportuarias COP - Jarvis.xlsx");
                    case "TasasAeroportuariasUSD":
                        List<Anexo4> Anexos4 = await ServicioOracle.GetAsync<List<Anexo4>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "TasasAeroportuariasUSD"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            ViewBag.Error = "Error Generar Excel";
                            return View("ParqueoAeronaves", new List<Anexo4>());
                        }
                        InformeasTasAeroportuariasUSD objInformeAnexo4 = new InformeasTasAeroportuariasUSD();
                        string filtroA4 = "";
                        string filtroA41 = "";
                        if (Datos[4] != "")
                        {
                            filtroA4 = "Factura Desde: " + Datos[4] + "   Factura hasta: " + Datos[5];
                        }
                        if (Datos[2] != "")
                        {
                            filtroA41 = "Fecha inicial: " + Datos[7] + "   Fecha final: " + Datos[8];
                        }
                        byte[] Anexo4 = objInformeAnexo4.ArmarExcel(Anexos4, Datos[6], filtroA4, filtroA41);
                        return File(Anexo4.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tasas Aeroportuarias USD - Jarvis.xlsx");
                    case "MostradoresMostradores":
                        List<Anexo5> Anexos5 = await ServicioOracle.GetAsync<List<Anexo5>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "MostradoresMostradores"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            ViewBag.Error = "Error Generar Excel";
                            return View("ParqueoAeronaves", new List<Anexo5>());
                        }
                        InformeasMostradores objInformeAnexo5 = new InformeasMostradores();
                        string filtroA5 = "";
                        string filtroA51 = "";
                        if (Datos[4] != "")
                        {
                            filtroA5 = "Factura Desde: " + Datos[4] + "   Factura hasta: " + Datos[5];
                        }
                        if (Datos[2] != "")
                        {
                            filtroA51 = "Fecha inicial: " + Datos[6] + "   Fecha final: " + Datos[7];
                        }
                        
                        byte[] Anexo5 = objInformeAnexo5.ArmarExcel(Anexos5, Datos[5], filtroA5, filtroA51);
                        return File(Anexo5.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Mostradores - Jarvis.xlsx");
                    case "DetallesVuelosInterventoria":
                        pasos = pasos + "1";
                        List<Anexo6> Anexos6 = await ServicioOracle.GetAsync<List<Anexo6>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "DetallesVuelosInterventoria"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("DetallesVuelosInterventoria", new List<Anexo6>());
                        }
                        string filtroA6 = "";
                        string filtroA61 = "";
                        if (Datos[4] != "")
                        {
                            filtroA6 = "Estado " + Datos[4];
                        }
                        if (Datos[6] != "")
                        {
                            filtroA61 = "Fecha inicial: " + Datos[6] + "   Fecha final: " + Datos[7];
                        }
                        InformeDetallesVuelosInterventoria objInformeAnexo6 = new InformeDetallesVuelosInterventoria();
                        byte[] Anexo6 = objInformeAnexo6.ArmarExcel(Anexos6, Datos[3],  filtroA61, "");
                        pasos = pasos + "3";
                        return File(Anexo6.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Detalles Vuelos Interventoria - Jarvis.xlsx");

                    case "UsoCarroBomberos":
                        pasos = pasos + "1";
                        List<Anexo8> Anexos8 = await ServicioOracle.GetAsync<List<Anexo8>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "UsoCarroBomberos"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("UsoCarroBomberos", new List<Anexo8>());
                        }
                        string filtro8 = "";
                        string filtro8A = "";
                        if (Datos[4] != "")
                        {
                            filtro8 = "Factura inicial: " + Datos[3] + "   Factura final: " + Datos[4];
                        }
                        if (Datos[1] != "")
                        {
                            filtro8A = "Fecha inicial: " + Datos[6] + "   Fecha final: " + Datos[7];
                        }
                        InformeUsoCarroBomberos objInformeAnexo8 = new InformeUsoCarroBomberos();
                        byte[] Anexo8 = objInformeAnexo8.ArmarExcel(Anexos8, Datos[2], filtro8, filtro8A);
                        pasos = pasos + "3";
                        return File(Anexo8.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Uso carro bomberos- Jarvis.xlsx");
                    case "ResumenTasasAeroportuariasFacturadas":
                        pasos = pasos + "1";
                        List<Anexo10> Anexos10 = await ServicioOracle.GetAsync<List<Anexo10>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "ResumenTasasAeroportuariasFacturadas"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("ResumenTasasAeroportuariasFacturadas", new List<Anexo10>());
                        }
                        string filtro10 = "";
                        string filtro10A = "";
                        
                        if (Datos[5] != "")
                        {
                            filtro10A = "Fecha inicial: " + Datos[5] + "   Fecha final: " + Datos[6];
                        }
                        InformeResumenTasasAeroportuariasFacturadas objInformeAnexo10 = new InformeResumenTasasAeroportuariasFacturadas();
                        byte[] Anexo10 = objInformeAnexo10.ArmarExcel(Anexos10, filtro10, filtro10A);
                        pasos = pasos + "3";
                        return File(Anexo10.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ResumenTasasAeroportuariasFacturadas - Jarvis.xlsx");
                    case "SoportesExenciones":
                        pasos = pasos + "1";
                        List<Anexo12> Anexos12 = await ServicioOracle.GetAsync<List<Anexo12>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "SoportesExenciones"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("SoportesExenciones", new List<Anexo10>());
                        }
                        string filtro12 = "";
                        string filtro12A = "";
                         
                        if (Datos[4] != "")
                        {
                            filtro12A = "Fecha inicial: " + Datos[4] + "   Fecha final: " + Datos[5];
                        }
                        InformeSoportesExenciones objInformeAnexo12 = new InformeSoportesExenciones();
                        byte[] Anexo12 = objInformeAnexo12.ArmarExcel(Anexos12, filtro12, filtro12A);
                        pasos = pasos + "3";
                        return File(Anexo12.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SoportesExenciones - Jarvis.xlsx");
                    case "FacturacionParqueosAmplicacionLA33":
                        pasos = pasos + "1";
                        List<Anexo13> Anexos13 = await ServicioOracle.GetAsync<List<Anexo13>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "FacturacionParqueosAmplicacionLA33"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("FacturacionParqueosAmplicacionLA33", new List<Anexo13>());
                        }
                        InformeParqueosAmplicacionLA33 objInformeAnexo13 = new InformeParqueosAmplicacionLA33();
                        string filtro13 = "";
                        string filtro13A = "";

                        if (Datos[1] != "")
                        {
                            filtro13A = "Factura inicial: " + Datos[1] + "   Factura final: " + Datos[2];
                        }
                        byte[] Anexo13 = objInformeAnexo13.ArmarExcel(Anexos13, Datos[3], filtro13, filtro13A);
                        pasos = pasos + "3";
                        return File(Anexo13.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FacturacionParqueosAmplicacionLA33 - Jarvis.xlsx");
                    case "FacturacionParqueosAmplicacionLA33100":
                        pasos = pasos + "1";
                        List<Anexo14> Anexos14 = await ServicioOracle.GetAsync<List<Anexo14>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "FacturacionParqueosAmplicacionLA33100"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("FacturacionParqueosAmplicacionLA33100", new List<Anexo13>());
                        }
                        string filtro14 = "";
                        string filtro14A = "";
                        if (Datos[1] != "")
                        {
                            filtro14A = "Factura inicial: " + Datos[2] + "   Factura final: " + Datos[3];
                        }
                        InformeParqueosAmplicacionLA33100 objInformeAnexo14 = new InformeParqueosAmplicacionLA33100();
                        byte[] Anexo14 = objInformeAnexo14.ArmarExcel(Anexos14, Datos[4], filtro14, filtro14A);
                        pasos = pasos + "3";
                        return File(Anexo14.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Facturacion Parqueos Amplicacion LA33 100% - Jarvis.xlsx");
                    case "GPU":
                        pasos = pasos + "1";
                        List<Anexo16> Anexos16 = await ServicioOracle.GetAsync<List<Anexo16>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "GPU"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("GPU", new List<Anexo16>());
                        }
                        string filtro16 = "";
                        string filtro16A = "";
                        if (Datos[4] != "")
                        {
                            filtro16 = "Factura inicial: " + Datos[5] + "   Factura final: " + Datos[6];
                        }
                        if (Datos[1] != "")
                        {
                            filtro16A = "Fecha inicial: " + Datos[7] + "   Fecha final: " + Datos[8];
                        }
                        InformeGPU objInformeAnexo16 = new InformeGPU();
                        byte[] Anexo16 = objInformeAnexo16.ArmarExcel(Anexos16, Datos[3], filtro16, filtro16A);
                        pasos = pasos + "3";
                        return File(Anexo16.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GPU - Jarvis.xlsx");
                    case "ResumenDetalleTasasAeroportuarias":
                        pasos = pasos + "1";
                        List<Anexo7B> Anexos7B = await ServicioOracle.GetAsync<List<Anexo7B>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "ResumenDetalleTasasAeroportuarias"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("ResumenDetalleTasasAeroportuarias", new List<Anexo7B>());
                        }
                        InformeResumenDetalleTasasAeroportuarias objInformeAnexo7B = new InformeResumenDetalleTasasAeroportuarias();
                        string filtro7B = "";
                        string filtro7B2 = "Vuelos " + Datos[3];
                        
                        if (Datos[1] != "")
                        {
                            filtro7B = "Fecha inicial: " + Datos[5] + "   Fecha final: " + Datos[6];
                        }
                        byte[] Anexo7B = objInformeAnexo7B.ArmarExcel(Anexos7B, Datos[3], filtro7B, filtro7B2);
                        pasos = pasos + "3";
                        return File(Anexo7B.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ResumenDetalleTasasAeroportuarias - Jarvis.xlsx");
                    case "GPUAerocivil":
                        pasos = pasos + "1";
                        List<Anexo17> Anexos17 = await ServicioOracle.GetAsync<List<Anexo17>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "GPUAerocivil"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("GPUAerocivil", new List<Anexo17>());
                        }
                        InformeGPUAerocivil objInformeAnexo17 = new InformeGPUAerocivil();
                        string filtro17 = "";
                        string filtro172 = "";
                        if (Datos[5] != "")
                        {
                            filtro17 = "Factura Desde: " + Datos[5] + "   Factura hasta: " + Datos[6];
                        }
                        if (Datos[5] != "" && Datos[5] != null)
                        {
                            filtro172 = "Fecha inicial: " + Datos[7] + "   Fecha final: " + Datos[8];
                        }
                        byte[] Anexo17 = objInformeAnexo17.ArmarExcel(Anexos17, Datos[3], filtro17, filtro172);
                        pasos = pasos + "3";
                        return File(Anexo17.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GPU Aerocivil - Jarvis.xlsx");

                    case "TasasAeroportuariasFacturadasporAerolinea":
                        pasos = pasos + "1";
                        List<Anexo11> Anexos11 = await ServicioOracle.GetAsync<List<Anexo11>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "TasasAeroportuariasFacturadasporAerolinea"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("TasasAeroportuariasFacturadasporAerolinea", new List<Anexo11>());
                        }
                        InformeTasasAeroportuariasFacturadasporAerolinea objInformeAnexo11 = new InformeTasasAeroportuariasFacturadasporAerolinea();
                     
                        string filtro11B = "";
                       
                        if (Datos[4] != "")
                        {
                            filtro11B = "Fecha inicial: " + Datos[4] + "   Fecha final: " + Datos[5];
                        }
                        byte[] Anexo11 = objInformeAnexo11.ArmarExcel(Anexos11, Datos[3], "", filtro11B);
                        pasos = pasos + "3";
                        return File(Anexo11.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tasas Aeroportuarias Facturadas por Aerolinea - Jarvis.xlsx");
                    case "InformeCarnetizacion":
                        pasos = pasos + "1";
                        
                        List<Anexo19> Anexos19 = await ServicioOracle.GetAsync<List<Anexo19>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "InformeCarnetizacion"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("InformeCarnetizacion", new List<Anexo19>());
                        }
                        InformeInformeCarnetizacion objInformeAnexo19 = new InformeInformeCarnetizacion();
                        string filtro19 = ""; 
                         
                        if (Datos[1] != "")
                        {
                            filtro19 = "Fecha inicial: " + Datos[1] + "   Fecha final: " + Datos[2];
                        }
                        byte[] Anexo19 = objInformeAnexo19.ArmarExcel(Anexos19, filtro19, "");
                        pasos = pasos + "3";
                        return File(Anexo19.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Informe Carnetizacion - Jarvis.xlsx");
                    case "DetallesVuelosInterventoriaInfrasa":
                        pasos = pasos + "1";
                        List<Anexo6> Anexos6I = await ServicioOracle.GetAsync<List<Anexo6>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "DetallesVuelosInterventoriaInfrasa"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("DetallesVuelosInterventoriaInfrasa", new List<Anexo6>());
                        }
                        InformeDetallesVuelosInterventoriaInfrasa objInformeAnexo6I = new InformeDetallesVuelosInterventoriaInfrasa();
                        string filtro9 = "";
                        string filtro9A = "";
                        if (Datos[4] != "")
                        {
                            if (Datos[4]== "08")
                            {
                                filtro9 = "Tipo de Vuelo: " + Datos[3] + " Estado: FACTURADO" ;
                            }
                            if (Datos[4] == "06")
                            {
                                filtro9 = "Tipo de Vuelo: " + Datos[3] + " Estado: AUDITADO";
                            }
                            if (Datos[4] == "02")
                            {
                                filtro9 = "Tipo de Vuelo: " + Datos[3] + " Estado: PRE-ANALIZADO";
                            }

                        }
                        if (Datos[1] != "")
                        {
                            filtro9A = "Fecha inicial: " + Datos[5] + "   Fecha final: " + Datos[6];
                        }
                        byte[] Anexo6I = objInformeAnexo6I.ArmarExcel(Anexos6I, Datos[3], filtro9, filtro9A);
                        pasos = pasos + "3";
                        return File(Anexo6I.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Infrasa- Jarvis.xlsx");
                    case "FacturacionAmplicacionLA33Puentes":
                        pasos = pasos + "1";
                        List<Anexo15> Anexos16I = await ServicioOracle.GetAsync<List<Anexo15>>(RutasGenericas.GenerarRutaRelativas(DatosConsulta, "FacturacionAmplicacionLA33Puentes"));
                        if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                        {
                            pasos = pasos + "2";
                            ViewBag.Error = "Error Generar Excel" + pasos;
                            return View("FacturacionAmplicacionLA33Puentes", new List<Anexo15>());
                        }
                        InformeFacturacionAmplicacionLA33Puentes objInformeAnexo15 = new InformeFacturacionAmplicacionLA33Puentes();
                        string filtro15 = "";
                        
                        
                        if (Datos[2] != "")
                        {
                            filtro15 = "Factura desde: " + Datos[2] + "   Factura hasta: " + Datos[3];
                        }
                        byte[] Anexosnuevo = objInformeAnexo15.ArmarExcel(Anexos16I,  filtro15, "");
                        
                        return File(Anexosnuevo.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Facturacion ampliacion La33 puentes.xlsx");

                    case "InformeTickets":
                        pasos = pasos + "1";
                        string rutaRelativainftickets = RutasGenericas.GenerarRutaRelativas(DatosConsulta, "InformeTickets");
                        DataTable respuestaticket = await servicioApi.GetAsync<DataTable>(rutaRelativainftickets);
                        List<InformeTickets> listaInfotickes = new List<InformeTickets>();
                        if (respuestaticket.Rows.Count >= 1)
                        {
                            foreach (DataRow itemCobro in respuestaticket.Rows)
                            {
                                try
                                {
                                    InformeTickets Infotickes = new InformeTickets();

                                    Infotickes.Aerolinea = itemCobro["Aerolinea"].ToString();
                                    Infotickes.Adjunto = itemCobro["Adjunto"].ToString();
                                    Infotickes.AdjuntoRespuesta = itemCobro["AdjuntoRespuesta"].ToString();
                                    Infotickes.FechaSolicitud = Convert.ToDateTime((itemCobro["FechaSolicitud"].ToString() == string.Empty) ? DateTime.MinValue : itemCobro["FechaSolicitud"]);
                                    Infotickes.FechaRespuesta = Convert.ToDateTime((itemCobro["FechaRespuesta"].ToString() == string.Empty) ? DateTime.MinValue : itemCobro["FechaSolicitud"]);
                                    Infotickes.Asunto = itemCobro["Asunto"].ToString();
                                    Infotickes.Depende = itemCobro["Depende"].ToString();
                                    Infotickes.EstadoFinal = itemCobro["EstadoFinal"].ToString();
                                    Infotickes.NumeroTicket = itemCobro["NumeroTicket"].ToString();
                                    Infotickes.Respuesta = itemCobro["Respuesta"].ToString();
                                    Infotickes.TipoTicket = itemCobro["TipoTicket"].ToString();
                                    Infotickes.Usuario = itemCobro["Usuario"].ToString();
                                     Infotickes.UsuarioRespuesta = itemCobro["UsuarioRespuesta"].ToString();
                                    listaInfotickes.Add(Infotickes);
                                }
                                catch (Exception ex)
                                {
                                    Console.Write(ex.StackTrace.ToString());
                                    throw;
                                }

                            }
                        }

                        string filtroticket = "";
                       
                        if (Datos[1] != "")
                        {
                            filtroticket = "Fecha inicial: " + Datos[1] + "   Fecha final: " + Datos[2];
                        }
                        InformeTicketsExcel listaInfotickesexcel = new InformeTicketsExcel();
                        byte[] listaInfotickes2 = listaInfotickesexcel.ArmarExcel(listaInfotickes, filtroticket);
                        pasos = pasos + "3";
                        return File(listaInfotickes2.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Tickets- Jarvis.xlsx");
                    default:
                        pasos = pasos + "4";
                        ViewBag.Error = "No obtuvo  nada" + pasos;
                        return View("ResumenTasasAeroportuariasFacturadas", new List<Anexo10>());
                        
                }
            }
           
            catch (Exception ex)
            {
                pasos = pasos + "5";
                ViewBag.Error = ex.Message.ToString() + pasos;
                return View("PuenteAbordaje",new List<Anexo1>());
            }
        }

    

        public string FechaJulianAGregor(string Julian)
        {
            int jDate = Convert.ToInt32(Julian);
            int day = jDate % 1000;
            int year = (jDate - day) / 1000;
            year = (year >= 17) ? year += 1900 : year += 2000;
            var tempDate = new DateTime(year, 1, day);
            tempDate.AddDays(day - 1);

            string s = tempDate.ToString("dd/MM/yyyy");
            return s;
        }


        #endregion

    }
}