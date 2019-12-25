using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Core.PlayList;
using Lunalipse.Presentation.BasicUI;
using System;
using System.Windows.Controls;

namespace Lunalipse.Pages
{
    /// <summary>
    /// CatalogueEditPage.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogueEditPage : Page, IDialogPage, ITranslatable
    {
        Catalogue catalogue;
        public CatalogueEditPage(ICatalogue catalogue)
        {
            InitializeComponent();
            this.catalogue = catalogue as Catalogue;
            CatalogueName.Text = this.catalogue.Name;
            SongDurDisp.Content = this.catalogue.getTotalElapse().ToString(@"hh\:mm\:ss");
            SongTotalDisp.Content = this.catalogue.GetCount();
            if (!this.catalogue.isUserDefined)
            {
                CatalogueName.IsReadOnly = true;
            }
        }

        public bool NegativeClicked()
        {
            return false;
        }

        public bool PositiveClicked()
        {
            if(CatalogueName.Text != catalogue.Name)
            {
                catalogue.Name = CatalogueName.Text;
            }
            return true;
        }

        public void Translate(II18NConvertor i8c)
        {
            Lable_SongDur.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, Lable_SongDur.Tag as string);
            Lable_SongTotal.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, Lable_SongTotal.Tag as string);
        }

        public void UnifiedTheme(ThemeTuple themeTuple)
        {
            Foreground = themeTuple.Foreground;
        }
    }
}
