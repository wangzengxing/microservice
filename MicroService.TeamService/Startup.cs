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
            // 1、注册上下文到IOC容器
            services.AddDbContext<TeamContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            // 2、注册团队service
            services.AddScoped<ITeamService, TeamServiceImpl>();

            // 3、注册团队仓储
            services.AddScoped<ITeamRepository, TeamRepository>();

            // 4、添加映射
            //services.AddAutoMapper();

            // 5、添加服务注册条件
            services.AddConsulRegistry(Configuration);

            // 6、校验AccessToken,从身份校验中心进行校验
            /*services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = "http://localhost:5005"; // 1、授权中心地址
                        options.ApiName = "TeamService"; // 2、api名称(项目具体名称)
                        options.RequireHttpsMetadata = false; // 3、https元数据，不需要
                    });*/

            // 7、注册saga分布式事务
            services.AddOmegaCore(option =>
            {
                option.GrpcServerAddress = "localhost:8080"; // 1、协调中心地址
                option.InstanceId = "TeamService-1";// 2、服务实例Id
                option.ServiceName = "TeamService";// 3、服务名称
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

          //  app.UseAuthentication();// 1、开启身份验证
            app.UseAuthorization();// 2、使用授权

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
