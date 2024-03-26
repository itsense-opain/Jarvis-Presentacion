using System;
using Microsoft.Extensions.Configuration;

using Opain.Jarvis.Presentacion.Web.Areas.Informes.ExportarInformes;

namespace Opain.Jarvis.Presentacion.Web.Areas.Informes.ValidacionInformes
{
    public class RutasRelativas
    {
        private readonly IConfiguration configuration;

        public RutasRelativas(IConfiguration cfg)
        {
            configuration = cfg;
        }

        public string GenerarRutaRelativas(string Cadena, string Informe)
        {
            string RetornoRuta = string.Empty;
            string[] DatosRuta = Cadena.Split(";");
            try
            {
                switch (DatosRuta[0])
                {
                    case "PuenteAbordaje":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_PuenteAbordaje").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[5],
                                                  DatosRuta[6],
                                                  DatosRuta[3],
                                                  DatosRuta[4],
                                                  DatosRuta[2],
                                                  true);
                        break;
                    case "ParqueoAeronaves":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_ParqueoAeronaves").Value,
                                               DatosRuta[1],
                                               DatosRuta[2],
                                               DatosRuta[3],
                                               DatosRuta[4],
                                               DatosRuta[5],
                                               DatosRuta[6],
                                               true);
                        break;
                    case "TasasAeroportuariasCOP":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_TasasAeroportuariasCOP").Value,
                                               DatosRuta[1],
                                               DatosRuta[2],
                                               DatosRuta[3],
                                               DatosRuta[4],
                                               DatosRuta[5],
                                               DatosRuta[6],
                                               true);
                        break;
                    case "TasasAeroportuariasUSD":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_TasasAeroportuariasUSD").Value,
                                               DatosRuta[1],
                                               DatosRuta[2],
                                               DatosRuta[3],
                                               DatosRuta[4],
                                               DatosRuta[5],
                                               DatosRuta[6],
                                               true);

                        break;
                    case "MostradoresMostradores":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_Mostradores").Value,
                                               DatosRuta[1],
                                               DatosRuta[2],
                                               DatosRuta[3],
                                               DatosRuta[4],
                                               DatosRuta[5],
                                               true);

                        break;
                    case "DetallesVuelosInterventoria":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_DetalleVuelosInterventoria").Value,
                                              DatosRuta[1],
                                              DatosRuta[2],
                                              DatosRuta[3],
                                              DatosRuta[4],
                                              DatosRuta[5],
                                              true);

                        break;
                    case "UsoCarroBomberos":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_UsoCarroBomberos").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4],
                                                  DatosRuta[5],
                                                  true);
                        break;
                    case "ResumenTasasAeroportuariasFacturadas":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_ResumenTasasAeroportuariasFacturadas").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4], true);
                        break;
                    case "SoportesExenciones":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_SoporteExencionesTasasAeroportuarias").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  true);
                        break;
                    case "FacturacionParqueosAmplicacionLA33":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_FacturacionParqueosAmplicacionLA33").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4], true, DatosRuta[5], DatosRuta[6]);
                        break;
                    case "FacturacionParqueosAmplicacionLA33100":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_FacturacionParqueosAmplicacionLA33100").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4], true, DatosRuta[5], DatosRuta[6]);
                        break;
                    case "GPU":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_GPU").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4],
                                                  DatosRuta[5],
                                                  DatosRuta[6], true);
                        break;
                    case "ResumenDetalleTasasAeroportuarias":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_ResumenDetalleTasasAeroportuariasFacturadas").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4], true);
                        break;
                    case "GPUAerocivil":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_GPUAerocivil").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4],
                                                  DatosRuta[5],
                                                  DatosRuta[6], true);
                        break;
                    case "TasasAeroportuariasFacturadasporAerolinea":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_TasasAeroportuariasFacturadasPorAerolineas").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3], true);
                        break;
                    case "InformeCarnetizacion":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_InformeCarnetizacion").Value,
                                                  DatosRuta[3],
                                                  DatosRuta[4], true);
                        break;
                    case "DetallesVuelosInterventoriaInfrasa":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_DetalleVuelosInterventoria").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4],
                                                   DatosRuta[7],
                                                  true);
                        break;
                    case "FacturacionAmplicacionLA33Puentes":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_FacturacionAmplicacionLA33PuentesAbordaje").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4],
                                                  DatosRuta[5]);
                        break;
                    case "InformeTickets":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_Tickets").Value,
                                                  DatosRuta[3],
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  "0");
                        break;
                    case "InformeCobro":
                        RetornoRuta = string.Format(configuration.GetSection("URIs:Informes_Cobros").Value,
                                                  DatosRuta[1],
                                                  DatosRuta[2],
                                                  DatosRuta[3],
                                                  DatosRuta[4]);
                        break;
                    default:
                        break;
                }
                return RetornoRuta;
            }
            catch (Exception ex)
            {
                Console.Write(ex.StackTrace.ToString());
                return RetornoRuta;
            }
        }
    }
}
