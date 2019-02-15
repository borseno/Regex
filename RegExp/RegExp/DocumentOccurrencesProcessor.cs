using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using Data_Structures;

namespace RegExp
{
    class DocumentOccurrencesProcessor
    {
        private List<TextRange> _textRanges;
        private Regex _currentRegex;

        public FlowDocument FlowDocument { get; }
 
        public DocumentOccurrencesProcessor(FlowDocument flowDocument)
        {
            FlowDocument = flowDocument;
        }

        public IEnumerable<TextRange> GetOccurrencesRanges(Regex regex)
        {
            if (regex != null)
            {
                List<TextRange> result = new List<TextRange>();

                _textRanges = new List<TextRange>(32);
                _currentRegex = regex;

                if (CheckIfMatchesWholeString())
                {
                    _textRanges.Add(new TextRange(FlowDocument.ContentStart, FlowDocument.ContentEnd));
                    return _textRanges;
                }

                TextPointer pointer = FlowDocument.ContentStart;

                while (pointer != null)
                {
                    if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                        MatchCollection matches = regex.Matches(textRun);
                        foreach (Match match in matches)
                        {
                            int startIndex = match.Index;
                            int length = match.Length;
                            TextPointer start = pointer.GetPositionAtOffset(startIndex);
                            TextPointer end = start?.GetPositionAtOffset(length);

                            TextRange textRange = new TextRange(start, end);

                            _textRanges.Add(textRange);
                        }
                    }

                    pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
                }

                return _textRanges;
            }

            return null;
        }

        // TODO: Split into multiple tasks.
        public void Highlight(params Brush[] brushes)
        {
            if (_textRanges?.Count > 0)
            {
                var textRangesArray = _textRanges.ToArray();

                CircularArray<Brush> backgroundValues = new CircularArray<Brush>(brushes);
                int index = 0;

                foreach (TextRange i in textRangesArray)
                {
                    string regExpression = _currentRegex.ToString();

                    regExpression = !regExpression.StartsWith("^") ? '^' + regExpression : regExpression;
                    regExpression = !regExpression.EndsWith("$") ? regExpression + '$' : regExpression;

                    if (Regex.IsMatch(i.Text.RemoveAll('\r', '\n'), regExpression))
                    {
                        i.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundValues[index++]);
                        i.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Azure);
                    }
                }
            }
        }

        private bool CheckIfMatchesWholeString()
        {
            string regexValue = _currentRegex.ToString();
            string text = new TextRange(FlowDocument.ContentStart, FlowDocument.ContentEnd).Text;

            return regexValue.EndsWith("$") &&
                regexValue.StartsWith("^") &&
                regexValue.Trim('^', '$') == text.RemoveAll('\r', '\n') ||
                regexValue == text.RemoveAll('\r', '\n');
        }
    }
}
