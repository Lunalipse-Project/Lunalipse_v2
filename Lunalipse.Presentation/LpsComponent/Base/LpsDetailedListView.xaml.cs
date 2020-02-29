using Lunalipse.Common.Data;
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

        public ObservableCollection<LpsDetailedListItem> Items = new ObservableCollection<LpsDetailedListItem>();
        public event OnItemSelected<LpsDetailedListItem> OnSelectionChanged;

        int _index = 0;
        int _prev_selected = -1;

        public LpsDetailedListView()
        {
            InitializeComponent();
            DataContext = this;
            ITEMS.ItemsSource = Items;
            if (Items.Count == 0)
            {
                NoItem.Visibility = Visibility.Visible;
            }
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Items.Count == 0)
            {
                NoItem.Visibility = Visibility.Visible;
            }
            else if (NoItem.Visibility == Visibility.Visible)
            {
                NoItem.Visibility = Visibility.Hidden;
            }
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
            foreach (LpsDetailedListItem ldi in Items)
            {
                if (string.IsNullOrEmpty(ldi.I18NDescription)) continue;
                ldi.DetailedDescription = i8c.ConvertTo(SupportedPages.CORE_FUNC, ldi.I18NDescription);
            }
            NoItemHint.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, NoItemHint.Tag as string);
            ITEMS.UpdateLayout();
        }

        public int SelectedIndex
        {
            get => _index;
            set
            {
                if (value >= Items.Count) return;
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
                OnSelectionChanged(Items[_index]);
            }
        }

        public LpsDetailedListItem SelectedItem
        {
            get
            {
                if (_index >= 0)
                {
                    return Items[_index];
                }
                return null;
            }
            set
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Equals(value))
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        public void Add(LpsDetailedListItem sc)
        {
            Items.Add(sc);
            NoItem.Visibility = Visibility.Hidden;
        }

        public void Clear()
        {
            Items.Clear();
            NoItem.Visibility = Visibility.Visible;
        }

        public void Remove(LpsDetailedListItem sc)
        {
            Items.Remove(sc);
            if (NoItem.Visibility == Visibility.Hidden && Items.Count == 0)
                NoItem.Visibility = Visibility.Visible;
        }

        public void Swap(int srcIndex,int destIndex)
        {
            Items.Insert(destIndex, Items[srcIndex]);
            Items.Insert(srcIndex + 1, Items[destIndex + 1]);
            Items.RemoveAt(srcIndex + 2);
            Items.RemoveAt(destIndex + 1);
            this.UpdateLayout();
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
            _index = Items.IndexOf(sc);
            _prev_selected = _index;
            OnSelectionChanged?.Invoke(sc);
        }
    }
}
