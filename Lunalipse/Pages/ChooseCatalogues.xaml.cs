using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Core.Cache;
using Lunalipse.Core.PlayList;
using Lunalipse.Pages.ConfigPage.Structures;
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
        MusicEntity WaitingForAdd;
        string title_dialogue;
        string selected;
        public ChooseCatalogues(MusicEntity Target)
        {
            InitializeComponent();
            cataloguePool = CataloguePool.INSATNCE;
            UserDefinedCatalogue.OnSelectionChanged += UserDefinedCatalogue_OnSelectionChanged;
            UpdateList();
            WaitingForAdd = Target;
        }

        private void UserDefinedCatalogue_OnSelectionChanged(LpsDetailedListItem selected, object tag = null)
        {
            this.selected = (selected as PlaylistStruc).UUID;
        }

        void UpdateList()
        {
            UserDefinedCatalogue.Clear();
            foreach (Catalogue cata in cataloguePool.GetUserDefined())
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
            if (WaitingForAdd == null || string.IsNullOrEmpty(selected)) return true;
            cataloguePool.GetCatalogue(selected).AddMusic(WaitingForAdd);
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
