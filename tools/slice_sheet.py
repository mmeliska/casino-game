"""Slice a portrait expression sheet into one transparent PNG per pose.

The sheets come from the portrait prompts: poses side by side on a flat light-grey
background with clear gaps. Each pose is written as <tag>.png, and every pose from one
sheet shares the same canvas size and baseline, so swapping poses in a scene doesn't
jump or rescale the character.

    python3 tools/slice_sheet.py Images/Characters/Kasia.png \
        kasia_asks kasia_stern kasia_joke kasia_neutral \
        --out game/Assets/Resources/Poses

Tags are listed left to right as the poses appear on the sheet (not in prompt order).
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


def cut_out(rgb, distance, tolerance, pocket_min):
    """Alpha for one pose: the background reached from the border, plus enclosed pockets
    of background (between an arm and the body) that match it closely, with a soft edge."""
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
        if tight[pocket].mean() > 0.95:
            remove |= pocket

    alpha = np.where(remove, 0.0, 1.0)
    band = ndimage.binary_dilation(remove, iterations=2) & ~remove
    alpha[band] = np.clip((distance[band] - tolerance / 4) / tolerance, 0, 1)
    return (alpha * 255).astype(np.uint8)


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("sheet", type=Path)
    parser.add_argument("tags", nargs="+", help="pose tags, left to right")
    parser.add_argument("--out", type=Path, required=True)
    parser.add_argument("--tolerance", type=float, default=36, help="colour distance still counted as background")
    parser.add_argument("--pocket-min", type=int, default=400, help="smallest enclosed background pocket to clear, in pixels")
    parser.add_argument("--pad", type=int, default=16)
    args = parser.parse_args()

    rgb = np.asarray(Image.open(args.sheet).convert("RGB")).astype(np.float32)
    distance = np.abs(rgb - background_colour(rgb)).sum(axis=2)
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
        alpha = cut_out(crop, distance[top:bottom, x0:x1], args.tolerance, args.pocket_min)

        canvas = np.zeros((bottom - top, width, 4), dtype=np.uint8)
        offset = (width - (x1 - x0)) // 2
        canvas[:, offset:offset + x1 - x0, :3] = crop.astype(np.uint8)
        canvas[:, offset:offset + x1 - x0, 3] = alpha
        path = args.out / f"{tag}.png"
        Image.fromarray(canvas, "RGBA").save(path)
        print(f"{path}  {width}x{bottom - top}")


if __name__ == "__main__":
    main()
