using Lunalipse.Common.Generic.Themes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// CatalogueSelectionListItem.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogueSelectionListItem : UserControl
    {

        

        public Brush SelectedColor { get; set; }
        public Brush DefaultColor { get; set; }
        private bool isSelected=false;
        public bool Selected => isSelected;
        public CatalogueSelectionListItem()
        {
            InitializeComponent();
            SelectedColor = new BrushConverter().ConvertFromString("#ff233c7c") as SolidColorBrush;
            DefaultColor = Brushes.Transparent;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Foreground = obj.Foreground;
            SelectedColor = obj.Secondary;
            if (isSelected)
                TagIcon.Background = SelectedColor;
        }

        public string CatalogueText
        {
            get => Text.Text;
            set => Text.Text = value;
        }

        public void SetSelected()
        {
            TagIcon.Background = SelectedColor;
            isSelected = true;
        }
        public void SetUnselected()
        {
            TagIcon.Background = DefaultColor;
            isSelected = false;
        }

        private void CATALOGUE_LIST_ITEM_Loaded(object sender, RoutedEventArgs e)
        {

            if (Tag != null)
            {
                switch ((string)Tag)
                {
                    case "ALBUM_COLLECTION":
                        TagIcon.Content = FindResource("Album");
                        break;
                    case "USER_PLAYLIST":
                        TagIcon.Content = FindResource("Favorite");
                        break;
                    case "ARTIST_COLLECTION":
                        TagIcon.Content = FindResource("Artist");
                        break;
                    case "GENERAL_CONFIG":
                        TagIcon.Content = FindResource("Setting");
                        break;
                    case "MENU_DETAIL":
                        TagIcon.Content = FindResource("Menu");
                        break;
                    case "INTERNET_MUSIC":
                        TagIcon.Content = FindResource("CloudMusic");
                        break;
                    default:
                        TagIcon.Content = FindResource("Music_Collection");
                        break;
                }
            }
            else
                TagIcon.Content = FindResource("Favorite_Outline");
        }
    }
}
