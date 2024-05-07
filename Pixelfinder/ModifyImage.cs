using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Pixelfinder
{
    internal class ModifyImage
    {

        // Findet Pixel einer bestimmten Farbe in einem Bitmap und kann optional diese Pixel auch entfernen.
        public static (List<string> resultsList, List<string> errorImagePaths) FindPixelInSpriteSheet(Bitmap bitmap, Point spriteSize, Color targetColor, Color changeAlphaTo, bool removePixel = false, bool changeAlpha = false, bool findCoordinates = false, bool removeAlpha = false)
        {
            // Überprüft, ob die Größe eines einzelnen Sprites kleiner oder gleich der Größe des gesamten Bitmaps ist.
            if (spriteSize.X > bitmap.Width || spriteSize.Y > bitmap.Height)
            {
                throw new ArgumentException("The sprite size must not be larger than the spritesheet size.");
            }

            // Konvertiert die Ziel-Farbe in ein ARGB-Integer-Format für einfache Vergleiche.
            int targetColorInt = targetColor.ToArgb();

            // Berechnet die Anzahl der Sprites in X- und Y-Richtung auf dem Spritesheet.
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);

            List<string> resultsList = new List<string>();
            List<string> errorImagePaths = new List<string>(); // Liste der Bildpfade mit dem Fehler

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
                    Point result = FindPixelInSprite(pixelData, bitmapData.Stride, spriteSize, targetColorInt, new Point(spriteSize.X * x, spriteSize.Y * y), removePixel, findCoordinates, bitmap.Tag.ToString());
                    if (findCoordinates)
                    {
                        if (result.X != -1 && result.Y != -1)
                        {
                            resultsList.Add(result.X + "," + result.Y);
                        }
                        else
                        {
                            // Fehler: Mehrere Pixel gefunden
                            errorImagePaths.Add(bitmap.Tag.ToString());
                        }
                    }
                }
            }
            if (changeAlpha)
            {

                ChangeAlphaToColor(pixelData, changeAlphaTo);
            }
            else if (removeAlpha)
            {
                Color transparent = Color.FromArgb(0, 0, 0, 0);
                ChangeAlphaToColor(pixelData, transparent);
            }

            // Kopiert die veränderten Pixeldaten zurück ins Bitmap.
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            // Optional: Speichert das veränderte Bitmap.
            if (removePixel || changeAlpha || removeAlpha)
            {
                SaveModifiedBitmap(bitmap);
            }

            // Gibt das Bitmap-Speicher frei.
            bitmap.Dispose();
            return (resultsList, errorImagePaths);
        }

        // Sucht das Pixel mit der Ziel-Farbe in einem einzelnen Sprite.
        private static Point FindPixelInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos, bool removePixel = false, bool findCoordinates = false, string imagePath = "")
        {
            Point result = new Point(0, 0);
            int foundPixelCount = 0; // Zähler für die Anzahl der gefundenen Pixel.

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
                        if (findCoordinates == true)
                        {
                            if (foundPixelCount > 0)
                            {
                                // Falls mehrere Pivots gefunden wurden, wird auf Gruppierung beding durch upscaling gesucht.
                                // Bei Gruppierung von 4 oder weniger Pivots wird der erste zurückgegeben, der gefunden wurde.
                                // Falls keine Gruppierung, wird ein falscher Wert zurückgegeben.
                                result = FindPixelGroupInSprite(pixelData, stride, spriteSize, targetColorInt, startPos);
                                 
                                return result; // Abbrechen und ungültiges Ergebnis zurückgeben.
                            }

                            result = new Point(x, y);
                        }

                        if (removePixel)
                        {
                            ReplacePixelWithDominantNeighboringColor(pixelData, stride, spriteSize, position, targetColorInt);
                        }
                        foundPixelCount++; // Inkrementieren des Zählers für gefundene Pixel.
                    }
                }
            }

            return result;
        }
        private static Point FindPixelGroupInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos)
        {
            int width = spriteSize.X;
            int height = spriteSize.Y;
            bool[,] visited = new bool[height, width];
            Point result = new Point(-1, -1); // Fehlerpunkt als Standardwert.

            Func<int, int, List<Point>> dfs = null;
            dfs = (x, y) =>
            {
                List<Point> component = new List<Point>();
                if (x < 0 || x >= width || y < 0 || y >= height || visited[y, x])
                    return component;

                int position = ((startPos.Y + y) * stride) + ((startPos.X + x) * 4);
                int color = BitConverter.ToInt32(pixelData, position);
                visited[y, x] = true;

                if (color == targetColorInt)
                {
                    component.Add(new Point(x, y));
                    foreach (var dir in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
                    {
                        component.AddRange(dfs(x + dir.Item1, y + dir.Item2));
                    }
                }
                return component;
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!visited[y, x])
                    {
                        var component = dfs(x, y);
                        if (component.Count > 0 && component.Count <= 4)
                        {
                            var minX = component.Min(p => p.X);
                            var minY = component.Min(p => p.Y);
                            // Korrektur: Addition von startPos.X und startPos.Y für die Rückgabe
                            return new Point(minX, minY);
                        }
                        else if (component.Count > 4)
                        {
                            // Rückgabe eines Fehlerpunkts mit Berücksichtigung der startPos
                            return new Point(-1, -1);
                        }
                    }
                }
            }

            return result;
        }

        // Speichert das veränderte Bitmap unter einem neuen Dateinamen.
        public static void SaveModifiedBitmap(Bitmap bitmap, string modifiedText = "_modified")
        {
            // Prüft, ob das Tag des Bitmaps einen gültigen Speicherpfad enthält
            if (bitmap.Tag == null || string.IsNullOrEmpty(bitmap.Tag.ToString()))
            {
                throw new InvalidOperationException("The bitmap tag does not contain a valid storage path.");
            }

            // Holt den ursprünglichen Pfad des Bildes aus dem Tag
            string originalPath = bitmap.Tag.ToString();
            // Bestimmt das Verzeichnis des Originalbildes
            string directory = Path.GetDirectoryName(originalPath);
            // Extrahiert den Dateinamen ohne Erweiterung
            string filename = Path.GetFileNameWithoutExtension(originalPath);
            // Extrahiert die Dateierweiterung
            string extension = Path.GetExtension(originalPath);
            // Erstellt den neuen Dateinamen mit dem Zusatztext
            string newFileName = Path.Combine(directory, filename + modifiedText + extension);

            // Speichert das Bitmap unter dem neuen Dateinamen als PNG
            bitmap.Save(newFileName, ImageFormat.Png);
        }

        // Ersetzt ein gefundenes Pixel durch die dominante Farbe der benachbarten Pixel, falls erforderlich.
        private static void ReplacePixelWithDominantNeighboringColor(byte[] pixelData, int stride, Point spriteSize, int position, int targetColorInt)
        {
            int[] neighboringColors = new int[4];
            int y = position / stride;
            int x = (position % stride) / 4;

            // Überprüfen der benachbarten Pixel und Sammeln ihrer Farben
            if (y > 0) neighboringColors[0] = BitConverter.ToInt32(pixelData, position - stride); // oben
            if (y < spriteSize.Y - 1) neighboringColors[1] = BitConverter.ToInt32(pixelData, position + stride); // unten
            if (x > 0) neighboringColors[2] = BitConverter.ToInt32(pixelData, position - 4); // links
            if (x < spriteSize.X - 1) neighboringColors[3] = BitConverter.ToInt32(pixelData, position + 4); // rechts

            // Filtere die targetColor aus den benachbarten Farben heraus
            var filteredColors = neighboringColors.Where(color => color != targetColorInt).ToArray();

            // Erstelle eine Gruppierung der benachbarten Farben, die nicht die targetColor sind
            var colorGroups = filteredColors.GroupBy(color => color).OrderByDescending(group => group.Count());

            // Versuche die erste Farbgruppe zu finden, die die meisten Elemente hat
            var dominantGroup = colorGroups.FirstOrDefault();

            int colorToSet = dominantGroup != null ? dominantGroup.Key : CalculateAverageColor(neighboringColors);

            // Setze die neue Farbe, falls eine dominante Gruppe vorhanden ist, oder die berechnete Durchschnittsfarbe
            BitConverter.GetBytes(colorToSet).CopyTo(pixelData, position);
        }

        // Berechnet den Durchschnittswert der Farben in einem Array.
        private static int CalculateAverageColor(int[] colors)
        {
            int r = 0, g = 0, b = 0; // Variablen zur Speicherung der summierten Farbkomponenten Rot, Grün, Blau.
            int count = 0; // Zähler für die Anzahl der Farben im Array.

            // Durchlaufen jedes Farbwertes im übergebenen Array.
            foreach (int color in colors)
            {
                r += (color >> 16) & 0xFF; // Extrahieren und Aufsummieren der Rot-Komponente.
                g += (color >> 8) & 0xFF;  // Extrahieren und Aufsummieren der Grün-Komponente.
                b += color & 0xFF;         // Extrahieren und Aufsummieren der Blau-Komponente.
                count++;                   // Inkrementieren des Zählers.
            }

            // Vermeidung der Division durch Null, falls das Array leer ist.
            if (count > 0)
            {
                r /= count; // Berechnen des Durchschnitts für Rot.
                g /= count; // Berechnen des Durchschnitts für Grün.
                b /= count; // Berechnen des Durchschnitts für Blau.
            }

            // Rückgabe des Durchschnittswerts als eine einzige Farbe im ARGB-Format.
            return (r << 16) | (g << 8) | b;
        }

        // Findet die Farbe, die einer Ziel-Farbe am nächsten liegt, aus einem Array von Farben.
        private static int FindNearestColor(int[] colors, int targetColor)
        {
            int minDistance = int.MaxValue; // Variable zur Speicherung der kleinsten gefundenen Distanz.
            int nearestColor = 0;           // Variable zur Speicherung der Farbe mit der kleinsten Distanz.

            // Durchlaufen aller Farben im Array.
            foreach (int color in colors)
            {
                int distance = ColorDistance(color, targetColor); // Berechnen der Distanz zur Ziel-Farbe.

                // Aktualisieren der minimalen Distanz und der nächsten Farbe, falls die aktuelle Distanz kleiner ist.
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestColor = color;
                }
            }
            return nearestColor; // Rückgabe der Farbe mit der geringsten Distanz zur Ziel-Farbe.
        }

        // Berechnet die Distanz zwischen zwei Farben im RGB-Farbraum.
        private static int ColorDistance(int color1, int color2)
        {
            // Extrahieren der Rot-, Grün- und Blau-Komponenten der ersten Farbe.
            int r1 = (color1 >> 16) & 0xFF;
            int g1 = (color1 >> 8) & 0xFF;
            int b1 = color1 & 0xFF;

            // Extrahieren der Rot-, Grün- und Blau-Komponenten der zweiten Farbe.
            int r2 = (color2 >> 16) & 0xFF;
            int g2 = (color2 >> 8) & 0xFF;
            int b2 = color2 & 0xFF;

            // Berechnen des quadratischen Abstands zwischen den Farben im RGB-Raum.
            return (r2 - r1) * (r2 - r1) + (g2 - g1) * (g2 - g1) + (b2 - b1) * (b2 - b1);
        }

        // Setzt alle Alpha-Werte zwischen 1 und 254 auf einen anderen Wert.
        private static void ChangeAlphaToColor(byte[] pixelData, Color targetColor)
        {
            // Durchlaufe alle Pixel im Bild
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                // Alpha-Wert des Pixels abrufen
                byte alpha = pixelData[i + 3];

                // auf Alpha-Wert prüfen
                if (alpha != 0 && alpha != 255)
                {
                    // Die Farbkomponenten auf die Ziel-Farbe setzen
                    pixelData[i] = targetColor.B;
                    pixelData[i + 1] = targetColor.G;
                    pixelData[i + 2] = targetColor.R;
                    pixelData[i + 3] = targetColor.A;
                }
            }
        }

        // Setzt Pixel in der gewünschten Farbe auf die Koordinaten der Liste.
        public static void SetPixelFromList(Bitmap bitmap, Point spriteSize, Color targetColor, List<Point> points)
        {
            // Ermittelt die Größe des Spritesheets
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Sperrt die Bitmap-Daten für den Schreib- und Lesezugriff
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Erhält die Adresse der ersten Zeile
            IntPtr ptr = bitmapData.Scan0;

            // Deklariert ein Array, um die Bytes der Bitmap zu halten
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];

            // Kopiert die RGB-Werte in das Array
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            int depth = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8; // Bytes pro Pixel

            // Berechnet die Anzahl der Sprites
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);
            int pointListPosition = 0;

            // Schleife über jede Zeile im Sprite-Raster
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                // Schleife über jede Spalte im aktuellen Raster der Zeile
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Überprüfe, ob der Punkt nicht am Ursprung (0,0) liegt
                    if (points[pointListPosition].X != 0 && points[pointListPosition].Y != 0)
                    {
                        // Berechne die X-Koordinate des Pixels in der Bitmap
                        int pixelX = (spriteSize.X * x) + points[pointListPosition].X;

                        // Berechne die Y-Koordinate des Pixels in der Bitmap
                        int pixelY = (spriteSize.Y * y) + points[pointListPosition].Y;

                        // Berechne die Position des Pixels im eindimensionalen Array 'rgbValues', das die Bitmap-Daten enthält
                        int position = (pixelY * bitmapData.Stride) + (pixelX * depth);

                        // Setze den Blauanteil des Pixels
                        rgbValues[position] = targetColor.B;

                        // Setze den Grünanteil des Pixels
                        rgbValues[position + 1] = targetColor.G;

                        // Setze den Rotanteil des Pixels
                        rgbValues[position + 2] = targetColor.R;

                        // Überprüfe, ob das Bildformat 32 Bits pro Pixel hat (RGBA), um den Alphawert zu setzen
                        if (depth == 4)
                        {
                            // Setze den Alphawert (Transparenz) des Pixels
                            rgbValues[position + 3] = targetColor.A;
                        }
                    }

                    // Erhöhe die Position in der Liste der Punkte
                    pointListPosition++;
                }
            }

            // Kopiert die RGB-Werte zurück in die Bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Entsperren der Bitmap-Daten
            bitmap.UnlockBits(bitmapData);
        }

    }
}


