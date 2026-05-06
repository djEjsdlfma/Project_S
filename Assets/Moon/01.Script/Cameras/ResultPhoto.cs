using System;
using Moon._01.Script.Datas;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Moon._01.Script.Cameras
{
    public class ResultPhoto : MonoBehaviour
    {
        [SerializeField] private int goodScore;
        [SerializeField] private int normalScore;
        [SerializeField] private int badScore;

        [SerializeField] private int goodLikeAbility;
        [SerializeField] private int normalLikeAbility;
        [SerializeField] private int badLikeAbility;
        [SerializeField] private int whatLikeAbility;

        [SerializeField] private ScriptListFinderSO guestScriptFinder;
        
        [SerializeField] private GameObject panel;
        
        private LikeAbility _likeAbility;

        private CurrentGuestManager _currentGuestManager;
        
        public void Result(int score)
        {
            _currentGuestManager = guestScriptFinder.GetTarget<CurrentGuestManager>();
            _likeAbility = guestScriptFinder.GetTarget<LikeAbility>();
            
            _currentGuestManager.SetCurrentGuest(_currentGuestManager.C1);
            
            /*if(String.IsNullOrEmpty(_currentGuestManager.CurrentGuest))
                return;*/
            
            if (score >= goodScore)
            {
                Debug.Log("Good");
                Good();
            }
            else if (score >= normalScore)
            {
                Debug.Log("Normal");
                Normal();
            }
            else if (score >= badScore)
            {
                Debug.Log("Bad");
                Bad();
            }
            else
            {
                Debug.Log("What");
                What();
            }
        }

        private void Good()
        {
            _likeAbility.AddLikeAbility(_currentGuestManager.CurrentGuest, goodLikeAbility);
        }

        private void Normal()
        {
            _likeAbility.AddLikeAbility(_currentGuestManager.CurrentGuest, normalLikeAbility);
        }

        private void Bad()
        {
            _likeAbility.AddLikeAbility(_currentGuestManager.CurrentGuest, badLikeAbility);
        }

        private void What()
        {
            _likeAbility.AddLikeAbility(_currentGuestManager.CurrentGuest, whatLikeAbility);
        }
    }
}