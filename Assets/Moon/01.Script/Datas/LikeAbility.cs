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
    
    public struct EndingData
    {
        public string Name;
        public Endings Ending;
        
        public EndingData(string name, Endings ending)
        {
            Name = name;
            Ending = ending;
        }
    }
    
    public class LikeAbility : MonoBehaviour
    {
        [field:SerializeField, SerializedDictionary("Ending", "LikeAbility")] private SerializedDictionary<Endings, float> endingLikeAbilityDict;
        
        [SerializeField] private ScriptListFinderSO guestScriptFinder;
        
        private Dictionary<string, float> _likeAbilityDict = new Dictionary<string, float>();
        
        private void Awake()
        {
            string[] C = guestScriptFinder.GetTarget<CurrentGuestManager>().C;

            foreach (var key in C)
            {
                if(DataManager.Instance.TryGetValue(key, out float value))
                {
                    _likeAbilityDict.Add(key, value);
                }
                else
                {
                    _likeAbilityDict.Add(key, 0);
                }
            }
        }
        
        private string MostLikeCharacter()
        {
            string character = string.Empty;
            float maxLikeAbility = float.MinValue;

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

        public List<EndingData> Ending()
        {
            List<EndingData> endingData = new List<EndingData>();
            Endings ending;
            string character;
            foreach (var kvp in _likeAbilityDict)
            {
                character = kvp.Key;
                if (endingLikeAbilityDict.TryGetValue(Endings.A, out float likeAbilityA) && likeAbilityA <= _likeAbilityDict[character])
                {
                    ending = Endings.A;
                }
                else if (endingLikeAbilityDict.TryGetValue(Endings.B, out float likeAbilityB) && likeAbilityB <= _likeAbilityDict[character])
                {
                    ending = Endings.B;
                }
                else
                {
                    ending = Endings.C;
                }
                EndingData data = new EndingData(character, ending);
                endingData.Add(data);
            }

            return endingData;
        }
        
        public void AddLikeAbility(string character, int amount)
        {
            if (!string.IsNullOrEmpty(character) && _likeAbilityDict.ContainsKey(character))
            {
                _likeAbilityDict[character] += amount;
                DataManager.Instance.SaveData(character, _likeAbilityDict[character]);
            }
        }
    }
}