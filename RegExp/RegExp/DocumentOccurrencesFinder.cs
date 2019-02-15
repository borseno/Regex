using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace RegExp
{
    class DocumentOccurrencesFinder
    {
        public IEnumerable<TextRange> GetOccurrencesRanges(FlowDocument flowDocument, Regex regex)
        {
            if (regex != null)
            {
                List<TextRange> result = new List<TextRange>();

                {
                    string text = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd).Text;
                    string pattern = regex.ToString();

                    if (CheckIfMatchesWholeString(text: text, pattern: pattern))
                    {
                        result.Add(new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd));
                        return result;
                    }

                }

                TextPointer pointer = flowDocument.ContentStart;

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

                            result.Add(textRange);
                        }
                    }
      
                    pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
                }

                return result;
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
