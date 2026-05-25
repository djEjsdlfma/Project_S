
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
        private List<GuestProfile> _profiles;

        private GuestProfile _currentProfile;

        private void Awake()
        {
            _profiles = GetComponentsInChildren<GuestProfile>().ToList();
        }

        public void DisableAllProfile(GuestProfile excludedProfile)
        {
            _profiles.ForEach(profile => profile.gameObject.SetActive(false));

            if (excludedProfile != null)
            {
                excludedProfile.gameObject.SetActive(true);
                excludedProfile.transform.SetAsFirstSibling();
            }

            _currentProfile = excludedProfile;
        }

        public void ChangeCurrentProfile(string lastMessage, bool hasAlarm = false)
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

        public void SetProfileActive(Guest guest, bool active)
        {
            var profile = _profiles.Find(p => p.Guest == guest);
            if (profile != null)
            {
                profile.gameObject.SetActive(active);
            }
        }

    public void EnableAllProfile()
    {
        var bubbleManager = SystemManager.Instance.GetSystemManager<BubbleManager>();
        var gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
        var dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();
        
        _profiles.ForEach(profile =>
        {
            if (profile.Guest != Guest.None)
            {
                int currentDay = gameStatueCore != null ? gameStatueCore.CurrentDay : 1;
                int firstDay = dialogueDataCore != null ? dialogueDataCore.GetFirstDialogueDay(profile.Guest) : int.MaxValue;
                
                profile.gameObject.SetActive(currentDay >= firstDay);
            }
            else
            {
                profile.gameObject.SetActive(true);
            }
        });
        _currentProfile = null;
    }

        public bool HasAnyUnread(BubbleManager bubbleManager)
        {
            if (_profiles == null) return false;
            
            foreach (var profile in _profiles)
            {
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

            var gameStatueCore = CoreHandler.Instance.GetCore<GameStatueCore>();
            var dialogueDataCore = CoreHandler.Instance.GetCore<DialogueDataCore>();

            foreach (var profile in _profiles)
            {
                Guest targetGuest = profile.Guest;
                if (targetGuest == Guest.None)
                {
                    var btn = profile.GetComponentInChildren<GuestProfileSelectBtn>();
                    if (btn != null)
                    {
                        profile.Guest = btn.Guest;
                        targetGuest = btn.Guest;
                    }
                }

                if (targetGuest == Guest.None) continue;
                
                string lastMsg = bubbleManager.GetLastDialogueContent(targetGuest);
                bool isUnread = bubbleManager.IsDialogueUnread(targetGuest);
                
                int currentDay = gameStatueCore != null ? gameStatueCore.CurrentDay : 1;
                int firstDay = dialogueDataCore != null ? dialogueDataCore.GetFirstDialogueDay(targetGuest) : int.MaxValue;
                
                profile.SetProfile(lastMsg, isUnread);
                profile.gameObject.SetActive(currentDay >= firstDay);
            }
        }
    }
}