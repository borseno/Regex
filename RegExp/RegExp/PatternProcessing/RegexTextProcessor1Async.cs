using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RegExp.PatternProcessing
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
