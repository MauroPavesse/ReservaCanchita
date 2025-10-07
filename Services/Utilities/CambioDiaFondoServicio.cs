using ReservaCanchita.Services.Canchas;
using ReservaCanchita.Services.HorariosDisponibles;

namespace ReservaCanchita.Services.Utilities
{
    public class CambioDiaFondoServicio : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CambioDiaFondoServicio(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken tokenFrenado)
        {
            while (!tokenFrenado.IsCancellationRequested)
            {
                var hoy = DateTime.Now;
                var fechaObjetivo = new DateTime(hoy.Year, hoy.Month, hoy.Day).AddDays(1);

                var tiempoHastaElSiguienteDia = fechaObjetivo - hoy;
                await Task.Delay(tiempoHastaElSiguienteDia, tokenFrenado);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var horarioDisponibleService = scope.ServiceProvider.GetRequiredService<HorarioDisponibleServicio>();
                    var canchaService = scope.ServiceProvider.GetRequiredService<CanchaServicio>();

                    var canchas = await canchaService.ObtenerTodo(new BuscadorEntrada());
                    foreach (var cancha in canchas)
                    {
                        await horarioDisponibleService.GenerarHorariosDisponibles(cancha.Id, 15, 10);
                    }
                }
            }
        }
    }
}
