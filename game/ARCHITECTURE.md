# Architecture

How the Unity project's code is laid out, which way its dependencies point, who owns which
state, and the recipes for adding to it. These are the rules for new code; where existing
code falls short, the gap is listed under [Known gaps](#known-gaps). Settled 2026-11-06
(Mike); see the [decision log](#decision-log). It supersedes the two-layer `Core`/`Game`
layout in `REFACTOR_PLAN.md`, whose phases are otherwise done.

## The shape in one paragraph

The game is a set of modules under `Assets/Scripts`, each its own assembly. Each casino game
is a **vertical slice**: one folder with its rules, its table and its tests, so adding a game
doesn't touch another. Code that several modules share sits **below** them, at the lowest
folder that holds everything using it. The **playthrough** sits on top: it owns the save and
builds the **city**, which moves you between **places**, and each place seats you at games
and plays its own scenes. Dependencies only point down, and everything a module needs is
handed to it through `Init` or a constructor.

## Modules

| Folder | Assembly | Engine? | What's in it |
| --- | --- | --- | --- |
| `Core/` | `Starflare.Core` | no | The game state every module shares: `PlayerState`, `GameClock`, `Sitting`, `ClockPace`, `BreakPoint`, the hidden scales |
| `Ui/` | `Starflare.Ui` | yes | `UiKit` and the shared screen pieces (`Hud`, `ScreenFader`, `SafeAreaFitter`, `OrientationLayout`, `CoverFit`, `LookAround`, `Reveal`) and `ICoachable` |
| `Games/Shared/<Kit>/` | `Starflare.<Kit>` | no | Kits used across many games, one assembly each. Today `Cards` (`Card`, `Shoe`); craps will add `Dice` |
| `Games/<Game>/Rules/` | `Starflare.<Game>.Rules` | no | A game's rules: its round, its rule set, its hands |
| `Games/<Game>/Table/` | `Starflare.<Game>` | yes | The MonoBehaviour you sit at (`BlackjackTable`, `VideoPokerMachine`) |
| `Phone/` | `Starflare.Phone` | yes | The in-game phone: `PhoneMap` today, its other apps as they come |
| `Game/` | `Starflare.Game` | yes | Everything story-aware, by feature: `Playthrough/`, `City/`, `Places/`, `Story/`, `Save/`, `Art/`, `Debug/` |
| `Editor/` | `Starflare.Editor` | editor only | The **Starflare > Rebuild Project Setup** menu and art import settings |

Every module keeps its tests beside it: `<Module>/Tests/EditMode/` and
`<Module>/Tests/PlayMode/`, each its own assembly named `Starflare.<Module>.Tests.EditMode`
(or `.PlayMode`). A module without tests yet has no `Tests/` folder; add it with the first test.

"Engine? no" means the `.asmdef` sets `noEngineReferences: true`, so the compiler refuses any
`UnityEngine` API there. That is what keeps rules and state testable without the engine.

## Dependencies

```
4   Editor
3   Game                 Playthrough, City, Places, Story, Save, Art, Debug
2   Games/<Game>/Table   Phone
1   Games/<Game>/Rules   Ui
0   Games/Shared/<Kit>   Core
```

A module references only modules on lower layers, and only those the table below allows (a
game's `Rules` can't use `Ui`, for one: it has no engine):

| Assembly | May reference |
| --- | --- |
| `Core` | nothing |
| `Games/Shared/<Kit>` | nothing (or another kit, if it truly builds on it) |
| `Ui` | `Core` |
| `<Game>.Rules` | the kits it plays with |
| `<Game>` (table) | `Core`, `Ui`, its own `Rules`, its kits, uGUI, TextMeshPro |
| `Phone` | `Core`, `Ui`, uGUI, TextMeshPro, Input System |
| `Game` | all of the above, plus Ink |
| `Editor` | `Game` and what it needs to build the scene |

The rules behind the table:

- **References only point down.** Nothing references `Game`; nothing below `Game` knows the
  story, the save or the city.
- **A game never references another game.** If two games need the same thing, it moves into a
  kit under `Games/Shared/`. If the city needs it too, it moves to the top level (`Core` or
  `Ui`).
- **A game takes what the city has through `Init`.** The video poker machine doesn't load
  Kasia's bar plate; `KasiasTap` hands it the plate. A table gets its rules and chips from its
  venue the same way.
- **Kits hold nothing game-specific.** A card's blackjack count lives in blackjack
  (`Hand.CountOf`), not on `Card`.
- **The `.asmdef` lists every reference directly.** A module that uses a type from `Core` lists
  `Core`, even if something it references already does. `tools/compile_check.sh` gives each
  assembly only what its `.asmdef` names, so it fails when one is missing.

## Where code goes

Ask in order:

1. **Can you reason about it without a screen?** (a rule, a payout, a clock step) Then it goes
   in a no-engine assembly: the game's `Rules`, a kit, or `Core` if it's state every module
   shares. If you need `UnityEngine` there, the class is doing two jobs; split it so the
   MonoBehaviour is thin (a humble object).
2. **Who uses it?** Put it at the lowest folder that holds every user. One game: in that game.
   Several games: `Games/Shared/`. Games and the city or phone: `Core` or `Ui`. The city alone:
   `Game/`.
3. **Is it story-aware?** (reads Ink, plays a scene, knows a place or event code) Then it's in
   `Game/`, whatever else it does.
4. **Is it a feature beside the city rather than shared code?** (the phone) Then it's its own
   top-level module that the city drives through arguments and callbacks.

Inside `Game/`, the features:

- `Playthrough/`: the scene's root. Loads the save, owns the state and story, builds the city,
  saves, starts over, attaches the debug panel (`Playthrough.Debug.cs`).
- `City/`: the city layer of the core loop. Holds the places, opens the phone's map, runs the
  trip, draws the place screen and plays the scenes places share. It is the `IPlaceHost`. It
  never saves.
- `Places/`: one class per place implementing `IPlace`, listed once in `PlaceList.cs`. A place
  owns its screens, its scene triggers (the Slack Water owns the arrival and Dot's register,
  A1.1) and the games it seats you at.
- `Story/`: the Ink boundary (`StoryContract`, `StoryBridge`) and the scene player.
- `Save/`, `Art/`, `Debug/`: the save store, art lookup by tag, the debug panel and its
  checkpoints.

## State

There are four kinds of state. Each has one owner, and the kind decides where it lives and
whether it's saved. The test for which kind you have: *would losing it on a restart be a bug,
and do the game's rules read it?*

| Kind | Examples | Owner | Lives in | Saved? |
| --- | --- | --- | --- | --- |
| **Game state**: the truth about the playthrough | cash, the clock, where you are, the register, the scales, the story's flags | the `Playthrough` (`PlayerState` and the Ink story) | `Core` (`PlayerState`), Ink | yes, after every settled change |
| **In-progress game state**: a rule mid-flow | a blackjack hand being played, this sitting's minutes | the game module | the game's `Rules` | no: it settles into `PlayerState` (`ApplyRoundResult`) and a hand cut off by a quit is gone |
| **View state**: the truth about the screen right now | the fade's progress, the map's zoom, which panel is up, where you've panned a plate | the screen component that shows it | beside that screen (`Ui`, `Phone`, `Game`) | no |
| **Device settings** | the clock pace | the component that uses it | `PlayerPrefs` | per device, never in the save |

Design data (rules, paytables, table limits, plate framings, the story itself) isn't state: it's
read-only, lives in code constants and Ink, and never goes in the save.

The rules:

- **Game state changes only through its own methods**, and each change raises
  `PlayerState.Changed`. Never write a `PlayerState` field from outside it.
- **Only the `Playthrough` saves.** It saves on `PlayerState.Changed` and on the city's
  `SceneEnded` (a scene may have moved the story on), and on pause and quit. The city, places,
  games and screens never touch `SaveStore`; they raise an event and the playthrough acts.
- **No static mutable state.** Statics outlive a playthrough: `SceneManager.LoadScene` (start
  over, a debug reload) doesn't reset them. State belongs to an instance the playthrough or one
  of its children owns. Static readonly tables (design data) are fine.
- **View state never goes in `PlayerState`**, and game state never hides in a screen.

### Saves

- Two JSON files written together, the player state (`JsonUtility`) and the Ink story's own
  JSON, so the story's flags and the cash they refer to always agree.
- Every write is atomic: write a `.tmp` file, then move it over the old one.
- The player save carries a `Version`. A schema change bumps it and adds a migration step in
  `SaveStore.Migrate` with a fixture test. Never a silent reset; a newer save than the build
  understands is refused. Never `BinaryFormatter`.
- `JsonUtility` can't hold dictionaries, properties or polymorphic lists. When the model needs
  them (pockets, story keys, relationships), switch to Newtonsoft Json.NET
  (`com.unity.nuget.newtonsoft-json`), with a line in `README.md` saying why.

## How modules talk

- **Dependencies come in through `Init(...)` or a constructor.** No singletons, no static
  service locators, no `FindObjectOfType` in gameplay code. The one bootstrap lookup is
  `UiKit.MakeCanvas` finding the `EventSystem`.
- **Events go up.** A child tells its owner something happened with a C# event, and the owner
  decides what to do: `PlayerState.Changed`, `BreakPointReached` (a game's player got up),
  `City.SceneEnded`, `City.StartOverRequested`. Don't poll state in `Update` when an event can
  say it changed.
- **Callbacks answer one request.** When a caller asks for one thing and wants the answer,
  pass an `Action`: `PhoneMap.Show(..., go)`, `RegisterCard.Show(onSigned)`,
  `PlayScene(knot, then)`.
- **An interface is what a child sees of its parent**, and no more: `IPlaceHost` (a place's
  view of the city), `IDebugHost` (the panel's view of the playthrough), `ICoachable` (a
  teacher's view of a game).
- **Games never read story state.** A table reports each settled round through
  `PlayerState.ApplyRoundResult` and raises `BreakPointReached` when you get up. Story beats
  fire only at break points, never mid-game.
- **The Ink/C# boundary is `Game/Story`.** Every variable, external function and knot the C#
  side touches is a constant in `StoryContract`, and `StoryContractTests` checks them against
  the compiled story. No `variablesState["..."]` literals anywhere else.
- **Shared contracts get their own file** (`BreakPoint`, `IPlace`, `ICoachable`); nothing
  shared hides inside a module's file.

## Scene and lifecycle

- There is one scene, `Assets/Scenes/Main.unity`, generated by `ProjectSetup`: a camera and a
  `Playthrough` object. Any scene `ProjectSetup` doesn't produce is stale and gets deleted. The
  debug panel's checkpoints replace a test scene: they bring a new game to any beat.
- UI is built in code with `UiKit`, with a portrait and a landscape layout for every screen
  (`UiKit.OnTurn`). This is a deliberate exception to Unity's prefab workflow so the scene
  stays diffable.
- Boot: `Playthrough.Awake` loads the save and story and builds the city, which builds its
  screens and the places (`PlaceList`). `Start` calls `City.Begin`, which goes to the saved
  place; on a new game that's the Slack Water, whose `Arrive` plays the arrival.
- Restarting is rebuilding from the files: start over wipes the save, and a debug edit that
  changes what a screen offers saves first; then both reload the scene, which boots fresh from
  the save the way a player's next launch would. Nothing carries over in memory, which is why
  static state is banned.

## Recipes

### Adding a game

1. `Games/<Game>/Rules/` with `Starflare.<Game>.Rules.asmdef`: `noEngineReferences: true`,
   references the kits it plays with. Namespace `Starflare.<Game>`.
2. `Games/<Game>/Tests/EditMode/` with `Starflare.<Game>.Tests.EditMode.asmdef` (copy
   blackjack's). Test the rules as you write them.
3. `Games/<Game>/Table/` with `Starflare.<Game>.asmdef`: references `Starflare.Core`,
   `Starflare.Ui`, its `Rules`, its kits, `UnityEngine.UI`, `Unity.TextMeshPro`. Same namespace
   as the rules. The table:
   - takes its setup through `Init` (rules, chips, its backdrop plate) and you through
     `SitDown(PlayerState, ClockPace)`;
   - reports each settled round with `PlayerState.ApplyRoundResult` (story minutes from its
     `Sitting`);
   - raises `BreakPointReached` when you get up, and hides itself;
   - builds its own UI with `UiKit`, portrait and landscape;
   - implements `ICoachable` if a teacher will coach it.
4. Add `Starflare.<Game>` and `Starflare.<Game>.Rules` to `Starflare.Game.asmdef` (and its
   test assemblies if they use them), and seat the player at it from a place.
5. Run `tools/compile_check.sh`; it finds the new assemblies from their `.asmdef` files.

### Adding a kit

`Games/Shared/<Kit>/` with `Starflare.<Kit>.asmdef` (`noEngineReferences: true`, no
references), namespace `Starflare.<Kit>`, and `Tests/EditMode/`. Only things at least two
games use, and nothing specific to one of them.

### Adding a place

1. A class in `Game/Places/` implementing `IPlace`, built with the `IPlaceHost`. A plain class
   if it only shows screens and plays scenes (`MillstoneLanes`); a MonoBehaviour with `Init` if
   it owns components such as a table (`KasiasTap`). Components it needs are created in
   `PlaceList` on the playthrough's object and handed in (`SlackWaterMotel` gets its
   `RegisterCard`).
2. Its id from the place's docs page, and its pin from the pin table on the Northlight page.
3. One line in `PlaceList.Build`. `City.cs` doesn't change.
4. Its Ink file, included from `northlight.ink`; any variable or knot the code uses goes in
   `StoryContract`.
5. A checkpoint in `Debug/Checkpoints.cs` for each beat as it's built.

### Adding a phone app

In `Phone/` (a subfolder per app once there are a few: `Map/`, `Messages/`). The city opens it
with what to show and gets the player's choice back through a callback; the app never reads
the save or the story. If an app needs story-aware data, the city computes it and passes it in.

### Adding a module

Any new top-level module follows the same pattern: a folder with an `.asmdef` whose name is
the assembly and whose namespace matches it, references that obey the table above, and
`Tests/EditMode` and `Tests/PlayMode` when it has tests. Test assemblies:

- EditMode: `includePlatforms: ["Editor"]`, PlayMode: `includePlatforms: []`.
- Both: `overrideReferences: true` with `precompiledReferences: ["nunit.framework.dll"]`,
  `defineConstraints: ["UNITY_INCLUDE_TESTS"]`, `autoReferenced: false`, and references to the
  module under test plus `UnityEngine.TestRunner` (and `UnityEditor.TestRunner` for EditMode).
- Test namespace `Starflare.Tests`, so `-testFilter` names stay short.

Copy an existing test `.asmdef` rather than writing one from scratch.

## Tests

- Every rule or state change lands with an EditMode test in the module that owns it.
- A MonoBehaviour flow gets a PlayMode smoke test in its module's `Tests/PlayMode`. Flows that
  cross modules (a teacher coaching a table, a playthrough booting) go in `Game/Tests/PlayMode`.
  PlayMode tests that boot a playthrough set `Playthrough.SaveFolder` to a temp folder so a
  real save is never touched.
- Prefer `[Test]`; use `[UnityTest]` only when a test must yield.
- Run both suites headless before review (commands in the repo's `CLAUDE.md`). While the
  editor has the project open, `tools/compile_check.sh` compiles every assembly from its
  `.asmdef` with only its declared references. It catches compile errors and missing
  references but doesn't run tests.

## Unity mechanics

- **`.meta` files are part of the asset.** Unity tracks every asset, folders included, by the
  GUID in the `.meta` beside it, not by path. Move and delete them together (`git mv` both, or
  move in the editor), and commit a new folder's `.meta`. A `.meta` without its asset is
  deleted by Unity; an asset without one gets a new random GUID, which breaks references.
- **An `.asmdef` owns its folder and every subfolder** down to the next `.asmdef`. That is how
  a module's `Tests/` folder is its own assembly inside the module's folder.
- **Reserved folder names:** `Editor`, `Resources`, `Plugins`, `StreamingAssets`, `Gizmos`.
  Unity ignores folders starting with `.` or ending in `~`. Don't use these for module folders
  (`Editor/` here is governed by its `.asmdef`).
- **Names:** namespace = assembly (`Starflare.Ui`, `Starflare.Blackjack`); a game's rules and
  table share its namespace. One class per file, the file named for the class; a MonoBehaviour's
  file must match its class name for Unity to attach it.
- **Generated, not hand-edited:** the scene and player settings come from `ProjectSetup`. If
  you have to touch the scene file (as when renaming its root), make it match what
  `ProjectSetup` generates and run **Rebuild Project Setup** to confirm.
- **Loading:** art loads through `Art`, the story through `StoryBridge`, the map through
  `PhoneMap`; no other `Resources.Load`. Addressables replace `Resources` before the first store
  build (`REFACTOR_PLAN.md`, "Later").
- **Packages:** keep `Packages/manifest.json` to what the game uses; each addition gets a line in
  `README.md` saying why.

## Known gaps

- **`Art` holds static view state.** `Art.Looked` (where you've panned each view) is view state
  in a static field, so it outlives a playthrough and any code can reach it; it's only right
  after a restart because `City.Begin` calls `ForgetLooks`. Fix: a small framing object the city
  owns and hands to the screens that frame plates, leaving `Art` a stateless lookup.
  (`Art.Reported`, the once-only missing-art warnings, is harmless.)
- **`JsonUtility` limits** will bite with pockets, story keys and relationships; see Saves.
- **`Resources`** until Addressables (see Unity mechanics).
- **`Phone` has no tests yet**; add `Phone/Tests/` with its first rule.

## Decision log

All Mike, 2026-11-06:

1. **No standalone game scenes.** The blackjack test scene was deleted: it was a second owner of
   the save and wrote to the real save file. Debug checkpoints get you to any table.
2. **Vertical slices for games**, because many more games are coming: each game is
   `Games/<Game>/` with `Rules/`, `Table/`, `Tests/`, and tests live inside each module.
3. **Shared code at the lowest folder holding all its users.** Card code is a kit in
   `Games/Shared/Cards`, not top-level, because only games use it; `Games/Shared/` is for code
   used across many games.
4. **The phone is its own module**, `Phone/`, beside the city.
5. **`City` split in three.** `Playthrough` (the scene root, `Main.unity`) owns the save; `City`
   keeps places, travel and the place screen and never saves; the arrival and register moved to
   the Slack Water; the place list moved to `PlaceList`.
6. **`compile_check` reads the `.asmdef` files** so a new module needs no script change.
