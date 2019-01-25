using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Bus.Event
{
    public enum EventBusTypes
    {
        /// <summary>
        /// 一个动作的开始
        /// </summary>
        ON_ACTION_START,
        /// <summary>
        /// 更新动作状态
        /// </summary>
        ON_ACTION_UPDATE,
        /// <summary>
        /// 一个动作结束
        /// </summary>
        ON_ACTION_COMPLETE,
        /// <summary>
        /// 动作请求的删除操作
        /// </summary>
        ON_ACTION_REQ_DELETE,
        /// <summary>
        /// 动作请求的添加/新建操作
        /// </summary>
        ON_ACTION_REQ_NEW,
        /// <summary>
        /// 动作请求的启用操作
        /// </summary>
        ON_ACTION_REQ_ENABLE,
        /// <summary>
        /// 动作请求的禁用操作
        /// </summary>
        ON_ACTION_REQ_DISABLE
    }
}
