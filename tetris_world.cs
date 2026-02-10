    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    using AlanZucconi.AI.Evo;
    using AlanZucconi.Tetris;
    
    public class Tetris_World : MonoBehaviour,
    	IGenomeFactory<ArrayGenome>,
    	IWorld<ArrayGenome>
    {
    	public TetrisGame Tetris;
    	public TetrisAI AI;
    
    	private TetrisAutomation automation;
    
    	public ArrayGenome Genome;
    
    	public ArrayGenome GetGenome()
    	{
        	return Genome;
    	}
    
    	public float GetScore()
    	{
        	// Initialize TetrisAutomation
        	automation = gameObject.AddComponent<TetrisAutomation>();
        	automation.Tetris = Tetris;
        	automation.TestsPerAI = 50;
        	automation.AIs = new List<TetrisAI> { AI };
    
        	automation.Run();
    
        	return AI.MedianScore;
    	}
    
    	public ArrayGenome Instantiate()
    	{
        	ArrayGenome genome = new ArrayGenome(6);
        	for (int i=0; i<6; i++)
        	{
            	genome.Params[i] = Random.Range(0f, 1f);
        	}
        	return genome;
    	}
    
    	public bool IsDone()
    	{
        	return !Tetris.Running;
    	}
    
    	public void ResetSimulation()
    	{
        	TetrisAI AI = ScriptableObject.CreateInstance<TetrisAI>();
        	AI.SetTetrisGame(Tetris);
        	AI.Tetris = Tetris;
        	Tetris.TetrisAI = AI;
    	}
    
    	public void SetGenome(ArrayGenome genome)
    	{
        	AI.sumHolesWt = genome.Params[0];
        	AI.sumHeightWt = genome.Params[1];
        	AI.rowFlipsWt = genome.Params[2];
        	AI.colFlipsWt = genome.Params[3];
        	AI.pieceHeightWt = genome.Params[4];
        	AI.sumWellWt = genome.Params[5];
    
        	Genome = genome;
    	}
    
    	public void StartSimulation()
    	{
        	// Initialize TetrisAutomation
        	automation = gameObject.AddComponent<TetrisAutomation>();
        	automation.Tetris = Tetris;
        	automation.TestsPerAI = 100;
        	automation.AIs = new List<TetrisAI> { AI };
    
        	Tetris.SetAI(AI);
        	Tetris.StartGame();
    	}
    }
      
