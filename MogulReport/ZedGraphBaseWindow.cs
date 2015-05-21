using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace MogulReport
{
    public partial class ZedGraphBaseWindow : Form
    {
        protected ZedGraphControl zg = new ZedGraphControl();
        public ZedGraphBaseWindow()
        {
            InitializeComponent();
            this.Controls.Add(zg);
            this.Height = 500;
            this.Width = 700;
        }
        public Image GraphicOutput
        {
            get
            {
                return zg.GraphPane.GetImage();
            }
        }
        protected virtual void drawLayout(GraphPane mypane)
        {
            mypane.TitleGap = 0;
            mypane.Title.IsVisible = false;
            mypane.Legend.IsVisible = false;
            mypane.Border.IsVisible = false;
            XAxis xaxis = mypane.XAxis;
            xaxis.Title.Text = "";
            xaxis.Scale.IsVisible = false;
            xaxis.MinorTic.IsAllTics = false;
            YAxis yaxis = mypane.YAxis;
            yaxis.Title.Text = "";
            yaxis.Scale.IsVisible = false;
            yaxis.MinorTic.IsAllTics = false;


            xaxis.MajorTic.IsOpposite = false;
            xaxis.MinorTic.IsOpposite = false;

            yaxis.MajorTic.IsOpposite = false;
            yaxis.MinorTic.IsOpposite = false;
        }


     

    }
}
