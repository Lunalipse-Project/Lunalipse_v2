using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Presentation.Generic;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// LpsDetailedListView.xaml 的交互逻辑
    /// </summary>
    public partial class LpsDetailedListView : UserControl
    {
        public static readonly DependencyProperty ITEM_HOVER =
            DependencyProperty.Register("DETAILEDLISTVIEW_ITEMHOVER",
                                typeof(Brush),
                                typeof(LpsDetailedListView),
                                new PropertyMetadata(Application.Current.FindResource("ItemHoverColorDefault")));
        public static readonly DependencyProperty ITEM_UNHOVER =
            DependencyProperty.Register("DETAILEDLISTVIEW_UNHOVER",
                                typeof(Brush),
                                typeof(LpsDetailedListView),
                                new PropertyMetadata(Application.Current.FindResource("ItemUnhoverColorDefault")));

        public static readonly DependencyProperty ICON_SIZE =
            DependencyProperty.Register("ListIconSize",
                        typeof(double),
                        typeof(LpsDetailedListView),
                        new PropertyMetadata(Application.Current.FindResource("IconLarge")));

        public ObservableCollection<LpsDetailedListItem> Classes = new ObservableCollection<LpsDetailedListItem>();
        public event OnItemSelected<LpsDetailedListItem> OnSelectionChanged;

        int _index = 0;
        int _prev_selected = -1;

        public LpsDetailedListView()
        {
            InitializeComponent();
            DataContext = this;
            ITEMS.ItemsSource = Classes;

            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            ITEMS.Background = obj.Primary.SetOpacity(1).ToLuna();
        }

        public SolidColorBrush ItemHovered
        {
            get => (SolidColorBrush)GetValue(ITEM_HOVER);
            set => SetValue(ITEM_HOVER, value);
        }
        public SolidColorBrush ItemUnhovered
        {
            get => (SolidColorBrush)GetValue(ITEM_UNHOVER);
            set => SetValue(ITEM_UNHOVER, value);
        }
        public double ListIconSize
        {
            get => (double)GetValue(ICON_SIZE);
            set => SetValue(ICON_SIZE, value);
        }

        public void Translate(II18NConvertor i8c)
        {
            foreach (LpsDetailedListItem ldi in Classes)
            {
                ldi.DetailedDescription = i8c.ConvertTo("CORE_FUNC", ldi.DetailedDescription);
            }
            ITEMS.UpdateLayout();
        }

        public int SelectedIndex
        {
            get => _index;
            set
            {
                if (value >= Classes.Count) return;
                if ((_index = value) == -1)
                {
                    for (int i = 0; i < ITEMS.Items.Count; i++)
                    {
                        GetContainer(i).Unselect();
                    }
                    _prev_selected = value;
                    OnSelectionChanged(null);
                    return;
                }
                _index = value;
                if (_prev_selected != -1)
                {
                    GetContainer(_prev_selected).Unselect();
                }
                GetContainer(_index).Select();
                _prev_selected = _index;
                OnSelectionChanged(Classes[_index]);
            }
        }

        public LpsDetailedListItem SelectedItem
        {
            get
            {
                if (_index != -1)
                {
                    return Classes[_index];
                }
                return null;
            }
            set
            {
                for (int i = 0; i < Classes.Count; i++)
                {
                    if (Classes[i].Equals(value))
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        public void Add(LpsDetailedListItem sc)
        {
            Classes.Add(sc);
        }

        public LpsDetailedItem GetContainer(int index)
        {
            var container = (ITEMS.ItemContainerGenerator
                        .ContainerFromIndex(index) as FrameworkElement);
            return ITEMS.ItemTemplate.FindName("ItemContainer", container) as LpsDetailedItem;
        }

        private void ItemContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LpsDetailedItem spsi = sender as LpsDetailedItem;
            LpsDetailedItem prev_spsi = null;
            if (spsi == null) return;
            LpsDetailedListItem sc = spsi.DataContext as LpsDetailedListItem;
            if (_prev_selected != -1)
            {
                prev_spsi = GetContainer(_prev_selected);
                if (prev_spsi == null) return;
                prev_spsi.Unselect();
            }
            spsi.Select();

            OnSelectionChanged?.Invoke(sc);
            _index = Classes.IndexOf(sc);
            _prev_selected = _index;
        }
    }
}
