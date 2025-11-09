using MercadoPago.Config;
using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Services.Canchas;
using ReservaCanchita.Services.Comidas;
using ReservaCanchita.Services.ComidasCategorias;
using ReservaCanchita.Services.Configuraciones;
using ReservaCanchita.Services.HorariosDisponibles;
using ReservaCanchita.Services.MercadoPago;
using ReservaCanchita.Services.PagosMercadoPago;
using ReservaCanchita.Services.Utilities;
using ReservaCanchita.Services.WhatsApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IConnectionStringResolver, ConnectionStringResolver>();
builder.Services.AddScoped<CanchaServicio>();
builder.Services.AddScoped<ComidaServicio>();
builder.Services.AddScoped<ComidaCategoriaServicio>();
builder.Services.AddScoped<ConfiguracionServicio>();
builder.Services.AddScoped<HorarioDisponibleServicio>();
builder.Services.AddHostedService<CambioDiaFondoServicio>();
builder.Services.AddHttpClient<WhatsAppServicio>();
builder.Services.AddScoped<MercadoPagoServicio>();
builder.Services.AddScoped<PagoMercadoPagoServicio>();

MercadoPagoConfig.AccessToken = builder.Configuration["MercadoPago:AccessToken"];

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var resolver = serviceProvider.GetRequiredService<IConnectionStringResolver>();
    var connectionString = resolver.GetConnectionString();
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

var connectionStrings = new[] {
    builder.Configuration.GetConnectionString("profutbol5"),
    builder.Configuration.GetConnectionString("demo"),
    builder.Configuration.GetConnectionString("DefaultConnection")
};

using (var scope = app.Services.CreateScope())
{
    foreach (var connStr in connectionStrings)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(connStr, ServerVersion.AutoDetect(connStr));

        using (var context = new AppDbContext(optionsBuilder.Options))
        {
            context.Database.Migrate();
            ReservaCanchita.Data.DbInitializer.Seed(context);
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
