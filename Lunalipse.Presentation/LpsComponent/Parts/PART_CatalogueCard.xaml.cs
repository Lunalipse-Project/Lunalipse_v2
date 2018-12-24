using Lunalipse.Common.Generic.Themes;
using Lunalipse.Presentation.Generic;
using Lunalipse.Utilities;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lunalipse.Presentation.LpsComponent.Parts
{
    /// <summary>
    /// PART_CatalogueCard.xaml 的交互逻辑
    /// </summary>
    public partial class PART_CatalogueCard : UserControl
    {

        public event Action OnCatalogueDeleted;
        public event Action<PART_CatalogueCard> OnCatalogueSelected;

        private Duration elapseTime = new Duration(TimeSpan.FromMilliseconds(400));
        private DoubleAnimation FloatingUp, FloatingDown;

        public PART_CatalogueCard()
        {
            InitializeComponent();
            this.MouseDown += PART_CatalogueCard_MouseDown;
            this.MouseEnter += PART_CatalogueCard_MouseEnter;
            this.MouseLeave += PART_CatalogueCard_MouseLeave;
            cata_cover.Stretch = Stretch.UniformToFill;

            FloatingUp      =    new DoubleAnimation(7, 14, elapseTime);
            FloatingDown    =    new DoubleAnimation(14, 7, elapseTime);

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Background = BottomStripColor = obj.Primary.SetOpacity(1);
            Foreground = obj.Foreground;
        }

        private void PART_CatalogueCard_MouseLeave(object sender, MouseEventArgs e)
        {
            Outliner.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, FloatingDown);
            e.Handled = true;
        }

        private void PART_CatalogueCard_MouseEnter(object sender, MouseEventArgs e)
        {
            Outliner.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, FloatingUp);
            e.Handled = true;
        }



        private void PART_CatalogueCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnCatalogueSelected?.Invoke(this);
            e.Handled = true;
        }

        public string CatalogueUid { get; set; }

        public int Row { get; set; }
        public int Col { get; set; }

        public SolidColorBrush BottomStripColor
        {
            get
            {
                return (SolidColorBrush)cata_card_bottom_strip.Background;
            }
            set
            {
                cata_card_bottom_strip.Background = value;
            }
        }

        public string CatalogueTitle
        {
            get
            {
                return cata_name.Text;
            }
            set
            {
                cata_name.Text = value;
            }
        }

        public ImageSource CatalogueCover
        {
            get
            {
                return cata_cover.Source;
            }
            set
            {
                if (value == null)
                    @default.Visibility = Visibility.Visible;
                else
                    @default.Visibility = Visibility.Hidden;
                cata_cover.Source = value;
            }
        }

        private void OnCatalogueDelete(object sender, RoutedEventArgs e)
        {
            OnCatalogueDeleted?.Invoke();
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //this.Opacity = 0.5;
        }
    }
}
