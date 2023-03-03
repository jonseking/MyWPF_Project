﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace CourseManagement.Common
{
    public static class ImageFormatConvertHelper
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        /// <summary>  /// 
        /// 从bitmap转换成ImageSource  /// 
        /// </summary>  /// 
        /// <param name="icon"></param>
        /// <returns></returns>  
        public static ImageSource ChangeBitmapToImageSource(Bitmap bitmap)
        { //Bitmap bitmap = icon.ToBitmap();  
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            if (!DeleteObject(hBitmap))
            { throw new System.ComponentModel.Win32Exception(); }
            return wpfBitmap;
        }
    }
}
