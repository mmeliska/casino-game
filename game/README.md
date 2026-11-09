# Starflare Casino (Unity project)

The game itself. The build spec is the GDD at `docs/docs/pages/gdd.md`.

- **Unity:** 6000.6.7f1. Add iOS and Android Build Support in Unity Hub to build for phones; Play mode and Mac builds work without them.
- **Rendering:** URP (the Built-in pipeline is deprecated in Unity 6). The pipeline asset is in `Assets/Settings`.
- **Git LFS:** binary art and audio go through LFS (see `.gitattributes` at the repo root). Run `git lfs install` once per machine.

## Open and play

1. Unity Hub > Add > select this `game/` folder.
2. Open `Assets/Scenes/Main.unity` and press Play. Set the Game view to a portrait size, such as 1080x1920, or a landscape one, such as 1920x1080: every screen has both layouts and rearranges when the window turns. To jump to a beat, such as Rosa's table, use a checkpoint in the debug panel.

The scene and player settings are built from code: **Starflare > Rebuild Project Setup** regenerates them. Run it once after cloning: it also imports TextMesh Pro's essential resources into `Assets/TextMesh Pro/` (every label is TMP) and makes the default font dynamic so the card suits render.

## Layout

The code is a set of modules under `Assets/Scripts`, one assembly each, with each casino game a vertical slice (`Games/<Game>/` with its rules, its table and its tests). [`ARCHITECTURE.md`](ARCHITECTURE.md) has the module map, the dependency rules, who owns which state, how saves work, and the recipes for adding a game, kit, place or phone app.

Game modules never read story state or save. A table reports each settled round through `PlayerState.ApplyRoundResult`, which raises the state's `Changed` event (the playthrough saves on it), and raises `BreakPointReached` when you get up.

## Story and art

The story is Ink, through inkle's [ink-unity-integration](https://github.com/inkle/ink-unity-integration) package (2.0, pinned in `Packages/manifest.json`); each `.ink` imports straight into an `InkFile`. The game loads one story, `northlight.ink`, which includes each place's file (`slack_water.ink`, `kasias_tap.ink`, `millstone_lanes.ink`, `bridgehead.ink`), so every scene shares its variables. Every variable, EXTERNAL and knot the code touches is named once in `StoryContract`, and `StoryContractTests` compiles the story to check they all exist; `StoryBridge` binds the externals to the player state. They live in `Assets/Resources/Story/` and use the tags from the writing guide (`docs/docs/pages/writing.md`): `# plate:`, `# pose:`, `# insert:` and `# black`. A line reads `NAME: text` for dialogue and plain text for narration.

Each tag is also the image's file name under `Assets/Resources/Plates`, `Poses` or `Inserts`; they import as sprites automatically. Missing art is logged once and skipped, so a scene can be written before its art exists. Plates are 16:9 for now and fill a landscape screen as they are; a portrait screen crops them around a focus column (`Art.PlateFocus`); a 9:16 plate under the same tag needs no entry. A place screen names a view, such as `motel_lot`, and shows `<view>_day` in the morning and afternoon or `<view>_night` at night, in winter its snow plate when it has one: `<view>_day_winter` or `<view>_night_winter`, else `<view>_winter`, one snow plate for any time of day (Kasia's bar has just that one). A view with only a night plate shows it all day (`Art.PlateAt`). Snow plates go in as `_winter`. A view's variants share a framing, so `Art.PlateFocus` is keyed by view. A few views reuse another view's plates with their own focus (`Art.SharedPlates`): `motel_river` is the lot framed on the rocket sign across the river, for the arrival; the place screen frames the lot on the motel. The `_rain` plates are in, waiting for weather; they follow the same rule, so `kasia_bar_rain` covers the bar at any time of day.

Pose sheets are sliced with `tools/slice_sheet.py` (Pillow, NumPy and SciPy), which writes one transparent PNG per pose with a shared canvas, so pose swaps don't jump. List the tags left to right as the poses sit on the sheet:

```sh
python3 tools/slice_sheet.py Images/Characters/Kasia.png \
  kasia_asks kasia_stern kasia_joke kasia_neutral --out game/Assets/Resources/Poses
```

## Tests

Window > General > Test Runner, EditMode and PlayMode, or headless (add `-testPlatform PlayMode` for the second run):

```sh
/Applications/Unity/Hub/Editor/6000.6.7f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -projectPath game \
  -runTests -testPlatform EditMode -testResults TestResults.xml
```

## Debug panel

In the editor and in Development builds (tick **Development Build** in Build Profiles to get it on a phone), a small **DEV** button on the left edge opens the debug panel over any screen. Release builds don't show it: the playthrough adds it only when `Debug.isDebugBuild` (Unity 6.6 deprecates the `DEVELOPMENT_BUILD` define, so the code is compiled into every build and gated at run time).

- **Checkpoints** start a new game brought to a beat, e.g. "A1.3 Juan has the box" leaves you at the Tap with the bar-back shift next. Each is a recipe in `Checkpoints.cs`, built on the one before through the same `PlayerState` methods play uses, plus the story flags the beat sets. They're recipes rather than saved files so they keep working as the Ink changes, and `CheckpointTests` plays every one against the compiled story. Add one there as each beat is built.
- **Snapshots** copy your real save (both files) into `snapshots/` beside it, for "get me back to exactly here". They survive Start over, but they're the Ink's saved state, so a big story edit can make an old one stale; take a fresh one.
- **Live edits**: cash, Kasia's book, the register box (Mr., Ms. or Other, chosen above the checkpoints: each checkpoint signs with it, it switches the game you're in, and this device remembers it; it decides who asks first at the wedding), the clock (an hour on, on to the next part of the day, sleep) and its pace (story minutes per real minute, hours a sitting; this device remembers it), the four hidden scales, every true/false story variable, and where you stand. Money, clock and scales go through `PlayerState` and save at once. Flags and places change what a screen offers, so a place jump, or closing the panel after a flag change, saves and boots the game again from the save.

## Prototype scope

The arrival (A1.1): the motel lot at night with the rocket sign across the river, then Dot's desk. You sign her register (your name, used from then on, and Mr., Ms. or Prefer not to answer) and she asks what brings you to Northlight. From the lot you can step back into the office to talk with Dot or go to your room; on night one you can't sleep, and from then on your room is where you pass time: sleep 1, 2, 4 or 8 hours, or watch TV or browse the internet for 1, 2 or 3 hours. Walking into the Tap after night one, Kasia says hello and you're at the bar, where you can sit down at the machine.

The clock (`GameClock`): it runs straight through the hours, and the part of the day is read from the hour (before dawn from midnight, morning from 6:00, afternoon from noon, night from 6:00 p.m.); the date changes at midnight. Day 1 is Monday, May 4, and the top bar shows the weekday ("Sat, May 9, Year 1"). At a table or machine, real time played becomes story time: 30 story minutes a real minute, and at most 8 hours a sitting, after which you play on and the clock waits until you get up (`Sitting`, at the city's `ClockPace`; tune both on the debug panel). Time doesn't pass in scenes, on the map or in menus, while a teacher talks over a table, or while the app is in the background. Travel, jobs and sleep cost set amounts, and each place has a way to wait an hour or until the next part of the day: your room at the motel, a $3 beer at Kasia's, watching the lanes at Millstone.

The top bar (`Hud`): place screens and story scenes share a bar across the top with the place's name, the date (season, day and year) and time, your cash (and what you owe, once you owe), and the phone button. During a scene the buttons step away. Dialogue and place actions are dark glass panels with teal edges; a scene's choices sit under the dialogue box.

The map: every screen outside the games (each place) has a phone button that opens Northlight's map on your phone. Drag to pan, pinch or scroll to zoom (through the Input System package; the legacy Input Manager is off). Each place open to you is a pin, a circle with its name beside it, until the place icons exist; tap one to go there. Each place's pin is its `IPlace.Pin`, from the pin table on the Northlight page; the base map is `Assets/Resources/Map/city_map.png`, a copy of `Images/Map/city_map.png`. A trip on the map takes 30 in-game minutes.

Looking around: a portrait screen crops a place's plate, so a place screen in portrait has an eye button on the right, under the phone button. Tap it and the top bar and actions step aside; drag to pan the plate edge to edge, then tap the picture or the eye to come back, and the plate eases back to its framing (`LookAround`). Story scenes don't have it yet.

Night one at Kasia's Tap (A1.2): an Ink scene with Kasia, then the bar, where you can sit down at Jacks or Better on the full-pay 9/6 table, $1 a credit, 1 to 5 credits, a fresh deck every hand. Getting up is a break point where Kasia has a line for the moment: when you're broke she offers $50 on her book (no interest, no due date), and when you're up she asks you to settle it. You start with $200 on the night of day 1. The story state saves beside the player state.

Millstone Lanes (A1.3): the first time you walk into the Tap after night one, Kasia asks you to take Juan a box of drink coupons for his Tuesday league. Carrying the box puts Millstone Lanes on the map. You arrive in its lot, under the sign; going inside the first time plays Juan's scene, where he tells you about the Starflare across the river. After that, inside is a place screen where you can find Juan at the counter or step back outside. Its free-play machines and the late game out back (craps) aren't built yet.

Kasia's other job and the back room (A1.3): once Juan has his box, walking into the Tap offers the bar-back shift. It pays $60 cash and takes 4 hours; a regular drops a fat wallet, and you run it out to him (straight, flaky) or thumb out two twenties and hand it to Kasia (crooked, reliable), steps on the hidden scales (`PlayerState.Scales`, never shown). With both jobs done, Kasia nods you toward the back door, and the bar gets a "Go through the back door" action. Rosa deals blackjack there, $1 to $50 (`KasiasBackRoom`). Your first hand at her table is a practice hand (`PracticeHand`): your 18 against her ace, stacked, not from the shoe, $10 on the house, and it doesn't count (no money, no time). Rosa coaches it over the table (`StoryPlayer.PlayOver`): when a line is tagged `# point: <control>`, she and her text step aside, that button glows and is the only one that works (`ICoachable`, ids in `BlackjackControls`), and a Help button repeats her; pressing it carries the scene on. She has you deal, explains insurance and has you take it (it loses), has you stand, and tells you it's your call from then on; the shoe is fair after that. Getting up from her table is a break point with a line from Rosa.

The wedding (A1.4): in the same talk as the back door, Kasia asks you to work a mill family's wedding upstairs. Say yes and it's booked for the next Saturday (`GameClock.NextSaturday`, pushed to the story as `next_saturday`); that night, from 6:00 p.m., the bar has "Go up to the hall". You work it as server ($80 plus $40 in tips) or busser ($80 plus $15, and you overhear two mill uncles on the Starflare and a grandpa on the man behind Bridgehead's counter) until about 1:00 a.m. Mid-shift Kasia brings a dance offer: the opposite sex to the register box first; the other only if you look past the first one at them, which Kasia carries quietly; both at once for Prefer not to answer. Who you dance with is `danced_with` (and `dance_keen` if your apron was off before Kasia finished), for the romance characters later. Say no or miss the night and she asks or rebooks once (a no-show leans flaky); the second time it's over without you (`wedding_over`). The walk-ons' sheets (the maid of honor, the bride's brother) have a black frame round each panel, which the slicer reads as one pose; crop each panel inside its frame onto plain grey before slicing. Her cut used `--tolerance 90 --pad 4 --wisp 20 --pocket-min 60` plus a hand pass over small grey pockets; his used `--wisp 30 --tolerance 70 --halo 6 --pocket-min 60`.

Bridgehead Fuel & Food (A1.6, S3): once Juan has told you about the Starflare and the wedding is behind you (worked or not), Bridgehead is on the map, at the Backwater end of the Long Bridge. The first time you pull in plays the stop on the way over: the forecourt, then Mitch at the counter ("Pump two's temperamental. Has been since I started."). The Flare across the bridge isn't built yet. After that you arrive on the forecourt and go inside, where you can play the store's poker machine (Jacks or Better on a 6/5 table, the worst pay in town, $1 a credit), talk to Mitch, or nurse a $1 coffee. On a night after your first stop, the trucker short on diesel is at the pumps once (S3): spot him $20 (sets `helped_trucker` and the day) or don't. A scene's `# plate:` tag can name a view (`bridgehead_store`) instead of a whole plate, and the game picks its day, night or snow plate, as the place screen does. The slots, pull tabs, scratchers and Mitch's race book out back aren't built yet.

Also in the project: fair blackjack at the Flare's limits: six decks with a 75% cut card, dealer stands on soft 17, blackjack pays 3:2 (an odd bet's half dollar rounds down), hit, stand, double (also after a split) and one split; split aces get one card each; insurance against a dealer ace, at half the bet in whole dollars, paying 2:1. A $1 bet can't be insured, as at a real table with no half chips, and the felt says so ("Insurance needs a $2 bet"). The game autosaves after every settled round.

Not yet: Raffy's envelope (his portrait is in), table events, heat (the back room should raise it), debts, the place icons on the map, and the phone's other tabs.
