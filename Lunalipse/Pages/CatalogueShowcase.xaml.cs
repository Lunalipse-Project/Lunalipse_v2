using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.PlayList;
using Lunalipse.Presentation.LpsWindow;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Lunalipse.Pages
{
    /// <summary>
    /// CatalogueShowCase.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogueShowCase : Page
    {
        List<Catalogue> catalogues;
        II18NConvertor converter;

        private string EDIT_CATALOGUE_TITLE;

        public event Action<Catalogue> CatalogueSelected;

        public CatalogueShowCase()
        {
            InitializeComponent();
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            showcase.OnCatalogueSelectChanged += Showcase_OnCatalogueSelectChanged;
            showcase.OnCatalogueEditRequest += Showcase_OnCatalogueEditRequest;
            converter = TranslationManagerBase.AquireConverter();
            TranslationManagerBase_OnI18NEnvironmentChanged(converter);


        }


        private void TranslationManagerBase_OnI18NEnvironmentChanged(II18NConvertor obj)
        {
            EDIT_CATALOGUE_TITLE = obj.ConvertTo(Common.Data.SupportedPages.CORE_FUNC, "CORE_CATALOGUE_INFO_TITLE");
            showcase.Translate(converter);
        }

        private void Showcase_OnCatalogueEditRequest(Common.Interfaces.IPlayList.ICatalogue obj)
        {
            UniversalDailogue EditCatalogue = new UniversalDailogue(new CatalogueEditPage(obj), EDIT_CATALOGUE_TITLE, MessageBoxButton.OK);
            EditCatalogue.ShowDialog();
        }

        public void SetCatalogues(List<Catalogue> catalogues)
        {
            
            this.catalogues = catalogues;
            showcase.ClearAll();
            if(catalogues!=null)
            {
                foreach (Catalogue c in catalogues)
                {
                    showcase.Add(c);
                }
            }
            showcase.Translate(converter);
        }

        private void Showcase_OnCatalogueSelectChanged(int arg1, int arg2, string arg3)
        {
            CatalogueSelected?.Invoke(catalogues.Find(x => x.UUID.Equals(arg3)));
        }
    }
}
