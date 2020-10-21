using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace MicroService.AggregateService
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
                    // 1、添加NLog日志并加载nlog.config文件
                    webBuilder.ConfigureLogging(logbuilder => {
                        logbuilder.AddNLog("nlog.config");
                    });
                    webBuilder.UseStartup<Startup>().UseNLog();
                });
    }
}
