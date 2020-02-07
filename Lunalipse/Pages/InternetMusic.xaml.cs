using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.Cache;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.IWebMusic;
using Lunalipse.Core;
using Lunalipse.Core.Cache;
using Lunalipse.Core.Metadata;
using Lunalipse.Core.WebMusic;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Utilities;
using Lunalipse.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lunalipse.Pages
{
    /// <summary>
    /// Interaction logic for InternetMusic.xaml
    /// </summary>
    public partial class InternetMusic : Page
    {
        SearchEngineManager engineManager;
        BlurEffect blurEffect;
        DoubleAnimation BlurOut, BlurIn;
        CacheHub cacheSystem;
        LpsCore core;
        List<IWebMusicDetail> details;
        LunalipseLogger logger;
        bool isSearchReinitiate = false;

        int currentPage = 0;
        string keyword;

        string i18n_loading_title = string.Empty;
        string i18n_loading_cimg = string.Empty;
        string i18n_loading_cdata = string.Empty;
        string i18n_error_title = string.Empty;
        string i18n_error_content = string.Empty;

        public InternetMusic()
        {
            InitializeComponent();
            engineManager = SearchEngineManager.Instance;
            cacheSystem = CacheHub.Instance();
            blurEffect = new BlurEffect();
            BlurOut = new DoubleAnimation(0, 7, new Duration(TimeSpan.FromMilliseconds(500)));
            BlurIn = new DoubleAnimation(7, 0, new Duration(TimeSpan.FromMilliseconds(500)));
            details = new List<IWebMusicDetail>();
            core = LpsCore.GetSession("MAIN_AUDIO_SESSION");
            logger = LunalipseLogger.GetLogger();

            Loaded += InternetMusic_Loaded;
            Unloaded += InternetMusic_Unloaded;
            blurEffect.Radius = 0;
            blurEffect.KernelType = KernelType.Gaussian;
            musicListBox.Effect = blurEffect;
            
        }

        private void InternetMusic_Unloaded(object sender, RoutedEventArgs e)
        {
            engineManager.OnRequesting -= EngineManager_OnRequesting;
            engineManager.OnRespond -= EngineManager_OnRespond;
            engineManager.OnError -= EngineManager_OnError;
            musicListBox.OnEntrySideEffectInvoked -= MusicListBox_OnEntrySideEffectInvoked;
            musicListBox.OnMainEffectInvoked -= MusicListBox_OnMainEffectInvoked;
            musicListBox.OnBottomTouched -= MusicListBox_OnBottomTouched;
        }

        private void InternetMusic_Loaded(object sender, RoutedEventArgs e)
        {
            engineManager.OnRequesting += EngineManager_OnRequesting;
            engineManager.OnRespond += EngineManager_OnRespond;
            engineManager.OnError += EngineManager_OnError;
            musicListBox.OnEntrySideEffectInvoked += MusicListBox_OnEntrySideEffectInvoked;
            musicListBox.OnMainEffectInvoked += MusicListBox_OnMainEffectInvoked;
            musicListBox.OnBottomTouched += MusicListBox_OnBottomTouched;
            Loading.Visibility = Visibility.Hidden;
        }

        

        private void MusicListBox_OnBottomTouched()
        {
            engineManager.CurrentSelected.SearchMusicByKeyword(keyword, 20, ++currentPage);
        }

        // Main effect will play the music
        private void MusicListBox_OnMainEffectInvoked(MusicEntity selected, object tag = null)
        {
            IWebMusicDetail webMusicDetail = details[musicListBox.SelectedIndex];
            WebAudioStuffs AudioCache = null;
            if (cacheSystem.ComponentCacheExists(Common.Generic.Cache.CacheType.WebAudioStuff, selected.MusicID))
            {
                AudioCache = cacheSystem.RestoreObject<WebAudioStuffs>(selected.MusicID, Common.Generic.Cache.CacheType.WebAudioStuff);
            }
            if (AudioCache == null)
            {
                AudioCache = CacheAudioData(selected, webMusicDetail);
            }
            if (AudioCache != null)
            {
                selected.Path = AudioCache.downloadURL;
                selected.Extension = AudioCache.fileType;
                selected.LyricContent = AudioCache.lyric;
                core.PrepareMusicBytesRepresentation(selected,AudioCache.audioData);
            }
            else
            {
                //Unable to get playback url or unable to get audio file
            }
        }

        private WebAudioStuffs CacheAudioData(MusicEntity selected, IWebMusicDetail webMusicDetail)
        {
            WebAudioStuffs stuff = null;
            ProgressDialogue progressDialogue = new ProgressDialogue(indicator =>
            {
                indicator.SetRange(0, -1);
                indicator.UpdateCaption(i18n_loading_title);
                WebClient webClient = new WebClient();
                webClient.Proxy = GLS.INSTANCE.ProxySetting;
                Tuple<string, string> url = engineManager.CurrentSelected.GetDownloadURL(webMusicDetail, EngineAudioQuality.QUALITY_LOW);
                string lyric = engineManager.CurrentSelected.GetLyric(webMusicDetail);
                if (url != null)
                {
                    selected.Path = url.Item1;
                    selected.Extension = $".{url.Item2}";
                }
                else
                {
                    selected.Path = null;
                    indicator.Complete();
                    return;
                }
                if (!cacheSystem.ComponentCacheExists(Common.Generic.Cache.CacheType.WebAlbumPic, selected.MusicID))
                {
                    indicator.ChangeCurrentVal(-1, i18n_loading_cimg); //正在缓冲专辑封面
                    byte[] img = webClient.DownloadData(webMusicDetail.getAlbumPicture());
                    cacheSystem.CacheObject(img, Common.Generic.Cache.CacheType.WebAlbumPic, selected.MusicID);
                    selected.InitializePicture(img);
                }
                else
                {
                    if (selected.AlbumPicture == null)
                    {
                        MediaMetaDataReader.RetrievePictureFromCache(selected);
                    }
                }
                if (!cacheSystem.ComponentCacheExists(Common.Generic.Cache.CacheType.WebAudioStuff,selected.MusicID))
                {
                    indicator.ChangeCurrentVal(-1, i18n_loading_cdata); //正在缓冲音频
                    byte[] audioData = webClient.DownloadData(selected.Path);
                    stuff = new WebAudioStuffs()
                    {
                        audioData = audioData,
                        downloadURL = selected.Path,
                        fileType = selected.Extension,
                        lyric = lyric ?? string.Empty
                    };
                    cacheSystem.CacheObject(stuff, Common.Generic.Cache.CacheType.WebAudioStuff, selected.MusicID);
                }
                indicator.Complete();
            });
            progressDialogue.ShowDialog();
            return stuff;
        }

        private void MusicListBox_OnEntrySideEffectInvoked(MusicEntity obj)
        {
            IWebMusicDetail webMusicDetail = details[musicListBox.SelectedIndex];
            if(obj.HasImage)
            {
                if (!cacheSystem.ComponentCacheExists(Common.Generic.Cache.CacheType.WebAlbumPic, obj.MusicID))
                {
                    // Download picture
                    ProgressDialogue progressDialogue = new ProgressDialogue(indicator =>
                    {
                        indicator.SetRange(0, -1);
                        indicator.UpdateCaption(i18n_loading_cimg);
                        WebClient webClient = new WebClient();
                        webClient.Proxy = GLS.INSTANCE.ProxySetting;
                        byte[] img = webClient.DownloadData(webMusicDetail.getAlbumPicture());
                        cacheSystem.CacheObject(img, Common.Generic.Cache.CacheType.WebAlbumPic, obj.MusicID);
                        indicator.Complete();
                    });
                    progressDialogue.ShowDialog();
                }
            }
            MusicInfoEditor musicInfoEditor = new MusicInfoEditor(obj, true, webMusicDetail);
            musicInfoEditor.ShowDialog();
        }

        private void EngineManager_OnRespond(EngineActionType obj)
        {
            Dispatcher.Invoke(() =>
            {
                HideLoading();
                if (obj == EngineActionType.QUERY_SONGS_LIST)
                {
                    AddToList(engineManager.CurrentSelected.GetMusics());
                }
                logger.Info($"Responded Normally: ActionType={obj.ToString()}");
            });
        }

        private void EngineManager_OnError(Exception exception)
        {
            Dispatcher.Invoke(() =>
            {
            CommonDialog dialog = new CommonDialog(
                    i18n_error_title,
                    i18n_error_content.FormateEx(exception.HResult, exception.Message.ToString().LimitLength(45))
                    , MessageBoxButton.OK);
                dialog.ShowDialog();
                HideLoading();
            });
        }

        private void EngineManager_OnRequesting(EngineActionType obj)
        {
            Dispatcher.Invoke(() => ShowLoading());
        }

        private void InternetMusicSearch_Loaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManager_OnI18NEnvironmentChanged;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            TranslationManager_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            Foreground = obj.Foreground;
            SearchIt.Background = obj.Secondary.ToLuna();
            SearchBox.BorderBrush = obj.Secondary;
            SearchIt.BorderBrush = obj.Secondary;
        }

        private void TranslationManager_OnI18NEnvironmentChanged(II18NConvertor i8c)
        {
            foreach (ContentControl contentControl in Utils.FindVisualChildren<ContentControl>(this))
            {
                if (contentControl.Tag == null) continue;
                if (!(contentControl.Tag is string)) continue;
                contentControl.Content = i8c.ConvertTo(SupportedPages.CORE_CLOUD_LIB, contentControl.Tag as string);
            }
            musicListBox.Translate(i8c);
            i18n_loading_title = i8c.ConvertTo(SupportedPages.CORE_CLOUD_LIB, "CORE_CLOUD_LIB_MSG_LOADING_T");
            i18n_loading_cimg = i8c.ConvertTo(SupportedPages.CORE_CLOUD_LIB, "CORE_CLOUD_LIB_MSG_LOADING_CIMG");
            i18n_loading_cdata = i8c.ConvertTo(SupportedPages.CORE_CLOUD_LIB, "CORE_CLOUD_LIB_MSG_LOADING_CDATA");
            i18n_error_title = i8c.ConvertTo(SupportedPages.CORE_CLOUD_LIB, "CORE_CLOUD_LIB_MSG_ERR_T");
            i18n_error_content = i8c.ConvertTo(SupportedPages.CORE_CLOUD_LIB, "CORE_CLOUD_LIB_MSG_ERR_C");
        }

        private void InternetMusicSearch_Unloaded(object sender, RoutedEventArgs e)
        {
            TranslationManagerBase.OnI18NEnvironmentChanged -= TranslationManager_OnI18NEnvironmentChanged;
            ThemeManagerBase.OnThemeApplying -= ThemeManagerBase_OnThemeApplying;
        }

        private void AddToList(List<IWebMusicDetail> musicDetails)
        {
            if(isSearchReinitiate)
            {
                musicListBox.Clear();
                details.Clear();
                isSearchReinitiate = false;
            }
            details.AddRange(musicDetails);
            foreach(IWebMusicDetail webMusicDetail in musicDetails)
            {
                MusicEntity musicEntity = new MusicEntity();
                musicEntity.Album = webMusicDetail.getAlbumName();
                musicEntity.Name = webMusicDetail.getMusicName();
                musicEntity.Artist = new string[] { webMusicDetail.getArtistName() };
                musicEntity.MusicID = webMusicDetail.getID();
                musicEntity.HasImage = webMusicDetail.getAlbumPicture() != string.Empty;
                musicEntity.EstDuration = TimeSpan.FromMilliseconds(webMusicDetail.getTotalSeconds());
                musicEntity.IsInternetLocation = true;
                musicListBox.Add(musicEntity);
            }
            musicListBox.UpdateList();
            HideLoading();
        }

        private void SearchIt_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text != string.Empty)
            {
                isSearchReinitiate = true;
                logger.Info($"Initiate search, keyword: \"{keyword}\"");
                engineManager.CurrentSelected.SearchMusicByKeyword(keyword = SearchBox.Text, 20, currentPage);
            }
        }

        private void HideLoading()
        {
            if(Loading.Visibility != Visibility.Hidden)
            {
                musicListBox.Effect.BeginAnimation(BlurEffect.RadiusProperty, BlurIn);
                Loading.Visibility = Visibility.Hidden;
            }
        }

        private void ShowLoading()
        {
            musicListBox.Effect.BeginAnimation(BlurEffect.RadiusProperty, BlurOut);
            Loading.Visibility = Visibility.Visible;
            //Loading.InitiateSpinning();
        }
        
    }
}
