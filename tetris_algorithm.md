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
	- weight6 * ((total_well_height)*2 / board_width)

## Weight-Tuning

So, the idea is to start with a random set of weights and use an evolutionary algorithm to mutate the set of weights to find the efficient one which ultimately results in a high game score. I decided to use the evolution library provided as part of the package to find such a set of weights.

### Note

I have changed the core code of the game to make my evolutionary algorithm work. So, I will be attaching two unit packages. 

* First is for the AI that uses the weights produced via the evolution library directly. This will simply contain one C\# file “students/2023-24/astom001/TetrisAI\_astom001”, which can be used to see how final weights are performing in the game.  
* Second is for the evolutionary algorithm, this will be the self-contained package including all the dependencies and the modified core code, it can be used to test the evolution process used to find the weights.

**Steps to run second package**

1. Open the “Tetris” scene present in the “assets/Tetris” folder.  
2. Click on the “TetrisSystem” object, run the scene, click on the “Run: StartEvolution” button present on the right-side of the inspector of the “TetrisSystem” object.  
3. Expand the Plot drop-down to see the progress.

## Process of Creating Evolutionary Algorithm

**Step1: Create an instance of an evolutionary system to evolve the set of weights.**

`// Assets/Tetris/Evolution/TetrisAI_Evolution`

`using System.Collections;`  
`using System.Collections.Generic;`  
`using UnityEngine;`

`using AlanZucconi.AI.Evo;`

`public class TetrisAI_Evolution : EvolutionSystem<ArrayGenome>`  
`{`  
      
`}`

**Step 2: Create world to simulate the set of weights in the game.**

**Step 2.1:** Modify TetrisAI class so that it can be used during ResetSimulation() to create a new instance of an AI and also add all the methods needed to make the AI score the moves and select the best one while using the set of weights used in the simulation.

* Implemented StartTetrisGame() to give the Tetris variable of this class access to the Tetris game in the simulated world.  
* Added variables to store weight values for different factors.  
* Implemented ChooseMove() function and its dependencies

`// Assets/Tetris/Scripts/TetrisAI`  
`using System.Collections;`  
`using System.Collections.Generic;`  
`using UnityEngine;`

`using AlanZucconi.Data;`

`namespace AlanZucconi.Tetris  
{  
	[CreateAssetMenu(  
    	fileName = "TetrisAI",
    	menuName = "Tetris/Scripts/TetrisAI"
	)]

	public class TetrisAI : ScriptableObject`  
	{  
    	[Header("Student Data")]
    	public string StudentLogin = "yourlogin"; 
    	public string StudentName = "FirstName LastName";
    	public string StudentEmail = "youremail@gold.ac.uk";

    	public float pieceHeightWt;  
		public float rowFlipsWt;  
    	public float colFlipsWt; 
		public float sumHolesWt;  
    	public float sumHeightWt;  
    	public float sumWellWt;

    	// Will be initialized by the Automation tool  
    	[Header("Statistics")]  
    	[Space]  
    	[ReadOnly]  
    	public float MedianScore = 0;  
    	[ReadOnly]  
    	public float AverageScore = 0;

    	[Header("Results")]  
    	//[LinePlot(LabelX = "test", LabelY = "points")]  
    	//[ScatterPlot(LabelX = "test", LabelY = "points")]  
    	//[HistogramPlot(Bins=15, LabelX = "points", LabelY = "count")]  
    	[HistogramPlot(LabelX = "points", LabelY = "count")]  
    	public PlotData PlotData = new PlotData();

    	[HideInInspector]  
    	public TetrisGame Tetris;

    	public void SetTetrisGame(TetrisGame tetris)  
    	{  
        	Tetris = tetris;  
    	}

    	// public abstract int ChooseMove(Move[] moves);

    	// Can be used to initialisation  
    	public virtual void Initialise() { }

    	// Method to choose the best move based on fitness  
    	public virtual int ChooseMove(Move[] moves)  
    	{  
        	float bestFitness = float.MinValue;  
        	int bestMoveIndex = 0;

        	// Iterate through all the possible moves  
        	for (int i = 0; i < moves.Length; i++)  
        	{  
            	`// Calculate the fitness for each move`  
            	`float fitness = CalculateMoveFitness(moves[i]);`

            	`// Update the best move if the current move has higher fitness`  
            	`if (fitness > bestFitness)`  
            	`{`  
                	`bestFitness = fitness;`  
                	`bestMoveIndex = i;`  
            	`}`  
        	`}`

        	`// Return the index of the chosen move`  
        	`return bestMoveIndex;`  
    	`}`

    	`// Method to calculate fitness for a specific move`  
    	`private float CalculateMoveFitness(Move move)`  
    	`{`  
        	`// Simulate the move to get the next state`  
        	`TetrisState nextState = Tetris.SimulateMove(move);`

        	`// Calculate fitness based on various metrics`  
        	`float pieceHeight = CalculatePieceHeight(move);`  
`float rowFlip = CalculateRowFlip(nextState);`  
        	`float colFlip = CalculateColFlip(nextState);`  
`float sumHoles = CalculateSumHoles(nextState);`  
        	`float sumHeight = CalculateSumHeight(nextState);`  
        	`float sumWell = CalculateSumWell(nextState);`

        	`// Given weights, determine the fitness of the grid`  
        	`float width = Tetris.Size.x;`  
        	`float fitness = (`  
            	`- sumHolesWt * sumHoles / width`  
            	`- sumHeightWt * sumHeight / width`  
            	`- rowFlipsWt * rowFlip / width`  
            	`- colFlipsWt * colFlip / width`  
            	`- pieceHeightWt * pieceHeight`  
            	`- sumWellWt * sumWell / width`  
        	`);`

        	`return fitness;`  
    	`}`

    	`// Method to calculate the sum of holes in the grid`  
    	`private float CalculateSumHoles(TetrisState state)`  
    	`{`  
        	`int sumHoles = 0;`

        	`// Iterate through columns`  
        	`for (int x = 0; x < state.Width; x++)`  
        	`{`  
            	`bool covered = false;`

            	`// Iterate through rows from bottom to top`  
            	`for (int y = state.Height - 1; y >= 0; y--)`  
            	`{`  
                	`bool cellFilled = state.Get(x, y);`

                	`// If the cell is empty and there is a filled cell above, we have a hole`  
                	`if (!cellFilled && covered)`  
                	`{`  
                    	`sumHoles++;`  
                	`}`

                	`covered |= cellFilled;`  
            	`}`  
        	`}`

        	`return sumHoles;`  
    	`}`

    	`// Method to calculate the sum of column heights in the grid`  
    	`private float CalculateSumHeight(TetrisState state)`  
    	`{`  
        	`int sumHeight = 0;`

        	`// Iterate through columns`  
        	`for (int x = 0; x < state.Width; x++)`  
        	`{`  
            	`int columnHeight = state.GetColumnHeight(x);`  
            	`sumHeight += columnHeight;`  
        	`}`

        	`return sumHeight;`  
    	`}`

    	`// Method to calculate the number of row flips in the grid`  
    	`private float CalculateRowFlip(TetrisState state)`  
    	`{`  
        	`int rowFlip = 0;`

        	`// Iterate through rows`  
        	`for (int y = 0; y < state.Height; y++)`  
        	`{`  
            	`bool covered = false;`

            	`// Iterate through columns`  
            	`for (int x = 0; x < state.Width; x++)`  
            	`{`  
                	`bool cellFilled = state.Get(x, y);`  
                	`rowFlip += cellFilled ^ covered ? 1 : 0;`  
                	`covered = cellFilled;`  
            	`}`  
        	`}`

        	`return rowFlip;`  
    	`}`

    	`// Method to calculate the number of column flips in the grid`  
    	`private float CalculateColFlip(TetrisState state)`  
    	`{`  
        	`int colFlip = 0;`

        	`// Iterate through columns`  
        	`for (int x = 0; x < state.Width; x++)`  
        	`{`  
            	`bool topCellFilled = state.Get(x, state.Height - 1);`

            	`// Iterate through rows from bottom to top`  
            	`for (int y = state.Height - 2; y >= 0; y--)`  
            	`{`  
                	`bool cellFilled = state.Get(x, y);`  
                	`colFlip += cellFilled ^ topCellFilled ? 1 : 0;`  
                	`topCellFilled = cellFilled;`  
            	`}`  
        	`}`

        	`return colFlip;`  
    	`}`

    	`// Method to calculate the piece height for a specific move`  
    	`private float CalculatePieceHeight(Move move)`  
    	`{`  
        	`Tetromino tetromino = move.Tetromino;`  
        	`Vector2Int position = move.Position;`  
        	`int width = tetromino.Width;`  
        	`int height = tetromino.Height;`

        	`// Iterate through columns to find the first non-empty cell from the top for the specified position`  
        	`for (int x = 0; x < width; x++)`  
        	`{`  
            	`for (int y = height - 1; y >= 0; y--)`  
            	`{`  
                	`if (tetromino.Area[x, y])`  
                	`{`  
                    	`// Found the top cell of the most recently placed piece`  
                    	`return position.y - y;`  
                	`}`  
            	`}`  
        	`}`

        	`// No piece in the specified position, return 0`  
        	`return 0;`  
    	`}`

    	`// Method to calculate the sum of squared well heights in the grid`  
    	`private float CalculateSumWell(TetrisState state)`  
    	`{`  
        	`int sumWell = 0;`

        	`for (int x = 1; x < state.Width - 1; x++)`  
        	`{`  
            	`int leftHeight = state.GetColumnHeight(x - 1);`  
            	`int rightHeight = state.GetColumnHeight(x + 1);`  
            	`int currentHeight = state.GetColumnHeight(x);`

            	`int wellHeight = Mathf.Min(leftHeight, rightHeight) - currentHeight;`  
            	`sumWell += wellHeight * wellHeight;`  
        	`}`

        	`return sumWell;`  
    	`}`

	`}`  
`}`

**Step 2.2:** Make the run() method in Automation class public, so that it can be accessed in the simulated world and used to find median scores, which will later be used in the getScore() method of the simulated world.

`public void Run()`  
    	`{`  
        	`//Snake.DeathCallback.AddListener(SimulationDone);`

        	`StartCoroutine(Run_Coroutine());`  
    	`}`

**Step 2.3:** Create a simulated world that will be used to find the efficient weights and implements the methods that will be used to control the simulated world.

`// Assets/Tetris/Scripts/Tetris_World`  
`using System.Collections;`  
`using System.Collections.Generic;`  
`using UnityEngine;`

`using AlanZucconi.AI.Evo;`  
`using AlanZucconi.Tetris;`

`public class Tetris_World : MonoBehaviour,`  
	`IGenomeFactory<ArrayGenome>,`  
	`IWorld<ArrayGenome>`  
`{`  
	`public TetrisGame Tetris;`  
	`public TetrisAI AI;`

	`private TetrisAutomation automation;`

	`public ArrayGenome Genome;`

	`public ArrayGenome GetGenome()`  
	`{`  
    	`return Genome;`  
	`}`

	`public float GetScore()`  
	`{`  
    	`// Initialize TetrisAutomation`  
    	`automation = gameObject.AddComponent<TetrisAutomation>();`  
    	`automation.Tetris = Tetris;`  
    	`automation.TestsPerAI = 50;`  
    	`automation.AIs = new List<TetrisAI> { AI };`

    	`automation.Run();`

    	`return AI.MedianScore;`  
	`}`

	`public ArrayGenome Instantiate()`  
	`{`  
    	`ArrayGenome genome = new ArrayGenome(6);`  
    	`for (int i=0; i<6; i++)`  
    	`{`  
        	`genome.Params[i] = Random.Range(0f, 1f);`  
    	`}`  
    	`return genome;`  
	`}`

	`public bool IsDone()`  
	`{`  
    		`return !Tetris.Running;`  
	`}`

	`public void ResetSimulation()`  
	`{`  
    	`TetrisAI AI = ScriptableObject.CreateInstance<TetrisAI>();`  
    	`AI.SetTetrisGame(Tetris);`  
    	`AI.Tetris = Tetris;`  
    	`Tetris.TetrisAI = AI;`  
	`}`

	`public void SetGenome(ArrayGenome genome)`  
	`{`  
    	`AI.sumHolesWt = genome.Params[0];`  
    	`AI.sumHeightWt = genome.Params[1];`  
    	`AI.rowFlipsWt = genome.Params[2];`  
    	`AI.colFlipsWt = genome.Params[3];`  
    	`AI.pieceHeightWt = genome.Params[4];`  
    	`AI.sumWellWt = genome.Params[5];`

    	`Genome = genome;`  
	`}`

	`public void StartSimulation()`  
	`{`  
    	`// Initialize TetrisAutomation`  
    	`automation = gameObject.AddComponent<TetrisAutomation>();`  
    	`automation.Tetris = Tetris;`  
    	`automation.TestsPerAI = 100;`  
    	`automation.AIs = new List<TetrisAI> { AI };`

    	`Tetris.SetAI(AI);`  
    	`Tetris.StartGame();`  
	`}`  
`}`

**Step 3: Create objects to run the evolution system and various worlds in it in the “Tetris” scene.**

* Created an empty object and renamed it “TetrisSystem” and added “TetrisAI\_Evolution” script to it, and set various parameters associated with it.  
* Created another empty object and renamed it “World”, added “TetrisGame” and then created multiple copies of this “World” object which will later be used as simulated worlds to test various sets of weights and evolutionary algorithms will help in finding the best set.  
* Run the TetrisSystem by clicking on “StartEvolution” button

# Reflection

## What worked

* This AI works well enough, since  it considers various aspects of the boardstate to test the efficiency of the move.

## What didn’t work

* There are probably many more important factors which if considered could help in improving the performance of AI.  
* AI relies on preset weights that might not work well enough for all Tetris situations.  
* It doesn’t consider upcoming pieces, which definitely affects the game dynamics.

## Future Improvements

* Adjusting weights based on the game state dynamically.  
* Using lookahead strategies to predict the impact of upcoming pieces.  
* Trying different fitness functions and machine learning methods for a more adaptable approach.
