using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MicroService.Core.Registry;
using MicroService.Core.Registry.Extentions;
using MicroService.TeamService.Context;
using MicroService.TeamService.Repositories;
using MicroService.TeamService.Services;
using Servicecomb.Saga.Omega.AspNetCore.Extensions;

namespace MicroService.TeamService
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
            services.AddDbContext<TeamContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            // 2��ע���Ŷ�service
            services.AddScoped<ITeamService, TeamServiceImpl>();

            // 3��ע���ŶӲִ�
            services.AddScoped<ITeamRepository, TeamRepository>();

            // 4�����ӳ��
            //services.AddAutoMapper();

            // 5����ӷ���ע������
            services.AddConsulRegistry(Configuration);

            // 6��У��AccessToken,�����У�����Ľ���У��
            /*services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = "http://localhost:5005"; // 1����Ȩ���ĵ�ַ
                        options.ApiName = "TeamService"; // 2��api����(��Ŀ��������)
                        options.RequireHttpsMetadata = false; // 3��httpsԪ���ݣ�����Ҫ
                    });*/

            // 7��ע��saga�ֲ�ʽ����
            services.AddOmegaCore(option =>
            {
                option.GrpcServerAddress = "localhost:8080"; // 1��Э�����ĵ�ַ
                option.InstanceId = "TeamService-1";// 2������ʵ��Id
                option.ServiceName = "TeamService";// 3����������
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

          //  app.UseAuthentication();// 1�����������֤
            app.UseAuthorization();// 2��ʹ����Ȩ

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
