using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SPInterface;
using System.Diagnostics;
using System.Threading;

namespace MogulReport
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// 生成Mogul定制发动机报告
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            System.Globalization.CultureInfo UICulture = new System.Globalization.CultureInfo("en");

            Thread.CurrentThread.CurrentUICulture = UICulture;


            //initialize getting the datas from Calypso
            SP_Path.folder_name =
            System.IO.Path.Combine(
            System.AppDomain.CurrentDomain.BaseDirectory
            , @"..\conf\spiConf.xml"
            );
            SPI.init();

            var protocol = MogulFactory.createProtocol(SPI.elements);
            string output_path = "C:\\temp\\output.pdf";
            
            protocol.save(output_path);
            Process proc = Process.Start("C:\\temp\\output.pdf");
    
        }
    }
}
