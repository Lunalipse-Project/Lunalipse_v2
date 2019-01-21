using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Core.PlayList;
using Lunalipse.Presentation.BasicUI;
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
    /// AddCatalogues.xaml 的交互逻辑
    /// </summary>
    public partial class AddCatalogues : Page, IDialogPage, ITranslatable
    {
        string PlaylistExistTitle, PlayListExistContent;
        CataloguePool cataloguePool;
        public AddCatalogues()
        {
            InitializeComponent();
            cataloguePool = CataloguePool.INSATNCE;
        }

        public bool NegativeClicked()
        {
            return false;
        }

        public bool PositiveClicked()
        {
            string name = PlayListName.Text;
            if (cataloguePool.SearchCatalogue(name).Count != 0)
            {
                CommonDialog Dialog = new CommonDialog(PlaylistExistTitle, PlayListExistContent.FormateEx(name), MessageBoxButton.OK);
                Dialog.ShowDialog();
                return false;
            }
            cataloguePool.AddCatalogue(CatalogueFactory.CreateUser(name) as Catalogue);
            EventBus.Instance.Boardcast(EventBusTypes.ON_ACTION_COMPLETE, "C_UPD_USR");
            return true;
        }

        public void Translate(II18NConvertor i8c)
        {
            Hint.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, Hint.Tag as string);
            PlaylistExistTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_ADDPLAYLIST_EXIST_TITLE");
            PlayListExistContent = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_ADDPLAYLIST_EXIST_CONTENT");
        }

        public void UnifiedTheme(ThemeTuple themeTuple)
        {
            Foreground = themeTuple.Foreground;
        }
    }
}
