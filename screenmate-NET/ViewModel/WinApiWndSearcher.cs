using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ScreenMateNET.ViewModel
{
    public class WndSearcher
    {

        private static WndSearcher instance = null;

        Process previousWindow = new Process();

        //static List<String> validApps = new List<string> {"Microsoft Teams", "Visual Studio", "Google Chrome"};
        //List<String> validApps = new List<string> { "Microsoft Teams", "Visual Studio" };
        List<String> validApps = new List<string> { "Microsoft Teams" };

        public static WndSearcher Instance
        {
            get
            {
                if (instance == null)
                    instance = new WndSearcher();
                return instance;
            }
        }

        public Point GetCoordinatesOfTopWindow()
        {
            foreach (Process window in Process.GetProcesses())
            {
                foreach (string validapp in validApps)
                {
                    if (window.MainWindowTitle.Contains(validapp))
                    {
                        RECT position = new RECT();
                        GetWindowRect(window.MainWindowHandle, out position);
                        Trace.WriteLine(" - Title: " + window.MainWindowTitle + " Place: " + position.Location.ToString());
                        return position.Location;
                    }
                }
                if (previousWindow == window) break;
            }
            return new Point(650,50);
           
        }

        public IntPtr SearchForWindow(string wndclass, string title)
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
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);



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


                    // 6. Get the handle to a dialog
                    IntPtr dlgHandle = GetWindow(window.MainWindowHandle, GetWindowType.GW_HWNDFIRST);


                    windowHandles.Add(window.MainWindowHandle);
                    //Trace.WriteLine("\n" +window.MainWindowHandle.ToString());
                    StringBuilder sb = new StringBuilder(1024);
                    GetClassName(dlgHandle, sb, sb.Capacity);

                    String classname = sb.ToString();

                    sb = new StringBuilder(1024);
                    GetWindowText(dlgHandle, sb, sb.Capacity);
                    String title = sb.ToString();


                    Trace.WriteLine("Handle: "+ window.MainWindowHandle.ToString() +" Class NAME: " + classname + " - Title: " + title + "Place:" + GetPlacement(window.MainWindowHandle));
                    Trace.WriteLine(dlgHandle.ToString());


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
            return placement.showCmd;
        }


        // 6.

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        private enum GetWindowType : uint
        {
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is highest in the Z order.
            /// <para/>
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDFIRST = 0,
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is lowest in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDLAST = 1,
            /// <summary>
            /// The retrieved handle identifies the window below the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDNEXT = 2,
            /// <summary>
            /// The retrieved handle identifies the window above the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDPREV = 3,
            /// <summary>
            /// The retrieved handle identifies the specified window's owner window, if any.
            /// </summary>
            GW_OWNER = 4,
            /// <summary>
            /// The retrieved handle identifies the child window at the top of the Z order,
            /// if the specified window is a parent window; otherwise, the retrieved handle is NULL.
            /// The function examines only child windows of the specified window. It does not examine descendant windows.
            /// </summary>
            GW_CHILD = 5,
            /// <summary>
            /// The retrieved handle identifies the enabled popup window owned by the specified window (the
            /// search uses the first such window found using GW_HWNDNEXT); otherwise, if there are no enabled
            /// popup windows, the retrieved handle is that of the specified window.
            /// </summary>
            GW_ENABLEDPOPUP = 6
        }

        // 7.
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        // 8.
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
    }
}
