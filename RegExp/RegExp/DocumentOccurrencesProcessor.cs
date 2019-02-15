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
        private readonly DocumentOccurrencesFinder _finder;
        private readonly DocumentOccurrencesHighlighter _highlighter;
        private IEnumerable<TextRange> _textRanges;
        private Regex _currentRegex;

        public FlowDocument FlowDocument { get; }
 
        public DocumentOccurrencesProcessor(FlowDocument flowDocument, Brush defaultBackGround, Brush defaultForeGround)
        {
            _finder = new DocumentOccurrencesFinder();
            _highlighter = new DocumentOccurrencesHighlighter(flowDocument, defaultBackGround, defaultForeGround);
            FlowDocument = flowDocument;
        }

        public IEnumerable<TextRange> GetOccurrencesRanges(Regex regex)
        {
            _currentRegex = regex;
            return _textRanges = _finder.GetOccurrencesRanges(FlowDocument, regex);
        }

        public void Highlight(Brush foreGround, params Brush[] brushes)
        {
            _highlighter.Highlight(_textRanges, _currentRegex, foreGround, brushes);
        }

        public void ResetTextProperties()
        {
            _highlighter.ResetTextProperties();
        }
    }
}
