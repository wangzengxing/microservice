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
    public class HttpTeamServiceClient : ITeamServiceClient
    {
        private readonly string ServiceSchme = "https";
        private readonly string ServiceName = "TeamService"; //服务名称
        private readonly string ServiceLink = "/Teams"; //服务名称
        // httpclient consul请求
        private readonly ConsulHttpClient consulHttpClient;
        public HttpTeamServiceClient(ConsulHttpClient consulHttpClient)
        {
            this.consulHttpClient = consulHttpClient;
        }

        public async Task<IList<Team>> GetTeams()
        {
            // 1、查询团队
            List<Team> teams = await consulHttpClient.GetAsync<List<Team>>(ServiceSchme, ServiceName, ServiceLink);
            return teams;
        }

        /// <summary>
        /// 添加团队信息
        /// </summary>
        /// <param name="team"></param>
        public void InsertTeams(Team team)
        {
            // 1、添加团队
            consulHttpClient.Post<Team>(ServiceSchme, ServiceName, ServiceLink, team);
        }
    }
}
