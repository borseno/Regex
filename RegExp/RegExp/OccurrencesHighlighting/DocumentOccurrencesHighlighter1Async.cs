using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using RegExp.Extensions;

namespace RegExp.OccurrencesHighlighting
{
    class DocumentOccurrencesHighlighter1Async : DocumentOccurrencesHighlighter1
    {
        public DocumentOccurrencesHighlighter1Async(FlowDocument document, 
            Brush defaultBackGround, Brush defaultForeGround, 
            IEnumerable<Brush> backgroundBrushes, IEnumerable<Brush> foregroundBrushes) 
            : base(document, defaultBackGround, defaultForeGround, backgroundBrushes, foregroundBrushes)
        {
        }

        public async Task ResetTextPropertiesAsync(TextRange range)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(
                    () => { ResetTextProperties(range); });
            });
        }

        public async Task ResetTextPropertiesAsync()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(
                    ResetTextProperties);
            });
        }

        public async Task HighlightAsync(IEnumerable<TextRange> textRanges, Regex regex,
            bool continueWithPreviousColors = false)
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
                        await Task.Run(() =>
                        {
                            Application.Current.Dispatcher.Invoke(
                                () =>
                                {
                                    i.ApplyPropertyValue(TextElement.BackgroundProperty, BackgroundBrushes[Index]);
                                    i.ApplyPropertyValue(TextElement.ForegroundProperty, ForegroundBrushes[Index]);
                                }
                            );
                        });

                        Index++;
                    }
                }
            }
        }
    }
}
