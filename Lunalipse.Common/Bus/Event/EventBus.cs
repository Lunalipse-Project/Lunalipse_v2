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
        /// 当某个耗时任务被执行时所触发的广播。
        /// </summary>
        public static event Action<EventBusTypes, object> OnActionProcced;

        public void BoardcastAction(EventBusTypes ActionStatus, object Tag)
        {
            OnActionProcced?.Invoke(ActionStatus, Tag);
        }
    }
}
