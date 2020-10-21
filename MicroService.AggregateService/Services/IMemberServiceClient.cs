using MicroService.TeamService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregateService.Services
{
    /// <summary>
    /// 成员服务调用
    /// </summary>
    public interface IMemberServiceClient
    {
        /// <summary>
        /// 根据团队id查询团队成员
        /// </summary>
        /// <returns></returns>
        Task<IList<Member>> GetMembers(int teamId);

        /// <summary>
        /// 添加团队成员信息
        /// </summary>
        /// <param name="member"></param>
        void InsertMembers(Member member);
        
    }
}
