# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

**Starflare Casino**: a story-driven casino life sim for iOS and Android (portrait first; every screen also has a landscape layout). Two halves:

- `game/`: the Unity project (prototype: the arrival at the Slack Water Motel, night one at Kasia's Tap with video poker, Kasia's two jobs (the coupon run to Juan at Millstone Lanes and the bar-back shift), and Rosa's blackjack table in her back room). See `game/README.md`.
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

**Architecture: read [`game/ARCHITECTURE.md`](game/ARCHITECTURE.md) before changing game code.** It is the rulebook for where code goes, which way dependencies point, who owns which state, how saves work, and the recipes for adding a game, kit, place, phone app or module. In short:

- Modules under `Assets/Scripts`, one assembly each: `Core` (shared game state, no engine), `Ui`, `Games/Shared/<Kit>` (e.g. `Cards`), `Games/<Game>/{Rules,Table,Tests}` (one vertical slice per game), `Phone`, `Game` (playthrough, city, places, story, save, art, debug), `Editor`. Tests sit inside each module (`<Module>/Tests/EditMode`, `/PlayMode`).
- References only point down. A game never references another game or `Game`; shared code sits at the lowest folder holding every user; a venue hands a game what it needs through `Init`.
- `Playthrough` (the root of `Assets/Scenes/Main.unity`) is the only owner of the save. The city, places, games and screens raise events (`PlayerState.Changed`, `SceneEnded`, `BreakPointReached`) and never touch `SaveStore`. No static mutable state.
- Adding a place is a line in `Places/PlaceList.cs`, never a change to `City.cs`.

Scenes are Ink files in `Assets/Resources/Story/` (inkle's ink-unity-integration package). Their `# plate:`/`# pose:`/`# insert:` tags are file names under `Assets/Resources/Plates`, `Poses`, `Inserts`. Slice a portrait sheet into poses with `tools/slice_sheet.py`.

Game modules never read story state. A table reports each settled round through `PlayerState.ApplyRoundResult` and raises `BreakPointReached` when the player gets up; story beats fire only at break points, never mid-game. The game autosaves after every settled round (`SaveStore`).

`.csproj`/`.slnx` files, `Library/`, `Temp/` etc. are Unity-generated and git-ignored. Unity YAML is kept as LF text; binary art/audio goes through Git LFS (see `.gitattributes`).

### Code standards

Set 2026-11-04 from Unity 6 guidance (sources in `game/REFACTOR_PLAN.md`). New code follows every rule now; where existing code doesn't yet, `game/ARCHITECTURE.md` lists it under "Known gaps".

**Layers, dependencies, state and organisation** are in `game/ARCHITECTURE.md` (updated 2026-11-06, Mike).

**Unity choices**

- UI is uGUI (Unity's runtime recommendation for 6000.6). Text is TextMeshPro, not the legacy `Text` component. Don't start a UI Toolkit migration.
- Input goes through the Input System package and uGUI pointer events. No `UnityEngine.Input` calls.
- Loading (`Art`, no other `Resources.Load`) and saves (versioned JSON, migrations, atomic writes, never `BinaryFormatter`) are in `game/ARCHITECTURE.md`.

**Tests and CI**

- Every rule or state change lands with an EditMode test in the module that owns it; MonoBehaviour flows get PlayMode smoke tests (where each goes: `game/ARCHITECTURE.md`, "Tests"). Prefer `[Test]`; use `[UnityTest]` only when a test must yield.
- There is no CI yet (decided 2026-11-04). Run the tests headless before asking for review. While the editor has the project open (so headless can't run), `tools/compile_check.sh` compiles every assembly with Roslyn against the Unity install's DLLs, reading the `.asmdef` files (so a new module needs no change to it) and giving each only the references its `.asmdef` names; it catches compile errors and missing references but doesn't run the tests.

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
