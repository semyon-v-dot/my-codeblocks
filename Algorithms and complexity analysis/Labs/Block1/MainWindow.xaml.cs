using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Block1Algo;
using Point = System.Drawing.Point;

namespace Block1;

public partial class MainWindow : Window
{
    private readonly PolygonAlgo _algo = new();
    private readonly MyWheel _wheel = new(new Point(200, 200), 100);

    private readonly Canvas _polygonCanvas = new();
    private readonly Canvas _geometricMotionCanvas = new();
    private readonly Canvas _objectLightingCanvas = new();

    private DispatcherTimer timer;

    public MainWindow()
    {
        InitializeComponent();
        Height = 700;
        Width = 1200;

        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(0.1)
        };
        timer.Tick += Timer_Tick;
        
        timer.Start();
    }

    private void DrawPolygonCanvas(object sender, RoutedEventArgs e)
    {
        DrawPolygon();
            
        DrawPolygonConvexHull();

        DrawAreaNumber();

        Content = _polygonCanvas;

    }

    private void DrawGeometricMotion(object sender, RoutedEventArgs e)
    {
        DrawWheel();

        Content = _geometricMotionCanvas;
    }

    private void Draw3DObjectLighting(object sender, RoutedEventArgs e)
    {        
        DrawPoint(_objectLightingCanvas, 500, 500);

        // TODO

        Content = _objectLightingCanvas;
    }

    private void DrawWheel()
    {
        DrawCircle(_geometricMotionCanvas, _wheel.WheelCircle.Center, _wheel.WheelCircle.Radius);
        foreach (var segment in _wheel.GetSegments())
            DrawLine(_geometricMotionCanvas, segment.Start, segment.End);
    }

    private void DrawPolygon()
    {
        var points = _algo.GetPoints();
        foreach (var point in points)
            DrawPoint(_polygonCanvas, point.X, point.Y);
    }

    private void DrawPolygonConvexHull()
    {
        var convexHull = _algo.GetConvexHull_GrahamScan();
        for (var i = 1; i < convexHull.Length; i++)
            DrawLine(_polygonCanvas, convexHull[i - 1], convexHull[i]);
        DrawLine(_polygonCanvas, convexHull[^1], convexHull[0]);
    }

    private void DrawAreaNumber()
    {
        const string areaText = "Площадь многоугольника (в пикселях): ";
        DrawText(
            _polygonCanvas,
            areaText + _algo.GetConvexHullArea().ToString(CultureInfo.CurrentCulture), 
            20, 
            20);
    }

    private void Window_MouseMove(object sender, MouseEventArgs mouseEventArgs)
    {
        var point = mouseEventArgs.GetPosition(this);
        var textStart = "Точка внутри многоугольника: ";
        var ifInsidePolygon = 
            _algo.IsInsidePolygonConvexHull(new Point((int)point.X, (int)point.Y))
            ? "ДА"
            : "НЕТ";
        
        DrawWhiteRectangle(_polygonCanvas, 300, 20, 50, 50);
        DrawText(_polygonCanvas, textStart + ifInsidePolygon, 50, 50);
    }
    
    private void Timer_Tick(object? sender, EventArgs e)
    {
        _wheel.WheelRight();
        _geometricMotionCanvas.Children.Clear();
        DrawWheel();
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
    
    private void DrawLine(Canvas canvas, Point a, Point b)
    {
        var line = new Line
        {
            X1 = a.X,
            Y1 = a.Y,
            X2 = b.X,
            Y2 = b.Y,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        canvas.Children.Add(line);
    }

    private void DrawText(Canvas mainCanvas, string text, int x, int y)
    {
        var canvas = new Canvas();
        Canvas.SetLeft(canvas, x);
        Canvas.SetTop(canvas, y);
        var textBlock = new TextBlock
        {
            Height = 30,
            Text = text,
            Foreground = Brushes.Black
        };

        canvas.Children.Add(textBlock);
        mainCanvas.Children.Add(canvas);
    }

    private void DrawWhiteRectangle(Canvas mainCanvas, int width, int height, int x, int y)
    {
        var canvas = new Canvas();
        Canvas.SetLeft(canvas, x);
        Canvas.SetTop(canvas, y);
        var textBlock = new Rectangle
        {
            Width = width,
            Height = height,
            Fill = Brushes.White
        };

        canvas.Children.Add(textBlock);
        mainCanvas.Children.Add(canvas);
    }

    private void DrawCircle(Canvas mainCanvas, Point center, int radius)
    {
        var canvas = new Canvas();
        Canvas.SetLeft(canvas, center.X);
        Canvas.SetTop(canvas, center.Y);
        
        var point = new Ellipse
        {
            Width = radius * 2,
            Height = radius * 2,
            Stroke = Brushes.Black,
            StrokeThickness = 2
        };

        canvas.Children.Add(point);
        mainCanvas.Children.Add(canvas);
    }
}