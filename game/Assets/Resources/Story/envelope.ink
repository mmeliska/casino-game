// RAFFY'S ENVELOPE COMES DUE (A1.5): Ray asks around. It crosses places, so it has its own
// file: Ray at Kasia's Tap, Dot's warning at the Slack Water (slack_water.ink), Raffy hiding at
// Millstone Lanes, your room, and Ray's spot at the Flare's bar. Included by northlight.ink.
//
// The envelope held $2,500 of Ray's; Raffy took $450 for his mom's rent and tipped you $50, so
// you hold $2,000 and Ray is $500 short whatever you do. Four ends (Mike, 2026-10-29): return
// it to Ray, keep it, cover the missing $500 for Raffy, or keep it and tell Raffy it's settled.
// The thumb drive stays with you whatever you pick. Ray only ever says "the envelope".

// The lie, found out: the $2,000 plus Ray's interest, which goes on what you owe him
// (proposed: ten percent; Mike, 2026-11-25 scoped it as its own debt until loans are built).
CONST ENVELOPE_DEBT = 2200

// Ray has been asking (the day he first did).
VAR ray_asking = false
VAR ray_asking_day = 0
VAR dot_warned_envelope = false
VAR found_raffy = false
// How it was settled: "returned", "kept", "helped" or "lied"; empty until it is.
VAR envelope_settled = ""
VAR envelope_settled_day = 0
// You told Ray where Raffy was hiding.
VAR gave_up_raffy = false
// What Raffy still owes you (the help path), and the day of his last payment.
VAR raffy_owes = 0
VAR raffy_paid_day = 0
VAR raffy_stopped_paying = false
VAR raffy_paid_off = false
// The lie path: Ray found Raffy, Raffy named you, and Ray came to your table.
VAR ray_named_you = false
// Ray never trusts you again (the lie found out).
VAR ray_burned = false
// Kasia has told you Raffy's mother called the bar.
VAR kasia_mom_called = false
// The drive, out of the envelope and in your pocket; plugged in at the motel (online, so it
// phoned home: Wren learns a decommissioned serial is alive, not who has it).
VAR has_drive = false
VAR drive_plugged_in = false
VAR drive_phoned_home = false


=== function settle(how) ===
~ envelope_settled = how
~ envelope_settled_day = day
~ has_envelope = false
~ opened_envelope = true
~ has_drive = true

// Dot stops warning you about the bridge if you kept Raffy's cash or gave him up.
=== function dot_warns_bridge() ===
~ return envelope_settled != "kept" && envelope_settled != "lied" && not gave_up_raffy


// Ray, first on screen: at Kasia's Tap, asking about "the envelope". Kasia gives nothing, then
// tells you where a kid in a hood has been sweeping.
=== ray_asks ===
~ ray_asking = true
~ ray_asking_day = day
# plate: kasia_bar_night
A big man at the bar, loud, buying rounds nobody asked for.
# pose: ray_neutral
RAY: Buddy! A new face. I love a new face. Ray.
RAY: I'm looking for a kid. Hoodie, one earbud, talks fast.
RAY: He's got an envelope of mine. Sentimental value.
# pose: kasia_stern
KASIA: I told him. We don't keep envelopes here.
# pose: ray_pitch
RAY: Course not. But if one turns up, I'm at the Starflare bar most nights.
RAY: A friend who finds my envelope never pays full price again. Ask anybody.
* [Say you haven't seen any envelope.]
    ~ lean("crooked")
    # pose: ray_neutral
    RAY: No? Shame. Keep your eyes open, buddy.
* [Say nothing and order a beer. (-$3)]
    ~ spend(3)
    # pose: ray_neutral
    RAY: Strong and silent. I like that too.
* [Slide Raffy's envelope across to him.]
    -> ray_takes_it
- # pose: kasia_neutral
After he goes, Kasia wipes the same spot on the bar for a while.
KASIA: Juan's had a new kid sweeping lanes this week. Hood up, all hours.
KASIA: Not my business. Maybe not yours either.
-> END


// Back at the Slack Water once Ray's been asking: Dot warns you, sideways.
=== dot_envelope_warning ===
~ dot_warned_envelope = true
# pose: dot_warning
DOT: Loud fella came by asking after an envelope. Called me "friend".
DOT: I told him I run a motel, not a post office.
DOT: I'd stay on this side of the river tonight, hon. No reason.
-> office.desk


// Millstone Lanes, while Ray's asking and you still hold it: Raffy, hiding with Juan.
=== raffy_hiding ===
~ found_raffy = true
# plate: lanes_lanes
A kid in a hood sweeps lane six. He sees you and freezes.
# pose: raffy_hiding
RAFFY: Oh no. Oh no no. Did he send you? Is he outside?
# pose: juan_flat
JUAN: He's sweeping. That's all he's doing here.
# pose: raffy_nervous_laugh
RAFFY: I took it outta his car. I thought it was just collection money. I swear.
RAFFY: Mom's rent was due. I took four-fifty, gave you fifty. Dumb. So dumb.
RAFFY: He's gonna want twenty-five hundred. I don't have five hundred. I don't have five.
* {bankroll >= 500} [Cover the missing $500 and send it all back to Ray. (-$500)]
    -> helped
* [Tell him you'll take Ray the envelope yourself.]
    # pose: raffy_hiding
    RAFFY: Okay. Okay. Don't tell him where I am? Please?
    -> END
* [Tell him it's settled, and he owes you $500.]
    -> lied
* [Tell him you're keeping the cash.]
    -> kept

= helped
~ spend(500)
~ settle("helped")
~ raffy_owes = 500
~ raffy_paid_day = day
~ lean("generous")
# pose: juan_flat
JUAN: I'll drive him over. Ray won't start anything with me in the car.
# pose: raffy_nervous_laugh
RAFFY: I'll pay you back. Every dollar. A little at a time, but every dollar.
# insert: raffy_envelope_open
Under the cash: a little black thumb drive. It stays with you.
-> END

= lied
~ earn(2000)
~ settle("lied")
~ raffy_owes = 500
~ raffy_paid_day = day
~ lean("crooked")
# pose: raffy_nervous_laugh
RAFFY: Settled? For real? You paid him? Oh man. Oh man, thank you.
RAFFY: Five hundred. I'll get it to you. I can go home tonight.
# insert: raffy_envelope_open
Under the cash: a little black thumb drive. It stays with you.
-> END

= kept
~ earn(2000)
~ settle("kept")
~ lean("crooked")
# pose: raffy_hiding
RAFFY: ...
RAFFY: Yeah. Okay. Okay.
# pose: juan_flat
JUAN: Door's that way.
# insert: raffy_envelope_open
Under the cash: a little black thumb drive. It stays with you.
-> END


// Your room, while Ray's asking: count the cash into your own wallet.
=== keep_envelope ===
~ earn(2000)
~ settle("kept")
~ lean("crooked")
# plate: motel_room
# insert: raffy_envelope_open
You count it into your wallet. Two thousand, even. Nobody's coming for it but Ray.
The little black thumb drive goes in your pocket.
-> END


// Ray's spot at the Flare's bar, while you still hold it.
=== ray_return ===
# plate: flare_bar
# pose: ray_neutral
RAY: Buddy! You came to see me. I'm touched. What are we drinking?
-> ray_takes_it

// Handing it over, at the Tap or the Flare. He counts it, looks at you, and says nothing.
=== ray_takes_it ===
{not opened_envelope:
    # insert: raffy_envelope_open
    You tear it open. Cash, and under it a little black thumb drive.
}
You hand him the cash. The drive stays in your pocket.
# pose: ray_flat
RAY: ...
He counts it under the bar, fast. Then he looks at you. A long time.
* [Say that's all there was.]
* {found_raffy} [Tell him the kid's sweeping lanes at Juan's.]
    ~ gave_up_raffy = true
    ~ lean("ruthless")
- ~ settle("returned")
~ lean("reliable")
He folds the cash into his coat and turns back to the bar.
-> END


// The lie found out, a few days on: Ray found Raffy, Raffy named you, and Ray comes to your table.
=== ray_names_you ===
~ ray_named_you = true
~ ray_burned = true
~ raffy_owes = 0
# plate: flare_floor
# pose: ray_hands_in_pockets
RAY: Buddy. We need to talk about an envelope.
RAY: Found the kid. He says, "They said they paid you." Funny. I don't feel paid.
RAY: Two thousand. Plus what waiting cost me. Twenty-two hundred.
~ owe_ray(ENVELOPE_DEBT)
RAY: I'll put it on your tab. Friends get tabs.
# pose: ray_flat
RAY: ...
# black
Nobody sees Raffy after that.
-> END


// After you kept the cash, lied, or gave him up: Raffy's mother called the Tap.
=== raffys_mother ===
~ kasia_mom_called = true
# plate: kasia_bar_night
# pose: kasia_stern
KASIA: Raffy's mother called the bar. Twice. Nobody's seen him.
KASIA: I told her you were the last one he talked to.
# pose: kasia_neutral
KASIA: Beer's cold. If you're staying.
-> END


// Raffy pays you back a little at a time, through Juan, then the last of it in person at the
// Lanes (the help path). On the lie path he pays a little too, until Ray finds him.
=== raffy_pays ===
~ raffy_paid_day = day
{raffy_owes > 100: -> through_juan}
-> last_payment

= through_juan
# plate: lanes_lanes
# pose: juan_flat
JUAN: Kid left this for you. Said you'd know what it's for.
A small envelope from Raffy. A hundred dollars inside, in fives and tens.
+ [Take it.]
    ~ earn(100)
    ~ raffy_owes -= 100
    JUAN: He's delivering again. Bike's fixed.
+ [Tell Juan the kid can stop paying.]
    ~ raffy_stopped_paying = true
    ~ raffy_owes = 0
    ~ lean("generous")
    # pose: juan_shrug
    JUAN: I'll tell him. He won't like it.
- -> END

= last_payment
~ raffy_paid_off = true
# plate: lanes_lanes
# pose: raffy_last_payment
Raffy, no hood, at the end of lane six. He waits till you're close.
{raffy_stopped_paying:
    RAFFY: Juan said stop. So, just the fifty.
    ~ earn(50)
- else:
    RAFFY: That's all of it. The fifty too.
    ~ earn(raffy_owes + 50)
}
~ raffy_owes = 0
RAFFY: The one I gave you that first night was Ray's. This one's mine.
-> END


// The drive, plugged into the motel's old laptop: a locked volume, and nothing else.
=== plug_in_drive ===
~ drive_plugged_in = true
~ drive_phoned_home = true
# plate: motel_room
# insert: raffy_drive_locked
A locked volume. REVARIS, and a long serial number. It wants a key.
No key. You pull it out and put it back in your pocket.
-> END
