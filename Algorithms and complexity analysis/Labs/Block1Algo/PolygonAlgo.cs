using System.Drawing;

namespace Block1Algo;

public class PolygonAlgo
{
    private readonly Point[] _points = new Point[PointsAmount];
    private Point[]? _convexHull;
    
    private const int PointsAmount = 100;
    private const int MinXY = 100;
    private const int MaxX = 1000;
    private const int MaxY = 500;

    public PolygonAlgo()
    {
        var random = new Random();
        for (var i = 0; i < 100; i++)
            _points[i] = new Point(
                random.Next(MinXY, MaxX + 1), random.Next(MinXY, MaxY + 1));
    }

    public Point[] GetPoints() => _points.ToArray();

    public double GetConvexHullArea()
    {
        var convexHull = GetConvexHull_GrahamScan();
        var area = 0.0d;
        for (var i = 0; i < convexHull.Length - 1; i++)
            area += convexHull[i].X * convexHull[i + 1].Y;
        area += convexHull[^1].X * convexHull[0].Y;

        for (var i = 0; i < convexHull.Length - 1; i++)
            area -= convexHull[i + 1].X * convexHull[i].Y;
        area -= convexHull[0].X * convexHull[^1].Y;

        return 2 * Math.Abs(area);
    }

    public Point[] GetConvexHull_GrahamScan()
    {
        if (_convexHull is null)
        {
            var convexHull = new List<Point> {GetFirstPointForConvexHull()};
            var pointsSortedByPolarAngles = 
                _points
                    .OrderBy(
                        point => GetPolarAngleCounterclockwise(convexHull[0], point))
                    .ToArray();
            convexHull.Add(pointsSortedByPolarAngles[0]);
            foreach (var point in pointsSortedByPolarAngles.Skip(1))
            {
                while (TurnIsRight(convexHull[^2], convexHull[^1], point))
                    convexHull.RemoveAt(convexHull.Count - 1);
                
                convexHull.Add(point);
            }

            _convexHull = convexHull.ToArray();
            return convexHull.ToArray();
        }

        return _convexHull.ToArray();
    }
    
    public double GetPolarAngleCounterclockwise(Point referencePoint, Point point)
    {
        var axisPoint = new Point(referencePoint.X + 1, referencePoint.Y);
        var a = GetDistance(referencePoint, axisPoint);
        var b = GetDistance(referencePoint, point);
        var c = GetDistance(point, axisPoint);
        var angle = b == 0 ? 0 : Math.Acos((-c * c + a * a + b * b) / (2 * a * b));

        return point.Y > referencePoint.Y ? -angle : angle;
    }
    
    public double GetDistance(Point a, Point b)
        => Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));

    public bool PointIsOnSegment(Point a, Point b, Point point)
    {
        var crossProduct = (point.Y - a.Y) * (b.X - a.X) - (point.X - a.X) * (b.Y - a.Y);
        if (crossProduct != 0)
            return false;

        var dotProduct = (point.X - a.X) * (b.X - a.X) + (point.Y - a.Y) * (b.Y - a.Y);
        if (dotProduct < 0)
            return false;

        if (dotProduct > Math.Pow(GetDistance(a, b), 2))
            return false;
        
        return true;
    }

    public bool TurnIsForward(Point a, Point b, Point c)
    {
        var u = new Point(b.X - a.X, b.Y - a.Y);
        var v = new Point(c.X - b.X, c.Y - b.Y);
        return u.X * v.Y - u.Y * v.X == 0;
    }
    
    public bool TurnIsLeft(Point a, Point b, Point c)
    {
        var u = new Point(b.X - a.X, b.Y - a.Y);
        var v = new Point(c.X - b.X, c.Y - b.Y);
        return u.X * v.Y - u.Y * v.X < 0;
    }
    
    public bool TurnIsRight(Point a, Point b, Point c)
    {
        var u = new Point(b.X - a.X, b.Y - a.Y);
        var v = new Point(c.X - b.X, c.Y - b.Y);
        return u.X * v.Y - u.Y * v.X > 0;
    }

    public bool IsInsidePolygonConvexHull(Point point)
    {
        var convexHull = _convexHull is null ? GetConvexHull_GrahamScan() : _convexHull.ToArray();
        var referencePoint = convexHull[0];
        if (TurnIsLeft(referencePoint, convexHull[^1], point)
                || TurnIsRight(referencePoint, convexHull[1], point))
            return false;
    
        for (var i = 2; i < convexHull.Length - 2; i++)
            if (!(TurnIsForward(referencePoint, convexHull[i], point) 
                  && TurnIsForward(referencePoint, convexHull[i + 1], point)
                  || TurnIsRight(referencePoint, convexHull[i], point) 
                  && TurnIsRight(referencePoint, convexHull[i + 1], point)
                  || TurnIsLeft(referencePoint, convexHull[i], point) 
                  && TurnIsLeft(referencePoint, convexHull[i + 1], point)))
                if (TurnIsLeft(convexHull[i], convexHull[i + 1], point)
                        || TurnIsForward(convexHull[i], convexHull[i + 1], point)
                        && PointIsOnSegment(convexHull[i], convexHull[i + 1], point))
                    return true;

        return false;
    }

    private Point GetFirstPointForConvexHull()
    {
        var firstPoint = _points[0];
        foreach (var point in _points)
            if (firstPoint.Y < point.Y || firstPoint.Y == point.Y && point.X < firstPoint.X)
                firstPoint = point;
        
        return firstPoint;
    }
}