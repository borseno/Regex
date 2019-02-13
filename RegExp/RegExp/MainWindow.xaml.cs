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
        private bool isBeingChanged;

        private string RegExpValue => InputRegExp.Text;

        private string Text => new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd).Text;

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

            if (String.IsNullOrEmpty(RegExpValue))
                return;

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

            IEnumerable<TextRange> textRanges;

            if (RegExpValue.EndsWith("$") &&
                    RegExpValue.StartsWith("^") &&
                    RegExpValue.Trim('^', '$') == Text.RemoveAll('\r', '\n') ||
                    RegExpValue == Text.RemoveAll('\r', '\n'))
                textRanges = new List<TextRange> { new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd) };
            else
                textRanges = GetAllWordRanges();

            HighlightTextRanges(textRanges);
        }

        public IEnumerable<TextRange> GetAllWordRanges()
        {
            FlowDocument document = InputString.Document;
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

            InputRegExp.TextDecorations.Clear();

            isBeingChanged = false;
        }

        private void HighlightTextRanges(IEnumerable<TextRange> textRanges)
        {
            isBeingChanged = true;

            var textRangesArray = textRanges.ToArray();

            CircularArray<Brush> backgroundValues = new CircularArray<Brush>(new Brush[] { Brushes.Black, Brushes.Gray });

            int index = 0;

            foreach (TextRange i in textRangesArray)
            {
                string regExpression = RegExpValue;

                regExpression = !regExpression.StartsWith("^") ? '^' + regExpression : regExpression;
                regExpression = !regExpression.EndsWith("$") ? regExpression + '$' : regExpression;

                if (Regex.IsMatch(i.Text, regExpression))
                {
                    i.ApplyPropertyValue(TextElement.BackgroundProperty, backgroundValues[index++]);
                    i.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Azure);
                }
            }

            isBeingChanged = false;
        }

        private void RedCurvyUnderline()
        {
            isBeingChanged = true;

            Pen path_pen = new Pen(new SolidColorBrush(Colors.Red), 0.2)
            {
                EndLineCap = PenLineCap.Square,
                StartLineCap = PenLineCap.Square
            };

            Point path_start = new Point(0, 1);
            BezierSegment path_segment = new BezierSegment(new Point(1, 0), new Point(2, 2), new Point(3, 1), true);
            PathFigure path_figure = new PathFigure(path_start, new PathSegment[] { path_segment }, false);
            PathGeometry path_geometry = new PathGeometry(new PathFigure[] { path_figure });

            DrawingBrush squiggly_brush = new DrawingBrush
            {
                Viewport = new Rect(0, 2.2, 6, 4),
                ViewportUnits = BrushMappingMode.Absolute,
                TileMode = TileMode.Tile,
                Drawing = new GeometryDrawing(null, path_pen, path_geometry)
            };

            TextDecoration squiggly = new TextDecoration
            {
                Pen = new Pen(squiggly_brush, 2.6)
            };
            InputRegExp.TextDecorations.Add(squiggly);

            isBeingChanged = false;
        }
        #endregion
    }
}
