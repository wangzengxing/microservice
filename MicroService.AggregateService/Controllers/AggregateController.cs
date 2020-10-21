using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using MicroService.AggregateService.Services;
using MicroService.TeamService.Models;
using MicroService.VideoService.Models;
using Servicecomb.Saga.Omega.Abstractions.Transaction;

namespace MicroService.AggregateService.Controllers
{
    [Route("api/Teams")]
    [ApiController]
    public class AggregateController : ControllerBase
    {
        private readonly ITeamServiceClient teamServiceClient;
        private readonly IMemberServiceClient memberServiceClient;
        private readonly ICapPublisher capPublisher;
        // NLog日志打印类
        private readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        public AggregateController(ITeamServiceClient teamServiceClient,
                                    IMemberServiceClient memberServiceClient,
                                    ICapPublisher capPublisher)
        {
            this.teamServiceClient = teamServiceClient;
            this.memberServiceClient = memberServiceClient;
            this.capPublisher = capPublisher;
        }

        // GET: api/Aggregate
        [HttpGet]
        public async Task<ActionResult<List<Team>>> Get()
        {
            Console.WriteLine($"查询团队成员消息");
            // 1、查询团队
            IList<Team> teams = await teamServiceClient.GetTeams();

            // 2、查询团队成员
            foreach (var team in teams)
            {
                IList<Member> members = await memberServiceClient.GetMembers(team.Id);
                team.Members = members;
            }
            _Logger.Info("团队信息添加成功");
            return Ok(teams);
        }

        // GET: api/Aggregate/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 添加团队和成员信息
        /// </summary>
        /// <param name="value"></param>
        [HttpPost,SagaStart]
        public ActionResult Post(string value)
        {
            Console.WriteLine($"添加团队信息和成员信息");
            // 1、添加团队
            /*var team = new Team() { Name = "研发"};
            teamServiceClient.InsertTeams(team);

            // 2、添加团队成员
            var member = new Member() { FirstName ="tony",NickName="tony-1", TeamId = team.Id};
            memberServiceClient.InsertMembers(member);*/
            

            // 出现了异常
           // Thread.Sleep(10000);
            // 3、异步添加视频信息
            Video video = new Video()
            {
                VideoUrl = "http://localhost:8888/1232133321",
                MemberId =1
            };
            capPublisher.PublishAsync<Video>("video.event.1", video);

            return Ok("添加成功");
        }

        // PUT: api/Aggregate/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }
}
