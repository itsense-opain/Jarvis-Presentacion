using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformePuenteAbordaje
    {
        #region "Descargar Excel"
        /// <summary>
        /// Metodo para validar el tipo de cobró y asu vez colocar los valores correspondientes en la cabecera del excel.
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
        public int SumarTotalPOSCobro(List<Anexo1> Anexo1, string TipoCobro)
        {
            int TotalPOS = 0;
            int TryParsePOS = 0;
            bool ValidarTryParsePOS = false;
            try
            {
                if (Anexo1.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo1)
                        {
                            ValidarTryParsePOS = Int32.TryParse(item.POSCobroCOP, out TryParsePOS);
                            if (ValidarTryParsePOS)
                                TotalPOS = TotalPOS + TryParsePOS;
                        }
                    //else if (TipoCobro.Equals("USD"))
                    //    foreach (var item in Anexo1)
                    //    {
                    //        ValidarTryParseCobro = Int32.TryParse(item.TotalUSD, out TryParseCobro);
                    //        if (ValidarTryParseCobro)
                    //            TotalCobro = TotalCobro + TryParseCobro;
                    //    }
                }
                return TotalPOS;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalPOS;
            }
        }
        public int SumarTotalCantidad(List<Anexo1> Anexo1, string TipoCobro)
        {
            int TotalCantidad = 0;
            int TryParseCantidad = 0;
            bool ValidarTryParseCantidad = false;
            try
            {
                if (Anexo1.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo1)
                        {

                            ValidarTryParseCantidad = Int32.TryParse(item.Cantidad, out TryParseCantidad);
                            if (ValidarTryParseCantidad)
                                TotalCantidad = TotalCantidad + TryParseCantidad;
                        }
                    else if (TipoCobro.Equals("USD"))
                        foreach (var item in Anexo1)
                        {

                            ValidarTryParseCantidad = Int32.TryParse(item.Cantidad, out TryParseCantidad);
                            if (ValidarTryParseCantidad)
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
        /// Metodo que se encarga de sumar el total de cobró ya sea "COP" || "USD"
        /// </summary>
        /// <param name="Anexo1"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>TotalCobro en base a la suma y validacion del cobró</returns>
        public int SumarTotalCobro(List<Anexo1> Anexo1, string TipoCobro)
        {
            int TotalCobro = 0;
            int TryParseCobro = 0;
            bool ValidarTryParseCobro = false;
            try
            {

                if (Anexo1.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo1)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.TotalCOP, out TryParseCobro);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryParseCobro;
                        }
                    else if (TipoCobro.Equals("USD"))
                        foreach (var item in Anexo1)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.TotalUSD, out TryParseCobro);
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
        public byte[] ArmarExcel(List<Anexo1> Anexo1, string TipoCobro, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty;
            int TotalCobro = 0;
            int TotalCantidad = 0;
            int TotalPosCobro = 0;
            try
            {
                //Se Valida el tipocobro y la suma del TotalCobro,TotalCantidad,TotalPosCobro ya sea "COP" || "USD"
                //ValueTotal = ValidarTipoCobro(TipoCobro);
                TotalCobro = SumarTotalCobro(Anexo1, TipoCobro);
                TotalCantidad = SumarTotalCantidad(Anexo1, TipoCobro);
                //TotalPosCobro = SumarTotalPOSCobro(Anexo1, TipoCobro);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo1");
                    //Generamos la cabecera
                    #region Diseño de encabezado 
                    if (TipoCobro.Equals("COP"))
                    {
                        worksheet.Range("A1:O1").Merge().Value = "";
                        worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:I2").Merge().Value = "Anexo 1 Puente Abordaje";
                        worksheet.Range("D2:I2").Style.Font.Bold = true;
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
                    }
                    else if (TipoCobro.Equals("USD"))
                    {
                        worksheet.Range("A1:N1").Merge().Value = "";
                        worksheet.Range("A1:N1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:H2").Merge().Value = "Anexo 1 Puente Abordaje";
                        worksheet.Range("D2:H2").Style.Font.Bold = true;
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
                    }
                    #endregion
                    worksheet.Cell("A6").Value = "Prefijo";
                    worksheet.Cell("B6").Value = "Factura";
                    worksheet.Cell("C6").Value = "Sigla Aerolínea";
                    worksheet.Cell("D6").Value = "Matricula";
                    worksheet.Cell("E6").Value = "Aerolínea";
                    worksheet.Cell("F6").Value = "Vuelo LLegada";
                    worksheet.Cell("G6").Value = "Vuelo Salida";
                    worksheet.Cell("H6").Value = "Fecha llegando";
                    worksheet.Cell("I6").Value = "Fecha Salida";
                    worksheet.Cell("J6").Value = "Hora LLegada";
                    worksheet.Cell("K6").Value = "Hora Salida";
                    worksheet.Cell("L6").Value = "Cantidad";
                    worksheet.Cell("M6").Value = "Posición";
                    if (TipoCobro.Equals("COP"))
                    {
                        worksheet.Cell("N6").Value = "Prec. Unt. COP";                        
                    }
                    else {
                        worksheet.Cell("N6").Value = "Prec. Unt. USD";                       
                    }
                    worksheet.Cell("O6").Value = "Total";

                    ////-----------Le damos el formato a la cabecera----------------
                    #region Estilo de los titulos
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
                    #endregion

                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo1)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.PrefijoFactura;
                        worksheet.Cell(nRow, 2).Value = datos.Factura;
                        worksheet.Cell(nRow, 3).Value = datos.SiglaAerolinea;
                        worksheet.Cell(nRow, 4).Value = datos.Matricula;
                        worksheet.Cell(nRow, 5).Value = datos.NombreAerolinea;
                        worksheet.Cell(nRow, 6).Value = datos.VueloIngreso;
                        worksheet.Cell(nRow, 7).Value = datos.VueloSalida;
                        worksheet.Cell(nRow, 8).Value = "'" + datos.FechaIngreso;
                        worksheet.Cell(nRow, 9).Value = "'" + datos.FechaSalida;
                        worksheet.Cell(nRow, 10).Value = "'" + datos.HoraIngreso.PadLeft(4, '0').Substring(0, 2) + ":" + datos.HoraIngreso.PadLeft(4, '0').Substring(2, 2);
                        worksheet.Cell(nRow, 11).Value = "'" + datos.HoraSalida.PadLeft(4, '0').Substring(0, 2) + ":" + datos.HoraSalida.PadLeft(4, '0').Substring(2, 2);
                        worksheet.Cell(nRow, 12).Value = datos.Cantidad;
                        worksheet.Cell(nRow, 13).Value = datos.POS;                        
                        /* Pendiente validacion de campos*/
                        //worksheet.Cell(nRow, 13).Value = datos.CobroCOP;
                        //worksheet.Cell(nRow, 14).Value = datos.CobroUSD;

                        if (TipoCobro.Equals("COP"))
                        {
                            //worksheet.Cell("O6").Style.Font.Bold = true;
                            //worksheet.Cell("O6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                            //worksheet.Cell(nRow, 13).Value = datos.POSCobroCOP;
                            //worksheet.Cell(nRow, 14).Value = datos.TotalCOP;
                            worksheet.Cell(nRow, 14).Value = datos.POSCobroCOP;
                            worksheet.Cell(nRow, 15).Value = datos.TotalCOP;
                        }
                        else if (TipoCobro.Equals("USD"))
                        {
                            worksheet.Cell(nRow, 14).Value = datos.CobroUSD;
                            worksheet.Cell(nRow, 15).Value = datos.TotalUSD;
                        }                           
                        nRow++;
                    }
                    // Se agrega el total de cobros generados
                    worksheet.Cell(nRow, 12).Value = TotalCantidad;
                    //if (TipoCobro.Equals("COP"))
                    //{
                    //    worksheet.Cell(nRow, 13).Value = TotalPosCobro;
                    //    worksheet.Cell(nRow, 13).Style.Font.Bold = true;
                    //    worksheet.Row(nRow).Style.Fill.BackgroundColor = XLColor.Black;
                    //    worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;
                    //    worksheet.Cell(nRow, 14).Value = TotalCobro;
                    //    worksheet.Cell(nRow, 14).Style.Font.Bold = true;
                    //}
                    //else
                    
                    worksheet.Cell(nRow, 15).Value = TotalCobro;
                    worksheet.Cell(nRow, 15).Style.Font.Bold = true;
                    //Le damos estilos a los totales sumados
                    worksheet.Cell(nRow, 11).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 1).Value = "Totales";

                    worksheet.Range("A" + nRow + ":O" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

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
