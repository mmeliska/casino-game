## About

Juan's struggling bowling alley. He bought it from the old owner on a handshake payment plan (Mike, 2026-10-30), keeps the ancient pinsetters running with spare parts and stubbornness, and runs dice out back after the last frame; the dice money pays for the parts. Inside, by the bar, a few licensed slots and video poker machines pay real money, like Kasia's (Mike, 2026-11-20; Backwater bars can run licensed terminals). It refuses Revaris cameras (Juan's dice and his regulars). Ed talks straight here.

| District | Games | Bets | Getting in | Who you'll find |
| --- | --- | --- | --- | --- |
| Backwater | Street craps in the back lot; inside, licensed bowling-themed slots and video poker, and the lanes themselves: open bowling for a fee, a league regular may challenge you to a game for money between games, and some Tuesdays a short team asks you to sub; otherwise league night is watch only. The counter sells balls and shoes (spec on [Bowling](#bowling); Mike, 2026-11-23). Built in the prototype (2026-11-23): "Bowl" inside, "Go to the counter" for Juan and what he sells | $1–$20 | Anyone; the late game runs 10 p.m. to 3 a.m., and the back lot is open any time (Mike, 2026-11-20); sweeps likelier when your heat is high | Juan, league regulars; Raffy hiding out later |

## Look and mood

The bowling-alley sign is one of Backwater's key lights. Inside: league night, worn lanes, old pinsetters. Out back: asphalt, a bare bulb, dice. If Umbra's push succeeds (R3), lane six goes dark; if a raid is traced to you, a sign about the pinsetters appears in the window.

## Plate prompts

For ChatGPT. A plate is a background with no people; an insert is a close-up of one of the place's props. The how-to, the avoid lists and the settings are on [Plate prompts](#plates). Each tag is also the image's file name and the scene's `# plate:` or `# insert:` tag.

**Proposed, not settled:** eight lanes; the alley's sign as a big blank bowling-pin shape; the shoe counter beside the machines; the back lot's chalk circle under a bulb. The late game is street craps on the asphalt, never a felt table (Mike, 2026-11-20).

**The back lot as drawn** (Mike, 2026-11-20): the chalk circle sits on a concrete pad under a lean-to roof on steel posts, by the steel back door and its bulb; the dice go off the alley's own brick wall. A dumpster, a drain grate, a chain-link fence and the lit mill beyond. Six plates: `lanes_backlot_day`, `_night`, each with `_rain` and `_winter`.

### Outside, the sign

Arriving at the Lanes; the coupons delivery (A1.3). Day and night, each in clear weather, rain and snow (`_rain`, `_winter`). Make the others from the night front.

`lanes_front_night` (plate, landscape 16:9)

```
Background plate, no people. A low old bowling alley building on a Backwater street, a tall vintage sign shaped like a bowling pin glowing above it, a small parking lot with puddles, a mill chimney in the distance. Timeless retro details. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### The lanes

League night, Juan at work, Raffy hiding out (A1.5, R3). Generate this first as the base for the lane-six variant.

`lanes_lanes_night` (plate, portrait 9:16)

```
Background plate, no people. Inside an old bowling alley: worn wooden lanes running away from the viewer, pins lit at the far end, retro scoring consoles and moulded seats in the foreground, a low ceiling with long light boxes. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### The lanes, lane six dark

After Umbra wins at the Lanes (R3): one lane's lights out. Make it from the lanes plate.

`lanes_lanes_night_lane6` (plate, portrait 9:16)

```
Background plate, no people. Inside an old bowling alley: worn wooden lanes running away from the viewer, pins lit at the far end except one lane that is completely dark, retro scoring consoles and moulded seats in the foreground. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### The shoe counter and the machines

Playing the machines; talking with Juan at the counter; Ed talking straight.

`lanes_counter_night` (plate, portrait 9:16)

```
Background plate, no people. An old bowling alley's front counter with cubbyholes of rental shoes behind it, a short row of glowing slot and video poker machines along the left wall, a worn carpet with a geometric pattern. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### Bowling: the lane

Not a plate: the lane is built as geometry and drawn with four repeating textures, `lane_boards`, `lane_approach`, `lane_gutter` and `lane_masking`, whose prompts are on [Bowling](#bowling) (Mike, 2026-11-24). This replaces `lanes_lane_pov`.

### Bowling: the deck from above

The close-up for the hit, looking straight down.

`lanes_deck_top` (insert, square 1:1)

```
Close-up insert panel, looking straight down from above onto the empty pin deck at the end of a bowling lane: polished pale wood with the ten pin spots marked as small dark dots in a triangle, point toward the bottom of the frame, the gutters down both sides, the dark steel kickback walls beyond them, and the black pit across the top edge. Flat top-down view, no perspective, the deck filling the square. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, No people, no pins, no ball, no text, no lettering.
```

### Behind the pinsetters

Juan's machine room behind the pins: where he fixes the pinsetters and where Raffy hides (R3), and where he shows you a weighted die.

`lanes_pinsetters_night` (plate, portrait 9:16)

```
Background plate, no people. A cramped machine room behind bowling pins: big old mechanical pinsetters with chains and gears, a workbench with tools and spare parts, a single caged work light. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### The back lot

The real-money dice after the last frame (A1.3, S1, R3).

`lanes_backlot_night` (plate, portrait 9:16)

```
Background plate, no people. A back lot behind a bowling alley: wet asphalt, a chalk circle on the ground against the foot of a low brick wall, a steel back door with a bare bulb above it, a dumpster, steam rising from a grate, a mill chimney beyond the fence. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### The loaded die

Juan shows you how to spot a weighted die (R3).

`lanes_dice` (insert, square 1:1)

```
Close-up insert panel. Two dice resting on wet asphalt by a chalk line, one slightly worn on a corner, lit by a bare bulb. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, no people, no text, no lettering; labels blank.
```
