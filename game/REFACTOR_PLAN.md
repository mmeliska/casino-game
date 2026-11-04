# Architecture refactor plan

Written 2026-11-04 from a survey of `game/Assets/Scripts` (about 4,200 lines, 30 files) against
current Unity 6 guidance. The standards themselves are in the repo's `CLAUDE.md`
("Code standards"); this file is the gap list and the order of work. Nothing here has been
done yet. Each phase is a separate PR, reviewed before the next starts.

## What the research said

Checked against Unity's own documentation for 6000.6 where it exists, and community practice
where it doesn't.

- **uGUI is still the recommended runtime UI for Unity 6.6**, with UI Toolkit as the
  alternative ([Comparison of UI systems](https://docs.unity.com/en-us/engine/6000.6/manual/uitoolkits/ui-system-compare.md)).
  uGUI is recommended for "easy referencing from MonoBehaviours", which is how every screen
  here works. **Keep uGUI. Do not migrate to UI Toolkit.**
- **Text:** TextMeshPro ships inside the `com.unity.ugui` 2.x package and is the scalable,
  signed-distance-field text path. The `UnityEngine.UI.Text` component the project uses is the
  legacy one (it even loads `LegacyRuntime.ttf`). On phones it blurs when scaled.
- **Input:** the legacy Input Manager (`Input.touchCount`, `StandaloneInputModule`) is documented
  as deprecated; new Unity 6 projects get the Input System package by default
  ([Input manual](https://docs.unity3d.com/6000.0/Documentation/Manual/input-introduction.html)).
- **Assets:** the `Resources` folder is fine for prototyping but Unity's performance guidance
  says production projects should usually use Addressables; everything in `Resources` ships in
  the build and is indexed at startup ([Resources at runtime](https://docs.unity3d.com/Manual/LoadingResourcesatRuntime.html)).
- **Assemblies and tests:** split code into assembly definitions, keep an Editor-only assembly,
  keep pure logic engine-free so it runs as EditMode tests, use PlayMode tests for
  MonoBehaviours and scenes, prefer NUnit `[Test]` over `[UnityTest]` unless you need to yield
  ([Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.4/manual/edit-mode-vs-play-mode-tests.html)).
- **Architecture:** Unity's "Level up your code with design patterns" e-book covers SOLID,
  Observer, State, Command, Factory, object pooling and MVP; the ScriptableObject e-book pushes
  data out of MonoBehaviours into data assets. Community guidance converges on: no singletons,
  explicit dependencies, event channels between systems, feature folders, one `.editorconfig`.
- **Saves:** JsonUtility is enough until you need dictionaries or polymorphism; never
  BinaryFormatter; put a schema version in the file and migrate raw JSON forward; on mobile
  save at every settled change, not at quit.
- **CI:** GameCI's `unity-test-runner` GitHub Action runs EditMode and PlayMode tests on every
  PR; it needs a Unity license stored as a repo secret and benefits from caching `Library/`.

## What is already right

Keep these; they match the guidance.

- Four assemblies with `Starflare.Core` engine-free and fully unit-tested. This is the humble
  object pattern done properly.
- Scene and player settings generated from code, so there is nothing hand-edited to drift.
- Game modules never read story state; story beats fire only at break points.
- Atomic save writes (write to `.tmp`, then swap); only settled state is saved.
- Explicit `Init(...)` wiring instead of singletons or `FindObjectOfType` in gameplay code.
- Portrait and landscape layouts as a first-class rule (`UiKit.OnTurn`).

## Gaps, by severity

| # | Gap | Where | Why it matters |
| --- | --- | --- | --- |
| 1 | Game modules save directly (`SaveStore.Save` inside `VideoPokerMachine.Draw`, seven calls in `BlackjackTable`) | `VideoPokerMachine.cs:117`, `BlackjackTable.cs` | A table owns persistence it shouldn't know about; the city already saves at scene ends. Two owners of the same file. |
| 2 | No save schema version or migration step | `PlayerState.cs`, `SaveStore.cs` | The first field rename after release silently resets players. Cheap now, expensive later. |
| 3 | Story contract scattered as magic strings | `City.cs`, `KasiasTap.cs` (seven Ink variable names, four external functions) | A typo in Ink or C# fails at run time. The Ink/C# boundary should be one file. |
| 4 | `City` is the places, the navigation, the save owner and the arrival flow | `City.cs` (288 lines; `ShowMotel`, `ShowTapOutside`, `ShowLanes`, `GoTo` if/else chain) | Every new place grows this file. The GDD has a dozen places. |
| 5 | Legacy `Text` component | `UiKit.MakeText` plus `Text` fields in six screens | Blurry on high-density phones; TMP is the maintained path. |
| 6 | Legacy Input Manager | `PhoneMap.cs:88-98`, `UiKit.cs:190` | Deprecated API; pinch zoom is hand-rolled. |
| 7 | Hud, OrientationLayout, SafeAreaFitter, CoverFit, PhoneMap poll every frame | five `Update`/`LateUpdate` methods | Wasted battery on a screen that changes a few times a minute. State should raise a change event. |
| 8 | No tests outside `Core`; no CI | `Assets/Tests` | Save round trip, Ink external bindings and the arrival flow are untested. Nothing runs tests on a PR. |
| 9 | `BreakPoint` enum lives inside `BlackjackTable.cs` | `BlackjackTable.cs:12` | A shared contract hidden in one module's file. |
| 10 | Stale `Assets/Scenes/KasiasTap.unity` | not generated by `ProjectSetup` | Hand-edited scene that the rebuild step doesn't own. |
| 11 | ~35 unused built-in modules in `Packages/manifest.json` (cloth, terrain, vehicles, XR, wind, AI, physics...) | `manifest.json` | Build size and startup time on phones. |
| 12 | No `.editorconfig` | repo root | Style is consistent by habit, not enforced. |
| 13 | `Resources` folder for all art and story | `Art.cs`, `PhoneMap.cs`, `City.cs` | Acceptable for the prototype (Unity says so). Needs a planned exit before the first store build. |

## Phases

Each phase ends with: EditMode tests green headless, **Starflare > Rebuild Project Setup**,
and a manual play of the arrival through to sitting down at the Tap, in portrait and in
landscape. No phase changes player-facing behaviour unless it says so.

### Phase 0: hygiene (small, no behaviour change)

- Add `.editorconfig` at the repo root matching the current style: 4 spaces, `_camelCase`
  private fields, no `private` keyword, `PascalCase` everything else, LF endings, file-scoped
  `sealed` classes.
- Move `BreakPoint` to `Core/State/BreakPoint.cs`. Game modules already treat it as a shared
  contract.
- Delete `Assets/Scenes/KasiasTap.unity` (or add it to `ProjectSetup` if it is still wanted).
- Trim `Packages/manifest.json` to the modules the game uses: ui, uielements, imgui (editor),
  imageconversion, jsonserialize, audio, animation, screencapture, unitywebrequest set, plus
  URP, uGUI, Ink, test framework and IDE packages. Verify with a Mac build.
- No CI for now (Mike, 2026-11-04). Tests run headless before review; GameCI can be added
  later, which needs a Unity license in repo secrets.

### Phase 1: state events and save ownership (done 2026-11-04)

- `PlayerState` raises `Changed` after every mutating method. Two new methods close the last
  gaps where callers wrote fields or reached into the clock: `Sleep()` and `MoveTo(place)`.
  The event is `[field: NonSerialized]` so JsonUtility skips it. Tested in `PlayerStateTests`.
- `PlayerState.Version` (currently 1) and `PlayerState.CurrentVersion`. `SaveStore.Parse`
  reads the version from the raw JSON first and runs the `Steps` chain (each step takes
  version n's JSON and returns n + 1) before deserialising; a newer save is refused, an
  unreadable one starts fresh with a warning. `SaveStoreTests` covers the round trip, a
  version-0 fixture, a future version and garbage.
- `SaveStore` is an instance built on a folder, so tests use a temp directory.
- **Deviation from the plan:** no `RoundSettled` event. Game modules mutate the state only at
  settle, through `ApplyRoundResult`, so the owner saving on `PlayerState.Changed` covers it
  with one event instead of two. `VideoPokerMachine` and `BlackjackTable` no longer call
  `SaveStore`; the table gets its state through `SitDown(PlayerState)` and raises
  `StartOverRequested`. A new `BlackjackScene` host owns the state in the standalone scene.
- `Hud` redraws on `Changed`; its `LateUpdate` only fits the backing on the frame after a
  redraw or a turn, since the cash pill's width isn't known until layout has run.
- `tools/compile_check.sh` compiles all four assemblies offline, for when the editor is open.

### Phase 2: one file for the Ink contract (done 2026-11-04)

- `Game/Story/StoryContract.cs`: every Ink variable the C# side reads or writes, every
  EXTERNAL it binds, and every knot it starts, as constants with `All` lists.
- `Game/Story/StoryBridge.cs`: loads the story, restores its saved state, binds the externals
  to `PlayerState`, validates the bindings on construction, and has `Flag`, `Set`, `Sync` and
  `SaveJson`. `City.Story` is now the bridge; `StoryPlayer` gets the raw Ink story from it.
- `StoryContractTests` compiles `northlight.ink` with the Ink compiler (Ink-Libraries is pure
  C#) and checks every name in the contract exists, that every EXTERNAL is bound, that `Sync`
  pushes the numbers, that the externals reach the state, and that a saved story state comes
  back. The test assembly now references `Ink-Libraries`.

### Phase 3: places out of the city

- New `Game/Places/` with one class per place implementing a small `IPlace` interface:
  `Id`, `Name`, `Pin`, `Arrive(PlaceHost)`. `SlackWaterMotel`, `KasiasTap` (the existing venue,
  moved), `MillstoneLanes`. Each owns its views, actions and scene triggers that are now
  `City.ShowMotel`, `ShowTapOutside`, `ShowLanesOutside`, `ShowLanes` and the `IsOpen` rule.
- `PlaceHost` (what the places see of the city): `State`, `Story`, `PlayScene`, `ShowPlace`,
  `OpenMap`, `LeavePlaceScreen`. `City` implements it and shrinks to boot, arrival, navigation
  and saving.
- `City.Places` becomes a registry built from the place classes, so `PhoneMap` pins and
  `IsOpen` come from the same list.
- Game folders become feature folders: `Game/City`, `Game/Places`, `Game/Story`,
  `Game/Tables` (the two game modules), `Game/Ui` (`UiKit`, `Hud`, `CoverFit`, `LookAround`,
  `SafeAreaFitter`, `OrientationLayout`), `Game/Save`. Namespaces stay `Starflare.Game`; only
  files move. One commit for the moves, so blame survives.

### Phase 4: TextMeshPro

- `UiKit.MakeText` returns `TMP_Text`; a bundled TMP font asset replaces `LegacyRuntime.ttf`
  (generate it with TMP's Font Asset Creator from a chosen typeface, checked in under
  `Assets/Fonts`, and have `ProjectSetup` verify it exists).
- Replace `Text` fields across `Hud`, `StoryPlayer`, `KasiasTap`, `PhoneMap`, `RegisterCard`,
  `VideoPokerMachine`, `BlackjackTable`. `StoryPlayer.FitBox` uses
  `TMP_Text.GetPreferredValues` instead of `cachedTextGeneratorForLayout`.
- The `Starflare.Game` asmdef adds `Unity.TextMeshPro`.
- **Player-facing change**: text renders sharper. Check the dialogue box sizing and the
  title's best-fit shrink in both orientations.

### Phase 5: Input System

- Add `com.unity.inputsystem`; set Active Input Handling to Input System only in
  `ProjectSetup.ConfigurePlayer`.
- `UiKit.MakeCanvas` creates `InputSystemUIInputModule` instead of `StandaloneInputModule`.
- `PhoneMap` pinch and scroll zoom move to `EnhancedTouch` (two active touches) and
  `Mouse.current.scroll`. Everything else already goes through uGUI pointer events and keeps
  working.
- Test on a phone or the device simulator: drag, pinch, scroll, tap pins.

### Phase 6: PlayMode tests

- New `Assets/Tests/PlayMode` with `Starflare.Tests.PlayMode.asmdef` (platforms empty,
  references `Starflare.Game`, `Starflare.Core`, `UnityEngine.TestRunner`).
- Smoke tests: the city boots from an empty save into the arrival scene; signing the register
  with a stacked story reaches the motel lot; `UiKit.OnTurn` rearranges when the screen
  aspect flips (drive `OrientationLayout.Apply` directly rather than resizing the window).
- Add PlayMode to the headless test command in `CLAUDE.md`.

### Later, on a trigger, not a date

- **Addressables** when either happens: the first store build is scheduled, or `Resources`
  passes about 300 files. `Art` is already the single entry point for plates, poses and
  inserts, so the change is one file plus making `Plate`/`Pose`/`Insert` return a handle or
  go async. `PhoneMap` and `City.LoadStory` are the other two `Resources.Load` calls.
- **Scene-change polish** (Observer for `OrientationLayout`, `SafeAreaFitter` reacting to
  `Screen.safeArea` changes instead of polling) once the Hud is event-driven and the pattern
  is proven.

## Decisions (Mike, 2026-11-04)

1. **CI:** none for now. Tests run headless before review.
2. **TextMeshPro:** now (Phase 4), before more screens exist.
3. **Input System:** now (Phase 5), while only `PhoneMap` uses raw input.
4. **Order:** 0, 1, 2, 3, 4, 5, 6. Phases 1 to 3 change how new code gets written, so first.

## Not planned, and why

- **UI Toolkit migration.** Unity recommends uGUI for runtime on this version.
- **Prefabs or UXML for screens.** The project builds UI in code so the scene stays generated
  and diffs stay readable. That is a deliberate exception to the usual prefab workflow; it is
  recorded as such in `CLAUDE.md`.
- **ScriptableObjects for place data.** With one author and generated scenes, plain C# place
  classes give the same separation with type checking and no asset files to keep in sync.
  Revisit if a designer joins who needs to edit places without code.
- **A DI container (VContainer, Zenject).** Explicit `Init(...)` wiring is enough at this
  size and is easier to read.
