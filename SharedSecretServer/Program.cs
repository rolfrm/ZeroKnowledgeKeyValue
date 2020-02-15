using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedSecret.Common;

namespace SharedSecret
{
    class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            var server = new Server();
            server.Start(args);
        }

        static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Debug.WriteLine("{1} {0}", args.Name, System.IO.Directory.GetCurrentDirectory());
            return null;
        }
    }

    public class Server
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            
            Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
                {
                    services.AddMvc();
                    services.Configure<KestrelServerOptions>(
                        context.Configuration.GetSection("Kestrel"));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                        {
                            // Set properties and call methods on options
                        })
                        .UseStartup<Startup>();
                });

        public void Start(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(
                Configuration.GetSection("Kestrel"));
            

        }
        
        

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Run(async (context) =>
            {
                var req = context.Request;
                var path = req.Path;
                if (path == "/files")
                {
                    if (req.Method != "POST")
                    {
                        await context.Response.WriteAsync("Error");
                    }
                    else
                    {
                        var msg = await System.Text.Json.JsonSerializer.DeserializeAsync<Message>(req.Body);
                        Debug.WriteLine("Writing to {0}", msg);
                    }
                }
                else
                {
                    await context.Response.WriteAsync("Welcome to the internet!");
                }
            });
        }
    }

}