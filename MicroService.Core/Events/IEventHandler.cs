using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Events
{

    /// <summary>
    /// 事件处理接口
    /// <typeparam name="TEvent">继承IEvent对象的事件源对象</typeparam>
    /// </summary>
   public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        /// <summary>
        /// 处理程序
        /// </summary>
        /// <param name="evt"></param>
        void Handle(TEvent evt);
    }
}
