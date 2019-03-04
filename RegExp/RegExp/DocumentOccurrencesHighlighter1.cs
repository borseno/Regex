using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RegExp
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
            range.ApplyPropertyValue(TextElement.BackgroundProperty, DefaultBackground);
            range.ApplyPropertyValue(TextElement.ForegroundProperty, DefaultForeground);

            Debug.WriteLine("ResetTextProperties range text: " + range.Text);
        }
    }
}
