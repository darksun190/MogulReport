using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPInterface;
using System.Diagnostics;

namespace MogulReport
{
    class CrankshaftCirclesProtocol : Protocol
    {
        private List<Circle> circles = new List<Circle>();
        public int groups
        {
            get;
            set;
        }
        ReferenceLine rLine
        {
            get; set;
        }


        List<CircleOffset> OffsetValues
        {
            get;
            set;
        }
        public CrankshaftCirclesProtocol(List<Feature> eles) 
        {
            foreach (var p in eles)
            {
                circles.Add((Circle)p);
            }

            //check how many groups(pages of protocol) of circles
            groups = guessGroups(circles.Count);
            int no_each_group = circles.Count / groups;

            var selectEva = new SelectEvaluationMethod();
            var ans = selectEva.ShowDialog();
            if(ans == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.CircleEvaluationMethod = "MIC";
            }
            else
            {
                Properties.Settings.Default.CircleEvaluationMethod = "LSC";
            }
            Properties.Settings.Default.Save();

            //calculate the reference line
            rLine = new ReferenceLine(circles.Last(), circles.First());

            OffsetValues = new List<CircleOffset>();
            foreach (Circle c in circles)
            {
                OffsetValues.Add(rLine.getDev(c));
            }
            for (int group_no = 0; group_no < groups; ++group_no)
            {
                MogulCircleProtocolPage Mcpp = new MogulCircleProtocolPage(
                    group_no+1, 
                    circles.GetRange(group_no * no_each_group, no_each_group), 
                    OffsetValues.GetRange(group_no * no_each_group, no_each_group)
                    );
                Debug.WriteLine(Mcpp.TotalHeight.ToString());
                protocolPages.Add(Mcpp);
            }
            
        }

        private int guessGroups(int count)
        {
            int result = 0;
            switch (count)
            {
                case 5:
                case 10:
                case 15:
                    result = 5;
                    break;
                case 7:
                    result = 7;
                    break;
                case 3:
                case 6:
                case 9:
                    result = 3;
                    break;
                case 4:
                case 8:
                    result = 4;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }

        /// <summary>
        /// only use in crankshaft circle
        /// </summary>

    }
    internal class CircleOffset
    {

        public CircleOffset(double v1, double v2)
        {
            x = v1;
            y = v2;
        }

        public double x { get; set; }
        public double y { get; set; }
    }
}
