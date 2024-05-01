using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace Pixelfinder
{
    // Interne Klasse FindPixel zur Suche nach einem spezifischen Pixel in einem Bitmap
    internal class FindPixel
    {
        // Methode zum Suchen eines Pixels in einem Sprite Sheet (Bild, das mehrere kleinere Bilder enthält)
        public static List<string> FindPixelInSpriteSheet(Bitmap bitmap, Point spriteSize, Color targetColor)
        {
            // Überprüfung, ob die Sprite-Größe größer als die Bitmap-Größe ist
            if (spriteSize.X > bitmap.Width || spriteSize.Y > bitmap.Height)
            {
                throw new ArgumentException("Die Sprite-Größe darf nicht größer als die Spritesheet-Größe sein.");
            }

            int targetColorInt = targetColor.ToArgb(); // Konvertieren der Ziel-Farbe zu einem ARGB-Wert

            // Berechnung der Anzahl der Sprites im Sprite Sheet basierend auf der Größe des Bitmaps und der Sprite-Größe
            Point bitmapSize = new Point(bitmap.Width, bitmap.Height);
            Point spriteAmount = new Point(bitmapSize.X / spriteSize.X, bitmapSize.Y / spriteSize.Y);

            // Liste zur Speicherung der Ergebnisse der Pixel-Suche
            List<string> resultsList = new List<string>();

            // Sperren des Bitmaps im Speicher zur effizienteren Verarbeitung
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            // Erstellen eines Byte-Arrays zur Aufnahme der Pixeldaten
            byte[] pixelData = new byte[bitmapData.Stride * bitmapData.Height];
            Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            // Durchlaufen jedes Sprites im Sprite Sheet
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Suche nach dem Ziel-Pixel im aktuellen Sprite
                    Point result = FindPixelInSprite(pixelData, bitmapData.Stride, spriteSize, targetColorInt, new Point(spriteSize.X * x, spriteSize.Y * y));
                    // Hinzufügen des Ergebnisses zur Liste, als Koordinaten im Format "x,y"
                    resultsList.Add(result.X + "," + result.Y);
                }
            }

            // Rückgabe der Liste mit Ergebnissen
            return resultsList;
        }


        // Hilfsmethode zur Suche eines Pixels in einem einzelnen Sprite
        private static Point FindPixelInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos)
        {
            // Initiale Position, wenn das Pixel nicht gefunden wird
            Point result = new Point(0, 0);

            // Durchlaufen jedes Pixels im Sprite
            for (int y = 0; y < spriteSize.Y; y++)
            {
                int rowStart = (startPos.Y + y) * stride + startPos.X * 4;

                for (int x = 0; x < spriteSize.X; x++)
                {
                    int position = rowStart + x * 4;
                    int color = BitConverter.ToInt32(pixelData, position);

                    // Überprüfung, ob das aktuelle Pixel der Ziel-Farbe entspricht
                    if (color == targetColorInt)
                    {
                        result = new Point(x, y);
                        return result; // Rückgabe der Position und Abbruch der Schleife, wenn gefunden
                    }
                }
            }

            return result;
        }
        public static List<string> FindPixelInSpriteSheetOld(Bitmap bitmap, Point spriteSize, Color targetColor)
        {
            // Breite und Höhe des Spritesheets erhalten
            Point bitmapSize = new Point(bitmap.Width, bitmap.Height);

            // Menge der Sprites
            Point spriteAmount = new Point(bitmapSize.X / spriteSize.X, bitmapSize.Y / spriteSize.Y);

            List<string> resultsList = new List<string>();


            for (int y = 0; y < spriteAmount.Y; y++)

            {
                for (int x = 0; x < spriteAmount.X; x++)
                {

                    Point result = FindPixelInSpriteOld(bitmap, spriteSize, targetColor, new Point(spriteSize.X * x, spriteSize.Y * y));
                    resultsList.Add(result.X + "," + result.Y);

                }
            }

            // Bild freigeben
            bitmap.Dispose();

            return resultsList;
        }
        private static Point FindPixelInSpriteOld(Bitmap bitmap, Point spriteSize, Color targetColor, Point startPos)
        {
            Point result = new Point(0, 0);

            // Durch das Bild iterieren
            for (int y = startPos.Y; y < spriteSize.Y + startPos.Y; y++)
            {
                for (int x = startPos.X; x < spriteSize.X + startPos.X; x++)
                {
                    // Pixel an der aktuellen Position erhalten
                    Color pixelColor = bitmap.GetPixel(x, y);

                    // Überprüfen, ob der aktuelle Pixel die gewünschte Farbe hat
                    if (pixelColor.ToArgb() == targetColor.ToArgb())
                    {
                        // Koordinaten speichern und die Schleifen durchbrechen
                        result = new Point(x - startPos.X, y - startPos.Y);
                        break;
                    }

                }
            }

            return result;
        }

    }
}
