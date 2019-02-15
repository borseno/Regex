using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace RegExp
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
