using BitArmory.ReCaptcha;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace Opain.Jarvis.Presentacion.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]

    public class LoginModel : PageModel
    {
        //private readonly SignInManager<Usuario> signInManager;
        private readonly IServicioApi servicioApi;
        private readonly ILogger<LoginModel> logger;
        private readonly AppSettings appSettings;
        public IConfiguration Configuration { get; }
        public bool Debug {get;}

        public LoginModel( IServicioApi api, ILogger<LoginModel> log, IConfiguration conf )
        {
            servicioApi = api;
            Debug = false;
            //signInManager = sim;
            logger = log;
            
            Configuration = conf;

            var appSettingsSection = Configuration.GetSection("reCaptcha");
            appSettings = appSettingsSection.Get<AppSettings>();

            SiteKey = appSettings.SiteKey;

            //Configurar la llave pública del reCaptcha            
            Input = new InputModel
            {
                SiteKey = appSettings.SiteKey,
                AceptarPoliticas = false
            };

#if DEBUG
            Debug = true;
#endif
        }

        public string SiteKey { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Ingrese su nombre de usuario")]
            [Display(Name = "Usuario", Prompt = "Usuario")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Ingrese su contraseña")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña", Prompt = "Contraseña")]
            public string Password { get; set; }

            [Display(Name = "Recordarme?")]
            public bool RememberMe { get; set; }

            [Display(Name = "Acepto y me obligo a cumplir la política de tratamiento de datos especialmente en relación con mi obligación de obtener la autorización de los titulares de los datos personales para poder compartirlos.")]
            public bool AceptarPoliticas { get; set; }

            public string SiteKey { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

           // await signInManager.SignOutAsync();

            returnUrl = returnUrl ?? Url.Content("~/");

            if (!appSettings.IgnoreIp.Equals("1"))
            {
                bool reCaptcha = await ValidateReCaptcha();

                if (!reCaptcha)
                {
                    ModelState.AddModelError(string.Empty, "Por favor confirme el reCAPTCHA");
                    reCaptcha = true;
                    return Page();
                }
            }

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true                

                //var result = await signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                //if (!result.Succeeded)
                //{
                //    ModelState.AddModelError(string.Empty, "Credenciales no válidas");
                //    return Page();
                //}
                // segmneto de codigo para el token valido segun usr autenticado...
                string pathToken = Configuration.GetSection("URIs:ObtenerTokenJWT").Value + "?usr=" + Input.Username + "&psw=" + Input.Password;
                string token = await servicioApi.GetAsync<string>(pathToken);

                await servicioApi.AddToken<string>(token); 

                UsuarioOtd usuario = await servicioApi.GetAsync<UsuarioOtd>(string.Format(Configuration.GetSection("URIs:UsuariosConsultarPorAlias").Value, Input.Username)).ConfigureAwait(false);
                // Verifico el password
                if (usuario != null)
                {
                    bool usuariovalido = await servicioApi.GetAsync<bool>(string.Format(Configuration.GetSection("URIs:UsuariosConsultarPorClaveUser").Value, Input.Username, Input.Password)).ConfigureAwait(false);
                    if (usuariovalido == false)
                    {
                        ModelState.AddModelError(string.Empty, "Credenciales no válidas");
                        return Page();
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Credenciales no válidas");
                    return Page();
                }

                if (!usuario.Activo)
                {
                    ModelState.AddModelError(string.Empty, "Usuario bloqueado.");
                    return Page();
                }
                AerolineaOtd aerolinea = new AerolineaOtd();
                aerolinea.Nombre = "OPAIN";
                aerolinea.IdEstado = "True";
                aerolinea.Id = 1165;

                if (usuario.RolesUsuario.Count(x => x.Rol.Name.Equals("AEROLINEA")) > 0 ||
                    usuario.RolesUsuario.Count(x => x.Rol.Name.Equals("SUPERVISOR")) > 0 ||
                    usuario.RolesUsuario.Count(x => x.Rol.Name.Equals("CARGA")) > 0 ||
                    usuario.RolesUsuario.Count(x => x.Rol.Name.Equals("SUPERVISOR CARGA")) > 0)
                {
                    var aerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(Configuration.GetSection("URIs:AerolineaObtenerTodos").Value).ConfigureAwait(false);                   

                    if (usuario.Aerolinea != null)
                    {
                        aerolinea.Nombre = usuario.UsuarioAerolinea.FirstOrDefault().Aerolinea.Nombre;
                        aerolinea.IdEstado = "True";
                        aerolinea.Id = usuario.UsuarioAerolinea.FirstOrDefault().Aerolinea.Id;

                        if (aerolineas.FirstOrDefault(x => x.Id == usuario.UsuarioAerolinea.FirstOrDefault().IdAerolinea).IdEstado == "False")
                        {
                            ModelState.AddModelError(string.Empty, "La aerolínea se encuentra inactiva");
                            return Page();
                        }
                    }

                    string rutaRelativa = Configuration.GetSection("URIs:HorarioAerolineaPrincipal").Value;
                    IList<HorarioAerolineaOtd> respuesta = await servicioApi.GetAsync<IList<HorarioAerolineaOtd>>(rutaRelativa);
                    HorarioAerolineaOtd horarioAero = new HorarioAerolineaOtd();
                    horarioAero = respuesta.Where(p => p.IdAerolinea == usuario.UsuarioAerolinea.FirstOrDefault().IdAerolinea).FirstOrDefault();
                    if (horarioAero != null)
                    {
                        DateTime fechaini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(horarioAero.HoraInicio.Substring(0, 2)), Convert.ToInt32(horarioAero.HoraInicio.Substring(3, 2)), 0);
                        DateTime fechafin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(horarioAero.HoraFin.Substring(0, 2)), Convert.ToInt32(horarioAero.HoraFin.Substring(3, 2)), 0);
                        // verifico que estè entre los parametros de horas permitidas
                        if (DateTime.Now >= fechaini && DateTime.Now <= fechafin)
                        {

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "El usuario no tiene horario de acceso en estos momentos");
                            return Page();
                        }


                    }
                    if (aerolinea.IdEstado.Equals("False"))
                    {
                        ModelState.AddModelError(string.Empty, "Usuario bloqueado.");
                        return Page();
                    }

                }

                string grupo = "";
                string rol = usuario.RolesUsuario.FirstOrDefault().Rol.Name;

                if (rol == "AEROLINEA" || rol == "CARGA" || rol == "SUPERVISOR" || rol == "SUPERVISOR CARGA")
                {
                    grupo = "AEROLINEA";
                }

                if (rol == "OPAIN" || rol == "TECNOLOGIA" || rol == "ADMINISTRADOR")
                {
                    grupo = "OPAIN";
                }


                if (rol == "EXTERNO")
                {
                    grupo = "EXTERIOR";
                }
               
                AccesoOtd acceso = new AccesoOtd()
                {
                    Fecha = DateTime.Now,
                    NombreUsuario = string.Format("{0} {1}", usuario.Nombre, usuario.Apellido),
                    Hora = DateTime.Now.ToString("H:mm"),
                    Rol = rol,
                    Grupo = grupo,
                    IdAeroLineas= aerolinea.Id

                };
                
                bool politica = await servicioApi.GetAsync<bool>(string.Format(Configuration.GetSection("URIs:PoliticasDeTratamientoDeDatosObtenerTodos").Value, Input.Username)).ConfigureAwait(false);
                var _UsuarioAerolinea = await servicioApi.GetAsync<IList<AerolineaOtd>>(Configuration.GetSection("URIs:AerolineaObtenerTodos").Value).ConfigureAwait(false);
                if (!politica)
                {
                        PoliticasDeTratamientoDeDatosOtd politicasDeTratamientoDeDatos = new PoliticasDeTratamientoDeDatosOtd();
                        politicasDeTratamientoDeDatos.NombreUsuario = usuario.UserName;
                        politicasDeTratamientoDeDatos.AceptarPoliticas = Input.AceptarPoliticas;
                        politicasDeTratamientoDeDatos.Fecha = acceso.Fecha;
                        politicasDeTratamientoDeDatos.Hora = acceso.Hora;
                        
                        //Nuevos campos
                        politicasDeTratamientoDeDatos.Email = usuario.Email.ToString();
                        politicasDeTratamientoDeDatos.PhoneNumber = usuario.Telefono.ToString();
                        politicasDeTratamientoDeDatos.NumeroDocumento = usuario.NumeroDocumento.ToString();
                        politicasDeTratamientoDeDatos.Cargo = usuario.Cargo.ToString();

                        if (usuario.UsuarioAerolinea.Count()>0)
                        {
                            politicasDeTratamientoDeDatos.Aerolinea = usuario.UsuarioAerolinea.FirstOrDefault().Aerolinea.Nombre.ToString();
                        }
                        else
                        {
                            politicasDeTratamientoDeDatos.Aerolinea = string.Empty;
                        }

                        //politicasDeTratamientoDeDatos.Aerolinea = _UsuarioAerolinea.FirstOrDefault(x => x.Nombre == usuario.UsuarioAerolinea.FirstOrDefault().Aerolinea.Nombre).ToString();

                        await servicioApi.PostAsync<PoliticasDeTratamientoDeDatosOtd>(Configuration.GetSection("URIs:PoliticasDeTratamientoDeDatosInsertar").Value, politicasDeTratamientoDeDatos).ConfigureAwait(false);
                    //string ip = GetClientIpAddress();
                }

                

                await servicioApi.PostAsync<AccesoOtd>(Configuration.GetSection("URIs:AccesoInsertar").Value, acceso).ConfigureAwait(false);

                logger.LogInformation("User logged in.");
                // Asigno los claims relacionados




                // ClaimsPrincipal user = User as ClaimsPrincipal;

                TasaAeroportuariaOtd t = null;
                t = await servicioApi.GetAsync<TasaAeroportuariaOtd>(string.Format(Configuration.GetSection("URIs:TasaAeroportuariaObtenerUltima").Value, usuario)).ConfigureAwait(false);
                // ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
                // //var identity = new ClaimsIdentity(user, CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                // identity.AddClaim(new Claim("NombreUsuario", usuario.Nombre + " " + usuario.Apellido));
                // identity.AddClaim(new Claim("UsuarioEmail", usuario.Email));
                // identity.AddClaim(new Claim("Tasa", string.Format("${0} COP = ${1} USD", t.ValorCOP, t.ValorUSD)));

                // if (usuario.UsuarioAerolinea.Count > 0 &&
                // (rol == "AEROLINEA" ||
                //  rol == "SUPERVISOR" ||
                //  rol == "CARGA" ||
                //  rol == "SUPERVISOR CARGA"))
                // {
                //     identity.AddClaim(new Claim("NombreAerolinea", usuario.UsuarioAerolinea[0].Aerolinea.Nombre));
                //     identity.AddClaim(new Claim("IdAerolinea", usuario.UsuarioAerolinea[0].Aerolinea.Id.ToString()));
                // }
                // else
                // {
                //     identity.AddClaim(new Claim("NombreAerolinea", string.Empty));
                //     identity.AddClaim(new Claim("IdAerolinea", "0"));
                // }

                // if (usuario.UsuarioAerolinea.Count > 0 && (rol == "AEROLINEA" || rol == "SUPERVISOR"))
                // {
                //     identity.AddClaim(new Claim("ActivarCarga", CargarHorario(usuario).Result));
                // }
                // else if (rol == "OPAIN" || rol == "ADMINISTRADOR")
                // {
                //     identity.AddClaim(new Claim("ActivarCarga", "1"));
                // }
                // else
                // {
                //     identity.AddClaim(new Claim("ActivarCarga", "0"));
                // }



                // await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user,
                //new Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties
                //{
                //    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                //    IsPersistent = false,
                //    AllowRefresh = false
                //});


                //var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);

                //identity.AddClaim(new Claim("NombreAerolinea", "jjj"));
                //identity.AddClaim(new Claim("NombreAerolinea2", "sss"));
                //identity.AddClaim(new Claim(ClaimTypes.Role, "User"));

                //var principal = new ClaimsPrincipal(identity);

                //var authProperties = new AuthenticationProperties
                //{
                //    AllowRefresh = true,
                //    ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                //    IsPersistent = true,
                //};

                //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(principal), authProperties);



                var claims = new List<Claim>();
                  claims.Add(new Claim(ClaimTypes.Name, usuario.UserName));
                claims.Add(new Claim("FullName", usuario.Nombre + " " + usuario.Apellido));
                claims.Add(new Claim(ClaimTypes.Role, rol));
                claims.Add(new Claim("token", token));
                //identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                //claims.Add(new Claim("NombreUsuario", usuario.Nombre + " " + usuario.Apellido));
                //claims.Add(new Claim("UsuarioEmail", usuario.Email));
                //claims.Add(new Claim("Tasa", string.Format("${0} COP = ${1} USD", t.ValorCOP, t.ValorUSD)));
                //if (usuario.UsuarioAerolinea.Count > 0 &&
                //(rol == "AEROLINEA" ||
                // rol == "SUPERVISOR" ||
                // rol == "CARGA" ||
                // rol == "SUPERVISOR CARGA"))
                //{
                //    claims.Add(new Claim("NombreAerolinea", usuario.UsuarioAerolinea[0].Aerolinea.Nombre));
                //    claims.Add(new Claim("IdAerolinea", usuario.UsuarioAerolinea[0].Aerolinea.Id.ToString()));
                //}
                //else
                //{
                //    claims.Add(new Claim("NombreAerolinea", string.Empty));
                //    claims.Add(new Claim("IdAerolinea", "0"));
                //}

                //if (usuario.UsuarioAerolinea.Count > 0 && (rol == "AEROLINEA" || rol == "SUPERVISOR"))
                //{
                //    claims.Add(new Claim("ActivarCarga", CargarHorario(usuario).Result));
                //}
                //else if (rol == "OPAIN" || rol == "ADMINISTRADOR")
                //{
                //    claims.Add(new Claim("ActivarCarga", "1"));
                //}
                //else
                //{
                //    claims.Add(new Claim("ActivarCarga", "0"));
                //}



                var claimsIdentity = new ClaimsIdentity(
                    claims, "Cookies");

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    "Cookies",
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect(returnUrl);

            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private void addToken()
        {
            throw new NotImplementedException();
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

            if (horaValidar < horaInicio || horaValidar > horaFinal)
            {
                if (horarioExtendido != null)
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

        private string GetClientIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }

        private async Task<bool> ValidateReCaptcha()
        {
            try
            {
                //1. Get the client IP address in your chosen web framework
                string clientIp = GetClientIpAddress();
                string captchaResponse = null;

                //2. Extract the `g-recaptcha-response` field from the HTML form in your chosen web framework
                if (this.Request.Form.TryGetValue(Constants.ClientResponseKey, out var formField))
                {
                    captchaResponse = formField;
                }
#if DEBUG
                return true;
#endif
                //3. Validate the reCAPTCHA with Google
                var captchaApi = new ReCaptchaService();
                var isValid = await captchaApi.Verify2Async(captchaResponse, clientIp, appSettings.SecretKey);
                return isValid;
                //return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
