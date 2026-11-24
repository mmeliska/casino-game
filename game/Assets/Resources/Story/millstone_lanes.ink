// MILLSTONE LANES: Backwater, night. Juan's bowling alley. The coupons (A1.3), then Juan at work,
// the lanes themselves (bowling, the bowling spec), and out the steel door, the back lot and his
// late game (street craps, 10 p.m. to 3 a.m.).
// Included by northlight.ink. The licensed machines inside pay real money, like Kasia's
// (Mike, 2026-11-20); they aren't built yet.

VAR met_juan = false
VAR saw_late_game = false
VAR rolled_with_juan = false

// Bowling. Juan warms to subbing most, to challenges won half as much, to open bowling never
// (Mike, 2026-11-23): his standing is 2 a sub night and 1 a challenge won (proposed numbers).
VAR bowled_at_lanes = false
VAR sub_nights = 0
VAR challenges_won = 0
// What you've bought at the counter, until Pockets: the ball's id, and whether you own shoes.
VAR owns_ball = ""
VAR owns_shoes = false
// The sub ask: the day it came, and the day you said yes (the lanes read it to seat you).
VAR sub_asked_day = -1
VAR sub_tonight_day = -1
// S1: Chuy's ask for the league's shirts, after your first sub night.
VAR shirts_asked = false
VAR chipped_in_shirts = false
// Set by the lanes when you get up, for the lines after.
VAR games_bowled = 0
VAR best_game = 0
VAR challenges_won_tonight = 0
VAR sub_games_won = 0
VAR sub_finished = false

=== function juan_standing() ===
~ return 2 * sub_nights + challenges_won


// Kasia's box of coupons: how you meet Juan, and how you hear about the Starflare.
=== coupons ===
~ met_juan = true
~ carrying_coupons = false
# plate: lanes_front_night
The bowling pin buzzes over a wet lot. Somebody inside is still bowling.
# plate: lanes_lanes_night
Pins crash at the far end. Nobody cheers.
# pose: juan_flat
JUAN: Shoes are at the counter. Rentals are two dollars.
* [Set Kasia's box on the counter.]
    JUAN: Coupons. Good. The league drinks better on her dime.
* [Say Kasia sent you.]
    JUAN: Then that's my box. Set it on the counter.
- JUAN: Tell her thanks. Tell her Tuesday.
* [Ask where a person plays cards around here.]
    JUAN: Kasia's got machines. You've seen those.
* [Watch the bowler on lane six.]
    JUAN: League guy. Practising. Don't stand behind him.
- JUAN: You want real tables, there's the Starflare. Across the river. # pose: juan_shrug
JUAN: The little rocket. Been lit since before I was clearing pins.
# pose: juan_flat
JUAN: Shoes go back on the counter. Not the ball return.
-> END


// Finding him at work, any night after: flat with a casual, and the back door's a shrug.
// Sub for his league, or take his regulars' money, and he warms up (bowling spec).
=== juan ===
# plate: lanes_lanes_night
{
- juan_standing() >= 6:
    # pose: juan_neutral
    JUAN: Lane six is running dry tonight. Move left two boards.
    JUAN: And the late game's out back after the last frame.
- juan_standing() >= 2:
    # pose: juan_flat
    {sub_nights > 0:
        JUAN: Chuy says you filled in.
    - else:
        JUAN: Heard you've been taking the league's money.
    }
    JUAN: Lane six is running dry tonight. Move left two boards.
- else:
    # pose: juan_flat
    JUAN: {~Pinsetter on four's jamming again. Don't stand there.|League's Tuesday. You're not league.|Rentals are two dollars. The ball you pick is your business.|You again. Shoes are where they always are.}
}
+ [Nod at the steel door out back.]
    # pose: juan_shrug
    JUAN: Couldn't tell you.
    -> END
+ [Leave him to his machines.]
    -> END


// THE BACK LOT. Out the steel door: the chalk circle under the lean-to, the bulb, the brick
// wall the dice go off. Juan runs the game from the edge and doesn't bet. Outside the
// alley he says "dice"; inside, it's "the late game".

// Stepping out while the late game's on (10 p.m. to 3 a.m.).
=== back_lot ===
# plate: lanes_backlot
{saw_late_game: -> again}
~ saw_late_game = true
A bare bulb over the steel door. Chalk on the concrete.
Three people around the circle. Dice crack off the brick.
# pose: juan_back_lot
JUAN: Late game. A dollar to twenty a bet.
JUAN: I run it. I don't bet it.
-> END
= again
# pose: juan_back_lot
JUAN: {~Circle's open.|Dollar to twenty. Same as always.|Bulb's on. Game's on.|Mind the grate.}
-> END


// Stepping out any other time: the empty lot.
=== back_lot_empty ===
# plate: lanes_backlot
{~The chalk circle, and nobody in it.|Just the bulb, the dumpster and the chalk.|Nobody out here till after the last frame.}
-> END


// Your first time in the circle: Juan's practice roll, over the table. Set dice: 6, 6, 10, 7.
// His money, pretend; nothing counts and the clock doesn't move (Mike, 2026-11-20).
// Each "# point:" waits for you to press that control (CrapsControls).
=== back_lot_practice ===
# pose: juan_back_lot
JUAN: First roll's practice. It's my money, so nothing you win or lose counts.
JUAN: Whoever's holding the dice is the shooter.
JUAN: Betting right means you're with the shooter. Betting wrong means you're against.
JUAN: You want to shoot, you have to bet right. Put a dollar on right. # point: right_up
JUAN: Your first throw is called the come-out.
JUAN: Roll a seven or eleven on it, and your right bet wins straight off.
JUAN: Roll two, three or twelve, that's craps. Your right bet loses.
JUAN: Roll anything else, and that number's your point.
JUAN: Then you have to roll your point again before you roll a seven.
JUAN: Throw it off the wall. Both dice have to hit the brick. # point: throw
JUAN: Six. So six is your point. Another six before a seven, and your bet wins.
JUAN: Once there's a point, you can bet on the point itself.
JUAN: It pays fair odds. Six comes up a lot, so it pays six for five.
JUAN: Four and ten are hard to roll. Those pay two for one.
JUAN: Put a bet for the six. Bets on the six go in fives. # point: for_up
JUAN: You can bet against the point too. Then the odds run the other way.
JUAN: Now you need that six again, before any seven. Go on. # point: throw
JUAN: Six. You made your point, so your right bet and your point bet both pay.
JUAN: Here's my end. For every ten dollars a bet wins, I take one.
JUAN: Win less than ten, I take nothing.
JUAN: That's the parts money. It keeps the pinsetters going.
JUAN: You made your point, so the dice stay with you. Bet right again. # point: right_up
JUAN: New come-out. Same as before. # point: throw
JUAN: Ten. That's your new point. Ten's a hard one. Only three ways to roll it.
JUAN: You need another ten before a seven. Throw. # point: throw
JUAN: Seven. That's a seven out. Your right bet loses, and you give up the dice.
JUAN: The dice go around the circle. Next one over shoots now.
JUAN: You can still bet on their roll, right or wrong, or sit it out.
JUAN: Put your bets down, then call the throw. Nobody throws till you call it.
JUAN: You can leave the game any time nothing of yours is riding.
JUAN: That was practice. Next roll, it's your own money.
~ rolled_with_juan = true
-> END


// Getting up from the circle: a break point. After three, Juan's picking up.
=== back_lot_get_up ===
# plate: lanes_backlot
# pose: juan_back_lot
{
- hour >= 3 && hour < 22: JUAN: That's three. I'm picking up the chalk.
- broke: JUAN: Circle's for cash. Come back with some.
- sitting_net >= 50: JUAN: Walking off ahead. Don't tell the league.
- else: JUAN: {~Night.|Mind the grate going in.|Same circle tomorrow.}
}
-> END


// THE LANES. Bowling on Juan's lanes: open bowling for the lane fee, a regular's challenge
// between games, and a short team's sub on a Tuesday. The lane plays the game; these are the
// lines around it.

// Your first time on a lane: Juan's frame, over the lane. His board, his numbers; the house
// ball, nothing counting, no clock. Each "# point:" waits for that control (BowlingControls).
=== bowl_first ===
# pose: juan_flat
JUAN: First frame's on me. House ball. Nothing counts.
JUAN: Ball's on the seventh board. Second arrow's your mark.
JUAN: Arrow's swinging. Tap anywhere to stop it. # point: aim
JUAN: Nothing on it. Stop the needle straight up. # point: hook
JUAN: Sixteen miles an hour. Let it go. # point: speed
JUAN: See that? Straight ball finds the head pin and leaves you two corners.
JUAN: Now put something on it. Six boards left. Same arrow. # point: aim
JUAN: Seventy on the hook, curling back to the pocket. # point: hook
JUAN: Same speed. # point: speed
JUAN: That's the lane doing the work.
JUAN: You just have to find where it starts.
~ bowled_at_lanes = true
-> END


// Getting up from the lane, any night but a sub night: a break point.
=== bowl_get_up ===
# plate: lanes_lanes_night
{
- games_bowled == 0: Shoes back on the counter. Not the ball return.
- challenges_won_tonight >= 2: The league's money is in your pocket. Somebody's going to mention it.
- challenges_won_tonight == 1: A league bowler counts out bills onto the seat beside you.
- best_game >= 200: Two hundred on a house shot. The pinsetter clanks like it noticed.
- broke: Not enough left for another game.
- else: {~Pins reset somewhere down the line.|The ball return rumbles. Somebody's still bowling.|Your arm will feel that tomorrow.}
}
-> END


// A short team, on a Tuesday between 6:30 and 7:30: the lanes ask at a break point, once.
=== league_sub_ask ===
~ sub_asked_day = day
# plate: lanes_lanes_night
# pose: juan_flat
JUAN: Chuy's short one. You've got shoes on.
JUAN: Three games. The side pot's five a game.
* [Bowl for Chuy's team.]
    ~ sub_tonight_day = day
    JUAN: Lane four. Don't make him look bad.
* [Shake your head.]
    # pose: juan_shrug
    JUAN: Suit yourself.
- -> END


// After a sub night: the team's word. Finish all three and Juan hears about it.
=== league_sub_done ===
# plate: lanes_lanes_night
{
- not sub_finished: CHUY: Leaving us one short. We'll remember that.
- sub_games_won >= 2: CHUY: Two out of three. You can bowl for us anytime.
- sub_games_won == 1: CHUY: One out of three. Better than bowling short.
- else: CHUY: Rough night. Thanks for filling the seat.
}
-> END


// S1: after your first full sub night, Chuy asks for the league's shirts.
=== bowl_shirts ===
~ shirts_asked = true
# plate: lanes_lanes_night
CHUY: League's forty short for team shirts.
CHUY: Everybody's kicking in what they can.
* {bankroll >= 40} [Hand Chuy forty dollars.]
    ~ spend(40)
    ~ chipped_in_shirts = true
    CHUY: There it is. We'll get you one in your size.
* [Keep your money.]
    CHUY: No harm. Somebody'll cover it.
- -> END
