using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Lunalipse.Common.Data;
using Lunalipse.Common.Generic.I18N;
using Lunalipse.Common.Generic.Themes;
using Lunalipse.Common.Interfaces.II18N;
using Lunalipse.Common.Interfaces.ILpsUI;
using Lunalipse.Presentation.BasicUI;
using Lunalipse.Utilities;

namespace Lunalipse.Presentation.LpsWindow
{
    /// <summary>
    /// Dialogue.xaml 的交互逻辑
    /// </summary>
    public partial class UniversalDailogue : LunalipseDialogue
    {
        string PositiveBtnI18N, NegativeBtnI18N;
        Page pageCotent;
        public UniversalDailogue(string title)
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Title = title;
            Loaded += UniversalDailogue_Loaded;
            TranslationManagerBase.OnI18NEnvironmentChanged += TranslationManagerBase_OnI18NEnvironmentChanged;
            ThemeManagerBase.OnThemeApplying += ThemeManagerBase_OnThemeApplying;
            Display.ContentRendered += Display_ContentRendered;
        }

        private void Display_ContentRendered(object sender, EventArgs e)
        {
            TranslationManagerBase_OnI18NEnvironmentChanged(TranslationManagerBase.AquireConverter());
        }

        private void ThemeManagerBase_OnThemeApplying(ThemeTuple obj)
        {
            if (obj == null) return;
            SolidColorBrush primary = obj.Primary.SetOpacity(1).ToLuna();
            Foreground = obj.Foreground;
            Background = primary.ToCelestia();
            Negative.Background = primary;
            Positive.Background = primary;
            (pageCotent as IDialogPage).UnifiedTheme(obj);
        }

        private void TranslationManagerBase_OnI18NEnvironmentChanged(II18NConvertor obj)
        {
            Positive.Content = obj.ConvertTo(SupportedPages.CORE_FUNC, PositiveBtnI18N);
            Negative.Content = obj.ConvertTo(SupportedPages.CORE_FUNC, NegativeBtnI18N);
            (pageCotent as ITranslatable).Translate(obj);
        }

        private void UniversalDailogue_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeManagerBase_OnThemeApplying(ThemeManagerBase.AcquireSelectedTheme());
            Positive.Click += (a, b) =>
            {
                if (!(pageCotent as IDialogPage).PositiveClicked())
                    return;
                DialogResult = true;
            };
            Negative.Click += (a, b) =>
            {
                if ((pageCotent as IDialogPage).NegativeClicked())
                    return;
                DialogResult = false;
            };
        }

        public UniversalDailogue(IDialogPage content, string title) : this(title)
        {
            pageCotent = content as Page;
            if (pageCotent == null)
            {
                throw new ApplicationException("非法的界面，请检查是否有实现IDialogPage接口。");
            }
            Width = pageCotent.Width + WidthBias;
            Height = pageCotent.Height + HeightBias;
            Display.Content = content;
            
        }

        public UniversalDailogue(IDialogPage content, string title, MessageBoxButton Buttons) : this(content, title)
        {
            switch (Buttons)
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
            Height += 45;
        }

        public Page RenderContent
        {
            get => Display.Content as Page;
            set => Display.Content = value;
        }
    }
}
