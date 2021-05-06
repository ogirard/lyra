using System.Windows;
using System.Windows.Media;
using Lyra.Features.Styles;
using ReactiveUI;

namespace Lyra.UI
{
    public class PresentationStyleViewModel : ReactiveObject
    {
        private readonly PresentationStyle style;

        public string Id => style.Id;

        public SolidColorBrush BackgroundColor { get; }

        public SolidColorBrush ForegroundColor { get; }

        public double FontSize { get; }

        public FontFamily NormalFontFamily { get; }

        public FontFamily SpecialFontFamily { get; }

        public FontFamily MutedFontFamily { get; }

        public SolidColorBrush TitleBackgroundColor { get; }

        public SolidColorBrush TitleForegroundColor { get; }

        public double TitleFontSize { get; }

        public FontFamily TitleFontFamily { get; }

        public Visibility TitleNumberVisibility { get; }

        public Visibility TitleVisibility { get; }

        public PresentationStyleViewModel(PresentationStyle style)
        {
            this.style = style;
            BackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(style.Body.BackgroundColor));
            ForegroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(style.Body.ForegroundColor));
            FontSize = style.Body.FontSize;
            NormalFontFamily = new FontFamily(style.Body.NormalFont);
            SpecialFontFamily = new FontFamily(style.Body.SpecialFont);
            MutedFontFamily = new FontFamily(style.Body.MutedFont);
            TitleBackgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(style.Title.BackgroundColor));
            TitleForegroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(style.Title.ForegroundColor));
            TitleFontSize = style.Title.FontSize;
            TitleFontFamily = new FontFamily(style.Title.TitleFont);
            TitleNumberVisibility = style.Title.Mode == TitleStyle.TitleMode.ShowNumberAndTitle ? Visibility.Visible : Visibility.Collapsed;
            TitleVisibility = style.Title.Mode == TitleStyle.TitleMode.Hide ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}