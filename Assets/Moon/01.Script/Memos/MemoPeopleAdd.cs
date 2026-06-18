using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Moon._01.Script.Memos
{
    public class MemoPeopleAdd : MonoBehaviour
    {

        [SerializeField]private MemoSystem memoSystem;

        public void FindLeeJaeYoon()
        {
            FindNewPeople(Human.LeeJaeYoon);
        }
        
        public void FindJeongDaeun()
        {
            FindNewPeople(Human.JeongDaeun);
        }
        
        public void FindParkYul()
        {
            FindNewPeople(Human.ParkYul);
        }
        
        public void FindYunSeo()
        {
            FindNewPeople(Human.YunSeo);
        }
        
        public void FindChoiMyungjin()
        {
            FindNewPeople(Human.ChoiMyungjin);
        }

        private void FindNewPeople(Human people)
        {
            memoSystem.NewCharacter(people);
        }
    }
}