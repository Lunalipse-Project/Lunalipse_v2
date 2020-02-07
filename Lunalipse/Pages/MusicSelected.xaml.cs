using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Core.PlayList;
using Lunalipse.I18N;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Presentation.Generic;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;
using Lunalipse.Windows;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Lunalipse.Pages
{
    /// <summary>
    /// MusicSelected.xaml 的交互逻辑
    /// </summary>
    public partial class MusicSelected : Page, ITranslatable
    {
        public Catalogue SelectedCatalogue { get; private set; } = null;

        public event Action<MusicEntity, object> OnSelectedMusicChange;

        bool isRecovery = false;

        SequenceControllerManager controllerManager;

        string AddToCatalogueTitle = "CORE_ADDPLAYLIST_TITLE";
        string MetadataEditorTitle = "CORE_MUSICENTITY_EDITOR_TITLE";

        string SelectionNotAvaTitle, SelectionNotAvaContent;

        II18NConvertor convertor;

        public MusicSelected()
        {
            InitializeComponent();
            musicListbox.OnMainEffectInvoked += MusicListbox_ItemSelectionChanged;
            convertor = TranslationManagerBase.AquireConverter();
            controllerManager = SequenceControllerManager.Instance;
            TranslationManagerBase.OnI18NEnvironmentChanged += Translate;
            Delegation.AddToNewCatalogue += MusicAddedRequest;
            Delegation.EditMetadata += EditMetadataRequest;
            Translate(convertor);
            //musicListbox.OnListStatusChanged += MusicListbox_OnListStatusChanged;
        }

        private void MusicAddedRequest(object obj)
        {
            List<MusicEntity> selected = musicListbox.AllSelectedItems;
            MusicEntity musicEntity = obj as MusicEntity;
            if (musicEntity != null)
            {
                if (!selected.Exists(x => x.MusicID.Equals(musicEntity.MusicID)))
                {
                    selected.Add(musicEntity);
                }
            }
            else if (selected.Count <= 0)
            {
                return;
            }
            ChooseCatalogues chooseCataloguesPage = new ChooseCatalogues(selected);
            UniversalDailogue ShowPage = new UniversalDailogue(chooseCataloguesPage,
                AddToCatalogueTitle.FormateEx(selected.Count), MessageBoxButton.OK);
            if(ShowPage.ShowDialog().Value)
            {
                musicListbox.DeselectAll();
            }
        }

        private void EditMetadataRequest(MusicEntity musicEntity)
        {
            //EntityEditDialoguePage entityEditDialoguePage = new EntityEditDialoguePage(musicEntity);
            //UniversalDailogue MetadataEditor = new UniversalDailogue(entityEditDialoguePage, MetadataEditorTitle, MessageBoxButton.OKCancel);
            //MetadataEditor.ShowDialog();
            MusicInfoEditor musicInfoEditor = new MusicInfoEditor(musicEntity);
            musicInfoEditor.ShowDialog();
        }

        //private void MusicListbox_OnListStatusChanged(GeneratorStatus status)
        //{
        //    if(status == GeneratorStatus.ContainersGenerated && isRecovery)
        //    {
        //        musicListbox.SelectedIndex = SelectedCatalogue.CurrentIndex;

        //    }
        //    isRecovery = false;
        //}

        private void MusicListbox_ItemSelectionChanged(MusicEntity selected, object tag = null)
        {
            if (!controllerManager.CurrentController.FullyTakeover)
            {
                OnSelectedMusicChange?.Invoke(selected, tag);
            }
            else
            {
                (new CommonDialog(SelectionNotAvaTitle, SelectionNotAvaContent, MessageBoxButton.OK)).ShowDialog();
            }
        }

        public void SetCatalogue(Catalogue cata, bool recovered = false)
        {
            if(SelectedCatalogue!=cata)
            {
                SelectedCatalogue = cata;
                musicListbox.Catalogue = cata;
                isRecovery = recovered;
                musicListbox.TranslateList(convertor);
            }
            //musicListbox.SelectedIndex = cata.CurrentIndex;
        }

        public void Translate(II18NConvertor i8c)
        {
            musicListbox.Translate(i8c);
            AddToCatalogueTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_ADDPLAYLIST_TITLE");
            MetadataEditorTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_MUSICENTITY_EDITOR_TITLE");
            SelectionNotAvaTitle = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_MSELECTION_NOVAV_TITLE");
            SelectionNotAvaContent = i8c.ConvertTo(SupportedPages.CORE_FUNC, "CORE_MSELECTION_NOVAV_CONTENT");
        }

        public int PlayingIndex
            { get => musicListbox.SelectedIndex; set => musicListbox.SelectedIndex = value; }
    }
}
