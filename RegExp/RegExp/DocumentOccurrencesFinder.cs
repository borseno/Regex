using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace RegExp
{
    class DocumentOccurrencesFinder
    {
        private List<TextRange> _previousRanges;

        public FlowDocument FlowDocument { get; }

        public DocumentOccurrencesFinder(FlowDocument flowDocument)
        {
            FlowDocument = flowDocument;
        }

        public IEnumerable<TextRange> GetOccurrencesRanges(Regex regex, bool updatePreviousCall = false)
        {
            if (regex != null)
            {
                List<TextRange> result = new List<TextRange>();

                #region returnAllDocumentIfMatchesAllDocument
                {
                    //string text = new TextRange(FlowDocument.ContentStart, FlowDocument.ContentEnd).Text;
                    //string pattern = regex.ToString();

                    //if (CheckIfMatchesWholeString(text: text, pattern: pattern))
                    //{
                    //    result.Add(new TextRange(FlowDocument.ContentStart, FlowDocument.ContentEnd));
                    //    return _previousRanges = result;
                    //}
                }
                #endregion

                TextPointer pointer = null;

                #region initPointer
                {
                    if (updatePreviousCall)
                    {
                        if (_previousRanges != null)
                            pointer = _previousRanges
                                .LastOrDefault()
                                ?.Start
                                .GetPositionAtOffset(1);
                    }

                    if (pointer == null)
                        pointer = FlowDocument.ContentStart;
                }
                #endregion

                while (pointer != null)
                {
                    if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        string textRun = pointer.GetTextInRun(LogicalDirection.Forward);

                        MatchCollection matches = regex.Matches(textRun);
                        
                        for (int i = 0; i < matches.Count; i++)
                        {
                            int startIndex = matches[i].Index;
                            int length = matches[i].Length;

                            TextPointer start = pointer.GetPositionAtOffset(startIndex);
                            TextPointer end = start?.GetPositionAtOffset(length);

                            TextRange textRange = new TextRange(start, end);

                            result.Add(textRange);
                        }
                    }
      
                    pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
                }

                Debug.WriteLine(result.Count);
                return _previousRanges = result;
            }

            return null;
        }
        private bool CheckIfMatchesWholeString(string text, string pattern)
        {
            return pattern.EndsWith("$") &&
                   pattern.StartsWith("^") &&
                   pattern.Trim('^', '$') == text.RemoveAll('\r', '\n') ||
                   pattern == text.RemoveAll('\r', '\n');
        }
    }
}
