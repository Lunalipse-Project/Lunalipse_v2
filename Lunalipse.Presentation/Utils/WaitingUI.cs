using Lunalipse.Common.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Presentation.Utils
{
    public static class WaitingUI
    {
        public static void AsyncExecute(this IWaitable comp,Action task)
        {
            Task.Run(() =>
            {
                comp.GetDispatcher().Invoke(() =>
                {
                    comp.StartWait();
                    task.Invoke();
                    comp.StopWait();
                });
            });
        } 
    }
}
