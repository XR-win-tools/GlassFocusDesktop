using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GlassFocusDesktop.Controls;

public class NoiseOverlay : FrameworkElement
{
    private ImageBrush? _noiseBrush;

    public static readonly DependencyProperty NoiseOpacityProperty =
        DependencyProperty.Register(nameof(NoiseOpacity), typeof(double), typeof(NoiseOverlay),
            new FrameworkPropertyMetadata(0.08, FrameworkPropertyMetadataOptions.AffectsRender));

    public double NoiseOpacity
    {
        get => (double)GetValue(NoiseOpacityProperty);
        set => SetValue(NoiseOpacityProperty, value);
    }

    protected override void OnRender(DrawingContext dc)
    {
        _noiseBrush ??= CreateNoiseBrush(128, 128);
        dc.PushOpacity(NoiseOpacity);
        dc.DrawRectangle(_noiseBrush, null, new Rect(RenderSize));
        dc.Pop();
    }

    private static ImageBrush CreateNoiseBrush(int width, int height)
    {
        var random = new Random(42);
        var stride = width * 4;
        var pixels = new byte[height * stride];
        for (int i = 0; i < pixels.Length; i += 4)
        {
            var v = (byte)random.Next(0, 256);
            pixels[i] = v;
            pixels[i + 1] = v;
            pixels[i + 2] = v;
            pixels[i + 3] = 255;
        }
        var bmp = BitmapSource.Create(width, height, 96, 96,
            PixelFormats.Bgra32, null, pixels, stride);
        var brush = new ImageBrush(bmp)
        {
            TileMode = TileMode.Tile,
            Viewport = new Rect(0, 0, width, height),
            ViewportUnits = BrushMappingMode.Absolute,
            Stretch = Stretch.None
        };
        brush.Freeze();
        return brush;
    }
}