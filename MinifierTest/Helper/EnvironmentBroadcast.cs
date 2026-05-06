
namespace MinifierTest
{


    // You only need to broadcast the WM_SETTINGCHANGE
    // message immediately after making permanent changes
    // to environment variables
    // (i.e., modifying the User or Machine registry scope).
    public static class EnvironmentBroadcast
    {

        // Import SendMessageTimeout from User32.dll to broadcast the setting change
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern System.IntPtr SendMessageTimeout(
            System.IntPtr hWnd,
            uint Msg,
            System.UIntPtr wParam,
            string lParam,
            uint fuFlags,
            uint uTimeout,
            out System.UIntPtr lpdwResult
        );

        // Constants for the broadcast
        private const uint WM_SETTINGCHANGE = 0x001A;
        private const uint SMTO_ABORTIFHUNG = 0x0002;
        private static readonly System.IntPtr HWND_BROADCAST = new System.IntPtr(0xffff);

        public static void BroadcastEnvironmentChange()
        {
            System.UIntPtr result;
            string section = "Environment";

            // Broadcast the message to all top-level windows
            SendMessageTimeout(
                HWND_BROADCAST,
                WM_SETTINGCHANGE,
                System.UIntPtr.Zero,
                section,
                SMTO_ABORTIFHUNG,
                5000, // 5 seconds timeout
                out result
            );
        } // End Sub BroadcastEnvironmentChange 


    } // End Class EnvironmentBroadcast 


} // End Namespace 
