using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArkEcho.Maui.Platforms.Windows
{
    public class NativeMediaKeyHook
    {
        // Define the key codes for media keys
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3; // 179
        public const int VK_MEDIA_STOP = 0xB2; // 178
        public const int VK_MEDIA_NEXT_TRACK = 0xB0; // 176
        public const int VK_MEDIA_PREV_TRACK = 0xB1; // 177

        // Windows message constants
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        public static event EventHandler<int> MediaKeyEventHandler;

        // Delegate for the hook procedure
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // Windows API functions
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private IntPtr hookHandle;

        private NativeMediaKeyHook() { }
        ~NativeMediaKeyHook()
        {
            // Clean up the hook when the application is closing
            UnhookWindowsHookEx(hookHandle);
        }

        public static NativeMediaKeyHook Instance = new NativeMediaKeyHook();

        public void StartKeyHook()
        {
            // Set up the key hook
            hookHandle = SetWindowsHookEx(13, LowLevelKeyboardHookProc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
        }

        // Key hook procedure
        public static IntPtr LowLevelKeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == VK_MEDIA_PLAY_PAUSE || vkCode == VK_MEDIA_STOP ||
                    vkCode == VK_MEDIA_NEXT_TRACK || vkCode == VK_MEDIA_PREV_TRACK)
                {
                    // Invoke the media key event
                    MediaKeyEventHandler?.Invoke(null, vkCode);
                }
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
    }
}
