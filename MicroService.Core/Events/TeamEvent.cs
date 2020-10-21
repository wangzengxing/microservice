using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Events
{
    /// <summary>
    /// 成员事件
    /// </summary>
   public class TeamEvent : IEvent
    {
        public int Id { set; get; }
    }
}
