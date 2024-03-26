using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;
using Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeParqueosAmplicacionLA33100 : ExportarInformes.Base
    {
        #region "Descargar Excel"
        /// <summary>
        /// Metodo para validar el tipo de cobro y asu vez colocar los valores correspondientes en la cabecera del excel.
        /// </summary>
        /// <param name="Cobro"></param>
        /// <returns>"TotalCOP" || "TotalUSD" en base a la validacion</returns>
        public string ValidarTipoCobro(string Cobro)
        {
            string CadenaRetorno = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Cobro))
                {
                    if (Cobro.Equals("COP"))
                        CadenaRetorno = "TotalCOP";
                    else if (Cobro.Equals("USD"))
                        CadenaRetorno = "TotalUSD";
                }
                return CadenaRetorno;
            }
            catch (Exception ex)
            {
                CadenaRetorno = ex.Message;
                return CadenaRetorno;
            }

        }
        public Decimal SumarTotalPOSCobro(List<Anexo14> Anexo14, string TipoCobro)
        {
            Decimal TotalPOS = 0;
            Decimal TryParsePOS = 0;
            bool ValidarTryParsePOS = false;
            try
            {
                if (Anexo14.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo14)
                        {
                            ValidarTryParsePOS = Decimal.TryParse(item.TotalCOP, out TryParsePOS);
                            if (ValidarTryParsePOS)
                                TotalPOS = TotalPOS + TryParsePOS;
                        }
                    else if (TipoCobro.Equals("USD"))
                        foreach (var item in Anexo14)
                        {
                            ValidarTryParsePOS = Decimal.TryParse(item.TotalUSD, out TryParsePOS);
                            if (ValidarTryParsePOS)
                                TotalPOS = TotalPOS + TryParsePOS;
                        }
                }
                return TotalPOS;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalPOS;
            }
        }

        /// <summary>
        /// Metodo que se encarga de sumar el total de cobró ya sea "COP" || "USD"
        /// </summary>
        /// <param name="Anexo1"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>TotalCobro en base a la suma y validacion del cobró</returns>
        public Decimal SumarTotalCobro(List<Anexo14> Anexo14, string TipoCobro)
        {
            Decimal TotalCobro = 0;
            Decimal TryValor = 0;
            bool ValidarTryParseCobro = false;
            try
            {

                if (Anexo14.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo14)
                        {
                            ValidarTryParseCobro = Decimal.TryParse(item.TotalCOP, out TryValor);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryValor;
                        }
                    else if (TipoCobro.Equals("USD"))
                        foreach (var item in Anexo14)
                        {
                            ValidarTryParseCobro = Decimal.TryParse(item.TarifaUSD, out TryValor);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryValor;
                        }
                }
                return TotalCobro;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalCobro;
            }
        }
                
        public Decimal SumarTotalCantidad(List<Anexo14> Anexo14)
        {
            Decimal TotalCantidad = 0;
            Decimal TryParseCantidad= 0;
            bool ValidarTryParseCobro = false;
            try
            {
                if (Anexo14.Count > 0)
                {
                    foreach (var item in Anexo14)
                    {
                        ValidarTryParseCobro = Decimal.TryParse(item.CantidadHoras, out TryParseCantidad);
                        if (ValidarTryParseCobro)
                            TotalCantidad = TotalCantidad + TryParseCantidad;
                    }
                }
                return TotalCantidad;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalCantidad;
            }
        }

        /// <summary>
        /// Metodo para generar y descargar el excel Anexo1
        /// </summary>
        /// <param name="Anexo1"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>File Excel</returns>
        public byte[] ArmarExcel(List<Anexo14> Anexo14, string TipoCobro, string filtro1, string filtro2)
        {             
            Decimal TotalCobro = 0;
            //Decimal TotalPosCobroCOP = 0;
            //Decimal TotalCobroUSD = 0;
            //Decimal TotalPosCobroUDS = 0;
            decimal TotalCantidad = 0;
            try
            {                 
                //TotalCobroCOP = SumarTotalCobro(Anexo14, "USD");  // unitario
                //TotalPosCobroCOP = SumarTotalPOSCobro(Anexo14, "COP");
                //TotalCobroUSD = SumarTotalCobro(Anexo14, "USD");  // unitario
                //TotalPosCobroUDS = SumarTotalPOSCobro(Anexo14, "USD");
                TotalCantidad = SumarTotalCantidad(Anexo14);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo14");
                    //Generamos la cabecera
                    #region Cabecera Informe
                    worksheet.Range("A1:O1").Merge().Value = "";
                    worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Merge().Value = "ParqueosAmplicacionLA33100% - Jarvis Informe";
                    worksheet.Range("A2:O2").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("D3:I3").Merge().Value = filtro1;
                    worksheet.Range("A3:O3").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D3:I3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("D4:I4").Merge().Value = filtro2;
                    worksheet.Range("A4:O4").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D4:I4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("D5:I5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    worksheet.Range("A5:O5").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D5:I5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("J2")).Scale(0.6);
                    #endregion

                    worksheet.Cell("A6").Value = "Prefijo";
                    worksheet.Cell("B6").Value = "Factura";                   
                    worksheet.Cell("C6").Value = "Matrícula";
                    worksheet.Cell("D6").Value = "Sigla";                    
                    worksheet.Cell("E6").Value = "Aerolínea";
                    worksheet.Cell("F6").Value = "Vuelo Llegada";
                    worksheet.Cell("G6").Value = "Vuelo Salida";                   
                    worksheet.Cell("H6").Value = "Fecha Llegada";
                    worksheet.Cell("I6").Value = "Fecha Salida";
                    worksheet.Cell("J6").Value = "Hora Llegada";
                    worksheet.Cell("K6").Value = "Hora Salida";
                    worksheet.Cell("L6").Value = "Cantidad Horas";
                    worksheet.Cell("M6").Value = "Categoría";
                    worksheet.Cell("N6").Value = "Posición";
                    worksheet.Cell("O6").Value = "Descripción";
                    worksheet.Cell("P6").Value = "Precio Unt.";
                    worksheet.Cell("Q6").Value = "Total";

                    #region  Estilo Hoja de calculo
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
                    worksheet.Cell("H6").Style.Font.Bold = true;
                    worksheet.Cell("H6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("I6").Style.Font.Bold = true;
                    worksheet.Cell("I6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("J6").Style.Font.Bold = true;
                    worksheet.Cell("J6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("K6").Style.Font.Bold = true;
                    worksheet.Cell("K6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("L6").Style.Font.Bold = true;
                    worksheet.Cell("L6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("M6").Style.Font.Bold = true;
                    worksheet.Cell("M6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("N6").Style.Font.Bold = true;
                    worksheet.Cell("N6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("O6").Style.Font.Bold = true;
                    worksheet.Cell("O6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("P6").Style.Font.Bold = true;
                    worksheet.Cell("P6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("Q6").Style.Font.Bold = true;
                    worksheet.Cell("Q6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    #endregion

                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7
                    string SubTotal = "0";
                    foreach (var datos in Anexo14)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.PrefijoFactura;
                        worksheet.Cell(nRow, 2).Value = datos.NumeroFactura;
                        worksheet.Cell(nRow, 3).Value = datos.Matricula;
                        worksheet.Cell(nRow, 4).Value = datos.Sigla;
                        worksheet.Cell(nRow, 5).Value = datos.Aerolinea;
                        worksheet.Cell(nRow, 6).Value = datos.CodigoVueloLlegada;
                        worksheet.Cell(nRow, 7).Value = datos.CodigoVueloSalida;
                        worksheet.Cell(nRow, 8).Value = "'" + datos.FechaLlegadaPosicion;
                        worksheet.Cell(nRow, 9).Value = "'" + datos.FechaSalidaPosicion;
                        worksheet.Cell(nRow, 10).Value = "'" + datos.HoraIngreso.PadLeft(4,'0').Substring(0,2) + ":" + datos.HoraIngreso.PadLeft(4, '0').Substring(2, 2);
                        worksheet.Cell(nRow, 11).Value = "'" + datos.HoraSalida.PadLeft(4, '0').Substring(0, 2) + ":" + datos.HoraSalida.PadLeft(4, '0').Substring(2, 2);
                        worksheet.Cell(nRow, 12).Value = datos.CantidadHoras;
                        worksheet.Cell(nRow, 13).Value = datos.Categoria;
                        worksheet.Cell(nRow, 14).Value = datos.Posicion;
                        worksheet.Cell(nRow, 15).Value = datos.TipoConexion;
                        worksheet.Cell(nRow, 16).Style.NumberFormat.NumberFormatId = 4;                        
                        worksheet.Cell(nRow, 16).Value = datos.TarifaCOP;
                        worksheet.Cell(nRow, 17).Style.NumberFormat.NumberFormatId = 4;
                        worksheet.Cell(nRow, 17).Value = datos.TotalCOP;
                        SubTotal = datos.TotalCOP;
                        if (datos.TipoMoneda=="USD")
                        {
                            SubTotal = datos.TotalUSD;                            
                            worksheet.Cell(nRow, 16).Value = datos.TarifaUSD;
                            worksheet.Cell(nRow, 17).Value = datos.TotalUSD;
                        }
                        TotalCobro = IIFValidDecimal(SubTotal) + TotalCobro;
                        nRow++;
                    }
                    // Se agrega el total de cobros generados 
                    worksheet.Cell(nRow, 12).Value = TotalCantidad;
                    worksheet.Cell(nRow, 12).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 12).Style.NumberFormat.NumberFormatId = 4;
                    worksheet.Cell(nRow, 17).Value = TotalCobro;
                    worksheet.Cell(nRow, 17).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 17).Style.NumberFormat.NumberFormatId = 4;
                    worksheet.Cell(nRow, 1).Value = "Totales";

                    worksheet.Range("A" + nRow + ":Q" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

                    worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;
                    worksheet.Columns(1, 17).AdjustToContents(); //Ajustamos el ancho de las columnas para que se muestren todos los contenidos
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);//Guardamos el fichero
                        byte[] byteRetorno = stream.ToArray();
                        return byteRetorno;
                    };
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
