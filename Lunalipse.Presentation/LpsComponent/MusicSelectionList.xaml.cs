using Lunalipse.Common.Bus.Event;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.Generic;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// MusicSelectionList.xaml 的交互逻辑
    /// </summary>
    public partial class MusicSelectionList : UserControl, ITranslatable, IWaitable
    {
        private ICatalogue DisplayedCatalogue;
        private ICatalogue CatalogueInUse;
        private ObservableCollection<MusicEntity> Items = new ObservableCollection<MusicEntity>();
        private Dictionary<int,MusicEntity> SelectedEntityIndex = new Dictionary<int, MusicEntity>();
        private int __index = -1;
        private Point startPoint;
        private MusicSelectionListItem lastClickedSender;

        private string deleteTitle, deleteContent;

        public event Action OnBottomTouched;
        public event Action<MusicEntity> OnEntrySideEffectInvoked;
        public event Action<MusicEntity,object> OnMainEffectInvoked;
        public event Action<GeneratorStatus> OnListStatusChanged;

        public static readonly DependencyProperty ITEM_HOVER =
            DependencyProperty.Register("MUSICLST_HOVERCOLOR",
                                        typeof(Brush),
                                        typeof(MusicSelectionList),
                                        new PropertyMetadata(Application.Current.FindResource("ItemHoverColorDefault")));
        public static readonly DependencyProperty ITEM_UNHOVER =
            DependencyProperty.Register("MUSICLST_UNHOVERCOLOR",
                                        typeof(Brush),
                                        typeof(MusicSelectionList),
                                        new PropertyMetadata(Application.Current.FindResource("ItemUnhoverColorDefault")));
        public static readonly DependencyProperty IsNotWebMusicShowcase =
            DependencyProperty.Register("MUSICLIST_IsNotWebMusicShowcase",
                                        typeof(bool),
                                        typeof(MusicSelectionList),
                                        new PropertyMetadata(true));
        DispatcherTimer timer = new DispatcherTimer();

        public Brush ItemHovered
        {
            get => GetValue(ITEM_HOVER) as Brush;
            set => SetValue(ITEM_HOVER, value);
        }
        public Brush ItemUnhovered
        {
            get => GetValue(ITEM_UNHOVER) as Brush;
            set => SetValue(ITEM_UNHOVER, value);
        }

        public bool isNotWebMusicShowcase
        {
            get => (bool)GetValue(IsNotWebMusicShowcase);
            set => SetValue(IsNotWebMusicShowcase, value);
        }

        /// <summary>
        /// Indicate the default behavior has filpped.
        /// In default, single clicked is side while double clicked is main effect.
        /// </summary>
        public bool isBehaviorFipped
        {
            get;
            set;
        } = false;

        public MusicSelectionList()
        {
            InitializeComponent();
            ITEMS.DataContext = Items;
            Loading.Visibility = Visibility.Hidden;
            Items.CollectionChanged += (x, y) =>
            {
                ITEMS.UpdateLayout();
            };
            //DragDrop.DoDragDrop
            Delegation.RemovingItem = RemoveItem;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            ITEMS.ItemContainerGenerator.StatusChanged += (a, b) => OnListStatusChanged?.Invoke(ITEMS.ItemContainerGenerator.Status);
            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Tick += Timer_Tick;
        }

        private void RemoveItem(object dctx)
        {
            if (dctx is MusicEntity)
            {
                DeselectAll();
                MusicEntity removed = dctx as MusicEntity;
                if (IsRootCatalogue)
                {
                    CommonDialog commonDialog = new CommonDialog(
                        deleteTitle, 
                        deleteContent.FormateEx(removed.MusicName, removed.Path), 
                        MessageBoxButton.YesNo);
                    if (commonDialog.ShowDialog().Value)
                    {
                        File.Delete(removed.Path);
                    }
                    else
                    {
                        return;
                    }
                }
                EventBus.Instance.Multicast(EventBusTypes.ON_ACTION_REQ_DELETE, dctx, DisplayedCatalogue.Uid());
                Items.Remove(removed);
            }
        }


        /// <summary>
        /// Single Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            if(isBehaviorFipped)
            {
                MainEffect();
            }
            else
            {
                SideEffect();
            }
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            ItemHovered = obj.Foreground.SetOpacity(0.15);
            ItemUnhovered = obj.Foreground.SetOpacity(0);
        }

        [Obsolete]
        public void Add(MusicEntity mie) => Items.Add(mie);
        public void Clear()
        {
            TipMessage.Visibility = Visibility.Hidden;
            Items.Clear();
        }
        public List<MusicEntity> CurrentItems => Items.ToList();

        public ICatalogue Catalogue
        {
            get => DisplayedCatalogue;
            set
            {
                if (value == null) return;
                if (DisplayedCatalogue == null || value.Uid() != DisplayedCatalogue.Uid())
                {
                    Items.Clear();
                    DisplayedCatalogue = value;
                    IsRootCatalogue = DisplayedCatalogue.IsLocationClassification();
                    if (CatalogueInUse == null) CatalogueInUse = value;
                    foreach (MusicEntity me in DisplayedCatalogue.GetAll())
                        Items.Add(me);
                    UpdateList();
                }
            }
        }

        /// <summary>
        /// Indicate whether current loaded catalogue is root.
        /// Meaning that other catalogue is derived from that such catalogue.
        /// </summary>
        public bool IsRootCatalogue
        {
            get;
            set;
        }

        public MusicEntity SelectedItem { get; private set; }
        public List<MusicEntity> AllSelectedItems
        {
            get
            {
                return SelectedEntityIndex.Values.ToList();
            }
        }

        // Change the visual effect of ListBox when selected something programatically
        public int SelectedIndex
        {
            get => __index;
            set
            {
                MusicSelectionListItem Temp;
                if(__index != -1)   //If index is not -1, means we have already selected something before
                {
                    Temp = GetContainer(__index);
                    Temp.Unchoose();
                }
                else if(value==-1) // value is -1, means unselect all
                {
                    __index = -1;
                    for (int i = 0; i < ITEMS.Items.Count; i++)
                    {
                        MusicSelectionListItem Container = GetContainer(i);
                        if (Container.Tag as Boolean? != false)
                        {
                            Container.Unchoose();
                            break;
                        }
                    }
                    return;
                }
                Temp = GetContainer(__index = value);
                Temp.Choose();
            }
        }

        public void Translate(II18NConvertor i8c)
        {
            TipMessage.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, (string)TipMessage.Content);
            Hint.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, Hint.Content as string);
            NoSongsHint.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, NoSongsHint.Content as string);
            TranslateList(i8c);
        }

        public void TranslateList(II18NConvertor i8c)
        {
            foreach(MusicEntity me in Items)
            {
                if(!string.IsNullOrEmpty(me.DefaultAlbum))
                {
                    me.Album = i8c.ConvertTo(SupportedPages.CORE_FUNC, me.DefaultAlbum);
                }
                if(!string.IsNullOrEmpty(me.DefaultArtist))
                {
                    me.Artist[0] = i8c.ConvertTo(SupportedPages.CORE_FUNC, me.DefaultArtist);
                }
            }
            deleteTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_CATALOGUE_DELETE_PREM_TITLE");
            deleteContent = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_CATALOGUE_DELETE_PREM_CONTENT");
        }

        public void DeselectAll()
        {
            foreach (int i in SelectedEntityIndex.Keys)
            {
                MusicSelectionListItem listItem = GetContainer(i);
                if (listItem != null)
                {
                    listItem.Unchoose();
                }
            }
            SelectedEntityIndex.Clear();
        }

        public void StartWait()
        {
            Loading.Visibility = Visibility.Visible;
        }

        public void StopWait()
        {
            Loading.Visibility = Visibility.Hidden;
        }
         
        public Dispatcher GetDispatcher()
        {
            return Dispatcher;
        }

        public void UpdateList()
        {
            if (Items.Count == 0)
            {
                SongsEmpty.Visibility = Visibility.Visible;
            }
            else if (Items.Count != 0 && SongsEmpty.Visibility == Visibility.Visible)
            {
                SongsEmpty.Visibility = Visibility.Hidden;
            }
        }

        private void ITEMS_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("MusicEntity"))
            {
                dragStart = false;
                MusicEntity data = e.Data.GetData("MusicEntity") as MusicEntity;
                MusicSelectionListItem musicSelectionListItem = 
                    FindAncestor<MusicSelectionListItem>(e.OriginalSource as DependencyObject);
                int oldIndex = Items.IndexOf(data);
                int newIndex = Items.IndexOf(musicSelectionListItem.DataContext as MusicEntity);
                if (oldIndex == newIndex) return;
                Items.RemoveAt(oldIndex);
                Items.Insert(newIndex, data);
                EventBus.Instance.Multicast(EventBusTypes.ON_ACTION_UPDATE, new Tuple<int, int>(oldIndex, newIndex), CatalogueInUse.Uid());
            }
        }

        private void ITEMS_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        // It seems useless but it looks like that is related to a feature that I planned years ago.
        // I am not sure what happen if I delete this whole function.
        bool dragStart = false;
        private void ITEMS_MouseMove(object sender, MouseEventArgs e)
        {
            Point MovedPoint = e.GetPosition(null);
            Point MovePointInList = e.GetPosition(this);
            Vector diff = startPoint - MovedPoint;
            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                dragStart = true;
                MusicSelectionListItem musicSelectionListItem = 
                    FindAncestor<MusicSelectionListItem>((DependencyObject)e.OriginalSource);
                DataObject data = new DataObject("MusicEntity",musicSelectionListItem.DataContext);
                DragDrop.DoDragDrop(musicSelectionListItem, data, DragDropEffects.Move | DragDropEffects.Scroll);
            }
            //Console.WriteLine(MovePointInList);
            //if (MovePointInList.Y < 55 && dragStart)
            //{
            //    double offset = 0;
            //    if (ScrollV.VerticalOffset - 10 >= 0) offset = ScrollV.VerticalOffset - 10;
            //    ScrollV.ScrollToVerticalOffset(offset);
            //}
            //else if (MovePointInList.Y > this.ActualHeight - 55 && dragStart)
            //{
            //    double offset = 0;
            //    if (ScrollV.VerticalOffset + 10 >= ScrollV.ScrollableHeight) offset = ScrollV.VerticalOffset + 10;
            //    ScrollV.ScrollToVerticalOffset(offset);
            //}
        }

        
        private void ItemConatiner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastClickedSender = sender as MusicSelectionListItem;
            timer.Start();
            if (e.ClickCount == 2)
            {
                if (!isBehaviorFipped)
                {
                    MainEffect();
                }
                else
                {
                    SideEffect();
                }
            }
        }

        private void MainEffect()
        {
            timer.Stop();
            if (CatalogueInUse != null && DisplayedCatalogue != null)
            {
                if (DisplayedCatalogue.Uid() != CatalogueInUse.Uid())
                {
                    CatalogueInUse = DisplayedCatalogue;
                    __index = -1;
                }
            }

            MusicSelectionListItem Temp;
            MusicEntity selected = lastClickedSender.DataContext as MusicEntity;
            if (__index != -1)
            {
                Temp = GetContainer(__index);
                Temp.Unchoose();
            }
            if (selected != null)
            {
                __index = Items.IndexOf(selected);
                DeselectAll();
                lastClickedSender.Choose();
                OnMainEffectInvoked?.Invoke(SelectedItem = selected, null);
            }
            lastClickedSender = null;
        }

        private void SideEffect()
        {
            if (CatalogueInUse != null && DisplayedCatalogue != null)
            {
                if (DisplayedCatalogue.Uid() != CatalogueInUse.Uid())
                {
                    CatalogueInUse = DisplayedCatalogue;
                    __index = -1;
                }
            }
            MusicEntity selected = lastClickedSender.DataContext as MusicEntity;
            if (selected != null)
            {
                __index = Items.IndexOf(selected);
                if(!SelectedEntityIndex.ContainsKey(__index))
                {
                    SelectedEntityIndex.Add(__index,selected);
                    lastClickedSender.Choose();
                }
                else
                {
                    SelectedEntityIndex.Remove(__index);
                    lastClickedSender.Unchoose();
                }
                OnEntrySideEffectInvoked?.Invoke(selected);
            }
            lastClickedSender = null;
        }

        private void ScrollV_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ScrollV.VerticalOffset == ScrollV.ScrollableHeight && ScrollV.VerticalOffset > 0)
            {
                OnBottomTouched?.Invoke();
            }
        }

        private static T FindAncestor<T>(DependencyObject current)  where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private MusicSelectionListItem GetContainer(int index)
        {
            var container = (ITEMS.ItemContainerGenerator
                        .ContainerFromIndex(index) as FrameworkElement);
            return ITEMS.ItemTemplate.FindName("ItemConatiner", container) as MusicSelectionListItem;
        }
    }
}
