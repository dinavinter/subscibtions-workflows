using System;
using System.Configuration;
using System.IO;
using AspNetCoreDemoApp.Activities;
using AspNetCoreDemoApp.Providers.ActivityTypes;
using AspNetCoreDemoApp.Providers.Extensions;
using CommunicationPreferences.Workflow;
using Elsa;
using Elsa.Activities.Conductor.Extensions;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreDemoApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages( );

            var elsaSection = Configuration.GetSection("Elsa");

            // Elsa services.
            services
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
                    // .AddConsoleActivities()

                     .AddXStateActivities(options => elsaSection.GetSection("XState").Bind(options))
                   .AddHttpActivities(elsaSection.GetSection("Server").Bind)
                    .AddActivity<UpdateAccountAttributes>()
                    .AddQuartzTemporalActivities()
                    .AddJavaScriptActivities()
                    .AddCommunicationPreferences()

                );

            // Elsa API endpoints.
            services
                .AddElsaSwagger()
                .AddElsaApiEndpoints();

            services
                .AddHttpsRedirection(options => { options.HttpsPort = 443; })
                .AddMvcCore()
                .AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithExposedHeaders("Content-Disposition"));
                });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                           ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DYNO")))
            {
                Console.WriteLine("Use https redirection");
                app.UseHttpsRedirection();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elsa"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elsa"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpActivities();
            app.UseRouting();
            app.UseAuthorization();
            app.UseHttpActivities();


            app.UseEndpoints(endpoints =>
                {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();

                    app.UseEndpoints(endpoints => { endpoints.MapFallbackToPage("/_Host"); });

                    endpoints.MapRazorPages();

                })
                .UseDefaultFiles()
                .UseStaticFiles()



                .UseCors("CorsPolicy")
                .UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.WebRootPath, "_api")),
                RequestPath = "/_api",
                EnableDirectoryBrowsing = true
            });
        }
    }
}