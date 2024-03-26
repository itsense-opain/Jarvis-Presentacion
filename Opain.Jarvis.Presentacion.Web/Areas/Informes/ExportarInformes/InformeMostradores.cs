using System;
using System.Collections.Generic;
using Opain.Jarvis.Dominio.Entidades;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeasMostradores
    {
        private Decimal SumarColumna1(List<Anexo5> Anexo5, string Tipo)
        {
            Decimal TotalSumarColumnas = 0;
            Decimal TryParseHoras = 0;
            bool ValidarTryParseHoras = false;
            try
            {
                if (Anexo5.Count > 0 )
                {
                        foreach (var item in Anexo5)
                        {
                            ValidarTryParseHoras = Decimal.TryParse(item.Cantidad, out TryParseHoras);
                            if (ValidarTryParseHoras)
                            TotalSumarColumnas = TotalSumarColumnas + TryParseHoras;
                        }
                }
                return TotalSumarColumnas;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalSumarColumnas;
            }
        }
        private int SumarColumna2(List<Anexo5> Anexo5, string Tipo)
        {
            int TotalSumarColumnas = 0;
            int TryParseHoras = 0;
            bool ValidarTryParseHoras = false;
            try
            {
                if (Anexo5.Count > 0 )
                {
                    foreach (var item in Anexo5)
                    {
                        ValidarTryParseHoras = Int32.TryParse(item.Cantidad, out TryParseHoras);
                        if (ValidarTryParseHoras)
                            TotalSumarColumnas = TotalSumarColumnas + TryParseHoras;
                    }
                }
                return TotalSumarColumnas;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalSumarColumnas;
            }
        }
        public Decimal SumarColumna3(List<Anexo5> Anexo5, string Tipo)
        {
            Decimal TotalSumarColumnas = 0;
            Decimal TryParseHoras = 0;
            bool ValidarTryParseHoras = false;
            try
            {
                if (Anexo5.Count > 0 )
                {
                    foreach (var item in Anexo5)
                    {
                        ValidarTryParseHoras = Decimal.TryParse(item.CobroUSD, out TryParseHoras);
                        if (ValidarTryParseHoras)
                            TotalSumarColumnas = TotalSumarColumnas + TryParseHoras;
                    }
                }
                return TotalSumarColumnas;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalSumarColumnas;
            }
        }

        public byte[] ArmarExcel(List<Anexo5> Anexo5, string Tipo, string filtro1, string filtro2)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    #region Encabezado del informe
                    var worksheet = workbook.Worksheets.Add("Anexo5");
                    //Generamos la cabecera
                    worksheet.Range("A1:O1").Merge().Value = "";
                    worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:G2").Merge().Value = "Informe Mostradores";
                    worksheet.Range("D2:G2").Style.Font.Bold = true;
                    worksheet.Range("A2:O2").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("D3:G3").Merge().Value = filtro1;
                    worksheet.Range("A3:O3").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D3:G3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("D4:G4").Merge().Value = filtro2;
                    worksheet.Range("A4:O4").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D4:G4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("D5:G5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    worksheet.Range("A5:O5").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D5:G5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("H2")).Scale(0.5);
                    #endregion
                   
                    worksheet.Cell("A6").Value = "Prefijo";
                    worksheet.Cell("B6").Value = "Factura";
                    worksheet.Columns(1,2).Width = 400;
                    worksheet.Cell("C6").Value = "Aerolínea";
                    worksheet.Cell("D6").Value = "Fecha Salida";
                    worksheet.Cell("E6").Value = "Matricula";
                    worksheet.Cell("F6").Value = "Sigla";
                    worksheet.Cell("G6").Value = "Vuelos Salida";
                    worksheet.Cell("H6").Value = "Precio Unitario";
                    worksheet.Cell("I6").Value = "Cantidad";
                    worksheet.Cell("J6").Value = "Total";

                    ////-----------Le damos el formato a la cabecera----------------
                    #region Estilo tuitulos de columnas
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
                    worksheet.Cell("J6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    #endregion

                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo5)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.PrefijoFactura;
                        worksheet.Cell(nRow, 2).Value = datos.Factura;
                        worksheet.Cell(nRow, 3).Value = datos.Aerolineas;
                        worksheet.Cell(nRow, 4).Value = "'" + datos.FechaVuelo;
                        worksheet.Cell(nRow, 5).Value = datos.Matricula;
                        worksheet.Cell(nRow, 6).Value = datos.Sigla;
                        worksheet.Cell(nRow, 7).Value = datos.NumeroVuelo;
                        worksheet.Cell(nRow, 8).Value = "'" + datos.TarifaUSD;
                        worksheet.Cell(nRow, 9).Value = datos.Cantidad;                       
                        //worksheet.Cell(nRow, 9).Style.NumberFormat.NumberFormatId = 4;
                        worksheet.Cell(nRow, 10).Value = "'" + datos.CobroUSD;
                        //worksheet.Cell(nRow, 10).Style.NumberFormat.NumberFormatId = 3;
                        nRow++;
                    }
                    // Se agrega el total de cobros generados
                    //worksheet.Cell(nRow, 7).Value = SumarColumna1(Anexo5, Tipo);
                    //worksheet.Cell(nRow, 7).Style.Font.Bold = true;

                    worksheet.Cell(nRow, 9).Value = SumarColumna2(Anexo5, Tipo);
                    worksheet.Cell(nRow, 9).Style.Font.Bold = true;

                    worksheet.Cell(nRow,10).Value = SumarColumna3(Anexo5, Tipo);
                    worksheet.Cell(nRow, 10).Style.Font.Bold = true;

                    worksheet.Cell(nRow, 1).Value = "Totales";

                    worksheet.Range("A" + nRow + ":J" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

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
    }
}
