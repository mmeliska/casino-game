## Overview

**Starflare Casino** (working title, settled 24 Oct 2026) is a story-driven casino life sim for iOS and Android. A newcomer drives into Northlight, a Great Lakes casino city, with about $200 and a car, falls for a fading rocket-age casino and its owner, and decides what becomes of it. It takes its inspiration from Vegas Stakes (SNES, 1993), with the open, no-deadline feel of Stardew Valley and the circumstance-driven events of BitLife.

This page is the build spec: the systems, their data and the rules they enforce. The story lives on its own pages ([Story timeline](#timeline), [Outcomes](#outcomes), [Seeds and reveals](#seeds), [Mysteries](#mysteries), [All characters](#cast), [Northlight](#city)). This page summarises only what the systems need from them and links there for the rest. Where this page and a story page differ on a story detail, the story page wins.

**Design pillars**

- **Money is a two-way gate.** Winning opens doors, and so does losing. Going broke is content, never a game over.
- **Story is the spine, gambling is the vehicle.** Players spend most of their time at the tables; the story finds them at break points.
- **Money and time move the story, never luck.** Every act gate needs a money milestone and a minimum time in the city, so a hot streak can't rush the plot and a cold one can't stall it for good.
- **Every choice is remembered.** Choices nudge hidden scales, NPC standing and flags, and consequences surface much later, often as a surprise.
- **Fair tables.** Ordinary play always uses real odds. Doubt about a table only ever comes from a story moment.
- **Built for short sessions.** A hand, spin or roll (5 to 30 seconds) is the unit of play, and the game saves after every one.
- **Low art burden.** Pixel art, still panels and commissioned portraits; no character animation.

## Setting in one paragraph

Northlight is a nice city on a lake, split by a river. Five districts run poorest to richest: Backwater (dry; it voted no on gaming, so every game there is illegal), the Bridges (the crossing), the Levee (old downtown: the Starflare, the riverboats, the RailYard), the Marquee Mile (hotel casinos) and Silvershore (old money, private clubs, yachts). The era is never named: a timeless retro look with full smartphones. Districts, venues, story places, housing and excursions are on [Northlight](#city).

## Core loop

```
City (map, phone) -> travel to a venue (takes part of the day; arriving is a break point)
  -> sit at a game -> play a round -> update state
  -> table event due? yes -> short event (1-2 choices), back to the table
  -> get up from the table = break point
       -> eligible story beat or reveal? yes -> Ink scene, choices, effects
  -> back to the city: move, sleep, take a job, travel (days pass only here)
```

- **Table events** fire mid-session at random (about every 40 to 80 rounds, at most two or three per sitting) and never move the main story.
- **Story beats and seed reveals** never fire mid-game. They wait for a break point: getting up, entering a venue, waking up, crossing the river.
- You can walk out of any venue at any time, except a Prairie Queen sailing or a yacht game, which hold you until the boat docks.

## Time and pacing

Time runs on what you do, not the real clock (settled 19 Oct 2026).

| Rule | Spec |
| --- | --- |
| Parts of the day | Morning, afternoon, night, plus a before-dawn window a few events use |
| Rounds | Each round advances the clock a few minutes, but only to the end of the current part of the day; after that play is unlimited |
| Days | Pass only when you move, sleep, travel or take a job |
| App closed | No in-game time passes; small timers count in-game time only |
| Day length | About 15 minutes of normal play (starting value; tune once the prototype runs, Mike 2026-10-26) |
| Season | 28 days, about 7 hours (starting value; tune once the prototype runs) |
| Main story | About 42 hours to opening night, about six seasons: act 1 ~8h, act 2 ~8h, act 3 ~8h, act 4 ~10h, act 5 ~8h |

There is no story clock. The only deadline on the main line is Starflare Field's Opening Day, the first one at least three seasons after the fall, and missing it leads to a resting point (outcome 7), not a fail. Seasons gate the excursions and a few beats. Small deadlines ("done by Friday", Ray's due date) are fine; a game-ending timer is not.

## Player state model

The bankroll alone doesn't decide what's open. These variables do, in combination.

| Variable | What it tracks | Moves when | What it gates |
| --- | --- | --- | --- |
| Bankroll now | Cash you can spend | Every round, jobs, loans, purchases | What you can bet and buy; broke content when low |
| Best bankroll | The most of your own money you've ever held: cash minus debts (Ray's loan, Kasia's ledger, Flare markers) | Only goes up | Venues and act gates; never drops, so a bad night can't lock anything |
| Debts | Each creditor, amount and due date (shown in Wallet) | Borrowing, repaying, Kasia's credit; also what Jalen owes you | Collector events, Ray's path, the reliable scale |
| Heat | How much the vice squad and the wrong crowd notice you: none, low, medium, high | Illegal games, crooked jobs, dirty act 4 routes | Raids and sweeps; upscale entry limits; the Cinder Room door |
| Hidden scales (4) | Generous/tight, reliable/flaky, ruthless/soft, straight/crooked | Choices in scenes, jobs and table events | NPC reactions, which events and jobs appear, epilogue picks |
| NPC state | Per key NPC: standing with you, fortune (broke, getting by, flush), arc stage | Your choices, plus authored drift on schedule | Their help or betrayal, arc beats, who speaks for you at the hearing |
| Flags | One-time facts: seeds planted (with the day set), mystery clues found, choices made, beats reached | Scenes and events | Reveals, mystery progress, beat variants |
| Pockets | 12 item slots, plus a separate row of story keys | Buying, finding, gifts, pawning | Doors (a suit for the Obsidian), gifts, scenes |
| Clock | Day, part of day, season, days since key beats | Moving, sleeping, travel, jobs | Minimum-time act gates, seasons, the Field timer, reveal delays |

Notes for the build:

- **Gus's trust** is ordinary NPC standing, with no special system; a few beats also check flags (for example whether you helped him quit).
- **The scales are never shown as numbers.** The player feels them through reactions, events opening or closing and things like how much credit Kasia gives.
- **Straight/crooked is separate from heat.** Heat is who's watching you; crooked is who you are.
- **Raffy's envelope** sits in story keys, and its $2,000 doesn't count toward best bankroll until you choose to keep it (A1.5).
- **The car** can't be sold. It's always the floor: rock bottom is sleeping in it under the Bridges.
- **Name and gender** are set once on Dot's register at check-in (A1.1). Gender sets the rival's version (Chase or Brooke) and which wedding dance is offered first; the wedding (A1.4) sets which romance versions you meet.

**Gating rule.** Content checks combinations, for example `best_bankroll >= 25000 AND days_since("bailout") >= 28`, never one threshold alone.

**Hysteresis rule.** Venues and gates key off best bankroll and flags, never the current balance. Losing access is a deliberate story event, never a side effect of a bad night.

## Money gates

| Best bankroll | Venues it opens | Story it opens |
| --- | --- | --- |
| $200 (start) | Kasia's Tap, Millstone Lanes, Bridgehead, the Flare, the Prairie Queen | Act 1 |
| $1,500 | The RailYard | First rung up (A1.8) |
| $5,000 | The Harborlight | Act 2, with Gus's trust and two weeks in the city |
| $10,000 | The Aurelian, the Northern Ledger (spring) | The Stella and Tae false lead |
| $25,000 | Aurelian high-limit, the Gilded Pine (summer) | Act 3, a season after the bailout; meet Vivian |
| $50,000 | The Obsidian (plus a suit and Vivian's sponsorship), which brings the yacht games invitation | Act 4, once who bought the Flare is solved |
| $250,000 | Blackspruce Lodge and the yacht games fill the climb | Act 5, once the name is won back |

The rebuild then spends money in phases (starting values): Vivian's land about $1M, a brand-new rocket sign about $250k, the gaming floor about $1M (opening night ends the main story), then post-game the hotel tower about $2M and the rooftop launch pad about $1.5M. The only backers are Jalen and Vivian, and you pay at least a quarter of every phase yourself. The Cinder Room and the yacht games open by heat and invitation, not money.

## Venues

Ten places to gamble, ladder-ordered and mapped to districts. Bets and entry rules are starting values to tune in playtesting. Full descriptions and who you'll find are on [Northlight](#city).

| Venue | District | Games | Bets | Getting in |
| --- | --- | --- | --- | --- |
| Millstone Lanes (back lot) | Backwater | Craps out back; free-play slots and video poker inside (practice, no money) | $1–$20 | Anyone; sweeps likelier at high heat |
| Kasia's Tap | Backwater | Poker, blackjack, "for amusement only" slots and video poker, pull tabs | $1–$50 | Bar open to all; back room after Kasia's two jobs (A1.3) |
| Bridgehead Fuel & Food | The Bridges | Three-reel slots, video poker, pull tabs, scratchers | $1–$5 | Anyone |
| Starflare Club (the Flare) | The Levee | Rocket slots, video poker, blackjack, craps, roulette, poker, keno, the Rocket Wheel | $5–$100 | Anyone, until the fall |
| Prairie Queen | The Levee | Blackjack, craps, roulette, paddlewheel slots, video poker | $5–$200 | A ticket for a set sailing |
| Cinder Room | The Levee | Casino war (house game), craps, no-limit poker, one-armed bandits | $100 to no limit, cash | Heat above a threshold or an Umbra vouch; Ray slips you the first password (A1.9) |
| The RailYard | The Levee | Core five, video poker, keno, rail slots; players' card, slot tournaments | $10–$500 | Best bankroll $1,500 |
| Harborlight | Marquee Mile | Core five, video poker, keno, lighthouse slots | $25–$1,000 | $5,000; heat below a limit; smart casual |
| Aurelian | Marquee Mile | Core five, video poker, big-screen slots; baccarat in high-limit | $25–$5,000 | $10,000; low heat; high-limit at $25,000 |
| Obsidian | Silvershore | Blackjack, poker (incl. seven-card stud), roulette, craps, slot salon, baccarat | $500 and up | $50,000; a suit; a member sponsors you; low heat |
| Yacht games | Silvershore | Poker, baccarat, craps | $1,000 and up | Vivian's invitation (A3.6) |

Excursions add one trip per season: the Northern Ledger in spring, the Gilded Pine in summer, Haybale Downs late summer to fall, and Blackspruce Lodge once in winter. Housing climbs with you: the car, the Slack Water Motel, a room over a Levee bar, a comped suite on the Mile, Silvershore.

## Casino games

Each game is a self-contained module that plays continuously, round after round, for in-game money, reports results to the state model and exposes break points. It never reads story state.

| Game | Round | Where | Build notes |
| --- | --- | --- | --- |
| Slots | ~5 s | Every venue but the yachts, themed per venue | Simplest; one engine with per-venue reel sets |
| Video poker | ~10 s | Most venues (not the Cinder Room or the yachts) | Jacks or Better is enough to start |
| Blackjack | ~20 s | Kasia's back room and up | Strongest fit for one-handed portrait play; the planned first prototype |
| Craps | ~20 s | Millstone back lot (street rules) and every casino | Street version uses simplified rules |
| Roulette | ~20 s | The Flare and up | The bet board needs careful touch layout |
| Poker | 1–2 min | Kasia's back room and up; private games | Needs opponent AI; the vehicle for named characters (Jalen, the rival, Vivian) |
| Extras | Varies | Per venue | Keno, pull tabs, scratchers, the Rocket Wheel, baccarat, casino war |

- **Core five:** slots, blackjack, craps, roulette, poker.
- **Story-event games** can be played a set number of rounds inside a scene, like the one-pass game of War with Richard (10 cards each, most cards wins). Venue games stay endlessly replayable.
- **One-off cons are events, not games:** three-card monte, a bookie, Mitch's race book and Haybale Downs bets are scenes with a payout, not modules.
- **Millstone Lanes' machines** are free play, the one exception to playing for money.

## Fair play and the cheating rule

Ordinary play is always fair: a losing player is never being cheated by the game, and the game never adjusts luck to push the story. Doubt only comes up in story moments: a tip from a regular, a look from Rosa, a payout that seems short.

- The player can't be sure. Some tips are wrong, and the dealer may just be having a bad night.
- Every call has a consequence. Accuse a crooked dealer and Umbra notices you. Accuse an honest one and you lose standing at that venue. Ride a crooked table and you bleed money (and lean crooked). Walk and you never know at the time.
- Inside a crooked table, and only after a story moment flags it, the odds shift for that session and the recent-results strip shows the pattern. Rosa's lessons sharpen the tells but never make you certain.
- The truth always surfaces later: a news item, a fired dealer, Rosa telling you.
- Kiran's skim on the Prairie Queen steals from the house, never from players, so play at his table stays fair. It's caught with a spot-the-difference look at a color-up.

## Heat and the camera network

Heat has four steps: none, low (they know you), medium (warnings and sweeps) and high (a raid: you lose the table money, pay a fine or spend a night in Lock Street lockup, and the venue goes quiet for a while). Ed, the honest vice cop, is its face.

- **Raises heat:** illegal games (Millstone dice, Kasia's back room, the Cinder Room, Mitch's book), crooked jobs and the dirty versions of the act 4 routes. Kasia's bar machines are too small for Ed to care about.
- **Lowers heat:** one step per week of licensed-only play; Vivian's lawyer clears a raid for a fee; a tip to Ed drops it at once but starts the informant path (R4).
- **Gates:** high heat closes the Mile and Silvershore doors and opens the Cinder Room's.

**Wren's cameras cover the city** (settled 26 Oct 2026). Revaris cameras sit in casinos, bars, shops, motels and parking lots, and the city's "Safe Streets" contract puts Revaris plate readers on streets and bridges. They're the story reason Wren always has something on you by act 4 (S10).

- **Every venue keeps its own footage.** Wren can wipe her own servers but never a venue's copy. City readers feed Revaris directly.
- **Dead zones:** all of Backwater (it voted the cameras down) and the Long Bridge (the hoodlums smash the readers). Scenes that need to happen off camera happen there.
- **Build note:** this is story logic, not a simulated system. A flag such as `face_scanned` (S10) plus the player's record of illegal play and debts is enough to build Wren's file.

## Jobs, the phone and inventory

The phone is the menu hub (settled 24 Oct 2026). It can't place bets, since online betting is illegal in-state.

| Tab | Holds |
| --- | --- |
| Wallet | Cash, Ray's loan and due date, Kasia's ledger, Flare markers, what Jalen owes you; the drive-away option from act 2 on |
| Jobs | Every job available right now; refreshes daily |
| Messages | Texts from people you've met, and shady jobs from unknown numbers |
| Pockets | 12 item slots plus a separate story-key row |

**Jobs** earn less than a good night at the tables, so gambling stays the way up.

- Pay: honest jobs $40 to $150 a shift; trusted repeat work up to about $250; crooked jobs $200 to $400, each adding heat or another risk.
- Each job costs a slice of the day (morning, afternoon or night) or runs a few days. Its real cost is a night away from the tables.
- Most shifts pay out in one panel with a line of flavour. The first shift, and about one in three after, is a short event with one or two choices from that job's own pool; the choices nudge the scales.
- New employers are met in person first; then their jobs appear in the tab on their own. Some jobs show only when you're broke or once someone trusts you. Repeat jobs for one person open their story.
- Crooked jobs form a ladder: running bets for Mitch, steering marks to the Cinder Room and selling information open early; collecting for Ray opens after several Ray jobs and can put someone you know (the rival, Kiran, even Gus in act 4) in front of you.
- The full job table is on [Rules and systems](#rules) and in task 13's planning record.

**Pockets.** Every item is meaningful: a suit for the Obsidian door, a gift to patch up Dot and Gus, Gus's lucky chip. Story keys (Raffy's envelope and thumb drive, a room 9 key) sit outside the 12 slots and can't be pawned unless a scene offers it. Troy at Bridgehead Loan & Pawn buys items for a fraction and holds them a few days so you can buy them back.

## Event and interruption system

Events are data, not code: each declares when it can fire, and the event director picks from the eligible pool. Adding content means adding event files and Ink knots, not rewiring the game.

**Kinds**

| Kind | Fires | Moves the main story | Examples |
| --- | --- | --- | --- |
| `beat` | At a break point, once, top priority | Yes | A1.1 to A5.7 |
| `reveal` | At a break point, once, when its seed flag is old enough | No; adds a bonus or a variant | The S1 to S13 payoffs |
| `random` | At a break point, by weight, with cooldowns | No | Venue and broke-path events |
| `table` | Mid-session, every `rounds_between` rounds | No | The 26 table events on [Event codes](#events) |
| `job` | During a job shift | No | Each employer's pool |

**Break points** (the only moments a non-table event may interrupt): getting up from a table, entering or leaving a venue, waking up, crossing the river, finishing a job, and the start of a new day or season.

**Selection at a break point**

1. Collect events whose conditions match the current state (venue, act, beat, best bankroll, bankroll now, heat, debts, flags, days since a flag, NPC standing, scale leans, season, part of day).
2. Drop events on cooldown, or already used if one-time.
3. Fire the highest-priority eligible beat, if any.
4. Otherwise fire an eligible reveal, if any.
5. Otherwise roll the interruption chance and pick one random event by weight.
6. Hand the scene to Ink, then apply its effects.

**Table events** run on their own counter: a random interval in `rounds_between` (about 40 to 80), at most two or three per sitting, no repeat for a few in-game days, stakes scaled to your current bet, and never a rigged game.

**Seed rules** (settled 21 Oct 2026)

- A seed is a small, forgettable moment that sets a flag with the day it was set. Nothing on screen says a seed was planted, and there's no journal.
- A reveal waits at least a week of in-game time, often an act, and fires only at a break point.
- The callback cue is a small inset of the seed's original panel plus one line ("The trucker from Bridgehead.").
- Missing a seed never locks a beat; the beat still happens, without the bonus. Some seeds go nowhere on purpose (S12).

**Event schema (starting format)**

Mike's format to refine when the event director is built. The seed itself only sets a flag (`helped_trucker`); this is its reveal.

```json
{
  "id": "trucker_saves_spaceman",
  "kind": "reveal",
  "venues": ["starflare_club"],
  "conditions": {
    "act": 3,
    "beat": "last_night_on_the_floor",
    "flags_all": ["helped_trucker"],
    "flags_none": [],
    "days_since_flag_min": { "helped_trucker": 7 },
    "best_bankroll_min": null,
    "npc_standing_min": {},
    "scales": {},
    "seasons": [],
    "heat_max": null
  },
  "fires_at": "break_point",
  "priority": 0,
  "weight": 10,
  "cooldown_days": 0,
  "one_time": true,
  "callback": { "inset": "bridgehead_trucker", "line": "The trucker from Bridgehead." },
  "scene": "ink:seeds.trucker_saves_spaceman",
  "effects": { "flags_set": ["spaceman_saved"], "standing": { "gus": 5 } }
}
```

- `scales` takes a lean, for example `{"generous": 1}`; `npc_standing_min` takes `{"gus": 20}`.
- Table events use the same shape with `kind: "table"` and `"rounds_between": [40, 80]` in place of `fires_at`.
- Effects can live in the Ink script instead; the JSON keeps eligibility and pacing where the engine can query and tune them.

## Story structure

One main storyline in five acts, with places to settle along the way. The Starflare's fate is the background story; most play time goes to the tables, the city, NPC arcs and the mysteries. Beats inside an act can come in any order unless a gate says otherwise. Every beat is on [Story timeline](#timeline).

| Act | Gate in | What happens |
| --- | --- | --- |
| 1. The Flare (~8h) | Start: $200 and a car | Backwater to the Starflare; you get attached to Gus and the regulars as the decline shows |
| 2. The bailout (~8h) | $5,000 + Gus's trust + two weeks | Ray collects; Gus refuses Tae; "a friend of Ray's" (really Revaris, Umbra's secret front) buys the Flare and puts its land into the Levee Stadium District |
| 3. The fall (~8h, always happens) | $25,000 + a season after the bailout | Starflare Field is announced, the Flare is condemned, the regulars scatter, the Field timer starts |
| 4. Winning back the name (~10h) | Who bought the Flare solved (M2) + $50,000 | Gus admits he sold to the mob; you win the name back by leverage, clean or dirty |
| 5. The new Starflare (~8h) | Name won back + $250,000 | Vivian's land, backers, a new rocket sign, the license hearing, opening night |

**The other story systems**

- **NPC arcs.** Core characters get a full arc with stages, NPC state and a portrait; supporting characters get a portrait and events. Influence runs both ways: you change their lives and they change your path. Side arcs (R1 to R10) hook into the spine at fixed beats.
- **Authored drift.** Some NPCs change on schedule regardless of the player (Pete goes to the Aurelian at the fall; the rival slides into debt). The player's flags decide the flavour, not whether it happens.
- **Mysteries.** Four at launch (M1 to M4). Clues are flags spread across broke and rich venues, found in any order. Only M2, who bought the Flare, gates the main story.
- **Broke and rich paths.** Going broke opens side stories and jobs (the car under the Bridges, Ray's loans, Backwater work, staking) but doesn't move the main line. Getting rich opens venues, people and the act gates. Some players will lose on purpose to see the broke content; that's the design working.

**Clean or dirty** (settled 26 Oct 2026). Act 4 has no cash route: once sold, it's sold, and the name comes back only through leverage. There are three routes: Richard's box against Bennett (A4.4), evidence against Umbra (A4.3) and the by-the-book conflict-of-interest complaint (A4.11). Each has a white-hat version (proof of real wrongdoing that never hurts an innocent person) and a black-hat version that is faster or cheaper. The dirty version wins the name too, and the game remembers: heat goes up, the crooked or ruthless lean grows, Gus, Dot, Rosa, Ed and Richard cool on you, Wren takes notice, the license hearing sets stricter conditions, and the story leans toward the Starflare without Gus (outcome 2) unless you repair things.

**The license hearing** (A5.7). Before opening night the gaming commission judges your whole record: heat, raids, illegal games, the informant path, and flags such as your name on Bennett's petition (S7) and Wren's file (S10). You pick who speaks for you from people you've earned, then answer one hard question honestly or not. It never fails: the worst result is conditions (a fine, a delay, a few weeks of clean play), never a lost license.

## Outcomes, endings and post-game

Ten outcomes. Only 1 and 2 end the main story; the other eight are resting points you can always climb back from. What leads to each and the way back: [Outcomes](#outcomes).

| # | Outcome | Kind |
| --- | --- | --- |
| 1 | The lakefront Starflare with Gus | Final (opening night) |
| 2 | The Starflare without Gus | Final (opening night) |
| 3 | The Starflare floats | Resting point |
| 4 | The people's Starflare | Resting point and stepping stone to act 5 |
| 5 | Tae's restaurant | Resting point |
| 6 | Gus lets go | Resting point |
| 7 | The name stays with Starflare Field | Resting point (the Field timer ran out) |
| 8 | Mob takeover | Resting point |
| 9 | Burned down | Resting point |
| 10 | The sellout | Resting point |

- **End trigger:** opening night of the lakefront floor (A5.6), after the license hearing.
- **Drive away:** from act 2 on, the Wallet offers to drive away over the Long Bridge. It plays a leaving epilogue built from that day; the save is kept and you can resume from the morning you left.
- **Epilogue builder:** about 8 still panels, one builder for every ending: (1) what became of the Starflare, (2 to 7) the 5 or 6 cast you touched most, picked from standing, flags, heat and the scales, (8) a Northlight newspaper front page (the mayor's race, Starflare Field, Umbra), then your record card.
- **Post-game:** endings 1 and 2 lead to light owner mode (hiring, hosting whales, keeping up with the Aurelian; gambling stays the main activity) with the tower and launch pad as goals. Each resting point adds one place or routine to the city plus its way back. No new systems beyond light owner mode.

## Platform and technical approach

Ship on iOS and Android first; consider a Switch 2 port if the game succeeds. The engine is Unity with Ink (settled by Mike, 26 Oct 2026), mainly to keep the Switch 2 door open.

| Decision | Choice | Why |
| --- | --- | --- |
| Engine | Unity (settled 26 Oct 2026; Godot was the alternative) | Mature mobile export and a Switch 2 path through Nintendo's developer program; Godot needs a third-party porting service for consoles |
| Narrative | Ink, via inkle's Unity integration | Branching story with variables and conditions in plain script files |
| Content data | JSON (or YAML) files for events, venues, jobs, items and NPCs | Tune content without touching engine code |
| Saving | Autosave after every round, choice and job | Phone sessions get cut off mid-hand |
| Orientation | Portrait first, adaptable to landscape | One-handed play now, Switch later |
| Input | Large tap targets for chips, cards and choices | Touch first; maps to a controller later |

**Architecture sketch**

| Module | Owns |
| --- | --- |
| Game modules | One per casino game: rules, odds, round flow; they report results and break points |
| State model | Bankrolls, debts, heat, scales, NPC state, flags with timestamps, Pockets |
| Clock | Parts of the day, days, seasons, the Field timer; advances only by the rules above |
| Event director | The table-event counter and break-point selection over the event files |
| Ink runtime | Scenes; reads and writes state through a narrow variable bridge |
| Phone | Wallet, Jobs, Messages, Pockets; the job list and the drive-away |
| City | Map, travel, housing, venue entry checks |
| Epilogue builder | Picks and orders panels from state |

Game modules never read story state. A story moment that makes a table crooked sets a session modifier on that module; nothing else touches the odds.

## Art and audio direction

A 16-bit-style pixel art look keeps the art burden low and suits a Vegas Stakes tribute.

| Asset | Source |
| --- | --- |
| Cards, chips, dice | Drawn in code, or Kenney.nl CC0 packs for prototyping |
| UI elements | Kenney.nl CC0 packs, restyled |
| Tables, slot symbols, backgrounds | Made in Aseprite with a fixed palette per district, or itch.io packs |
| Character portraits | Commissioned pixel-art portraits ($20 to $80 each): about 19 (9 core, 10 supporting), plus Richard and the six romance versions if they get portraits |
| Story panels | Still poses over backgrounds; a seed's panel is reused as its callback inset |
| Music and sound | Not yet decided |

- **No character animation.** Characters change by swapping poses; motion is written as text.
- **Districts read at a glance.** Each has its own key light and palette (on [Northlight](#city)); weather and seasons are cheap overlay layers on reused backgrounds.
- AI-generated art is for placeholders and concept exploration only.

## Monetization and store

Premium, or a free demo with a one-time unlock. Never sell chips.

- **No selling chips.** It pushes the game toward "social casino" store rules and scrutiny, and it would break the design, since losing money is meant to open story paths.
- **Age rating.** Simulated gambling earns a mature rating on both stores; plan for an adult audience.
- **Store listing.** "Starflare Casino" plus a subtitle such as "A Casino Life Story" (the App Store allows 30 characters) and an icon with the rocket sign plus dice or a chip. Run a trademark search (USPTO and both stores) before launch.
- **Disclosure.** Any AI-generated content that ships must be disclosed where stores require it.

## Open questions and next steps

**Open questions**

- Investigation mini-games for "you can't be sure" moments, such as spot the difference built from story panels (Mike, 26 Oct 2026, to consider).
- Music and sound direction (later).
- Price point and demo length (later).
- Proposals still open elsewhere: the job table's rows and the inventory list ([Rules and systems](#rules)), and the romance beats ([Romance](#romance)).

**Next steps**

- [x] Write the premise and protagonist (task 5, [The player](#player))
- [x] Outline the main story and endings (tasks 7 and 10)
- [x] Draft sample events across the venue ladder (task 9's seeds and the 26 table events)
- [x] Choose a working title (task 11: Starflare Casino)
- [ ] Prototype blackjack wired to the state model, clock and event director
- [ ] Write act 1's beats in Ink against the schema above
- [x] Settle the engine choice (Unity)
- [ ] Tune day and season length once the prototype runs
