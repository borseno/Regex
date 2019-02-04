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
using Data_Structures;

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

            if (regex.IsMatch(Text.TrimEnd(new[] { '\r', '\n' })))
            {
                IEnumerable<TextRange> textRanges = GetAllWordRanges(InputString);

                if (!textRanges.Any())
                    textRanges = new List<TextRange> { new TextRange(InputString.Document.ContentStart, InputString.Document.ContentEnd) };

                HighlightTextRanges(textRanges);
            }
        }

        private IEnumerable<TextRange> GetAllWordRanges(RichTextBox richTextBox)
        {
            string pattern = RegExpValue;

            Match[] matches = Regex.Matches(Text.TrimEnd('\r', '\n'), pattern).Cast<Match>().ToArray();

            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            for (int i = 0; i < matches.Length; i++)
            {
                int startIndex = matches[i].Index;
                int length = matches[i].Length;

                TextRange subRange = Select(richTextBox, startIndex, length);

                yield return subRange;
            }
        }

        private TextRange Select(RichTextBox rtb, int index, int length)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            if (textRange.Text.TrimEnd('\r', '\n').Length >= (index + length))
            {
                TextPointer start = textRange.Start.GetPositionAtOffset(index, LogicalDirection.Forward);
                TextPointer end = textRange.Start.GetPositionAtOffset(index + length, LogicalDirection.Backward);

                return new TextRange(start, end);
            }
            return textRange;
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
            var textRangesArray = textRanges.ToArray();
            isBeingChanged = true;
            
            // TODO: Implement this.
            CircularArray<Brush> backgroundValues = new CircularArray<Brush>(new Brush[] {Brushes.Black, Brushes.Gray});
            
            int index = 0;

            foreach (TextRange i in textRangesArray)
            {
                string regExpression = RegExpValue;
                regExpression = !regExpression.StartsWith("^") ? '^' + regExpression : regExpression;
                regExpression = !regExpression.EndsWith("$") ? regExpression + '$' : regExpression;

                if (Regex.IsMatch(i.Text.TrimEnd('\r', '\n'), regExpression))
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
