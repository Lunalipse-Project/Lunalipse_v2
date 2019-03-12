using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.IPlayList;
using Lunalipse.Presentation.Generic;
using Lunalipse.Presentation.LpsComponent.Parts;
using Lunalipse.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lunalipse.Presentation.LpsComponent
{
    /// <summary>
    /// LpsGridList.xaml 的交互逻辑
    /// </summary>
    public partial class LpsGridList : UserControl, ITranslatable
    {
        const int CardPerRow = 3;
        int totalRows = 1;
        int totalCard = 0;
        //the index of the row for user selected card
        int currentRow = 0;
        //In-row offset for user selected card
        int CurrentOffset = 0;

        Brush OverlayColor;

        //int row, int col, string uid
        public event Action<int, int, string> OnCatalogueSelectChanged;
        public event Action<ICatalogue> OnCatalogueEditRequest;

        public LpsGridList()
        {
            InitializeComponent();
            Scroller.ScrollChanged += Scroller_ScrollChanged;
        }

        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Console.WriteLine(e.VerticalOffset);
        }

        public void Add(ICatalogue catalogue)
        {
            totalCard++;
            int colum = totalCard % CardPerRow;
            colum = colum == 0 ? 2 : colum - 1;
            PART_CatalogueCard cataCard = new PART_CatalogueCard(catalogue);
            cataCard.Margin = new Thickness(0, 20, 0, 20);
            cataCard.Col = colum;
            cataCard.Row = totalRows;
            cataCard.OnCatalogueSelected += catalogueSelected;
            cataCard.OnCatalogueEditRequest += CataCard_OnCatalogueEditRequest;
            if (totalCard > totalRows * CardPerRow)
            {
                //New Row required
                totalRows++;
                cataCard.Row = totalRows;
                Grid g = CreateContainer();
                Grid.SetColumn(cataCard, colum);
                g.Children.Add(cataCard);
                StackedItems.Children.Add(g);
                
            }
            else
            {
                Grid grid = (Grid)StackedItems.Children[StackedItems.Children.Count - 1];
                Grid.SetColumn(cataCard, colum);
                grid.Children.Add(cataCard);
            }
            if(EmptyTip.Visibility != Visibility.Hidden && totalCard != 0)
            {
                EmptyTip.Visibility = Visibility.Hidden;
            }
        }

        private void CataCard_OnCatalogueEditRequest(ICatalogue obj)
        {
            OnCatalogueEditRequest?.Invoke(obj);
        }

        private void catalogueSelected(PART_CatalogueCard obj)
        {
            OnCatalogueSelectChanged?.Invoke(obj.Row, obj.Col, obj.Uid);
        }

        public bool RemoveAt(int row, int offset)
        {
            int cardAt = (row - 1) * CardPerRow + offset;
            int counter = 1;
            if (cardAt > totalCard || offset > 3 || row * offset == 0) return false;
            IteratingCard(card =>
            {
                if(counter==cardAt)
                {
                    ((Grid)StackedItems.Children[row - 1]).Children.RemoveAt(offset - 1);
                    for (int r = row - 1; r < StackedItems.Children.Count;)
                    {
                        _loopInner(offset, ref r);
                        r++;
                    }
                    if(((Grid)StackedItems.Children[totalRows - 1]).Children.Count == 0)
                    {
                        StackedItems.Children.RemoveAt(totalRows - 1);
                        totalRows--;
                    }
                    return true;
                }
                counter++;
                return false;
            });
            totalCard--;
            return true;
        }

        public void ClearAll()
        {
            totalCard = 0;
            totalRows = 0;
            StackedItems.Children.Clear();
        }

        private Grid CreateContainer()
        {
            Grid container = new Grid();
            for(int i = 0; i < 3; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(0.3, GridUnitType.Star);
                container.ColumnDefinitions.Add(cd);
            }
            return container;
        }

        private void IteratingCard(Func<PART_CatalogueCard, bool> delegation)
        {
            foreach(object g in StackedItems.Children)
            {
                Grid grid = (Grid)g;
                for(int i=0;i<grid.Children.Count;i++)
                {
                    if (delegation((PART_CatalogueCard)grid.Children[i])) break;
                }
            }
        }

        private void _loopInner(int offset, ref int r)
        {
            bool flag = false;
            for (int c = offset - 1; c < CardPerRow;)
            {
                PART_CatalogueCard temp_card;
                if (c + 1 >= CardPerRow)
                {
                    c = 0;
                    r++;
                    flag = true;
                    temp_card = (PART_CatalogueCard)((Grid)StackedItems.Children[r]).Children[c];
                    ((Grid)StackedItems.Children[r]).Children.RemoveAt(c);
                    Grid.SetColumn(temp_card, CardPerRow - 1);
                    r--;
                }
                else
                {
                    temp_card = (PART_CatalogueCard)((Grid)StackedItems.Children[r]).Children[c + 1];
                    ((Grid)StackedItems.Children[r]).Children.RemoveAt(c + 1);
                    Grid.SetColumn(temp_card, c);
                }
                ((Grid)StackedItems.Children[r]).Children.Add(temp_card);
                if (flag) break;
            }
        }

        public void Translate(II18NConvertor i8c)
        {
            EmptyTip.Content = i8c.ConvertTo(SupportedPages.CORE_FUNC, EmptyTip.Content.ToString());
            IteratingCard((card) =>
            {
                string name = card.CatalogueTitle;
                if(name.Equals("CORE_CATALOGUE_AllMusic"))
                {
                    card.CatalogueTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_CATALOGUE_AllMusic");
                    return true;
                }
                return false;
            });
        }
    }
}
