using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeResumenTasasAeroportuariasFacturadas
    {
        #region "Descargar Excel"
        /// <summary>
        /// Metodo para validar el tipo de cobro y asu vez colocar los valores correspondientes en la cabecera del excel.
        /// </summary>
        /// <param name="Cobro"></param>
        /// <returns>"TotalCOP" || "TotalUSD" en base a la validacion</returns>

        public Decimal SumarTotalPOSCobro(List<Anexo10> Anexo10)
        {
            Decimal TotalPOS = 0;
            Decimal TryParsePOS = 0;
            bool ValidarTryParsePOS = false;
            try
            {
                if (Anexo10.Count > 0)
                {

                    foreach (var item in Anexo10)
                    {
                        ValidarTryParsePOS = Decimal.TryParse(item.Total, out TryParsePOS);
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
        public byte[] ArmarExcel(List<Anexo10> Anexo10,string filtro1,string filtro2)
        {
            string ValueTotal = string.Empty;
            decimal TotalPosCobro = 0;
            try
            {
                //Se Valida el tipocobro y la suma del TotalCobro,TotalCantidad,TotalPosCobro ya sea "COP" || "USD"

                TotalPosCobro = SumarTotalPOSCobro(Anexo10);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo10");
                    //Generamos la cabecera

                    worksheet.Range("A1:O1").Merge().Value = "";
                    worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("C2:H2").Merge().Value = "Resumen Tasas Aeroportuarias Facturadas - Jarvis Informe";
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

                    //worksheet.Range("A1:O1").Merge().Value = "Resumen Tasas Aeroportuarias Facturadas - Jarvis Informe";
                    //worksheet.Range("A1:O1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    //worksheet.Range("A1:O1").Style.Font.Bold = true;
                    //worksheet.Range("A2:O5").Merge();
                    //worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("A3")).Scale(0.5);
                    //worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("L3")).Scale(0.5);


                    worksheet.Cell("A6").Value = "NIT/CEDULA";
                    worksheet.Cell("B6").Value = "Nombre de Tercero";
                    worksheet.Cell("C6").Value = "Valor";
                    worksheet.Cell("D6").Value = "Nota Credito";
                    worksheet.Cell("E6").Value = "Total";


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


                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo10)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.NIT_CEDULA;
                        worksheet.Cell(nRow, 2).Value = datos.NombredeTercero;
                        worksheet.Cell(nRow, 3).Value = datos.Valor;
                        worksheet.Cell(nRow, 4).Value = datos.NotaCredito;
                        worksheet.Cell(nRow, 5).Value = datos.Total;
                       
                        nRow++;
                    }
                    // Se agrega el total de cobros generados
                    
                      worksheet.Cell(nRow, 5).Value = TotalPosCobro;
                        worksheet.Cell(nRow, 5).Style.Font.Bold = true; 
                  
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
