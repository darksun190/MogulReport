using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;

namespace MogulReport
{
    class MogulLineProtocolPage : ProtocolPage
    {
        private PairedLine pLine;
        LineResults line_result1;
        LineResults line_result2;

        protected override PdfPTable Graphic
        {
            get
            {
                var graphic = base.Graphic;
                graphic.DefaultCell.Padding = 0.0f;
                graphic.DefaultCell.Border = 0;
                ZedGraphLineWindow graphLineWindows = new ZedGraphLineWindow(pLine);
                if (Debugger.IsAttached)
                {
                    //graphLineWindows.ShowDialog();
                }

                line_result1 = graphLineWindows.Result1;
                line_result2 = graphLineWindows.Result2;

                PdfPCell cell = new PdfPCell(Image.GetInstance(graphLineWindows.GraphicOutput, BaseColor.WHITE), true);
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                graphic.AddCell(cell);

                return graphic;
            }
        }

        protected override PdfPTable Datas
        {
            get
            {
                var datas = base.Datas;
                datas.DefaultCell.Border = 0;
                datas.DefaultCell.Padding = 0.0f;
                iTextSharp.text.Font font1 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 11f, 1, BaseColor.BLACK);
                iTextSharp.text.Font font2 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 11f, 0, BaseColor.BLACK);
                PdfPTable table1 = new PdfPTable(2);
                table1.SetWidths(new int[2]
        {
          67,
          104
        });
                PdfPTable table2 = new PdfPTable(1);
                table2.DefaultCell.Border = 0;
                table2.AddCell(new Phrase(Properties.Resources.MultiLineHeader, font1));
                table2.AddCell(new Phrase()
        {
          (IElement) new Phrase(string.Format("{0}", Properties.Resources.Name), font1),
          (IElement) new Phrase(string.Format(":{0}{1}", Properties.Resources.Linear, pLine.group_no.ToString()), font2)
        });
                table1.AddCell(table2);
                PdfPTable table3 = new PdfPTable(2);
                table3.DefaultCell.Border = 0;
                table3.SetWidths(new int[2]
        {
          3,
          7
        });
                table3.AddCell(new Phrase(string.Format("{0}", Properties.Resources.Datum), font1));
                table3.AddCell(new Phrase(string.Format(":{0}",  Properties.Resources.Reference), font2));
                table3.AddCell(new Phrase(string.Format("{0}",  Properties.Resources.Reference), font1));
                table3.AddCell(new Phrase(string.Format(":{0}",  Properties.Resources.Axis), font2));
                table1.AddCell(table3);
                datas.AddCell(table1);
                datas.AddCell(MogulLineProtocolPage.MogulLineProtocolPageDataTableItem.ColumnName);
                datas.AddCell(MogulLineProtocolPage.MogulLineProtocolPageDataTableItem.getInstance(pLine,line_result1,line_result2));
                return datas;
            }
        }

        public MogulLineProtocolPage(PairedLine pl)
        {
            pLine = pl;
            AddCell(Header);
            AddCell(Graphic);
            AddCell(Datas);
        }

        private class MogulLineProtocolPageDataTableItem : PdfPTable
        {
            private static int[] dataColWidth = new int[7]
      {
        27,
        17,
        23,
        17,
        29,
        29,
        31
      };

            internal static PdfPTable ColumnName
            {
                get
                {
                    iTextSharp.text.Font font1 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 9f, 1, BaseColor.BLACK);
                    iTextSharp.text.Font font2 = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.UNDEFINED, 9f, 0, BaseColor.BLACK);
                    PdfPTable pdfPtable = new PdfPTable(7);
                    pdfPtable.SetWidths(MogulLineProtocolPage.MogulLineProtocolPageDataTableItem.dataColWidth);
                    pdfPtable.DefaultCell.Border = 12;
                    string str1 = string.Format(Properties.Resources.MeasuringPoint);
                    string str2 = string.Format(Properties.Resources.Filter);
                    string str3 = string.Format(Properties.Resources.PosC);
                    string str4 = string.Format(Properties.Resources.Evaluation);
                    string str5 = string.Format(Properties.Resources.Slope);
                    string str6 = string.Format(Properties.Resources.Straightness);
                    string str7 = string.Format(Properties.Resources.Parallel);
                    Phrase phrase1 = new Phrase(str1, font1);
                    Phrase phrase2 = new Phrase("\n[upr]", font2);
                    Phrase phrase3 = new Phrase("\n[µm]", font2);
                    Phrase phrase4 = new Phrase("\n[mm]", font2);
                    Phrase phrase5 = new Phrase("\n[µm/100mm]", font2);
                    Phrase phrase6 = new Phrase("°", font2);
                    Phrase phrase7 = new Phrase();
                    phrase7.Add((IElement)new Phrase(str2, font1));
                    phrase7.Add((IElement)phrase4);
                    Phrase phrase8 = new Phrase();
                    phrase8.Add((IElement)new Phrase(str3, font1));
                    phrase8.Add((IElement)phrase6);
                    Phrase phrase9 = new Phrase(str4, font1);
                    Phrase phrase10 = new Phrase();
                    phrase10.Add((IElement)new Phrase(str5, font1));
                    phrase10.Add((IElement)phrase5);
                    Phrase phrase11 = new Phrase();
                    phrase11.Add((IElement)new Phrase(str6, font1));
                    phrase11.Add((IElement)phrase3);
                    Phrase phrase12 = new Phrase();
                    phrase12.Add((IElement)new Phrase(str7, font1));
                    phrase12.Add((IElement)phrase3);
                    pdfPtable.AddCell(phrase1);
                    pdfPtable.AddCell(phrase7);
                    pdfPtable.AddCell(phrase8);
                    pdfPtable.AddCell(phrase9);
                    pdfPtable.AddCell(phrase10);
                    pdfPtable.AddCell(phrase11);
                    pdfPtable.AddCell(phrase12);
                    return pdfPtable;
                }
            }

            internal static PdfPTable getInstance(PairedLine pLine, LineResults lr1, LineResults lr2)
            {
                PdfPTable pdfPtable = new PdfPTable(7);
                pdfPtable.SetWidths(MogulLineProtocolPage.MogulLineProtocolPageDataTableItem.dataColWidth);
                string[] strArray = new string[2]
                {
                  lr1.name,
                  lr2.name
                };
                string wavelength = Properties.Settings.Default.LineFilterWaveLength.ToString("F2");
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(strArray[0])));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(wavelength)));
                double num1 = 180.0 * (pLine.angle1 / Math.PI);
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(num1.ToString("F3"))));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph("LSS")));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(lr1.Neigung.ToString("F2"))));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(lr1.straingness.ToString("F2"))));
                pdfPtable.AddCell(lr1.parallel.ToString("F2"));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(strArray[1])));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(wavelength)));
                double num2 = 180.0 * (pLine.angle2 / Math.PI);
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(num2.ToString("F3"))));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph("LSS")));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(lr2.Neigung.ToString("F2"))));
                pdfPtable.AddCell(new PdfPCell((Phrase)new Paragraph(lr2.straingness.ToString("F2"))));
                pdfPtable.AddCell(lr2.parallel.ToString("F2"));
                float num3 = 70f - pdfPtable.CalculateHeights();
                for (int index = 0; index < 7; ++index)
                    pdfPtable.AddCell(new PdfPCell()
                    {
                        FixedHeight = num3
                    });
                return pdfPtable;
            }
        }
    }
}
