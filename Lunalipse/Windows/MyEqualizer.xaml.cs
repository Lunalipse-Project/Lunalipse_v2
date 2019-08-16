using CSCore.Streams.Effects;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Core.LpsAudio;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;
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
using System.Windows.Shapes;

namespace Lunalipse.Windows
{
    /// <summary>
    /// Interaction logic for MyEqualizer.xaml
    /// </summary>
    public partial class MyEqualizer : LunalipseDialogue
    {
        LpsAudio lpsAudio;
        bool isValueRestored = false;
        public MyEqualizer()
        {
            InitializeComponent();
            lpsAudio = LpsAudio.Instance();
            Closing += MyEqualizer_Closing;
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());

        }

        private void MyEqualizer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GLS gLS = GLS.INSTANCE;
            for(int i=0;i<10;i++)
            {
                gLS.EqualizerSets[i] = lpsAudio.LpsEqualizer.SampleFilters[i].AverageGainDB;
            }
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(II18NConvertor obj)
        {
            Title = obj.ConvertTo(SupportedPages.CORE_FUNC, "CORE_EQUALIZER_TITLE");
        }

        protected override void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            base.ThemeManagerBase_OnThemeApplying(obj);
            Background = obj.Primary.SetOpacity(1).ToLuna();
            equalizer.SetDragBarTheme(obj.Primary.ToLuna(), obj.Secondary);
        }

        private void Equalizer_OnEqualizerValueChanged(int index, double value)
        {
            if(isValueRestored)
            {
                lpsAudio.SetEqualizerIndex(index - 1, value - 12d);
            }
        }

        private void LunalipseDialogue_Loaded(object sender, RoutedEventArgs e)
        {
            equalizer.ApplyEqualizerValue(GLS.INSTANCE.EqualizerSets);
            isValueRestored = true;
        }
    }
}
