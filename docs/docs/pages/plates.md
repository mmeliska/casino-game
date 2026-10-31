## How to use these

Every place in the game has its own page (listed on [Northlight](#city)) with a **Plate prompts** section: one prompt for each panel the scenes there need. Each prompt has a note above it saying what the panel shows and which scenes use it, then its tag and size. The prompts are written for Leonardo.ai or Adobe Firefly, in the same inked style as the [portraits](#portraits), so a character's pose laid over a plate looks like one picture.

Two kinds of panel from the [panel kit](#writing):

- **Plates** are backgrounds with no people: portrait 9:16, the full phone screen. Portraits are laid over them for two-shots and pose swaps.
- **Inserts** are close-ups of a place's props: square 1:1. Clues arrive this way, uncaptioned.

**These plates ship.** Like the portraits, AI-generated plates and inserts are final art, because there's no budget for commissions now (Mike, 2026-10-31). If plates are ever commissioned, each prompt and its note is the brief.

**The rules for every plate:**

- **No people.** Generators like to add a figure; regenerate or paint it out. A busy room is shown by what people leave: pulled-out chairs, drinks, lit machines.
- **Eye level, open floor.** The view is from standing height, with open floor in the lower middle of the frame, so one or two full-body portraits can stand there at the same scale in every plate.
- **The place's main feature goes in the upper third or on the left.** Characters face the viewer or the viewer's left, so what they look at sits on the left (the rocket sign across the river, the lighthouse on the harbour).
- **No text.** Signs, screens, posters, labels and plates are blank; names, news and numbers are added in layout. Generators garble lettering. The rocket sign is a shape, not a word.
- **District light and palette in every prompt.** Each place's prompts carry its district's key light and palette from the districts table on [Northlight](#city), so a player knows where they are at a glance.
- **Weather is a layer.** Rain, fog and snow are overlays made in layout over a reused plate. Weather is baked into a plate only where a scene names it: the Prairie Queen's mist, the Long Bridge's fog before dawn, Blackspruce's snow.
- **One room, many states.** Each place's first plate is its base. Make its variants (day, before dawn, the Flare's decline, the last night, cameras off) from the base as a reference image, so it stays the same room and only the light or the props change.
- **The rocket sign comes down.** It glows across the river or down the street only until the Flare is demolished, then goes into storage (see [the Flare](#the-flare)). Plates that show it are for before then; make a variant without it for later scenes. Plates used in every act leave it out.
- **Match the big compositions.** The motel lot on night one and the yacht deck later share their framing: the city across the water with the rocket a coral dot. Use the lot as the yacht deck's composition reference.

**Tags.** A plate's tag is `place_area_time`, plus a state or a baked-in weather where there is one (`flare_floor_night_decline`, `pq_deck_night_mist`), as in the [writing guide's](#writing) script layout. An insert's tag is `place_prop` (`flare_bucket`). The tag is also the image's file name.

**Negative prompts** (Leonardo only):

Plates:

```
people, person, figure, silhouette, crowd, face, hands, text, letters, words, numbers, captions, readable signs, logo, watermark, signature, speech bubbles, photorealistic, 3D render, anime, fisheye, tilted horizon, bird's-eye view, cluttered foreground, blurry
```

Inserts:

```
people, face, hands, text, letters, words, numbers, captions, logo, watermark, signature, photorealistic, 3D render, anime, blurry, cluttered
```

Photo inserts (a framed photograph with people in it: the Flare's band photo, the two sisters, Bennett's desk photo):

```
text, letters, words, numbers, captions, logo, watermark, signature, photorealistic, 3D render, anime, blurry, extra fingers, deformed faces
```

**Tool settings:**

- **Leonardo.ai.** Set 9:16 for plates and 1:1 for inserts, and put the negative prompt in its own field. Add the Dot proof of concept under Image Guidance as a Style Reference, so plates and portraits share one look. For a variant, add the place's base plate as a Content Reference (or use it for image-to-image at about half strength).
- **Adobe Firefly.** There's no negative prompt field; the "No people, no text" sentence in each prompt does that job. Set the content type to Art, the aspect to 9:16 or 1:1, and upload the Dot proof of concept as the Style reference. For a variant, upload the base plate as the Composition reference.
- **Photo inserts** need the characters' approved portrait sheets as a reference (Character Reference in Leonardo) so the faces match.

**Checking a result:**

- **Lay a portrait over it.** A full-body pose should stand on the floor at a natural size, with nothing important behind its head.
- **No people, no lettering.** Regenerate or paint out anything that crept in.
- **The same room.** Across a place's variants, the doors, windows and the sign stay where they were.
- **Nothing that contradicts the docs.** Settled details (the spaceman by the doors, the meeting sign on Kasia's back-room door, the Stacks outside every Levee window) beat whatever the generator invents. Each place page lists its own proposals under **Proposed, not settled**.

The style sentence is the same in every prompt. If the style changes, change it everywhere.

## Not settled yet

| What | Not settled |
| --- | --- |
| Every interior and building | The layouts, furniture and materials in the prompts are proposals unless the place page says otherwise. |
| When the Flare is demolished | The sign is in storage after demolition, but when demolition happens against acts 4 and 5 isn't settled, so it's unclear which later scenes still see it. |
| Unnamed places | The old Levee bank, the bar under your Levee room, and where Bennett's office is. |
| The new rocket sign | You pick its design at A5.4; how many designs there are, and what they look like, isn't settled. The prompt proposes one. |
| Wren's Starflare (O8) | Where the Revaris showcase stands isn't settled; no plates yet. |
| Characters' own props | Things a character carries (Gus's mug, Kasia's ledger, Wren's tablet, Raffy's envelope and the drive, Kiran's chip-tray photo) are inserts too, but they belong to the characters and aren't prompted yet. |
| Game screens | Table felts, slot symbols and the Rocket Wheel are game art, not panels, and aren't covered here. |
