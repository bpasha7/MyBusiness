using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Contacts
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Check())
                return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        static bool Check()
        {
            string[] files = System.IO.Directory.GetFiles(Directory.GetCurrentDirectory(), "*.lic");
            if (files.Count() != 1)
                return false;
            //file
            var computerName = System.Environment.MachineName;
            return true;
        }
    }
}
