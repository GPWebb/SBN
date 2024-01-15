using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SBN.Lib.Definitions;

namespace SBN
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appSettings.json");
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.ReturnHttpNotAcceptable = true;
                    options.InputFormatters.Add(new XmlSerializerInputFormatter(options));
                    options.OutputFormatters.Insert(0, new XmlDataContractSerializerOutputFormatter());
                    options.OutputFormatters.Add(new StringOutputFormatter());
                    options.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
                    options.FormatterMappings.SetMediaTypeMappingForFormat("html", "text/html");
                    options.EnableEndpointRouting = false;
                })
                .AddXmlDataContractSerializerFormatters()
                .AddSessionStateTempDataProvider();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSession();

            services.AddHttpClient();

            Config.RegisterComponents(services);

            services.Configure<EnvironmentConfig>(Configuration.GetSection("EnvironmentConfig"));

            //https://stackoverflow.com/questions/47735133/asp-net-core-synchronous-operations-are-disallowed-call-writeasync-or-set-all
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "NewSessionRoute",
                    pattern: Routes.NewSession,
                    defaults: new { controller = "Session", action = "New" }
                );
                endpoints.MapControllerRoute(
                    name: "LoginRoute",
                    pattern: Routes.Login,
                    defaults: new { controller = "LogInOut", action = "Login" }
                );
                endpoints.MapControllerRoute(
                    name: "LogoutRoute",
                    pattern: Routes.Logout,
                    defaults: new { controller = "LogInOut", action = "Logout" }
                );
                endpoints.MapControllerRoute(
                    name: "DocumentReadRoute",
                    pattern: "api/documents/DocumentType/{DocumentTypeID}/{DocumentID}/Read",
                    defaults: new { contorller = "Document", action = "Read" }
                );
                endpoints.MapControllerRoute(
                    name: "ApiRoute",
                    pattern: "api/{*url}",
                    defaults: new { controller = "Api", action = "Call" }
                );
                endpoints.MapControllerRoute(
                    name: "AssetIDRoute",
                    pattern: "asset/ID/{ID}/{VariantSuffix}",
                    defaults: new { controller = "Asset", action = "CallByIDAndVariant" }
                );
                endpoints.MapControllerRoute(
                    name: "AssetRoute",
                    pattern: "asset/{*url}",
                    defaults: new { controller = "Asset", action = "Call" }
                );
                endpoints.MapControllerRoute(
                    name: "PageRoute",
                    pattern: "{*url}",
                    defaults: new { controller = "Page", action = "Display" }
                );
            });
        }
    }
}