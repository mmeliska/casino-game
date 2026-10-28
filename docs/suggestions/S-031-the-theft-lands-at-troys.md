---
id: S-031
title: "The rival's theft (A4.8) is one object, worth the most to one person, and you find it in Troy's cage"
status: accepted
area: gameplay-integration
character: Chase/Brooke
type: change
impact: high
effort: low
depends_on: S-009, S-030
conflicts_with: none
---

## Summary
Instead of "they may end up stealing from you somehow," the rival steals one specific thing from your Pockets, the one worth the most to someone else, and pawns it at Bridgehead Loan & Pawn. You find it behind Troy's glass, with a buy-back date.

## Current state
A4.8: "If you refused them: theft, or all is well" (pages/storymap-beats.md); "refuse and they may end up stealing from you somehow" (people/chase.md). Troy "buys your items for a fraction and holds them a few days so you can buy them back. Some items are worth far more to one person" (people/troy.md). Pockets holds a suit, a gift for Dot and Gus, tires and Gus's lucky chip (plan/t13.md); Pete hands you his pin at A3.4 (people/pete.md) and its slot is not stated. Story keys "can't be pawned unless a scene offers it."

## Proposed change
The theft picks from your 12 slots the item with the highest "worth to one person" (an authored rank: Gus's lucky chip first, then the Dot-and-Gus gift, then the spaceman bobblehead, then anything), never a story key. It's gone at a break point, no scene. Then, days later, at Bridgehead:

- Insert: Troy's cage, your thing on the shelf, a tag with a date. No caption.
- Troy: "Two hundred. You got till Thursday to want it back." He doesn't say who sold it. He doesn't have to; the watch is on the shelf next to it (S-030).
- Choices, as actions with stakes: buy it back ($200); buy the watch too ($150) and give it back to the rival (generous, and the rival's "all is well" branch reopens); leave both; or tell Ed (ruthless; the rival's heat appears for the first time in the game).

If you never buy it back, it stays gone; whoever it mattered to notices once and says nothing.

The theft fires only if you refused them at A3.8 with middle or low standing (S-030's nights out). At high standing, a refusal ends the friendship without a crime.

## Why it works
"Somehow" becomes a scene the pawn system was built for ("some items are worth far more to one person" is Troy's line, and this is its payoff). Loss hurts most when the thing lost belongs to a relationship, and finding it priced at $200 says everything about the rival's state. Buying back the watch is a kindness the player invents, which is the best kind.

## How the player experiences it
A missing slot, then Troy's shelf. The watch beside it is the only clue to who. The buy-back timer is the pressure.

## Risks and tradeoffs
Needs an authored rank on items (one field). Pete's pin and Gus's IOU are story keys (S-009) and can't be stolen or pawned.

## Decision notes
- Mike, 2026-10-28: accepted.

