using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSelectionExtractor;
using TextToSpeechConverter;
using AudioPlayer;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace MainThread
{
    class CMainThread
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static void Main()
        {
            Thread workerThread = new Thread(WorkerThreadDelegate);
            workerThread.SetApartmentState(ApartmentState.STA);
            workerThread.Start();
            workerThread.Join();
        }

        private static void WorkerThreadDelegate()
        {
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
        }
        
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            { 
                int vkCode = Marshal.ReadInt32(lParam);
                //Console.WriteLine((Keys)vkCode);

                if ((Keys)vkCode == Keys.F7)
                {
                    CTextSelectionExtractor textSelectionExtractor = new CTextSelectionExtractor();
                    CTextToSpeechConverter textToSpeechConverter = new CTextToSpeechConverter();
                    CAudioPlayer audioPlayer = new CAudioPlayer();

                    try
                    {
                        Console.WriteLine((Keys)vkCode);
                        
                        Console.WriteLine(textSelectionExtractor.GetText());
                        
                        string fileName = textToSpeechConverter.Convert(textSelectionExtractor.GetText());
                        
                        audioPlayer.play(fileName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        audioPlayer.play("UnalbleTOReadTheText.mp3");
                    }
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        }
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}