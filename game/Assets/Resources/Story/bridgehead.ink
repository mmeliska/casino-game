// BRIDGEHEAD FUEL & FOOD: the Bridges, the Backwater foot of the Long Bridge. Mitch at the
// counter, the store's cheap machines, the forecourt. Included by northlight.ink.
// Plates name the view (bridgehead_forecourt, bridgehead_store); the game picks the day or
// night plate, and the snow one in winter. The race book out back (R5) isn't built yet.

VAR met_mitch = false
// The day you first stopped, so the trucker isn't at the pumps the same night.
VAR met_mitch_day = 0
// The trucker short on diesel (S3): he's been at the pumps, whatever you did, and whether
// you spotted him the twenty, with the day you did (the seed's reveal waits on it).
VAR trucker_passed = false
VAR helped_trucker = false
VAR helped_trucker_day = 0


// Over the bridge (A1.6): the last stop on the Backwater side, on your way to the Starflare.
=== bridgehead_first ===
~ met_mitch = true
~ met_mitch_day = day
# plate: bridgehead_forecourt
Last lights before the Long Bridge. The rocket sign is on the far side.
# plate: bridgehead_store
Fluorescent hum. Slot machines along the wall, a poker machine at the end.
# pose: mitch_neutral
MITCH: Pumps are prepay. Coffee's a dollar.
* [Lay a ten on the counter for pump two. (-$10)]
    MITCH: Pump two's temperamental. Has been since I started.
    ~ spend(10)
    MITCH: Squeeze it twice.
* [Pour a coffee. (-$1)]
    ~ spend(1)
    MITCH: Cream's out. Cream's always out.
* [Ask the way to the Starflare.]
    # pose: mitch_shrug
    MITCH: Over the bridge. Look up. You'll see the rocket.
- # pose: mitch_neutral
* [Nod at the machines.]
    MITCH: Six-five poker. Worst pay in town.
    MITCH: People play it anyway. They're on their way somewhere.
* [Nod at the door behind him.]
    # pose: mitch_shrug
    MITCH: Stockroom. Motor oil.
- # pose: mitch_neutral
MITCH: Going over? Everybody's going over.
-> END


// Mitch at the counter, any visit after: clipped, horses and prices. The back door is a
// stockroom until he trusts you (R5).
=== mitch ===
# plate: bridgehead_store
# pose: mitch_neutral
MITCH: {~Coffee's a dollar. Same as yesterday.|Gas went up four cents. Nobody asked me.|Scratchers are two. The good ones went this morning.|You again. Machines are where you left them.|Track's dark this week. Nothing to watch but the pumps.}
+ [Pour a coffee. (-$1)]
    ~ spend(1)
    MITCH: {~Lid's on the left.|Burnt since six.|Cream's out.}
    -> END
+ [Ask about the door behind him.]
    # pose: mitch_shrug
    MITCH: {~What door.|Stockroom.|Motor oil. Want a case?}
    -> END
+ [Ask about pump two.]
    MITCH: {~Pump two's temperamental. Has been since I started.|Squeeze it twice. Then once more.|Works when it wants. Like everybody.}
    -> END
+ [Leave him to the register.]
    -> END


// The trucker short on diesel (S3): a night at the pumps after your first stop. Spot him the
// twenty or don't. He remembers on the Flare's last night (A3.3).
=== trucker ===
~ trucker_passed = true
# plate: bridgehead_forecourt
# pose: trucker_neutral
A flatbed at pump one. Its driver counts bills on the hood.
# pose: trucker_short
TRUCKER: Card won't go through. I'm twenty short on diesel.
TRUCKER: Got a load due up north by morning.
* [Hand him a twenty. (-$20)]
    ~ spend(20)
    ~ helped_trucker = true
    ~ helped_trucker_day = day
    ~ lean("generous")
    # pose: trucker_thanks
    TRUCKER: That's decent. I'm over this bridge every week.
    TRUCKER: Anything heavy needs moving, you find me.
* [Point him at the counter.]
    ~ lean("tight")
    # pose: trucker_neutral
    TRUCKER: Asked already. He laughed.
    TRUCKER: I'll find a machine. Somewhere.
* [Walk past him to the door.]
    ~ lean("tight")
    # pose: trucker_neutral
    He goes back to counting.
- -> END


// Getting up from the poker machine: Mitch, without looking up.
=== bridgehead_get_up ===
# plate: bridgehead_store
# pose: mitch_neutral
{
- broke:
    MITCH: Tapped? Coffee's still a dollar.
- sitting_net > 0:
    MITCH: {~Paid out. Write down the date.|Huh. It does that sometimes.}
- sitting_net < 0:
    MITCH: {~Six-five. That's how it goes.|Machine says thanks.}
- else:
    MITCH: Even. Here, that's a win.
}
-> END
