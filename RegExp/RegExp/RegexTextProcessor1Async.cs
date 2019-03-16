using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RegExp
{
    class RegexTextProcessor1Async : RegexTextProcessor1
    {
        public RegexTextProcessor1Async(TextBox textBox, Color color) : base(textBox, color)
        {
        }

        public async Task ResetRegexPropertiesAsync()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(
                    () => { ResetRegexProperties(); });
            });
        }

        public async Task AddCurvyUnderlineAsync()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(
                    () => { AddCurvyUnderline(); });
            });
        }
    }
}
