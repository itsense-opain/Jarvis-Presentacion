using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeGPU
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
        public int SumarTotalPOSCobro(List<Anexo16> Anexo16, string TipoCobro)
        {
            int TotalPOS = 0;
            int TryParsePOS = 0;
            bool ValidarTryParsePOS = false;
            try
            {
                if (Anexo16.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo16)
                        {
                            ValidarTryParsePOS = Int32.TryParse(item.TarifaCOP, out TryParsePOS);
                            if (ValidarTryParsePOS)
                                TotalPOS = TotalPOS + TryParsePOS;
                        }
                    else if (TipoCobro.Equals("USD"))
                        foreach (var item in Anexo16)
                        {
                            ValidarTryParsePOS = Int32.TryParse(item.TarifaCOP, out TryParsePOS);
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
        /// Totalizar los minutos
        /// </summary>
        /// <param name="Anexo16"></param>
        /// <returns></returns>
        public int SumarTotalMinutos(List<Anexo16> Anexo16)
        {
            int TotalMinutos = 0;
            try
            {
                if (Anexo16.Count > 0)
                {
                    foreach (var item in Anexo16)
                    {
                        int TryParseMinutos;
                        bool ValidarTryParseMinutos = Int32.TryParse(item.Minutos, out TryParseMinutos);
                        if (ValidarTryParseMinutos)
                            TotalMinutos = TotalMinutos + TryParseMinutos;
                    }
                }

                return TotalMinutos;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalMinutos;
            }
        }

        /// <summary>
        /// Metodo que se encarga de sumar el total de cobró ya sea "COP" || "USD"
        /// </summary>
        /// <param name="Anexo1"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>TotalCobro en base a la suma y validacion del cobró</returns>
        public int SumarTotalCobro(List<Anexo16> Anexo16, string TipoCobro)
        {
            int TotalCobro = 0;
            int TryParseCobro = 0;
            bool ValidarTryParseCobro = false;
            try
            {

                if (Anexo16.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo16)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.TotalCOP, out TryParseCobro);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryParseCobro;
                        }
                    else if (TipoCobro.Equals("USD"))
                        foreach (var item in Anexo16)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.TotalCOP, out TryParseCobro);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryParseCobro;
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
        /// <summary>
        /// Metodo para generar y descargar el excel Anexo1
        /// </summary>
        /// <param name="Anexo1"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>File Excel</returns>
        public byte[] ArmarExcel(List<Anexo16> Anexo16, string TipoCobro, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty;
            int TotalCobro = 0;
            int TotalMinutos = 0;
            int TotalPosCobro = 0;
            try
            {
                //Se Valida el tipocobro y la suma del TotalCobro,TotalCantidad,TotalPosCobro ya sea "COP" || "USD"
                ValueTotal = ValidarTipoCobro(TipoCobro);
                TotalCobro = SumarTotalCobro(Anexo16, TipoCobro);  // unitario
                TotalPosCobro = SumarTotalPOSCobro(Anexo16, TipoCobro);
                TotalMinutos = SumarTotalMinutos(Anexo16);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo16");
                    //Generamos la cabecera
                    #region Diseño de cabecera
                    if (TipoCobro.Equals("COP"))
                    {
                        worksheet.Range("A1:O1").Merge().Value = "";
                        worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:I2").Merge().Value = "GPU - Jarvis Informe";
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

                        //worksheet.Range("A1:O1").Merge().Value = "GPU - Jarvis Informe";
                        //worksheet.Range("A1:O1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //worksheet.Range("A1:O1").Style.Font.Bold = true;
                        //worksheet.Range("A2:O5").Merge();
                        //worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A3")).Scale(0.5);
                        //worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("L3")).Scale(0.5);
                    }
                    else if (TipoCobro.Equals("USD"))
                    {
                        worksheet.Range("A1:N1").Merge().Value = "";
                        worksheet.Range("A1:N1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:H2").Merge().Value = "GPU - Jarvis Informe";
                        worksheet.Range("A2:N2").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:H2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("D3:H3").Merge().Value = filtro1;
                        worksheet.Range("A3:N3").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D3:H3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("D4:H4").Merge().Value = filtro2;
                        worksheet.Range("A4:N4").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D4:H4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("D5:H5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                        worksheet.Range("A5:N5").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D5:H5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;



                        worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                        worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("I2")).Scale(0.6);

                        //worksheet.Range("A1:N1").Merge().Value = "GPU - Jarvis Informe";
                        //worksheet.Range("A1:N1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //worksheet.Range("A1:N1").Style.Font.Bold = true;
                        //worksheet.Range("A2:N5").Merge();
                        //worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A3")).Scale(0.5);
                        //worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("K3")).Scale(0.5);
                    }
                    #endregion

                    worksheet.Cell("A6").Value = "Prefijo";
                    worksheet.Cell("B6").Value = "Factura";
                    worksheet.Cell("C6").Value = "Aerolinea";
                    worksheet.Cell("D6").Value = "Vuleo llegada";
                    worksheet.Cell("E6").Value = "Vuelo Salida";
                    worksheet.Cell("F6").Value = "Fecha Conexión";
                    worksheet.Cell("G6").Value = "Hora Conexión";
                    worksheet.Cell("H6").Value = "Fecha Desconexión";
                    worksheet.Cell("I6").Value = "Hora Desconexión";
                    worksheet.Cell("J6").Value = "Minutos";
                    worksheet.Cell("K6").Value = "Tarifa USD";
                    worksheet.Cell("L6").Value = "TRM";
                    worksheet.Cell("M6").Value = "Tarifa COP";
                    worksheet.Cell("N6").Value = "Total";

                    #region Diseño Titulo Tabla
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
                    #endregion Fin diseño de titulos tabla

                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo16)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.PrefijoFactura;
                        worksheet.Cell(nRow, 2).Value = datos.Factura;
                        worksheet.Cell(nRow, 3).Value = datos.NombreAerolinea;
                        worksheet.Cell(nRow, 4).Value = datos.NumeroVueloIngreso;
                        worksheet.Cell(nRow, 5).Value = datos.NumeroVueloSalida;
                        worksheet.Cell(nRow, 6).Value = "'" + datos.FechaConexion;
                        worksheet.Cell(nRow, 7).Value = "'" + datos.HoraConexion.PadLeft(4, '0').Substring(0, 2) + ":" + datos.HoraConexion.PadLeft(4, '0').Substring(2, 2);
                        worksheet.Cell(nRow, 8).Value = "'" + datos.FechaDesconexion;
                        worksheet.Cell(nRow, 9).Value = "'" + datos.HoraDesconexion.PadLeft(4, '0').Substring(0, 2) + ":" + datos.HoraDesconexion.PadLeft(4, '0').Substring(2, 2);
                        worksheet.Cell(nRow, 10).Value = datos.Minutos;
                        worksheet.Cell(nRow, 11).Style.NumberFormat.NumberFormatId = 4;
                        worksheet.Cell(nRow, 11).Value = datos.TarifaUSD;
                        worksheet.Cell(nRow, 12).Value = datos.TRM;
                        worksheet.Cell(nRow, 13).Style.NumberFormat.NumberFormatId = 4;
                        worksheet.Cell(nRow, 13).Value = datos.TarifaCOP;
                        worksheet.Cell(nRow, 14).Style.NumberFormat.NumberFormatId = 4;
                        worksheet.Cell(nRow, 14).Value = datos.TotalCOP;

                        nRow++;
                    }
                    // Se agrega el total de cobros generados 
                    worksheet.Cell(nRow, 10).Value = TotalMinutos;
                    worksheet.Cell(nRow, 10).Style.Font.Bold = true;
                    //worksheet.Cell(nRow, 14).Value = TotalPosCobro;
                    //worksheet.Cell(nRow, 14).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 14).Value = TotalCobro;
                    worksheet.Cell(nRow, 14).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 14).Style.NumberFormat.NumberFormatId = 4;

                    worksheet.Cell(nRow, 1).Value = "Totales";

                    worksheet.Range("A" + nRow + ":N" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

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
