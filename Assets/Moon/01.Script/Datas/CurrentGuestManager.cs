using System;
using CSILib.SoundManager.RunTime;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    public class CurrentGuestManager : MonoBehaviour
    {
        public string CurrentGuest { get; private set; }
        
        public static readonly string[] C = { "LeeJaeYun" , "NaSohee" , "ParkYul" , "ChoiMyeongJin" , "JungDaEun" };

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