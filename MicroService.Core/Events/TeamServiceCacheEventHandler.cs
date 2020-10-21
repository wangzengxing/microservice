using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Events
{
    /// <summary>
    /// 团队服务事件处理(将成员信息添加到缓存)
    /// </summary>
    public class TeamServiceCacheEventHandler : IEventHandler<TeamEvent>
    {
        public void Handle(TeamEvent evt)
        {
            Console.WriteLine($"成员信息条件到缓存成功：{evt.Id}");
        }
    }
}
