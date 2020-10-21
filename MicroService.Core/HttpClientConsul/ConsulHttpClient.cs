using Newtonsoft.Json;
using MicroService.Core.Cluster;
using MicroService.Core.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core.HttpClientConsul
{
    /// <summary>
    /// consul httpclient扩展
    /// </summary>
   public class ConsulHttpClient
   {
        private readonly IServiceDiscovery serviceDiscovery;
        private readonly ILoadBalance loadBalance;
        private readonly IHttpClientFactory httpClientFactory;
        public ConsulHttpClient(IServiceDiscovery serviceDiscovery,
                                    ILoadBalance loadBalance,
                                    IHttpClientFactory httpClientFactory)
        {
            this.serviceDiscovery = serviceDiscovery;
            this.loadBalance = loadBalance;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// param name="ServiceSchme">服务名称:(http/https)</param>
        /// <param name="ServiceName">服务名称</param>
        /// <param name="serviceLink">服务路径</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string Serviceshcme, string ServiceName,string serviceLink)
        {
            // 1、获取服务
            IList<ServiceUrl> serviceUrls = await serviceDiscovery.Discovery(ServiceName);

            // 2、负载均衡服务
            ServiceUrl serviceUrl = loadBalance.Select(serviceUrls);

            // 3、建立请求
            Console.WriteLine($"请求路径：{Serviceshcme} +'://'+{serviceUrl.Url} + {serviceLink}");
            HttpClient httpClient = httpClientFactory.CreateClient("mrico");
            // HttpResponseMessage response = await httpClient.GetAsync(serviceUrl.Url + serviceLink);
            HttpResponseMessage response = await httpClient.GetAsync(Serviceshcme +"://"+serviceUrl.Url + serviceLink);

            // 3.1json转换成对象
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(json);
            } else
            {
                // 3.2、进行自定义异常处理，这个地方进行了降级处理
                throw new Exception($"{ServiceName}服务调用错误:{response.Content.ReadAsStringAsync()}");
            }
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// param name="ServiceSchme">服务名称:(http/https)</param>
        /// <param name="ServiceName">服务名称</param>
        /// <param name="serviceLink">服务路径</param>
        /// <param name="paramData">服务参数</param>
        /// <returns></returns>
        public T Post<T>(string Serviceshcme, string ServiceName, string serviceLink, object paramData = null)
        {
            // 1、获取服务
            IList<ServiceUrl> serviceUrls = serviceDiscovery.Discovery(ServiceName).Result;

            // 2、负载均衡服务
            ServiceUrl serviceUrl = loadBalance.Select(serviceUrls);

            // 3、建立请求
            Console.WriteLine($"请求路径：{Serviceshcme} +'://'+{serviceUrl.Url} + {serviceLink}");
            HttpClient httpClient = httpClientFactory.CreateClient("mrico");

            // 3.1 转换成json内容
            HttpContent hc = new StringContent(JsonConvert.SerializeObject(paramData), Encoding.UTF8, "application/json");

            // HttpResponseMessage response = await httpClient.GetAsync(serviceUrl.Url + serviceLink);
            HttpResponseMessage response = httpClient.PostAsync(Serviceshcme + "://" + serviceUrl.Url + serviceLink, hc).Result;

            // 3.1json转换成对象
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                string json = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                // 3.2、进行自定义异常处理，这个地方进行了降级处理
                throw new Exception($"{ServiceName}服务调用错误:{response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
