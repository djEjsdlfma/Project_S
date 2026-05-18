using UnityEngine;

namespace Moon._01.Script.Cameras.Puzzle.Weight
{
    public class WeightObj : CamObject
    {
        [field:SerializeField] public int Weight { get; protected set; }
        [field: SerializeField] public float MaxRatioToWeightAlive { get; protected set; } = 0.9f;

        public override float Ratio
        {
            get => ratio;
            set
            {
                ratio = value;
                if (ratio <= MaxRatioToWeightAlive)
                {
                    Weight = 0;
                }
            }
        }
    }
}