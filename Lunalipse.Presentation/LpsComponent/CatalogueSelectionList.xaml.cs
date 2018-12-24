using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Generic.Catalogue;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Utilities;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// CatalogueSelectionList.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogueSelectionList : UserControl, ITranslatable
    {
        private int __index = -5;
        private CatalogueSections TAG;

        const string UI_COMPONENT_THEME_UID = "PR_COMP_CatalogueSelectionList";

        /// <summary>
        /// 选项更改事件。参数tag的类型是<see cref="CatalogueSections"/>
        /// </summary>
        public event Action<CatalogueSections> OnSelectionChange;

        public event Action OnConfigClicked;

        public event Action OnMenuButtonClicked;

        public CatalogueSelectionList()
        {
            InitializeComponent();
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }



        public static readonly DependencyProperty ITEM_HOVER =
            DependencyProperty.Register("CATALIST_HOVERCOLOR",
                                        typeof(Brush),
                                        typeof(CatalogueSelectionList),
                                        new PropertyMetadata(Application.Current.FindResource("ItemHoverColorDefault")));
        public static readonly DependencyProperty ITEM_UNHOVER =
            DependencyProperty.Register("CATALIST_UNHOVERCOLOR",
                                        typeof(Brush),
                                        typeof(CatalogueSelectionList),
                                        new PropertyMetadata(Application.Current.FindResource("ItemUnhoverColorDefault")));

        public SolidColorBrush ItemHovered
        {
            get => (SolidColorBrush)GetValue(ITEM_HOVER);
            private set => SetValue(ITEM_HOVER, value);
        }
        public SolidColorBrush ItemUnhovered
        {
            get => (SolidColorBrush)GetValue(ITEM_UNHOVER);
            private set => SetValue(ITEM_UNHOVER, value);
        }

        public int SelectedIndex
        {
            get => __index;
            set
            {
                CatalogueSelectionListItem csli = null;
                if (value == -1)
                {
                    csli = MainCatalogue;
                    TAG = CatalogueSections.BY_LOCATION;
                }
                else if (value == -2)
                {
                    csli = AlbumCollection;
                    TAG = CatalogueSections.ALBUM_COLLECTIONS;
                }
                else if (value == -3)
                {
                    csli = UserPlaylist;
                    TAG = CatalogueSections.USER_PLAYLISTS;
                }
                else if (value == -4)
                {
                    csli = ArtistCollection;
                    TAG = CatalogueSections.ARTIST_COLLECTIONS;
                }
            }
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            ItemHovered = obj.Primary.ToLuna();
            ItemUnhovered = ItemHovered;
            ItemUnhovered.Opacity = 0;
            Foreground = obj.Foreground;
        }

        private void ItemConatiner_MouseDown(object sender, MouseButtonEventArgs args)
        {
            CatalogueSelectionListItem csli = (CatalogueSelectionListItem)sender;

            if (csli != null)
            {
                if (__index != -5)
                {
                    // For Item in listbox
                    if (__index == -1)
                        MainCatalogue.SetUnselected();
                    else if (__index == -2)
                        AlbumCollection.SetUnselected();
                    else if (__index == -3)
                        UserPlaylist.SetUnselected();
                    else if (__index == -4)
                        ArtistCollection.SetUnselected();
                }
                switch ((string)csli.Tag)
                {
                    case "MAINCATA":
                        __index = -1;
                        TAG = CatalogueSections.BY_LOCATION;
                        break;
                    case "ALBUM_COLLECTION":
                        __index = -2;
                        TAG = CatalogueSections.ALBUM_COLLECTIONS;
                        break;
                    case "USER_PLAYLIST":
                        __index = -3;
                        TAG = CatalogueSections.USER_PLAYLISTS;
                        break;
                    case "ARTIST_COLLECTION":
                        __index = -4;
                        TAG = CatalogueSections.ARTIST_COLLECTIONS;
                        break;
                }
                csli.SetSelected();
                OnSelectionChange?.Invoke(TAG);
            }
        }

        public void Translate(II18NConvertor i8c)
        {
            MainCatalogue.CatalogueText = i8c.ConvertTo("CORE_FUNC", "CORE_CATALOGUE_LOCATION");
            AlbumCollection.CatalogueText = i8c.ConvertTo("CORE_FUNC", "CORE_CATALOGUE_ALBUM");
            UserPlaylist.CatalogueText = i8c.ConvertTo("CORE_FUNC", "CORE_CATALOGUE_PLAYLIST");
            ArtistCollection.CatalogueText = i8c.ConvertTo("CORE_FUNC", "CORE_CATALOGUE_ARTSIT");
            ConfigEntry.CatalogueText = i8c.ConvertTo("CORE_FUNC", "CORE_CATALOGUE_CONFIG");
            DetailedMenu.CatalogueText = "";
        }

        private void GenericButtonDown(object sender, MouseButtonEventArgs e)
        {
            CatalogueSelectionListItem csli = (CatalogueSelectionListItem)sender;
            switch(csli.Tag as string)
            {
                case "GENERAL_CONFIG":
                    OnConfigClicked?.Invoke();
                    break;
                case "MENU_DETAIL":
                    OnMenuButtonClicked?.Invoke();
                    break;
            }
        }
    }
}
