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
            this.defaultCell.Padding = 0.8f;
            //this.defaultCell.Border = 2;
        }

        virtual protected PdfPTable Header
        {
            get
            {
                Font fontBold = new Font(Font.FontFamily.UNDEFINED, 9, Font.BOLD, BaseColor.BLACK);//FontFactory.GetFont("Arial", 5);
                Font fontNormal = new Font(Font.FontFamily.UNDEFINED, 9, Font.NORMAL, BaseColor.BLACK);
                Font fontTitle = new Font(Font.FontFamily.UNDEFINED, 15, Font.BOLD, BaseColor.BLACK);


                PdfPTable header = new PdfPTable(1);
                header.DefaultCell.Padding = 0;
                PdfPTable Logo_Title = new PdfPTable(3);
                Logo_Title.SetWidths(new int[] { 1, 2, 1 });
                
                Logo_Title.DefaultCell.Padding = 0;
                Image logo = Image.GetInstance(Properties.Resources.logo, BaseColor.YELLOW);

                PdfPCell Logo_Mogul = new PdfPCell(logo, true);
                Logo_Mogul.PaddingRight = 1f;
                Logo_Mogul.PaddingLeft = 1f;
                Logo_Mogul.FixedHeight = 30f * 2f; //75 is pixels per inch

                Logo_Mogul.HorizontalAlignment = Element.ALIGN_CENTER;
                Logo_Mogul.VerticalAlignment = Element.ALIGN_MIDDLE;
                Logo_Title.AddCell(Logo_Mogul);

                PdfPCell Header_Title = new PdfPCell(new Phrase(MogulReport.Properties.Settings.Default.Title, fontTitle));
                Header_Title.PaddingRight = 1f;
                Header_Title.PaddingLeft = 1f;
                Header_Title.FixedHeight = 30f * 2f; //75 is pixels per inch

                Header_Title.HorizontalAlignment = Element.ALIGN_CENTER;
                Header_Title.VerticalAlignment = Element.ALIGN_MIDDLE;
                Logo_Title.AddCell(Header_Title);

                Image zeiss_logo = Image.GetInstance(Properties.Resources.ZeissLogo, BaseColor.YELLOW);

                PdfPCell Logo_Zeiss = new PdfPCell(zeiss_logo, true);
                Logo_Zeiss.Padding = 2f;
                Logo_Zeiss.FixedHeight = 30f * 2f; //75 is pixels per inch

                Logo_Zeiss.HorizontalAlignment = Element.ALIGN_CENTER;
                Logo_Zeiss.VerticalAlignment = Element.ALIGN_MIDDLE;
                Logo_Title.AddCell(Logo_Zeiss);

                header.AddCell(Logo_Title);

                PdfPTable Calypso_headers = new PdfPTable(4);
                Calypso_headers.SetWidths(new int[] { 1, 3, 1, 3 });
                List<PdfPCell> CH_name = new List<PdfPCell>();
                List<PdfPCell> CH_value = new List<PdfPCell>();

                //customer:
                {
                    string n = "Customer:";
                    string t = "Customer";

                    PdfPCell name = new PdfPCell(
                        new Phrase(
                            n,
                            fontNormal));
                    PdfPCell value = new PdfPCell();
                    try
                    {
                        value = new PdfPCell(
                            new Phrase(
                                SPInterface.SPI.sys_dict[t],
                                fontBold));
                    }
                    catch
                    {

                    }
                    CH_name.Add(name);
                    CH_value.Add(value);
                }
                //drawing No:
                {
                    string n = "Drawing No.:";
                    string t = "drawingno";

                    PdfPCell name = new PdfPCell(
                        new Phrase(
                            n,
                            fontNormal));
                    PdfPCell value = new PdfPCell();
                    try
                    {
                        value = new PdfPCell(
                            new Phrase(
                                SPInterface.SPI.sys_dict[t],
                                fontBold));
                    }
                    catch
                    {

                    }
                    CH_name.Add(name);
                    CH_value.Add(value);
                }
                //Type:
                {
                    string n = "Type:";
                    string t = "type";

                    PdfPCell name = new PdfPCell(
                        new Phrase(
                            n,
                            fontNormal));
                    PdfPCell value = new PdfPCell();
                    try
                    {
                        value = new PdfPCell(
                            new Phrase(
                                SPInterface.SPI.sys_dict[t],
                                fontBold));
                    }
                    catch
                    {

                    }
                    CH_name.Add(name);
                    CH_value.Add(value);
                }
                //Operator:
                {
                    string n = "Operator:";
                    string t = "operid";

                    PdfPCell name = new PdfPCell(
                        new Phrase(
                            n,
                            fontNormal));
                    PdfPCell value = new PdfPCell();
                    try
                    {
                        value = new PdfPCell(
                            new Phrase(
                                SPInterface.SPI.sys_dict[t],
                                fontBold));
                    }
                    catch
                    {

                    }
                    CH_name.Add(name);
                    CH_value.Add(value);
                }
                //TR No:
                {
                    string n = "TR No.:";
                    string t = "u_tr";

                    PdfPCell name = new PdfPCell(
                        new Phrase(
                            n,
                            fontNormal));
                    PdfPCell value = new PdfPCell();
                    try
                    {
                        value = new PdfPCell(
                            new Phrase(
                                SPInterface.SPI.sys_dict[t],
                                fontBold));
                    }
                    catch
                    {

                    }
                    CH_name.Add(name);
                    CH_value.Add(value);
                }
                //Date:
                {
                    string n = "Date:";
                    string t = "date";

                    PdfPCell name = new PdfPCell(
                        new Phrase(
                            n,
                            fontNormal));
                    PdfPCell value = new PdfPCell();
                    try
                    {
                        value = new PdfPCell(
                            new Phrase(
                                SPInterface.SPI.sys_dict[t],
                                fontBold));
                    }
                    catch
                    {

                    }
                    CH_name.Add(name);
                    CH_value.Add(value);
                }

                for (int i = 0; i < CH_name.Count; ++i)
                {
                    var name_cell = CH_name[i];
                    var value_cell = CH_value[i];

                    Calypso_headers.AddCell(name_cell);
                    Calypso_headers.AddCell(value_cell);
                }
                Calypso_headers.CompleteRow();
                header.AddCell(Calypso_headers);

                string comment = "Comment:";
                if(SPInterface.SPI.sys_dict.Keys.Contains("vda_remark"))
                {
                    comment += SPInterface.SPI.sys_dict["vda_remark"];
                }
                PdfPCell comment_header =new PdfPCell(
                        new Phrase(
                            comment,
                            fontNormal));
                
              
              
                comment_header.VerticalAlignment = Element.ALIGN_TOP;
                comment_header.HorizontalAlignment = Element.ALIGN_LEFT;
                comment_header.FixedHeight = 30f;
          
                header.AddCell(comment_header);


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
                PdfPTable graphic = new PdfPTable(1);
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
