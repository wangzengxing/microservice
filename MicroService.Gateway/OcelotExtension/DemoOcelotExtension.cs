using Ocelot.Middleware.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.Gateway.OcelotExtension
{
    /// <summary>
    /// 注册ocelot中间件
    /// </summary>
    public static class DemoOcelotExtension
    {
        public static IOcelotPipelineBuilder UseDemoResponseMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<DemoResponseMiddleware>();
        }
    }
}
