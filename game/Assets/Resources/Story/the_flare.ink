// THE STARFLARE CLUB ("the Flare"): the Levee waterfront, where the river meets the canal,
// across the water from the Slack Water. Gus's fading rocket-age casino: out front under the
// rocket sign, the floor, the bar, the lounge, and Gus's office upstairs. Included by
// northlight.ink. Plates name the view (flare_front, flare_floor, flare_bar, flare_office);
// the game picks the day or night plate. Once you've seen the notice (A1.7) the floor is its
// decline plate, flare_floor_night_decline.
//
// The Flare half of A1.6 (coffee with Gus, the spaceman's boot) and A1.7 over a few visits:
// Pete at the pit, Jalen at the bar, the decline and Gus's secret, Pete saying your name.
// Dot's side of R1 is at her desk (slack_water.ink).

VAR met_gus = false
VAR met_gus_day = 0
VAR rubbed_boot = false
VAR tipped_jar = false
// Visits on separate days, and the day of the last one: A1.7 plays out a beat a visit.
VAR flare_visits = 0
VAR flare_last_day = 0
// Blackjack sittings at the Flare, counted at each get-up, toward Pete's name scene.
VAR flare_sittings = 0
VAR met_pete = false
VAR met_pete_day = 0
VAR met_jalen = false
VAR met_jalen_day = 0
// Gus's secret (A1.7): the past-due power notice on his desk (Mike, 2026-11-25). You saw it;
// then Pete asks, and you keep it or tell him.
VAR saw_notice = false
VAR saw_notice_day = 0
VAR kept_gus_secret = false
VAR told_pete = false
// The A1.7 beat: Pete says the name from Dot's register in place of "kid".
VAR pete_said_name = false
// R1: Gus and Dot have each asked you after the other (once each, so far).
VAR gus_asked_after_dot = false
VAR dot_asked_after_gus = false
// Gus's trust: the standing every key person has, moved by small things (Mike, 2026-10-21).
VAR gus_trust = 0


// Jalen's honorific follows the box on Dot's register: ", sir", ", ma'am", or nothing.
=== function sir() ===
{
- register_box == "mr":
    ~ return ", sir"
- register_box == "ms":
    ~ return ", ma'am"
- else:
    ~ return ""
}

// What Pete calls you: "kid", until he's said your name.
=== function kid() ===
{pete_said_name:
    ~ return player_name
}
~ return "kid"

// Pete's rating, read off what you carry: he never shows the card.
=== function rating() ===
{
- bankroll >= 1000:
    ~ return "fifty-dollar"
- bankroll >= 400:
    ~ return "twenty-five-dollar"
- else:
    ~ return "ten-dollar"
}

=== function evening() ===
{hour >= 17 || hour < 5:
    ~ return "Evening"
}
~ return "Afternoon"


// Over the bridge, the Flare half (A1.6): Gus pours you a coffee under the rocket sign and
// shows you the spaceman's boot. Nothing is wrong yet, but the floor is half empty.
=== flare_first ===
~ met_gus = true
~ met_gus_day = day
# plate: flare_front
Over the Long Bridge, the rocket sign. Coral and teal, humming over the water.
A big man in a red dinner jacket stands under it, holding two mugs.
# pose: gus_welcome
GUS: New face. Good. We could use a few more of those.
GUS: I'm Gus. This is my dad's place. Was. Is. You know what I mean.
GUS: Coffee? Pot's older than me, but it's the best in the Levee.
GUS: And you are?
* [Tell him your name and take the mug.]
    # pose: gus_neutral
    GUS: {player_name}. Good. I'll remember it. I remember everybody.
    GUS: Comes with the jacket.
* [Take the mug and leave it at that.]
    # pose: gus_neutral
    GUS: Fair enough. Coffee first, names later. That was my dad's rule too.
- # pose: gus_old_days
GUS: See the fella by the doors? My dad bought him before I was born.
# insert: flare_spaceman_boot
GUS: Regulars rub his boot on the way in. For luck. Look at that toe shine.
* [Rub the spaceman's boot.]
    ~ rubbed_boot = true
    # pose: gus_welcome
    GUS: There you go. Now you're a regular.
    GUS: Took me a while. I was nearly born at table six.
* [Leave the spaceman be.]
    # pose: gus_neutral
    GUS: Suit yourself. He's patient. He'll still be waving next time.
- # plate: flare_floor
Inside: a navy carpet full of coral starbursts. Half the stools are empty.
# pose: gus_old_days
GUS: You should've seen this floor when the band played. Line out the door.
GUS: Anyway.
# pose: gus_neutral
GUS: Blackjack's five to a hundred. Poker machines by the bar, a dollar a credit.
GUS: Pete runs the pit. He'll find you. He finds everybody.
* [Drop a five in the busboys' jar by the door. (-$5)]
    ~ spend(5)
    ~ tipped_jar = true
    ~ lean("generous")
    # pose: gus_neutral
    GUS: One of them's saving for school. He won't thank you. He'll remember, though.
* [Ask where everybody is.]
    # pose: gus_eyes_down
    GUS: ...
    # pose: gus_old_days
    GUS: My dad used to say a slow {evening() == "Evening":night|afternoon} is for the regulars.
    GUS: Anyway. Coffee's free as long as you're here.
* [Head for the tables.]
    # pose: gus_welcome
    GUS: {rubbed_boot:You rubbed the boot. You'll be fine.|Good luck. The boot's still there if you change your mind.}
- -> END


// Out front, any visit after the first: rub the boot on the way in, like the regulars.
=== spaceman ===
~ rubbed_boot = true
# plate: flare_front
# insert: flare_spaceman_boot
{~Cold chrome. The toe's worn bright, like a coin.|You rub the boot. The spaceman keeps waving.|Somebody's rubbed it already tonight. It's still warm.}
-> END


// Meeting Pete (A1.7): at the pit the first time you get up from blackjack, or at the door on
// a later visit if you haven't sat down. Count everything, say nothing.
=== pete_first ===
~ met_pete = true
~ met_pete_day = day
# plate: {saw_notice:flare_floor_night_decline|flare_floor}
A stocky man in a three-piece suit. A gold rocket pin on his lapel.
# pose: pete_neutral
PETE: Pete. I run the pit.
# insert: pete_rating_card
He writes on a little card. He doesn't show you.
PETE: Kid. Your rating says you're a {rating()} player. Play like one.
{
- sitting_net > 0:
    PETE: Up {sitting_net} tonight. Walk out with it. Most don't.
- sitting_net < 0:
    PETE: Down tonight. That's the five-dollar table. It'll still be here.
}
# pose: pete_neutral
PETE: Count everything, kid. Say nothing. Worked for forty years.
* [Ask what he wrote on the card.]
    # pose: pete_eyes_down
    PETE: ...
    # pose: pete_neutral
    PETE: Walt, seat one, thirty years. Twenty-five-dollar player.
    PETE: Lorraine plays the Rocket Wheel. Ten. Augie's a five who bets like a hundred.
* [Tell him your name.]
    # pose: pete_neutral
    PETE: Kid's fine. For now.
* [Nod.]
    # pose: pete_appraising
    PETE: Good. You listen.
- -> END


// Meeting Jalen (A1.7): a later visit, at the bar. The notebook is closed when you walk up.
=== jalen_first ===
~ met_jalen = true
~ met_jalen_day = day
# plate: flare_bar
A young busboy clears the bar, quick and quiet. A rocket on his vest.
# pose: jalen_neutral
JALEN: {evening()}{sir()}. I'm Jalen. Can I bring you anything?
{tipped_jar:
    JALEN: Gus says you're the one with the jar. I won't make a thing of it.
}
# insert: jalen_notebook
By his tray: a little black notebook, a handheld game, a fat textbook.
* [Ask what's in the notebook.]
    # pose: jalen_caught
    JALEN: Oh. Just orders{sir()}. Who has what.
    # pose: jalen_neutral
    JALEN: Sorry. You were saying.
* [Nod at the handheld.]
    # pose: jalen_gaming
    JALEN: Nah, this boss is impossible. You gotta cheese the second phase.
    # pose: jalen_neutral
    JALEN: Sorry. Break's over.
* [Ask who's worth knowing here.]
    # pose: jalen_neutral
    JALEN: Miss Lorraine drinks rye when she's winning, tea when she's not.
    JALEN: Tonight's tea. I'd let her have the Wheel to herself.
* [Leave a five on his tray. (-$5)]
    ~ spend(5)
    ~ lean("generous")
    # pose: jalen_neutral
    JALEN: Thank you{sir()}. You didn't have to.
- -> END


// Gus's secret (A1.7): a later visit. The decline shows; Gus takes you up to his office for
// the view, and you see the notice on his desk. He asks how Dot is (R1). Back on the floor,
// Pete asks about the view: keep Gus's secret, or tell Pete.
=== flare_decline ===
~ saw_notice = true
~ saw_notice_day = day
# plate: flare_front
# insert: flare_rocket_sign
Half the rocket's tubes are dark tonight. It flickers, like it's thinking.
# plate: flare_floor_night_decline
Inside, half the starbursts are out. One table sits behind a velvet rope.
# insert: flare_bucket
A bucket on the carpet. Every few seconds, a drip.
# pose: gus_welcome
GUS: {player_name}! Come up a minute. Best view in the Levee's in my office.
# plate: flare_office
Wood panels, old photographs, a window full of river.
# pose: gus_old_days
GUS: My dad picked this office for the window. You can see the whole bend.
# insert: flare_notice
On the desk: the power company. PAST DUE in red, and a shutoff date.
# pose: gus_eyes_down
GUS: ...
# insert: gus_mug
He sets his mug down on top of it.
# pose: gus_eyes_down
GUS: That's nothing. Paperwork. Don't mention it to Pete, all right? He worries.
* [Say you didn't see anything.]
    # pose: gus_neutral
    GUS: Good. Good.
* [Ask how far behind he is.]
    # pose: gus_old_days
    GUS: You know, my dad lit the first neon sign on this river.
    GUS: People drove out to see it. Sat on their hoods with a thermos. Anyway.
* [Tell him Pete should hear it from him.]
    ~ lean("straight")
    # pose: gus_eyes_down
    GUS: Pete knows the numbers. He doesn't need the bills on top of them.
- -> window

// Gus's side of R1: the Slack Water's sign is across the water from his window.
= window
~ gus_asked_after_dot = true
# plate: flare_office
# pose: gus_neutral
GUS: That sign across the water. The Slack Water. You're staying there?
GUS: The lady at the desk. Dot. She doing all right?
* [Tell him she runs the place like a ship.]
    # pose: gus_old_days
    GUS: She would. She always did. Everybody fed, nobody's business but their own.
* [Tell him she's fine.]
    # pose: gus_neutral
    GUS: Fine. Good. That's good.
* {dot_asked_after_gus} [Tell him she asked how he was.]
    ~ gus_trust += 1
    # pose: gus_eyes_down
    GUS: ...
    GUS: Did she.
- # pose: gus_neutral
GUS: Anyway. Coffee?
-> pete_asks

= pete_asks
# plate: flare_floor_night_decline
# pose: pete_neutral
Back down on the floor, Pete's at the pit. He doesn't look up.
PETE: He show you the view?
* [Tell Pete about the notice on Gus's desk.]
    ~ told_pete = true
    ~ gus_trust -= 1
    ~ lean("straight")
    # pose: pete_eyes_down
    PETE: ...
    PETE: Go play your hand, kid.
* [Tell him it's a good view.]
    ~ kept_gus_secret = true
    ~ gus_trust += 1
    ~ lean("reliable")
    # pose: pete_appraising
    PETE: It is.
    # pose: pete_neutral
    PETE: Count everything, kid. Say nothing.
- -> END


// Becoming a regular (A1.7): after a few sittings, at the pit when you get up. The same line
// as the first night, with the name from Dot's register in place of "kid".
=== pete_name ===
~ pete_said_name = true
# plate: flare_floor_night_decline
# insert: pete_rating_card
# pose: pete_appraising
PETE: {player_name}. Your rating says you're a {rating()} player. Play like one.
{met_jalen:
    # pose: jalen_caught
    JALEN: Excuse me{sir()}. He doesn't do that.
}
# pose: pete_neutral
PETE: Same seat next time. I'll know where to find you.
-> END


// Getting up from blackjack, once you know Pete: a word from the pit.
=== flare_get_up ===
# plate: {saw_notice:flare_floor_night_decline|flare_floor}
# pose: pete_neutral
{
- broke:
    PETE: Tapped. Go home, {kid()}. Come back with a bankroll.
- sitting_net > 0:
    PETE: {~Up {sitting_net}. Cage is that way, {kid()}.|Up {sitting_net}. Don't give it to the machines.}
- sitting_net < 0:
    PETE: {~Shoe goes both ways, {kid()}. Tonight it went one.|Down. The shoe doesn't know you yet, {kid()}.}
- else:
    PETE: Even. Nobody remembers even, {kid()}.
}
-> END


// Getting up from the poker machine by the bar.
=== flare_machine_get_up ===
# plate: flare_bar
{
- broke:
    The machine plays a little rocket tune for nobody. You're out.
- met_jalen:
    # pose: jalen_neutral
    JALEN: {~Refill{sir()}?|Mind the stool. It spins.|Good luck tonight{sir()}.}
- sitting_net > 0:
    The machine plays a little rocket tune. Nobody looks up.
- else:
    The screen glows. The bartender polishes the same glass.
}
-> END


// The lounge: a regular moment. Somebody's always at the piano, playing to empty booths.
=== flare_lounge ===
# plate: flare_lounge
{~Somebody's playing something slow to six empty booths.|Stars painted on the ceiling. Somebody took their time on those.|The piano player nods at you and doesn't stop.}
-> END
