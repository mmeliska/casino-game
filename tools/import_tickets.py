"""Cut the scratcher tickets out of their drop-folder pictures and put them in the game.

Each picture under Images/LotteryTickets/ is one ticket drawn twice on a grey table: the
portrait card on the left, the landscape card on the right. This finds both cards, makes
the grey round them (and any drop shadow) transparent so the rounded corners show, and
writes them as the ticket's art under its id (SOURCES):

    Resources/Scratchers/<id>.png        the portrait card, for a phone held upright
    Resources/Scratchers/<id>_wide.png   the landscape card, for a phone on its side

Each ticket's symbol sheet, under Images/LotteryTickets/Symbols/, is its pictures in a grid
on flat magenta, read left to right and top to bottom (SYMBOL_SHEETS). Each one is cut out,
the magenta keyed clear, and written as Resources/Scratchers/Symbols/<id>_<name>.png.

It also prints where each card's illustration ends and its flat colour begins, as a share
of the card's height from the top, because the game prints the name and the play area on
the flat colour and keeps the picture clear (TicketLook.Header, WideHeader).

    python3 tools/import_tickets.py             # cut, copy, and print the headers
    python3 tools/import_tickets.py --dry-run   # only measure

Needs Pillow and NumPy.
"""

import argparse
from collections import deque
from pathlib import Path

import numpy as np
from PIL import Image, ImageFilter

ROOT = Path(__file__).resolve().parent.parent
DROP = ROOT / "Images" / "LotteryTickets"
OUT = ROOT / "game" / "Assets" / "Resources" / "Scratchers"

# Drop-folder file -> ticket id (ScratchTicket.Bridgehead).
SOURCES = {
    "lucky_sevens.png": "lucky7s",
    "penny_jar.png": "pennyjar",
    "ice_fishing.png": "icefishing",
    "lake_effect.png": "lakeeffect",
    "northern_lights.png": "northernlights",
    "photo_finish.png": "photofinish",
    "gold_rush.png": "goldrush",
    "ore_boat.png": "oreboat",
    "diamond_deluxe.png": "diamonddeluxe",
    "cash_blizzard.png": "cashblizzard",
}


# Symbol sheet under Symbols/ -> (ticket id, picture names in reading order). The names are the
# symbols the ticket prints (Gold Rush's GOLD) or the look's MarkNames, KeyMarkName and
# BackingName (TicketLook.Bridgehead).
SYMBOL_SHEETS = {
    "gold_rush.png": ("goldrush", ["gold", "rock", "coal", "tin", "sand", "iron", "clay"]),
    "lucky_sevens.png": ("lucky7s", ["7", "cherry", "bell", "bar", "horseshoe", "clover", "lemon"]),
    "ice_fishing.png": ("icefishing", ["perch", "walleye", "pike", "bass", "lunker"]),
    "photo_finish.png": ("photofinish", ["red", "blue", "green", "yellow", "purple", "orange", "favorite"]),
    "backings.png": (None, ["pennyjar_penny", "lakeeffect_snowflake", "northernlights_star",
                            "oreboat_wheel", "diamonddeluxe_diamond", "cashblizzard_banknote"]),
}
SYMBOL_SIZE = 384


def runs(mask, shortest=40):
    """Stretches of True at least `shortest` long, as (start, end)."""
    out, start = [], None
    for i, v in enumerate(mask):
        if v and start is None:
            start = i
        elif not v and start is not None:
            out.append((start, i))
            start = None
    if start is not None:
        out.append((start, len(mask)))
    return [r for r in out if r[1] - r[0] >= shortest]


def table_colour(rgb):
    """The grey the cards lie on, from the picture's top and bottom edges."""
    edge = np.concatenate([rgb[:4].reshape(-1, 3), rgb[-4:].reshape(-1, 3)])
    return np.median(edge, axis=0)


def is_table(rgb, grey):
    """The table and the cards' shadows on it: near its grey, or a darker grey with no colour."""
    diff = np.abs(rgb - grey).sum(2)
    spread = rgb.max(2) - rgb.min(2)
    shade = rgb.mean(2)
    shadow = (spread < 14) & (shade < grey.mean() + 4) & (shade > grey.mean() - 90)
    return (diff < 36) | shadow


def cards(rgb, grey):
    """Each card's box (left, top, right, bottom), left to right."""
    away = ~is_table(rgb, grey)
    boxes = []
    for c0, c1 in runs(away.mean(0) > 0.05):
        rows = runs(away[:, c0:c1].mean(1) > 0.3)
        boxes.append((c0, rows[0][0], c1, rows[-1][1]))
    return boxes


def cut(rgb, grey, box):
    """One card on a transparent ground: the table that reaches in from the edges goes clear."""
    left, top, right, bottom = box
    pad = 6
    left, top = max(0, left - pad), max(0, top - pad)
    right, bottom = min(rgb.shape[1], right + pad), min(rgb.shape[0], bottom + pad)
    piece = rgb[top:bottom, left:right]
    table = is_table(piece, grey)
    h, w = table.shape

    clear = np.zeros_like(table)
    queue = deque()
    for x in range(w):
        for y in (0, h - 1):
            if table[y, x] and not clear[y, x]:
                clear[y, x] = True
                queue.append((y, x))
    for y in range(h):
        for x in (0, w - 1):
            if table[y, x] and not clear[y, x]:
                clear[y, x] = True
                queue.append((y, x))
    while queue:
        y, x = queue.popleft()
        for ny, nx in ((y - 1, x), (y + 1, x), (y, x - 1), (y, x + 1)):
            if 0 <= ny < h and 0 <= nx < w and table[ny, nx] and not clear[ny, nx]:
                clear[ny, nx] = True
                queue.append((ny, nx))

    # One pixel in from the table, softened, so no grey or shadow rims the corners.
    edge = Image.fromarray(np.where(clear, 0, 255).astype(np.uint8))
    alpha = np.asarray(edge.filter(ImageFilter.MinFilter(3)).filter(ImageFilter.GaussianBlur(0.6)))
    rows, cols = np.where(alpha > 0)
    y0, y1, x0, x1 = rows.min(), rows.max() + 1, cols.min(), cols.max() + 1
    rgba = np.dstack([piece.astype(np.uint8), alpha])[y0:y1, x0:x1]
    return Image.fromarray(rgba, "RGBA")


def header(card):
    """
    Where the picture ends and the flat colour starts, from the top, as a share of the
    card's height: the first row below which the inside of the card stays plain.
    """
    rgb = np.asarray(card.convert("RGB")).astype(float)
    h, w = rgb.shape[:2]
    inside = rgb[:, int(w * 0.12):int(w * 0.88)]
    busy = inside.std(1).mean(1)
    plain = np.median(busy[int(h * 0.7):int(h * 0.9)])
    limit = plain * 1.8 + 4
    # Walk up from the plain bottom until the rows stay busy.
    y = int(h * 0.85)
    while y > 0 and np.mean(busy[max(0, y - 6):y] > limit) < 0.5:
        y -= 1
    return y / h


def body_colour(card, top):
    """The flat colour the game prints on, from the middle of the card below the picture."""
    rgb = np.asarray(card.convert("RGB")).astype(float)
    h, w = rgb.shape[:2]
    patch = rgb[int(h * (top + (1 - top) * 0.3)):int(h * 0.88), int(w * 0.2):int(w * 0.8)]
    r, g, b = np.median(patch.reshape(-1, 3), axis=0) / 255
    return f"({r:.2f}, {g:.2f}, {b:.2f})"


def keyed(rgb):
    """
    The magenta ground: near-pure magenta, and the blend into each picture's outline just
    beside it. Purples and lilacs inside a picture (Northern Lights' star, the purple silks)
    are walled off by the outline, so they stay.
    """
    r, g, b = rgb[..., 0], rgb[..., 1], rgb[..., 2]
    ground = (r > 200) & (b > 200) & (g < 80) & (np.abs(r - b) < 60)
    tinged = (r > 110) & (b > 110) & (g < 120) & (np.abs(r - b) < 100)
    near = np.asarray(Image.fromarray(ground.astype(np.uint8) * 255).filter(ImageFilter.MaxFilter(5))) > 0
    return ground | (tinged & near)


def pictures(rgb):
    """Each picture's box on a symbol sheet, row by row, left to right."""
    away = ~keyed(rgb)
    boxes = []
    for r0, r1 in runs(away.mean(1) > 0.004, shortest=20):
        band = away[r0:r1]
        for c0, c1 in runs(band.mean(0) > 0.01, shortest=20):
            rows = np.where(band[:, c0:c1].any(1))[0]
            boxes.append((c0, r0 + rows[0], c1, r0 + rows[-1] + 1))
    return boxes


def cut_picture(rgb, box):
    """One picture on a clear ground: every magenta pixel goes, enclosed or not, and it's scaled to fit SYMBOL_SIZE."""
    left, top, right, bottom = box
    pad = 4
    piece = rgb[max(0, top - pad):bottom + pad, max(0, left - pad):right + pad]
    edge = Image.fromarray(np.where(keyed(piece), 0, 255).astype(np.uint8))
    alpha = np.asarray(edge.filter(ImageFilter.MinFilter(3)).filter(ImageFilter.GaussianBlur(0.6)))
    picture = Image.fromarray(np.dstack([piece.astype(np.uint8), alpha]), "RGBA")
    picture = picture.crop(picture.getbbox())
    picture.thumbnail((SYMBOL_SIZE, SYMBOL_SIZE), Image.LANCZOS)
    return picture


def import_symbols(dry_run):
    folder = DROP / "Symbols"
    if not folder.exists():
        print("No Symbols/ folder yet: no symbol sheets to cut.")
        return
    out = OUT / "Symbols"
    for path in sorted(folder.glob("*.png")):
        if path.name not in SYMBOL_SHEETS:
            print(f"  Symbols/{path.name}: no entry in SYMBOL_SHEETS, skipped")
    for name, (ticket, names) in SYMBOL_SHEETS.items():
        path = folder / name
        if not path.exists():
            continue
        rgb = np.asarray(Image.open(path).convert("RGB")).astype(int)
        boxes = pictures(rgb)
        if len(boxes) != len(names):
            print(f"  Symbols/{name}: found {len(boxes)} pictures, expected {len(names)}; skipped. "
                  "Check every picture stands clear of the others on plain magenta.")
            continue
        tags = [f"{ticket}_{n}" if ticket else n for n in names]
        print(f"Symbols/{name}: {', '.join(tags)}")
        if dry_run:
            continue
        out.mkdir(parents=True, exist_ok=True)
        for box, tag in zip(boxes, tags):
            cut_picture(rgb, box).save(out / f"{tag}.png")


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--dry-run", action="store_true", help="measure only; write nothing")
    args = parser.parse_args()
    import_symbols(args.dry_run)

    known = set(SOURCES)
    for path in sorted(DROP.glob("*.png")):
        if path.name not in known:
            print(f"  {path.name}: no entry in SOURCES, skipped")
    if not args.dry_run:
        OUT.mkdir(parents=True, exist_ok=True)

    print(f"{'ticket':16} {'portrait':>12} {'header':>7}   {'landscape':>12} {'header':>7}   body colour")
    for name, ticket in SOURCES.items():
        path = DROP / name
        if not path.exists():
            print(f"  {name}: missing")
            continue
        rgb = np.asarray(Image.open(path).convert("RGB")).astype(int)
        grey = table_colour(rgb)
        boxes = cards(rgb, grey)
        if len(boxes) != 2:
            print(f"  {name}: found {len(boxes)} cards, expected 2; skipped")
            continue
        tall, wide = (cut(rgb, grey, b) for b in boxes)
        if tall.width > tall.height or wide.width < wide.height:
            print(f"  {name}: expected the portrait card on the left; skipped")
            continue
        top = header(tall)
        print(f"{ticket:16} {tall.width:5}x{tall.height:<6} {top:7.3f}   "
              f"{wide.width:5}x{wide.height:<6} {header(wide):7.3f}   {body_colour(tall, top)}")
        if not args.dry_run:
            tall.save(OUT / f"{ticket}.png")
            wide.save(OUT / f"{ticket}_wide.png")


if __name__ == "__main__":
    main()
