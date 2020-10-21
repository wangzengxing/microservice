using Microsoft.EntityFrameworkCore;
using MicroService.VideoService.Context;
using MicroService.VideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Repositories
{
    /// <summary>
    /// 团队仓储实现
    /// </summary>
    public class VideoRepository : IVideoRepository
    {
        public VideoContext videoContext;
        public VideoRepository(VideoContext videoContext)
        {
            this.videoContext = videoContext;
        }
        public void Create(Video video)
        {
            videoContext.Videos.Add(video);
            videoContext.SaveChanges();
        }

        public void Delete(Video video)
        {
            videoContext.Videos.Remove(video);
            videoContext.SaveChanges();
        }

        public Video GetVideoById(int id)
        {
            return videoContext.Videos.Find(id);
        }

        public IEnumerable<Video> GetVideos()
        {
            return videoContext.Videos.ToList();
        }

        public void Update(Video video)
        {
            videoContext.Videos.Update(video);
            videoContext.SaveChanges();
        }
        public bool VideoExists(int id)
        {
            return videoContext.Videos.Any(e => e.Id == id);
        }
    }
}
