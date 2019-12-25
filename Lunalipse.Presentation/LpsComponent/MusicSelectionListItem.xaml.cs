using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Presentation.Generic;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// MusicSelectionListItem.xaml 的交互逻辑
    /// </summary>
    public partial class MusicSelectionListItem : UserControl
    {
        public static readonly DependencyProperty ITEM_SELECTED_MARK =
            DependencyProperty.Register("MUSICLST_ITEM_SELECTED_MARK",
                                        typeof(Brush),
                                        typeof(MusicSelectionListItem),
                                        new PropertyMetadata(Application.Current.FindResource("ArtistName_List")));
        public static readonly DependencyProperty PEnableAddToPlayList =
            DependencyProperty.Register("EnableAddToPlayList", typeof(bool), typeof(MusicSelectionListItem), new PropertyMetadata(true));
        public static readonly DependencyProperty PEnableEditOrSeeDetails =
            DependencyProperty.Register("EnableEditOrSeeDetails", typeof(bool), typeof(MusicSelectionListItem), new PropertyMetadata(true));
        public static readonly DependencyProperty PEnableDeletion =
            DependencyProperty.Register("EnableDeletion", typeof(bool), typeof(MusicSelectionListItem), new PropertyMetadata(true));

        public bool EnableAddToPlayList
        {
            get => (bool)GetValue(PEnableAddToPlayList);
            set => SetValue(PEnableAddToPlayList, value);
        }
        public bool EnableEditOrSeeDetails
        {
            get => (bool)GetValue(PEnableEditOrSeeDetails);
            set => SetValue(PEnableEditOrSeeDetails, value);
        }
        public bool EnableDeletion
        {
            get => (bool)GetValue(PEnableDeletion);
            set => SetValue(PEnableDeletion, value);
        }
        /// <summary>
        /// 选中标识颜色
        /// </summary>
        public Brush MarkColor
        {
            get => (Brush)GetValue(ITEM_SELECTED_MARK);
            set => SetValue(ITEM_SELECTED_MARK, value);
        }

        public MusicSelectionListItem()
        {
            InitializeComponent();
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            Loaded += MusicSelectionListItem_Loaded;
        }

        private void MusicSelectionListItem_Loaded(object sender, RoutedEventArgs e)
        {
            AddToList.Visibility = EnableAddToPlayList ? Visibility.Visible : Visibility.Collapsed;
            EditOrSeeDetails.Visibility = EnableEditOrSeeDetails ? Visibility.Visible : Visibility.Collapsed;
            Deletion.Visibility = EnableDeletion ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Foreground = obj.Foreground;
            MarkColor = obj.Foreground;
            if (SelectedMark.Background != null)
                SetChosen();
        }

        public void SetChosen()
        {
            Tag = true;
            SelectedMark.Background = MarkColor;
        }

        public void RemoveChosen()
        {
            Tag = false;
            SelectedMark.Background = null;
        }

        private void MusicRemove(object sender, RoutedEventArgs e)
        {
            Delegation.RemovingItem?.Invoke((sender as Button).DataContext);
            e.Handled = true;
        }

        private void AddCatalogueBelonging(object sender, System.Windows.RoutedEventArgs e)
        {
            Delegation.AddToNewCatalogue?.Invoke((sender as Button).DataContext);
            e.Handled = true;
        }

        private void OpenMetadataEditor(object sender, RoutedEventArgs e)
        {
            Delegation.EditMetadata?.Invoke((sender as Button).DataContext as MusicEntity);
            e.Handled = true;
        }
    }
}
