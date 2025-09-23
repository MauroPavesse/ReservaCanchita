using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservaCanchita.Models;
using ReservaCanchita.Services.Comidas;
using ReservaCanchita.Services.ComidasCategorias;

namespace ReservaCanchita.Pages
{
    public class MenuModel : PageModel
    {
        private ComidaService comidaService;
        private ComidaCategoriaService comidaCategoriaService;

        public MenuModel(ComidaService comidaService, ComidaCategoriaService comidaCategoriaService)
        {
            this.comidaService = comidaService;
            this.comidaCategoriaService = comidaCategoriaService;
        }

        [BindProperty]
        public List<Comida> Comidas { get; set; } = new List<Comida>();
        [BindProperty]
        public List<ComidaCategoria> ComidasCategorias { get; set; } = new List<ComidaCategoria>();

        public async Task OnGet()
        {
            Comidas = await comidaService.ObtenerTodo();
            ComidasCategorias = await comidaCategoriaService.ObtenerTodo();
        }
    }
}
