using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using SPInterface;

namespace MogulReport
{
    public partial class ZedGraphDebuggerWindow : Form
    {
        /// <summary>
        /// define some parameters for protocol
        /// </summary>
        /// 
        double[] radius = new double[] { 30, 50, 70, 90, 110, 130 };
        double[] line_angles = new double[] 
        {
            Math.PI *0.75 + 1* Math.PI/18,
            Math.PI *0.75 + 2* Math.PI/18,
            Math.PI *0.75 + 3* Math.PI/18,
            Math.PI *0.75 + 4* Math.PI/18,
            Math.PI *0.75 + 5* Math.PI/18,
            Math.PI *0.75 + 6* Math.PI/18
        };
        double mag = 1000;

        ZedGraphControl zg = new ZedGraphControl();

        internal ZedGraphDebuggerWindow()
        {
            InitializeComponent();
            zg.Size = new Size(600, 540);
            this.Controls.Add(zg);
        }
        private List<Circle> circles;
        private List<CircleOffset> OffsetValues;
        int group_no;
        int circle_no;

        public Image GraphicOutput
        {
            get
            {
                return zg.GetImage();
            }
        }

        internal ZedGraphDebuggerWindow(List<Circle> list1, List<CircleOffset> list2, int group_index)
            : this()
        {
            circles = list1;
            OffsetValues = list2;
            group_no = group_index;
            circle_no = circles.Count;

            var mypane = zg.GraphPane;

            //clear all the exist data
            mypane.CurveList.Clear();

            formatPane(mypane);

            drawObjects(mypane);

            drawFitCircles(mypane);

            //deat with the points, filter, and draw them
            drawActualPoints(mypane);


            zg.AxisChange();
            //zedGraphControl1.Invalidate();
            zg.Refresh();

        }

        private void drawActualPoints(GraphPane mypane)
        {
            for (int index = 0; index < circle_no; ++index)
            {
                //offset values of center
                double x = OffsetValues[index].x * mag;
                double y = OffsetValues[index].y * mag;

                double theo_radius = radius[circle_no - 1 - index];

                List<PointPairList> circle_act_points = calcFilterPoints(circles[index], x, y, theo_radius);

                foreach (var curve in circle_act_points)
                {
                    LineItem circle_points = mypane.AddCurve("", curve, Color.Red, SymbolType.None);
                    circle_points.Line.Width = 0.7f;
                }

            }
        }
        /// <summary>
        /// filter and calc the points
        /// </summary>
        /// <param name="circle">Circle feature from Calypso, by SPInterface</param>
        /// <param name="x">offset vlaue x * maginification</param>
        /// <param name="y">offset vlaue y * maginification</param>
        /// <param name="theo_radius">radius</param>
        /// <returns></returns>
        private List<PointPairList> calcFilterPoints(Circle circle, double x, double y, double theo_radius)
        {
            ZegFilterCircle zfc = new ZegFilterCircle(circle);
            return zfc.filterPoints(50, x, y, theo_radius,mag);
        }
        /// <summary>
        /// draw fit circle exclude points
        /// center marks
        /// text marks (legend)
        /// </summary>
        /// <param name="mypane"></param>
        private void drawFitCircles(GraphPane mypane)
        {
            string[] names;
            switch (circle_no)
            {
                case 2:
                    names = new string[] {
                            String.Format("{0}{1}  ",group_no.ToString(),Properties.Resources.up),
                            String.Format("{0}{1}  ",group_no.ToString(),Properties.Resources.down)
                        };
                    break;
                case 3:
                    names = new string[] {
                            String.Format("{0}{1}  ",group_no.ToString(),Properties.Resources.up),
                            String.Format("{0}{1}  ",group_no.ToString(),Properties.Resources.middle),
                            String.Format("{0}{1}  ",group_no.ToString(),Properties.Resources.down)
                        };
                    break;
                default:
                    throw new NotImplementedException("section number of each position only 2 or 3");
            }

            //from ouside to inside
            for (int index = 0; index < circle_no; ++index)
            {
                //draw offset value (X mark)
                double x = OffsetValues[index].x * mag;
                double y = OffsetValues[index].y * mag;

                LineObj xmark_line1 = new LineObj(
                    x - 1.5,
                    y - 1.5,
                    x + 1.5,
                    y + 1.5
                    );
                LineObj xmark_line2 = new LineObj(
                   x - 1.5,
                   y + 1.5,
                   x + 1.5,
                   y - 1.5
                   );
                mypane.GraphObjList.Add(xmark_line1);
                mypane.GraphObjList.Add(xmark_line2);

                //draw ref circle
                double theo_radius = radius[circle_no - 1 - index];
                int point_no = 36;
                double step = 2 * Math.PI / point_no;
                PointPairList ref_circle = new PointPairList();
                for (int pi = 0; pi <= point_no; pi++)
                {
                    ref_circle.Add(
                        x + theo_radius * Math.Cos(pi * step),
                        y + theo_radius * Math.Sin(pi * step)
                        );
                }
                LineItem ref_circle_line = mypane.AddCurve("", ref_circle, Color.Blue, SymbolType.None);

                //draw text mark on left-side with line
                //draw_line first
                //line start x,y coordinates
                double x_s = x + theo_radius * Math.Cos(line_angles[index]);
                double y_s = y + theo_radius * Math.Sin(line_angles[index]);
                //line end x,y coordinates
                double x_end = mypane.XAxis.Scale.Min - 3;
                double y_end = y_s + Math.Tan(line_angles[index]) * (x_end - x_s);

                LineObj textmark_line1 = new LineObj(
                    x_s,
                    y_s,
                    x_end,
                    y_end
                    );
                mypane.GraphObjList.Add(textmark_line1);

                LineObj textmark_line2 = new LineObj(
                    x_end,
                    y_end,
                    x_end - 3,
                    y_end
                    );
                mypane.GraphObjList.Add(textmark_line2);

                TextObj textmark = new TextObj(
                    names[index],
                    x_end - 3,
                    y_end,
                    CoordType.AxisXYScale
                    );
                textmark.Location.AlignV = AlignV.Center;
                textmark.Location.AlignH = AlignH.Right;
                mypane.GraphObjList.Add(textmark);

            }
        }

        private void drawObjects(GraphPane mypane)
        {
            //draw inside circle
            PointPairList inside_ref_circle = new PointPairList();
            int point_no = 36;
            double step = (2 * Math.PI / point_no);
            for (int i = 0; i <= point_no; ++i)
            {
                inside_ref_circle.Add(
                    20 * Math.Cos(i * step),
                    20 * Math.Sin(i * step)
                    );
            }
            LineItem inside_ref_circle_line = mypane.AddCurve("", inside_ref_circle, Color.Black, SymbolType.None);
            inside_ref_circle_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Custom;
            inside_ref_circle_line.Line.Width = 1.0f;
            inside_ref_circle_line.Line.DashOff = 10;
            inside_ref_circle_line.Line.DashOn = 10;

            //draw outside circle
            double outside_circle_radius = radius[circle_no] - 10;
            PointPairList outside_ref_circle = new PointPairList();
            for (int i = 0; i <= point_no; ++i)
            {
                outside_ref_circle.Add(
                    outside_circle_radius * Math.Cos(i * step),
                    outside_circle_radius * Math.Sin(i * step)
                    );
            }
            LineItem outside_ref_circle_line = mypane.AddCurve("", outside_ref_circle, Color.Black, SymbolType.None);
            outside_ref_circle_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Custom;
            outside_ref_circle_line.Line.Width = 1.0f;
            outside_ref_circle_line.Line.DashOff = 10;
            outside_ref_circle_line.Line.DashOn = 10;

            //draw 10micro scale ruler
            //text
            TextObj tenmicrometer = new TextObj(string.Format("10{0}", Properties.Resources.micrometer)
                , outside_circle_radius - 20, outside_circle_radius - 5, CoordType.AxisXYScale);
            tenmicrometer.FontSpec.Border.IsVisible = false;
            tenmicrometer.Location.AlignH = AlignH.Left;
            tenmicrometer.Location.AlignV = AlignV.Center;
            mypane.GraphObjList.Add(tenmicrometer);

            //line1
            LineObj tenmim_line1 = new LineObj(
                outside_circle_radius - 10,
                outside_circle_radius - 10,
                outside_circle_radius - 10,
                outside_circle_radius - 8
                );
            //line2
            LineObj tenmim_line2 = new LineObj(
                outside_circle_radius - 20,
                outside_circle_radius - 10,
                outside_circle_radius - 20,
                outside_circle_radius - 8
                );
            //line3
            LineObj tenmim_line3 = new LineObj(
               outside_circle_radius - 10,
               outside_circle_radius - 9,
               outside_circle_radius - 20,
               outside_circle_radius - 9
               );
            mypane.GraphObjList.Add(tenmim_line1);
            mypane.GraphObjList.Add(tenmim_line2);
            mypane.GraphObjList.Add(tenmim_line3);

            //draw text in left=top corner 1000 fach DIN
            TextObj left_top_text1 = new TextObj(
                "1000 fach",
                -outside_circle_radius + 3,
                outside_circle_radius - 5,
                CoordType.AxisXYScale
                );
            left_top_text1.FontSpec.Border.IsVisible = false;
            left_top_text1.Location.AlignV = AlignV.Top;
            left_top_text1.Location.AlignH = AlignH.Left;

            TextObj left_top_text2 = new TextObj(
                "(DIN)",
                -outside_circle_radius + 3,
                outside_circle_radius - 10,
                CoordType.AxisXYScale
                );
            left_top_text2.FontSpec.Border.IsVisible = false;
            left_top_text2.Location.AlignV = AlignV.Top;
            left_top_text2.Location.AlignH = AlignH.Left;

            mypane.GraphObjList.Add(left_top_text1);
            mypane.GraphObjList.Add(left_top_text2);

            //draw degree labels
            TextObj degree0 = new TextObj(
                string.Format("0{0}", Properties.Resources.degree),
                18,
                2,
                CoordType.AxisXYScale
                );
            degree0.Location.AlignH = AlignH.Right;
            degree0.Location.AlignV = AlignV.Bottom;
            degree0.FontSpec.Border.IsVisible = false;
            mypane.GraphObjList.Add(degree0);

            TextObj degree90 = new TextObj(
               string.Format("90{0}", Properties.Resources.degree),
               -2,
               18,
               CoordType.AxisXYScale
               );
            degree90.Location.AlignH = AlignH.Right;
            degree90.Location.AlignV = AlignV.Top;
            degree90.FontSpec.Border.IsVisible = false;
            mypane.GraphObjList.Add(degree90);

            TextObj degree180 = new TextObj(
                           string.Format("180{0}", Properties.Resources.degree),
                           -18,
                           -2,
                           CoordType.AxisXYScale
                           );
            degree180.Location.AlignH = AlignH.Left;
            degree180.Location.AlignV = AlignV.Top;
            degree180.FontSpec.Border.IsVisible = false;
            mypane.GraphObjList.Add(degree180);

            TextObj degree270 = new TextObj(
                           string.Format("270{0}", Properties.Resources.degree),
                           2,
                           -18,
                           CoordType.AxisXYScale
                           );
            degree270.Location.AlignH = AlignH.Left;
            degree270.Location.AlignV = AlignV.Bottom;
            degree270.FontSpec.Border.IsVisible = false;
            mypane.GraphObjList.Add(degree270);

            zg.Refresh();
        }

        private void formatPane(GraphPane mypane)
        {
            mypane.Margin.Top = 20;
            mypane.Margin.Bottom = 20;
            mypane.Margin.Left = 90;
            mypane.Margin.Right = 10;
            mypane.TitleGap = 0;
            mypane.Title.IsVisible = false;
            mypane.Legend.IsVisible = false;

            var xaxis = mypane.XAxis;
            var yaxis = mypane.YAxis;

            xaxis.Cross = 0;

            xaxis.Scale.Min = -(radius[circle_no] - 10);
            xaxis.Scale.Max = (radius[circle_no] - 10);
            yaxis.Scale.Min = -(radius[circle_no] - 10);
            yaxis.Scale.Max = (radius[circle_no] - 10);

            yaxis.Cross = 0;
            mypane.Title.Text = "";
            xaxis.Title.Text = "";
            yaxis.Title.Text = "";

            xaxis.Scale.MajorStep = 20;
            xaxis.Scale.MinorStep = 10;
            yaxis.Scale.MajorStep = 20;
            yaxis.Scale.MinorStep = 10;

            //xaxis.Scale.IsVisible = false;
            //xaxis.MajorGrid.IsZeroLine = true;
            //yaxis.MajorGrid.IsZeroLine = true;
            xaxis.Scale.IsVisible = false;
            yaxis.Scale.IsVisible = false;

            xaxis.MajorTic.IsOpposite = false;
            xaxis.MinorTic.IsOpposite = false;

            yaxis.MajorTic.IsOpposite = false;
            yaxis.MinorTic.IsOpposite = false;



            //xaxis.MajorTic.IsAllTics = false;
            //yaxis.MajorTic.IsAllTics = false;

            //xaxis.MinorTic.IsAllTics = false;
            //yaxis.MinorTic.IsAllTics = false;  

            //xaxis.IsVisible = false;
            //yaxis.IsVisible = false;
        }


    }
}
