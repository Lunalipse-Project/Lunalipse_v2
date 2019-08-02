using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.ICommunicator
{
    public interface IProgressIndicator
    {
        void SetRange(double min, double max);
        void ChangeCurrentVal(double current, string message = null);
        void Complete();
    }
}
