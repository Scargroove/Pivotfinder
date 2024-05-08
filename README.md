Pivotfinder
Pivotfinder ist ein Tool, um Ankerpunkte (Pivots) für Sprites in einem Spritesheet auszurichten und die Koordinaten in einer Textdatei zu speichern. Diese Datei kann dann von einer Spiele-Engine gelesen werden, um zu bestimmen, wo Objekte wie z.B. Waffen dargestellt werden sollen. Zusätzlich ermöglicht das Tool die Entfernung von Ankerpunkten, das Verändern von Alpha-Werten und das Wiederauftragen von Pivots auf ein Spritesheet.


Hauptfunktionen

1. Save Pivots to Textfile
Diese Funktion speichert Pivot-Koordinaten in einer Textdatei. Der Pivotfinder sucht in jedem Abschnitt des Bildes (bestimmt durch die Spritesize) nach einem bestimmten Farbwert. Standardmäßig ist Magenta vordefiniert, kann jedoch durch „Set Pivot Color“ geändert werden.
•	Wenn mehrere Farbwerte innerhalb eines Abschnitts gefunden werden, überprüft der Pivotfinder, ob es sich um eine Gruppe von Pixeln unter 4 Pixeln handelt. Ist dies der Fall, wird der erste gefundene Pixel als Pivotpunkt gespeichert.
•	Wenn mehr Pixel vorhanden sind oder sie weit auseinander liegen, wird der Vorgang abgebrochen.

  Ergebnis:
  Die Koordinaten werden Zeile für Zeile in eine Textdatei gespeichert (x,y). Wenn kein Pivot gefunden wird, wird der Wert 0,0 geschrieben. Die Koordinaten beginnen bei 1,1.

2. Remove Pivots
Entfernt markierte Stellen und setzt einen neuen Pixel passend zur Farbumgebung. Es werden folgende Regeln verwendet:
•	Dominante Farbe: Wenn mindestens 2 gleiche Pixelnachbarn gefunden werden (oben, unten, links, rechts), wird deren Farbe genommen.
•	Durchschnittsfarbe: Wenn alle Pixelnachbarn unterschiedliche Farben haben, wird die Farbe des Pixels genommen, der dem Durchschnitt aller Nachbarn am nächsten kommt.

  Ergebnis:
  Es wird eine bearbeitete Kopie mit dem Suffix _modified erstellt.

3. Set Alpha to Fully Transparent
Setzt alle Pixel des Spritesheets im angegebenen Bereich auf voll transparent (alpha=0), behält die Farbe oder wechselt sie, falls angegeben.

  Ergebnis:
  Es wird eine bearbeitete Kopie mit dem Suffix _modified erstellt.

4. Set Alpha to Fully Opaque
Setzt alle Pixel des Spritesheets im angegebenen Bereich auf volle Deckung (alpha=255), behält die Farbe oder wechselt sie, falls angegeben.

  Ergebnis:
  Es wird eine bearbeitete Kopie mit dem Suffix _modified erstellt.

5. Pivots to Sprite
Der umgekehrte Vorgang zu „Save Pivots to Textfile“:
•	Wähle ein Spritesheet, ein Textfile und die Farbe, in der die Pivots gesetzt werden sollen.

  Ergebnis:
  Es wird eine bearbeitete Kopie mit dem Präfix pivots_ erstellt.

