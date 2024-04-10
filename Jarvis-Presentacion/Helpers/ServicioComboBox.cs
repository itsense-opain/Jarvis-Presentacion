using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Helpers
{
    public class ServicioComboBox
    {
        private IMemoryCache _cache;
        private readonly ServicioOracle ServicioOracle;
        private readonly IConfiguration configuration;

        public ServicioComboBox(IConfiguration cfg, IMemoryCache memoryCache, ServicioOracle oracle)
        {
            _cache = memoryCache;
            configuration = cfg;
            ServicioOracle = oracle;
        }

        public List<TextoValor> TraerAerolineas()
        {
            Task<List<TextoValor>> Tarea = TraerAerolineasAsync();
            Task.Run(async() => { await Tarea; } ).Wait();

            return Tarea.Result;
        }
        public List<TextoValor> TraerAerolineas2()
        {
            Task<List<TextoValor>> Tarea = TraerAerolineasAsync2();
            Task.Run(async () => { await Tarea; }).Wait();

            return Tarea.Result;
        }

        public async Task<List<TextoValor>> TraerAerolineasAsync()
        {
            List<TextoValor> AerolineaCache = _cache.Get<List<TextoValor>>("AerolineasCache");
            if (AerolineaCache != null)
                return AerolineaCache;

            string rutaRelativa = configuration.GetSection("URIs:Informes_TraerAerolineas").Value;

            List<AerolineaViewModel> Aerolineas = await ServicioOracle.GetAsync<List<AerolineaViewModel>>(rutaRelativa);
            Aerolineas = Aerolineas.OrderBy(x => x.Texto).ToList();
            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                return new List<TextoValor>();
            
            if (Aerolineas == null)
                return new List<TextoValor>();

            List<TextoValor> Resultado = new List<TextoValor>();

            foreach (var item in Aerolineas)
            {
                Resultado.Add(new TextoValor
                {
                    Valor = item.Valor,
                    Texto = item.Texto
                });
            }

            _cache.Set("AerolineasCache", Resultado, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));

            return Resultado;
        }

        public async Task<bool> UpdVueloValidJDE()
        {
            string rutaRelativa = configuration.GetSection("URIs:UpdVueloValidJDE").Value;

            await ServicioOracle.GetAsync<bool>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                return false;

            return true;
        }
        public async Task<bool> UpdVueloValidJDEVuelo( string idvuelo)
        {
            string rutaRelativa =string.Format(configuration.GetSection("URIs:UpdVueloValidJDEVuelo").Value, idvuelo);
            await ServicioOracle.GetAsync<bool>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                return false;

            return true;
        }

        public async Task<bool> TraerCiudadesJDE()
        {
            string rutaRelativa = configuration.GetSection("URIs:TraerCiudadesJDE").Value;

            await ServicioOracle.GetAsync<bool>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                return false;

            return true;
        }

        public async Task<bool> TraerVuelosJDE()
        {
            string rutaRelativa = configuration.GetSection("URIs:TraerVuelosJDE").Value;

            await ServicioOracle.GetAsync<bool>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                return false;

            return true;
        }

        public async Task<List<TextoValor>> TraerAerolineasAsync2()
        {
            List<TextoValor> AerolineaCache = _cache.Get<List<TextoValor>>("AerolineasCache");
            if (AerolineaCache != null)
                return AerolineaCache;

            string rutaRelativa = configuration.GetSection("URIs:Informes_TraerAerolineas2").Value;

            List<AerolineaViewModel> Aerolineas = await ServicioOracle.GetAsync<List<AerolineaViewModel>>(rutaRelativa);
            Aerolineas = Aerolineas.OrderBy(x => x.Texto).ToList();

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                return new List<TextoValor>();

            if (Aerolineas == null)
                return new List<TextoValor>();

            List<TextoValor> Resultado = new List<TextoValor>();

            foreach (var item in Aerolineas)
            {
                Resultado.Add(new TextoValor
                {
                    Valor = item.Valor,
                    Texto = item.Texto
                });
            }

            _cache.Set("AerolineasCache", Resultado, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(15)));

            return Resultado;
        }

        public List<TextoValor> TraerTipos()
        {
            List<TextoValor> Resultado = new List<TextoValor>();

            Resultado.Add(new TextoValor
            {
                Valor = "USD",
                Texto = "USD"
            });

            Resultado.Add(new TextoValor
            {
                Valor = "COP",
                Texto = "COP"
            });

            return Resultado;
        }

        public List<TextoValor> TraerTiposV2()
        {
            List<TextoValor> Resultado = new List<TextoValor>();

            Resultado.Add(new TextoValor
            {
                Valor = "DOM_COP",
                Texto = "NACIONALES COP"
            });

            Resultado.Add(new TextoValor
            {
                Valor = "DOM_USD",
                Texto = "NACIONALES USD"
            });

            Resultado.Add(new TextoValor
            {
                Valor = "INT_COP",
                Texto = "INTERNACIONALES COP"
            });

            Resultado.Add(new TextoValor
            {
                Valor = "INT_USD",
                Texto = "INTERNACIONAL USD"
            });

            Resultado.Add(new TextoValor
            {
                Valor = "TOT_COP",
                Texto = "TOTAL COP"
            });

            Resultado.Add(new TextoValor
            {
                Valor = "TOT_USD",
                Texto = "TOTAL USD"
            });

            return Resultado;
        }
        public List<TextoValor> TraerTiposTotales()
        {
            List<TextoValor> Resultado = new List<TextoValor>();
             
            Resultado.Add(new TextoValor
            {
                Valor = "TOT_COP",
                Texto = "TOTAL COP"
            });

            Resultado.Add(new TextoValor
            {
                Valor = "TOT_USD",
                Texto = "TOTAL USD"
            });

            return Resultado;
        }

        public List<TextoValor> TraerTiposVuelos()
        {
            List<TextoValor> Resultado = new List<TextoValor>();

            Resultado.Add(new TextoValor
            {
                Valor = "INT",
                Texto = "Vuelo Internacional"
            });

            Resultado.Add(new TextoValor
            {
                Valor = "NAL",
                Texto = "Vuelo Nacional"
            });

            return Resultado;
        }

        public List<TextoValor> TraerEstados()
        {
            List<TextoValor> Resultado = new List<TextoValor>();

            Resultado.Add(new TextoValor
            {
                Valor = "08",
                Texto = "FACTURADO"
            });
            Resultado.Add(new TextoValor
            {
                Valor = "02",
                Texto = "PRE-ANALIZADO"
            });
            Resultado.Add(new TextoValor
            {
                Valor = "06",
                Texto = "AUDITADO"
            });

            //Resultado.Add(new TextoValor
            //{
            //    Valor = "03",
            //    Texto = "CONGELADO"
            //});

            return Resultado;
        }

        public async Task<List<DatosExentos>> TraerExentos(string numeroVuelo, DateTime fecha)
        {
            string dia = fecha.Day.ToString();
            string mes = fecha.Month.ToString();
            string anio = fecha.Year.ToString();
            string rutaRelativa = string.Format(configuration.GetSection("URIs:Informes_TraerExentos").Value, numeroVuelo,dia,mes,anio);

            List<DatosExentos> lstExentos = await ServicioOracle.GetAsync<List<DatosExentos>>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                return new List<DatosExentos>();

            if (lstExentos == null)
                return new List<DatosExentos>();
             
            return lstExentos;
        }        
        public async Task<bool> TraerAeroJDE()
        {
            
            string rutaRelativa = configuration.GetSection("URIs:TraerAeroJDE").Value;
            
            await ServicioOracle.GetAsync<bool>(rutaRelativa);

            if (ServicioOracle.httpStatus != System.Net.HttpStatusCode.OK)
                return false;

            return true;

        }
    }

    public class TextoValor
    {
        public string Texto { get; set; }
        public string Valor { get; set; }
    }
}
