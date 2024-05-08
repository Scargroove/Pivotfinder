PivotFinder
PivotFinder is a tool designed to align anchor points (pivots) for sprites in a spritesheet and store the coordinates in a text file. This file can then be read by a game engine to determine where objects, such as weapons, should be displayed. Additionally, the tool enables the removal of anchor points, changing of alpha values, and reapplying pivots to a spritesheet.

Key Features
Save Pivots to Text File
This feature stores pivot coordinates in a text file. PivotFinder searches each section of the image (determined by sprite size) for a specific color value. By default, magenta is predefined, but it can be changed via the "Set Pivot Color" option.

If multiple color values are found within a section, PivotFinder checks if it's a cluster of fewer than 4 pixels. If so, the first found pixel is saved as the pivot point.
If more pixels are present or they're too far apart, the operation is aborted.
Result:
The coordinates are saved line by line in a text file (x,y). If no pivot is found, the value 0,0 is written. Coordinates start at 1,1.

Remove Pivots
Removes marked areas and replaces them with a new pixel matching the surrounding color environment. The following rules are used:

Dominant Color: If at least two identical neighboring pixels are found (top, bottom, left, right), their color is used.
Average Color: If all neighboring pixels have different colors, the pixel closest to the average color of the neighbors is used.
Result:
A modified copy with the suffix _modified is created.

Set Alpha to Fully Transparent
Sets all pixels in the specified area of the spritesheet to fully transparent (alpha=0), either retaining or changing the color if specified.

Result:
A modified copy with the suffix _modified is created.

Set Alpha to Fully Opaque
Sets all pixels in the specified area of the spritesheet to fully opaque (alpha=255), either retaining or changing the color if specified.

Result:
A modified copy with the suffix _modified is created.

Pivots to Sprite
The reverse operation of "Save Pivots to Text File":

Choose a spritesheet, a text file, and the color in which the pivots should be set.
At the end, a copy with the prefix pivots_ is created.
