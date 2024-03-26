using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Opain.Jarvis.Dominio.Entidades;
using Microsoft.Extensions.Configuration;
using Opain.Jarvis.Presentacion.Web.Helpers;
using Opain.Jarvis.Dominio.Entidades;
namespace Opain.Jarvis.Presentacion.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPassModel : PageModel
    {

        public IConfiguration Configuration { get; }
        private readonly IServicioApi servicioApi;
        public ResetPassModel(IServicioApi api, IConfiguration conf)
        {
            servicioApi = api;
            Configuration = conf;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password1 { get; set; }
            [DataType(DataType.Password)]
            public string Password2 { get; set; }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (Input.Password1 == null) {
                ViewData["Confirmacion"] = "La contraseña es requerida";
                return Page();
            }

            if (Input.Password2 == null)
            {
                ViewData["Confirmacion"] = "La confirmacion de contraseña es requerida";
                return Page();
            }

            if (Input.Password1 != Input.Password2)
            {
                ViewData["Confirmacion"] = "Las contraseñas deben ser iguales";
                return Page();
            }
            else {
                string rutaUsuarios = string.Format(Configuration.GetSection("URIs:UsuariosConsultarPorEmail").Value, HttpContext.Request.Query["Email"]);
                UsuarioOtd userDetalles = await servicioApi.GetAsync<UsuarioOtd>(rutaUsuarios).ConfigureAwait(false);
                string rutaRelativa = string.Format(Configuration.GetSection("URIs:UsuariosActualizarClave").Value, userDetalles.UserName,Input.Password1 );
                bool respuesta = await servicioApi.GetAsync<bool>(rutaRelativa).ConfigureAwait(false);
                if (respuesta == false)
                {
                    ViewData["Confirmacion"] = "No se pudo actualizar la clave, debe contener caracteres especiales y números";
                    return Page();
                }
                else
                {
                    ViewData["Confirmacion"] = "1";
                    return Page();
                }
            }


        }


    }
}