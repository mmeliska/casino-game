# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

**Starflare Casino**: a story-driven casino life sim for iOS and Android (portrait first; every screen also has a landscape layout). Two halves:

- `game/`: the Unity project (prototype: the arrival at the Slack Water Motel, night one at Kasia's Tap with video poker, Kasia's coupon run to Juan at Millstone Lanes, plus a blackjack table). See `game/README.md`.
- `docs/`: the design docs site, which is the **source of truth for the design**. Its GDD (`docs/docs/pages/gdd.md`) is the build spec. Where the GDD and a story page differ on a story detail, the story page wins.

Most work in this repo so far is design and story editing in `docs/`, not code.

## Game (Unity 6000.6.7f1, URP)

Run the EditMode tests headless from the repo root:

```sh
/Applications/Unity/Hub/Editor/6000.6.7f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -projectPath game \
  -runTests -testPlatform EditMode -testResults TestResults.xml
```

Run the PlayMode tests the same way with `-testPlatform PlayMode`. Add `-testFilter <FullTestName or regex>` to run a single test or fixture. Unity can't run headless while the editor has the project open.

The scene and player settings are generated from code, not hand-edited. After changing `game/Assets/Scripts/Editor/ProjectSetup.cs`, regenerate with the **Starflare > Rebuild Project Setup** menu, or headless:

```sh
Unity -batchmode -quit -projectPath game -executeMethod Starflare.EditorTools.ProjectSetup.Run
```

Assembly layout and its rules:

- `Starflare.Core` (`Assets/Scripts/Core`): pure C#, `noEngineReferences: true`. Cards/shoe, blackjack and video poker rules and rounds, `PlayerState`, `GameClock`. Keep Unity APIs out of it so it stays testable.
- `Starflare.Game` (`Assets/Scripts/Game`, grouped by feature): MonoBehaviours, each building its UI in code with `UiKit` (`Ui/`), with a portrait and a landscape layout (`UiKit.OnTurn` rearranges a screen when it turns). `City/City.cs` owns the player state and the one Ink story (`northlight.ink`, which includes each place's file, through `Story/StoryBridge`), saves both (`Save/SaveStore`) and moves you between places, which you pick on `PhoneMap`. Each place is a class in `Places/` implementing `IPlace` (`SlackWaterMotel`, `KasiasTap`, `MillstoneLanes`); it owns its screens and scene triggers and sees the city as `IPlaceHost`. `Story/StoryContract` names every Ink variable, external and knot the code uses; `Story/StoryPlayer` plays scenes; `Tables/` holds the game modules `VideoPokerMachine` and `BlackjackTable`; `Art/Art.cs` finds plates and poses by tag.
- `Starflare.Editor`: the project setup menu.
- `Starflare.Tests.EditMode`: NUnit tests for the rules, state, save store and the Ink contract (it compiles the story); references `Starflare.Core`, `Starflare.Game` and `Ink-Libraries`.
- `Starflare.Tests.PlayMode` (`Assets/Tests/PlayMode`): smoke tests that boot the `City` into a temp save folder (`City.SaveFolder`).

Scenes are Ink files in `Assets/Resources/Story/` (inkle's ink-unity-integration package). Their `# plate:`/`# pose:`/`# insert:` tags are file names under `Assets/Resources/Plates`, `Poses`, `Inserts`. Slice a portrait sheet into poses with `tools/slice_sheet.py`.

Game modules never read story state. A table reports each settled round through `PlayerState.ApplyRoundResult` and raises `BreakPointReached` when the player gets up; story beats fire only at break points, never mid-game. The game autosaves after every settled round (`SaveStore`).

`.csproj`/`.slnx` files, `Library/`, `Temp/` etc. are Unity-generated and git-ignored. Unity YAML is kept as LF text; binary art/audio goes through Git LFS (see `.gitattributes`).

### Code standards

Set 2026-11-04 from Unity 6 guidance (see `game/REFACTOR_PLAN.md` for the sources, the gap list and the order of work). Where current code doesn't meet a rule yet, the plan says which phase fixes it; new code follows the rule now.

**Layers and dependencies**

- Rules, state and anything you can reason about without a screen go in `Starflare.Core`. If a class needs `UnityEngine`, it belongs in `Starflare.Game`; if it's thin enough to need no test, that's the sign it's in the right place (humble object).
- Dependencies are passed in explicitly through `Init(...)` or a constructor. No singletons, no static service locators, no `FindObjectOfType` in gameplay code. The one bootstrap lookup (`UiKit.MakeCanvas` finding the `EventSystem`) is the exception.
- Systems talk through events, not references to each other: every change to `PlayerState` goes through one of its methods and raises `Changed`; game modules raise `BreakPointReached`; screens subscribe. Don't poll state in `Update`/`LateUpdate` when an event can tell you, and don't write a `PlayerState` field from outside it.
- Only the owner of the state saves: the `City`, or `BlackjackScene` in the standalone table scene. It saves on `PlayerState.Changed`. Game modules, venues and screens never touch `SaveStore`.
- The Ink/C# boundary lives in one place (`Game/Story`): every variable name and external function the C# side touches is a constant there, and a test checks them against the compiled story. Don't write `variablesState["..."]` with a literal anywhere else.
- Shared contracts (`BreakPoint`, place ids, state events) get their own file; nothing shared hides inside a module's file.

**Organisation**

- `Game/` is grouped by feature (`City`, `Places`, `Story`, `Tables`, `Ui`, `Save`, `Art`), one class per file, file named for the class. `Core/` already is.
- One class per place, implementing `IPlace`; the city is a coordinator, not the list of places. Adding a place must not grow `City.cs`.
- Scenes and settings stay generated by `ProjectSetup`. Any scene not produced by it is stale and gets deleted. UI is built in code with `UiKit`, with a portrait and a landscape layout for every screen; this is a deliberate exception to Unity's prefab workflow so the scene stays diffable.
- Keep `Packages/manifest.json` to the modules the game uses. Don't add a package without a line in `game/README.md` saying why.

**Unity choices**

- UI is uGUI (Unity's runtime recommendation for 6000.6). Text is TextMeshPro, not the legacy `Text` component. Don't start a UI Toolkit migration.
- Input goes through the Input System package and uGUI pointer events. No `UnityEngine.Input` calls.
- Art and story load through `Art` (and the two `Resources.Load` calls in `PhoneMap` and the story loader), never direct `Resources.Load` elsewhere. `Resources` is acceptable while prototyping; the plan moves it to Addressables before the first store build.
- Saves are JsonUtility files with a `Version` field; a schema change adds a migration step and a fixture test, never a silent reset. Save after every settled change, not at quit. Never `BinaryFormatter`.

**Tests and CI**

- Every rule or state change lands with an EditMode test in `Starflare.Core`. MonoBehaviour flows get PlayMode smoke tests in `Assets/Tests/PlayMode`. Prefer `[Test]`; use `[UnityTest]` only when a test must yield.
- There is no CI yet (decided 2026-11-04). Run the tests headless before asking for review. While the editor has the project open (so headless can't run), `tools/compile_check.sh` compiles all four assemblies with Roslyn against the Unity install's DLLs and catches compile errors; it doesn't run the tests.

**Style**

- Follow the repo `.editorconfig`: 4 spaces, `_camelCase` private fields, no `private` keyword, `sealed` classes by default, LF endings. XML doc comments say why, in the design's own words (place names, event codes), not what the code already shows.

## Design docs site (`docs/`)

Static site, no build step. The Markdown is fetched at run time, so serve it rather than opening the file:

```sh
cd docs && python3 -m http.server 8000
```

How it fits together:

- `docs/app.js`: page templates (`P.<page>`), router, and a small custom Markdown renderer. Pages pull Markdown in with `part(path, ...)`; `EXTRA`/`TOP` append sections to pages; `planRec([n])` embeds planning-task records; `OLD` redirects retired addresses.
- `docs/data/story.js`: the structured data: characters `C`, places `PL`, relationship ties `E`, events `EV`/`EVD`, walk-ons `EVX`, seeds, outcomes, acts, mysteries. Each character's full profile lives in `docs/docs/people/<id>.md`; `C` holds only what lists and the map need. Each place's details and plate prompts live in `docs/docs/places/<id>.md`, the same way. In strings, `[id]` / `[id|label]` links a character and `{CODE}` links an event code.
- `docs/docs/`: page text as Markdown: `people/` (one per character), `places/` (one per location), `pages/` (story map, outcomes, GDD, writing rules, etc.), `plan/` (planning tasks `tNN.md`, whose "Already decided" paragraphs hold dated decisions).
- `docs/docs/index.js`: lists every Markdown file for search. **Add an entry whenever you add a file.**
- `docs/docs/pages/changelog.md`: newest first under `## D Mon YYYY` headings. The site's "updated" date is read from the first heading, so add an entry whenever the design changes.

Event codes are used everywhere: `A<act>.<n>` story beats, `M<n>` mysteries, `S<n>` seeds, `O<n>` outcomes, `R<n>` romance/relationship beats, `X<n>`, `EP` epilogue.

Supporting material:

- `docs/story-bible.md`: compiled archive of the design with source tags and numbered open questions ("OQ n"). It states an order of authority: the audit's Fix status table, then live story/character pages, then `plan/`, then `story.js`, then historical pages. `pages/notebook.md`, `direction.md`, `review.md`, `cast-planning.md`, `bible-fixes.md` are history and keep superseded text (marked "Superseded"); don't "fix" them to match.
- `audits/`: the narrative consistency audit and its Fix status table.

Project agents in `.claude/agents/`: `notes-organizer` (builds the story bible, never invents), `story-checker` (continuity check after any drafting pass; reports only).

## Writing rules to respect

Scene and dialogue rules are in `docs/docs/pages/writing.md` (panel kit, one line per panel at most 15 words, choices are actions with no intent labels, regular play is always fair, no religion, Great Lakes understatement rather than gangster slang). A few settled terms:

- Say "interest", never "vig". Keep "dry" out of player-facing text (Juan's bowling-lane line is the one exception).
- The founder is always "Mr. Hamilton"; Gus and Stella have no surname.
- Not every character needs a tie to another; the city should feel big.
- When recording a settled decision, stamp it like the existing ones (e.g. "Mike, 2026-10-30").
