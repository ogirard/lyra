using System;
using System.Windows;
using System.Windows.Media;
using SourceChord.FluentWPF;

namespace Lyra.Framework.MVVM
{
    public class LyraWindow : AcrylicWindow
    {
        public LyraWindow()
        {
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(@"/Lyra;component/Styles.xaml", UriKind.Relative) });
            FontFamily = (FontFamily)FindResource("PrimaryFont");
            FontSize = 14;
        }
    }
}
