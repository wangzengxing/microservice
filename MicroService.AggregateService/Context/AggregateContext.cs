using Microsoft.EntityFrameworkCore;
using MicroService.TeamService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.TeamService.Context
{
    /// <summary>
    /// Aggregate服务上下文
    /// </summary>
    public class AggregateContext : DbContext
    {
        public AggregateContext(DbContextOptions<AggregateContext> options) : base(options)
        {
        }

    }
}
