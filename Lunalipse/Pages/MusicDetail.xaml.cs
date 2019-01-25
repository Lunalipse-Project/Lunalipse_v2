using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
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
    /// MusicDetail.xaml 的交互逻辑
    /// </summary>
    public partial class MusicDetail : Page
    {
        II18NConvertor converter;

        MusicEntity musicEntity;

        string Artist,Album;

        public MusicDetail(MusicEntity musicEntity, Brush source)
        {
            InitializeComponent();
            converter = TranslationManagerBase.AquireConverter();
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            this.musicEntity = musicEntity;
            AlbumPicture.Background = source;
            if (source==null)
            {
                NoPictureFound.Visibility = Visibility.Visible;
            }
            else
            {
                NoPictureFound.Visibility = Visibility.Hidden;
            }
            Artist = musicEntity.Artist.Length > 0 ? musicEntity.ArtistFrist : musicEntity.DefaultArtist;
            Album = string.IsNullOrEmpty(musicEntity.Album) ? musicEntity.DefaultAlbum : musicEntity.Album;
            MusicName.Text = musicEntity.MusicName;

            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            Foreground = obj.Foreground;
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(II18NConvertor obj)
        {
            MusicArtist.Text = obj.ConvertTo(SupportedPages.CORE_FUNC, Artist);
            MusicAlbum.Text = obj.ConvertTo(SupportedPages.CORE_FUNC, Album);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
