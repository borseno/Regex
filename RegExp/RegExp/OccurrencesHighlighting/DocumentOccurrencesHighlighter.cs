using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using Data_Structures;
using RegExp.Extensions;

namespace RegExp.OccurrencesHighlighting
{
    class DocumentOccurrencesHighlighter
    {
        protected FlowDocument FlowDocument { get; }
        protected int Index { get; set; }

        public Brush DefaultBackground { get; set; }
        public Brush DefaultForeground { get; set; }

        public CircularArray<Brush> BackgroundBrushes { get; }
        public CircularArray<Brush> ForegroundBrushes { get; }

        public DocumentOccurrencesHighlighter(
            FlowDocument document, Brush defaultBackGround, Brush defaultForeGround, 
            IEnumerable<Brush> backgroundBrushes, IEnumerable<Brush> foregroundBrushes)
        {
            FlowDocument = document;
            DefaultBackground = defaultBackGround;
            DefaultForeground = defaultForeGround;

            BackgroundBrushes = new CircularArray<Brush>(backgroundBrushes.ToArray());
            ForegroundBrushes = new CircularArray<Brush>(foregroundBrushes.ToArray());
        }

        public void Highlight(IEnumerable<TextRange> textRanges, Regex regex, bool continueWithPreviousColors = false)
        {
            var textRangesArray = textRanges.ToArray();

            if (textRangesArray.Length > 0)
            {
                if (!continueWithPreviousColors)
                    Index = 0;

                foreach (TextRange i in textRangesArray)
                {
                    string regExpression = regex.ToString();

                    regExpression = !regExpression.StartsWith("^") ? '^' + regExpression : regExpression;
                    regExpression = !regExpression.EndsWith("$") ? regExpression + '$' : regExpression;

                    if (Regex.IsMatch(i.Text.RemoveAll('\r', '\n'), regExpression))
                    {
                        i.ApplyPropertyValue(TextElement.BackgroundProperty, BackgroundBrushes[Index]);
                        i.ApplyPropertyValue(TextElement.ForegroundProperty, ForegroundBrushes[Index]);

                        Index++;
                    }
                }
            }
        }

        public void ResetTextProperties()
        {
            TextRange textRange = new TextRange(
                FlowDocument.ContentStart,
                FlowDocument.ContentEnd
            );

            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, DefaultBackground);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, DefaultForeground);
        }
    }
}
