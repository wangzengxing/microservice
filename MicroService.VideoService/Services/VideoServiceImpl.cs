using MicroService.VideoService.Models;
using MicroService.VideoService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Services
{
    /// <summary>
    /// 团队服务实现
    /// </summary>
    public class VideoServiceImpl : IVideoService
    {
        public readonly IVideoRepository VideoRepository;

        public VideoServiceImpl(IVideoRepository VideoRepository)
        {
            this.VideoRepository = VideoRepository;
        }

        public void Create(Video Video)
        {
            VideoRepository.Create(Video);
        }

        public void Delete(Video Video)
        {
            VideoRepository.Delete(Video);
        }

        public Video GetVideoById(int id)
        {
            return VideoRepository.GetVideoById(id);
        }

        public IEnumerable<Video> GetVideos()
        {
            return VideoRepository.GetVideos();
        }

        public void Update(Video Video)
        {
            VideoRepository.Update(Video);
        }

        public bool VideoExists(int id)
        {
            return VideoRepository.VideoExists(id);
        }
    }
}
