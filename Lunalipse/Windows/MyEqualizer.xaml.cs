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
        public MyEqualizer()
        {
            InitializeComponent();
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
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
            float perc = (((float)value - 12f) / 12f);
            LpsAudio.INSTANCE().SetEqualizerIndex(index - 1, perc);
        }
    }
}
