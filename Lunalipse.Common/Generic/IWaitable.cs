using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Lunalipse.Common.Generic
{
    public interface IWaitable
    {
        void StartWait();
        void StopWait();

        Dispatcher GetDispatcher();
    }
}
