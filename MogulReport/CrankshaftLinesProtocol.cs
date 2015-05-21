using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPInterface;

namespace MogulReport
{
    class CrankshaftLinesProtocol : Protocol
    {
        private List<Line> Lines = new List<Line>();
        List<PairedLine> Line_groups = new List<PairedLine>();
        public int groups
        {
            get
            {
                return Line_groups.Count;
            }
        }
        public CrankshaftLinesProtocol(List<Feature> eles)
        {
            foreach (var p in eles)
            {
                Lines.Add((Line)p);
            }

            //check how many groups(pages of protocol) of circles
            Line_groups = guessGroups(Lines);

            foreach (PairedLine pl in Line_groups)
                this.protocolPages.Add(new MogulLineProtocolPage(pl));
            int no_each_group = 2;



        }

        private List<PairedLine> guessGroups(List<Line> lines)
        {
            List<PairedLine> line_groups = new List<PairedLine>();
            List<axisType> line_axis = new List<axisType>();
            foreach (var l in lines)
            {
                line_axis.Add(guessAxis(l));
            }
            axisType first_direction = line_axis.First();
            var get_numbers = line_axis.Where(n => n == first_direction).Count();
            if (get_numbers != lines.Count)
            {
                throw new Exception("lines are not in the same direction");
            }
            Properties.Settings.Default.LineAxis = Convert.ToInt32(line_axis.First());
            Properties.Settings.Default.Save();
            //get all angles of line
            //try to seperate to different groups
            List<double> line_angles = new List<double>();
            for (int i = 0; i < lines.Count; ++i)
            {
                double triangle_2D_x = 0;
                double triangle_2D_y = 0;

                switch (line_axis[i])
                {
                    case axisType.X:
                        triangle_2D_x = lines[i].Alignment_Points.First().y;
                        triangle_2D_y = lines[i].Alignment_Points.First().z;
                        break;
                    case axisType.Y:
                        triangle_2D_x = lines[i].Alignment_Points.First().x;
                        triangle_2D_y = lines[i].Alignment_Points.First().z;
                        break;
                    case axisType.Z:
                        triangle_2D_x = lines[i].Alignment_Points.First().x;
                        triangle_2D_y = lines[i].Alignment_Points.First().y;
                        break;

                    default:
                        throw new Exception("error type on axis direction");
                }

                line_angles.Add(Math.Atan2(triangle_2D_y, triangle_2D_x));
            }

            //group the lines
            for (int i = 0; i < lines.Count; ++i)
            {
                bool new_group = true;
                for (int j = 0; j < line_groups.Count; ++j)
                {
                    var pl = line_groups[j];

                    if (pl.Line2 != null)
                        continue;
                    else
                    {
                        double a1 = pl.angle1;
                        double a2 = line_angles[i];
                        if (Math.Cos(a1 - a2) < -0.99)
                        {
                            pl.Line2 = lines[i];
                            pl.angle2 = line_angles[i];
                            new_group = false;
                            break ;
                        }
                    }

                }
                if(new_group)
                {
                    line_groups.Add(new PairedLine()
                    {
                        group_no = line_groups.Count + 1,
                        Line1 = lines[i],
                        angle1 = line_angles[i]
                    });
                }
            }

            return line_groups;
        }

        private axisType guessAxis(Line l)
        {
            var x_values = l.Alignment_Points.Select(n => n.x).ToList();
            var y_values = l.Alignment_Points.Select(n => n.y).ToList();
            var z_values = l.Alignment_Points.Select(n => n.z).ToList();

            double x_dev = x_values.Max() - x_values.Min();
            double y_dev = y_values.Max() - y_values.Min();
            double z_dev = z_values.Max() - z_values.Min();

            if (x_dev > y_dev && x_dev > z_dev)
                return axisType.X;

            if (y_dev > x_dev && y_dev > z_dev)
                return axisType.Y;

            if (z_dev > x_dev && z_dev > y_dev)
                return axisType.Z;

            throw new Exception("error on comparation, to get a axis direction");
        }



    }
    class PairedLine
    {
        public int group_no
        {
            get;
            set;
        }
        public Line Line1
        {
            get;
            set;
        }
        public Line Line2
        {
            get;
            set;
        }
        public double angle1
        {
            get;
            set;
        }
        public double angle2
        {
            get;
            set;
        }
    }
    enum axisType : int 
    {
        X =1,
        Y =2,
        Z =3,
        UNDEFINE = 0
    }
}
