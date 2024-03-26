using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeUsoCarroBomberos
    {
        #region "Descargar Excel"
        /// <summary>
        /// Metodo para validar el tipo de cobro y asu vez colocar los valores correspondientes en la cabecera del excel.
        /// </summary>
        /// <param name="Cobro"></param>
        /// <returns>"TotalCOP" || "TotalUSD" en base a la validacion</returns>

        public Decimal SumarTarifa(List<Anexo8> Anexo8)
        {
            Decimal TotalTarifa = 0;
            Decimal TryParsePOS = 0;
            bool ValidarTryParsePOS = false;
            try
            {
                if (Anexo8.Count > 0)
                {
                    foreach (var item in Anexo8)
                    {
                        ValidarTryParsePOS = Decimal.TryParse(item.Tarifa, out TryParsePOS);
                        if (ValidarTryParsePOS)
                            TotalTarifa = TotalTarifa + TryParsePOS;
                    }

                }
                return TotalTarifa;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalTarifa;
            }
        }

        /// <summary>
        /// Metodo que se encarga de sumar el total de cobró ya sea "COP" || "USD"
        /// </summary>
        /// <param name="Anexo8"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>TotalCobro en base a la suma y validacion del cobró</returns>
        public Decimal SumarTotalCobro(List<Anexo8> Anexo8)
        {
            Decimal TotalCobro = 0;
            Decimal TryParseCobro = 0;
            bool ValidarTryParseCobro = false;
            try
            {

                if (Anexo8.Count > 0)
                {

                    foreach (var item in Anexo8)
                    {
                        ValidarTryParseCobro = Decimal.TryParse(item.ValorCobroCOP, out TryParseCobro);
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
        public byte[] ArmarExcel(List<Anexo8> Anexo8, string TipoCobro, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty;
            Decimal TotalTarifa = 0;
            Decimal TotalPosCobro = 0;
            try
            {
                //Se Valida el tipocobro y la suma del TotalCobro,TotalCantidad,TotalPosCobro ya sea "COP" || "USD"

                TotalTarifa = SumarTarifa(Anexo8);
                TotalPosCobro = SumarTotalCobro(Anexo8);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo8");
                    //Generamos la cabecera
                    worksheet.Range("A1:G1").Merge().Value = "";
                    worksheet.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:E2").Merge().Value = "Anexo 8. Uso Carro Bomberos";
                    worksheet.Range("D2:E2").Style.Font.Bold = true;
                    worksheet.Range("A2:G2").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:E2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("D3:E3").Merge().Value = filtro1;
                    worksheet.Range("A3:G3").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D3:E3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("D4:E4").Merge().Value = filtro2;
                    worksheet.Range("A4:G4").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D4:E4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("D5:E5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    worksheet.Range("A5:G5").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D5:E5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;



                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A1")).Scale(0.6);
                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("F2")).Scale(0.5);

                    //worksheet.Range("A1:O1").Merge().Value = "Uso Carro Bomberos - Jarvis Informe";
                    //worksheet.Range("A1:O1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    //worksheet.Range("A1:O1").Style.Font.Bold = true;
                    //worksheet.Range("A2:G5").Merge();
                    //worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A3")).Scale(0.5);
                    //worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("E3")).Scale(0.5);


                    worksheet.Cell("A6").Value = "Prefijo";
                    worksheet.Cell("B6").Value = "Factura";
                    worksheet.Cell("C6").Value = "Aerolinea";
                    worksheet.Cell("D6").Value = "Matricula";
                    worksheet.Cell("E6").Value = "Tipo de servicio";
                    worksheet.Cell("F6").Value = "Fecha de Servicio";
                    worksheet.Cell("G6").Value = "Precio Unit";
                    worksheet.Cell("H6").Value = "Total";

                    #region Diseño Hoja
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
                    #endregion

                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo8)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.PrefijoFactura;
                        worksheet.Cell(nRow, 2).Value = datos.Factura;
                        worksheet.Cell(nRow, 3).Value = datos.NombreAerolinea;
                        worksheet.Cell(nRow, 4).Value = datos.Matricula;
                        worksheet.Cell(nRow, 5).Value = datos.TipodeServicio;
                        worksheet.Cell(nRow, 6).Value = "'" + datos.FechaServicio;
                        worksheet.Cell(nRow, 7).Value = datos.Tarifa;
                        worksheet.Cell(nRow, 8).Value = datos.ValorCobroCOP;
                        nRow++;
                    }
                    // Se agrega el total de cobros generados

                    //worksheet.Cell(nRow, 6).Value = TotalTarifa.ToString();
                    //worksheet.Cell(nRow, 6).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 8).Value = TotalPosCobro.ToString();
                    worksheet.Cell(nRow, 8).Style.Font.Bold = true;

                    worksheet.Cell(nRow, 1).Value = "Totales";

                    worksheet.Range("A" + nRow + ":H" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

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
