using System;
using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;
using WebApiTemplate.Extensions;
using WebApiTemplate.Middleware;

namespace WebApiTemplate
{
    public class Startup
    {
        //modified the configuration
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Open Postgres Connection
            services.AddScoped<DbConnection, NpgsqlConnection>(provider => new NpgsqlConnection
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")
            });

            services.AddCors(); // add cors to allow and disallowed cors

            // Add services here
            services.AddApplicationServices(_config);
            // End of services

            services.AddControllers();

            // Add api version
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            // Register the Swagger Generator service. This service is responsible for genrating Swagger Documents.
            // Note: Add this service at the end after AddMvc() or AddMvcCore().
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ASP NET Core 3.1 API",
                    Version = "v1",
                    Description = "Description for the API goes here.",
                    Contact = new OpenApiContact
                    {
                        Name = "Nur Akmal Bin Jalil",
                        Email = "nurakmaljalil91@gmail.com",
                        Url = new Uri("https://nurakmaljalil.com/"),
                    },
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP NET Core 3.1 API V1");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = string.Empty;
            });

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging(); // For serilog to log all request

            app.UseRouting();

            app.UseCors(
                options =>
                    options.WithOrigins("http://178.128.212.132", "http://localhost:8080")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                ); // This rule allow any origin

            //app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // This rule allow any origin

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
