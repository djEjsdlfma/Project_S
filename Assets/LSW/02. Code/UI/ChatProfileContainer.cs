
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.Core.Cores;
using LSW._02._Code.System___Manager;
using UnityEngine;

namespace LSW._02._Code.UI
{
    public class ChatProfileContainer : MonoBehaviour
    {
        private List<GuestProfile> _profiles;
        private GuestProfile _currentProfile;
        
        private GameStatueCore _gameStatueCore;
        
        private void Awake()
        {
            _profiles = GetComponentsInChildren<GuestProfile>().ToList();
        }

        public void DisableAllProfile(GuestProfile excludedProfile)
        {
            _profiles.ForEach(profile =>
            {
                // profile.gameObject.SetActive(false);
                profile.OpenChat(false);
            });

            if (excludedProfile != null)
            {
                // excludedProfile.gameObject.SetActive(true);
                excludedProfile.transform.SetAsFirstSibling();
                excludedProfile.OpenChat();
            }

            _currentProfile = excludedProfile;
        }

        public void SetCurrentProfile(string lastMessage, bool hasAlarm = false)
        {
            if(_currentProfile == null)
                return;

            _currentProfile.SetProfile(lastMessage, hasAlarm);
        }

        public void EnableCurrentDayProfile()
        {
            int currentDay = _gameStatueCore.CurrentDay;
            EnableProfileOnly((Guest)(currentDay % 5));
            _currentProfile = null;
        }
        
        public void EnableProfileOnly(Guest guest)
        {
            _profiles.ForEach(profile =>
            {
                // profile.gameObject.SetActive(profile.Guest == guest && profile.IsActivable);
            });
            _currentProfile = null;
        }

        public void ChangeProfileToActivable()
        {
            _profiles.ForEach(profile =>
            {
                if((Guest)(_gameStatueCore.CurrentDay % 5) == profile.Guest)
                    profile.IsActivable = true;
            });
        }

        public bool IsChatActive()
        {
            return _profiles.Exists(p => p.IsOpenedChat);
        }

        public void InitializeProfiles(BubbleManager bubbleManager, GameStatueCore gameStatueCore)
        {
            if(_gameStatueCore == null)
                _gameStatueCore = gameStatueCore;
            
            if (_profiles == null || _profiles.Count == 0)
                _profiles = GetComponentsInChildren<GuestProfile>(true).ToList();

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
                
                if((int)profile.Guest > gameStatueCore.CurrentDay)
                    profile.gameObject.SetActive(false);
            }
        }

        public void SetAllProfileClosed()
        {
            _profiles.ForEach(profile => profile.OpenChat(false));
        }
    }
}