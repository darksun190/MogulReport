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
        public ProtocolPage() :base(1)
        {
            this.widthPercentage = 100f;
        }
    
    }
}
