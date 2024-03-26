using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeResumenDetalleTasasAeroportuarias
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
        public int SumarTotalPOSCobro(List<Anexo7B> Anexo7B, string TipoCobro)
        {
            int TotalPOS = 0;
            int TryParsePOS = 0;
            bool ValidarTryParsePOS = false;
            try
            {
                if (Anexo7B.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo7B)
                        {
                            ValidarTryParsePOS = Int32.TryParse(item.TarifaCOP, out TryParsePOS);
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

        /// <summary>
        /// Metodo que se encarga de sumar el total de cobró ya sea "COP" || "USD"
        /// </summary>
        /// <param name="Anexo1"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>TotalCobro en base a la suma y validacion del cobró</returns>
        public int SumarTotalCobro(List<Anexo7B> Anexo7B, string TipoCobro)
        {
            int TotalCobro = 0;
            int TryParseCobro = 0;
            bool ValidarTryParseCobro = false;
            try
            {

                if (Anexo7B.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP") || TipoCobro.Equals("NAL"))
                        foreach (var item in Anexo7B)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.CobroCOP, out TryParseCobro);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryParseCobro;
                        }
                    else if (TipoCobro.Equals("USD") || TipoCobro.Equals("INT"))
                        foreach (var item in Anexo7B)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.CobroUSD, out TryParseCobro);
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
        public byte[] ArmarExcel(List<Anexo7B> Anexo7B, string TipoCobro, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty;
            int TotalCobro = 0;
            
            int TotalPosCobro = 0;
            try
            {
                //Se Valida el tipocobro y la suma del TotalCobro,TotalCantidad,TotalPosCobro ya sea "COP" || "USD"
                ValueTotal = ValidarTipoCobro(TipoCobro);
                TotalCobro = SumarTotalCobro(Anexo7B, TipoCobro);
                TotalPosCobro = SumarTotalPOSCobro(Anexo7B, TipoCobro);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo7B");
                    //Generamos la cabecera
                    if (TipoCobro.Equals("NAL"))
                    {
                        worksheet.Range("A1:O1").Merge().Value = "";
                        worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:I2").Merge().Value = "Anexo 7 Resumen Detalle Tasas Aeroportuarias - Jarvis Informe";
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
                    else if (TipoCobro.Equals("INT"))
                    {
                        worksheet.Range("A1:N1").Merge().Value = "";
                        worksheet.Range("A1:N1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:H2").Merge().Value = "Anexo 7 Resumen Detalle Tasas Aeroportuarias - Jarvis Informe";
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
                    worksheet.Cell("A6").Value = "Sigla";
                    worksheet.Cell("B6").Value = "NIT";
                    worksheet.Cell("C6").Value = "Nombre Aerolínea";
                    worksheet.Cell("D6").Value = "Fecha Vuelo";
                    worksheet.Cell("E6").Value = "Matrícula";
                    worksheet.Cell("F6").Value = "Número Vuelo";
                    if (TipoCobro.Equals("NAL"))
                    {
                        worksheet.Cell("G6").Value = "Tarifa COP";
                        worksheet.Cell("H6").Value = "Numeros cop";
                        worksheet.Cell("I6").Value = "Cobrocop";
                    }
                    else
                    {
                        worksheet.Cell("G6").Value = "Tarifa USD";
                        worksheet.Cell("H6").Value = "Numeros USD";
                        worksheet.Cell("I6").Value = "Cobro USD";
                    }
                    worksheet.Cell("J6").Value = "Tipo Vuelo";
                    worksheet.Cell("K6").Value = "Factura";
                    worksheet.Cell("L6").Value = "Tipo Factura";


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

                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo7B)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.Sigla;
                        worksheet.Cell(nRow, 2).Value = datos.NIT;
                        worksheet.Cell(nRow, 3).Value = datos.NombreAerolinea;
                        worksheet.Cell(nRow, 4).Value = "'" + datos.FechaVuelo;
                        worksheet.Cell(nRow, 5).Value = datos.Matricula;
                        worksheet.Cell(nRow, 6).Value = datos.NumeroVuelo;
                        if (TipoCobro.Equals("NAL"))
                        {
                            worksheet.Cell(nRow, 7).Value = datos.TarifaCOP;
                            worksheet.Cell(nRow, 8).Value = datos.NumNormales;
                            worksheet.Cell(nRow, 9).Value = datos.CobroCOP;
                        }
                        else
                        {
                            worksheet.Cell(nRow, 7).Value = datos.TarifaUSD;
                            worksheet.Cell(nRow, 8).Value = datos.NumNormalesUSD;
                            worksheet.Cell(nRow, 9).Value = datos.CobroUSD;
                        }

                        worksheet.Cell(nRow, 10).Value = datos.TipoVuelo;
                        worksheet.Cell(nRow, 11).Value = datos.Factura;
                        worksheet.Cell(nRow, 12).Value = datos.TipoFactura;


                        nRow++;
                    }
                    // Se agrega el total de cobros generados

                    //worksheet.Cell(nRow, 7).Value = TotalPosCobro;
                    //worksheet.Cell(nRow, 7).Style.Font.Bold = true;
        
                    worksheet.Cell(nRow, 9).Value = TotalCobro;
                    worksheet.Cell(nRow, 9).Style.Font.Bold = true;



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
