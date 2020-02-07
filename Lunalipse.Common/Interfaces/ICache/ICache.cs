using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lunalipse.Common.Generic.Cache.SerializeInfo;

namespace Lunalipse.Common.Interfaces.ICache
{
    public interface ICache
    {
        byte[] CacheToBin<T>(T instance, WinterWrapUp wwu);
        T BinRestoreTo<T>(byte[] bytes, out WinterWrapUp winterWrapUp);
    }
}
