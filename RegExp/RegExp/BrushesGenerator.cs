using System.Collections.Generic;
using System.Windows.Media;

namespace RegExp
{
    public static class BrushesGenerator
    {
        // unsigned Add values
        public static IEnumerable<Brush> GenerateBrushes
        (
            byte R, byte rAdd,
            byte G, byte gAdd,
            byte B, byte bAdd,
            int amount)
        {
            List<Brush> brushes = new List<Brush>(amount);

            for (int i = 0; i < amount; i++)
            {
                Color currentColor;

                unchecked
                {
                    currentColor =
                        Color.FromRgb(
                            (byte)(R + i * rAdd),
                            (byte)(G + i * gAdd),
                            (byte)(B + i * bAdd)
                        );
                }

                brushes.Add(new SolidColorBrush(currentColor));
            }

            return brushes;
        }

        // signed Add values
        public static IEnumerable<Brush> GenerateBrushes
        (byte R, sbyte rAdd,
            byte G, sbyte gAdd,
            byte B, sbyte bAdd,
            int amount)
        {
            List<Brush> brushes = new List<Brush>(amount);

            for (int i = 0; i < amount; i++)
            {
                Color currentColor;

                unchecked
                {
                    currentColor =
                        Color.FromRgb(
                            (byte)(R + i * rAdd),
                            (byte)(G + i * gAdd),
                            (byte)(B + i * bAdd)
                        );
                }

                brushes.Add(new SolidColorBrush(currentColor));
            }

            return brushes;
        }
    }
}
