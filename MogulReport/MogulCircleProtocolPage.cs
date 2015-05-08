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
        PdfPTable header = new PdfPTable(2);
        PdfPTable graphic = new PdfPTable(1);
        PdfPTable datas = new PdfPTable(1);
        public MogulCircleProtocolPage(int group_no, List<Circle> circles, List<CircleOffset> offsetvalues) : base()
        {

            //header part
            Image logo = Image.GetInstance(Properties.Resources.logo, BaseColor.WHITE);
            header.SetWidths(new int[] { 3, 10 });

            PdfPCell headerCell1 = new PdfPCell(logo);
            headerCell1.FixedHeight = 100f;

            headerCell1.HorizontalAlignment = Element.ALIGN_CENTER;
            header.AddCell(headerCell1);

            PdfPCell headerCell2 = new PdfPCell(new Paragraph(Properties.Resources.HeaderString1));
            headerCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
            headerCell2.PaddingBottom = 10f;
            headerCell2.PaddingLeft = 20f;
            header.AddCell(headerCell2);

            //graphic part

            var form = new ZedGraphDebuggerWindow(circles, offsetvalues);

            if (Debugger.IsAttached)
            {
                // form.ShowDialog();
            }
            Image roundness = Image.GetInstance(form.GraphicOutput, BaseColor.WHITE);
            //roundness.ScaleAbsolute(550, 200);
            
            PdfPCell protocol_pic = new PdfPCell(roundness);
            protocol_pic.FixedHeight = 500f;
            protocol_pic.HorizontalAlignment = Element.ALIGN_RIGHT;
            graphic.AddCell(protocol_pic);


            //data part
            PdfPTable datas_header = new PdfPTable(2);
            datas_header.SetWidths(new int[] { 3, 5 });
            PdfPCell cell;
            string dtheader1 = string.Format("{0}{1}{2}",
                Properties.Resources.DataTableHeader1,
                Properties.Resources.DataTableCircle,
                group_no.ToString()
                );
            cell = new PdfPCell(new Paragraph(dtheader1));
            datas_header.AddCell(cell);
            string dtheader2 = string.Format("{0}{1}\n{2}{3}",
             Properties.Resources.DataTableHeader2,
             Properties.Resources.DataTableRef,
             Properties.Resources.DataTableHeader3,
             Properties.Resources.DataTableAxis
             );
            cell = new PdfPCell(new Paragraph(dtheader2));
            datas_header.AddCell(cell);
            datas.AddCell(datas_header);
            //column names
            datas.AddCell(MogulCircleProtocolPageDataTableItem.ColumnName);
            datas.AddCell(MogulCircleProtocolPageDataTableItem.getInstance(group_no, circles, offsetvalues));

         

            this.AddCell(header);

            this.AddCell(graphic);
            this.AddCell(datas);
        }

        class MogulCircleProtocolPageDataTableItem : PdfPTable
        {

            internal static PdfPTable ColumnName
            {
                get
                {
                    Font fontBold = new Font(Font.FontFamily.UNDEFINED,9, Font.BOLD, BaseColor.BLACK);//FontFactory.GetFont("Arial", 5);
                    Font fontNormal = new Font(Font.FontFamily.UNDEFINED, 9, Font.NORMAL, BaseColor.BLACK);
                    PdfPTable cName = new PdfPTable(8);
                    string col_name1 = string.Format(Properties.Resources.DataTableMessPt);
                    string col_name2 = string.Format(Properties.Resources.DataTableFilter);
                    string col_name3 = string.Format(Properties.Resources.DataTablePosZ);
                    string col_name4 = string.Format(Properties.Resources.DatatableEvaluation);
                    string col_name5 = string.Format(Properties.Resources.DataTableExcentX);
                    string col_name6 = string.Format(Properties.Resources.DataTableExcentY);
                    string col_name7 = string.Format(Properties.Resources.DataTableRoundness);
                    string col_name8 = string.Format(Properties.Resources.DataTableCoaxial);
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
                for(int i=0;i<list1.Count;++i)
                {
                    res.AddCell(new PdfPCell(new Paragraph(names[i])));
                    res.AddCell(new PdfPCell(new Paragraph("0-50")));
                    res.AddCell(new PdfPCell(new Paragraph(list1[i].z.ToString("F3"))));
                    res.AddCell(new PdfPCell(new Paragraph("LSC")));
                    res.AddCell(new PdfPCell(new Paragraph((list2[i].x*1000).ToString("F2"))));
                    res.AddCell(new PdfPCell(new Paragraph((list2[i].y * 1000).ToString("F2"))));
                    double roundness = list1[i].maxDev() - list1[i].minDev();

                    res.AddCell(new PdfPCell(new Paragraph((roundness*1000).ToString("F2"))));
                    double coax = Math.Sqrt(
                        Math.Pow(list2[i].x, 2) +
                        Math.Pow(list2[i].y, 2)
                        );
                    res.AddCell(new PdfPCell(new Paragraph((coax*1000).ToString("F2"))));


                }
                return res;
            }

        }
    }
}
