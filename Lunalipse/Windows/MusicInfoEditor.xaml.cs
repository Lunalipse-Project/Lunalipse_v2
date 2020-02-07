using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.ICommunicator;
using Lunalipse.Common.Interfaces.IMetadata;
using Lunalipse.Common.Interfaces.IWebMusic;
using Lunalipse.Core.Metadata;
using Lunalipse.Core.WebMusic;
using Lunalipse.Pages;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;
using LunaNetCore;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using CommonDialog = Lunalipse.Presentation.BasicUI.CommonDialog;

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
        IWebMusicDetail webMusicDetail;

        string LyricOnLocal, LyricOnWeb, LyricNotFound;
        string SaveChangeTitle, SaveChangeBody;
        string NSaveTitle,NSaveWeb,NSaveInUse;
        string Downloading, chooseALocation;
        bool isReadonly;
        public MusicInfoEditor(MusicEntity musicEntity, bool isReadonly = false, IWebMusicDetail webMusicDetail = null)
        {
            EnableFocused = musicEntity.HasImage;
            InitializeComponent();
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            if (!musicEntity.IsInternetLocation)
            {
                metadataWriter = new MediaMetadataWriter(musicEntity.Path);
                DownloadRegion.Visibility = Visibility.Hidden;
            }
            this.musicEntity = musicEntity;
            this.webMusicDetail = webMusicDetail;
            Unloaded += MusicInfoEditor_Unloaded;
            Title.Text = musicEntity.MusicName;
            Artist.Text = musicEntity.ArtistFrist;
            Album.Text = musicEntity.Album;

            Title.IsReadOnly = isReadonly;
            Artist.IsReadOnly = isReadonly;
            Album.IsReadOnly = isReadonly;

            this.isReadonly = isReadonly;
        }

        private void MusicInfoEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged -= TranslationManagerBase_OnI18NEnvironmentChanged;
            Unloaded -= MusicInfoEditor_Unloaded;
        }

        private void InfoEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            musicEntity.DisposePicture();
            if (!isReadonly)
            {
                ApplyChange();
            }
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            ChooseCatalogues chooseCatalogues = new ChooseCatalogues(Common.Generic.Catalogue.CatalogueType.LOCATION);
            UniversalDailogue locationBroswer = new UniversalDailogue(chooseCatalogues, chooseALocation,MessageBoxButton.OK);
            bool downloadSucc = false;
            if (locationBroswer.ShowDialog() == true)
            {
                string path = chooseCatalogues.SelectedCatalogue.Name;
                ProgressDialogue progressDialogue = new ProgressDialogue(indicator =>
                {
                    DownloadingMusic(indicator, path, out downloadSucc);
                });
                progressDialogue.ShowDialog();
                if(downloadSucc)
                {
                    MusicEntity newme = musicEntity.Clone() as MusicEntity;
                    newme.IsInternetLocation = false;
                    newme.Path = $"{path}/{musicEntity.Name}{musicEntity.Extension}";
                    newme.LyricPath = $"{path}/Lyrics/{musicEntity.Name}.lrc";
                    //newme.MusicID = Utils.getRandomID();
                    chooseCatalogues.SelectedCatalogue.AddMusic(newme);
                }
            }
        }

        private void DownloadingMusic(IProgressIndicator indicator, string path, out bool noerror)
        {
            indicator.UpdateCaption(Downloading);
            indicator.SetRange(-1, -1);
            IWebMusicsSearchEngine searchEngine = SearchEngineManager.Instance.CurrentSelected;
            if (searchEngine == null)
            {
                noerror = false;
                indicator.Complete();
            }

            //Getting meta data
            Tuple<string, string> download_info =
                    searchEngine.GetDownloadURL(webMusicDetail, (EngineAudioQuality)QualityChoose.SelectedItemValue);
            string lyric = searchEngine.GetLyric(webMusicDetail);

            if (!string.IsNullOrEmpty(lyric))
            {
                using (FileStream fs = new FileStream($"{path}/Lyrics/{musicEntity.Name}.lrc", FileMode.Create))
                {
                    byte[] lrc = Encoding.UTF8.GetBytes(lyric);
                    fs.Write(lrc, 0, lrc.Length);
                }
            }

            indicator.SetRange(0, 100);
            if (download_info == null)
            {
                indicator.Complete();
                noerror = false;
            }
            else
            {
                musicEntity.Path = download_info.Item1;
                musicEntity.Extension = $".{download_info.Item2}";
            }
            Downloader downloader = new Downloader();
            string filePath = $"{path}/{musicEntity.Name}{musicEntity.Extension}";
            downloader.OnTaskUpdate += (downloaded, total) =>
            {
                double precentage = (double)downloaded / (double)total * 100;
                indicator.ChangeCurrentVal(precentage, $"{Math.Round(precentage, 3)}% ");
            };
            downloader.OnDownloadFinish += (error) =>
            {
                if (error != null)
                {
                    System.Windows.Forms.MessageBox.Show(error.Message, "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (File.Exists(filePath))
                    {
                        MediaMetadataWriter metadataWriter = new MediaMetadataWriter(filePath);
                        metadataWriter.SetPicture(0, musicEntity.AlbumPicture);
                        metadataWriter.SetArtist(0, musicEntity.ArtistFrist);
                        metadataWriter.SetAlbum(musicEntity.Album);
                        metadataWriter.SetTitle(musicEntity.MusicName);
                        metadataWriter.Done();
                    }
                }
                indicator.Complete();
            };
            downloader.DownloadFile(musicEntity.Path, filePath, GLS.INSTANCE.ProxySetting);
            noerror = true;
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(Common.Interfaces.II18N.II18NConvertor obj)
        {
            foreach (ContentControl contentControl in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (contentControl.Tag == null) continue;
                if (!(contentControl.Tag is string)) continue;
                contentControl.Content = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, contentControl.Tag as string);
            }
            if(musicEntity.IsInternetLocation)
            {
                QualityChoose.Clear();
                foreach (EngineAudioQuality quality in Enum.GetValues(typeof(EngineAudioQuality)))
                {
                    QualityChoose.Add(
                        obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, 
                        $"CORE_MEEDITOR_{quality.ToString()}"), quality);
                }
                QualityChoose.SelectedIndex = 1;
            }
            LyricOnLocal = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ONLOCAL");
            LyricOnWeb = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ONWEB");
            LyricNotFound = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_NONE");
            SaveChangeBody = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ISSVAE_MSG");
            SaveChangeTitle = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ISSVAE_CAP");
            NSaveTitle = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ERR_CAP");
            NSaveWeb = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ERR_MSG_WEB");
            NSaveInUse = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_ERR_MSG_LOCAL");
            Downloading = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_DOWNLOADING");
            chooseALocation = obj.ConvertTo(SupportedPages.CORE_MUSICENTITY_EDITOR, "CORE_MEEDITOR_DOWNLOAD_LOCATION");
        }

        protected override void DialogueLoaded(object sender, EventArgs args)
        {
            base.DialogueLoaded(sender, args);
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
            MediaMetaDataReader.RetrievePictureFromCache(musicEntity);
            profileImage = MediaMetaDataReader.GetPicture(musicEntity);
            if (musicEntity.HasImage)
            {
                MusicProfileImage.Background = new ImageBrush(profileImage);
                SetFocusedBackground(profileImage);
            }
            if (musicEntity.LyricPath != string.Empty && File.Exists(musicEntity.LyricPath))
            {
                LyricFileName.Content = LyricOnLocal;
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
            Download.Background = obj.Secondary;
            QualityChoose.DropDownBackground = obj.Secondary;
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
