"""Slice the crowd-face sheets and copy the faces into the game.

Crowd faces are walk-ons shown as faces only: the other players at the games (patrons)
and, later, the venues' staff (workers). They're drawn four to a sheet, a 2x2 grid of
square faces on flat light grey, and dropped in Images/Avatars/ as avatars-<n>.png.
Each face's id is its sheet and place on it, avatars-<n>_r<row>c<col>, so its picture
can be redrawn or its name changed without touching the other.

Images/Avatars/avatars.json is the roster: each face's id, name, type (patron or
worker), sheet, row, column and a short note of its look. This slices every sheet into
game/Assets/Resources/Faces/<id>.png and copies the roster there as faces.json, for the
game to read (Art.Face). A face on a sheet that's not in the roster yet gets an entry
with no name, as a patron; give it a name before it goes in a scene.

    python3 tools/import_faces.py             # slice, update the roster, copy
    python3 tools/import_faces.py --dry-run   # list what would change

Needs Pillow.
"""

import argparse
import io
import json
import re
from pathlib import Path

from PIL import Image

ROOT = Path(__file__).resolve().parent.parent
DROP = ROOT / "Images" / "Avatars"
ROSTER = DROP / "avatars.json"
OUT = ROOT / "game" / "Assets" / "Resources" / "Faces"
SHEET = re.compile(r"avatars-(\d+)\.png$")
GRID = 2
TYPES = {"patron", "worker"}


def faces(sheet):
    """The sheet's faces as (row, col, image), cut on an even 2x2 grid."""
    image = Image.open(sheet).convert("RGB")
    w, h = image.size
    if w != h or w % GRID:
        raise SystemExit(f"{sheet.name} is {w}x{h}; a face sheet is square, {GRID}x{GRID} faces.")
    size = w // GRID
    for row in range(1, GRID + 1):
        for col in range(1, GRID + 1):
            yield row, col, image.crop(((col - 1) * size, (row - 1) * size, col * size, row * size))


def png(image):
    buffer = io.BytesIO()
    image.save(buffer, "PNG", optimize=True)
    return buffer.getvalue()


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--dry-run", action="store_true", help="list what would change and write nothing")
    args = parser.parse_args()

    roster = json.loads(ROSTER.read_text())
    entries = {face["id"]: face for face in roster["avatars"]}
    for face in entries.values():
        if face.get("type") not in TYPES:
            raise SystemExit(f"{face['id']} has type {face.get('type')!r}; it must be one of {sorted(TYPES)}.")

    sheets = sorted((p for p in DROP.iterdir() if SHEET.match(p.name)), key=lambda p: int(SHEET.match(p.name)[1]))
    written, added = [], []
    if not args.dry_run:
        OUT.mkdir(parents=True, exist_ok=True)
    for sheet in sheets:
        n = SHEET.match(sheet.name)[1]
        for row, col, image in faces(sheet):
            face_id = f"avatars-{n}_r{row}c{col}"
            if face_id not in entries:
                entries[face_id] = {"id": face_id, "name": None, "type": "patron", "source": sheet.name,
                                    "row": row, "col": col, "look": ""}
                added.append(face_id)
            target = OUT / f"{face_id}.png"
            data = png(image)
            if target.exists() and target.read_bytes() == data:
                continue
            written.append(target.name)
            if not args.dry_run:
                target.write_bytes(data)

    missing = [i for i, f in entries.items() if not (DROP / f["source"]).exists()]
    if missing:
        print("In the roster with no sheet:", ", ".join(missing))

    roster["avatars"] = sorted(entries.values(), key=lambda f: (int(SHEET.match(f["source"])[1]), f["row"], f["col"]))
    text = json.dumps(roster, indent=2, ensure_ascii=False) + "\n"
    copy = OUT / "faces.json"
    roster_changed = ROSTER.read_text() != text
    copy_changed = not copy.exists() or copy.read_text() != text
    if not args.dry_run:
        if roster_changed:
            ROSTER.write_text(text)
        if copy_changed:
            copy.write_text(text)

    verb = "Would write" if args.dry_run else "Wrote"
    print(f"{verb} {len(written)} faces" + (f": {', '.join(written)}" if written else "") + ".")
    if roster_changed or copy_changed:
        print(f"{verb} the roster" + (" (new faces added)" if added else "") + ".")
    unnamed = [i for i, f in entries.items() if not f.get("name")]
    if unnamed:
        print("Name these in Images/Avatars/avatars.json:", ", ".join(unnamed))


if __name__ == "__main__":
    main()
