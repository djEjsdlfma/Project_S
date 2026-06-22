
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.System___Manager;
using UnityEngine;

namespace LSW._02._Code.UI
{
    public class ChatProfileContainer : MonoBehaviour
    {
        private List<GuestProfile> _profiles = new List<GuestProfile>();
        private GuestProfile _currentProfile;
        private GameStatueCore _gameStatueCore;
        
        private void Awake()
        {
            EnsureProfilesCached();
        }

        private void EnsureProfilesCached()
        {
            if (_profiles.Count == 0)
            {
                _profiles = GetComponentsInChildren<GuestProfile>(true).ToList();
            }
        }

        public void InitializeProfiles(BubbleManager bubbleManager)
        {
            if(_gameStatueCore == null)
                _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            
            EnsureProfilesCached();

            foreach (var profile in _profiles)
            {
                Guest targetGuest = profile.Guest;
                if (targetGuest == Guest.None)
                {
                    var btn = profile.GetComponentInChildren<GuestProfileSelectBtn>();
                    if (btn != null) 
                        targetGuest = btn.Guest;
                }

                if (targetGuest == Guest.None) 
                    continue;
                
                string lastMsg = bubbleManager.GetLastDialogueContent(targetGuest);
                profile.SetProfile(lastMsg, true);
                
                bool isUnlocked = (int)profile.Guest < _gameStatueCore.CurrentDay;
                profile.gameObject.SetActive(isUnlocked);
                profile.IsActivable = isUnlocked;
            }
        }
        
        public void EnableActivableProfile()
        {
            if (_gameStatueCore == null)
            {
                _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            }
            
            for (int i = 1; i <= 5; i++)
            {
                EnableProfileOnly((Guest)i);
            }
        }
        
        public void EnableProfileOnly(Guest guest)
        {
            foreach (var profile in _profiles)
            {
                if(profile.Guest == guest && profile.IsActivable)
                    profile.gameObject.SetActive(true);
            }
            _currentProfile = null;
        }
        
        public void ChangeProfileToActivable(Guest guest)
        {
            foreach (var profile in _profiles)
            {
                if (profile.Guest == guest)
                {
                    profile.IsActivable = true;
                }
            }
        }

        public void DisableAllProfile(GuestProfile excludedProfile)
        {
            foreach (var profile in _profiles)
            {
                profile.OpenChat(false);
                profile.gameObject.SetActive(false);
            }

            if (excludedProfile != null)
            {
                excludedProfile.gameObject.SetActive(true);
                excludedProfile.transform.SetAsFirstSibling();
                excludedProfile.OpenChat(true);
            }

            _currentProfile = excludedProfile;
        }

        public void SetCurrentProfile(string lastMessage, bool hasAlarm = false)
        {
            if (_currentProfile == null) return;
            _currentProfile.SetProfile(lastMessage, hasAlarm);
        }

        public bool IsChatActive()
        {
            if (_profiles.Count <= 0) return false;
            return _profiles.Exists(p => p.IsOpenedChat);
        }

        public void SetAllProfileClosed()
        {
            foreach (var profile in _profiles)
            {
                profile.OpenChat(false);
            }
        }
    }
}