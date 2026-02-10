    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    using System.Linq;
    
    namespace AlanZucconi.Tetris
    {
    	public class TetrisAutomation : MonoBehaviour
    	{
        	[Header("Settings")]
        	[EditorOnly]
        	public TetrisGame Tetris;
        	[Range(0f, 1f)]
        	public float Delay = 0f;
    
    
        	[Header("Testing Parameters")]
        	[Range(1, 1000)]
        	public int TestsPerAI = 500;
        	public bool Rendering = false;
        	//public bool PauseOnDeath = false;
        	[EditorOnly]
        	public bool ClearData = true;
    
        	[Space]
        	public List<TetrisAI> AIs;
    
    
    
    
        	// Use this for initialization
        	//void Start()
        	[Button(Editor = false)]
         	public void Run()
        	{
            	//Snake.DeathCallback.AddListener(SimulationDone);
    
            	StartCoroutine(Run_Coroutine());
        	}
    
        	IEnumerator Run_Coroutine()
        	{
            	foreach (TetrisAI ai in AIs)
            	{
                	Debug.Log("Testing AI: [" + ai.name + "]...");
    
                	if (ClearData)
                    	ai.PlotData.Data.Clear();
    
                	for (int i = 0; i < TestsPerAI; i++)
                	{
                    	Debug.Log("\tSimulation " + i + "\tof " + TestsPerAI + "...");
    
                    	// Setup
                    	Tetris.Delay = Delay;
                    	Tetris.Rendering = Rendering;
                    	//Tetris.PauseOnDeath = PauseOnDeath;
                    	Tetris.SetAI(ai);
    
                    	// Starts the game
                    	Tetris.StartGame();
                    	yield return new WaitWhile(() => Tetris.Running); // Wait until simulation done
                    	Tetris.StopGame();
    
                    	CollectStats(ai);
                	}
            	}
    
    
    
            	Debug.Log("DONE!");
        	}
    
    
    
    
    
        	public void CollectStats(TetrisAI ai)
        	{
            	//ai.PlotData.Add(new Vector2(0, Tetris.Turn));
            	ai.PlotData.Add(new Vector2(ai.PlotData.Data.Count, Tetris.Turn));
    
            	// Calculates the statistics
            	ai.MedianScore = ai.PlotData.Data.Median(point => point.y);
            	ai.AverageScore = ai.PlotData.Data.Average(point => point.y);
    
    #if UNITY_EDITOR
            	UnityEditor.EditorUtility.SetDirty(ai);
    #endif
        	}
    
    
    	}
    }
