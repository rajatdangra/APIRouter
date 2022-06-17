using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Router.Configuration;
using Router.Interfaces;
using Router.Services;
using System.Text;
using System;
using Microsoft.AspNetCore.Builder;

namespace Router.DependencyInjection
{
    public static class AppServicesExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddHttpClient<RouterService>();
            //services.AddHttpClient<JwtTokenService>();

            #region Configure Basic Authentication
            //services.AddAuthentication("BasicAuthentication")
            //    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            #endregion

            //services.AddAuthentication();
            //services.AddAuthorization();

            bool isAuthenticationDisabled = Convert.ToBoolean(configuration["RouterConfig:DisableAuthentication"]);
            if (!isAuthenticationDisabled)
            {
                #region Configure JWT Authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["TokenConfig:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["TokenConfig:Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenConfig:Key"]))
                    };
                });
                #endregion
            }
            else
            {
                #region Configure Basic Authentication
                services.AddAuthentication("BasicAuthentication")
                    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
                #endregion
            }

            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRouterService, RouterService>();
            services.AddScoped<ITokenService, JwtTokenService>();

            return services;
        }

        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app, IConfiguration configuration)
        {
            bool isAuthenticationDisabled = Convert.ToBoolean(configuration["RouterConfig:DisableAuthentication"]);
            //if (!isAuthenticationDisabled)
            {
                app.UseAuthentication();
            }
            return app;
        }
    }
}
