using MicroService.VideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Services
{
    /// <summary>
    /// 团队服务接口
    /// </summary>
    public interface IVideoService
    {
        IEnumerable<Video> GetVideos();
        Video GetVideoById(int id);
        void Create(Video video);
        void Update(Video video);
        void Delete(Video video);
        bool VideoExists(int id);
    }
}
