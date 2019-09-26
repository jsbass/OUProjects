using System.Drawing;
using System.Security.Cryptography.Xml;

namespace Portal.Helpers
{
    public class RectangleD
    {
        private PointD _topLeft;
        private PointD _bottomRight;
        private SizeD _size;

        public double X
        {
            get { return _topLeft.X; }
            set
            {
                _size.Width += (value - _topLeft.X);
                _topLeft.X = value;
            }
        }

        public double Y
        {
            get { return _topLeft.Y; }
            set
            {
                _size.Height += (value - _topLeft.Y);
                _topLeft.Y = value;
            }
        }

        public double Left => _topLeft.X;
        public double Top => _topLeft.Y;
        public double Right => _bottomRight.X;
        public double Bottom => _bottomRight.Y;
        public double Height
        {
            get { return _size.Height; }
            set
            {
                _size.Height = value;
                _bottomRight.Y = _topLeft.Y - _size.Height;
            }
        }

        public double Width
        {
            get { return _size.Width; }
            set
            {
                _size.Width = value;
                _bottomRight.X = _topLeft.X + _size.Width;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">top left point</param>
        /// <param name="size">size</param>
        public RectangleD(PointD point, SizeD size)
        {
            _topLeft = point;
            _size = size;
            _bottomRight = new PointD(_topLeft.X + _size.Width, _topLeft.Y - _size.Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">top eft X</param>
        /// <param name="y">top left Y</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        public RectangleD(double x, double y, double w, double h)
        {
            _topLeft = new PointD(x, y);
            _size = new SizeD(w, h);
            _bottomRight = new PointD(_topLeft.X + _size.Width, _topLeft.Y - _size.Height);
        }
        
        public static RectangleD FromLTRB(double left, double top, double right, double bottom)
        {
            var w = right - left;
            var h = top - bottom;
            return new RectangleD(left, top, w, h);
        }

        public bool Contains(PointD p)
        {
            return Contains(p.X, p.Y);
        }

        public bool Contains(double x, double y)
        {
            return
                Left <= x &&
                Right >= x &&
                Bottom <= y &&
                Top >= y;
        }

        public bool Contains(RectangleD rect)
        {
            return
                Left <= rect.Left &&
                Right >= rect.Right &&
                Top >= rect.Top &&
                Bottom <= rect.Bottom;
        }

        public bool IntersectsWith(RectangleD rect)
        {
            return
                !(rect.Left > Right ||
                  rect.Right < Left ||
                  rect.Top > Bottom ||
                  rect.Bottom < Top);
        }
    }
    public class LineD
    {
        public PointD p1 { get; set; }
        public PointD p2 { get; set; }

        public double A;
        public double B;
        public double C;

        public LineD(double x1, double y1, double x2, double y2)
        {
            p1 = new PointD(x1, y1);
            p2 = new PointD(x2, y2);
            A = y2 - y1;
            B = x1 - x2;
            C = (x2 * y1) - (x1 * y2);
        }

        public bool Intersects(LineD l)
        {
            var d1 = (A * l.p1.X) + (B * l.p1.Y) + C;
            var d2 = (A * l.p2.X) + (B * l.p2.Y) + C;

            if (d1 > 0 && d2 > 0) return false;
            if (d1 < 0 && d2 < 0) return false;

            d1 = (l.A * p1.X) + (l.B * p1.Y) + l.C;
            d2 = (l.A * p2.X) + (l.B * p2.Y) + l.C;

            if (d1 > 0 && d2 > 0) return false;
            if (d1 < 0 && d2 < 0) return false;

            //Lines are colinear or intersect
            return true;
        }
    }

    public class PointD
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class SizeD
    {
        public double Height { get; set; }
        public double Width { get; set; }

        public SizeD(double w, double h)
        {
            Height = h;
            Width = w;
        }
    }
}