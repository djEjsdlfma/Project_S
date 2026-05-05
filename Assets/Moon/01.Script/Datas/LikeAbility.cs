using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using MoonLib.ScriptFinder_Pro.RunTime.DevLogs;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.ListFinder;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    public enum Endings
    {
        A,
        B,
        C
    }
    
    public class LikeAbility : MonoBehaviour
    {
        [field:SerializeField, SerializedDictionary("Ending", "LikeAbility")] private SerializedDictionary<Endings, int> endingLikeAbilityDict;
        
        [SerializeField] private ScriptListFinderSO guestScriptFinder;
        
        private Dictionary<string, int> _likeAbilityDict = new Dictionary<string, int>();
        
        private void Awake()
        {
            string C1 = guestScriptFinder.GetTarget<CurrentGuestManager>().C1;
            string C2 = guestScriptFinder.GetTarget<CurrentGuestManager>().C2;
            string C3 = guestScriptFinder.GetTarget<CurrentGuestManager>().C3;
            
            if(DataManager.Instance.TryGetValue(C1, out int value))
            {
                _likeAbilityDict.Add(C1, value);
            }
            else
            {
                _likeAbilityDict.Add(C1, 0);
            }
            
            if(DataManager.Instance.TryGetValue(C2, out value))
            {
                _likeAbilityDict.Add(C2, value);
            }
            else
            {
                _likeAbilityDict.Add(C2, 0);
            }
            
            if(DataManager.Instance.TryGetValue(C3, out value))
            {
                _likeAbilityDict.Add(C3, value);
            }
            else
            {
                _likeAbilityDict.Add(C3, 0);
            }
        }
        
        private string MostLikeCharacter()
        {
            string character = string.Empty;
            int maxLikeAbility = int.MinValue;

            foreach (var kvp in _likeAbilityDict)
            {
                if (kvp.Value > maxLikeAbility)
                {
                    maxLikeAbility = kvp.Value;
                    character = kvp.Key;
                }
            }

            return character;
        }

        public (string, Endings) Ending()
        {
            string character = MostLikeCharacter();
            Endings ending = Endings.A;

            if (endingLikeAbilityDict.TryGetValue(Endings.A, out int likeAbilityA) && likeAbilityA <= _likeAbilityDict[character])
            {
                ending = Endings.A;
            }
            else if (endingLikeAbilityDict.TryGetValue(Endings.B, out int likeAbilityB) && likeAbilityB <= _likeAbilityDict[character])
            {
                ending = Endings.B;
            }
            else if (endingLikeAbilityDict.TryGetValue(Endings.C, out int likeAbilityC) && likeAbilityC <= _likeAbilityDict[character])
            {
                ending = Endings.C;
            }

            DevLog.Log($"{character} : Ending{ending}");

            return (character, ending);
        }
        
        public void AddLikeAbility(string character, int amount)
        {
            if (_likeAbilityDict.ContainsKey(character))
            {
                _likeAbilityDict[character] += amount;
                DataManager.Instance.SaveData(character, _likeAbilityDict[character]);
            }
        }
    }
}