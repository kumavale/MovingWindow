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
            int t = HWND_NOTOPMOST;
            string app = "notepad.exe";
            System.Random r = new System.Random(Seed: (int)DateTime.Now.Ticks);
            Rectangle rect = new Rectangle();
            Process p;

            for (int i = 0; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "-s":
                    case "--speed":
                    case "/s":
                        speed = Int32.Parse(args[++i]);
                        if(speed < 1) speed = 0;
                        //speed = 100 / speed;
                        break;
                    case "-t":
                    case "--top":
                    case "/t":
                        t = HWND_TOPMOST;
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
            w -= rect.Width  - rect.X;
            h -= rect.Height - rect.Y;

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
                SetWindowPos(p.MainWindowHandle, t, posX, posY, 0, 0, SWP_NOSIZE);
                //GetWindowRect(p.MainWindowHandle, ref rect);
                if ((posX < 0) || (posX > w)) dx *= -1;
                if ((posY < 0) || (posY > h)) dy *= -1;
                //System.Threading.Thread.Sleep(speed);
                var sw = Stopwatch.StartNew();
                while(sw.ElapsedMilliseconds < speed){};
            }
        }
    }
}
