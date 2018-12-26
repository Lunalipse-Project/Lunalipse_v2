using Lunalipse.Common.Data;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.PlayList;
using Lunalipse.I18N;
using System;
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

        II18NConvertor convertor;

        public MusicSelected()
        {
            InitializeComponent();
            musicListbox.ItemSelectionChanged += MusicListbox_ItemSelectionChanged;
            convertor = TranslationManager.AquireConverter();
            TranslationManager.OnI18NEnvironmentChanged += Translate;
            //musicListbox.OnListStatusChanged += MusicListbox_OnListStatusChanged;
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
            OnSelectedMusicChange?.Invoke(selected, tag);
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
        }

        public int PlayingIndex
            { get => musicListbox.SelectedIndex; set => musicListbox.SelectedIndex = value; }
    }
}
