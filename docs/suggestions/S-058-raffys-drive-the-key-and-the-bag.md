---
id: S-058
title: "Raffy's drive: an encrypted Revaris key ledger, the paper key in Ray's burn bag, and the subpoena that exposes Umbra"
status: accepted
area: main-story
character: none
type: change
impact: high
effort: medium
depends_on: none
conflicts_with: none
---

## Summary
Settle what the drive is, why it ties Revaris to Umbra, and how anyone can read it. The drive is Revaris's encrypted key ledger: the only place an anonymous face ID is tied to a name. On its own it proves nothing and can't be opened. In Ed's hands it is probable cause for a subpoena that forces Revaris to show its cameras are blind, citywide, to Umbra's roster and to the men who pay Umbra. The player can also open it themselves, by reassembling a shredded paper key from Ray's burn bag in the Cinder Room office (the GDD's jigsaw mini-game). Worked out with Mike, 2026-10-28.

## Current state
The docs say the drive holds "Ray's insurance copy of Wren's face-match logs, taken off the Prairie Queen camera feed," in two files: the logs, which "prove the Revaris feeds go to Umbra" and are all the clean route uses, and the payers list (people/raffy.md, people/ray.md, audit 4.6). The player opens it at A1.5 and chooses its fate. A4.3 uses it as proof; A4.5 is Wren's deal for it. Nothing explains how a log of face matches proves who owns the camera company, how Ray got it, or why a system like Revaris would put it on a thumb drive.

## Proposed change
**Separation of knowledge.** A Revaris site box stores face templates with anonymous IDs. Its exclusion list is a set of IDs: don't log or alert on these. No box holds a name. The ID-to-name key exists in one place, on a drive that never touches a network, because Wren keeps the one file that could sink her as far from a machine as she can.

**Why a key has to exist.** The list changes. Men pay to be unseen and stop paying; the uncles' people come and go. Someone has to know which ID to remove. So there is a ledger, and it is encrypted, and it is carried by hand.

**Enrollment at the Cinder Room.** The room's long tradition of powerful, shady men is why Wren keeps it (Mike, 2026-10-28): intel on those men, and a place to run Umbra's dealings through proxies. She rarely goes there herself. When a man pays to be unseen, he is enrolled there: face captured, ID generated, name and ID written to the ledger. Ray runs the room for her, so Ray holds the token that writes to the ledger. He is the one person who ever sees a name beside an ID. Wren trusts him with the pen and has made sure he knows nothing else.

**What Ray did.** At a refresh he kept the superseded drive instead of destroying it. Most of the names are still good. That is his insurance against Umbra, and his own name is on it, which is why he can never admit he kept it: a missing drive points at exactly one man. He carried it the way he carries everything, in an envelope in his car with a night's collection cash, and Raffy lifted the envelope (A1.2, as the docs have it).

**What the player sees at A1.5.** They plug it in and get a locked volume with Revaris's name on it and a serial number. They can't open it. They know two things: it is Revaris property, and it was in Umbra's collector's car with collection cash. The envelope's fate choices stay as written.

**The key is on paper.** At each refresh a one-time key sheet is printed, loaded at the enrollment station, and destroyed. Wren's rule is cross-cut shredding into a burn bag her people collect weekly. The Cinder Room is the one room she rarely enters, and Ray takes the shortcut: a strip shredder, and a burn bag in the office closet that nobody has collected in months, because collecting it would mean someone checking on him. Her design is sound. Her mistake is Ray.

**Getting the bag.** Three existing doors into that office: working off a debt to Ray on the broke path (the docs already put the player "in the room"), the collect-for-Ray job, or Raffy as a runner (S-050). The bag is a theft and carries heat. If Ray ever works out the bag is gone, he knows which of his people was in the closet, and the player's standing with him drops to the floor.

**The mini-game.** The GDD's tile-rotate/jigsaw for a shredded document, with a twist: the bag holds strips from many refreshes, and only one sheet matches the drive. The drive's serial says which. The player assembles one sheet out of a haystack of near-identical strips. Reassemble enough of the key string and the volume opens. Insert: the first page of the ledger, names beside numbers, Ray's among them, two aldermen, men the player has met. No caption.

**Who can read it.** Wren. Revaris, under subpoena. Ray, who holds the token. And a player who has rebuilt the key sheet.

**Why it ties Revaris to Umbra.** A box alone is forty numbers that mean nothing. The ledger alone is forty names beside numbers that prove nothing. Put a judge between them and Revaris must produce what its system does with those IDs: nothing. It is configured, citywide, not to see them. A security company blind to a mob roster and to men who pay the mob.

**The routes, revised.**
- **Clean (A4.3):** hand Ed the drive, opened or not. Encrypted, it is still probable cause: Revaris-serialized hardware in a mob collector's car, with Raffy's statement. Opened, it is stronger. Either way Ed needs a subpoena, and the encryption works for him: the ciphertext is in evidence, so Wren can produce the real key or refuse, and refusing is its own answer. The route is slow and loud, because a subpoena to Revaris tells Revaris, and Ed can't serve it through his own squad, whose readers are Wren's (S-048). He goes around his department to the commission or the state. Umbra and the payers go down together, and some of the payers didn't deserve it, which is the clean route's cost.
- **Dirty:** the payers' names are only useful to someone who can read them. Either the player rebuilt the key, or Ray opens it for them (hold his debt over him, or earn his confidence), and the two of them lean on the payers together, which is what Ray wanted the insurance for all along.
- **Ruthless (A4.5):** give the device back to Wren. O8, as settled. She never knows whether it was read.

**What Wren is afraid of.** Not the player reading it. Ed holding it. Once the drive is in evidence she can't make it say anything else. So A4.5 is the deal she makes to stop A4.3 from happening, and the only person who can give her the drive is someone who may have no idea what's on it.

## Why it works
A smart adversary's system fails the way smart systems fail: at the one human it had to trust. Every piece is realistic (air-gapped keys on paper, strip shredders, subpoenaed configuration) and every piece is a character beat: Wren's separation of knowledge, Ray's shortcut, Ed's compromised department. The drive is a sealed thing in act 1, a puzzle in act 2 or 3, and a clock Wren can't see in act 4. Opening it shows the player what the Cinder Room is without a line of exposition.

## How the player experiences it
A locked volume at A1.5; a closet, a bag and a jigsaw if they go looking; a page of names; and three ways to spend it.

## Risks and tradeoffs
Changes the docs: `people/ray.md` and `people/raffy.md` lose "copied the logs"; the drive is one encrypted file, not two; A4.3's clean proof becomes "Ed subpoenas Revaris"; Ray's Cinder Room role gains "runs enrollment for Wren." The bag theft needs one authored event and the jigsaw needs a "haystack" difficulty setting. Keep the ledger page to names and numbers; never show the system's internals on screen. Nothing player-facing may name Umbra as Revaris before M2 (bible 4.1): the locked volume says Revaris, the envelope says Ray, and the player draws the line themselves.

## Decision notes
- Mike, 2026-10-28: worked out in conversation and accepted. Mike's points, in order: the Cinder Room is kept for intel on powerful shady men and to run dealings through proxies, not sentiment; "face match logs" don't tie Revaris to Umbra on their own; the tie must be IDs that only the drive maps to names; the mapping can only be confirmed through Revaris's own data, by subpoena once Ed has the drive; the drive would certainly be encrypted; and the player should be able to obtain the key through a mini-game. The paper key in Ray's burn bag and the jigsaw are the answer to the last point.
