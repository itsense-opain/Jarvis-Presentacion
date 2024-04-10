using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.Presentacion.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Opain.Jarvis.Presentacion.Web.Areas.Administracion.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,TECNOLOGIA")]
    [Area("Administracion")]
    public class AdmonGeneralController : Controller
    {
        private readonly IServicioApi servicioApi;
        private readonly IConfiguration configuration;
        private readonly IEmailSender emailSender;
        private readonly ILogger<AdmonGeneralController> _logger;

        public AdmonGeneralController(IServicioApi api, IConfiguration cfg, IEmailSender email, ILogger<AdmonGeneralController> logger)
        {
            configuration = cfg;
            servicioApi = api;
            emailSender = email;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR,SUPERVISOR,SUPERVISOR CARGA,TECNOLOGIA")]
        public async Task<IActionResult> Principal(string mensaje = "")
        {

            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);

            if(User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
            {
                var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;

                ViewData["Aerolineas"] = respuesta.Where(x => x.Nombre.Equals(aerolinea)).Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Nombre.ToString()
                }).OrderBy(x => x.Text).ToList();
            }
            else
            {
                ViewData["Aerolineas"] = respuesta.Where(a=> a.IdEstado.Equals("True")).Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Nombre.ToString()
                }).OrderBy(x => x.Text).ToList();
            }
                
            AdministracionOtd admon = new AdministracionOtd();

            ViewData["mensaje"] = mensaje;

            return View(admon);
        }

        [HttpGet]
        public async Task<IActionResult> Actualizar(string Id)
        {
      
            string rutaRelativa = string.Format(configuration.GetSection("URIs:AdmonGeneralObtenerUsuario").Value, Id);
            UsuarioOtd usuarioOtd = await servicioApi.GetAsync<UsuarioOtd>(rutaRelativa);
            if (usuarioOtd.TipoDocumento.ToUpper()=="PASAP")
            {
                usuarioOtd.TipoDocumento = "Pasaporte";
            }
            string rutaRelativaAerolinea = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAerolinea);

            usuarioOtd.Perfil = usuarioOtd.RolesUsuario.FirstOrDefault().Rol.Name;

            if(usuarioOtd.UsuarioAerolinea.Count > 0)
            {
                usuarioOtd.Aerolinea = usuarioOtd.UsuarioAerolinea.FirstOrDefault().Aerolinea.Nombre.Trim();
            }            

            

            if (User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
            {
                var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;

                ViewData["Aerolineas"] = respuesta.Where(x => x.Nombre.Equals(aerolinea) && x.IdEstado.Equals("1")).Select(n => new SelectListItem
                {
                    Value = n.Nombre.ToString(),
                    Text = n.Nombre.ToString(),
                    Selected = usuarioOtd.Aerolinea == n.Nombre.ToString()
                }).OrderBy(x => x.Text).ToList();

                if (User.IsInRole("SUPERVISOR"))
                {
                    ViewData["Perfiles"] = new List<SelectListItem>
                    {
                        new SelectListItem { Selected = usuarioOtd.Perfil == "AEROLINEA", Text = "Aerolínea Comercial o Charter", Value =  "AEROLINEA"},
                        new SelectListItem { Selected = usuarioOtd.Perfil == "SUPERVISOR", Text = "Supervisor Aerolínea", Value =  "SUPERVISOR"}
                    };
                }
                else
                {
                    ViewData["Perfiles"] = new List<SelectListItem>
                    {
                        new SelectListItem { Selected = usuarioOtd.Perfil == "CARGA", Text = "Aerolínea de Carga", Value = "CARGA" },
                        new SelectListItem { Selected = usuarioOtd.Perfil == "SUPERVISOR CARGA", Text = "Supervisor Aerolínea de Carga", Value = "SUPERVISOR CARGA" }
                    };
                }
            }
            else
            {
                ViewData["Perfiles"] = new List<SelectListItem>
                {
                    new SelectListItem { Selected = usuarioOtd.Perfil == "ADMINISTRADOR", Text = "Administrador", Value = "ADMINISTRADOR" },
                    new SelectListItem { Selected = usuarioOtd.Perfil == "TECNOLOGIA", Text = "Tecnologia OPAIN", Value =  "TECNOLOGIA"},
                    new SelectListItem { Selected = usuarioOtd.Perfil == "EXTERNO", Text = "Usuario Externo", Value =  "EXTERNO"},
                    new SelectListItem { Selected = usuarioOtd.Perfil == "OPAIN", Text = "Usuario OPAIN", Value = "OPAIN" },
                    new SelectListItem { Selected = usuarioOtd.Perfil == "AEROLINEA", Text = "Aerolínea Comercial o Charter", Value =  "AEROLINEA"},
                    new SelectListItem { Selected = usuarioOtd.Perfil == "CARGA", Text = "Aerolínea de Carga", Value = "CARGA" },
                    new SelectListItem { Selected = usuarioOtd.Perfil == "SUPERVISOR", Text = "Supervisor Aerolínea", Value =  "SUPERVISOR"},
                    new SelectListItem { Selected = usuarioOtd.Perfil == "SUPERVISOR CARGA", Text = "Supervisor Aerolínea de Carga", Value = "SUPERVISOR CARGA" }   
                };
                AerolineaOtd SeleccionUno = new AerolineaOtd();
                SeleccionUno.Nombre="Seleccione uno";
                SeleccionUno.Id = 0;
                SeleccionUno.Sigla = "000";
                SeleccionUno.IdEstado = "True";
                respuesta.Add(SeleccionUno);
                if (usuarioOtd.UsuarioAerolinea[0].Aerolinea.IdEstado=="False")
                {
                    usuarioOtd.Aerolinea = SeleccionUno.Nombre;
                }
                ViewData["Aerolineas"] = respuesta.OrderBy(x => x.Sigla).Where(a=>a.IdEstado.Equals("True")).Select(n => new SelectListItem
                {
                    Value = n.Nombre.ToString(),
                    Text =  n.Nombre.ToString(),
                    Selected = usuarioOtd.Aerolinea == n.Nombre.ToString().Trim()
                }).OrderBy(x => x.Text).ToList();
            }

            ViewBag.Aerolinea = usuarioOtd.Aerolinea;

            return PartialView(usuarioOtd);
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar(UsuarioOtd usuarioOtd, string estado)
        {
            if(estado == "on")
            {
                usuarioOtd.Activo = true;
            }
            else
            {
                usuarioOtd.Activo = false;
            }

            if(usuarioOtd.Perfil == "OPAIN" || usuarioOtd.Perfil == "TECNOLOGIA" || usuarioOtd.Perfil == "EXTERNO" || usuarioOtd.Perfil == "ADMINISTRADOR")
            {
                usuarioOtd.Aerolinea = "";
            }

            string rutaRelativa = configuration.GetSection("URIs:AdmonGeneralActualizarUsuario").Value;
            await servicioApi.PostAsync<UsuarioOtd>(rutaRelativa, usuarioOtd);

            string mensaje = "Usuario actualizado exitosamente.";

            return RedirectToAction("Principal", new { mensaje = mensaje });
        }

        [HttpGet]
        public async Task<IActionResult> Insertar()
        {

           

            string rutaRelativa = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativa);


            if (User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
            {
                var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;

                ViewData["Aerolineas"] = respuesta.Where(x => x.Nombre.Equals(aerolinea)).Select(n => new SelectListItem
                {
                    Value = n.Nombre.ToString(),
                    Text = n.Nombre.ToString()
                }).OrderBy(x => x.Text).ToList();



                if (User.IsInRole("SUPERVISOR"))
                {
                    ViewData["Perfiles"] = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Aerolínea Comercial o Charter", Value =  "AEROLINEA"},
                        new SelectListItem { Text = "Supervisor Aerolínea", Value =  "SUPERVISOR"}
                    };
                }
                else
                {
                    ViewData["Perfiles"] = new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Aerolínea de Carga", Value = "CARGA" },
                        new SelectListItem { Text = "Supervisor Aerolínea de Carga", Value = "SUPERVISOR CARGA" }
                    };
                }
            }
            else
            {
                ViewData["Perfiles"] = new List<SelectListItem>
                {
                    new SelectListItem { Text = "Administrador", Value = "ADMINISTRADOR" },
                    new SelectListItem { Text = "Tecnologia OPAIN", Value =  "TECNOLOGIA"},
                    new SelectListItem { Text = "Usuario Externo", Value =  "EXTERNO"},
                    new SelectListItem { Text = "Usuario OPAIN", Value = "OPAIN" },
                    new SelectListItem { Text = "Aerolínea Comercial o Charter", Value =  "AEROLINEA"},
                    new SelectListItem { Text = "Aerolínea de Carga", Value = "CARGA" },
                    new SelectListItem { Text = "Supervisor Aerolínea", Value =  "SUPERVISOR"},
                    new SelectListItem { Text = "Supervisor Aerolínea de Carga", Value = "SUPERVISOR CARGA" }
                };

                ViewData["Aerolineas"] = respuesta.OrderBy(x=>x.Sigla).Select(n => new SelectListItem
                {
                    Value = n.Nombre.ToString(),
                    Text = n.Nombre.ToString() //(n.Sigla.ToString() + " -- " + n.Nombre.ToString()).ToString()
                                               //Text = String.Concat(n.Sigla.ToString()," -- ", n.Nombre.ToString())//(n.Sigla.ToString() + " -- " + n.Nombre.ToString()).ToString()
                }).OrderBy(x => x.Text).ToList();

            }

            
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Insertar(UsuarioOtd usuarioOtd)
        {
            var aerolinea = usuarioOtd.Aerolinea;
            string mensaje = "";

            string rutaRelativaUsuarios = configuration.GetSection("URIs:UsuariosObtenerTodos").Value;
            IList<UsuarioOtd> usuarios = await servicioApi.GetAsync<IList<UsuarioOtd>>(rutaRelativaUsuarios);
            // verifico que el usuario no exista ya en la bd con ese número de documento
            int existe = usuarios.Where(p => p.NumeroDocumento == usuarioOtd.NumeroDocumento && p.TipoDocumento == usuarioOtd.TipoDocumento).Count();
            if (existe > 0)
            {
                mensaje = "ya existe un usuario con ese tipo y número de documento";
                return RedirectToAction("Principal", new { mensaje = mensaje });
            }


            if (aerolinea != null){
                string rutaRelativaAerolineas = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
                IList<AerolineaOtd> aerolineas = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAerolineas);

               
                var cantidadUsuariosCreados = (aerolinea != null ? usuarios.Count(x => x.Aerolinea == aerolinea) : 0);
                var cantidadUsuariosAerolineas = (aerolinea != null ? aerolineas.FirstOrDefault(x => x.Nombre.Equals(aerolinea)).CantidadUsuarios : 0);
                
                if (cantidadUsuariosCreados >= cantidadUsuariosAerolineas){
                    mensaje = "No se puede exceder la cantidad de usuarios por aerolínea.";
                    return RedirectToAction("Principal", new { mensaje = mensaje });
                }
            }

            var guid = Guid.NewGuid();
            var matrizClave = guid.ToString().Split("-");
            var clave = "O" + matrizClave[0] + "*";
            usuarioOtd.Clave = "Cambiar*123"; // Toca cambiar y colocar la clave real guid
            usuarioOtd.EmailConfirmed = 1;
            usuarioOtd.Email = usuarioOtd.Email.ToUpper();
            string rutaRelativa = configuration.GetSection("URIs:AdmonGeneralInsertarUsuario").Value;
            await servicioApi.PostAsync<UsuarioOtd>(rutaRelativa, usuarioOtd);

            string url = HttpContext.Request.Path;
            var callbackUrl = string.Concat("https://",string.Concat(HttpContext.Request.Host , "/Identity/Account/ResetPass?Email="+usuarioOtd.Email));
          
            mensaje = "Usuario creado exitosamente.";

            await emailSender.SendEmailAsync(
               usuarioOtd.Email,
               "Usuario Creado",
               $"Ha sido creado su usuario para el Sistema JARVIS - OPAIN, por favor establezca su contraseña. </br> enlace: <a href="+ callbackUrl + ">Click Aquí </a> " );

            return RedirectToAction("Principal", new { mensaje = mensaje});
        }

        public async Task<IActionResult> Eliminar(int Id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:HorarioAerolineaObtener").Value, Id);
            HorarioAerolineaOtd respuesta = await servicioApi.GetAsync<HorarioAerolineaOtd>(rutaRelativa);
            return PartialView(respuesta);
        }

        [HttpGet]
        public async Task<IActionResult> EliminarUser(string Id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:AdmonGeneralObtenerUsuario").Value, Id);
            UsuarioOtd usuarioOtd = await servicioApi.GetAsync<UsuarioOtd>(rutaRelativa);

            string rutaRelativaAerolinea = configuration.GetSection("URIs:HorarioAerolineaObtenerAerolineas").Value;
            IList<AerolineaOtd> respuesta = await servicioApi.GetAsync<IList<AerolineaOtd>>(rutaRelativaAerolinea);

            usuarioOtd.Perfil = usuarioOtd.RolesUsuario.FirstOrDefault().Rol.Name;

            if (usuarioOtd.UsuarioAerolinea.Count > 0)
            {
                usuarioOtd.Aerolinea = usuarioOtd.UsuarioAerolinea.FirstOrDefault().Aerolinea.Nombre.Trim();
            }



            if (User.IsInRole("SUPERVISOR") || User.IsInRole("SUPERVISOR CARGA"))
            {
                var aerolinea = User.Claims.Where(c => c.Type.Equals("NombreAerolinea")).FirstOrDefault().Value;

                ViewData["Aerolineas"] = respuesta.Where(x => x.Nombre.Equals(aerolinea)).Select(n => new SelectListItem
                {
                    Value = n.Nombre.ToString(),
                    Text = n.Nombre.ToString(),
                    Selected = usuarioOtd.Aerolinea == n.Nombre.ToString()
                }).ToList();

                if (User.IsInRole("SUPERVISOR"))
                {
                    ViewData["Perfiles"] = new List<SelectListItem>
                    {
                        new SelectListItem { Selected = usuarioOtd.Perfil == "AEROLINEA", Text = "Aerolínea Comercial o Charter", Value =  "AEROLINEA"},
                        new SelectListItem { Selected = usuarioOtd.Perfil == "SUPERVISOR", Text = "Supervisor Aerolínea", Value =  "SUPERVISOR"}
                    };
                }
                else
                {
                    ViewData["Perfiles"] = new List<SelectListItem>
                    {
                        new SelectListItem { Selected = usuarioOtd.Perfil == "CARGA", Text = "Aerolínea de Carga", Value = "CARGA" },
                        new SelectListItem { Selected = usuarioOtd.Perfil == "SUPERVISOR CARGA", Text = "Supervisor Aerolínea de Carga", Value = "SUPERVISOR CARGA" }
                    };
                }
            }
            else
            {
                ViewData["Perfiles"] = new List<SelectListItem>
                {
                    new SelectListItem { Selected = usuarioOtd.Perfil == "ADMINISTRADOR", Text = "Administrador", Value = "ADMINISTRADOR" },
                    new SelectListItem { Selected = usuarioOtd.Perfil == "TECNOLOGIA", Text = "Tecnologia OPAIN", Value =  "TECNOLOGIA"},
                    new SelectListItem { Selected = usuarioOtd.Perfil == "EXTERNO", Text = "Usuario Externo", Value =  "EXTERNO"},
                    new SelectListItem { Selected = usuarioOtd.Perfil == "OPAIN", Text = "Usuario OPAIN", Value = "OPAIN" },
                    new SelectListItem { Selected = usuarioOtd.Perfil == "AEROLINEA", Text = "Aerolínea Comercial o Charter", Value =  "AEROLINEA"},
                    new SelectListItem { Selected = usuarioOtd.Perfil == "CARGA", Text = "Aerolínea de Carga", Value = "CARGA" },
                    new SelectListItem { Selected = usuarioOtd.Perfil == "SUPERVISOR", Text = "Supervisor Aerolínea", Value =  "SUPERVISOR"},
                    new SelectListItem { Selected = usuarioOtd.Perfil == "SUPERVISOR CARGA", Text = "Supervisor Aerolínea de Carga", Value = "SUPERVISOR CARGA" }
                };

                ViewData["Aerolineas"] = respuesta.OrderBy(x => x.Sigla).Select(n => new SelectListItem
                {
                    Value = n.Nombre.ToString(),
                    Text = String.Concat(n.Sigla.ToString(), " -- ", n.Nombre.ToString()),
                    Selected = usuarioOtd.Aerolinea == n.Nombre.ToString().Trim()
                }).ToList();
            }

            ViewBag.Aerolinea = usuarioOtd.Aerolinea;

            return PartialView(usuarioOtd);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarUsuario(UsuarioOtd usuarioOtd, string estado)
        {

            //string rutaRelativa = string.Format(configuration.GetSection("URIs:AdmonGeneralEliminarUsuario").Value, usuarioOtd.Id);
            string rutaRelativa = string.Format(configuration.GetSection("URIs:AdmonGeneralEliminarUsuario").Value, usuarioOtd.Id);
            await servicioApi.PostAsync<string>(rutaRelativa, usuarioOtd.Id);

            string mensaje = "Usuario eliminado exitosamente.";

            return RedirectToAction("Principal", new { mensaje = mensaje });
        }

        public async Task<bool> ActualizarVuelo(OperacionVueloOtd operacionVueloOtd)
        {
            string rutaRelativa = configuration.GetSection("URIs:VuelosActualizarVuelo").Value;
            return await servicioApi.PostAsync<bool>(rutaRelativa, operacionVueloOtd);
        }

        public async Task<OperacionVueloOtd> ObtenerAsync(int id)
        {
            string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosObtener").Value, id);
            OperacionVueloOtd respuesta = await servicioApi.GetAsync<OperacionVueloOtd>(rutaRelativa);

            return respuesta;
        }


        public async Task EnviarCorreosNotificacioVuelos(int idAreolinea , DateTime Fecha)
        {
            //_logger.LogInformation($"Inicio notificación 6");
            //_logger.LogInformation($"Notificación 6, Id Aeroliniea = " + idAreolinea);
            var emailUser = User.Claims.Where(c => c.Type.Equals("UsuarioEmail")).FirstOrDefault().Value;
            try
            {

                string ConCopia = "";
                if (configuration.GetSection("SendGrid:cc").Value != null)
                {
                    ConCopia = configuration.GetSection("SendGrid:cc").Value;
                }
                //string rutaRelativa = string.Format(configuration.GetSection("URIs:VuelosPendientesProcesados").Value, idAreolinea, Fecha.ToShortDateString());
                //int respuesta = await servicioApi.GetAsync<int>(rutaRelativa);
                //  _logger.LogInformation($"Cantidad vuelos pendientes procesados =" + respuesta);


                //if (respuesta == 0)
                //{
                //    _logger.LogInformation($"Ingresa notificación 6, Vuelos pendientes Procesados = " + respuesta);
                //esto es lo que llega...
                IEnumerable<UsuarioOtd> usuarios = await servicioApi.GetAsync<IEnumerable<UsuarioOtd>>(configuration.GetSection("URIs:UsuariosObtenerTodos").Value).ConfigureAwait(false);
                    string file = "https://" + Request.Host + "/images/firma.png";
                

                var ListaUsuarios = usuarios.Where(x => x.UsuarioAerolinea.Any() 
                && (x.Perfil.Equals("AEROLINEA") || x.Perfil.Equals("SUPERVISOR"))
                && (x.Activo==true));
                var ListaEmail = ListaUsuarios.Where(y => y.UsuarioAerolinea.Any(r => r.IdAerolinea.Equals(idAreolinea)))
                    .Distinct().GroupBy(x => x.Id).Select(x => x.First());


                foreach (var datosUsuario in ListaEmail)
                {
                    string Mensaje = "<p>Buen día estimada aerolínea,</p></br></br></br><p>Nos permitimos informarles que sus vuelos con fecha " + Fecha.ToString("dd/MM/yyyy") + " han sido revisados, ya puede proceder a generar la respectiva infrasa, en el caso de detectar novedades o cobros, realizar la solicitud en el módulo contáctenos</p></br></br><p>Cordialmente,</p></br><b> Equipo de facturación </b></br><b> OPAIN S.A.</b></br> <p><img src='" + file + "' width='250' height='120'/><p><p></p> </br> Bogotá D.C. - Colombia </br>www.eldorado.aero </br>Call Center Aeropuerto(24 horas) Tel. + 57(1) 266 2000 ";


                    await SendMail("Vuelos Procesados Notificacion 6",
                    datosUsuario.Email.Trim(),
                    ConCopia, Mensaje);

                                      
                 
                    _logger.LogInformation("EnviarCorreosNotificacioVuelos notificacion 6 a:  " + datosUsuario.Email.Trim());
                }  
            }
            catch (Exception ex)
            {
                _logger.LogError("Error en:EnviarCorreosNotificacioVuelos " + ex, ResourceMessage.ErrorSystem);
            }
        }


        [HttpPost]
        public async Task<IActionResult> EnviarNoti(List<Registros> registros)
        {     
            try
            {
                OperacionVueloOtd _OperacionVueloOtd = new OperacionVueloOtd();

                var agruparPorFechayIdAerolinea = registros.GroupBy(x => new { x.FechaVuelo, x.IdAerolinea }).Distinct().ToList();

                List<int> _listaAerolineas = new List<int>();

                if (agruparPorFechayIdAerolinea != null && agruparPorFechayIdAerolinea.Count() > 0)
                {
                    foreach (var registroItem in agruparPorFechayIdAerolinea)
                    {
                        int c = 0;
                        foreach (var enviar in registroItem)
                        {
                            _OperacionVueloOtd = await ObtenerAsync(int.Parse(enviar.Id));
                            _OperacionVueloOtd.EnvioNotificacion = 1.ToString();
                            await ActualizarVuelo(_OperacionVueloOtd);
                            _logger.LogInformation("EnviarNoti: Numero de vuelo actualizado "+ enviar.Id);

                            if (c == 0)
                            {
                            await EnviarCorreosNotificacioVuelos(_OperacionVueloOtd.IdAerolinea, _OperacionVueloOtd.Fecha);
                                c++;
                            }
                            
                        }
                    }
                }
                return RedirectToAction("Principal");
             }
            catch (Exception ex)
            {
                _logger.LogError("Error en:EnviarNoti "  + ex, ResourceMessage.ErrorSystem);
                throw;
            }
        }

        private async Task SendMail(string _Asunto, string _Para, string _ConCopia, string _Mensaje)
        {
            string urlAPI = configuration.GetSection("URIs:EmailSender").Value;
            Opain.Jarvis.Dominio.Entidades.Notificacion notificacion = new Notificacion
            {
                Asunto = _Asunto,
                Destinos = _Para,
                Copias = _ConCopia,
                Cuerpo = _Mensaje
            };

            bool result = await servicioApi.PostAsync<bool>(urlAPI, notificacion);
        }
    }
}