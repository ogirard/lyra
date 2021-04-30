using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Lyra.Features.Styles;

namespace Lyra.UI
{
    /// <summary>
    /// Interaction logic for SongDocument.xaml
    /// </summary>
    public partial class PreviewSongDocument : UserControl
    {
        private static readonly PresentationStyle PreviewStyle = new PresentationStyle
        {
            Id = "PresentationStyle/__preview",
            Body = new()
            {
                BackgroundColor = "#FFFFFFFF",
                ForegroundColor = "#FF000000",
                UseBackgroundImage = false,
                FontSize = 15,
                NormalFont = "Roboto",
                SpecialFont = "Roboto Slab",
                MutedFont = "Roboto",
            },
            Title = new()
            {
                BackgroundColor = "#FFFFFFFF",
                ForegroundColor = "#FF000000",
                FontSize = 15,
                Mode = TitleStyle.TitleMode.ShowNumberAndTitle,
                TitleFont = "Roboto",
            },
        };

        private static readonly PresentationStyleViewModel PreviewStyleViewModel = new(PreviewStyle);

        public PreviewSongDocument()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(nameof(Document), typeof(FlowDocument), typeof(PreviewSongDocument));

        public FlowDocument Document
        {
            get => (FlowDocument)GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        public static readonly DependencyProperty SongProperty = DependencyProperty.Register(nameof(Song), typeof(SongViewModel), typeof(PreviewSongDocument), new FrameworkPropertyMetadata(null, OnSongPropertyChanged));

        private static void OnSongPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var songDocument = (PreviewSongDocument)d;
            if (!(e.NewValue is SongViewModel song))
            {
                songDocument.Document = null;
                return;
            }

            songDocument.Document = new SongFlowDocument(song, PreviewStyleViewModel)
                    .ActivateColumnOverflow((songDocument.ActualWidth - 100) / 2);
        }

        public SongViewModel Song
        {
            get => (SongViewModel)GetValue(SongProperty);
            set => SetValue(SongProperty, value);
        }
    }
}
