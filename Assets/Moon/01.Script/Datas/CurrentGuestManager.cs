using System;
using CSILib.SoundManager.RunTime;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    public class CurrentGuestManager : MonoBehaviour
    {
        public string CurrentGuest { get; private set; }
        
        public readonly string[] C = { "Character_1" , "Character_2" , "Character_3" , "Character_4" , "Character_5" };

        private void Awake()
        {
            CurrentGuest = C[0];
        }

        public void SetCurrentGuest(string guestName)
        {
            CurrentGuest = guestName;
        }
    }
}