# Starflare Casino (Unity project)

The game itself. The build spec is the GDD at `docs/docs/pages/gdd.md`.

- **Unity:** 6000.6.7f1. Add iOS and Android Build Support in Unity Hub to build for phones; Play mode and Mac builds work without them.
- **Rendering:** URP (the Built-in pipeline is deprecated in Unity 6). The pipeline asset is in `Assets/Settings`.
- **Git LFS:** binary art and audio go through LFS (see `.gitattributes` at the repo root). Run `git lfs install` once per machine.

## Open and play

1. Unity Hub > Add > select this `game/` folder.
2. Open `Assets/Scenes/City.unity` and press Play. Set the Game view to a portrait size, such as 1080x1920, or a landscape one, such as 1920x1080: every screen has both layouts and rearranges when the window turns. `Assets/Scenes/Blackjack.unity` is the older blackjack table.

The scene and player settings are built from code: **Starflare > Rebuild Project Setup** regenerates them.

## Layout

| Folder | Assembly | What's in it |
| --- | --- | --- |
| `Assets/Scripts/Core` | `Starflare.Core` | Pure C# with no engine references: cards and shoe, blackjack and video poker rules, player state and clock |
| `Assets/Scripts/Game` | `Starflare.Game` | By feature: `City/` (state and story owner, the phone's map, Dot's register), `Places/` (one class per place behind `IPlace`), `Story/` (the Ink contract, bridge and player), `Tables/` (video poker, blackjack), `Ui/` (`UiKit`, top bar, safe area, orientation), `Save/`, `Art/` |
| `Assets/Scripts/Editor` | `Starflare.Editor` | Project setup menu, art import settings |
| `Assets/Tests/EditMode` | `Starflare.Tests.EditMode` | NUnit tests for the rules and state |

Game modules never read story state or save. A table reports each settled round through `PlayerState.ApplyRoundResult`, which raises the state's `Changed` event (the city saves on it), and raises `BreakPointReached` when you get up. `Assets/Scenes/Blackjack.unity` is the table on its own, with `BlackjackScene` standing in for the city as the state's owner.

## Story and art

The story is Ink, through inkle's [ink-unity-integration](https://github.com/inkle/ink-unity-integration) package (2.0, pinned in `Packages/manifest.json`); each `.ink` imports straight into an `InkFile`. The game loads one story, `northlight.ink`, which includes each place's file (`slack_water.ink`, `kasias_tap.ink`, `millstone_lanes.ink`), so every scene shares its variables. Every variable, EXTERNAL and knot the code touches is named once in `StoryContract`, and `StoryContractTests` compiles the story to check they all exist; `StoryBridge` binds the externals to the player state. They live in `Assets/Resources/Story/` and use the tags from the writing guide (`docs/docs/pages/writing.md`): `# plate:`, `# pose:`, `# insert:` and `# black`. A line reads `NAME: text` for dialogue and plain text for narration.

Each tag is also the image's file name under `Assets/Resources/Plates`, `Poses` or `Inserts`; they import as sprites automatically. Missing art is logged once and skipped, so a scene can be written before its art exists. Plates are 16:9 for now and fill a landscape screen as they are; a portrait screen crops them around a focus column (`Art.PlateFocus`); a 9:16 plate under the same tag needs no entry. A place screen names a view, such as `motel_lot`, and shows `<view>_day` in the morning and afternoon or `<view>_night` at night, with `_winter` on the end in winter when that plate exists; a view with only a night plate shows it all day (`Art.PlateAt`). A view's variants share a framing, so `Art.PlateFocus` is keyed by view. A few views reuse another view's plates with their own focus (`Art.SharedPlates`): `motel_river` is the lot framed on the rocket sign across the river, for the arrival; the place screen frames the lot on the motel. The `_rain` plates are in, waiting for weather.

Pose sheets are sliced with `tools/slice_sheet.py` (Pillow, NumPy and SciPy), which writes one transparent PNG per pose with a shared canvas, so pose swaps don't jump. List the tags left to right as the poses sit on the sheet:

```sh
python3 tools/slice_sheet.py Images/Characters/Kasia.png \
  kasia_asks kasia_stern kasia_joke kasia_neutral --out game/Assets/Resources/Poses
```

## Tests

Window > General > Test Runner > EditMode > Run All, or headless:

```sh
/Applications/Unity/Hub/Editor/6000.6.7f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -projectPath game \
  -runTests -testPlatform EditMode -testResults TestResults.xml
```

## Prototype scope

The arrival (A1.1): the motel lot at night with the rocket sign across the river, then Dot's desk. You sign her register (your name, used from then on, and Mr., Ms. or Prefer not to answer) and she asks what brings you to Northlight. From the lot you can step back into the office to talk with Dot or go to your room; on night one you can't sleep, and from then on your room sleeps you till morning. Walking into the Tap after night one, Kasia says hello and you're at the bar, where you can sit down at the machine.

The top bar (`Hud`): place screens and story scenes share a bar across the top with the place's name, the date (season, day and year) and time, your cash (and what you owe, once you owe), and the phone button. During a scene the buttons step away. Dialogue and place actions are dark glass panels with teal edges; a scene's choices sit under the dialogue box.

The map: every screen outside the games (each place, and Kasia's break menu) has a phone button that opens Northlight's map on your phone. Drag to pan, pinch or scroll to zoom. Each place open to you is a pin, a circle with its name beside it, until the place icons exist; tap one to go there. Each place's pin is its `IPlace.Pin`, from the pin table on the Northlight page; the base map is `Assets/Resources/Map/city_map.png`, a copy of `Images/Map/city_map.png`. Walking doesn't take time yet.

Looking around: a portrait screen crops a place's plate, so a place screen in portrait has an eye button on the right, under the phone button. Tap it and the top bar and actions step aside; drag to pan the plate edge to edge, then tap the picture or the eye to come back, and the plate eases back to its framing (`LookAround`). Story scenes don't have it yet.

Night one at Kasia's Tap (A1.2): an Ink scene with Kasia, then the bar, where you can sit down at Jacks or Better on the full-pay 9/6 table, $1 a credit, 1 to 5 credits, a fresh deck every hand, 2 in-game minutes a hand. Getting up is a break point where Kasia has a line for the moment: when you're broke she offers $50 on her book (no interest, no due date), and when you're up she asks you to settle it. You start with $200 on the night of day 1. The story state saves beside the player state.

Millstone Lanes (A1.3): the first time you walk into the Tap after night one, Kasia asks you to take Juan a box of drink coupons for his Tuesday league. Carrying the box puts Millstone Lanes on the map. You arrive in its lot, under the sign; going inside the first time plays Juan's scene, where he tells you about the Starflare across the river. After that, inside is a place screen where you can find Juan at the counter or step back outside. Its free-play machines and the late game out back (craps) aren't built yet.

Also in the project: fair blackjack at the Flare's limits: six decks with a 75% cut card, dealer stands on soft 17, blackjack pays 3:2 (an odd bet's half dollar rounds down), hit, stand, double (also after a split) and one split; split aces get one card each. Each round takes 5 in-game minutes, up to the end of the part of the day. The game autosaves after every settled round.

Not yet: Raffy's envelope (it waits for his portrait), table events, heat, debts, the place icons on the map, and the phone's other tabs.
