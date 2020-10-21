using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
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
using NLog.Extensions.Logging;
using Polly;
using Polly.Timeout;
using MicroService.AggregateService.Services;
using MicroService.Core.Cluster;
using MicroService.Core.HttpClientConsul;
using MicroService.Core.HttpClientPolic;
using MicroService.Core.HttpClientPolly;
using MicroService.Core.Registry.Extentions;
using MicroService.TeamService.Context;
using Servicecomb.Saga.Omega.AspNetCore.Extensions;

namespace MicroService.AggregateService
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
            // 1、自定义异常处理(用缓存处理)
            var fallbackResponse = new HttpResponseMessage
            {
                Content = new StringContent("系统正繁忙，请稍后重试"),// 内容，自定义内容
                StatusCode = HttpStatusCode.GatewayTimeout // 504
            };

            // 1.2 封装之后的调用PollyHttpClient
            services.AddPollyHttpClient("mrico", options => {
                options.TimeoutTime = 60; // 1、超时时间
                options.RetryCount = 3;// 2、重试次数
                options.CircuitBreakerOpenFallCount = 2;// 3、熔断器开启(多少次失败开启)
                options.CircuitBreakerDownTime = 100;// 4、熔断器开启时间
                options.httpResponseMessage = fallbackResponse;// 5、降级处理
            })
             .AddHttpClientConsul<ConsulHttpClient>(); // 1.3、HttpClient下consul封装(实现负载均衡请求)

            /*// 2、注册consul服务发现
            services.AddConsulDiscovery();

            // 3、注册负载均衡
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();*/

            // 4、注册team服务
            services.AddSingleton<ITeamServiceClient, HttpTeamServiceClient>();
            // 5、注册成员服务
            services.AddSingleton<IMemberServiceClient, HttpMemberServiceClient>();

            // 6、添加服务注册
            services.AddConsulRegistry(Configuration);

            // 7、注册saga分布式事务
            services.AddOmegaCore(option =>
            {
                option.GrpcServerAddress = "localhost:8080"; // 1、协调中心地址
                option.InstanceId = "AggregateService-1";// 2、服务实例Id
                option.ServiceName = "AggregateService";// 3、服务名称
            });

            // 8、添加事件总线cap
            services.AddCap(x => {
                // 8.1 使用内存存储消息(消息发送失败处理)
                // x.UseInMemoryStorage();
                // 8.1 使用EntityFramework进行存储操作
                x.UseEntityFramework<AggregateContext>();
                // 8.2 使用sqlserver进行事务处理
                x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                // 8.3 使用RabbitMQ进行事件中心处理
                x.UseRabbitMQ(rb => {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });

                // 8.4添加cap后台监控页面(人工处理)
                x.UseDashboard();
            });

            // 9、注册上下文到IOC容器
            services.AddDbContext<AggregateContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            // 10、配置使用Nlog


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // 1、consul服务注册
            app.UseConsulRegistry();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
