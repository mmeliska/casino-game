# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

**Starflare Casino**: a story-driven casino life sim for iOS and Android (portrait). Two halves:

- `game/`: the Unity project (prototype: fair blackjack only). See `game/README.md`.
- `docs/`: the design docs site, which is the **source of truth for the design**. Its GDD (`docs/docs/pages/gdd.md`) is the build spec. Where the GDD and a story page differ on a story detail, the story page wins.

Most work in this repo so far is design and story editing in `docs/`, not code.

## Game (Unity 6000.6.7f1, URP)

Run the EditMode tests headless from the repo root:

```sh
/Applications/Unity/Hub/Editor/6000.6.7f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -projectPath game \
  -runTests -testPlatform EditMode -testResults TestResults.xml
```

Add `-testFilter <FullTestName or regex>` to run a single test or fixture. Unity can't run headless while the editor has the project open.

The scene and player settings are generated from code, not hand-edited. After changing `game/Assets/Scripts/Editor/ProjectSetup.cs`, regenerate with the **Starflare > Rebuild Project Setup** menu, or headless:

```sh
Unity -batchmode -quit -projectPath game -executeMethod Starflare.EditorTools.ProjectSetup.Run
```

Assembly layout and its rules:

- `Starflare.Core` (`Assets/Scripts/Core`): pure C#, `noEngineReferences: true`. Cards/shoe, blackjack rules and round, `PlayerState`, `GameClock`. Keep Unity APIs out of it so it stays testable.
- `Starflare.Game` (`Assets/Scripts/Game`): MonoBehaviours. `BlackjackTable` builds its portrait UI in code.
- `Starflare.Editor`: the project setup menu.
- `Starflare.Tests.EditMode`: NUnit tests; references only `Starflare.Core`.

Game modules never read story state. A table reports each settled round through `PlayerState.ApplyRoundResult` and raises `BreakPointReached` when the player gets up; story beats fire only at break points, never mid-game. The game autosaves after every settled round (`SaveStore`).

`.csproj`/`.slnx` files, `Library/`, `Temp/` etc. are Unity-generated and git-ignored. Unity YAML is kept as LF text; binary art/audio goes through Git LFS (see `.gitattributes`).

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
