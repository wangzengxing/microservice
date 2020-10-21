using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using MicroService.Gateway.IdentityServer;
using MicroService.Gateway.OcelotExtension;

namespace MicroService.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // 1������ids4
            var identityServerOptions = new IdentityServerOptions();
            Configuration.Bind("IdentityServerOptions", identityServerOptions);
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(identityServerOptions.IdentityScheme, options =>
                    {
                        options.Authority = identityServerOptions.AuthorityAddress; // 1����Ȩ���ĵ�ַ
                        options.ApiName = identityServerOptions.ResourceName; // 2��api����(��Ŀ��������)
                        options.RequireHttpsMetadata = false; // 3��httpsԪ���ݣ�����Ҫ
                    });

            // 1���������Ocelot��ioc����
            services.AddOcelot().AddConsul().AddPolly();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // 2��ʹ������
            app.UseOcelot((build, config) =>
            {
                build.BuildCustomeOcelotPipeline(config); // 2.1 �Զ���ocelot�м�����
            }).Wait();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
