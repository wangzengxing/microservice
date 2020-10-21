using MicroService.TeamService.Models;
using MicroService.TeamService.Repositories;
using Servicecomb.Saga.Omega.Abstractions.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.MemberService.Services
{
    /// <summary>
    /// 团队服务实现
    /// </summary>
    public class MemberServiceImpl : IMemberService
    {
        public readonly IMemberRepository memberRepository;

        public MemberServiceImpl(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        //
       /* public void Create(Member member)
        {
            // 1、未确认
            memberRepository.Create(member);
        }*/

        /// <summary>
        /// saga事务参与者 Compensable撤销业务 逻辑
        /// </summary>
        /// <param name="member"></param>
        [Compensable(nameof(DeleteMember))]
        public void Create(Member member)
        {
            memberRepository.Create(member);
        }

        /// <summary>
        /// 删除补偿方法
        /// </summary>
        /// <param name="member"></param>
        void DeleteMember(Member member)
        {
            memberRepository.Delete(member);
        }

        /*// commit 
        public void Create(Member member)
        {
            // 1确认
            memberRepository.Create(member);
        }


        // cancel
        public void DeleteC(Member member)
        {
            // 1、撤销或者删除
            memberRepository.Create(member);
        }
*/

        public void Delete(Member member)
        {
            memberRepository.Delete(member);
        }

        public Member GetMemberById(int id)
        {
            return memberRepository.GetMemberById(id);
        }

        public IEnumerable<Member> GetMembers()
        {
            return memberRepository.GetMembers();
        }
        
        public void Update(Member member)
        {
            memberRepository.Update(member);
        }

        public bool MemberExists(int id)
        {
            return memberRepository.MemberExists(id);
        }

        public IEnumerable<Member> GetMembers(int teamId)
        {
            return memberRepository.GetMembers(teamId);
        }
    }
}
