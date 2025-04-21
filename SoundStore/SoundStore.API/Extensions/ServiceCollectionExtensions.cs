using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SoundStore.API.Middlewares;
using SoundStore.Core.Commons;
using SoundStore.Core.Constants;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SoundStore.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Register(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                o.JsonSerializerOptions.WriteIndented = true;
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            services.AddEndpointsApiExplorer();

            services.AddHttpContextAccessor();

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SoundStore API",
                    Version = "v1",
                    Description = "API for SoundStore"
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token in the format: `Bearer {token}`"
                });

                o.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>() // No specific scopes required
                    }
                });
            });

            // Override the default response when using annotation to validate request model (400 code)
            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.SuppressModelStateInvalidFilter = true;
            });

            #region Add application variables
            services.Configure<JwtSettings>(configuration.GetRequiredSection("Jwt"));
            services.Configure<EmailSettings>(configuration.GetRequiredSection("EmailSettings"));
            services.Configure<CloudinarySettings>(configuration.GetRequiredSection("CloudinarySettings"));
            services.Configure<PayOSSettings>(configuration.GetRequiredSection("payOS"));
            #endregion


            var jwtSection = configuration.GetRequiredSection("Jwt");
            #region App configurations
            services.AddTransient<GlobalExceptionHandlingMiddleware>();

            services.AddScoped<UserClaimsMiddleware>();

            services.AddApiVersioning();

            services.AddCors();

            services.ConfigureAuthentication(jwtSection);
            #endregion
            
            #region Policy-based authorization
            services.AddAuthorization(o =>
            {
                o.AddPolicy("Customer", p =>
                {
                    p.RequireRole(UserRoles.Customer);
                });

                o.AddPolicy("Admin", p =>
                {
                    p.RequireRole(UserRoles.Admin);
                });

                o.AddPolicy("CustomerAndAdmin", p =>
                {
                    p.RequireRole(UserRoles.Customer, UserRoles.Admin);
                });
            });
            #endregion
            return services;
        }

        private static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1);
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'V";
                opt.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        private static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
            return services;
        }

        private static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
            IConfigurationSection jwtSection)
        {
            // Add authentication and authorization services here
            // Example: services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => { ... });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
            {
                var configKey = jwtSection["Key"] ?? string.Empty;
                var key = Encoding.UTF8.GetBytes(configKey);

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"]
                };
            });

            return services;
        }
    }
}
