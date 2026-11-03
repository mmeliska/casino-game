// MILLSTONE LANES: Backwater, night. Juan's bowling alley. The coupons (A1.3), then Juan at work.
// Included by northlight.ink. The free-play machines inside and the late game out back
// (craps) aren't built yet, so for now the Lanes is Juan and the place screen.

VAR met_juan = false


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
=== juan ===
# plate: lanes_lanes_night
# pose: juan_flat
JUAN: {~Pinsetter on four's jamming again. Don't stand there.|League's Tuesday. You're not league.|Rentals are two dollars. The ball you pick is your business.|You again. Shoes are where they always are.}
+ [Nod at the steel door out back.]
    # pose: juan_shrug
    JUAN: Couldn't tell you.
    -> END
+ [Leave him to his machines.]
    -> END
