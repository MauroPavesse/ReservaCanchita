using MercadoPago.Client.Preference;

namespace ReservaCanchita.Services.MercadoPago
{
    public class MercadoPagoServicio
    {
        public MercadoPagoServicio()
        {
        }

        public async Task<string?> CrearLinkPagoAsync(decimal monto, int reservaId, string pageUrl)
        {
            try
            {
                var successUrl = pageUrl + "/MercadoPago/Exito";
                var failureUrl = pageUrl + "/MercadoPago/Error";
                var pendingUrl = pageUrl + "/MercadoPago/Pendiente";
                var notificationUrl = pageUrl + "/api/mercadopago/notificaciones";

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

                // Opcional: guardar el ID de la preferencia para rastrear el pago luego
                // preference.Id

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
