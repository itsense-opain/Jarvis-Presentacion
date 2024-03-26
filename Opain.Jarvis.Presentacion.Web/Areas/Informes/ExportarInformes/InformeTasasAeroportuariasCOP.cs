using System;
using System.Collections.Generic;
using Opain.Jarvis.Dominio.Entidades;
using System.IO;
using ClosedXML.Excel;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeasTasAeroportuariasCOP
    {
        

        public int SumarColumna1(List<Anexo3> Anexo3, string Tipo)
        {
            int TotalSumarColumnas = 0;
            int TryParseHoras = 0;
            bool ValidarTryParseHoras = false;
            try
            {
                if (Anexo3.Count > 0 )
                {
                        foreach (var item in Anexo3)
                        {
                            ValidarTryParseHoras = Int32.TryParse(item.TarifaCOP, out TryParseHoras);
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

        public int SumarColumna2(List<Anexo3> Anexo3, string Tipo)
        {
            int TotalSumarColumnas = 0;
            int TryParseHoras = 0;
            bool ValidarTryParseHoras = false;
            try
            {
                if (Anexo3.Count > 0 )
                {
                    foreach (var item in Anexo3)
                    {
                        ValidarTryParseHoras = Int32.TryParse(item.PaganCOP, out TryParseHoras);
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

        public Int64 SumarColumna3(List<Anexo3> Anexo3, string Tipo)
        {
            Int64 TotalSumarColumnas = 0;
            Int64 TryParseHoras = 0;
            bool ValidarTryParseHoras = false;
            try
            {
                if (Anexo3.Count > 0 )
                {
                    foreach (var item in Anexo3)
                    {
                        ValidarTryParseHoras = Int64.TryParse(item.CobroCOP, out TryParseHoras);
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

        public int SumarColumna4(List<Anexo3> Anexo3, string Tipo)
        {
            int TotalSumarColumnas = 0;
            int TryParseHoras = 0;
            bool ValidarTryParseHoras = false;
            try
            {
                if (Anexo3.Count > 0 )
                {
                    foreach (var item in Anexo3)
                    {
                        ValidarTryParseHoras = Int32.TryParse(item.VrComisionCOP, out TryParseHoras);
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




        public byte[] ArmarExcel(List<Anexo3> Anexo3, string Tipo, string filtro1, string filtro2)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo3");
                    //Generamos la cabecera
                    #region Encabezado de Informes
                    worksheet.Range("A1:K1").Merge().Value = "";
                    worksheet.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.White;
                
                    worksheet.Range("C2:G2").Merge().Value = "Anexo 3 Tasas aeroportuarias COP";
                    worksheet.Range("C2:G2").Style.Font.Bold = true;

                    worksheet.Range("A2:K2").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("C2:G2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("C3:G3").Merge().Value = filtro1;
                    worksheet.Range("A3:K3").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("C3:G3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("C4:G4").Merge().Value = filtro2;
                    worksheet.Range("A4:K4").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("C4:G4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Range("C5:G5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    worksheet.Range("A5:K5").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("C5:G5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("H2")).Scale(0.6);
                    #endregion


                    worksheet.Cell("A6").Value = "Prefijo";
                    worksheet.Cell("B6").Value = "Factura";
                    worksheet.Cell("C6").Value = "Aerolínea";
                    worksheet.Cell("D6").Value = "Fecha Salida";
                    worksheet.Cell("E6").Value = "Matricula";
                    worksheet.Cell("F6").Value = "Sigla";
                    worksheet.Cell("G6").Value = "Vuelo Salida";
                    worksheet.Cell("H6").Value = "Precio Unt.";
                    worksheet.Cell("I6").Value = "Cantidad";
                    worksheet.Cell("J6").Value = "Total";
                    worksheet.Cell("K6").Value = "Comisión";

                    ////-----------Le damos el formato a la cabecera---------------- 
                    #region Diseño de títulos columnas
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
                    #endregion

                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo3)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.PrefijoFactura;
                        worksheet.Cell(nRow, 2).Value = datos.Factura;
                        worksheet.Cell(nRow, 3).Value = datos.Aerolinea;
                        worksheet.Cell(nRow, 4).Value = "'" + datos.FechaVuelo;
                        worksheet.Cell(nRow, 5).Value = datos.Matricula;
                        worksheet.Cell(nRow, 6).Value = datos.Sigla;
                        worksheet.Cell(nRow, 7).Value = datos.VueloSalida;
                        worksheet.Cell(nRow, 8).Value = datos.TarifaCOP;
                        worksheet.Cell(nRow, 9).Value = datos.Cantidad;
                        worksheet.Cell(nRow, 10).Value = datos.CobroCOP;
                        worksheet.Cell(nRow, 11).Value = datos.VrComisionCOP;                     
                        nRow++;
                    }
                    // Se agrega el total de cobros generados
                    worksheet.Cell(nRow, 8).Value = SumarColumna1(Anexo3, Tipo);
                    worksheet.Cell(nRow, 8).Style.Font.Bold = true;

                    worksheet.Cell(nRow, 9).Value = SumarColumna2(Anexo3, Tipo);
                    worksheet.Cell(nRow, 9).Style.Font.Bold = true;

                    worksheet.Cell(nRow, 10).Value = SumarColumna3(Anexo3, Tipo);
                    worksheet.Cell(nRow, 10).Style.Font.Bold = true;

                    worksheet.Cell(nRow, 11).Value = SumarColumna4(Anexo3, Tipo);
                    worksheet.Cell(nRow, 11).Style.Font.Bold = true;

                    worksheet.Cell(nRow, 1).Value = "Totales";

                    worksheet.Range("A" + nRow + ":K" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

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
