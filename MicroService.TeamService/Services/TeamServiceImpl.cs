using MicroService.TeamService.Models;
using MicroService.TeamService.Repositories;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.TeamService.Services
{
    /// <summary>
    /// 团队服务实现
    /// </summary>
    public class TeamServiceImpl : ITeamService
    {
        public readonly ITeamRepository teamRepository;

        public TeamServiceImpl(ITeamRepository teamRepository)
        {
            this.teamRepository = teamRepository;
        }

        
        /*public void Create(Team team)
        {
            teamRepository.Create(team);
        }*/

        /// <summary>
        /// saga事务参与者 Compensable撤销业务 逻辑
        /// </summary>
        /// <param name="team"></param>
        [Compensable(nameof(DeleteTeam))]
        public void Create(Team team)
        {
            teamRepository.Create(team);
            // 异常
        }

        /// <summary>
        /// 撤销方法
        /// </summary>
        /// <param name="team"></param>
        void DeleteTeam(Team team)
        {
            Console.WriteLine($"删除数据:{team.ToString()}");
            teamRepository.Delete(team);
        }

        public void Delete(Team team)
        {
            teamRepository.Delete(team);
        }

        public Team GetTeamById(int id)
        {
            return teamRepository.GetTeamById(id);
        }

        public IEnumerable<Team> GetTeams()
        {
            return teamRepository.GetTeams();
        }
        [Compensable(nameof(UpdateTeam))]
        public void Update(Team team)
        {
            // 1、创建一个新字段
            
            // 2、创建一个临时表
            // 添加原有数据

            // 3、创建本地缓存
            teamRepository.Update(team);

        }

        public void UpdateTeam(Team team)
        {
            // 1、取出原有数据
            // 进行update
            // 2、取出原有缓存，进行更新
            teamRepository.Update(team);
        }

        public bool TeamExists(int id)
        {
            return teamRepository.TeamExists(id);
        }
    }
}
