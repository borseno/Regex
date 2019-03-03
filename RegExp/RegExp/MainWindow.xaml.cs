﻿using System;
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
        private TextPointer _previousCaretPosition; // used if input is in the end to get the latest textRange

        private string RegExpValue => InputRegExp.Text;

        private string Text => new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd).Text;

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

                if (_current.ContainsInStart(_previous) && _current.Length > _previous.Length)
                    UpdateValues();
                else if (!MatchesComparer.Equals(_current, _previous))
                    ResetValues();
                else
                    ResetLatestInputProperties();

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

            _occurrencesHighlighter.Highlight(foundRanges, _currentRegex, continueWithPreviousColors: true);
        }
        private void ResetValues()
        {
            _occurrencesHighlighter.ResetTextProperties();

            var foundRanges = _occurrencesFinder
                .GetOccurrencesRanges(_currentRegex)
                .ToArray();

            _occurrencesHighlighter.Highlight(foundRanges, _currentRegex);
        }
        #endregion

        private void ResetLatestInputProperties()
        {
            TextRange latest = new TextRange(_previousCaretPosition, InputString.CaretPosition); // bug here!?

            _occurrencesHighlighter.ResetTextProperties(latest);
        }

        private void InputString_KeyDown(object sender, KeyEventArgs e)
        {
            _previousCaretPosition = InputString.CaretPosition;
        }
    }
}
