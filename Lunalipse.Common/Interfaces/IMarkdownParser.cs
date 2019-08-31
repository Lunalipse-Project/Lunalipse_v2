using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Lunalipse.Common.Interfaces
{
    public interface IMarkdownParser
    {
        FlowDocument CreateDocument(string markdown);
        Brush DocumentForeground { get; set; }
        Brush ReferenceForeground { get; set; }
    }
}
