# Starflare Casino (Unity project)

The game itself. The build spec is the GDD at `docs/docs/pages/gdd.md`.

- **Unity:** 6000.6.7f1. Add iOS and Android Build Support in Unity Hub to build for phones; Play mode and Mac builds work without them.
- **Rendering:** URP (the Built-in pipeline is deprecated in Unity 6). The pipeline asset is in `Assets/Settings`.
- **Git LFS:** binary art and audio go through LFS (see `.gitattributes` at the repo root). Run `git lfs install` once per machine.

## Open and play

1. Unity Hub > Add > select this `game/` folder.
2. Open `Assets/Scenes/Blackjack.unity` and press Play. Set the Game view to a portrait size, such as 1080x1920.

The scene and player settings are built from code: **Starflare > Rebuild Project Setup** regenerates them.

## Layout

| Folder | Assembly | What's in it |
| --- | --- | --- |
| `Assets/Scripts/Core` | `Starflare.Core` | Pure C# with no engine references: cards and shoe, blackjack rules, player state and clock |
| `Assets/Scripts/Game` | `Starflare.Game` | MonoBehaviours: the blackjack table UI, autosave, safe area |
| `Assets/Scripts/Editor` | `Starflare.Editor` | Project setup menu |
| `Assets/Tests/EditMode` | `Starflare.Tests.EditMode` | NUnit tests for the rules and state |

Game modules never read story state. The table reports each settled round through `PlayerState.ApplyRoundResult` and raises `BreakPointReached` when you get up.

## Tests

Window > General > Test Runner > EditMode > Run All, or headless:

```sh
/Applications/Unity/Hub/Editor/6000.6.7f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -projectPath game \
  -runTests -testPlatform EditMode -testResults TestResults.xml
```

## Prototype scope

Fair blackjack at the Flare's limits: six decks with a 75% cut card, dealer stands on soft 17, blackjack pays 3:2 (an odd bet's half dollar rounds down), hit, stand, double (also after a split) and one split; split aces get one card each. Starting bankroll $500. Each round takes 24 in-game minutes, up to the end of the part of the day. The game autosaves after every settled round.

Not yet: Ink, table events, heat, debts, the city and the phone.
