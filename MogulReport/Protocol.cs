using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


namespace MogulReport
{
    /// <summary>
    /// the base class to create a protocol
    /// </summary>
    abstract class Protocol
    {
        virtual protected List<ProtocolPage> protocolPages
        {
            get;
            set;
        }
        public Protocol()
        {
            protocolPages = new List<ProtocolPage>();
        }
        internal virtual void save(string output_path)
        {
            // step 1
            Document document = new Document();
            using (document)
            {

                // step 2
                PdfWriter.GetInstance(document, new FileStream(output_path, FileMode.Create));
                // step 3
                document.Open();
                // step 4
                for (int i = 0; i < protocolPages.Count - 1; ++i)
                {
                    document.Add(protocolPages[i]);
                    document.NewPage();
                }
                document.Add(protocolPages.Last());

                //document.Add(new Paragraph("Hello World"));
                //step 5
                document.Close();
            }
        }
    }
}
