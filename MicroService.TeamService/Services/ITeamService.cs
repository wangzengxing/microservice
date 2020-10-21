using MicroService.TeamService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.TeamService.Services
{
    /// <summary>
    /// 团队服务接口
    /// </summary>
    public interface ITeamService
    {
        IEnumerable<Team> GetTeams();
        Team GetTeamById(int id);
        void Create(Team team);
        void Update(Team team);
        void Delete(Team team);
        bool TeamExists(int id);
    }
}
