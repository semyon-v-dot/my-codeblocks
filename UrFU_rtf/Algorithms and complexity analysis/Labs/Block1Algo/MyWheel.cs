using System.Drawing;

namespace Block1Algo;

public class MyWheel
{
    public Circle WheelCircle { get; }

    private Segment[] WheelSegments { get; }

    private static readonly int WheelStep = 5;
    private static readonly double WheelAngleStep = Math.PI / 36;
    private readonly int AmountOfSegments = 3;
    
    public class Circle
    {
        public Point Center { get; private set; }
        public int Radius { get; }

        public Circle(Point center, int radius)
        {
            Center = center;
            Radius = radius;
        }

        public void WheelRight()
        {
            Center = new Point(Center.X + WheelStep, Center.Y);
        }
    }

    public class Segment
    {
        public Point Start { get; }
        public Point End { get; }
        
        public int RotationMultiplier { get; private set; }
        
        public Segment(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        // public void WheelRight(Point circleCenter, int radius)
        // {
        //     var rotated = GetRotated(WheelAngleStep, circleCenter, radius);
        //     Start = new Point(rotated.Start.X + WheelStep, rotated.Start.Y);
        //     End = new Point(rotated.End.X + WheelStep, rotated.End.Y);
        // }
        
        public static Point RotateVector(Point point, double angle)
            => new (
                (int) (point.X * Math.Cos(angle) - point.Y * Math.Sin(angle)),
                (int) (point.X * Math.Sin(angle) + point.Y * Math.Cos(angle)));

        public Segment GetRotated(double angle, Point circleCenter, int radius)
        {
            var wheelCenter = new Point(circleCenter.X + radius, circleCenter.Y + radius);
            var centerStartVector = new Point(Start.X - wheelCenter.X, Start.Y - wheelCenter.Y);  
            centerStartVector = RotateVector(centerStartVector, angle);
            var centerEndVector = new Point(End.X - wheelCenter.X, End.Y - wheelCenter.Y);
            centerEndVector = RotateVector(centerEndVector, angle);

            return new Segment(
                new Point(
                    centerStartVector.X + wheelCenter.X + WheelStep, 
                    centerStartVector.Y + wheelCenter.Y),
                new Point(
                    centerEndVector.X + wheelCenter.X + WheelStep, 
                    centerEndVector.Y + wheelCenter.Y));
        }

        public void IncrementMultiplier()
        {
            RotationMultiplier += 1;
        }
    }
    
    public MyWheel(Point circleCenter, int radius)
    {
        WheelCircle = new Circle(circleCenter, radius);
        WheelSegments = new Segment[AmountOfSegments];

        var referenceSegment = new Segment(
            new Point(circleCenter.X + radius, circleCenter.Y),
            new Point(circleCenter.X + radius, circleCenter.Y + 2 *  radius));
        var rotationAngle = Math.PI / AmountOfSegments;
        for (var i = 0; i < AmountOfSegments; i++)
            WheelSegments[i] = referenceSegment.GetRotated(
                rotationAngle * i, circleCenter, radius);
    }

    public void WheelRight()
    {
        WheelCircle.WheelRight();
        foreach (var segment in WheelSegments)
            segment.IncrementMultiplier();
    }

    public Segment[] GetSegments()
    {
        var output = new Segment[AmountOfSegments];
        for (var i = 0; i < AmountOfSegments; i++)
            output[i] = 
                WheelSegments[i]
                    .GetRotated(
                        WheelAngleStep * WheelSegments[i].RotationMultiplier,
                        WheelCircle.Center,
                        WheelCircle.Radius);

        return output;
    }
}