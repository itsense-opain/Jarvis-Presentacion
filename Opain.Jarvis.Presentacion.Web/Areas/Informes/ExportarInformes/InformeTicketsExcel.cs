using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeTicketsExcel
    {
        #region "Descargar Excel"

        public byte[] ArmarExcel(List<InformeTickets> listatickets, string filtro1)
        {
            string ValueTotal = string.Empty;

            try
            {

                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("InformeTickets");

                    worksheet.Range("A1:O1").Merge().Value = "";
                    worksheet.Range("A1:O1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Merge().Value = "Informe de tickets";
                    worksheet.Range("D2:I2").Style.Font.Bold = true;
                    worksheet.Range("A2:O2").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D2:I2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("D3:I3").Merge().Value = filtro1;
                    worksheet.Range("A3:O3").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D3:I3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;



                    worksheet.Range("D5:I5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    worksheet.Range("A5:O5").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("D5:I5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("M2")).Scale(0.6);


                    worksheet.Cell("A6").Value = "Número Ticket";
                    worksheet.Cell("B6").Value = "Tipo Ticket";
                    worksheet.Cell("C6").Value = "Estado Inicial";
                    worksheet.Cell("D6").Value = "Fecha Solicitud";
                    worksheet.Cell("E6").Value = "Hora Solicitud";
                    worksheet.Cell("F6").Value = "Número Vuelo y/o Factura ";
                    worksheet.Cell("G6").Value = "Asunto";
                    worksheet.Cell("H6").Value = "Aerolínea";
                    worksheet.Cell("I6").Value = "Usuario Aerolínea";
                    worksheet.Cell("J6").Value = "Documento Adjunto";
                    worksheet.Cell("K6").Value = "Fecha Respuesta";
                    worksheet.Cell("L6").Value = "Hora Respuesta";
                    worksheet.Cell("M6").Value = "Respuesta";
                    worksheet.Cell("N6").Value = "Usuario Opain";
                    worksheet.Cell("O6").Value = "Adjunto Respuesta";
                    worksheet.Cell("P6").Value = "Estado Final";



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
                    worksheet.Cell("P6").Style.Font.Bold = true;
                    worksheet.Cell("P6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);



                    //-----------Genero la tabla de datos-----------
                    int nRow = 7; //Indicamos el valor en la celda nRow, 7    
                    foreach (var datos in listatickets)
                    {
                        worksheet.Cell(nRow, 1).Value = datos.NumeroTicket;
                        worksheet.Cell(nRow, 2).Value = datos.TipoTicket;
                        worksheet.Cell(nRow, 3).Value = "Abierto";
                        worksheet.Cell(nRow, 4).Value = datos.FechaSolicitud.Day.ToString().PadLeft(2, '0') + "/" + datos.FechaSolicitud.Month.ToString().PadLeft(2, '0') + "/" + datos.FechaSolicitud.Year.ToString();
                        worksheet.Cell(nRow, 5).Value = datos.FechaSolicitud.Hour.ToString().PadLeft(2, '0') + ":" + datos.FechaSolicitud.Minute.ToString().PadLeft(2, '0') + ":" + datos.FechaSolicitud.Second.ToString();
                        worksheet.Cell(nRow, 6).Value = datos.Depende;
                        worksheet.Cell(nRow, 7).Value = datos.Asunto;
                        worksheet.Cell(nRow, 8).Value = datos.Aerolinea;
                        worksheet.Cell(nRow, 9).Value = datos.Usuario;
                        worksheet.Cell(nRow, 10).Value = datos.Adjunto;
                        if (datos.FechaRespuesta.Year == 1)
                        {
                            worksheet.Cell(nRow, 11).Value = "";
                            worksheet.Cell(nRow, 12).Value = "";
                        }
                        else
                        {
                            worksheet.Cell(nRow, 11).Value = datos.FechaRespuesta.Day.ToString().PadLeft(2, '0') + "/" + datos.FechaRespuesta.Month.ToString().PadLeft(2, '0') + "/" + datos.FechaRespuesta.Year.ToString();
                            worksheet.Cell(nRow, 12).Value = datos.FechaRespuesta.Hour.ToString().PadLeft(2, '0') + ":" + datos.FechaRespuesta.Minute.ToString().PadLeft(2, '0') + ":" + datos.FechaRespuesta.Second.ToString();

                        }

                        worksheet.Cell(nRow, 13).Value = datos.Respuesta;
                        worksheet.Cell(nRow, 14).Value = datos.UsuarioRespuesta;

                        if (datos.FechaRespuesta.Year == 1)
                        {
                            worksheet.Cell(nRow, 15).Value = "";
                        }
                        else
                        {
                            worksheet.Cell(nRow, 15).Value = datos.AdjuntoRespuesta;

                        }
                        worksheet.Cell(nRow, 16).Value = datos.EstadoFinal;


                        nRow++;
                    }

                    string celda = "A" + nRow + ":E" + nRow;
                    var rango = worksheet.Range(celda).Merge();
                    rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(nRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 1).Value = "Total General ";
                    worksheet.Cell(nRow, 1).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Cell(nRow, 1).Style.Font.FontColor = XLColor.White;

                    string celda2 = "F" + nRow + ":p" + nRow;
                    var rango2 = worksheet.Range(celda2).Merge();
                    rango2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell(nRow, 6).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 6).Value = listatickets.Count;
                    worksheet.Cell(nRow, 6).Style.Fill.BackgroundColor = XLColor.Black;
                    worksheet.Cell(nRow, 6).Style.Font.FontColor = XLColor.White;
                    // Se agrega el total de cobros generados


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
