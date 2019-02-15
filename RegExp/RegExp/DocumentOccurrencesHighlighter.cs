using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using Data_Structures;

namespace RegExp
{
    class DocumentOccurrencesHighlighter
    {
        private readonly FlowDocument _flowDocument;
        private readonly Brush _defaultBackGround;
        private readonly Brush _defaultForeGround;

        public DocumentOccurrencesHighlighter(FlowDocument document, Brush defaultBackGround, Brush defaultForeGround)
        {
            _flowDocument = document;
            _defaultBackGround = defaultBackGround;
            _defaultForeGround = defaultForeGround;
        }

        // TODO: Split into multiple tasks.
        public void Highlight(IEnumerable<TextRange> textRanges, Regex regex, Brush foreGround, params Brush[] brushes)
        {
            if (textRanges?.Count() > 0)
            {
                var textRangesArray = textRanges.ToArray();

                CircularArray<Brush> backgroundValues = new CircularArray<Brush>(brushes);
                int index = 0;

                foreach (TextRange i in textRangesArray)
                {
                    string regExpression = regex.ToString();

                    regExpression = !regExpression.StartsWith("^") ? '^' + regExpression : regExpression;
                    regExpression = !regExpression.EndsWith("$") ? regExpression + '$' : regExpression;

                    if (Regex.IsMatch(i.Text.RemoveAll('\r', '\n'), regExpression))
                    {
                        i.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundValues[index++]);
                        i.ApplyPropertyValue(TextElement.ForegroundProperty, foreGround);
                    }
                }
            }
        }

        public void ResetTextProperties()
        {
            TextRange textRange = new TextRange(
                _flowDocument.ContentStart,
                _flowDocument.ContentEnd
            );

            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, _defaultBackGround);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, _defaultForeGround);
        }
    }
}
