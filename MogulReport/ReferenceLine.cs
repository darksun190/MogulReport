using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPInterface;

namespace MogulReport
{
    /// <summary>
    /// reference line evaluating the crankshaft circles
    /// the line express by x=x0+t*i ; y=y0+t*j ; z=z0+t*k
    /// </summary>
    class ReferenceLine
    {
        public double x
        {
            get; set;
        }
        public double y
        {
            get; set;
        }
        public double z
        {
            get; set;
        }
        public double i
        {
            get; set;
        }
        public double j
        {
            get; set;
        }
        public double k
        {
            get; set;
        }



        public ReferenceLine(Circle c1, Circle c2)
        {
            x = c1.x;
            y = c1.y;
            z = c1.z;
            double u = Math.Sqrt(
                Math.Pow(c2.x - c1.x, 2) +
                Math.Pow(c2.y - c1.y, 2) +
                Math.Pow(c2.z - c1.z, 2)
                );
            i = (c2.x - c1.x) / u;
            j = (c2.y - c1.y) / u;
            k = (c2.z - c1.z) / u;

        }

        internal CircleOffset getDev(Circle c)
        {
            double ix, iy;
            double t = (c.z - z) / k;
            ix = x + t * i;
            iy = y + t * j;
            return new CircleOffset(c.x - ix, c.y - iy);
        }
    }
}
