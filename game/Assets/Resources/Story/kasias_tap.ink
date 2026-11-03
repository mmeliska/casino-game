// KASIA'S TAP: Backwater, night. Night one (A1.2), and Kasia at each break point.
// Included by northlight.ink, which holds the externals and the variables the game sets.
// Tags: #plate (background), #pose (portrait swap), #insert (close-up), #black (text on black)
// Raffy's envelope (the first seed) joins night one once his portrait exists.

// Her credit starts around $50 (kasia.md); it grows or shrinks with the reliable scale later.
VAR book_limit = 50
VAR met_kasia = false
VAR tipped_kasia = false
VAR turned_down_book = false
VAR offered_coupons = false
VAR carrying_coupons = false


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
    KASIA: For amusement only. Says so right on the glass.
    # pose: kasia_neutral
    KASIA: A dollar a hand. How amused you get is up to you.
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
