using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Data_Structures;

namespace RegExp
{
    public partial class MainWindow : Window
    {
        private readonly DocumentOccurrencesFinder _occurrencesFinder;
        private readonly DocumentOccurrencesHighlighter _occurrencesHighlighter;

        private readonly RegexTextProcessor1 _regexProcessor;
        private bool _isBeingChanged;

        private string RegExpValue => InputRegExp.Text;

        private string Text => new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd).Text;

        public MainWindow()
        {
            InitializeComponent();
 
            _occurrencesFinder = new DocumentOccurrencesFinder(InputString.Document);

            {
                Brush defaultBack = Brushes.White;
                Brush defaultFore = Brushes.Black;
                _occurrencesHighlighter = 
                    new DocumentOccurrencesHighlighter(
                        InputString.Document, 
                        defaultBack, defaultFore
                        );
            }

            _regexProcessor = new RegexTextProcessor1(InputRegExp, Colors.Red);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isBeingChanged)
                UpdateValues();
        }

        #region processing
        private void UpdateValues()
        {
            _isBeingChanged = true;

            _occurrencesHighlighter.ResetTextProperties();
            _regexProcessor.ResetRegexProperties();

            if (String.IsNullOrEmpty(RegExpValue))
                return;

            Regex regex = null;
            try
            {
                regex = new Regex(RegExpValue);
            }
            catch (ArgumentException)
            {
                _regexProcessor.AddCurvyUnderline();
                return;
            }

            var foundRanges = _occurrencesFinder.GetOccurrencesRanges(regex);
            _occurrencesHighlighter
                .Highlight(
                foundRanges, 
                regex, 
                Brushes.Azure,
                Brushes.Black, Brushes.DimGray, Brushes.Gray, Brushes.LightGray
                );

            _isBeingChanged = false;
        }
        #endregion
    }
}
