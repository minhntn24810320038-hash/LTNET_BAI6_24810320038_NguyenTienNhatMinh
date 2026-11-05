using System;
using System.Windows.Forms;
using LTNET_BAI6.GUI;

namespace LTNET_BAI6
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMainHub());
        }
    }
}