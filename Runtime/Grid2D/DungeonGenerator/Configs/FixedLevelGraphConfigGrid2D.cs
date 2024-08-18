using System;
using System.Collections.Generic;
using UnityEngine;

namespace Edgar.Unity
{
    [Serializable]
    public class FixedLevelGraphConfigGrid2D
    {
        // Change this to a list of LevelGraphs
        public List<LevelGraph> LevelGraphs = new List<LevelGraph>();

        public bool UseCorridors = true;

        // Add a method to randomly select a LevelGraph
        public LevelGraph GetRandomLevelGraph()
        {
            if (LevelGraphs == null || LevelGraphs.Count == 0)
            {
                Debug.LogError("No LevelGraphs assigned!");
                return null;
            }
            int randomIndex = UnityEngine.Random.Range(0, LevelGraphs.Count);
            return LevelGraphs[randomIndex];
        }
    }
}