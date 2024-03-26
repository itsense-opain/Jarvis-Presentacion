using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeInformeCarnetizacion
    {
        #region "Descargar Excel"

        /// <summary>
        /// Metodo para generar y descargar el excel Anexo1
        /// </summary>
        /// <param name="Anexo19"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>File Excel</returns>
        public byte[] ArmarExcel(List<Anexo19> Anexo19, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty; 
            try
            {

                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo19");
                    //Generamos la cabecera

                    worksheet.Range("A1:O1").Merge().Value = "";
                    worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Merge().Value = "InformeCarnetizacion - Jarvis Informe";
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

                    worksheet.Cell("A6").Value = "Fecha Auditoria";
                    worksheet.Cell("B6").Value = "Documento Factura";
                    worksheet.Cell("C6").Value = "Número Factura";
                    worksheet.Cell("D6").Value = "Documento Pago";
                    worksheet.Cell("E6").Value = "Número Pago";
                    worksheet.Cell("F6").Value = "Radicado";
                    worksheet.Cell("G6").Value = "Cliente";
                    worksheet.Cell("H6").Value = "Tipo carné";
                    worksheet.Cell("I6").Value = "ID Quien Recibe";
                    worksheet.Cell("J6").Value = "Nombre Quien Recibe";
                    worksheet.Cell("K6").Value = "Entregados";
                    worksheet.Cell("L6").Value = "Valor";
                    worksheet.Cell("M6").Value = "Estado Impresión";
                    worksheet.Cell("N6").Value = "Tipo Radicado";

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



                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in Anexo19)
                    {
                        worksheet.Cell(nRow, 1).Value = "'" + datos.FechaAuditoria;
                        worksheet.Cell(nRow, 2).Value = datos.DocumentoFactura;
                        worksheet.Cell(nRow, 3).Value = datos.NumeroFactura;
                        worksheet.Cell(nRow, 4).Value = datos.DocumentoPago;
                        worksheet.Cell(nRow, 5).Value = datos.NumeroPago;
                        worksheet.Cell(nRow, 6).Value = datos.Radicado;
                        worksheet.Cell(nRow, 7).Value = datos.Cliente;
                        worksheet.Cell(nRow, 8).Value = datos.TipoCarnet;
                        worksheet.Cell(nRow, 9).Value = datos.IdQuienRecibe;
                        worksheet.Cell(nRow, 10).Value = datos.NombreQuienRecibe;
                        worksheet.Cell(nRow, 11).Value = datos.Entregados;
                        worksheet.Cell(nRow, 12).Value = datos.Valor; 
                        worksheet.Cell(nRow, 13).Value = datos.EstadoImpresion;
                        worksheet.Cell(nRow, 14).Value = datos.TipoRadicado;
                        nRow++;
                    } 
                     
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
