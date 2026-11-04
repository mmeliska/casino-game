"""Slice a portrait expression sheet into one transparent PNG per pose.

The sheets come from the portrait prompts: poses side by side on a flat light-grey
background with clear gaps. Each pose is written as <tag>.png, and every pose from one
sheet shares the same canvas size and baseline, so swapping poses in a scene doesn't
jump or rescale the character.

    python3 tools/slice_sheet.py Images/Characters/Kasia.png \
        kasia_asks kasia_stern kasia_joke kasia_neutral \
        --out game/Assets/Resources/Poses

Tags are listed left to right as the poses appear on the sheet (not in prompt order).
A sheet with loose dark hair against the grey (Kasia's) leaves a pale haze between the
strands; add --wisp 20 for it. Leave it off for pale grey hair (Dot's), which it thins.
A sheet whose grey is uneven, with narrow gaps between panels (Raffy's), leaves grey
between the legs and a sliver of the next pose; add --tolerance 54 --pad 4 for it.
Needs Pillow, NumPy and SciPy.
"""

import argparse
from pathlib import Path

import numpy as np
from PIL import Image
from scipy import ndimage


def background_colour(rgb):
    h, w, _ = rgb.shape
    corners = [rgb[4:40, 4:40], rgb[4:40, w - 40:w - 4], rgb[h - 40:h - 4, 4:40], rgb[h - 40:h - 4, w - 40:w - 4]]
    return np.median(np.concatenate([c.reshape(-1, 3) for c in corners]), axis=0)


def pose_columns(ink, min_width):
    """Runs of columns that hold any ink, widest first trimmed to the poses."""
    used = ink.mean(axis=0) > 0.01
    runs, start = [], None
    for x, on in enumerate(used):
        if on and start is None:
            start = x
        elif not on and start is not None:
            runs.append((start, x))
            start = None
    if start is not None:
        runs.append((start, len(used)))
    return [r for r in runs if r[1] - r[0] >= min_width]


def cut_out(rgb, distance, background, tolerance, pocket_min, halo, wisp):
    """RGBA for one pose: the background reached from the border, plus enclosed pockets
    of background (between an arm and the body) that match it closely, cleared; then the
    light halo the generator leaves outside the ink line; then a soft edge against the ink,
    reaching into loose hair."""
    bg_like = distance < tolerance
    labels, count = ndimage.label(bg_like)
    border = set(np.unique(np.concatenate([labels[0], labels[-1], labels[:, 0], labels[:, -1]])))
    border.discard(0)
    remove = np.isin(labels, list(border))

    sizes = ndimage.sum(bg_like, labels, range(1, count + 1))
    tight = distance < tolerance / 2
    for i, size in enumerate(sizes, start=1):
        if i in border or size < pocket_min:
            continue
        pocket = labels == i
        if tight[pocket].mean() > 0.9:
            remove |= pocket

    # The generator sharpens its ink lines, which leaves a ring a pixel or two wide that is
    # lighter than the background just outside them. It reads as a white border on a dark
    # plate, so eat into anything no darker than the background, up to `halo` pixels in.
    lum = rgb.mean(axis=2)
    bg_lum = background.mean()
    for _ in range(halo):
        ring = ndimage.binary_dilation(remove) & ~remove & (lum > bg_lum - tolerance / 3)
        if not ring.any():
            break
        remove |= ring

    # Loose strands of hair are thin grey lines, ink blended with the background, with
    # background caught between them; kept whole they read as a pale haze on a dark plate.
    # Reach in through faint, colourless pixels like these, up to `wisp` pixels; a solid ink
    # line (or any colour) stops it, so it never gets inside the figure.
    chroma = rgb.max(axis=2) - rgb.min(axis=2)
    faint = (lum > 0.4 * bg_lum) & (chroma < 30)
    reach = remove.copy()
    for _ in range(wisp):
        grown = ndimage.binary_dilation(reach) & faint
        if not (grown & ~reach).any():
            break
        reach |= grown

    # The pixels left along the edge are ink blended with the background: take their alpha
    # from how dark they are, and unmix the background out of their colour.
    alpha = np.where(remove, 0.0, 1.0)
    band = ndimage.binary_dilation(reach) & ~remove
    alpha[band] = np.clip((bg_lum - lum[band]) / (0.5 * bg_lum), 0, 1)
    colour = rgb.copy()
    a = np.maximum(alpha[band], 1e-3)[:, None]
    colour[band] = np.clip((rgb[band] - (1 - a) * background) / a, 0, 255)
    colour[remove] = 0
    return np.dstack([colour, alpha * 255]).astype(np.uint8)


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("sheet", type=Path)
    parser.add_argument("tags", nargs="+", help="pose tags, left to right")
    parser.add_argument("--out", type=Path, required=True)
    parser.add_argument("--tolerance", type=float, default=36, help="colour distance still counted as background")
    parser.add_argument("--pocket-min", type=int, default=400, help="smallest enclosed background pocket to clear, in pixels")
    parser.add_argument("--halo", type=int, default=3, help="how far in, in pixels, to clear the light ring outside the ink line")
    parser.add_argument("--wisp", type=int, default=0, help="how far, in pixels, to soften loose dark hair; it also thins pale grey hair, so only for sheets that need it")
    parser.add_argument("--pad", type=int, default=16)
    args = parser.parse_args()

    rgb = np.asarray(Image.open(args.sheet).convert("RGB")).astype(np.float32)
    background = background_colour(rgb)
    distance = np.abs(rgb - background).sum(axis=2)
    ink = distance > args.tolerance

    columns = pose_columns(ink, min_width=rgb.shape[1] // 40)
    if len(columns) != len(args.tags):
        raise SystemExit(f"Found {len(columns)} poses on the sheet but got {len(args.tags)} tags.")

    rows = np.where(ink[:, columns[0][0]:columns[-1][1]].any(axis=1))[0]
    top, bottom = max(rows[0] - args.pad, 0), min(rows[-1] + args.pad + 1, rgb.shape[0])
    width = max(b - a for a, b in columns) + 2 * args.pad

    args.out.mkdir(parents=True, exist_ok=True)
    for tag, (left, right) in zip(args.tags, columns):
        x0, x1 = max(left - args.pad, 0), min(right + args.pad, rgb.shape[1])
        crop = rgb[top:bottom, x0:x1]
        pose = cut_out(crop, distance[top:bottom, x0:x1], background, args.tolerance, args.pocket_min, args.halo, args.wisp)

        canvas = np.zeros((bottom - top, width, 4), dtype=np.uint8)
        offset = (width - (x1 - x0)) // 2
        canvas[:, offset:offset + x1 - x0] = pose
        path = args.out / f"{tag}.png"
        Image.fromarray(canvas, "RGBA").save(path)
        print(f"{path}  {width}x{bottom - top}")


if __name__ == "__main__":
    main()
