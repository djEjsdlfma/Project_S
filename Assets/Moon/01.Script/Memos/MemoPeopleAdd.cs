using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Moon._01.Script.Memos
{
    public class MemoPeopleAdd : MonoBehaviour
    {
        [SerializeField] private ScriptFinderSO memoSystemFinder;

        private MemoSystem _memoSystem;

        private void Start()
        {
            _memoSystem = memoSystemFinder.GetTarget<MemoSystem>();
        }

        public void FindNewPeople(Human people)
        {
            _memoSystem.NewCharacter(people);
        }
    }
}