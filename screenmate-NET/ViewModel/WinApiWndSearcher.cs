using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ScreenMateNET.ViewModel
{
    public class WndSearcher
    {
        public static IntPtr SearchForWindow(string wndclass, string title)
        {
            SearchData sd = new SearchData { Wndclass = wndclass, Title = title };
            //SearchData sd = new SearchData();

            EnumWindows(new EnumWindowsProc(EnumProc), ref sd);
            return sd.hWnd;
        }

        private static bool EnumProc(IntPtr hWnd, ref SearchData data)
        {
            // Check classname and title
            // This is different from FindWindow() in that the code below allows partial matches
            StringBuilder sb = new StringBuilder(1024);
            GetClassName(hWnd, sb, sb.Capacity);

            String classname = sb.ToString();

            sb = new StringBuilder(1024);
            GetWindowText(hWnd, sb, sb.Capacity);
            String title = sb.ToString();

            //Trace.WriteLine("Class NAME: " + classname + " - Title: " + title);

            data.hWnd = hWnd;
            uint wprocessid;
            GetWindowThreadProcessId(data.hWnd, out wprocessid);
            Trace.WriteLine("Wproc: " + wprocessid + " - hwnd: " + hWnd);

            if (hWnd == Process.GetCurrentProcess().Handle)
            {
                return false;
            }

            if (classname.StartsWith(data.Wndclass))
            {
                
                if (title.StartsWith(data.Title))
                {
                    return false;    // Found the wnd, halt enumeration
                }
            }
            return true;
        }

        private class SearchData1
        {
            // You can put any dicks or Doms in here...
            public IntPtr hWnd;
        }
        private class SearchData
        {
            // You can put any dicks or Doms in here...
            public string Wndclass;
            public string Title;
            public IntPtr hWnd;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, ref SearchData data);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, ref SearchData data);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);



        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);



        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public static void GetAllWindows()
        {
            GetWindowHandles();
            /*ArrayList list = _GetAllWindows();
            int index = 0;
            foreach (var item in list)
            {
                Trace.WriteLine("__ " + index);
                Trace.WriteLine(item.ToString());

                index++;
            }
            */
        }

        //3.
        private static ArrayList _GetAllWindows()
        {
            var windowHandles = new ArrayList();
            EnumedWindow callBackPtr = GetWindowHandle;
            EnumWindows(callBackPtr, windowHandles);

            foreach (IntPtr windowHandle in windowHandles.ToArray())
            {
                EnumChildWindows(windowHandle, callBackPtr, windowHandles);
            }

            return windowHandles;
        }

        private delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumedWindow lpEnumFunc, ArrayList lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumedWindow callback, ArrayList lParam);

        private static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            windowHandles.Add(windowHandle);
            return true;
        }

        //4.
        public static void GetWindowHandles()
        {
            List<IntPtr> windowHandles = new List<IntPtr>();

            foreach (Process window in Process.GetProcesses())
            {
                window.Refresh();

                if (window.MainWindowHandle != IntPtr.Zero)
                {
                    windowHandles.Add(window.MainWindowHandle);
                    Trace.WriteLine("\n" +window.MainWindowHandle.ToString());
                    StringBuilder sb = new StringBuilder(1024);
                    GetClassName(window.MainWindowHandle, sb, sb.Capacity);

                    String classname = sb.ToString();

                    sb = new StringBuilder(1024);
                    GetWindowText(window.MainWindowHandle, sb, sb.Capacity);
                    String title = sb.ToString();

                    Trace.WriteLine("Class NAME: " + classname + " - Title: " + title + "Place:" + GetPlacement(window.MainWindowHandle));

                }
            }
        }

        //5.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        private struct WINDOWPLACEMENT
        {
            public int length;
            public uint flags;
            public uint showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        const uint SW_HIDE = 0;
        const uint SW_MAXIMIZE = 3;
        const uint SW_MINIMIZE = 6;
        const uint SW_RESTORE = 9;
        const uint SW_SHOW = 5;
        const uint SW_SHOWMAXIMIZED = 3;
        const uint SW_SHOWMINIMIZED = 2;
        const uint SW_SHOWMINNOACTIVE = 7;
        const uint SW_SHOWNA = 8;
        const uint SW_SHOWNOACTIVATE = 4;
        const uint SW_SHOWNORMAL = 1;

        public static uint GetPlacement(IntPtr handle)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(handle, ref placement);
            //Trace.WriteLine(placement);
            return placement.flags;
        }


    }
}
