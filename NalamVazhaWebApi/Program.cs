using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json.Serialization;

using NLog.Web;

namespace NalamVazhaWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // =========================================================
            // .NET 10 HOST
            // =========================================================

            var builder = WebApplication.CreateBuilder(args);

            // Keep local development self-contained and avoid Windows Event Log
            // and user-profile key-ring access in sandboxed developer sessions.
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();

                builder.Services
                    .AddDataProtection()
                    .PersistKeysToFileSystem(
                        new DirectoryInfo(
                            Path.Combine(
                                builder.Environment.ContentRootPath,
                                ".dataprotection-keys")))
                    .SetApplicationName("NalamVazha.Api.Local");
            }


            // Preserve existing WebAPI port
            builder.WebHost.UseUrls("http://*:50012");


            // Preserve existing NLog integration
            // NLog.config is NOT changed
            builder.Host.UseNLog();


            // Preserve original Program.cs Kestrel setting
            builder.WebHost.UseKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 30 * 1024;
            });


            // =========================================================
            // STARTUP.CONFIGURESERVICES -> PROGRAM.CS
            // =========================================================


            // =========================================================
            // HTTP CONTEXT ACCESSOR
            // =========================================================

            // Required by controllers such as tenantController
            builder.Services.AddHttpContextAccessor();


            // =========================================================
            // JWT AUTHENTICATION
            // =========================================================

            const string JwtScheme = "JwtBearer";

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtScheme;

                    options.DefaultAuthenticateScheme =
                        JwtScheme;

                    options.DefaultChallengeScheme =
                        JwtScheme;
                })
                .AddJwtBearer(
                    JwtScheme,
                    options =>
                    {
                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ValidateIssuer = true,

                                ValidateAudience = true,

                                ValidateLifetime = true,

                                ValidateIssuerSigningKey = true,

                                ValidIssuer =
                                    builder.Configuration["ValidIssuer"],

                                ValidAudience =
                                    "yourdomain.com",

                                IssuerSigningKey =
                                    new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(
                                            builder.Configuration[
                                                "SecurityKey"]!))
                            };
                    });


            // =========================================================
            // CONNECTION SETTINGS
            // =========================================================

            builder.Services.Configure<ConnectionSettings>(
                builder.Configuration.GetSection(
                    "ConnectionSettings"));


            // =========================================================
            // EXISTING SERVICES
            // =========================================================

            builder.Services.AddSingleton<
                NalamVazha.DAL.IEmailService,
                NalamVazha.DAL.EmailService>();


            builder.Services.AddSingleton<
                PreAdmissionNotificationService>();


            // =========================================================
            // HOSTED SERVICES
            // =========================================================

            // These workers can update the database or send notifications.
            // Keep them off during local development unless explicitly enabled.
            if (!builder.Environment.IsDevelopment())
            {
                builder.Services.AddHostedService<
                    PreAdmissionNotificationJob>();

                builder.Services.AddHostedService<
                    IPDSLANotificationService>();

                builder.Services.AddHostedService<
                    AgeUpdateService>();

                builder.Services.AddHostedService<
                    OverdueIpdReviewTaskService>();
            }


            // =========================================================
            // FORM / UPLOAD LIMITS
            // =========================================================

            builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit =
                    int.MaxValue;

                options.MultipartBodyLengthLimit =
                    int.MaxValue;
            });


            // =========================================================
            // CORS + MVC + NEWTONSOFT
            // =========================================================

            builder.Services
                .AddCors()
                .AddMvc()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver();
                });


            // =========================================================
            // SWAGGER
            // =========================================================

            builder.Services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "NalamVazha",
                        Description = "NalamVazha Apis"
                    });


                s.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme " +
                            "(Example: 'Bearer 12345abcdef')",

                        Name =
                            "Authorization",

                        In =
                            ParameterLocation.Header,

                        Type =
                            SecuritySchemeType.ApiKey,

                        Scheme =
                            "Bearer"
                    });


                s.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference =
                                    new OpenApiReference
                                    {
                                        Type =
                                            ReferenceType.SecurityScheme,

                                        Id =
                                            "Bearer"
                                    }
                            },

                            Array.Empty<string>()
                        }
                    });
            });


            // =========================================================
            // EXISTING KESTREL CONFIGURATION FROM STARTUP
            // =========================================================

            // Existing .NET 9 code contained this in Startup.cs.
            // It is intentionally retained during migration.
            builder.Services.Configure<KestrelServerOptions>(
                options =>
                {
                    options.Limits.MaxRequestBodySize =
                        int.MaxValue;
                });


            // =========================================================
            // BUILD
            // =========================================================

            var app = builder.Build();


            // =========================================================
            // STARTUP.CONFIGURE -> PROGRAM.CS
            // =========================================================


            // Development exception page
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            // =========================================================
            // AUTHENTICATION
            // =========================================================

            app.UseAuthentication();


            // =========================================================
            // CUSTOM ERROR LOGGING
            // =========================================================

            app.UseMiddleware<ErrorLoggingMiddleware>();


            // =========================================================
            // CORS
            // =========================================================

            app.UseCors(options =>
                options
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());


            // =========================================================
            // STATIC FILES
            // =========================================================

            app.UseStaticFiles();


            // =========================================================
            // ROUTING
            // =========================================================

            app.UseRouting();


            // =========================================================
            // AUTHORIZATION
            // =========================================================

            app.UseAuthorization();


            // =========================================================
            // CONTROLLERS
            // =========================================================

            app.MapControllers();


            // =========================================================
            // SWAGGER
            // =========================================================

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        "/swagger/v1/swagger.json",
                        "NalamVazha");

                    // Preserve existing Swagger-at-root behavior
                    c.RoutePrefix = "";
                });
            }
            else
            {
                var basePath =
                    "/NalamVazhaDev/NalamVazhaWebApi";


                app.UseSwagger(c =>
                {
                    c.RouteTemplate =
                        "swagger/{documentName}/swagger.json";

                    c.PreSerializeFilters.Add(
                        (swaggerDoc, httpReq) =>
                        {
                            swaggerDoc.Servers =
                                new List<OpenApiServer>
                                {
                                    new OpenApiServer
                                    {
                                        Url =
                                            $"https://{httpReq.Host.Value}{basePath}"
                                    }
                                };
                        });
                });


                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "";

                    c.SwaggerEndpoint(
                        "/NalamVazhaDev/NalamVazhaWebApi/swagger/v1/swagger.json",
                        "NalamVazha");
                });
            }


            // =========================================================
            // CONSOLE STARTUP DISPLAY
            // =========================================================

            // Keeps the same simple startup display as your
            // previous .NET application.
            // NLog.config remains unchanged.
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine(
                    $"Hosting environment: {app.Environment.EnvironmentName}");

                Console.WriteLine(
                    $"Content root path: {app.Environment.ContentRootPath}");

                foreach (var address in app.Urls)
                {
                    Console.WriteLine(
                        $"Now listening on: {address}");
                }

                Console.WriteLine(
                    "Application started. Press Ctrl+C to shut down.");
            });


            // =========================================================
            // RUN WEBAPI
            // =========================================================

            app.Run();
        }
    }
}
