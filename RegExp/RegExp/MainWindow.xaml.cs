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
        private readonly DocumentOccurrencesHighlighter1Async _occurrencesHighlighter;
        private readonly RegexTextProcessor1Async _regexProcessor;
        private bool _isBeingChanged;
        private Match[] _previous;
        private Match[] _current;
        private Regex _currentRegex;
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
                    new DocumentOccurrencesHighlighter1Async(
                        InputString.Document,
                        defaultBack, defaultFore,
                        foregroundHighlightingBrushes, backgroundHighlightingBrushes
                        );
            }
            #endregion

            _regexProcessor = new RegexTextProcessor1Async(InputRegExp, Colors.Red);
        }

        private async void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isBeingChanged)
            {
                //InputString.IsReadOnly = true;

                _isBeingChanged = true;

                try
                {
                    _currentRegex = new Regex(RegExpValue);
                }
                catch (ArgumentException)
                {
                    await _regexProcessor.AddCurvyUnderlineAsync();
                    _isBeingChanged = false;
                    return;
                }

                await _regexProcessor.ResetRegexPropertiesAsync();

                _current = _currentRegex.Matches(Text).Cast<Match>().ToArray();

                bool previousIsCurrent = MatchesComparer.Equals(_current, _previous);
                const int symbolsToRemove = 2;

                if (_current.LastOrDefault()?.Value.Length > 1 && _latestOffset != -1 && LatestSymbolIndex <= _latestOffset)
                {
                    InputString.IsReadOnly = true;
                    await ResetValuesAsync();
                    InputString.IsReadOnly = false;
                }
                else if (_current.LastOrDefault()?.Value.Length == 1 && _latestOffset != -1 && LatestSymbolIndex < _latestOffset)
                {
                    InputString.IsReadOnly = true;
                    await ResetValuesAsync();
                    InputString.IsReadOnly = false;
                }
                else if (_current.LastOrDefault()?.Index + _current.LastOrDefault()?.Length != Text.Length - symbolsToRemove &&
                    previousIsCurrent)
                {
                    InputString.IsReadOnly = true;
                    ResetLatestInputProperties();
                    InputString.IsReadOnly = false;
                }
                else if (_current.ContainsInStart(_previous) && _current.Length > _previous.Length)
                {
                    InputString.IsReadOnly = true;
                    ResetLatestInputProperties();
                    UpdateValues();
                    InputString.IsReadOnly = false;
                }
                else if (!previousIsCurrent)
                {
                    InputString.IsReadOnly = true;
                    await ResetValuesAsync();
                    InputString.IsReadOnly = false;
                }

                _previous = _current;
                _isBeingChanged = false;

                //InputString.IsReadOnly = false;
            }
        }

        #region processing
        private async Task UpdateValuesAsync()
        {
            Debug.WriteLine("UpdateValuesAsync()");

            var foundRanges = _occurrencesFinder
                .GetOccurrencesRanges(_currentRegex, updatePreviousCall: true)
                .ToArray();

            if (foundRanges.Length > 0)
            {
                await _occurrencesHighlighter.HighlightAsync(foundRanges, _currentRegex, continueWithPreviousColors: true);
                _latestOffset = InputString.Document.ContentStart.GetOffsetToPosition(foundRanges.Last().Start) + 1;
            }
            else
            {
                _latestOffset = -1;
            }
        }

        private void UpdateValues()
        {
            Debug.WriteLine("UpdateValues()");

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

        private async Task ResetValuesAsync()
        {
            Debug.WriteLine("ResetValuesAsync()");

            await _occurrencesHighlighter.ResetTextPropertiesAsync();

            var foundRanges = _occurrencesFinder
                .GetOccurrencesRanges(_currentRegex)
                .ToArray();

            if (foundRanges.Length > 0)
            {
                await _occurrencesHighlighter.HighlightAsync(foundRanges, _currentRegex);
                _latestOffset = InputString.Document.ContentStart.GetOffsetToPosition(foundRanges.Last().Start) + 1;
            }
            else
            {
                _latestOffset = -1;
            }

        }

        private void ResetValues()
        {
            Debug.WriteLine("ResetValues()");

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

        private async Task ResetLatestInputPropertiesAsync()
        {
            await _occurrencesHighlighter.ResetTextPropertiesAsync(LatestSymbol);
        }

        private void ResetLatestInputProperties()
        {
            _occurrencesHighlighter.ResetTextProperties(LatestSymbol);
        }
    }
}
