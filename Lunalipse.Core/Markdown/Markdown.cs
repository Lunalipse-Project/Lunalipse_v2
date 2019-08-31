using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using PARA = Lunalipse.Core.Markdown.Paragraph;
using DocParagraph = System.Windows.Documents.Paragraph;
using System.Windows.Media;
using System.Windows;
using System.Text.RegularExpressions;
using Lunalipse.Common.Interfaces;

namespace Lunalipse.Core.Markdown
{
    public class Markdown : IMarkdownParser
    {
        Regex regex = new Regex(@"[+|-]\s?(.*)", RegexOptions.Compiled);
        public Brush DocumentForeground { get; set; }
        public Brush ReferenceForeground { get; set; }
        public FontFamily ParagraphFontFamily { get; set; } = new FontFamily("Microsoft YaHei UI");

        ParseParagraph paragraph;
        public Markdown()
        {
            paragraph = new ParseParagraph();

        }
        private List<PARA> Parse(string markdown)
        {
            List<PARA> result = new List<PARA>();
            string[] elements = markdown.Split(new[] { "\r\n" }, StringSplitOptions.None);
            string paragraphBlock = "";
            for (int i = 0; i < elements.Length; i++) 
            {
                elements[i] = elements[i].Trim('\n', '\t', '\r', ' ');
                if (string.IsNullOrEmpty(elements[i]) || i + 1 >= elements.Length ? true : regex.IsMatch(elements[i + 1])) 
                {
                    paragraphBlock += elements[i].Trim() + " ";
                    PARA pARA = paragraph.Parse(paragraphBlock);
                    paragraphBlock = "";
                    result.Add(pARA);
                }
                else
                {
                    paragraphBlock += elements[i].Trim() + " ";
                }

            }
            return result;
        }

        public FlowDocument CreateDocument(string markdown)
        {
            FlowDocument flowDocument = new FlowDocument();
            List list = null;
            List<PARA> paragraphs = Parse(markdown);
            foreach(PARA par in paragraphs)
            {
                if (par.isList)
                {
                    if (list == null)
                    {
                        list = new List();
                        list.MarkerOffset=5;
                        list.MarkerStyle = TextMarkerStyle.Disc;
                    }
                    list.ListItems.Add(new ListItem(CreateParagraph(par)));
                }
                else
                {
                    if (list != null)
                    {
                        flowDocument.Blocks.Add(list);
                        list = null;
                    }
                    flowDocument.Blocks.Add(CreateParagraph(par));
                }
            }
            if (list != null) flowDocument.Blocks.Add(list);
            return flowDocument;
        }

        private DocParagraph CreateParagraph(PARA par)
        {
            DocParagraph paragraph = new DocParagraph();
            paragraph.FontFamily = ParagraphFontFamily;
            paragraph.Margin = par.thickness;
            paragraph.FontWeight = par.isHeader ? FontWeights.SemiBold : FontWeights.Normal;
            paragraph.TextAlignment = par.needCenter ? TextAlignment.Center : par.needRight ? TextAlignment.Right : TextAlignment.Left;
            if (par.texts != null)
            {
                foreach (TextFormate textFormate in par.texts)
                {
                    Run run = new Run(textFormate.content);
                    run.FontStyle = textFormate.isItalic ? FontStyles.Italic : FontStyles.Normal;
                    if(!par.isHeader)
                        run.FontWeight = textFormate.isBold ? FontWeights.SemiBold : FontWeights.Normal;
                    paragraph.Inlines.Add(run);
                }
            }
            paragraph.Foreground = par.isReferrence ? ReferenceForeground : DocumentForeground;
            paragraph.FontSize = par.fontSize;
            return paragraph;
        }
    }
}
