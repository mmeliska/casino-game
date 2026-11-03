*Supporting · Backwater · Kasia's Tap on night one, later hiding at Millstone Lanes*

## Look

19, Filipino-American, skinny, oversized hoodie, cracked phone in hand, one earbud in, a delivery-bike helmet clipped to his backpack. Talks fast, laughs nervously.

## Portrait prompt

For Leonardo.ai or Adobe Firefly (Mike, 2026-10-30). Every pose is full body and standing, facing the viewer or turned toward the viewer's left (the character's own right), with nothing in the hands; anything a character holds is an insert. The how-to, the single-pose template and the negative prompt are on [Portrait prompts](#portraits). Each pose tag is also the image's file name and the scene's `# pose:` tag.

**Proposed, not settled:** hair; the hoodie's colour; the joggers and sneakers.

**Sheet prompt** (16:9, hoodie outfit):

```
Full-body expression sheet: 4 poses of one person, each in its own evenly spaced panel with clear gaps, no overlap. Standing, facing the viewer or turned toward the viewer's left, whole body in frame head to shoes, empty hands. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted colours, realistic proportions, expressive face and hands. Flat light-grey background. No text, no speech bubbles.
Raffy: a skinny 19-year-old Filipino-American guy, messy black hair, one white earbud in. An oversized grey hoodie with a cracked phone peeking from the front pocket, a backpack with a bike helmet clipped to it, baggy black joggers, beat-up sneakers.
Panels: 1) turned toward the viewer's left as if heading out the door, a nervous grin, one hand up; 2) laughing nervously, rubbing the back of his neck; 3) hood up, hunched, scared, glancing sideways toward the viewer's left; 4) calm and serious for once, hands still at his sides.
```

**Outfits** (paste one as `[CHARACTER]` in the single-pose prompt):

*Hoodie* (default):

```
Raffy: a skinny 19-year-old Filipino-American guy, messy black hair, one white earbud in. An oversized grey hoodie with a cracked phone peeking from the front pocket, a backpack with a bike helmet clipped to it, baggy black joggers, beat-up sneakers.
```

*Helmet on*:

```
Raffy: a skinny 19-year-old Filipino-American guy, messy black hair, one white earbud in. A bike helmet on his head, an oversized grey hoodie, a backpack, baggy black joggers, beat-up sneakers.
```

| Pose tag | Outfit | Pose line | Used for |
| --- | --- | --- | --- |
| `raffy_neutral` | Hoodie | turned toward the viewer's left as if heading out the door, a nervous grin, one hand up | A1.2, the envelope (the envelope is an insert) |
| `raffy_nervous_laugh` | Hoodie | laughing nervously, rubbing the back of his neck | Lying by talking faster |
| `raffy_hiding` | Hoodie | hood up, hunched, scared, glancing sideways toward the viewer's left | Hiding at Millstone Lanes |
| `raffy_last_payment` | Hoodie | calm and serious for once, hands still at his sides | "This one's mine." |
| `raffy_free` | Helmet on | grinning, one fist raised | His epilogue panel, debt-free |

## Insert prompts

For Leonardo.ai or Adobe Firefly. An insert is a square close-up panel. Portraits keep their hands empty, so whatever this character holds or hands over gets one. The how-to, the negative prompts (including the one for inserts with a hand in frame) and the settings are on [Plate prompts](#plates). Each prompt carries the light of the place where its scene happens. Each tag is also the image's file name and the scene's `# insert:` tag.

**Proposed, not settled:** the drive (a small black metal thumb drive); the lock screen.

### The envelope

Night one at Kasia's Tap (A1.2): "Hold this till Friday, don't open it." His hand is in frame; use the hand negative prompt.

`raffy_envelope` (insert, square 1:1)

```
Close-up insert panel. A skinny young Filipino-American man's hand, the sleeve of an oversized grey hoodie pushed back, sliding a fat sealed manila envelope across a worn wooden bar top. Night: amber sodium streetlight and warm bar neon. Palette: amber, brick red, tired green. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, one hand only, no face, no text, no lettering; labels blank.
```

### Inside the envelope

What's inside, if you open it: $2,000 and a thumb drive (A1.5 at the latest).

`raffy_envelope_open` (insert, square 1:1)

```
Close-up insert panel. A manila envelope torn open on a faded motel bedspread, a thick fold of plain banknotes and a small black metal thumb drive spilling out of it. Night: amber sodium streetlight and warm bar neon. Palette: amber, brick red, tired green. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, no people, no text, no lettering; labels blank.
```

### The locked drive

Plugged in, it shows a locked volume with Revaris's name and a serial number (A1.5). The name and the number are added in layout.

`raffy_drive_locked` (insert, square 1:1)

```
Close-up insert panel. A small black metal thumb drive plugged into the side of an old laptop on a faded motel bedspread, the screen showing a blank grey dialog box with a padlock icon. Night: amber sodium streetlight and warm bar neon. Palette: amber, brick red, tired green. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, no people, no text, no lettering; labels blank.
```

## Backstory

Grew up in Backwater. Runs envelopes for Ray's collections because it paid better than delivery and made him feel like somebody. His mom is a nurse working double shifts; he lifted a fat envelope from Ray's car to cover her rent, thinking it was only collection cash. It held $2,500. He took $450 for the rent (Mike, 2026-10-29). It also held something that wasn't cash (see the drive), and when he realized what he was holding, he panicked. A good kid making a dumb mistake.

## The envelope

On night one at Kasia's Tap (A1.2) he slides you a sealed envelope ("hold this till Friday, don't open it"), tips you $50 out of it, and never comes back. Inside: $2,000 and a thumb drive. Weeks later (A1.5) Ray asks around Kasia's and the Slack Water about "the envelope," and Dot warns you. Ray is $500 short even if every dollar you hold goes back (Mike, 2026-10-29).

A1.5 settles the cash and Raffy. The drive stays in your pocket whatever you pick (Mike, 2026-10-29):
- **Return it to Ray.** He gets $2,000 and lowers his interest on your loans. Raffy still owes him $500 and stays in hiding.
- **Help Raffy pay Ray back.** Find him hiding at Millstone Lanes with Juan. The $2,000 goes to Ray and you cover the missing $500. Raffy is clear and owes you $500.
- **Keep the cash.**
- **The lie.** Keep the $2,000 and tell Raffy, "It's settled. You owe me $500."

**What he owes you** (help path). He can't pay in one go, so he chips away at it: small amounts of cash left for you, and the delivery-bike helmet in Pockets ("in case"). The last payment is one scene at the Lanes, slowly, without talking fast: "That's all of it. The fifty too. The one I gave you that first night was Ray's. This one's mine." If you tell him to stop paying, he does, but the last scene still comes, with just the fifty. His epilogue panel is him on the bike, debt-free.

**The lie.** Raffy believes he's clear, so he stops hiding. Ray finds him within days, so you collect a payment or two at most. Raffy tells Ray, "They said they paid you." Ray comes to your table: "Buddy. We need to talk about an envelope." The $2,000 plus interest goes on your debt to Ray (the debt screen shows it as "the envelope"), and Ray never trusts you again. Raffy disappears. His mom calls Kasia's looking for him, Kasia tells you she said you were the last one he talked to, and Dot stops warning you about the bridge.

**Badly:** if you keep the $2,000 without lying, or tell Ray where he's hiding, Kasia tells you his mother called the bar looking for him, and that she told her you were the last one he talked to. Dot stops warning you about the bridge.

## The drive

Nobody trusted Raffy with it; he stole the wrong envelope (audit 3.6). It is Revaris's old encrypted key ledger: the only place an anonymous Revaris face ID is tied to a name, and the IDs it names are the ones the cameras are set never to see, Umbra's roster and the men who pay Umbra to be unseen. Ray enrolls those men at the Cinder Room for Wren. When she moved the ledger to a newer device she told him to destroy the old drive; he kept it as insurance, and carried it in his car with the collection cash. His own name is on it, so he can never admit it exists, and he only ever asks about "the envelope" (Mike, 2026-10-28).

Plugged in, it shows a locked volume with Revaris's name and a serial number. That's all you learn at A1.5: it is Revaris property, and it was in Ray's car with collection cash. No A1.5 text calls Ray Umbra's man; the player draws the line themselves (Mike, 2026-10-29). Plugging it into anything online makes it phone home: Wren learns a decommissioned serial is alive, but not who has it (Mike, 2026-10-29).

It opens only with the old key, the password Ray typed at every enrollment. He still has a copy. At least one other copy is shredded into the uncollected burn bag in his office closet, and you can steal the bag (by working off a debt to Ray, or on a collect-for-Ray job) and rebuild the key in the jigsaw mini-game, sorting Ray's handwritten scraps, old key and new, out of months of printed strips. The opened ledger's first page, names beside numbers with Ray's among them, is the proof in M2; a locked drive is only a hint (Mike, 2026-10-28, 2026-10-29). Once you've solved the puzzle you can always read it.

From act 2 on you choose its fate (Mike, 2026-10-29):
- **Ed** (clean, A4.3), opened or not. It is probable cause for a subpoena that makes Revaris show its cameras are blind to Umbra. Slow and loud; Umbra and the payers go down together.
- **Back to Ray** ("I read it" / "I looked"). He owes you, but Ed's subpoena and O8 are gone.
- **Wren** (A4.5). O8. She never knows whether it was read.
- **Keep it.** If you read it, or Ray opens it for you, the payers' names are a dirty route.

In Pockets it is a story key: "Raffy gave you this at Kasia's. He made you promise not to look." Spending it names him: "Hand Wren the drive Raffy gave you."

## Voice

Fast and run-on, one earbud in, always halfway out the door. Talks about deliveries, his bike and his mom's double shifts; never says what's in the envelopes or who they're for. Lies by talking faster. "Hold this till Friday? Don't open it. Don't lose it. Fifty bucks. Cool? Cool." (Settled 2026-10-23.) At the last payment he slows down for once: "That's all of it. The fifty too. The one I gave you that first night was Ray's. This one's mine." (Mike, 2026-10-29.)

### Voice sheet (settled 2026-10-18; write every line of dialogue to this)

| Rhythm | Says / never says | Lies by | Sample line |
| --- | --- | --- | --- |
| Fast and run-on, one earbud in, always halfway out the door | Deliveries, his bike, his mom's double shifts; never says what's in the envelopes or who they're for | Talking faster | "Hold this till Friday? Don't open it. Don't lose it. Fifty bucks. Cool? Cool." / "My mom works doubles at the hospital. Somebody's gotta make rent." (Mike, 2026-10-23) |

## Related

Ray, Kasia, Dot, Juan, Ed, Wren.

## Planning record

*From the cast table in task 6, with the date each detail was settled.*

**Raffy (the stranger: the courier; named 2026-10-17)**

- **Listed as:** Raffy (the stranger: the courier; named 2026-10-17) (Supporting)
- **Role:** Asks you to hold something on night one. The courier (settled 2026-10-17): a jittery kid who runs envelopes for Ray's collections. Who (settled 2026-10-17): 19 (Mike: 18 or 19), Filipino-American, grew up in Backwater; took the job because it paid better than his delivery gig and made him feel like somebody; his mom is a nurse working double shifts, and he skimmed to cover her rent, then panicked; a good kid making a dumb mistake. Look: skinny, oversized hoodie, cracked phone in hand, one earbud in, a delivery-bike helmet clipped to his backpack; talks fast, laughs nervously; at Kasia's to drop off, not to drink or play. at Kasia's on night one he slides you a sealed envelope ("hold this till Friday, don't open it") and tips you $50, then doesn't come back. You can open it anytime: about $2,000 in cash and a thumb drive. He skimmed from a collection and panicked; weeks later Ray is asking around Kasia's and the Slack Water and Dot warns you (Superseded 2026-10-29: he stole the envelope from Ray's car and took $450 of its $2,500 for rent; he didn't skim.)
- **Ties to:** Unknown (Superseded 2026-10-29: Ray.)
- **Hangouts:** Kasia's Tap at first, then hiding out at Millstone Lanes
- **Hook:** The first seed. Your choice: return it to Ray (reliable; a small Umbra favor; easier loans), keep the cash (crooked and risky), hand the drive to Ed (straight; starts the informant path), or find the courier hiding out at Millstone Lanes with Juan and help him pay Ray back. Reveal: the drive holds face-match logs from Wren's cameras and a list of who pays her to stay quiet, which is where her leverage comes from; the first thread into the bigger Umbra story. (Superseded 2026-10-29: A1.5 settles only the cash and Raffy, the drive's fate comes from act 2 on, and the drive is Revaris's old encrypted key ledger.)

## Proposals from the review

*From the 18 Oct 2026 Story and Character Review, not yet settled.*

- **Companion (proposal):** edge: A runner: gets messages and cash across the river, finds people; joins: Find him at Millstone Lanes and help him pay Ray; you lose them: Hand him to Ray, or keep his $2,000 (Superseded 2026-10-29: the runner is cut; Raffy appears through the repayments, the fifty and the lie.)
