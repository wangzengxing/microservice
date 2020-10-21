using Newtonsoft.Json;
using MicroService.Core.Cluster;
using MicroService.Core.HttpClientConsul;
using MicroService.Core.Registry;
using MicroService.TeamService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Services
{
    /// <summary>
    /// 服务调用实现
    /// </summary>
    public class HttpMemberServiceClient : IMemberServiceClient
    {
        private readonly string ServiceSchme = "https";
        private readonly string ServiceName = "memberservice"; //服务名称
        private readonly string ServiceLink = "/Members"; //资源名称
        // httpclient consul请求
        private readonly ConsulHttpClient consulHttpClient;
        public HttpMemberServiceClient(ConsulHttpClient consulHttpClient)
        {
            this.consulHttpClient = consulHttpClient;
        }

        public async Task<IList<Member>> GetMembers(int teamId)
        {
            // 1、设置参数连接
            string urlLink = ServiceLink + "?teamId=" + teamId;
            // 2、查询团队成员
            List<Member> members = await consulHttpClient.GetAsync<List<Member>>(ServiceSchme, ServiceName, urlLink);
            return members;
        }

        public void InsertMembers(Member member)
        {
            // 2、添加成员
            consulHttpClient.Post<Member>(ServiceSchme, ServiceName, ServiceLink, member);
        }
    }
}
