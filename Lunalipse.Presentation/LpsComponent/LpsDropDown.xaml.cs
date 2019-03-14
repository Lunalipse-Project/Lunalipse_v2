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

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// LpsDropDown.xaml 的交互逻辑
    /// </summary>
    public partial class LpsDropDown : UserControl
    {
        public event Action<object> OnSelectionChanged;
        ObservableCollection<DropDownItem> DropdownsSource = new ObservableCollection<DropDownItem>();
        public LpsDropDown()
        {
            InitializeComponent();
            DropDownItems.ItemsSource = DropdownsSource;
            MouseDown += LpsDropDown_MouseDown;
        }

        private void LpsDropDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectionArea.IsOpen = !SelectionArea.IsOpen;
        }

        public void Add(string ItemKey,object ItemObject)
        {
            DropdownsSource.Add(new DropDownItem()
            {
                Key = ItemKey,
                Value = ItemObject
            });
        }

        private void DropDownItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DropDownItem dropDownItem = DropDownItems.SelectedItem as DropDownItem;
            if (dropDownItem == null) return;
            SelectedContent.Text = dropDownItem.Key;
            SelectionArea.IsOpen = false;
            OnSelectionChanged?.Invoke(dropDownItem.Value);
        }

        public int SelectedIndex
        {
            get
            {
                return DropDownItems.SelectedIndex;
            }
            set
            {
                if (value < 0 || value >= DropDownItems.Items.Count) return;
                DropDownItems.SelectedIndex = value;
            }
        }

        public void SetSelectionByVal(object Value)
        {
            foreach(DropDownItem dropDownItem in DropdownsSource)
            {
                if(dropDownItem.Value.Equals(Value))
                {
                    DropDownItems.SelectedItem = dropDownItem;
                    break;
                }
            }
        }
    }
    class DropDownItem
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public FontFamily LabelFontFamily
        {
            get
            {
                if (Value is FontFamily)
                    return Value as FontFamily;
                return new FontFamily("Microsoft YaHei UI");
            }
            set
            {

            }
        }
    }
}
