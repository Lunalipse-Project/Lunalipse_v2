using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Catalogue;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Core.Cache;
using Lunalipse.Core.PlayList;
using Lunalipse.Pages.ConfigPage.Structures;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.LpsWindow;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lunalipse.Pages
{
    /// <summary>
    /// ChooseCatalogues.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseCatalogues : Page, IDialogPage, ITranslatable
    {
        CataloguePool cataloguePool;
        List<MusicEntity> WaitingForAddBatch;
        string title_dialogue;
        private string music_exist;
        private string music_exist_c;
        string selected;
        CatalogueType catalogueType;

        public Catalogue SelectedCatalogue { get; private set; } = null;

        public ChooseCatalogues(CatalogueType catalogueType = CatalogueType.USER_DEFINED)
            : this(null, catalogueType)
        {

        }
        public ChooseCatalogues(List<MusicEntity> MusicsToAdd, CatalogueType catalogueType = CatalogueType.USER_DEFINED)
        {
            InitializeComponent();
            cataloguePool = CataloguePool.Instance;
            this.catalogueType = catalogueType;
            UserDefinedCatalogue.OnSelectionChanged += UserDefinedCatalogue_OnSelectionChanged;
            UpdateList();
            WaitingForAddBatch = MusicsToAdd;

            if (catalogueType != CatalogueType.USER_DEFINED)
            {
                CreatNewButtonArea.Visibility = Visibility.Hidden;
            }
        }

        private void UserDefinedCatalogue_OnSelectionChanged(LpsDetailedListItem selected, object tag = null)
        {
            this.selected = (selected as PlaylistStruc).UUID;
            SelectedCatalogue = cataloguePool.GetCatalogue(this.selected);
        }

        void UpdateList()
        {
            UserDefinedCatalogue.Clear();
            List<Catalogue> presented_catalogue = null;
            switch (catalogueType)
            {
                case CatalogueType.USER_DEFINED:
                    presented_catalogue = cataloguePool.GetUserDefined();
                    break;
                case CatalogueType.LOCATION:
                    presented_catalogue = cataloguePool.GetLocationClassified();
                    break;
                case CatalogueType.ARTIST:
                    presented_catalogue = cataloguePool.GetArtistClassfied();
                    break;
                case CatalogueType.ALBUM:
                    presented_catalogue = cataloguePool.GetAlbumClassfied();
                    break;
                case CatalogueType.ALL:
                    presented_catalogue = cataloguePool.All;
                    break;
                default:
                    break;
            }
            foreach (Catalogue cata in presented_catalogue)
            {
                UserDefinedCatalogue.Add(new PlaylistStruc()
                {
                    UUID = cata.UUID,
                    DetailedDescription = cata.Name
                });
            }
        }
        public void UnifiedTheme(ThemeTuple obj)
        {
            if (obj == null) return;
            CreateNew.Background = obj.Primary;
            Foreground = obj.Foreground;
        }

        public bool PositiveClicked()
        {

            if (WaitingForAddBatch == null || string.IsNullOrEmpty(selected) || WaitingForAddBatch.Count == 0) return true;
            if(!cataloguePool.GetCatalogue(selected).AddMusicCollection(WaitingForAddBatch))
            {
                CommonDialog commonDialog = new CommonDialog(music_exist, music_exist_c, MessageBoxButton.OK);
                commonDialog.ShowDialog();
            }
            return true;
        }

        public bool NegativeClicked()
        {
            return false;
        }

        public void Translate(II18NConvertor i8c)
        {
            NoFavCatalogue.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, NoFavCatalogue.Tag as string);
            CreateNew.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, CreateNew.Tag as string);
            UserDefinedCatalogue.Translate(i8c);
            title_dialogue = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_ADDPLAYLIST_CREATE_TITLE");
            music_exist = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_ADDPLAYLIST_EXIST_TITLE");
            music_exist_c = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_ADDPLAYLIST_EXIST_MUSIC");
        }

        private void CreateNew_Click(object sender, RoutedEventArgs e)
        {
            UniversalDailogue addCatalogues = new UniversalDailogue(new AddCatalogues(), title_dialogue, MessageBoxButton.YesNo);
            if(addCatalogues.ShowDialog().Value)
            {
                UpdateList();
            }
        }
    }
}
