// SLACK WATER MOTEL: Backwater, night. Arrival and check-in (A1.1), and turning in.
// Included by northlight.ink. Between arrival and checking_in the game shows the register
// card: you write your name and tick a box, and the game sets player_name.

EXTERNAL sleep()

VAR checked_in = false
// What you did when Dot asked what brings you: "twenty", "shrug", "rocket" or "private".
VAR told_dot = ""
// Night one (A1.2): you've lain awake and remembered a bar from the drive in, and Dot has
// told you it's Kasia's Tap and sent you to your phone's map, which glows until you open it.
VAR cant_sleep = false
VAR asked_dot_about_bar = false


// You pull in at night; the rocket sign across the river is the game's first image.
=== arrival ===
# plate: motel_river_night
Backwater, after dark. Two hundred dollars and a car that made it.
Across the river, a little neon rocket glows on the far bank.
The motel office still has a light on.
# plate: motel_office
# pose: dot_neutral
DOT: Saw your headlights. Come in out of that wind.
DOT: Weekly's cheaper than nightly, and you look like weekly.
DOT: Just your name in the book, hon. Then you can tell me everything.
# insert: motel_register
Dot turns the register around. The pen's on a chain.
-> END


// After the register card: she reads your name and asks what brings you to Northlight.
=== checking_in ===
# plate: motel_office
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
# plate: motel_office
{cant_sleep && not met_kasia: -> the_bar}
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

// Night one, sleepless: she points you at the bar down the block, and at your phone.
= the_bar
{asked_dot_about_bar: -> the_bar_again}
~ asked_dot_about_bar = true
# pose: dot_neutral
DOT: Can't sleep? That radiator's louder than it is warm.
DOT: The bar up the block? That's Kasia's Tap. She keeps late hours.
# pose: dot_story
DOT: Cold beer, hot pierogi, and nobody asks your business.
# pose: dot_neutral
DOT: Check your phone, hon. The map will show you the way.
-> END

= the_bar_again
# pose: dot_neutral
DOT: Kasia's Tap, hon. It's on your phone's map. Go on.
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


// Going to your room on night one: you can't sleep, and you remember a bar you passed on the
// drive in, so you go ask Dot (A1.2). After that night the room is a place screen (SlackWaterMotel):
// sleep, TV or the internet, by the hour.
=== turn_in ===
# plate: motel_room_night
{cant_sleep:
    Still awake. The radiator's still knocking.
    {asked_dot_about_bar:
        Kasia's Tap is on your phone's map.
    - else:
        That bar you passed on the drive in. Maybe ask Dot about it.
    }
    -> END
}
~ cant_sleep = true
The motel bed sags, and the radiator knocks like it wants in.
The motel sign buzzes outside the window.
# black
You can't sleep.
You passed a bar on the drive in, up the block. It looked open.
Maybe you should go ask Dot about the bar.
-> END


// Opening Raffy's envelope (A1.2), from your room, any time after he leaves it with you. The
// cash stays in the envelope: what happens to it is settled when Ray comes asking (A1.5).
// The drive is just a drive until then.
=== open_envelope ===
~ opened_envelope = true
# plate: motel_room
The flap gives on the second try. It was never sealed very well.
# insert: raffy_envelope_open
Twenties and fifties under a rubber band. You count twice. Two thousand even.
Under the cash, a small black thumb drive. No label.
# black
{day > 5:
    It's past Friday. Nobody's come for it.
- else:
    Hold this till Friday, he said. Don't open it.
}
-> END
