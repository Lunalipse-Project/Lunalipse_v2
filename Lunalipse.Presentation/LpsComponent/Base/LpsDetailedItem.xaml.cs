using Lunalipse.Common.Generic.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lunalipse.Presentation.LpsComponent.Base
{
    /// <summary>
    /// LpsDetailedItem.xaml 的交互逻辑
    /// </summary>
    public partial class LpsDetailedItem : UserControl
    {
        public static readonly DependencyProperty ICON_SIZE =
            DependencyProperty.Register("IconSize",
                        typeof(double),
                        typeof(LpsDetailedItem),
                        new PropertyMetadata(Application.Current.FindResource("IconLarge")));

        public SolidColorBrush SelectedColor { get; set; }

        public double IconSize
        {
            get
            {
                return (double)GetValue(ICON_SIZE);
            }
            set
            {
                SetValue(ICON_SIZE, value);
            }
        }

        public LpsDetailedItem()
        {
            InitializeComponent();
            //Set default
            SelectedColor = Brushes.Black;
            Loaded += (a, b) =>
            {
                ITEM_ICON.Content = Application.Current.FindResource(ITEM_ICON.Content);
            };
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            SelectedColor = obj.Foreground;
            Foreground = SelectedColor;
        }

        public void Select()
        {
            SelectedMark.Background = SelectedColor;
            Tag = true;
        }
        public void Unselect()
        {
            SelectedMark.Background = Brushes.Transparent;
            Tag = false;
        }
    }
}
