using System;
using System.Windows.Controls;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Utilities;

namespace Lunalipse.Presentation.LpsWindow
{
    /// <summary>
    /// Dialogue.xaml 的交互逻辑
    /// </summary>
    public partial class Dialogue : LunalipseDialogue
    {
        public Dialogue(string title)
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Title = title;
        }

        public Dialogue(Page content, string title) : this(title)
        {
            Width = content.Width + BorderThickness.Right + BorderThickness.Left + 16;
            Height = content.Height + 25 + 16;
            Display.Content = content;
        }

        public Dialogue(string title, string content, bool YesNo, Action<bool> PressAction = null)
            : this(new CommonDialogue(content, YesNo, PressAction), title)
        {

        }

        public Page RenderContent
        {
            get => Display.Content as Page;
            set => Display.Content = value;
        }
    }
}
