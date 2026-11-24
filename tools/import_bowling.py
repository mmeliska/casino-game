"""Bring the bowling art into the game: the lane's textures, the deck and the sprite sheet.

Everything is drawn in ChatGPT and saved in Images/Bowling/ (docs: Bowling, "Art"), and
written to game/Assets/Resources/Bowling/:

- The four lane textures (lane_boards and so on, TEXTURES), made to tile; see below.
- The deck from above (DECK), copied in as deck_top. Its spots are measured and printed, as
  shares of the picture: those numbers go in DeckView (HeadX, HeadY, InchAcross, InchDown) so
  the pins stand on the painted spots.
- The sprite sheet (SHEET): the pictures on flat magenta, cut out in reading order with the
  magenta keyed clear (the scratchers' cutter, tools/import_tickets.py), as SPRITES.

The lane is built as geometry and drawn with the four textures, which repeat down it. ChatGPT's "seamless" textures rarely meet exactly at their edges, so this blends
each one into itself across the edges it repeats over: it shifts the picture half its width
(or height), and fades from the shifted copy at the edges to the original in the middle, so
the left edge runs on into the right and the top into the bottom. Then it scales it down and
writes game/Assets/Resources/Bowling/<tag>.png, which the game imports as a repeating
texture (ArtImporter).

    python3 tools/import_bowling.py               # Images/Bowling/    -> Resources/Bowling
    python3 tools/import_bowling.py --style alt   # ImagesAlt/Bowling/ -> Resources/Styles/alt/Bowling
    python3 tools/import_bowling.py --dry-run     # say what would be written, and measure the deck

The masking panel is drawn as a band across a black picture; this cuts the band out and
keeps its shape.

Grain blended across itself can show a faint double line in the middle; if one does,
regenerate the picture rather than live with it.

Needs Pillow, NumPy and SciPy (for the deck's spots).
"""

import argparse
from pathlib import Path

import numpy as np
from PIL import Image

from import_tickets import cut_picture, pictures

ROOT = Path(__file__).resolve().parent.parent

# Tag -> (repeats across, repeats down, size written: width, height).
# The gutter only repeats down the lane: across, it runs from the lane's edge to the kickback.
# The masking panel only repeats across. It's drawn as a band on black: the band is cut out
# first (BANDS) and written this wide, as tall as its own shape makes it (height None).
TEXTURES = {
    "lane_boards": (True, True, (512, 512)),
    "lane_approach": (True, True, (512, 512)),
    "lane_gutter": (False, True, (256, 256)),
    "lane_masking": (True, False, (1024, None)),
}

BANDS = {"lane_masking"}

# A row darker than this on average (0 to 255) is the black round a band.
BLACK = 24

# The deck from straight above, as drawn -> its tag.
DECK = ("lanes_pin_deck.png", "deck_top")

# The sprite sheet, and its pictures in reading order (left to right, top row first).
SHEET = "bowling_sprites.png"
SPRITES = ["ball_house", "ball_urethane", "ball_reactive", "pin_up", "pin_down", "pin_top", "shoes"]

# Pin spots on a deck: 12 in apart across, rows 10.39 in apart down (PinDeck).
SPOT_INCHES_ACROSS = 12.0


def blend(pixels, axis):
    """Blend a picture into itself shifted by half along an axis, so it repeats along it."""
    n = pixels.shape[axis]
    shifted = np.roll(pixels, n // 2, axis=axis)
    # Weight of the original: 0 at the edges, 1 in the middle.
    weight = np.sin(np.linspace(0, np.pi, n)) ** 2
    shape = [1] * pixels.ndim
    shape[axis] = n
    weight = weight.reshape(shape)
    return pixels * weight + shifted * (1 - weight)


def cut_band(image):
    """The rows between the black above and below: the band a picture was drawn as."""
    rows = np.asarray(image.convert("L"), dtype=np.float64).mean(axis=1)
    lit = np.nonzero(rows > BLACK)[0]
    if len(lit) == 0:
        raise SystemExit("found no band: the picture is all black")
    return image.crop((0, int(lit[0]), image.width, int(lit[-1]) + 1))


def tile(source, across, down, size, band=False):
    image = Image.open(source).convert("RGB")
    if band:
        image = cut_band(image)
    if size[1] is None:
        size = (size[0], max(1, round(size[0] * image.height / image.width)))
    target_aspect = size[0] / size[1]
    w, h = image.size
    # Crop the middle to the shape it's written at, so nothing's stretched.
    if w / h > target_aspect:
        cw = round(h * target_aspect)
        image = image.crop(((w - cw) // 2, 0, (w - cw) // 2 + cw, h))
    elif w / h < target_aspect:
        ch = round(w / target_aspect)
        image = image.crop((0, (h - ch) // 2, w, (h - ch) // 2 + ch))
    pixels = np.asarray(image, dtype=np.float64)
    if across:
        pixels = blend(pixels, 1)
    if down:
        pixels = blend(pixels, 0)
    out = Image.fromarray(np.clip(pixels, 0, 255).astype(np.uint8))
    return out.resize(size, Image.LANCZOS)


def measure_deck(path):
    """
    The deck picture's head-pin spot, as shares of its width from the left and its height from
    the top, and an inch across and an inch down the lane as shares of its width: the dark
    round spots on the wood, the head pin the nearest, the back row four 12 in apart.
    """
    from scipy import ndimage

    rgb = np.asarray(Image.open(path).convert("RGB")).astype(int)
    h, w, _ = rgb.shape
    lum = rgb.mean(2)
    wood = (rgb[..., 0] > 150) & (rgb[..., 0] - rgb[..., 2] > 60)
    labels, _ = ndimage.label((lum < 60) & ndimage.binary_closing(wood, iterations=25))
    spots = []
    for i, box in enumerate(ndimage.find_objects(labels), 1):
        size = (labels[box] == i).sum()
        y0, y1 = box[0].start, box[0].stop
        x0, x1 = box[1].start, box[1].stop
        round_ = 0.6 < (y1 - y0) / max(1, x1 - x0) < 1.6
        if round_ and w * h * 0.0001 < size < w * h * 0.002:
            cy, cx = ndimage.center_of_mass(labels == i)
            spots.append((cx, cy))
    if len(spots) != 10:
        print(f"  {path.name}: found {len(spots)} spots, expected 10; measure it by hand")
        return
    spots.sort(key=lambda p: p[1])
    back, head = spots[:4], spots[-1]
    rows = sorted({round(y) for _, y in spots})
    across = (max(x for x, _ in back) - min(x for x, _ in back)) / (3 * SPOT_INCHES_ACROSS)
    down = (head[1] - np.mean([y for _, y in back])) / (3 * 10.39)
    print(f"  {path.name}: HeadX = {head[0] / w:.4f}f, HeadY = {head[1] / h:.4f}f, "
          f"InchAcross = {across / w:.6f}f, InchDown = {down / w:.6f}f  (DeckView)")


def folders(style):
    resources = ROOT / "game/Assets/Resources"
    if style == "base":
        return ROOT / "Images/Bowling", resources / "Bowling"
    return ROOT / f"Images{style.capitalize()}/Bowling", resources / f"Styles/{style}/Bowling"


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--style", default="base", help="art style: base (Images/) or e.g. alt (ImagesAlt/)")
    parser.add_argument("--dry-run", action="store_true", help="say what would be written without writing")
    args = parser.parse_args()

    source, target = folders(args.style)
    found = 0
    for tag, (across, down, size) in TEXTURES.items():
        path = source / f"{tag}.png"
        if not path.exists():
            print(f"not drawn yet  {path.relative_to(ROOT)}")
            continue
        found += 1
        dest = target / f"{tag}.png"
        print(f"{'would write' if args.dry_run else 'wrote'}  {path.relative_to(ROOT)}  ->  {dest.relative_to(ROOT)}")
        if not args.dry_run:
            dest.parent.mkdir(parents=True, exist_ok=True)
            tile(path, across, down, size, tag in BANDS).save(dest)
    print(f"{found} of {len(TEXTURES)} textures {'to write' if args.dry_run else 'written'}.")

    deck_name, deck_tag = DECK
    deck = source / deck_name
    if deck.exists():
        dest = target / f"{deck_tag}.png"
        print(f"{'would copy' if args.dry_run else 'copied'}  {deck.relative_to(ROOT)}  ->  {dest.relative_to(ROOT)}")
        measure_deck(deck)
        if not args.dry_run:
            dest.parent.mkdir(parents=True, exist_ok=True)
            Image.open(deck).convert("RGB").save(dest)
    else:
        print(f"not drawn yet  {deck.relative_to(ROOT)}")

    sheet = source / SHEET
    if not sheet.exists():
        print(f"not drawn yet  {sheet.relative_to(ROOT)}")
        return
    rgb = np.asarray(Image.open(sheet).convert("RGB")).astype(int)
    boxes = pictures(rgb)
    if len(boxes) != len(SPRITES):
        raise SystemExit(f"{SHEET}: found {len(boxes)} pictures, expected {len(SPRITES)}. "
                         "Check every picture stands clear of the others on plain magenta.")
    print(f"{SHEET}: {', '.join(SPRITES)}")
    if not args.dry_run:
        target.mkdir(parents=True, exist_ok=True)
        for box, tag in zip(boxes, SPRITES):
            cut_picture(rgb, box).save(target / f"{tag}.png")


if __name__ == "__main__":
    main()
