using System.ComponentModel;
using System.Diagnostics;

namespace VS_CUWSISMED
{
    internal static class DesignTimeHelper
    {
        public static bool IsActive
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    return true;
                }

                string processName = Process.GetCurrentProcess().ProcessName;
                return processName == "devenv" || processName == "XDesProc";
            }
        }
    }
}
