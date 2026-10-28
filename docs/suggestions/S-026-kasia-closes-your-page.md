---
id: S-026
title: Kasia's signature moment: she closes your page in the ledger
status: accepted
area: gameplay-integration
character: Kasia
type: new
impact: medium
effort: low
depends_on: S-016, S-024
conflicts_with: none
---

## Summary
The ledger is how the player feels the reliable scale; give it one scene. At the end of the main story or a resting point, Kasia opens the book to your page and does one of a few things with it, depending on how you paid. It's the game's quietest report card.

## Current state
"Gives credit but keeps a ledger and remembers every cent"; "her credit is how the player feels the reliable/flaky scale"; sample line: "Your tab's in the book. The book never forgets. Neither do I." (people/kasia.md). The Wallet shows "Kasia's ledger" as a debt line (pages/gdd.md). No scene uses the book.

## Proposed change
**The mechanic underneath** (Mike, 2026-10-28; the docs open Kasia's credit at A1.2 but never define it):
- **"Put it on the book."** At the Tap, when your cash is short, Kasia offers to cover you for pull tabs, and once you're invited in, her back-room game, up to a limit. The limit starts small, about $50.
- **No interest and no due date**, unlike Ray's loans. She just remembers. Paying it back within about a week of in-game days counts toward reliable. Leaving it longer, or borrowing again with a balance open, counts toward flaky.
- **Her limit is how the hidden scale shows.** It grows when you're reliable and shrinks or closes when you're flaky. That is the docs' "how much credit Kasia gives." The Wallet shows the balance as "Kasia's ledger," as already specified, and it's subtracted from best bankroll.

**The closing scene.** At opening night, or when a resting point settles, if you ever had credit at the Tap, one scene at the bar, four panels:

- **Paid every cent, on time (reliable):** she draws a line under your page and writes the date. "Book's closed. Drinks aren't." Your credit is unlimited afterward, which the player will never need and will remember.
- **Paid, late, often (mixed):** she turns the page and starts a fresh one with your name at the top. "New page. Same book."
- **Skipped out, ever (flaky):** she tears the page out and puts it in the till. "I keep these." You can still drink; you can't run a tab. Rosa deals you, but she doesn't warn you off a hand.
- **If Kasia's gone crooked (S-024, bad ending):** the book is Ray's man's, and your page has interest on it. (Ray's terms, not Kasia's: her own credit never charges interest.)

The ledger insert is the same prop every time; only the hand and the line change.

## Why it works
The hidden scales are felt, never shown; this is the designed way to feel one, done once, by the character who owns it. Three lines and a prop insert give the reliable scale a face and a consequence, with no number. It reads as a judgment from someone whose judgment the player has been managing since night one.

## How the player experiences it
One short scene at the Tap, fired at the ending or resting point as a reveal-type event. The dialogue is the only feedback.

## Risks and tradeoffs
Players with no Tap credit never see it; that's fine, Kasia says "you never owed me anything" and the player hears the compliment or the accusation. Keep the torn-page version from doubling with Dot's torn register (S-016); one is for stiffing a creditor, the other for betraying Gus, and they should never fire for the same act.

## Decision notes
- Mike, 2026-10-28: accepted, with the "put it on the book" credit mechanic added, since the docs never defined how you owe Kasia.

