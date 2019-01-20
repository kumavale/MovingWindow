using System;

using System.Runtime.InteropServices; // DLL Import
using System.Diagnostics;
using System.Drawing;

namespace MovingWindow
{
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, ref Rectangle lpRect);

        private const int SWP_NOSIZE = 0x0001;

        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;

        static void Main(string[] args)
        {
            int h = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
            int w = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            int speed = 10;
            int posX, posY, dx, dy;
            int seed = Environment.TickCount;
            System.Random r = new System.Random();
            Rectangle rect = new Rectangle();
            Process p;
            string app = "notepad.exe";

            //ToDo
            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "-s":
                    case "/s":
                        speed = Int32.Parse(args[++i]);
                        if(speed < 1) speed = 1;
                        break;
                    default:
                        app = args[i];
                        break;
                }
            }

            p = Process.Start(app);

            while (rect.Width == 0)
                GetWindowRect(p.MainWindowHandle, ref rect);
            posX = rect.X;
            posY = rect.Y;

            do
            {
                dx = r.Next(-1, 2);
                dy = r.Next(-1, 2);
            } while (dx == 0 || dy == 0);

            while (!p.HasExited)
            {
                //Console.WriteLine(rect);
                posX += dx;
                posY += dy;
                SetWindowPos(p.MainWindowHandle, HWND_NOTOPMOST, posX, posY, 0, 0, SWP_NOSIZE);
                GetWindowRect(p.MainWindowHandle, ref rect);
                if ((posX < 0) || (posX > w - rect.Width + rect.X))  dx *= -1;
                if ((posY < 0) || (posY > h - rect.Height + rect.Y)) dy *= -1;
                System.Threading.Thread.Sleep(100 / speed);
            }
        }
    }
}
