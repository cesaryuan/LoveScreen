using LoveScreen.Windows.Models;
using LoveScreen.Windows.SystemDll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace LoveScreen.Windows.Extensions
{
    public static class WindowExt
    {
        public static Rectangle RectWithDpi(this Window window)
        {
            double dpi = window.Dpi();
            return new Rectangle((int)(window.Left * dpi),
                                 (int)(window.Top * dpi),
                                 (int)(window.Width * dpi),
                                 (int)(window.Height * dpi));
        }

        public static double Dpi(this Window window)
        {
            //RECT rect;
            //User32.GetWindowRect(new WindowInteropHelper(window).Handle, out rect);
            //return rect.width / window.ActualWidth;
            return VisualTreeHelper.GetDpi(window).DpiScaleX;
        }

        public static Rect RectWithDpi(this Rect rect, Window window)
        {
            double dpi = window.Dpi();
            Rect r = new Rect(rect.Location, rect.Size);
            r.Width *= dpi;
            r.Height *= dpi;
            r.X *= dpi;
            r.Y *= dpi;
            return r;
        }
    }
}
