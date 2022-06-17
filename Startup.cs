using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Router.Configuration;
using Router.DependencyInjection;
using Router.Services;
using SCM.CisApi.WebApi.DependencyInjection;

namespace Router
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
            services.Configure<RouterConfig>(Configuration.GetSection("RouterConfig"));
            services.Configure<TokenConfig>(Configuration.GetSection("TokenConfig"));

            /// Add CORS
            services.AddCorsPolicy();

            services.AddControllers();
            //services.AddEndpointsApiExplorer();
            
            /// Add Swagger 
            services.AddSwagger();

            services.AddAppServices(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Router v1"));
            }

            /// Use Swagger
            app.UseVersionedSwagger(Configuration);

            app.UseHttpsRedirection();

            app.UseRouting();

            /// Use CORS
            app.UseCorsPolicy();

            //  app.UseAuthenticationFilter();
            //app.UseAuthentication();
            app.UseCustomAuthentication(Configuration);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
