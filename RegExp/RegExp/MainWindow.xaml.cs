﻿using System;
using System.Collections.Generic;
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

namespace RegExp
{
    public partial class MainWindow : Window
    {
        private bool isBeingChanged;

        private string RegExpValue
        {
            get
            {
                return InputRegExp.Text;
            }
            set
            {
                InputRegExp.Text = value;
            }
        }
        private string Text
        {
            get
            {
                return new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd).Text;
            }
            set
            {
                InputString.Document.Blocks.Clear();
                InputString.Document.Blocks.Add(new Paragraph(new Run(value)));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isBeingChanged)
                UpdateValues();
        }

        #region processing
        private void UpdateValues()
        {
            ResetTextProperties();
            ResetRegexProperties();

            Regex regex = null;
            try
            {
                regex = new Regex(RegExpValue);
            }
            catch (ArgumentException)
            {
                RedCurvyUnderline();
                return;
            }

            if (regex.IsMatch(Text.TrimEnd(new[] { '\r', '\n' })))
            {
                IEnumerable<TextRange> textRanges = GetAllWordRanges(InputString.Document);

                isBeingChanged = true;
                foreach (TextRange i in textRanges)
                {
                    string regExpression = RegExpValue;
                    regExpression = !regExpression.StartsWith("^") ? '^' + regExpression : regExpression;
                    regExpression = !regExpression.EndsWith("$") ? regExpression + '$' : regExpression;

                    if (Regex.IsMatch(i.Text, regExpression))
                    {
                        i.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Black);
                        i.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Azure);
                    }
                }
                isBeingChanged = false;
            }
        }

        // TODO: 
        // Reconstruct this method. Buggy as hell.
        // Latest bug: the words in a string are split by space, can't match the whole string
        //        bug: e.g ( String: 'Марку 15 лет...,:sf12', Regex: 'Марку 15 лет...,:sf12' )
        private IEnumerable<TextRange> GetAllWordRanges(FlowDocument document)
        {
            string pattern = RegExpValue;
            TextPointer pointer = document.ContentStart;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                    MatchCollection matches = Regex.Matches(textRun, pattern);
                    foreach (Match match in matches)
                    {
                        int startIndex = match.Index;
                        int length = match.Length;
                        TextPointer start = pointer.GetPositionAtOffset(startIndex);
                        TextPointer end = start.GetPositionAtOffset(length);
                        yield return new TextRange(start, end);
                    }
                }
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        private void ResetTextProperties()
        {
            isBeingChanged = true;

            TextRange textRange = new TextRange(
                InputString.Document.ContentStart,
                InputString.Document.ContentEnd
            );

            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);

            isBeingChanged = false;
        }

        private void ResetRegexProperties()
        {
            isBeingChanged = true;

            for (int i = 0; i < InputRegExp.TextDecorations.Count; i++)
            {
                InputRegExp.TextDecorations.RemoveAt(i);
            }

            isBeingChanged = false;
        }


        private void RedCurvyUnderline()
        {
            isBeingChanged = true;

            Pen path_pen = new Pen(new SolidColorBrush(Colors.Red), 0.2);
            path_pen.EndLineCap = PenLineCap.Square;
            path_pen.StartLineCap = PenLineCap.Square;

            Point path_start = new Point(0, 1);
            BezierSegment path_segment = new BezierSegment(new Point(1, 0), new Point(2, 2), new Point(3, 1), true);
            PathFigure path_figure = new PathFigure(path_start, new PathSegment[] { path_segment }, false);
            PathGeometry path_geometry = new PathGeometry(new PathFigure[] { path_figure });

            DrawingBrush squiggly_brush = new DrawingBrush();
            squiggly_brush.Viewport = new Rect(0, 2.2, 6, 4);
            squiggly_brush.ViewportUnits = BrushMappingMode.Absolute;
            squiggly_brush.TileMode = TileMode.Tile;
            squiggly_brush.Drawing = new GeometryDrawing(null, path_pen, path_geometry);

            TextDecoration squiggly = new TextDecoration();
            squiggly.Pen = new Pen(squiggly_brush, 2.6);
            InputRegExp.TextDecorations.Add(squiggly);

            isBeingChanged = false;
        }
        #endregion
    }
}
