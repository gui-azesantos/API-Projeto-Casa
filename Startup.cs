using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_Projeto_Casa.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API_Projeto_Casa {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddControllers ();
            services.AddDbContext<ApplicationDbContext> (options => options.UseMySql (Configuration.GetConnectionString ("DefaultConnection")));

            //Swagger
            services.AddSwaggerGen (config => {
                config.SwaggerDoc ("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API de Produtos", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
            });
            app.UseSwagger (config => {
                config.RouteTemplate = "eventos.com/{documentName}/swagger.json";
            }); //Gerar um arquivo JSON - Swagger.json
            app.UseSwaggerUI (config => { //View HTML do Swagger
                config.SwaggerEndpoint ("/eventos.com/v1/swagger.json", "v1 docs");
            });
        }
    }
}