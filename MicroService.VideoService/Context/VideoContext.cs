using Microsoft.EntityFrameworkCore;
using MicroService.VideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Context
{
    /// <summary>
    /// 视频服务上下文
    /// </summary>
    public class VideoContext : DbContext
    {
        public VideoContext(DbContextOptions<VideoContext> options) : base(options)
        {
        }

        public DbSet<Video> Videos { get; set; }
    }
}
