## How to use these

The portraits are generated with Leonardo.ai or Adobe Firefly (Mike, 2026-10-30), in the inked style of the Dot proof of concept (a style reference, not final art). Each character's page has a **Portrait prompt** section with:

- a sheet prompt, ready to paste, under 1,000 characters
- their look, head to toe, with a second outfit where a scene calls for one
- a pose table: the tag (also the file name and the scene's `# pose:` tag), the pose line, and the scene it's for

**The rules for every portrait** (Mike, 2026-10-30):

- **Full body, standing.** The frame always holds the whole body, from the top of the head to the shoes, in every pose, so pose swaps line up and characters can stand side by side in a two-shot.
- **Facing the viewer or turned to the character's own right** (Mike, 2026-10-30). In the picture that's toward the **viewer's left**, and the prompts always say it that way, because generators read a bare "right" as either the character's right or the picture's. Heads and eyes turn the same way. If a result faces the other way, regenerate it, or flip it horizontally and check anything one-sided (a watch, a badge, a lapel pin, the vest patch, a hair parting). Flip a finished image in layout if a scene ever needs the other direction.
- **Poses never overlap** (Mike, 2026-10-30). On a sheet, each pose has its own panel with clear space around it, so each one can be cut out cleanly.
- **Nothing in the hands.** Hands carry the expression: folded arms, a raised finger, an open palm. Anything a character holds (Gus's mug, Kasia's ledger, Wren's tablet, the drive) is an insert, a close-up panel of its own, prompted on the character's page under **Insert prompts**. Worn things stay: Kasia's towel, Pete's glasses, Richard's key on its lanyard.
- **The full look.** Build, hair and face, and clothes down to the shoes. Where the docs don't settle a detail, the prompt proposes one and the page says so under **Proposed, not settled**. Change those freely.

**Two passes per character:**

1. **The sheet.** Paste the sheet prompt at landscape 16:9. Regenerate until the face and outfit match the look. That sheet becomes the character's model.
2. **Single poses, which are the game's assets.** Paste the single-pose prompt below. Replace `[CHARACTER]` with the character's look (or the outfit the pose table names) and `[POSE]` with the pose line. Generate at portrait 2:3, with the approved sheet as the reference image so the face holds. Save each image under its pose tag (`dot_doorway.png`).

Single-pose prompt:

```
Full-body character portrait of one person standing, facing the viewer or turned toward the viewer's left, the whole body in frame from head to shoes, centred, nothing in the hands. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, realistic proportions, expressive face and hands. Flat light-grey background. No text, no speech bubbles.
[CHARACTER]
Pose: [POSE].
```

Negative prompt (Leonardo only):

```
text, letters, captions, speech bubbles, watermark, logo, signature, holding objects, props in hands, sitting, barefoot, overlapping figures, crowded panels, facing the viewer's right, turned toward the right side of the image, looking toward the viewer's right, cropped feet, cropped head, close-up, photorealistic, 3D render, anime, chibi, extra fingers, deformed hands, busy background, extra people
```

**Tool settings:**

- **Leonardo.ai.** Put the negative prompt in its own field. For single poses, add the approved sheet under Image Guidance as a Character Reference. Reuse the seed of a good result for the rest of that character's poses.
- **Adobe Firefly.** There's no negative prompt field; the facing, "nothing in the hands" and "No text" sentences inside each prompt do that job. Set the content type to Art and upload the Dot proof of concept as the Style reference, so the whole cast shares one look. If a face drifts between poses, generate a few and keep the closest.

**Checking a result:**

- **Match the look.** Generators add things, like the necklace on the Dot proof of concept, and they like to put something in an empty hand. Regenerate rather than keep it.
- **No text in the image.** Generators garble lettering (one caption on the Dot proof of concept came out as "Angry oulbber"). Add names and labels in layout. Any sample line comes from the character's voice sheet, never from the generator: a line like "Don't you dare talk about Gus that way!" would break Dot's rule that she never talks about Gus.
- **Same scale in every pose.** Pose swaps are the game's only animation, so every pose is the same size on the same flat grey, ready to cut out. Plates (backgrounds) are separate art; their prompts are on [Plate prompts](#plates).
- **Artist references stay out.** Gus's and Claire's pages name a cartoon and a game character as references. Naming them in a prompt pulls the generator toward a copy, and the portrait must be original, so the prompts describe the traits instead.

The style sentence is the same in every prompt. If the style changes, change it everywhere.

## Where the prompts are

Each character's sheet prompt and pose table is on their own page, under **Portrait prompt**, right after their Look:

- Core: [Gus](#gus), [Kasia](#kasia), [Rosa](#rosa), [Jalen](#jalen), [Chase / Brooke](#chase), [Ray](#ray), [Bennett](#bennett), [Vivian](#vivian), [Tae](#tae)
- Supporting: [Dot](#dot), [Stella](#stella), [Pete](#pete), [Mitch](#mitch), [Juan](#juan), [Raffy](#raffy), [Kiran](#kiran), [Wren](#wren), [Ed](#ed), [Mayor Dabareski](#mayor)
- Minor, with portrait: [Richard](#richard), [Troy](#troy), and Walt, Lorraine and Augie below (they have no pages)
- Romance: [Miles / Audrey](#miles), [Felix / Nora](#felix), [Graham / Claire](#graham)

The player has no portrait: they're a blank slate.

### Walt, Lorraine and Augie

The three Flare regulars have portraits but no pages, and only their roles are settled, so every detail of their looks below is a proposal.

#### Walt

**Proposed, not settled:** everything (only his role is settled).

**Sheet prompt** (16:9, regular outfit):

```
Full-body expression sheet: 2 poses of one person, each in its own evenly spaced panel with clear gaps, no overlap. Standing, facing the viewer or turned toward the viewer's left, whole body in frame head to shoes, empty hands. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted colours, realistic proportions, expressive face and hands. Flat light-grey background. No text, no speech bubbles.
Walt: a white man in his early 70s, wiry, bald with a white fringe, sharp eyes. A tan windbreaker over a checked shirt, brown slacks, brown shoes.
Panels: 1) arms folded, a knowing look; 2) a short nod, mouth shut.
```

**Look** (paste as `[CHARACTER]` in the single-pose prompt):

```
Walt: a white man in his early 70s, wiry, bald with a white fringe, sharp eyes. A tan windbreaker over a checked shirt, brown slacks, brown shoes.
```

| Pose tag | Pose line | Used for |
| --- | --- | --- |
| `walt_neutral` | arms folded, a knowing look | "Boat's cheap tables count short." |
| `walt_nod` | a short nod, mouth shut | Sending you to the Prairie Queen |

#### Lorraine

**Proposed, not settled:** everything (only her role is settled).

**Sheet prompt** (16:9, regular outfit):

```
Full-body expression sheet: 2 poses of one person, each in its own evenly spaced panel with clear gaps, no overlap. Standing, facing the viewer or turned toward the viewer's left, whole body in frame head to shoes, empty hands. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted colours, realistic proportions, expressive face and hands. Flat light-grey background. No text, no speech bubbles.
Lorraine: a white woman in her late 60s, short permed grey hair, cat-eye glasses. A lavender cardigan over a floral blouse, navy slacks, comfortable white shoes.
Panels: 1) hands on her hips, scolding; 2) standing up straight, chin up, determined.
```

**Look** (paste as `[CHARACTER]` in the single-pose prompt):

```
Lorraine: a white woman in her late 60s, short permed grey hair, cat-eye glasses. A lavender cardigan over a floral blouse, navy slacks, comfortable white shoes.
```

| Pose tag | Pose line | Used for |
| --- | --- | --- |
| `lorraine_neutral` | hands on her hips, scolding | "It owes me." |
| `lorraine_standing` | standing up straight, chin up, determined | Standing beside the league (A3.2) |

#### Augie

**Proposed, not settled:** everything (only his role is settled).

**Sheet prompt** (16:9, regular outfit):

```
Full-body expression sheet: 3 poses of one person, each in its own evenly spaced panel with clear gaps, no overlap. Standing, facing the viewer or turned toward the viewer's left, whole body in frame head to shoes, empty hands. Gritty graphic-novel art: bold black ink outlines, heavy shadows, cel shading, muted colours, realistic proportions, expressive face and hands. Flat light-grey background. No text, no speech bubbles.
Augie: a Black man in his late 70s, thin, short white hair, a neat white moustache. A brown tweed flat cap, a grey cardigan over a collared shirt, brown corduroy trousers, polished brown shoes.
Panels: 1) hands folded in front of him, content; 2) looking off toward the viewer's left, wistful; 3) turned toward the viewer's left to leave, one hand raised in a small wave.
```

**Look** (paste as `[CHARACTER]` in the single-pose prompt):

```
Augie: a Black man in his late 70s, thin, short white hair, a neat white moustache. A brown tweed flat cap, a grey cardigan over a collared shirt, brown corduroy trousers, polished brown shoes.
```

| Pose tag | Pose line | Used for |
| --- | --- | --- |
| `augie_neutral` | hands folded in front of him, content | "I come for the band." (his coffee is an insert) |
| `augie_wistful` | looking off toward the viewer's left, wistful | The covered piano |
| `augie_leaving` | turned toward the viewer's left to leave, one hand raised in a small wave | The last night (A3.3) |

## Not settled yet

Every detail the docs don't settle is proposed in the prompts and listed on each page under **Proposed, not settled**. A few need a decision across characters:

| Who | Not settled |
| --- | --- |
| Gus, Dot, Stella | The prompts propose all three as white. They're family (Stella is Gus's daughter, Dot her aunt), so they should look it. |
| Bennett, Richard | Proposed as white, sharing a strong square jaw so the desk photo clicks. |
| Alice | No look at all, but she appears in Dot's old-photo panels (two women on a beach, one in each shirt). Not prompted yet. |
| Walt, Lorraine, Augie | Everything but their role; the looks above are proposals. |
| The house uniform | Jalen's host jacket is proposed as midnight blue with a gold rocket pin; the new Starflare's staff should match. |
