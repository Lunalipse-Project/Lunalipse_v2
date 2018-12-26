using Lunalipse.Common.Data;
using Lunalipse.Common.Generic;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Presentation.Generic;
using Lunalipse.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int __index = -1;

        public event OnItemSelected<MusicEntity> ItemSelectionChanged;
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

        public MusicSelectionList()
        {
            InitializeComponent();
            ITEMS.DataContext = Items;
            Loading.Visibility = Visibility.Hidden;
            Items.CollectionChanged += (x, y) =>
            {
                
            };
            Delegation.RemovingItem += dctx =>
            {
                MusicEntity removed = dctx as MusicEntity;
                if (dctx is MusicEntity)
                {
                    if (!IsMotherCatalogue)
                    {
                        Delegation.CatalogueUpdated?.Invoke(removed);
                        Items.Remove(removed);
                    }
                    else
                    {
                        // TODO 永久从母分类中删除歌曲（本地文件永久删除），包括：提醒
                    }
                }
            };
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            ITEMS.ItemContainerGenerator.StatusChanged += (a, b) => OnListStatusChanged?.Invoke(ITEMS.ItemContainerGenerator.Status);
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            ItemUnhovered = obj.Foreground.SetOpacity(0);
            ItemHovered = obj.Foreground.SetOpacity(0.15);
        }

        [Obsolete]
        private void Add(MusicEntity mie) => Items.Add(mie);
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
                    if (CatalogueInUse == null) CatalogueInUse = value;
                    foreach (MusicEntity me in DisplayedCatalogue.GetAll())
                        Items.Add(me);
                    CheckElemets();
                }
            }
        }

        public bool IsMotherCatalogue
        {
            get;
            set;
        }

        public MusicEntity SelectedItem { get; private set; }

        public int SelectedIndex
        {
            get => __index;
            set
            {
                if (__index == -1)
                {
                    for (int i = 0; i < ITEMS.Items.Count; i++)
                    {
                        MusicSelectionListItem Container = GetContainer(i);
                        if (Container.Tag as Boolean? != false)
                        {
                            Container.RemoveChosen();
                            break;
                        }
                    }
                }
                else
                {
                    MusicSelectionListItem Temp = GetContainer(__index);
                    Temp.RemoveChosen();
                    Temp = GetContainer(__index = value);
                    Temp.SetChosen();
                    ItemSelectionChanged(Temp.DataContext as MusicEntity);
                }
            }
        }

        private void ItemConatiner_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DisplayedCatalogue.Uid() != CatalogueInUse.Uid())
                CatalogueInUse = DisplayedCatalogue;

            MusicSelectionListItem Item = (MusicSelectionListItem)sender;
            MusicSelectionListItem Temp;
            MusicEntity selected = Item.DataContext as MusicEntity;
            if (__index != -1)
            {
                Temp = GetContainer(__index);
                Temp.RemoveChosen();
            }
            if (selected != null)
            {
                __index = Items.IndexOf(selected);
                Item.SetChosen();
                ItemSelectionChanged(SelectedItem = selected);
            }
        }

        private MusicSelectionListItem GetContainer(int index)
        {
            var container = (ITEMS.ItemContainerGenerator
                        .ContainerFromIndex(index) as FrameworkElement);
            return ITEMS.ItemTemplate.FindName("ItemConatiner",container) as MusicSelectionListItem;
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

        private void CheckElemets()
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
    }
}
