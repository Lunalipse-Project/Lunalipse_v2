using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Common.Data.BehaviorScript
{
    public class BScriptLocation
    {
        string _name, _location;
        public string ScriptName { get => _name; }
        public string ScriptLocation { get => _location; }
        public BScriptLocation(string name, string location)
        {
            _name = name;
            _location = location;
        }
    }
}
