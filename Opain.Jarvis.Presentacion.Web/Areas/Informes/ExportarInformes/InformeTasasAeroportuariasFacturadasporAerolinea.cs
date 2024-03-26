using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeTasasAeroportuariasFacturadasporAerolinea
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
        public int SumarTotalPOSCobro(List<Anexo11> Anexo11, string TipoCobro)
        {
            int TotalPOS = 0;
            int TryParsePOS = 0;
            bool ValidarTryParsePOS = false;
            try
            {
                if (Anexo11.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo11)
                        {
                            ValidarTryParsePOS = Int32.TryParse(item.CobroCOP, out TryParsePOS);
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
        public int SumarTotalCobro(List<Anexo11> Anexo11, string TipoCobro)
        {
            int TotalCobro = 0;
            int TryParseCobro = 0;
            bool ValidarTryParseCobro = false;
            try
            {

                if (Anexo11.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("COP"))
                        foreach (var item in Anexo11)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.PaganCOP, out TryParseCobro);
                            if (ValidarTryParseCobro)
                                TotalCobro = TotalCobro + TryParseCobro;
                        }
                    else if (TipoCobro.Equals("USD"))
                        foreach (var item in Anexo11)
                        {
                            ValidarTryParseCobro = Int32.TryParse(item.PaganUSD, out TryParseCobro);
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
        public byte[] ArmarExcel(List<Anexo11> Anexo11, string TipoCobro, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty;
            int TotalCobro = 0;
            int TotalPosCobro = 0;
            try
            {
                //Se Valida el tipocobro y la suma del TotalCobro,TotalCantidad,TotalPosCobro ya sea "COP" || "USD"
                ValueTotal = ValidarTipoCobro(TipoCobro);
                TotalCobro = SumarTotalCobro(Anexo11, TipoCobro);
                TotalPosCobro = SumarTotalPOSCobro(Anexo11, TipoCobro);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo11");
                    //Generamos la cabecera
                    if (TipoCobro.Equals("COP"))
                    {
                        worksheet.Range("A1:O1").Merge().Value = "";
                        worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C2:H2").Merge().Value = "TasasAeroportuariasFacturadasporAerolinea - Jarvis Informe";
                        worksheet.Range("A2:O2").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C2:H2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("C3:H3").Merge().Value = filtro1;
                        worksheet.Range("A3:O3").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C3:H3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("C4:H4").Merge().Value = filtro2;
                        worksheet.Range("A4:O4").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C4:H4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("C5:H5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                        worksheet.Range("A5:O5").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C5:H5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A1")).Scale(0.6);
                        worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("I2")).Scale(0.6);
                    }
                    else if (TipoCobro.Equals("USD"))
                    {
                        worksheet.Range("A1:N1").Merge().Value = "";
                        worksheet.Range("A1:N1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C2:H2").Merge().Value = "TasasAeroportuariasFacturadasporAerolinea - Jarvis Informe";
                        worksheet.Range("A2:N2").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C2:H2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("C3:H3").Merge().Value = filtro1;
                        worksheet.Range("A3:N3").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C3:H3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("C4:H4").Merge().Value = filtro2;
                        worksheet.Range("A4:N4").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C4:H4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                        worksheet.Range("C5:H5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                        worksheet.Range("A5:N5").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("C5:H5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;



                        worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A1")).Scale(0.6);
                        worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("I2")).Scale(0.6);
                    }
                    worksheet.Cell("A6").Value = "Facturado a";
                    worksheet.Cell("B6").Value = "Número Vuelos";
                    worksheet.Cell("C6").Value = "Pasajeros";
                    worksheet.Cell("D6").Value = "Tránsito";
                    worksheet.Cell("E6").Value = "Local";
                    worksheet.Cell("F6").Value = "Tripulación";
                    worksheet.Cell("G6").Value = "Excentos";
                    worksheet.Cell("H6").Value = "Infantes";
                    worksheet.Cell("I6").Value = "Pagan Tasa";
                    if (TipoCobro.Equals("COP"))
                    {
                        worksheet.Cell("J6").Value = "Pagan COP";
                        worksheet.Cell("K6").Value = "Cobro COP";
                        worksheet.Cell("L6").Value = "VRCOP";
                    }
                    else
                    {
                        worksheet.Cell("J6").Value = "Pagan USD";
                        worksheet.Cell("K6").Value = "Cobro USD";
                        worksheet.Cell("L6").Value = "VRUSD";
                    }


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
                    foreach (var datos in Anexo11)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.FacturadoA;
                        worksheet.Cell(nRow, 2).Value = datos.NumeroVuelos;
                        worksheet.Cell(nRow, 3).Value = datos.Pasajeros;
                        worksheet.Cell(nRow, 4).Value = datos.Transito;
                        worksheet.Cell(nRow, 5).Value = datos.Local;
                        worksheet.Cell(nRow, 6).Value = datos.Tripulacion;
                        worksheet.Cell(nRow, 7).Value = datos.Excentos;
                        worksheet.Cell(nRow, 8).Value = datos.Infantes;
                        worksheet.Cell(nRow, 9).Value = datos.PaganTasa;


                        if (TipoCobro.Equals("COP"))
                        {
                            worksheet.Cell(nRow, 10).Value = datos.PaganCOP;
                            worksheet.Cell(nRow, 11).Value = datos.CobroCOP;
                            worksheet.Cell(nRow, 12).Value = datos.VRCOP;
                        }
                        else if (TipoCobro.Equals("USD"))
                        {
                            worksheet.Cell(nRow, 10).Value = datos.PaganUSD;
                            worksheet.Cell(nRow, 11).Value = datos.CobroUSD;
                            worksheet.Cell(nRow, 12).Value = datos.VRUSD;
                        }
                        nRow++;
                    }
                    // Se agrega el total de cobros generados


                    worksheet.Cell(nRow, 10).Value = TotalCobro;
                    worksheet.Cell(nRow, 10).Style.Font.Bold = true; 

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
