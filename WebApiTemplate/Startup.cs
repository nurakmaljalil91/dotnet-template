using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Serilog;
using WebApiTemplate.Services;

namespace WebApiTemplate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Open Postgres Connection
            services.AddScoped<DbConnection, NpgsqlConnection>(provider => new NpgsqlConnection
            {
                ConnectionString = Configuration.GetConnectionString("DefaultConnection")
            });

            services.AddCors(); // add cors to allow and disallowed cors

            // Add services here
            services.AddScoped<ITodoRepository, TodoRepository>();
            // End of services

            services.AddControllers();

            // Add api version
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseCors(
                options => options.WithOrigins("http://178.128.212.132", "http://localhost:8080").AllowAnyMethod().AllowAnyHeader()
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
