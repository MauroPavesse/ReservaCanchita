using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservaCanchita.Models;
using ReservaCanchita.Services.Configuraciones;

namespace ReservaCanchita.Pages.Administrador
{
    public class ConfiguracionModel : PageModel
    {
        private readonly ConfiguracionServicio configuracionService;

        public ConfiguracionModel(ConfiguracionServicio configuracionService)
        {
            this.configuracionService = configuracionService;
        }

        [BindProperty] public string NombreEmpresa { get; set; } = "";
        [BindProperty] public string NumeroContacto { get; set; } = "";
        [BindProperty] public bool MuestraValorReserva { get; set; } = false;
        [BindProperty] public decimal? ValorReserva { get; set; } = 0;
        [BindProperty] public bool TrabajaConToleranciaCancelarReserva { get; set; } = false;
        [BindProperty] public int? HoraToleranciaCancelarReserva { get; set; } = 0;
        [BindProperty] public bool VersionWhatsApp { get; set; } = false;
        [BindProperty] public bool TrabajaConWhatsApp { get; set; } = false;
        [BindProperty] public bool VersionMenuComidas { get; set; } = false;
        [BindProperty] public bool TrabajaConMenuComidas { get; set; } = false;
        [BindProperty] public bool VersionIncluyeMercadoPago { get; set; } = false; 
        [BindProperty] public bool TrabajaConSena { get; set; } = false;
        [BindProperty] public string AliasMercadoPago { get; set; } = string.Empty;
        [BindProperty] public string AccessTokenMercadoPago { get; set; } = string.Empty;

        public async Task OnGet()
        {
            var configuraciones = await configuracionService.ObtenerConfiguraciones(new List<string>());

            NombreEmpresa = configuraciones.FirstOrDefault(x => x.Campo == "NombreEmpresa")?.ValorString ?? "";
            NumeroContacto = configuraciones.FirstOrDefault(x => x.Campo == "NumeroContacto")?.ValorString ?? "";

            MuestraValorReserva = configuraciones.FirstOrDefault(x => x.Campo == "MuestraValorReserva")?.ValorNumerico == 1;
            ValorReserva = configuraciones.FirstOrDefault(x => x.Campo == "ValorReserva")?.ValorNumerico;

            TrabajaConToleranciaCancelarReserva = configuraciones.FirstOrDefault(x => x.Campo == "TrabajaConToleranciaCancelarReserva")?.ValorNumerico == 1;
            HoraToleranciaCancelarReserva = Convert.ToInt32(configuraciones.FirstOrDefault(x => x.Campo == "HoraToleranciaCancelarReserva")?.ValorNumerico);

            TrabajaConWhatsApp = configuraciones.FirstOrDefault(x => x.Campo == "TrabajaConWhatsapp")?.ValorNumerico == 1;
            VersionWhatsApp = configuraciones.FirstOrDefault(x => x.Campo == "VersionIncluyeWhatsapp")?.ValorNumerico == 1;

            TrabajaConMenuComidas = configuraciones.FirstOrDefault(x => x.Campo == "TrabajaConMenuComidas")?.ValorNumerico == 1;
            VersionMenuComidas = configuraciones.FirstOrDefault(x => x.Campo == "VersionIncluyeMenuComidas")?.ValorNumerico == 1;

            VersionIncluyeMercadoPago = configuraciones.FirstOrDefault(x => x.Campo == "VersionIncluyeMercadoPago")?.ValorNumerico == 1;
            TrabajaConSena = configuraciones.FirstOrDefault(x => x.Campo == "TrabajaConSena")?.ValorNumerico == 1;
            AliasMercadoPago = configuraciones.FirstOrDefault(x => x.Campo == "AliasMp")?.ValorString ?? "";
            AccessTokenMercadoPago = configuraciones.FirstOrDefault(x => x.Campo == "AccessTokenMp")?.ValorString ?? "";
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var configuraciones = new List<Configuracion>
            {
                new Configuracion { Id = 1, Campo = "NombreEmpresa", ValorString = NombreEmpresa },
                new Configuracion { Id = 2, Campo = "NumeroContacto", ValorString = NumeroContacto },
                new Configuracion { Id = 4, Campo = "ValorReserva", ValorNumerico = ValorReserva ?? 0 },
                new Configuracion { Id = 5, Campo = "MuestraValorReserva", ValorNumerico = Convert.ToDecimal(MuestraValorReserva) },
                new Configuracion { Id = 6, Campo = "HoraToleranciaCancelarReserva", ValorNumerico = HoraToleranciaCancelarReserva ?? 0 },
                new Configuracion { Id = 7, Campo = "TrabajaConToleranciaCancelarReserva", ValorNumerico = Convert.ToDecimal(TrabajaConToleranciaCancelarReserva) },
                new Configuracion { Id = 8, Campo = "TrabajaConWhatsapp", ValorNumerico = Convert.ToInt32(TrabajaConWhatsApp) },
                new Configuracion { Id = 10, Campo = "TrabajaConMenuComidas", ValorNumerico = Convert.ToInt32(TrabajaConMenuComidas) },
                new Configuracion { Id = 13, Campo = "TrabajaConSena", ValorNumerico = Convert.ToInt32(TrabajaConSena) },
                new Configuracion { Id = 14, Campo = "AccessTokenMp", ValorString = AccessTokenMercadoPago },
                new Configuracion { Id = 15, Campo = "AliasMp", ValorString = AliasMercadoPago }
            };
            await configuracionService.GuardarConfiguraciones(configuraciones);

            return RedirectToPage();
        }
    }
}
