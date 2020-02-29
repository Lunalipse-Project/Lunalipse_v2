using Lunalipse.Core.BehaviorScript.ScriptV3.LetterElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Core.BehaviorScript.ScriptV3
{
    public class Utils
    {
        public static ElementType GetLetterType(Type type)
        {
            switch(Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return ElementType.NUMBER;
                case TypeCode.String:
                    return ElementType.STRING;
                case TypeCode.Boolean:
                    return ElementType.BOOL;
                default:
                    return ElementType.GAMMY;
            }
        }
    }
}
