using System;
using System.IO;

using Microsoft.AspNetCore.DataProtection;
using DinkToPdf;
using DinkToPdf.Contracts;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.Kestrel.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Serialization;

using NLog.Web;

namespace Admin
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
                    .SetApplicationName("NalamVazha.Admin.Local");
            }

            // Preserve existing Admin port
            builder.WebHost.UseUrls("http://*:50011");

            // Preserve existing NLog integration
            // NLog.config is NOT changed
            builder.Host.UseNLog();


            // =========================================================
            // CONFIGURATION
            // =========================================================

            // Preserve original .NET 9 configuration loading behavior
            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile(
                    "appsettings.json",
                    optional: true,
                    reloadOnChange: false)
                .AddJsonFile(
                    $"appsettings.{builder.Environment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: false);


            // =========================================================
            // STARTUP.CONFIGURESERVICES -> PROGRAM.CS
            // =========================================================

            // PDF
            builder.Services.AddSingleton<IConverter>(
                new SynchronizedConverter(new PdfTools()));


            // Precompile Razor views. Runtime compilation fails when optional
            // transitive assemblies (for example Pkcs from the PDF stack) don't
            // expose a compilation-library path in the local runtime.
            builder.Services.AddRazorPages();


            // Application settings
            builder.Services.Configure<ApiSettings>(
                builder.Configuration.GetSection("ApiSettings"));

            builder.Services.Configure<MailSettings>(
                builder.Configuration.GetSection("MailSettings"));

            builder.Services.Configure<PublicFormSettings>(
                builder.Configuration.GetSection("PublicFormSettings"));


            // Form / Upload limits
            builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
            });


            // Session
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });


            // IIS
            builder.Services.Configure<IISOptions>(options =>
            {
            });


            // MVC + FluentValidation + Newtonsoft
            builder.Services
                .AddMvc()
                .AddFluentValidation()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver();
                });


            // Kestrel upload/request limit
            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize =
                    Convert.ToInt64(
                        builder.Configuration[
                            "FileUploadSettings:MultipartBodyTotalLimit"]);
            });


            // Localization
            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resource";
            });


            // Controllers + Views
            builder.Services
                .AddControllersWithViews(options =>
                {
                    options.Filters.Add(typeof(LanguageActionFilter));
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();


            // Razor View Location
            builder.Services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(
                    new LanguageViewLocationExpander());
            });


            // =========================================================
            // BUILD
            // =========================================================

            var app = builder.Build();


            // =========================================================
            // STARTUP.CONFIGURE -> PROGRAM.CS
            // =========================================================

            app.UseStaticFiles();


            // Environment exception handling
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (app.Environment.EnvironmentName == "CodeServer")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // Preserve original behavior
                app.UseForwardedHeaders();
            }


            // Session
            app.UseSession();


            // Existing custom middleware
            // app.UseMiddleware<TenantResolutionMiddleware>();

            app.UseMiddleware<ErrorLoggingMiddleware>();


            // =========================================================
            // PATH BASE
            // =========================================================

            if (app.Environment.IsProduction())
            {
                app.Use((context, next) =>
                {
                    context.Request.PathBase =
                        builder.Configuration.GetValue<string>(
                            "envSettings:basePath");

                    return next();
                });
            }


            if (app.Environment.EnvironmentName == "CodeServer")
            {
                app.Use((context, next) =>
                {
                    context.Request.PathBase =
                        builder.Configuration.GetValue<string>(
                            "envSettings:basePath");

                    return next();
                });
            }


            // =========================================================
            // SECURITY HEADERS
            // =========================================================

            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Frame-Options"] =
                    "SAMEORIGIN";

                context.Response.Headers["X-XSS-Protection"] =
                    "1";

                context.Response.Headers["X-Content-Type-Options"] =
                    "nosniff";

                await next();
            });


            // =========================================================
            // FORWARDED HEADERS
            // =========================================================

            app.UseForwardedHeaders(
                new ForwardedHeadersOptions
                {
                    ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor |
                        ForwardedHeaders.XForwardedProto
                });


            // =========================================================
            // ERROR HANDLING
            // =========================================================

            app.UseExceptionHandler("/System/Error");

            app.UseStatusCodePagesWithReExecute(
                "/System/StatusCode",
                "?code={0}");


            // =========================================================
            // ROUTING
            // =========================================================

            app.UseRouting();


            // Authentication before Authorization
            app.UseAuthentication();
            app.UseAuthorization();


            // =========================================================
            // EXISTING ROUTES
            // =========================================================

            app.MapControllerRoute(
                name: "area",
                pattern:
                    "{area:exists}/{controller=users}/{action=Home}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern:
                    "{controller=users}/{action=Home}/{id?}");


            // =========================================================
            // CONSOLE STARTUP DISPLAY
            // =========================================================

            // This keeps the same simple console display you had before.
            // It does NOT modify NLog.config.
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
            // RUN
            // =========================================================

            app.Run();
        }
    }
}
