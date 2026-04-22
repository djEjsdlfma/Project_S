using CSI._01Script.Animation;
using UnityEngine;

namespace LSW._02._Code.Entity
{
    public interface IEntityAnimator
    {
        Animator Animator { get; }
        
        void PlayClip(int clipHash, int layer = -1, float normalPosition = float.NegativeInfinity);

        void SetParam(AnimParamSO param, bool value);
        void SetParam(AnimParamSO param, int value);
        void SetParam(AnimParamSO param, float value);
        void SetParam(AnimParamSO param);

        void ResetBooleanParams();
    }
}