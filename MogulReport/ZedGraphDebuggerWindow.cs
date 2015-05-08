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
        double[] diameters = new double[] { 30, 50, 70, 90, 110, 130 };


        ZedGraphControl zg = new ZedGraphControl();

        internal ZedGraphDebuggerWindow()
        {
            InitializeComponent();
            var v = Math.Min(Width, Height);
            zg.Size = new Size(v, v);
            this.Controls.Add(zg);
        }
        private List<Circle> circles;
        private List<CircleOffset> OffsetValues;
        public Image GraphicOutput
        {
            get
            {
                return zg.GetImage();
            }
        }

        internal ZedGraphDebuggerWindow(List<Circle> list1, List<CircleOffset> list2) : this()
        {
            circles = list1;
            OffsetValues = list2;
            var mypane = zg.GraphPane;
            //clear all the exist data
            mypane.CurveList.Clear();

            for (int index = 0; index < circles.Count; ++index)
            {
                double _actual_radius = circles[index].r;
                var dev = circles[index].Deviations;
                double _p_radius = Math.Round(dev.Average(), 4);
                double _scaling = Math.Floor(_actual_radius / 3.0 / (dev.Max() - dev.Min()));
                var corrected = new List<double>();
                var filtered = new List<double>();
                //fft = new List<double>();
                corrected.Clear();
                foreach (var p in dev)
                {
                    corrected.Add(p - _p_radius);
                }
                #region copy from other
              

                int size = corrected.Count;

                PointPairList actual = new PointPairList();
                PointPairList nominal = new PointPairList();
                PointPairList original = new PointPairList();

                double step = Math.PI * 2 / size;
                for (int i = 0; i < size; i++)
                {
                    //actual.Add(
                    //    (_actual_radius + filtered[i] * _scaling) * Math.Cos(i * step),
                    //    (_actual_radius + filtered[i] * _scaling) * Math.Sin(i * step)
                    //    );
                    nominal.Add(
                        (_actual_radius) * Math.Cos(i * step),
                        (_actual_radius) * Math.Sin(i * step)
                        );
                    original.Add(
                        (_actual_radius + corrected[i] * _scaling) * Math.Cos(i * step) + OffsetValues[index].x * _scaling,
                        (_actual_radius + corrected[i] * _scaling) * Math.Sin(i * step) + OffsetValues[index].y * _scaling
                        );
                    //list.Add(i, points[i]);
                }


                // LineItem actual_line = mypane.AddCurve("", actual, Color.Blue, SymbolType.None);
                LineItem nominal_line = mypane.AddCurve("", nominal, Color.Black, SymbolType.None);
                LineItem original_line = mypane.AddCurve("", original, Color.DarkGray, SymbolType.None);
                // actual_line.Line.Width = 3;
                nominal_line.Line.Width = 3;
                original_line.Line.Width = 1;
                zg.AxisChange();
                //zedGraphControl1.Invalidate();
                zg.Refresh();
                #endregion
            }
        }

        private void ZedGraphDebuggerWindow_SizeChanged(object sender, EventArgs e)
        {
            int v = Math.Min(Width, Height);
            zg.Size = new Size(v, v);
        }
    }
}
