using System;
using System.Collections.Generic;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    [Serializable]
    public class DynamicSaveData : ISerializationCallbackReceiver
    {
        public Dictionary<string, int> IntData = new Dictionary<string, int>();
        public Dictionary<string, float> FloatData = new Dictionary<string, float>();
        public Dictionary<string, string> StringData = new Dictionary<string, string>();
        public Dictionary<string, bool> BoolData = new Dictionary<string, bool>();

        [SerializeField] private List<StringIntPair> _intPairs = new List<StringIntPair>();
        [SerializeField] private List<StringFloatPair> _floatPairs = new List<StringFloatPair>();
        [SerializeField] private List<StringStringPair> _stringPairs = new List<StringStringPair>();
        [SerializeField] private List<StringBoolPair> _boolPairs = new List<StringBoolPair>();

        public void OnBeforeSerialize()
        {
            _intPairs.Clear();
            foreach (var kvp in IntData)
            {
                _intPairs.Add(new StringIntPair { Key = kvp.Key, Value = kvp.Value });
            }

            _floatPairs.Clear();
            foreach (var kvp in FloatData)
            {
                _floatPairs.Add(new StringFloatPair { Key = kvp.Key, Value = kvp.Value });
            }

            _stringPairs.Clear();
            foreach (var kvp in StringData)
            {
                _stringPairs.Add(new StringStringPair { Key = kvp.Key, Value = kvp.Value });
            }

            _boolPairs.Clear();
            foreach (var kvp in BoolData)
            {
                _boolPairs.Add(new StringBoolPair { Key = kvp.Key, Value = kvp.Value });
            }
        }

        public void OnAfterDeserialize()
        {
            IntData.Clear();
            foreach (var pair in _intPairs)
            {
                IntData[pair.Key] = pair.Value;
            }

            FloatData.Clear();
            foreach (var pair in _floatPairs)
            {
                FloatData[pair.Key] = pair.Value;
            }

            StringData.Clear();
            foreach (var pair in _stringPairs)
            {
                StringData[pair.Key] = pair.Value;
            }

            BoolData.Clear();
            foreach (var pair in _boolPairs)
            {
                BoolData[pair.Key] = pair.Value;
            }
        }

        [Serializable]
        public struct StringIntPair
        {
            public string Key;
            public int Value;
        }

        [Serializable]
        public struct StringFloatPair
        {
            public string Key;
            public float Value;
        }

        [Serializable]
        public struct StringStringPair
        {
            public string Key;
            public string Value;
        }

        [Serializable]
        public struct StringBoolPair
        {
            public string Key;
            public bool Value;
        }

        public bool TryGetValue(string key, out int value) => IntData.TryGetValue(key, out value);
        
        public bool TryGetValue(string key, out float value) => FloatData.TryGetValue(key, out value);
        
        public bool TryGetValue(string key, out string value) => StringData.TryGetValue(key, out value);
        
        public bool TryGetValue(string key, out bool value) => BoolData.TryGetValue(key, out value);

        public void SaveData(string key, int value) => IntData[key] = value;
        public void SaveData(string key, float value) => FloatData[key] = value;
        public void SaveData(string key, string value) => StringData[key] = value;
        public void SaveData(string key, bool value) => BoolData[key] = value;
    }
}