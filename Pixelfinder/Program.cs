using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pixelfinder
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            // Pfad zum Bild angeben
            string imagePath = @"C:\texture.png";

            // Farbcode definieren (RGB-Wert)
            Color targetColor = Color.FromArgb(255, 255, 0, 255);

            // Bild laden
            Bitmap bitmap = new Bitmap(imagePath);

            // Breite und Höhe des Spritesheets erhalten
            Point bitmapSize = new Point(bitmap.Width, bitmap.Height);

            // Breite und Höhe des Sprites erhalten
            Point spriteSize = new Point(128, 128);


            // Menge der Sprites
            Point spriteAmount = new Point(bitmapSize.X / spriteSize.X, bitmapSize.Y / spriteSize.Y);



            for (int y = 0; y < spriteAmount.Y; y++)

            {
                for (int x = 0; x < spriteAmount.Y; x++)
                {

                    Point result = FindPixel(spriteSize, new Point(spriteSize.X * x, spriteSize.Y * y), targetColor, bitmap);
                    Console.WriteLine(result.X + "," + result.Y);

                }
            }

            // Bild freigeben
            bitmap.Dispose();
        }

        private static Point FindPixel(Point spriteSize, Point startPos, Color targetColor, Bitmap bitmap)
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
