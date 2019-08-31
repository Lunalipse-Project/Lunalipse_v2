using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Core.Metadata;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;
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
namespace Lunalipse.Windows
{
    /// <summary>
    /// Interaction logic for MusicInfoEditor.xaml
    /// </summary>
    public partial class MusicInfoEditor : LunalipseDialogue
    {
        MusicEntity musicEntity;
        ImageSource profileImage = null;
        IMediaMetadataWriter metadataWriter;

        string LyricOnLocal, LyricOnWeb, LyricNotFound;
        string SaveChangeTitle, SaveChangeBody;
        string NSaveTitle,NSaveWeb,NSaveInUse;
        public MusicInfoEditor(MusicEntity musicEntity)
        {
            EnableFocused = musicEntity.HasImage;
            InitializeComponent();
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            if (!musicEntity.IsInternetLocation)
            {
                metadataWriter = new MediaMetadataWriter(musicEntity.Path);
            }
            this.musicEntity = musicEntity;
            Unloaded += MusicInfoEditor_Unloaded;
            Title.Text = musicEntity.MusicName;
            Artist.Text = musicEntity.ArtistFrist;
            Album.Text = musicEntity.Album;           
        }

        private void MusicInfoEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged -= TranslationManagerBase_OnI18NEnvironmentChanged;
            Unloaded -= MusicInfoEditor_Unloaded;
        }

        private void InfoEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ApplyChange();
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(Common.Interfaces.II18N.II18NConvertor obj)
        {
            foreach (ContentControl contentControl in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (contentControl.Tag == null) continue;
                if (!(contentControl.Tag is string)) continue;
                contentControl.Content = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, contentControl.Tag as string);
            }
            LyricOnLocal = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ONLOCAL");
            LyricOnWeb = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ONWEB");
            LyricNotFound = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_NONE");
            SaveChangeBody = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ISSVAE_MSG");
            SaveChangeTitle = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ISSVAE_CAP");
            NSaveTitle = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ERR_CAP");
            NSaveWeb = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ERR_MSG_WEB");
            NSaveInUse = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ERR_MSG_LOCAL");
        }

        protected override void DialogueLoaded(object sender, EventArgs args)
        {
            base.DialogueLoaded(sender, args);
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
            profileImage = MediaMetaDataReader.GetPicture(musicEntity.Path);
            if (musicEntity.HasImage)
            {
                MusicProfileImage.Background = new ImageBrush(profileImage);
                SetFocusedBackground(profileImage);
            }
            if (musicEntity.HasLyricLocal)
            {
                LyricFileName.Content = LyricOnLocal;
            }
            else if (musicEntity.HasLyricOnline)
            {
                LyricFileName.Content = LyricOnWeb;
            }
            else
            {
                LyricFileName.Content = LyricNotFound;
            }
        }

        protected override void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            base.ThemeManagerBase_OnThemeApplying(obj);
            ChangeLyric.Background = obj.Secondary;
        }

        void ApplyChange()
        {
            string name = Title.Text;
            string artist = Artist.Text;
            string album = Album.Text;
            if (musicEntity.MusicName != name || musicEntity.ArtistFrist != artist || musicEntity.Album != album)
            {
                string body = SaveChangeBody.FormateEx(Path.GetFileName(musicEntity.Path), Path.GetDirectoryName(musicEntity.Path));
                bool result = new CommonDialog(SaveChangeTitle, body, MessageBoxButton.YesNo).ShowDialog().Value;
                if(!result)
                {
                    return;
                }
            }
            if(musicEntity.IsInternetLocation)
            {
                new CommonDialog(NSaveTitle, NSaveWeb, MessageBoxButton.OK).ShowDialog();
                return;
            }
            else
            {
                if (IsFileUsing(musicEntity.Path))
                {
                    new CommonDialog(NSaveTitle, NSaveInUse, MessageBoxButton.OK).ShowDialog();
                    return;
                }
            }

            metadataWriter.SetArtist(0, artist);
            metadataWriter.SetAlbum(album);
            metadataWriter.SetTitle(name);
            metadataWriter.Done();
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
