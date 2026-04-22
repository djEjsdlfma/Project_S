using UnityEngine;

namespace CSI._01Script.Animation
{
    [CreateAssetMenu(fileName = "Animator param", menuName = "SO/Animator/Param", order = 0)]
    public class AnimParamSO : ScriptableObject
    {
        [field: SerializeField] public string ParamName { get; private set; }
        [field: SerializeField] public int HashValue { get; private set; }

        private void OnValidate()
        {
            HashValue = Animator.StringToHash(ParamName);
        }
    }
}