Keep these in mind when writing any scene. They come from the Story & Character Review and the rules already settled in Planning Tasks.

## The panel kit

There is no character animation, so the screen is the panel. Build every scene from these five units, not from blocks of text.

| Unit | What it is | Use it for |
| --- | --- | --- |
| Plate | Full background, no characters | Opening a scene, time passing, a beat of silence |
| Two-shot | Background plus one or two portrait poses | Normal dialogue |
| Pose swap | Same frame, one portrait changes | A reaction, a lie, a tell: the cheapest drama you have |
| Insert | Close-up of a prop or a hand | Clues and tells: the chipped mug, the torn patch, a palmed chip, the envelope |
| Black panel | One line of text on black | A hard cut, a threat, the moment before a reveal |

- **One line per panel, 15 words at most.** On a portrait phone, anything longer splits into two panels or becomes an insert.
- **Motion is a short caption.** "Quick hands" goes under an insert of the chip tray, not in a sentence of action.
- **Silence is a panel too.** An empty speech bubble or a plate with no text often says more than a line.

## Mood and place

Each district's light, palette, recurring image and weather now live in [Northlight Locations](https://claude.ai/code/artifact/98ecfc48-81d8-492b-956b-449fa566f8dd), along with every venue and story place. Check it before you set a scene. While you write, keep these in mind:

- **Noir by night, warm by day.** Northlight is a city people like. Daylight scenes are warm and busy (pierogi at Kasia's, league night at Millstone Lanes); the grit belongs to the hours after the tables close.
- **Weather is a free mood layer.** A static rain, fog or snow overlay changes a reused background more than a new pose would. Seasons do the same.
- **Show the clue, say nothing.** Clues arrive as inserts with no caption. The player who notices is rewarded; the one who doesn't hasn't been told.
- **Props replace exposition.** City history is never a paragraph: it's Kasia's dad's union pin, Pete's 25-year rocket pin, the Stacks outside every Levee window, the founder's jacket straining on Gus.
- **Let decline show.** The Flare's plate gets darker over time (more lights out, a roped-off table, the piano covered) without anyone saying so.
- **Backstory in 3 or 4 panels.** A character's story (Dot's flood, Richard's lost house) is told as a short run of old-photo panels, not a monologue.

## Dialogue and choices

- **Write to the voice sheets.** Every line should sound like its speaker on the settled voice table in Planning Tasks, task 6. If you can swap two characters' lines without noticing, rewrite one.
- **Great Lakes, not movie-gangster.** Understatement, dry humour, work-and-weather talk, people who say less than they feel. Noir comes from what's withheld, not from tough-guy slang.
- **Slang is rare.** Riding the light, gone dark, going over the water: a few uses per act. Say "took a bribe" plainly.
- **Choices are actions.** "Slide the envelope across" can be a bribe, a return or a test. Never label intent ("\[Lie\]") or show scale effects ("Straight +1").
- **No moral menus.** Each option should cost something on one hidden scale and earn on another (generous but flaky, straight but ruthless), never good / neutral / bad.
- **The key line is the one nobody says.** Let characters answer the question you didn't ask.
- **Bluffs run on information.** A lie works if the character knows nothing that contradicts it, and fails if they do. No dice rolls in conversation.
- **Use double meanings.** Marker, the house, cash out, fold, all in, comped: the reply shows which meaning the character heard.
- **Keep the guardrails.** No religion; romance only with the romance characters and never changes the odds.

## Tension at the table

- **Put a stake on the choice.** When a decision matters, say what it costs: "Cover Jalen's $500 buy-in", "Bet again and watch where the chips go (-$25)". A choice with no stake reads as a menu.
- **Tells are shown, never narrated.** A tell is a pose swap or a prop insert, plus an empty bubble where a talker goes quiet. Write "# pose: kiran\_upright\_eyes\_down", not "he stopped talking".
- **Tells belong to people, not house games.** Read poker opponents, dealers in a story moment, and characters who may be lying. Never blackjack, roulette, craps or slots in normal play.
- **Follow the cheating rule.** Regular play is always fair. Doubt comes only in story moments (a tip, Rosa's look, a short payout), and the player can't be sure: some tips are wrong. Every call has a cost (accuse a crooked dealer and Revaris notices you; accuse an honest one and you lose standing; ride it and you bleed money; walk and you never know), and the truth surfaces later.
- **Let the proof be checkable.** On a crooked table, the results strip shows the pattern, so the player can verify a read.
- **Use the setting as pressure.** A boat that hasn't docked, a game you can't leave until dawn, a password that changes: if walking away is easy, the scene is slack.
- **Ask the tip-giver to look, not to judge.** "Count your chips after every hand" beats "the dealer is cheating"; the player should make the discovery.
- **Companions change the scene.** If Rosa, Jalen, Juan or Raffy is along, give them one option or one line that only they unlock.

## Script layout

Write scenes in Ink, the proposed engine, with a tag on every beat, so the script is also the art list. A worked example is Kiran's table in section 5 of the Story & Character Review.

```ink
// SCENE TITLE: venue, time of day. Story moment or regular?
// Tags: #plate (background), #pose (portrait swap), #insert (close-up), #black (text on black)

=== scene_name ===
# plate: venue_area_time_weather
# pose: character_pose
CHARACTER: One line, 15 words at most.
# insert: prop_detail
* [An action, not a label.] -> branch_a
* [Another action with a stake. (-$25)]
    ~ money -= 25
    -> branch_b
* {companion == "Rosa"} [A companion-only option.] -> branch_c

= branch_a
# pose: character_reaction
~ flag_the_world_remembers = true
-> next_scene
```

**Before you call a scene done**

- [ ] Every beat has a plate, pose, insert or black tag
- [ ] No panel runs over 15 words
- [ ] Every line matches its speaker's voice sheet
- [ ] Clues are shown as inserts, not explained
- [ ] Options are actions, with no labels or scale effects
- [ ] At least one option carries a stake
- [ ] Walking away isn't free
- [ ] Normal play stays fair; any doubt is a story moment with a later reveal
- [ ] Each choice sets a flag someone will remember
- [ ] The time of day and weather fit the mood
