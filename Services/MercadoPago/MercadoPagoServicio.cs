using MercadoPago.Client.Preference;
using MercadoPago.Config;
using ReservaCanchita.Services.Configuraciones;
using ReservaCanchita.Services.PagosMercadoPago;

namespace ReservaCanchita.Services.MercadoPago
{
    public class MercadoPagoServicio
    {
        private readonly PagoMercadoPagoServicio pagoMercadoPagoServicio;
        private readonly ConfiguracionServicio configuracionServicio;

        public MercadoPagoServicio(PagoMercadoPagoServicio pagoMercadoPagoServicio, ConfiguracionServicio configuracionServicio)
        {
            this.pagoMercadoPagoServicio = pagoMercadoPagoServicio;
            this.configuracionServicio = configuracionServicio;
        }

        public async Task<string?> CrearLinkPagoAsync(decimal monto, int reservaId, HttpRequest httpRequest)
        {
            try
            {
                var successUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/MercadoPago/Exito";
                var failureUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/MercadoPago/Error";
                var pendingUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/MercadoPago/Pendiente";
                var notificationUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/api/mercadopago/notificaciones";

                var config = await configuracionServicio.ObtenerConfiguracion("AccessTokenMp");
                var accessToken = config.ValorString;

                MercadoPagoConfig.AccessToken = accessToken;

                var request = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                    {
                        new PreferenceItemRequest
                        {
                            Title = $"Reserva #{reservaId}",
                            Quantity = 1,
                            CurrencyId = "ARS",
                            UnitPrice = monto
                        }
                    },
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = successUrl,
                        Failure = failureUrl,
                        Pending = pendingUrl
                    },
                    AutoReturn = "approved",
                    ExternalReference = reservaId.ToString(),
                    NotificationUrl = notificationUrl
                };

                var client = new PreferenceClient();
                var preference = await client.CreateAsync(request);

                await pagoMercadoPagoServicio.Guardar(new Models.PagoMercadoPago()
                {
                    PreferenceId = preference.Id,
                    ExternalReference = reservaId.ToString(),
                    InitPoint = preference.InitPoint,
                    Status = "open",
                    DateCreated = preference.DateCreated ?? DateTime.UtcNow
                });

                return preference.InitPoint;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando link de pago: {ex.Message}");
                return null;
            }
        }

    }
}
