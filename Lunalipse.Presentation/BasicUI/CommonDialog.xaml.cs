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
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Presentation.LpsWindow;
using Lunalipse.Utilities;

namespace Lunalipse.Presentation.BasicUI
{
    /// <summary>
    /// CommonDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CommonDialog : LunalipseDialogue
    {
        string PositiveBtnI18N, NegativeBtnI18N;
        string title, content;

        public string DialogTitle
        {
            get => title;
            set
            {
                title = value;
                Title = title;
            }
        }

        public string DialogContent
        {
            get => content;
            set
            {
                content = value;
                DetialContent.Text = content;
            }
        }

        public CommonDialog()
        {
            InitializeComponent();
            Negative.Click += Negative_Click;
            Positive.Click += Positive_Click;
            Loaded += CommonDialog_Loaded;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            Unloaded += CommonDialog_Unloaded;
        }

        private void CommonDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            ThemeManagerBase.OnThemeApplying -= ThemeManagerBase_OnThemeApplying;
            TranslationManagerBase.OnI18NEnvironmentChanged -= TranslationManagerBase_OnI18NEnvironmentChanged;

        }

        private void CommonDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Height = DetialContent.ActualHeight + HeightBias + 45;
            this.HideWindowFromAltTab();
            ShowInTaskbar = false;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
        }

        private void Positive_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Negative_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public CommonDialog(string Caption, string Message, MessageBoxButton Buttons) : this()
        {
            DialogTitle = Caption;
            DialogContent = Message;
            switch(Buttons)
            {
                case MessageBoxButton.OK:
                    PositiveBtnI18N = "CORE_DIALOG_OK";
                    NegativeBtnI18N = "";
                    Negative.Visibility = Visibility.Hidden;
                    break;
                case MessageBoxButton.OKCancel:
                    PositiveBtnI18N = "CORE_DIALOG_OK";
                    NegativeBtnI18N = "CORE_DIALOG_CANCEL";
                    break;
                case MessageBoxButton.YesNo:
                case MessageBoxButton.YesNoCancel:
                    PositiveBtnI18N = "CORE_DIALOG_YES";
                    NegativeBtnI18N = "CORE_DIALOG_NO";
                    break;

            }
        }

        public CommonDialog(string Caption, string Message, MessageBoxButton Buttons, string PositiveContent, string NegativeContent)
            : this(Caption, Message, Buttons)
        {
            PositiveBtnI18N = PositiveContent;
            NegativeBtnI18N = NegativeContent;
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(II18NConvertor obj)
        {
            Positive.Content = obj.ConvertTo(SupportedPages.CORE_FUNC, PositiveBtnI18N);
            Negative.Content = obj.ConvertTo(SupportedPages.CORE_FUNC, NegativeBtnI18N);
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            Brush secondary = obj.Secondary.ToLuna();
            Foreground = obj.Foreground;
            Background = obj.Primary;
            Negative.Background = secondary;
            Positive.Background = secondary;
        }
    }
}
