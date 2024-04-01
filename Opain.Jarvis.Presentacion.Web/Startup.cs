using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Opain.Jarvis.Dominio.Entidades;
using Opain.Jarvis.EnvioCorreos;
using Opain.Jarvis.Presentacion.Web.Bussiness;
using Opain.Jarvis.Presentacion.Web.Helpers;
using Serilog;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Opain.Jarvis.Presentacion.Web
{
    public class Startup
    {
        //private readonly IConfiguration configuration;
         
        public Startup(IConfiguration configuration )
        {
            Configuration = configuration;
      
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public  void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizePage("/Contact");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMvc().AddRazorOptions(options => options.AllowRecompilingViewsOnFileChange = true);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAutoMapper(x => x.AddProfile(new PerfilMapeos()));

            var appSettingsSection = Configuration.GetSection("Config");
            services.Configure<AppSettings>(appSettingsSection);

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.CheckConsentNeeded = context => false;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            //try
            //{
            //    string rutaFn = Configuration.GetSection("URIs:pathCxString").Value;
            //    HttpClient servicio = new HttpClient();
            //    var urlServicio = Configuration.GetSection("Rutas:BaseServicio").Value;
            //    var server = urlServicio;
            //    servicio.BaseAddress = new Uri(server);
            //    servicio.DefaultRequestHeaders.Accept.Clear();
            //    servicio.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    var resultados = servicio.GetAsync(rutaFn).Result.Content.ReadAsStringAsync().Result;
            //    string cadena = resultados.Substring(1, resultados.Length - 2).ToString();
            //    // desencriptar
            //    string cadenaOriginal =Seguridad.Desencriptar(cadena);


            //    services.AddDbContext<ContextoOpain>(options => options.UseMySQL(cadenaOriginal));
            //}
            //catch (Exception es)
            //{
            //    throw;
            //}
            services.AddTransient<IClaimsTransformation, JarvisClaims>();
            //services.AddDbContext<ContextoOpain>(options => options.UseMySQL("Cadenita"));
            //services.AddDbContext<ContextoOpain>(options => options.UseMySQL(Configuration.GetConnectionString("ConexionJarvisBD")));


            //services.AddIdentity<Usuario, Rol>(config =>
            //{
            //    config.SignIn.RequireConfirmedEmail = false;
            //})
            //    .AddDefaultUI(UIFramework.Bootstrap4)
            //    .AddEntityFrameworkStores<ContextoOpain>().AddDefaultTokenProviders();



            services.Configure<DataProtectionTokenProviderOptions>(o =>
                o.TokenLifespan = TimeSpan.FromHours(3));

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddScoped<IServicioApi, ServicioApi>();
            services.AddScoped<IEmail, EmailSender>();

            services.AddScoped<ServicioOracle>();

            services.AddScoped<ServicioComboBox>();

            var sendGridConfiguracion = Configuration.GetSection("SendGrid");
            services.AddAuthorization();
            services.AddMemoryCache();

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
        }

    

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var appSett = Configuration.GetConnectionString("MostrarErrIU");

            string debug = appSett;

            if (debug.Equals("false"))
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }
            }
            else {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStatusCodePages();
            app.UseStatusCodePagesWithRedirects("/Home/Error");
            //app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Administracion",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "CargaInformacion",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "TransitoConexion",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "Consulta",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "Contactenos",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "Informes",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
            });

            /*app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                await next();
            });*/


        }
    }
}
