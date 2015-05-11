using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using SPInterface;
using System.Diagnostics;
using iTextSharp.text;

namespace MogulReport
{
    /// <summary>
    /// circles protocol always had 3 parts
    /// </summary>
    class MogulCircleProtocolPage : ProtocolPage
    {
        List<Circle> circles;
        List<CircleOffset> offsets;
        int index;
        protected override PdfPTable Graphic
        {
            get
            {
                //graphic part
                var graphic = base.Graphic;

                var form = new ZedGraphDebuggerWindow(circles, offsets,index);

                if (Debugger.IsAttached)
                {
                    //form.ShowDialog();
                }
                Image roundness = Image.GetInstance(form.GraphicOutput, BaseColor.WHITE);
                Debug.WriteLine("{0} ; {1}", roundness.Width, roundness.Height);
                //roundness.ScaleAbsolute(550, 200);

                PdfPCell protocol_pic = new PdfPCell(roundness,true);
               // protocol_pic.FixedHeight = 440;
                protocol_pic.HorizontalAlignment = Element.ALIGN_MIDDLE;
                graphic.AddCell(protocol_pic);

                return graphic;
            }
        }
        protected override PdfPTable Datas
        {
            get
            {
                var datas = base.Datas;
                //data part
                PdfPTable datas_header = new PdfPTable(2);
                datas_header.SetWidths(new int[] { 23, 37 });

                Font fontBold = new Font(Font.FontFamily.UNDEFINED, 9, Font.BOLD, BaseColor.BLACK);//FontFactory.GetFont("Arial", 5);
                Font fontNormal = new Font(Font.FontFamily.UNDEFINED, 9, Font.NORMAL, BaseColor.BLACK);
                
                datas.DefaultCell.Padding = 0;

                PdfPTable dtheader1 = new PdfPTable(1);
                dtheader1.DefaultCell.BorderWidth = 0;
 
                dtheader1.AddCell(new Phrase(Properties.Resources.MultiCircleHeader, fontBold));
                Phrase dth1cell2 = new Phrase();

                dth1cell2.Add(new Phrase(Properties.Resources.Name, fontBold));
                dth1cell2.Add(new Phrase(string.Format(": {0}{1}", Properties.Resources.Drilling, index.ToString()), fontNormal));
                dtheader1.AddCell(dth1cell2);

                datas_header.AddCell(dtheader1);
                PdfPTable dtheader2 = new PdfPTable(2);
                dtheader2.DefaultCell.BorderWidth = 0;

                dtheader2.SetWidths(new int[] { 10, 27 });
                 //Properties.Resources.Datum,
                 //Properties.Resources.Reference,
                 //Properties.Resources.Reference,
                 //Properties.Resources.Axis
                dtheader2.AddCell(new Phrase(Properties.Resources.Datum, fontBold));
                dtheader2.AddCell(new Phrase(string.Format(": {0}",Properties.Resources.Reference),fontNormal));
                dtheader2.AddCell(new Phrase(Properties.Resources.Reference, fontBold));
                dtheader2.AddCell(new Phrase(string.Format(": {0}",Properties.Resources.Axis),fontNormal));


                datas_header.AddCell(dtheader2);
                datas.AddCell(datas_header);
                //column names
                datas.AddCell(MogulCircleProtocolPageDataTableItem.ColumnName);
                datas.AddCell(MogulCircleProtocolPageDataTableItem.getInstance(index, circles, offsets));

                return datas;
            }
        }
        public MogulCircleProtocolPage(int group_no, List<Circle> circles, List<CircleOffset> offsetvalues)
            : base()
        {

            this.circles = circles;

            this.offsets = offsetvalues;
            index = group_no;



            this.AddCell(Header);

            this.AddCell(Graphic);
            this.AddCell(Datas);
        }

        class MogulCircleProtocolPageDataTableItem : PdfPTable
        {

            internal static PdfPTable ColumnName
            {
                get
                {
                    Font fontBold = new Font(Font.FontFamily.UNDEFINED, 9, Font.BOLD, BaseColor.BLACK);//FontFactory.GetFont("Arial", 5);
                    Font fontNormal = new Font(Font.FontFamily.UNDEFINED, 9, Font.NORMAL, BaseColor.BLACK);
                    PdfPTable cName = new PdfPTable(8);
                    cName.SetWidths(new int[] { 9, 6, 8, 6, 7, 7, 7, 10 });

                    string col_name1 = string.Format(Properties.Resources.MeasuringPoint);
                    string col_name2 = string.Format(Properties.Resources.Filter);
                    string col_name3 = string.Format(Properties.Resources.PosZ);
                    string col_name4 = string.Format(Properties.Resources.Evaluation);
                    string col_name5 = string.Format(Properties.Resources.ExcentX);
                    string col_name6 = string.Format(Properties.Resources.ExcentY);
                    string col_name7 = string.Format(Properties.Resources.Roundness);
                    string col_name8 = string.Format(Properties.Resources.Coaxial);
                    Phrase column_header_cell1 = new Phrase(col_name1, fontBold);
                    Phrase unit_upr = new Phrase("\n[upr]", fontNormal);
                    Phrase unit_um = new Phrase("\n[µm]", fontNormal);
                    Phrase unit_mm = new Phrase("\n[mm]", fontNormal);
                    Phrase column_header_cell2 = new Phrase();
                    column_header_cell2.Add(new Phrase(col_name2, fontBold));
                    column_header_cell2.Add(unit_upr);

                    Phrase column_header_cell3 = new Phrase();
                    column_header_cell3.Add(new Phrase(col_name3, fontBold));
                    column_header_cell3.Add(unit_mm);

                    Phrase column_header_cell4 = new Phrase(col_name4, fontBold);
                    Phrase column_header_cell5 = new Phrase();
                    column_header_cell5.Add(new Phrase(col_name5, fontBold));
                    column_header_cell5.Add(unit_um);
                    Phrase column_header_cell6 = new Phrase();
                    column_header_cell6.Add(new Phrase(col_name6, fontBold));
                    column_header_cell6.Add(unit_um);
                    Phrase column_header_cell7 = new Phrase();
                    column_header_cell7.Add(new Phrase(col_name7, fontBold));
                    column_header_cell7.Add(unit_um);
                    Phrase column_header_cell8 = new Phrase();
                    column_header_cell8.Add(new Phrase(col_name8, fontBold));
                    column_header_cell8.Add(unit_um);

                    cName.AddCell(column_header_cell1);
                    cName.AddCell(column_header_cell2);
                    cName.AddCell(column_header_cell3);
                    cName.AddCell(column_header_cell4);
                    cName.AddCell(column_header_cell5);
                    cName.AddCell(column_header_cell6);
                    cName.AddCell(column_header_cell7);
                    cName.AddCell(column_header_cell8);

                    return cName;
                }
            }
            internal static PdfPTable getInstance(int group_no, List<Circle> list1, List<CircleOffset> list2)
            {
                PdfPTable res = new PdfPTable(8);
                res.SetWidths(new int[] { 9, 6, 8, 6, 7, 7, 7, 10 });
                string[] names;
                switch (list1.Count)
                {
                    case 2:
                        names = new string[] {
                            String.Format("{0}{1}",group_no.ToString(),Properties.Resources.up),
                            String.Format("{0}{1}",group_no.ToString(),Properties.Resources.down)
                        };
                        break;
                    case 3:
                        names = new string[] {
                            String.Format("{0}{1}",group_no.ToString(),Properties.Resources.up),
                            String.Format("{0}{1}",group_no.ToString(),Properties.Resources.middle),
                            String.Format("{0}{1}",group_no.ToString(),Properties.Resources.down)
                        };
                        break;
                    default:
                        throw new NotImplementedException("section number of each position only 2 or 3");
                }
                for (int i = 0; i < list1.Count; ++i)
                {
                    res.AddCell(new PdfPCell(new Paragraph(names[i])));
                    res.AddCell(new PdfPCell(new Paragraph("0-50")));
                    res.AddCell(new PdfPCell(new Paragraph(list1[i].z.ToString("F3"))));
                    res.AddCell(new PdfPCell(new Paragraph("LSC")));
                    res.AddCell(new PdfPCell(new Paragraph((list2[i].x * 1000).ToString("F2"))));
                    res.AddCell(new PdfPCell(new Paragraph((list2[i].y * 1000).ToString("F2"))));
                    double roundness = list1[i].maxDev() - list1[i].minDev();

                    res.AddCell(new PdfPCell(new Paragraph((roundness * 1000).ToString("F2"))));
                    double coax = Math.Sqrt(
                        Math.Pow(list2[i].x, 2) +
                        Math.Pow(list2[i].y, 2)
                        );
                    res.AddCell(new PdfPCell(new Paragraph((coax * 1000).ToString("F2"))));


                }
                return res;
            }

        }
    }
}
