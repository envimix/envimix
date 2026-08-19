# Envimix Team Match

Envimix Team Match is a N versus N competitive mode that tests your adaptability to N different cars. It’s not just about how well you drive your favorite car, but how well you can master every car in the lineup.

## Ban Phase

Teams take turns banning maps from the competitive pool until exactly one single map remains. The team that bans first is determined by a coin flip.

There are 7 maps in the competitive pool:

1. Team A: bans 2 maps
2. Team B: bans 2 maps
3. Team A: bans 1 map
4. Team B: bans 1 map
5. 1 map remains

Once map is selected, teams will then ban cars from the competitive pool in exactly 4 phases until exactly N cars remain, which both teams must use (mirror match). The team that banned first in the map banning phase will ban second in the car banning phase.

There are up to 11 cars in the competitive pool, though the gamemode or map can sometimes provide a smaller pool. The order of the cars is randomized before the banning phase starts. Let P be the number of cars in the pool and N the number of players per team (P must be at least N for the match to be valid), so the total number of cars to ban is T = P − N. The 4 phases alternate between the 1st and 2nd banning team (1st, 2nd, 1st, 2nd), and T is split across the phases as evenly as possible, with any leftover ban going to the earliest phases first:

| Cars to ban (T) | Phases (P1-P2-P3-P4) | Team A total | Team B total |
|:---:|:---:|:---:|:---:|
| 0 | – | 0 | 0 |
| 1 | random | 0 | 0 |
| 2 | 1-1-0-0 | 1 | 1 |
| 3 | 1-1-1-0 | 2 | 1 |
| 4 | 1-1-1-1 | 2 | 2 |
| 5 | 2-1-1-1 | 3 | 2 |
| 6 | 2-2-1-1 | 3 | 3 |
| 7 | 2-2-2-1 | 4 | 3 |
| 8 | 2-2-2-2 | 4 | 4 |
| 9 | 3-2-2-2 | 5 | 4 |
| 10 | 3-3-2-2 | 5 | 5 |

When T = 1, only one team would ever get to ban a car, since the other team's 3 phases are all empty. In that case, no team executes a ban, and the remaining car is removed at random instead.

### Example: 4v4 with the full 11-car pool

Coin flip: Team B bans first (so Team A will ban first in the car phase).

Map pool: Map 1, Map 2, Map 3, Map 4, Map 5, Map 6, Map 7

1. Team B bans 2 maps: Map 3, Map 6
2. Team A bans 2 maps: Map 1, Map 7
3. Team B bans 1 map: Map 5
4. Team A bans 1 map: Map 2

Remaining map: Map 4

With P = 11 and N = 4, T = 7, so the phases follow the 2-2-2-1 pattern (Team A bans first in this example):

Pool after randomizing the order: LagoonCar, StadiumCar, BayCar, CanyonCar, ValleyCar, TrafficCar, DesertCar, RallyCar, CoastCar, IslandCar, SnowCar

1. Team A bans 2 cars: CanyonCar, LagoonCar
2. Team B bans 2 cars: StadiumCar, TrafficCar
3. Team A bans 2 cars: DesertCar, SnowCar
4. Team B bans 1 car: CoastCar

Remaining 4 cars (used by both teams): BayCar, IslandCar, RallyCar, ValleyCar

## Pick Phase

Once exactly N cars remain, each team independently assigns its players to those cars. Since it's a mirror match, both teams pick from the same surviving pool and never compete with each other for a car.

Each team gets its own random pick order for its players, generated once the pool is set. Following that order, players pick their starting car one at a time, choosing any car not already taken by a teammate. When only one car is left, it is automatically assigned to the last player in the pick order. What matters for the later rotation isn't this pick order, but which car each player ends up with, since the two teams pick independently and won't necessarily draft in the same sequence.

## Rotation

Even though the full rosters race together each round, a player's only *relevant* opponent that round is the one player on the other team driving the same car, since that's the only fair time comparison (different cars aren't comparable). So covering "every car against every opponent" needs N blocks of N rounds each, for N² rounds in total.

Every player switches to a new car every single round, cycling through all N cars in the same shared order (the order the N cars survived the ban phase). What stays constant for a while is the rival: for a whole block of N rounds, a player keeps racing the same opposing player, just on a different shared car each round.

Once both have raced each other on all N cars, the block ends and everyone is handed a new rival for the next block. So after N blocks, every player has driven every car and against every one of the N opponents, shared a car with them at least once.

### Example: block 1 of a 4v4 match

Say these 4 cars survived the ban phase in this order: BayCar, IslandCar, RallyCar, ValleyCar. In block 1, the Team A player who picked BayCar faces the Team B player who also picked BayCar, and likewise for IslandCar, RallyCar and ValleyCar. Every round, each paired duo advances together to the next car in that shared order:

| Round | BayCar pickers share | IslandCar pickers share | RallyCar pickers share | ValleyCar pickers share |
|:---:|:---:|:---:|:---:|:---:|
| 1 | BayCar | IslandCar | RallyCar | ValleyCar |
| 2 | IslandCar | RallyCar | ValleyCar | BayCar |
| 3 | RallyCar | ValleyCar | BayCar | IslandCar |
| 4 | ValleyCar | BayCar | IslandCar | RallyCar |

By round 4, the two BayCar pickers have raced each other on all 4 cars. Block 2 then shifts every player to a new rival and runs through the same 4-round car rotation, until 4 blocks (16 rounds total) have paired every player with every opponent on every car.

## Scoring & Winning

Each round is a duel: the player with the better time on the shared car wins 1 point for their team, the other player scores 0. If both players tie exactly, neither team scores a point for that duel. A DNF (did not finish) counts as the worst possible time, so a player who finishes beats one who DNFs, and a duel where both players DNF is a draw with no points awarded. There's a duel per player each round, so up to N points are handed out per round and up to N³ points across the whole match (N² rounds × N points per round).

The team with the most total points at the end of the last block wins the match.

The match also ends early if the trailing team can no longer catch up. Once its point deficit exceeds the number of points still up for grabs in the remaining rounds, the leading team is declared the winner without playing out the rest of the blocks.

# Envimix Rounds

Every map is played over as many rounds as there are cars available on the map or the gamemode (up to 11 cars).

## Rotation

Every player starts the map in the same car, and every round the whole lobby shifts together to the next car in the official order, wrapping back to the start once they reach the end:

CanyonCar, StadiumCar, ValleyCar, LagoonCar, TrafficCar, DesertCar, SnowCar, RallyCar, IslandCar, BayCar, CoastCar

If the pool is smaller than 11, this same order is just followed with the missing cars skipped. Since there are exactly as many rounds as cars in the pool, everyone drives every car exactly once, and the map ends right as the rotation would loop back to the car everyone started in.

## Scoring & Winning

Like regular Rounds gamemode.

# Envimix Mixed Rounds

Envimix Mixed Rounds is Envimix Rounds with a twist: instead of the whole lobby sharing the same car every round, each player rotates on their own individual offset, so most rounds put you on a car with a completely different set of rivals.

## Rotation

The map still runs for as many rounds as there are cars in the pool (up to 11), following the same official order: CanyonCar, StadiumCar, ValleyCar, LagoonCar, TrafficCar, DesertCar, SnowCar, RallyCar, IslandCar, BayCar, CoastCar.

Each player's starting position is assigned by first putting the whole lobby into a random order, then handing out positions 0, 1, 2, ... along the car list, wrapping back to 0 if there are more players than cars. Every round they shift one further along it, wrapping back to the start once they reach the end, exactly like Envimix Rounds, just with everyone starting from a different point instead of all together. Spreading the starting positions this way keeps every car's group as evenly sized as possible each round (never more than 1 player apart), rather than leaving it to chance for some cars to be crowded while others sit empty.

## Scoring & Winning

Each car has its own mini-leaderboard every round: if 3 players end up sharing a car that round, the fastest of them gets 3 points, second gets 2, third gets 1. A car with only 2 players sharing it pays out 2 and 1, and a car nobody else picked that round is worth a flat 1 point to its lone driver. Anyone who DNFs earns 0 points regardless of which car they were on or how many others shared it.

Standings are every player ranked by their total points once all rounds are done.

# Envimix Gambit

Envimix Gambit rewards variety: every round, the less popular your car choice, the more points are on the table for you, though you still have to beat whoever else made the same rare pick to cash in on it.

## Car Picks

The map runs for as many rounds as there are cars in the pool, and each player can only drive each car once. But instead of following a fixed rotation, every round each player freely picks any car they haven't driven yet.

## Scoring & Winning

After a round finishes, players are grouped by the car they picked. Groups are then sorted smallest to largest. The car fewer people picked outranks a car more people picked, with ties broken by whichever group's fastest time is quicker. Within each group, players are ranked by their own time, same DNF and tie rules as the other Envimix gamemodes.

Stacking the groups in that order (rarest group first, its fastest player first) produces one single ranking for the whole round. Points are then handed out down that ranking like a normal race: the player at the very top gets the most points, and each spot below gets one point less, down to 1 point for the last player overall.

So the fastest player on the least-picked car always scores the most points that round, but coming in last on that same rare car still scores worse than someone who wins outright on a more popular car. Standings are every player ranked by their total points once all rounds are done.

### Example: one round with 6 players

Say this round's picks split into 3 groups: CanyonCar (picked by Alice only), StadiumCar (picked by Bob and Carol), and ValleyCar (picked by Dave, Eve and Finn). Sorted rarest first, and ranked by time within each group:

| Overall rank | Player | Car | Points |
|:---:|:---:|:---:|:---:|
| 1 | Alice | CanyonCar | 6 |
| 2 | Bob | StadiumCar (faster of the two) | 5 |
| 3 | Carol | StadiumCar | 4 |
| 4 | Dave | ValleyCar (fastest of the three) | 3 |
| 5 | Eve | ValleyCar | 2 |
| 6 | Finn | ValleyCar (slowest) | 1 |

Alice tops the round on points alone, purely because nobody else risked CanyonCar that round, even if her lap time would've only placed her mid-pack against Dave, Eve and Finn on ValleyCar.