using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MicroService.Core.Registry.Extentions;
using MicroService.VideoService.Context;
using MicroService.VideoService.Repositories;
using MicroService.VideoService.Services;

namespace MicroService.VideoService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 1、注册上下文到IOC容器
            services.AddDbContext<VideoContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            // 2、注册团队service
            services.AddScoped<IVideoService, VideoServiceImpl>();

            // 3、注册团队仓储
            services.AddScoped<IVideoRepository, VideoRepository>();

            // 4、添加映射
            //services.AddAutoMapper();

            // 5、添加服务注册条件
            services.AddConsulRegistry(Configuration);

            // 8、添加事件总线cap
            services.AddCap(x => {
                // 8.1 使用内存存储消息(消息发送失败处理)
                //  x.UseInMemoryStorage();
                // 8.1 使用EntityFramework进行存储操作
                x.UseEntityFramework<VideoContext>();
                // 8.2 使用sqlserver进行事务处理
                x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                // 8.1 使用RabbitMQ进行事件中心处理
                x.UseRabbitMQ(rb => {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });

                // 8.4 配置定时器尽早启动
               // x.FailedRetryInterval = 2;
                x.FailedRetryCount = 5; // 3 次失败 3分钟

                // 8.5 人工干预，修改表，后面管理页面
                x.UseDashboard();
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // 1、consul服务注册
            app.UseConsulRegistry();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
