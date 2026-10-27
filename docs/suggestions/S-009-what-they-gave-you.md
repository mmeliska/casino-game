---
id: S-009
title: Story keys show the face of the person who gave them to you
status: proposed
area: gameplay-integration
character: none
type: change
impact: high
effort: low
depends_on: S-006
conflicts_with: none
---

## Summary
Every item in the story-key row is something a person handed you. Show that person's portrait inset on the item, and when an act 4 choice spends one, show the face, not the object. The record card lists what you kept.

## Current state
Pockets has 12 slots plus "a separate row of story keys" (pages/gdd.md): Raffy's envelope and thumb drive, a room 9 key. Pete's gold rocket pin and Gus's IOU (S6) are also given to you and held until a scene spends them, but aren't listed as keys (people/pete.md, plan/t09.md). Act 4 routes are described as routes: "give her Gus's markers or Raffy's thumb drive" (A4.5), "hand the proof to Ed" (A4.3).

## Proposed change
1. **Define the row as "what they gave you."** Raffy's drive, Gus's IOU, Pete's pin, the Cinder Room password Ray slipped you, Richard's trust (not the key, which stays on his lanyard; represent it as the deck of cards from the picnic table, which he gives you to keep once the box is settled and the last hand with his son is played, S-006), Vivian's jeweler's receipt (from the drive job, if she asked you to keep quiet). Each shows a small portrait inset of the giver.
2. **Spending one shows the face.** Options stay actions ("Hand Wren the drive"), but the panel behind the option is the giver's seed panel: Raffy at Kasia's, one earbud in. No caption. The callback-cue mechanism already exists for reveals; reuse it here.
3. **The record card ends with the row.** Under your bankroll and heat: what you still hold, in portraits. A player who finishes with every face intact has a different last screen from one who spent them all.

## Why it works
Act 4 is currently three routes; this makes it a choice about people. Loss aversion does the work: a player hesitates to spend Raffy's face in a way they would never hesitate to spend a thumb drive. It costs nothing new (the seed panels and portraits exist) and it is the cheapest possible way to make "every choice is remembered" visible without a journal.

## How the player experiences it
Pockets tab, story-key row, portrait insets. During A4.3, A4.4 and A4.5, the choice panels get the giver's inset. On the record card, the row of faces.

## Risks and tradeoffs
Adding the pin and IOU to the key row changes two existing item placements (minor). The password as a "key" is slightly abstract; drop it if it reads wrong. The record card row must not look like a scoreboard; portraits only, no counts.

## Decision notes

