# Starflare Casino: Story Bible

Compiled 2026-10-27 from the author's design docs. This is an archive of existing material. Nothing here is invented. Where the notes are silent a field says "Not stated", and the gap is listed in section 5.

## How to read this document

**Source tags.** Every fact carries its source in parentheses. Paths are relative to `/Users/mike/Projects/casino-game/docs/docs/` unless noted.

- `people/x.md` = a character page
- `pages/x.md` = a design page
- `plan/tNN.md` = a planning-task record, where the "Already decided" paragraph holds the settled decisions with dates
- `data/story.js` = `/Users/mike/Projects/casino-game/docs/data/story.js` (the site's character list `C`, ties `E`, event list `EV`, event descriptions `EVD`, walk-on list `EVX`, seeds `SEEDS`, outcomes `OUT`, acts `ACTS`, mysteries `MYST`)
- `audit` = `/Users/mike/Projects/casino-game/audits/2026-10-24-narrative-consistency.md`. Its "Fix status" table is treated as authoritative where a page still disagrees. Every page that still disagrees is listed in section 5.

**Dates.** "Mike 2026-10-26" and similar are the author's own settlement stamps, kept as written.

**Open-question references.** "OQ n" points to numbered items in section 5.

**Stale-material warning.** `pages/notebook.md` ("copied here word for word"), `pages/direction.md`, `pages/review.md`, `pages/cast-planning.md`, `pages/bible-fixes.md`, and parts of `plan/t06.md` and `plan/t09.md` carry superseded details. They are cited only for what they alone say. Conflicts with the live pages are flagged in section 5.

**Order of authority used here.**
1. The audit "Fix status" table.
2. The live story pages and character pages.
3. `plan/` records.
4. `data/story.js`.
5. The historical pages above.

`pages/gdd.md` states: "Where this page and a story page differ on a story detail, the story page wins."

---

# 1. CHARACTERS

**Entry fields.** Aliases; tier and group; role; look (brief); personality; want/goal; wound or backstory; secrets; arc and outcomes; relationships; first appearance; event codes; player interaction. "Not stated" means the notes do not say.

**Event codes.** Codes come from the character's `appears` list in `data/story.js`. Extra codes found in `EV`/`EVD` but missing from that list are marked "also in EV/EVD". Code families:
- A1.x to A5.x: main-story beats
- M1 to M4: mysteries
- S1 to S13: seeds
- R1 to R10: side arcs
- X1 to X4: excursions
- O1 to O10: outcomes
- EP: epilogue

(`pages/event-legend.md`)

**Tiers** (`plan/t06.md`, settled 2026-10-17):
- Core: Gus, Rosa, Kasia, Ray, Vivian, Chase/Brooke, Jalen, Bennett, Tae.
- Supporting: Dot, Mitch, Stella, Pete, Wren, Mayor Dabareski, Ed, Kiran, Juan, Raffy.
- The three romance pairs get "a small arc that opens only if you lean in."
- Richard is "Supporting (story)" and Troy is "Minor" (`data/story.js`).
- Portrait budget: "9 core and 10 supporting, about 19 portraits ($380 to $1,520 at $20 to $80 each), plus Richard and the six romance versions if they get portraits" (`plan/t06.md`). Troy is not mentioned in that count (OQ 41).

**Naming rule** (`plan/t06.md`, Mike 2026-10-17): "characters go by first names only; add a last name only if a story needs one." The one exception is Mayor Dabareski.

## 1.1 Backwater and the Bridges

### Dot
- **Aliases:** Dot only. Notebook: "Name: a play on dice. Dropped names: Bev Halloran, Starview, Tailwater" (`pages/notebook.md`, cast).
- **Tier/group:** Supporting; Backwater. Where: the Slack Water Motel desk (`data/story.js`).
- **Role:** Owns the Slack Water Motel in Backwater and works the desk herself. "Slightly older, sympathetic and in the know." (`people/dot.md`)
- **Look:** Grey hair, a headband, an open Hawaiian-style shirt over a white T-shirt, jeans rolled at the ankles, white socks and gym shoes. (`people/dot.md`)
- **Personality:** Chatty, motherly, "a story for everything." Voice sheet: "She doesn't lie; she warns sideways." Sample: "I'd stay on this side of the river tonight, hon. No reason." (`people/dot.md`)
- **Want/goal:** Not stated. Closest: she and Gus "haven't really spoken since the funeral," and each quietly asks the player how the other is doing (`people/dot.md`, `data/story.js` R1).
- **Wound/backstory:**
  - Her late sister, Alice (named by Mike, 2026-10-27), was Gus's wife, so she is Gus's sister-in-law and Stella's aunt.
  - It is a reveal (a framed photo of the two sisters in Gus's office), "never part of her intro."
  - The shirt is a "souvenir of the one trip she took after her own big win."
  - Age and earlier life: Not stated (OQ 40).
  - (`people/dot.md`)
- **Her stories** (`people/dot.md`, `plan/t08.md`; the suitcase story was dropped 2026-10-22):
  - The flood: she let everyone sleep upstairs free and the dockworkers brought food, hence her soft spot for union people like Rosa.
  - The quiet man in room 9: he has lived there "the last couple of years," his bill comes in an envelope every week, and he never takes off a key on a lanyard.
  - The shirt, told once she trusts you.
- **Warnings:** Ray's loans; "the dice behind Millstone Lanes when the vice squad is sweeping"; the Cinder Room; carrying winnings over the Long Bridge before dawn (`people/dot.md`).
- **Secrets:** None stated. She "doesn't know his story" about Richard and "only knows Pete asked a favor" (`people/richard.md`, `people/dot.md`).
- **Arc/outcomes:**
  - R1: reconciling with Gus; she is present when Gus tries to quit (A4.2).
  - O6 (Gus lets go) brings back "Visits with sober Gus, Dot and Stella."
  - Pete is her go-between with Gus.
- **Relationships:**
  - Related line: Gus, Stella, Rosa, Richard, Ray, Raffy, Pete (go-between with Gus) (`people/dot.md`).
  - Ties in `data/story.js`:
    - Gus (family, brother-in-law)
    - Stella (family, aunt)
    - Pete (ally, go-between)
    - Richard (ally, "room 9 guest")
- **First appearance:** A1.1 Arrival (Night 1). Her register is where the player enters name and gender (`pages/storymap-beats.md`).
- **Codes:** A1.1, A1.5, A1.7, A3.7, A4.2, M4, R1, R10, S11, O6, EP (`data/story.js`).
- **Player interaction:**
  - Register: sign your name and tick Mr. or Ms. She asks what brings you; you answer with an action (A1.1).
  - Jobs (proposal): "Motel rooms in the morning for a night off rent; something left behind in room 9 leads into M4."
  - Warns you about Ray (A1.5) and the Long Bridge (S11).
  - Her stories unfold over time (R10).
  - You carry messages between her and Gus (R1).
  - "No guest photo wall (Mike: an invasion of privacy)."
- **Sources:** `people/dot.md`, `plan/t05.md`, `pages/storymap-beats.md`, `data/story.js`.

### Kasia
- **Tier/group:** Core; Backwater. Where: Kasia's Tap.
- **Role:** Owner of Kasia's Tap. "First credit and first friend." Offers the two jobs that open her back room, then hires you for the wedding. (`people/kasia.md`, `data/story.js`)
- **Look:** Dark hair in a braid, a bar towel over her shoulder, a flannel shirt with rolled sleeves, her dad's old mill union pin on the collar (pin dropped 2026-10-28, see 4.1.8). (`people/kasia.md`)
- **Personality:** Mid-40s. "Warm but tough. Runs a tight bar, gives credit but keeps a ledger and remembers every cent, quick to defend her regulars, slow to forgive a skipped debt." Her credit is how the player feels the reliable/flaky scale. (`people/kasia.md`)
- **Want/goal:** Not stated. Per `plan/t05.md`, "she'd rather keep you for her back room" than send you to the Flare.
- **Backstory:** Born in Backwater to a mill family (Polish-American dad, Mexican-American mom). (Dad is now a businessman, 2026-10-28, see 4.1.8.) Took over her dad's tavern, where the mill shifts drank before the mills closed. Knows everyone's family and remembers the town before the casinos. (`people/kasia.md`)
- **Secrets:** Quietly pays "for amusement only" machine winners from the till (illegal but too small for Ed) (`pages/northlight.md`). Never discusses the back room in front of strangers (voice).
- **Arc/outcomes:** No arc stated beyond her appearances (arc proposed and accepted in `suggestions/S-024`). She appears at A5.5 (co-op / relaunch) and in O4 (people's Starflare).
- **Relationships:**
  - Related line: Rosa (deals her back room), Raffy (drops envelopes at her bar), the wedding party, Ray (asks around her bar for Raffy) (`people/kasia.md`).
  - Ties in `data/story.js`:
    - Rosa (ally)
    - You (money: credit on her ledger)
- **First appearance:** A1.2 (Night one, Kasia's Tap).
- **Codes:** A1.2, A1.3, A1.4, A1.5, A5.5, S2, S11, O4 (`data/story.js`). S11 is on her list but not on the S11 character list (OQ 16).
- **Player interaction:**
  - Pull tabs and "for amusement only" video poker at about a dollar a go; she opens your line of credit in her ledger (A1.2).
  - Two jobs: a bar-back shift (a drunk regular drops a fat wallet; $60 cash) and taking Juan a box of free-drink coupons. Then you are invited into the back room (A1.3).
  - A separate third job, the wedding (A1.4): server or busser, $80 cash plus tips (proposed). Mid-shift she relays dance offers.
  - Keg run (table event, proposal).
  - Seed S2: cover an old mill pensioner's tab ($30).
- **Stale on her page:** Hook says "mentions the Flare on night one" (OQ 6).
- **Sources:** `people/kasia.md`, `plan/t05.md`, `plan/t06.md`, `plan/t13.md`.

### Rosa
- **Tier/group:** Core; Backwater. Where: Kasia's back room; near the Prairie Queen landing (`data/story.js`). "never inside the Flare" (`people/rosa.md`).
- **Role:** Blacklisted former dealer. She deals Kasia's back room. She "teaches the player the rules of the games and sharpens their read of tells, never giving certainty." Tied to the dealers' union. (`people/rosa.md`)
- **Look:** Early 40s, hair pinned up with a pencil, old dealer's vest with the casino patch torn off. "Fast, precise hands are description only." (`people/rosa.md`)
- **Heritage:** Puerto Rican; parents came up for factory work; grew up in the Levee.
- **Personality (voice):** Precise, quiet, teacherly; rules and odds; never mentions the Starflare. "Lies by telling the truth about the wrong thing." Sample: "Watch his hands, not his face. Faces are for amateurs."
- **Want/goal:** Her real payoff is her license, "won back in any ending by solving M1" (`people/rosa.md`). Earlier: "ultimately you give her a job at the reopened Starflare" (superseded by the license payoff, 2026-10-22).
- **Wound/backstory (M1):**
  - She "really was guilty, but for a good reason."
  - Gus, drunk, lost the staff payroll at her table, so she dealt him back to even.
  - Cameras caught it and she took the fall without saying why.
  - The commission saw only the overhead dealer camera (hands, chip flow, no faces).
  - Gus kept the only wide-angle tape.
  - The commission's penalty ordered new cameras, which opened the door to Wren's deal.
  - The torn patch is from the Starflare. She was "never a Flare regular and won't step inside it."
- **Secrets:** She protects Gus's secret (took the fall for him).
- **Arc/outcomes:** M1 solved: "Gus admits it; Pete and Gus testify and Rosa gets her license back." If you run a floor (O1, O2, O3, O4) she manages all casino staff; otherwise she deals at the Harborlight (`people/rosa.md`). After the scatter she keeps dealing Kasia's back room (A3.5).
- **Relationships:**
  - Related line: Gus (she covered for him), Pete (goes quiet at her name), Kasia, Bennett (holds the tape after the fall), Vivian (her lawyer can pull the commission file), Kiran (she may flag his table) (`people/rosa.md`).
  - Ties in `data/story.js`:
    - Kasia (ally)
    - Gus (secret)
    - Bennett (secret)
    - Kiran (ally)
    - Vivian (ally)
    - You (ally: she teaches you the games)
- **First appearance:** A1.3 (back room: she warns you off a hand).
- **Codes:** A1.3, A2.3, A3.2, A3.5, A4.7, A5.3, M1, M3, O1, O2, O3, O4, EP, A5.7 (`data/story.js`).
- **Player interaction:**
  - Mid-hand she gives "a look, or a quiet word, that says fold" (A1.3).
  - She teaches the rules and tells.
  - She won't enter the Flare (A2.3).
  - She joins the clean fight (A3.2).
  - She can speak for you at the license hearing (A5.7).
  - Companion (review proposal, unsettled): "After she warns you off a hand at Kasia's and you listen."
- **Sources:** `people/rosa.md`, `plan/t08.md`, `pages/storymap-beats.md`.

### Mitch
- **Tier/group:** Supporting; the Bridges. Where: Bridgehead Fuel & Food counter.
- **Role:** "The side-action guy." Cynical clerk. Quietly books Haybale Downs bets out the back (an event source, not a game). (`people/mitch.md`)
- **Look:** Early 50s, white, straight brown hair, jeans and a clerk-style collared shirt, a cynical look.
- **Personality (voice):** Clipped, cynical, mumbled; horses and prices; "never says Bennett's name until he trusts you." Lies by pretending not to know. Sample: "Pump four's broken. Has been since I started."
- **Want/goal:** Not stated.
- **Wound/backstory:** Years ago he made big bets on supposedly inside information and lost almost everything. The bad tip came from Bennett. It left him distrustful of everyone. Finding out about Bennett is how you earn his trust. The tip ran through the filing lawyer's trust account "years ago."
- **Secrets:** The book itself (illegal; raises heat).
- **Arc/outcomes:** He recognizes the lawyer on the Flare's sale filing from Bennett's old deals (M2 clue). Once he trusts you he can name the lawyer's trust account, the proof for the by-the-book route (A4.11).
- **Relationships:**
  - Related line: Bennett, Raffy ("Wren files Mitch's slips"), the player's Haybale tip (`people/mitch.md`; the Raffy line is unclear, OQ 19).
  - Tie: Bennett (secret: "Bennett's bad tip ruined him").
- **First appearance:** A1.6 (Over the bridge, Bridgehead).
- **Codes:** A1.6, A3.7, M2, R5, S3, X3, A4.11 (`data/story.js`).
- **Player interaction:**
  - Gossip.
  - Haybale Downs tips (X3).
  - Jobs: count the bets (a cut); run bets ($200).
  - Table event "A slip across the bridge."
  - Seed S3: spot the trucker $20 on diesel.
- **Sources:** `people/mitch.md`, `plan/t13.md`.

### Juan
- **Tier/group:** Supporting; Backwater. Where: the back lot of Millstone Lanes.
- **Role:** Bowling-alley mechanic who runs the back-lot dice. He is the one who tells you about the Flare. (`people/juan.md`)
- **Look:** Early 40s, Mexican-American, dark stubble, grease-stained work shirt with name patch, bowling-alley cap, rag in back pocket.
- **Personality:** Friendly with league bowlers, indifferent to everyone else, annoyed by casuals. Keeps his game fair, "knows every Backwater rumor," and Ed leaves him alone. Never says "dice" inside the alley; it's "the late game." Lies with a shrug and "Couldn't tell you."
- **Want/goal:** Not stated.
- **Backstory:** Started as a teenager clearing jammed pins; about 25 years there. The owner looks away because the dice money pays for parts.
- **Secrets:** The illegal back-lot dice game.
- **Arc/outcomes:** R3: Umbra tries to muscle in with loaded dice; help him push them out and he teaches you to spot a weighted die. Raffy hides with him. He appears in O4 and A5.5.
- **Relationships:**
  - Related line: Raffy, Ed, the Tuesday league (S1), Umbra.
  - Tie: Raffy (ally: hides him at Millstone).
- **First appearance:** A1.3 (you deliver coupons for his Tuesday league).
- **Codes:** A1.3, A1.5, A5.5, R3, S1, S11, O4. S11 is on his list but not on the S11 character list (OQ 16).
- **Player interaction:**
  - Delivery of coupons (A1.3).
  - Bowling in the Tuesday league warms him up.
  - Lane-shift job ($50; a league regular is short on fees).
  - Seed S1: $40 for the team shirts.
  - Companion (review proposal): "Help push Umbra's loaded dice out of Millstone Lanes."
- **Sources:** `people/juan.md`, `plan/t13.md`.

### Raffy (Rafael)
- **Aliases:** Raffy; Rafael ("full" in `data/story.js`); "the stranger" / "the courier" (`people/raffy.md`).
- **Tier/group:** Supporting; Backwater. Where: Kasia's Tap on night one, later hiding at Millstone Lanes.
- **Role:** Courier kid; the first seed.
- **Look:** 19, Filipino-American, skinny, oversized hoodie, cracked phone, one earbud, delivery-bike helmet on his backpack. Talks fast, laughs nervously.
- **Personality (voice):** Fast, run-on, always halfway out the door. Lies by talking faster. "Hold this till Friday? Don't open it. Don't lose it. Fifty bucks. Cool? Cool." "A good kid making a dumb mistake."
- **Want/goal:** Cover his mom's rent. Mom is "a nurse working double shifts."
- **Backstory/wound:**
  - Runs envelopes for Ray's collections "because it paid better than delivery and made him feel like somebody."
  - He "lifted a fat envelope from Ray's car to cover her rent, thinking it was only collection cash," then panicked (`people/raffy.md`).
  - The Planning record says "he skimmed" (OQ 24).
- **Secrets:** The envelope holds about $2,000 and a thumb drive.
  - Ray's insurance copy of Wren's face-match logs, taken off the Prairie Queen camera feed. (Superseded 2026-10-28: an encrypted Revaris key ledger; see 4.1.8 and `suggestions/S-058`.)
  - Wren's list of who pays her to stay quiet.
  - Two files (audit 4.6): the logs, which prove the Revaris feeds go to Umbra and are all the clean route at A4.3 uses; and the payers list. (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
  - First time you open the drive you choose its fate. Destroy it (straight, soft). Keep it (crooked; a black-hat tool; Wren may come looking). Or later hand Ed the whole drive, names and all (ruthless). (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
- **Arc/outcomes:**
  - A1.5: Ray asks around and the envelope comes due. Return it to Ray; keep the cash; hand the drive to Ed; or find Raffy hiding at Millstone Lanes with Juan and help him pay Ray back. (Superseded 2026-10-29, see 4.1.8: A1.5 settles the cash and Raffy; the drive stays with you and its fate, Ed included, comes from act 2 on.)
  - Later: A4.3 (the drive as proof), A4.5 (Wren wants the drive), O8, O10.
  - Companion (review proposal): "A runner: gets messages and cash across the river."
- **Relationships:**
  - Related line: Ray, Kasia, Dot, Juan, Ed, Wren.
  - Ties in `data/story.js`:
    - Ray (money: courier; stole the wrong envelope from his car)
    - Juan (ally)
    - You (secret: holding his envelope)
- **First appearance:** A1.2.
- **Codes:** A1.2, A1.5, A4.5, M2, R3, R4, O8, O10, A4.3 (`data/story.js`).
- **Player interaction:**
  - Hold the envelope (it is a story key in Pockets; the $2,000 does not count toward best bankroll until you keep it at A1.5).
  - Open it any time.
  - The four A1.5 choices above.
- **Sources:** `people/raffy.md`, `pages/gdd.md`, `plan/t13.md`.

### Troy
- **Tier/group:** Minor; the Bridges. Where: Bridgehead Loan & Pawn, next to Bridgehead Fuel & Food.
- **Role:** Pawn-shop owner who buys your items for a fraction and holds each a few days so you can buy it back. "Some items are worth far more to one person." Buzzer door, cage counter, open until 3am. (`people/troy.md`, `plan/t13.md`)
- **Look:** Male, white, stocky but strong, about 5'9", tattoos on his arms (Mike, 2026-10-23).
- **Personality (voice):** Short and flat; names a number, then waits; never haggles first. "It's worth what someone pays"; never asks why you're selling. Lies by pricing low and blaming the market; honest about the buy-back date. Sample: "Two hundred. You got till Thursday to want it back."
- **Want, wound, secrets, arc, relationships:** Not stated.
- **First appearance:** Not coded. Appears in no event, seed, outcome or tie (`data/story.js` `appears:[]`).
- **Player interaction:** Pawn items; buy them back. Story keys cannot be pawned "unless a scene offers it" (`pages/gdd.md`).
- **Sources:** `people/troy.md`, `plan/t13.md`, `pages/gdd.md`.

## 1.2 The Starflare

### Gus
- **Aliases:** Gus; "the founder's son"; surname Hamilton "only if a story needs it" (`people/gus.md`).
- **Tier/group:** Core; Starflare. Where: Starflare floor and office; coffee under the rocket sign.
- **Role:** Owner of the Starflare Club; the player's emotional anchor and the Flare's fate.
- **Look:** Very heavyset ("Teddy from Bob's Burgers without the beanie, as an artist reference only"), silver hair combed back, his dad's old Starflare dinner jacket straining at the buttons, a chipped rocket-logo coffee mug.
- **Personality:** Proud, sentimental, stubborn, bad with money. Voice: long, warm, nostalgic sentences that trail off; "my dad's place"; never says the word "debt"; lies by changing the subject to the old days. Sample: "You should've seen this floor when the band played. Anyway. Coffee?"
- **Want/goal:** Not stated as such. Closest:
  - His late wife's touches are all over the Flare, "which is why he can't let go." Her name is Alice (Mike, 2026-10-27; see 4.1.8).
  - He refuses to sell "his dad's place to a chain."
  - He tells the regulars "nothing's changing, they gave me their word" because the alternative is admitting he sold to the people who blackmailed him.
- **Wound/backstory:**
  - Late 50s; son of Mr. Hamilton, who built the Starflare in the rocket age. Grew up there and inherited it "with its debts and secrets."
  - Late wife was Dot's sister; he and Dot "haven't really spoken since the funeral."
  - Years ago, drunk, he lost the staff payroll at Rosa's table and let her take the fall.
  - The commission's penalty ordered a modern camera system he couldn't afford.
  - (`people/gus.md`)
- **Secrets (as written):**
  - He is secretly a gambler in the Cinder Room, and "Wren's cameras see all of it."
  - He signed Wren's camera deal knowing it was Umbra (`pages/room9.md`: "his second secret, next to his gambling").
  - He let Rosa take the fall.
  - He gave Richard the key to a safety-deposit box rented in Richard's name. It holds the original signed camera contract and Gus's own recording of Bennett's demand, "the only copy."
  - He arranged room 9 through Pete and paid the first weeks.
  - He quietly remortgaged his house (`people/stella.md`).
  - He sold the Flare to Revaris knowing it was the mob.
  - He kept the only wide-angle tape of Rosa's table.
- **Arc/outcomes:**
  - Acts 1-2: Ray collects (A1.10, A2.1); Gus turns down Tae's satellite offer; a "friend of Ray's" buys the Flare and clears his debt (A2.2). Gus tells everyone nothing will change.
  - A3.1 breaks the lie.
  - A4.1: he admits he sold his dad's place to Umbra. Forgive him or hold it over him.
  - A4.2: relapse at the Cinder Room, borrowing from Ray again; help him quit with Dot and Stella. If no reopening follows, O6.
  - O1 (ribbon with you), O2 (without Gus), O4 (hosts), O5 (greets at the Aurelian), O6, O7. He is the name-return beneficiary of the act 4 routes.
  - His trust is "ordinary NPC standing plus a few flag checks" (`people/gus.md`, `pages/storymap-open.md`).
- **Relationships:**
  - Related line: Dot, Stella, Pete, Rosa, Ray, Bennett, Wren, Tae, Jalen, the founder (`people/gus.md`).
  - Ties in `data/story.js`:
    - Stella (family, father and daughter)
    - Dot (family, brother-in-law)
    - Pete (ally, best friends)
    - Jalen (ally)
    - Miles (ally: "Gus gave them their first gig")
    - Ray (money)
    - Richard (money: "his markers took Richard's house")
    - Bennett, Wren, Rosa (secret)
    - Tae (rival: "rival visions; old family friend")
    - You (ally)
- **First appearance:** A1.6 (coffee, rub the spaceman's boot).
- **Codes:** A1.6, A1.7, A1.9, A1.10, A2.1, A2.2, A3.1, A3.2, A3.3, A3.4, A4.1, A4.2, A4.5, A4.10, A5.6, M1, M2, M4, R1, S3, S4, S6, S7, S11, S12, O1 to O10, EP (`data/story.js`). Also in EV: A4.4. S11 is on his list but not on the S11 character list (OQ 16).
- **Player interaction:**
  - Coffee and the spaceman's boot (A1.6).
  - Keep his secret or not (A1.7).
  - See him at the Cinder Room (A1.9); lend him cash for an IOU, refuse, or walk him out (S6).
  - Cover part of the debt (A2.1).
  - Confront him (A4.1).
  - Help him quit (A4.2).
  - Win the name back "for" him (act 4 routes).
  - Cut the ribbon (A5.6).
  - S12: he laughs at the bobblehead.
  - Tell (review proposal): chin-up swaps to looking at the mug; the chipped mug empty.
- **Sources:** `people/gus.md`, `pages/room9.md`, `plan/t07.md`, `plan/t08.md`, `data/story.js`.

### Stella
- **Tier/group:** Supporting; Starflare. Where: Starflare office, Aurelian meetings.
- **Role:** Gus's grown daughter; a finance analyst downtown who "has run the Flare's numbers and knows it's sinking." The human face of the buyout and closure endings.
- **Look:** Mid-20s, long brown hair, well educated and attractive (confirmed Mike 2026-10-23).
- **Personality (voice):** Precise, well spoken, "an analyst's numbers"; the Flare in figures, her dad in stories. "Dad" to his face, "my father" to strangers. Never criticizes him in front of anyone; worry comes out as concern for his health. Sample: "I'm not trying to take it from him. I'm trying to keep him."
- **Want/goal:** She "loves her dad and thinks the Flare is killing him, so she wants him to sell." After the sale she asks Tae to find out who is really behind Revaris because "the deal smells wrong to her" (audit 3.9).
- **Wound/backstory:**
  - She never sees the Ray debt (it is Gus's personal debt, off the Flare's books). She sees only operating losses.
  - She notices Gus quietly remortgaged his house and reads it as him propping up the Flare. "When the truth comes out, she realizes what the mortgage was really for."
  - Dot is her aunt; Pete is "Uncle" Pete; Tae was "like a big brother to her when she was little."
- **Secrets:** She does not know he gambles until the torn patch (M1), "where she goes speechless" (voice) (OQ 33).
- **Arc/outcomes:**
  - A2.5 false lead in M2: she meets Tae at the Aurelian; clearing her brings her and Gus closer.
  - A4.2 helps him quit.
  - A4.6 backs Tae's offer, which leads to O5.
  - O5, O6.
- **Relationships:**
  - Related line: Gus, Dot, Pete, Tae, Felix/Nora (they see who she meets).
  - Ties:
    - Gus (family)
    - Dot (family, aunt)
    - Pete (ally, "Uncle" Pete)
    - Tae (ally, big brother figure)
    - Felix (ally)
- **First appearance:** A1.9 (worries about her dad's health and the numbers).
- **Codes:** A1.9, A2.2, A2.5, A4.2, A4.6, M2, R1, O5, O6, S13 (`data/story.js`).
- **Player interaction:**
  - Follow her at the Aurelian or not (A2.5); clear her privately and compare notes.
  - Help Gus quit with her (A4.2).
  - Side with her and Tae (A4.6).
  - Warn Tae about the cameras via Stella (S13).
- **Sources:** `people/stella.md`, `plan/t08.md`.

### Pete (Petras)
- **Aliases:** Pete; Petras; "Uncle" Pete (to Stella). Calls the player "kid" "until they earn their name."
- **Tier/group:** Supporting; Starflare. Where: the Starflare floor.
- **Role:** Pit boss and Gus's best friend. "Face for comps, credit and higher limits; watches your hot streaks." Quiet go-between for Dot and Gus; your way in to Gus.
- **Look:** Stocky, thin neatly trimmed beard (not a mustache), short sandy-grey hair, dark three-piece suit with the vest always buttoned, a gold 25-year rocket pin from Gus's father, reading glasses on his head, a rating card.
- **Personality (voice):** Short and flat; numbers and facts; never compliments Gus to his face; lies by not answering. Sample: "Kid. Your rating says you're a $50 player. Play like one." Gruff, dry humor.
- **Want/goal:** Not stated. At the fall: "he needs the money and can't afford to retire yet" (A3.4).
- **Backstory:**
  - Lithuanian-American, about 60; grew up in the Levee alongside Gus; grandparents and dad worked the steel mills.
  - At 19 the founder hired him to bus tables at the supper club the Flare then was.
  - He learned cards by watching, and was "one of its first dealers" when the Flare got its license. He started exactly where Jalen is now.
  - Best man at Gus's wedding; kept the Flare running after the funeral; "the only one who tells Gus hard truths." (audit 1.7)
- **Secrets:** He quietly arranged room 9 at Dot's motel for Gus. "Dot only knows Pete asked a favor."
- **Arc/outcomes:**
  - Authored drift: goes to Tae at the Aurelian at the fall (A3.5).
  - A3.4: on the Flare's last night he is alone at the dark pit and hands you his gold rocket pin. Choices: tell Gus, push Pete to tell him, or say nothing. Returning the pin when you open a floor hires him home, "unless you sold Gus out."
  - O1; the epilogue.
- **Relationships:**
  - Related line: Gus, Stella, Dot, Tae, Jalen, Rosa (goes quiet at her name).
  - Ties:
    - Gus (ally)
    - Stella (ally)
    - Dot (ally, go-between)
    - Jalen (ally: "started as a busboy too; keeps an eye on him")
    - Tae (ally: "hires him away at the fall")
- **First appearance:** A1.7.
- **Codes:** A1.7, A1.9, A2.1, A2.3, A3.3, A3.4, A3.5, A5.3, M1, R1, O1, EP, A5.7, M4 (`data/story.js`).
- **Player interaction:**
  - Rating card, comps and limits.
  - Tell him or not what you saw at the Cinder Room (A1.9).
  - Tell him about Ray's collection (A2.1).
  - Valet job ($80 plus tips).
  - Table event "Pete needs a hand."
  - The pin (A3.4, A5.3).
  - He speaks for you at the hearing (A5.7).
- **Sources:** `people/pete.md`, `plan/t06.md`, `pages/room9.md`.

### Jalen
- **Tier/group:** Core; Starflare. Where: Busing tables at the Flare, watching the poker room.
- **Role:** Busboy learning poker; you can stake him.
- **Look:** Early 20s, Black, Starflare busboy vest over a white shirt with sleeves pushed up, a fade haircut; plays a handheld game (a made-up brand) on breaks.
- **Personality (voice):** Well spoken and "super polite"; slips into younger talk about video games. "The one the player should want to see succeed." Sample: "Excuse me, sir. I don't mean to overstep, but the gentleman in seat three has been bluffing all night."
- **Want/goal:** "Saving up to get out of Backwater" (not out of Northlight).
- **Backstory:** Grew up in Backwater; learned poker by quietly watching the poker room.
- **Secrets:** None stated.
- **Arc/outcomes:**
  - Feel-good rags to riches. He gets out of Backwater, "pays back his stake in act 4," and becomes one of only two backers of the lakefront Starflare.
  - Per `pages/storymap-beats.md` A5.2 and audit 2.9: "Jalen invests and runs the poker room (the hotel once the post-game tower opens)."
  - A spotless player may see Wren threaten him instead (S10).
  - Other pages say "head of the hotel" (OQ 1).
- **Relationships:**
  - Related line: Gus, Pete, the player (his backer), Wren.
  - Ties:
    - Gus (ally)
    - Pete (ally)
    - You (money: "you stake him")
    - Wren (secret: "threat if you're spotless")
- **First appearance:** A1.7.
- **Codes:** A1.7, A5.2, A5.5, R2, S10, O1, O4, EP.
- **Player interaction:**
  - Stake him in poker; his wins pay you back (R2). Your choices decide whether he climbs clean.
  - Companion (review proposal): "You stake his first buy-in."
  - "Cover Jalen's $500 buy-in" is an example stake choice in `pages/writing.md`.
- **Sources:** `people/jalen.md`, `pages/storymap-beats.md`, `data/story.js`.

### Chase / Brooke (the rival)
- **Aliases:** Chase (male version, if the player ticks Mr.); Brooke (female version, if Ms.). Formerly "Teddy" (`people/chase.md`, `plan/t06.md`). Older notes saying Teddy mean Chase/Brooke.
- **Tier/group:** Core; Starflare. Where: RailYard, Harborlight; "moves up and down the ladder" (OQ 7).
- **Role:** The other newcomer; friendly rival turned arrogant.
- **Look:** White, in their 30s, from upper-middle-income families, well dressed but not over the top at first. Divorced, no kids, starting fresh, "a clear head start on you."
- **Personality (voice):** Friendly, then clipped and showy; wins, never losses; lies by overstating. Sample: "Just a cold night. I'll be up by Friday."
- **Want/goal:** Not stated. "Chases the same tables, backers and breaks you do."
- **Arc/outcomes:**
  - Starts friendly, turns arrogant and self-centered as they succeed.
  - A2.7: luck turns, they borrow from Ray.
  - A3.8: they hit bottom and come to you for cash. Lend it and "all is well"; refuse and "they may steal from you later" (A4.8).
  - Authored drift: the slide happens either way.
- **Relationships:**
  - Related line: Ray, the player.
  - Ties:
    - Ray (money: "borrows and can't pay")
    - You (rival)
- **First appearance:** A1.8 (the RailYard).
- **Codes:** A1.8, A2.4, A2.7, A3.8, A4.8.
- **Player interaction:**
  - Collect-for-Ray job can put them in front of you (acts 2-3, A2.7/A3.8).
  - Lend or refuse.
  - Tell (review proposal): checks phone; watch or cuff adjusted.
- **Sources:** `people/chase.md`, `plan/t06.md`, `plan/t13.md`.

## 1.3 Power players

### Ray
- **Aliases:** Ray. Formerly "Dutch" (renamed 2026-10-17 because it clashed with Dutch Schultz and Red Dead Redemption 2). "Read Ray wherever older notes say Dutch" (`people/ray.md`).
- **Tier/group:** Core; Umbra. Where: Starflare bar, Cinder Room, Blackspruce Lodge.
- **Role:** Loan shark; "your easiest big debt." Go-between in Gus's sale. Recruits new money for the Cinder Room.
- **Look:** Mid-40s, trying too hard: leather car coat over open-collared shirt, gold chain, a pinky ring he thinks is classy, a fat roll of cash, a car he can't quite afford.
- **Personality:** "A little too confident"; acts like the big man but answers to Wren, "a sore spot." Voice: too loud, too friendly; "friend", "buddy", big numbers; never says who he works for; lies by flattery. Sample: "Buddy! My favourite person. You know what I like about you?"
- **Want/goal:** Not stated. Context: "Resenting that Wren knows everything, he secretly copied her logs ... as insurance." (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
- **Wound/backstory:** Not stated beyond role.
- **Secrets:**
  - He copied Wren's logs off the Prairie Queen feed onto a thumb drive; Raffy stole it from his car by mistake (A1.2). He hunts quietly because Umbra must never learn he made it. (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
  - He makes indebted dealers (Kiran) skim the Prairie Queen to cover bad loans and thinks Wren does not know. She does.
  - He secretly took a finder's fee from the filing lawyer (S11).
- **Arc/outcomes:**
  - Collects on Gus (A1.10, A2.1), squeezes him until he sells, brings Umbra's offer as "a friend of mine."
  - Lends too much (Chase/Brooke) and "shrinks when the real Umbra shows up."
  - Gus borrows again in act 4 (A4.2).
  - Leverage via S11 and M3.
  - O8.
- **Relationships:**
  - Related line: Wren (his boss), Gus, Chase/Brooke, Kiran, Raffy (his courier), Bennett, Ed (keeps a file on him).
  - Ties:
    - Gus (money)
    - Kiran (money)
    - Raffy (money)
    - Chase (money)
    - Wren (secret: boss)
    - Ed (secret: file)
- **First appearance:** A1.5 (first listed code); his first in-person meeting is not stated; at A1.9 he slips the password (OQ 17).
- **Codes:** A1.5, A1.9, A1.10, A2.1, A2.2, A2.7, A3.8, A4.7, M2, M3, S11, X4, O8.
- **Player interaction:**
  - Loans; Ray's envelope run ($200); "Collect for Ray" ($400, only after several Ray jobs; debtors: the rival acts 2-3, Kiran any time, Gus in act 4).
  - Cinder Room password (A1.9).
  - Blackspruce Lodge invitation as stake partner (X4).
  - Return Raffy's envelope to him (A1.5; "a small Umbra favor and easier loans").
  - The plate on the Long Bridge (S11) is leverage.
  - Tell (review proposal): hands in coat pockets; pinky ring turned inward.
- **Sources:** `people/ray.md`, `plan/t08.md`, `plan/t13.md`, `plan/t09.md`.

### Wren
- **Tier/group:** Supporting; Umbra. Where: Cinder Room, Blackspruce Lodge (rarely seen).
- **Role:** Umbra's boss; runs a camera-blackmail business through Revaris Technologies.
- **Look:** Early 30s (about 32), hoodie, expensive sneakers, a plain watch worth more than Ray's car, always on a phone or tablet. "When she shows up in person it unsettles everyone."
- **Personality (voice):** Calm, quick, technical, polite; data and outcomes; never raises her voice or uses slang; "doesn't need to lie." Sample: "I already know. I wanted to see if you'd tell me."
- **Want/goal:** Not stated as such. As written: she wants to tie off loose ends. "Only two loose ends can tie Revaris Technologies to Umbra: Raffy's thumb drive ... and Gus." She wants Gus's recording and the original contract (the box). Her stated rule: she always has something on the player by act 4.
- **Backstory:**
  - "A tech genius and the old Umbra boss's granddaughter, whom nobody took seriously."
  - She left, made her first money exploiting people through technology (crypto scams in her early 20s), then invented AI face-matching cameras and built Revaris Technologies.
  - About four years before the game she came back and worked with her grandfather to land the Flare's camera deal.
  - About two years before the game he "handed her the reins over their heads."
  - She kept what her cameras caught on the old bosses and half the Machine. The old guard resent her. She keeps the Cinder Room open "out of sentiment or for laundering." (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
- **Secrets:**
  - Revaris is Umbra's front.
  - She pays Bennett's hidden stadium cut herself through Revaris. Her own books are the proof, and she owns him.
  - Her blackmail of Gus (Cinder Room footage).
  - She knows about Ray's skim and "lets him dig his own hole."
  - She never saw Bennett squeeze Gus (off the floor cameras).
- **Arc/outcomes:**
  - Buys the Flare via Revaris and puts its land into the Levee Stadium District, "making it the District's biggest investor."
  - A4.5 "Wren's deal": hand her Gus's markers or Raffy's drive and she pulls the name out of the District for her own crooked Starflare (O8 / O10).
  - S10: her file on you.
  - S13: the Revaris camera pitch to Tae.
- **Relationships:**
  - Related line: Ray, Bennett, Gus, Raffy (his drive holds her logs), Ed (wants her for surveillance and blackmail), Kiran. (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
  - Ties:
    - Bennett (secret: "owns him")
    - Gus (secret: "footage forced the camera deal")
    - Ray (secret: boss; knows about the skim)
    - Ed (secret)
    - Jalen (secret: threat if you're spotless)
- **First appearance:** A2.2 (first listed; she is behind the sale). First in-person appearance: not stated (OQ 17).
- **Codes:** A2.2, A4.3, A4.5, A4.7, M2, M3, R4, S6, S10, X4, O8, O10, S13.
- **Player interaction:**
  - Rare scenes.
  - A4.3: trade your silence for the name.
  - A4.5: her offer.
  - A4.7: tell her about the skim (she smiles; Ray is humbled; costs heat).
  - X4: appears at Blackspruce.
  - Tell (review proposal): eyes up from tablet; tablet screen dark.
- **Sources:** `people/wren.md`, `pages/gdd.md`, `plan/t02.md`, `pages/room9.md`.

### Bennett
- **Tier/group:** Core; the Machine. Where: Harborlight, Northern Ledger, his office in the Levee (`people/bennett.md`; `data/story.js`).
- **Role:** Levee alderman and ward boss who "holds the Flare's license"; the political engine of the stadium deal and the condemnation. Runs for mayor on "clean up Northlight." "An obstacle you negotiate with, never defeat."
- **Look:** Early 50s, very professional (Mike). A photo of his father on his office desk is a clue in M4.
- **Personality (voice):** Polished public voice, plain private voice; "The Levee", "our families"; never mentions his father; lies with speeches. Sample: "This ward deserves better than a leaking roof and a flickering sign."
- **Want/goal:** Genuinely "loves the Levee and plays every room for his own gain." He wants a domed baseball stadium, Starflare Field, "to revive the Levee."
- **Wound/backstory:** His father Richard lost his house to markers at the Flare, so his dislike of it "is personal." He gave Mitch the bad tip. Publicly against gambling but "takes bribes from the casino business."
- **Secrets:**
  - Hidden cut of the stadium deal (paid by Wren through Revaris).
  - Umbra campaign money.
  - Used Gus's Umbra camera deal (traced via records and Ed's link) to force Gus to wipe Richard's debt.
  - Pays Richard's room weekly and never visits.
  - Keeps Gus's wide-angle tape "as insurance."
- **Arc/outcomes:**
  - A2.6 mayoral launch; A2.8 Starflare Field announced; A3.1 the drawing; A3.3 condemnation; A3.5 finds the tape.
  - The Starflare name sits in the stadium ordinance he sponsored. Routes: shame or reconcile via Richard (he amends it); the commission voids the clause (A4.11); Wren pulls the name out.
  - A5.7: stance set by your act 4 route.
  - Mayor's race result: Not stated (OQ 32).
- **Relationships:**
  - Related line: Richard, Wren, Ray, Gus, Mitch, Mayor Dabareski, Rosa, Graham/Claire.
  - Ties:
    - Richard (family: son)
    - Gus (secret)
    - Wren (secret)
    - Mitch (secret)
    - Rosa (secret: keeps the tape)
    - Graham (secret)
    - Miles (secret: saw his people take the tape)
    - Ed (secret)
    - Mayor (rival)
- **First appearance:** A2.2 (offstage); first in person A2.6.
- **Codes:** A2.2, A2.6, A2.8, A3.1, A3.2, A3.5, A4.4, A4.9, M1, M2, M4, R5, R6, S7, X1, O2, O5, O7, A4.11, A5.7.
- **Player interaction:**
  - Petition S7 (he reads your name at the hearing).
  - His line (A3.1): "The Starflare was a supper club on our block long before it was a casino. We're keeping the Levee's light, not its dice."
  - Leverage routes A4.4 / A4.11.
  - Northern Ledger invite (X1).
  - Tell (review proposal): handshake held a beat too long; photo face-down.
- **Sources:** `people/bennett.md`, `plan/t07.md`, `plan/t08.md`, `pages/room9.md`.

### Richard
- **Tier/group:** Supporting (story); the Machine side. Where: room 9, Slack Water Motel; the motel picnic table.
- **Role:** Bennett's father; wears the key to Gus's safety deposit box. "He has the final say" on the box.
- **Look:** Late 70s, worn down, thin uncombed white hair, grey stubble, tired eyes, moves slowly. Faded flannel, worn work trousers, thrift-store cardigan missing a button, scuffed shoes. (Optional: Bennett's jaw.)
- **Personality (voice):** Slow, sparing, a little dry; pride under the tiredness. Talks about card games, the old Levee, his son as a boy. Lies with "It's nothing" and a hand over the key. Sample: "War's an honest game. No choices, no bluffing. Just the cards."
- **Want/goal:** Not stated. "He keeps his word and is too proud to take Gus's help."
- **Backstory/wound:** A few years ago he ran up markers at the Flare; the house he'd lived in for decades went to cover them, and he still owed more. Bennett forced Gus to wipe out the rest.
- **Secrets:** The key. He has never opened the box. He believes Gus pays his room; actually Bennett does.
- **Arc/outcomes:**
  - M4: once he trusts you, Umbra men come to room 9 with his old Flare markers and demand payment or the key. He opens the box with you.
  - His four choices: shame Bennett; reconcile; burn it (road closes); hand it to Ed (Gus's deal comes out too).
  - Dirty version: lean on Richard for the key (he and Dot cool on you).
  - A5.7 appears via Bennett's reconciled stance.
- **Relationships:**
  - Related line: Bennett, Gus, Dot, Ed, Umbra.
  - Ties:
    - Bennett (family; secret: holds the box key)
    - Dot (ally)
    - Gus (money)
- **First appearance:** A3.7 (War at the picnic table).
- **Codes:** A3.7, A4.4, M4, R10, A5.7.
- **Player interaction:** Join his game of War (story-event game, one pass, 10 cards each) at the motel picnic table. Each visit nudges his trust.
- **Sources:** `people/richard.md`, `pages/room9.md`, `plan/t08.md`.

### Mayor Dabareski
- **Aliases:** Mayor Dabareski; "a wink at 'Da Bears'" (`people/mayor.md`).
- **Tier/group:** Supporting; the Machine. Where: Northern Ledger, Gilded Pine.
- **Role:** "The casino mayor, in office forever"; head of the Machine; Bennett's boss and target. Seen rarely.
- **Look:** Woman, late 60s, big laugh, red blazer, a photographer always nearby.
- **Personality (voice):** Big, booming, folksy; "slightly arrogant: she thinks the world revolves around her." Calls everyone "friend." Lies with a story about the old days. Sample: "Clean up Northlight? Honey, I built Northlight."
- **Backstory:** She brought in the riverboats and land casinos after the mills closed; openly friendly with the casino business and the Aurelian.
- **Arc/outcomes:** Race against Bennett in the background; "the epilogue's newspaper front page shows who won" (outcome not stated, OQ 32).
- **Relationships:**
  - Related line: Bennett, Tae.
  - Ties: Bennett (rival).
  - The city's "Safe Streets" plate-reader contract: "the mayor signed it" (`people/wren.md`).
- **First appearance:** R6 / X1 (Northern Ledger).
- **Codes:** R6, X1, X2, EP.
- **Player interaction:** Background (bar TVs, posters); Northern Ledger and Gilded Pine sightings.
- **Sources:** `people/mayor.md`, `plan/t06.md`.

### Vivian
- **Tier/group:** Core; Silvershore old money. Where: Obsidian, Harborlight high tables, yacht games, Gilded Pine.
- **Role:** Old-money high roller on a losing streak; your door into Silvershore; owner (with her family) of the lakefront land. One of only two backers (silent partner).
- **Look:** Early 60s, sleek silver bob, pearls, camel coat, dark sunglasses indoors. Later reveal: "her pearls are copies" (`people/vivian.md`).
- **Heritage:** Black old money; her great-grandfather founded an insurance company that covered the mill and dock workers nobody else would insure. (Now her grandfather, 2026-10-28; see 4.1.8.)
- **Personality (voice):** Dry wit, old-money understatement; calls her losses "a rough season"; never says she's selling; lies with grace. Sample: "Darling, I've been losing since before you were born. I'm very good at it."
- **Want/goal:** Not stated. "Her losses push her to sell."
- **Secrets:** She sold a family piece at a jeweler on the Mile to cover losses (Drive Vivian job). Her pearls are copies. A "secret Aurelian land meeting" is saved as a possible act 5 escalation (proposal, `pages/notebook.md`).
- **Arc/outcomes:**
  - A2.4: glimpsed at the Harborlight high tables.
  - A3.6: you meet her at the Gilded Pine ($25,000, summer). At $50,000, with a suit and her sponsorship, the Obsidian opens, with her yacht invitation.
  - A5.1: the land (~$1M); how you've treated her decides price and seller. S9 and the jeweler job both test silence.
  - A5.2: backer.
- **Relationships:**
  - Related line: Tae, Rosa, Ed, the society columnist (S9).
  - Ties:
    - Rosa (ally: her lawyer pulls the commission file)
    - Tae (rival: bids for her land)
    - You (rival: "her land: you or Tae")
- **First appearance:** Glimpsed A2.4; met A3.6.
- **Codes:** A3.6, A5.1, A5.2, M1, R4, S9, X2, O1, O2, O10, A5.7, S13, A2.4.
- **Player interaction:**
  - Sponsor into the Obsidian and yacht games.
  - "Drive Vivian" job ($150).
  - Her lawyer clears a raid for a fee (R4) and pulls Rosa's commission file (M1).
  - S9 columnist.
  - A5.1 land.
  - A5.7 her lawyer speaks.
  - Tell (review proposal): sunglasses pushed up; pearls in her fist.
- **Sources:** `people/vivian.md`, `plan/t13.md`, `plan/t09.md`.

### Tae
- **Tier/group:** Core; new money. Where: the Aurelian, Gilded Pine.
- **Role:** The Aurelian's general manager; "the Flare's rival in vision, not business." Not a backer.
- **Look:** Korean-American man, early 40s, sharp charcoal suit, no tie, sleeves never wrinkled, North Light lighthouse cufflinks.
- **Personality (voice):** Confident, sincere, salesman's warmth; "never insults the Flare"; "doesn't lie at all, which is what makes him dangerous." Sample: "Northlight deserves a world-class house. Gus deserves to rest."
- **Want/goal:** Buy Gus out fairly "so Gus can rest," own the Starflare name (and sign) for a restaurant in the Aurelian (O5), and win Vivian's land. He believes "the city deserves a world-class casino."
- **Backstory:** Grew up in the Levee; made it big with the out-of-town chain; asked to be sent home because he loves Northlight. His dad played drums in the Flare's rocket-age house band, "so he grew up hearing the Flare through the wall and has a soft spot for it he'd never admit" (OQ 60). Old family friend of Gus's family; big brother to Stella.
- **Secrets:** The soft spot for the Flare.
- **Arc/outcomes:**
  - A2.2: pre-bailout satellite offer (built on Stella's numbers) refused.
  - A2.5: false lead in M2.
  - A3.4/A3.5: hires Pete.
  - S13: the Revaris camera pitch (installs either way).
  - A4.6: offers to buy the name from the District, with Gus greeting guests (O5).
  - A5.1: bids on the land; the sellout O10 helps him get it.
  - If the house band reunites (Miles/Audrey route) he sells the name at a fair price.
- **Relationships:**
  - Related line: Gus, Pete, Stella, Vivian, his dad (the drummer), Felix/Nora (his desk staff).
  - Ties:
    - Stella (ally)
    - Pete (ally)
    - Felix (ally)
    - Gus (rival)
    - Vivian (rival)
- **First appearance:** A2.2 (offer; he is "offstage" in A3.4 per EVX).
- **Codes:** A2.2, A2.5, A3.4, A3.5, A4.6, A5.1, M2, S4, X2, O5, O10, S13.
- **Player interaction:**
  - Seed S4: ask Gus who the drummer was.
  - Warn him (via Felix/Nora or Stella) about the cameras (S13).
  - Side with him (A4.6).
  - Compete for land (A5.1).
  - Buy the name back at a premium (O5).
- **Sources:** `people/tae.md`, `plan/t06.md`, `plan/t07.md`.

## 1.4 Minor roles

### Ed ("Big Ed")
- **Aliases:** Ed; "Big Ed" on the street.
- **Tier/group:** Supporting; police. Where: wherever your heat is high; Lock Street Station.
- **Role:** Honest vice cop; the face of the heat system. Wants Wren for illegal surveillance and blackmail.
- **Look:** "A typical Chicago-style cop": white, late 40s, slightly overweight, big bushy horseshoe mustache, rumpled sport coat, loosened tie, badge on belt, coffee or a Stacker in hand.
- **Personality (voice):** Gruff, tired, cop shorthand; names and favours; never threatens directly; lies by playing dumber than he is. Sample: "I don't care about your dice. I care who you owe."
- **Want/goal:** Wren; "tired of watching the big fish get away."
- **Secrets:** He once gave Bennett the Umbra link in good faith and "never learns what Bennett did with it unless you tell him." His squad uses the Revaris plate readers, "so Wren sees whatever Ed's people look up; he knows it and hates it." He keeps a file on Ray.
- **Arc/outcomes:** The informant path (tip him and your heat drops at once, but Umbra may notice). Recipient of the drive, the skim, or the box. Can speak for you at A5.7 if you were his informant.
- **Relationships:**
  - Related line: Wren, Ray (keeps a file on him), Juan (leaves him alone), Kiran, Raffy, Vivian.
  - Ties:
    - Wren, Bennett, Ray (secret)
- **First appearance:** A1.5 (offered the drive).
- **Codes:** A1.5, A4.4, A4.7, M3, M4, R4, S10, A4.3, A5.7.
- **Player interaction:** Heat sweeps and raids; informant path; hand him proof (A4.3), the skim (A4.7), or the box (A4.4); S10 (Ed trusts you more if you never scanned in).
- **Sources:** `people/ed.md`, `plan/t06.md`, `plan/t08.md`.

### Kiran
- **Tier/group:** Supporting; "Umbra, maybe" (`people/kiran.md`, OQ 14). Where: the Prairie Queen.
- **Role:** Friendly Prairie Queen blackjack dealer squeezed into skimming. "Squeezed, not evil."
- **Look:** Indian-American, early 30s; riverboat uniform (striped vest, sleeve garters, crooked bow tie), neat black hair, wedding ring he turns when nervous (text only), daughter's photo by his chip tray.
- **Personality (voice):** "The friendliest dealer on the boat"; goes quiet on a fixed hand (his tell); lies by keeping up the patter. Sample: "Welcome aboard! Sit, sit. My daughter says this seat is lucky. She's seven, so she'd know."
- **Want/goal:** Not stated. "Kiran gets out" (M3 solve).
- **Backstory/wound:** Grew up in the Levee. A young dad who borrowed from Ray when his daughter needed surgery insurance wouldn't cover; the interest buried him.
- **Secrets:** He skims the cheap tables: palms chips off the rack on color-ups and short-counts the drop box. He steals from the house, never from players, so "players' odds at his table are always fair" (audit 2.3).
- **Arc/outcomes:** M3: tell Wren, tell Ed, or keep it as leverage on Ray. Call him out (job lost; Umbra notices you), walk, or help him get clear (pay the debt or pass what you know to Ed).
- **Relationships:**
  - Related line: Ray, Wren, Ed, Rosa.
  - Ties:
    - Ray (money: "owes him; skims to pay")
    - Rosa (ally: "she may flag his table")
- **First appearance:** The Prairie Queen (open from the start); first coded in M3. Offstage family: his daughter.
- **Codes:** A4.7, M3.
- **Player interaction:** Clue puzzle at a color-up (spot-the-difference); Rosa's line "watch the hand that isn't busy"; the envelope at the landing. Cheating-rule scenario. Collect-for-Ray can put him in front of you.
- **Sources:** `people/kiran.md`, `plan/t08.md`, `pages/gdd.md`.

## 1.5 Romance characters

Each has a male and a female version. The version you meet follows the wedding dance at A1.4; turning down both means romance appears only if you lean in later. All six are in their 30s. (`pages/romance.md`, `plan/t06.md`)

Shared rules (`pages/romance.md`):
- No dating system, meters, gifts or date button.
- Never touches the odds or fairness.
- Never with the main cast.
- You can pursue more than one; a partner who finds out counts it strongly against you.
- The test (beat 4) counts double.
- An ended romance can always be rebuilt.
- Each opens one route or clue, none required.
- Beats and voices are proposals unless marked settled.

### Miles / Audrey (the Flare's piano player)
- **Tier/group:** Romance. Where: the Flare's half-empty lounge. Value: loyalty.
- **Look:**
  - Miles: Black, mid-30s, close-cropped hair and trim beard, vintage tux jacket with a loosened bow tie, his grandfather's old wristwatch, easy smile, hums while he plays.
  - Audrey (Mike): early 30s, purple hair in pinned-up curls, round glasses, emerald vintage-style dress, dry wit.
- **Role:** Plays the lounge nightly, knows every regular's song, overhears everything. Can play the new Starflare's showroom.
- **Story role:** Sees Bennett's people carry the tape out at the fall (M1). Together: helps you get it back (private events at Bennett's fundraisers). Friend: only "someone took a tape box." Ended: nothing.
- **Exclusive route:** The house-band reunion, after which Tae sells the Starflare name at a fair price. Deal-breaker: selling Gus out.
- **Want:** Personal goal to help with (proposal): "record an album of the Flare's old songs" (`pages/romance.md`).
- **Backstory:** "Gus gave them their first real gig"; the piano was Gus's late wife's pick (beat 2, proposal).
- **Beats (proposal):**
  - Meet (act 1, Flare lounge).
  - First date (act 1-2).
  - Opening up (act 2: they overheard Gus on the phone with "a friend of Ray's").
  - The test (act 3: they need rent; the Aurelian offers a steady gig).
  - Landing (act 5 opening night: play the showroom).
- **Relationships:** Gus, the Flare regulars (Related); Gus (ally), Bennett (secret: saw his people take the tape).
- **Codes:** A5.3, R7, O1 (`data/story.js`); also in EV: M1.
- **Interaction:** Song request (table event, proposal); observed choices include Pete's last night, how you stand by Gus, the torn patch, selling Gus out.
- **Sources:** `people/miles.md`, `pages/romance.md`, `plan/t06.md`.

### Felix / Nora (the Aurelian desk clerk)
- **Tier/group:** Romance. Where: the Aurelian front desk. Value: honesty.
- **Look:**
  - Felix: Cuban-American, early 30s, slim, neat dark hair, charcoal vest, gold name badge, a pen behind his ear; "warm and a little too honest for a front desk."
  - Nora: early 30s, freckles, auburn bob, Aurelian blazer with one bright scarf that bends the dress code; "quick and funny."
- **Role:** Inside view of the Aurelian: who checks in, who gets comped suites, who Stella meets.
- **Story role:** Shows you the filing lawyer meeting Bennett (M2) and clears Stella early; helps with the comped-suite step.
- **Exclusive route:** The Aurelian's own copy of its camera footage (Wren can't erase it) shows Wren's people meeting Bennett, giving leverage over Wren (A4.3).
- **Want:** Personal goal (proposal): "saving for night school to get out of hotel work."
- **First meeting:** "act 1 or 2" (`people/felix.md`); the Aurelian opens at $10,000 (act 2) (OQ 18).
- **Landing:** Together: they leave to run the new Starflare's guest services desk, then the hotel front desk under Jalen once the post-game tower opens (audit 2.9). If you used their inside view against the Aurelian, Tae takes it personally (settled).
- **Relationships:** Tae (their boss), Stella (Related); Tae (ally: works his front desk), Stella (ally).
- **Codes:** A2.5, R8, A4.3, S13 (`data/story.js`); also in EV: M2.
- **Sources:** `people/felix.md`, `pages/romance.md`.

### Graham / Claire (the Northern Ledger patron)
- **Tier/group:** Romance. Where: club car of the Northern Ledger, every spring. Value: integrity with power.
- **Look:**
  - Graham: late 30s, wire-rim glasses, tweed blazer, loosened tie, beat-up briefcase of bills he's marking up; "earnest and a bit rumpled."
  - Claire (Mike): Japanese-American, late 30s, long dark hair; trench coat, laptop, thermos of tea; "calm, with a sly sense of humor about politics." Art reference only: Tifa from Final Fantasy VII Remake, "the portrait must be original."
- **Role:** Statehouse staffer who rides the train each spring for the legislative session. They live in Northlight (settled 2026-10-23).
- **Story role / exclusive route:** The conflict-of-interest license rule against Bennett's hidden cut (A4.11): the by-the-book way to win back the name without exposing Gus's Umbra deal. Together: they get the complaint in front of the right people. Friend: you file and push it yourself. Ended: you never hear about it.
- **Want:** Personal goal (proposal): "being asked to run for office."
- **Beats (audit 1.10, Mike 2026-10-25):**
  - Meet on the spring train (the only beat needing the train).
  - First date.
  - Dinner downtown (act 3-4, any season).
  - Test: a Starflare Field fundraiser in Northlight (act 3-4).
  - Landing: opening night; the spring ride together moves to the epilogue.
- **Relationships:** Bennett, Mayor Dabareski; Bennett (secret: conflict-of-interest rule).
- **Codes:** R9, X1, A4.11, A5.7.
- **Sources:** `people/graham.md`, `pages/romance.md`.

## 1.6 Named recurring walk-ons and offstage figures

From `pages/walkons.md` ("No portraits planned unless noted. Unnamed characters stay unnamed until a scene needs it."), `pages/cast-planning.md`, and `data/story.js`.

| Who | What the notes say | Related | Codes | Interaction |
|---|---|---|---|---|
| Mr. Hamilton, the founder (Gus's father) | Built the Starflare in the rocket age. Gave Pete his first job and 25-year pin. "Offstage; dead before the game starts; no plot role." Family surname Hamilton only if a story needs it. (`pages/walkons.md`, `people/gus.md`) | Gus, Pete, Richard, Bennett | M4, R10 | None. |
| Gus's late wife | Dot's sister. Touches all over the Flare; framed photo of the two sisters in Gus's office. Offstage. Named Alice (Mike, 2026-10-27). | Gus, Dot, Stella | R1 | Find the photo (R1). |
| Tae's dad | Played drums in the Flare's rocket-age house band; face in the faded band photo by the lounge. Offstage. | Tae, Gus | S4, O5 | Ask Gus who the drummer was (S4). |
| The bride's brother and the maid of honor | From the mill family wedding at Kasia's Tap. Each asks via Kasia to dance. Opposite sex first; the other only after a no. | Kasia, romance characters | A1.4 | Dance or decline. |
| Richard's weekly driver | Drops Bennett's envelope at Dot's desk every week. | Bennett, Richard, Dot | M4 | Clue. |
| The filing lawyer | Lawyer on the Flare's sale paperwork. Mitch recognizes him from Bennett's old deals. Secretly Bennett's and Wren's. Meets Ray on the Long Bridge (S11). Trust account carries Bennett's cut. | Bennett, Mitch | A2.2, M2 | Clue. |
| Vivian's lawyer | Pulls Rosa's gaming commission file; clears a raid for a fee. | Vivian, Rosa, Ed | M1, R4, A5.7 | Fee. |
| Umbra's old guard and "the uncles" | Old bosses passed over; resent Wren; she has dirt on them. Wren's grandfather: unnamed; "still Umbra's boss then" (`pages/room9.md`). | Wren | A4.5 | Not stated. |
| The Umbra crew | Moves into the Flare after the bailout; muscles in on Juan's dice. | Wren, Ray, Juan | A2.3, R3 | Push them out (R3). |
| Umbra men at room 9 | Follow you to room 9 and demand pay or the key with Richard's old markers. | Richard | A4.4 | Triggers the box. |
| The Tuesday league | Millstone Lanes bowlers, $40 short for shirts. | Juan | S1, A3.2 | Chip in. |
| The mill pensioner | Behind on his tab at Kasia's; wins a RailYard slot tournament. | Kasia | S2 | Cover $30. |
| The trucker | $20 short on diesel; saves the spaceman on the last night. | Mitch, Gus | S3, A3.3 | Spot him. |
| The old pilot | Grumbles about the Queen's rusting sister boat. | Kiran's boat | S5, A5.5 | Listen. |
| The petition volunteer | Asks you to sign the "New Levee" petition. | Bennett | S7 | Sign or not. |
| The tourist (a travel writer) | The tourist you taught; her piece names you on opening night. | The player | S8, A5.6 | "Teach a tourist." |
| The society columnist | Asks how much Vivian lost. | Vivian | S9 | Tell or not. |
| The Long Bridge hoodlums | "Unaffiliated, rob people before dawn now and then. No ties to any organization." Smash Revaris plate readers. | Dot | S11 | Rare robbery. |
| The lucky charm seller | Sells a "lucky spaceman" bobblehead. | Gus | S12 | Buy or laugh. |
| Kiran's daughter, Raffy's mom | Offstage family. Kiran's daughter is seven (voice). Raffy's mom is a nurse on double shifts. | Kiran, Raffy | M3, A1.5 | None. |
| The Millstone Lanes owner | Looks the other way on the dice because the money buys parts. | Juan | R3 | None. |
| The mayor's photographer | "Always nearby." | Mayor | R6 | None. |
| The suitcase guest | Dropped 2026-10-22; folded into room 9. | Dot, the founder | R10 | Dropped. |

Other unnamed figures mentioned elsewhere:
- The drunk regular who drops a fat wallet (A1.3, `data/story.js`).
- The Queen's floor manager (grumbles the cheap tables count short, `people/kiran.md`).
- Ray's men and collector (`people/kiran.md`, `pages/review.md`).
- "Walt," a placeholder regular in the Kiran sample scene: "(name not checked for clashes)" (`pages/review.md`). Not canon.

## 1.7 Relationship map (all ties in `data/story.js` `E`)

Types: family, ally, money, secret, rival.

**Family**
- Gus-Stella: father and daughter.
- Gus-Dot: "brother-in-law; his late wife was her sister."
- Dot-Stella: aunt.
- Bennett-Richard: "son; pays his room in secret."

**Ally**
- Gus-Pete: best friends; pit boss.
- Gus-Jalen: busboy at the Flare.
- Pete-Stella: "Uncle" Pete.
- Pete-Dot: go-between with Gus.
- Pete-Jalen: "started as a busboy too; keeps an eye on him."
- Tae-Stella: "big brother figure; buyout talks."
- Tae-Pete: "hires him away at the fall."
- Rosa-Kasia: deals her back room.
- Juan-Raffy: hides him at Millstone.
- Dot-Richard: room 9 guest.
- Miles-Gus: "Gus gave them their first gig."
- Felix-Tae: works his front desk.
- Felix-Stella: "sees who she meets."
- Kiran-Rosa: "she may flag his table."
- Vivian-Rosa: "her lawyer pulls the commission file."
- You-Rosa: "she teaches you the games."
- You-Gus: "you come to care about him."

**Money**
- You-Jalen: you stake him.
- You-Kasia: credit on her ledger.
- Gus-Ray: "collects Gus's debt; brings Umbra's offer."
- Kiran-Ray: owes him; skims to pay.
- Raffy-Ray: courier; "stole the wrong envelope from his car."
- Chase-Ray: borrows and can't pay.
- Richard-Gus: "his markers took Richard's house."

**Secret**
- You-Raffy: holding his envelope.
- Bennett-Gus: "pushed the stadium deal that took the Flare."
- Wren-Bennett: "owns him: his hidden cut, on camera" (OQ 15).
- Wren-Gus: "footage forced the camera deal; Umbra bought the Flare."
- Wren-Ray: "his boss; knows about the skim."
- Rosa-Gus: took the fall for him.
- Richard-Bennett: holds the box key.
- Ed-Wren: wants her for blackmail.
- Ed-Bennett: gave him the Umbra link.
- Ed-Ray: keeps a file on him.
- Mitch-Bennett: Bennett's bad tip ruined him.
- Bennett-Rosa: keeps the tape of her table.
- Wren-Jalen: threat if you're spotless.
- Graham-Bennett: conflict-of-interest rule.
- Miles-Bennett: saw his people take the tape.

**Rival**
- Tae-Gus: "rival visions; old family friend."
- Tae-Vivian: bids for her land.
- Bennett-Mayor: the mayor's race.
- You-Chase: the other newcomer.
- You-Vivian: "her land: you or Tae."

Ties present in a page's Related line but not in `E`: Rosa-Pete (`people/rosa.md`, `people/pete.md`), Kasia-Raffy, Ed-Raffy, Ed-Juan, Wren-Raffy, Wren-Kiran, Dot-Raffy, Dot-Ray. Treat these as noted, not as map edges.

---

# 2. STORY OUTLINE

## 2.1 Premise

- **The game.** "Starflare Casino (working title, settled 24 Oct 2026) is a story-driven casino life sim for iOS and Android. A newcomer drives into Northlight, a Great Lakes casino city, with about $200 and a car, falls for a fading rocket-age casino and its owner, and decides what becomes of it." Inspirations: Vegas Stakes (SNES, 1993), Stardew Valley, BitLife. (`pages/gdd.md`)
- **Spine.** "One main storyline in five acts, with places to settle along the way. The Starflare's fate is the background story; most play time goes to the tables, the city, NPC arcs and the mysteries." (`pages/gdd.md`)
- **Core premise** (`plan/t07.md`; reworked 2026-10-23):
  - Gus is secretly a gambler in the Cinder Room.
  - Wren's cameras see it. She used that footage to force her camera deal.
  - Revaris Technologies buys the Flare, clears Gus's debt, and promises to keep him on. Gus signs knowing it's the mob and, ashamed, tells everyone "a friend of Ray's" bought it.
  - Umbra puts the Flare's land into the Levee Stadium District, which makes Umbra its biggest investor.
  - Alderman Bennett is the political engine, not the buyer. He gets Umbra campaign money and a hidden cut of the stadium deal.
  - "Once sold, it's sold: there is no buy-back."
  - The name sits in Bennett's ordinance. It comes back only through leverage.

## 2.2 Pacing, gates and hour budgets

**Overall.** About 42 hours to opening night, about six seasons (roughly a year and a half in game).
- A season is 28 days, about 7 hours.
- A day is about 15 minutes of normal play (starting values).
- (`pages/storymap-intro.md`, `pages/gdd.md`)

**Gating rule.** "Content checks combinations, for example `best_bankroll >= 25000 AND days_since("bailout") >= 28`, never one threshold alone." (`pages/gdd.md`)

**Act gates and hours** (`data/story.js` `ACTS`, `pages/gdd.md`, `pages/storymap-intro.md`):

| Act | Name | Hours | Gate in | Gate out |
|---|---|---|---|---|
| 1 | The Flare | ~8h | Start: $200 and a modest car | $5,000 + Gus's trust + two weeks |
| 2 | The bailout | ~8h | $5,000 + Gus's trust + two weeks | $25,000 + a season after the bailout |
| 3 | The fall | ~8h, always happens | $25,000 + a season after the bailout | Who bought the Flare solved (M2) + $50,000 |
| 4 | Winning back the name | ~10h | Mystery solved + $50,000 | Name won back + $250,000 |
| 5 | The new Starflare | ~8h | Name won back + $250,000 | Opening night ends the main story |

- Act 2 "can't be shorter than the season its gate waits for (audit 1.1, Mike 2026-10-24)."
- "No story clock: the only deadline on the main line is Starflare Field's Opening Day, the first one at least three seasons after the fall. It leads to a resting point (outcome 7), not a fail."
- Field timer (Mike 2026-10-20; `pages/storymap-open.md`):
  - Starflare Field is a domed baseball stadium.
  - Outdoor work runs spring to fall; winter is indoor fit-out under the dome.
  - It opens on Opening Day in early spring.
  - "That's 3 seasons if the fall comes in summer, 4 in spring, 5 in winter and 6 in fall."

**Money thresholds** (`pages/storymap-intro.md`, `pages/gdd.md`):

| Best bankroll | Opens |
|---|---|
| $200 (start) | Kasia's Tap, Millstone Lanes, Bridgehead, the Flare, the Prairie Queen; Act 1 |
| $1,500 | The RailYard; first rung up (A1.8) |
| $5,000 | The Harborlight; act 2 can open |
| $10,000 | The Aurelian; the Northern Ledger (spring); the Stella and Tae false lead |
| $25,000 | Aurelian high-limit; the Gilded Pine (summer); act 3; meet Vivian |
| $50,000 | The Obsidian (suit and Vivian's sponsorship; brings the yacht invitation); act 4 once M2 solved |
| $250,000 | "Blackspruce Lodge and yacht games fill the climb" (OQ 12); act 5 once the name is won back |
| ~$1M | Vivian's land |
| ~$250k | Relight the rocket sign |
| ~$1M | The gaming floor; opening night ends the main story |
| ~$2M / ~$1.5M | Post-game: tower / rooftop launch pad |

## 2.3 Act 1: The Flare (~8h)

Table source: `pages/storymap-beats.md` (one row per code, audit 1.3). Extra one-liners: `data/story.js` `EV` / `EVD`.

- **A1.1 Arrival** (Slack Water Motel; Night 1).
  - Sign Dot's register with your name (used from then on) and tick Mr. or Ms.; answer her question.
  - Leads to: first hidden-scale nudge.
  - Extra (`EVD`): you arrive with about $200; from the lot you see the rocket sign across the river, "the game's first image." The Mr./Ms. choice "sets the rival (Chase or Brooke) and which wedding dance is offered first."
- **A1.2 Night one** (Kasia's Tap; Night 1, after checking in).
  - Pull tabs and video poker; hold Raffy's envelope or not.
  - Leads to: Kasia opens your credit; first seed (Raffy's envelope).
  - Extra (`EVD`): Raffy slides you a sealed envelope ("hold this till Friday"), tips you $50, never comes back. Inside: about $2,000 and a thumb drive. Kasia does not send you anywhere else.
- **A1.3 Kasia's jobs and the back room** (Kasia's Tap, Millstone Lanes, Kasia's back room; after night one; back room opens once both jobs are done).
  - A paid bar-back shift (a drunk regular drops a fat wallet: return it or not).
  - Take Juan a box of free-drink coupons for his Tuesday league (Mike, 2026-10-23).
  - In the back room Rosa warns you off a hand: fold or play it.
  - Leads to: meet Juan, who tells you about the Flare across the river; meet Rosa.
  - Extra: Rosa's old vest has a casino patch torn off, "the start of M1."
- **A1.4 The wedding** (Kasia's Tap; after Kasia's jobs; "a separate third job").
  - Server or busser; dance with whoever asks, or say you're on the clock.
  - Leads to: sets which romance versions you meet.
  - Extra: $80 cash plus tips; the first offer is from the opposite sex to the box ticked (maid of honor for Mr., bride's brother for Ms.); the other only after a no; "Nobody comments on any choice."
- **A1.5 Ray asks around** (Kasia's Tap, Slack Water Motel, Millstone Lanes; weeks after night one, if you still hold Raffy's envelope).
  - Return it to Ray, keep the cash, hand the drive to Ed, or help Raffy pay Ray back. (Superseded 2026-10-29, see 4.1.8: A1.5 settles the cash and Raffy; the drive stays with you and its fate, Ed included, comes from act 2 on.)
  - Leads to: the first thread into the Umbra story.
- **A1.6 Over the bridge** (Bridgehead, the Flare; after Juan tells you about the Flare and you've worked the wedding).
  - Coffee with Gus, rub the spaceman's boot.
  - Leads to: meet Gus.
  - Extra: the spaceman is "the life-size chrome astronaut waving by the front doors"; "Nothing is wrong yet, but the floor is half empty."
- **A1.7 Becoming a regular** (the Flare; a few Flare visits).
  - Keep Gus's secret or not.
  - Leads to: meet Pete and Jalen; Dot and Gus each ask about the other.
- **A1.8 First rung up** (the RailYard; best bankroll $1,500).
  - Leads to: the rival is already a step ahead.
- **A1.9 Cracks** (Cinder Room, the Flare; any order, after Ray slips you the Cinder Room password once you've won at the Flare).
  - Tell Pete what you saw, or keep quiet.
  - Leads to seeds: Gus gambles; Stella's worry.
- **A1.10 Ray comes to collect** (Gus's office; $5,000 + Gus's trust + two weeks).
  - Leads to: Act 2. "This closes act 1."

## 2.4 Act 2: The bailout (~8h)

- **A2.1 Ray collects** (Gus's office; start of act).
  - Cover part of the debt (generous), stay out, or tell Pete.
  - Leads to: Gus's trust. "Whatever you do, it isn't enough to clear it."
- **A2.2 A friend of Ray's** (the Flare; after Ray collects).
  - Gus has turned down Tae's satellite offer.
  - Gus won't say who bought it, though the public record names Revaris Technologies, "a dull-looking camera company (secretly Umbra's front, which later puts the land into the Levee Stadium District)."
  - The lawyer's name on the paperwork is a clue.
  - Extra (`EVD`): Ray "swears about Revaris, 'I just know a guy, never met him' (S11)." Gus tells everyone the debt is cleared and nothing will change. Truth: Ray brought Umbra's offer, Revaris bought the Flare, Gus signed knowing it was the mob "because Wren already had his Cinder Room footage."
- **A2.3 Things change anyway** (the Flare; after the bailout).
  - Umbra crew moves in; Pete uneasy; Rosa won't come in.
- **A2.4 The Mile** (the Harborlight; best bankroll $5,000).
  - First comped suite; the rival is a regular there; you glimpse Vivian at the high tables (no meeting yet).
- **A2.5 Higher up** (the Aurelian; best bankroll $10,000).
  - Follow Stella or not. Stella and Tae false lead; Northern Ledger in spring.
- **A2.6 Bennett** (Harborlight fundraiser; after the bailout).
  - Bennett's mayoral run on "clean up Northlight."
- **A2.7 The rival borrows** (anywhere; after the bailout).
  - The rival's slide begins.
- **A2.8 Starflare Field announced** (bar TV; $25,000 + a season after the bailout).
  - Leads to: Act 3. The Levee Stadium District will build "a domed baseball stadium, on the Flare's block, with Bennett at the podium."

## 2.5 Act 3: The fall (~8h, always happens)

- **A3.1 Starflare Field** (the Flare; start of act).
  - The drawing shows the rocket sign over the stadium gate.
  - Extra: Bennett: "The Starflare was a supper club on our block long before it was a casino. We're keeping the Levee's light, not its dice." Gus learns the Flare will be bulldozed.
- **A3.2 The fight** (council hearing, union rally; after the announcement).
  - Fight clean (allies), fight dirty (heat), or stay out.
  - Leads to: allies; the dirty route can lead to the fire (O9).
- **A3.3 Condemnation** (the Flare; after the fight).
  - Leads to: the last night on the floor.
  - Extra: "a wake"; Gus in his dad's dinner jacket; the trucker (S3) brings his flatbed.
- **A3.4 Pete alone at the dark pit** (the Flare's last night).
  - Tell Gus Pete is leaving, push Pete to stay, or say nothing.
  - Leads to: Pete's gold rocket pin; returning it later hires him back.
- **A3.5 The scatter** (Aurelian, RailYard, Kasia's Tap, Cinder Room; after the last night).
  - Pete to Tae, regulars to the RailYard, Rosa stays dealing Kasia's back room; Bennett finds the tape; Field timer starts.
- **A3.6 Higher still** (Gilded Pine, Obsidian; $25,000 (summer); $50,000).
  - Meet Vivian; Obsidian opens.
- **A3.7 Clues come together** (Mitch's, room 9; any order).
  - Play War with Richard at the motel picnic table.
  - Leads to: the lawyer recognized; Richard's trust.
- **A3.8 The rival hits bottom** (anywhere; after the scatter).
  - Lend them cash or refuse. Refuse and they may steal from you later.
- **A3.9 Who bought the Flare, solved** (M2 solved + $50,000).
  - Leads to: Act 4.
  - Extra: Wren pays the cut through Revaris, "so her own books are the proof and she owns him."

## 2.6 Act 4: Winning back the name (~10h)

- **A4.1 The truth** (start of act).
  - Confront Gus: he admits he sold his dad's place to Umbra (2026-10-23). Forgive him, or hold it over him.
- **A4.2 Gus at the bottom** (Cinder Room; start of act).
  - Help him quit, with Dot and Stella.
  - Leads to: Gus's trust; Gus lets go (O6).
- **A4.3 Expose Umbra** (Cinder Room, the Aurelian; you kept Raffy's drive, or Felix/Nora show you the Aurelian's footage).
  - Trade your silence to Wren for the name, or hand the proof to Ed.
  - Leads to: name won back (Wren remembers); Ed gets his case if you hand it over.
  - Dirty: blackmail with the payers list, or hand Ed the whole drive. (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
- **A4.4 Leverage** (old Levee bank, room 9; Richard trusts you).
  - Umbra men bring Richard's old markers and demand pay or the key.
  - Richard opens Gus's box and decides: shame, reconcile, burn, or hand it to Ed.
  - Leads to: name won back from Bennett (burn closes this road).
  - Dirty: lean on Richard for the key.
- **A4.11 By the book** (the commission; Graham/Claire together or friend).
  - File the conflict-of-interest complaint citing the trust account Mitch names; the commission subpoenas the records.
  - Leads to: name won back without exposing Gus.
  - Dirty: have the trust-account records lifted instead of subpoenaed.
- **A4.5 Wren's deal** (Cinder Room; any time in act).
  - Give her Gus's markers or Raffy's thumb drive.
  - Leads to: she pulls the name out of the District for her own Starflare: mob takeover (O8) or sellout (O10).
- **A4.6 Tae's offer** (the Aurelian; any time in act).
  - Side with Tae and Stella.
  - Leads to: Tae's restaurant (O5).
- **A4.7 Loose ends** (Prairie Queen, commission; clues found).
  - Tell Wren, tell Ed, or keep the skim as leverage.
  - Leads to: Rosa's license; leverage on Ray.
  - Extra (`EVD`): the Long Bridge plate (S11) puts Ray at a meeting he denied; "With the tape and her file you can get Rosa's license back."
- **A4.8 The rival** (anywhere; if you refused them).
  - Theft, or all is well.
- **A4.9 The clock runs out** (Starflare Field; the Field timer ends before the name is won back).
  - Leads to: the name stays with Starflare Field (O7), resting.
- **A4.10 Name won back** (name won back + $250,000).
  - Short of $250k: take the regulars' shoestring offer now, or keep saving.
  - Leads to: Act 5, or the people's Starflare (O4) as a stepping stone.

**Clean or dirty** (Mike 2026-10-26; `pages/storymap-branches.md`, `pages/gdd.md`).
- "Act 4 has no cash route; leverage wins the name back."
- White-hat means "proof of real wrongdoing that never hurts an innocent person."
- Every route also has a black-hat version "that works faster or cheaper."
- The game remembers:
  - heat rises;
  - your crooked or ruthless lean grows;
  - "Gus, Dot, Rosa, Ed and Richard cool on you";
  - "Wren takes notice";
  - the license hearing "weighs it and sets stricter conditions";
  - it "leans toward the Starflare without Gus (O2) unless you repair things."

## 2.7 Act 5: The new Starflare (~8h)

- **A5.1 Vivian's land** (Gilded Pine, Obsidian; start of act).
  - Win Vivian or help Tae (sellout). Land ~$1M, cheaper the closer you are to her.
  - If Umbra was exposed in act 4, the Aurelian rips out its Revaris cameras and takes a reputation hit, tipping Vivian your way.
- **A5.2 Backers** (around the city; start of act).
  - Win Jalen (if he climbed clean) and Vivian as backers. Jalen runs the poker room (the hotel once the post-game tower opens); Vivian is a silent partner; you pay at least a quarter of each phase.
- **A5.3 The crew** (Aurelian, Kasia's Tap; land won).
  - Rosa, license restored, manages all casino staff; friendly cast are offered jobs (Pete back from Tae).
- **A5.4 Relight the rocket** (the lakefront site; land won).
  - Pick the design for a brand-new rocket sign over the new casino, where the hotel tower will rise (~$250k). "The old sign stays over the Starflare Field gate" (OQ 10).
- **A5.5 If the land falls through** (riverfront, a Levee union hall; Tae wins the land).
  - Relaunch on a boat or go co-op. Leads to: Floats (O3) or people's (O4), resting.
- **A5.7 The license hearing** (gaming commission hearing room; land won and the floor built).
  - Pick who speaks for you (Rosa, Pete, Vivian's lawyer, Ed, Graham/Claire, as earned); answer one hard question honestly or dress it up; Bennett's stance set by your act 4 route.
  - Leads to: license restored, or restored with conditions. "It never fails: the worst result is conditions."
  - "The license follows the name" (`plan/t02.md`).
- **A5.6 Opening night** (the lakefront Starflare; sign relit, floor paid ~$1M, hearing held).
  - Gus cuts the ribbon, or you do alone.
  - Leads to: ending 1 or 2; owner mode begins.

## 2.8 Outcomes (O-codes)

Only O1 and O2 end the main story; the rest are resting points you can climb back from. (`plan/t07.md`, `plan/t10.md`, `pages/storymap-branches.md`, `data/story.js` `OUT`)

| # | Outcome | Can happen in | What leads there | Way back | Post-game |
|---|---|---|---|---|---|
| O1 | The lakefront Starflare with Gus (Final) | Act 5, opening night | "Name won back, Vivian's land won, a big bankroll, Gus trusts you." | Final | Light owner mode; tower (~$2M) and launch pad (~$1.5M) as goals |
| O2 | The Starflare without Gus (Final) | Act 5, opening night | "As 1, but you use the leverage to take the name for yourself (ruthless) or Gus has turned on you." | Final | Gus never on the floor; some regulars cool |
| O3 | The Starflare floats | Act 5, if land falls through | Name won back, mid bankroll, an old boat (S5) and the dockworkers on your side | Win the land later and move ashore | Boat venue run lightly |
| O4 | The people's Starflare | Act 4 at A4.10 if short of $250k, or act 5 if land falls through | Name won back by leverage; close to the regulars; least money. The regulars pool small shares and reopen "on a shoestring, with Gus hosting (no union stake)." "Players who never get rich settle here." | Stepping stone: light owner mode earns income; at $250k act 5 opens with the regulars as crew and their shares paid back on opening night (only Jalen and Vivian back the lakefront). If you never reach $250k it stays a full resting point. | Co-op nights at the union hall (licensed) |
| O5 | Tae's restaurant | Act 4, Tae's offer | Side with Tae and Stella; Tae buys the name from the Stadium District. Gus greets; the band photo and old rocket sign hang inside. | Buy the name back at a premium once you hold the land; fair price after the band reunion or if you warned him (S13) | Hangout inside the Aurelian |
| O6 | Gus lets go | Act 4, after helping him quit | You help Gus quit and reunite him with Dot and Stella; nobody funds a reopening; the name retires | A sober Gus comes back and asks you to reopen | Visits with sober Gus, Dot and Stella |
| O7 | The name stays with Starflare Field | Act 4, Field timer ends first | The default if the Field opens before anyone wins the name back | Win the name back later by leverage | Games at Starflare Field under the old rocket sign |
| O8 | Mob takeover | Act 4, Wren's deal | Take Wren's deal, "or deep debt to Ray and a crooked lean"; she pulls the name out of the District through Revaris | Expose Wren later to take the name (leads to O2) | Wren's Starflare as a crooked venue |
| O9 | Burned down | Act 3 or 4 | Fight the demolition dirty with high heat, or cross Umbra; "the empty building is torched as a message before demolition, and the old sign, still on the shell, burns with it" | Carry on; stay clear of Umbra for a while before backers trust you | The burned shell as a story place |
| O10 | The sellout | Act 4 (Wren) or act 5 (Tae) | Betray Gus for a payout: give Wren her loose ends, or help Tae get Vivian's land. Gus, Dot and the regulars turn cold. | Leverage can still take the name (O2), or a long climb on generous and reliable wins Gus back (the only road to O1) | Comped Mile life (Tae) or Umbra favors (Wren) |

**Rocket-sign handling** (audit 1.5; `plan/t07.md`):
- It stays bolted to the empty shell until demolition.
- The District then takes it down and stores it on site until the Field gate is built.
- The O9 fire happens before demolition and takes it.
- Tae (O5) takes it from storage or from the gate.
- Relighting the rocket always means a brand-new sign for the new Starflare, so "the old sign's fate ... is flavour only" (`pages/storymap-branches.md`).

**Cash-out / drive-away** (cut by Mike on 2026-10-27, see 4.1.8; as the docs still describe it, `plan/t10.md`, Mike 2026-10-22): from act 2 on, a phone option lets you drive away over the Long Bridge in your car. It plays a leaving epilogue built from the current state. The save is kept and you can resume from the morning you left.

**Epilogue (EP).** About 8 still panels (`plan/t10.md`):
- Panel 1: what became of the Starflare.
- Panels 2 to 7: the 5 or 6 cast you touched most.
- Panel 8: a Northlight newspaper front page (the mayor's race, Starflare Field, Umbra).
- Then your record card.
- Per `data/story.js` EP: "picked from standing and flags; often Gus, Dot, Rosa, Pete, Jalen; Mayor Dabareski on the front page."

## 2.9 Mysteries (M-codes)

(`plan/t08.md`, `data/story.js` `MYST`.) Rule (Mike): some mysteries only affect characters or the protagonist; "Only 'who bought the Flare' feeds the Starflare story."

**M1 The torn patch** (backstory)
- **Truth:**
  - Rosa's patch is from the Starflare.
  - Gus, drunk, lost the staff payroll at her table, so she dealt him back to even.
  - Cameras caught it and she took the fall without saying why.
  - The commission saw only the overhead dealer camera (Rosa's hands, chip flow, no faces).
  - Gus kept the only wide-angle tape, which shows him in the seat.
  - The commission ordered a modern camera system Gus couldn't afford, which led to Wren's camera deal.
- **Clues:**
  - Rosa won't step inside the Flare (broke).
  - Pete goes quiet at her name (Flare regular).
  - The surveillance tape: in Gus's office before the fall, in Bennett's hands after (Miles/Audrey saw it taken).
  - Her commission file via Vivian's lawyer (rich).
  - Clue puzzle: slide puzzle on the torn patch (`pages/gdd.md`).
- **Solve:** "Gus admits it; Pete and Gus testify and Rosa gets her license back. First proof that Gus gambles."

**M2 Who bought the Flare** (main story; gates act 4)
- **Truth:**
  - "Who is Revaris really, and why won't Gus say its name?"
  - The sale is public record: Revaris Technologies, a camera company. Nobody outside knows it is Umbra's front.
  - Gus signed knowing it was the mob and tells everyone "a friend of Ray's" bought it.
  - Umbra then put the land into the Levee Stadium District, which makes Umbra the biggest investor.
  - Bennett pushed the deal through for campaign money and a hidden cut, which Wren pays herself through Revaris, so her books are the proof and she owns him.
  - False leads: Stella (meeting Tae, but asking who is really behind Revaris) and Tae (would have bought it openly).
- **Clues:**
  - Stella and Tae at the Aurelian (rich).
  - The buyer is public and Gus still says "a friend of Ray's" (free).
  - Proof Revaris is Umbra's front: Raffy's thumb drive shows the Revaris camera feeds going to Umbra, or working off your debt to Ray puts you in the room when he calls the company "us" (broke). (Drive half superseded 2026-10-29, see 4.1.8: the opened ledger is the proof; a locked drive is only a hint.)
  - Mitch recognizes the filing lawyer from Bennett's old deals (broke).
  - Felix/Nora show you the lawyer meeting Bennett (romance).
- **Solve:** Opens act 4. "Clearing Stella brings her and Gus closer."

**M3 The Prairie Queen skim** (backstory)
- **Truth:**
  - Ray makes Kiran and other indebted dealers skim the cheap tables to cover his bad loans.
  - It steals from the house (palmed chips on color-ups, a short-counted drop box), never from players.
  - He thinks it is behind Wren's back; her cameras are on the Queen, she knows and "lets him dig his own hole."
- **Clues:**
  - Kiran's tell and Rosa flagging a table (broke).
  - Ray's new car he can't afford.
  - Ed's file on Ray.
  - The Queen's floor manager grumbling about short counts.
  - A spot-the-difference color-up.
  - Kiran passing an envelope at the landing.
- **Solve:** Kiran gets out. Tell Wren (Ray learns she knew and is humbled; costs heat), tell Ed (informant path), or keep it as leverage on Ray.

**M4 Room 9 and Gus's box** (backstory, leads to the main story)
- **Truth:**
  - The quiet guest in room 9 is Richard, Bennett's father, whose house went to Flare markers.
  - Bennett traced Gus's camera deal to Umbra and forced Gus to wipe out Richard's remaining debt in exchange for silence.
  - Ashamed, Gus gave Richard the key to a box rented in Richard's name (it opens only with his signature) at an old Levee bank. It holds the original signed camera contract and Gus's own recording of Bennett's demand: a recorder in his desk; Bennett chose the office because it is off the floor cameras; the only copy.
  - Bennett pays the room weekly; Richard thinks Gus does. Umbra can't use the key without Richard's signature, so it watches the door to see if he goes to the bank.
- **Clues:**
  - Dot sees a driver drop an envelope every week (broke).
  - A photo of Richard on Bennett's desk (rich).
  - War with Richard at the picnic table (the house he lost; Gus "takes care of" his room, a false lead; the game he taught his son).
- **Solve:** Once Richard trusts you, Umbra men who followed you come to room 9 with his old Flare markers. Gus's condition is met ("If anyone ever comes after you over the debt, open it") and you open the box together. Richard decides: shame Bennett into amending his stadium ordinance to return the name to Gus; reconcile; burn it; or hand it to Ed. "Any public use exposes Gus too."

## 2.10 Seeds (S-codes) in full

(`plan/t09.md` and `data/story.js` `SEEDS`.) Seed rules (Mike 2026-10-21):
- "Nothing on screen says a seed was planted, and there's no journal."
- A reveal waits at least a week of in-game time (often an act), fires only at a break point, and the callback cue is "a small inset of the seed's original panel plus one line."
- "Missing a seed never locks a beat."

`p` and `r` are the numeric fields in `data/story.js` (apparently the act planted and the act revealed; the file does not define them, OQ 56).

- **S1** (Backwater, Millstone Lanes; p1 r3).
  - Seed: The Tuesday league is $40 short for team shirts. Chip in or not.
  - Reveal: Act 3 council hearing: the league turns up behind you in those shirts.
  - Pays into: an extra ally in the clean fight.
- **S2** (Backwater, Kasia's Tap; p1 r2).
  - Seed: An old mill pensioner is behind on his tab. Cover it ($30) or not.
  - Reveal: A season later at the RailYard he wins a slot tournament and splits it with you.
  - Pays into: small money; a generous nudge.
- **S3** (The Bridges, Bridgehead; p1 r3).
  - Seed: A trucker is $20 short on diesel. Spot him or not.
  - Reveal: The Flare's last night: he brings his flatbed and helps save the spaceman statue.
  - Pays into: the spaceman stands in the new Starflare's lobby; Gus's trust.
- **S4** (The Levee, the Flare; p1 r2).
  - Seed: A faded photo of the house band hangs by the lounge. Ask Gus who the drummer was.
  - Reveal: At the Aurelian, Tae hears you say his dad's name and drops his guard.
  - Pays into: Tae's standing; the photo in O5.
- **S5** (The Levee, the Prairie Queen; p1 r5).
  - Seed: An old pilot grumbles about the Queen's sister boat rusting at a south pier. Listen or walk off.
  - Reveal: Act 5, if the land falls through: the boat is there and cheap ("Without the seed you have to find one").
  - Pays into: O3.
- **S6** (The Levee, the Cinder Room; p1 r4).
  - Seed (act 1): Gus is out of chips and refused credit in front of the room. Lend him cash (he signs you an IOU), refuse (humiliated, cool for a while), or walk him out before he plays (sore at first; helps him quit in act 4).
  - Reveal: Act 4: Wren wants the IOU with his other markers; tear it up in front of Gus for big trust; repaid with extra once the name is won back.
  - Pays into: Gus's trust; Wren's deal; the sellout; helping Gus quit.
- **S7** (Marquee Mile, Harborlight fundraiser; p2 r3).
  - Seed: Before the stadium is public, a volunteer asks you to sign the "New Levee" petition.
  - Reveal: Act 3 hearing: Bennett reads your name out as a supporter, with Gus in the room.
  - Pays into: Gus's trust; a scene where you explain.
- **S8** (Marquee Mile, the Aurelian; p2 r5).
  - Seed: The tourist you taught at the table is a travel writer.
  - Reveal: Opening night: her piece names you and the crowd is bigger.
  - Pays into: opening-night flavour; first-week takings.
- **S9** (Silvershore, the Obsidian; p3 r5).
  - Seed: A society columnist asks how much Vivian lost tonight. Tell her (she pays) or keep quiet.
  - Reveal: Act 5: Vivian knows who talked. Quiet makes the land cheaper; talking sends her toward Tae.
  - Pays into: land price; Vivian's standing.
- **S10** (The Levee, the RailYard; p1 r4).
  - Seed: Sign up for the players' card with a face scan, or pay cash and keep your head down.
  - Reveal: Act 4: Wren quotes your losses and where you've been. A spotless player gets a threat against Gus or Jalen instead.
  - Pays into: Wren's deal; Ed trusts you more if you never scanned in.
  - Rule: "she always has something on you by act 4."
- **S11** (The Bridges, Long Bridge; p2 r4).
  - Seed: Crossing before dawn despite Dot's warning (hoodlums, no organization), you see Ray's car with the lights off. Note the other car's plate or drive on.
  - Reveal: Act 4 loose ends: the plate is the filing lawyer's. Ray handed him Gus's signed sale contract and took a secret finder's fee behind Wren's back, after swearing he never met the man. Meeting place: the Long Bridge before dawn (no Wren cameras; hoodlums smash them).
  - Pays into: leverage on Ray.
  - Setup in A2.2: Ray says "I just know a guy, never met him."
- **S12** (Anywhere; p2 r0).
  - Seed: A man sells a "lucky spaceman" bobblehead ("blessed by the real one"). Buy it or laugh.
  - Reveal: Gus laughs at it, and that's all.
  - Pays into: "Nothing, on purpose."
- **S13** (Marquee Mile, the Aurelian; p2 r5; added Mike 2026-10-26).
  - Seed: A Revaris rep pitches Tae a state-of-the-art AI camera system, priced well below its worth. Warn him (via Felix/Nora or Stella) or say nothing. He installs it either way.
  - Reveal: Act 5: if you warned him, Tae remembers you were right and goes easier in the land fight, or pays a fair price for the name (O5). If Revaris's link to Umbra went public in act 4, the Aurelian rips the cameras out and takes a reputation hit, tipping Vivian toward you (A5.1). The new Starflare opens with its own cameras.
  - Pays into: Tae's standing; the land contest. "The Aurelian stays a friendly rival, never an ally."

## 2.11 Side arcs (R-codes) and excursions (X-codes)

Source: `data/story.js` `EV`/`EVD`.

- **R1** Bringing Dot and Gus back together. From act 1; carry messages, find the photo of the two sisters, get them in the same room.
- **R2** Staking Jalen. Stake him in poker; his wins pay you back; he repays in act 4 and becomes a backer.
- **R3** Juan and the loaded dice; Raffy hiding out. Umbra muscles in with loaded dice; he teaches you to spot a weighted die.
- **R4** Heat, Ed and the informant path. Illegal games raise heat; at medium there are sweeps, at high a raid. Tipping Ed drops heat at once but Umbra may notice. Heat cools a step per week of licensed-only play; Vivian's lawyer can clear a raid for a fee.
- **R5** Mitch's book and the bad tip. Learn the tip came from Bennett; he lets you in.
- **R6** The mayor's race. Bennett runs against Mayor Dabareski; background; the epilogue front page.
- **R7** Romance: the piano player (Miles/Audrey).
- **R8** Romance: the desk clerk (Felix/Nora).
- **R9** Romance: the train patron (Graham/Claire).
- **R10** Dot's stories (the flood, room 9, the shirt).
- **X1** The Northern Ledger (spring). Overnight train to the state capital; club-car poker. Unlock: Bennett's invite or best bankroll $10,000. You meet Graham/Claire.
- **X2** The Gilded Pine (summer). Old-money lake resort; unlock $25,000 once. Where you meet Vivian.
- **X3** Haybale Downs (late summer to fall). Harness track; broke-friendly; a tip at Bridgehead.
- **X4** Blackspruce Lodge (winter, once). One-time invite-only high-stakes poker; Ray's invitation; Ray needs a stake partner; Wren appears.

## 2.12 Table events (26) (`pages/table-events.md`; all proposals)

Fired mid-session; never move the main story. Format: Event (Where): the call.
- Jackpot next door (any slots): congratulate, ask for their "system," or keep playing.
- A whale walks in (Harborlight and up): watch, or follow.
- Sore loser (Flare, RailYard, Prairie Queen): calm him down or stay out.
- A familiar face (any): chat or buy a drink.
- Chip on the floor (any casino): pocket it or hand to the pit.
- Side bet (craps, blackjack): take or pass.
- Short a stake (Backwater, Flare): stake a stranger $50 or not.
- Pickpocket bump (busy floors, Long Bridge): tap to check your pocket.
- Spilled drink (bars, lounges).
- Towed (Levee): run out now or finish the hand.
- Loan till payday (Kasia's, Flare).
- Pete needs a hand (Flare).
- Too drunk to drive (any casino).
- A slip across the bridge (Bridgehead): Mitch texts you to run a betting slip.
- Keg run (Kasia's).
- Three-card monte (Levee street, Long Bridge).
- Lucky charm (Bridgehead, Prairie Queen): the bobblehead.
- Fake comp (phone).
- Distract the dealer (Prairie Queen, RailYard).
- Stadium talk (bar TVs): Bennett talking up "a new Levee."
- Storm on the lake (Prairie Queen, yachts).
- Jackpot fever (Bridgehead, phone).
- Race results (Bridgehead, Haybale).
- Song request (Flare lounge, Harborlight).
- Teach a tourist (Harborlight, Aurelian, RailYard): seed S8.
- Birthday table (any).

---

# 3. TIMELINE

Uncertain timing is marked **[uncertain]**. The notes give almost no calendar dates for in-fiction events; era is never named (`plan/t01.md`). Where I give a number derived from stated numbers, it is labelled "derived."

## 3.1 Backstory (before the game begins)

**Settled order (Mike, 2026-10-27; an assumption written in so writers stop re-deriving it, changeable later).** The founder, Mr. Hamilton, died about fifteen years before the game, when Stella was ten. Payroll night was about six years before the game; the commission then ordered the cameras. Gus signed Wren's camera deal about four years before. Richard's markers and Bennett's squeeze fell about three years before. Richard moved into room 9 and Wren took the reins of Umbra about two years before. Items 10 to 15 below keep their source wording; this paragraph is the order to use.

1. **[undated]** Northlight was "a steel, rail and shipping town." The mills closed and the waterfront emptied. The state legalized riverboat gambling "on the water only" to bring jobs back. "Land casinos came later." Backwater voted its district dry. (`plan/t02.md`; `pages/gdd.md`) The mayor "brought in the riverboats and land casinos after the mills closed" (`people/mayor.md`).
2. **[undated; "the rocket age"]** Mr. Hamilton, the founder, builds the Starflare as a rocket-age supper club. The Flare's house band plays there; Tae's dad is its drummer. (`pages/walkons.md`, `people/tae.md`, `plan/t02.md`) The illegal back room was dropped 2026-10-22.
3. **[undated]** Gus is born and grows up in the Flare; Pete (Levee, steel-mill family) grows up alongside him. (`people/pete.md`, `people/gus.md`)
4. **[derived: about 41 years before the game; Pete is "about 60" and started "at 19"]** The founder hires Pete to bus tables at the supper club. Pete learns cards by watching. When "land casinos came and the Flare got its license" he is one of its first dealers. The Flare's license date: not stated. (`people/pete.md`, audit 1.7)
5. **[undated]** Juan starts at Millstone Lanes as a teenager, about 25 years before. (`people/juan.md`)
6. **[undated]** Pete is best man at Gus's wedding; Stella is born; Tae, a family friend, is a big brother to little Stella. (`people/pete.md`, `people/stella.md`)
7. **[undated, "Years ago"]** Mitch bets on supposedly inside information from Bennett and loses almost everything; the money ran through the filing lawyer's trust account. (`people/mitch.md`)
8. **[undated]** Gus's late wife dies; the funeral. Dot and Gus have "not really spoken since." Pete keeps the Flare running. (`people/gus.md`, `people/pete.md`)
9. **[undated]** Wren's early 20s: crypto scams and other exploitation through technology. She invents her AI face-matching cameras and founds Revaris Technologies. (`people/wren.md`)
10. **[undated, "Years ago"; before the camera deal]** Payroll night. Gus, drunk, loses the staff payroll at Rosa's table; Rosa deals him back to even; cameras catch it. Rosa takes the fall and is blacklisted. The commission sees only the overhead dealer camera. Gus keeps the wide-angle tape. The commission orders a modern camera system. (`plan/t08.md`)
11. **[undated, "a few years before the game starts"]** Richard runs up markers at the Flare; the house he lived in for decades goes to cover them; he still owes more. Relative order with item 10 is not stated. (`pages/room9.md`, `people/richard.md`)
12. **[about four years before the game]** Wren and her grandfather land the Flare's camera deal. Wren offers cheap cameras; Gus refuses; her people show him Cinder Room footage of his gambling and threaten to send it to Stella and the commission; he signs knowing it is Umbra. Wren's own copy of the contract goes in a shredder. (`people/wren.md`, `plan/t08.md`; the motive differs in `pages/room9.md`, OQ 13)
13. **[after 12; before about two years ago]** Bennett, who holds the Flare's license, traces the camera company through business and license records and links it to Umbra through Ed. Ed gave him the link in good faith. Bennett forces Gus to wipe out Richard's remaining debt in exchange for silence. Gus secretly records it on a desk recorder. Gus rents a safety-deposit box in Richard's name and gives Richard the key ("If anyone ever comes after you over the debt, open it"). (`pages/room9.md`)
14. **[about two years before the game]** Wren's grandfather hands her the reins of Umbra over the uncles. The Flare's, then RailYard's and Prairie Queen's camera data was "worth more than anything the uncles ran." (`people/wren.md`)
15. **[the last couple of years]** Richard lives in room 9 at the Slack Water. Gus arranged it through Pete (Dot only knows Pete asked a favor) and paid the first weeks; Bennett took over the bill by envelope out of pride. (`people/dot.md`, `pages/room9.md`) Timing relative to items 12-14: [uncertain] (OQ 43).
16. **[undated, before A1.2]** Kiran borrows from Ray for his daughter's surgery; the Prairie Queen skim begins. Ray copies Wren's logs off the Prairie Queen feed onto a thumb drive (superseded 2026-10-28, see 4.1.8: he keeps the old ledger drive when Wren upgrades the device). Gus gambles at the Cinder Room, borrows from Ray and quietly remortgages his house. (`people/kiran.md`, `people/raffy.md`, `people/stella.md`) Dates not stated.
17. **[the night of A1.2]** Raffy lifts the envelope from Ray's car. (`people/raffy.md`)

## 3.2 The game (player-paced; durations are budgets, not calendar dates)

Day, season and time model:
- Parts of the day: morning, afternoon, night, plus a before-dawn window.
- Season: 28 days.
- No story clock except the Field Opening Day.

**Act 1 (~8h)**
- Night 1: arrival (A1.1); Kasia's Tap (A1.2); Raffy's envelope.
- After night one: Kasia's two jobs and the back room (A1.3); the wedding, a third job (A1.4). The first Flare visit (A1.6) follows the wedding. A1.5 comes "weeks after night one."
- A1.7 to A1.9 in any order. A1.8 at $1,500; A1.9 once Ray slips the password.
- A1.10 at $5,000 + Gus's trust + two weeks.

**Act 2 (~8h)**
- A2.1 Ray collects.
- A2.2 Tae's offer refused; "a friend of Ray's" buys the Flare; the debt clears. Umbra puts the land into the District.
- A2.3 to A2.7.
- X1 in spring if $10k.
- A2.8 the Field is announced: $25,000 + a season (28 days) after the bailout.

**Act 3 (~8h)**
- A3.1 renderings; A3.2 the fight; A3.3 condemnation and the last night; A3.4 Pete and the pin; A3.5 the scatter (Field timer starts).
- A3.6 to A3.8 anywhere in the act.
- A3.9 M2 solved + $50,000.
- Rocket sign sequence: stays on the shell; at demolition the District takes it down and stores it on site; later it hangs over the Field gate. O9: the fire happens before demolition and takes it.

**Act 4 (~10h)**
- A4.1, A4.2 at the start of the act.
- A4.3 to A4.8 in any order.
- A4.9 if the Field opens first.
- A4.10 name won back.

**Field timer** (`pages/storymap-open.md`):
- Opens on Opening Day (early spring), the first one at least three seasons after the fall.
- Derived: three seasons is about 21 hours at 7 hours a season; compare act 4's ~10 hours (OQ 29).

**Act 5 (~8h)**
- A5.1 to A5.5, then A5.7 the hearing, then A5.6 opening night. O1/O2 end the main story.

**Post-game.** Light owner mode. The tower (~$2M) and rooftop launch pad (~$1.5M) are goals. At the tower's opening Jalen runs the hotel; Felix/Nora move to its front desk.

**Epilogue.** After opening night or at a resting point; or on drive-away (cut 2026-10-27, see 4.1.8).

---

# 4. ESTABLISHED FACTS

## 4.1 Hard constraints

### 4.1.1 Settled decisions from `plan/t01` to `plan/t13` ("Already decided")

**t01, Time period (plan/t01.md)**
- Era-agnostic (decided 2026-10-16). The game "never names a year and has a timeless retro feel, the way Stardew Valley does." Leaves room for "a truly futuristic lakefront Starflare." Passed on: a fixed early-1990s setting; eras that change as the game goes on.
- Full smartphones exist (2026-10-16, task 2). "Hustles, rumors and online betting need to be designed around them."

**t02, The city (plan/t02.md)**
- A big fictional Great Lakes city, Chicago-like, not desert. A river flows into the lake and splits the city. Working name Northlight (2026-10-16); runner-up Breakwater.
- Districts (poorest to richest): Backwater; the Bridges; the Levee; the Marquee Mile; Silvershore. Names avoid real places and other IP (the Narrows dropped for Batman).
- Backwater is dry by local vote (same city and state; Mike 2026-10-17, replacing the earlier "other state" setup). The state allows gaming only in licensed districts, which voted. Backwater voted no, so every Backwater game is illegal.
- One Northlight vice squad covers the whole city, working out of Lock Street Station (Mike 2026-10-23; Ed's base).
- Online casino betting is illegal in the state (default until revisited).
- LICENSE RULE (Mike 2026-10-26, audit 2.5): "the license follows the name." The commission treats the Starflare "as one licensee changing address," so winning back the name lets it be licensed again wherever it reopens in a licensed district (the lakefront on the Marquee Mile at the Silvershore line, a boat on the water, or a union hall in the Levee), "after a commission hearing on the player's record (A5.7)."
- NAMING RULE (Mike 2026-10-25, audit 2.1): "the outfit is Umbra to everyone, on the street and inside; Revaris Technologies is only Wren's camera company, a respectable-looking tech firm." "Player-facing text never ties the two before M2 is solved."
- No religion in the game.
- Power: the Machine; Umbra; the Unions; new money versus old.
- Housing ladder, landmarks, slang and food (see below).

**t03, Venues (plan/t03.md)**
- Venue roster settled 2026-10-16. RailYard added 2026-10-19.
- Rule (Mike 2026-10-16): "every game must be playable continuously ... one-off cons ... are events, not games."
- Story-event games (Mike 2026-10-22): a story event may include a game played a set number of hands (War with Richard); venue games stay replayable.
- CHEATING RULE (Mike, revised 2026-10-19): "ordinary play is always fair." Doubt only in story moments.
  - Accuse a crooked dealer and Umbra notices you.
  - Accuse an honest one and you lose standing at that venue.
  - Ride a crooked table and you bleed money (crooked).
  - Walk and you never know.
  - "The truth always surfaces later."
  - Rosa's lessons "sharpen the tells but never make you certain."
- Slots (Mike 2026-10-19): every venue except the yacht games has themed slots; at Kasia's "for amusement only"; at Millstone free play.
- Video poker in most venues, not the Cinder Room or yachts.
- Hazard dropped as the Cinder Room's house game (2026-10-19); casino war chosen.
- Backwater game access: Kasia's back room is by Kasia's invitation after two jobs (Mike 2026-10-23).
- Name history: the Starflare replaced the earlier "Starlight" name, which a real casino in Canada already uses.

**t04, Excursions (plan/t04.md; settled 2026-10-16)**
- Four trips, one per season; all repeatable except the lodge.
- Dropped: a second riverboat; the ice shanty camp is a later add-on.
- Northern Ledger: spring; Levee alderman's invite or $10,000 once.
- Gilded Pine: summer; $25,000 once (Silvershore-invite unlock dropped; audit 4.5).
- Haybale Downs: late summer to fall; a tip at Bridgehead.
- Blackspruce Lodge: winter; "Ray's invitation in your first winter as a Cinder Room regular: he needs a stake partner (audit 4.4)."

**t05, Protagonist and opening (plan/t05.md)**
- Blank slate. You arrive with about $200 in a modest car at night.
- Reason for coming: never a form. The motel clerk asks; you answer with an action (2026-10-16).
- Name entered by signing Dot's register (Mike 2026-10-23); the game uses it from then on.
- First image: the Starflare's rocket sign across the river.
- Four hidden scales (2026-10-16): generous/tight, reliable/flaky, ruthless/soft, straight/crooked. Never shown as numbers. Straight/crooked is separate from heat.
- Gender: Mike 2026-10-17; set at check-in (2026-10-23). The rival matches it.
- Kasia doesn't send you to the Flare (Mike 2026-10-23).
- The opening is player-paced; "three rungs in one session" dropped (audit 1.4, Mike 2026-10-24).
- You meet Gus on your first visit to the Flare.

**t06, Cast (plan/t06.md)**
- Tiers and hangouts (2026-10-17); naming rule (Mike 2026-10-17); tone rule: "Northlight is a nice city with some rough areas, like any big city; characters never treat it as a place to escape."
- Romance rules (see section 1.5).
- Voice sheets settled 2026-10-18: "Great Lakes, not movie-gangster: understatement, dry humour, work-and-weather talk, people who say less than they feel." Slang (riding the light, gone dark, going over the water) stays rare, "a few uses per act."
- The wedding at Kasia's Tap (settled 2026-10-22; revised 2026-10-23; "a separate third job," Mike 2026-10-24).

**t07, The Starflare story (plan/t07.md)**
- Settled 2026-10-17: slow decline felt from the first visit; Gus secretly gambles; one storyline with places to settle.
- Reworked 2026-10-23: the Levee Stadium District; Revaris buys the Flare ("no shell company"); the sale clears Gus's debt; "once sold, it's sold."
- 2026-10-26 (audit 2.6): the name sits in the stadium ordinance Bennett sponsored.
- Act 4 has no cash route.
- White-hat leverage is the clean road; every route has a black-hat version (Mike 2026-10-26).
- Rocket sign (Mike 2026-10-24): stays on the shell until demolition, then District storage on site until the Field gate is built.
- Rebuild costs (starting values): Vivian's land ~$1M; relight rocket ~$250k; gaming floor ~$1M; tower ~$2M; launch pad ~$1.5M.
- Backers (2026-10-21): only Jalen and Vivian; you pay at least a quarter of every phase.
- O4 revised 2026-10-21 (no union stake) and 2026-10-26 (stepping stone).
- Field named by Mike 2026-10-23.

**t08, Mysteries (plan/t08.md)**
- Four mysteries settled 2026-10-18. The Long Bridge robbery crew was cut as a mystery (kept as the unaffiliated hoodlums of S11).
- Room 9 rewritten 2026-10-22.
- Revised 2026-10-24, 2026-10-25, 2026-10-26 (audits 2.2, 2.3, 3.1 to 3.7, 4.8).

**t09, Seeds (plan/t09.md)**
- Settled one by one 2026-10-21. Seed rules as above. Event schema accepted as a starting point.
- S13 added 2026-10-26.

**t10, Endings (plan/t10.md; 2026-10-22)**
- Opening night of the lakefront floor ends the main story; every other outcome is a resting point.
- Cash-out via the phone from act 2 (drive away over the Long Bridge).
- Epilogue is about 8 panels.
- Post-game: light owner mode; no new systems beyond light owner mode for O3 and O4.

**t11, Title (plan/t11.md; 2026-10-24)**
- Working title: Starflare Casino. Store subtitle such as "A Casino Life Story" (30 characters max on the App Store); icon of the rocket sign plus dice or a chip. Run a USPTO and app-store trademark search before launch. Mike chose not to use "Stakes" because of trademark worries.

**t12, GDD (plan/t12.md; done 2026-10-26)**
- The GDD is rewritten as a developer-facing build spec. It stays separate from the story pages and links to them.
- Items still marked proposals (day and season length, the job rows, the inventory list, the romance beats) are written in and labelled as proposals.

**t13, Jobs, phone and inventory (plan/t13.md; settled 2026-10-19 and 2026-10-24)**
- Jobs are small, earn money, take time; shown on a Jobs menu on the phone (2026-10-19); no corkboard.
- Pay (2026-10-24): jobs pay less than a good night at the tables. Honest $40-$150 a shift; trusted repeat work up to about $250; crooked $200-$400, each adding heat or another risk.
- Each job costs a slice of the day or runs a few days. "Some carry a small deadline ('done by Friday'), never a story clock."
- Phone tabs: Wallet, Jobs, Messages, Pockets. "The phone can't place bets."
- Inventory: 12 slots; story keys in their own row, outside the 12; keys "can't be pawned unless a scene offers it."
- "The car can't be sold. It is always your floor."
- Crooked jobs form a ladder; "collecting for Ray opens only after several Ray jobs" (debtors follow the story, audit 2.12).
- Troy runs Bridgehead Loan & Pawn.

(`plan/checklist.md`: all thirteen tasks ticked.)

### 4.1.2 Rules in `pages/writing.md`

**Art/panels**
- "There is no character animation, so the screen is the panel."
- Panel units: Plate, Two-shot, Pose swap, Insert, Black panel.
- "One line per panel, 15 words at most."
- "Motion is a short caption."

**Tone**
- "Noir by night, warm by day."
- Weather and seasons are free mood layers.
- "Show the clue, say nothing."
- "Props replace exposition."
- "Let decline show" (the Flare's plate gets darker).
- Backstory in 3 or 4 panels.

**Dialogue**
- "Write to the voice sheets."
- "Great Lakes, not movie-gangster."
- "Slang is rare" (a few uses per act). Say "took a bribe" plainly.
- "Choices are actions." Never label intent ("[Lie]") or show scale effects ("Straight +1").
- "No moral menus."
- "The key line is the one nobody says."
- "Bluffs run on information"; no dice rolls in conversation.
- Use double meanings (marker, the house, cash out, fold, all in, comped).
- "No religion; romance only with the romance characters and never changes the odds."

**Tension**
- Put a stake on the choice.
- Tells are shown, never narrated.
- "Tells belong to people, not house games."
- Follow the cheating rule.
- "Let the proof be checkable."
- Use the setting as pressure.
- Ask the tip-giver to look, not to judge.
- Companions change the scene.

**Script layout**
- Ink with tags.
- A completion checklist (plate/pose/insert tags, no panel over 15 words, voice sheets, clues as inserts, options are actions, one stake, walking away isn't free, normal play fair, each choice sets a flag, time/weather fit).

### 4.1.3 Rules in `pages/direction.md`

Under "Decided":
- No hard deadline or story clock as the main driver; life-simulator feel.
- Blank-slate protagonist.
- NPCs have their own stories that affect your advancement, and you can affect theirs.
- Consequences can surface much later; some NPCs change independently of the player (authored drift).
- One big fictional Great Lakes city, Midwestern, "not desert, so it doesn't read as another Las Vegas."
- Structure: "There is no timer on the story ... A hard deadline would make going broke feel like losing."
- "Choices build a reputation, not a personality."
- "Action-first dialogue."
- Set aside: a small town, a multi-city tour, a desert setting.
- Earlier premises set aside: The Wager, Hot Money, Last Week of the Starlight (hard deadlines); The Inheritance, The Comeback (defined protagonists).
- "Seasons that matter" (winter blizzards, summer festivals, the lake freezes and the yacht games stop until spring).
- Themed casinos.
- Each key NPC has fortune, arc stage and standing.
- Seed and reveal guidelines (let time pass; show them anywhere; callback cue; let some go nowhere).

Under "Proposed, not yet confirmed" (as written): the "New in Town" framing; the draft cast (Rosa, Dutch, Teddy, Vivian, Gus); seeds and reveals as the mechanism.

Stale content here is in OQ 2.

### 4.1.4 Rules in `pages/gdd.md`

**Design pillars**
- "Money is a two-way gate." Going broke is content, never a game over.
- "Story is the spine, gambling is the vehicle."
- "Money and time move the story, never luck."
- "Every choice is remembered."
- "Fair tables."
- "Built for short sessions."
- "Low art burden."

**Systems rules**
- Hysteresis: "Venues and gates key off best bankroll and flags, never the current balance. Losing access is a deliberate story event."
- "Raffy's envelope sits in story keys, and its $2,000 doesn't count toward best bankroll until you choose to keep it (A1.5)."
- "The car can't be sold."
- Seed rules (settled 21 Oct 2026).
- Clue puzzles (Mike 26 Oct 2026): slide; spot the difference; tile rotate or jigsaw. Optional, no fail state, cost a slice of the day, "evidence, never certainty," never at a real table, replayable from a phone shelf. Randomized.
- Clean or dirty (26 Oct 2026).
- The license hearing (A5.7).
- Platform: Unity with Ink (Mike 26 Oct 2026); portrait first; iOS and Android first; consider Switch 2.
- Monetization: "Premium, or a free demo with a one-time unlock. Never sell chips." Mature age rating. AI-generated art is for placeholders only; disclose where stores require.
- Event schema (starting format).

### 4.1.5 Other rules from `pages/romance.md`, `pages/storymap-*.md`, `pages/room9.md`

- Romance (settled 2026-10-23): no dating system; never touches fairness; never with the main cast; the wedding sets the version (see 1.5).
- Best bankroll = "the most of your own money you've ever held: cash minus what you owe (Ray's loan, Kasia's ledger, Flare markers), so borrowing can't open anything."
- Each act gate needs money and minimum time.
- Backers: only Jalen and Vivian. "Tae is not a backer and stays a rival. The union doesn't invest (conflict of interest)." (`pages/storymap-open.md`)
- Rosa "never a Flare regular" (fixed 2026-10-22).
- War (Mike 2026-10-22): 10 cards each, every card flipped once, won cards to a score pile, most cards wins. Ties: three cards down and one up; if either player has fewer than four cards left they flip their last card face up. War replaces gin rummy everywhere.

### 4.1.6 Other settled rules recorded only in `pages/notebook.md` (history; settled per its text)

- Art rule (Mike): no animations; swap poses; describe motion as text.
- Mike likes to describe looks himself; "ask him before suggesting one." A varied cast across backgrounds is wanted.
- "Keep such family links rare" (the Dot-Gus link).
- Names must not collide with real places or other IP; "looks that reference other IP are artist notes only."
- Mike is the developer ("Mike (2026-10-21): he is a software developer and will develop the game personally").

### 4.1.7 What the game is not (as recorded)

- Not a timed run: no story clock or game-ending timer (`pages/direction.md`, `pages/gdd.md`).
- Not a game with a defined protagonist or a dating sim (`pages/direction.md`, `pages/romance.md`).
- Not Las Vegas or a desert (`pages/direction.md`).
- Not set in a named year or decade (`plan/t01.md`).
- Not a place to escape: "Northlight is a nice city" (`plan/t06.md`).
- Not a "social casino": never sell chips (`pages/gdd.md`).
- Not online betting: the phone can't place bets (`pages/gdd.md`).
- No game over for going broke (`pages/gdd.md`).
- No religion (`plan/t02.md`, `pages/writing.md`).
- Not movie-gangster in voice (`pages/writing.md`).
- No character animation (`pages/cast.md`, `pages/writing.md`).
- Not rigged in ordinary play (`plan/t03.md`).

### 4.1.8 Decisions from Mike, 2026-10-27 (story-editing session; not yet on the site)

These answer open questions below and were given while reviewing `docs/suggestions/`. Each is a settled fact for later writing until the site says otherwise.

- **The license hearing's hard question (A5.7)** is put to the player, about the camera deal: did Gus know what he was signing, and when did you find out. Truth, bluff or take the concealment onto your own record. "Take it" means owning the non-disclosure, not the deal: "I won't speak for him. I knew before I filed, and I filed anyway." Gus holds no license, so the cost falls on the player, and Gus is kept out of the record. It's offered only if the player heard Gus's confession at A4.1 (revised 2026-10-28). See `suggestions/S-001`. A question about Gus's own play was judged too small; the question must be about approving illegal activity.
- **Gus never owns the new Starflare and is never its licensed host.** He retires. In O1 he comes to opening night as a guest and cuts the ribbon. Stella's "he should retire" thread resolves this way. This rules O1 and O2 only; his informal roles in the other resting points (hosting the regulars' floor in O4, greeting at the Aurelian in O5) are as the docs have them.
- **Gus's late wife is named Alice.** Dot says it; Gus says it once, at the end; Stella says "Mom."
- **Gus's wound stands:** the night Alice died he was in the Cinder Room with his phone silenced, and Dot is the one who couldn't reach him (`suggestions/S-012`).
- **OQ 33, Stella and the gambling:** she learns from the player, if the player chooses to tell her (a third stake at A4.1). If never told, she learns at the torn patch, speechless, as her sheet says.
- **OQ 34, Rosa's tape:** any act 4 leverage route can trade for the tape as part of its deal. The romance pair (Miles/Audrey) is only the easiest way.
- **OQ 44, the register box:** Mr., Ms. and Prefer not to answer.
- **OQ 25, the ruthless O2:** at A4.3 Wren will sell the name to you rather than return it to Gus, at a price plus a favor. Dirty and fast, and she owns a piece of you.
- **OQ 26, O8 versus O10:** decided by what you hand Wren. Raffy's drive (the evidence): she keeps you on as hers and the mob runs the new floor with you in it (O8). Gus's markers plus the payout: you sold him and walked away with money (O10).
- **OQ 42 and 43, the backstory order:** see the settled paragraph at the top of 3.1.
- **Two men in parked cars** (Bennett outside room 9, Ray outside the Flare after Gus signs) is a deliberate rhyme; both stay (`suggestions/S-038`, `S-045`). (Revised 2026-10-29: Ray's half is S11, his car on the Long Bridge before dawn. The Flare-lot hour is backstory only, not a plate.)
- **The Flare's last night (A3.3)** has three scene slots out of six scenes (`suggestions/S-003`). Every Flare game is open that night, the Rocket Wheel included, and a black panel tells the player it is their last chance to play at the Flare. Playing doesn't use a slot.
- **Truth is told where nothing records** (`suggestions/S-004`): confessions happen in Backwater or on the Long Bridge. Gus's A4.1 confession is at the Slack Water Motel picnic table.
- **Room 9's reconcile ending stays as the docs have it.** No peek and no thrown hand (`suggestions/S-006` rejected).
- **The cash-out / drive-away is cut.** There is no leaving ending: at the end you stay and run the new casino/hotel. This overrides the 2026-10-22 decision in 2.8 (`plan/t10.md`); the design docs still describe the drive-away and need changing later (`suggestions/S-010` rejected).
- **Going broke is fixed by doing jobs.** No NPC rescues the player when they're down (`suggestions/S-053` rejected, 2026-10-28).
- **The Flare has three named regulars with portraits:** Walt, Lorraine and Augie (`suggestions/S-055`, 2026-10-28). This adds three portraits to the `plan/t06.md` budget.
- **Gus's turning point is the recipe box** (`suggestions/S-011`, 2026-10-28). Alice and Dot cooked with their mother as girls. The mother's recipe box went to Alice. After the funeral Dot asked Gus for it through Pete and he refused. The phone night (S-012) is the wound, and the refusal made the silence a break. In act 5, once he has quit, he drives it over to Dot himself (R1's payoff).
- **A4.2 ends at Dot's lot** (`suggestions/S-016`, 2026-10-28). You get Gus out of the Cinder Room and drive him over the Long Bridge before dawn. Dot says "Two ten," her first words to him since the funeral. Dot hasn't crossed the river since the funeral; she first does on opening night. There is no behind-the-desk rule.
- **Pete has a family** (`suggestions/S-020`, 2026-10-28): his wife Ona and two kids (a grown daughter, and the youngest, Tomas, at college), all offstage with no portraits. Tomas's tuition is why Pete "can't afford to retire yet." Pete's rating of the player is never shown on screen (`suggestions/S-019`).
- **"Dry" stays out of player-facing text** (Mike, 2026-10-28). In the docs, Backwater's "dry" means no legal gambling; alcohol is legal there. In-game lines say "voted no on the casinos" or similar, so players don't read it as a ban on drinking.
- **Kasia's dad was a businessman, not a mill man, and Kasia wears no union pin** (`suggestions/S-025`, 2026-10-28). This overrides `people/kasia.md`. He opened the Tap at the mill gate and rented the back room to the local, and he backed the "no" vote because a casino would have emptied his bar.
- **Kasia's credit** (`suggestions/S-026`, 2026-10-28): "put it on the book" at the Tap for pull tabs or, once invited, the back room, up to a limit that starts around $50. No interest and no due date. Repaying within about a week counts toward reliable. The limit rises and falls with the reliable/flaky scale.
- **Jalen grew up in the Levee, not Backwater**, and has no family tie to anyone in the cast (`suggestions/S-027` rejected, 2026-10-28). He takes hospitality classes by day and buses tables at the Flare at night to pay tuition. On opening night he runs the front of house, and later the hotel; this settles OQ 1 (`suggestions/S-029`). He is not staked and doesn't play for money. The player can lend him tuition ("what Jalen owes you"), which he repays in tip envelopes. In act 4 an inheritance from a great-aunt lets him decide, on his own, to become a backer (`suggestions/S-028`). "Saving up to get out of Backwater" no longer applies.
- **Stella's arc** (`suggestions/S-046`, 2026-10-29): "Tell Stella" is a third option at A4.1. If told, she joins the A4.2 reunion at Dot's lot ("Dad. ... How much.") and can speak for the player at the A5.7 hearing, in stories. In O1 she closes the count ledger without writing the total: "It's enough." At the O6 visits she comes without her laptop.
- **Stella's history** (`suggestions/S-047`, 2026-10-29): she learned to count at the cage beside her mother, with no Pete in the memory. Gus keeps the Flare's books himself, badly; Stella runs quarterly copies at her kitchen table and has never been on the payroll. She was at school downstate during payroll night, and the books never showed it, because Rosa dealt Gus back to even.
- **Ed** (`suggestions/S-048`, 2026-10-29): running the player's plate adds a line to Wren's file. If the player helped him once and asked nothing back, he tells them at Millstone Lanes that he logged that lookup under another case number, and his squad never adds to her file again. On the informant path he uses the player's tips partly as bait, and says so if asked.
- **Kiran after the skim** (`suggestions/S-049`, 2026-10-29): helped, the color-up inserts match ("She's seven. Eight now."); walked away, he's gone and his daughter's photo is still by the tray; called out, he's at the Prairie Queen landing out of uniform and looks away. No ring turn: the game has no animation, so his ring is text only.
- **Mitch's ruin** (`suggestions/S-051`, 2026-10-29): about thirty years ago Bennett told people Backwater would vote yes on gaming; Mitch bought a Backwater storefront with his and his father-in-law's savings, and Backwater voted no. The filing lawyer handled those sales, and Mitch's money went through the trust account he names at A4.11.
- **Juan** (`suggestions/S-052`, 2026-10-29): "Gus runs a casino that wishes it was a supper club. I run a bowling alley that's a dice game. Same thing." He knows of Gus, not Gus himself. If R3 fails, lane six goes dark.
- **Raffy's envelope** (`suggestions/S-050`, 2026-10-29): it held $2,500 when he lifted it; he took $450 for his mom's rent and tipped the player $50 from it, leaving the player $2,000. Helping him pay Ray means covering the missing $500, which he repays, the last of it at the Lanes: "The one I gave you that first night was Ray's. This one's mine." A darker A1.5 option: keep the $2,000 and tell Raffy it's settled. He stops hiding, Ray finds him within days, he names the player, and Ray adds the $2,000 plus interest to the player's debt and never trusts them again.
- **Motel scenes reviewed** (Mike, 2026-10-29): a motel is an odd place for most encounters, but these stay at the Slack Water: Richard's War games, Gus's A4.1 confession at the picnic table, the A4.2 Dot–Gus reunion in the lot, and the O6 visits. Don't add new motel scenes without a reason tied to Dot or Richard.
- **No deck from Richard** (Mike, 2026-10-29): Richard doesn't give the player his deck of cards, and he has no item in the story-key row (`suggestions/S-009`).
- **Ray's history** (`suggestions/S-045`, 2026-10-29): a Levee kid who ran errands for the Cinder Room under the old Umbra boss and stayed twenty years; has worked for Wren two years. He kept the old ledger drive on purpose, as insurance, and still has its key written down. The hour in his car after Gus signs is backstory, not a plate.
- **Ray's arc** (`suggestions/S-044`, 2026-10-29): from act 2 on the player can give the drive back to Ray ("I read it" / "I looked"). Gains: no interest on his loans, "Ray owes you" at A4.7, he walks Gus out at A4.2, and he leans on the payers with the player in act 4. Costs: no Ed subpoena, no O8. Returning only the cash at A1.5 lowers his interest. A player who read the drive keeps the names in their notebook; names are not evidence and not what Wren wants.
- **Say "interest," never "vig"** (Mike, 2026-10-29). Ray's loans charge interest; the word "vig" is not used anywhere, player-facing or in the docs.
- **Honorifics follow the register** (2026-10-28). An NPC who says "sir" says "ma'am" if the player ticked Ms. and uses no honorific if they chose Prefer not to answer.
- **Not every character needs a tie to another** (Mike, 2026-10-28). The city should feel big. Avoid coincidental backstory links between cast members.
- **The rival for Prefer not to answer** is Chase or Brooke, picked at random (Mike, 2026-10-28).
- **Nights out with the rival** (`suggestions/S-030`, 2026-10-28): three invitations in acts 1 and 2 (the RailYard, the Harborlight, the Prairie Queen), each with one event that moves the rival's standing. That standing decides A3.8 and whether the theft at A4.8 can happen (`suggestions/S-031`). The rival plays clean all game.
- **Vivian** (`suggestions/S-033`, 2026-10-28): her family's insurance company was founded by her grandfather, not her great-grandfather. She is spending the family money down on purpose and holding the lakefront land for whoever asks what it should become. No handshake with the founder. The idea in `plan/t07.md` is dropped.
- **Bennett and Richard** (`suggestions/S-038` to `S-041`, 2026-10-28): Bennett never visits room 9 but sometimes sits in his car in the lot, most Tuesdays. Richard was a crane operator on the Levee docks. His trust rises when the player asks him for something, never when they give him something.
- **The Cinder Room** (Mike, 2026-10-28): Wren keeps it because powerful, shady men have always gathered there. It is her intel source on those men and the place Umbra's dealings run through proxies. She rarely goes there herself. Not sentiment. Wren is a mysterious figure the player meets once, when she needs something and offers something in trade.
- **Raffy's drive** (`suggestions/S-058`, 2026-10-28): Revaris's encrypted key ledger, the only place a face ID is tied to a name. Site boxes hold IDs only; their exclusion list is Umbra's roster plus men who pay to be unseen. Ray runs enrollment at the Cinder Room for Wren and kept a superseded ledger as insurance. Wren moved the ledger to a newer device and told Ray to destroy the old drive; he kept it. The key is the password Ray typed at every enrollment, written down in a couple of places: he still has a copy and can open the old drive himself, and at least one copy is shredded into his uncollected burn bag, where the player can rebuild it with the jigsaw mini-game. Ed exposes Umbra by subpoena, slow and loud. The drive phones home: plugged into anything online, it tells Wren a decommissioned serial is alive, but not who has it; that is how she learns it exists (2026-10-29). At A1.5 the volume says Revaris, and no A1.5 text calls Ray Umbra's man (naming rule). M2's drive clue is the opened ledger (Ray's name and Cinder Room men beside Revaris IDs); a locked drive is only a hint (2026-10-29). Read rule (2026-10-29): the player can always read the drive once they've solved the puzzle, so it never leaves their hands at A1.5. A1.5 settles the envelope's cash and Raffy; Ray can only say "the envelope." The drive's fate (Ed, back to Ray, Wren, keep) is chosen from act 2 on. This replaces "Ray copied the face-match logs" and the two-file drive in `people/ray.md`, `people/raffy.md` and A4.3.

## 4.2 Places and venues

**Districts** (key light / palette / recurring image / weather layer; `pages/northlight.md`):

| District | What it is | Key light | Palette | Recurring image | Weather |
|---|---|---|---|---|---|
| Backwater | Dry district across the river; every game illegal; mill families | Sodium streetlamp, bar neon, bowling-alley sign | Amber, brick red, tired green | The rocket sign across the river from the motel lot | Rain on asphalt, steam from mill grates |
| The Bridges | Main crossing; gas-station slots | Gas-station canopy fluorescents | Flat white, oil-slick purple | The Long Bridge's lights over black water | Fog before dawn |
| The Levee | Old downtown on the river; the Starflare, the riverboats, the Stacks | The rocket sign, flickering | Teal, coral, chrome, rust | The Stacks; the bucket under the Flare's roof leak | River mist; the Prairie Queen's paddle lights |
| The Marquee Mile | Neon lakefront strip; hotel casinos; new money | Hotel marquees, LED walls | Magenta, gold, lake blue | The North Light sweeping the strip | Wind off the lake, winter snow |
| Silvershore | Old money; private clubs, resorts, yachts | Warm lamps, candlelight | Ivory, deep green, brass | Yachts on a dark lake | Clear and still; "no neon is the luxury" |

- Matching compositions: the rocket sign from Dot's motel lot on night one, and the city from Vivian's yacht later, "with the rocket a tiny coral dot across the water."
- "Noir by night, warm by day."

**Venues** (`pages/gdd.md`, `pages/northlight.md`, `plan/t03.md`; bets are starting values):

| Venue | District | What it is | Games | Bets | Getting in |
|---|---|---|---|---|---|
| Millstone Lanes (back lot) | Backwater | Struggling bowling alley; Juan runs dice out back | Craps (back lot, real money); free-play slots and video poker inside | $1-$20 | Anyone; sweeps likelier at high heat |
| Kasia's Tap | Backwater | Kasia's dad's old tavern at the mill gate (he was a businessman, see 4.1.8) | Poker, blackjack, "for amusement only" slots and video poker, pull tabs | $1-$50 | Bar open to all; back room by Kasia's invitation after two jobs (A1.3) |
| Bridgehead Fuel & Food | The Bridges | Gas station at the foot of the Long Bridge; "worst odds in town, best gossip" | Three-reel slots, video poker, pull tabs, scratchers | $1-$5 | Anyone |
| Starflare Club ("the Flare") | The Levee | Gus's rocket-age casino, slowly fading | Rocket slots, video poker, blackjack, craps, roulette, poker, keno, the Rocket Wheel | $5-$100 | Anyone, until the fall |
| Prairie Queen | The Levee | Old paddlewheel riverboat built "to haul prairie grain"; you are aboard until it docks | Blackjack, craps, roulette, paddlewheel slots, video poker | $5-$200 | A ticket for a set sailing |
| Cinder Room | The Levee | Umbra's after-hours game in an old mill basement under the Stacks (roughly 2am to dawn); the password changes | Casino war (house game), craps, no-limit poker, old one-armed bandits | $100 to no limit, cash only | Heat above a threshold or an Umbra vouch; Ray slips the first password (A1.9) |
| The RailYard | The Levee | Locals' casino in the old rail depot; "clean, busy, a little plain, with a buffet"; what the Flare could be if Gus ran it well | Rail slots, video poker, blackjack, craps, roulette, poker, keno; players' card; slot tournaments | $10-$500 | Best bankroll $1,500 |
| Harborlight | Marquee Mile | Lighthouse-themed hotel casino | Lighthouse slots, video poker, blackjack, craps, roulette, poker, keno | $25-$1,000 | $5,000; heat below a limit; smart casual |
| Aurelian | Marquee Mile | Out-of-town chain; "cameras everywhere, counters get backed off" | Big-screen slots, video poker, blackjack, craps, roulette, poker; baccarat in high-limit | $25-$5,000 | $10,000; low heat; high-limit at $25,000 |
| Obsidian | Silvershore | Members-only card room | Blackjack (high-roller side bets), poker (incl. seven-card stud), roulette, craps, slot salon, baccarat | $500 and up | $50,000; a suit; a member sponsors you; low heat |
| Yacht games | Silvershore | Private games on the lake; can't leave until the boat comes in | Poker, baccarat, craps | $1,000 and up | Vivian's invitation (A3.6) |

Note: `pages/gdd.md` and `pages/northlight.md` both say "Ten places to gamble" but list 11 rows (OQ 9).

**Excursions:** Northern Ledger (spring), Gilded Pine (summer), Haybale Downs (late summer to fall), Blackspruce Lodge (winter, once). Ice shanty camp is a parked later add-on.

**Story places** (`pages/northlight.md`):
- Slack Water Motel (Backwater; rocket sign visible from the lot; named 2026-10-17).
- Room 9.
- Gus's office, the Flare: the tape of Rosa's table; the framed photo of Dot and her sister.
- The Flare's lounge: Miles/Audrey; "covered piano as the Flare declines."
- Prairie Queen landing: Rosa's hangout.
- Old Levee bank: Gus's safety deposit box; bank not yet named.
- Bennett's office: location not yet named.
- Mitch's back room, Bridgehead.
- Aurelian front desk.
- Vivian's lakefront land: "on the Marquee Mile, right at the Silvershore line: licensed and on the lake."
- Lock Street Station (Ed's base; "Lock Street" doubles as slang for the lockup).

**Housing ladder:**
- Your car under the Bridges (broke).
- Slack Water Motel (start).
- A room over a bar in the Levee.
- A comped suite on the Marquee Mile (lost when you cool off).
- Silvershore (buy in, or stay as someone's guest).

**Landmarks:**
- The North Light, the harbor lighthouse the city is named for; "when it flickers, the house is running cold."
- The Long Bridge, "locals say don't carry winnings across before dawn."
- The Stacks, dead mill chimneys.
- The Starflare rocket sign.
- The spaceman, a life-size chrome astronaut statue by the doors; regulars rub his boot toe for luck.

**Slang and food** (`plan/t02.md`):
- "Riding the light" (hot streak).
- "Gone dark" (broke).
- "Going over the water" (to Backwater).
- Lake perch fry, steelworker pierogi, the Stacker (sausage-and-pepper sandwich).

**Places that appear only as the story moves:**
- Starflare Field (domed baseball stadium on the Flare's site).
- The lakefront Starflare.
- The Starflare afloat.
- Tae's restaurant inside the Aurelian.
- The burned shell.

## 4.3 Factions

- **Umbra.**
  - Old mob behind the pre-legal back rooms; "now skimming through loans, vending and security."
  - Name history (`plan/t02.md`): "the Families," then Umbra; briefly Argus, then Revaris and the Black Hand on 2026-10-23; back to Umbra 2026-10-25 by Mike.
  - Under Wren; Ray answers to her. Old guard and uncles resent her.
  - Runs the Cinder Room. Ties to Ray and the mob-takeover ending.
- **Revaris Technologies.**
  - Wren's AI-camera company.
  - "A respectable-looking tech firm." Umbra's secret front. The link is the secret Gus hides (M2) and the box proves (M4).
  - Buys the Flare and puts its land into the District, becoming "the District's biggest investor."
- **The Machine.**
  - The mayor and ward aldermen "trading licenses and permits for votes and payoffs."
  - Bennett (the Levee alderman) matters most to the Flare; Mayor Dabareski heads it.
  - Champions the "Safe Streets" contract.
- **The Levee Stadium District.**
  - Named by Mike 2026-10-23.
  - Holds the Flare's site and, via the ordinance Bennett sponsored, the Starflare name.
  - Building Starflare Field.
- **The Unions.**
  - Dealers' and dockworkers' locals; tied to Rosa. The mill local that rented Kasia's dad's back room.
  - "The union doesn't invest (conflict of interest)."
- **New money.**
  - Out-of-town chains on the Marquee Mile (the Aurelian, Tae).
- **Old money.**
  - Silvershore landowners (Vivian's family).
- **Northlight vice squad.**
  - Lock Street Station; Ed. Uses Revaris plate readers.
- **The gaming commission.**
  - Judged Rosa's payroll night; hears the A5.7 license hearing; can void a name clause on a conflict of interest.
- **Backwater** (mill families).
  - Voted down casinos and Revaris cameras.
- **The Levee** (old downtown).
- **Silvershore.**
- **The Long Bridge hoodlums.**
  - Unaffiliated; "No ties to any organization."
- **The Aurelian chain.**
  - Corporate pushes Tae to take the Revaris cameras. Keeps its own footage archive.
- **The regulars.**
  - Scatter at the fall; pool shares for O4.
- **The Tuesday league.**
  - Millstone bowlers.

## 4.4 Objects

- **The rocket sign.**
  - Over the Flare. Stays on the empty shell until demolition; then District storage on site until the Field gate is built. The O9 fire takes it. Tae (O5) takes it from storage or the gate.
  - The player's relit sign is always brand new.
- **The spaceman statue.**
  - The Flare's lucky object. S3 saves it for the new lobby. The fin is gone (audit 1.6).
- **Gus's dad's Starflare dinner jacket.**
- **Gus's chipped rocket-logo coffee mug.**
- **Pete's gold 25-year rocket pin.**
  - From Gus's father; he gives it to you (A3.4); returning it hires him back.
- **Rosa's torn patch.**
- **Two tapes.**
  - The wide-angle tape of Rosa's table (Gus's office; Bennett's hands after A3.5).
  - Gus's recording of Bennett's demand (in the box, the only copy).
- **The box and key.**
  - Rented in Richard's name; opens only with his signature; the key on a lanyard.
  - Holds the original signed camera contract and the recording.
- **Raffy's envelope.**
  - About $2,000 and a thumb drive with two files (face-match logs; Wren's payers list). (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
- **Gus's IOU** (S6) and "Gus's markers."
- **Photos.**
  - The band photo by the lounge.
  - The photo of the two sisters in Gus's office.
  - The photo of Richard on Bennett's desk.
- **Kasia's ledger.**
- **Kasia's dad's union pin.** Dropped 2026-10-28 (see 4.1.8).
- **Dot's register.**
- **The car.** Can't be sold.
- **The Starflare Field ordinance.** Holds the name clause.
- **The filing lawyer's trust account.**
- **Revaris cameras and the "Safe Streets" plate readers.**
- **The Prairie Queen's sister boat.**
- **The Cinder Room password.**
- **Vivian's pearls.** Copies, a later reveal.
- **The "lucky spaceman" bobblehead.** S12.
- **Gus's lucky chip.** Named as an inventory example only (OQ 39).

## 4.5 Money and economy

- Start: about $200 and a car (`pages/gdd.md`).
- **Best bankroll.**
  - The most of your own money ever held: cash minus debts. Only goes up.
  - Gates venues and acts.
- **Bankroll now.**
  - Cash you can spend. Broke content opens when low.
- **Debts** (shown in Wallet): Ray's loan and due date; Kasia's ledger; Flare markers; what Jalen owes you.
- **Gate numbers.**
  - $1,500 / $5,000 / $10,000 / $25,000 / $50,000 / $250,000 (see 2.2).
  - $25,000 also opens the Aurelian high-limit room and the Gilded Pine.
- **Rebuild.**
  - Vivian's land ~$1M; sign ~$250k; floor ~$1M; tower ~$2M; launch pad ~$1.5M.
  - Backers Jalen and Vivian; you pay at least a quarter of every phase.
- **Jobs.**
  - Honest $40-$150 a shift; trusted repeat up to about $250; crooked $200-$400.
  - Table (`plan/t13.md`, proposals unless noted):
    - Lane shift (Juan) $50
    - Bar-back (Kasia) $60 cash
    - Motel rooms (Dot) a night off rent
    - Valet (Pete) $80 plus tips
    - Drive Vivian $150
    - Envelope run (Ray) $200
    - Count the bets (Mitch) a cut
    - Run bets (Mitch) $200
    - Steer a mark (Cinder Room) $250 finder's fee
    - Sell information $200-$300
    - Collect for Ray $400
- **Small sums in the story.**
  - Raffy's tip $50; wedding $80 plus tips.
  - S1 $40; S2 $30; S3 $20.
  - Race slips; Troy's buy-back window ("till Thursday").
- **Credit.**
  - Kasia's ledger; Ray's loans; Flare markers.
- **Heat.**
  - A fine, a night in Lock Street lockup, or lost table money at a raid.
- **Monetization.**
  - No chips for sale. Premium or demo.
- **Pockets.**
  - 12 slots plus the story-key row.
  - Troy buys items "for a fraction."

## 4.6 Time system

- **Parts of the day:** morning, afternoon, night, plus a before-dawn window a few events use (`pages/gdd.md`).
- **Clock rules:**
  - "Time runs on what you do, not the real clock (settled 19 Oct 2026)."
  - Each round advances the clock a few minutes, but only to the end of the current part of the day; after that play is unlimited.
  - Days pass only when you move, sleep, travel or take a job.
  - App closed: no in-game time passes.
- **Blackjack hand = 5 in-game minutes** (Mike, 2026-10-26, after the prototype; `pages/gdd.md`, `pages/changelog.md`). That is 0.2 hands per in-game minute. "A six-hour part of the day fills after about 72 hands" (`pages/changelog.md`). Other games' per-round clock cost: "a few minutes," not given (OQ 38).
- **Real-time round lengths:** slots ~5s, video poker ~10s, blackjack ~20s, craps ~20s, roulette ~20s, poker 1-2 min.
- **Day length:** about 15 minutes of normal play (starting value).
- **Season:** 28 days, about 7 hours (starting value). Four seasons a year (six seasons is "roughly a year and a half in game").
- **Seasons gate:** the excursions, a few beats, and the Field timer.
- **Table events:** every 40 to 80 rounds, at most two or three per sitting.
- **Break points:** getting up, entering or leaving a venue, waking, crossing the river, finishing a job, start of a new day or season.
- **Heat cooldown:** one step per week of licensed-only play.
- **Wait rules:** seeds reveal after at least a week of in-game time; "a season after the bailout" = 28 days.
- **Field timer:** see 2.2.

## 4.7 Cameras, heat and Wren's network

(Settled 26 Oct 2026; `pages/gdd.md`, `plan/t02.md`, `people/wren.md`)
- Revaris cameras sit in casinos, bars, shops, motels and parking lots. The city's "Safe Streets" contract puts Revaris plate readers on streets and bridges.
  - Bennett championed it; the mayor signed it; Ed's squad uses it.
- "Every venue keeps its own footage. Wren can wipe her own servers but never a venue's copy." City readers feed Revaris directly.
- Dead zones: all of Backwater (voted the cameras down) and the Long Bridge (hoodlums smash the readers).
- The Aurelian ran its own system until act 2, when it takes Revaris cameras (S13).
- The Flare, the RailYard and the Prairie Queen take the camera deal.
- Heat has four steps: none, low (they know you), medium (warnings and sweeps), high (raid).
  - Raised by illegal games, crooked jobs, dirty act 4 routes.
  - High heat closes the Mile and Silvershore doors and opens the Cinder Room's.

## 4.8 Other established facts

- Gus's debt is personal and off the Flare's books (audit 3.8).
- Gus told his debt-free story to regulars; A3.1 breaks the lie (audit 3.11).
- The commission saw only the overhead camera; Gus kept the wide-angle tape (audit 3.7).
- Bennett keeps the tape as insurance (audit 3.4).
- Wren pays Bennett's cut herself through Revaris (audit 3.1).
- The O4 stepping-stone rule (audit 2.4).
- Pete started at the supper club (audit 1.7).
- The sale is public; the secret is the Revaris-Umbra link (audit 2.2).
- By-the-book proof: the filing lawyer's trust account (audit 4.3).
- The drive's two files and the clean/dirty handling (audit 4.6). (Superseded 2026-10-28, see 4.1.8 and `suggestions/S-058`.)
- Vivian is glimpsed in act 2 and met in act 3 at the Gilded Pine (audit 4.5).
- The yacht invite comes with Vivian's Obsidian sponsorship; Ray invites you to Blackspruce (audit 4.4).
- The District never sells to Gus's side; Tae is a neutral buyer (audit 4.7).
- Gus arranged room 9 through Pete; Bennett took over the bill (audit 4.8).
- The Cinder Room password comes from Ray in act 1 (audit 4.2).
- Seed S11's meeting (audit 4.1).
- Graham/Claire: only the meet needs the spring train (audit 1.10).

---

# 5. OPEN QUESTIONS

Format: the conflict or gap, quotes, and where each appears. Items 1 to 23 are pages that still disagree with the audit decisions or with a live page. Items 24 onward are contradictions and gaps inside the live material.

## 5A. Pages that still disagree with the audit decisions or with live pages

**OQ 1. Jalen "head of the hotel" vs "runs the poker room, then the hotel" (audit 2.9).**
- **Resolved (Mike, 2026-10-28):** head of the hotel; front of house on opening night. He studies hospitality and is not staked. See 4.1.8 and `suggestions/S-029`.
- `pages/storymap-open.md`: "Jalen becomes head of the hotel and Vivian stays a silent partner."
- `plan/t07.md` Rebuild phases: "only Jalen (later head of the hotel) and Vivian (silent partner)."
- Versus audit 2.9 and `pages/storymap-beats.md` A5.2: "Jalen invests and runs the poker room (the hotel once the post-game tower opens)."
- Versus `data/story.js` `C` Jalen line: "later runs the new Starflare's poker room, then the hotel."
- Also stale: `pages/notebook.md` (story-map): "Jalen (becomes head of the hotel)."

**OQ 2. `pages/direction.md` contradicts settled decisions.**
- "built in the 1960s at the height of the space race" (also "1960s dream of the future"). Audit 1.8 says the "history pages" were left as they were. `plan/t01.md`: the game "never names a year."
- "one of the first land casinos, built in the 1960s". `plan/t02.md`: the Flare was "a rocket-age supper club" that became "a licensed casino" when land casinos came.
- "where a river flows into the lake and the river is the state line." `plan/t02.md`: "same city and state; changed 2026-10-17"; `pages/gdd.md`: Backwater is dry "by local vote."
- Open-question list still asks: "What is the city called, and what is its era: a period setting such as the 1980s or early 90s, or present day?" `plan/t01.md` and `plan/t02.md` answer both.
- "Dutch's territory" and Teddy in the draft cast (renamed Ray and Chase/Brooke).
- The outcome list ("Closes for good", "Condemned", "Burns down", "Taken over by organized crime") does not match the ten settled outcomes.
- "Considered and set aside ... Last Week of the Starlight". The name Starlight was replaced by Starflare (`plan/t03.md`).
- Whether this page is meant as historical or live is not marked on the page.

**OQ 3. `plan/t09.md` JSON example is stale (audit 1.6).**
- `"id": "trucker_saves_fin"`, `"scene": "ink:seeds.trucker_saves_fin"`, `"effects": { "inventory_add": "rocket_fin", ... }`.
- The audit: "The fin is gone. The lucky object is now the spaceman." `pages/gdd.md` uses `trucker_saves_spaceman`. The S3 row in `plan/t09.md` is already updated.
- Whether the spaceman is a Pockets item or stays a statue is not stated (S3 "stands in the new Starflare's lobby").

**OQ 4. `pages/review.md` carries superseded details.**
- "The rebuild costs ($250k to free Gus, ~$1M for Vivian's land, $250k for the rocket)." Audit 2.8: "Gus freed" removed everywhere; the $250k is a gate in savings, not a price.
- "the people's Starflare needs the unions, the mob takeover needs Revaris, the lakefront Starflare needs old money." `pages/storymap-open.md`: "The union doesn't invest (conflict of interest)." `plan/t07.md` O4: "no union stake."
- "The four power blocs ... Revaris (Wren, Ray)". `plan/t02.md` naming rule: the outfit is Umbra to everyone.
- "Help push Revaris's loaded dice out of Millstone Lanes." Naming rule: Umbra.
- "Wren's people should know your reads if you've been careless on her app." Wren's betting app was replaced by cameras (2026-10-21).
- "Dot's four stories (the guest who paid everyone's week, the flood, room 9, the shirt)." The suitcase story was dropped 2026-10-22.
- `pages/review.md` presents these as proposals, but it is on the site as a page.

**OQ 5. `pages/notebook.md` carries superseded details** (the page says it records planning "word for word").
- "act 4 mystery solved + $50k; act 5 Gus freed + $250k"; "~40 hours to opening night." Current: name won back; about 42 hours.
- "Under the old bosses it was the BLACK HAND; Wren renamed it Revaris after her front company." Dropped 2026-10-25.
- "now run by a late-20s female tech boss Wren." Now early 30s (about 32).
- "Starflare rocket sign (tap the fin for luck)"; "the trucker saves the rocket fin". The fin is gone.
- The Gilded Pine: "Silvershore invite or $25k once." The invite unlock was dropped (audit 4.5).
- Bennett as the buyer, "as long as Gus owes Bennett", "Bennett frees Gus". Superseded 2026-10-23.
- "Dot's stories: the guest who paid everyone's week (founder's back room)." Dropped.
- Rosa "ultimately the player gives her a job at the reopened Starflare." Now the license is her payoff.
- Dutch/Teddy are explained in context and flagged in the character entries.

**OQ 6. `people/kasia.md` Hook is stale.**
- Planning record: "First credit and first friend; mentions the Flare on night one."
- `plan/t05.md`: "Kasia doesn't send you to the Flare; she'd rather keep you for her back room (Mike, 2026-10-23)."
- `data/story.js` A1.2: "She doesn't send you anywhere else."
- `pages/storymap-beats.md` A1.3: Juan "tells you about the Flare."

**OQ 7. `people/chase.md` hangouts disagree.**
- Header: "Core · moves up and down the ladder · Bridgehead, RailYard, Starflare, Harborlight."
- Planning record: "Hangouts: Moves up and down the ladder: Bridgehead, Starflare, Harborlight."
- `data/story.js`: "RailYard, Harborlight; moves up and down the ladder."
- `pages/storymap-beats.md` A1.8: the rival is first met at the RailYard; A2.4: a regular at the Harborlight.
- Whether Chase/Brooke appears at Bridgehead and the Flare is not clear.

**OQ 8. `plan/t06.md` and `pages/cast-planning.md` Already-decided text uses stale names and a stale gap list.**
- "Draft cast: Rosa (blacklisted former dealer), Dutch (loan shark), Teddy (the other newcomer) ..."
- "Not yet covered: Stella, Jalen, Juan, Raffy, Kiran, Mayor Dabareski, Richard and the romance characters." Voice sheets now exist for all but the romance characters (which are marked "Voice (proposal)").
- Both pages say "That's 9 core and 10 supporting" but Troy and Richard are not counted (see OQ 41).

**OQ 9. `pages/gdd.md` Venues: "Ten places to gamble" lists 11 rows.**
- Same wording in `pages/northlight.md`: "Ten places to gamble." The RailYard was added 2026-10-19 (`plan/t03.md`).

**OQ 10. What happens to the old rocket sign is described several ways.**
- `pages/storymap-beats.md` A5.4: "the old sign stays over the Starflare Field gate."
- `data/story.js` A5.4: "The old sign stays wherever the story left it."
- `plan/t07.md`: O5 Tae "takes from the District's storage yard or the Starflare Field gate"; O9: "the rocket sign, still on the shell, burns with it."
- `pages/storymap-branches.md`: "the old sign's fate (stadium gate, fire, or Tae's restaurant in outcome 5) is flavour only."
- `pages/northlight.md` Landmarks: "Stays on the empty Flare until demolition, then in storage until it goes over the Starflare Field gate; lost if the Flare burns first." Places list: "Starflare Field: ... with the restored rocket sign over its gate."
- `plan/t07.md`: "The restored rocket sign hangs over the gate of Starflare Field."
- Is the sign "restored" or "old"? Who restores it? When does demolition happen in relation to the last night and the Field timer? Not stated.

**OQ 11. `pages/storymap-open.md` has an unticked box for a fixed item.**
- "[ ] Rosa's license snag is fixed (2026-10-22)."

**OQ 12. Blackspruce Lodge and yacht-game unlocks are described inconsistently.**
- `pages/gdd.md` and `pages/storymap-intro.md` money table: "$250,000 | Blackspruce Lodge and yacht games fill the climb."
- Same pages: "The Cinder Room and the yacht games open by heat and invitation, not money."
- `plan/t04.md` (audit 4.4): Blackspruce is "Ray's invitation in your first winter as a Cinder Room regular."
- `data/story.js` X4: "Winter, once; Ray's invitation once you're a Cinder Room regular."
- Is Blackspruce gated by $250,000 at all? The invitation timing (a first winter as a Cinder Room regular, an early-game state) cannot match a $250,000 best bankroll.
- Yachts: the GDD venue table says "Vivian's invitation (A3.6)", but the money table lists yachts at $250,000.

**OQ 13. Wren's motive and terms for the camera deal differ.**
- `pages/room9.md` step 1: "Wren's security company offers cheap AI cameras plus cash up front... Gus signs knowing the company is Umbra's front, because it keeps the doors open."
- `people/gus.md`, `plan/t08.md`: "when he refused she showed him Cinder Room footage of his gambling and threatened to send it to Stella and the commission, so he signed knowing it was Umbra."
- `people/wren.md`: "at a price well below their worth, because the real payment is the data." Notebook: "NOT 'cheap'".
- `pages/room9.md` never mentions the blackmail; "cash up front" appears nowhere else.

**OQ 14. Kiran's "Umbra, maybe" tie.**
- `people/kiran.md` header: "Supporting · Umbra, maybe · the Prairie Queen"; Planning record "Ties to: Umbra, maybe."
- M3 says Ray (Umbra) makes him skim and "Umbra collects by having him skim." Whether Kiran is part of Umbra or only owes Ray is not settled.

**OQ 15. Wren-Bennett edge wording.**
- `data/story.js` `E`: "owns him: his hidden cut, on camera."
- Audit 3.1 and `people/wren.md`: "She pays his hidden cut of the stadium deal herself, through Revaris, so her own books are the proof"; but she "has dirt on Bennett taking casino money" (from cameras).
- The edge text implies the hidden cut is on camera; the fixed text says the books are the proof.

**OQ 16. `data/story.js` `C.appears` lists disagree with `EV`/`SEEDS`.**
- Kasia lists S11; Juan lists S11; Gus lists S11. `SEEDS` S11 `who` is `["dot","ray"]`.
- Gus's list omits A4.4 (`EV` includes him).
- Miles's list omits M1 (`EV` M1 `who` includes miles; `EVD` R7 says they saw the tape carried out).
- Felix's list omits M2 (`EV` M2 `who` includes felix).
- Pete's list includes M4 but `EV` M4 `who` does not.
- Richard's list includes A5.7 but `EV` A5.7 does not.
- Vivian's list includes A2.4; `EV` A2.4 `who` is `["chase"]` only (`EVD` mentions her).

**OQ 17. First in-person appearance of Ray, Wren and Bennett is not stated.**
- Ray's first coded appearance is A1.5, yet in A1.5 he only "starts asking around"; he is "hanging around the Flare" at A1.9. Hangouts "Starflare bar, Cinder Room, Blackspruce Lodge."
- Wren's first listed code is A2.2, but she is "rarely seen"; her first on-screen scene is not stated.
- Bennett appears in A2.2 only as a name on the sale; A2.6 is his "first good look."

**OQ 18. Felix/Nora's first meeting vs the Aurelian's gate.**
- `people/felix.md`: "First met: the Aurelian's front desk, act 1 or 2."
- The Aurelian opens at best bankroll $10,000 (A2.5, act 2) (`pages/gdd.md`, `pages/storymap-intro.md`). Act 1 closes at $5,000.
- Whether the desk can be reached earlier than $10,000 is not stated. (Graham's first meet needs the spring train: Bennett's invite or $10,000.)

**OQ 19. `people/mitch.md` Related line is unclear.**
- "Raffy (Wren files Mitch's slips)." Nothing else ties Raffy to Mitch; `data/story.js` has no Mitch-Raffy edge. `plan/t09.md` S10: Wren's file on you includes "Mitch's slips."

**OQ 20. Dot's voice vs her events.**
- `people/dot.md` voice sheet: "Gossip about everyone; never talks about Gus."
- `pages/storymap-beats.md` A1.7 / `data/story.js` R1: "Dot and Gus each ask you how the other is doing."

**OQ 21. Event "Where" and cast mismatches in the act tables.**
- `pages/storymap-beats.md` A3.5 Where: "Aurelian, RailYard, Kasia's Tap, Cinder Room." `data/story.js` A3.5 where: "Aurelian, RailYard, Kasia's"; the description never mentions the Cinder Room.
- `data/story.js` `EVX` A3.4: "Tae (offstage)"; `EV` A3.4 lists `tae` among its characters, and Tae's `appears` list includes A3.4.
- `pages/storymap-beats.md` A3.7 Where: "Mitch's, room 9"; `data/story.js`: "Bridgehead, Slack Water Motel."

**OQ 22. Ed's station placement is a proposal on one page and fact on another.**
- `pages/northlight.md`: "Placement (proposal): the Levee, beside the old canal lock on the river."
- `people/ed.md`: "Works out of the Northlight vice squad at Lock Street Station (Mike, 2026-10-23), next to the old canal lock."

**OQ 23. "Taught Bennett" line status.**
- `pages/room9.md` "Still open": "the 'taught Bennett' line is a proposal."
- `people/richard.md`, `plan/t08.md`: "It's the game he taught Bennett as a boy" as fact.
- Also `pages/room9.md` labels "Proposal, Ed's part" while `people/ed.md` and `plan/t08.md` state it as settled.

## 5B. Contradictions and gaps inside the live material

**OQ 24. Raffy: skimmed vs stole.**
- `people/raffy.md` Backstory: "he lifted a fat envelope from Ray's car to cover her rent, thinking it was only collection cash."
- Same page, Planning record: "he skimmed to cover her rent, then panicked"; Hook: "He skimmed from a collection and panicked."
- `data/story.js` `E`: "stole the wrong envelope from his car."
- Also the drive is "Ray's secret insurance copy," yet the Planning-record Hook still says the drive holds "face-match logs from Wren's cameras and a list of who pays her to stay quiet, which is where her leverage comes from."
- `people/raffy.md` Planning record: "Ties to: Unknown."

**OQ 25. How the O2 mechanism works.**
- `plan/t07.md`: "The only way to win the name back is leverage traded for the name going back to Gus."
- O2: "you use the leverage to take the name for yourself (ruthless) or Gus has turned on you."
- Room 9 routes "return the name to Gus"; A4.11 "the commission ... voids the name clause"; A4.3 "Wren pulls the name out herself."
- Where the name goes after a void or a Wren pull (and who then owns it) is not stated. How the player takes the name "for yourself" is not stated.

- **Resolved (Mike, 2026-10-27):** the ruthless O2 is buying the name from Wren at A4.3 instead of returning it, at a price plus a favor. See 4.1.8.

**OQ 26. O8 vs O10 distinction.**
- `data/story.js` A4.5: "Taking it leads to the mob takeover (O8) or the sellout (O10)."
- `plan/t07.md` O8: "You take Wren's deal, or deep debt to Ray and a crooked lean." O10: "give Wren her loose ends (Gus's markers, to keep him quiet, or Raffy's thumb drive, the evidence)."
- `people/wren.md`: "(O8, or O10 if you sold Gus out for the payout)."
- Both outcomes involve handing Wren the same two items. What decides which one fires is not stated.

- **Resolved (Mike, 2026-10-27):** what you hand Wren decides it: the drive (evidence) leads to O8, Gus's markers plus the payout to O10. See 4.1.8.

**OQ 27. "Gus's markers" and how you hold them.**
- A4.5: "hand over Gus's markers (your IOU from S6 among them)." `people/wren.md`: "Gus's old markers and your IOU from S6."
- How the player comes to hold Gus's markers other than the S6 IOU is not stated. Gus's debt is cleared by the sale (`plan/t07.md`).
- `plan/t09.md` S6 says the IOU is repaid "if the name is won back"; the markers' status after A4.5 is unspecified.
- "Flare markers" also appear as player debts in the best-bankroll definition; whose markers these are is not stated.

**OQ 28. When the Field timer starts and what it measures.**
- `pages/storymap-open.md`: "it opens on the first Opening Day at least three seasons after the fall."
- `pages/storymap-beats.md` A3.5: "Ground breaks and the Starflare Field timer starts."
- `pages/storymap-open.md`: "once ground breaks, posters and the bar TV announce the date."
- "The fall" is not defined as a moment: A3.3 condemnation, the last night, or A3.5 ground-breaking. The table "3 seasons if the fall comes in summer, 4 in spring" depends on it.

**OQ 29. Timer length against act budgets (derived).**
- A season is about 7 hours; three seasons is about 21 hours of play. Act 4 is budgeted ~10h and act 5 ~8h.
- Whether the Field timer is expected to expire during a normal-paced act 4 (so O7 is a common outcome) or rarely is not stated. No source gives how the in-game days of the timer map to play time beyond the season arithmetic.

**OQ 30. Revaris vs Umbra as "biggest investor."**
- `people/wren.md`: "Revaris Technologies ... puts its land into the Levee Stadium District, making it the District's biggest investor."
- `people/gus.md`, `plan/t07.md`, `data/story.js` M2: "Umbra then puts the land into the Stadium District, which makes Umbra its biggest investor."
- `people/bennett.md`: "Revaris, as the District's biggest investor, insists the District hold the Starflare name."
- The notes shift between Revaris and Umbra as the investor; the naming rule says player-facing text never ties them before M2 is solved.

**OQ 31. O9 (burned down) sequencing and consequences.**
- `plan/t07.md`: "the empty building torched as a message before demolition; the rocket sign, still on the shell, burns with it."
- A3.3 condemnation and the last night on the floor are on the main line; O9 can happen in "Act 3 or 4."
- Not stated: whether the last night, the scatter and Pete's pin still happen; whether the Field and its timer still go ahead on the burned site; who holds the name afterward; and what "Carry on as normal" means for the main line.

**OQ 32. The mayor's race outcome.**
- `pages/storymap-beats.md`/`data/story.js` R6: "the epilogue's newspaper front page shows who won."
- Who wins, and what determines it, is not stated. `people/graham.md`: they "shape the epilogue headline about Bennett's mayor race."

**OQ 33. Stella's knowledge of Gus's gambling vs act 4.**
- `people/stella.md`: "She doesn't know he gambles, so the torn patch leaves her speechless"; voice: "never mentions gambling (she doesn't know until the torn patch...)."
- A4.2 (start of act 4): "Gus is gambling harder than ever at the Cinder Room ... With Dot and Stella you can help him quit."
- A4.7 (where the torn patch is resolved, "Once you've found the clues") has no stated order relative to A4.2.
- Also `plan/t08.md` M2 solve: "Clearing Stella brings her and Gus closer."
- Whether she learns of the gambling before A4.2 is not stated. Also: "When the truth comes out, she realizes what the mortgage was really for." Which truth is not stated.

- **Resolved (Mike, 2026-10-27):** Stella learns from the player, if the player chooses to tell her; otherwise at the torn patch. See 4.1.8.

**OQ 34. How the tape is recovered and when M1 resolves.**
- A4.7 (`data/story.js`): "With the tape and her file you can get Rosa's license back." `plan/t08.md`: "you get it back by trading for it or with the room 9 leverage."
- `people/miles.md`: only Miles/Audrey together help you get it back (private events at Bennett's fundraisers).
- `people/rosa.md`: "Her license, won back in any ending by solving M1." The earliest resting point (O9) can occur in act 3, before M1's slot in A4.7.
- Bennett's insurance trigger is Gus testifying about the room 9 squeeze (audit 3.4); M1's solve is "Pete and Gus testify." Whether M1's testimony triggers Bennett's use of the tape is not stated.
- Whether a player with no romance can recover the tape is not stated beyond "trading for it."

- **Resolved (Mike, 2026-10-27):** any act 4 leverage route can trade for the tape; the romance pair is only the easiest way. See 4.1.8.

**OQ 35. Gilded Pine and Obsidian timing.**
- A3.6: meet Vivian at the Gilded Pine "$25,000 (summer)"; the Obsidian opens at $50,000 "with a suit and her sponsorship."
- The Gilded Pine is summer only. If a player reaches $25,000 or $50,000 outside summer, whether the Obsidian can open before the Gilded Pine meeting (the sponsorship comes from Vivian) is not stated.

**OQ 36. The A4.3 "trade your silence" route versus the clean/dirty rule.**
- `data/story.js` A4.3 lists "Trade your silence to Wren for the Starflare name" as the base route, and "blackmail with the payers list" as a dirty version.
- Graham/Claire's observed choices: shaming Bennett "counts against (it's blackmail, however good the cause)."
- Why trading silence to Wren is a white-hat route is not stated.

**OQ 37. License hearing coverage.**
- `plan/t02.md`: "winning back the name lets it be licensed again wherever it reopens ... a boat on the water, or a union hall in the Levee), after a commission hearing."
- A5.7 is only scheduled before the lakefront opening night. No hearing beat exists for O3 or O4.
- Who holds the license authority (the alderman "holds the Flare's license"; "the Machine ... trading licenses"; the gaming commission hears A5.7; the state allows gaming in licensed districts) is not laid out.

**OQ 38. Per-round clock cost for games other than blackjack.**
- `pages/gdd.md`: "Each round advances the clock a few minutes (blackjack: 5, Mike 2026-10-26)."
- Slots, video poker, craps, roulette, poker clock costs are not given. The length of the day in in-game hours is not stated (only "a six-hour part of the day" in `pages/changelog.md`).

**OQ 39. "Gus's lucky chip."**
- `pages/gdd.md` and `plan/t13.md`: "Gus's lucky chip" is an example of a meaningful Pockets item. No other page defines it, who has it or what it unlocks.

**OQ 40. Dot's age and background.**
- `people/dot.md`: "Slightly older, sympathetic and in the know." Older than whom is not stated. Age is not given.

**OQ 41. Troy's portrait and ties; Richard and Troy in the cast counts.**
- `pages/gdd.md`: "about 19 (9 core, 10 supporting), plus Richard and the six romance versions"; Troy is not listed.
- `data/story.js` GINTRO: "Supporting roles with their own portraits (Ed and Kiran) and Troy, the pawn shop owner. The walk-ons and offstage figures, who have no portrait yet."
- `plan/t06.md` tier lists do not include Richard or Troy; `data/story.js` calls Richard "Supporting (story)" and Troy "Minor."
- Troy has no codes, ties, want or backstory.

**OQ 42. Pete's service years and the 25-year pin.**
- `people/pete.md`: "about 60"; the founder hired him "at 19"; "gold 25-year rocket pin from Gus's father." Audit 1.7 says "The 25-year pin still works."
- When the pin was given, and whether the founder's death falls before or after Pete's 25th year, is not stated. The Flare's license date is not stated either.

- **Resolved (Mike, 2026-10-27):** the founder died about fifteen years before the game, when Stella was ten; the pin came before that. Pete's exact start year stays derived. See 3.1.

**OQ 43. Backstory timing is vague.**
- `pages/room9.md`: "all a few years before the game starts" (the whole chain: camera deal, Richard's markers, squeeze, box, room 9).
- `people/wren.md`: camera deal "about four years before the game"; reins "about two years before."
- `people/dot.md`: Richard has lived in room 9 "the last couple of years."
- Payroll night: "Years ago" (`people/rosa.md`, `plan/t08.md`). Mitch's tip: "Years ago."
- Relative order of payroll night, Richard's markers and the camera deal is stated only as a chain: payroll night to commission penalty to camera deal (`plan/t08.md`). Richard's markers' date is not placed.
- The squeeze must fall between about four and about two years before the game, but "a few years" and "a couple of years" are the only figures.

- **Resolved (Mike, 2026-10-27):** settled order written into 3.1 (payroll night ~6 years before; camera deal ~4; markers and squeeze ~3; room 9 and Wren's reins ~2).

**OQ 44. Mr./Ms. box offers two options; also "proposed."**
- `plan/t05.md`/`pages/storymap-beats.md`: the register has "a Mr./Ms. box"; the notebook calls it "proposed Mr./Ms. box beside the name." `pages/gdd.md` states it as settled.
- Nothing is stated for a player who does not tick either box or who is neither (the rival's version and the first dance offer depend on it). A1.4 is also written around a male/female attendant pair (Mike's inclusive rule covers tone, not options).

- **Resolved (Mike, 2026-10-27):** the box offers Mr., Ms. and Prefer not to answer. See 4.1.8.

**OQ 45. Wren's grandfather and the uncles.**
- `people/wren.md`: "the old Umbra boss's granddaughter." `pages/room9.md`: "her grandfather, still Umbra's boss then." Name, whether he is alive and where he is now are not stated. The uncles' names and roles are not stated. `pages/walkons.md` lists "Umbra's old guard and the uncles" under A4.5, but A4.5 has no stated role for them.

**OQ 46. Two Aurelian meetings with Bennett.**
- `people/felix.md`: "The filing lawyer checks into the Aurelian the week the purchase goes through ... you see him with Bennett" (M2).
- `people/felix.md` exclusive route / A4.3: "Wren's people meeting Bennett" at the Aurelian (her showcase, S13).
- Two separate meetings (lawyer with Bennett; Wren's people with Bennett) are both said to occur at the Aurelian. The A4.3 meeting must postdate the camera install in act 2, but the dates of both meetings are not stated.

**OQ 47. Ray's knowledge level.**
- `people/ray.md`: "He's the go-between and never knows the whole plan."
- `data/story.js` A2.2: "Ray swears about Revaris, 'I just know a guy, never met him' (S11)"; M2: "Ray calls the company 'us'" (while you work off your debt).
- How much Ray knows about Revaris-Umbra, and why he says "us" if he doesn't know the whole plan, is not stated.

**OQ 48. A1.10 and A2.1 describe the same scene.**
- A1.10: "Ray walks into Gus's office with his roll of cash ... Gus's markers are due. This closes act 1."
- A2.1: "Ray collects ... Start of act 2. Ray presses Gus for what he owes."
- Whether these are one scene (the gate and the choice) or two events is not stated.

**OQ 49. Ownership of the new Starflare and how the player funds it.**
- A5.6: Gus "cuts the ribbon with you." Backers: Jalen and Vivian; "you pay at least a quarter of every phase." O4: the regulars hold shares paid back on opening night.
- Who owns the lakefront Starflare and what stake Gus, Jalen, Vivian and the player each hold is not stated.
- A quarter of the phases through opening night (land ~$1M + sign ~$250k + floor ~$1M) is more than the $250,000 act 5 gate (derived). The notes call the $250,000 gate "savings for the rebuild" only.

**OQ 50. How Wren's file reaches Backwater illegal play (S10 rule).**
- `plan/t09.md` S10: "she always has something on you by act 4, built from what her cameras caught (Raffy's cash, Ray's debt, Mitch's slips, illegal games)."
- `pages/gdd.md`: Backwater is a dead zone; Kasia's back room, Millstone's dice and room 9 are in Backwater.
- How her file captures Backwater illegal play is not stated.

**OQ 51. Gus's and Stella's surname.**
- `pages/walkons.md`: Mr. Hamilton is the founder. `people/gus.md`: "surname Hamilton if a story needs it." The audit text mentions "the Hamilton family company"; that phrasing was an audit suggestion and is not in the docs. Gus's and Stella's surname is "Not stated."

**OQ 52. Gus's trust mechanics.**
- `pages/storymap-open.md`: "same standing value every key NPC has, with no special system." A1.10's gate includes "Gus's trust" with no threshold given. The thresholds for NPC standing (e.g. `npc_standing_min: {"gus": 20}` in `pages/gdd.md`) are example values only.

**OQ 53. O6 needs a name that has been "won back."**
- O6: "You help Gus quit ...; nobody funds a reopening; the name retires." Way back: "A sober Gus comes back and asks you to reopen."
- A4.2 is at the start of act 4, before any name-return route has resolved. Whether the name has been won back when O6 happens, and who holds it, is not stated.

**OQ 54. A3.9 has no content.**
- `data/story.js` `["A3.9", ..., "", []]`: no location and no characters. `pages/storymap-beats.md` gives no location, choice or characters either; it is purely a gate.

**OQ 55. Cinder Room entry vs heat.**
- `pages/gdd.md`: "Heat above a threshold or an Umbra vouch; Ray slips you the first password (A1.9)." and "High heat ... opens the Cinder Room's [door]."
- Whether the player who entered via Ray's password in act 1 (low heat) keeps access, and what "the password changes" does for return visits, are not stated.

**OQ 56. The `p` and `r` fields in `data/story.js` `SEEDS` are undocumented.**
- Values like `p:1, r:3` appear to mean the act planted and the act revealed. S12 has `r:0`. The file does not say.

**OQ 57. Author-flagged open items still unresolved.**
- Music and sound direction; price point and demo length (`pages/gdd.md`).
- The job table's rows, the inventory list, the romance beats (marked proposals).
- Tune day length and season length once the prototype runs; prototype blackjack; write act 1 in Ink (`pages/gdd.md` Next steps).
- Voice sheets for the romance characters (proposal only) (`pages/notebook.md`, `pages/romance.md`).
- Unnamed locations: the old Levee bank; Bennett's office; the room over a Levee bar; the union hall (`pages/northlight.md`).
- The review's tells and companion system are labelled "Proposals from the review ... not yet settled" on each character page.
- Vivian's pearls-are-copies reveal has no event code; the "secret Aurelian land meeting" is a saved proposal.
- Jalen's act 4 repayment has no event code (`people/jalen.md`: "pays back his stake in act 4").
- A5.7: "one hard question" is not specified.

**OQ 58. Tae's age vs the house band and "through the wall."**
- `people/tae.md`: "he grew up hearing the Flare through the wall." Which wall (his family's home next to the Flare?) and when the house band played relative to Tae's childhood are not stated ("rocket-age").

---

# Appendix: counts and notes for the editor

- Character pages: 24 (`people/*.md`).
- Walk-on table rows in `pages/walkons.md`: 22 (21 live; 1 dropped: the suitcase guest). One row covers two people (Kiran's daughter and Raffy's mom).
- Other named or recurring figures from other files: Wren's grandfather (unnamed), Walt (placeholder, non-canon), the drunk regular, the Queen's floor manager, Ray's men/collector, Umbra men at room 9.
- Mystery codes: 4. Seeds: 13. Outcomes: 10. Side arcs: 10. Excursions: 4. Table events: 26. Acts: 5. A-code beats: 10 + 8 + 9 + 11 + 7 = 45.
- Open questions: 58 (numbered in section 5).
