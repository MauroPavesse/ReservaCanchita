using Microsoft.EntityFrameworkCore;
using ReservaCanchita.Data;
using ReservaCanchita.Services.Canchas;
using ReservaCanchita.Services.Comidas;
using ReservaCanchita.Services.ComidasCategorias;
using ReservaCanchita.Services.Configuraciones;
using ReservaCanchita.Services.HorariosDisponibles;
using ReservaCanchita.Services.Utilities;
using ReservaCanchita.Services.WhatsApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSession();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<CanchaServicio>();
builder.Services.AddScoped<ComidaServicio>();
builder.Services.AddScoped<ComidaCategoriaServicio>();
builder.Services.AddScoped<ConfiguracionServicio>();
builder.Services.AddScoped<HorarioDisponibleServicio>();
builder.Services.AddHostedService<CambioDiaFondoServicio>();
builder.Services.AddHttpClient<WhatsAppServicio>();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate();
    ReservaCanchita.Data.DbInitializer.Seed(context);
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
