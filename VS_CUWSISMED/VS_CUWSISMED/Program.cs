using System;
using System.Windows.Forms;

namespace VS_CUWSISMED
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new login_page()); // <- startowe okno
        }
    }
}