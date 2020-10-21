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
            // 1���Զ����쳣����(�û��洦��)
            var fallbackResponse = new HttpResponseMessage
            {
                Content = new StringContent("ϵͳ����æ�����Ժ�����"),// ���ݣ��Զ�������
                StatusCode = HttpStatusCode.GatewayTimeout // 504
            };

            // 1.2 ��װ֮��ĵ���PollyHttpClient
            services.AddPollyHttpClient("mrico", options => {
                options.TimeoutTime = 60; // 1����ʱʱ��
                options.RetryCount = 3;// 2�����Դ���
                options.CircuitBreakerOpenFallCount = 2;// 3���۶�������(���ٴ�ʧ�ܿ���)
                options.CircuitBreakerDownTime = 100;// 4���۶�������ʱ��
                options.httpResponseMessage = fallbackResponse;// 5����������
            })
             .AddHttpClientConsul<ConsulHttpClient>(); // 1.3��HttpClient��consul��װ(ʵ�ָ��ؾ�������)

            /*// 2��ע��consul������
            services.AddConsulDiscovery();

            // 3��ע�Ḻ�ؾ���
            services.AddSingleton<ILoadBalance, RandomLoadBalance>();*/

            // 4��ע��team����
            services.AddSingleton<ITeamServiceClient, HttpTeamServiceClient>();
            // 5��ע���Ա����
            services.AddSingleton<IMemberServiceClient, HttpMemberServiceClient>();

            // 6����ӷ���ע��
            services.AddConsulRegistry(Configuration);

            // 7��ע��saga�ֲ�ʽ����
            services.AddOmegaCore(option =>
            {
                option.GrpcServerAddress = "localhost:8080"; // 1��Э�����ĵ�ַ
                option.InstanceId = "AggregateService-1";// 2������ʵ��Id
                option.ServiceName = "AggregateService";// 3����������
            });

            // 8������¼�����cap
            services.AddCap(x => {
                // 8.1 ʹ���ڴ�洢��Ϣ(��Ϣ����ʧ�ܴ���)
                // x.UseInMemoryStorage();
                // 8.1 ʹ��EntityFramework���д洢����
                x.UseEntityFramework<AggregateContext>();
                // 8.2 ʹ��sqlserver����������
                x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                // 8.3 ʹ��RabbitMQ�����¼����Ĵ���
                x.UseRabbitMQ(rb => {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });

                // 8.4���cap��̨���ҳ��(�˹�����)
                x.UseDashboard();
            });

            // 9��ע�������ĵ�IOC����
            services.AddDbContext<AggregateContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            // 10������ʹ��Nlog


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // 1��consul����ע��
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
