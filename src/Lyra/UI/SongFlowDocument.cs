using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace Lyra.UI
{
    public class SongFlowDocument : FlowDocument
    {
        private const string TopJumpmarkName = "top";
        private const string JumpmarkStyle = nameof(JumpmarkStyle);
        private const string BoldStyle = nameof(BoldStyle);
        private const string ItalicStyle = nameof(ItalicStyle);
        private const string RefrainStyle = nameof(RefrainStyle);
        private const string SpecialStyle = nameof(SpecialStyle);

        private static readonly Regex JumpmarkRegex = new(
            "<jumpmark.*name=\"(?<name>.*)\".*/>",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public SongFlowDocument(SongViewModel song, PresentationStyleViewModel style)
        {
            FontFamily = style.NormalFontFamily;
            Foreground = style.ForegroundColor;
            FontSize = style.FontSize;
            Background = Brushes.Transparent;

            var jumpmarkStyle = new Style { TargetType = typeof(Span) };
            jumpmarkStyle.Setters.Add(new Setter(BackgroundProperty, Brushes.LightGray));
            jumpmarkStyle.Setters.Add(new Setter(ForegroundProperty, Brushes.DimGray));
            Resources.Add(JumpmarkStyle, jumpmarkStyle);

            var boldStyle = new Style { TargetType = typeof(Span) };
            boldStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.SemiBold));
            Resources.Add(BoldStyle, boldStyle);

            var italicStyle = new Style { TargetType = typeof(Span) };
            italicStyle.Setters.Add(new Setter(FontStyleProperty, FontStyles.Italic));
            Resources.Add(ItalicStyle, italicStyle);

            var refrainStyle = new Style { TargetType = typeof(Span) };
            refrainStyle.Setters.Add(new Setter(FontWeightProperty, FontWeights.SemiBold));
            Resources.Add(RefrainStyle, refrainStyle);

            var specialStyle = new Style { TargetType = typeof(Span) };
            specialStyle.Setters.Add(new Setter(FontFamilyProperty, song.PresentationStyle.SpecialFontFamily));
            Resources.Add(SpecialStyle, specialStyle);

            var xamlText = ToXamlString(song.Text);
            var songTextBlock = (Section)XamlReader.Parse(xamlText);
            Blocks.Add(songTextBlock);
        }

        private string ToXamlString(string songText)
        {
            var paragraphs = GetJumparkParagraphs(songText);
            var documentXaml = $@"<Section xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
{string.Join(Environment.NewLine, paragraphs.Select(ReplaceAllTags))}
</Section>";
            return documentXaml;
        }

        private static IReadOnlyList<string> GetJumparkParagraphs(string songText)
        {
            var jumpmarkCount = 1;
            if (!songText.StartsWith("<jumpmark", StringComparison.InvariantCultureIgnoreCase))
            {
                songText = $"<jumpmark name=\"{TopJumpmarkName}\" />" + songText;
            }

            var paragraphs = new List<string>();
            var jumpmarks = JumpmarkRegex.Matches(songText);
            var position = jumpmarks[0].Length;
            var name = jumpmarks[0].Groups["name"].Value;
            foreach (var jumpmark in jumpmarks.Skip(1))
            {
                var paragraphText = songText.Substring(position, jumpmark.Index - position);
                AddParagraph(name, paragraphText);

                position = jumpmark.Index + jumpmark.Length;
                name = jumpmark.Groups["name"].Value;
            }

            var finalParagraphText = songText.Substring(position);
            AddParagraph(name, finalParagraphText);

            void AddParagraph(string jumpmarkName, string text)
            {
                var jumpmark = jumpmarkName != TopJumpmarkName ? $"<Span Style=\"{{DynamicResource {JumpmarkStyle}}}\">{EscapeText(jumpmarkName)}</Span>" : string.Empty;
                paragraphs.Add($"<Paragraph Name=\"jumpmark{jumpmarkCount++}\" Tag=\"{EscapeText(jumpmarkName)}\">{jumpmark}<LineBreak />{EscapeText(text)}</Paragraph>");
            }

            return paragraphs;
        }

        private static string EscapeText(string name)
            => SecurityElement.Escape(name);

        public SongFlowDocument ActivateColumnOverflow(double columnWidth)
        {
            ColumnGap = 20;
            ColumnRuleBrush = Brushes.DarkGray;
            ColumnRuleWidth = 1;
            ColumnWidth = columnWidth;
            return this;
        }

        private static string ReplaceAllTags(string text)
        {
            text = ReplaceTags(text, "b", $"<Span Style=\"{{DynamicResource {BoldStyle}}}\">", "</Span>");
            text = ReplaceTags(text, "i", $"<Span Style=\"{{DynamicResource {ItalicStyle}}}\">", "</Span>");
            text = ReplaceTags(text, "special", $"<Span Style=\"{{DynamicResource {SpecialStyle}}}\">", "</Span>");
            text = ReplaceTags(text, "refrain", $"<Span Style=\"{{DynamicResource {RefrainStyle}}}\">", "</Span>");
            text = text.Replace(Environment.NewLine, "<LineBreak />");
            return text;
        }

        private static string ReplaceTags(string text, string tagName, string openReplace, string closeReplace)
        {
            text = Regex.Replace(text, $"&lt;\\s*{tagName}\\s*&gt;", openReplace, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            text = Regex.Replace(text, $"&lt;\\s*/\\s*{tagName}\\s*&gt;", closeReplace, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return text;
        }
    }
}