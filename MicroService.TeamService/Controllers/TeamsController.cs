using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MicroService.TeamService.Context;
using MicroService.TeamService.Models;
using MicroService.TeamService.Services;

namespace MicroService.TeamService.Controllers
{

    /// <summary>
    /// 团队微服务api
    /// </summary>
    [Route("Teams")]
    //  [Authorize] // 1、保护起来
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService teamService;

        // 1、配置信息依赖
        public readonly IConfiguration configuration;

        // 2、Ef core DbContext
        public readonly TeamContext teamContext;

        public TeamsController(ITeamService teamService, IConfiguration configuration, TeamContext teamContext)
        {
            this.teamService = teamService;
            this.configuration = configuration;
            this.teamContext = teamContext;
        }

        // 
        // GET: api/Teams
        [HttpGet]
        public ActionResult<IEnumerable<Team>> GetTeams()
        {
            // Thread.Sleep(10000000);
            // 1、演示宕机
            Console.WriteLine($"查询团队信息:{configuration["common"]}");

            // 2、动态数据连接
            teamContext.Database.GetDbConnection().ConnectionString = configuration.GetConnectionString("DefaultConnection");

            // 3、降级 从缓存获取 --- 开关配置 isAbleCache
            string isAbleCache = configuration["IsAllowCache"];
            if (isAbleCache == "true")
            {
                // 3.1 加载缓存
                return new List<Team>();
            }
            else
            {
                // 3.2 加载数据库
                return teamService.GetTeams().ToList();
            }
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public ActionResult<Team> GetTeam(int id)
        {
            Team team = teamService.GetTeamById(id);

            if (team == null)
            {
                return NotFound();
            }
            return team;
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public IActionResult PutTeam(int id, Team team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            try
            {
                teamService.Update(team);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!teamService.TeamExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Teams
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public ActionResult<Team> PostTeam(Team team)
        {
            Console.WriteLine("添加团队信息");
            teamService.Create(team);

            return CreatedAtAction("GetTeam", new { id = team.Id }, team);
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public ActionResult<Team> DeleteTeam(int id)
        {
            var team = teamService.GetTeamById(id);
            if (team == null)
            {
                return NotFound();
            }

            teamService.Delete(team);
            return team;
        }
    }
}
