using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Helpers
{
    public class JarvisClaims : IClaimsTransformation
    {
        private IMemoryCache _cache;
        private readonly IServicioApi servicioApi;
        public IConfiguration Configuration { get; }

        public JarvisClaims(IServicioApi api, IConfiguration conf, IMemoryCache memoryCache)
        {
            servicioApi = api;
            Configuration = conf;
            _cache = memoryCache;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            string usuario = principal.Identity.Name;
            string CacheUsuario = string.Format("Usuario_{0}", usuario);
            string CacheAerolineasUsuario = string.Format("AerolineasUsuario_{0}", usuario);

            UsuarioOtd u = null;
            TasaAeroportuariaOtd t = null;

            u = _cache.Get<UsuarioOtd>(CacheUsuario);
            t = _cache.Get<TasaAeroportuariaOtd>(CacheAerolineasUsuario);

            ClaimsIdentity identity = (ClaimsIdentity)principal.Identity;

            string token = "false";

            if (identity != null)
            {
                token = principal.Claims.FirstOrDefault(c => c.Type.Equals("token")).Value;
                await servicioApi.AddToken<string>(token);
            }

            if (u == null)
            {
                u = await servicioApi.GetAsync<UsuarioOtd>(string.Format(Configuration.GetSection("URIs:UsuariosConsultarPorAlias").Value, usuario)).ConfigureAwait(false);
                if (u != null) _cache.Set(CacheUsuario, u, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            
            //if (t == null)
            //{
                t = await servicioApi.GetAsync<TasaAeroportuariaOtd>(string.Format(Configuration.GetSection("URIs:TasaAeroportuariaObtenerUltima").Value, usuario)).ConfigureAwait(false);
                if (t != null) _cache.Set(CacheAerolineasUsuario, t, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            //}
            
            if (u != null)
            {
                if (u.UsuarioAerolinea.Count > 0 &&
                (principal.IsInRole("AEROLINEA") ||
                principal.IsInRole("SUPERVISOR") ||
                principal.IsInRole("CARGA") ||
                principal.IsInRole("SUPERVISOR CARGA")))
                {
                    identity.AddClaim(new Claim("NombreAerolinea", u.UsuarioAerolinea[0].Aerolinea.Nombre));
                    identity.AddClaim(new Claim("IdAerolinea", u.UsuarioAerolinea[0].Aerolinea.Id.ToString()));
                    identity.AddClaim(new Claim("SiglaAerolinea", u.UsuarioAerolinea[0].Aerolinea.Sigla));
                    identity.AddClaim(new Claim("CodigoAeroJDE", u.UsuarioAerolinea[0].Aerolinea.Codigo));
                }
                else
                {
                    identity.AddClaim(new Claim("NombreAerolinea", string.Empty));
                    identity.AddClaim(new Claim("IdAerolinea", "0"));
                    identity.AddClaim(new Claim("CodigoAeroJDE", "0"));
                }

                if (u.UsuarioAerolinea.Count > 0 && (principal.IsInRole("AEROLINEA") || principal.IsInRole("SUPERVISOR")))
                {
                    identity.AddClaim(new Claim("ActivarCarga", CargarHorario(u).Result));
                }
                else if (principal.IsInRole("OPAIN") || principal.IsInRole("ADMINISTRADOR"))
                {
                    identity.AddClaim(new Claim("ActivarCarga", "1"));
                }
                else
                {
                    identity.AddClaim(new Claim("ActivarCarga", "0"));
                }

                identity.AddClaim(new Claim("Tasa", string.Format("$ {0} COP / $ {1} USD", t.ValorCOP.ToString("###,###") , t.ValorUSD)));
                identity.AddClaim(new Claim("TasaPeso", t.ValorCOP.ToString()));
                identity.AddClaim(new Claim("TasaDolar", t.ValorUSD.ToString()));
                identity.AddClaim(new Claim("NombreUsuario", u.Nombre + " " + u.Apellido));
                identity.AddClaim(new Claim("UsuarioEmail", u.Email));
            }
            
            return principal;
        }

        private async Task<string> CargarHorario(UsuarioOtd u)
        {
            string cargar = "0";
            CultureInfo ci = new CultureInfo("es-co");
            string hoy = ci.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek).ToUpper();
            string diaValidar = hoy.Substring(0, 1).ToUpper();

            if (hoy.Equals("MARTES"))
            {
                diaValidar = "M";
            }
            if (hoy.Equals("MIÉRCOLES"))
            {
                diaValidar = "W";
            }

            var dia = DateTime.Now.Day;
            var mes = DateTime.Now.Month;
            var anio = DateTime.Now.Year;

            var horariosOperacion = await servicioApi.GetAsync<IList<HorarioOperacionOtd>>(string.Format(Configuration.GetSection("URIs:HorarioOperacionPrincipal").Value)).ConfigureAwait(false);

            var horarioExtendido = u.UsuarioAerolinea[0].Aerolinea.HorarioAerolinea.FirstOrDefault(d => d.Fecha.Equals(new DateTime(anio, mes, dia)));

            var horarioOperacion = horariosOperacion.FirstOrDefault(x => x.Dia.Equals(diaValidar));

            double horaValidar = TimeSpan.Parse(string.Format("{0}:{1}", DateTime.Now.Hour, DateTime.Now.Minute)).TotalHours;

            if (horarioOperacion == null)
            {
                horarioOperacion = new HorarioOperacionOtd();
                horarioOperacion.HoraInicio = "00:00";
                
                //Parche
                horarioOperacion.HoraFin = "00:00";
            }

            double horaInicio = TimeSpan.Parse(horarioOperacion.HoraInicio).TotalHours;
            double horaFinal = TimeSpan.Parse(horarioOperacion.HoraFin).TotalHours;
            
            if(horaValidar < horaInicio || horaValidar > horaFinal)
            {
                if(horarioExtendido != null)
                {
                    horaInicio = TimeSpan.Parse(horarioExtendido.HoraInicio).TotalHours;
                    horaFinal = TimeSpan.Parse(horarioExtendido.HoraFin).TotalHours;

                    if (horaValidar >= horaInicio && horaValidar <= horaFinal)
                    {
                        cargar = "1";
                    }
                }
            }
            else
            {
                cargar = "1";
            }

            cargar = "1";

            return cargar;
        }

    }
}
