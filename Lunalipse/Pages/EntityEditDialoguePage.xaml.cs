using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Core.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// EntityEditDialoguePage.xaml 的交互逻辑
    /// </summary>
    public partial class EntityEditDialoguePage : Page, IDialogPage, ITranslatable
    {
        MusicEntity musicEntity;
        IMediaMetadataWriter mediaMetadataWriter;
        public EntityEditDialoguePage(MusicEntity musicEntity)
        {
            InitializeComponent();
            this.musicEntity = musicEntity;
            mediaMetadataWriter = new MediaMetadataWriter(musicEntity.Path);
            MusicName.Text = musicEntity.MusicName;
            MusicArtist.Text = musicEntity.ArtistFrist;
            MusicAlbum.Text = musicEntity.Album;
        }

        public bool NegativeClicked()
        {
            return true;
        }

        public bool PositiveClicked()
        {
            ApplyChange();
            return true;
        }

        public void Translate(II18NConvertor i8c)
        {
            TranslateLables(i8c);
        }

        void TranslateLables(II18NConvertor i8c)
        {
            foreach (Label label in FindVisualChildren<Label>(this))
            {
                if (label.Tag == null) continue;
                label.Content = i8c.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, label.Tag as string);
            }
        }

        public void UnifiedTheme(ThemeTuple themeTuple)
        {
            Foreground = themeTuple.Foreground;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        void ApplyChange()
        {
            string name = MusicName.Text;
            string artist = MusicArtist.Text;
            string album = MusicAlbum.Text;
            mediaMetadataWriter.SetArtist(0, artist);
            mediaMetadataWriter.SetAlbum(album);
            mediaMetadataWriter.SetTitle(name);
            mediaMetadataWriter.Done();
        }

        bool IsFileUsing(string path)
        {
            Stream stream = null;
            try
            {
                stream = File.Open(path, FileMode.Open);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
