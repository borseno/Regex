using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace RegExp
{
    partial class MainWindow : Window
    {
        private readonly DocumentOccurrencesFinder _occurrencesFinder;
        private readonly DocumentOccurrencesHighlighter1 _occurrencesHighlighter;
        private readonly RegexTextProcessor1 _regexProcessor;
        private bool _isBeingChanged;
        private Match[] _previous;
        private Match[] _current;
        private Regex _currentRegex;
        private bool _latestTextRangePropertiesReset;
        private int _latestOffset;

        private string RegExpValue => InputRegExp.Text;

        private string Text => new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd).Text;

        private TextRange LatestSymbol
        {
            get
            {
                TextPointer behindCurrentCaret = InputString.CaretPosition.GetPositionAtOffset(-1);

                if (behindCurrentCaret != null)
                    return new TextRange(behindCurrentCaret, InputString.CaretPosition);
                return null;
            }
        }

        private int LatestSymbolIndex
        {
            get
            {
                if (LatestSymbol == null)
                    return -1;
                return InputString.Document.ContentStart.GetOffsetToPosition(LatestSymbol?.Start);
            }
        }

        private bool LatestTextRangeResetRequired
        {
            // TODO: Optimize this
            get
            {
                var currentCaret = InputString.CaretPosition;
                var behindCurrentCaret = InputString.CaretPosition.GetPositionAtOffset(-1);

                if (behindCurrentCaret == null)
                    return false;

                var prop =
                    new TextRange(behindCurrentCaret, currentCaret)
                        .GetPropertyValue(TextElement.ForegroundProperty);

                var anotherRTB = new RichTextBox();

                var emptyRange =
                    new TextRange(
                        anotherRTB.Document.ContentStart,
                        anotherRTB.Document.ContentEnd
                        );
                emptyRange.ApplyPropertyValue(TextElement.ForegroundProperty, _occurrencesHighlighter.DefaultForeground);

                var prop1 = emptyRange.GetPropertyValue(TextElement.ForegroundProperty);

                bool result = !Equals(prop, prop1);

                Debug.WriteLine("LatestTextRangeResetRequired result: " + result);

                return result;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            _occurrencesFinder = new DocumentOccurrencesFinder(InputString.Document);

            #region initHighlighter
            {
                Brush defaultBack = Brushes.White;
                Brush defaultFore = Brushes.Black;
                Brush[] foregroundHighlightingBrushes;
                Brush[] backgroundHighlightingBrushes;

                #region init brushes
                {
                    #region init foreground highlighting brushes
                    {
                        byte foreStart = 0;
                        byte foreAdd = 24;
                        int foreAmount = 10;

                        foregroundHighlightingBrushes =
                            BrushesGenerator.GenerateBrushes(
                                foreStart, foreAdd,
                                foreStart, foreAdd,
                                foreStart, foreAdd,
                                foreAmount
                            ).ToArray();
                    }
                    #endregion
                    #region init background highlighting brushes
                    {
                        byte backStart = 255;
                        sbyte backAdd = -24;
                        int backAmount = 10;

                        backgroundHighlightingBrushes =
                            BrushesGenerator
                                .GenerateBrushes(
                                    backStart, backAdd,
                                    backStart, backAdd,
                                    backStart, backAdd,
                                    backAmount
                                ).ToArray();
                    }
                    #endregion
                }
                #endregion

                _occurrencesHighlighter =
                    new DocumentOccurrencesHighlighter1(
                        InputString.Document,
                        defaultBack, defaultFore,
                        foregroundHighlightingBrushes, backgroundHighlightingBrushes
                        );
            }
            #endregion

            _regexProcessor = new RegexTextProcessor1(InputRegExp, Colors.Red);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isBeingChanged)
            {
                _isBeingChanged = true;

                try
                {
                    _currentRegex = new Regex(RegExpValue);
                }
                catch (ArgumentException)
                {
                    _regexProcessor.AddCurvyUnderline();
                    _isBeingChanged = false;
                    return;
                }
                _regexProcessor.ResetRegexProperties();

                _current = _currentRegex.Matches(Text).Cast<Match>().ToArray();

                // TODO: 1. Optimize case when changing the text after already highlighted text.
                // TODO:    Not to reset the whole text, but update.

                bool previousIsCurrent = MatchesComparer.Equals(_current, _previous);
                const int symbolsToRemove = 2;

                if (_latestOffset != -1 && LatestSymbolIndex <= _latestOffset)
                {
                    ResetValues();
                    _latestTextRangePropertiesReset = false;
                }
                else if (_current.LastOrDefault()?.Index + _current.LastOrDefault()?.Length != Text.Length - symbolsToRemove &&
                    previousIsCurrent &&
                    (!_latestTextRangePropertiesReset || LatestTextRangeResetRequired))
                {
                    ResetLatestInputProperties();
                    _latestTextRangePropertiesReset = true;
                }
                else if (_current.ContainsInStart(_previous) && _current.Length > _previous.Length)
                {
                    ResetLatestInputProperties();
                    UpdateValues();
                    _latestTextRangePropertiesReset = false;
                }
                else if (!previousIsCurrent)
                {
                    ResetValues();
                    _latestTextRangePropertiesReset = false;
                }

                _previous = _current;
                _isBeingChanged = false;
            }
        }

        #region processing
        private void UpdateValues()
        {
            var foundRanges = _occurrencesFinder
                .GetOccurrencesRanges(_currentRegex, updatePreviousCall: true)
                .ToArray();

            if (foundRanges.Length > 0)
            {
            _occurrencesHighlighter.Highlight(foundRanges, _currentRegex, continueWithPreviousColors: true);
            _latestOffset = InputString.Document.ContentStart.GetOffsetToPosition(foundRanges.Last().Start) + 1;
            }
            else
            {
                _latestOffset = -1;
            }
        }
        private void ResetValues()
        {
            Debug.WriteLine("Reset is called");

            _occurrencesHighlighter.ResetTextProperties();

            var foundRanges = _occurrencesFinder
                .GetOccurrencesRanges(_currentRegex)
                .ToArray();


            if (foundRanges.Length > 0)
            {
                _occurrencesHighlighter.Highlight(foundRanges, _currentRegex);
                _latestOffset = InputString.Document.ContentStart.GetOffsetToPosition(foundRanges.Last().Start) + 1;
            }
            else
            {
                _latestOffset = -1;
            }

        }
        #endregion

        private void ResetLatestInputProperties()
        {
            _occurrencesHighlighter.ResetTextProperties(LatestSymbol);
        }
    }
}
