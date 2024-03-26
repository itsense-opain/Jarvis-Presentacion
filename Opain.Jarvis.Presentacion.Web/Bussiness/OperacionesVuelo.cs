using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Opain.Jarvis.Dominio.Entidades;

namespace Opain.Jarvis.Presentacion.Web.Bussiness
{
    public class OperacionesVuelo
    {
        public async Task<IList<OperacionVueloOtd>> ObtenerVuelosPendientesProcesar(System.Security.Claims.ClaimsPrincipal User,
            Microsoft.Extensions.Configuration.IConfiguration config,
            Opain.Jarvis.Presentacion.Web.Helpers.IServicioApi servicioApi,
            OperacionVueloOTDRequest oFiltro)
        {
            string urlAPI = "api/Vuelos/ObtenerDatos";
            
            if (User.IsInRole("AEROLINEA") || User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
            {
                oFiltro.IdAerolinea= User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value;
                oFiltro.EstadoProceso = " In (0,1,2,3,4) ";                
            }
            else
            {
                oFiltro.EstadoProceso = " In (0,1,3,4,5) ";
            }
            IList<OperacionVueloOtd> oResult = await servicioApi.PostAsync<IList<OperacionVueloOtd>>(urlAPI, oFiltro);
            return oResult;
        }
        public async Task<IList<OperacionVueloOtd>> ObtenerVuelosParaValidar(System.Security.Claims.ClaimsPrincipal User,
           Microsoft.Extensions.Configuration.IConfiguration config,
           Opain.Jarvis.Presentacion.Web.Helpers.IServicioApi servicioApi,
           OperacionVueloOTDRequest oFiltro)
        {
            string urlAPI = "api/Vuelos/ObtenerDatos";
            oFiltro.OpcionOperacion = "V";
            oFiltro.EstadoProceso = " not In (1,2,3,4,5,6) ";   
            
            IList<OperacionVueloOtd> oResult = await servicioApi.PostAsync<IList<OperacionVueloOtd>>(urlAPI, oFiltro);
            return oResult;
        }

        public async Task<IList<OperacionVueloOtd>> ObtenerVuelosLiquidar(System.Security.Claims.ClaimsPrincipal User,
           Microsoft.Extensions.Configuration.IConfiguration config,
           Opain.Jarvis.Presentacion.Web.Helpers.IServicioApi servicioApi,
           OperacionVueloOTDRequest oFiltro)
        {
            string urlAPI = "api/Vuelos/ObtenerDatos";
            oFiltro.OpcionOperacion = "LIQ";
            oFiltro.IdAerolinea = User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value;

            IList<OperacionVueloOtd> oResult = await servicioApi.PostAsync<IList<OperacionVueloOtd>>(urlAPI, oFiltro);
            return oResult;
        }

        public async Task<IdAerolineaYIdOperacionVueloOtd> GenerarPDFVuelosSoporte(System.Security.Claims.ClaimsPrincipal User,
            Microsoft.Extensions.Configuration.IConfiguration config,
            Opain.Jarvis.Presentacion.Web.Helpers.IServicioApi servicioApi,
            IList<OperacionVueloOtd> vuelosFiltrados
            )
        {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images");
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25);

            IdAerolineaYIdOperacionVueloOtd IdAerolineaYIdOperacionVuelo = new IdAerolineaYIdOperacionVueloOtd();
            IdAerolineaYIdOperacionVuelo.IdAerolinea = 0;

            using (MemoryStream ms = new MemoryStream())
            {
                int IdAerolinea = int.Parse(User.Claims.FirstOrDefault(c => c.Type.Equals("IdAerolinea")).Value);

                string rutaRelativa = string.Format(config.GetSection("URIs:VuelosPendientesSoporte").Value,
                                                    IdAerolinea,
                                                    0,
                                                    0);

                Dominio.Entidades.RespuestaGrilla<OperacionVueloOtd> respuesta = 
                    await servicioApi.GetAsync<Dominio.Entidades.RespuestaGrilla<OperacionVueloOtd>>(rutaRelativa);

                List<OperacionVueloOtd> vuelosCoincidentes = new List<OperacionVueloOtd>();
                foreach (var item in vuelosFiltrados)
                {
                    OperacionVueloOtd objComparar = respuesta.Resultado.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (objComparar!=null)
                    {
                        vuelosCoincidentes.Add(objComparar);
                    }
                }

                respuesta.Resultado = vuelosCoincidentes;
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;
                table.DefaultCell.Border = Rectangle.NO_BORDER;

                iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(ruta + "\\logo-jarvis-informe.png");
                imagen.BorderWidth = 0;
                imagen.ScalePercent(40f);
                PdfPCell cellImg = new PdfPCell();
                cellImg.Border = 0;
                cellImg.HorizontalAlignment = 0;
                cellImg.AddElement(imagen);
                table.AddCell(cellImg);

                Paragraph parrafo = new Paragraph();
                parrafo.Alignment = Element.ALIGN_CENTER;
                parrafo.Font = FontFactory.GetFont("Arial", 14);
                parrafo.Font.SetStyle(Font.BOLD);
                parrafo.Font.SetColor(235, 204, 64);
                parrafo.Add("Soporte Carga Vuelos " + DateTime.Now);
                table.AddCell(parrafo);

                cellImg = new PdfPCell();
                imagen = iTextSharp.text.Image.GetInstance(ruta + "\\opain-logo-informe.png");
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
                table = new PdfPTable(14);
                table.WidthPercentage = 100;
                //0=Left, 1=Centre, 2=Right
                table.HorizontalAlignment = 1;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 30f;

                table.AddCell("Fecha Salida");
                table.AddCell("Tipo");
                table.AddCell("Matrícula");
                table.AddCell("Vuelo");
                table.AddCell("Destino");
                table.AddCell("Hora Salida");
                table.AddCell("Total PAX");
                table.AddCell("INF");
                table.AddCell("TTL");
                table.AddCell("TTC");
                table.AddCell("EX");
                table.AddCell("TRIP");
                table.AddCell("Pago COP");
                table.AddCell("Pago USD");

                foreach (PdfPCell celda in table.Rows[0].GetCells())
                {
                    celda.BackgroundColor = new iTextSharp.text.BaseColor(235, 204, 64);
                    celda.HorizontalAlignment = 1;
                    celda.Colspan = 1;
                }

                var contValidos = 0;
                var cantPasajeros = 0;
                var cantInfantes = 0;
                var cantttl = 0;
                var canttc = 0;
                var cantEx = 0;
                var cantTRIOP = 0;
                var pagoCop = 0;
                var pagoUsd = 0;

                foreach (var item in respuesta.Resultado)
                {
                    rutaRelativa = string.Format(config.GetSection("URIs:VuelosValidaManual").Value, item.Id);
                    Dominio.Entidades.RespuestaValidacionManual respuestaValidaManual = 
                        await servicioApi.GetAsync<Dominio.Entidades.RespuestaValidacionManual>(rutaRelativa);

                    if (item.Tipo.Equals("DOM"))
                    {


                        if (item.ConfirmacionPasajeros.Equals(1) && item.ConfirmacionManifiesto.Equals(1))
                        {

                            table.AddCell(item.Fecha.ToString("dd/MM/yy"));
                            table.AddCell(item.Tipo.ToString());
                            table.AddCell(item.Matricula.ToString());
                            table.AddCell(item.Vuelo.ToString());
                            table.AddCell(item.Destino.ToString());
                            table.AddCell(item.Hora.ToString());
                            table.AddCell(respuestaValidaManual.CantPasajeros.ToString());
                            table.AddCell(respuestaValidaManual.CantInfantes.ToString());
                            table.AddCell(respuestaValidaManual.CantTTL.ToString());
                            table.AddCell(respuestaValidaManual.CantTTC.ToString());
                            table.AddCell(respuestaValidaManual.CantEX.ToString());
                            table.AddCell(respuestaValidaManual.CantTRIP.ToString());
                            table.AddCell(item.PAGOCOP_LIQ.ToString());
                            table.AddCell(item.PAGOUSD_LIQ.ToString());

                            cantPasajeros += respuestaValidaManual.CantPasajeros;
                            cantInfantes += respuestaValidaManual.CantInfantes;
                            cantttl += respuestaValidaManual.CantTTL;
                            canttc += respuestaValidaManual.CantTTC;
                            cantEx += respuestaValidaManual.CantEX;
                            cantTRIOP += respuestaValidaManual.CantTRIP;
                            pagoCop += item.PAGOCOP_LIQ.Value;
                            pagoUsd += item.PAGOUSD_LIQ.Value;

                            contValidos = contValidos + 1;
                        }
                    }

                    if (item.Tipo == "INT")
                    {

                        if (item.ConfirmacionPasajeros.Equals(1) && item.ConfirmacionManifiesto.Equals(1) && item.ConfirmacionGenDec.Equals(1))
                        {

                            table.AddCell(item.Fecha.ToString("dd/MM/yy"));
                            table.AddCell(item.Tipo.ToString());
                            table.AddCell(item.Matricula.ToString());
                            table.AddCell(item.Vuelo.ToString());
                            table.AddCell(item.Destino.ToString());
                            table.AddCell(item.Hora.ToString());
                            table.AddCell(respuestaValidaManual.CantPasajeros.ToString());
                            table.AddCell(respuestaValidaManual.CantInfantes.ToString());
                            table.AddCell(respuestaValidaManual.CantTTL.ToString());
                            table.AddCell(respuestaValidaManual.CantTTC.ToString());
                            table.AddCell(respuestaValidaManual.CantEX.ToString());
                            table.AddCell(respuestaValidaManual.CantTRIP.ToString());
                            table.AddCell(item.PAGOCOP_LIQ.ToString());
                            table.AddCell(item.PAGOUSD_LIQ.ToString());

                            cantPasajeros += respuestaValidaManual.CantPasajeros;
                            cantInfantes += respuestaValidaManual.CantInfantes;
                            cantttl += respuestaValidaManual.CantTTL;
                            canttc += respuestaValidaManual.CantTTC;
                            cantEx += respuestaValidaManual.CantEX;
                            cantTRIOP += respuestaValidaManual.CantTRIP;
                            pagoCop += item.PAGOCOP_LIQ.Value;
                            pagoUsd += item.PAGOUSD_LIQ.Value;

                            contValidos = contValidos + 1;

                        }
                    }
                }
                //totalizado

                var FontColour = new BaseColor(255, 255, 255);
                var Calibri8 = FontFactory.GetFont("Calibri", 12, FontColour);
                PdfPCell cell = new PdfPCell(new Paragraph("Total", Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                cell.Colspan = 6;
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(cantPasajeros.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(cantInfantes.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(cantttl.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(canttc.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(cantEx.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(cantTRIOP.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(pagoCop.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);
                cell = new PdfPCell(new Paragraph(pagoUsd.ToString(), Calibri8));
                cell.BackgroundColor = new iTextSharp.text.BaseColor(34, 36, 38);
                table.AddCell(cell);

                if (contValidos == 0)
                {
                    IdAerolineaYIdOperacionVuelo.IdAerolinea = -1;
                    return IdAerolineaYIdOperacionVuelo;
                }

                pdfDoc.Add(table);

                string idcargue = respuesta.Resultado.FirstOrDefault().IdConsecutivoCargue.ToString();
                string idOperacionVuelo = respuesta.Resultado.FirstOrDefault().Id.ToString();
                               

                Paragraph parrafofinal = new Paragraph();
                parrafofinal.Alignment = Element.ALIGN_CENTER;
                parrafofinal.Font = FontFactory.GetFont("Arial", 18);
                parrafofinal.Font.SetStyle(Font.BOLD);
                parrafofinal.Font.SetColor(0, 0, 0);
                var ClaimNombre = User.Claims.FirstOrDefault(c => c.Type.Equals("NombreUsuario"));
                parrafofinal.Add("Id cargue: " + idcargue + ", Generado por: " + ClaimNombre.Value);
                pdfDoc.Add(parrafofinal);

               
                pdfDoc.Close();
                byte[] bytes = ms.ToArray();


                string BytesEncode = System.Convert.ToBase64String(bytes);
                var token = User.Claims.Where(c => c.Type.Equals("token")).FirstOrDefault().Value;
                string rutaRelativaAero = config.GetSection("URIs:AerolineaObtenerTodos").Value;
                IList<AerolineaOtd> respuestaAero = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAero);
                string Abreviatura = respuestaAero.Where(p => p.Id == IdAerolinea).FirstOrDefault().Sigla;

                string FolderSoporte = Abreviatura + "//" + "Soporte";

                var cargaArch = Opain.Jarvis.Presentacion.Web.Helpers.CargarArchivos.CargarArchSoportes(config,
                    BytesEncode, token,
                    FolderSoporte,
                    idOperacionVuelo);

                string UrlFile = FolderSoporte
                    + "//" + System.DateTime.Now.ToString("yyyyMMdd")
                    + "//" + idOperacionVuelo + ".pdf";

                IdAerolineaYIdOperacionVuelo.IdAerolinea = IdAerolinea;
                IdAerolineaYIdOperacionVuelo.IdOperacionVuelo = idOperacionVuelo;
                IdAerolineaYIdOperacionVuelo.UrlFile = UrlFile;

                return IdAerolineaYIdOperacionVuelo;
            }            
            
        }
    }
}
