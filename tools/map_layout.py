"""Writes the city map layout sketch (a composition reference for the generator, no text)
and a pin check: the numbered pins drawn over a small copy of the final map, Images/Map/city_map.png.
The pins are measured on the final map and match the table under "The city map" on the Northlight page.

    sips -Z 1000 Images/Map/city_map.png --out /tmp/city_map_small.png
    python3 tools/map_layout.py Images/Map /tmp/city_map_small.png
    cd Images/Map && qlmanage -t -s 1000 -o . city_map_layout.svg city_map_pins.svg
"""
import base64, sys, os

out = sys.argv[1]
os.makedirs(out, exist_ok=True)

LAKE = ("M0,0 H1000 V1000 H790 C778,820 788,620 778,440 C772,320 748,230 690,198 "
        "C620,182 540,188 398,182 L352,182 C260,178 140,196 0,188 Z")
# the river turns west for a short stretch in line with the canal, then south again,
# so from the motel the river seems to run straight on east into the canal
RIVER = ("M375,170 C362,230 362,290 366,320 C369,342 358,350 335,350 L322,350 "
         "C302,350 296,368 296,400 C296,480 302,560 296,640 C290,760 302,880 292,1010")
# the highway along Backwater's western edge; country beyond it
BORDER = "100,188 112,400 95,600 108,800 104,1000"

ZONES = [  # name, fill, polygon
    ("Country", "#b9b46a", "0,188 " + BORDER + " 0,1000"),
    ("Backwater", "#c98a4b", BORDER.split(" ", 1)[0] + " 260,180 352,182 360,250 352,325 330,330 300,332 276,360 274,420 278,560 274,660 280,800 272,1000 "
     + " ".join(reversed(BORDER.split()[1:]))),
    ("The Levee", "#4f9a94", "320,350 660,350 660,1000 316,1000 322,880 318,720 324,560 318,420"),
    ("Marquee Mile", "#c2477f", "398,182 540,188 620,182 690,198 748,230 772,320 776,345 382,345 382,260"),
    ("Silvershore", "#3f6b4a", "660,345 776,345 778,440 788,620 778,820 790,1000 660,1000"),
]

PINS = [  # n, x, y, label: measured on the final map, Images/Map/city_map.png
    (1, 450, 318, "Slack Water Motel"), (2, 405, 298, "Kasia's Tap"), (3, 220, 440, "Millstone Lanes"),
    (4, 215, 650, "Backwater streets"), (5, 345, 322, "Your car"), (6, 300, 300, "Bridgehead"),
    (7, 255, 345, "Loan &amp; Pawn"), (8, 375, 357, "Long Bridge"), (9, 545, 372, "The Flare / Field"),
    (10, 345, 600, "Prairie Queen"), (11, 460, 535, "Cinder Room"), (12, 585, 780, "RailYard"),
    (13, 420, 395, "First Levee Bank"), (14, 650, 440, "Civic hall"), (15, 455, 455, "Room over a bar"),
    (16, 545, 610, "Levee streets"), (17, 620, 650, "Union hall"), (18, 680, 380, "Lock Street"),
    (19, 590, 200, "Harborlight"), (20, 690, 240, "The strip"), (21, 800, 275, "Aurelian"),
    (22, 870, 315, "Vivian's land"), (23, 810, 430, "Obsidian"), (24, 830, 640, "Silvershore"),
    (25, 940, 560, "Yachts"), (26, 325, 930, "South pier"), (27, 535, 985, "Northern Ledger"),
    (28, 30, 155, "Gilded Pine"), (29, 30, 440, "Blackspruce"), (30, 40, 905, "Haybale Downs"),
]


def pin_check(base_png):
    with open(base_png, "rb") as f:
        data = base64.b64encode(f.read()).decode()
    p = ['<svg xmlns="http://www.w3.org/2000/svg" width="1000" height="1000" viewBox="0 0 1000 1000">',
         f'<image href="data:image/png;base64,{data}" width="1000" height="1000"/>']
    for n, x, y, label in PINS:
        p.append(f'<circle cx="{x}" cy="{y}" r="13" fill="#fff" stroke="#111" stroke-width="2"/>')
        p.append(f'<text x="{x}" y="{y + 5}" font-family="Helvetica" font-size="14" font-weight="bold" '
                 f'text-anchor="middle">{n}</text>')
        anchor, dx = ("end", -17) if x > 880 or (230 < x < 350 and y < 400) else ("start", 17)
        p.append(f'<text x="{x + dx}" y="{y + 5}" font-family="Helvetica" font-size="13" font-weight="bold" '
                 f'text-anchor="{anchor}" fill="#fff" stroke="#000" stroke-width="3" paint-order="stroke">{label}</text>')
    p.append("</svg>")
    return "\n".join(p)


def svg(labelled):
    p = ['<svg xmlns="http://www.w3.org/2000/svg" width="1000" height="1000" viewBox="0 0 1000 1000">',
         '<rect width="1000" height="1000" fill="#d9cfb8"/>']
    for name, fill, pts in ZONES:
        p.append(f'<polygon points="{pts}" fill="{fill}" fill-opacity="0.55" stroke="#222" stroke-width="2"/>')
    p.append(f'<path d="{LAKE}" fill="#3b6f9e" stroke="#222" stroke-width="3"/>')
    # whitecaps hint that the lake is open water to the horizon
    for x, y in [(150, 80), (380, 60), (640, 90), (860, 160), (900, 420), (880, 760), (930, 900)]:
        p.append(f'<path d="M{x},{y} q12,-10 24,0 q12,-10 24,0" fill="none" stroke="#dfe9f2" stroke-width="3"/>')
    p.append(f'<path d="{RIVER}" fill="none" stroke="#3b6f9e" stroke-width="44" stroke-linecap="round"/>')
    # canal from the river to the lake with its lock: the Mile's southern border,
    # so every way onto the Mile is a bridge
    p.append('<path d="M360,346 H778" stroke="#3b6f9e" stroke-width="14"/>')
    p.append('<rect x="515" y="336" width="10" height="18" fill="#222"/>')
    for x in (440, 720):
        p.append(f'<path d="M{x},328 V362" stroke="#222" stroke-width="8"/>')
    # harbour breakwater and the North Light
    p.append('<path d="M408,182 L460,108" stroke="#555" stroke-width="8" stroke-linecap="round"/>')
    p.append('<circle cx="462" cy="104" r="10" fill="#f4e7a1" stroke="#222" stroke-width="2"/>')
    # river bridges: the top one onto the Mile and three lesser ones south; the Long Bridge,
    # the main crossing, spans the westward stretch at the bend by the motel
    for x, y in [(368, 255), (297, 500), (297, 700), (297, 870)]:
        p.append(f'<path d="M{x - 50},{y} H{x + 50}" stroke="#222" stroke-width="6"/>')
    p.append('<path d="M335,318 V384" stroke="#222" stroke-width="14"/>')
    # the border highway, with a tree line along its country side
    p.append(f'<polyline points="{BORDER}" fill="none" stroke="#222" stroke-width="8"/>')
    for y in range(230, 1000, 45):
        p.append(f'<circle cx="82" cy="{y}" r="9" fill="#4d6b3a" stroke="#222" stroke-width="1.5"/>')
    # fields in the country
    for x, y in [(8, 470), (45, 530), (8, 650), (45, 720), (8, 830)]:
        p.append(f'<rect x="{x}" y="{y}" width="32" height="32" fill="#d8c27a" stroke="#7a6a3a" stroke-width="2"/>')
    # the Stacks
    for dx in (0, 14, 28, 42):
        p.append(f'<rect x="{513 + dx}" y="490" width="7" height="30" fill="#222"/>')
    # rail line off the bottom edge, roads out through the country
    p.append('<path d="M560,810 C570,880 580,940 580,1000" stroke="#222" stroke-width="4" stroke-dasharray="10 6"/>')
    p.append('<path d="M240,215 C160,212 80,220 0,215" stroke="#7a5a3a" stroke-width="6" fill="none"/>')
    p.append('<path d="M230,420 C160,400 80,385 0,380" stroke="#7a5a3a" stroke-width="6" fill="none"/>')
    p.append('<path d="M230,820 C170,880 100,930 50,1000" stroke="#7a5a3a" stroke-width="6" fill="none"/>')
    # Vivian's empty lot on the shore at the canal mouth
    p.append('<rect x="722" y="300" width="40" height="40" fill="#9fbf7a" stroke="#222" stroke-width="2"/>')
    if labelled:
        for name, _, pts in ZONES:
            xs = [float(v.split(",")[0]) for v in pts.split()]
            ys = [float(v.split(",")[1]) for v in pts.split()]
            cx, cy = sum(xs) / len(xs), sum(ys) / len(ys)
            p.append(f'<text x="{cx:.0f}" y="{cy:.0f}" font-family="Helvetica" font-size="26" font-weight="bold" '
                     f'fill="#111" fill-opacity="0.35" text-anchor="middle">{name}</text>')
        for n, x, y, label in PINS:
            p.append(f'<circle cx="{x}" cy="{y}" r="13" fill="#fff" stroke="#111" stroke-width="2"/>')
            p.append(f'<text x="{x}" y="{y + 5}" font-family="Helvetica" font-size="14" font-weight="bold" '
                     f'text-anchor="middle">{n}</text>')
            anchor, dx = ("end", -17) if x > 820 or (180 < x < 290 and y < 600) else ("start", 17)
            p.append(f'<text x="{x + dx}" y="{y + 5}" font-family="Helvetica" font-size="13" '
                     f'text-anchor="{anchor}" fill="#111">{label}</text>')
    p.append("</svg>")
    return "\n".join(p)


with open(os.path.join(out, "city_map_layout.svg"), "w") as f:
    f.write(svg(False))
if len(sys.argv) > 2:
    with open(os.path.join(out, "city_map_pins.svg"), "w") as f:
        f.write(pin_check(sys.argv[2]))
