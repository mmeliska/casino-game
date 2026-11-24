## Bowling at Millstone Lanes

A game of skill on Juan's lanes: open bowling to pass the time, now and then a league regular who comes over and challenges you to a game for money, and the odd Tuesday a team short a bowler asks you to sub. It's the Lanes' own thing to do, the way the chalk circle is the back lot's, and filling in for his league is how you warm Juan up.

**Settled (Mike, 2026-11-23):** the feel is a 90s console bowling game, played with a few timed taps on meters, not swipes ("the swipe mechanics are overdone on mobile"). The lane is modelled for real: an oil pattern down its length, and a ball that reacts to it, so a throw can go straight, run straight and snap at the break point, or arc out and back the whole way down. Also settled the same day, going through the open questions: you can't bowl while the league has the lanes, except when a team's a bowler short and you're asked to sub (three games on a four-person team); any other time is open bowling; money games aren't on a menu, a bowler sometimes comes up between games and challenges you, he names the stake and waving him off costs nothing; a regular who loses three games running is done for the night; the lane fee is kept low so a fair bowler can come out ahead ($2 a game is Claude's number under that; shoes $2); Juan's counter sells a urethane ball for $40 and a reactive one for $120; a left-handed toggle is in the first build; and it's subbing for his league that warms Juan up most, with challenges won counting half and open bowling on its own never. The rest of this page is proposed (Claude, 2026-11-23) until stamped. Numbers are starting values to tune in playtesting.

The inspiration is the feel of that era's console bowling, not any one game: the lanes, the regulars, the look and the names are Millstone's own.

### What you can do

| Way to play | Money | Round | Who's there |
| --- | --- | --- | --- |
| **Open bowling** | A lane fee of $2 a game and $2 for rental shoes, once a visit, until you own a pair (Juan: "Rentals are two dollars."). No bets. Mike lowered the fee from $3 so a fair bowler's challenge winnings aren't eaten by the lane (2026-11-23); $2 is the proposed number | One game of ten frames; it reports the fee as the round's loss and the minutes played | You alone; the regulars bowl on the lanes next to you |
| **A challenge** | Between games, sometimes a bowler from the next lane comes over: "Game for ten?" He names the stake, $5, $10 or $20, from his average and your record against him. Accept, or wave him off at no cost; he may ask again another night (Mike, 2026-11-23). High score wins, a tie is a push. Nobody takes a cut: it's a bet between two bowlers, not a house game. No lane fee on a challenge game: you're on his lane | One game of ten frames; settled at the last ball | Chuy or another Lanes face (below) |
| **A sub night** | Some Tuesdays a league team is a bowler short and you're asked to sub (Mike, 2026-11-23). Four-person teams: you and three teammates against four, three games, team pins win each game. No lane fee. The league's side pot is $5 a bowler a game, winners split it: +$5 to you for each game your team takes, −$5 for each it drops (proposed numbers; the pot can go to $10 a bowler if subbing should pay more) | Each of the three games is a settled round | Chuy's team or another; the other seven bowlers are Lanes faces with averages |

- **Fair.** The ball goes where you throw it and the pins fall where it sends them; the only luck is in the pin action, and it's the same for everybody. Your misses are your timing. The cheating rule has nothing to grab here, and that's the point: a game where a losing player knows exactly why.
- **No heat.** Betting a friend on a bowling game isn't the late game; nobody sweeps the lanes for it.
- **Time.** A sitting like any other: the clock follows real time at the clock pace, capped per sitting (GDD, "Time and pacing"). A ten-frame game is about three real minutes, so about an hour and a half of story time; a sub night's three games, with seven others bowling on a tap, run about ten real minutes and carry you to the last frame.
- **When.** Open bowling whenever the Lanes are open, except on Tuesday nights from 7 to 10, when the league has every lane: then you watch (the existing wait) unless a team asks you to sub, and the lanes open again after the last frame (Mike, 2026-11-23). You're never in the league; you fill a chair when they're short.
- **Challenges, how often.** About one game in four once you've bowled a game, never two in a row, and only from a regular who's on the lanes tonight (proposed). Built as one chance in three after any game that didn't follow an offer, which comes to a quarter over a night. The stake: $10 from a bowler of 175 or better, $5 below; a step up if he's ahead of you tonight, a step down if he's behind; never more than you've got, and no offer if you can't cover $5. The offer is the table's own, between rounds, like any table event; it needs no story.
- **Sub nights, how often.** About one Tuesday in three that you're at the Lanes between 6:30 and 7:30 a team's short (proposed). The ask is a scene at a break point (arriving, or getting up from the machines): Chuy, or Juan from under a pinsetter ("Chuy's short one. You've got shoes on."). Say no and nothing happens; they ask again another Tuesday. A sub night is three games; leaving between them is allowed, but the team remembers and Juan doesn't count the night.
- **Keeping it a side game.** Stakes stay Backwater-sized, and a regular who has lost three games running to you is done for the night ("I'm out. Go bowl Chuy.") (Mike, 2026-11-23). The regulars' money is their own; they don't fade a hot bowler forever.

### Controls: four taps a throw

Every throw is the same short series, one thumb in portrait. The first step is a drag; the other three are sweeps you stop with a tap, and the tap can be anywhere on the screen, not on the meter (Mike, 2026-11-24): three taps, aim, hook, speed. Where you stop a sweep is the input, and how far you were from where you meant to stop is the only error in the game.

| Step | What sweeps | What it sets | Range |
| --- | --- | --- | --- |
| 1. **Stand** | Nothing sweeps: drag the ball left or right along the foul line and let go; it snaps to a board and the board number shows while you hold it (Mike, 2026-11-23). Two small arrows beside it nudge a board a tap, for "move left two boards". No bowler is drawn: the ball itself sits on the approach and is what you line up (Mike, 2026-11-23) | The board the ball starts on | Boards 1 to 39 (the 20 board is the middle dot) |
| 2. **Aim** | A gold arrow on the lane, from your ball, swings across the seven lane arrows, out and back (Mike, 2026-11-24) | The arrow (or the board between arrows) your ball rolls over, at that arrow's distance in the V | Boards 5 to 35 |
| 3. **Hook** | A dial's needle swings from a big hook to the left, through straight in the middle, to a big hook to the right, and back (Mike, 2026-11-24) | Which way the ball turns and how much: revs and axis together, one dial | -100 to 100: the usual hook (left for a right-hander, right for a left-hander) one way, the other way the other. The middle tenth either side is "nothing on it", rolling end over end, so a straight ball is easy to stop: a sliver of axis near nothing moves a ball boards by the pins (Claude, 2026-11-23). Hooking against your hand is a real throw (a backup ball): it turns as far the other way |
| 4. **Speed** | A power bar fills and drains, cool to hot (Mike, 2026-11-24) | The ball's speed off your hand | 12 to 20 mph |

Then the ball rolls, and you watch. Two taps on the lane skip to the pins, and two more past them.

The meters take the idea of a 90s console bowling game's (Mike asked for Championship Bowling on the NES; ours are built on the same ideas in our own art, not copied): the aim is drawn on the lane itself, the hook is a dial, the speed a power bar. Built: the aim's arrow starts swinging as you step up, and you can still drag the ball while it swings (the arrow swings from wherever the ball is); a tap anywhere stops it, and the hook and speed sweeps start on their own as the one before stops (Mike, 2026-11-24; at first the aim took a tap to start and one to stop). In a coached frame you drag and tap through as usual, and the ball and meters snap to the teacher's numbers.

- **The drag is placement, not a throw.** Dragging the ball to a board is the one touch gesture, and it's a still one: nothing about the throw comes from how you moved your finger, only from where you let go. The throw itself stays on the meters.
- **Sweep speed is the difficulty.** The aim marker and the hook and speed meters sweep at a set rate; a sharp player can stop them within a board and a few per cent, and a tired thumb can't. There's no hidden random error on top: what you stop is what you throw. (If playtesting finds that too exact, a jitter of up to ±1 board and ±2 per cent goes in as a rules setting, off by default.)
- **Hand.** Right-handed by default, the hook going left. A left-handed toggle in the game's settings mirrors the lane, the hook and the regulars' track; it's in the first build (Mike, 2026-11-23). It's a setting, not a story thing.
- **The ball.** The house ball at first: plastic, barely grips, hooks a little at full meter. Juan's counter sells two more: a used urethane ball for $40 and a reactive one for $120, each gripping more (Mike, 2026-11-23). Buying one is the Lanes' upgrade and a reason to come back with winnings.
- **Shoes.** Rentals are $2 a visit until you buy your own at the counter (Mike, 2026-11-23): a used pair for $30 (proposed). After that no rental, ever, and Juan's "Shoes go back on the counter" has nothing to point at.
- **In your Pockets.** A bought ball and bought shoes are items in Pockets (GDD, "Jobs, the phone and inventory"), a slot each, like the suit for the Obsidian: you own them, you carry them, and they could be pawned on a bad week. The venue reads Pockets at sit-down and hands the table the ball you own and whether you've got shoes; the table never knows what you own. Juan's counter is a store screen like Bridgehead's: "Buy a ball" and "Buy shoes" among the choices inside, each gone once bought.

#### The view

- **No avatar, for now.** Nobody is drawn on the approach; the ball sits on the foul line and you line it up (Mike, 2026-11-23). It keeps the first build to the lane, the ball and the pins, and a bowler can be added over it later without changing anything underneath.
- **The whole screen** (Mike, 2026-11-24), the way the craps circle fills the screen: the lanes run edge to edge under everything, yours in the middle and the others either side, the approach on down under the meters, the masking panel up behind the scores. The layout leaves the lane a frame, the space between the scores and the meters, and the frame places it: the foul line near the frame's foot, the pins near its top. Behind the pins the dark pit, and the masking panel hangs above it, so the pins stand against the dark (Mike, 2026-11-24).
- **Portrait.** From the top: your cash and the way out, the scores, the message ("8 down, 4-7 left."), the lane, and at the foot the meters on a dark glass panel (or, between turns, the buttons). The ball rolls away up the lane; at the pins the view cuts to the deck from above.
- **Landscape.** The lane's frame on the left, the cash, scores, message and meters stacked on the right, the lanes still across the whole screen.
- **The trace.** After each throw a narrow upright strip down the right side of the lane, the foul line at its foot and the pins at its top (Mike, 2026-11-24), draws the path the ball took: where it skidded, where it broke, where it hit. This is how the player reads the oil. Nobody sees oil on a lane; they see what the ball did on it.
- **The pattern sheet** is never shown. Juan tells you about a lane in words ("Lane six is running dry tonight. Move left two boards."), and the trace shows you he's right.

### The lane

Real dimensions, in the rules' own units: boards across (39 of them, 1.064 in each, board 1 on the right gutter edge for a right-hander) and feet down (0 at the foul line, 60 at the head pin). Internally the simulation runs in metres and seconds.

| Thing | Where |
| --- | --- |
| Width | 41.5 in, 39 boards; gutters beyond board 1 and board 39 |
| Guide dots | 7 ft; ten, on boards 3, 5, 8, 11 and 14 and their mirrors, none in the middle (USBC: up to ten, 6 to 8 ft) |
| Approach dots | 4 in behind the foul line, seven, under the arrows' boards (USBC: up to seven a row, 2 to 6 in back; the rows farther back are off the screen) |
| Arrows | Boards 5, 10, 15, 20, 25, 30, 35, in a V pointing at the pins: the middle one at 16 ft, a foot nearer each one out, the outside two at 13 ft. Each 6 in long and 1¼ in wide (USBC: 12 to 16 ft, at most 6 in by 1¼ in, evenly spaced in a uniform pattern) |
| Range finders | Boards 10 and 30, 40 to 43 ft, a board wide (USBC: up to four, 33 to 44 ft, at most a board wide and 36 in long) |
| Head pin | 60 ft, board 20 |
| Pin deck | Pins on 12 in centres in four rows 10.39 in apart; kickbacks at the deck's sides; the pit 2.9 ft past the head pin |

**Oil.** The lane is a grid of oil amounts, one cell a board wide and a foot long (39 × 60), in units (the industry's unit; only ratios matter here). A pattern is a short list of loads, each `(from ft, to ft, from board, to board, units)`, laid down in order and summed. The house pattern (proposed, "Millstone house"):

```
Loads: ft 0–40 boards 1–39: 6 units        (a light sheet, tapering off by 40 ft)
       ft 0–36 boards 8–32: 20 units       (the body)
       ft 0–30 boards 13–27: 20 units      (heavy in the middle: about 46 in, 6 outside)
Past 40 ft: no oil.
```

That's a 40 ft house shot at roughly 8 to 1 middle to outside, which is what a Backwater alley oils for its league: easy to the pocket from the right line, forgiving of a miss inside, punishing of one outside.

**Friction** comes from the oil: `μ(cell) = μ_oil + (μ_bare − μ_oil) × e^(−units / 4)`, with `μ_bare` 0.20 and `μ_oil` 0.015, each scaled by the ball's grip on bare lane and on oil. A thin film takes most of the grip away and more oil takes a little more, so the body of the pattern is slick, the 6-unit outside boards are slick too but less so, and past 40 ft everything grabs. Built and tuned (Claude, 2026-11-23): the spec first had friction falling linearly to a full coat at 30 units, but that left the outside boards nearly as grippy as bare lane, every ball turned before the break point and no line reached the pocket at more than about 3°. (This is design text; the word "dry" stays out of the game's own strings, as the writing rules say. The one exception is Juan's line.)

**The lane changes.** Each ball that passes over a cell picks up a fraction of its oil (0.3% a pass, proposed) and lays a tenth of what it picked up down on the cells it crosses past the end of the pattern, fading with distance (carrydown). Over a night the track the league bowls, boards 8 to 12 from the right, wears thin and the ball breaks earlier and more; past 40 ft a faint stripe of carried oil makes it skid a touch longer on the usual line. Both are what real bowlers deal with, and both make Juan's advice true.

- **Within a sitting** the grid is live: your throws and your opponent's wear it.
- **At sit-down** the grid isn't saved; it's computed. Juan oils the lanes every afternoon at 5 (proposed), and the number of balls since then follows from the clock: nothing until opening, a league's worth from 7 on Tuesdays (five bowlers a lane, about 60 balls a game), a few open bowlers an hour otherwise. Built (`LaneWear`): 60 balls an hour 7 to 10 on a Tuesday, 10 an hour 5 to 11 p.m., 5 an hour noon to 5, nobody 11 p.m. to noon. That many statistical passes, weighted to a right-hander's track, are applied to the fresh pattern. Same clock, same lane; nothing to save, and no module touches the save.
- **Lane six** has its own pattern: shorter and lighter (built: the oil ends at 34 ft, the body at 31 and the middle at 26, and every load is two thirds), so it breaks early: the same throw's turn is half done about 5 ft sooner and it goes in high. That's "running dry tonight", and the fix is his: "Move left two boards."

### The ball

A rigid sphere on the lane: radius 0.108 m (8.5 in), mass 6.8 kg (15 lb). State: position `(x across, y down)`, velocity `v`, and spin `ω` as a 3D vector (`x` across the lane, `y` down it, `z` up). The simulation steps at 1 ms with the same arithmetic on every platform (doubles, our own small vector struct, no engine types, so it runs in EditMode tests and gives the same throw for the same inputs everywhere).

**From the four inputs:**

- Start at the stand board on the foul line, 0 ft.
- Direction: through the aim board at its arrow's distance (`LaneGeometry.ArrowFt`: 16 ft on board 20, a foot less every five boards out). It was 15 ft on every board until the arrows were put in their V (2026-11-24).
- Speed: the meter's mph, converted.
- Spin: the ball leaves the hand spinning as fast as its speed would roll it, and the hook meter `h` in 0..1 turns that spin's axis `45° × h` toward the lane's length. A ball at `h = 0` rolls end over end from the hand (its axis across the lane, matching its forward roll, so it never slides and keeps its speed). At `h = 1` the axis is turned 45°, so the contact point slides sideways and friction pulls the ball left. Built (Claude, 2026-11-23): the first idea, a rev rate of `400 × h` rpm with the axis at `60° × h`, left a hook-0 ball with no spin at all, so it skidded and reached the pins at 11 mph from 16.
- **The balls' grip.** Each ball scales the lane's friction separately on oil and on bare lane: the house ball (plastic) 0.8 and 0.15, so it barely grips anywhere and hooks a little at full meter; the urethane 1.2 and 1.0, gripping early for a smooth arc; the reactive pearl 0.7 and 1.4, skidding through the oil and grabbing hard where it ends, which is where the angle into the pocket comes from (Claude, 2026-11-23, replacing one cover factor of 0.8, 1 and 1.2, with which the house ball hooked late and hardest).

**Each step** (the standard rigid-sphere model):

1. The contact point's slip: `s = v + ω × r`, with `r = (0, 0, −R)`.
2. If `|s|` is above a hair, friction opposes the slip: `F = −μ(cell under the ball) × m × g × s/|s|`. It changes the velocity (`dv = F/m × dt`) and the spin (`dω = (r × F)/I × dt`, with `I = 2/5 × m × R²`).
3. When the slip reaches zero the ball is rolling: it goes straight from there, losing speed only to rolling resistance (0.25 m/s², proposed).
4. Off the edge of board 1 or 39: the gutter, and the ball's done. Past 60 ft: into the pins.

This one rule produces the three phases bowlers name, and the three shot shapes fall out of it with no special cases:

| Shot | Inputs | What the lane does |
| --- | --- | --- |
| **Straight** | Hook 0 | No side slip, so friction only slows the ball. It holds its line to the pins |
| **Straight, then a snap at the break point** | Hook high, speed high, on the house pattern | The ball skids on the oil, keeping its spin; where the oil ends at 40 ft it grabs, the sideways slip turns it sharply toward the pocket, and it rolls out of the turn |
| **Out and back, the long arc** | Hook medium, speed lower, or an outside line on the light boards, or lane six | Friction gets hold of it early and it bends the whole way down, a gentle curve back to the pocket |

The tests pin these down (see Tests). The art of the game is picking a stand, an arrow, a hook and a speed that put the ball in the pocket on *this* lane *tonight*.

### The pins

Top-down rigid circles, the same stepper, the same determinism. Ten pins, 1.53 kg (3 lb 6 oz), 4.766 in across at the belly, racked on the standard spots with a seeded jitter of up to ⅛ in on each, so two identical throws don't knock down identical pins.

- **Collisions.** Ball–pin, pin–pin, pin–kickback and pin–pit are impulse collisions between circles (and lines), restitution 0.4 ball–pin, 0.6 pin–pin, 0.3 against the kickbacks (proposed). Position correction each step, so nothing tunnels at 1 ms.
- **Falling.** A 2D sim can't tip a pin, so a rule does: a pin that takes an impulse above the fall threshold is down. Tuned (Claude, 2026-11-23): 3 kg·m/s, each pin's own threshold up to 30% either side of it (a pin caught high or low); at 1.5 nearly any touch felled a pin and a pocket ball struck 99% of the time. Down, it keeps sliding as a circle a third wider than its belly (a pin on its side sweeps more) with heavy drag and can knock other pins, but a knock from a down pin needs the same impulse to fell its target. A pin that takes less wobbles and stands: that's the ringing 10 pin and the stone 8, which every bowler knows.
- **The luck in the pin action.** A flat sim with an eighth of an inch of rack jitter turned out all geometry: a half-board decided between 0% and 100% strikes. So each rack carries a seed that turns every impulse up to about 10° off true, as a pin tilts and spins when it's hit, and sets the pins' thresholds (Claude, 2026-11-23). The same rack falls the same way, and the strike rate moves smoothly with the board and the angle.
- **Done** when everything is still or 3 s have passed, whichever first. Standing pins stay for the second ball; fallen ones are swept.
- **Pocket geometry** is where the model earns its keep (built: a ball on board 17.5 at the head pin's row strikes about 35% at 2°, 50% at 4° and 60% at 6°; board 16 leaves the 10; board 19 leaves the 4 and 7; flush on board 20 splits): a ball between the 1 and 3 pins at 4° to 6° of entry angle, 15 to 17 mph at the pins, should strike most of the time; flush on the head pin should leave splits; light in the pocket should leave the 10; high should leave the 4 or the 7. A tuning harness (below) reports these rates so the thresholds can be set by the numbers.

### Scoring

Standard tenpins: a strike counts the next two balls, a spare the next one, a strike or spare in the tenth earns its extra balls, 300 is perfect. A gutter ball is 0. No foul-line fouls: there's no foot input to foul with.

### The regulars

An opponent is a `Bowler`: a name, a face, an average, and a line (stand, aim, hook, speed) they bowl. From the average comes a timing spread, a standard deviation on each meter: tuned in the harness so simulated bowlers score their averages, a 185 bowler stops the aim within 0.3 of a board and the hook and speed within 1% of the meter, a 135 bowler 0.6 of a board and 3% (Claude, 2026-11-23). The first guess, a board and 4% for a 180 bowler, made him a 120 bowler: a board at the arrows is two to four at the pins, and the pocket is about two wide. For scale, a sharp player stopping each sweep within about 25 ms is near 0.4 of a board and 2%, around a 170 average.

- **Lines by average** (built): 175 and up throw a urethane ball from the 23 board at 12.8, hook 60, 16.5 mph; 150 and up the urethane from the 19 just inside the second arrow (10.5), hook 50, 16 mph; below that the house ball from the 34 board at 25, hook 55, 15 mph.
- **Spares** are a straight ball from across the lane (the further the pins are to one side, the further he stands to the other) at the board where the ball takes the most pins itself, the front row counting double. When no one line takes every pin, he tries a few lines on that leave in his head the first time he sees it tonight and remembers the best. Splits stay splits. They bowl through the same lane with the same ball sim, so they feel the lane change too, and a simple rule adjusts them: a first ball that reaches the head pin's row two boards or more off the pocket (17.5) moves their stand a board toward the miss. That's all the AI there is, and it's enough for a game to feel like someone is bowling against you.

- **Chuy** (the league bowler from the back-lot circle, `avatars-7_r1c1`) is the main opponent: average 185, a stroker's line, second arrow, medium hook. He talks about averages and nothing else.
- Two more from the Lanes' crowd faces, picked by the night like the back lot's seats (`CrowdFor`): a 160 and a 135. No names on their pages, no ties; the city stays big.
- On a sub night the venue fills the other seven chairs from the same faces with averages from 130 to 190, Chuy on your team. Teams have names on the scoresheet only ("Millstone Parts", "Lane Six"; proposed), never pages.
- Juan doesn't bowl. He's under a pinsetter.

### Rounds, money and break points

The module follows the architecture's recipe for a game, the same shape as `CrapsTable`:

- `BowlingLane.Init(BowlingRules rules, Sprite plate, LaneArt art)` once (the venue loads the lane's textures and hands them over; any missing, the lane makes its own), and `SitDown(PlayerState state, ClockPace pace, LaneSetup setup)` each visit. `LaneSetup` is what changes from one sitting to the next: tonight's `LaneOil` (the venue wears it from the clock with `LaneWear` and `LaneOil.Worn`), the ball you own, whether you've got shoes, the regulars on the lanes, a sub night's `LeagueMatch`, a `PracticeFrame`, and the night's seed (built; the spec first had all of it in `Init`).
- You bowl open games. Open bowling takes the fee at the first ball (−$2, or −$4 the first game of a visit if you're renting shoes). A challenge or a sub game takes no fee. Built: from the first ball you're committed (no getting up), and the money moves once, when the game settles at its last ball, as every other table settles a round. A game you can't cover isn't started, and a game cut off by a quit settles nothing, like a blackjack hand.
- **A sub night** is a `LeagueMatch` the venue hands in at `SitDown`: your team's three other bowlers and the four across the lane, as `Bowler`s. The eight bowl in turn, a frame at a time, a bowler from each side alternately and you leading off, you tapping through yours and each regular's throw playing on a tap (as the back lot's regulars shoot), and the side pot settles at each game's tenth frame. After the third game the table reports the night (`GamesWon`, `TeamWonNight`) and you get up.
- **Challenges** come from the table, between games: with the seeded odds in `BowlingRules` (one game in four, never twice running), a regular present tonight steps over and names a stake from `BowlingRules.Stakes` (5, 10, 20) by his average and the night's record against you. Two buttons: take the game or wave him off. A regular who's lost three in a row tonight doesn't ask again. The stake is taken at the first ball and paid at the tenth frame.
- Every finished game is a settled round: `PlayerState.ApplyRoundResult(net, minutes)` with the sitting's story minutes: the fee for an open game, ±stake or 0 for a challenge.
- **Getting up** is between games, never mid-game; that's the break point (`BreakPoint.GotUpFromTable`). `SittingNet`, `GamesBowled`, `BestGame`, `ChallengesWon`, `ChallengesLost` and, on a sub night, `GamesWon` and `FinishedNight` stay readable at the break point for the venue to turn into story.
- `ICoachable` with `BowlingControls` ids: `stand` (the ball on the approach, dragged), `stand_left`, `stand_right`, `aim`, `hook`, `speed`, `take_game`, `wave_off`, `get_up`, and (built) `bowl` for another game and `next` for the regulars' balls, which play one after another until you're up again. The venue can hand in a `PracticeFrame` (fixed inputs the meters snap to, nothing counting), the way the back lot hands craps a `PracticeTurn`.
- The table never reads story state, counts heat or saves. The lane's wear is computed by the venue from the clock and handed in, and so are tonight's regulars and whether the league has the lanes (then the venue doesn't seat you at all).

### Juan's first frame

Your first game, Juan coaches a frame over the meters, the way he coaches the practice roll out back (all lines proposed):

> JUAN: Seventh board. Second arrow. Don't put anything on it.
> *(He locks the meters to stand 7, aim 10, hook 0, speed 16. You tap through. The ball goes straight up the second arrow and takes the head pin flush: a split.)*
> JUAN: See that? Straight ball finds the head pin and leaves you two corners.
> JUAN: Now put something on it. Six boards left.
> *(Stand 13, aim 10, hook 70, speed 16. The ball runs up the second arrow, holds through the oil, and breaks into the pocket.)*
> JUAN: That's the lane doing the work. You just have to find where it starts.

(Built, Claude, 2026-11-23: Juan's first lines had the ball on the middle dot, board 20, but a straight ball from there over the second arrow is in the gutter by 28 ft; from board 7 it reaches the head pin.)

Then it's your game. He's gone back to the machines.

### Standing with Juan

Story stays in the venue, at break points. When you get up:

- `bowled_at_lanes` is set the first time; `sub_nights` counts the Tuesdays you bowled all three games for a short team; `challenges_won` counts the money games you've taken off his regulars. These go in `StoryContract`.
- Subbing for his league is what warms Juan most; a challenge won counts half, and open bowling on its own never counts (Mike, 2026-11-23). In numbers (proposed): his standing is `2 × sub_nights + challenges_won`. At 2 (one sub night, or two wins) the warmer greeting opens on his knot ("Chuy says you filled in. Lane six is running dry tonight; move left two boards."); at 6 he's easy with you, the way he is with league bowlers, and his once-a-season Kasia line and the mirror (his page) can come. S1's $40 for shirts is offered by Chuy, not Juan, after your first sub night.
- R3 later: if Umbra wins and lane six goes dark, the venue hands the table five lanes' patterns, not six. The table doesn't know why.

### Code

Following `ARCHITECTURE.md`, "Adding a game": namespace `Starflare.Bowling`.

`Games/Bowling/Rules/` (`Starflare.Bowling.Rules`, `noEngineReferences`, no kit references; craps' `Dice` isn't needed):

| File | What |
| --- | --- |
| `Vec2.cs`, `Vec3.cs` | Small double-precision structs: the sim's own arithmetic |
| `LaneGeometry.cs` | Boards, feet, metres, the arrows and dots, the deck spots, the conversions |
| `LanePattern.cs` | The loads and `ToGrid()`; the Millstone house and lane-six patterns as statics |
| `LaneOil.cs` | The mutable 39 × 60 grid: `FrictionAt(x, y, ball)`, `Pass(path)` (pick-up and carrydown), `Worn(pattern, balls)` (statistical wear for sit-down) |
| `LaneWear.cs` | Balls since oiling from a clock's weekday and minute: the venue's helper, no engine |
| `BowlingBall.cs` | Radius, mass, cover factor; the house, urethane and reactive balls |
| `Throw.cs` | The four inputs, and `Clamp` to their ranges |
| `BallSim.cs` | `Roll(Throw, LaneOil, BowlingBall) → BallPath`: the stepper; `BallPath` holds the samples for the trace and the pin entry state |
| `PinDeck.cs` | The ten spots, the rack with its seeded jitter, which pins stand |
| `PinSim.cs` | `Hit(BallPath entry, PinDeck) → PinResult`: the circles, the fall rule, which pins fell and the pins' motion samples for the view |
| `Frame.cs`, `ScoreSheet.cs` | Ten frames, the tenth's extra balls, running totals, `IsComplete` |
| `Bowler.cs` | Name, average, line, spread; `Next(LaneOil, ScoreSheet, seed) → Throw` with the miss adjustment; the stake he names from his average and the record |
| `Challenges.cs` | Who asks tonight and when: the one-in-four odds, never twice running, not after three straight losses; seeded |
| `BowlingRules.cs` | Fee, shoe rental, stakes, the challenge odds, the sweep rates and the optional jitter, the balls and shoes for sale with their prices, the hand |
| `BowlingGame.cs` | One game: whose ball, the sheet(s), the stake, `Settle()`; open or against a `Bowler` |
| `LeagueMatch.cs` | A sub night: two teams of four, three games, the bowling order, team pins per game, the side pot's settlement per game |

`Games/Bowling/Table/` (`Starflare.Bowling`: `Core`, `Ui`, its rules, `UnityEngine.UI`, `Unity.TextMeshPro`):

| File | What |
| --- | --- |
| `BowlingLane.cs` | The MonoBehaviour: `Init`, `SitDown`, the game loop, rounds and the break point, `ICoachable` |
| `ApproachBall.cs` | The ball on the foul line: a uGUI drag handler (`IBeginDragHandler`/`IDragHandler`/`IEndDragHandler`) that snaps to boards and shows the board number while held |
| `BowlingControls.cs` | The control ids |
| `LaneProjection.cs` | The perspective: sim coordinates to the screen, the lanes' spacing, the light down the lane |
| `LaneSurface.cs` | One textured surface (boards, gutters, approach, masking panel) built as geometry through the projection |
| `LaneArt.cs` | The textures and pictures the venue hands over, and plain stand-ins for any texture not drawn yet |
| `SpriteQuads.cs` | A layer drawing one sprite many times (the pins, the ball), or shapes while it isn't in |
| `LaneView.cs` | Over the surfaces: the ink (edges, board joints, foul line, dots, arrows), the capping and pit, the racks either side, the ball, the pins, the sweep |
| `Meter.cs` | A sweep you stop with a tap: shared by aim, hook and speed |
| `PathTrace.cs` | The overhead strip after each throw, upright down the lane's right side |
| `HookGauge.cs` | The hook's dial |
| `DeckView.cs` | The close-up of the deck for the hit, from above (built: the sim is flat, so the close-up shows it as it is) |
| `LaneSetup.cs` | What the venue hands over at each sit-down (built) |
| `ScoreSheetView.cs` | The ten boxes, portrait strip and landscape column |
| `PracticeFrame.cs` | Juan's locked inputs |

`Games/Bowling/Tests/EditMode/` and `/PlayMode/`.

`Game/`:

Built (Claude, 2026-11-23), as below with these differences: Juan's counter is its own screen ("Go to the counter": talk to Juan, the balls and shoes for sale, back to the lanes) rather than more choices inside, and it only offers what you can pay for; `owns_ball` holds the ball's id ("urethane", "reactive"), the best you've bought; the hand is a device setting on the debug panel until there's a settings screen; Juan puts you on a lane by the night, and one night in six it's lane six; the sub ask is Juan's line ("Chuy's short one. You've got shoes on."); a sub night has no coached frame; and the lines when you get up read `games_bowled`, `best_game` and `challenges_won_tonight`, with `sub_games_won` and `sub_finished` after a sub night, all set by the venue at the break point. `sub_asked_day` and `sub_tonight_day` carry the ask and your answer, and `shirts_asked` and `chipped_in_shirts` carry S1.

- `MillstoneLanes` becomes a MonoBehaviour with `Init` (it owns a table now), built in `PlaceList` with `root.AddComponent<BowlingLane>()` beside the back lot. "Bowl" joins the choices inside, except on Tuesdays from 7 to 10, when it's not offered and "Watch the lanes" is the only thing to do with them, unless the sub ask has fired: then "Bowl for Chuy's team" seats you with the `LeagueMatch`.
- `LaneWear` from the clock each sit-down; tonight's regulars by the night, like the back lot's seats; the player's ball and shoes from Pockets (the house ball and rentals until bought); the hand from settings. Juan's counter sells the two balls and the shoes: `Spend` on the player state and an item into Pockets, the way Bridgehead's store works. Pockets itself is on the phone's list and not built yet; until it is, the two items are flags on the save (`owns_ball`, `owns_shoes`) that Pockets adopts when it lands. The sub ask is the venue's: one Tuesday in three, seeded by the date so it's the same on a reload, fired at a break point between 6:30 and 7:30.
- Ink in `millstone_lanes.ink`: `bowl_first` (Juan's frame), `bowl_get_up` (the lines after, by `GamesBowled`, `ChallengesWon` and the night), `league_sub_ask` (Chuy or Juan asks), `league_sub_done` (after the third game, by `GamesWon`), `bowl_shirts` (Chuy's shirts ask, S1, after your first sub night). Variables `bowled_at_lanes`, `sub_nights`, `challenges_won`, `owns_ball`, `owns_shoes` in `StoryContract`, checked by `StoryContractTests`.
- Checkpoints: "A1.3 On the lanes" (Juan has his box, Wednesday 8 p.m., at the Lanes), "A1.3 Short a bowler" (Tuesday 6:45 p.m., the sub ask about to fire) and "A1.3 League to Juan" (standing 6).

### Tests

EditMode, in `Starflare.Bowling.Tests.EditMode`, written as the rules are:

- **Geometry:** board 20 is the lane's centre; arrow 2 is board 10 at 14 ft, the arrows in a V within USBC's 12 to 16 ft; metres round-trip.
- **Oil:** the house grid has more oil at board 20 than board 5 at 20 ft; none past 40 ft; a pass lowers the cells it crossed and raises cells past 40 ft on its line; `Worn` with 1,000 balls has a thin track on boards 8 to 12 and nothing changed on board 35.
- **Ball, the three shapes** (the ones Mike asked for, each on the fresh house pattern unless said):
  - Hook 0 from board 7 over arrow 2 (board 10) at 16 mph, the house ball: the ball's `x` at every sample follows the straight line within half a board, and it reaches the head pin.
  - Hook 75, speed 17, stand 32, aim 19, the reactive ball: under 15% of its turn is done by 30 ft and under 25% by 36 ft, half of it by 38 to 48 ft (the break point sits at the end of the oil), it enters the pocket at least 4 boards left of where it crossed 40 ft, at 4° to 8°, and it's rolling (slip zero) before 55 ft.
  - Hook 50, speed 13, stand 15, aim 7, the urethane ball: it turns left at every foot from 5 ft until it rolls (monotone curve: no straight-then-snap), over 40% of the turn is done by 30 ft, no 5 ft stretch holds more than 40% of it, and it's rolled out before 45 ft. (Past 40 ft the lane is bare, so a ball still sliding there snaps; an arc is a ball that bends from early on and is spent before the pins.)
  - The snap throw on lane six's pattern is half turned and rolling at least 4 ft earlier than on the house pattern, and goes in high.
  - The house ball turns less than half as much as the urethane on the snap throw; the reactive skids further through the oil than the urethane.
  (Test lines rebuilt from the tuned sim, Claude, 2026-11-23: the spec's first lines started the hook ball on board 20 over the second arrow, which is a Brooklyn on this lane.)
  - The same `Throw` on the same grid gives a bit-identical `BallPath`.
- **Pins:** a pocket entry (board 17.5 at the head pin's row) at 5° and 16 mph strikes 45% to 75% of 500 seeded racks; flush on the head pin leaves two or more pins on at least 60%; light (board 16) most often leaves the 10, and high (board 19) leaves the 4 or the 7 more often than not; a 10-pin ball (board 2.5, crossing at 2°) takes exactly the 10 on most tries; a glancing 1.0 kg·m/s tap leaves a pin standing; the same rack falls the same way.
- **Scoring:** twelve strikes make 300; all spares with 5s make 150; the tenth frame's extra balls; gutters are 0; a mid-game sheet reports `IsComplete` false.
- **Bowlers:** a 185 bowler over 200 seeded games averages 175–195 on the fresh house pattern; a 135 one averages 125–145; the miss rule moves the stand the right way.
- **Rules and settlement:** a challenger names 5, 10 or 20 within the bankroll; a won game nets +stake, a tie 0, open bowling −fee and −(fee + rental) the first game, no rental with your own shoes, a challenge or a sub game no fee; the practice frame nets nothing.
- **The counter:** buying a ball takes its price once and the choice goes; buying shoes ends the rental from the next visit; the bought ball is the one `Init` receives.
- **League match:** eight bowlers bowl in the right order; a game's team pins are the sum of four sheets; the side pot pays +5 to each winner and −5 to each loser, 0 all round on a tie; a match reports three settled games and `FinishedNight` only after the third.
- **Challenges:** over 10,000 seeded games about a quarter draw an offer, none follows another directly, none comes before the first game's over, and a regular on three straight losses never asks again that night.
- **Hand:** the same throw left-handed mirrors the right-handed path board for board, and the worn track mirrors with it.
- **A tuning harness** (`[Explicit]` test): bowls a grid of inputs through a lane and prints strike, spare and leave rates and the break-point foot, so the thresholds above are set from numbers, not guesses.

PlayMode smoke tests in `Starflare.Bowling.Tests.PlayMode`: the lane builds in portrait and landscape, a full open game plays through with a scripted drag and taps and reports exactly one round, dragging the ball snaps it to a board and past the gutters it stops on board 1 or 39, getting up raises the break point once, a coached frame locks and unlocks the right controls.

### Art

For ChatGPT, in the style of the Lanes' other plates (the how-to, the avoid lists and the settings are on [Plate prompts](#plates)); attach `lanes_lanes_night` as the style reference for each. Written out in full (Claude, 2026-11-23); proposed.

The game draws the ball, the pins, the aim marker and the trace itself, so the pictures leave them out.

#### The lane: geometry and textures

The lane isn't a picture. It's built as geometry through the same projection the sim's ball and pins are drawn through, and covered in textures that repeat down it (Mike, 2026-11-24; the details below are proposed, Claude). A painted lane could never have 39 even boards with the arrows and dots on exactly the boards the sim uses, and stand, aim and Juan's "move left two boards" are all counted in boards; built from `LaneGeometry`, the lane is right by construction. It also lays out for portrait and landscape alike, and leaves room for showing the oil later. This replaces `lanes_lane_pov`, the lane as one portrait plate, in the first draft of this section.

What's built (`LaneProjection`, `LaneSurface`, `LaneView`, `LaneArt`):

- **A true perspective**, the eye 60 ft behind the foul line for both how the lane narrows and how its length spreads up the screen, so its lines are straight; a long lens, like TV bowling, so the far end where the hook happens keeps its room. (First built with two distances, which bent every line.)
- **Your lane and three either side**, a lane every 63 in (41.5 in of boards, two 9.25 in gutters and 4 in of capping), each dimmer the further out: nobody's bowling on them. Their racks stand full and dim.
- **Light by vertex colour**: the approach under the house lights, the middle of the lane dimmer, the deck bright under its hood.
- **Ink drawn by the code**, to match the bold outlines of the other art: every lane's edges and its gutters', the foul line, the dots and arrows outlined, the ball and pins outlined, and your lane's 38 board joints, fading out down the lane so they don't shimmer where they crowd together.
- **Past the pit**, the dark pit behind the pins, a pin and a half tall, and above it the masking panel that hangs over the far end of the lanes, up to the top of the screen.
- **Until the pictures are in**, the lane makes plain textures of its own (wood grain, a grey channel, cream panels over green), so the textured path is what's tuned on.

The textures repeat, and ChatGPT's never quite meet at their edges, so each is saved in `Images/Bowling/` under its tag and brought in with `python3 tools/import_bowling.py`: it blends each picture into itself across the edges it repeats over, scales it down and writes `game/Assets/Resources/Bowling/<tag>.png`, which the game imports as a repeating texture. They're textures, not scenes, so each prompt asks for something flat, seen straight on, edge to edge.

`lane_boards` (texture, square 1:1)

```
A seamless tileable texture, seen flat from directly above and filling the whole square edge to edge: the polished maple surface of a bowling lane, pale honey wood with long straight grain running from top to bottom, a soft varnish sheen, faint scuffs. No board joints, no lines across, no gaps, no edges: just continuous grain. Even lighting, no shadows, no vignette, no perspective. Gritty graphic-novel texture art to match the attached plate: cel shading, muted desaturated colours, a few fine dark ink strokes in the grain. No text, no markings, no arrows, no dots.
```

`lane_approach` (texture, square 1:1)

```
A seamless tileable texture, seen flat from directly above and filling the whole square edge to edge: the worn wooden approach in front of a bowling lane, darker amber maple scuffed dull by years of shoes, grain running from top to bottom, a few scrapes and old wax smears. No board joints, no lines across, no gaps, no edges. Even lighting, no shadows, no vignette, no perspective. Gritty graphic-novel texture art to match the attached plate: cel shading, muted desaturated colours, a few fine dark ink strokes in the grain. No text, no markings, no dots, no foul line.
```

`lane_gutter` (texture, square 1:1)

```
A seamless texture that repeats from top to bottom, seen flat from directly above: the inside of a bowling lane's gutter running straight from the top edge to the bottom edge and filling the square from side to side, a shallow curved channel of dull dark grey composite, darker along the bottom of the curve, a soft highlight along the far wall on the right, scuffs and ball marks. Even lighting, no perspective, nothing at the top or bottom edge that wouldn't continue. Gritty graphic-novel texture art to match the attached plate: cel shading, muted desaturated colours, fine dark ink strokes. No text, no markings.
```

`lane_masking` (texture, landscape 3:2)

The front of the masking unit as it is in `lanes_lanes_night` (Mike, 2026-11-24): plain cream panels over a strip of tired green, a thin navy line between. Asked for as a band across the middle of a black picture; the import tool cuts the band out by itself, keeping its shape, and the game repeats it across the top of the view.

```
A seamless texture that repeats from left to right, seen flat and straight on with no perspective: the masking front that hangs over the far end of the lanes in an old bowling alley, drawn as a band across the middle of the picture from the left edge to the right edge, about a third of the picture's height. In the band, from the top: two long plain cream panels side by side, yellowed and scuffed, each framed in a black ink outline, with a dark gap between them and half of the same gap at the left and right edges so the strip repeats; then a thin navy stripe the whole width; then a strip of tired green panelling about two thirds as tall as the cream panels, with faint vertical grain and grime, the warm light of the lanes below catching its bottom edge. Above and below the band, plain flat black. Gritty graphic-novel art to match the attached plate: bold black ink outlines, cel shading, muted desaturated colours, worn and grimy. The panels are blank: no text, no lettering, no numbers, no logos, no pictures.
```

#### The deck, from above

The close-up for the hit. The pin sim is flat, so the close-up looks straight down on the deck and shows it as it is. (This replaces `lanes_deck_close`, the deck from the front, in the first draft of this page.)

Drawn (Mike, 2026-11-24) as `Images/Bowling/lanes_pin_deck.png`; the import tool copies it in as `deck_top`, a picture the bowling game draws rather than a story insert. It also measures the ten spots and prints where they are, and those numbers are in `DeckView`, so the drawn pins stand on the painted spots and a ball's hit plays out on the picture. The spots are painted 12 in apart across, as on a real deck, but the rows 9 in apart rather than 10.4, so the close-up maps the sim down the lane at its own scale. Standing pins are the sprite from above; knocked pins lie on their sides, pointing the way they were knocked; anything that flies into the gutters stays inside the picture. A redrawn deck needs the tool run again and its numbers put in `DeckView`.

`lanes_deck_top` (square 1:1)

```
Close-up insert panel, looking straight down from above onto the empty pin deck at the end of a bowling lane: polished pale wood with the ten pin spots marked as small dark dots in a triangle, point toward the bottom of the frame, the gutters down both sides, the dark steel kickback walls beyond them, and the black pit across the top edge. Flat top-down view, no perspective, the deck filling the square. Night: a deep navy sky and cool blue shadows, old yellow streetlamps and lit windows casting small warm pools, red and green bar neon. Palette: brick red, tired green, navy. Gritty graphic-novel: bold black ink outlines, heavy shadows, cel shading, muted desaturated colours, No people, no pins, no ball, no text, no lettering.
```

#### The balls, the pins and the shoes

One sheet. The three balls are the house ball and the two on Juan's counter; the pin is drawn upright and on its side for the lane, and from above for the deck close-up; the balls and the shoes double as their Pockets icons. Make it in one ChatGPT chat with `lanes_lanes_night` attached, and save it as `Images/Bowling/bowling_sprites.png`. The magenta ground is there so the black urethane ball and the grey of a fallen pin come out clean. Drawn (Mike, 2026-11-24). `tools/import_bowling.py` cuts it with the scratchers' magenta cutter and writes `game/Assets/Resources/Bowling/<name>.png`, in sheet order: `ball_house`, `ball_urethane`, `ball_reactive`, `pin_up`, `pin_down`, `pin_top`, `shoes`. The game draws the ball you're bowling with on the approach, rolling and on the deck, the pins upright on the lane (dimmer on the lanes either side) and from above or on their sides on the deck; while a sprite isn't in, that thing is drawn in shapes. The shoes wait for Pockets.

`bowling_sprites` (sprite sheet, landscape 3:2)

```
A sheet of bowling game sprites, each a separate small illustration, laid out in a grid with wide empty space between them, none touching. Solid flat magenta background (#FF00FF), the same everywhere: no shadows, no gradient, no floor, no frames. Each sprite centred in its own cell, all at the same scale, filling most of the cell. Gritty graphic-novel art to match the attached plate: bold black ink outlines, cel shading, muted desaturated colours, clear at a small size. Nothing magenta or pink in the sprites. No text, no letters, no numbers.

Seven sprites, four on the top row and three on the bottom, in this order: an old house bowling ball, glossy red with a darker red swirl, three finger holes, seen from slightly above and behind; a used urethane bowling ball, matte black with scuffs, three finger holes, the same view; a reactive bowling ball, pearly pale lavender with a soft shimmer, three finger holes, the same view; a white bowling pin standing upright with two red neck stripes, seen from slightly above and behind; the same pin lying on its side; the same pin seen from directly above, a white circle with the red stripes as rings; a worn pair of two-tone bowling shoes, red and cream, side by side.
```

### Build order

1. **Rules, the lane and the ball** with the three-shape tests. The harness prints paths as ASCII lane strips so the hook can be read in a test log before any art exists.
2. **Pins and scoring** with the strike-rate tests; tune the fall threshold and restitutions.
3. **Bowlers, challenges, rules and the game** (open and for money); settlement and odds tests.
4. **The table** in placeholder shapes, portrait and landscape; the PlayMode smoke tests.
5. **The place**: `MillstoneLanes` as a MonoBehaviour, the ink, the contract, the checkpoints, the docs' Lanes page and GDD rows.
6. **Art** and the trace. All the art is in (2026-11-24): the lane built and textured, the deck close-up, the balls and the pins. The trace still draws in shapes.
7. **Tune** on a phone: sweep rates, the regulars' spreads, the fee and stakes.

Steps 1 to 3 are a day or two each of rules code and tests and can be checked headless. Step 4 is the biggest single piece, about the size of `SlotMachine`.

### Decisions (Mike, 2026-11-23)

Gone through with Mike the day the spec was written:

1. **The regulars' patience, not their money,** ends a run: three straight losses and a regular is done for the night. No stake cap.
2. **Open bowling is per game,** and the fee is low: Mike's point was that at $3 a game with a challenge only now and then, bowling couldn't pay until stakes reached $15 to $20, so the fee comes down. $2 a game with shoes still $2 is Claude's number under that decision; challenge and sub games carry no fee.
3. **No avatar** in the first build: the ball sits on the approach and you drag it left or right along the foul line to line it up (it snaps to a board; two arrows nudge a board at a time); a bowler can be drawn over it later.
4. **Balls and shoes for sale** at Juan's counter: a used urethane ball for $40, a reactive for $120, and your own shoes (Claude's price: $30), after which there's no rental. What you buy sits in your Pockets.
5. **No bowling during the league, except as a sub.** Tuesdays 7 to 10 the lanes are the league's and you watch; any other time is open bowling. Money games are never on a menu: now and then a bowler comes up between games and challenges you. He names the stake; waving him off costs nothing.
6. **A left-handed toggle** is in the first build.
7. **Subbing.** Some Tuesdays a team's a bowler short and asks you to fill in: four-person teams, three games, team pins win each game, no lane fee, and the league's side pot pays the winners.
8. **Juan warms to subbing most,** to challenges won half as much, and to open bowling never. (This replaces "bowling in the Tuesday league is how you warm him up" on his page.)
