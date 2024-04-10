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
    public class ForgotPasswordModel : PageModel
    {
        //private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;
        public IConfiguration Configuration { get; }
        private readonly IServicioApi servicioApi;
        public ForgotPasswordModel( IEmailSender emailSender, IServicioApi api,  IConfiguration conf)
        {
            //_userManager = userManager;
            _emailSender = emailSender;
            servicioApi = api;
            Configuration = conf;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                string rutaRelativa = string.Format(Configuration.GetSection("URIs:emailExiste").Value, Input.Email);
                bool respuesta = await servicioApi.GetAsync<bool>(rutaRelativa).ConfigureAwait(false);
                if (respuesta == false)
                {
                    //    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
                string rutaUsuarios = string.Format(Configuration.GetSection("URIs:UsuariosConsultarPorEmail").Value, Input.Email);
                UsuarioOtd userDetalles = await servicioApi.GetAsync<UsuarioOtd>(rutaUsuarios).ConfigureAwait(false);
                

                ////var user = await _userManager.FindByEmailAsync(Input.Email);
                //if (usuarioDetalle == null || !(await usuarioDetalle.EmailConfirmed ==1))
                //{
                //    // Don't reveal that the user does not exist or is not confirmed
                //    return RedirectToPage("./ForgotPasswordConfirmation");
                //}

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPass",
                    pageHandler: null,
                    values: new { userDetalles.Email },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Restablecer contraseña",
                    $"Por favor restablezca su contraseña dando <a href='"+ callbackUrl+ "'>click aqui.</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}