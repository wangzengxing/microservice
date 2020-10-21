using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroService.VideoService.Context;
using MicroService.VideoService.Models;
using MicroService.VideoService.Services;

namespace MicroService.VideoService.Controllers
{
   
    /// <summary>
    /// 团队微服务api
    /// </summary>
    [Route("Video")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService videoService;

        public VideoController(IVideoService videoService)
        {
            this.videoService = videoService;
        }

        // GET: api/Videos
        [HttpGet]
        public ActionResult<IEnumerable<Video>> GetVideos()
        {
            return videoService.GetVideos().ToList();
        }

        // GET: api/Videos/5
        [HttpGet("{id}")]
        public ActionResult<Video> GetVideo(int id)
        {
            Video Video = videoService.GetVideoById(id);

            if (Video == null)
            {
                return NotFound();
            }
            return Video;
        }

        // PUT: api/Videos/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public IActionResult PutVideo(int id, Video Video)
        {
            if (id != Video.Id)
            {
                return BadRequest();
            }

            try
            {
                videoService.Update(Video);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!videoService.VideoExists(id))
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

        /// <summary>
        /// 视频添加
        /// </summary>
        /// <param name="Video"></param>
        /// <returns></returns>
        /// video.event video.event  video.event.1
        /// video.event video.1 video.1
        /// *  一对多匹配
        /// # 一对一匹配
        [NonAction]
        [CapSubscribe("video.*")]
        public ActionResult<Video> PostVideo(Video Video)
        {
            // 1、阻塞30
            // Thread.Sleep(30000);
           // throw new Exception("出现异常");
            Console.WriteLine($"接受到视频事件消息");
            videoService.Create(Video);
            return CreatedAtAction("GetVideo", new { id = Video.Id }, Video);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public ActionResult<Video> DeleteVideo(int id)
        {
            var Video = videoService.GetVideoById(id);
            if (Video == null)
            {
                return NotFound();
            }

            videoService.Delete(Video);
            return Video;
        }
    }
}
