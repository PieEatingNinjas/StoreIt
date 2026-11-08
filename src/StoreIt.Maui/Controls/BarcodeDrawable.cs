using ZXing.Common;

namespace StoreIt.Maui.Controls;

// Draws a ZXing bit matrix: 1D codes stretch to the full height, 2D codes keep their aspect ratio.
public class BarcodeDrawable : IDrawable
{
    public BitMatrix? Matrix { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var matrix = Matrix;
        if (matrix is null || matrix.Width == 0 || matrix.Height == 0 || dirtyRect.Width <= 0 || dirtyRect.Height <= 0)
        {
            return;
        }

        canvas.Antialias = false;
        canvas.FillColor = Colors.Black;

        var isLinear = matrix.Height == 1;
        var moduleWidth = dirtyRect.Width / matrix.Width;
        var moduleHeight = isLinear ? dirtyRect.Height : dirtyRect.Height / matrix.Height;

        if (!isLinear)
        {
            moduleWidth = moduleHeight = Math.Min(moduleWidth, moduleHeight);
        }

        var offsetX = dirtyRect.X + (dirtyRect.Width - (moduleWidth * matrix.Width)) / 2;
        var offsetY = dirtyRect.Y + (dirtyRect.Height - (moduleHeight * matrix.Height)) / 2;

        for (var y = 0; y < matrix.Height; y++)
        {
            var x = 0;
            while (x < matrix.Width)
            {
                if (!matrix[x, y])
                {
                    x++;
                    continue;
                }

                // Merge adjacent dark modules into a single rectangle to avoid hairline gaps.
                var start = x;
                while (x < matrix.Width && matrix[x, y])
                {
                    x++;
                }

                canvas.FillRectangle(
                    offsetX + (start * moduleWidth),
                    offsetY + (y * moduleHeight),
                    (x - start) * moduleWidth,
                    moduleHeight);
            }
        }
    }
}
