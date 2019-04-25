using System.Windows.Controls;
using System.Windows.Media;

namespace RegExp.PatternProcessing
{
    class RegexTextProcessor1 : RegexTextProcessor
    {
        public RegexTextProcessor1(TextBox textBox, Color color) : base(textBox, color)
        {
        }

        public void ResetRegexProperties()
        {
            TextBox.TextDecorations.Clear();
        }
    }
}
