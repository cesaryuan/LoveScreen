using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LoveScreen.Windows
{
    public static class SystemHelper
    {
        public static BitmapSource SetClipboard(BitmapSource bitmapSource)
        {
            Clipboard.SetImage(bitmapSource);
            return bitmapSource;
        }
    }
}
