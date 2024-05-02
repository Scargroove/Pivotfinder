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
        // Diese Methode findet Pixel einer bestimmten Farbe in einem Bitmap und kann optional diese Pixel auch entfernen.
        public static List<string> FindPixelInSpriteSheet(Bitmap bitmap, Point spriteSize, Color targetColor, bool removePixel = false)
        {
            // Überprüft, ob die Größe eines einzelnen Sprites kleiner oder gleich der Größe des gesamten Bitmaps ist.
            if (spriteSize.X > bitmap.Width || spriteSize.Y > bitmap.Height)
            {
                throw new ArgumentException("Die Sprite-Größe darf nicht größer als die Spritesheet-Größe sein.");
            }

            // Konvertiert die Ziel-Farbe in ein ARGB-Integer-Format für einfache Vergleiche.
            int targetColorInt = targetColor.ToArgb();

            // Berechnet die Anzahl der Sprites in X- und Y-Richtung auf dem Spritesheet.
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);

            List<string> resultsList = new List<string>();

            // Sperren des Bitmaps im Speicher für schnellen Zugriff.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] pixelData = new byte[bitmapData.Stride * bitmapData.Height];
            Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);

            // Durchläuft jedes Sprite auf dem Spritesheet.
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Findet die Position des gesuchten Pixels im Sprite.
                    Point result = FindPixelInSprite(pixelData, bitmapData.Stride, spriteSize, targetColorInt, new Point(spriteSize.X * x, spriteSize.Y * y), removePixel);
                    resultsList.Add(result.X + "," + result.Y);
                }
            }

            // Kopiert die veränderten Pixeldaten zurück ins Bitmap.
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            // Optional: Speichert das veränderte Bitmap.
            if (removePixel)
            {
                SaveModifiedBitmap(bitmap);
            }

            // Gibt das Bitmap-Speicher frei.
            bitmap.Dispose();
            return resultsList;
        }

        // Diese Hilfsmethode sucht das Pixel mit der Ziel-Farbe in einem einzelnen Sprite.
        private static Point FindPixelInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos, bool removePixel = false)
        {
            Point result = new Point(0, 0);

            // Durchsucht das Sprite Pixel für Pixel.
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
                            ReplacePixelWithDominantNeighboringColor(pixelData, stride, spriteSize, position);
                        }
                        return result;
                    }
                }
            }

            return result;
        }

        // Diese Methode speichert das veränderte Bitmap unter einem neuen Dateinamen.
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

        // Diese Methode ersetzt ein gefundenes Pixel durch die dominante Farbe der benachbarten Pixel, falls erforderlich.
        private static void ReplacePixelWithDominantNeighboringColor(byte[] pixelData, int stride, Point spriteSize, int position)
        {
            // Initialisiere ein Array, um die Farben der vier direkten Nachbarpixel zu speichern
            int[] neighboringColors = new int[4];
            // Berechne die Y-Koordinate des Pixels im Bild (Zeilenposition)
            int y = position / stride;
            // Berechne die X-Koordinate des Pixels im Bild (Spaltenposition)
            int x = (position % stride) / 4;

            // Prüfe ob es ein Pixel oberhalb gibt und speichere seine Farbe
            if (y > 0) neighboringColors[0] = BitConverter.ToInt32(pixelData, position - stride);
            // Prüfe ob es ein Pixel unterhalb gibt und speichere seine Farbe
            if (y < spriteSize.Y - 1) neighboringColors[1] = BitConverter.ToInt32(pixelData, position + stride);
            // Prüfe ob es ein Pixel links gibt und speichere seine Farbe
            if (x > 0) neighboringColors[2] = BitConverter.ToInt32(pixelData, position - 4);
            // Prüfe ob es ein Pixel rechts gibt und speichere seine Farbe
            if (x < spriteSize.X - 1) neighboringColors[3] = BitConverter.ToInt32(pixelData, position + 4);

            // Gruppiere die gespeicherten Farben nach ihrer ARGB-Wert und zähle, wie oft jede Farbe vorkommt
            var colorGroups = neighboringColors.GroupBy(c => c).ToList();

            // Suche nach einer Gruppe, die genau drei Pixel derselben Farbe enthält
            var threeColorMatch = colorGroups.FirstOrDefault(g => g.Count() == 3);
            // Wenn eine solche Gruppe existiert, benutze deren Farbe, um das aktuelle Pixel zu ersetzen
            if (threeColorMatch != null)
            {
                int newColor = threeColorMatch.Key;
                BitConverter.GetBytes(newColor).CopyTo(pixelData, position);
                return; // Beende die Methode frühzeitig nach dem Ersetzen
            }

            // Wenn keine drei gleichen Farben gefunden wurden, suche nach einer Gruppe mit genau zwei gleichen Farben
            var twoColorMatch = colorGroups.FirstOrDefault(g => g.Count() == 2);
            // Wenn eine solche Gruppe existiert und das linke Nachbarpixel (index 2) diese Farbe hat, 
            // benutze diese Farbe zum Ersetzen des aktuellen Pixels
            if (twoColorMatch != null && neighboringColors[2] == twoColorMatch.Key)
            {
                int newColor = twoColorMatch.Key;
                BitConverter.GetBytes(newColor).CopyTo(pixelData, position);
            }
        }
    }
}
