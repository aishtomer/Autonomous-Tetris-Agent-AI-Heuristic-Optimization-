    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    using AlanZucconi.Data;
    
    namespace AlanZucconi.Tetris
    {
    	[CreateAssetMenu(
        	fileName = "TetrisAI",
        	menuName = "Tetris/Scripts/TetrisAI"
    	)]
    
    	public class TetrisAI : ScriptableObject
    	{
        	[Header("Student Data")]
        	public string StudentLogin = "yourlogin";
        	public string StudentName = "FirstName LastName";
        	public string StudentEmail = "youremail@gold.ac.uk";
    
        	public float sumHolesWt;
        	public float sumHeightWt;
        	public float rowFlipsWt;
        	public float colFlipsWt;
        	public float pieceHeightWt;
        	public float sumWellWt;
    
    
    
        	// Will be initialised by the Automation tool
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
                	// Calculate the fitness for each move
                	float fitness = CalculateMoveFitness(moves[i]);
    
                	// Update the best move if the current move has higher fitness
                	if (fitness > bestFitness)
                	{
                    	bestFitness = fitness;
                    	bestMoveIndex = i;
                	}
            	}
    
            	// Return the index of the chosen move
            	return bestMoveIndex;
        	}
    
        	// Method to calculate fitness for a specific move
        	private float CalculateMoveFitness(Move move)
        	{
            	// Simulate the move to get the next state
            	TetrisState nextState = Tetris.SimulateMove(move);
    
            	// Calculate fitness based on various metrics
            	float sumHoles = CalculateSumHoles(nextState);
            	float sumHeight = CalculateSumHeight(nextState);
            	float rowFlip = CalculateRowFlip(nextState);
            	float colFlip = CalculateColFlip(nextState);
            	float pieceHeight = CalculatePieceHeight(move);
            	float sumWell = CalculateSumWell(nextState);
    
            	// Given weights, determine the fitness of the grid
            	float width = Tetris.Size.x;
            	float fitness = (
                	- sumHolesWt * sumHoles / width
                	- sumHeightWt * sumHeight / width
                	- rowFlipsWt * rowFlip / width
                	- colFlipsWt * colFlip / width
                	- pieceHeightWt * pieceHeight
                	- sumWellWt * sumWell / width
            	);
    
            	return fitness;
        	}
    
        	// Method to calculate the sum of holes in the grid
        	private float CalculateSumHoles(TetrisState state)
        	{
            	int sumHoles = 0;
    
            	// Iterate through columns
            	for (int x = 0; x < state.Width; x++)
            	{
                	bool covered = false;
    
                	// Iterate through rows from bottom to top
                	for (int y = state.Height - 1; y >= 0; y--)
                	{
                    	bool cellFilled = state.Get(x, y);
    
                    	// If the cell is empty and there is a filled cell above, we have a hole
                    	if (!cellFilled && covered)
                    	{
                        	sumHoles++;
                    	}
    
                    	covered |= cellFilled;
                	}
            	}
    
            	return sumHoles;
        	}
    
        	// Method to calculate the sum of column heights in the grid
        	private float CalculateSumHeight(TetrisState state)
        	{
            	int sumHeight = 0;
    
            	// Iterate through columns
            	for (int x = 0; x < state.Width; x++)
            	{
                	int columnHeight = state.GetColumnHeight(x);
                	sumHeight += columnHeight;
            	}
    
            	return sumHeight;
        	}
    
        	// Method to calculate the number of row flips in the grid
        	private float CalculateRowFlip(TetrisState state)
        	{
            	int rowFlip = 0;
    
            	// Iterate through rows
            	for (int y = 0; y < state.Height; y++)
            	{
                	bool covered = false;
    
                	// Iterate through columns
                	for (int x = 0; x < state.Width; x++)
                	{
                    	bool cellFilled = state.Get(x, y);
                    	rowFlip += cellFilled ^ covered ? 1 : 0;
                    	covered = cellFilled;
                	}
            	}
    
            	return rowFlip;
        	}
    
        	// Method to calculate the number of column flips in the grid
        	private float CalculateColFlip(TetrisState state)
        	{
            	int colFlip = 0;
    
            	// Iterate through columns
            	for (int x = 0; x < state.Width; x++)
            	{
                	bool topCellFilled = state.Get(x, state.Height - 1);
    
                	// Iterate through rows from bottom to top
                	for (int y = state.Height - 2; y >= 0; y--)
                	{
                    	bool cellFilled = state.Get(x, y);
                    	colFlip += cellFilled ^ topCellFilled ? 1 : 0;
                    	topCellFilled = cellFilled;
                	}
            	}
    
            	return colFlip;
        	}
    
        	// Method to calculate the piece height for a specific move
        	private float CalculatePieceHeight(Move move)
        	{
            	Tetromino tetromino = move.Tetromino;
            	Vector2Int position = move.Position;
            	int width = tetromino.Width;
            	int height = tetromino.Height;
    
            	// Iterate through columns to find the first non-empty cell from the top for the specified position
            	for (int x = 0; x < width; x++)
            	{
                	for (int y = height - 1; y >= 0; y--)
                	{
                    	if (tetromino.Area[x, y])
                    	{
                        	// Found the top cell of the most recently placed piece
                        	return position.y - y;
                    	}
                	}
            	}
    
            	// No piece in the specified position, return 0 or a suitable default value
            	return 0;
        	}
    
        	// Method to calculate the sum of squared well heights in the grid
        	private float CalculateSumWell(TetrisState state)
        	{
            	int sumWell = 0;
    
            	for (int x = 1; x < state.Width - 1; x++)
            	{
                	int leftHeight = state.GetColumnHeight(x - 1);
                	int rightHeight = state.GetColumnHeight(x + 1);
                	int currentHeight = state.GetColumnHeight(x);
    
                	int wellHeight = Mathf.Min(leftHeight, rightHeight) - currentHeight;
                	sumWell += wellHeight * wellHeight;
            	}
    
            	return sumWell;
        	}
    
    	}
    }
