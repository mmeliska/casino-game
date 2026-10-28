---
id: S-009
title: "Story keys tell you who gave them to you"
status: accepted
area: gameplay-integration
character: none
type: change
impact: high
effort: low
depends_on: none
conflicts_with: none
---

## Summary
Every item in the story-key row is something a person handed you. Tapping a key opens a short dialog that describes it and says who gave it to you. When an act 4 choice spends one, the option names the giver. The record card lists what you kept.

## Current state
Pockets has 12 slots plus "a separate row of story keys" (pages/gdd.md): Raffy's envelope and thumb drive, a room 9 key. Pete's gold rocket pin and Gus's IOU (S6) are also given to you and held until a scene spends them, but aren't listed as keys (people/pete.md, plan/t09.md). Act 4 routes are described as routes: "give her Gus's markers or Raffy's thumb drive" (A4.5), "hand the proof to Ed" (A4.3).

## Proposed change
1. **Define the row as "what they gave you."** Raffy's drive, Gus's IOU, Pete's pin, the Cinder Room password Ray slipped you, Richard's trust (not the key, which stays on his lanyard; represent it as the deck of cards from the picnic table, which he gives you to keep once the box is settled), Vivian's jeweler's receipt (from the drive job, if she asked you to keep quiet). Tapping a key opens a small dialog: the item's name, a line or two describing it, and who gave it to you and where ("Raffy gave you this at Kasia's. He made you promise not to look."). No portraits on the icons.
2. **Spending one names the giver.** Options stay actions, but say whose it was: "Hand Wren the drive Raffy gave you." Tapping the item before choosing opens the same dialog.
3. **The record card ends with the row.** Under your bankroll and heat: what you still hold, each with its giver's name. A player who kept everything has a different last screen from one who spent it all.

## Why it works
Act 4 is currently three routes; this makes it a choice about people. Loss aversion does the work: a player hesitates to spend "the drive Raffy gave you" in a way they would never hesitate to spend a thumb drive. It costs no art, only a short description per key, and it is the cheapest possible way to make "every choice is remembered" visible without a journal.

## How the player experiences it
Pockets tab, story-key row: tap a key to read what it is and who gave it. During A4.3, A4.4 and A4.5, the options name the giver. On the record card, the list of what you kept and from whom.

## Risks and tradeoffs
Adding the pin and IOU to the key row changes two existing item placements (minor). The password as a "key" is slightly abstract; drop it if it reads wrong. The record card row must not look like a scoreboard; names only, no counts. Each key needs a one- or two-line description written in the giver's context.

## Decision notes
- Mike, 2026-10-28: accepted with a change: no portraits. "Clicking/tapping on it will bring up a dialog describing it along with who gave it to you." Act 4 options and the record card name the giver in text.

