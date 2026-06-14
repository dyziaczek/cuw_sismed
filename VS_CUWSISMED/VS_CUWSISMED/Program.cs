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

            Employee employee;
            using (var login = new login_page())
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                employee = login.AuthenticatedEmployee;
            }

            Application.Run(new main_app(employee));
        }
    }
}
