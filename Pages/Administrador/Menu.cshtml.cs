using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ReservaCanchita.Models;
using ReservaCanchita.Services.Comidas;
using ReservaCanchita.Services.ComidasCategorias;

namespace ReservaCanchita.Pages.Administrador
{
    public class MenuModel : PageModel
    {
        private ComidaCategoriaService comidaCategoriaService;
        private ComidaService comidaService;

        public MenuModel(ComidaCategoriaService comidaCategoriaService, ComidaService comidaService)
        {
            this.comidaCategoriaService = comidaCategoriaService;
            this.comidaService = comidaService;
        }

        [BindProperty]
        public List<ComidaCategoria> ComidasCategorias { get; set; } = new List<ComidaCategoria>();
        [BindProperty]
        public List<Comida> Comidas { get; set; } = new List<Comida>();

        public async Task OnGet()
        {
            ComidasCategorias = await comidaCategoriaService.ObtenerTodo();
            Comidas = await comidaService.ObtenerTodo();
        }

        public async Task<IActionResult> OnPost(string Nombre, string Descripcion, decimal Precio, int CategoriaId)
        {
            var comida = new Comida
            {
                Nombre = Nombre,
                Descripcion = Descripcion.Length > 0 ? Descripcion : null,
                Importe = Precio,
                ComidaCategoriaId = CategoriaId > 0 ? CategoriaId : null
            };

            await comidaService.Guardar(comida);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAgregarCategoria(string Nombre)
        {
            var categoria = new ComidaCategoria { Nombre = Nombre };

            await comidaCategoriaService.Guardar(categoria);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetObtenerComida(int id)
        {
            var comida = await comidaService.ObtenerPorId(id);
            if (comida == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                id = comida.Id,
                nombre = comida.Nombre,
                descripcion = comida.Descripcion,
                precio = comida.Importe,
                categoriaId = comida.ComidaCategoriaId == null ? 0 : comida.ComidaCategoriaId
            });
        }


        public async Task<IActionResult> OnPostActualizarComida(int id, string nombre, string? descripcion, decimal precio, int categoriaId)
        {
            var comida = await comidaService.ObtenerPorId(id);
            if (comida != null)
            {
                comida.Nombre = nombre;
                comida.Descripcion = descripcion != null && descripcion.Length > 0 ? descripcion : null;
                comida.Importe = precio;
                comida.ComidaCategoriaId = categoriaId > 0 ? categoriaId : null;

                await comidaService.Guardar(comida);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarComida(int id)
        {
            var comida = await comidaService.ObtenerPorId(id);
            if (comida != null)
            {
                await comidaService.Eliminar(comida);
            }
            return RedirectToPage(); // Recarga la página
        }

    }
}
