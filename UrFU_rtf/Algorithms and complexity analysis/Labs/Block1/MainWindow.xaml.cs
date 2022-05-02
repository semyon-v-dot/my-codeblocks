using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Block1;

public partial class MainWindow : Window
{
    private readonly Canvas _polygonsMainCanvas = new();

    public MainWindow()
    {
        InitializeComponent();
        Height = 700;
        Width = 1200;

        DrawPoint(_polygonsMainCanvas, 200, 200);
        DrawPoint(_polygonsMainCanvas,100, 100);
        DrawPoint(_polygonsMainCanvas,10, 10);

        Content = _polygonsMainCanvas; // Canvas switch
    }

    private void DrawPoint(Canvas mainCanvas, int x, int y)
    {
        var canvas = new Canvas();
        Canvas.SetLeft(canvas, x);
        Canvas.SetTop(canvas, y);
        
        var point = new Ellipse
        {
            Width = 3,
            Height = 3,
            Fill = Brushes.Black
        };

        canvas.Children.Add(point);
        mainCanvas.Children.Add(canvas);
    }
}