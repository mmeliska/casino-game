---
id: S-023
title: Rosa's lessons are her confession: each tell she teaches is one from the payroll night
status: proposed
area: gameplay-integration
character: Rosa
type: expansion
impact: medium
effort: medium
depends_on: S-022
conflicts_with: none
---

## Summary
Structure Rosa's teaching so that each lesson is a tell from the night she dealt Gus back to even, told as a rule about someone else. The player learns to read tables and, without knowing it, learns M1. When M1 is solved, the lessons re-read.

## Current state
Rosa "teaches the player the rules of the games and sharpens their read of tells, never giving certainty" and "lies by telling the truth about the wrong thing" (people/rosa.md). The review proposed that each lesson unlock one tell type (pages/review.md). Clues to M1: she won't enter the Flare; Pete goes quiet; the tape; the commission file (plan/t08.md). Her one settled teaching line is about Kiran: "watch the hand that isn't busy."

## Proposed change
Five lessons, one per act or trust step, each unlocking a read in the results strip (as the review proposed). Each is a true rule and a true memory:

| # | The lesson | The read it unlocks | What it really is |
| --- | --- | --- | --- |
| 1 | "Watch his hands, not his face. Faces are for amateurs." | Dealer tells on a flagged table | The overhead camera: hands, no faces |
| 2 | "A drunk bets the same whether he's winning or losing. That's how you know." | Opponent tilt in poker | Gus at her table, betting the payroll flat |
| 3 | "A run of cards isn't luck. Count how many hands it took." | The results strip's pattern, only on a table a story moment has flagged | Forty minutes to deal him back to even |
| 4 | "The dealer who's helping you is the one who stops talking." | Kiran's quiet hand (M3) and any dealer a story moment has flagged | Herself, that night |
| 5 | "When the pit comes over, look at who doesn't look up." | Who at a table knows something | Pete, at the pit, who didn't look up |

After M1 is solved, each lesson gets a one-line callback the next time it fires in play, in Rosa's voice, in the past tense. "It took forty minutes." That's all.

## Why it works
"Lies by telling the truth about the wrong thing" becomes the structure of her teaching, not just a line. The player is rewarded twice for the same content: once as a skill, once as a story. It solves the review's ask (Rosa as the teacher of reading) and makes the mystery's solution land as recognition rather than exposition. (Technique: dramatic irony paid as a re-read.)

## How the player experiences it
Lessons at Kasia's back room, at the Prairie Queen landing, and anywhere she's your companion. Each unlock is a new line in the results strip. After M1, five quiet callbacks.

## Risks and tradeoffs
Needs the read system the review proposed, which is a small UI feature (a line in the results strip). The lessons must be useful as play advice first, or the trick reads as a trick, and every read they unlock applies only inside flagged story moments, never in ordinary play. Keep the callbacks optional and rare.

## Decision notes

