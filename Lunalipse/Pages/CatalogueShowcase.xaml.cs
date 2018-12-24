using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.PlayList;
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
    /// CatalogueShowCase.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogueShowCase : Page
    {
        List<Catalogue> catalogues;
        II18NConvertor converter;

        public event Action<Catalogue> CatalogueSelected;

        public CatalogueShowCase(II18NConvertor converter)
        {
            InitializeComponent();
            showcase.OnCatalogueSelectChanged += Showcase_OnCatalogueSelectChanged;
            this.converter = converter;
            showcase.Translate(converter);
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
