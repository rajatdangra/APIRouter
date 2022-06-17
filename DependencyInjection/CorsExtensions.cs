using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.CisApi.WebApi.DependencyInjection
{
    public static class CorsExtensions
    {
        public const string CorsPolicyName = "APIRouter_CorsPolicy";

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    builder.AllowAnyOrigin()
                    //.AllowAnyHeader()
                    //.AllowAnyMethod()
                    ;
                });
            });

            return services;
        }
        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {

            app.UseCors(CorsPolicyName);

            return app;
        }
    }
}
