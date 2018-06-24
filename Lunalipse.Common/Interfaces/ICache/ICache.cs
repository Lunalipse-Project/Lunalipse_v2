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
        string CacheTo<T>(T instance, WinterWrapUp cw, string fieldName = null) where T : ICachable;
        T RestoreTo<T>(object text) where T : ICachable;
        object RestoreField<T>(T ancestor, object ctx, string fieldName);
    }
}
