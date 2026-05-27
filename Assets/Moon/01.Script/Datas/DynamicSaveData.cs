using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Moon._01.Script.Datas
{
    [Serializable]
    public class DynamicSaveData : ISerializationCallbackReceiver
    {
        private Dictionary<string, int> _intData = new Dictionary<string, int>();
        private Dictionary<string, float> _floatData = new Dictionary<string, float>();
        private Dictionary<string, string> _stringData = new Dictionary<string, string>();
        private Dictionary<string, bool> _boolData = new Dictionary<string, bool>();

        [SerializeField] private List<StringIntPair> _intPairs = new List<StringIntPair>();
        [SerializeField] private List<StringFloatPair> _floatPairs = new List<StringFloatPair>();
        [SerializeField] private List<StringStringPair> _stringPairs = new List<StringStringPair>();
        [SerializeField] private List<StringBoolPair> _boolPairs = new List<StringBoolPair>();

        public void OnBeforeSerialize()
        {
            _intPairs.Clear();
            foreach (var kvp in _intData)
            {
                _intPairs.Add(new StringIntPair { Key = kvp.Key, Value = kvp.Value });
            }

            _floatPairs.Clear();
            foreach (var kvp in _floatData)
            {
                _floatPairs.Add(new StringFloatPair { Key = kvp.Key, Value = kvp.Value });
            }

            _stringPairs.Clear();
            foreach (var kvp in _stringData)
            {
                _stringPairs.Add(new StringStringPair { Key = kvp.Key, Value = kvp.Value });
            }

            _boolPairs.Clear();
            foreach (var kvp in _boolData)
            {
                _boolPairs.Add(new StringBoolPair { Key = kvp.Key, Value = kvp.Value });
            }
        }

        public void OnAfterDeserialize()
        {
            _intData.Clear();
            foreach (var pair in _intPairs)
            {
                _intData[pair.Key] = pair.Value;
            }

            _floatData.Clear();
            foreach (var pair in _floatPairs)
            {
                _floatData[pair.Key] = pair.Value;
            }

            _stringData.Clear();
            foreach (var pair in _stringPairs)
            {
                _stringData[pair.Key] = pair.Value;
            }

            _boolData.Clear();
            foreach (var pair in _boolPairs)
            {
                _boolData[pair.Key] = pair.Value;
            }
        }

        public void Save(string filePath)
        {
            string jsonData = JsonUtility.ToJson(this, true);
            string encryptedData = RealKey.Encrypt(jsonData);
            File.WriteAllText(filePath, encryptedData);
        }

        public bool Load(string filePath)
        {
            if (!File.Exists(filePath)) return false;

            try
            {
                string encryptedData = File.ReadAllText(filePath);
                string jsonData = RealKey.Decrypt(encryptedData);
                
                if (!string.IsNullOrEmpty(jsonData))
                {
                    JsonUtility.FromJsonOverwrite(jsonData, this);
                    return true;
                }
            }
            catch { }

            return false;
        }

        [Serializable] public struct StringIntPair { public string Key; public int Value; }
        [Serializable] public struct StringFloatPair { public string Key; public float Value; }
        [Serializable] public struct StringStringPair { public string Key; public string Value; }
        [Serializable] public struct StringBoolPair { public string Key; public bool Value; }

        public bool TryGetValue(string key, out int value) => _intData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out float value) => _floatData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out string value) => _stringData.TryGetValue(key, out value);
        public bool TryGetValue(string key, out bool value) => _boolData.TryGetValue(key, out value);

        public void SaveData(string key, int value) => _intData[key] = value;
        public void SaveData(string key, float value) => _floatData[key] = value;
        public void SaveData(string key, string value) => _stringData[key] = value;
        public void SaveData(string key, bool value) => _boolData[key] = value;
    }
}