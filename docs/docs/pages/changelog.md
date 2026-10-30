## 30 Oct 2026

### Portrait prompts

Every character with a portrait has a new Portrait prompt section on their page, after their Look, with a Leonardo.ai or Adobe Firefly prompt. A new page, Portrait prompts (under People), has the how-to, a single-pose template, a negative prompt, and Walt, Lorraine and Augie. Each prompt makes an expression sheet in the inked graphic-novel style of the Dot proof of concept, and a table lists the character's poses with the tag scenes use. Every pose comes from a settled scene or tell. Every portrait is full body and standing, facing the viewer or turned to the right, with nothing in the hands (hands carry the expression; held props become inserts), and every prompt describes the character head to toe, with a second outfit where a scene needs one. Details the docs don't settle are proposed and marked on each page. Unsettled looks (ethnicity for several characters, Walt, Lorraine and Augie, Alice) are listed at the end.

### Art style: inked graphic novel

The game's look is now gritty, inked graphic-novel art in the style of the Dot proof of concept, replacing 16-bit pixel art (Mike, 2026-10-30). The GDD's design pillars, art direction and asset table are updated: backgrounds are illustrated in the same style with a palette per district. Portraits are no longer commissioned, for budget reasons: the final game ships AI-generated portraits unless funds later allow commissions. They are generated with Leonardo.ai or Adobe Firefly from the Portrait prompts page, an expression sheet first and then each pose as its own image. Other AI art is still for placeholders and concepts only.

A story-editing pass reviewed 58 suggestions (46 accepted, 12 rejected) and settled most of the open questions. Every character page, the story map, the outcomes, the GDD, the planning tasks and the story data were updated to match. Older pages that are kept as history (the direction document, the 18 Oct review, the notebook, cast planning) keep their text, with "Superseded" notes where it's now wrong.

### Suggestions folded in

Every accepted suggestion was checked against the live pages and the gaps were filled, so the suggestion files have been removed. The additions:

- **New on the site:** the five emotional beats, one per act, on the story map intro. "They ask about each other" (one line per pair, once a season) in the writing rules. The three answers to the A5.7 hard question and what each costs, in the GDD.
- **Everyone gets the hard question at A5.7.** Only "take it" needs Gus's A4.1 confession; the GDD had gated the whole question.
- **Story keys and puzzles:** a story key names who gave it to you. Ray's burn-bag key is a jigsaw with handwritten scraps mixed into printed strips.
- **The Flare's last night (A3.3):** three time slots (afternoon, night, before dawn). After it, no Flare game can be played. Tae's father's drum kit has to leave the building, and in O5 it's on a riser under the band photo.
- **Smaller details on the character pages:** Alice's three touches (the covered piano, the wrong constellation, the mug chip), Gus's turn told in panels, Rosa's stoop, Kasia's mother and the pierogi, Jalen's notebook, Pete telling Mr. Hamilton the band was paid too much, Wren's "never been asked a question she couldn't answer," Ray's written-down key, and Richard's trust that rises only when you ask him for something.
- **Event data:** Kasia stands with the league at A3.2 and faces Umbra's protection offer at R3. Vivian's hidden "what should the land become" question is at A5.1. The rival's standing decides what happens when you refuse them at A3.8. Wren's A4.5 meeting needs the drive or a Gus who still plays.

### The main story

- **Raffy's drive** is Revaris's old encrypted key ledger, the only place a face ID is tied to a name. Ray kept it when he was told to destroy it. You open it by rebuilding his shredded password in the jigsaw puzzle; plugging it in online tells Wren it's alive. A1.5 settles only the envelope's cash and Raffy (four choices); what you do with the drive is chosen from act 2 on.
- **The Ed route** is the clean way to expose Umbra. Ed's case undoes the sale because Gus signed under blackmail, but only if Gus testifies. If the drive is in the case, Wren may move first: she returns the name early and blames Ray. Trading your silence to Wren (through her lawyer) is the dirty version.
- **Wren's markers offer (A4.5):** bring Gus back to her table so he signs new markers against his house. Wren's face appears in only one scene, A4.5.
- **The fire (O9)** is a setback you play through, not an ending. It can only happen after the last night.
- **The new Starflare:** you own it and hold the license; Jalen and Vivian are minority partners. Gus never owns it; he signs the name over when act 5 opens. The $250,000 act 5 gate is your quarter of the land.
- **The Field timer** starts at the A3.5 ground-breaking. The old rocket sign is restored by the District over the Field gate.
- **No leaving ending.** The cash-out / drive-away is cut.
- A1.10 and A2.1 are two separate visits from Ray. A3.9 is a one-panel notebook beat. O6 doesn't need the name.
- The commission grants every license; the alderman only gives local sign-off. O3 and O4 get short hearings.
- Bennett wins the mayor's race unless he's publicly exposed in act 4.

### Characters

- New backstories, arcs and sample lines for almost the whole cast. Gus's late wife is Alice; the founder is always "Mr. Hamilton," and Gus and Stella have no surname.
- Kiran's debt now comes from a loan he co-signed for his wife's brother. Pete's son Tomas has a chronic illness. Juan owns Millstone Lanes. Jalen is from the Levee and isn't staked; he studies hospitality and runs the hotel. Dot is in her early 60s.
- The Flare has three named regulars with portraits: Walt, Lorraine and Augie. Portraits now total about 24, plus the romance versions.

### Rules and the city

- **Backwater has cameras.** Only Kasia's Tap, Millstone Lanes and the Slack Water refuse them; the Long Bridge stays dark. The truth is told in those places.
- Say "interest," never "vig." "Dry" stays out of player-facing text (Juan's bowling-lane line is the one exception).
- Not every character needs a tie to another; the city should feel big.
- Going broke is fixed by jobs. Staking stays as a job (a contract, not a rescue).
- **Venues:** the Gilded Pine is open all year; Blackspruce needs Ray's invite plus about $25,000; the yachts need Vivian's invite plus about $100,000; the whole Aurelian opens at $10,000; once you're in the Cinder Room you stay in, but every visit raises heat.

## 26 Oct 2026

The rest of the consistency audit was fixed and merged, and a few new rules came out of it. The game design document was then brought up to date, which finishes the planning tasks.

### Game design document

- The [Game design](#design) page was rewritten as the build spec, matching every decision on the site (task 12). It now covers time and pacing without a story clock; the full state model (best bankroll, debts, heat, the hidden scales, NPC state, flags, Pockets); the money gates; venues by district; the games and the cheating rule; heat and Wren's camera network; jobs, the phone and inventory; the event director with beats, reveals, table events and the merged event schema; the five acts, clean or dirty and the license hearing; and the outcomes, epilogue and post-game.
- All thirteen planning tasks are now done.
- **Engine: Unity**, with Ink for the story (Mike). Day length (about 15 minutes) and season length (28 days) stay as starting values until the prototype runs; audio, price and demo length are left for later.
- **A blackjack hand takes 5 in-game minutes** (Mike, after playing the prototype). That is close to a real table, so a six-hour part of the day fills after about 72 hands; travel and jobs fill the rest of a day.
- **Clue puzzles.** Visual clues are worked with short randomized puzzles (slide, spot the difference, tile rotate or jigsaw). They are optional, have no fail state, cost a slice of the day, give evidence but never certainty, and once solved in the story they can be replayed from a puzzle shelf on the phone.

### New rules and additions

- **Clean or dirty.** Act 4 is no longer white-hat only. Every route to win back the name now has a dirty version that is faster or cheaper but leaves lasting costs: heat, cooler relationships, Wren takes notice and the license hearing sets stricter conditions. The rule is on the [Outcomes](#outcomes) page.
- **Wren's cameras cover the city.** Revaris cameras sit in bars, shops, motels and parking lots, and the city's "Safe Streets" plate-reader contract puts them on streets and bridges. Backwater (which voted cameras down) and the Long Bridge (where the hoodlums smash them) are the dead zones.
- **Every venue keeps its own footage.** Wren can wipe her own servers but never a venue's copy.
- **New seed S13.** A Revaris rep pitches Tae a state-of-the-art AI camera system for the Aurelian. Warn him or say nothing; he installs it either way, and it pays off in act 5. The Aurelian stays a friendly rival, never an ally.
- **New beat A5.7, the license hearing.** The license follows the name, so before opening night the gaming commission judges your record. It never fails; the worst result is a set of conditions.
- This change log, and the site's "updated" date now comes from it.

### Audit fixes

- Kiran skims from the house, never from players, so play at his table stays fair (2.3).
- If you win the name back but are short of $250,000, the regulars offer the people's Starflare right away as a stepping stone to act 5 (2.4).
- The license follows the name; new license hearing at A5.7 (2.5).
- The Starflare name sits in Bennett's stadium ordinance, which is how each act 4 route gets it back (2.6).
- Wren's deal is about tying off her loose ends: Raffy's drive and Gus (2.7).
- "Gus freed" is gone everywhere; the goal is "name won back" (2.8).
- Jalen runs the poker room until the post-game hotel tower opens (2.9).
- Best bankroll counts only your own money, cash minus debts (2.10).
- The Northlight venue table now matches how Kasia's back room opens (2.11).
- "Collect for Ray" debtors follow the story, so the job never runs dry (2.12).
- Gus secretly recorded Bennett's squeeze on his own desk recorder, the only copy; Wren pays Bennett's cut herself (3.1).
- The room 9 box is in Richard's name and opens only with his signature, which is why Umbra watches instead of acting (3.2).
- Umbra comes after Richard over his old markers in act 4, which meets Gus's condition word for word (3.3).
- Bennett keeps the tape as insurance against Gus; city inspectors give him lawful access (3.4).
- Revaris insists the District keep the Starflare name, with Bennett's supper-club line to explain it (3.5).
- Raffy stole the wrong envelope; the drive is Ray's secret insurance copy of Wren's logs (3.6).
- The commission only saw the overhead dealer camera; Gus kept the wide-angle tape that shows him (3.7).
- Gus's debt is personal and off the books; Stella misreads his remortgage (3.8).
- After the sale, Stella asks Tae who is really behind Revaris (3.9).
- The Aurelian takes Revaris cameras and its own archive keeps the Bennett meeting; it rips them out only if Umbra is exposed (3.10).
- Gus tells himself Umbra's promise but doesn't believe it (3.11).
- Seed S11's meeting is Ray taking a secret finder's fee from the filing lawyer (4.1).
- Ray slips you the Cinder Room password in act 1 (4.2).
- Mitch names the filing lawyer's trust account, and the commission subpoenas Bennett's cut (4.3).
- Vivian invites you to the yachts; Ray invites you to Blackspruce Lodge (4.4).
- You glimpse Vivian in act 2 and meet her at the Gilded Pine in act 3 (4.5).
- Raffy's drive holds two files, the face-match logs and Wren's payers list, and you decide what to do with each (4.6).
- The District never sells the name to Gus's side; Tae is a neutral buyer (4.7).
- Gus arranged room 9 through Pete, and Bennett took over the bill (4.8).

All 43 audit findings are now fixed, and the work was merged into the main site.

## 25 Oct 2026

### Audit fixes

- "1960s" became "rocket age" on the live pages; the game names no decade (1.8).
- Wren is early 30s; she invented the cameras and earned the reins through the Flare camera deal (1.9).
- Graham/Claire's middle beats move into Northlight, so the arc needs only one spring train ride (1.10).
- The outfit is Umbra to everyone; Revaris Technologies is only its secret front company (2.1).
- The Flare's sale is public; the mystery (M2) is proving Revaris is Umbra's front and why Gus won't say so (2.2).

## 24 Oct 2026

### The site

- The design docs site was added to GitHub and published on GitHub Pages. It is the single source for the design: every character has a full page and every planning task sits on the page it belongs to.
- The working title was settled: **Starflare Casino**.
- Jobs, the phone and inventory were planned (task 13).
- Mike's outside narrative consistency audit (43 findings) was added, to be fixed one finding at a time.

### Audit fixes

- Act 2 runs about 8 hours to fit its season gate; about 42 hours in all to opening night (1.1).
- Raffy's envelope hand-off moves to night one (A1.2); A1.5 is the payoff weeks later (1.2).
- Every act table on the Story timeline has a code column, one row per code (1.3).
- The "three rungs in one session" promise is dropped; the opening is player-paced (1.4).
- The rocket sign stays on the empty Flare until demolition (1.5).
- The rocket fin is gone; the Flare's lucky object is the spaceman statue (1.6).
- Pete started as a supper-club busboy, like Jalen (1.7).

## Before the site (15 to 23 Oct 2026)

- **15 Oct:** the first game design document and story direction.
- **16 to 19 Oct:** the city of Northlight, its venues and excursions, the player and opening, the cast and the Starflare story, the mysteries and the story map.
- **21 to 22 Oct:** the seeds and reveals, the endings and post-game, and the continuity fixes from the story review.
- **23 Oct:** the premise revision (Revaris buys the Flare; act 4 wins back the name by leverage) and the first version of this site.

Those decisions are recorded in each page's Planning record and on [Early drafts and notes](#history).
