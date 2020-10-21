using Microsoft.EntityFrameworkCore;
using MicroService.TeamService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.TeamService.Context
{
    /// <summary>
    /// 团队服务上下文
    /// </summary>
    public class TeamContext : DbContext
    {
        public TeamContext(DbContextOptions<TeamContext> options) : base(options)
        {
        }

        public DbSet<Team> Teams { get; set; }
    }
}
