using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReservaCanchita.Pages.Administrador
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Remove("Admin"); // Elimina la sesión
            return RedirectToPage("/Administrador/Login"); // Redirige al login
        }
    }
}
