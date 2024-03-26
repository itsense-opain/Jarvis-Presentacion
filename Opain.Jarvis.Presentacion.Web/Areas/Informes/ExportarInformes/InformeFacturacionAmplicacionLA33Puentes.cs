using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeFacturacionAmplicacionLA33Puentes : ExportarInformes.Base
    {
        #region "Descargar Excel"
        /// <summary>
        /// Metodo para validar el tipo de cobró y asu vez colocar los valores correspondientes en la cabecera del excel.
        /// </summary>
        /// <param name="Cobro"></param>
        /// <returns>"TotalCOP" || "TotalUSD" en base a la validacion</returns>

        public decimal SumarTotalPOSCobro(List<Anexo15> Anexo15 )
        {
            decimal TotalPOS = 0;
            decimal TryParsePOS = 0;
            bool ValidarTryParsePOS = false;
            try
            {
                if (Anexo15.Count > 0  )
                {
                     
                        foreach (var item in Anexo15)
                        {
                            ValidarTryParsePOS = decimal.TryParse(item.CobroUSD, out TryParsePOS);
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

        public decimal SumarTotalCobro(List<Anexo15> Anexo15 )
        {
            decimal TotalCobro = 0;
            decimal TryParseCobro = 0;
            bool ValidarTryParseCobro = false;
            try
            {

                if (Anexo15.Count > 0 )
                {
                    
                        foreach (var item in Anexo15)
                        {
                            ValidarTryParseCobro = decimal.TryParse(item.TotalCOP, out TryParseCobro);
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
        /// <param name="Anexo15"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>File Excel</returns>
        public byte[] ArmarExcel(List<Anexo15> Anexo15, string filtro1, string filtro2)
        {

            Decimal TotalCobro = 0;
            try
            {               
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo1");
                    //Generamos la cabecera
                    #region Ecabezado del informe
                    worksheet.Range("A1:O1").Merge().Value = "";
                    worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Merge().Value = "Anexo 15 Facturacion Ampliacion LA 33 puentes";
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
                    #endregion

                    worksheet.Cell("A6").Value = "Prefijo";
                    worksheet.Cell("B6").Value = "Factura";
                    worksheet.Cell("C6").Value = "Sigla Aer.";
                    worksheet.Cell("D6").Value = "Matrícula";
                    worksheet.Cell("E6").Value = "Aerolínea";
                    worksheet.Cell("F6").Value = "Vuelos Llegada";
                    worksheet.Cell("G6").Value = "Vuelo Salida";
                    worksheet.Cell("H6").Value = "Fecha Llegada";
                    worksheet.Cell("I6").Value = "Fecha Salida";
                    worksheet.Cell("J6").Value = "Hora Llegada";
                    worksheet.Cell("K6").Value = "Hora Salida";
                    worksheet.Cell("L6").Value = "Cantidad";
                    worksheet.Cell("M6").Value = "Posición";
                    worksheet.Cell("N6").Value = "Precio Unt.";
                    worksheet.Cell("O6").Value = "Total";
                    
                    ////-----------Le damos el formato a la cabecera----------------
                    #region Titulos
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
                    #endregion

                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7
                    string SubTotal = "0";
                    foreach (var datos in Anexo15)
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
                        worksheet.Cell(nRow, 10).Value = datos.HoraIngreso;
                        worksheet.Cell(nRow, 11).Value = datos.HoraSalida;
                        worksheet.Cell(nRow, 12).Value = datos.Cantidad;
                        /* Pendiente validacion de campos*/
                        worksheet.Cell(nRow, 13).Value = datos.POS;
                        worksheet.Cell(nRow, 14).Value = datos.TotalCOP;
                        worksheet.Cell(nRow, 15).Value = datos.CobroCOP;

                        //if (TipoCobro.Equals("COP"))
                        //{
                        //    worksheet.Cell("O6").Style.Font.Bold = true;
                        //    worksheet.Cell("O6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                        //    worksheet.Cell(nRow, 13).Value = datos.POSCobroCOP;
                        //    worksheet.Cell(nRow, 14).Value = datos.TotalCOP;
                        //}
                        //else if (TipoCobro.Equals("USD"))
                        //{
                        //    worksheet.Cell(nRow, 13).Value = datos.CobroUSD;
                        //    worksheet.Cell(nRow, 14).Value = datos.TotalUSD;
                        //}

                        SubTotal = datos.TotalCOP;
                        if (datos.TipoMoneda == "USD")
                        {
                            SubTotal = datos.TotalUSD;
                            worksheet.Cell(nRow, 14).Value = datos.TotalUSD;
                            worksheet.Cell(nRow, 15).Value = datos.CobroUSD;
                        }
                        TotalCobro = IIFValidDecimal(SubTotal) + TotalCobro;
                        nRow++;
                    }
                    // Se agrega el total de cobros generados
                    
                    worksheet.Cell(nRow, 14).Style.Font.Bold = true;

                    worksheet.Cell(nRow, 15).Value = TotalCobro;
                    worksheet.Cell(nRow, 15).Style.Font.Bold = true;                    

                    worksheet.Row(nRow).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;                   

                    worksheet.Cell(nRow, 16).Style.Font.Bold = true;
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
