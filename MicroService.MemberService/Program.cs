using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Winton.Extensions.Configuration.Consul;

namespace MicroService.MemberService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // 1、动态加载配置中心的配置文件
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {

                        // 加载默认配置信息到Configuration
                        hostingContext.Configuration = config.Build();
                        // 加载consul配置中心配置
                        string consul_url = hostingContext.Configuration["Consul_Url"];
                        Console.WriteLine($"consul_url:{consul_url}");
                        // 动态加载环境信息，主要在于动态获取服务名称和环境名称
                        var env = hostingContext.HostingEnvironment;
                        config.AddConsul(
                                    $"{env.ApplicationName}/appsettings.json",
                                    options =>
                                    {
                                        options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1、consul地址
                                        options.Optional = true; // 2、配置选项
                                        options.ReloadOnChange = true; // 3、配置文件更新后重新加载
                                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4、忽略异常
                                    }
                                    ).AddConsul(
                                   // 3、加载通用的配置
                                   "common.json",
                                   options =>
                                   {
                                       options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1、consul地址
                                       options.Optional = true; // 2、配置选项
                                       options.ReloadOnChange = true; // 3、配置文件更新后重新加载
                                       options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4、忽略异常
                                   }
                                   );
                        // 5、consul中加载的配置信息加载到Configuration对象，然后通过Configuration 对象加载项目中
                        hostingContext.Configuration = config.Build();
                    });
                    webBuilder.UseKestrel(option =>
                     {
                         option.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(60);
                         option.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(60);
                     });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
