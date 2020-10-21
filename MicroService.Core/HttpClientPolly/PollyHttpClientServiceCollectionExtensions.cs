using Microsoft.Extensions.DependencyInjection;
using Polly;
using MicroService.Core.HttpClientPolic;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core.HttpClientPolly
{
   /// <summary>
   /// 微服务中HttpClient熔断，降级策略扩展
   /// </summary>
   public static class PollyHttpClientServiceCollectionExtensions
   {
        /// <summary>
        /// Httpclient扩展方法
        /// </summary>
        /// <param name="services">ioc容器</param>
        /// <param name="name">HttpClient 名称(针对不同的服务进行熔断，降级)</param>
        /// <param name="action">熔断降级配置</param>
        /// <param name="TResult">降级处理错误的结果</param>
        /// <returns></returns>
        public static IServiceCollection AddPollyHttpClient(this IServiceCollection services, string name,Action<PollyHttpClientOptions> action)
        {
            // 1、创建选项配置类
            PollyHttpClientOptions options = new PollyHttpClientOptions();
            action(options);

            // 2、配置httpClient,熔断降级策略
            services.AddHttpClient(name,client => {
                client.Timeout = TimeSpan.FromSeconds(60);
            })
           //1.1 降级策略
           .AddPolicyHandler(Policy<HttpResponseMessage>.HandleInner<Exception>().FallbackAsync(options.httpResponseMessage, async b =>
           {
               // 1、降级打印异常
               Console.WriteLine($"服务{name}开始降级,异常消息：{b.Exception.Message}");
               // 2、降级后的数据
               Console.WriteLine($"服务{name}降级内容响应：{options.httpResponseMessage.Content.ToString()}");
               await Task.CompletedTask;
           }))
            // 1.2 断路器策略
            .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<Exception>().CircuitBreakerAsync(options.CircuitBreakerOpenFallCount, TimeSpan.FromSeconds(options.CircuitBreakerDownTime), (ex, ts) => {
                Console.WriteLine($"服务{name}断路器开启，异常消息：{ex.Exception.Message}");
                Console.WriteLine($"服务{name}断路器开启时间：{ts.TotalSeconds}s");
            }, () => {
                Console.WriteLine($"服务{name}断路器关闭");
            }, () => {
                Console.WriteLine($"服务{name}断路器半开启(时间控制，自动开关)");
            }))
            // 1.3 重试策略
            .AddPolicyHandler(Policy<HttpResponseMessage>
              .Handle<Exception>()
              .RetryAsync(options.RetryCount)
            )
            // 1.4 超时策略
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(options.TimeoutTime)));
            
            return services;
        }


    }
}
