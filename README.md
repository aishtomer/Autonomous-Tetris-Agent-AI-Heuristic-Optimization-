# Autonomous-Tetris-Agent-AI-Heuristic-Optimization-
An autonomous AI agent capable of clearing an average of 682 lines per game by implementing a linear evaluation function based on six weighted board-state factors.



https://github.com/aishtomer/Strategist/assets/91372700/19d8d25f-57f8-4e3e-958c-8fdfcf3a83e4



# Algorithm
The goal of the AI is to choose the most effective move to perform from the list of valid moves. So, we need to assign some sort of score to each move and choose the move with the highest score. 

## Designing the Formula
We could first simulate the move and then analyze the state of the board.There are several factors of the board state which could negatively affect the state of the board and hence, the efficiency of the move.

1. Total number of holes: Count how many empty cells have filled cells above them in the same column. 
2. Add up how tall each column is.
3. See how many times neighboring cells switch between empty and filled in rows. 
4. Check how often cells in columns switch between empty and filled. 
5. Look at how high the top cell of the current shape is.
6. Squared sum of well-like structures heights: Total of squared heights of narrow 1-celled gaps between filled cells with no filled cells above. 

There are few things to consider, since we are adding a single tetromino in the current simulated move. We are technically affecting only a small part of the board and obviously we cannot expect a single tetromino to fill all the holes, or wells in the board. 

So, it is important that we scale the factor values by width(more like average factor value per column). But the height of the top-cell of the current tetromino doesn’t need to be scaled since it’s only a single value. We also need to add weights to the factor values, since they might have an inverse relation with the fitness score but they don’t affect it equally.

So, final fitness score formula will be:
	
	Fitness score  = 	- weight1 * (piece_height)
                    - weight2 * (total_row_flips / board_width)
                    - weight3 * (total_col_flips / board_width)
                    - weight4 * (total_number_holes /  board_width)
                    - weight5 * (total_column_heights / board_width)
	                  - weight6 * ((total_well_height)2 / board_width)

## Weight-Tuning
So, the idea is to start with a random set of weights and use an evolutionary algorithm to mutate the set of weights to find the efficient one which ultimately results in a high game score. I decided to use the evolution library provided as part of the package to find such a set of weights.

### Evolution graph for weights
![image(2)](https://github.com/aishtomer/Strategist/assets/91372700/b85dac7f-35c4-4834-8a28-05a0512ec930)


# Reflection
## What worked
This AI works well enough, since  it considers various aspects of the boardstate to test the efficiency of the move.
## What didn’t work
- There are probably many more important factors which if considered could help in improving the performance of AI.
- AI relies on preset weights that might not work well enough for all Tetris situations.
- It doesn’t consider upcoming pieces, which definitely affects the game dynamics.
## Future Improvements
- Adjusting weights based on the game state dynamically.
- Using lookahead strategies to predict the impact of upcoming pieces.
- Trying different fitness functions and machine learning methods for a more adaptable approach.

# Testing

![Screenshot 2024-03-18 at 19-03-55 Tetris Report](https://github.com/aishtomer/Strategist/assets/91372700/4e6a011e-7730-488f-85dd-14a962b0d594)

