*Supporting · Ray · the Prairie Queen*

## Look

The Prairie Queen's old-time riverboat uniform (striped vest, sleeve garters, a bow tie always a little crooked), neat black hair, a wedding ring he turns when nervous (text only), a photo of his daughter tucked by his chip tray.

## Portrait prompt

For Leonardo.ai or Adobe Firefly (Mike, 2026-10-30). Every pose is full body and standing, facing the viewer or turned toward the viewer's left (the character's own right), with nothing in the hands; anything a character holds is an insert. The how-to, the single-pose template and the negative prompt are on [Portrait prompts](#portraits). Each pose tag is also the image's file name and the scene's `# pose:` tag.

**Proposed, not settled:** clean-shaven; the vest's colours; the plain clothes.

**Sheet prompt** (16:9, uniform outfit):

```
Full-body expression sheet: 4 poses of one person, each in its own evenly spaced panel with clear gaps, no overlap. Standing, facing the viewer or turned toward the viewer's left, whole body in frame head to shoes, empty hands. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted colours, realistic proportions, expressive face and hands. Flat light-grey background. No text, no speech bubbles.
Kiran: an Indian-American man in his early 30s, slim, neat black hair, clean-shaven, a gold wedding ring. An old-time riverboat dealer's uniform: a white shirt, a red-and-cream striped vest, black sleeve garters, a black bow tie a little crooked, black trousers, polished black shoes.
Panels: 1) leaning forward, a big friendly smile, hands open mid-joke; 2) upright, unsmiling, eyes down, hands flat at his sides; 3) tired, shoulders down, hands in his pockets; 4) turned toward the viewer's left, looking off into the distance, saying nothing.
```

**Outfits** (paste one as `[CHARACTER]` in the single-pose prompt):

*Uniform* (default):

```
Kiran: an Indian-American man in his early 30s, slim, neat black hair, clean-shaven, a gold wedding ring. An old-time riverboat dealer's uniform: a white shirt, a red-and-cream striped vest, black sleeve garters, a black bow tie a little crooked, black trousers, polished black shoes.
```

*No bow tie*:

```
Kiran: an Indian-American man in his early 30s, slim, neat black hair, clean-shaven, a gold wedding ring. A white shirt with the collar open and no bow tie, a red-and-cream striped vest, black sleeve garters, black trousers, polished black shoes.
```

*Plain clothes*:

```
Kiran: an Indian-American man in his early 30s, slim, neat black hair, clean-shaven, a gold wedding ring. A plain dark jacket over a grey T-shirt, jeans, worn sneakers.
```

| Pose tag | Outfit | Pose line | Used for |
| --- | --- | --- | --- |
| `kiran_smile_lean` | Uniform | leaning forward, a big friendly smile, hands open mid-joke | Default: "Welcome aboard!" |
| `kiran_upright_eyes_down` | Uniform | upright, unsmiling, eyes down, hands flat at his sides | His tell: he goes quiet on a fixed hand |
| `kiran_at_rail_no_bow_tie` | No bow tie | tired, shoulders down, hands in his pockets | Confronted on deck |
| `kiran_looks_away` | Plain clothes | turned toward the viewer's left, looking off into the distance, saying nothing | Called out: at the landing |

## Insert prompts

For Leonardo.ai or Adobe Firefly. An insert is a square close-up panel. Portraits keep their hands empty, so whatever this character holds or hands over gets one. The how-to, the negative prompts (including the one for inserts with a hand in frame) and the settings are on [Plate prompts](#plates). Each prompt carries the light of the place where its scene happens. Each tag is also the image's file name and the scene's `# insert:` tag.

**Proposed, not settled:** the chip colours; the daughter's photo (she's offstage, with no portrait).

The color-up clue is a spot-the-difference nobody points out, so the two racks must match except for one chip: make `kiran_rack_after` from `kiran_rack_before` as a reference image, and paint out one chip by hand if the generator changes anything else. Five tags come from the [sample scene](#writing) as written there. Where his hand is in frame, use the hand negative prompt.

### The photo by the tray

His daughter's photo, tucked by his chip tray, facing out (the sample scene). After the walk-away ending it's still there at a new dealer's table. Use the photo negative prompt.

`kiran_chip_tray_photo` (insert, square 1:1)

```
Close-up insert panel. A dealer's chip tray of neat coloured chip stacks at the edge of a green blackjack table, a small worn photograph of a smiling seven-year-old girl tucked upright at its end, facing out. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, no text, no lettering.
```

### The photo turned

His tell on the hand where he palms a chip: the photo turned away (the sample scene). Make it from the photo by the tray.

`kiran_chip_tray_photo_turned` (insert, square 1:1)

```
Close-up insert panel. A dealer's chip tray of neat coloured chip stacks at the edge of a green blackjack table, a small photograph tucked upright at its end, turned around so its plain white back faces out. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, no people, no text, no lettering; labels blank.
```

### The rack before

The color-up clue, before. In the helped ending, a season on, this panel is shown twice, before and after, and they match.

`kiran_rack_before` (insert, square 1:1)

```
Close-up insert panel. Looking straight down into a dealer's chip tray set in green felt: ten neat columns of chips in red, green and black, every column the same height. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, no people, no text, no lettering; labels blank.
```

### The rack after

The color-up clue, after: one chip gone. Make it from the rack before.

`kiran_rack_after` (insert, square 1:1)

```
Close-up insert panel. Looking straight down into a dealer's chip tray set in green felt: ten neat columns of chips in red, green and black, one column a single chip shorter than the rest. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, no people, no text, no lettering; labels blank.
```

### Four chips, not five

The short payout (the sample scene). The results strip carries the numbers; this panel only shows the chips.

`payout_four_chips_not_five` (insert, square 1:1)

```
Close-up insert panel. A short stack of four red casino chips pushed onto a betting spot on green blackjack felt beside two playing cards. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, no people, no text, no lettering; labels blank.
```

### His hand on the felt

Called to the pit (the sample scene): his hand goes flat and still.

`kiran_ring_hand_flat_on_felt` (insert, square 1:1)

```
Close-up insert panel. An Indian-American man's hand with a gold wedding ring, a white shirt cuff and a black sleeve garter above the wrist, pressed flat and still on green blackjack felt, a chip tray behind it. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, one hand only, no face, no text, no lettering; labels blank.
```

### The vest pocket

Betting again to watch where the chips go (the sample scene).

`kiran_palm_to_vest_pocket` (insert, square 1:1)

```
Close-up insert panel. An Indian-American man's hand with a gold wedding ring, a white shirt cuff and a black sleeve garter above the wrist, slipping a single casino chip into the small pocket of a red-and-cream striped waistcoat. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, one hand only, no face, no text, no lettering; labels blank.
```

### The envelope at the landing

After a sailing, he passes an envelope to one of Ray's men at the landing. Two hands; use the hand negative prompt.

`kiran_envelope_handoff` (insert, square 1:1)

```
Close-up insert panel. Two hands passing a thick plain envelope at a riverboat landing at night, river mist: an Indian-American man's hand with a gold wedding ring, a white shirt cuff and a black sleeve garter above the wrist, and a man's hand in a dark jacket sleeve. Wet planks and a mooring rope below. Night: teal and coral neon, chrome highlights. Palette: teal, coral, chrome, rust. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours. Close-up, two hands only, no faces, no text, no lettering; labels blank.
```

## Backstory

An Indian-American man in his early 30s who grew up in the Levee. A young dad who co-signed a loan for his wife's brother. The brother stopped paying and left town; nobody knows where, and Kiran has stopped looking. The whole debt fell on Kiran, and with the default on his record no bank would help, so he borrowed from Ray to pay it off, and the interest buried him. None of it was his fault; he lives with all of the consequences. His wife feels responsible, and he never blames her. The brother-in-law and the wife stay offstage, with no portraits and no ties to the cast (Mike, 2026-10-29).

He only owes Ray. He is not part of Umbra, and he doesn't know or ask who Ray answers to (Mike, 2026-10-29). Ray collects by having him skim the riverboat's cheap tables: he steals from the house, palming chips off the rack on color-ups and short-counting the drop box. Players' odds at his table are always fair (audit 2.3, Mike 2026-10-26).

## Personality

The friendliest dealer on the boat: chatty, quick with a joke, remembers your name, so he's the last one you'd suspect. His tell is that he goes quiet and stops joking on the hands where he palms a chip (dialogue and a pose swap, not animation). Squeezed, not evil.

## Story

A cheating-rule scenario, aimed at theft from the house rather than at you. The clues build up: the Queen's floor manager grumbles that the packed cheap tables always count short; Rosa tells you to "watch the hand that isn't busy," and one of her lessons ("The dealer who's helping you is the one who stops talking") points at his quiet hand; at Kiran's table a color-up shows two insert panels of his chip rack, before and after, with one chip gone (a spot-the-difference nobody points out), while his pose swaps to the quiet one; after a sailing you see him pass an envelope to one of Ray's men at the landing. Each clue is deniable and some tips point at the wrong dealer. Call him out (he loses the job and Umbra notices you), walk away, or help him get clear of Ray by paying the debt or quietly passing what you know to Ed. Getting him clear is one of the favors that can make Ed stop running your plate. At A4.7 the skim itself can go to Wren (Ray learns she knew and is humbled), to Ed, or stay as your leverage on Ray. The collect-for-Ray job can also put him in front of you.

**After** (Mike, 2026-10-29):
- **Helped** (paid the debt or told Ed quietly): a later sailing, at least a season on, Kiran at table four. The same two inserts as the clue, before and after a color-up, and this time they match. His pose never swaps. "Welcome aboard! My daughter says this seat is lucky. She's seven. Eight now. She'd know." He never mentions the debt, Ray or you; the matching inserts are the thanks.
- **Walked away:** the floor manager's grumble becomes a notice: Kiran's gone. His table has a new dealer who doesn't talk. His daughter's photo is still tucked by the tray; nobody took it down.
- **Called out:** he's at the Prairie Queen landing one evening, out of uniform, and when he sees you he looks away, toward the water, and says nothing (a pose swap; no ring turn, because the game has no animation). Rosa, if present: "Watch the hand that isn't busy. His aren't, anymore."

## Voice

The friendliest dealer on the boat, with easy patter and jokes, and he remembers your name; goes quiet on a fixed hand (his tell). Talks about his daughter, the river and the regulars' lucky streaks; never says Ray's name or what he owes. Lies by keeping the patter going so you don't look at the cards. "Welcome aboard! Sit, sit. My daughter says this seat is lucky. She's seven, so she'd know." (Settled 2026-10-23.) After you've helped him: "Welcome aboard! My daughter says this seat is lucky. She's seven. Eight now. She'd know." (Mike, 2026-10-29.) A sample scene of his table, written in Ink, is in the [Story and Character Review](#writing).

### Voice sheet (settled 2026-10-18; write every line of dialogue to this)

| Rhythm | Says / never says | Lies by | Sample line |
| --- | --- | --- | --- |
| Friendliest dealer on the boat: easy patter, jokes, remembers your name; goes quiet on a fixed hand (his tell) | His daughter, the river, the regulars' lucky streaks; never says Ray's name, never talks about what he owes | Keeping the patter going so you don't look at the cards | "Welcome aboard! Sit, sit. My daughter says this seat is lucky. She's seven, so she'd know." / On a fixed hand, only: "...Cards." (Mike, 2026-10-23) |

## Related

Ray, Wren, Ed, Rosa.

## Planning record

*From the cast table in task 6, with the date each detail was settled.*

**Kiran (the Prairie Queen's crooked dealer; named 2026-10-17)**

- **Listed as:** Kiran (the Prairie Queen's crooked dealer; named 2026-10-17) (Supporting)
- **Role:** Blackjack dealer who cheats. Who (settled 2026-10-17): an Indian-American man in his early 30s who grew up in the Levee; a young dad who borrowed from Ray when his daughter needed surgery insurance wouldn't cover, and the interest buried him. The friendliest dealer on the boat: chatty, quick with a joke, remembers your name, so he's the last one you'd suspect; his tell is that he goes quiet and stops joking on the hands he's fixing (dialogue, not animation). Look: the Prairie Queen's old-time riverboat uniform (striped vest, sleeve garters, a bow tie always a little crooked), neat black hair, a wedding ring he turns when nervous (text only), a photo of his daughter tucked by his chip tray (Superseded 2026-10-29: he borrowed from Ray to pay off a loan he co-signed for his wife's brother.)
- **Ties to:** Umbra, maybe (Superseded 2026-10-29: Ray; he only owes Ray and is not part of Umbra.)
- **Hangouts:** Prairie Queen
- **Hook:** Catch them (rigged-riverboat mystery idea). Squeezed, not evil (settled 2026-10-17): they owe Ray, and Umbra collects by having them skim the riverboat's cheap tables. A story scenario under the cheating rule (a regular complains, or Rosa says to watch a table, and the tip may be wrong); you can call them out (they lose the job and Umbra notices you), walk away, or help them get clear of Ray by paying the debt or quietly passing what you know to Ed (Superseded 2026-10-29: Ray, not Umbra, has him skim; he doesn't know who Ray answers to.)

## Proposals from the review

*From the 18 Oct 2026 Story and Character Review, not yet settled.*

- **Tell (proposal):** pose: Friendly lean swaps to upright, eyes on the shoe; prop: Daughter's photo turned slightly away; means: This hand is fixed (settled tell: goes quiet)
