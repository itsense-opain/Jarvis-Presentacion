using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeSoportesExenciones
    {
        #region "Descargar Excel"
        /// <summary>
        /// Metodo para validar el tipo de cobro y asu vez colocar los valores correspondientes en la cabecera del excel.
        /// </summary>
        /// <param name="Cobro"></param>
        /// <returns>"TotalCOP" || "TotalUSD" en base a la validacion</returns>

        //public Decimal SumarTotalPOSCobro(List<Anexo12> Anexo12)
        //{
        //    Decimal TotalPOS = 0;
        //    Decimal TryParsePOS = 0;
        //    bool ValidarTryParsePOS = false;
        //    try
        //    {
        //        if (Anexo12.Count > 0)
        //        {

        //            foreach (var item in Anexo12)
        //            {
        //                ValidarTryParsePOS = Decimal.TryParse(item., out TryParsePOS);
        //                if (ValidarTryParsePOS)
        //                    TotalPOS = TotalPOS + TryParsePOS;
        //            }

        //        }
        //        return TotalPOS;
        //    }
        //    catch (Exception ex)
        //    {
        //        return TotalPOS;
        //    }
        //}

        /// <summary>
        /// Metodo que se encarga de sumar el total de cobró ya sea "COP" || "USD"
        /// </summary>
        /// <param name="Anexo1"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>TotalCobro en base a la suma y validacion del cobró</returns>

        /// <summary>
        /// Metodo para generar y descargar el excel Anexo1
        /// </summary>
        /// <param name="Anexo1"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>File Excel</returns>
        public byte[] ArmarExcel(List<Anexo12> Anexo12, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty;
            //decimal TotalPosCobro = 0;
            try
            {
                //Se Valida el tipocobro y la suma del TotalCobro,TotalCantidad,TotalPosCobro ya sea "COP" || "USD"

                // TotalPosCobro = 0;
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo12");
                    //Generamos la cabecera
                    worksheet.Range("A1:O1").Merge().Value = "";
                    worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Merge().Value = "Soportes Exenciones - Jarvis Informe";
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


                    //worksheet.Range("A1:O1").Merge().Value = "Soportes Exenciones - Jarvis Informe";
                    //worksheet.Range("A1:O1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    //worksheet.Range("A1:O1").Style.Font.Bold = true;
                    //worksheet.Range("A2:O5").Merge();
                    //worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A3")).Scale(0.5);
                    //worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("L3")).Scale(0.5);


                    worksheet.Cell("A6").Value = "TI";
                    worksheet.Cell("B6").Value = "Exención";
                    worksheet.Cell("C6").Value = "Fecha Vuelo";
                    worksheet.Cell("D6").Value = "Cédula";
                    worksheet.Cell("E6").Value = "Pasajero";
                    worksheet.Cell("F6").Value = "Apellido 1";
                    worksheet.Cell("G6").Value = "Apellido 2";
                    worksheet.Cell("H6").Value = "Clase";
                    worksheet.Cell("I6").Value = "Destino";
                    worksheet.Cell("J6").Value = "Solicitante";
                    worksheet.Cell("K6").Value = "Aerolínea";
                    worksheet.Cell("L6").Value = "Estado";
                    worksheet.Cell("M6").Value = "Fecha Itinerada Vuelo";
                    worksheet.Cell("N6").Value = "# Vuelo";
                    worksheet.Cell("O6").Value = "Causal";


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
                    foreach (var datos in Anexo12)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.TI;
                        worksheet.Cell(nRow, 2).Value = datos.Extencion;
                        worksheet.Cell(nRow, 3).Value = "'" + datos.FechaItineradaVuelo;
                        worksheet.Cell(nRow, 4).Value = datos.Cedula;
                        worksheet.Cell(nRow, 5).Value = datos.Pasajero;
                        worksheet.Cell(nRow, 6).Value = datos.Apellido1;
                        worksheet.Cell(nRow, 7).Value = datos.Apellido2;
                        worksheet.Cell(nRow, 8).Value = datos.Clase;
                        worksheet.Cell(nRow, 9).Value = datos.Destino;
                        worksheet.Cell(nRow, 10).Value = datos.Solicitante;
                        worksheet.Cell(nRow, 11).Value = datos.Aerolinea;
                        worksheet.Cell(nRow, 12).Value = datos.Reportada;
                        worksheet.Cell(nRow, 13).Value = "'" + datos.FechaVuelo;
                        worksheet.Cell(nRow, 14).Value = datos.Vuelo;
                        worksheet.Cell(nRow, 15).Value = datos.Causal;
                        nRow++;
                    }
                    // Se agrega el total de cobros generados
                    
                      //worksheet.Cell(nRow, 5).Value = TotalPosCobro;
                      //  worksheet.Cell(nRow, 5).Style.Font.Bold = true; 
                  
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
