using System;
using System.Collections.Generic;
using Opain.Jarvis.Dominio.Entidades;
using System.IO;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeParqueAeronaves
    {
        /// <summary>
        /// Metodo para validar el tipo de cobró y asu vez colocar los valores correspondientes en la cabecera del excel.
        /// </summary>
        /// <param name="Cobro"></param>
        /// <returns>"TotalCOP" || "TotalUSD" en base a la validacion</returns>
        public string ValidarTipo(string Tipo)
        {
            string CadenaRetorno = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Tipo))
                {
                    if (Tipo.Equals("DOM_COP") || Tipo.Equals("INT_COP") || Tipo.Equals("TOT_COP"))
                        CadenaRetorno = "Total COP";
                    else if (Tipo.Equals("DOM_USD") || Tipo.Equals("INT_USD") || Tipo.Equals("TOT_USD"))
                        CadenaRetorno = "Total USD";
                }
                return CadenaRetorno;
            }
            catch (Exception ex)
            {
                CadenaRetorno = ex.Message;
                return CadenaRetorno;
            }

        }

        public int SumaTotalHoras(List<Anexo2> Anexo2, string Tipo)
        {
            int TotalHoras = 0;
            int TryParseHoras = 0;
            bool ValidarTryParseHoras = false;
            try
            {
                if (Anexo2.Count > 0 && !string.IsNullOrEmpty(Tipo))
                {
                    if (Tipo.Equals("DOM_COP") || Tipo.Equals("INT_COP") || Tipo.Equals("TOT_COP") || Tipo.Equals("DOM_USD") || Tipo.Equals("INT_USD") || Tipo.Equals("TOT_USD"))
                        foreach (var item in Anexo2)
                        {
                            ValidarTryParseHoras = Int32.TryParse(item.TotalHoras, out TryParseHoras);
                            if (ValidarTryParseHoras)
                                TotalHoras = TotalHoras + TryParseHoras;
                        }
                }
                return TotalHoras;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalHoras;
            }
        }

        public int SumaTotalCobro(List<Anexo2> Anexo2, string Tipo)
        {
            int TotalCobro = 0;
            int TryParseCobro = 0;
            bool ValidarTryParseCobro = false;
            try
            {
                if (Anexo2.Count > 0 && !string.IsNullOrEmpty(Tipo))
                {
                    if (Tipo.Equals("DOM_COP") || Tipo.Equals("INT_COP") || Tipo.Equals("TOT_COP"))
                    {
                        foreach (var item in Anexo2)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.CobroCOP, out TryParseCobro);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryParseCobro;
                        }
                    }
                    else if (Tipo.Equals("DOM_USD") || Tipo.Equals("INT_USD") || Tipo.Equals("TOT_USD"))
                    {
                        foreach (var item in Anexo2)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.CobroUSD, out TryParseCobro);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryParseCobro;
                        }
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
        public int SumaTotal(List<Anexo2> Anexo2, string Tipo)
        {
            int TotalSuma = 0;
            int TryParseSuma = 0;
            bool ValidarTryParseSuma = false;
            try
            {
                if (Anexo2.Count > 0 && !string.IsNullOrEmpty(Tipo))
                {
                    if (Tipo.Equals("DOM_COP") || Tipo.Equals("INT_COP") || Tipo.Equals("TOT_COP"))
                        foreach (var item in Anexo2)
                        {
                            ValidarTryParseSuma = Int32.TryParse(item.TotalCOP, out TryParseSuma);
                            if (ValidarTryParseSuma)
                                TotalSuma = TotalSuma + TryParseSuma;
                        }
                    else if (Tipo.Equals("DOM_USD") || Tipo.Equals("INT_USD") || Tipo.Equals("TOT_USD"))
                        foreach (var item in Anexo2)
                        {
                            ValidarTryParseSuma = Int32.TryParse(item.TotalUSD, out TryParseSuma);
                            if (ValidarTryParseSuma)
                                TotalSuma = TotalSuma + TryParseSuma;
                        }
                }
                return TotalSuma;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalSuma;
            }
        }
        public byte[] ArmarExcel(List<Anexo2> Anexo2, string Tipo, string filtro1, string filtro2)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo2");
                    //Generamos la cabecera
                    #region Cabecera del informe
                    if (Tipo.Equals("DOM_COP") || Tipo.Equals("INT_COP") || Tipo.Equals("TOT_COP"))
                    {
                        worksheet.Range("A1:P1").Merge().Value = "";
                        worksheet.Range("A1:P1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:I2").Merge().Value = "Anexo 2 Parqueo Aeronaves";
                        worksheet.Range("D2:I2").Style.Font.Bold = true;
                        worksheet.Range("A2:P2").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Range("D3:I3").Merge().Value = filtro1;
                        worksheet.Range("A3:P3").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D3:I3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Range("D4:I4").Merge().Value = filtro2;
                        worksheet.Range("A4:P4").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D4:I4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Range("D5:I5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                        worksheet.Range("A5:P5").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D5:I5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                        worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("J2")).Scale(0.6);                 
                    }
                    else if (Tipo.Equals("DOM_USD") || Tipo.Equals("INT_USD") || Tipo.Equals("TOT_USD"))
                    {
                        worksheet.Range("A1:P1").Merge().Value = "";
                        worksheet.Range("A1:P1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:I2").Merge().Value = "Anexo 2 Parqueo Aeronaves";
                        worksheet.Range("D2:I2").Style.Font.Bold = true;
                        worksheet.Range("A2:P2").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Range("D3:I3").Merge().Value = filtro1;
                        worksheet.Range("A3:P3").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D3:I3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Range("D4:I4").Merge().Value = filtro2;
                        worksheet.Range("A4:P4").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D4:I4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.Range("D5:I5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                        worksheet.Range("A5:P5").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D5:I5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                        worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("J2")).Scale(0.6);

                        //worksheet.Range("A1:P1").Merge().Value = "Puente Aeronaves - Jarvis Informe";
                        //worksheet.Range("A1:P1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        //worksheet.Range("A1:P1").Style.Font.Bold = true;
                        //worksheet.Range("A2:P5").Merge();
                        //worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A3")).Scale(0.5);
                        //worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("O3")).Scale(0.4);
                    }
                    #endregion
                    worksheet.Cell("A6").Value = "Prefijo";
                    worksheet.Cell("B6").Value = "Factura";
                    worksheet.Cell("C6").Value = "Matricula";
                    worksheet.Cell("D6").Value = "Sigla";
                    worksheet.Cell("E6").Value = "Aerolínea";
                    worksheet.Cell("F6").Value = "Vuelo Llegada";
                    worksheet.Cell("G6").Value = "Vuelo Salida";
                    worksheet.Cell("H6").Value = "Fecha Llegada";
                    worksheet.Cell("I6").Value = "Fecha Salida";
                    worksheet.Cell("J6").Value = "Hora Llegada";
                    worksheet.Cell("K6").Value = "Hora Salida";
                    worksheet.Cell("L6").Value = "Cantidad";
                    worksheet.Cell("M6").Value = "Categoría";
                    worksheet.Cell("N6").Value = "Posición";
                    worksheet.Cell("O6").Value = "Descripción";
                    worksheet.Cell("P6").Value = "Precio Unt.";
                    if (Tipo.Equals("DOM_COP") || Tipo.Equals("INT_COP") || Tipo.Equals("TOT_COP"))                    {
                       
                        worksheet.Cell("Q6").Value = ValidarTipo(Tipo);
                    }
                    else if (Tipo.Equals("DOM_USD") || Tipo.Equals("INT_USD") || Tipo.Equals("TOT_USD"))                    {
                       
                        worksheet.Cell("Q6").Value = ValidarTipo(Tipo);
                    }

                    ////-----------Le damos el formato a la cabecera----------------
                    #region Estilo de los títulos
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
                    foreach (var datos in Anexo2)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.PrefijoFactura;
                        worksheet.Cell(nRow, 2).Value = datos.Factura;
                        worksheet.Cell(nRow, 3).Value = datos.Matricula;
                        worksheet.Cell(nRow, 4).Value = datos.Sigla;
                        worksheet.Cell(nRow, 5).Value = datos.NombreAerolinea;
                        worksheet.Cell(nRow, 6).Value = datos.VueloIngreso;
                        worksheet.Cell(nRow, 7).Value = datos.VueloSalida;
                        worksheet.Cell(nRow, 8).Value = "'" + datos.FechaIngreso;
                        worksheet.Cell(nRow, 9).Value = "'" + datos.FechaSalida;
                        worksheet.Cell(nRow, 10).Value = "'" + datos.HoraIngreso.PadLeft(4, '0').Substring(0, 2) + ":" + datos.HoraIngreso.PadLeft(4, '0').Substring(2, 2);
                        worksheet.Cell(nRow, 11).Value = "'" + datos.HoraSalida.PadLeft(4, '0').Substring(0, 2) + ":" + datos.HoraSalida.PadLeft(4, '0').Substring(2, 2);
                        worksheet.Cell(nRow, 12).Value = datos.TotalHoras;
                        worksheet.Cell(nRow, 13).Value = datos.CA;
                        worksheet.Cell(nRow, 14).Value = datos.POS;
                        worksheet.Cell(nRow, 15).Value = datos.TipoConexion;                        
                        if (Tipo.Equals("DOM_COP") || Tipo.Equals("INT_COP") || Tipo.Equals("TOT_COP"))
                        {
                            worksheet.Cell(nRow, 16).Value = datos.CobroCOP;
                            worksheet.Cell(nRow, 17).Value = datos.TotalCOP;
                        }
                        else if (Tipo.Equals("DOM_USD") || Tipo.Equals("INT_USD") || Tipo.Equals("TOT_USD"))
                        {
                            worksheet.Cell(nRow, 16).Value = datos.CobroUSD;
                            worksheet.Cell(nRow, 17).Value = datos.TotalUSD;
                        }
                        nRow++;
                    }
                    // Se agrega el total de cobros generados
                    worksheet.Cell(nRow, 12).Value = SumaTotalHoras(Anexo2, Tipo);
                    worksheet.Cell(nRow, 12).Style.Font.Bold = true;
                    if (Tipo.Equals("DOM_COP") || Tipo.Equals("INT_COP") || Tipo.Equals("TOT_COP"))
                    {
                        //worksheet.Cell(nRow, 15).Value = SumaTotalCobro(Anexo2, Tipo);
                        //worksheet.Cell(nRow, 15).Style.Font.Bold = true;
                        worksheet.Cell(nRow, 17).Value = SumaTotal(Anexo2, Tipo);
                        worksheet.Cell(nRow, 17).Style.Font.Bold = true;
                    }
                    else if (Tipo.Equals("DOM_USD") || Tipo.Equals("INT_USD") || Tipo.Equals("TOT_USD"))
                    {
                        //worksheet.Cell(nRow, 15).Value = SumaTotalCobro(Anexo2, Tipo);
                        //worksheet.Cell(nRow, 15).Style.Font.Bold = true;
                        worksheet.Cell(nRow, 17).Value = SumaTotal(Anexo2, Tipo);
                        worksheet.Cell(nRow, 17).Style.Font.Bold = true;
                    }
                    worksheet.Cell(nRow,1).Value ="Totales";
                    
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
    }
}
