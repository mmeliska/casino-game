// KASIA'S TAP: Backwater, night. Night one (A1.2), Kasia at each break point, her two jobs
// and the back room (A1.3). Included by northlight.ink, which holds the externals and the
// variables the game sets.
// Tags: #plate (background), #pose (portrait swap), #insert (close-up), #black (text on black)
// Raffy's envelope (the first seed) isn't in night one yet.

// Her credit starts around $50 (kasia.md); it grows or shrinks with the reliable scale later.
VAR book_limit = 50
VAR met_kasia = false
VAR tipped_kasia = false
VAR turned_down_book = false
VAR offered_coupons = false
VAR carrying_coupons = false
VAR offered_bar_back = false
VAR did_bar_back = false
VAR kept_wallet = false
VAR back_room_open = false
VAR met_rosa = false
VAR sat_with_rosa = false


// Night one: you can't sleep, so you walk to the Tap. A regular moment, not a beat yet.
=== night_one ===
~ met_kasia = true
# plate: kasia_bar_night
You can't sleep. Down the block from the motel, one bar's still lit.
# pose: kasia_neutral
KASIA: New face. Kitchen's closed, but there's pierogi if you're not fussy.
* [Take a stool at the bar.]
    KASIA: Saw your plates through the window. You're from away.
    KASIA: Lake wind's turning. Grey till May now.
* [Nod at the machines along the bar.]
    # pose: kasia_joke
    KASIA: Licensed. State sticker's right there on the side.
    # pose: kasia_neutral
    KASIA: A dollar a hand. The rest is up to you.
* {bankroll >= 5} [Leave a five on the bar for a beer. (-$5)]
    ~ spend(5)
    ~ tipped_kasia = true
    KASIA: Beer's three. I'll remember the other two.
- KASIA: Machines run honest. I check them myself.
KASIA: Go on. I'll be at the taps.
-> END


// Walking in on a regular night: Kasia says hello before you sit down.
=== walk_in ===
# plate: kasia_bar_night
# pose: kasia_neutral
{book > 0:
    KASIA: Evening. Book's still open. No rush.
- else:
    KASIA: {~Evening. Machines are warm, beer's cold.|Back again. Your stool's where you left it.|Wind brought you in? Sit anywhere.}
}
-> END


// Getting up from the machine: a break point.
=== get_up ===
# plate: kasia_bar_night
{
- broke && book == 0: -> credit
- broke: -> tapped_out
- book > 0 && bankroll >= book + 20: -> settle_up
- sitting_net >= 50: -> up_on_the_night
- else: -> quiet_night
}

= credit
{turned_down_book: -> credit_again}
# pose: kasia_stern
KASIA: Machine cleaned you out? Happens to the regulars too.
# pose: kasia_neutral
KASIA: I'll put you on the book. Fifty. No interest, no due date.
-> credit_choice

= credit_again
# pose: kasia_neutral
KASIA: Offer's still good. Fifty on the book.
-> credit_choice

= credit_choice
* [Put it on the book. (+${book_limit}, owed)]
    ~ borrow(book_limit)
    # insert: kasia_ledger
    KASIA: Your tab's in the book. The book never forgets. Neither do I.
    -> END
* [Shake your head and push the stool in.]
    ~ turned_down_book = true
    KASIA: Suit yourself. Door's open tomorrow.
    -> END

= tapped_out
# pose: kasia_stern
KASIA: Book's still open. I don't stack pages.
KASIA: Get some sleep. The machines will be here.
-> END

= settle_up
# pose: kasia_neutral
KASIA: You're up tonight. The book's still open, though.
* [Count out what you owe. (-${book})]
    ~ repay(book)
    # insert: kasia_ledger
    KASIA: Square. I'll draw the line once I know you're staying.
    -> END
* [Tell her you'll square it tomorrow.]
    # pose: kasia_joke
    KASIA: Tomorrow. Everybody's favorite day of the week.
    -> END

= up_on_the_night
# pose: kasia_joke
KASIA: Look at you. Don't tell the regulars, they'll want your stool.
-> END

= quiet_night
# pose: kasia_neutral
KASIA: {~Lake wind's coming up. Walk back quick.|Snow by the weekend, the regulars say. They're always wrong.|Same stool tomorrow. It's not going anywhere.}
-> END


// The coupons (A1.3), one of her two jobs: offered when you walk in after night one.
// Taking the box opens Millstone Lanes; it's yours to carry until you meet Juan.
=== coupon_job ===
# plate: kasia_bar_night
{offered_coupons: -> again}
~ offered_coupons = true
# pose: kasia_asks
KASIA: You. You've got legs and nowhere to be. Do me a favor.
# pose: kasia_neutral
KASIA: Box of drink coupons for Juan's Tuesday league. Good here, on me.
KASIA: Millstone Lanes, up the block. Follow the bowling pin.
-> take_it

= again
# pose: kasia_neutral
KASIA: Juan's box is still behind the bar.
-> take_it

= take_it
* [Tuck the box under your arm.]
    ~ carrying_coupons = true
    # pose: kasia_joke
    KASIA: Don't let the regulars see. They'll want a league.
    -> END
* [Tell her you came to play.]
    KASIA: Box'll keep. Machines first, then.
    -> END


// The bar-back shift (A1.3), her other job: offered when you walk in once Juan has his box.
// Sixty cash for the shift, which takes about four hours.
=== bar_back_job ===
# plate: kasia_bar_night
{offered_bar_back: -> again}
~ offered_bar_back = true
# pose: kasia_asks
KASIA: Juan got his box. Now I need hands. Mine are full.
# pose: kasia_neutral
KASIA: Bar-back. Cases, ice, glasses. Sixty, cash.
-> take_it

= again
# pose: kasia_neutral
KASIA: Apron's on the hook if you want the shift.
-> take_it

= take_it
+ [Tie on the apron.]
    -> shift
+ [Tell her you came to play.]
    # pose: kasia_joke
    KASIA: Machines first. The ice will wait. It's ice.
    -> END

= shift
# pose: none
Cases up from the basement. Ice into the well. Glasses, glasses.
# pose: kasia_joke
KASIA: You lift like a mill kid. That's a compliment.
# pose: none
Near close, a regular climbs off his stool. Something hits the floor.
# insert: kasia_wallet
A fat wallet, half under the rail. He's already at the door.
* [Run it out to him on the sidewalk.]
    ~ lean("straight")
    ~ lean("flaky")
    # pose: none
    He takes it without looking at you. The ice well's half melted.
    # pose: kasia_stern
    KASIA: That was Stan. I'd have kept it for him. Ice, please.
    -> paid
* [Thumb out two twenties, then hand Kasia the wallet. (+$40)]
    ~ earn(40)
    ~ kept_wallet = true
    ~ lean("crooked")
    ~ lean("reliable")
    # pose: kasia_neutral
    KASIA: Stan's. I'll keep it behind the till. You never left the ice.
    -> paid

= paid
~ work(60, 4)
~ did_bar_back = true
KASIA: Sixty. You showed up and didn't break anything.
-> END


// Both jobs done: once, as you walk in, Kasia nods you toward the back door (A1.3).
=== back_room_invite ===
~ back_room_open = true
# plate: kasia_bar_night
# pose: kasia_neutral
KASIA: Box to Juan. A shift on the ice. You keep showing up.
# pose: kasia_asks
KASIA: Back door. There's a game. Rosa deals. Mind her.
-> END


// KASIA'S BACK ROOM: night, one lamp. Rosa deals blackjack, a dollar to fifty.
// Walking in: the first time, you meet her.
=== back_room ===
# plate: kasia_backroom_night
{met_rosa: -> again}
~ met_rosa = true
One lamp over green felt. Folding chairs. Beer cases to the ceiling.
# insert: kasia_meeting_sign
The door swings shut behind you.
# pose: rosa_neutral
ROSA: Kasia's friend. Sit. A dollar minimum, fifty max.
ROSA: I deal the rules as written. Ask, and I'll tell you them.
-> END

= again
# pose: rosa_neutral
ROSA: {~Seat's open.|Same rules as last time.|A dollar minimum. Fifty max.}
-> END


// Your first hand at her table: a practice hand on the house, played over the table so you
// see the cards. She coaches it, and each "# point:" line is an instruction: you press that
// button for real (BlackjackControls names them). Your eighteen against her ace, a six under it,
// so the insurance she has you take loses: she shows how it works and leaves the call to you.
=== rosa_first_hand ===
# pose: rosa_neutral
ROSA: First hand's practice. On the house. Nothing changes hands.
ROSA: Deal yourself in. # point: deal
ROSA: Your eighteen. My ace, face up.
ROSA: Ace up, the rules say I offer insurance.
# pose: rosa_teaching
ROSA: It's a side bet, half your bet, that I have blackjack.
ROSA: If I do, it pays two to one. Your hand still loses.
ROSA: Take it this time. See how it works. # point: insurance
# pose: rosa_neutral
ROSA: Now your hand. Eighteen against an ace. Stand. # point: stand
ROSA: Six under the ace. No blackjack, so the insurance is gone.
ROSA: Seventeen. I stand on seventeen. Your eighteen wins.
# pose: rosa_teaching
ROSA: Ace up, the house makes blackjack under a third of the time.
# pose: rosa_one_finger
ROSA: Insurance or not is your call. Every time.
# pose: rosa_neutral
ROSA: That's practice. Now it's your money. Bets.
~ sat_with_rosa = true
-> END


// Getting up from her table: a break point.
=== rosa_get_up ===
# plate: kasia_backroom_night
# pose: rosa_neutral
{
- broke: ROSA: Kasia keeps the book. Not me.
- sitting_net >= 50: ROSA: Getting up ahead. Most people can't.
- else: ROSA: {~Same game tomorrow.|Push your chair in.|Same rules tomorrow.}
}
-> END
