using System;


namespace rt
{
    public class Ellipsoid : Geometry
    {
        private Vector Center { get; }
        private Vector SemiAxesLength { get; }
        private double Radius { get; }
        
        
        public Ellipsoid(Vector center, Vector semiAxesLength, double radius, Material material, Color color) : base(material, color)
        {
            Center = center;
            SemiAxesLength = semiAxesLength;
            Radius = radius;
        }

        public Ellipsoid(Vector center, Vector semiAxesLength, double radius, Color color) : base(color)
        {
            Center = center;
            SemiAxesLength = semiAxesLength;
            Radius = radius;
        }

        public override Intersection GetIntersection(Line line, double minDist, double maxDist)
        {
            var a = Math.Pow(line.Dx.X * SemiAxesLength.Y * SemiAxesLength.Z, 2) +
                    Math.Pow(line.Dx.Y * SemiAxesLength.X * SemiAxesLength.Z, 2) +
                    Math.Pow(line.Dx.Z * SemiAxesLength.X * SemiAxesLength.Y, 2);

            var b = 2 * line.Dx.X * Math.Pow(SemiAxesLength.Y, 2) * Math.Pow(SemiAxesLength.Z, 2) * (line.X0.X - Center.X) +
                    2 * line.Dx.Y * Math.Pow(SemiAxesLength.X, 2) * Math.Pow(SemiAxesLength.Z, 2) * (line.X0.Y - Center.Y) +
                    2 * line.Dx.Z * Math.Pow(SemiAxesLength.X, 2) * Math.Pow(SemiAxesLength.Y, 2) * (line.X0.Z - Center.Z);

            var c = Math.Pow(SemiAxesLength.Y, 2) * Math.Pow(SemiAxesLength.Z, 2) * Math.Pow(line.X0.X - Center.X, 2) +
                    Math.Pow(SemiAxesLength.X, 2) * Math.Pow(SemiAxesLength.Z, 2) * Math.Pow(line.X0.Y - Center.Y, 2) +
                    Math.Pow(SemiAxesLength.X, 2) * Math.Pow(SemiAxesLength.Y, 2) * Math.Pow(line.X0.Z - Center.Z, 2) -
                    Math.Pow(Radius, 2) * Math.Pow(SemiAxesLength.X, 2) * Math.Pow(SemiAxesLength.Y, 2) * Math.Pow(SemiAxesLength.Z, 2);

            var delta = Math.Pow(b, 2) - 4 * a * c;
            
            if (delta < 0 )
            {
                return new Intersection();
            }
            if (delta == 0)
            {
                var t =  - b / (2 * a);
                var visible = (line.CoordinateToPosition(t) - line.X0).Length() < minDist ||
                              (line.CoordinateToPosition(t) - line.X0).Length() > maxDist ||
                              t < 0 ? false : true;
                var normal = new Vector(
                    (line.CoordinateToPosition(t).X - Center.X) * 2 / (SemiAxesLength.X * SemiAxesLength.X),
                    (line.CoordinateToPosition(t).Y - Center.Y) * 2 / (SemiAxesLength.Y * SemiAxesLength.Y),
                    (line.CoordinateToPosition(t).Z - Center.Z) * 2 / (SemiAxesLength.Z * SemiAxesLength.Z)
                    )
                    .Normalize();
                return new Intersection(true, visible, this, line, t, normal, Material, Color);
            }
            var t1 = (- b - Math.Sqrt(delta))/ (2 * a);
            var t2 = (- b + Math.Sqrt(delta))/ (2 * a);

            if (t1 < t2)
            {
                var visible = (line.CoordinateToPosition(t1) - line.X0).Length() < minDist ||
                              (line.CoordinateToPosition(t1) - line.X0).Length() > maxDist ||
                              t1 < 0 ? false : true;
                var normal = new Vector(
                    (line.CoordinateToPosition(t1).X - Center.X) * 2 / (SemiAxesLength.X * SemiAxesLength.X),
                    (line.CoordinateToPosition(t1).Y - Center.Y) * 2 / (SemiAxesLength.Y * SemiAxesLength.Y),
                    (line.CoordinateToPosition(t1).Z - Center.Z) * 2 / (SemiAxesLength.Z * SemiAxesLength.Z)
                    )
                    .Normalize();
                return new Intersection(true, visible, this, line, t1, normal, Material, Color);
            }
            else
            {
                var visible = (line.CoordinateToPosition(t2) - line.X0).Length() < minDist ||
                              (line.CoordinateToPosition(t2) - line.X0).Length() > maxDist ||
                              t2 < 0 ? false : true;
                var normal = new Vector(
                    (line.CoordinateToPosition(t2).X - Center.X) * 2 / (SemiAxesLength.X * SemiAxesLength.X),
                    (line.CoordinateToPosition(t2).Y - Center.Y) * 2 / (SemiAxesLength.Y * SemiAxesLength.Y),
                    (line.CoordinateToPosition(t2).Z - Center.Z) * 2 / (SemiAxesLength.Z * SemiAxesLength.Z)
                    )
                    .Normalize();
                return new Intersection(true, visible, this, line, t2, normal , Material, Color);
            }
        }
    }
}