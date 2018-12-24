using Lunalipse.Common.Generic.Themes;
using Lunalipse.Utilities;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// HintableLable.xaml 的交互逻辑
    /// </summary>
    public partial class HintableLable : UserControl
    {
        const string COMPONENT_ID = "PR_COMP_HintableLable";
        
        public HintableLable()
        {
            InitializeComponent();
            _hint.PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade;
            this.MouseEnter += (a, b) => _hint.IsOpen = true;
            this.MouseLeave += (a, b) => _hint.IsOpen = false;
            HintBorder = new SolidColorBrush(Colors.Transparent);

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Foreground = obj.Foreground;
            HintBackground = obj.Primary.ToCelestia();
        }

        public object ContentLabel
        {
            get => GenericLabel.Content;
            set => GenericLabel.Content = value;
        }
        public object ContentHint
        {
            get => HintContent.Content;
            set => HintContent.Content = value;
        }

        public Brush HintBackground
        {
            get => HintContent.Background;
            set => HintContent.Background = value;
        }

        public Brush HintBorder
        {
            get => Hint.BorderBrush;
            set => Hint.BorderBrush = value;
        }
    }
}
