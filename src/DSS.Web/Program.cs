using System;
using System.Threading.Tasks;
using DSS.Utilities;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using Serilog;
using Serilog.Events;

namespace DSS.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File($"Logs/DSS-Logs-{DateTime.Now.ToString("ddMMyyyy")}.txt"))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Or LicenseContext.Commercial
        DataManager.Log = LogManager.GetLogger("DSS");

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);

            // Add session configuration
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".DSS.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(20); 
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; 
            });


            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<DSSWebModule>();
            var app = builder.Build();

            app.UseSession();


            await app.InitializeApplicationAsync();
            await app.RunAsync();
            
            return 0;
        }
        catch (Exception ex)
        {
            if (ex is HostAbortedException)
            {
                throw;
            }

            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
