using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Gamma_Library
{
    public class Algorithm : ImageHandler
    {
        public string HandlerName { get => "Гамма-коррекция"; }
        public Bitmap Source { set; get; }
        public Bitmap Result { set; get; }
        public static float GammaNum { set; get; } // Коффициент гамма-коррекции
        public int ThreadNumber { set; get; }
        public Stopwatch stopwatch { set; get; }
        public void init(SortedList<string, object> parameters)
        {
            Source = (Bitmap)parameters["Image"];
            ThreadNumber = (int)parameters["Number"];
            stopwatch = new Stopwatch();
        }
        // Выполняем гамма-коррекцию изображения
        public void startHandle(ProgressDelegate progress)
        {
            // Засекаем время
            stopwatch.Start();

            // Выполняем прямой доступ к пикселям 
            BitmapData bmpData = Source.LockBits(new Rectangle(0, 0, Source.Width, Source.Height),
                ImageLockMode.ReadOnly, Source.PixelFormat);
            byte[] array = new byte[bmpData.Stride * bmpData.Height];
            Marshal.Copy(bmpData.Scan0, array, 0, array.Length);
            Source.UnlockBits(bmpData);

            // Выполняем алгоритм задачи по варианту
            int index;
            int max = Source.Height - 1;
            for (int i = 0; i < Source.Height; i++)
            {
                for (int j = 0; j < Source.Width; j++)
                {
                    index = bmpData.Stride * i + 3 * j;
                    // Выполняем функцию гамма-коррекции f = c * x^y
                    array[index + 0] = (byte)(255 * Math.Pow(((int)array[index + 0] / 255.0), GammaNum));
                    array[index + 1] = (byte)(255 * Math.Pow(((int)array[index + 1] / 255.0), GammaNum));
                    array[index + 2] = (byte)(255 * Math.Pow(((int)array[index + 2] / 255.0), GammaNum));
                    if (array[index + 0] > 255) // blue
                        array[index + 0] = 255;
                    if (array[index + 1] > 255) // green
                        array[index + 1] = 255;
                    if (array[index + 2] > 255) // red
                        array[index + 2] = 255;
                }
                progress?.Invoke(ThreadNumber, (double)i / max * 100, Source.Width, Source.Height);
            }

            Result = new Bitmap(Source.Width, Source.Height, bmpData.Stride, 
                Source.PixelFormat, Marshal.UnsafeAddrOfPinnedArrayElement(array, 0));

            stopwatch.Stop();
        }
    }
}
