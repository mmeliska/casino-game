// NORTHLIGHT: the one story the game loads. Each place's scenes are an include, so every
// scene shares the same variables and the save holds one story state.
// Tags: #plate (background), #pose (portrait swap), #insert (close-up), #black (text on black)

INCLUDE slack_water.ink
INCLUDE kasias_tap.ink
INCLUDE millstone_lanes.ink

EXTERNAL spend(amount)
EXTERNAL earn(amount)
EXTERNAL borrow(amount)
EXTERNAL repay(amount)
EXTERNAL work(pay, hours)
EXTERNAL lean(pole)

// Set by the game before each scene.
VAR player_name = ""
VAR bankroll = 200
VAR book = 0
VAR broke = false
VAR sitting_net = 0
// The calendar: today's day number, the day number of the next Saturday after today (day 1 is
// a Monday), and the hour, 0 to 23.
VAR day = 1
VAR next_saturday = 6
VAR hour = 18
// The box on Dot's register (A1.1): "mr", "ms" or "none".
VAR register_box = "none"

-> END
