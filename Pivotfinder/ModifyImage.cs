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
        // Finds pixels of a specified color in a bitmap and can optionally remove these pixels.
        public static (List<string> resultsList, List<string> errorImagePaths) FindPixelInSpriteSheet(Bitmap bitmap, Point spriteSize, Color targetColor, Color changeAlphaTo, bool removePixel = false, bool changeAlphaToFullOpaque = false, bool findCoordinates = false, bool changeAlphaToFullTransparent = false, bool setNewAlphaColor = false, Point alphaRange = default)
        {
            // Ensure that the size of a single sprite is less than or equal to the size of the entire bitmap.
            if (spriteSize.X > bitmap.Width || spriteSize.Y > bitmap.Height)
            {
                throw new ArgumentException("The sprite size must not be larger than the spritesheet size.");
            }

            // Convert the target color to ARGB integer format for easy comparisons.
            int targetColorInt = targetColor.ToArgb();

            // Calculate the number of sprites horizontally and vertically on the spritesheet.
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);

            List<string> resultsList = new List<string>();
            List<string> errorImagePaths = new List<string>(); // List of image paths with errors

            // Lock the bitmap in memory for quick access.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte[] pixelData = new byte[bitmapData.Stride * bitmapData.Height];
            Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);

            // Iterate over each sprite on the spritesheet.
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Find the position of the desired pixel within the sprite.
                    Point result = FindPixelInSprite(pixelData, bitmapData.Stride, spriteSize, targetColorInt, new Point(spriteSize.X * x, spriteSize.Y * y), removePixel, findCoordinates, bitmap.Tag.ToString());
                    if (findCoordinates)
                    {
                        if (result.X != -1 && result.Y != -1)
                        {
                            if (result.X == 0 && result.Y == 0)
                            {
                                resultsList.Add(result.X + "," + result.Y);
                            }
                            else
                            {
                                resultsList.Add((result.X + 1) + "," + (result.Y + 1));
                            }
                        }
                        else
                        {
                            // Error: Multiple pixels found
                            errorImagePaths.Add(bitmap.Tag.ToString());
                        }
                    }
                }
            }

            // Change alpha values if required
            if (changeAlphaToFullOpaque || changeAlphaToFullTransparent)
            {
                ChangeAlpha(pixelData, changeAlphaTo, alphaRange, changeAlphaToFullTransparent, changeAlphaToFullOpaque, setNewAlphaColor);
            }

            // Copy the modified pixel data back into the bitmap.
            Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length);
            bitmap.UnlockBits(bitmapData);

            // Optionally, save the modified bitmap.
            if (removePixel || changeAlphaToFullOpaque || changeAlphaToFullTransparent)
            {
                SaveModifiedBitmap(bitmap);
            }

            // Release bitmap memory.
            bitmap.Dispose();
            return (resultsList, errorImagePaths);
        }

        // Searches for the pixel with the target color in a single sprite.
        private static Point FindPixelInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos, bool removePixel = false, bool findCoordinates = false, string imagePath = "")
        {
            Point result = new Point(0, 0);
            int foundPixelCount = 0; // Counter for the number of found pixels.

            // Search each pixel within the sprite.
            for (int y = 0; y < spriteSize.Y; y++)
            {
                int rowStart = (startPos.Y + y) * stride + startPos.X * 4;

                for (int x = 0; x < spriteSize.X; x++)
                {
                    int position = rowStart + x * 4;
                    int color = BitConverter.ToInt32(pixelData, position);

                    if (color == targetColorInt)
                    {
                        if (findCoordinates)
                        {
                            if (foundPixelCount > 0)
                            {
                                // If multiple pivots are found, search for grouping due to upscaling.
                                // Return the first found point if grouped within 4 or fewer pivots.
                                // Otherwise, return an incorrect value.
                                result = FindPixelGroupInSprite(pixelData, stride, spriteSize, targetColorInt, startPos);
                                return result;
                            }

                            result = new Point(x, y);
                        }

                        if (removePixel)
                        {
                            ReplacePixelWithDominantOrNearestAverageColor(pixelData, stride, spriteSize, position, targetColorInt);
                        }
                        foundPixelCount++; // Increment the count for found pixels.
                    }
                }
            }

            return result;
        }

        // Searches for groups of pixels and returns the coordinate of the first pixel if the group is under 4.
        private static Point FindPixelGroupInSprite(byte[] pixelData, int stride, Point spriteSize, int targetColorInt, Point startPos)
        {
            int width = spriteSize.X; // Width of the sprite
            int height = spriteSize.Y; // Height of the sprite
            bool[,] visited = new bool[height, width]; // Array to store visited pixels
            Point result = new Point(-1, -1); // Default error point if no suitable group is found

            Func<int, int, List<Point>> dfs = null; // Function for depth-first search
            dfs = (x, y) =>
            {
                List<Point> component = new List<Point>(); // List of points in the current component
                if (x < 0 || x >= width || y < 0 || y >= height || visited[y, x]) // Check bounds and if already visited
                    return component;

                int position = ((startPos.Y + y) * stride) + ((startPos.X + x) * 4); // Calculate position in byte array
                int color = BitConverter.ToInt32(pixelData, position); // Convert byte data to integer color
                visited[y, x] = true; // Mark point as visited

                if (color == targetColorInt) // Check if color matches the target color
                {
                    component.Add(new Point(x, y)); // Add point to component
                    foreach (var dir in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) }) // Explore in four directions
                    {
                        component.AddRange(dfs(x + dir.Item1, y + dir.Item2)); // Recursive DFS call
                    }
                }
                return component;
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!visited[y, x]) // Check if point has not been visited yet
                    {
                        var component = dfs(x, y); // Start DFS from this point
                        if (component.Count > 0 && component.Count <= 4) // Check component size
                        {
                            var minX = component.Min(p => p.X); // Find smallest X value
                            var minY = component.Min(p => p.Y); // Find smallest Y value
                            // Return the first found point of the component adjusted by startPos
                            return new Point(minX, minY);
                        }
                        else if (component.Count > 4) // If component is too large
                        {
                            // Return an error point
                            return new Point(-1, -1);
                        }
                    }
                }
            }

            return result; // Return result point if no suitable component is found
        }

        // Saves the modified bitmap under a new filename.
        public static void SaveModifiedBitmap(Bitmap bitmap, string modifiedText = "_modified")
        {
            // Check if the bitmap tag contains a valid storage path
            if (bitmap.Tag == null || string.IsNullOrEmpty(bitmap.Tag.ToString()))
            {
                throw new InvalidOperationException("The bitmap tag does not contain a valid storage path.");
            }

            // Retrieve the original path of the image from the tag
            string originalPath = bitmap.Tag.ToString();
            // Get the directory of the original image
            string directory = Path.GetDirectoryName(originalPath);
            // Extract the filename without extension
            string filename = Path.GetFileNameWithoutExtension(originalPath);
            // Extract the file extension
            string extension = Path.GetExtension(originalPath);
            // Create the new filename with the additional text
            string newFileName = Path.Combine(directory, filename + modifiedText + extension);

            // Save the bitmap under the new filename as a PNG
            bitmap.Save(newFileName, ImageFormat.Png);
        }

        // Replaces the searched pixel with nearby colors or average colors if all are different.
        private static void ReplacePixelWithDominantOrNearestAverageColor(byte[] pixelData, int stride, Point spriteSize, int position, int targetColorInt)
        {
            int[] neighboringColors = new int[4];
            int y = position / stride;
            int x = (position % stride) / 4;

            // Capture the colors of the surrounding pixels
            if (y > 0) neighboringColors[0] = BitConverter.ToInt32(pixelData, position - stride); // Top
            if (y < spriteSize.Y - 1) neighboringColors[1] = BitConverter.ToInt32(pixelData, position + stride); // Bottom
            if (x > 0) neighboringColors[2] = BitConverter.ToInt32(pixelData, position - 4); // Left
            if (x < spriteSize.X - 1) neighboringColors[3] = BitConverter.ToInt32(pixelData, position + 4); // Right

            // Filter out the target color from the surrounding colors
            var filteredColors = neighboringColors.Where(color => color != targetColorInt).ToArray();

            // Group colors to determine the dominant color
            var groupedColors = filteredColors.GroupBy(color => color)
                                              .Select(group => new { Color = group.Key, Count = group.Count() })
                                              .OrderByDescending(group => group.Count);

            // Determine the dominant color if available
            var dominantColor = groupedColors.FirstOrDefault(group => group.Count >= 2);

            int colorToSet;
            if (dominantColor != null)
            {
                // Use the dominant color if one is found
                colorToSet = dominantColor.Color;
            }
            else
            {
                // No dominant color found, calculate the average and find the nearest color
                int averageColor = CalculateAverageColor(filteredColors);
                colorToSet = filteredColors.OrderBy(color => ColorDistance(averageColor, color)).FirstOrDefault();
            }

            // Set the selected color at the current pixel
            BitConverter.GetBytes(colorToSet).CopyTo(pixelData, position);
        }

        // Calculates the average value of colors in an array of integer color values.
        private static int CalculateAverageColor(int[] colors)
        {
            // Initialize sum of red, green, and blue values.
            int r = 0, g = 0, b = 0;
            // Counter for the number of processed colors.
            int count = 0;
            // Iterate over each color integer in the array.
            foreach (int color in colors)
            {
                // Extract the red component and add it to the sum.
                r += (color >> 16) & 0xFF;
                // Extract the green component and add it to the sum.
                g += (color >> 8) & 0xFF;
                // Extract the blue component and add it to the sum.
                b += color & 0xFF;
                // Increment the counter by one for each processed color.
                count++;
            }
            // Divide the sums by the number of colors to calculate the average.
            if (count > 0)
            {
                r /= count;
                g /= count;
                b /= count;
            }
            // Combine the average red, green, and blue values into a single color integer.
            return (r << 16) | (g << 8) | b;
        }

        // Calculates the distance between two colors based on their RGB values.
        private static int ColorDistance(int color1, int color2)
        {
            // Extract the RGB components of the first color.
            int r1 = (color1 >> 16) & 0xFF;
            int g1 = (color1 >> 8) & 0xFF;
            int b1 = color1 & 0xFF;

            // Extract the RGB components of the second color.
            int r2 = (color2 >> 16) & 0xFF;
            int g2 = (color2 >> 8) & 0xFF;
            int b2 = color2 & 0xFF;

            // Calculate the squared distance between the two colors.
            return (r1 - r2) * (r1 - r2) + (g1 - g2) * (g1 - g2) + (b1 - b2) * (b1 - b2);
        }

        private static void ChangeAlpha(byte[] pixelData, Color targetColor, Point alphaRange, bool changeToFullTransparent, bool changeToFullOpaque, bool changeColor)
        {
            // Iterate over all pixels in the image
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                // Retrieve the blue, green, and red components and the alpha value of the pixel
                byte blue = pixelData[i];
                byte green = pixelData[i + 1];
                byte red = pixelData[i + 2];
                byte alpha = pixelData[i + 3];
                byte newAlpha;

                if (alpha >= alphaRange.X && alpha <= alphaRange.Y)
                {
                    if (changeToFullOpaque)
                    {
                        newAlpha = 255;
                    }
                    else if (changeToFullTransparent)
                    {
                        newAlpha = 0;
                    }
                    else
                    {
                        newAlpha = alpha;
                    }

                    if (changeColor)
                    {
                        pixelData[i] = targetColor.B;
                        pixelData[i + 1] = targetColor.G;
                        pixelData[i + 2] = targetColor.R;
                    }

                    pixelData[i + 3] = newAlpha;
                }
            }
        }

        // Sets pixels in the desired color at coordinates specified in a list.
        public static void SetPixelFromList(Bitmap bitmap, Point spriteSize, Color targetColor, List<Point> points)
        {
            // Determine the size of the spritesheet
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            // Lock bitmap data for read and write access
            BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Get the address of the first line
            IntPtr ptr = bitmapData.Scan0;

            // Declare an array to hold the bitmap bytes
            int bytes = Math.Abs(bitmapData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            int depth = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8; // Bytes per pixel

            // Calculate the number of sprites
            Point spriteAmount = new Point(bitmap.Width / spriteSize.X, bitmap.Height / spriteSize.Y);
            int pointListPosition = 0;

            // Loop over each row in the sprite grid
            for (int y = 0; y < spriteAmount.Y; y++)
            {
                // Loop over each column in the current grid row
                for (int x = 0; x < spriteAmount.X; x++)
                {
                    // Check if the point is not at the origin (0,0)
                    if (points[pointListPosition].X != 0 && points[pointListPosition].Y != 0)
                    {
                        // Calculate the X-coordinate of the pixel in the bitmap
                        int pixelX = (spriteSize.X * x) + points[pointListPosition].X;

                        // Calculate the Y-coordinate of the pixel in the bitmap
                        int pixelY = (spriteSize.Y * y) + points[pointListPosition].Y;

                        // Offset
                        pixelX = pixelX - 1;
                        pixelY = pixelY - 1;

                        // Calculate the pixel position in the one-dimensional array `rgbValues` that contains the bitmap data
                        int position = (pixelY * bitmapData.Stride) + (pixelX * depth);

                        // Set the blue component of the pixel
                        rgbValues[position] = targetColor.B;

                        // Set the green component of the pixel
                        rgbValues[position + 1] = targetColor.G;

                        // Set the red component of the pixel
                        rgbValues[position + 2] = targetColor.R;

                        // Check if the image format is 32 bits per pixel (RGBA) to set the alpha value
                        if (depth == 4)
                        {
                            // Set the alpha value (transparency) of the pixel
                            rgbValues[position + 3] = targetColor.A;
                        }
                    }

                    // Increase the position in the list of points
                    pointListPosition++;
                }
            }

            // Copy the RGB values back into the bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock bitmap data
            bitmap.UnlockBits(bitmapData);
        }
    }
}
