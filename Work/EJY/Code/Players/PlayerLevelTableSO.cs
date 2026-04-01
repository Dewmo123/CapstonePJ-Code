using System;
using UnityEngine;

namespace Code.Players
{
    [CreateAssetMenu(fileName = "Player Level Table", menuName = "SO/Players/Level Table", order = 0)]
    public class PlayerLevelTableSO : ScriptableObject
    {
        public int[] requestLevels;
        
        public int this[int idx]
        {
            get
            {
                if(idx < 1) return 0;
                
                return requestLevels[idx - 1];
            }
        }
    }
}