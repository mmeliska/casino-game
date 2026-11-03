*Supporting · the Levee · the Starflare floor*

## Look

Stocky, a thin, neatly trimmed beard (not a mustache), short sandy-grey hair combed neat, a dark three-piece suit with the vest always buttoned, a gold 25-year rocket pin from Gus's father, reading glasses on his head, a rating card in hand.

## Portrait prompt

For Leonardo.ai or Adobe Firefly (Mike, 2026-10-30). Every pose is full body and standing, facing the viewer or turned toward the viewer's left (the character's own right), with nothing in the hands; anything a character holds is an insert. The how-to, the single-pose template and the negative prompt are on [Portrait prompts](#portraits). Each pose tag is also the image's file name and the scene's `# pose:` tag.

**Proposed, not settled:** the suit's colour, the tie and the shoes.

**Sheet prompt** (16:9, suit outfit):

```
Full-body expression sheet: 4 poses of one person, each in its own evenly spaced panel with clear gaps, no overlap. Standing, facing the viewer or turned toward the viewer's left, whole body in frame head to shoes, empty hands. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted colours, realistic proportions, expressive face and hands. Flat light-grey background. No text, no speech bubbles.
Pete: a stocky Lithuanian-American man about 60, a thin trimmed beard, no moustache, short sandy-grey hair combed neat, reading glasses pushed up on his head. A charcoal three-piece suit with the vest buttoned, a white shirt, a dark tie, a small gold rocket pin on the lapel, a rating card in the breast pocket, polished black shoes.
Panels: 1) hands clasped behind his back, a flat expression; 2) one finger pulling his glasses down his nose, looking the viewer over; 3) eyes down, jaw set, not answering; 4) alone and tired, one open hand held out palm-up, offering.
```

**Outfits** (paste one as `[CHARACTER]` in the single-pose prompt):

*Suit* (default):

```
Pete: a stocky Lithuanian-American man about 60, a thin trimmed beard, no moustache, short sandy-grey hair combed neat, reading glasses pushed up on his head. A charcoal three-piece suit with the vest buttoned, a white shirt, a dark tie, a small gold rocket pin on the lapel, a rating card in the breast pocket, polished black shoes.
```

*No pin*:

```
Pete: a stocky Lithuanian-American man about 60, a thin trimmed beard, no moustache, short sandy-grey hair combed neat, reading glasses pushed up on his head. A charcoal three-piece suit with the vest buttoned, a white shirt, a dark tie, nothing on the lapel, polished black shoes.
```

| Pose tag | Outfit | Pose line | Used for |
| --- | --- | --- | --- |
| `pete_neutral` | Suit | hands clasped behind his back, a flat expression | Default: "Kid." |
| `pete_appraising` | Suit | one finger pulling his glasses down his nose, looking the viewer over | Your name scene |
| `pete_eyes_down` | Suit | eyes down, jaw set, not answering | Lying by not answering; Rosa's name |
| `pete_pin` | No pin | alone and tired, one open hand held out palm-up, offering | A3.4, the dark pit (the pin is an insert) |

## Insert prompts

For Leonardo.ai or Adobe Firefly. An insert is a square close-up panel. Portraits keep their hands empty, so whatever this character holds or hands over gets one. The how-to, the negative prompts (including the one for inserts with a hand in frame) and the settings are on [Plate prompts](#plates). Each prompt carries the light of the place where its scene happens. Each tag is also the image's file name and the scene's `# insert:` tag.

**Proposed, not settled:** the rating card (a cream index card with ruled lines).

### The pin

The dark pit on the Flare's last night (A3.4): he hands you his gold rocket pin, and calls you "kid" once more. His hand is in frame; use the hand negative prompt.

`pete_pin_hand` (insert, square 1:1)

```
Close-up insert panel. An older white man's hand, a charcoal suit sleeve over a white shirt cuff, held out open, palm up, a small gold rocket-shaped lapel pin lying in the palm. Most of the lights are dark. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, one hand only, no face, no text, no lettering; labels blank.
```

### The rating card

Your name scene at the pit, card in hand. The card never shows numbers. His hand is in frame; use the hand negative prompt.

`pete_rating_card` (insert, square 1:1)

```
Close-up insert panel. An older white man's hand, a charcoal suit sleeve over a white shirt cuff, holding a small cream index card with blank ruled lines and a short pencil, against a casino pit podium. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, one hand only, no face, no text, no lettering; labels blank.
```

## Backstory

Lithuanian-American, about 60, the third son of a mill family in the Levee: his grandfather came over for the furnaces and his father kept the same job until they went cold. Pete was supposed to follow. At 19 he walked into the Starflare supper club in his brother's suit, and the founder, Mr. Hamilton, gave him a bus tray and a rule: count everything, say nothing. He has kept it for forty-one years and never worked anywhere else. He learned cards by watching, and when land casinos came and the Flare got its license he was one of the first men Mr. Hamilton handed a shoe. He dealt for years and has run the pit for decades. The only time he ever risked his job was telling Mr. Hamilton the band was being paid too much. At his 25th year, a few months before his own death, Mr. Hamilton gave him the gold rocket pin with a note that said only "You were right about the band." He started exactly where Jalen is now, so he keeps a quiet eye on the kid (audit 1.7, Mike 2026-10-24).

He was Gus's older brother in everything but blood: he taught him to count a rack, stood up at his wedding and held Stella at the cage while Alice counted the drop (his memory, not hers: Stella remembers only her mother there). The night Alice died, Dot called the Flare and Pete told her Gus had left after closing. After the funeral he carried Dot's one request to Gus, for her mother's recipe box, and carried back the no. Then he did what the rule said and kept the Flare running while Gus couldn't. He watched Rosa shuffle at a dealer-school audition, said nothing, and the Flare hired her; on payroll night he was at the pit and didn't look up.

**His family** (Mike, 2026-10-28; revised 2026-10-29). The year he got his first shoe he married Ona, a girl from two streets over. They have two kids: a grown daughter, and the youngest, Tomas, who has a chronic illness and lives at home. Ona stays home to take care of him, so Pete's is the only paycheck, and the medical bills are why he can't afford to retire. Ona has never set foot on the floor, by agreement: the Flare gets his nights and she gets his mornings. Stella grew up calling him Uncle because his kids called Gus the same. They are all offstage, with no portraits.

**What he knows.** The payroll night, and the room 9 arrangement, because he made it: he quietly arranged room 9 at Dot's motel for Gus, which is why Richard lives there; Dot only knows Pete asked a favor (M4, audit 4.8). The Cinder Room he suspects and has never been told (you can tell him, A1.9). The cameras and the key he knows nothing of. He is the only one who tells Gus hard truths, about the floor, the numbers and the band; the ones about the things Gus won't name, he keeps. He is loyal past the point where loyalty helps.

## Role

The Flare's face for comps, credit and higher limits; watches your hot streaks. He knows the regulars' ratings by heart, which is how you learn Walt's, Lorraine's and Augie's names. Tae keeps trying to hire him away. Caught between Gus and Stella, a quiet go-between for Dot and Gus, and your way in to Gus. He can speak for you at the license hearing (A5.7).

**Your name** (Mike 2026-10-28). Your rating is never shown on screen: the Flare tracks your sittings, and Pete's card is a prop with no numbers. After a few sittings at the Flare (tuned), a short scene at the pit: card in hand, the same line he's said before, with the name you wrote on Dot's register in place of "kid." That's the act 1 beat: you belong somewhere. Comps start the next visit. He uses your name from then on, and goes back to "kid" exactly once, at the dark pit when he hands you the pin. If you sold Gus out, he never says your name again. If Jalen is on the floor for the name scene, he says, politely, "He doesn't do that."

## Leaving at the fall

On the Flare's last night you find him alone at the dark pit (A3.4, one of the night's six scenes). He has taken Tae's offer: it's the first time in forty years the arithmetic has beaten the rule, Tomas's medical bills against a pit boss's pay, and he asks you to tell Gus, because it's the one hard truth he can't tell him himself. Tell Gus yourself, make Pete do it, or push him to stay (he can't, because of Tomas's bills); he goes either way (Mike, 2026-10-30). He hands you his gold rocket pin; if you didn't choose the pit, he finds you at the doors on your way out, shorter, and still hands it over. The pin is a story key: it can't be stolen or pawned. When you open a floor, giving it back hires him home, unless you sold out Gus. In O2, when Rosa stops at the door, Pete is the one who says her name.

## Voice

Short and flat, calls everyone "kid" until they earn their name; numbers and facts; never compliments Gus to his face. Lies by not answering. *"Kid. Your rating says you're a $50 player. Play like one."*

### Voice sheet (settled 2026-10-18; write every line of dialogue to this)

| Rhythm | Says / never says | Lies by | Sample line |
| --- | --- | --- | --- |
| Short and flat; calls you "kid" | Numbers and facts; never compliments Gus to his face | Not answering | "Kid. Your rating says you're a $50 player. Play like one." / Early, while you're still "kid": "Count everything, kid. Say nothing. Worked for forty years." / About Rosa: "You see her? ... Don't tell her I asked." (Mike, 2026-10-28) |

## Jobs (proposal)

Valet at night, $80 plus tips; the "Pete needs a hand" table event (proposal).

## Related

Gus, Stella, Dot, Tae, Jalen (started where Jalen is now), Rosa (his silence got her hired; goes quiet at her name).

## Planning record

*From the cast table in task 6, with the date each detail was settled.*

**Pete (Petras; the Flare's pit boss; named 2026-10-17)**

- **Listed as:** Pete (Petras; the Flare's pit boss; named 2026-10-17) (Supporting)
- **Role:** Runs the Starflare floor. The lifer and Gus's best friend (settled 2026-10-17): Lithuanian-American, about 60, grew up in the Levee alongside Gus; his grandparents and dad worked the steel mills, but Gus's father gave him his first job at 19, busing tables at the supper club (dealer once the license came, revised 2026-10-24), and he's never worked anywhere else. Best man at Gus's wedding, "Uncle" to Stella, kept the Flare running after the funeral, and the only one who can tell Gus a hard truth. Gruff, dry humor, calls everyone "kid" until they earn their name. Look: stocky, a thin, neatly trimmed beard (Mike, 2026-10-17), short sandy-grey hair combed neat, dark three-piece suit with the vest always buttoned, a gold rocket pin from Gus's father for 25 years, reading glasses on his head, a rating card in hand
- **Ties to:** The Flare
- **Hangouts:** Starflare floor
- **Hook:** The Flare's face for comps, credit and higher limits; watches your hot streaks; Tae keeps trying to hire him away; caught between Gus and Stella; a quiet go-between for Dot and Gus; earning his trust is your way in to Gus. Leaving at the fall (settled 2026-10-22): on the Flare's last night, after the floor closes, you find Pete alone at the dark pit. He has taken Tae's offer because he needs the money and can't afford to retire yet, and he asks you to tell Gus: he's the one who always tells Gus hard truths, and this is the one he can't. Your choice: tell Gus yourself (Gus's trust, a hard talk), push Pete to do it (Pete respects you, Gus takes it hard), or say nothing (Gus reads about it, Pete goes cool on you). Before he goes he hands you his gold rocket pin to keep for the Flare; when you open a floor, giving it back hires him home, unless you sold out Gus
