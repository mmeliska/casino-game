# ImagesAlt

The drop folder for the second art style, `alt`. It mirrors `Images/`: put each picture
under the same folder and file name as the one it replaces there (for example
`Locations/kasias_tap/inside_night.png`). Only redrawn pictures are needed; the game shows
the base art for anything missing here.

- Plates: `python3 tools/import_plates.py --style alt` copies `Locations/` into
  `game/Assets/Resources/Styles/alt/Plates` under each plate's tag.
- Poses: slice a character sheet into `Styles/alt/Poses` with `tools/slice_sheet.py ... --out
  game/Assets/Resources/Styles/alt/Poses`, listing the tags left to right as they sit on this
  sheet.

Switch styles in the game's debug panel, under **Art style**.
