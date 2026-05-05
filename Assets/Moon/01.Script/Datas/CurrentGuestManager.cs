using CSILib.SoundManager.RunTime;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    public class CurrentGuestManager : MonoBehaviour
    {
        public string CurrentGuest { get; private set; }
        
        public readonly string C1 = "Character_1";
        public readonly string C2 = "Character_2";
        public readonly string C3 = "Character_3";
        
        public void SetCurrentGuest(string guestName)
        {
            CurrentGuest = guestName;
        }
    }
}