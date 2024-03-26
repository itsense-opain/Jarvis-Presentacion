using ClosedXML.Excel;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.IO;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes
{
    public class InformeDetallesVuelosInterventoriaInfrasa
    {
        Decimal TotalCantidad = 0;
        Decimal TotalInfantes = 0;
        Decimal TotalTripulacion = 0;
        Decimal TotalExentos = 0;
        Decimal TotalLocal = 0;
        Decimal TotalTC = 0;
        Decimal TotalTL = 0;
        Decimal TotalEmbarcados = 0;
        Decimal TotalPagaTasa = 0;
        Decimal TotalTransito = 0;
        Decimal TotalPaganCOP = 0;
        #region "Descargar Excel"

        public Decimal SumarTotalCantidad(List<Anexo6> Anexo6, string TipoCobro)
        {
            Decimal TryParseCantidad = 0;
            bool ValidarTryParseCantidad = false;
            try
            {
                if (Anexo6.Count > 0 && !string.IsNullOrEmpty(TipoCobro))
                {
                    if (TipoCobro.Equals("NAL"))
                        foreach (var item in Anexo6)
                        {

                            ValidarTryParseCantidad = Decimal.TryParse(item.PaganCOP, out TryParseCantidad);
                            if (ValidarTryParseCantidad)
                                TotalCantidad = TotalCantidad + TryParseCantidad;
                        }
                    else if (TipoCobro.Equals("INT"))
                        foreach (var item in Anexo6)
                        {

                            ValidarTryParseCantidad = Decimal.TryParse(item.PaganUSD, out TryParseCantidad);
                            if (ValidarTryParseCantidad)
                                TotalCantidad = TotalCantidad + TryParseCantidad;
                        }
                }

                if (Anexo6.Count > 0)
                {
                    foreach (var item in Anexo6)
                    {
                        ValidarTryParseCantidad = Decimal.TryParse(item.Infantes, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalInfantes = TotalInfantes + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.Exento, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalExentos = TotalExentos + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.Local, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalLocal = TotalLocal + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.TransitoConexion, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalTC = TotalTC + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.TransitoLinea, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalTL = TotalTL + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.Embarcados, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalEmbarcados = TotalEmbarcados + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.Tripulacion, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalTripulacion = TotalTripulacion + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.PaganTasa, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalPagaTasa = TotalPagaTasa + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.Transito, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalTransito = TotalTransito + TryParseCantidad;

                        ValidarTryParseCantidad = Decimal.TryParse(item.PaganCOP, out TryParseCantidad);
                        if (ValidarTryParseCantidad)
                            TotalPaganCOP = TotalPaganCOP + TryParseCantidad;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return TotalCantidad;
            }
        }

        //public Decimal SumarInfantes(List<Anexo6> Anexo6)
        //{
        //    Decimal TotalCantidad = 0;
        //    Decimal TryParseCantidad = 0;
        //    bool ValidarTryParseCantidad = false;
        //    try
        //    {
        //        if (Anexo6.Count > 0)
        //        {
        //            foreach (var item in Anexo6)
        //            {

        //                ValidarTryParseCantidad = Decimal.TryParse(item.Infantes, out TryParseCantidad);
        //                if (ValidarTryParseCantidad)
        //                    TotalCantidad = TotalCantidad + TryParseCantidad;
        //            }

        //        }
        //        return TotalCantidad;
        //    }
        //    catch (Exception ex)
        //    {
        //        return TotalCantidad;
        //    }
        //}

        //public Decimal SumarExentos(List<Anexo6> Anexo6)
        //{
        //    Decimal TotalCantidad = 0;
        //    Decimal TryParseCantidad = 0;
        //    bool ValidarTryParseCantidad = false;
        //    try
        //    {
        //        if (Anexo6.Count > 0)
        //        {

        //            foreach (var item in Anexo6)
        //            {

        //                ValidarTryParseCantidad = Decimal.TryParse(item.Exento, out TryParseCantidad);
        //                if (ValidarTryParseCantidad)
        //                    TotalCantidad = TotalCantidad + TryParseCantidad;
        //            }

        //        }
        //        return TotalCantidad;
        //    }
        //    catch (Exception ex)
        //    {
        //        return TotalCantidad;
        //    }
        //}
        //public Decimal SumarLocal(List<Anexo6> Anexo6)
        //{
        //    Decimal TotalCantidad = 0;
        //    Decimal TryParseCantidad = 0;
        //    bool ValidarTryParseCantidad = false;
        //    try
        //    {
        //        if (Anexo6.Count > 0)
        //        {

        //            foreach (var item in Anexo6)
        //            {

        //                ValidarTryParseCantidad = Decimal.TryParse(item.Local, out TryParseCantidad);
        //                if (ValidarTryParseCantidad)
        //                    TotalCantidad = TotalCantidad + TryParseCantidad;
        //            }

        //        }
        //        return TotalCantidad;
        //    }
        //    catch (Exception ex)
        //    {
        //        return TotalCantidad;
        //    }
        //}
        //public Decimal SumarTC(List<Anexo6> Anexo6)
        //{
        //    Decimal TotalCantidad = 0;
        //    Decimal TryParseCantidad = 0;
        //    bool ValidarTryParseCantidad = false;
        //    try
        //    {
        //        if (Anexo6.Count > 0)
        //        {

        //            foreach (var item in Anexo6)
        //            {

        //                ValidarTryParseCantidad = Decimal.TryParse(item.TransitoConexion, out TryParseCantidad);
        //                if (ValidarTryParseCantidad)
        //                    TotalCantidad = TotalCantidad + TryParseCantidad;
        //            }

        //        }
        //        return TotalCantidad;
        //    }
        //    catch (Exception ex)
        //    {
        //        return TotalCantidad;
        //    }
        //}
        //public Decimal SumarTL(List<Anexo6> Anexo6)
        //{
        //    Decimal TotalCantidad = 0;
        //    Decimal TryParseCantidad = 0;
        //    bool ValidarTryParseCantidad = false;
        //    try
        //    {
        //        if (Anexo6.Count > 0)
        //        {

        //            foreach (var item in Anexo6)
        //            {

        //                ValidarTryParseCantidad = Decimal.TryParse(item.TransitoLinea, out TryParseCantidad);
        //                if (ValidarTryParseCantidad)
        //                    TotalCantidad = TotalCantidad + TryParseCantidad;
        //            }

        //        }
        //        return TotalCantidad;
        //    }
        //    catch (Exception ex)
        //    {
        //        return TotalCantidad;
        //    }
        //}
        //public Decimal SumarEmbarcados(List<Anexo6> Anexo6)
        //{
        //    Decimal TotalCantidad = 0;
        //    Decimal TryParseCantidad = 0;
        //    bool ValidarTryParseCantidad = false;
        //    try
        //    {
        //        if (Anexo6.Count > 0)
        //        {

        //            foreach (var item in Anexo6)
        //            {

        //                ValidarTryParseCantidad = Decimal.TryParse(item.Embarcados, out TryParseCantidad);
        //                if (ValidarTryParseCantidad)
        //                    TotalCantidad = TotalCantidad + TryParseCantidad;
        //            }

        //        }
        //        return TotalCantidad;
        //    }
        //    catch (Exception ex)
        //    {
        //        return TotalCantidad;
        //    }
        //}

        /// <summary>
        /// Metodo para generar y descargar el excel Anexo1
        /// </summary>
        /// <param name="Anexo19"></param>
        /// <param name="TipoCobro"></param>
        /// <returns>File Excel</returns>
        public byte[] ArmarExcel(List<Anexo6> Anexo6, string tipo, string filtro1, string filtro2)
        {
            string ValueTotal = string.Empty;

            try
            {
                decimal prueba = SumarTotalCantidad(Anexo6, tipo);
                //TotalInfantes = SumarInfantes(Anexo6);
                //TotalExentos = SumarExentos(Anexo6);
                //TotalLocal = SumarLocal(Anexo6);
                //TotalTC = SumarTC(Anexo6);
                //TotalTL = SumarTL(Anexo6);
                //TotalEmbarcados = SumarEmbarcados(Anexo6);
                using (var workbook = new XLWorkbook())
                {
                    //Generamos la hoja
                    var worksheet = workbook.Worksheets.Add("Anexo9");
                    //Generamos la cabecera

                    worksheet.Range("A1:P1").Merge().Value = "";
                    worksheet.Range("A1:P1").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("E2:J2").Merge().Value = "Infrasa - Jarvis Informe";
                    worksheet.Range("A2:P2").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("E2:J2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("E3:J3").Merge().Value = filtro1;
                    worksheet.Range("A3:P3").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("E3:J3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("E4:J4").Merge().Value = filtro2;
                    worksheet.Range("A4:P4").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("E4:J4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                    worksheet.Range("E5:J5").Merge().Value = "Fecha de ejecución " + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    worksheet.Range("A5:P5").Style.Fill.BackgroundColor = XLColor.White;
                    worksheet.Range("E5:J5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.AddPicture(@"wwwroot\images\logo-jarvis-informe.png").MoveTo(worksheet.Cell("B1")).Scale(0.6);
                    worksheet.AddPicture(@"wwwroot\images\opain-logo-informe.png").MoveTo(worksheet.Cell("K2")).Scale(0.6);

                    worksheet.Cell("A6").Value = "Nombre Aerolínea";
                    worksheet.Cell("B6").Value = "Fecha Vuelo";
                    worksheet.Cell("C6").Value = "Matrícula";
                    worksheet.Cell("D6").Value = "Sigla";
                    worksheet.Cell("E6").Value = "Código Vuelo";
                    worksheet.Cell("F6").Value = "Tipo Vuelo";
                    worksheet.Cell("G6").Value = "Embarcados";
                    worksheet.Cell("H6").Value = "Tránsito";
                    worksheet.Cell("I6").Value = "Tránsito Linea";
                    worksheet.Cell("J6").Value = "Tránsito Conexión";
                    worksheet.Cell("K6").Value = "Local";
                    worksheet.Cell("L6").Value = "Exento";
                    worksheet.Cell("M6").Value = "Tripulación";
                    worksheet.Cell("N6").Value = "Infantes";
                    worksheet.Cell("O6").Value = "Pagan Tasa";
                    //worksheet.Cell("Q6").Value = "Pagan COP";

                    if (tipo == "INT")
                    {
                      
                        worksheet.Cell("P6").Value = "Pagan USD";
                        worksheet.Cell("Q6").Value = "Pagan COP";
                    }
                    else 
                    {   worksheet.Cell("P6").Value = "Pagan COP"; 
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
                    worksheet.Cell("M6").Style.Font.Bold = true;
                    worksheet.Cell("M6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("N6").Style.Font.Bold = true;
                    worksheet.Cell("N6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    worksheet.Cell("O6").Style.Font.Bold = true;
                    worksheet.Cell("O6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);

                    if (tipo == "INT")
                    {
                        worksheet.Cell("P6").Style.Font.Bold = true;
                        worksheet.Cell("P6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                        worksheet.Cell("Q6").Style.Font.Bold = true;
                        worksheet.Cell("Q6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    }
                    else
                    {
                        worksheet.Cell("P6").Style.Font.Bold = true;
                        worksheet.Cell("P6").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 183, 12);
                    }



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
                        worksheet.Cell(nRow, 13).Value = datos.Tripulacion;
                        worksheet.Cell(nRow, 14).Value = datos.Infantes;
                        worksheet.Cell(nRow, 15).Value = datos.PaganTasa;
                        //worksheet.Cell(nRow, 16).Value = datos.PaganCOP;
                        if (tipo == "INT")
                        {
                            worksheet.Cell(nRow, 16).Value = datos.PaganUSD;
                            worksheet.Cell(nRow, 17).Value = datos.PaganCOP;
                        }
                        else 
                        {   worksheet.Cell(nRow, 16).Value = datos.PaganCOP; 
                        }


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
                    worksheet.Cell(nRow, 12).Value = TotalExentos;
                    worksheet.Cell(nRow, 12).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 13).Value = TotalTripulacion;
                    worksheet.Cell(nRow, 13).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 14).Value = TotalInfantes;
                    worksheet.Cell(nRow, 14).Style.Font.Bold = true;
                    worksheet.Cell(nRow, 15).Value = TotalPagaTasa;
                    worksheet.Cell(nRow, 15).Style.Font.Bold = true;

                    if (tipo == "INT")
                    {
                        worksheet.Cell(nRow, 16).Value = TotalCantidad;
                        worksheet.Cell(nRow, 16).Style.Font.Bold = true;
                        worksheet.Cell(nRow, 17).Value = TotalPaganCOP;
                        worksheet.Cell(nRow, 17).Style.Font.Bold = true;
                    }
                    else
                    {
                        worksheet.Cell(nRow, 16).Value = TotalCantidad;
                        worksheet.Cell(nRow, 16).Style.Font.Bold = true;
                    }



                    worksheet.Cell(nRow, 1).Value = "Totales";
                    
                    if (tipo == "INT")
                    {
                        worksheet.Range("A" + nRow + ":Q" + nRow).Style.Fill.BackgroundColor = XLColor.Black;
                    }
                    else 
                    {
                        worksheet.Range("A" + nRow + ":P" + nRow).Style.Fill.BackgroundColor = XLColor.Black;
                    }


                    //worksheet.Range("A" + nRow + ":Q" + nRow).Style.Fill.BackgroundColor = XLColor.Black;

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
