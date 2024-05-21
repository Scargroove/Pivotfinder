# PivotFinder

PivotFinder is a Windows Forms application that provides various tools to process images. It allows users to find pivot points, remove specific pixels, change alpha values and reverse apply coordinates to spritesheets. This tool is particularly useful for game developers working with sprite-based graphics.

## Features

- **Find Pivot Points**: Identify pivot points using the specified pivot color and sprite size.
- **Remove Pivot Points**: Remove pivot points by changing them to the color of the dominant neighboring pixel.
- **Change Alpha Values**: Convert pixels with alpha values in the specified range to fully opaque, fully transparent, or a new color.
- **Apply Coordinates to Spritesheets**: Draws the pivots from a `.txt` file onto an image using the specified sprite size and pivot color options.

## Installation

1. Download the installer from the [release page](https://github.com/Scargroove/Pivotfinder/releases).

2. Run the downloaded installer (`PivotFinderSetup.exe`).

3. Follow the on-screen instructions to complete the installation.

## Usage

### Drag and Drop

You can drag and drop image files directly into the application window or specific areas like the PictureBox or ListBox. The application supports `.jpg`, `.jpeg`, `.png`, and `.bmp` file formats.

### Sprite Size

This value represents the size of each cell for the sprites in the spritesheet.

### Image Operations

1.**Find Pivot Points**:
   - Check the "find pivots" checkbox to identify pivot points with the specified pivot color and sprite size. The default pivot color is magenta.

2. **Remove Pixels**:
   - Check the "remove pivot" checkbox to remove pivot points by changing them to the color of the dominant neighboring pixel.


3. **Delete Images**:
   - Select images in the ListBox and press the "delete" key or click the "delete" button to remove them from the list.

4. **Change Alpha Values**:
   - Check the "change alpha" checkbox to convert alpha values within the specified range to fully opaque.
   - Check the "remove alpha" checkbox to convert alpha values within the specified range to fully transparent.
   - Check the "set new alpha color" checkbox and click the "set new color" button to change alpha values within the specified range to a new color.

5. **Apply Coordinates to Spritesheets**:
   - Click the "Draw Pivots to Image" button to draw the pivots from a `.txt` file onto an image using the specified sprite size and pivot color options.


### Logging

- The application logs various messages and processing times. The log appears at the end of the operation.

## License

This project is licensed under the GNU General Public License.


## Contact

For any questions or suggestions, please contact martinrieckhof@gmail.com.
