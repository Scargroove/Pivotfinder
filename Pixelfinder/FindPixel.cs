using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixelfinder
{
    internal class FindPixel
    {
        public static List<string> FindPixelInSpriteSheet(Bitmap bitmap, Point spriteSize, Color targetColor)
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

                    Point result = FindPixelInSprite(bitmap, spriteSize, targetColor, new Point(spriteSize.X * x, spriteSize.Y * y));
                    resultsList.Add(result.X + "," + result.Y);

                }
            }
           
            // Bild freigeben
            bitmap.Dispose();

            return resultsList;
        }
        private static Point FindPixelInSprite(Bitmap bitmap, Point spriteSize, Color targetColor, Point startPos)
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
