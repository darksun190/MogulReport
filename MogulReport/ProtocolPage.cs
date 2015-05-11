using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace MogulReport
{
    /// <summary>
    /// base class, one A4 page
    /// </summary>
    abstract class ProtocolPage : PdfPTable
    {
        public ProtocolPage()
            : base(1)
        {
            this.widthPercentage = 100f;
            this.defaultCell.Padding = 0;
        }
     
        virtual protected PdfPTable Header
        {
            get
            {
                PdfPTable header = new PdfPTable(2);
                header.DefaultCell.Padding = 0;
                //header part
                Image logo = Image.GetInstance(Properties.Resources.logo, BaseColor.YELLOW);
                header.SetWidths(new int[] { 1, 5 });

                PdfPCell headerCell1 = new PdfPCell(logo, true);
                headerCell1.PaddingRight =1f;
                headerCell1.PaddingLeft = 1f;
                headerCell1.FixedHeight = 30f * 2f ; //75 is pixels per inch

                headerCell1.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                header.AddCell(headerCell1);

                string header_str = string.Format(
                    "{0} : ________   ,   ____               {1}: ______ {2}: _____",
                    Properties.Resources.HeaderString1,
                    Properties.Resources.Page,
                    Properties.Resources.To);
                PdfPCell headerCell2 = new PdfPCell(new Paragraph(header_str));

                headerCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
                headerCell2.HorizontalAlignment = Element.ALIGN_LEFT;

                headerCell2.PaddingBottom = 10f;
                headerCell2.PaddingLeft = 15f;
                header.AddCell(headerCell2);

                return header;
            }
        }
        virtual protected PdfPTable Graphic
        {
            get
            {
                PdfPTable graphic =  new PdfPTable(1);
                return graphic;
            }
        }

        virtual protected PdfPTable Datas
        {
            get
            {
                PdfPTable datas = new PdfPTable(1);
                return datas;
            }
        }
    }
}
