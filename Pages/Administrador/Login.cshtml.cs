using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservaCanchita.Data;

namespace ReservaCanchita.Pages.Administrador
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Usuario { get; set; } = string.Empty;

        [BindProperty]
        public string Clave { get; set; } = string.Empty;

        public string Error { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var admin = _context.Cuentas.FirstOrDefault(a => a.Usuario == Usuario && a.Clave == Clave);

            if (admin != null)
            {
                // Guardar sesión (simple por ahora)
                HttpContext.Session.SetString("Admin", admin.Usuario);
                return RedirectToPage("Dashboard");
            }

            Error = "Usuario o contraseña incorrectos";
            return Page();
        }
    }
}
