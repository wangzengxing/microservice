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
                    // 1����̬�����������ĵ������ļ�
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {

                        // ����Ĭ��������Ϣ��Configuration
                        hostingContext.Configuration = config.Build();
                        // ����consul������������
                        string consul_url = hostingContext.Configuration["Consul_Url"];
                        Console.WriteLine($"consul_url:{consul_url}");
                        // ��̬���ػ�����Ϣ����Ҫ���ڶ�̬��ȡ�������ƺͻ�������
                        var env = hostingContext.HostingEnvironment;
                        config.AddConsul(
                                    $"{env.ApplicationName}/appsettings.json",
                                    options =>
                                    {
                                        options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1��consul��ַ
                                        options.Optional = true; // 2������ѡ��
                                        options.ReloadOnChange = true; // 3�������ļ����º����¼���
                                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4�������쳣
                                    }
                                    ).AddConsul(
                                   // 3������ͨ�õ�����
                                   "common.json",
                                   options =>
                                   {
                                       options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consul_url); }; // 1��consul��ַ
                                       options.Optional = true; // 2������ѡ��
                                       options.ReloadOnChange = true; // 3�������ļ����º����¼���
                                       options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; }; // 4�������쳣
                                   }
                                   );
                        // 5��consul�м��ص�������Ϣ���ص�Configuration����Ȼ��ͨ��Configuration ���������Ŀ��
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
