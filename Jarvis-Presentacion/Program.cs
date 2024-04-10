

using Opain.Jarvis.Presentacion.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    // Agregar convención de ruta personalizada para el Razor Page Index.cshtml
    //options.Conventions.AddPageRoute("/Pages", "");
    options.Conventions.AddPageRoute("/Index", "mycustomroute");

}).AddRazorRuntimeCompilation();

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddTransient<IServicioApi, ServicioApi>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Admon",
    pattern: "{area:exists}/{controller=AdmonGeneral}/{action=Index}/{id?}");

app.Run();
