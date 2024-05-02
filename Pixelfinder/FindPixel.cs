using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pixelfinder
{
    internal class FindPixel
    {
        public static List<string> FindPixelInSpriteSheet(Bitmap bitmap, Point spriteSize, Color targetColor, bool removePixel = false)
        {
            if (spriteSize.X > bitmap.Width || spriteSize.Y > bitmap.Height)
            {
                throw new ArgumentException("Die Sprite-Größe darf nicht größer als die Spritesheet-Größe sein.");
            }

            int targetColorInt = targetColor.ToArgb();
            Point bitmapSize = new Point(bitmap.Width, bitmap.Height);
            Point spriteAmount = new Point(bitmapSize.X / spriteSize.X, bitmapSize.Y / spriteSize.Y);
            List<string> resultsList = new List<string>();

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] pixelData = new byte[bitmapData.Stride * bitmapData.Height];
            Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);

            for (int y = 0; y < spriteAmount.Y; y++)
            {
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    Point result = FindPixelInSprite(pixelData, bitmapData.Stride, spriteSize, targetColorInt, new Point(spriteSize.X * x, spriteSize.Y * y), removePixel);
                    resultsList.Add(result.X + "," + result.Y);
                }
            }

            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            if (removePixel)
            {
                // Save the modified Bitmap using the tag of the bitmap as the path
                SaveModifiedBitmap(bitmap);

            }
            bitmap.Dispose();
            return resultsList;
        }

        private static Point FindPixelInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos, bool removePixel = false)
        {
            Point result = new Point(0, 0);

            for (int y = 0; y < spriteSize.Y; y++)
            {
                int rowStart = (startPos.Y + y) * stride + startPos.X * 4;

                for (int x = 0; x < spriteSize.X; x++)
                {
                    int position = rowStart + x * 4;
                    int color = BitConverter.ToInt32(pixelData, position);

                    if (color == targetColorInt)
                    {
                        result = new Point(x, y);
                        if (removePixel)
                        {
                            int[] neighboringColors = new int[4];
                            if (y > 0) neighboringColors[0] = BitConverter.ToInt32(pixelData, position - stride);
                            if (y < spriteSize.Y - 1) neighboringColors[1] = BitConverter.ToInt32(pixelData, position + stride);
                            if (x > 0) neighboringColors[2] = BitConverter.ToInt32(pixelData, position - 4);
                            if (x < spriteSize.X - 1) neighboringColors[3] = BitConverter.ToInt32(pixelData, position + 4);

                            var colorGroups = neighboringColors.GroupBy(c => c).Where(g => g.Count() >= 2).ToList();
                            if (colorGroups.Any())
                            {
                                int newColor = colorGroups.First().Key;
                                BitConverter.GetBytes(newColor).CopyTo(pixelData, position);
                            }
                        }
                        return result;
                    }
                }
            }

            return result;
        }

        public static void SaveModifiedBitmap(Bitmap bitmap, string modifiedText = "_modified")
        {
            if (bitmap.Tag == null || string.IsNullOrEmpty(bitmap.Tag.ToString()))
            {
                throw new InvalidOperationException("Das Bitmap-Tag enthält keinen gültigen Speicherpfad.");
            }

            string originalPath = bitmap.Tag.ToString();
            string directory = Path.GetDirectoryName(originalPath);
            string filename = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);
            string newFileName = Path.Combine(directory, filename + modifiedText + extension);

            bitmap.Save(newFileName, ImageFormat.Png);
        }

        // alte Methoden
        //public static List<string> FindPixelInSpriteSheetOld(Bitmap bitmap, Point spriteSize, Color targetColor)
        //{
        //    // Breite und Höhe des Spritesheets erhalten
        //    Point bitmapSize = new Point(bitmap.Width, bitmap.Height);

        //    // Menge der Sprites
        //    Point spriteAmount = new Point(bitmapSize.X / spriteSize.X, bitmapSize.Y / spriteSize.Y);

        //    List<string> resultsList = new List<string>();


        //    for (int y = 0; y < spriteAmount.Y; y++)

        //    {
        //        for (int x = 0; x < spriteAmount.X; x++)
        //        {

        //            Point result = FindPixelInSpriteOld(bitmap, spriteSize, targetColor, new Point(spriteSize.X * x, spriteSize.Y * y));
        //            resultsList.Add(result.X + "," + result.Y);

        //        }
        //    }

        //    // Bild freigeben
        //    bitmap.Dispose();

        //    return resultsList;
        //}
        //private static Point FindPixelInSpriteOld(Bitmap bitmap, Point spriteSize, Color targetColor, Point startPos)
        //{
        //    Point result = new Point(0, 0);

        //    // Durch das Bild iterieren
        //    for (int y = startPos.Y; y < spriteSize.Y + startPos.Y; y++)
        //    {
        //        for (int x = startPos.X; x < spriteSize.X + startPos.X; x++)
        //        {
        //            // Pixel an der aktuellen Position erhalten
        //            Color pixelColor = bitmap.GetPixel(x, y);

        //            // Überprüfen, ob der aktuelle Pixel die gewünschte Farbe hat
        //            if (pixelColor.ToArgb() == targetColor.ToArgb())
        //            {
        //                // Koordinaten speichern und die Schleifen durchbrechen
        //                result = new Point(x - startPos.X, y - startPos.Y);
        //                break;
        //            }

        //        }
        //    }

        //    return result;
        //}


    }
}
