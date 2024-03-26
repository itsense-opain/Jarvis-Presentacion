using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeDetallesVuelosInterventoria
    {
        decimal TotalEmbarcados = 0;
        decimal TotalTransito = 0;
        decimal TotalTL = 0;
        decimal TotalTC = 0;
        decimal TotalEX = 0;
        decimal TotalINF = 0;
        Decimal TotalLocal = 0;
        Decimal TotalPagaTasa = 0;
        Decimal TotalCantidad = 0;
        #region "Descargar Excel"

        public decimal SumarTotales(List<Anexo6> Anexo6, string TipoCobro)
        {
            decimal TryParseCantidad = 0;
            bool ValidarTryParseCantidad = false;
            try
            {
                if (Anexo6.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {

                    foreach (var item in Anexo6)
                    {
                        if (TipoCobro.Equals("NAL"))
                                ValidarTryParseCantidad = Decimal.TryParse(item.PaganCOP, out TryParseCantidad);
                                if (ValidarTryParseCantidad)
                                    TotalCantidad = TotalCantidad + TryParseCantidad;
                             
                        else if (TipoCobro.Equals("INT"))
                             

                                ValidarTryParseCantidad = Decimal.TryParse(item.PaganUSD, out TryParseCantidad);
                                if (ValidarTryParseCantidad)
                                    TotalCantidad = TotalCantidad + TryParseCantidad;
                             

                        ValidarTryParseCantidad = decimal.TryParse(item.Embarcados, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalEmbarcados = TotalEmbarcados + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.Infantes, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalINF = TotalINF + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.Exento, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalEX = TotalEX + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.Local, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalLocal = TotalLocal + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.TransitoConexion, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalTC = TotalTC + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.TransitoLinea, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalTL = TotalTL + TryParseCantidad;
 
                        ValidarTryParseCantidad = Decimal.TryParse(item.Transito, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalTransito = TotalTransito + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.PaganTasa, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalPagaTasa = TotalPagaTasa + TryParseCantidad;
                    }

                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return 0;
            }
        }

        public byte[] ArmarExcel(List<Anexo6> Anexo6, string TipoCobro, string filtro1, string filtro2)
        {

            try
            {
                //Se Valida el tipocobro y la suma del TotalCobro,TotalCantidad,TotalPosCobro ya sea "COP" || "USD"
                decimal prueba = SumarTotales(Anexo6, TipoCobro);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo6");
                    //Generamos la cabecera
                    if (TipoCobro.Equals("NAL"))
                    {
                        worksheet.Range("A1:O1").Merge().Value = "";
                        worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:I2").Merge().Value = "Anexo 6 Detalle vuelos interventoria";
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
                    else if (TipoCobro.Equals("INT"))
                    {
                        worksheet.Range("A1:N1").Merge().Value = "";
                        worksheet.Range("A1:N1").Style.Fill.BackgroundColor = XLColor.White;
                        worksheet.Range("D2:H2").Merge().Value = "Anexo 6 Detalle vuelos interventoria";
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
                    worksheet.Cell("A6").Value = "NombreAerolínea";
                    worksheet.Cell("B6").Value = "Fecha Vuelo";
                    worksheet.Cell("C6").Value = "Matrícula";
                    worksheet.Cell("D6").Value = "Sigla";
                    worksheet.Cell("E6").Value = "CodigoVuelo";
                    worksheet.Cell("F6").Value = "TipoVuelo";
                    worksheet.Cell("G6").Value = "Embarcados";
                    worksheet.Cell("H6").Value = "Tránsito";
                    worksheet.Cell("I6").Value = "Tránsito Linea";
                    worksheet.Cell("J6").Value = "Tránsito Conexion";
                    worksheet.Cell("K6").Value = "Local";
                    worksheet.Cell("L6").Value = "Exentos";
                    worksheet.Cell("M6").Value = "Infantes";
                    worksheet.Cell("N6").Value = "Pagan tasa";
                    if (TipoCobro.Equals("NAL"))
                    {
                        worksheet.Cell("O6").Value = "PaganCop";

                    }
                    else
                        worksheet.Cell("O6").Value = "PaganUSD";

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


                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo6)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.NombreAerolinea;
                        worksheet.Cell(nRow, 2).Value = "'" + datos.FechaVuelo;
                        worksheet.Cell(nRow, 3).Value = datos.Matricula;
                        worksheet.Cell(nRow, 4).Value = datos.Sigla;
                        worksheet.Cell(nRow, 5).Value = datos.CodigoVuelo;
                        worksheet.Cell(nRow, 6).Value = datos.TipoVuelo;
                        worksheet.Cell(nRow, 7).Value = datos.Embarcados;
                        worksheet.Cell(nRow, 8).Value = datos.Transito;
                        worksheet.Cell(nRow, 9).Value = datos.TransitoLinea;
                        worksheet.Cell(nRow, 10).Value = datos.TransitoConexion;
                        worksheet.Cell(nRow, 11).Value = datos.Local;
                        worksheet.Cell(nRow, 12).Value = datos.Exento;
                        worksheet.Cell(nRow, 13).Value = datos.Infantes;
                        worksheet.Cell(nRow, 14).Value = datos.PaganTasa;
                        if (TipoCobro.Equals("NAL"))
                        {
                            worksheet.Cell(nRow, 15).Value = datos.PaganCOP;
                        }
                        else if (TipoCobro.Equals("INT"))
                            worksheet.Cell(nRow, 15).Value = datos.PaganUSD;
                        nRow++;
                    }

                    worksheet.Cell(nRow, 7).Value = TotalEmbarcados;
                    worksheet.Cell(nRow, 7).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 8).Value = TotalTransito;
                    worksheet.Cell(nRow, 8).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 9).Value = TotalTL;
                    worksheet.Cell(nRow, 9).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 10).Value = TotalTC;
                    worksheet.Cell(nRow, 10).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 11).Value = TotalLocal;
                    worksheet.Cell(nRow, 11).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 12).Value = TotalEX;
                    worksheet.Cell(nRow, 12).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 13).Value = TotalINF;
                    worksheet.Cell(nRow, 13).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 14).Value = TotalPagaTasa;
                    worksheet.Cell(nRow, 14).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 15).Value = TotalCantidad;
                    worksheet.Cell(nRow, 15).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 16).Value = 0;
                    worksheet.Cell(nRow, 16).Style.Font.Bold = true;

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
