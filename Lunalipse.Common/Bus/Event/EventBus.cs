using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Bus.Event
{
    public class EventBus
    {

        static volatile EventBus EVT_BUS_INSTANCE = null;
        static readonly object EVT_BUS_LOCK = new object();

        public static EventBus Instance
        {
            get
            {
                if (EVT_BUS_INSTANCE == null)
                {
                    lock (EVT_BUS_LOCK)
                    {
                        EVT_BUS_INSTANCE = EVT_BUS_INSTANCE ?? new EventBus();
                    }
                }
                return EVT_BUS_INSTANCE;
            }
        }

        /// <summary>
        /// 接受事件总线的全局广播
        /// </summary>
        public static event Action<EventBusTypes, object> OnBoardcastRecieved;

        /// <summary>
        /// 接受事件总线的多路广播
        /// </summary>
        public static event Action<EventBusTypes, object, object> OnMulticastRecieved;

        /// <summary>
        /// 单播接受端注册表
        /// </summary>
        Dictionary<Type, Action<EventBusTypes, object[]>> UnicastRegisterTable;

        /// <summary>
        /// 向事件总线发送一个全局广播
        /// </summary>
        /// <param name="ActionStatus">广播信息类别</param>
        /// <param name="Tag">广播消息</param>
        public void Boardcast(EventBusTypes ActionStatus, object Tag)
        {
            OnBoardcastRecieved?.Invoke(ActionStatus, Tag);
        }

        /// <summary>
        /// 向事件总线发送一个多路广播
        /// </summary>
        /// <param name="ActionStatus">广播信息类别</param>
        /// <param name="Tag">广播消息</param>
        /// <param name="RecieverIdentity">接受者标识符</param>
        public void Multicast(EventBusTypes ActionStatus, object Tag, object RecieverIdentity)
        {
            OnMulticastRecieved?.Invoke(ActionStatus, Tag, RecieverIdentity);
        }

        /// <summary>
        /// 向事件总线进行点对点单播
        /// </summary>
        /// <param name="ActionStatus">广播信息类别</param>
        /// <param name="Receiver">单播接受者</param>
        /// <param name="Message">携带的信息</param>
        public void Unicast(EventBusTypes ActionStatus, Type Receiver, params object[] Message)
        {
            UnicastRegisterTable[Receiver]?.Invoke(ActionStatus, Message);
        }
    }
}
