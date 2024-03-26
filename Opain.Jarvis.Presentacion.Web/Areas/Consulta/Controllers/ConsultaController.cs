using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Net.Http;
using System.Net;
using System.IO.Compression;
using ClosedXML.Excel;
using Opain.Jarvis.Presentacion.Web.Areas.CargaInformacion.Controllers;
using Microsoft.Extensions.Logging;
//using Microsoft.AspNetCore.Http;
//using System.Web;

namespace Opain.Jarvis.Presentacion.Web.Areas.Consulta.Controllers
{
    [Area("Consulta")]
    public class ConsultaController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IServicioApi servicioApi;
        private static List<OperacionVueloOtd> operacionVueloOtd = new List<OperacionVueloOtd>();
        private readonly ILogger<VuelosController> _logger;
        //private readonly Servicios.Store.OperacionesVuelo Store_OperacionesVuelo;

        public ConsultaController(IConfiguration cfg, IServicioApi api, ILogger<VuelosController> logger)
        {
            configuration = cfg;
            servicioApi = api;
            _logger = logger;
        }


        public async Task<IActionResult> Principal()
        {

            int DiasConsulta =int.Parse( configuration.GetSection("Config:DiasConsulta").Value);



            string idAerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value;
            int idAerolineaPorCodigo = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type.Equals("CodigoAeroJDE")).Value);
            string fechaInicio = Uri.EscapeDataString(DateTime.Now.AddDays(DiasConsulta).ToString("yyyy-MM-dd"));
            string fechaFinal = Uri.EscapeDataString(DateTime.Now.ToString("yyyy-MM-dd"));
            var _ListaNovedadesAgrupadas =await NovedadesAgrupadas(idAerolinea, fechaInicio, fechaFinal);




            /////////////////////////////// 

            List<ConsultaOtd> consultas = new List<ConsultaOtd>();


            if (_ListaNovedadesAgrupadas != null)
            {
                foreach (var item in _ListaNovedadesAgrupadas)
                {
                    consultas.Add(new ConsultaOtd
                    {
                        Fecha = item.FechaVuelo,
                        CantidadVuelos = item.CantidadVuelos,
                        NovedadesCargue = item.NovedadesCargue,
                        NovedadesProceso = item.NovedadesProcesos,
                        Exitoso = item.Exitoso ? 1 : 0,
                        Procesados = item.Procesados,
                        Finalizados = item.Procesados
                    });
                }
            }


            //var vuelosTotales = await ObtenerTodosAsync("", "", "CON");
            //var vuelos = vuelosTotales.Where(p => p.EstadoProceso != "2" && p.EstadoProceso != "0" && (p.Id_Daily != null || p.Id_Daily != "0")).ToList();


            //foreach (var item in vuelos)
            //{
            //    string rutaRelativa = configuration.GetSection("URIs:ObtenerNovedadesxOperacion").Value;
            //    IList<NovedadOtd> respuesta = await servicioApi.GetAsync<IList<NovedadOtd>>(string.Format(rutaRelativa, item.Id));
            //    if (respuesta.Count(str => str.TipoValidacion == 1) > 0)
            //    {
            //        item.NovedadCargue = 1;

            //    }
            //    if (respuesta.Count(str => str.TipoValidacion == 0) > 0)
            //    {
            //        item.NovedadProceso = 1;

            //    }

            //}


            //var grupo = vuelos.GroupBy(x => x.Fecha);

            //foreach (var item in grupo)
            //{
            //    ConsultaOtd consulta = new ConsultaOtd();
            //    consulta.Fecha = item.Key;
            //    consulta.CantidadVuelos = item.Count();
            //    consulta.NovedadesCargue = vuelos.Where(x => x.Fecha.Equals(item.Key)).Sum(x => x.NovedadCargue);
            //    consulta.NovedadesProceso = vuelos.Where(x => x.Fecha.Equals(item.Key)).Sum(x => x.NovedadProceso);
            //    consulta.Exitoso = vuelos.Where(x => x.Fecha.Equals(item.Key) && x.NovedadCargue.Equals(0) && x.NovedadProceso.Equals(0) && x.ConfirmacionOperacion.Equals(1)).Count();
            //    consulta.Procesados = vuelos.Where(x => x.Fecha.Equals(item.Key) && x.ConfirmacionOperacion.Equals(1)).Count();
            //    consulta.Finalizados = vuelos.Where(x => x.Fecha.Equals(item.Key) && x.EstadoProceso.Equals("5")).Count();

            //    consultas.Add(consulta);
            //}

            return View(consultas);
        }




        /*[Authorize(Roles = "AEROLINEA,SUPERVISOR")]*/
        [HttpPost]
        public async Task<IActionResult> EnviarEmail(List<string> chkAprobar)
        {
            try
            {
                /*
                List<PasajeroTransitoOtd> listaPasajeroTransitoOtd = new List<PasajeroTransitoOtd>();

                if (chkAprobar.Count > 0)
                {
                    foreach (var item in chkAprobar)
                    {
                        string[] datoss = item.Split(',');
                        string id = datoss[0];
                        string fecha = datoss[1];

                        string rutaRelativa = string.Format(configuration.GetSection("URIs:TransitoConexionFirmar").Value, id);
                        PasajeroTransitoOtd respuesta = await servicioApi.GetAsync<PasajeroTransitoOtd>(rutaRelativa).ConfigureAwait(false);
                        respuesta.Firmado = 1;

                        string rutaRelativaActualizar = configuration.GetSection("URIs:TransitoConexionFirmar2").Value;
                        await servicioApi.PostAsync<bool>(rutaRelativaActualizar, respuesta);
                        // Actualizo el estado de vuelo si todos los transitos en conexión de ese vuelo ya fueron firmados
                        int IdOperacionVuelo = respuesta.Operacion;
                        
                        string rutaRelativa2 = string.Format(configuration.GetSection("URIs:ActualizarVuelosFirmados").Value, IdOperacionVuelo);
                        await servicioApi.PostAsync<int>(rutaRelativa2, IdOperacionVuelo);
                        
                        PasajeroTransitoOtd transito = await ObtenerTransito(Convert.ToInt32(id));
                        
                        //Notificacion 6
                        // await this.EnviarCorreos(idAreolinea, respuesta.Fecha);

                         listaPasajeroTransitoOtd.Add(transito);

                    }

                    if (listaPasajeroTransitoOtd != null && listaPasajeroTransitoOtd.ToString() != "" && listaPasajeroTransitoOtd.Count >= 1)
                    {
                        //await this.EnviarCorreos(idAreolinea, respuesta.Fecha);
                        //listaPasajeroTransitoOtd[0].Operacion.
                    }



                    }
                */
                    //Se debe dispara la notificacion 
                    return RedirectToAction("Principal");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                throw;
            }
        }

        public async Task<IActionResult> ConsultarVuelos(string fecha, string id = "")
        {

            var vuelos = await ObtenerTodosAsync(fecha, fecha, "CON1");
                
            DateTime fechaFiltro = new DateTime(int.Parse(fecha.Substring(0, 4)), int.Parse(fecha.Substring(5, 2)), int.Parse(fecha.Substring(8, 2)));

            var vuelosFiltro = vuelos.Where(x => x.Fecha.Equals(fechaFiltro) && x.EstadoProceso != "2" && x.EstadoProceso != "0");
            // consulto las novedades de los vuelos que salen ahi
            //List<OperacionVueloOtd> listaovuelosFiltro = new List<OperacionVueloOtd>();
            //var listaoperacionVueloOtd = vuelosFiltro.GroupBy(x => x.IdVuelo).Select(x => x.First()).ToList();
            //operacionVueloOtd = null;
            //operacionVueloOtd.Clear();
            //operacionVueloOtd.RemoveAll();
            //for (int i = listaoperacionVueloOtd.Count - 1; i >= 0; i--)
            //{
            //    listaoperacionVueloOtd.RemoveAt(i);
            //}
            if (operacionVueloOtd != null)
            {
                for (int i = operacionVueloOtd.Count - 1; i >= 0; i--)
                {
                    operacionVueloOtd.RemoveAt(i);
                }
            }

            foreach (var item in vuelosFiltro)
            {
                string rutaRelativa = configuration.GetSection("URIs:ObtenerNovedadesxOperacion").Value;
                IList<NovedadOtd> respuesta = await servicioApi.GetAsync<IList<NovedadOtd>>(string.Format(rutaRelativa, item.Id));
                item.NovedadCargue = respuesta.Count(str => str.TipoValidacion == 1);
                item.NovedadProceso = respuesta.Count(str => str.TipoValidacion == 0);
                operacionVueloOtd.Add(item);
            }

            return PartialView(vuelosFiltro);
        }
        [HttpGet]
        public async Task<bool> emailExiste(string email)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:emailExiste").Value, email);
            bool respuesta = await servicioApi.GetAsync<bool>(rutaRelativa).ConfigureAwait(false); ;

            return respuesta;
        }

        [HttpPost]
        public async Task<IList<NovedadesAgrupadasOtd>> NovedadesAgrupadas(string idAerolinea, string fechaInicio, string fechaFinal)
        {
            string _rutaRelativa = configuration.GetSection("URIs:ObtenerNovedadesAgrupadas").Value;
            IList<NovedadesAgrupadasOtd> _respuesta = await servicioApi.GetAsync<IList<NovedadesAgrupadasOtd>>(string.Format(_rutaRelativa, idAerolinea, fechaInicio, fechaFinal));
            return _respuesta;
        }


        [HttpGet]
        public async Task<IActionResult> NotifyNotRead()
        {
            string idUsr = User.Identity.Name;

            string rutaRelativa = string.Format(configuration.GetSection("URIs:NotifyNotRead").Value, idUsr);

            var rpta = servicioApi.GetAsync<List<NotificacionODT>>(rutaRelativa).GetAwaiter().GetResult();

            return Json(rpta);
        }

        [HttpGet]
        public async Task<bool> NotifyUpd(int notify)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:NotifyUpd").Value, notify);
            bool respuesta = await servicioApi.GetAsync<bool>(rutaRelativa).ConfigureAwait(false); ;

            return respuesta;
        }

        public async Task<IActionResult> ConsultarNovedades(int idOperacion)
        {
            string rutaRelativa = configuration.GetSection("URIs:ObtenerNovedadesxOperacion").Value;
            IList<NovedadOtd> respuesta = await servicioApi.GetAsync<IList<NovedadOtd>>(string.Format(rutaRelativa, idOperacion));
            ViewBag.SinDatos = "0";
            if (respuesta.Count() == 0)
            {
                ViewBag.SinDatos = "1";
                return PartialView();
            }
            return PartialView(respuesta);
        }

        public async Task<IList<OperacionVueloOtd>> ObtenerTodosAsync(string fechaInicio, string fechaFinal, string bandera = "0")
        {

            string rutaRelativa = configuration.GetSection("URIs:VuelosObtenerTodos").Value;

            if (bandera == "CON1")
            {
                rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFinal, "CON");
            }
            else
            {
                if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFinal))
                    rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFinal);

                if (bandera == "CON")
                    rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtenerTodos2").Value, fechaInicio, fechaFinal, bandera);
            }

            IList<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<IList<OperacionVueloOtd>>(rutaRelativa);

            if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR"))
            {
                string aerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreAerolinea")).Value;
                respuesta = respuesta.Where(x => x.NombreAerolinea.ToLower().Equals(aerolinea.ToLower())).ToList();
            }

            return respuesta;
        }

        [HttpGet]
        public async Task<IActionResult> DescargarPDF()
        {
            _logger.LogInformation($"Inicio descarga PDF consulta");
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25);

            try
            {
                _logger.LogInformation($"paso 1 descarga PDF consulta");
                MemoryStream ms = new MemoryStream();
                PdfWriter.GetInstance(pdfDoc, ms).CloseStream = false;
                int IdAerolinea = int.Parse(User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);

                string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosPendientes").Value,
                                                    IdAerolinea,
                                                    0,
                                                    0);

                _logger.LogInformation($"paso 2 descarga PDF consulta");

                Dominio.Entidades.RespuestaGrilla<OperacionVueloOtd> respuesta = await servicioApi.GetAsync<Dominio.Entidades.RespuestaGrilla<OperacionVueloOtd>>(rutaRelativa);

                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;
                table.DefaultCell.Border = Rectangle.NO_BORDER;

                iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(ruta + "\\logo-jarvis-informe.png");
                _logger.LogInformation($"Imagen 1 descarga PDF consulta");
                imagen.BorderWidth = 0;
                imagen.ScalePercent(40f);
                PdfPCell cellImg = new PdfPCell();
                cellImg.Border = 0;
                cellImg.HorizontalAlignment = 0;
                cellImg.AddElement(imagen);
                table.AddCell(cellImg);

                Paragraph parrafo = new Paragraph();
                parrafo.Alignment = Element.ALIGN_CENTER;
                parrafo.Font = FontFactory.GetFont("Arial", 16);
                parrafo.Font.SetStyle(Font.BOLD);
                parrafo.Font.SetColor(235, 204, 64);
                parrafo.Add("Consultas - Jarvis");
                table.AddCell(parrafo);

                cellImg = new PdfPCell();
                imagen = iTextSharp.text.Image.GetInstance(ruta + "\\opain-logo-informe.png");
                _logger.LogInformation($"Imagen 2 descarga PDF consulta");
                imagen.BorderWidth = 0;
                imagen.ScalePercent(45f);
                cellImg.Border = 0;
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
                table = new PdfPTable(7);
                table.WidthPercentage = 100;
                //0=Left, 1=Centre, 2=Right
                table.HorizontalAlignment = 1;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;

                table.AddCell("Tipo");
                table.AddCell("Número de Vuelo");
                table.AddCell("Fecha de vuelo");
                table.AddCell("Número de Matrícula");
                table.AddCell("Tasas reportadas");
                table.AddCell("Tasas cobradas");
                table.AddCell("Diferencia de tasas");

                foreach (PdfPCell celda in table.Rows[0].GetCells())
                {
                    celda.BackgroundColor = new iTextSharp.text.BaseColor(235, 204, 64);
                    celda.HorizontalAlignment = 1;
                    celda.Colspan = 1;
                }

                var tasasReportadas = 0;
                var tasasCobradas = 0;
                var detalleNovedad = 0;
                var diferenciaTasas = 0;
                var sumarDiferenciasTasasPDF = 0;
                _logger.LogInformation($"paso 3 descarga PDF consulta");

                foreach (var item in operacionVueloOtd)
                {
                    _logger.LogInformation($"Inicio paso 3.1 descarga PDF consulta");
                    table.AddCell(item.Tipo.ToString());
                    _logger.LogInformation($"Inicio paso 3.2 descarga PDF consulta");
                    table.AddCell(item.Vuelo.ToString());
                    _logger.LogInformation($"Inicio paso 3.3 descarga PDF consulta");
                    table.AddCell(item.Fecha.Day.ToString().PadLeft(2, '0') + "/" + @item.Fecha.Month.ToString().PadLeft(2, '0') + "/" + @item.Fecha.Year.ToString());
                    _logger.LogInformation($"Inicio paso 3.4 descarga PDF consulta");
                    table.AddCell(item.Matricula.ToString());
                    _logger.LogInformation($"Inicio paso 3.5 descarga PDF consulta");

                    //table.AddCell(item.tasasReportadas.ToString());

                    int tasasnacionales = item.PagoCOP.ToString() == string.Empty ? 0 : int.Parse(item.PagoCOP.ToString());
                    int tasasinternacionales = item.PagoUSD.ToString() == string.Empty ? 0 : int.Parse(item.PagoUSD.ToString());

                    int tasasnacionales_liq = item.PAGOCOP_LIQ.ToString() == string.Empty ? 0 : int.Parse(item.PAGOCOP_LIQ.ToString());
                    int tasasinternacionales_liq = item.PAGOUSD_LIQ.ToString() == string.Empty ? 0 : int.Parse(item.PAGOUSD_LIQ.ToString());


                    table.AddCell(((tasasnacionales) + (tasasinternacionales)).ToString());          // Tasas reportadas

                    _logger.LogInformation($"Inicio paso 3.6 descarga PDF consulta");
                    table.AddCell((tasasnacionales_liq + tasasinternacionales_liq).ToString());  // Tasas cobradas

                    _logger.LogInformation($"Inicio paso 3.7 descarga PDF consulta");

                    _logger.LogInformation($"Inicio paso 3.8.4 descarga PDF consulta");
                    //sumarDiferenciasTasasPDF += Convert.ToInt32(diferenciaTasas);   // Diferencia de tasas
                    table.AddCell((Math.Abs(Convert.ToDecimal((tasasnacionales + tasasinternacionales) - (tasasnacionales_liq + tasasinternacionales_liq)))).ToString());

                    _logger.LogInformation($"paso 3.9 descarga PDF consulta");

                    tasasReportadas += Convert.ToInt32((tasasnacionales + tasasinternacionales));

                    tasasCobradas += Convert.ToInt32((tasasnacionales_liq + tasasinternacionales_liq));

                    sumarDiferenciasTasasPDF += Convert.ToInt32((Math.Abs(Convert.ToDecimal((tasasnacionales + tasasinternacionales) -(tasasnacionales_liq + tasasinternacionales_liq)
                        ))).ToString());       //(item.PagoCOP + item.PagoUSD);

                }

                _logger.LogInformation($"Sale paso 3 descarga PDF consulta");
                //totalizado
                var FontColour = new BaseColor(255, 255, 255);
                var Calibri8 = FontFactory.GetFont("Calibri", 12, FontColour);
                PdfPCell cell = new PdfPCell(new Paragraph("Total", Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.Colspan = 4;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(tasasReportadas.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(tasasCobradas.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                
                cell = new PdfPCell(new Paragraph(sumarDiferenciasTasasPDF.ToString(), Calibri8));
                //cell = new PdfPCell(new Paragraph(diferenciaTasas.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);

                pdfDoc.Add(table);



                Paragraph parrafofinal = new Paragraph();
                parrafofinal.Alignment = Element.ALIGN_CENTER;
                parrafofinal.Font = FontFactory.GetFont("Arial", 18);
                parrafofinal.Font.SetStyle(Font.BOLD);
                parrafofinal.Font.SetColor(0, 0, 0);
                var ClaimNombre = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreUsuario"));
                parrafofinal.Add("Generado por: " + ClaimNombre.Value);
                pdfDoc.Add(parrafofinal);

                _logger.LogInformation($"paso 4 descarga PDF consulta");

                writer.CloseStream = false;
                pdfDoc.Close();
                byte[] bytes = ms.ToArray();


                string RutaArchivos = configuration.GetSection("Config:RutaArchivosPdf").Value;
                _logger.LogInformation($"paso  5 descarga PDF consulta, Ruta " + RutaArchivos);
                //string rutaArchivosZip = configuration.GetSection("Config:RutaArchivosPdf").Value + "//ConsultasJarvis.zip";


                if (Directory.Exists(RutaArchivos))
                {
                    Directory.Delete(RutaArchivos, true);
                    _logger.LogInformation($"Elimina ruta: {RutaArchivos} ");
                }

                if (!Directory.Exists(RutaArchivos))
                {
                    Directory.CreateDirectory(RutaArchivos);
                    _logger.LogInformation($"Crea la ruta: {RutaArchivos} ");
                }

                FileStream fs = new FileStream(RutaArchivos + "/" + "ConsultasJarvis.pdf", FileMode.OpenOrCreate);
                fs.Write(bytes, 0, bytes.Length);
                ms.Close();
                fs.Close();
                _logger.LogInformation($"Crea el archivo en la ruta: {RutaArchivos} ");

                string filename = "ConsultasJarvis.pdf";
                string filepath = RutaArchivos;
                string fullName = Path.Combine(filepath, filename);


                _logger.LogInformation($"Ruta de archivo completa combinado {fullName} ");

                return File(new FileStream(fullName, FileMode.Open), "application/pdf", filename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ResourceMessage.ErrorSystem);
                _logger.LogInformation($"paso 6 descarga PDF consulta, error " + ex.Message);
                throw;
            }

        }


        [HttpGet]
        public async Task<IActionResult> DescargarExcel()
        {
            decimal TotalUSD = 0;
            decimal TotalCOP = 0;
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Consulta");
                    //Generamos la cabecera

                    worksheet.Range("A1:O1").Merge().Value = "";
                    worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Cell("D2").Value = "Consulta - Jarvis";
                    worksheet.Range("D2:I2").Style.Font.Bold = true;
                    worksheet.Range("A2:O2").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("D5:I5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    worksheet.Range("A5:O5").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D5:I5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("E2")).Scale(0.6);

                    worksheet.Cell("A6").Value = "Tipo";
                    worksheet.Cell("B6").Value = "Número de Vuelo";
                    worksheet.Cell("C6").Value = "Fecha de vuelo";
                    worksheet.Cell("D6").Value = "Número de Matrícula";
                    worksheet.Cell("E6").Value = "Tasas reportadas";
                    worksheet.Cell("F6").Value = "Tasas cobradas";
                    worksheet.Cell("G6").Value = "Diferencia de tasas";

                    ////-----------Le damos el formato a la cabecera----------------
                    worksheet.Cell("A6").Style.Font.Bold = true;
                    worksheet.Cell("A6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("B6").Style.Font.Bold = true;
                    worksheet.Cell("B6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("C6").Style.Font.Bold = true;
                    worksheet.Cell("C6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("D6").Style.Font.Bold = true;
                    worksheet.Cell("D6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("E6").Style.Font.Bold = true;
                    worksheet.Cell("E6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("F6").Style.Font.Bold = true;
                    worksheet.Cell("F6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("G6").Style.Font.Bold = true;
                    worksheet.Cell("G6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);


                    //-----------Genero la tabla de datos-----------
                    var tasasReportadas = 0;
                    var tasasCobradas = 0;
                    var diferenciaTasasExcel = 0;
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    var sumarDiferenciasTasas = 0;
                    //IEnumerable<Product> noduplicates = operacionVueloOtd.Distinct()=new    ;
                    List<OperacionVueloOtd> listaoperacionVueloOtd = new List<OperacionVueloOtd>();
                    //listaoperacionVueloOtd=(OperacionVueloOtd as operacionVueloOtd.Distinct());
                     listaoperacionVueloOtd = operacionVueloOtd.GroupBy(x => x.Vuelo).Select(x => x.First()).ToList();

                    foreach (var datos in listaoperacionVueloOtd.ToList())
                    {
                        worksheet.Cell(nRow, 1).Value = datos.Tipo.ToString();
                        worksheet.Cell(nRow, 2).Value = datos.Vuelo.ToString();
                        worksheet.Cell(nRow, 3).Value = datos.Fecha.Day.ToString().PadLeft(2, '0') + "/" + datos.Fecha.Month.ToString().PadLeft(2, '0') + "/" + datos.Fecha.Year.ToString();
                        worksheet.Cell(nRow, 4).Value = datos.Matricula.ToString();

                        int tasasnacionales = datos.PagoCOP.ToString() == string.Empty ? 0 : int.Parse(datos.PagoCOP.ToString());
                        int tasasinternacionales = datos.PagoUSD.ToString() == string.Empty ? 0 : int.Parse(datos.PagoUSD.ToString());

                        int tasasnacionales_liq = datos.PAGOCOP_LIQ.ToString() == string.Empty ? 0 : int.Parse(datos.PAGOCOP_LIQ.ToString());
                        int tasasinternacionales_liq = datos.PAGOUSD_LIQ.ToString() == string.Empty ? 0 : int.Parse(datos.PAGOUSD_LIQ.ToString());


                        // Correcion del excel
                        worksheet.Cell(nRow, 5).Value = (tasasnacionales+ tasasinternacionales).ToString(); //datos.tasasReportadas.ToString();

                        worksheet.Cell(nRow, 6).Value = (tasasnacionales_liq + tasasinternacionales_liq).ToString();  //Tasas cobradas

                        worksheet.Cell(nRow, 7).Value = (Math.Abs(Convert.ToDecimal((tasasnacionales + tasasinternacionales) - (tasasnacionales_liq +tasasinternacionales_liq)))).ToString();//diferenciaTasas.ToString();
                        
                        tasasReportadas += Convert.ToInt32((tasasnacionales + tasasinternacionales));

                        tasasCobradas += Convert.ToInt32((tasasnacionales_liq + tasasinternacionales_liq));

                        diferenciaTasasExcel += Convert.ToInt32((Math.Abs(Convert.ToDecimal((tasasnacionales + tasasinternacionales) -(tasasnacionales_liq + tasasinternacionales_liq)))).ToString());  //(item.PagoCOP + item.PagoUSD);

                        nRow++;
                    }
                    // Se agrega el total de cobros generados
                    worksheet.Cell(nRow, 14).Value = TotalCOP;
                    worksheet.Cell(nRow, 16).Value = TotalUSD;


                    worksheet.Cell(nRow, 14).Style.Font.Bold = true;
                    //worksheet.Row(nRow).Style.Fill.BackgroundColor = XLColor.Black;
                    string celda = "A" + nRow + ":D" + nRow;
                    var rango = worksheet.Range(celda).Merge();
                    rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(nRow, 1).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Cell(nRow, 2).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Cell(nRow, 3).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Cell(nRow, 4).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Cell(nRow, 5).Style.Fill.BackgroundColor = XLColor.Black;
                    
                    worksheet.Cell(nRow, 5).Value = tasasReportadas.ToString();  //tasas reportadas
                    worksheet.Cell(nRow, 5).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(nRow, 6).Style.Fill.BackgroundColor = XLColor.Black;

                    worksheet.Cell(nRow, 6).Value = tasasCobradas.ToString();        //tasas cobradas
                    worksheet.Cell(nRow, 6).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(nRow, 7).Style.Fill.BackgroundColor = XLColor.Black;

                    worksheet.Cell(nRow, 7).Value = diferenciaTasasExcel.ToString();// diferenciaTasas.ToString();
                    worksheet.Cell(nRow, 7).Style.Font.FontColor = XLColor.White;
                    worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;


                    worksheet.Cell(nRow, 16).Style.Font.Bold = true;
                    //Le damos estilos a los totales sumados
                    worksheet.Cell(nRow, 11).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 1).Value = "Totales";

                    //worksheet.Range("A" + nRow + ":N" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

                    worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;

                    worksheet.Columns(1, 17).AdjustToContents(); //Ajustamos el ancho de las columnas para que se muestren todos los contenidos
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);//Guardamos el fichero
                        byte[] byteRetorno = stream.ToArray();
                        string RutaArchivos = configuration.GetSection("Config:RutaArchivosPdf").Value;

                        if (Directory.Exists(RutaArchivos))
                        {
                            Directory.Delete(RutaArchivos, true);
                            _logger.LogInformation($"Elimina ruta: {RutaArchivos} ");
                        }

                        if (!Directory.Exists(RutaArchivos))
                        {
                            Directory.CreateDirectory(RutaArchivos);
                            _logger.LogInformation($"Crea la ruta: {RutaArchivos} ");
                        }

                        FileStream fs = new FileStream(RutaArchivos + "/" + "ConsultasJarvis.xlsx", FileMode.OpenOrCreate);
                        fs.Write(byteRetorno, 0, byteRetorno.Length);
                        stream.Close();
                        fs.Close();
                        _logger.LogInformation($"Crea el archivo en la ruta: {RutaArchivos} ");

                        string filename = "ConsultasJarvis.xlsx";
                        string filepath = RutaArchivos;
                        string fullName = Path.Combine(filepath, filename);
                        _logger.LogInformation($"Ruta de archivo completa combinado {fullName} ");
                        listaoperacionVueloOtd.Clear();
                        //window.location.href = "/{controller}/{action}/{params}";
                        return File(new FileStream(fullName, FileMode.Open), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);

                    };
                }
            }
            catch
            {
                throw;
            }
        }

    }
}