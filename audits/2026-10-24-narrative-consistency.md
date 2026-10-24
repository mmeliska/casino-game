# Starflare Casino: Narrative Consistency Audit

*Audited 2026-10-24 against the Starflare Design Docs (https://mmeliska.github.io/casino-game/).*

## Fix status

Each finding is worked through with Mike one at a time. The decision and the files changed are recorded here. The audit text below is kept exactly as received.

| # | Finding | Status | Decision |
|---|---|---|---|
| 1.1 | Act 2 is shorter than its own gate | Fixed | Gate stays a season after the bailout; act 2 is now ~8h and the total ~42h (nothing trimmed). |
| 1.2 | Raffy happens on night one and also after the wedding | Fixed | Split: the hand-off joins night one at Kasia's Tap (A1.2); A1.5 is now "Ray asks around", the weeks-later choice. |
| 1.3 | Act 1 event codes don't match the Act 1 table | Fixed | Every act table on the Story timeline now has a Code column, one row per code. Act 1 rebuilt (A1.1 to A1.10), act 3 gained the A3.4 Pete row, act 5's "The floor" row folded into A5.6. |
| 1.4 | "Three rungs in one session" is impossible under the new Act 1 gating | Fixed | Promise dropped (t05 and A1.6). The opening is player-paced; the rocket sign across the river keeps the Flare in view. |
| 1.5 | The rocket sign burns in a building it has already left | Fixed | Sign stays on the empty shell until demolition, then District storage until the Field gate; the O9 fire happens before demolition and takes it; Tae (O5) gets it from storage or the gate. |
| 1.6 | The rocket fin is in three places | Fixed | The fin is gone. The lucky object is now the spaceman, a life-size chrome astronaut statue by the doors; regulars rub his boot. The trucker (S3) saves it for the new lobby; S12 sells a "lucky spaceman" bobblehead. The big sign is a separate object. |
| 1.7 | Pete has been dealing longer than the Flare has been licensed | Fixed | Pete started at 19 busing tables at the supper club, learned cards by watching, and was one of the first dealers when the license came. He started where Jalen is now and keeps an eye on him (new Pete–Jalen tie on the map). |
| 1.8 | "Era-agnostic" names a decade | Open | |
| 1.9 | Wren's résumé doesn't fit her age | Open | |
| 1.10 | The Graham/Claire arc needs more springs than the game has | Open | |
| 2.1 | The outfit is named after the secret | Open | |
| 2.2 | The sale is public, so M2 has no mystery | Open | |
| 2.3 | Ordinary play at Kiran's table isn't fair | Open | |
| 2.4 | Never-rich players can't reach the never-rich ending | Open | |
| 2.5 | The new Starflare has no license, and may not be licensable | Open | |
| 2.6 | Bennett can't give back what he doesn't own | Open | |
| 2.7 | Wren needs leverage to take what her own company controls | Open | |
| 2.8 | "Gus freed" no longer means anything | Open | |
| 2.9 | The hotel doesn't exist until post-game | Open | |
| 2.10 | Holding Raffy's cash unlocks a venue on night one | Open | |
| 2.11 | The Northlight venue table is stale | Open | |
| 2.12 | "Collect for Ray: maybe Gus" has a narrow window | Open | |
| 3.1 | Bennett blackmails inside a room he knows is wired | Open | |
| 3.2 | Revaris watches room 9 for years and never acts | Open | |
| 3.3 | Richard's trigger has already been met | Open | |
| 3.4 | Bennett has no reason to keep the tape | Open | |
| 3.5 | An anti-gambling candidate brands his stadium with a casino | Open | |
| 3.6 | Raffy carries Wren's data for no reason | Open | |
| 3.7 | The commission saw the tape but nobody knows Gus gambled | Open | |
| 3.8 | Stella runs the numbers but never asks about the debt | Open | |
| 3.9 | Stella and Tae keep meeting after the sale | Open | |
| 3.10 | Wren's people meet Bennett where she can't erase the footage | Open | |
| 3.11 | Gus trusts the people who blackmailed him | Open | |
| 4.1 | Seed 11's payoff meeting is never defined | Open | |
| 4.2 | The Cinder Room seed fires before you can enter | Open | |
| 4.3 | The by-the-book route has no evidence source | Open | |
| 4.4 | The excursions have no invitations | Open | |
| 4.5 | Meeting Vivian is circular | Open | |
| 4.6 | The expose-Revaris route hurts innocents | Open | |
| 4.7 | Way back from O5 contradicts "no cash route" | Open | |
| 4.8 | The Richard–Dot coincidence is unexplained | Open | |

---

**Source note:** The live site returns only its page shell (content is fetched by JavaScript at runtime), so this audit runs against the uploaded Markdown sources that the site renders. `data/story.js`, which holds the timeline and relationship-map data, was not included. Story timeline findings were checked against `storymap-*.md`, and Relationship map findings against the character pages' *Related* lines.

---

## 1. CHRONOLOGY & TIMELINE BREAKS

### 1.1 Act 2 is shorter than its own gate
- **The Flaw:** Act 2 is budgeted at about 6 hours. Its exit gate is "$25,000 + a season after the bailout," and the bailout fires at the start of act 2. A season is 28 days at about 15 minutes each, so about 7 hours. Act 2 cannot be shorter than 7 hours.
- **Documentation Gap:** Story timeline (`storymap-intro.md` Pacing; `storymap-beats.md` Act 2 gate).
- **The Impact:** The 40-hour pacing model is wrong at its base. Every downstream estimate inherits the error, including the Field timer math and the Graham/Claire spring windows.
- **The Cold Fix:** Change the act 2 gate to "half a season after the bailout," or re-budget act 2 to about 8 hours and trim act 4.

### 1.2 Raffy happens on night one and also after the wedding
- **The Flaw:** `t05.md`, `raffy.md` and the Story Map's Arrival row put Raffy's envelope on night one, at A1.1. The notebook's renumbering makes Raffy A1.5, after the wedding (A1.4) and after Kasia's jobs (A1.3). The walk-on table also tags "Raffy's mom" as A1.5.
- **Documentation Gap:** Event codes vs. Story timeline (`storymap-beats.md` Act 1) vs. `people/raffy.md`.
- **The Impact:** The first seed of the game has two contradictory trigger points. Any event director keyed to A1.5 fires Raffy days late, after the player has already crossed the river.
- **The Cold Fix:** Make the envelope part of A1.1 and repurpose A1.5 as "Ray asks around" (the weeks-later follow-up). Alternatively, split it into A1.1 (the hand-off) and A1.5 (the consequence).

### 1.3 Act 1 event codes don't match the Act 1 table
- **The Flaw:** The Story Map's Act 1 table has eight rows: Arrival, Kasia's jobs, Back room, Over the bridge, Regular, First rung, Cracks, Ray. There is no wedding row and no Raffy row. The codes cited elsewhere (A1.3 jobs and back room, A1.4 wedding, A1.6 over the bridge, A1.7 regular) imply a different table. "Over the bridge" also gates on the wedding, a beat the map never lists.
- **Documentation Gap:** Event codes (`event-legend.md`) vs. Story timeline (`storymap-beats.md`).
- **The Impact:** The cross-reference system is the site's backbone, and Act 1 references resolve to the wrong beats.
- **The Cold Fix:** Add the wedding (A1.4) and Raffy (A1.5) rows to the Act 1 table, and merge "Kasia's jobs" and "The back room" into one row, A1.3.

### 1.4 "Three rungs in one session" is impossible under the new Act 1 gating
- **The Flaw:** `t05.md` says the first session runs Backwater, then Bridgehead, then the Starflare. The current gates require a bar-back shift (a Night slice), the coupon delivery, and a separately scheduled wedding shift before "Over the bridge" opens. That is several day-slices at minimum.
- **Documentation Gap:** The player (`t05.md`) vs. Story timeline (Act 1 "Over the bridge" gate) vs. Rules and systems (`t13.md`, jobs cost a slice of the day).
- **The Impact:** The first-session design promise, that you see the Flare early, is broken. Players spend their whole opening on Kasia's errands.
- **The Cold Fix:** Either drop "three rungs in one session" from `t05.md`, or let the coupon delivery happen the morning after night one and make the wedding that same evening, so both fit in day two.

### 1.5 The rocket sign burns in a building it has already left
- **The Flaw:** At the fall, "the sign comes down" (`t07.md`) and hangs over the Starflare Field gate. Outcome 9 is the empty building torched *after* the fall, and "the sign is lost." The Landmarks table repeats "lost if the Flare burns."
- **Documentation Gap:** Outcomes (O9) vs. Story timeline (A3 Condemnation/Scatter) vs. Northlight Landmarks.
- **The Impact:** The sign cannot burn in a building it was already removed from. Wiki fodder.
- **The Cold Fix:** State that the sign stays on the shell (or in an on-site storage yard) until the Field gate is built, and the fire takes it there.

### 1.6 The rocket fin is in three places
- **The Flaw:** Seed 3 has the trucker save "the rocket fin the regulars tap" on the last night. It goes into the new Starflare's lobby, and the reveal adds it to inventory. Meanwhile the whole old sign hangs over the Field gate (default), goes to Tae's restaurant (O5), or burns (O9).
- **Documentation Gap:** Seeds and reveals (S3) vs. Outcomes (O5, O7, O9) vs. Rules and systems (Pockets holds "meaningful items").
- **The Impact:** Either the sign at the Field gate is visibly missing its fin, which no outcome acknowledges, or the fin exists twice. A giant sign fin as a pocket item is also absurd.
- **The Cold Fix:** Make S3 save "the fin's hand-worn plate" (the tapped panel), a small object. Add one line to O5 and O7 noting the sign hangs with a patched fin.

### 1.7 Pete has been dealing longer than the Flare has been licensed
- **The Flaw:** Pete is about 60 and the founder hired him "dealing at 19," about 41 years ago. On 2026-10-22 the Flare's illegal back room was dropped. It is now a supper club that only became a licensed casino after riverboats were legalized post-mill-closure, and land casinos came "later."
- **Documentation Gap:** All characters (`people/pete.md`) vs. Northlight history (`t02.md`, back room dropped 2026-10-22).
- **The Impact:** With no back room, there was nothing for a 19-year-old to deal four decades ago. The 2026-10-22 change orphaned Pete's backstory.
- **The Cold Fix:** Pete started at 19 as a supper-club busboy or bartender and became the first dealer when the license came. The 25-year pin still works.

### 1.8 "Era-agnostic" names a decade
- **The Flaw:** `t01.md` says the game never names a year. The same file asks about "the Starflare built in the 1960s," and `t03.md` and the Story Direction say "1960s rocket theme." With Pete's tenure, Gus's age and full smartphones, the setting is pinned to roughly the 2020s.
- **Documentation Gap:** Northlight (`t01.md` era rule) vs. Northlight venues (`t03.md`).
- **The Impact:** Any in-game "1960s" text breaks the timeless conceit the era decision was built on.
- **The Cold Fix:** Replace "1960s" with "the rocket age" everywhere in player-facing and spec text.

### 1.9 Wren's résumé doesn't fit her age
- **The Flaw:** Wren is late 20s. "A few years" before the game she had already run crypto scams, built Revaris Technologies, returned home, displaced the uncles, wired the Cinder Room, and blackmailed Gus with Cinder Room footage.
- **Documentation Gap:** All characters (`people/wren.md`) vs. Mysteries (M1, M4, "all a few years before the game starts" in `room9.md`).
- **The Impact:** She would have been about 22 to 24 at the peak of this. It reads as a timeline nobody checked.
- **The Cold Fix:** Make her early-to-mid 30s, or set the camera deal after her takeover. Her company could have installed the Cinder Room cameras for her grandfather before she took over, which is also how she got the dirt.

### 1.10 The Graham/Claire arc needs more springs than the game has
- **The Flaw:** The arc needs a first spring (meet, $10k minimum, so act 2), a following spring (beats 3 and 4, which unlock A4.11), and optionally a third. The full game is about 40 hours, roughly 5.7 seasons. Hitting $10k one season late pushes beat 3 out by about 28 hours, past the Field timer.
- **Documentation Gap:** Romance (Graham beats) vs. Story timeline (pacing, Field timer, A4.11).
- **The Impact:** The exclusive route is silently uncompletable for ordinary pacing.
- **The Cold Fix:** Move beat 3 into Northlight as a chance meeting (they already live there), so only the meet requires the train.

---

## 2. LORE & WORLD-SYSTEM CONTRADICTIONS

### 2.1 The outfit is named after the secret
- **The Flaw:** Wren renamed the mob from the Black Hand to "Revaris" after Revaris Technologies. The M4 secret is "the box proves Wren's camera company is Revaris," and Bennett had to trace it through records and police contacts. Gus "suspected who she was." M2 asks who bought the Flare, but the buyer on the papers is literally named Revaris.
- **Documentation Gap:** Northlight power structure (`t02.md`) vs. Mysteries (M2, M4) vs. `room9.md`.
- **The Impact:** This is the single largest plot hole. Two of the four mysteries collapse the moment a player reads a company name.
- **The Cold Fix:** Keep "Revaris" as the insider name only. On the street and in player-facing text before act 4, the outfit is still "the Black Hand." Alternatively, Revaris Technologies trades under a different brand (for example a bland "-Secure" name), and the corporate link is what the box proves.

### 2.2 The sale is public, so M2 has no mystery
- **The Flaw:** Revaris Technologies buys the Flare in its own name ("no shell company"). Property and license transfers are recorded filings, and the M2 clue literally reads "the sale papers naming Revaris Technologies." Yet the papers are gated behind Ray's debt or Raffy's drive.
- **Documentation Gap:** Mysteries (M2) vs. Story timeline (A2.2 "the lawyer's name on the paperwork").
- **The Impact:** Any player who knows real estate asks why they can't just look it up.
- **The Cold Fix:** Revaris buys the *shares* of the Hamilton family company privately, with no deed change and no public filing. The paper trail only appears when the land moves into the Stadium District, which is after the scatter.

### 2.3 Ordinary play at Kiran's table isn't fair
- **The Flaw:** M3 has Kiran skimming the Prairie Queen's cheap tables as an ongoing operation. The cheating rule says ordinary play is always fair and doubt arises only in story moments.
- **Documentation Gap:** Rules and systems / Northlight venues (cheating rule, `t03.md`) vs. Mysteries (M3) vs. `people/kiran.md`.
- **The Impact:** Either the player is silently cheated at an ordinary table, violating the core fairness law, or the skim doesn't really happen.
- **The Cold Fix:** Kiran skims only when Ray's collector is aboard, which is only during scripted story sailings. Off those sailings his table is fair.

### 2.4 Never-rich players can't reach the never-rich ending
- **The Flaw:** Outcome 4 is "least money needed," and "players who never get rich settle" there. It can only happen "Act 5, if the land falls through." The Act 5 gate requires $250,000.
- **Documentation Gap:** Outcomes (O4) vs. Story timeline (Act 5 gate, money thresholds).
- **The Impact:** The ending built for broke players is locked behind a bankroll most of them will never see.
- **The Cold Fix:** Let O4 trigger from act 4 the moment the name is won back, with no $250k requirement.

### 2.5 The new Starflare has no license, and may not be licensable
- **The Flaw:** Gaming is legal only in licensed districts that voted yes, and Silvershore allows "private clubs" only. The site of Vivian's land is "not yet settled." No beat obtains a gaming license for the lakefront Starflare, the floating Starflare (O3, a riverboat needs one), or the people's Starflare at a union hall (O4). The Flare's own license was the condemned one Bennett held.
- **Documentation Gap:** Northlight (`t02.md` licensing law) vs. Story timeline (Act 5) vs. Outcomes (O1, O3, O4).
- **The Impact:** If the union hall is in Backwater (Story Direction places union halls on the dry side), O4 is an illegal casino and Ed should raid it.
- **The Cold Fix:** Put Vivian's land on the Marquee Mile side of Silvershore's border, explicitly. Make "the license follows the name" an explicit rule, so winning back the name restores the license, and site the union hall in the Levee.

### 2.6 Bennett can't give back what he doesn't own
- **The Flaw:** After the 2026-10-23 rework, the Levee Stadium District owns the name and Bennett is only the political engine. Yet the room 9 routes still say "Bennett frees Gus" or "gets the name back for Gus out of gratitude." `room9.md` still uses "frees Gus."
- **Documentation Gap:** Mysteries (M4), `people/richard.md` vs. Outcomes rework (`t07.md`).
- **The Impact:** Two of three leverage routes have no mechanism after the rework.
- **The Cold Fix:** State that the District's condemnation and zoning ride on a council ordinance Bennett sponsors. His price for the vote, or his leverage over the District board, is the name's return to Gus.

### 2.7 Wren needs leverage to take what her own company controls
- **The Flaw:** O8 and O10 have the player give Wren "what she needs to take the name (Gus's markers or Raffy's drive)." Revaris is already the District's biggest investor.
- **Documentation Gap:** Outcomes (O8, O10) and Story timeline (A4 "Wren's deal") vs. `t07.md` rework.
- **The Impact:** This is a pre-rework leftover, from when Bennett owned the name.
- **The Cold Fix:** Reframe it: Wren wants the threats against her neutralized. Raffy's drive is evidence against her, and Gus's IOU is leverage to keep him from testifying. Handing them over lets her pull the name out of the District for a mob-run venue.

### 2.8 "Gus freed" no longer means anything
- **The Flaw:** The sale cleared Gus's debt at act 2. Outcomes 1, 3 and 4 still require "Gus freed," and Graham's page says "without Graham/Claire, freeing Gus means paying or the room 9 box." Paying is dead ("no cash route"), and the expose-Revaris route is omitted.
- **Documentation Gap:** Outcomes table (`t07.md`) and `people/graham.md` vs. the 2026-10-23 rework.
- **The Impact:** Spec readers will build conditions against a variable that no longer exists.
- **The Cold Fix:** Global replace "Gus freed" with "name won back," and correct Graham's line to list the other two routes.

### 2.9 The hotel doesn't exist until post-game
- **The Flaw:** Jalen "will run the hotel" in act 5, and Felix/Nora's together-landing runs "the front desk of the new Starflare hotel." The tower with suites is a post-game goal of about $2M. Act 5 builds only land, sign and gaming floor.
- **Documentation Gap:** Story timeline (Act 5 phases) vs. All characters (`jalen.md`) vs. Romance (`felix.md` beat 5).
- **The Impact:** The opening-night landing references a building that isn't there.
- **The Cold Fix:** Jalen runs the floor's player development until the tower opens. Felix/Nora run guest services at the casino, then move to the hotel desk post-game.

### 2.10 Holding Raffy's cash unlocks a venue on night one
- **The Flaw:** The best bankroll is "the most you've ever held." Raffy's envelope holds about $2,000 and can be opened on night one, against a $200 start. That clears the $1,500 RailYard gate and the "First rung up" beat immediately.
- **Documentation Gap:** Story timeline (Money thresholds) vs. Seeds (Raffy) vs. Rules and systems (Pockets).
- **The Impact:** This exploit breaks the money gate on night one.
- **The Cold Fix:** Envelope cash sits in Pockets as a story key and doesn't count toward the bankroll until spent. Alternatively, the best bankroll counts only money won or earned.

### 2.11 The Northlight venue table is stale
- **The Flaw:** `northlight.md` says Kasia's back room "needs you to be a regular or vouched for." `t03.md` (2026-10-23) says it opens by Kasia's invitation after two jobs.
- **Documentation Gap:** Northlight venue table vs. Rules and systems / `t03.md`.
- **The Impact:** Two "rigid law" pages disagree on the first gate in the game.
- **The Cold Fix:** Copy the 2026-10-23 entry rule into `northlight.md`.

### 2.12 "Collect for Ray: maybe Gus" has a narrow window
- **The Flaw:** Gus's debt to Ray is cleared by the act 2 sale. "Collect for Ray" opens only after several Ray jobs, which realistically means act 2 or later.
- **Documentation Gap:** Rules and systems (`t13.md` job table) vs. Story timeline (A2.2).
- **The Impact:** The marquee moment of the crooked ladder points at a debt that no longer exists.
- **The Cold Fix:** Tie the Gus collection to his act 4 relapse at the Cinder Room ("Gus at the bottom").

---

## 3. CHARACTER MOTIVATION & KNOWLEDGE FAULTS

### 3.1 Bennett blackmails inside a room he knows is wired
- **The Flaw:** Bennett traced Wren's cameras to Revaris, then made his demand to Gus on camera, in Gus's office, on Wren's system. Later he takes his hidden cut where "Wren's cameras catch it," twice.
- **Documentation Gap:** Mysteries (M4) and `people/bennett.md` vs. `people/wren.md`.
- **The Impact:** This is authorial puppeteering. The man who discovered the surveillance network walks into it repeatedly.
- **The Cold Fix:** For the office recording, Gus secretly recorded the squeeze on his own device, which makes the box unique. For the hidden cut, Wren pays it herself, so her books are the evidence.

### 3.2 Revaris watches room 9 for years and never acts
- **The Flaw:** Revaris has watched Richard's door for years over a key on a lanyard worn by a frail man in his late 70s, and never takes it. Wren also already has her own copy of the office footage, so the box seems to hold nothing she needs.
- **Documentation Gap:** Mysteries (M4), `room9.md` items 5 to 7.
- **The Impact:** The surveillance has no logic, and the threat feels toothless.
- **The Cold Fix:** The box was rented in Richard's name, so the key is useless without his signature at the bank. They watch to see if he goes there. Add the original signed contract as the item Wren lacks.

### 3.3 Richard's trigger has already been met
- **The Flaw:** Gus's condition is "if anyone ever comes after you over the debt." Revaris has watched the door from before the game starts, and Dot tells you so. The solve treats "you notice Revaris watching" as the new trigger.
- **Documentation Gap:** Mysteries (M4 solve) vs. `people/dot.md` (her room 9 story).
- **The Impact:** Nothing changes at the moment of the solve, and the watching isn't about the debt anyway.
- **The Cold Fix:** In act 4, Revaris men approach Richard asking about the key and the old markers. Make it Wren's act 4 rule escalating because you've been digging.

### 3.4 Bennett has no reason to keep the tape
- **The Flaw:** Bennett keeps Gus's tape "for his campaign," but Gus no longer owns the Flare. The land is in Bennett's own stadium project, and exposing Flare scandal taints Starflare Field. Bennett's people also somehow clear a building owned by Revaris and the District.
- **Documentation Gap:** All characters (`bennett.md` Proposal) vs. Mysteries (M1) vs. `miles.md` story role.
- **The Impact:** The motive contradicts his own campaign, and access to the building is unexplained.
- **The Cold Fix:** He keeps it as insurance against Gus testifying about the M4 squeeze. His city inspectors do the condemnation walk-through, which gives him lawful access.

### 3.5 An anti-gambling candidate brands his stadium with a casino
- **The Flaw:** Bennett runs on "clean up Northlight," against gambling, yet his stadium is named Starflare Field and he keeps the casino's rocket sign over its gate.
- **Documentation Gap:** All characters (`bennett.md`) vs. Story timeline (A3.1 drawing).
- **The Impact:** His central political brand contradicts itself, with no in-world reason given.
- **The Cold Fix:** Revaris, as biggest investor, insists on the name to hold the trademark, and Bennett sells it to voters as "honoring the Levee's history." One line from Bennett covers it.

### 3.6 Raffy carries Wren's data for no reason
- **The Flaw:** A 19-year-old courier for Ray's street collections carries face-match logs from Wren's cameras and her list of blackmail payers. No chain explains why that data would be in a collection envelope.
- **Documentation Gap:** All characters (`raffy.md`) vs. Relationship map (Raffy ties only to Ray, Kasia, Dot, Juan, Ed, Wren as "his drive holds her logs").
- **The Impact:** The game's first seed carries a payload no one could have put there.
- **The Cold Fix:** The drive is Ray's secret insurance copy against Wren, hidden in the envelope Raffy skimmed. This also pays off M3, where Ray resents Wren knowing everything.

### 3.7 The commission saw the tape but nobody knows Gus gambled
- **The Flaw:** The commission ruled on the payroll night from camera footage and ordered new cameras. Yet "nobody knows" Gus gambled, Stella "doesn't know," and Rosa's file must be pulled as a mystery clue.
- **Documentation Gap:** Mysteries (M1) vs. `people/stella.md`.
- **The Impact:** If the commission saw the player at the table, Gus's identity is on the public record.
- **The Cold Fix:** The commission's copy showed only Rosa's hands and the chip flow. Gus kept the only wide-angle tape, which is why it matters.

### 3.8 Stella runs the numbers but never asks about the debt
- **The Flaw:** Stella is a finance analyst who "has run the Flare's numbers," and Tae's satellite offer used her numbers. The Ray debt came from gambling, yet she never asks where it came from.
- **Documentation Gap:** All characters (`stella.md`) vs. Story timeline (A2.1).
- **The Impact:** A professional analyst missing an unexplained debt in the books strains belief.
- **The Cold Fix:** The debt is personal and off the Flare's books. She sees only operating losses.

### 3.9 Stella and Tae keep meeting after the sale
- **The Flaw:** The false lead fires at $10k, after the bailout. Tae's satellite offer was before the bailout and is dead. Nothing remains for them to meet about.
- **Documentation Gap:** Story timeline (A2 "Higher up") vs. Mysteries (M2).
- **The Impact:** The false lead has no motive at the moment it fires.
- **The Cold Fix:** After the sale, Stella asks Tae to help her find out who bought it. That is a real motive, and it makes the false lead an echo of the player's own search.

### 3.10 Wren's people meet Bennett where she can't erase the footage
- **The Flaw:** Felix/Nora's exclusive route depends on Wren's people meeting Bennett at the Aurelian, the only casino with cameras she doesn't control. Wren is the city's surveillance expert.
- **Documentation Gap:** Romance (`felix.md` exclusive route) vs. All characters (`wren.md`).
- **The Impact:** Her character is defined by information control, and here she is careless.
- **The Cold Fix:** Bennett chooses the venue, because it's the one place *he* thinks Wren can't record. That is in character for him and explains why Wren didn't prevent it.

### 3.11 Gus trusts the people who blackmailed him
- **The Flaw:** Gus knows the buyer is the mob that blackmailed him with Cinder Room footage. He still "believes Revaris's promise to keep the Flare open."
- **Documentation Gap:** Story timeline (A2.2) vs. `people/gus.md`.
- **The Impact:** Naive trust from a man with every reason for suspicion reads as a plot convenience.
- **The Cold Fix:** He doesn't believe it. He tells himself it, and says it out loud to the regulars. His voice sheet already says he lies by changing the subject.

---

## 4. NARRATIVE LEAPS & UNSET-UP TWISTS

### 4.1 Seed 11's payoff meeting is never defined (Unearned)
- **The Flaw:** Ray's plate "puts Ray at a meeting he denied." No document defines what the meeting was, who was there, or when Ray denied it.
- **Documentation Gap:** Seeds and reveals (S11).
- **The Impact:** The reveal has nothing to reveal.
- **The Cold Fix:** The meeting is Ray handing Gus's sale papers to the filing lawyer. In act 2, Ray tells Gus "I just know a guy, never met him." One line of setup in A2.2 covers it.

### 4.2 The Cinder Room seed fires before you can enter (prerequisite failure)
- **The Flaw:** Seed 6 and A1 "Cracks" put the player in the Cinder Room in act 1. Entry requires heat above a threshold or a Revaris vouch. An act 1 player is usually low-heat.
- **Documentation Gap:** Seeds (S6) vs. Northlight venue table (Cinder Room entry).
- **The Impact:** A core act 1 seed may never be reachable.
- **The Cold Fix:** Gus brings you as his guest the first time. He is the vouch, which is also its own small reveal.

### 4.3 The by-the-book route has no evidence source (Deus Ex Machina)
- **The Flaw:** The A4.11 conflict-of-interest complaint needs proof of Bennett's hidden cut. The only established record is Wren's cameras. The "friend" version tells you to "gather the proof yourself," from nowhere.
- **Documentation Gap:** Story timeline (A4.11) vs. Romance (`graham.md`) vs. Mysteries.
- **The Impact:** The route is promised as independent but secretly depends on the other two routes' evidence.
- **The Cold Fix:** Seed it through Mitch. The filing lawyer handles both the stadium paperwork and Bennett's side payments, and Mitch can name the account.

### 4.4 The excursions have no invitations (Unearned)
- **The Flaw:** Blackspruce Lodge and yacht games open only "through a story event." No beat in Acts 1 to 5 issues either invitation.
- **Documentation Gap:** Excursions (`t04.md`) vs. Story timeline.
- **The Impact:** Two venues are effectively unreachable.
- **The Cold Fix:** Vivian's Obsidian sponsorship (act 3) brings the yacht invite. Ray's "friend of mine" offer at act 2 includes the Blackspruce invite.

### 4.5 Meeting Vivian is circular
- **The Flaw:** You "meet Vivian" at the Gilded Pine at $25k, but the Gilded Pine unlocks by "a Silvershore invite," and Vivian is that invite. She also sits at the Harborlight high tables from $5k (act 2).
- **Documentation Gap:** Story timeline (A3 "Higher still") vs. Northlight venue table vs. Excursions.
- **The Impact:** The meeting is either impossible by its own invite path or happens an act early.
- **The Cold Fix:** You meet her at the Harborlight in act 2, and her invite opens the Gilded Pine.

### 4.6 The expose-Revaris route hurts innocents
- **The Flaw:** White-hat leverage is "never anything that hurts an innocent person." Raffy's drive includes Wren's blackmail victims (gamblers hiding losses from family). Handing it to Ed or brandishing it exposes them.
- **Documentation Gap:** Outcomes (`t07.md` white-hat rule) vs. Story timeline (A4.3).
- **The Impact:** The flagship act 4 route violates the act's own founding rule.
- **The Cold Fix:** Only the face-match logs are used. The victim list is a separate file, and choosing whether to destroy it is its own small scene.

### 4.7 Way back from O5 contradicts "no cash route"
- **The Flaw:** "No cash route" governs act 4, yet the way back from O5 is "buy the name back from Tae" with cash. Nothing explains why the Revaris-controlled District sells to Tae but never to you.
- **Documentation Gap:** Outcomes (`storymap-branches.md` O5) vs. `t07.md`.
- **The Impact:** An unexplained exception to a stated rule.
- **The Cold Fix:** Make the rule explicit: the District never sells to Gus's side, and Tae is a neutral buyer. Once Tae holds the name, it is ordinary private property.

### 4.8 The Richard–Dot coincidence is unexplained (Unearned)
- **The Flaw:** Richard, ruined by Gus's casino, lives at a motel owned by Gus's estranged sister-in-law. This is exactly the kind of family link the cast rules say to keep rare, and it is unremarked.
- **Documentation Gap:** All characters (Gus's family-link rule) vs. Mysteries (M4).
- **The Impact:** The coincidence reads as authorial convenience.
- **The Cold Fix:** Gus chose the motel, not Bennett. He arranged the room through Dot without telling her why. That also explains Richard's belief that Gus pays.
