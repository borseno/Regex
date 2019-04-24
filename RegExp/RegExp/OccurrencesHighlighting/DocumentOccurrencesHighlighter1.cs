using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;

namespace RegExp.OccurrencesHighlighting
{
    class DocumentOccurrencesHighlighter1 : DocumentOccurrencesHighlighter
    {
        public DocumentOccurrencesHighlighter1(FlowDocument document, 
            Brush defaultBackGround, Brush defaultForeGround, 
            IEnumerable<Brush> backgroundBrushes, IEnumerable<Brush> foregroundBrushes
            ) : base(document, defaultBackGround, defaultForeGround, backgroundBrushes, foregroundBrushes)
        {
        }

        public void ResetTextProperties(TextRange range)
        {
            if (range != null)
            {
                range.ApplyPropertyValue(TextElement.BackgroundProperty, DefaultBackground);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, DefaultForeground);
            }
        }
    }
}
