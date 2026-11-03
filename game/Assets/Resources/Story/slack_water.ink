// SLACK WATER MOTEL: Backwater, night. Arrival and check-in (A1.1), and turning in.
// Included by northlight.ink. Between arrival and checking_in the game shows the register
// card: you write your name and tick a box, and the game sets player_name.

EXTERNAL sleep()

VAR checked_in = false
// What you did when Dot asked what brings you: "twenty", "shrug", "rocket" or "private".
VAR told_dot = ""


// You pull in at night; the rocket sign across the river is the game's first image.
=== arrival ===
# plate: motel_lot_night
Backwater, after dark. Two hundred dollars and a car that made it.
Across the river, a little neon rocket glows on the far bank.
The motel office still has a light on.
# plate: motel_office_night
# pose: dot_neutral
DOT: Saw your headlights. Come in out of that wind.
DOT: Weekly's cheaper than nightly, and you look like weekly.
DOT: Just your name in the book, hon. Then you can tell me everything.
# insert: motel_register
Dot turns the register around. The pen's on a chain.
-> END


// After the register card: she reads your name and asks what brings you to Northlight.
=== checking_in ===
# plate: motel_office_night
# pose: dot_neutral
DOT: {player_name}. I'm Dot. I own it, I run it, I fix the ice machine.
DOT: So. What brings you to Northlight?
* {bankroll >= 20} [Slide a twenty across the desk. (-$20)]
    ~ spend(20)
    ~ told_dot = "twenty"
    # pose: dot_story
    DOT: Nobody's tipped this desk since the flood. That's a story for another night.
    # pose: dot_neutral
    DOT: It goes in the coffee can. You'll get your coffee out of it.
* [Shrug.]
    ~ told_dot = "shrug"
    DOT: Half of Backwater got here on a shrug. The other half was born here.
* [Ask about the neon rocket across the river.]
    ~ told_dot = "rocket"
    # pose: dot_warning
    DOT: That's over the water, hon.
    DOT: I'd stay on this side of the river tonight. No reason.
* [Tell her that's your business.]
    ~ told_dot = "private"
    DOT: Good answer. Everybody's business is their own here.
- DOT: You're at the far end. Heat works if you're patient with it. # pose: dot_neutral
~ checked_in = true
-> END


// Back in the office: a regular moment. Her stories, and the warnings that matter, come later.
=== office ===
# plate: motel_office_night
# pose: dot_neutral
DOT: {~There you are.|Back again, hon.|Door sticks. Lift and push.}
-> desk

= desk
+ [Lean on the desk and ask what's new.]
    -> news
+ [Head back out to the lot.]
    # pose: dot_neutral
    DOT: Night, hon. Mind the puddles.
    -> END

= news
{shuffle:
- # pose: dot_story
  DOT: Somebody left one bowling shoe in room 3. Just the one. Three years now.
- # pose: dot_warning
  DOT: Don't carry your winnings over the Long Bridge before dawn, hon. No reason.
- # pose: dot_neutral
  DOT: Coffee's on at six. It isn't good, but it's hot and it's free.
- # pose: dot_story
  DOT: Lake wind took the M off my sign once. Slack Water Otel, all winter.
- # pose: dot_neutral
  DOT: Ice machine works if you kick it low. Not high. Low.
}
-> desk


// Going to your room. Night one you can't sleep, which is how you end up at Kasia's (A1.2).
=== turn_in ===
{met_kasia:
    ~ sleep()
    -> END
}
# plate: motel_room_night
The sign buzzes through the curtain. The heat ticks and doesn't come on.
# black
You can't sleep.
-> END
