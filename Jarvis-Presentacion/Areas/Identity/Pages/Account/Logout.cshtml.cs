using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Opain.Jarvis.Dominio.Entidades;
using System.Threading.Tasks;

namespace Opain.Jarvis.Presentacion.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        //private readonly SignInManager<Usuario> signInManager;

        public LogoutModel(ILogger<LogoutModel> logger)
        {
            //signInManager = sm;
            _logger = logger;
        }

        //public void OnGet()
        //{
        //}

        public async Task<IActionResult> OnGet()
        {
            //await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();

            _logger.LogInformation("User logged out.");
            return RedirectToPage("Login");
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            //await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage("Login");
            }
        }
    }
}