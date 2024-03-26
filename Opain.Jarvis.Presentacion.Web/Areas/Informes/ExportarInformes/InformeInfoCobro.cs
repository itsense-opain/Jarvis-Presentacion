using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeInfoCobro
    {
        #region "Descargar Excel"

        public int SumarTotalCantidad(List<InformeCobro> infoCobro)
        {
            int TotalCantidad = 0;
            //int TryParseCantidad = 0;
            //bool ValidarTryParseCantidad = false;
            try
            {
                //if (Anexo1.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                //{
                //    if (TipoCobro.Equals("COP"))
                //        foreach (var item in Anexo1)
                //        {

                //            ValidarTryParseCantidad = Int32.TryParse(item.Cantidad, out TryParseCantidad);
                //            if (ValidarTryParseCantidad)
                //                TotalCantidad = TotalCantidad + TryParseCantidad;
                //        }
                //    else if (TipoCobro.Equals("USD"))
                //        foreach (var item in Anexo1)
                //        {

                //            ValidarTryParseCantidad = Int32.TryParse(item.Cantidad, out TryParseCantidad);
                //            if (ValidarTryParseCantidad)
                //                TotalCantidad = TotalCantidad + TryParseCantidad;
                //        }
                //}
                return TotalCantidad;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalCantidad;
            }
        }

        public byte[] ArmarExcel(List<InformeCobro> informeCobro, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty;
            //int TotalCobro = 0;
            int TotalCantidad = 0;
            //int TotalPosCobro = 0;
            try
            {
                TotalCantidad = SumarTotalCantidad(informeCobro);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Informe Cobro");
                    //Generamos la cabecera
                    #region Cabecera del informe
                    worksheet.Range("A1:R1").Merge().Value = "";
                    worksheet.Range("A1:R1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Merge().Value = "Informe de cobro";
                    worksheet.Range("D2:I2").Style.Font.Bold = true;
                    worksheet.Range("A2:R2").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("D3:I3").Merge().Value = filtro1;
                    worksheet.Range("A3:R3").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D3:I3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("D4:I4").Merge().Value = filtro2;
                    worksheet.Range("A4:R4").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D4:I4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("D5:I5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    worksheet.Range("A5:R5").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D5:I5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("O2")).Scale(0.6);
                    #endregion

                    worksheet.Cell("A6").Value = "Fecha Salida";
                    worksheet.Cell("B6").Value = "Matrícula";
                    worksheet.Cell("C6").Value = "Tipo Vuelo";
                    worksheet.Cell("D6").Value = "Oaci";
                    worksheet.Cell("E6").Value = "Número Vuelo";
                    worksheet.Cell("F6").Value = "Tasas reportadas por la aerolínea";
                    worksheet.Cell("G6").Value = "Tasas cobradas por Opain";
                    worksheet.Cell("H6").Value = "Pasajeros";
                    worksheet.Cell("I6").Value = "Infantes";
                    worksheet.Cell("J6").Value = "Tránsitos en Línea";
                    worksheet.Cell("K6").Value = "Tránsitos en Conexión";
                    worksheet.Cell("L6").Value = "Exentos";
                    worksheet.Cell("M6").Value = "Tripulantes";
                    worksheet.Cell("N6").Value = "Total Diferencia en Tasas";
                    worksheet.Cell("O6").Value = "Concepto de cobro";
                    worksheet.Cell("P6").Value = "Usuario Aerolìnea";
                    worksheet.Cell("Q6").Value = "Fecha de Cargue del vuelo";
                    worksheet.Cell("R6").Value = "Hora de Cargue del vuelo";

                    ////-----------Le damos el formato a la cabecera----------------
                    #region Estilo de titulos de columna
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
                    worksheet.Cell("R6").Style.Font.Bold = true;
                    worksheet.Cell("R6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    #endregion
                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7   
                 
                    for (int i = 0; i < informeCobro.Count - 1; i++)
                    {
                        int filaactual = nRow;
                        int filafinal = nRow + informeCobro[i].lstNovedades.Count;
                        worksheet.Range("A" + filaactual.ToString() + ":A" + filafinal.ToString()).Merge().Value = informeCobro[i].FechaSalida;
                        worksheet.Range("B" + filaactual.ToString() + ":B" + filafinal.ToString()).Merge().Value = informeCobro[i].Matricula;
                        worksheet.Range("C" + filaactual.ToString() + ":C" + filafinal.ToString()).Merge().Value = informeCobro[i].TipoVuelo;
                        worksheet.Range("D" + filaactual.ToString() + ":D" + filafinal.ToString()).Merge().Value = informeCobro[i].Oaci;
                        worksheet.Range("E" + filaactual.ToString() + ":E" + filafinal.ToString()).Merge().Value = informeCobro[i].NumeroVuelo;
                        worksheet.Range("F" + filaactual.ToString() + ":F" + filafinal.ToString()).Merge().Value = informeCobro[i].TasasReportadas;
                        worksheet.Range("G" + filaactual.ToString() + ":G" + filafinal.ToString()).Merge().Value = informeCobro[i].TasasCobradas;
                        worksheet.Range("H" + filaactual.ToString() + ":H" + filafinal.ToString()).Merge().Value = informeCobro[i].Pasajeros;
                        worksheet.Range("I" + filaactual.ToString() + ":I" + filafinal.ToString()).Merge().Value = informeCobro[i].Infantes;
                        worksheet.Range("J" + filaactual.ToString() + ":J" + filafinal.ToString()).Merge().Value = informeCobro[i].Linea;

                        worksheet.Range("K" + filaactual.ToString() + ":K" + filafinal.ToString()).Merge().Value = informeCobro[i].TransitoConexion;
                        worksheet.Range("L" + filaactual.ToString() + ":L" + filafinal.ToString()).Merge().Value = informeCobro[i].exentos;
                        worksheet.Range("M" + filaactual.ToString() + ":M" + filafinal.ToString()).Merge().Value = informeCobro[i].tripulantes;
                        worksheet.Range("N" + filaactual.ToString() + ":N" + filafinal.ToString()).Merge().Value = informeCobro[i].DiferenciaTasas;

                        //worksheet.Cell(nRow, 1).Value = informeCobro[i].FechaSalida;
                        // worksheet.Cell(nRow, 2).Value = informeCobro[i].Matricula;
                        //worksheet.Cell(nRow, 3).Value = informeCobro[i].TipoVuelo;
                        //worksheet.Cell(nRow, 4).Value = informeCobro[i].Oaci;
                        //worksheet.Cell(nRow, 5).Value = informeCobro[i].NumeroVuelo;
                        //worksheet.Cell(nRow, 6).Value = informeCobro[i].TasasReportadas;
                        //worksheet.Cell(nRow, 7).Value = informeCobro[i].TasasCobradas;
                        //worksheet.Cell(nRow, 8).Value = informeCobro[i].Pasajeros;
                        //worksheet.Cell(nRow, 9).Value = informeCobro[i].Infantes;
                        //worksheet.Cell(nRow, 10).Value = informeCobro[i].Linea;
                        //worksheet.Cell(nRow, 11).Value = informeCobro[i].TransitoConexion;
                        //worksheet.Cell(nRow, 12).Value = informeCobro[i].exentos;
                        //worksheet.Cell(nRow, 13).Value = informeCobro[i].tripulantes;
                        //worksheet.Cell(nRow, 14).Value = informeCobro[i].DiferenciaTasas;
                      
                        string novedades = "";
                        foreach (var item in informeCobro[i].lstNovedades)
                        {
                            novedades = novedades + item.ToString() + ","; 
                        }
                        worksheet.Range("O" + filaactual.ToString() + ":O" + filafinal.ToString()).Merge().Value = novedades;
                        worksheet.Range("O" + filaactual.ToString() + ":O" + filafinal.ToString()).Style.Alignment.WrapText = true;

                        //worksheet.Cell(nRow, 15).Value = "Aqui va el concepto";
                        worksheet.Range("P" + filaactual.ToString() + ":P" + filafinal.ToString()).Merge().Value = informeCobro[i].Usuario;
                        //worksheet.Cell(nRow, 16).Value = informeCobro[i].Usuario;
                        if (informeCobro[i].fechaCargue != DateTime.MinValue)
                        {
                            worksheet.Range("Q" + filaactual.ToString() + ":Q" + filafinal.ToString()).Merge().Value = informeCobro[i].fechaCargue.Day.ToString().PadLeft(2, '0') + "/" + informeCobro[i].fechaCargue.Month.ToString().PadLeft(2, '0') + "/" + informeCobro[i].fechaCargue.Year.ToString();
                            worksheet.Range("R" + filaactual.ToString() + ":R" + filafinal.ToString()).Merge().Value = informeCobro[i].fechaCargue.Hour.ToString().PadLeft(2, '0') + ":" + informeCobro[i].fechaCargue.Minute.ToString().PadLeft(2, '0') + ":" + informeCobro[i].fechaCargue.Second.ToString().PadLeft(2, '0');
                            //worksheet.Cell(nRow, 17).Value = informeCobro[i].fechaCargue.Day.ToString().PadLeft(2, '0') + "/" + informeCobro[i].fechaCargue.Month.ToString().PadLeft(2, '0') + "/" + informeCobro[i].fechaCargue.Year.ToString();
                            //worksheet.Cell(nRow, 18).Value = informeCobro[i].fechaCargue.Hour.ToString().PadLeft(2, '0') + ":" + informeCobro[i].fechaCargue.Minute.ToString().PadLeft(2, '0') + ":" + informeCobro[i].fechaCargue.Second.ToString().PadLeft(2, '0');
                        }
                        else
                        {
                            worksheet.Range("Q" + filaactual.ToString() + ":Q" + filafinal.ToString()).Merge().Value = "";
                            worksheet.Range("R" + filaactual.ToString() + ":R" + filafinal.ToString()).Merge().Value = "";
                            //worksheet.Cell(nRow, 17).Value = "";
                            //worksheet.Cell(nRow, 18).Value = "";
                        }

                        nRow = filafinal + 1;

                        //nRow++;
                    }

                    worksheet.Cell(nRow, 1).Value = "";
                    worksheet.Cell(nRow, 2).Value = informeCobro[informeCobro.Count - 1].Matricula;
                    worksheet.Cell(nRow, 3).Value = informeCobro[informeCobro.Count - 1].TipoVuelo;
                    worksheet.Cell(nRow, 4).Value = informeCobro[informeCobro.Count - 1].Oaci;
                    worksheet.Cell(nRow, 5).Value = informeCobro[informeCobro.Count - 1].NumeroVuelo;
                    worksheet.Cell(nRow, 6).Value = informeCobro[informeCobro.Count - 1].TasasReportadas;
                    worksheet.Cell(nRow, 7).Value = informeCobro[informeCobro.Count - 1].TasasCobradas;
                    worksheet.Cell(nRow, 8).Value = informeCobro[informeCobro.Count - 1].Pasajeros;
                    worksheet.Cell(nRow, 9).Value = informeCobro[informeCobro.Count - 1].Infantes;
                    worksheet.Cell(nRow, 10).Value = informeCobro[informeCobro.Count - 1].Linea;
                    worksheet.Cell(nRow, 11).Value = informeCobro[informeCobro.Count - 1].TransitoConexion;
                    worksheet.Cell(nRow, 12).Value = informeCobro[informeCobro.Count - 1].exentos;
                    worksheet.Cell(nRow, 13).Value = informeCobro[informeCobro.Count - 1].tripulantes;
                    worksheet.Cell(nRow, 14).Value = informeCobro[informeCobro.Count - 1].DiferenciaTasas;
                    worksheet.Cell(nRow, 15).Value = informeCobro[informeCobro.Count - 1].Concepto;
                    worksheet.Cell(nRow, 16).Value = informeCobro[informeCobro.Count - 1].Usuario;
                    worksheet.Cell(nRow, 17).Value = "";
                    worksheet.Cell(nRow, 18).Value = "";
                    worksheet.Range("A" + nRow + ":R" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

                    worksheet.Row(nRow).Style.Font.FontColor = XLColor.White;
                    // Se agrega el total de cobros generados
                    //worksheet.Cell(nRow, 11).Value = TotalCantidad;
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
                    //    worksheet.Cell(nRow, 14).Value = TotalCobro;
                    //worksheet.Cell(nRow, 14).Style.Font.Bold = true;
                    ////Le damos estilos a los totales sumados
                    //worksheet.Cell(nRow, 11).Style.Font.Bold = true;
                    //worksheet.Cell(nRow, 1).Value = "Totales";



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
