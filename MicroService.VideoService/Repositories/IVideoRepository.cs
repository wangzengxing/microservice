using MicroService.VideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.VideoService.Repositories
{
    /// <summary>
    /// 视频仓储接口
    /// </summary>
    public interface IVideoRepository
    {
        IEnumerable<Video> GetVideos();
        Video GetVideoById(int id);
        void Create(Video video);
        void Update(Video video);
        void Delete(Video video);
        bool VideoExists(int id);
    }
}
