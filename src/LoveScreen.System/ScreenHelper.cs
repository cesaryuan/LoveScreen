using LoveScreen.Windows.SystemDll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using L_Windows = System.Windows;

namespace LoveScreen.Windows
{
    public class ScreenHelper
    {
        public static Bitmap CaptureInternal(Rectangle rect, bool IncludeCursor = false)
        {
            Bitmap bitmap = new Bitmap(rect.Width, rect.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, bitmap.Size);
            }
            return bitmap;

            //var bmp = new Bitmap(rect.Width, rect.Height);

            //using (var g = Graphics.FromImage(bmp))
            //{
            //    g.CopyFromScreen(rect.Location, Point.Empty, rect.Size, CopyPixelOperation.SourceCopy);

            //    if (IncludeCursor)
            //        MouseCursor.Draw(g, P => new Point(P.X - rect.X, P.Y - rect.Y));

            //    g.Flush();
            //}

            //return bmp;
        }

        public static Bitmap CaptureFullScreen(bool IncludeCursor = false)
        {
            int left = SystemInformation.VirtualScreen.Left;
            int top = SystemInformation.VirtualScreen.Top;
            int width = SystemInformation.VirtualScreen.Width;
            int height = SystemInformation.VirtualScreen.Height;
            Bitmap bmp = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(Point.Empty, Point.Empty, bmp.Size, CopyPixelOperation.SourceCopy);

                if (IncludeCursor)
                    MouseCursor.Draw(g, P => new Point(P.X, P.Y));

                g.Flush();
            }
            return bmp;
        }

        public static void SetMousePos(int x, int y)
        {
            User32.SetCursorPos(x, y);
        }

        public static void GetVisualWindows()
        {

        }
    }
}
