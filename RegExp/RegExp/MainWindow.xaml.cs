using System;
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

                if (!textRanges.Any())
                    textRanges = new List<TextRange> { new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd) };

                HighlightTextRanges(textRanges);
            }
        }

        private IEnumerable<TextRange> GetAllWordRanges(FlowDocument document)
        {
            string pattern = RegExpValue;
            TextPointer pointer = document.ContentStart;
            while (pointer != null)
            {
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

        // TODO: If 2 matches lie next to each other (e.g 'ab' where regex = "[ab]",
        // TODO: then highlight them with different colors.
        private void HighlightTextRanges(IEnumerable<TextRange> textRanges)
        {
            isBeingChanged = true;
            foreach (TextRange i in textRanges)
            {
                string regExpression = RegExpValue;
                regExpression = !regExpression.StartsWith("^") ? '^' + regExpression : regExpression;
                regExpression = !regExpression.EndsWith("$") ? regExpression + '$' : regExpression;

                if (Regex.IsMatch(i.Text.TrimEnd('\r', '\n'), regExpression))
                {
                    i.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Black);
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
