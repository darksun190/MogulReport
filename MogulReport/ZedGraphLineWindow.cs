using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.Configuration;
using SPInterface;
using MathNet.Numerics.Statistics;

namespace MogulReport
{
    public partial class ZedGraphLineWindow : ZedGraphBaseWindow
    {
        private PairedLine pLine;
        double magnification;
        List<double> line1_point_x = null;
        List<double> line1_point_z = null;
        List<double> line2_point_x = null;
        List<double> line2_point_z = null;

        double line1_angle;
        double line2_angle;

        List<PointPairList> dealed_line1;
        List<PointPairList> dealed_line2;


        public ZedGraphLineWindow()
        {
            InitializeComponent();
        }

        internal ZedGraphLineWindow(PairedLine pLine)
            : this()
        {
            // TODO: Complete member initialization
            this.pLine = pLine;
            axisType axist = (axisType)Properties.Settings.Default.LineAxis;
            magnification = Convert.ToDouble(ConfigurationManager.AppSettings["LineMagnification"]);
            switch (axist)
            {
                case axisType.X:
                    line1_point_x = pLine.Line1.Alignment_Points.Select(n => n.x).ToList();
                    //  line1_point_z = pLine.Line1.Alignment_Points.Select(n => Math.Sqrt(Math.Pow(n.y, 2) + Math.Pow(n.z, 2))).ToList();
                    line2_point_x = pLine.Line2.Alignment_Points.Select(n => n.x).ToList();
                    // line2_point_z = pLine.Line2.Alignment_Points.Select(n => Math.Sqrt(Math.Pow(n.y, 2) + Math.Pow(n.z, 2))).ToList();
                    line1_angle = pLine.Line1.i;
                    line2_angle = pLine.Line2.i;
                    break;

                case axisType.Y:
                    line1_point_x = pLine.Line1.Alignment_Points.Select(n => n.y).ToList();
                    line1_point_z = pLine.Line1.Alignment_Points.Select(n => Math.Sqrt(Math.Pow(n.x, 2) + Math.Pow(n.z, 2))).ToList();
                    line2_point_x = pLine.Line2.Alignment_Points.Select(n => n.y).ToList();
                    line2_point_z = pLine.Line2.Alignment_Points.Select(n => Math.Sqrt(Math.Pow(n.x, 2) + Math.Pow(n.z, 2))).ToList();
                    line1_angle = pLine.Line1.j;
                    line2_angle = pLine.Line2.j;
                    break;

                case axisType.Z:
                    line1_point_x = pLine.Line1.Alignment_Points.Select(n => n.z).ToList();
                    //line1_point_z = pLine.Line1.Alignment_Points.Select(n => Math.Sqrt(Math.Pow(n.y, 2) + Math.Pow(n.x, 2))).ToList();
                    line2_point_x = pLine.Line2.Alignment_Points.Select(n => n.z).ToList();
                    // line2_point_z = pLine.Line2.Alignment_Points.Select(n => Math.Sqrt(Math.Pow(n.y, 2) + Math.Pow(n.x, 2))).ToList();
                    line1_angle = pLine.Line1.k;
                    line2_angle = pLine.Line2.k;
                    break;

                default:
                    throw new Exception("axis type error");
            }
            line1_point_z = pLine.Line1.Deviations.Select(n => -n).ToList();
            line2_point_z = pLine.Line2.Deviations.Select(n => -n).ToList();
            double a1 = line1_angle;
            double b1 = line1_point_z[0] - a1 * line1_point_x[0];
            double b0_1 = line1_point_z[0];
            double a2 = line2_angle;
            double b2 = line2_point_z[0] - a2 * line2_point_x[0];
            double b0_2 = line2_point_z[0];

            PointPairList temp_ppl1 = new PointPairList(line1_point_x.ToArray(), line1_point_z.ToArray());
            PointPairList temp_ppl2 = new PointPairList(line2_point_x.ToArray(), line2_point_z.ToArray());

            line1_point_z = temp_ppl1.Select(n => (n.Y - b0_1) / Math.Sqrt(1 - Math.Pow(a1, 2)) + (a1 * n.X + b1)).ToList();
            line2_point_z = temp_ppl2.Select(n => (n.Y - b0_2) / Math.Sqrt(1 - Math.Pow(a1, 2)) + (a2 * n.X + b2)).ToList();

            int num = 150;
            this.zg.Size = new Size(500 + num, 500);
            GraphPane graphPane = zg.GraphPane;
            graphPane.Margin.Left = (float)num;
            graphPane.CurveList.Clear();
            this.drawLayout(graphPane);

            drawObjects(graphPane);

            drawRefLine(graphPane);


            drawLinePoints(graphPane);
        }

        private void drawObjects(GraphPane graphPane)
        {
            var xaxis = graphPane.XAxis;
            var yaxis = graphPane.YAxis;
            //draw scale number
            TextObj textObj = new TextObj(string.Format("{0}fach(DIN)", magnification), xaxis.Scale.Min, yaxis.Scale.Max + 2.0, CoordType.AxisXYScale, AlignH.Left, AlignV.Bottom);
            textObj.FontSpec.Border.IsVisible = false;
            graphPane.GraphObjList.Add((GraphObj)textObj);


            //draw hori scale ruler
            #region hori
            //draw 10micro scale ruler
            double v = graphPane.XAxis.Scale.MajorStep;
            ////text
            TextObj tenmicrometer = new TextObj(string.Format("{0}{1}", v.ToString("F0"), Properties.Resources.millimeter)
                , xaxis.Scale.Min + v * 5, yaxis.Scale.Max + 2.0, CoordType.AxisXYScale);
            tenmicrometer.FontSpec.Border.IsVisible = false;
            tenmicrometer.Location.AlignH = AlignH.Right;
            tenmicrometer.Location.AlignV = AlignV.Bottom;
            graphPane.GraphObjList.Add(tenmicrometer);

            //line1
            LineObj tenmim_line1 = new LineObj(
                xaxis.Scale.Min + v * 3,
                yaxis.Scale.Max + 3,
                xaxis.Scale.Min + v * 3,
                yaxis.Scale.Max + 6
                );
            //line2
            LineObj tenmim_line2 = new LineObj(
               xaxis.Scale.Min + v * 3,
                yaxis.Scale.Max + 4.5,
                xaxis.Scale.Min + v * 4,
                yaxis.Scale.Max + 4.5
                );
            //line3
            LineObj tenmim_line3 = new LineObj(
               xaxis.Scale.Min + v * 4,
                yaxis.Scale.Max + 3,
                xaxis.Scale.Min + v * 4,
                yaxis.Scale.Max + 6
                );
            graphPane.GraphObjList.Add(tenmim_line1);
            graphPane.GraphObjList.Add(tenmim_line2);
            graphPane.GraphObjList.Add(tenmim_line3);

            #endregion


            //draw vertical scale ruler
            #region vertical
            //draw 10micro scale ruler
            double vv = yaxis.Scale.MajorStep;
            ////text
            TextObj vertical_rule = new TextObj(string.Format("{0}{1}", vv * 1000 / magnification, Properties.Resources.micrometer)
                , xaxis.Scale.Min + v * 6 + 5, yaxis.Scale.Max + 2.0, CoordType.AxisXYScale);
            vertical_rule.FontSpec.Border.IsVisible = false;
            vertical_rule.Location.AlignH = AlignH.Left;
            vertical_rule.Location.AlignV = AlignV.Bottom;
            graphPane.GraphObjList.Add(vertical_rule);

            //line1
            LineObj verti_line1 = new LineObj(
                xaxis.Scale.Min + v * 6,
                yaxis.Scale.Max + 2,
                xaxis.Scale.Min + v * 6 + 3,
                yaxis.Scale.Max + 2
                );
            //line2
            LineObj verti_line2 = new LineObj(
               xaxis.Scale.Min + v * 6 + 1.5,
                yaxis.Scale.Max + 2,
                xaxis.Scale.Min + v * 6 + 1.5,
                yaxis.Scale.Max + 7
                );
            //line3
            LineObj verti_line3 = new LineObj(
               xaxis.Scale.Min + v * 6,
                yaxis.Scale.Max + 7,
                xaxis.Scale.Min + v * 6 + 3,
                yaxis.Scale.Max + 7
                );
            graphPane.GraphObjList.Add(verti_line1);
            graphPane.GraphObjList.Add(verti_line2);
            graphPane.GraphObjList.Add(verti_line3);

            #endregion
            //throw new NotImplementedException();

            //draw text mark on left-side with line
            //draw_line first
            //line start x,y coordinates
            double x_s = xaxis.Scale.Min;
            double y_s = 25;
            //line end x,y coordinates
            double x_end = x_s - xaxis.Scale.MajorStep / 2.0;
            double y_end = y_s;

            LineObj textmark_line1 = new LineObj(
                x_s,
                y_s,
                x_end,
                y_end
                );
            graphPane.GraphObjList.Add(textmark_line1);

            LineObj textmark_line2 = new LineObj(
                x_s,
                -y_s,
                x_end,
                -y_end
                );
            graphPane.GraphObjList.Add(textmark_line2);

            TextObj textmark1 = new TextObj(
                string.Format("{0}    ", pLine.Line1.identifier),
                x_end,
                y_end,
                CoordType.AxisXYScale
                );
            textmark1.Location.AlignV = AlignV.Center;
            textmark1.Location.AlignH = AlignH.Right;
            graphPane.GraphObjList.Add(textmark1);

            TextObj textmark2 = new TextObj(
              string.Format("{0}    ", pLine.Line2.identifier),
              x_end,
              -y_end,
              CoordType.AxisXYScale
              );
            textmark2.Location.AlignV = AlignV.Center;
            textmark2.Location.AlignH = AlignH.Right;
            graphPane.GraphObjList.Add(textmark2);


            //draw X axis range text
            double max = xaxis.Scale.Max;
            double min = xaxis.Scale.Min;

            TextObj text_x_min = new TextObj(
                string.Format("{0:F0}{1}", min, Properties.Resources.millimeter),
                min,
                yaxis.Scale.Min - 1,
                CoordType.AxisXYScale,
                AlignH.Left,
                AlignV.Top);

            TextObj text_x_max = new TextObj(
                string.Format("{0:F0}{1}", max, Properties.Resources.millimeter),
                max,
                yaxis.Scale.Min - 1,
                CoordType.AxisXYScale,
                AlignH.Right,
                AlignV.Top);
            text_x_min.FontSpec.Border.IsVisible = false;
            text_x_max.FontSpec.Border.IsVisible = false;


            graphPane.GraphObjList.Add(text_x_max);
            graphPane.GraphObjList.Add(text_x_min);

            //arrow below the X axis
            double arrow_start_x =
                (max + min) / 2.0 - xaxis.Scale.MajorStep / 2;
            double arrow_start_y = yaxis.Scale.Min - 4;
            double arrow_end_x =
                (max + min) / 2.0 + xaxis.Scale.MajorStep / 2;
            double arrow_end_y = yaxis.Scale.Min - 4;

            ArrowObj arrow_x_axis = new ArrowObj(
                arrow_start_x,
                arrow_start_y,
                arrow_end_x,
                arrow_end_y
                );
            graphPane.GraphObjList.Add(arrow_x_axis);

            string text_below_x = null;
            axisType axist = (axisType)Properties.Settings.Default.LineAxis;
            switch (axist)
            {
                case axisType.X:
                    text_below_x = "X";
                    break;
                case axisType.Y:
                    text_below_x = "Y";
                    break;
                case axisType.Z:
                    text_below_x = "Z";
                    break;
                default:
                    throw new NotImplementedException("Axis direction error");
            }
            TextObj text_arrow_below_x = new TextObj(
                text_below_x,
                arrow_end_x + 5,
                arrow_end_y,
                CoordType.AxisXYScale,
                AlignH.Left,
                AlignV.Center
                );
            text_arrow_below_x.FontSpec.Border.IsVisible = false;
            graphPane.GraphObjList.Add(text_arrow_below_x);

        }

        private void drawLinePoints(GraphPane graphPane)
        {
            double line1_aver = line1_point_z.Average();
            List<double> line1_z = line1_point_z.Select(n => (n - line1_aver)).ToList();
            PointPairList ppl1 = new PointPairList(line1_point_x.ToArray(), line1_z.ToArray());
            //outlier
            PointPairList ppl1_outlier = LineOutlier(ppl1);

            var ppl1_list = guessLines(ppl1_outlier);
            dealed_line1 = new List<PointPairList>();
            foreach (var l in ppl1_list)
            {
                PointPairList ppl1_filter = LineFilter(l);
                PointPairList ppl1_offset = LineOffset(ppl1_filter, 25);
                dealed_line1.Add(ppl1_filter);
                graphPane.AddCurve("", ppl1_offset, Color.Red, SymbolType.None);
            }

            double line2_aver = line2_point_z.Average();
            List<double> line2_z = line2_point_z.Select(n => (n - line2_aver)).ToList();
            PointPairList ppl2 = new PointPairList(line2_point_x.ToArray(), line2_z.ToArray());
            //outlier
            var ppl2_outlier = LineOutlier(ppl2);
            var tv1 = ppl1_outlier.Select(n => n.Y).Max();
            var tv2 = ppl1_outlier.Select(n => n.Y).Min();

            var ppl2_list = guessLines(ppl2_outlier);

            List<PointPair> tv3 = new List<PointPair>();

            dealed_line2 = new List<PointPairList>();
            foreach (var l in ppl2_list)
            {
                PointPairList ppl2_filter = LineFilter(l);
                PointPairList ppl2_offset = LineOffset(ppl2_filter, -25);
                dealed_line2.Add(ppl2_filter);
                tv3.AddRange(ppl2_filter);
                graphPane.AddCurve("", ppl2_offset, Color.Red, SymbolType.None);
            }
            var tv4 = tv3.Select(n => n.Y).ToList();
        }

        private PointPairList LineOffset(PointPairList ppl_filter, int offset)
        {
            var x = ppl_filter.Select(n => n.X).ToArray();
            var y = ppl_filter.Select(n => n.Y * Math.Sign(offset) * magnification + offset).ToArray();
            return new PointPairList(x, y);
        }

        private PointPairList LineFilter(PointPairList ppl_outlier)
        {
            ZegFilterLine zfl = new ZegFilterLine(ppl_outlier);
            var res = zfl.filterPoints(Properties.Settings.Default.LineFilterWaveLength);
            return res;
        }

        private PointPairList LineOutlier(PointPairList ppl)
        {
            double outlier_scale = 3;
            var st = new DescriptiveStatistics(ppl.Select(n => n.Y));
            var sigma = st.StandardDeviation;
            var mean = st.Mean;
            var max = mean + outlier_scale * sigma;
            var min = mean - outlier_scale * sigma;
            var res = ppl.Where(n => (n.Y < max && n.Y > min)).ToList();
            return new PointPairList(res.Select(n => n.X).ToArray(), res.Select(n => n.Y).ToArray());
        }

        private void drawRefLine(GraphPane mypane)
        {
            double y1 = 25;
            double x11 = line1_point_x.Min();
            double x12 = line1_point_x.Max();
            PointPair pointPair11 = new PointPair((x11 + x12) / 2.0, y1);
            PointPair pointPair12 = new PointPair(x11, y1 + (x11 - pointPair11.X) * line1_angle * magnification);
            PointPair pointPair13 = new PointPair(x12, y1 + (x12 - pointPair11.X) * line1_angle * magnification);
            mypane.AddCurve("", new PointPairList()
              {
                pointPair12,
                pointPair13
              }, Color.Black, SymbolType.None);

            double y2 = -25;
            double x21 = line2_point_x.Min();
            double x22 = line2_point_x.Max();
            PointPair pointPair21 = new PointPair((x21 + x22) / 2.0, y2);
            PointPair pointPair22 = new PointPair(x21, y2 - (x21 - pointPair21.X) * line2_angle * magnification);
            PointPair pointPair23 = new PointPair(x22, y2 - (x22 - pointPair21.X) * line2_angle * magnification);
            mypane.AddCurve("", new PointPairList()
              {
                pointPair22,
                pointPair23
              }, Color.Black, SymbolType.None);

        }
        protected override void drawLayout(GraphPane mypane)
        {
            base.drawLayout(mypane);
            mypane.Margin.Top = 50f;
            mypane.Margin.Bottom = 40f;
            XAxis xaxis = mypane.XAxis;
            YAxis yaxis = mypane.YAxis;
            yaxis.Scale.Max = 55;
            yaxis.Scale.Min = -55;
            yaxis.Scale.MajorStep = 5.0;
            yaxis.MajorTic.IsInside = true;
            yaxis.MajorTic.IsOutside = false;
            xaxis.Scale.Min = line1_point_x.Min();
            xaxis.Scale.Max = line1_point_x.Max();
            xaxis.Scale.MajorStep = (xaxis.Scale.Max - xaxis.Scale.Min) / 10.0;
            xaxis.MajorTic.IsOutside = false;

            xaxis.MajorTic.IsOpposite = true;
            yaxis.MajorTic.IsOpposite = true;

        }

        private List<PointPairList> guessLines(PointPairList line1_actual_points)
        {
            List<PointPairList> list = new List<PointPairList>();
            PointPairList pointPairList = new PointPairList();
            pointPairList.Add(line1_actual_points[0]);
            for (int index = 1; index < line1_actual_points.Count; ++index)
            {
                if (pointPairList.Count == 0)
                {
                    pointPairList.Add(line1_actual_points[index]);
                }
                else
                {
                    if (this.calcDis(Enumerable.Last<PointPair>((IEnumerable<PointPair>)pointPairList), line1_actual_points[index]) > 3.0)
                    {
                        list.Add(pointPairList);
                        pointPairList = new PointPairList();
                    }
                    pointPairList.Add(line1_actual_points[index]);
                }
            }
            if (pointPairList.Count > 2)
                list.Add(pointPairList);

            //remove some points for each section at itself beggining
            foreach (var l in list)
            {
                for (int i = 0; i < Properties.Settings.Default.LineMaskedPoints; ++i)
                {
                    l.RemoveAt(0);
                }
            }

            return list;
        }

        private double calcDis(PointPair last_p, PointPair pointPair)
        {
            return Math.Abs(last_p.X - pointPair.X);
        }



        public LineResults Result1
        {
            get
            {
                var x_1 = new List<double>();
                var y_1 = new List<double>();
                foreach (var l in dealed_line1)
                {
                    x_1.AddRange(l.Select(n => n.X));
                    y_1.AddRange(l.Select(n => n.Y));
                }

                List<PointPair> s = new PointPairList(x_1.ToArray(), y_1.ToArray());
                s = s.OrderBy(n => n.X).ToList();

                string name = pLine.Line1.identifier;


                //line1 y=ax+b;

                var s_line_dev = calcAsLineDeviation(s, line1_angle);
                var line1_devs = s_line_dev.Select(n => n.Y).ToList();
                double straightness = line1_devs.Max() - line1_devs.Min();


                //calc neigung per 100mm
                List<List<PointPair>> ss = new List<List<PointPair>>();
                double min = s_line_dev.Select(n => n.X).Min();
                double max = s_line_dev.Select(n => n.X).Max();
                int last_index = 0;
                for (int i = 1; ; ++i)
                {
                    if (min + 100 * i > max)
                    {
                        ss.Add(s_line_dev.GetRange(last_index, s_line_dev.Count - last_index));
                        break;
                    }
                    var item = s_line_dev.First(n => n.X > min + 100 * i);
                    int current_index = s_line_dev.IndexOf(item);
                    if (current_index > 0)
                    {
                        ss.Add(s_line_dev.GetRange(last_index, current_index - last_index));
                        last_index = current_index;
                    }
                    else
                    {
                        break;
                    }
                }
                var query = ss.Select(n => n.Select(o => o.Y)).ToList();
                var test = query.Select(n => n.Max() - n.Min()).ToArray();
                var test1 = query.Select(n => n.Count()).ToArray();
                double neigung = query.Select(n => n.Max() - n.Min()).Max();

                //parallel
                double parallel = y_1.Max() - y_1.Min();

                LineResults r1 = new LineResults()
                {
                    name = name,
                    Neigung = neigung * 1000,
                    straingness = straightness * 1000,
                    parallel = parallel * 1000
                };
                return r1;
            }

        }

        private List<PointPair> calcAsLineDeviation(List<PointPair> s, double angle)
        {
            double a = angle;
            double b = s[0].Y - a * s[0].X;

            var devs = s.Select(n => Math.Sqrt(1 - Math.Pow(a, 2)) * (n.Y - (a * n.X + b))).ToList();

            var test = s.Select(n => a * n.X + b).ToList();
            var test1 = s.Select(n => n.Y - (a * n.X + b)).ToList();
            var test2 = s.Select(n => n.Y).ToList();


            return (new PointPairList(s.Select(n => n.X).ToArray(), devs.ToArray())).ToList();
        }

        public LineResults Result2
        {
            get
            {
                var x_2 = new List<double>();
                var y_2 = new List<double>();
                foreach (var l in dealed_line2)
                {
                    x_2.AddRange(l.Select(n => n.X));
                    y_2.AddRange(l.Select(n => n.Y));
                }

                List<PointPair> s = new PointPairList(x_2.ToArray(), y_2.ToArray());
                s = s.OrderBy(n => n.X).ToList();

                string name = pLine.Line2.identifier;


                //line2 y=ax+b;

                var s_line_dev = calcAsLineDeviation(s, line2_angle);
                var line2_devs = s_line_dev.Select(n => n.Y).ToList();
                double straightness = line2_devs.Max() - line2_devs.Min();


                //calc neigung per 100mm
                List<List<PointPair>> ss = new List<List<PointPair>>();
                double min = s_line_dev.Select(n => n.X).Min();
                double max = s_line_dev.Select(n => n.X).Max();
                int last_index = 0;
                for (int i = 1; ; ++i)
                {
                    if (min + 100 * i > max)
                    {
                        ss.Add(s_line_dev.GetRange(last_index, s_line_dev.Count - last_index));
                        break;
                    }
                    var item = s_line_dev.First(n => n.X > min + 100 * i);
                    int current_index = s_line_dev.IndexOf(item);
                    if (current_index > 0)
                    {
                        ss.Add(s_line_dev.GetRange(last_index, current_index - last_index));
                        last_index = current_index;
                    }
                    else
                    {
                        break;
                    }
                }
                var query = ss.Select(n => n.Select(o => o.Y)).ToList();
                var test = query.Select(n => n.Max() - n.Min()).ToArray();
                var test2 = query.Select(n => n.Count()).ToArray();
                double neigung = query.Select(n => n.Max() - n.Min()).Max();

                //parallel
                double parallel = y_2.Max() - y_2.Min();

                LineResults r2 = new LineResults()
                {
                    name = name,
                    Neigung = neigung * 1000,
                    straingness = straightness * 1000,
                    parallel = parallel * 1000
                };
                return r2;
            }
        }
    }

    public class LineResults
    {
        /// <summary>
        /// name of line
        /// </summary>
        public string name
        {
            get;
            set;
        }
        /// <summary>
        /// straightness per 100 mm
        /// </summary>
        public double Neigung
        {
            get;
            set;
        }
        /// <summary>
        /// straightness
        /// </summary>
        public double straingness
        {
            get;
            set;
        }
        /// <summary>
        /// parallel
        /// </summary>
        public double parallel
        {
            get;
            set;
        }
    }
}
