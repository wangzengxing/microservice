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
            // 1��ע�������ĵ�IOC����
            services.AddDbContext<VideoContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            // 2��ע���Ŷ�service
            services.AddScoped<IVideoService, VideoServiceImpl>();

            // 3��ע���ŶӲִ�
            services.AddScoped<IVideoRepository, VideoRepository>();

            // 4�����ӳ��
            //services.AddAutoMapper();

            // 5����ӷ���ע������
            services.AddConsulRegistry(Configuration);

            // 8������¼�����cap
            services.AddCap(x => {
                // 8.1 ʹ���ڴ�洢��Ϣ(��Ϣ����ʧ�ܴ���)
                //  x.UseInMemoryStorage();
                // 8.1 ʹ��EntityFramework���д洢����
                x.UseEntityFramework<VideoContext>();
                // 8.2 ʹ��sqlserver����������
                x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                // 8.1 ʹ��RabbitMQ�����¼����Ĵ���
                x.UseRabbitMQ(rb => {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });

                // 8.4 ���ö�ʱ����������
               // x.FailedRetryInterval = 2;
                x.FailedRetryCount = 5; // 3 ��ʧ�� 3����

                // 8.5 �˹���Ԥ���޸ı��������ҳ��
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

            // 1��consul����ע��
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
