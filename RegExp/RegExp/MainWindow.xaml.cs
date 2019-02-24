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
using System.Windows.Controls.Primitives;
using Data_Structures;

namespace RegExp
{
    public partial class MainWindow : Window
    {
        private readonly DocumentOccurrencesFinder _occurrencesFinder;
        private readonly DocumentOccurrencesHighlighter _occurrencesHighlighter;
        private readonly RegexTextProcessor1 _regexProcessor;
        private bool _isBeingChanged;
        private Match[] _previous;
        private Regex _currentRegex;
        private TextPointer _previousCaretPosition; // used if input is in the end to get the latest textrange

        private bool ResetRequired
        {
            get
            {
                if (_previousCaretPosition == null || InputString.CaretPosition == null)
                    return false;

                return !new TextRange(_previousCaretPosition, InputString.CaretPosition)
                    .GetPropertyValue(TextBlock.BackgroundProperty)?.Equals(Brushes.White) ?? false;
            }
        } // displays whether or not reset is needed even if matches are the same

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
            {
                try
                {
                    _currentRegex = new Regex(RegExpValue);
                }
                catch (ArgumentException)
                {
                    _regexProcessor.AddCurvyUnderline();
                    return;
                }
                _regexProcessor.ResetRegexProperties();

                var current = _currentRegex.Matches(Text).Cast<Match>().ToArray();

                if (ResetRequired)
                    ResetValues();
                else if (!MatchesComparer.Equals(current, _previous))
                    ResetValues(); // todo: change to UpdateValues()
                else
                    ResetLatestInputProperties();

                _previous = current;
            }
        }

        #region processing
        private void UpdateValues()
        {
            _isBeingChanged = true;

            var foundRanges = _occurrencesFinder.GetOccurrencesRanges(_currentRegex);
            _occurrencesHighlighter
                .Highlight(
                foundRanges,
                _currentRegex,
                Brushes.Azure,
                Brushes.Black, Brushes.DimGray, Brushes.Gray, Brushes.LightGray
                );

            _isBeingChanged = false;
        }
        private void ResetValues()
        {
            _isBeingChanged = true;

            _occurrencesHighlighter.ResetTextProperties();

            var foundRanges = _occurrencesFinder.GetOccurrencesRanges(_currentRegex);
            _occurrencesHighlighter
                .Highlight(
                    foundRanges,
                    _currentRegex,
                    Brushes.Azure,
                    Brushes.Black, Brushes.DimGray, Brushes.Gray, Brushes.LightGray
                );

            _isBeingChanged = false;
        }
        #endregion

        private void ResetLatestInputProperties()
        {
            // TODO: 
            // Reset the latest input's back and foreground properties 
            _isBeingChanged = true;

            TextRange latest = new TextRange(_previousCaretPosition, InputString.CaretPosition);

            var defaultBack = Brushes.White;
            var defaultFore = Brushes.Black;

            latest.ApplyPropertyValue(TextElement.BackgroundProperty, defaultBack);
            latest.ApplyPropertyValue(TextElement.ForegroundProperty, defaultFore);

            _isBeingChanged = false;
        }

        private void InputString_KeyDown(object sender, KeyEventArgs e)
        {
            TextRange textRange = new TextRange(InputString.CaretPosition, InputString.Document.ContentEnd);
           
            _previousCaretPosition = InputString.CaretPosition;

        }
    }
}
