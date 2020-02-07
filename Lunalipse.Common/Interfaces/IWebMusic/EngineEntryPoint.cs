using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Interfaces.IWebMusic
{
    public class EngineEntryPoint : Attribute
    {
        public string EngineName { get; private set; }
        public EngineEntryPoint(string EngineName)
        {
            this.EngineName = EngineName;
        }
    }
}
