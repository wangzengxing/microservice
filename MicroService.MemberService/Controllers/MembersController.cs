using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MicroService.MemberService.Services;
using MicroService.TeamService.Context;
using MicroService.TeamService.Models;

namespace MicroService.TeamService.Controllers
{
    [Route("/Members")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService memberService;

        // 1、配置信息依赖
        public readonly IConfiguration configuration;

        public MembersController(IMemberService memberService, IConfiguration configuration)
        {
            this.memberService = memberService;
            this.configuration = configuration;
        }


        /// <summary>
        /// 查询所有成员信息
        /// </summary>
        /// <param name="teamId">?teamId参数结尾方式</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<Member>> GetMembers(int teamId)
        {
            Console.WriteLine($"查询团队成员信息：{configuration["common"]}");
            if (teamId == 0)
            {
                return memberService.GetMembers().ToList();
            }
            else
            {
                return memberService.GetMembers(teamId).ToList();
            }
        }


        // GET: api/Members/5
        [HttpGet("{id}")]
        public ActionResult<Member> GetMember(int id)
        {
            var member = memberService.GetMemberById(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public IActionResult PutMember(int id, Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

            try
            {
                memberService.Update(member);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!memberService.MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public ActionResult<Member> PostMember(int teamId, Member member)
        {
            memberService.Create(member);

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public ActionResult<Member> DeleteMember(int id)
        {
            var member = memberService.GetMemberById(id);
            if (member == null)
            {
                return NotFound();
            }
            memberService.Delete(member);

            return member;
        }
    }
}
