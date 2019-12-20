using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunalipse.Utilities.Misc
{
    public class LpsCommandParser
    {
        public static bool checkFormat(string[] commands)
        {
            for(int i = 1; i < commands.Length; i++)
            {
                if(i%2 != 0 && !commands[i].StartsWith("-"))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckQuote(string commandline)
        {
            int i = 0;
            bool meetAQuote = false;
            for(int j=0;j<commandline.Length;j++)
            {
                if (commandline[j] == '"')
                {
                    if(!meetAQuote)
                    {
                        i++;
                        meetAQuote = true;
                    }
                    else
                    {
                        i--;
                        meetAQuote = false;
                    }
                }
            }
            return i == 0;
        }

        public static string[] ParseCommand(string commandline, char terminator=' ')
        {
            List<string> cmds = new List<string>();
            bool withinQuote = false, withinBracket = false;
            string component = "";
            for (int i = 0; i < commandline.Length; i++)
            {
                while (i<commandline.Length && (commandline[i] != terminator || withinQuote || withinBracket))
                {
                    if (commandline[i] == '"' && commandline[(i) == 0 ? i : i - 1] != '\\')
                    {
                        withinQuote = withinQuote ? false : true;
                    }
                    if(!withinQuote && (commandline[i]=='(' || commandline[i] == ')'))
                    {
                        if (commandline[i] == '(') withinBracket = true;
                        if (commandline[i] == ')') withinBracket = false;
                        i++;
                        continue;
                    }
                    component += commandline[i];
                    i++;
                }
                cmds.Add(component);
                component = "";
                withinQuote = false;
            }
            return cmds.ToArray();
        }
    }
}
