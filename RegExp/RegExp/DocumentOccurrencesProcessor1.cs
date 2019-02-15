using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace RegExp
{
    class DocumentOccurrencesProcessor1 : DocumentOccurrencesProcessor
    {
        private readonly Brush _defaultBackGround;
        private readonly Brush _defaultForeGround;

        public DocumentOccurrencesProcessor1(FlowDocument flowDocument, Brush defaultBackGround, Brush defaultForeGround) : base(flowDocument)
        {
            _defaultBackGround = defaultBackGround;
            _defaultForeGround = defaultForeGround;
        }

        public void ResetTextProperties()
        {
            TextRange textRange = new TextRange(
                FlowDocument.ContentStart,
                FlowDocument.ContentEnd
            );

            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, _defaultBackGround);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, _defaultForeGround);
        }
    }
}
