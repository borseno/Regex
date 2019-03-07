using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace RegExp
{
    class RegexTextProcessor
    {
        public Color Color { get; set; }

        public TextBox TextBox { get; }

        public RegexTextProcessor(TextBox textBox, Color color)
        {
            TextBox = textBox;
            Color = color;
        }

        public void AddCurvyUnderline()
        {
            Pen path_pen = new Pen(new SolidColorBrush(Color), 0.2)
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

            TextBox.TextDecorations.Add(squiggly);
        }
    }
}
