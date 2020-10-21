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
using MicroService.MemberService.Services;
using MicroService.TeamService.Context;
using MicroService.TeamService.Repositories;
using Servicecomb.Saga.Omega.AspNetCore.Extensions;

namespace MicroService.MemberService
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
            services.AddDbContext<MemberContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            // 2、注册成员service
            services.AddScoped<IMemberService, MemberServiceImpl>();

            // 3、注册成员仓储
            services.AddScoped<IMemberRepository, MemberRepository>();

            // 4、添加服务注册条件
            services.AddConsulRegistry(Configuration);

            // 7、注册saga分布式事务
            services.AddOmegaCore(option =>
            {
                option.GrpcServerAddress = "localhost:8080"; // 1、协调中心地址
                option.InstanceId = "MemberService-1";// 2、服务实例Id
                option.ServiceName = "MemberService";// 3、服务名称
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
