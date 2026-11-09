// KASIA'S TAP: Backwater, night. Night one (A1.2), Kasia at each break point, her two jobs
// and the back room (A1.3), and the wedding upstairs (A1.4). Included by northlight.ink, which holds the externals and the
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
// The wedding (A1.4). wedding_day is the day number it's booked for, 0 until it is.
VAR wedding_day = 0
VAR wedding_misses = 0
VAR wedding_asked_day = 0
VAR worked_wedding = false
VAR wedding_over = false
// "server" or "busser"; who you danced with ("maid_of_honor", "brides_brother" or nobody)
// sets which versions of the romance characters you meet, and dance_keen is whether you had
// your apron off before Kasia finished asking.
VAR wedding_role = ""
VAR danced_with = ""
VAR dance_keen = false


// Night one: you can't sleep, so you walk to the Tap. A regular moment, not a beat yet.
=== night_one ===
~ met_kasia = true
# plate: kasia_bar_night
The motel bed sags, and the radiator knocks like it wants in.
You can't sleep. Down the block, one bar still has its lights on.
# pose: kasia_neutral
KASIA: In or out, but shut the door. That wind's coming off the lake.
KASIA: I'm Kasia. This is the Tap. My dad's place, before it was mine.
KASIA: Kitchen closed at ten, but there's pierogi left if you're not fussy.
* [Take a stool at the bar.]
    KASIA: Saw your plates when you pulled into the motel. You're from away.
    KASIA: Most people only see Backwater through a windshield on the way to somewhere else.
    KASIA: First night in a new town, nobody sleeps. Eat something. It helps.
    KASIA: May's supposed to be spring. Up here, spring takes its time.
* (machines) [Nod at the machines along the bar.]
    # pose: kasia_joke
    KASIA: Video poker. Licensed by the state. The sticker's right there on the side.
    # pose: kasia_neutral
    KASIA: Backwater voted no on the casinos. A few bar machines is all we allow.
    KASIA: A dollar a hand. Five cards, keep the ones you like, draw the rest.
* {bankroll >= 5} [Leave a five on the bar for a beer. (-$5)]
    ~ spend(5)
    ~ tipped_kasia = true
    KASIA: Beer's three. I'll remember the other two.
    KASIA: I remember every dollar that crosses this bar. Ask anybody in here.
- {not machines:
    KASIA: Those machines along the bar are video poker. A dollar a hand, if you're curious.
  }
KASIA: They run honest. I check them myself every morning before I open.
KASIA: Anybody tells you different, you send them to me.
KASIA: Go on and try one if you like. I'll be down at the taps.
-> END


// Walking in on a regular night: Kasia says hello before you sit down.
=== walk_in ===
# plate: kasia_bar_night
# pose: kasia_neutral
{
- day == wedding_day && hour >= 18:
    KASIA: Family's upstairs already. Apron's on the hook by the stairs.
- day == wedding_day:
    KASIA: Wedding's tonight. Upstairs, six o'clock. Eat something first.
- book > 0:
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
-> wedding_ask.first


// Kasia's wedding (A1.4): she asks in the same talk as the back door. A no, or a booked
// Saturday you don't show up for, leaves one more chance: she asks again on a later day, or
// rebooks (Mike, 2026-11-06). The second no or no-show and the wedding's over without you.
=== wedding_ask ===
# plate: kasia_bar_night
# pose: kasia_asks
KASIA: Another mill wedding's booked the hall. I'm still short a pair of hands.
-> ask

= first
# pose: kasia_neutral
KASIA: Saturday there's a mill wedding upstairs. I'm short a pair of hands.
-> ask

= ask
~ wedding_asked_day = day
KASIA: Eighty, cash, plus tips. Saturday night, six o'clock.
+ [Say you'll be there.]
    ~ wedding_day = next_saturday
    # pose: kasia_joke
    KASIA: Wear shoes you can stand in. The polka goes till one.
    -> END
+ [Tell her weddings aren't your thing.]
    ~ wedding_misses += 1
    {wedding_misses >= 2:
        ~ wedding_over = true
        # pose: kasia_joke
        KASIA: My cousin's kid, then. He breaks glasses, but he says yes.
    - else:
        # pose: kasia_neutral
        KASIA: Suit yourself. Mill families get married a lot.
    }
    -> END


// Walking in after a booked Saturday came and went without you.
=== wedding_missed ===
~ wedding_day = 0
~ wedding_misses += 1
~ lean("flaky")
# plate: kasia_bar_night
# pose: kasia_stern
KASIA: Saturday came. The wedding came. You didn't.
{wedding_misses >= 2:
    ~ wedding_over = true
    # pose: kasia_neutral
    KASIA: Hired my cousin's kid. He broke four glasses. I'm keeping him.
    -> END
}
# pose: kasia_neutral
KASIA: Another mill family's booked the hall. One more try.
-> wedding_ask.ask


// THE WEDDING (A1.4): the hall upstairs, Saturday night, a Backwater mill family's reception.
// You work it as server (more tips) or busser (you overhear more). Mid-shift Kasia brings a
// dance offer from the opposite sex to the register box. Take it keenly or plainly, say you're
// on the clock, or look past them at the other one: only that brings the same-sex offer, which
// Kasia carries quietly (Mike, 2026-11-06). With no answer on the register, both at once.
// Saying no costs nothing. The shift runs till about one.
=== wedding ===
# plate: kasia_wedding_night
String lights over the beams. A polka band tuning up by the cake.
# pose: kasia_neutral
KASIA: Two jobs tonight. Plates or bins.
KASIA: Plates tip better. Bins hear more.
* [Take the tray.]
    ~ wedding_role = "server"
    # pose: none
    Pierogi, kielbasa, a tray of beers. Again. Again.
    An aunt tucks a five in your apron. Then another.
* [Take the bus tub.]
    ~ wedding_role = "busser"
    # pose: none
    Plates, bones, napkins. The tub fills faster than the floor clears.
    At table six, two mill uncles, louder than they think.
    UNCLE: Took Marge to the Starflare for our twenty-fifth. The owner poured the coffee himself.
    OTHER UNCLE: Half empty on a Saturday now. He still pours it.
    By the window, somebody's grandpa squints toward the Bridges.
    GRANDPA: Bridgehead's got the worst machines in town.
    GRANDPA: Fella behind that counter bought a building the year of the vote.
    GRANDPA: Backwater voted no. He rings up gas these days.
- Toasts. A first dance. The band finds its second wind. # pose: none
{
- register_box == "mr": -> offer("maid_of_honor")
- register_box == "ms": -> offer("brides_brother")
- else: -> both
}

= offer(who)
# pose: kasia_asks
{who == "maid_of_honor":
    KASIA: The maid of honor asked if you'd dance. I'll cover your tables.
    Over by the cake table. # pose: maid_of_honor_neutral
- else:
    KASIA: The bride's brother asked if you'd dance. I'll cover your tables.
    Over by the cake table. # pose: brides_brother_neutral
}
+ [Have your apron off before Kasia finishes.]
    ~ dance_keen = true
    -> dance(who, false)
+ [Untie your apron.]
    -> dance(who, false)
+ [Look past {who == "maid_of_honor":her at the bride's brother|him at the maid of honor}.]
    # pose: kasia_neutral
    KASIA: I'll tell {who == "maid_of_honor":her|him} you're on the clock.
    {who == "maid_of_honor":
        -> quiet_offer("brides_brother")
    - else:
        -> quiet_offer("maid_of_honor")
    }
+ [Tell Kasia you're on the clock.]
    # pose: kasia_neutral
    KASIA: I'll tell {who == "maid_of_honor":her|him} you're on the clock.
    -> finish

// The same-sex offer, only if you looked past the first one at them. The asker went through
// Kasia so nobody else would hear, and she keeps it that way, whatever you say.
= quiet_offer(who)
# pose: none
Another tray, another round of toasts. Kasia finds you by the coffee urn.
# pose: kasia_neutral
{who == "maid_of_honor":
    KASIA: The maid of honor asked me to ask. Quietly.
- else:
    KASIA: The bride's brother asked me to ask. Quietly.
}
KASIA: If you'd dance. Yes or no, it stays with me.
{who == "maid_of_honor":
    Over by the cake table. # pose: maid_of_honor_neutral
- else:
    Over by the cake table. # pose: brides_brother_neutral
}
+ [Have your apron off before Kasia finishes.]
    ~ dance_keen = true
    -> dance(who, true)
+ [Untie your apron.]
    -> dance(who, true)
+ [Tell Kasia you're on the clock.]
    -> quiet_no(who)
+ [Tell Kasia you're not that kind of {register_box == "ms":gal|guy}.]
    -> quiet_no(who)

= quiet_no(who)
# pose: kasia_neutral
KASIA: Fair enough. I'll tell {who == "maid_of_honor":her|him} you're on the clock.
-> finish

= both
# pose: kasia_asks
KASIA: The maid of honor and the bride's brother. Both asked if you'd dance.
KASIA: I'll cover your tables.
+ [Dance with the maid of honor.]
    -> dance("maid_of_honor", false)
+ [Dance with the bride's brother.]
    -> dance("brides_brother", false)
+ [Tell Kasia you're on the clock.]
    -> finish

= dance(who, quiet)
~ danced_with = who
{
- quiet && who == "maid_of_honor":
    MAID OF HONOR: Asked Kasia to ask. Easier that way. # pose: maid_of_honor_hand_out
- quiet:
    BRIDE'S BROTHER: Asked Kasia to ask. Easier that way. # pose: brides_brother_hand_out
- who == "maid_of_honor":
    MAID OF HONOR: I don't really dance. Nobody up here does. # pose: maid_of_honor_hand_out
- else:
    BRIDE'S BROTHER: I don't really dance. Nobody up here does. # pose: brides_brother_hand_out
}
# pose: none
{quiet:
    One slow song, near the edge of the floor. Nobody looks twice.
- else:
    One slow song. The string lights sway when the floor does.
}
The song ends.
{
- quiet && who == "maid_of_honor":
    MAID OF HONOR: Thanks. I wasn't sure anybody would say yes. # pose: maid_of_honor_neutral
- quiet:
    BRIDE'S BROTHER: Thanks. I wasn't sure anybody would say yes. # pose: brides_brother_neutral
- who == "maid_of_honor":
    MAID OF HONOR: Thanks. You didn't step on me once. # pose: maid_of_honor_neutral
- else:
    BRIDE'S BROTHER: Thanks. You didn't step on me once. # pose: brides_brother_neutral
}
# pose: none
The band kicks back into a polka.
-> finish

= finish
~ worked_wedding = true
~ wedding_over = true
# pose: none
One o'clock. Chairs up. The bride's asleep with her shoes in her lap.
# pose: kasia_neutral
{wedding_role == "server":
    ~ work(120, 25 - hour)
    KASIA: Eighty from me. Forty in tips. The family says thank you.
- else:
    ~ work(95, 25 - hour)
    KASIA: Eighty from me. Fifteen in tips. The family says thank you.
}
# pose: kasia_joke
KASIA: Go sleep. Sunday's for sleeping.
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
