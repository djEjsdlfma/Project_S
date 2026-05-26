
using System.Collections.Generic;
using System.Linq;
using LSW._02._Code.Core;
using LSW._02._Code.Core.Cores;
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
            _gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            
            _profiles = GetComponentsInChildren<GuestProfile>().ToList();
        }

        public void DisableAllProfile(GuestProfile excludedProfile)
        {
            _profiles.ForEach(profile =>
            {
                profile.gameObject.SetActive(false);
                profile.OpenChat(false);
            });

            if (excludedProfile != null)
            {
                excludedProfile.gameObject.SetActive(true);
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

        public void UpdateAlarmState(bool hasAlarm)
        {
            if (_currentProfile == null)
                return;

            _currentProfile.SetProfile(null, hasAlarm); 
        }

        public void EnableAllProfile()
        {
            _profiles.ForEach(profile =>
            {
                if((int)profile.Guest <= _gameStatueCore.CurrentDay)
                    profile.gameObject.SetActive(true);
                profile.OpenChat(false);
            });
            _currentProfile = null;
        }

        public bool HasAnyUnread(BubbleManager bubbleManager)
        {
            if (_profiles == null) return false;
            
            foreach (var profile in _profiles)
            {
                if(!profile.isActiveAndEnabled)
                    continue;
                
                if (profile.Guest != Guest.None && bubbleManager.IsDialogueUnread(profile.Guest))
                {
                    return true;
                }
            }
            return false;
        }

        public void InitializeProfiles(BubbleManager bubbleManager)
        {
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
                bool isUnread = bubbleManager.IsDialogueUnread(targetGuest);

                profile.SetProfile(lastMsg, isUnread);
                
                if((int)profile.Guest > _gameStatueCore.CurrentDay)
                    profile.gameObject.SetActive(false);
            }
        }
    }
}