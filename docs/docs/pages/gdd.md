## Overview

A story-driven casino adventure for iOS and Android where your bankroll, rich or broke, decides which scenes, people and choices open up. It takes its inspiration from Vegas Stakes (SNES, 1993) and goes much deeper on the story.

**Design pillars**

- **Money is a two-way gate.** Winning opens doors, and so does losing. Going broke is content, not a game over.
- **Story is the spine, gambling is the vehicle.** Players spend most of their time at the tables, and the story finds them there.
- **Every outcome has weight.** A hand changes your bankroll and can also change what happens next.
- **Built for short sessions.** A hand or a spin (10–30 seconds) is the unit of play.
- **Low art burden.** Pixel art, simple casino props, and commissioned character portraits.

Working title: not yet chosen.

## Core loop

The story is the spine, but most play happens at the tables: the player can go gamble at any venue at any time, and the story finds them there.

```
Story hub -> pick a venue -> play a hand -> update state -> does an event fire?
  no  -> play the next hand
  yes -> Ink scene and choice -> effects applied -> update state
         (a main story beat returns you to the story hub)
Leaving the venue also returns you to the story hub.
```

After every hand, results update the state model and the event director checks for an interruption. Random events apply their effects and return you to the table; main story beats pull you back into the story. You can also walk out of a venue at any time.

## Player state model

A handful of state variables, not the bankroll alone, decides what content is available. Money pushes on the other variables, so how you got your money matters as much as how much you have.

| Variable | What it tracks | Moves up when | What it gates |
| --- | --- | --- | --- |
| Bankroll | Cash on hand | Winning, paid jobs, loans | Venue access, table limits, purchases, rich and broke story paths |
| Heat | How much attention you draw from casinos and the wrong crowd | Sketchy jobs, cheating, big wins at shady venues | Closes upscale doors, opens underworld ones |
| Debt | What you owe and to whom, with a due date | Borrowing from a loan shark or a friend | Deadline pressure, collector events, desperate-choice branches |
| Relationships | Standing with each companion and key character | Choices and how you made your money | Companion help, bailouts, betrayals, character arcs |
| Clock | Days and hours elapsed | Every gambling session and story scene | Story pacing, debt deadlines, time-of-day events |
| Milestones | One-time flags the player has reached | Hitting thresholds or story beats | Permanent unlocks, so access doesn't flicker |

**Gating rule.** Content checks combinations of variables, for example `bankroll < 500 AND heat > 3`, rather than a single threshold.

**Hysteresis rule.** Unlocks key off milestones reached, not the current balance. Crossing $10,000 once unlocks the high-roller room; one bad hand does not lock it again. Losing access is a deliberate story event, not a side effect of a balance dip.

## Venues

The player can go gamble at any time, and venues form a ladder from the street corner to the fancy casino. Each venue differs in stakes, odds, entry rules and the events it can throw at you. Bet limits below are starting values to tune in playtesting.

| Venue | Bet range (starting values) | Odds and risk | Entry conditions | Typical events |
| --- | --- | --- | --- | --- |
| Street corner | $1–$20 | Fair dice, but you can get robbed or caught in a loaded game | None | Sketchy job offers, hustlers, police sweeps |
| Gas station | $1–$5 slots | Worst odds in the game; zero risk | None | Down-and-out locals, tips about other games |
| Run-down casino | $5–$100 | Normal house edge, loose rules | None | Loan shark approaches, small-time grifters |
| Hotel casino | $25–$1,000 | Normal house edge | Bankroll milestone; heat below a limit | Business contacts, a companion intro, private-game invitations |
| Fancy casino | $500 minimum and up | Normal house edge, high-roller perks | Bankroll milestone, a suit, low heat | High rollers, VIP rooms, endgame story beats |

Low-tier venues stay available forever, but the clock and their tiny limits stop them from being a safe grinding spot.

## Casino games

The launch set is the Vegas Stakes core: blackjack, poker, craps, roulette and slots. Each game is a self-contained module that reports results to the state model and offers break points to the event system.

| Game | Round length | Where it appears | Notes |
| --- | --- | --- | --- |
| Slots | \~5 seconds | Gas station, all casinos | Simplest to build; good first prototype |
| Blackjack | \~20 seconds | Run-down casino and up | Strongest fit for one-handed portrait play |
| Roulette | \~20 seconds | Hotel and fancy casino | Bet board needs careful touch layout |
| Craps | \~20 seconds | Street corner (street dice) and all casinos | Street version uses simplified rules |
| Poker | 1–2 minutes | Run-down casino and up; private games | Needs opponent AI; good vehicle for named characters |

**Venue variations.** The same game can play differently by venue: loaded dice on the street, a dealer who cheats at the run-down casino, high-roller side bets at the fancy casino.

**Fairness.** Outcomes use real odds unless a story event says otherwise (a rigged game the player can discover). The game never secretly adjusts luck to push the story.

## Event and interruption system

Events are data, not code: each one declares when it can fire, and the engine picks from the eligible pool at natural break points. Adding content means adding event files, not rewiring the game.

**Break points** (the only moments an event may interrupt):

- Between hands, spins or rolls
- After a big win or a big loss
- On a winning or losing streak
- On entering or leaving a venue
- When the clock crosses a day or a deadline

**Selection at each break point**

1. Collect events whose conditions match the current state (venue, bankroll range, heat, debt, flags, time of day).
2. Drop events on cooldown or already used, if marked one-time.
3. Fire any eligible main story beat first.
4. Otherwise roll the overall interruption chance; on a hit, pick one event by weight.
5. Hand the event's scene to the narrative layer (Ink), then apply its effects to state.

**Event schema (draft)**

```json
{
  "id": "loan_shark_intro",
  "kind": "random",
  "venues": ["rundown_casino"],
  "conditions": { "bankroll_max": 200, "debt_max": 0, "losing_streak_min": 3 },
  "weight": 10,
  "cooldown_hours": 48,
  "one_time": false,
  "scene": "ink:loan_shark.intro",
  "effects_on_choice": {
    "accept": { "bankroll": 500, "debt": 750, "debt_due_days": 7 },
    "decline": {}
  }
}
```

In practice, `effects_on_choice` may live in the Ink script itself; the schema keeps eligibility and pacing in data the engine can query and tune.

## Story structure

The story advances on deliberate triggers (days elapsed, milestones, choices), never on luck alone, so a hot or cold streak changes the path but can't skip or stall the plot.

**Two kinds of content**

- **Main story beats:** authored chapters that move the plot. They have priority at break points and fire once.
- **Random events:** flavor and pressure drawn from venue pools. They can fire many times and colour the run without moving the main plot.

**Rich and broke paths**

The broke path should feel like a different game, not a punishment. Examples to develop:

- **Down on your luck:** sketchy courier jobs with a risk roll, back-alley card games, pawning items, a loan shark with a deadline, a companion who bails you out at a cost.
- **On a roll:** high-roller invitations, private poker games, business deals, people who want a piece of your money, rivals who notice you.

Some players will lose on purpose to see the broke content. That's a sign the design is working.

**Endings:** not yet defined. Likely several, shaped by bankroll, heat, debt and relationships at the end of the story clock.

## Platform and technical approach

Ship on iOS and Android first; consider a Switch 2 port if the game succeeds. The proposed stack is Unity with Ink, chosen mainly to keep the Switch 2 door open.

| Decision | Proposal | Why |
| --- | --- | --- |
| Engine | Unity (alternative: Godot) | Mature iOS/Android export and a Switch 2 path through Nintendo's developer program. Godot needs a third-party porting service for consoles. |
| Narrative layer | Ink, via inkle's Unity integration | Built for branching story with variables and conditions; writers can work in plain script files |
| Content data | JSON or YAML event and venue files | Tune content without touching engine code |
| Saving | Autosave after every hand and every story choice | Phone sessions get cut off mid-hand |
| Orientation | Portrait first, with a layout that can adapt to landscape | One-handed play now, Switch later |
| Input | Large tap targets for chips, cards and choices | Touch first; maps to controller later |

**Architecture sketch:** casino game modules, a state model, an event director that runs selection at break points, and the Ink runtime for scenes. Game modules never read story state directly; they report results and expose break points.

## Art and audio direction

A 16-bit-style pixel art look keeps the art burden low and suits a Vegas Stakes tribute. Only the character portraits need a dedicated artist.

| Asset | Source |
| --- | --- |
| Cards, chips, dice | Drawn in code, or Kenney.nl CC0 packs for prototyping |
| UI elements | Kenney.nl CC0 packs, restyled |
| Tables, slot symbols, backgrounds | Made in-house in Aseprite with a fixed Lospec palette, or itch.io packs |
| Character portraits | Commissioned pixel-art portraits (roughly $20–$80 each) |
| Music and sound effects | Not yet decided |

AI-generated art is for placeholders and concept exploration only; shipped art avoids it because of style consistency and store disclosure requirements.

Plan: prototype with Kenney assets and code-drawn cards, then commission portraits once the cast is settled.

## Monetization and store considerations

The proposed model is a premium price or a free demo with a one-time unlock, with no real-money purchase of chips.

- **No selling chips.** Buying in-game money moves the game toward "social casino" territory, with stricter store policies and regulatory scrutiny in some countries. It would also break the design, since losing money is meant to open story paths.
- **Age rating.** Simulated gambling earns a mature rating on both the App Store and Google Play; plan marketing and content for an adult audience.
- **Disclosure.** Any AI-generated content that ships must be disclosed where stores require it.

Price point and demo length: not yet decided.

## Open questions and next steps

**Open questions**

- Who is the protagonist, and why are they gambling in the first place?
- Setting: real Las Vegas, a fictional city, or somewhere else?
- How long is a full playthrough, and what ends the story clock?
- What wins the game: a money target, a story resolution, or both?
- Which companions and key characters make up the cast?
- Unity or Godot: confirm after a small prototype in each, or decide up front?

**Next steps**

- [ ] Write the premise and protagonist in one page
- [ ] Outline the main story chapters and endings
- [ ] Draft 10–15 sample events across the venue ladder
- [ ] Prototype one game (blackjack) wired to the state model and event director
- [ ] Choose a working title
