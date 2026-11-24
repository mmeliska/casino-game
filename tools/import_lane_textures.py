"""Make the bowling lane's textures tile and put them in the game.

The lane at Millstone Lanes is built as geometry and drawn with four textures that repeat
down it (docs: Bowling, "Art"). They're drawn in ChatGPT and saved in Images/Bowling/ under
their tags. ChatGPT's "seamless" textures rarely meet exactly at their edges, so this blends
each one into itself across the edges it repeats over: it shifts the picture half its width
(or height), and fades from the shifted copy at the edges to the original in the middle, so
the left edge runs on into the right and the top into the bottom. Then it scales it down and
writes game/Assets/Resources/Bowling/<tag>.png, which the game imports as a repeating
texture (ArtImporter).

    python3 tools/import_lane_textures.py               # Images/Bowling/    -> Resources/Bowling
    python3 tools/import_lane_textures.py --style alt   # ImagesAlt/Bowling/ -> Resources/Styles/alt/Bowling
    python3 tools/import_lane_textures.py --dry-run     # say what would be written

The masking panel is drawn as a band across a black picture; this cuts the band out and
keeps its shape.

Grain blended across itself can show a faint double line in the middle; if one does,
regenerate the picture rather than live with it.

Needs Pillow and NumPy.
"""

import argparse
from pathlib import Path

import numpy as np
from PIL import Image

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


if __name__ == "__main__":
    main()
