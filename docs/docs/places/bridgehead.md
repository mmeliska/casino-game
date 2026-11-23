## About

A gas station at the main bridge: worst odds in town, best gossip. Mitch is at the counter, and his race book is out back. The machines and scratchers are licensed; the race book isn't (Mike, 2026-11-06). Its video poker is Jacks or Better on a 6/5 table (a full house pays 6, a flush 5), the worst pay in town, printed on the glass (Mike, 2026-11-07).

**Mitch's back room.** His Haybale Downs race book (events, not a game). It opens up as your straight/crooked scale allows. The bad tip that ruined him came from Bennett (R5).

| District | Games | Bets | Getting in | Who you'll find |
| --- | --- | --- | --- | --- |
| The Bridges | Cheap three-reel slots, video poker, pull tabs, scratchers | $1–$5 | Anyone | Mitch at the counter (and his race book out back) |

**The slots** (Mike, 2026-11-22): a cheap three-reel called Triple Bar, plain and old, no theme: cherries, single, double and triple BAR, and a red 7, with a blank between every symbol. Three rows show in the window and the one payline runs across the middle. $1 a credit, 1 to 3 credits. It pays back 86% at one credit and 88% at three, the worst machine in town (the 6/5 poker beside it is about 95%); it pays something about one pull in four and a half, and three 7s come up once in 10,648 pulls. The glass, per credit:

| Line | 1 credit | 2 | 3 |
| --- | --- | --- | --- |
| 7 7 7 | 200 | 400 | 1,200 |
| Triple bar x3 | 80 | 160 | 240 |
| Double bar x3 | 40 | 80 | 120 |
| Single bar x3 | 20 | 40 | 60 |
| Cherry x3 | 20 | 40 | 60 |
| Cherry x2 | 10 | 20 | 30 |
| Any bar x3 | 5 | 10 | 15 |
| Cherry | 2 | 4 | 6 |

Cherries count from the left reel. Every stop on a reel is equally likely and the strip you see is the strip that spins, so the odds are what the reels say. Bulbs run round the reel window: a slow chase while it waits, a fast one while the reels turn, all of them flashing on a win, and the halves against each other for three 7s.

**In the game** (Mike, 2026-11-07). It opens on the map once Juan has told you about the Starflare and the wedding is behind you. The first stop is the start of A1.6; the trucker (S3) is at the pumps on a night after it. Its pin sits right by the northwest end of the Long Bridge, on the Backwater bank.

## Look and mood

Gas-station canopy fluorescents: flat white and oil-slick purple against the black. The Long Bridge's lights are right behind it. The trucker short on diesel (S3) is at these pumps.

## Plate prompts

For Leonardo.ai or Adobe Firefly. A plate is a background with no people; an insert is a close-up of one of the place's props. The how-to, the negative prompts and the settings are on [Plate prompts](#plates). Each tag is also the image's file name and the scene's `# plate:` or `# insert:` tag.

**Proposed, not settled:** the store's layout.

### The forecourt at night

Stopping on the way over the bridge (A1.6); the trucker short on diesel (S3). Day and night, each in clear weather, rain and snow (`_rain`, `_winter`). Make the others from the night forecourt.

`bridgehead_forecourt_night` (plate, landscape 16:9)

```
Background plate, no people. A small gas station forecourt under a flat white fluorescent canopy, two old pumps, oil stains on the concrete, a convenience store window glowing behind, the lights of a long steel bridge rising into the dark behind it. Night: flat white gas-station fluorescents against black. Palette: flat white, oil-slick purple. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### The store at night

Mitch's counter, the cheap slots and the gossip. Like the forecourt, in clear weather, rain and snow (`_rain`, `_winter`), the weather showing through the front windows.

`bridgehead_store_night` (plate, landscape 16:9)

```
Background plate, no people. Inside a small gas-station convenience store: a counter with a lottery scratcher rack across the back, a short row of cheap three-reel slot machines along the left wall, snack aisles, a coffee station, fluorescent tubes overhead. Night: flat white gas-station fluorescents against black. Palette: flat white, oil-slick purple. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### The store by day

Daytime visits. Make it from the night store.

`bridgehead_store_day` (plate, landscape 16:9)

```
Background plate, no people. Inside a small gas-station convenience store in daytime: a counter with a lottery scratcher rack across the back, a short row of cheap slot machines along the left wall, snack aisles, grey daylight through the front windows. Flat, overcast daylight. Palette: flat white, oil-slick purple, grey. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```

### The slot machine's cherries

The Triple Bar's cherry symbol. The 7 and the BARs are lettering, so the game draws them itself; only the cherries want a picture. Without it the game draws its own. Drop the finished picture in as `game/Assets/Resources/Symbols/triplebar_cherry.png`, on a transparent background.

`triplebar_cherry` (symbol, square 1:1)

```
Slot machine reel symbol, isolated. A pair of glossy red cherries on two green stems joined at the top with one small leaf, centred, filling most of the square. Bold black ink outlines, cel shading, one white highlight on each cherry, flat colours, a little worn like an old printed reel strip. Plain white background, nothing else in frame. No text, no lettering.
```

### Mitch's back room

Mitch's Haybale Downs race book (R5); Mitch recognizing the lawyer (A3.7).

**Drawn** (Mike, 2026-11-23): `Images/Locations/bridge_head_fuel/backroom.png`, landscape 16:9 rather than the portrait the prompt asks for; a portrait screen centres on the chalkboard and desk. The doorway on the right opens straight into the store behind the counter, with the register beyond it, so the way in is past Mitch. The chalkboard came out lettered, not blank: "Race 4", 1 Night Shift 2-1, 2 Copper Mile 5-2, 3 River Ghost 4-1, 4 Last Call 8-1. The radio sits on the desk beside a green cash box. One plate for any time of day: the room has no window.

`bridgehead_backroom_night` (plate, portrait 9:16)

```
Background plate, no people. A cramped stockroom behind a gas station: a metal desk with a cash box and an old radio, a chalkboard of horse-race odds on the wall with only blank chalk marks, cases of motor oil, a single hanging bulb. Night: flat white gas-station fluorescents against black. Palette: flat white, oil-slick purple. Gritty graphic-novel background art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic perspective. Eye-level view from standing height, open floor in the lower middle of the frame. No people, no text, no lettering; signs blank.
```
